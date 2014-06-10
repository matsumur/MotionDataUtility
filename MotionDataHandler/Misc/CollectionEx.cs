using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MotionDataHandler.Misc {
    public static class CollectionEx {

        public static string Join<T>(char separator, IEnumerable<T> values) {
            return Join(separator.ToString(), values);
        }
        public static string Join<T>(string separator, IEnumerable<T> values) {
            //return values.Any() ? values.Select(v => v.ToString()).Aggregate((a, b) => a + separator + b) : "";
            StringBuilder ret = new StringBuilder();
            bool first = true;
            foreach(T value in values) {
                if(first) {
                    first = false;
                } else {
                    ret.Append(separator);
                }
                ret.Append(value.ToString());
            }
            return ret.ToString();
        }

        /// <summary>
        /// SortedListのキーに対しkeyの値以下の最大のキーを二分探索して返します。
        /// </summary>
        /// <typeparam title="TKey">SortedListのTKey</typeparam>
        /// <typeparam title="TValue">SortedListのTValue</typeparam>
        /// <param title="sortedList">探索されるSortedList</param>
        /// <param title="key">探索に用いる値</param>
        /// <param title="firstIndex">探索の開始インデックス</param>
        /// <param title="firstOffset">開始探索幅</param>
        /// <returns>キーのインデックス。見つからなければ-1</returns>
        public static int GetLastIndexBeforeKey<TKey, TValue>(SortedList<TKey, TValue> sortedList, TKey searchKey, int firstIndex, int firstOffset) where TKey : IComparable<TKey> {
            IList<TKey> keys = sortedList.Keys;
            if(keys.Count == 0)
                return -1;
            if(firstIndex < 0)
                firstIndex = 0;
            if(firstIndex >= keys.Count)
                firstIndex = keys.Count - 1;
            if(firstOffset >= keys.Count)
                firstOffset = keys.Count - 1;
            if(firstOffset < 1)
                firstOffset = 1;
            int offset = firstOffset;
            int index = firstIndex;
            // // to achieve
            // var before = from keu in sortedList.Keys where key <= searchKey select key;
            // int index = before.Count() == 0 ? -1 : sortedList.Keys.IndexOf(before.Last());
            // //
            int cmp = keys[index].CompareTo(searchKey);
            if(cmp == 0) {
                // do nothing
            } else if(cmp < 0) {
                // indexを前進させながら探索
                // searchKey以下の最後のキーのインデックスを取得
                while(index + offset < keys.Count && keys[index + offset].CompareTo(searchKey) <= 0) {
                    index += offset;
                    offset *= 2;
                }
                offset /= 2;
                while(offset > 0) {
                    while(index + offset < keys.Count && keys[index + offset].CompareTo(searchKey) <= 0) {
                        index += offset;
                    }
                    offset /= 2;
                }
            } else {
                // indexを後退させながら探索
                // searchKeyより大きな最初のキーのインデックスを取得
                while(index - offset >= 0 && keys[index - offset].CompareTo(searchKey) > 0) {
                    index -= offset;
                    offset *= 2;
                }
                offset /= 2;
                while(offset > 0) {
                    while(index - offset >= 0 && keys[index - offset].CompareTo(searchKey) > 0) {
                        index -= offset;
                    }
                    offset /= 2;
                }
                Debug.Assert(keys[index - offset].CompareTo(searchKey) > 0);
                index--; // 注)indexは-1にもなる: searchKeyがsortedListの最初のキーより小さい場合
            }
            return index;
        }
    }
}
