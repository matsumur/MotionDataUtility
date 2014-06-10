using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Diagnostics;

namespace MotionDataHandler.Sequence {
    using Misc;

    /// <summary>
    /// TimeSeriesValueDataとペアでSequenceDataに所有されるラベル境界情報．
    /// TimeSeriesValueDataは時系列データを保持し，LabelingBordersはデータの値の範囲に付与するラベル名を保持する．
    /// </summary>
    public class LabelingBorders : IXmlSerializable, ICloneable {
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly LockDisposable _lockDisposable;
        public LockDisposable Lock { get { return _lockDisposable; } }

        int _targetColumnIndex = 0;
        /// <summary>
        /// 境界の評価をするSequenceValuesの対象のカラムのインデックスを取得または設定します。
        /// </summary>
        public int TargetColumnIndex {
            get { return _targetColumnIndex; }
            set {
                if(value < 0)
                    throw new ArgumentOutOfRangeException("value", "'TargetColumnIndex' cannot be negative");
                _targetColumnIndex = value;
            }
        }
        /// <summary>
        /// 境界値/境界名のリスト
        /// </summary>
        private SortedList<decimal, string> __borders = new SortedList<decimal, string>();
        /// <summary>
        /// 値がない時と値が最低の境界値以下のときの境界
        /// </summary>
        private string _defaultName = "";
        /// <summary>
        /// 境界名一覧を作るときに全部の境界を探索しないで済むように境界名の数を保持する
        /// </summary>
        private Dictionary<string, int> _nameCounter = new Dictionary<string, int>();
        /// <summary>
        /// 境界名=>色のリスト
        /// </summary>
        private Dictionary<string, Color> __colorPalette = new Dictionary<string, Color>();
        /// <summary>
        /// 境界名一覧のキャッシュ更新フラグ
        /// </summary>
        bool _nameChanged = false;
        /// <summary>
        /// 境界名一覧のキャッシュ
        /// </summary>
        List<string> _nameCache;


        /// <summary>
        /// 境界名の色を設定します。
        /// </summary>
        /// <param name="name">境界名</param>
        /// <param name="color">色</param>
        public void SetColor(string name, Color color) {
            _rwLock.EnterWriteLock();
            try {
                _colorPalette[name] = color;
            } finally { _rwLock.ExitWriteLock(); }
        }
        /// <summary>
        /// 境界名に設定されている色を返します
        /// </summary>
        /// <param name="name">境界名</param>
        /// <returns></returns>
        public Color GetLabelColor(string name) {
            _rwLock.EnterReadLock();
            try {
                var palette = _colorPalette;
                if(palette.ContainsKey(name))
                    return palette[name];
                return Color.Empty;
            } finally { _rwLock.ExitReadLock(); }
        }
        /// <summary>
        /// 境界名から色を得るためのディクショナリを返します．
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Color> GetColorPalette() {
            return this.GetLabelNames().ToDictionary(l => l, l => this.GetLabelColor(l));
        }
        /// <summary>
        /// 指定された境界名の色を名前から決まるデフォルト名に設定します．
        /// </summary>
        /// <param name="name"></param>
        public void ResetLabelColor(string name) {
            _colorPalette.Remove(name);
        }
        /// <summary>
        /// すべての境界名の色をデフォルトに設定します．
        /// </summary>
        public void ClearColorPalette() {
            _colorPalette.Clear();
        }
        /// <summary>
        /// ColorPalette変更時用のロックオブジェクト
        /// </summary>
        private readonly object _lockColorPalette = new object();
        /// <summary>
        /// 境界名=>色の一覧．新しい境界名が指定されたときはデフォルトの色を設定
        /// </summary>
        private Dictionary<string, Color> _colorPalette {
            get {
                _rwLock.EnterReadLock();
                try {
                    lock(_lockColorPalette) {
                        IList<string> names = this.GetLabelNames();
                        foreach(var name in names) {
                            if(!__colorPalette.ContainsKey(name)) {
                                __colorPalette[name] = GetDefaultColorFor(name);
                            }
                        }
                        return __colorPalette;
                    }
                } finally { _rwLock.ExitReadLock(); }
            }
            set {
                _rwLock.EnterWriteLock();
                try {
                    if(value == null) {
                        __colorPalette = new Dictionary<string, Color>();
                    } else {
                        __colorPalette = new Dictionary<string, Color>(value);
                    }
                } finally { _rwLock.ExitWriteLock(); }
            }
        }

        /// <summary>
        /// 境界名に対するデフォルトの色を生成します
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Color GetDefaultColorFor(string name) {
            System.Security.Cryptography.SHA256Managed __sha256 = new System.Security.Cryptography.SHA256Managed();
            byte[] mainName = __sha256.ComputeHash(Encoding.UTF8.GetBytes(name.Substring(0, 3 > name.Length ? name.Length : 3)));
            byte[] subName = __sha256.ComputeHash(Encoding.UTF8.GetBytes(name));
            int hueInt = (mainName[7] / 16) * 4096 + subName[9] * 16 + subName[17] / 16;
            float hue = (float)(Math.PI * 2) * hueInt / 65536;
            int satInt = mainName[13] + subName[3];
            float sat = 0.1f + 0.8f * satInt / 510;
            long valueInt = (255 - (int)subName[0] * subName[2] * subName[7] / 255 / 255)
                + (255 - (int)subName[1] * subName[4] * subName[18] / 255 / 255)
                + (255 - (int)subName[17] * subName[12] * subName[30] / 255 / 255)
                + (255 - (int)subName[21] * subName[22] * subName[8] / 255 / 255);
            float value = (float)valueInt / 1020;
            return ColorEx.ColorFromHSV(hue, sat, value);
        }

        /// <summary>
        /// 指定されたインデックスが境界一覧に含まれるかを返します．
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsValidIndex(int index) {
            _rwLock.EnterReadLock();
            try {
                return index >= 0 && index < __borders.Count;
            } finally { _rwLock.ExitReadLock(); }
        }


        /// <summary>
        /// 境界を列挙します。
        /// </summary>
        public IEnumerable<KeyValuePair<decimal, string>> Enumerate() {
            _rwLock.EnterReadLock();
            try {
                foreach(var pair in __borders) { yield return pair; }
            } finally { _rwLock.ExitReadLock(); }
        }
        /// <summary>
        /// 指定されたインデックスの境界を取得します。
        /// </summary>
        /// <param name="index">取得する境界のインデックス</param>
        /// <returns></returns>
        public KeyValuePair<decimal, string> GetBorderByIndex(int index) {
            _rwLock.EnterReadLock();
            try {
                return new KeyValuePair<decimal, string>(__borders.Keys[index], __borders.Values[index]);
            } finally { _rwLock.ExitReadLock(); }
        }
        /// <summary>
        /// 境界の数を取得します。
        /// </summary>
        public int BorderCount { get { return __borders.Count; } }
        /// <summary>
        /// 境界領域が指定されていない部分の境界名を取得または設定します。
        /// </summary>
        public string DefaultName {
            get { return _defaultName; }
            set {
                _rwLock.EnterWriteLock();
                try {
                    _nameChanged = true;
                    decrementLabelRef(_defaultName);
                    if(value == null) {
                        _defaultName = "";
                    } else {
                        _defaultName = value;
                    }
                    incrementLabelRef(_defaultName);
                } finally { _rwLock.ExitWriteLock(); }
            }
        }
        /// <summary>
        /// 標準のコンストラクタ
        /// </summary>
        public LabelingBorders() {
            _lockDisposable = new LockDisposable(_rwLock);
            _defaultName = "";
            incrementLabelRef("");
        }
        /// <summary>
        /// 指定された値における境界名を取得します．または，指定された値の位置に境界を設定します．
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[decimal key] {
            get { return this.GetLabelByValue(key); }
            set { this.SetBorder(key, value); }
        }
        /// <summary>
        /// 指定されたインデックスの境界名を取得または設定します．
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string this[int index] {
            get { return this.GetBorderByIndex(index).Value; }
            set { this.SetBorder(index, value); }
        }

        /// <summary>
        /// 境界名ラベルの参照数を1増やします。
        /// </summary>
        /// <param name="label">境界名</param>
        private void incrementLabelRef(string label) {
            _rwLock.EnterWriteLock();
            try {
                if(!_nameCounter.ContainsKey(label)) {
                    _nameCounter[label] = 0;
                }
                _nameCounter[label]++;
                _nameChanged = true;
            } finally { _rwLock.ExitWriteLock(); }
        }
        /// <summary>
        /// 境界名ラベルの参照数を1減らします。
        /// </summary>
        /// <param name="label">境界名</param>
        private void decrementLabelRef(string label) {
            _rwLock.EnterWriteLock();
            try {
                System.Diagnostics.Debug.Assert(_nameCounter.ContainsKey(label));
                _nameCounter[label]--;
                System.Diagnostics.Debug.Assert(_nameCounter[label] >= 0);
                _nameChanged = true;
            } finally { _rwLock.ExitWriteLock(); }
        }
        /// <summary>
        /// 指定された値の位置に境界を設定します．
        /// </summary>
        /// <param name="lowerEnd"></param>
        /// <param name="name"></param>
        public void SetBorder(decimal lowerEnd, string name) {
            _rwLock.EnterWriteLock();
            try {
                if(name == null)
                    name = "";
                RemoveBorder(lowerEnd);
                __borders[lowerEnd] = name;
                incrementLabelRef(name);
            } finally { _rwLock.ExitWriteLock(); }
        }
        /// <summary>
        /// 指定されたインデックスの境界名を設定します．
        /// </summary>
        /// <param name="lowerEnd"></param>
        /// <param name="name"></param>
        public void SetBorder(int index, string name) {
            _rwLock.EnterWriteLock();
            try {
                if(name == null)
                    name = "";
                decimal time = __borders.Keys[index];
                RemoveAt(index);
                __borders[time] = name;
                incrementLabelRef(name);
            } finally { _rwLock.ExitWriteLock(); }
        }
        /// <summary>
        /// 指定された位置の境界を取り除きます。
        /// </summary>
        /// <param name="lowerEnd"></param>
        public void RemoveBorder(decimal lowerEnd) {
            _rwLock.EnterWriteLock();
            try {
                if(__borders.ContainsKey(lowerEnd)) {
                    string label = __borders[lowerEnd];
                    __borders.Remove(lowerEnd);
                    decrementLabelRef(label);
                }
            } finally { _rwLock.ExitWriteLock(); }
        }
        /// <summary>
        /// 条件を満たす境界を取り除きます
        /// </summary>
        /// <param name="match"></param>
        public void RemoveAll(Predicate<KeyValuePair<decimal, string>> match) {
            _rwLock.EnterWriteLock();
            try {
                foreach(var pair in __borders.ToDictionary(p => p.Key, p => p.Value)) {
                    if(match(pair)) {
                        RemoveBorder(pair.Key);
                    }
                }
            } finally { _rwLock.ExitWriteLock(); }
        }
        /// <summary>
        /// 指定されたインデックスの境界を取り除きます。
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index) {
            _rwLock.EnterWriteLock();
            try {
                if(index >= 0 && index < BorderCount) {
                    string label = __borders.Values[index];
                    __borders.RemoveAt(index);
                    decrementLabelRef(label);
                }
            } finally { _rwLock.ExitWriteLock(); }
        }
        /// <summary>
        /// 指定された範囲を一つの境界領域とします。
        /// </summary>
        /// <param name="lowerEnd"></param>
        /// <param name="upperEnd"></param>
        /// <param name="name"></param>
        public void SetBorderRange(decimal lowerEnd, decimal upperEnd, string name) {
            _rwLock.EnterWriteLock();
            try {
                string upperLabel = GetLabelByValue(upperEnd);
                SetBorder(upperEnd, upperLabel);
                SetBorder(lowerEnd, name);
                foreach(var time in (from time in __borders.Keys where time > lowerEnd && time < upperEnd select time).ToList()) {
                    RemoveBorder(time);
                }
            } finally { _rwLock.ExitWriteLock(); }
        }
        /// <summary>
        /// 値による境界名探索時の前回の戻り値
        /// </summary>
        private int _prevIndex = 0;
        /// <summary>
        /// 指定された値以下の最も近い境界のインデックスを取得します。存在しない場合は-1を返します。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int GetIndexFromValue(decimal value) {
            _rwLock.EnterReadLock();
            try {
                int ret = CollectionEx.GetLastIndexBeforeKey<decimal, string>(__borders, value, _prevIndex, 1);
                _prevIndex = ret;
                return ret;
            } finally { _rwLock.ExitReadLock(); }
        }
        /// <summary>
        /// 指定された値以下の最も近い境界のインデックスを取得します。存在しない場合は-1を返します。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int GetIndexFromValue(decimal? value) {
            if(!value.HasValue)
                return -1;
            return this.GetIndexFromValue(value.Value);
        }
        /// <summary>
        /// 指定された値における境界名を取得します．
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetLabelByValue(decimal value) {
            _rwLock.EnterReadLock();
            try {
                _prevIndex = this.GetIndexFromValue(value);
                if(_prevIndex == -1)
                    return this.DefaultName;
                return __borders.Values[_prevIndex];
            } finally { _rwLock.ExitReadLock(); }
        }
        /// <summary>
        /// 指定された値における境界名を取得します．値がnullの場合にはデフォルト名を返します．
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetLabelByValue(decimal? value) {
            if(!value.HasValue)
                return this.DefaultName;
            return this.GetLabelByValue(value.Value);
        }
        /// <summary>
        /// 境界名の一覧を取得します．
        /// </summary>
        /// <returns></returns>
        public IList<string> GetLabelNames() {
            return this.GetLabelNames(false);
        }
        /// <summary>
        /// 境界名一覧を取得します。
        /// </summary>
        /// <param name="includeEmpty">境界名が空のものを含めるか</param>
        /// <returns></returns>
        public IList<string> GetLabelNames(bool includeEmpty) {
            _rwLock.EnterReadLock();
            try {
                if(_nameCache == null || _nameChanged) {
                    var labels = from pair in _nameCounter where pair.Value > 0 select pair.Key;
                    _nameCache = (from name in labels.Union(new string[] { this.DefaultName })
                                  where name != null
                                  orderby name
                                  select name).ToList();
                    _nameChanged = false;
                }
                return _nameCache.Where(x => includeEmpty || x != "").ToList();
            } finally { _rwLock.ExitReadLock(); }
        }
        /// <summary>
        /// 前後で境界名が変わらない境界を取り除きます。
        /// </summary>
        public void RemoveBorderSameToPrevious() {
            _rwLock.EnterWriteLock();
            try {
                string prev = null;
                HashSet<decimal> remove = new HashSet<decimal>();
                foreach(var pair in __borders) {
                    if(pair.Value == prev) {
                        remove.Add(pair.Key);
                    }
                    prev = pair.Value;
                }
                RemoveAll(x => remove.Contains(x.Key));
            } finally { _rwLock.ExitWriteLock(); }
        }
        /// <summary>
        /// 引数に応じて境界名を変更します．
        /// </summary>
        /// <param name="map"></param>
        public void ReplaceBorderName(IDictionary<string, string> map) {
            _rwLock.EnterWriteLock();
            try {
                var prev = this.Enumerate().ToList();
                int index = 0;
                foreach(var border in prev) {
                    if(map.ContainsKey(border.Value)) {
                        this.SetBorder(index, map[border.Value]);
                    }
                    index++;
                }
                if(map.ContainsKey(this.DefaultName)) {
                    this.DefaultName = map[this.DefaultName];
                }
            } finally { _rwLock.ExitWriteLock(); }
        }



        public void Serialize(Stream stream) {
            _rwLock.EnterReadLock();
            try {
                XmlSerializer sel = new XmlSerializer(typeof(LabelingBorders));
                sel.Serialize(stream, this);
            } finally { _rwLock.ExitReadLock(); }
        }

        public static LabelingBorders Deserialize(Stream stream) {
            XmlReader reader = XmlReader.Create(stream);
            return Deserialize(reader);
        }

        public void Serialize(string fileName) {
            _rwLock.EnterReadLock();
            try {
                Misc.StreamEx.SaferSave(fileName, (st) => this.Serialize(st));
            } finally { _rwLock.ExitReadLock(); }
        }
        public static LabelingBorders Deserialize(string fileName) {
            using(FileStream stream = new FileStream(fileName, FileMode.Open)) {
                return Deserialize(stream);
            }
        }
        public void Serialize(XmlWriter writer) {
            _rwLock.EnterReadLock();
            try {
                XmlSerializer sel = new XmlSerializer(typeof(LabelingBorders));
                sel.Serialize(writer, this);
            } finally { _rwLock.ExitReadLock(); }
        }
        public static LabelingBorders Deserialize(XmlReader reader) {
            LabelingBorders ret = new LabelingBorders();
            ret.ReadXml(reader);
            return ret;
        }

        #region IXmlSerializable

        public System.Xml.Schema.XmlSchema GetSchema() {
            throw new System.NotImplementedException();
        }

        public void ReadXml(XmlReader reader) {
            _rwLock.EnterWriteLock();
            try {
                reader.MoveToContent();
                if(reader.IsEmptyElement) {
                    reader.Skip();
                    return;
                }
                if(reader.Name != typeof(LabelingBorders).Name && reader.Name != "TimeValueBorder" && reader.Name != "SequenceBorder")
                    throw new XmlException("Node not for " + typeof(LabelingBorders).Name);
                reader.ReadStartElement();
                for(reader.MoveToContent(); reader.NodeType != XmlNodeType.None && reader.NodeType != XmlNodeType.EndElement; reader.MoveToContent()) {
                    if(reader.IsEmptyElement) {
                        reader.Skip();
                        continue;
                    }
                    switch(reader.Name) {
                    case "DefaultName":
                        if(reader.IsEmptyElement) {
                            reader.Skip();
                            continue;
                        }
                        reader.ReadStartElement();
                        this.DefaultName = reader.ReadString();
                        while(reader.NodeType != XmlNodeType.None && reader.NodeType != XmlNodeType.EndElement)
                            reader.Skip();
                        reader.ReadEndElement();
                        break;
                    case "Borders":
                        if(reader.IsEmptyElement) {
                            reader.Skip();
                            continue;
                        }
                        reader.ReadStartElement();
                        while(reader.NodeType != XmlNodeType.None && reader.NodeType != XmlNodeType.EndElement) {
                            switch(reader.Name) {
                            case "Value":
                                string keyStr;
                                if((keyStr = reader.GetAttribute("Key")) == null) {
                                    reader.Skip();
                                    continue;
                                }
                                decimal key;
                                if(!decimal.TryParse(keyStr, out key)) {
                                    reader.Skip();
                                }

                                if(reader.IsEmptyElement) {
                                    SetBorder(key, null);
                                    reader.Skip();
                                    continue;
                                }
                                reader.ReadStartElement();
                                string value = reader.ReadString();
                                SetBorder(key, value);
                                while(reader.NodeType != XmlNodeType.None && reader.NodeType != XmlNodeType.EndElement)
                                    reader.Skip();
                                reader.ReadEndElement();
                                break;
                            default:
                                reader.Skip();
                                break;
                            }
                        }
                        reader.ReadEndElement();
                        break;
                    case "ColorPalette":
                        reader.ReadStartElement();
                        _colorPalette.Clear();
                        for(reader.MoveToContent(); reader.NodeType != XmlNodeType.None && reader.NodeType != XmlNodeType.EndElement; reader.MoveToContent()) {
                            if(reader.IsEmptyElement) {
                                reader.Skip();
                                continue;
                            }
                            switch(reader.Name) {
                            case "Color":
                                string label;
                                label = reader.GetAttribute("Label");
                                Color color;
                                if(label == null) {
                                    reader.Skip();
                                } else {
                                    string colorStr = reader.ReadElementContentAsString();
                                    try {
                                        color = ColorTranslator.FromHtml(colorStr);
                                    } catch { break; }
                                    this.SetColor(label, color);
                                }
                                break;
                            default:
                                reader.Skip();
                                break;
                            }
                        }
                        reader.ReadEndElement();
                        break;
                    default:
                        reader.Skip();
                        break;
                    }
                }
                reader.ReadEndElement();
            } finally { _rwLock.ExitWriteLock(); }
        }

        public void WriteXml(XmlWriter writer) {
            _rwLock.EnterReadLock();
            try {
                writer.WriteStartElement("DefaultName");
                writer.WriteString(DefaultName);
                writer.WriteEndElement();
                writer.WriteStartElement("Borders");
                foreach(var pair in Enumerate()) {
                    writer.WriteStartElement("Value");
                    writer.WriteStartAttribute("Key");
                    writer.WriteValue(pair.Key);
                    writer.WriteEndAttribute();
                    writer.WriteValue(pair.Value);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteStartElement("ColorPalette");
                foreach(var pair in _colorPalette) {
                    writer.WriteStartElement("Color");
                    writer.WriteAttributeString("Label", pair.Key);
                    writer.WriteValue(ColorTranslator.ToHtml(pair.Value));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            } finally { _rwLock.ExitReadLock(); }
        }

        #endregion


        #region ICloneable メンバ

        public object Clone() {
            return new LabelingBorders(this);
        }

        #endregion

        public LabelingBorders(LabelingBorders original)
            : this() {
            foreach(var pair in original.GetColorPalette()) {
                this.SetColor(pair.Key, pair.Value);
            }
            this.DefaultName = original.DefaultName;
            foreach(var pair in original.Enumerate()) {
                this.SetBorder(pair.Key, pair.Value);
            }
            this.TargetColumnIndex = original.TargetColumnIndex;
        }
    }
}
