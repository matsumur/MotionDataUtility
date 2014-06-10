using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Xml;

namespace MotionDataHandler.Sequence {
    using Misc;

    /// <summary>
    /// SequenceDataの保持するデータのタイプ
    /// </summary>
    [Flags]
    public enum SequenceType {
        /// <summary>
        /// データを時系列数値データが主であるとして扱うタイプ
        /// </summary>
        Numeric = 0x01,
        /// <summary>
        /// データを時系列数値データと時系列ラベルデータの両方として扱うタイプ
        /// </summary>
        NumericLabel = 0x03,
        /// <summary>
        /// データを時系列ラベルデータとして扱うタイプ
        /// </summary>
        Label = 0x02,
        /// <summary>
        /// シーケンスを持たない
        /// </summary>
        None = 0x00,
    }

    /// <summary>
    /// 時系列データとラベル境界情報を保持するクラス．
    /// </summary>
    public class SequenceData : IDisposable {
        /// <summary>
        /// 変更されたプロパティのタイプ
        /// </summary>
        public enum PropertyType {
            Title,
            Values,
            Borders,
            Type,
        }
        public class PropertyTypeEventArgs : EventArgs {
            private readonly PropertyType _propertyType;
            public PropertyType PropertyType {
                get { return _propertyType; }
            }
            public PropertyTypeEventArgs(PropertyType propertyType) {
                _propertyType = propertyType;
            }
        }
        /// <summary>
        /// データのプロパティが変更されたときに呼ばれるイベント
        /// </summary>
        public event EventHandler<PropertyTypeEventArgs> PropertyTypeChanged = (s, e) => { };
        public void DoPropertyChanged(PropertyType propertyType) {
            PropertyTypeChanged.Invoke(this, new PropertyTypeEventArgs(propertyType));
        }
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);


        SequenceType _type;
        /// <summary>
        /// 保持するデータを，時系列数値データとして扱うか，時系列ラベルデータとして扱うかを取得または設定します．
        /// </summary>
        public SequenceType Type {
            get { return _type; }
            set {
                _rwLock.EnterWriteLock();
                try {
                    var prev = _type;
                    _type = value;
                    if(prev != value) {
                        this.DoPropertyChanged(PropertyType.Type);
                    }
                    this.DoDataChanged();
                } finally { _rwLock.ExitWriteLock(); }
            }
        }
        /// <summary>
        /// このオブジェクトを破棄します
        /// </summary>
        public void Dispose() {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing) {
            _rwLock.EnterWriteLock();
            try {
                this.DataChanged = null;
                _isDisposed = true;
            } finally { _rwLock.ExitWriteLock(); }
        }

        ~SequenceData() {
            this.Dispose(false);
        }

        private bool _isDisposed;
        public bool IsDisposed { get { return _isDisposed; } }

        #region IDataChanged
        /// <summary>
        /// データが変更された時に呼び出されるイベント
        /// </summary>
        public event EventHandler DataChanged = (s, e) => { };
        private bool _isDataChanged = false;
        public bool IsDataChanged {
            get { return _isDataChanged; }
            set {
                bool prev = _isDataChanged;
                _rwLock.EnterWriteLock();
                try {
                    _isDataChanged = value;
                    if(value && !prev) {
                        DataChanged.Invoke(this, new EventArgs());
                    }
                } finally { _rwLock.ExitWriteLock(); }
            }
        }

        #endregion
        /// <summary>
        /// データの変更を他のオブジェクトに反映させます．
        /// </summary>
        public void DoDataChanged() {
            this.IsDataChanged = true;
        }

        private TimeSeriesValues _values;
        /// <summary>
        /// 時系列データを取得または設定します
        /// </summary>
        public TimeSeriesValues Values {
            get { return _values; }
            set {
                using(this.Lock.GetWriteLock()) {
                    if(this.IsDisposed)
                        throw new ObjectDisposedException(this.GetType().ToString());

                    value = value ?? new TimeSeriesValues();
                    if(ReferenceEquals(_values, value))
                        return;
                    if(_values != null)
                        _lockDisposable.SubLocks.Remove(_values.Lock);
                    _values = value;
                    if(_values != null)
                        _lockDisposable.SubLocks.Add(_values.Lock);

                    this.IsDataChanged = true;
                    this.DoPropertyChanged(PropertyType.Values);
                }
            }
        }

        private LabelingBorders _border;
        /// <summary>
        /// ラベル化境界情報を取得または設定します
        /// </summary>
        public LabelingBorders Borders {
            get { return _border; }
            set {
                using(this.Lock.GetWriteLock()) {
                    if(this.IsDisposed)
                        throw new ObjectDisposedException(this.GetType().ToString());

                    value = value ?? new LabelingBorders();
                    if(ReferenceEquals(_border, value))
                        return;
                    if(_border != null)
                        _lockDisposable.SubLocks.Remove(_border.Lock);
                    _border = value;
                    if(_border != null)
                        _lockDisposable.SubLocks.Add(_border.Lock);

                    this.DoPropertyChanged(PropertyType.Borders);
                    this.IsDataChanged = true;
                }
            }
        }

        static string normalizeTitle(string value) {
            if(value == null)
                value = "";
            value = value.Trim();
            foreach(char c in Path.GetInvalidPathChars()) {
                value = value.Replace(c, '_');
            }
            return value;
        }


        private string _title = "scratch";
        /// <summary>
        /// データのタイトルを取得または設定します．
        /// </summary>
        public string Title {
            get { return _title; }
            set {
                _rwLock.EnterWriteLock();
                try {
                    if(this.IsDisposed)
                        throw new ObjectDisposedException(this.GetType().ToString());

                    value = normalizeTitle(value);
                    if(_title == value)
                        return;

                    _title = value;
                    this.DoPropertyChanged(PropertyType.Title);
                    this.IsDataChanged = true;
                } finally { _rwLock.ExitWriteLock(); }
            }
        }
        /// <summary>
        /// デフォルトのコンストラクタ
        /// </summary>
        public SequenceData()
            : this(null, null, "scratch") {
        }
        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="sequence"></param>
        public SequenceData(SequenceData sequence)
            : this(new TimeSeriesValues(sequence.Values), new LabelingBorders(sequence.Borders), sequence.Title) {
            this.Type = sequence.Type;
        }
        /// <summary>
        /// データを指定するコンストラクタ
        /// </summary>
        /// <param name="sequence">時系列データ</param>
        /// <param name="border">ラベル境界情報</param>
        /// <param name="title">このデータの識別名</param>
        public SequenceData(TimeSeriesValues sequence, LabelingBorders border, string title) {
            _lockDisposable = new LockDisposable(_rwLock);

            this.Values = sequence;
            this.Borders = border;
            this.Title = title;
            this.Type = SequenceType.Numeric;
            this.IsDataChanged = false;
        }

        /// <summary>
        /// ラベル系列を返します．
        /// </summary>
        /// <returns></returns>
        public ICSLabelSequence GetLabelSequence() {
            if(this.IsDisposed)
                throw new ObjectDisposedException(this.GetType().ToString());
            using(this.Lock.GetReadLock()) {
                if(this.Values.ColumnCount == 0) {
                    return new ICSLabelSequence();
                }
                if(this.Borders.TargetColumnIndex >= this.Values.ColumnCount) {
                    this.Borders.TargetColumnIndex = this.Values.ColumnCount - 1;
                }
                decimal span = this.Values.EndTime;
                if(span <= 0) {
                    span = 1;
                }
                ICSLabelSequence ret = new ICSLabelSequence(span);
                string preLabel = null;
                int prevBorderIndex = -1;
                foreach(var pair in this.Values.Enumerate()) {
                    decimal? value = pair.Value[this.Borders.TargetColumnIndex];
                    int borderIndex;
                    if(value.HasValue) {
                        borderIndex = this.Borders.GetIndexFromValue(value.Value);
                    } else {
                        borderIndex = -1;
                    }

                    string label = this.Borders.GetLabelByValue(value);
                    if(prevBorderIndex != borderIndex) {
                        if(pair.Key < span) {
                            ret.SetLabel(pair.Key, span, label);
                        }
                        preLabel = label;
                    }
                    prevBorderIndex = borderIndex;
                }
                return ret;
            }
        }
        /// <summary>
        /// ラベル系列からデータを作成します
        /// </summary>
        /// <param name="labelSequence">ラベル系列</param>
        /// <param name="title">このデータに与えるタイトル</param>
        /// <returns></returns>
        public static SequenceData FromLabelSequence(ICSLabelSequence labelSequence, string title) {
            return FromLabelSequence(labelSequence, title, null);
        }
        /// <summary>
        /// ラベル系列からデータを作成します
        /// </summary>
        /// <param name="labelSequence">ラベル系列</param>
        /// <param name="title">このデータに与えるタイトル</param>
        /// <param name="colorPalette">ラベル名からラベルの色を生成するディクショナリ</param>
        /// <returns></returns>
        public static SequenceData FromLabelSequence(ICSLabelSequence labelSequence, string title, IDictionary<string, Color> colorPalette) {
            if(labelSequence == null)
                throw new ArgumentNullException("labelSequence", "'labelSequence' cannot be null");
            TimeSeriesValues newSeqeunce = new TimeSeriesValues(title);
            LabelingBorders newBorder = new LabelingBorders();
            foreach(var label in labelSequence.EnumerateLabels()) {
                newSeqeunce[label.BeginTime] = new decimal?[] { label.BeginTime };
                newBorder.SetBorder(label.BeginTime, label.LabelText);
            }
            newSeqeunce[labelSequence.Duration] = new decimal?[] { labelSequence.Duration };
            newBorder.SetBorder(labelSequence.Duration, newBorder.DefaultName);

            if(colorPalette != null) {
                foreach(var pair in colorPalette) {
                    newBorder.SetColor(pair.Key, pair.Value);
                }
            }

            SequenceData ret = new SequenceData(newSeqeunce, newBorder, title);
            ret.Type = SequenceType.Label;
            return ret;
        }
        /// <summary>
        /// 指定された時間におけるラベル名を取得します．
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public string GetLabelAt(decimal time) {
            using(this.Lock.GetReadLock()) {
                return this.Borders.GetLabelByValue(GetTargetColumnValueAt(time));
            }
        }

        /// <summary>
        /// 指定された時間におけるラベルの開始時間を返します．
        /// </summary>
        /// <param name="time">時間</param>
        /// <returns></returns>
        public decimal? GetLabelStartTimeAt(decimal time) {
            return this.GetLabelStartTimeAt(time, 0);
        }
        /// <summary>
        /// 指定された時間におけるラベルの開始時間を返します．
        /// </summary>
        /// <param name="time">時間</param>
        /// <param name="offset">取得するラベルのインデックスのオフセット．正を指定すると指定された時間以降のラベルの開始時間を返す．負を指定すると指定された時間依然のラベルの可視時間を返す．</param>
        /// <returns></returns>
        public decimal? GetLabelStartTimeAt(decimal time, int offset) {
            int curBorder = this.Borders.GetIndexFromValue(this.GetTargetColumnValueAt(time));
            int curIndex = this.Values.GetIndexAt(time);
            decimal curTime;
            if(!this.Values.TryGetTimeFromIndex(curIndex, out curTime)) {
                curTime = time;
            }
            decimal ret = curTime;
            if(offset > 0) {
                // ret <- offsetが0になった直後のラベル開始時間
                while(offset > 0) {
                    int prevBorder = curBorder;
                    curIndex++;
                    if(!this.Values.TryGetTimeFromIndex(curIndex, out curTime)) {
                        return null;
                    }
                    curBorder = this.Borders.GetIndexFromValue(this.GetTargetColumnValueAt(curTime));
                    if(curBorder != prevBorder) {
                        offset--;
                    }

                    ret = curTime;
                }
            } else {
                if(curIndex == -1)
                    return null;
                // ret <- offsetが0より大きくなる直前のラベル開始時間
                while(offset <= 0) {
                    ret = curTime;

                    int prevBorder = curBorder;
                    curIndex--;
                    if(!this.Values.TryGetTimeFromIndex(curIndex, out curTime)) {
                        if(offset == 0)
                            break;
                        return null;
                    }
                    curBorder = this.Borders.GetIndexFromValue(this.GetTargetColumnValueAt(curTime));
                    if(curBorder != prevBorder) {
                        offset++;
                    }
                }
            }
            return ret;
        }
        /// <summary>
        /// 指定された時間における時系列データの値のうちラベル生成対象の列の値を返します．
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public decimal? GetTargetColumnValueAt(decimal time) {
            using(this.Lock.GetReadLock()) {
                decimal?[] values = this.Values.GetValueAt(time);
                if(values == null || this.Borders.TargetColumnIndex >= this.Values.ColumnCount)
                    return null;
                return values[Borders.TargetColumnIndex];
            }
        }

        /// <summary>
        /// this.TypeがSequenceType.Labelであるときに指定された時間にラベル境界を追加します．
        /// </summary>
        /// <param name="time">ラベルの境界となる時間</param>
        /// <param name="labelName">ラベル名</param>
        public void SetLabelAt(decimal time, string labelName) {
            if((this.Type & SequenceType.Label) == 0)
                throw new InvalidOperationException("Cannot call SetLabelAt when SequenceData.Type is not Label");
            if(!this.Values.HasColumns) {
                this.Values = new TimeSeriesValues(1);
                this.Borders.TargetColumnIndex = 0;
            }
            decimal?[] values = this.Values.GetValueAt(time);
            if(values == null)
                values = new decimal?[this.Values.ColumnCount];
            if(!this.HasValidTargetColumnIndex())
                return;
            values[this.Borders.TargetColumnIndex] = time;
            this.Values.SetValue(time, values);

            this.Borders.SetBorder(time, labelName);
            this.IsDataChanged = true;
        }
        /// <summary>
        /// this.TypeがSequenceType.Labelであるときに指定された時間範囲にラベルを設定します．
        /// </summary>
        /// <param name="lower">ラベルの開始時間</param>
        /// <param name="upper">ラベルの終了時間</param>
        /// <param name="labelName">ラベル名</param>
        public void SetLabelAt(decimal beginTime, decimal endTime, string labelName) {
            if((this.Type & SequenceType.Label) == 0)
                throw new InvalidOperationException("Cannot call SetLabelAt when SequenceData.Type is not Label");
            if(beginTime > endTime) {
                decimal tmp = beginTime;
                beginTime = endTime;
                endTime = tmp;
            }
            if(!this.Values.HasColumns) {
                this.Values = new TimeSeriesValues(1);
                this.Borders.TargetColumnIndex = 0;
            }
            // 終りの方をセットしておく
            this.SetLabelAt(endTime, this.GetLabelAt(endTime));
            if(beginTime == endTime)
                return;
            decimal?[] beginValues = this.Values.GetValueAt(beginTime);
            if(beginValues == null)
                beginValues = new decimal?[this.Values.ColumnCount];
            if(!this.HasValidTargetColumnIndex())
                return;
            beginValues[this.Borders.TargetColumnIndex] = beginTime;
            this.Values.SetRange(beginTime, endTime, beginValues);

            this.Borders.SetBorderRange(beginTime, endTime, labelName);
            this.IsDataChanged = true;
        }
        /// <summary>
        /// 指定されたインデックスがこのオブジェクトに対して有効であるかを返します．
        /// </summary>
        /// <param name="targetColumnIndex"></param>
        /// <returns></returns>
        public bool HasValidTargetColumnIndex(int targetColumnIndex) {
            return 0 <= targetColumnIndex && targetColumnIndex < this.Values.ColumnCount;
        }

        /// <summary>
        /// このオブジェクトの持つBorder.TargetColumnIndexが有効であるかを返します．
        /// </summary>
        /// <returns></returns>
        public bool HasValidTargetColumnIndex() {
            return this.HasValidTargetColumnIndex(this.Borders.TargetColumnIndex);
        }
        /// <summary>
        /// データをストリームへ出力します
        /// </summary>
        /// <param name="stream"></param>
        public void Serialize(Stream stream) {
            XmlWriter writer = XmlWriter.Create(stream);
            this.Serialize(writer);
        }
        /// <summary>
        /// データをストリームへ出力します
        /// </summary>
        /// <param name="writer"></param>
        public void Serialize(XmlWriter writer) {
            using(this.Lock.GetReadLock()) {
                writer.WriteStartElement(typeof(SequenceData).Name);
                writer.WriteStartElement("Title");
                using(MemoryStream tmpStream = new MemoryStream()) {
                    this.SerializeDataHeader(tmpStream);
                    tmpStream.Position = 0;
                    using(StreamReader reader = new StreamReader(tmpStream)) {
                        writer.WriteValue(reader.ReadToEnd());
                    }
                }
                writer.WriteEndElement();
                writer.WriteStartElement("Sequence");
                using(MemoryStream tmpStream = new MemoryStream()) {
                    this.Values.WriteTo(tmpStream);
                    tmpStream.Position = 0;
                    using(StreamReader reader = new StreamReader(tmpStream)) {
                        writer.WriteValue(reader.ReadToEnd());
                    }
                }
                writer.WriteEndElement();
                writer.WriteStartElement("Border");
                this.Borders.Serialize(writer);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }
        /// <summary>
        /// ストリームからデータを復元します
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static SequenceData Deserialize(Stream stream) {
            XmlReader reader = XmlReader.Create(stream);
            return SequenceData.Deserialize(reader);
        }
        /// <summary>
        /// ストリームからデータを復元します
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static SequenceData Deserialize(XmlReader reader) {
            SequenceData ret = null;
            reader.ReadStartElement(typeof(SequenceData).Name);
            byte[] titleBytes = null, sequenceBytes = null;
            LabelingBorders border = null;
            string tmp;
            MemoryStream tmpStream;
            StreamWriter tmpWriter;
            for(reader.MoveToContent(); reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None; reader.MoveToContent()) {
                if(reader.IsEmptyElement) {
                    reader.Skip();
                    continue;
                }
                switch(reader.Name) {
                case "Title":
                    tmp = reader.ReadElementContentAsString();
                    tmpStream = new MemoryStream();
                    tmpWriter = new StreamWriter(tmpStream);
                    tmpWriter.Write(tmp);
                    tmpWriter.Flush();
                    titleBytes = tmpStream.ToArray();
                    break;
                case "Sequence":
                    tmp = reader.ReadElementContentAsString();
                    tmpStream = new MemoryStream();
                    tmpWriter = new StreamWriter(tmpStream);
                    tmpWriter.Write(tmp);
                    tmpWriter.Flush();
                    sequenceBytes = tmpStream.ToArray();
                    break;
                case "Border":
                    reader.ReadStartElement();
                    border = LabelingBorders.Deserialize(reader.ReadSubtree());
                    for(reader.MoveToContent(); reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None; reader.MoveToContent()) {
                        reader.Skip();
                    }
                    reader.ReadEndElement();
                    break;
                default:
                    reader.Skip();
                    break;
                }
            }
            reader.ReadEndElement();
            if(titleBytes != null && sequenceBytes != null && border != null) {
                ret = new SequenceData();
                ret.Borders = border;
                using(MemoryStream stream2 = new MemoryStream(sequenceBytes)) {
                    ret.Values = TimeSeriesValues.Deserialize(stream2);
                }
                using(MemoryStream stream2 = new MemoryStream(titleBytes)) {
                    ret.RetrieveDataHeader(stream2);
                }
            }
            return ret;
        }
        /// <summary>
        /// このデータのヘッダ情報をストリームへ出力します．
        /// </summary>
        /// <param name="stream"></param>
        public void SerializeDataHeader(Stream stream) {
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(Title);
            writer.WriteLine(this.Type.ToString());
            writer.Flush();
        }
        /// <summary>
        /// ヘッダ情報をストリームから復元します．
        /// </summary>
        /// <param name="stream"></param>
        public void RetrieveDataHeader(Stream stream) {
            StreamReader reader = new StreamReader(stream);
            this.Title = reader.ReadLine();
            try {
                this.Type = (SequenceType)Enum.Parse(typeof(SequenceType), reader.ReadLine());
            } catch(ArgumentException) {
                this.Type = SequenceType.Numeric;
            }
        }

        /// <summary>
        /// 境界データ、値データ、タイトルを別々のファイルに保存します。拡張子は自動的に付加されます。
        /// </summary>
        /// <param name="path">保存するファイルのパス。自動で拡張子が付加される</param>
        public void SaveState(string path) {
            using(this.Lock.GetUpgradeableReadLock()) { // 中に IsDataChanged = false があるのでUpgradeableRead
                string titlePath = string.Format("{0}{1}", path, DefaultExtensionForHeader);
                if(IsDataChanged || !File.Exists(titlePath)) {
                    Misc.StreamEx.SaferSave(titlePath, (st) => {
                        this.SerializeDataHeader(st);
                    });
                }
                string sequencePath = string.Format("{0}{1}", path, DefaultExtensionForValues);
                if(IsDataChanged || !File.Exists(sequencePath)) {
                    Values.Serialize(sequencePath);
                }
                string borderPath = string.Format("{0}{1}", path, DefaultExtensionForBorder);
                if(IsDataChanged || !File.Exists(borderPath)) {
                    Borders.Serialize(borderPath);
                }
                this.IsDataChanged = false;
            }
        }
        /// <summary>
        /// ヘッダ情報を保存するパスに使われる拡張子を取得します
        /// </summary>
        public readonly static string DefaultExtensionForHeader = ".txt";
        /// <summary>
        /// 時系列データを保存するパスに使われる拡張子を取得します．
        /// </summary>
        public readonly static string DefaultExtensionForValues = ".seq";
        /// <summary>
        /// ラベル境界情報を保存するパスに使われる拡張子を取得します
        /// </summary>
        public readonly static string DefaultExtensionForBorder = ".seqbdr";

        /// <summary>
        /// 指定されたパスのファイルからSequenceDataを作成します。拡張子は自動的に付加されます。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SequenceData RetrieveState(string path) {
            SequenceData ret = new SequenceData();
            if(!File.Exists(path + DefaultExtensionForHeader)) {
                string ext = Path.GetExtension(path);
                if(ext == DefaultExtensionForHeader || ext == DefaultExtensionForValues || ext == DefaultExtensionForBorder) {
                    path = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
                    if(!File.Exists(path + DefaultExtensionForHeader)) {
                        return null;
                    }
                } else {
                    return null;
                }
            }
            string titlePath = string.Format("{0}{1}", path, DefaultExtensionForHeader);
            string sequencePath = string.Format("{0}{1}", path, DefaultExtensionForValues);
            string borderPath = string.Format("{0}{1}", path, DefaultExtensionForBorder);
            using(FileStream titleStream = new FileStream(titlePath, FileMode.Open)) {
                ret.RetrieveDataHeader(titleStream);
            }
            ret.Values = TimeSeriesValues.Deserialize(sequencePath);
            ret.Borders = LabelingBorders.Deserialize(borderPath);
            ret.IsDataChanged = false;
            return ret;
        }

        readonly LockDisposable _lockDisposable;
        /// <summary>
        /// オブジェクトへのロック用オブジェクトを取得します．
        /// </summary>
        public LockDisposable Lock { get { return _lockDisposable; } }
    }
}
