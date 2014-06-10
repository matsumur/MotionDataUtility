using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MotionDataHandler.Misc {
    /// <summary>
    /// 両方向ディクショナリクラス
    /// </summary>
    /// <typeparam name="TKey">主にキーとして働くクラス</typeparam>
    /// <typeparam name="TValue">主に値として働くクラス</typeparam>
    public class BijectiveDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
        readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private Dictionary<TKey, TValue> _forward;
        private Dictionary<TValue, TKey> _backward;

        public BijectiveDictionary() {
            _forward = new Dictionary<TKey, TValue>();
            _backward = new Dictionary<TValue, TKey>();
        }
        public BijectiveDictionary(IEqualityComparer<TKey> compareKey, IEqualityComparer<TValue> compareValue) {
            _forward = new Dictionary<TKey, TValue>(compareKey);
            _backward = new Dictionary<TValue, TKey>(compareValue);
        }
        public BijectiveDictionary(IDictionary<TKey, TValue> dictionary) {
            _forward = new Dictionary<TKey, TValue>(dictionary);
            _backward = dictionary.ToDictionary(d => d.Value, d => d.Key);
        }
        public BijectiveDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> compareKey, IEqualityComparer<TValue> compareValue) {
            _forward = new Dictionary<TKey, TValue>(dictionary, compareKey);
            _backward = new Dictionary<TValue, TKey>(compareValue);
            foreach(var pair in _forward) {
                _backward.Add(pair.Value, pair.Key);
            }
        }

        public bool ContainsValue(TValue value) {
            _rwLock.EnterReadLock();
            try {
                return _backward.ContainsKey(value);
            } finally { _rwLock.ExitReadLock(); }
        }

        public bool RemoveByValue(TValue value) {
            _rwLock.EnterWriteLock();
            try {
                TKey k;
                if(_backward.TryGetValue(value, out k)) {
                    _forward.Remove(k);
                    _backward.Remove(value);
                    return true;
                }
                return false;
            } finally { _rwLock.ExitWriteLock(); }
        }

        public bool TryGetKey(TValue value, out TKey key) {
            _rwLock.EnterReadLock();
            try {
                return _backward.TryGetValue(value, out key);
            } finally { _rwLock.ExitReadLock(); }
        }

        #region IDictionary<TKey,TValue> メンバ

        public void Add(TKey key, TValue value) {
            _rwLock.EnterWriteLock();
            try {
                // すでにあるなら例外を起こす
                if(_forward.ContainsKey(key))
                    _forward.Add(key, value);
                if(_backward.ContainsKey(value))
                    _backward.Add(value, key);

                _forward.Add(key, value);
                _backward.Add(value, key);
            } finally { _rwLock.ExitWriteLock(); }
        }

        public bool ContainsKey(TKey key) {
            _rwLock.EnterReadLock();
            try {
                return _forward.ContainsKey(key);
            } finally { _rwLock.ExitWriteLock(); }
        }

        public ICollection<TKey> Keys {
            get {
                _rwLock.EnterReadLock();
                try {
                    return _forward.Keys;
                } finally { _rwLock.ExitWriteLock(); }
            }
        }

        public bool Remove(TKey key) {
            _rwLock.EnterWriteLock();
            try {
                TValue v;
                if(_forward.TryGetValue(key, out v)) {
                    _forward.Remove(key);
                    _backward.Remove(v);
                    return true;
                }
                return false;
            } finally { _rwLock.ExitWriteLock(); }
        }

        public bool TryGetValue(TKey key, out TValue value) {
            _rwLock.EnterReadLock();
            try {
                return _forward.TryGetValue(key, out value);
            } finally { _rwLock.ExitReadLock(); }
        }

        public ICollection<TValue> Values {
            get {
                _rwLock.EnterReadLock();
                try {
                    return _backward.Keys;
                } finally { _rwLock.ExitReadLock(); }
            }
        }

        public TValue this[TKey key] {
            get {
                _rwLock.EnterReadLock();
                try {
                    return _forward[key];
                } finally { _rwLock.ExitReadLock(); }

            }
            set {
                _rwLock.EnterWriteLock();
                try {
                    this.Remove(key);
                    this.RemoveByValue(value);
                    this.Add(key, value);
                } finally { _rwLock.ExitWriteLock(); }
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> メンバ

        public void Add(KeyValuePair<TKey, TValue> item) {
            _rwLock.EnterWriteLock();
            try {
                this.Add(item.Key, item.Value);
            } finally { _rwLock.ExitWriteLock(); }
        }

        public void Clear() {
            _rwLock.EnterWriteLock();
            try {
                _forward.Clear();
                _backward.Clear();
            } finally { _rwLock.ExitWriteLock(); }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) {
            _rwLock.EnterReadLock();
            try {
                TValue v;
                if(_forward.TryGetValue(item.Key, out v)) {
                    if(_backward.Comparer.Equals(item.Value, v)) {
                        return true;
                    }
                }
                return false;
            } finally { _rwLock.ExitReadLock(); }
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            _rwLock.EnterReadLock();
            try {
                foreach(var pair in _forward) {
                    array[arrayIndex++] = pair;
                }
            } finally { _rwLock.ExitReadLock(); }
        }

        public int Count {
            get {
                _rwLock.EnterReadLock();
                try {
                    return _forward.Count;
                } finally { _rwLock.ExitReadLock(); }
            }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) {
            _rwLock.EnterWriteLock();
            try {
                TValue v;
                if(_forward.TryGetValue(item.Key, out v)) {
                    if(_backward.Comparer.Equals(item.Value, v)) {
                        return this.Remove(item.Key);
                    }
                }
                return false;
            } finally { _rwLock.ExitWriteLock(); }
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> メンバ

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            _rwLock.EnterReadLock();
            try {
                foreach(var pair in _forward) {
                    yield return pair;
                }
            } finally { _rwLock.ExitReadLock(); }
        }

        #endregion

        #region IEnumerable メンバ

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            _rwLock.EnterReadLock();
            try {
                foreach(var pair in _forward) {
                    yield return pair;
                }
            } finally { _rwLock.ExitReadLock(); }
        }

        #endregion
    }
}
