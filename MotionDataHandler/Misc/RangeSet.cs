using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionDataHandler.Misc {
    /// <summary>
    /// 数直線上の範囲の集合を保持するクラス
    /// </summary>
    /// <typeparam name="T">数値</typeparam>
    public class RangeSet<T> : ICollection<RangeSet<T>.Range> where T : IComparable<T> {
        /// <summary>
        /// 範囲を保持するクラス
        /// </summary>
        public struct Range {
            private T _start, _end;
            /// <summary>
            /// 既定のコンストラクタ
            /// </summary>
            /// <param name="start">範囲の開始の値</param>
            /// <param name="end">範囲の終了の値</param>
            public Range(T start, T end) {
                _start = start;
                _end = end;
                FixOrder();
            }
            /// <summary>
            /// コピーコンストラクタ
            /// </summary>
            /// <param name="range">元となる範囲</param>
            public Range(Range range)
                : this(range.Start, range.End) {
            }
            /// <summary>
            /// 範囲の開始位置を取得または設定します．
            /// </summary>
            public T Start { get { return _start; } set { _start = value; this.FixOrder(); } }
            /// <summary>
            /// 範囲の終了位置を取得または設定します．
            /// </summary>
            public T End { get { return _end; } set { _end = value; this.FixOrder(); } }
            /// <summary>
            /// Endの値がStartの値より小さい場合に，Startの値とEndの値を入れ替えます．
            /// </summary>
            public void FixOrder() {
                if(_start.CompareTo(_end) > 0) {
                    T tmp = _end;
                    _end = _start;
                    _start = tmp;
                }
            }
            /// <summary>
            /// 範囲が他の範囲と重複部分を持つかを返します．
            /// </summary>
            /// <param name="range">他の範囲</param>
            /// <returns></returns>
            public bool Overlaps(Range range) {
                return this.Start.CompareTo(range.End) < 0 && range.Start.CompareTo(this.End) < 0;
            }
            /// <summary>
            /// 範囲が他の範囲と重複せずに隣接しているかを返します．
            /// </summary>
            /// <param name="range"></param>
            /// <returns></returns>
            public bool Touches(Range range) {
                return range.Start.CompareTo(this.End) == 0 || range.End.CompareTo(this.Start) == 0;
            }

            /// <summary>
            /// 範囲が他の範囲と重複または隣接するかを返します．
            /// </summary>
            /// <param name="range"></param>
            /// <returns></returns>
            public bool Intersects(Range range) {
                return this.Start.CompareTo(range.End) <= 0 && range.Start.CompareTo(this.End) <= 0;
            }

            /// <summary>
            /// 他の範囲との重複部分を返します．重複部分がない場合には大きさが0の範囲を返します
            /// </summary>
            /// <param name="range"></param>
            /// <returns></returns>
            public Range GetOverlap(Range range) {
                if(this.Overlaps(range)) {
                    // 大きいほうのStartから小さいほうのEndまで
                    T start = this.Start;
                    if(start.CompareTo(range.Start) < 0)
                        start = range.Start;
                    T end = this.End;
                    if(end.CompareTo(range.End) > 0)
                        end = range.End;
                    return new Range(start, end);
                }
                return new Range(this.Start, this.Start);
            }
            /// <summary>
            /// 範囲が他の範囲を内包しているかを返します．
            /// </summary>
            /// <param name="range"></param>
            /// <returns></returns>
            public bool Contains(Range range) {
                return this.Start.CompareTo(range.Start) <= 0 && this.End.CompareTo(range.End) >= 0;
            }
            /// <summary>
            /// 範囲の大きさが0であるかを取得します．
            /// </summary>
            public bool IsEmpty { get { return this.Start.CompareTo(this.End) == 0; } }

            public static bool operator ==(Range a, Range b) {
                return a.Start.CompareTo(b.Start) == 0 && a.End.CompareTo(b.End) == 0;
            }
            public static bool operator !=(Range a, Range b) {
                return a.Start.CompareTo(b.Start) != 0 || a.End.CompareTo(b.End) != 0;
            }
            public override bool Equals(object obj) {
                if(obj is Range) {
                    Range b = (Range)obj;
                    return this.Start.CompareTo(b.Start) == 0 && this.End.CompareTo(b.End) == 0;
                }
                return base.Equals(obj);
            }
            public override int GetHashCode() {
                return this.Start.GetHashCode() ^ this.End.GetHashCode();
            }
        }

        readonly List<Range> _rangeList = new List<Range>();

        private class RangeEndComparer : IComparer<Range> {
            #region IComparer<Range> メンバ

            public int Compare(Range x, Range y) {
                return x.End.CompareTo(y.End);
            }

            #endregion
        }

        private readonly RangeEndComparer _rangeEndComparer = new RangeEndComparer();

        public RangeSet() { }
        public RangeSet(IEnumerable<Range> ranges) {
            this.Add(ranges);
        }
        /// <summary>
        /// 内部で保持する範囲が後から変えられた状況等を修復します．
        /// </summary>
        public void FixCorruption() {
            List<Range> tmp = new List<RangeSet<T>.Range>(_rangeList);
            this.Clear();
            foreach(var range in tmp) {
                range.FixOrder();
                this.Add(range);
            }
        }

        protected static T max(T t1, T t2) {
            if(t1.CompareTo(t2) < 0)
                return t2;
            return t1;
        }

        protected static T min(T t1, T t2) {
            if(t1.CompareTo(t2) > 0)
                return t2;
            return t1;
        }

        public void Add(T start, T end) {
            this.Add(new RangeSet<T>.Range(start, end));
        }

        #region ICollection<Range> メンバ
        /// <summary>
        /// 範囲を集合に追加します
        /// </summary>
        /// <param name="item">範囲</param>
        /// <remarks>
        /// 内部の実装的にはこのような感じ
        ///   ##### #### #### ### ###
        /// +         ########
        /// -------------------------
        ///   #####               ###
        /// +       #############
        /// -------------------------
        ///   ##### ############# ###
        /// </remarks>
        public virtual void Add(RangeSet<T>.Range item) {
            // 範囲の終了位置が引数の範囲の開始位置以降であるような最初の範囲を探す
            Range start = new Range(item.Start, item.Start);
            int index = _rangeList.BinarySearch(start, _rangeEndComparer);
            if(index < 0) {
                index = ~index;
            } else {
                // 接触している場合
            }
            T newStart = item.Start;
            T newEnd = item.End;
            // 引数の範囲とかぶっている部分を除く
            while(index < _rangeList.Count && _rangeList[index].Start.CompareTo(item.End) <= 0) {
                newStart = min(newStart, _rangeList[index].Start);
                newEnd = max(newEnd, _rangeList[index].End);
                _rangeList.RemoveAt(index);
            }
            // 除かれた範囲と引数の範囲の和を追加
            Range newRange = new RangeSet<T>.Range(newStart, newEnd);
            if(!newRange.IsEmpty) {
                _rangeList.Insert(index, newRange);
            }
        }

        public void Add(IEnumerable<Range> ranges) {
            foreach(Range range in ranges) {
                this.Add(range);
            }
        }

        public void Remove(IEnumerable<Range> ranges) {
            foreach(Range range in ranges) {
                this.Remove(range);
            }
        }

        public void Clear() {
            _rangeList.Clear();
        }

        /// <summary>
        /// 指定された範囲を含むかを返します．
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public bool Contains(RangeSet<T>.Range item) {
            Range start = new Range(item.Start, item.Start);
            int index = _rangeList.BinarySearch(start, _rangeEndComparer);
            if(index < 0) {
                index = ~index;
            }
            if(index >= _rangeList.Count)
                return false;
            Range candidate = _rangeList[index];
            // 隣接しているのがあれば結合する
            for(index++; index < _rangeList.Count && candidate.Touches(_rangeList[index]); index++) {
                candidate = new RangeSet<T>.Range(candidate.Start, _rangeList[index].End);
            }
            return candidate.Contains(item);
        }

        public void CopyTo(RangeSet<T>.Range[] array, int arrayIndex) {
            if(array == null)
                throw new ArgumentNullException("array", "'array' cannot be null");
            if(arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex", "'arrayIndex' cannot be negative");
            if(array.Rank > 1)
                throw new ArgumentException("'array' must be 1-dimensional array", "array");
            if(array.Length < arrayIndex + _rangeList.Count)
                throw new ArgumentException("'array' must have sufficient length", "array");
            foreach(Range range in _rangeList) {
                array[arrayIndex] = range;
                arrayIndex++;
            }
        }

        /// <summary>
        /// 範囲の分割数を取得します．
        /// </summary>
        public int Count {
            get { return _rangeList.Count; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public bool Remove(RangeSet<T>.Range item) {
            // 範囲の終了位置が引数の範囲の開始位置以降であるような最初の範囲を探す
            Range start = new Range(item.Start, item.Start);
            int index = _rangeList.BinarySearch(start, _rangeEndComparer);
            if(index < 0) {
                index = ~index;
            } else {
                // 接触している場合
            }
            T newStart = item.Start;
            T newEnd = item.End;
            // 引数の範囲とかぶっている範囲を取り除く
            bool overlaps = false;
            while(index < _rangeList.Count && _rangeList[index].Start.CompareTo(item.End) <= 0) {
                newStart = min(newStart, _rangeList[index].Start);
                newEnd = max(newEnd, _rangeList[index].End);
                _rangeList.RemoveAt(index);
                overlaps = true;
            }
            if(overlaps) {
                if(item.IsEmpty) {
                    // 引数の範囲が空だった場合には，消した物を元に戻す
                    _rangeList.Insert(index, new RangeSet<T>.Range(newStart, newEnd));
                } else {
                    // 引数の範囲によって部分的に覆われている範囲は，覆われなかった部分を戻す
                    if(newStart.CompareTo(item.Start) < 0) {
                        _rangeList.Insert(index, new RangeSet<T>.Range(newStart, item.Start));
                        index++;
                    }
                    if(item.End.CompareTo(newEnd) < 0) {
                        _rangeList.Insert(index, new RangeSet<T>.Range(item.End, newEnd));
                    }
                }
            }
            return overlaps;
        }



        #endregion
        /// <summary>
        /// 範囲の集合のうち，指定された引数の部分を切り出します．
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public RangeSet<T> GetClipped(Range range) {
            // 範囲の終了位置が引数の範囲の開始位置以降であるような最初の範囲を探す
            Range start = new Range(range.Start, range.Start);
            int index = _rangeList.BinarySearch(start, _rangeEndComparer);
            if(index < 0) {
                index = ~index;
            } else {
                // 接触している場合は重複がないので無視
                index++;
            }
            // 被っている各範囲との重複部分を求める
            RangeSet<T> ret = new RangeSet<T>();
            for(; index < _rangeList.Count && _rangeList[index].Start.CompareTo(range.End) < 0; index++) {
                ret.Add(range.GetOverlap(_rangeList[index]));
            }
            return ret;
        }

        #region IEnumerable<Range> メンバ

        public IEnumerator<RangeSet<T>.Range> GetEnumerator() {
            return _rangeList.GetEnumerator();
        }

        #endregion

        #region IEnumerable メンバ

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _rangeList.GetEnumerator();
        }

        #endregion

        public RangeSet<T> GetUnion(RangeSet<T> rangeSet) {
            RangeSet<T> ret = new RangeSet<T>(this);
            ret.Add(rangeSet);
            return ret;
        }
        public RangeSet<T> GetExcept(RangeSet<T> rangeSet) {
            RangeSet<T> ret = new RangeSet<T>(this);
            ret.Remove(rangeSet);
            return ret;
        }
        public RangeSet<T> GetIntersect(RangeSet<T> rangeSet) {
            RangeSet<T> ret = new RangeSet<T>();
            if(this.Count < rangeSet.Count) {
                foreach(var range in this) {
                    ret.Add(rangeSet.GetClipped(range));
                }
            } else {
                foreach(var range in rangeSet) {
                    ret.Add(this.GetClipped(range));
                }
            }
            return ret;
        }
    }
    public static class RangeSetExtension {
        public static int Total(this RangeSet<int> rangeSet) {
            return rangeSet.Sum(range => range.End - range.Start);
        }
    }
}
