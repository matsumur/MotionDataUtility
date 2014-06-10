using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionDataHandler.Misc {
    /// <summary>
    /// 弱参照オブジェクトのジェネリックコレクション
    /// </summary>
    /// <typeparam name="T">任意のクラス</typeparam>
    public class WeakCollection<T> : ICollection<T> where T : class {
        /// <summary>
        /// 弱参照のリスト
        /// </summary>
        private List<WeakReference> _weakRefs = new List<WeakReference>();
        /// <summary>
        /// 弱参照のリストへのアクセスを排他制御するためのロックオブジェクト
        /// </summary>
        private readonly object _lock = new object();
        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        public WeakCollection() { }

        #region IEnumerable<T> メンバ

        /// <summary>
        /// ジェネリック型の反復処理をする列挙子を返します。
        /// </summary>
        /// <returns>列挙子</returns>
        public IEnumerator<T> GetEnumerator() {
            lock(_lock) {
                List<WeakReference> alive = new List<WeakReference>();
                foreach(var weakRef in _weakRefs) {
                    T item = weakRef.Target as T;
                    if(weakRef.IsAlive) {
                        yield return item;
                        alive.Add(new WeakReference(item));
                    }
                }
                _weakRefs = alive;
            }
        }

        #endregion

        #region IEnumerable メンバ

        /// <summary>
        /// 反復処理をする列挙子を返します。
        /// </summary>
        /// <returns>列挙子</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        #endregion

        #region ICollection<T> メンバ

        /// <summary>
        /// コレクションにオブジェクトを追加します。
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item) {
            lock(_lock) {
                if(!this.Contains(item)) {
                    _weakRefs.Add(new WeakReference(item));
                }
            }
        }

        /// <summary>
        /// コレクションからすべての要素を削除します。
        /// </summary>
        public void Clear() {
            lock(_lock) {
                _weakRefs.Clear();
            }
        }

        /// <summary>
        /// ある要素がコレクション内に存在するかを判断します。
        /// </summary>
        /// <param name="item">検索するオブジェクト</param>
        /// <returns></returns>
        public bool Contains(T item) {
            lock(_lock) {
                return this.Any(i => i == item);
            }
        }

        /// <summary>
        /// すべての要素を互換性のある1次元配列にコピーします。
        /// </summary>
        /// <param name="array">要素がコピーされる1次元の配列</param>
        /// <param name="arrayIndex">コピーの開始位置</param>
        public void CopyTo(T[] array, int arrayIndex) {
            lock(_lock) {
                foreach(var item in this) {
                    array[arrayIndex++] = item;
                }
            }
        }

        /// <summary>
        /// コレクションに実際に格納されている要素の数を取得します。
        /// コレクションの要素は消失する可能性があるため、一度通常のリストにコピーしてから
        /// 数を確認することが推奨されます。
        /// </summary>
        public int Count {
            get { return _weakRefs.Count; }
        }

        /// <summary>
        /// コレクションが読み取り専用かどうかを示す値を取得します。
        /// </summary>
        public bool IsReadOnly {
            get { return false; }
        }

        /// <summary>
        /// コレクション内で最初に見つかった特定のオブジェクトを削除します。
        /// </summary>
        /// <param name="item">削除するオブジェクト。参照型の場合、null の値を使用できます。</param>
        /// <returns>item が正常に削除された場合は true。それ以外の場合は false。
        /// このメソッドは、item がコレクションに見つからなかった場合にも false を返します。</returns>
        public bool Remove(T item) {
            lock(_lock) {
                var found = _weakRefs.Find(i => i.Target == item);
                if(found != null) {
                    _weakRefs.Remove(found);
                }
                return found != null;
            }
        }

        #endregion
    }
}
