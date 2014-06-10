using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MotionDataHandler.Misc {
    /// <summary>
    /// ReaderWriterLockSlimのEnter/ExitのペアをコンストラクタとDisposeにて自動的に行うためのクラス
    /// </summary>
    /// <example>
    /// class Example {
    ///     private ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
    ///     private LockDisposable _locker;
    ///     private int[] _value1;
    ///     public Example() {
    ///         _locker = new LockDisposable(_rwLock);
    ///     }
    ///     public int[] GetValue1() {
    ///         using(_locker.GetReadLock()) {
    ///             return _value1;
    ///         }
    ///     }
    ///     public void SetValue1(int[] values) {
    ///         using(_locker.GetWriteLock()) {
    ///             _value1 = new int[values.Length];
    ///             for(int i = 0; i &lt; _value1.Length; i++) {
    ///                 _value1[i] = values[i];
    ///             }
    ///         }
    ///     }
    /// }
    /// </example>
    public class LockDisposable {
        private readonly ReaderWriterLockSlim _rwLock;
        private readonly List<LockDisposable> _subLocks = new List<LockDisposable>();
        public Collection<LockDisposable> SubLocks { get { return new Collection<LockDisposable>(_subLocks); } }
        public LockDisposable(ReaderWriterLockSlim rwLock) {
            if(rwLock == null)
                throw new ArgumentNullException("rwLock", "'rwLock' cannot be null");
            _rwLock = rwLock;
        }

        public IDisposable GetReadLock() {
            if(_subLocks.Count > 0)
                return this.GetReadLock(_subLocks);
            return new DisposableReadLock(_rwLock);
        }
        public IDisposable GetWriteLock() {
            if(_subLocks.Count > 0)
                return this.GetWriteLock(_subLocks);
            return new DisposableWriteLock(_rwLock);
        }
        public IDisposable GetUpgradeableReadLock() {
            if(_subLocks.Count > 0)
                return this.GetUpgradeableReadLock(_subLocks);
            return new DisposableUpgradeableReadLock(_rwLock);
        }
        public IDisposable GetReadLock(Action onBeforeReleaseLock) {
            if(onBeforeReleaseLock == null)
                return new DisposableReadLock(_rwLock);
            return new DisposableReadLockCleanup(_rwLock, onBeforeReleaseLock);
        }
        public IDisposable GetWriteLock(Action onBeforeReleaseLock) {
            if(onBeforeReleaseLock == null)
                return new DisposableWriteLock(_rwLock);
            return new DisposableWriteLockCleanup(_rwLock, onBeforeReleaseLock);
        }
        public IDisposable GetUpgradeableReadLock(Action onBeforeReleaseLock) {
            if(onBeforeReleaseLock == null)
                return new DisposableUpgradeableReadLock(_rwLock);
            return new DisposableUpgradeableReadLockCleanup(_rwLock, onBeforeReleaseLock);
        }

        public IDisposable GetReadLock(IList<LockDisposable> subLocks) {
            List<IDisposable> disposables = new List<IDisposable>();
            IDisposable ret = new DisposableReadLockCleanup(_rwLock, () => disposables.ForEach(d => d.Dispose()));
            foreach(LockDisposable subLock in subLocks) {
                disposables.Add(subLock.GetReadLock());
            }
            return ret;
        }
        public IDisposable GetWriteLock(IList<LockDisposable> subLocks) {
            List<IDisposable> disposables = new List<IDisposable>();
            IDisposable ret = new DisposableWriteLockCleanup(_rwLock, () => disposables.ForEach(d => d.Dispose()));
            foreach(LockDisposable subLock in subLocks) {
                disposables.Add(subLock.GetWriteLock());
            }
            return ret;
        }
        public IDisposable GetUpgradeableReadLock(IList<LockDisposable> subLocks) {
            List<IDisposable> disposables = new List<IDisposable>();
            IDisposable ret = new DisposableUpgradeableReadLockCleanup(_rwLock, () => disposables.ForEach(d => d.Dispose()));
            foreach(LockDisposable subLock in subLocks) {
                disposables.Add(subLock.GetUpgradeableReadLock());
            }
            return ret;
        }
    }


    class DisposableReadLock : IDisposable {
        ReaderWriterLockSlim _rwLock;
        bool _isDisposed;

        public DisposableReadLock(ReaderWriterLockSlim rwLock) {
            _rwLock = rwLock;
            _rwLock.EnterReadLock();
        }

        #region IDisposable メンバ

        public void Dispose() {
            if(!_isDisposed) {
                _rwLock.ExitReadLock();
                _isDisposed = true;
            }
        }

        #endregion
    }

    class DisposableWriteLock : IDisposable {
        ReaderWriterLockSlim _rwLock;
        bool _isDisposed;

        public DisposableWriteLock(ReaderWriterLockSlim rwLock) {
            _rwLock = rwLock;
            _rwLock.EnterWriteLock();
        }

        #region IDisposable メンバ

        public void Dispose() {
            if(!_isDisposed) {
                _rwLock.ExitWriteLock();
                _isDisposed = true;
            }
        }

        #endregion
    }

    class DisposableUpgradeableReadLock : IDisposable {
        ReaderWriterLockSlim _rwLock;
        bool _isDisposed;

        public DisposableUpgradeableReadLock(ReaderWriterLockSlim rwLock) {
            _rwLock = rwLock;
            _rwLock.EnterUpgradeableReadLock();
        }

        #region IDisposable メンバ

        public void Dispose() {
            if(!_isDisposed) {
                _rwLock.ExitUpgradeableReadLock();
                _isDisposed = true;
            }
        }

        #endregion
    }

    class DisposableReadLockCleanup : IDisposable {
        ReaderWriterLockSlim _rwLock;
        Action _onReleaseLock;
        bool _isDisposed;

        public DisposableReadLockCleanup(ReaderWriterLockSlim rwLock, Action onReleaseLock) {
            _onReleaseLock = onReleaseLock;
            _rwLock = rwLock;
            _rwLock.EnterReadLock();
        }

        #region IDisposable メンバ

        public void Dispose() {
            if(!_isDisposed) {
                _onReleaseLock();
                _rwLock.ExitReadLock();
                _isDisposed = true;
            }
        }

        #endregion
    }

    class DisposableWriteLockCleanup : IDisposable {
        ReaderWriterLockSlim _rwLock;
        Action _onReleaseLock;
        bool _isDisposed;

        public DisposableWriteLockCleanup(ReaderWriterLockSlim rwLock, Action onReleaseLock) {
            _onReleaseLock = onReleaseLock;
            _rwLock = rwLock;
            _rwLock.EnterWriteLock();
        }

        #region IDisposable メンバ

        public void Dispose() {
            if(!_isDisposed) {
                _onReleaseLock();
                _rwLock.ExitWriteLock();
                _isDisposed = true;
            }
        }

        #endregion
    }

    class DisposableUpgradeableReadLockCleanup : IDisposable {
        ReaderWriterLockSlim _rwLock;
        Action _onReleaseLock;
        bool _isDisposed;

        public DisposableUpgradeableReadLockCleanup(ReaderWriterLockSlim rwLock, Action onReleaseLock) {
            _onReleaseLock = onReleaseLock;
            _rwLock = rwLock;
            _rwLock.EnterUpgradeableReadLock();
        }

        #region IDisposable メンバ

        public void Dispose() {
            if(!_isDisposed) {
                _onReleaseLock();
                _rwLock.ExitUpgradeableReadLock();
                _isDisposed = true;
            }
        }

        #endregion
    }

}
