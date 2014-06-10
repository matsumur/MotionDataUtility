using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
namespace MotionDataHandler.Sequence {
    using Misc;

    /// <summary>
    /// 時系列データのある区間での値を保持する構造体．TimeSeriesValuesからのデータ出力用
    /// </summary>
    public struct TimeSeriesRowValue {
        /// <summary>
        /// 区間の開始時間
        /// </summary>
        public decimal BeginTime;
        /// <summary>
        /// 区間の終了時間
        /// </summary>
        public decimal EndTime;
        /// <summary>
        /// この区間での各列の値
        /// </summary>
        public decimal?[] Values;
        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="values"></param>
        public TimeSeriesRowValue(decimal beginTime, decimal endTime, decimal?[] values) {
            this.BeginTime = Math.Min(beginTime, endTime);
            this.EndTime = Math.Max(beginTime, endTime);
            this.Values = values.ToArray();
        }
        /// <summary>
        /// 区間の長さを返します
        /// </summary>
        /// <returns></returns>
        public decimal Duration() {
            return this.EndTime - this.BeginTime;
        }
    }

    /// SequenceData用の時系列データ．
    public class TimeSeriesValues {
        /// <summary>
        /// 共有・排他ロック
        /// </summary>
        readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        /// <summary>
        /// 列の名前．列数は固定なのでreadonly
        /// </summary>
        private readonly string[] _columnNames;

        /// <summary>
        /// 列の名前のリストを取得します
        /// </summary>
        public string[] ColumnNames {
            get { return _columnNames; }
        }

        /// <summary>
        /// 列の名前を一度に設定します．
        /// </summary>
        /// <param name="names">列名の列挙</param>
        public void SetColumnNames(IEnumerable<string> names) {
            int count = 0;
            foreach(string name in names) {
                if(count >= this.ColumnCount)
                    break;
                _columnNames[count] = name;
                count++;
            }
        }

        /// <summary>
        /// 列の名前を一度に設定します．
        /// </summary>
        /// <param name="names">列名の配列</param>
        public void SetColumnNames(params string[] names) {
            if(names.Length < this.ColumnCount) {
                names.CopyTo(_columnNames, 0);
            } else {
                for(int i = 0; i < _columnNames.Length; i++) {
                    _columnNames[i] = names[i];
                }
            }
        }
        /// <summary>
        /// 時間/値のリスト
        /// </summary>
        SortedList<decimal, decimal?[]> _sequence = new SortedList<decimal, decimal?[]>();
        /// <summary>
        /// 列の数を取得します
        /// </summary>
        public int ColumnCount { get { return _columnNames.Length; } }
        /// <summary>
        /// 時系列データの個数を取得します．
        /// </summary>
        public int SequenceLength { get { return _sequence.Count; } }
        /// <summary>
        /// 探索用の前回のインデックス
        /// </summary>
        private int _prevIndex = 0;
        /// <summary>
        /// 列を持つかを取得します．
        /// </summary>
        public bool HasColumns {
            get { return this.ColumnCount != 0; }
        }
        /// <summary>
        /// 値が設定されている時間範囲を取得します．
        /// </summary>
        public decimal Duration {
            get {
                _rwLock.EnterReadLock();
                try {
                    return this.EndTime - this.BeginTime;
                } finally { _rwLock.ExitReadLock(); }
            }
        }
        /// <summary>
        /// 値が設定されている最初の時間を取得します．
        /// </summary>
        public decimal BeginTime {
            get {
                _rwLock.EnterReadLock();
                try {
                    if(this.SequenceLength == 0)
                        return 0;
                    return _sequence.Keys.First();
                } finally { _rwLock.ExitReadLock(); }
            }
        }
        /// <summary>
        /// 値が設定されている最後の時間を取得します．
        /// </summary>
        public decimal EndTime {
            get {
                _rwLock.EnterReadLock();
                try {
                    if(this.SequenceLength == 0)
                        return 0;
                    return _sequence.Keys.Last();
                } finally { _rwLock.ExitReadLock(); }
            }
        }
        private TimeSeriesValues() {
            _lockDisposable = new LockDisposable(_rwLock);
        }
        /// <summary>
        /// 列数を指定するコンストラクタ
        /// </summary>
        /// <param name="columnCount"></param>
        public TimeSeriesValues(int columnCount)
            : this() {
            _columnNames = new string[columnCount];
            for(int i = 0; i < _columnNames.Length; i++) {
                _columnNames[i] = "";
            }
        }
        /// <summary>
        /// 列名を指定するコンストラクタ
        /// </summary>
        /// <param name="columnNames"></param>
        public TimeSeriesValues(params string[] columnNames)
            : this((IList<string>)columnNames) {
        }
        /// <summary>
        /// 列名を指定するコンストラクタ
        /// </summary>
        /// <param name="columnNames"></param>
        public TimeSeriesValues(IEnumerable<string> columnNames)
            : this() {
            _columnNames = columnNames.ToArray();
        }
        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="original"></param>
        public TimeSeriesValues(TimeSeriesValues original)
            : this(original.ColumnNames) {
            foreach(var pair in original.Enumerate()) {
                this.SetValue(pair.Key, pair.Value);
            }
        }
        /// <summary>
        /// 指定された範囲で時間をクリッピングします．
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        public void ClipTime(decimal beginTime, decimal endTime) {
            _rwLock.EnterWriteLock();
            try {
                if(_sequence == null)
                    return;
                lock(_sequence) {
                    SortedList<decimal, decimal?[]> newSequence = new SortedList<decimal, decimal?[]>();
                    decimal?[] beginValues = GetValueAt(beginTime);
                    SetValue(beginTime, beginValues);
                    decimal?[] endValues = new decimal?[ColumnCount];
                    SetValue(endTime, endValues.ToArray());
                    for(int i = 0; i < SequenceLength; i++) {
                        decimal time = _sequence.Keys[i];
                        if(time >= beginTime && time <= endTime) {
                            newSequence[time] = _sequence.Values[i].ToArray();
                        }
                    }
                    newSequence[BeginTime] = endValues.ToArray();
                    newSequence[EndTime] = endValues.ToArray();
                    _sequence = newSequence;
                }
            } finally { _rwLock.ExitWriteLock(); }
        }
        /// <summary>
        /// 指定された範囲で値をクリッピングします．
        /// </summary>
        /// <param name="valueMin"></param>
        /// <param name="valueMax"></param>
        public void ClipValue(decimal valueMin, decimal valueMax) {
            _rwLock.EnterWriteLock();
            try {
                for(int i = 0; i < _sequence.Count; i++) {
                    for(int j = 0; j < this.ColumnCount; j++) {
                        if(_sequence.Values[i][j].HasValue) {
                            if(valueMin > _sequence.Values[i][j] || _sequence.Values[i][j] > valueMax) {
                                _sequence.Values[i][j] = null;
                            }
                        }
                    }
                }
            } finally { _rwLock.ExitWriteLock(); }
        }
        /// <summary>
        /// 指定された時間におけるインデックスを返します．
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public int GetIndexAt(decimal time) {
            _rwLock.EnterReadLock();
            try {
                int ret = CollectionEx.GetLastIndexBeforeKey<decimal, decimal?[]>(_sequence, time, _prevIndex, 1);
                _prevIndex = ret;
                return ret;
            } finally { _rwLock.ExitReadLock(); }
        }
        /// <summary>
        /// 指定された時間におけるインデックスを返します．
        /// </summary>
        /// <param name="time"></param>
        /// <param name="firstIndex"></param>
        /// <returns></returns>
        public int GetIndexAt(decimal time, int firstIndex) {
            _rwLock.EnterReadLock();
            try {
                int ret = CollectionEx.GetLastIndexBeforeKey<decimal, decimal?[]>(_sequence, time, firstIndex, 1);
                _prevIndex = ret;
                return ret;
            } finally { _rwLock.ExitReadLock(); }
        }
        /// <summary>
        /// 指定されたインデックスにおける時間を返します．
        /// </summary>
        /// <param name="index"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool TryGetTimeFromIndex(int index, out decimal time) {
            _rwLock.EnterReadLock();
            try {
                if(index < 0 || index >= _sequence.Count) {
                    time = 0;
                    return false;
                }
                time = _sequence.Keys[index];
                return true;
            } finally { _rwLock.ExitReadLock(); }
        }
        /// <summary>
        /// 指定されたインデックスにおける値を返します．
        /// </summary>
        /// <param name="index"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool TryGetValueFromIndex(int index, out decimal?[] values) {
            _rwLock.EnterReadLock();
            try {
                if(index < 0 || index >= _sequence.Count) {
                    values = null;
                    return false;
                }
                values = _sequence.Values[index].ToArray();
                return true;
            } finally { _rwLock.ExitReadLock(); }
        }

        /// <summary>
        /// 指定された時刻の値を返します。値がない場合はnull。
        /// </summary>
        /// <param title="time">指定する時刻</param>
        /// <returns>値</returns>
        public decimal?[] GetValueAt(decimal time) {
            _rwLock.EnterReadLock();
            try {
                decimal?[] ret;
                int index = GetIndexAt(time);
                if(!TryGetValueFromIndex(index, out ret)) {
                    return null;
                }
                return ret.ToArray();
            } finally { _rwLock.ExitReadLock(); }
        }
        /// <summary>
        /// 指定された時刻に値を設定します．
        /// </summary>
        /// <param name="time"></param>
        /// <param name="values"></param>
        public void SetValue(decimal time, params decimal?[] values) {
            if(values == null) {
                values = new decimal?[this.ColumnCount];
            } else if(values.Length > this.ColumnCount) {
                decimal?[] tmp = values;
                values = new decimal?[this.ColumnCount];
                for(int i = 0; i < this.ColumnCount; i++) {
                    values[i] = tmp[i];
                }
            } else if(values.Length < this.ColumnCount) {
                decimal?[] tmp = values;
                values = new decimal?[this.ColumnCount];
                tmp.CopyTo(values, 0);
            } else {
                values = values.ToArray();
            }

            _rwLock.EnterWriteLock();
            try {
                _sequence[time] = values;
            } finally { _rwLock.ExitWriteLock(); }
        }
        /// <summary>
        /// 指定された時刻における値を取得または設定します．
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public decimal?[] this[decimal time] {
            get { return this.GetValueAt(time); }
            set { this.SetValue(time, value); }
        }
        /// <summary>
        /// 指定されたインデックスに対する値を取得または設定します．
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public decimal?[] this[int index] {
            get { _rwLock.EnterReadLock(); try { return _sequence.Values[index]; } finally { _rwLock.ExitReadLock(); } }
            set { _rwLock.EnterWriteLock(); try { _sequence.Values[index] = value; } finally { _rwLock.ExitWriteLock(); } }
        }



        /// <summary>
        /// 指定した範囲の値を変更します
        /// </summary>
        /// <param name="lower">変更する範囲の開始時間</param>
        /// <param name="upper">変更する範囲の終了時間</param>
        /// <param name="values">新しい値</param>
        public void SetRange(decimal beginTime, decimal endTime, decimal?[] values) {
            _rwLock.EnterWriteLock();
            try {
                var endValues = this[endTime];
                this.RemoveTimeRange(beginTime, endTime);
                this[endTime] = endValues;
                this[beginTime] = values;
            } finally { _rwLock.ExitWriteLock(); }
        }

        /// <summary>
        /// 指定した範囲の値をnullにします。
        /// </summary>
        /// <param name="lower">設定する範囲の開始時間</param>
        /// <param name="upper">設定する範囲の終了時間</param>
        public void RemoveTimeRange(decimal beginTime, decimal endTime) {
            _rwLock.EnterWriteLock();
            try {
                int index = this.GetIndexAt(beginTime);
                if(index == -1 || _sequence.Keys[index] < beginTime)
                    index++;
                while(index < _sequence.Count) {
                    if(_sequence.Keys[index] >= endTime)
                        break;
                    _sequence.RemoveAt(index);
                }
            } finally { _rwLock.ExitWriteLock(); }
        }
        /// <summary>
        /// 時間と値のペアを列挙します．
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<decimal, decimal?[]>> Enumerate() {
            _rwLock.EnterReadLock();
            try {
                foreach(var pair in _sequence)
                    yield return pair;
            } finally { _rwLock.ExitReadLock(); }
        }

        public IEnumerable<TimeSeriesRowValue> EnumerateRows() {
            _rwLock.EnterReadLock();
            try {
                decimal prevTime = decimal.MinValue;
                decimal?[] prevValues = null;
                foreach(var pair in this.Enumerate()) {
                    if(prevValues != null && prevTime < pair.Key)
                        yield return new TimeSeriesRowValue(prevTime, pair.Key, prevValues);
                    prevTime = pair.Key;
                    prevValues = pair.Value;
                }
                if(prevValues != null && prevTime < this.EndTime) {
                    yield return new TimeSeriesRowValue(prevTime, this.EndTime, prevValues);
                }
            } finally { _rwLock.ExitReadLock(); }
        }

        static int _prevEnumerateRowsBeginIndex, _prevEnumerateRowsEndIndex;
        public IEnumerable<TimeSeriesRowValue> EnumerateRows(decimal beginTime, decimal endTime) {
            _rwLock.EnterReadLock();
            try {
                int beginIndex = this.GetIndexAt(beginTime, _prevEnumerateRowsBeginIndex);
                _prevEnumerateRowsBeginIndex = beginIndex;
                int endIndex = this.GetIndexAt(endTime, _prevEnumerateRowsEndIndex);
                _prevEnumerateRowsEndIndex = endIndex;
                decimal prevTime = beginTime;

                decimal?[] prevValues;
                if(beginIndex == -1) {
                    prevValues = new decimal?[this.ColumnCount];
                } else {
                    prevValues = this[beginIndex];
                }
                for(int i = beginIndex + 1; i <= endIndex; i++) {
                    decimal time = _sequence.Keys[i];
                    decimal?[] values = _sequence.Values[i];
                    if(prevValues != null && prevTime < time)
                        yield return new TimeSeriesRowValue(prevTime, time, prevValues);
                    prevTime = time;
                    prevValues = values;
                }
                if(prevTime < endTime) {
                    yield return new TimeSeriesRowValue(prevTime, endTime, prevValues);
                }
            } finally { _rwLock.ExitReadLock(); }
        }

        /// <summary>
        /// 値が設定されている時間を列挙します．
        /// </summary>
        /// <returns></returns>
        public IEnumerable<decimal> EnumerateTime() {
            _rwLock.EnterReadLock();
            try {
                foreach(var time in _sequence.Keys)
                    yield return time;
            } finally { _rwLock.ExitReadLock(); }
        }

        public static TimeSeriesValues Deserialize(Stream stream) {
            CSVReader reader = new CSVReader(stream);
            TimeSeriesValues ret = null;
            int count = 0;
            while(!reader.EndOfStream) {
                count++;
                string[] row = reader.ReadValues();
                if(row.Length == 0)
                    continue;
                string trimmed = row[0].Trim();
                if(trimmed.StartsWith("#")) {
                    if(ret == null && trimmed.Substring(1).Trim().ToLower() == "name") {
                        ret = new TimeSeriesValues(row.Skip(1).ToArray());
                    }
                    continue;
                }
                decimal time;
                if(!decimal.TryParse(row[0], out time))
                    throw new InvalidDataException("Invalid value at line " + count.ToString());
                decimal?[] values = new decimal?[row.Length - 1];
                for(int i = 1; i < row.Length; i++) {
                    decimal value;
                    if(decimal.TryParse(row[i], out value)) {
                        values[i - 1] = value;
                    }
                }
                if(ret == null) {
                    ret = new TimeSeriesValues(row.Length - 1);
                }
                ret[time] = values;
            }
            // ファイルが空ならばretはnull
            if(ret != null) {
                ret.TrimExcess();
            }
            return ret;
        }

        public void TrimExcess() {
            if(_sequence.Count * 10 < _sequence.Capacity * 9) {
                _sequence.Capacity = _sequence.Count;
            }
        }

        public void WriteTo(Stream stream) {
            _rwLock.EnterReadLock();
            try {
                StreamWriter writer = new StreamWriter(stream);
                string names = CharacterSeparatedValues.ToString(_columnNames.ToArray());
                if(names != "") {
                    names = names.Replace('\r', ' ');
                    names = names.Replace('\n', ' ');
                    writer.WriteLine("#name,{0}", names);
                }
                foreach(var pair in this.Enumerate()) {
                    writer.Write(pair.Key);
                    foreach(var value in pair.Value) {
                        if(value.HasValue) {
                            writer.Write(",{0}", value.Value);
                        } else {
                            writer.Write(",");
                        }
                    }
                    writer.WriteLine();
                }
                writer.Flush();
            } finally { _rwLock.ExitReadLock(); }
        }

        public void Serialize(string fileName) {
            Misc.StreamEx.SaferSave(fileName, (stream) => WriteTo(stream));
        }

        public static TimeSeriesValues Deserialize(string fileName) {
            using(FileStream stream = new FileStream(fileName, FileMode.Open)) {
                return Deserialize(stream);
            }
        }

        readonly LockDisposable _lockDisposable;
        public LockDisposable Lock { get { return _lockDisposable; } }

    }
}
