using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Collections.ObjectModel;


namespace MotionDataHandler.Sequence {
    using Misc;
    public class ICSLabel {
        /// <summary>
        /// ラベルの開始時間を取得します．
        /// </summary>
        public readonly decimal BeginTime;
        /// <summary>
        /// ラベルの終了時間を取得します．
        /// </summary>
        public readonly decimal EndTime;
        /// <summary>
        /// ラベル名を取得します
        /// </summary>
        public readonly string LabelText;
        /// <summary>
        /// ラベルの時間長を取得します．
        /// </summary>
        public decimal Duration {
            get { return EndTime - BeginTime; }
        }
        /// <summary>
        /// プライベートコンストラクタ
        /// </summary>
        private ICSLabel() { }
        /// <summary>
        /// ラベルを作成します
        /// </summary>
        /// <param name="lower">ラベルの開始時間</param>
        /// <param name="upper">ラベルの終了時間．ラベルの開始時間より大きい必要があります</param>
        /// <param name="labelText">ラベル名</param>
        public ICSLabel(decimal beginTime, decimal endTime, string labelText) {
            if(beginTime >= endTime) {
                throw new ArgumentOutOfRangeException("beginTime", "'beginTime' must be less than 'endTime'");
            }
            if(labelText == null)
                labelText = "";
            labelText = labelText.Replace('\n', ' ');
            labelText = labelText.Replace('\r', ' ');
            LabelText = labelText.Trim();

            BeginTime = beginTime;
            EndTime = endTime;
        }
        /// <summary>
        /// ラベル名を変更した新しいラベルを返します．
        /// </summary>
        /// <param name="newLabelText">新しいラベル名</param>
        /// <returns></returns>
        public ICSLabel ChangeText(string newLabelText) {
            return new ICSLabel(this.BeginTime, this.EndTime, newLabelText);
        }
        /// <summary>
        /// ラベル期間を変更した新しいラベルを返します．
        /// </summary>
        /// <param name="newBeginTime">ラベルの開始時間</param>
        /// <param name="newEndTime">ラベルの終了時間</param>
        /// <returns></returns>
        public ICSLabel ChangeTime(decimal newBeginTime, decimal newEndTime) {
            return new ICSLabel(newBeginTime, newEndTime, LabelText);
        }
        /// <summary>
        /// ラベル期間にオフセットを加えた新しいラベルを返します．
        /// </summary>
        /// <param name="offsetTime"></param>
        /// <returns></returns>
        public ICSLabel Offset(decimal offsetTime) {
            return ChangeTime(BeginTime + offsetTime, EndTime + offsetTime);
        }
        /// <summary>
        /// ラベル内容が空であるかを取得します。
        /// </summary>
        public bool IsEmptyLabel {
            get { return LabelText == "" || LabelText == null; }
        }
        /// <summary>
        /// ラベルをカンマ区切りで文字列として返します．
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return CharacterSeparatedValues.ToString(new string[] { BeginTime.ToString(), EndTime.ToString(), LabelText });
        }
    }
    /// <summary>
    /// iCorpusStudio用のラベルデータを扱うためのクラス
    /// </summary>
    public class ICSLabelSequence {
        /// <summary>
        /// ラベルの開始位置とラベル名のペア
        /// </summary>
        protected SortedList<decimal, string> _labelBorders;
        /// <summary>
        /// ラベルの終端の秒数
        /// </summary>
        protected decimal _duration;
        /// <summary>
        /// ラベルの終端の秒数を取得または設定します。
        /// </summary>
        public decimal Duration {
            get {
                if(_labelBorders.Count == 0) {
                    return 0;
                }
                return _labelBorders.Keys.Last();
            }
            set {
                if(Duration >= value) {
                    foreach(var time in (from time in _labelBorders.Keys where time > value select time).ToList()) {
                        _labelBorders.Remove(time);
                    }
                    _labelBorders[value] = "";
                } else {
                    this.SetLabel(Duration, value, "");
                }
            }
        }
        /// <summary>
        /// ラベルを列挙します
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ICSLabel> EnumerateLabels() {
            bool first = true;
            decimal begin = 0;
            string pre = null;
            foreach(var pair in _labelBorders) {
                if(!first) {
                    yield return new ICSLabel(begin, pair.Key, pre);
                } else {
                    first = false;
                }
                pre = pair.Value;
                begin = pair.Key;
            }
        }
        /// <summary>
        /// 有効なラベル名一覧を返します。
        /// </summary>
        /// <returns>ラベル名一覧</returns>
        public Collection<string> GetLabelTexts() {
            return new Collection<string>((from label in _labelBorders.Values.Distinct() where label != null && label != "" orderby label select label).ToList());
        }
        /// <summary>
        /// ラベル名一覧を返します。
        /// </summary>
        /// <param title="addEmpty">空白のラベルを出力するかを指定します。</param>
        /// <returns>ラベル名一覧</returns>
        public Collection<string> GetLabelTexts(bool addEmpty) {
            return new Collection<string>((from label in _labelBorders.Values.Distinct() where (addEmpty || label != null) orderby label select label == null ? "" : label).ToList());
        }

        /// <summary>
        /// ICSLabelSequenceを生成するコンストラクタ
        /// </summary>
        public ICSLabelSequence() : this(0) { }

        /// <summary>
        /// ICSLabelSequenceを生成するコンストラクタ
        /// </summary>
        /// <param title="span">ラベルの終端の秒数</param>
        public ICSLabelSequence(decimal span) {
            _labelBorders = new SortedList<decimal, string>();
            Duration = span;
        }

        /// <summary>
        /// ラベルの列挙からICSLabelSequenceを生成するコンストラクタ
        /// </summary>
        /// <param name="labels"></param>
        public ICSLabelSequence(IEnumerable<ICSLabel> labels)
            : this(0) {
            foreach(var label in labels) {
                this.SetLabel(label);
            }
        }

        /// <summary>
        /// 指定された時間のLabels.Valuesのインデックスを返します。
        /// </summary>
        /// <param title="time">指定する時間</param>
        /// <returns>インデックス</returns>
        public int GetLabelIndexAt(decimal time) {
            int ret = CollectionEx.GetLastIndexBeforeKey<decimal, string>(_labelBorders, time, _prevIndex, 1);
            _prevIndex = ret;
            return ret;
        }
        int _prevIndex = 0;

        /// <summary>
        /// 指定された時間のラベル名を返します。
        /// </summary>
        /// <param title="time">指定する時間</param>
        /// <returns>ラベル名</returns>
        public ICSLabel GetLabelAt(decimal time) {
            int index = GetLabelIndexAt(time);
            return this.GetLabelByIndex(index);
        }

        /// <summary>
        /// 指定されたインデックスを持つラベルを返します．存在しない場合はnull．
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ICSLabel GetLabelByIndex(int index) {
            if(index < 0 || index >= _labelBorders.Count - 1)
                return null;
            return new ICSLabel(_labelBorders.Keys[index], _labelBorders.Keys[index + 1], _labelBorders.Values[index]);
        }

        /// <summary>
        /// 範囲にラベルを設定します。
        /// </summary>
        /// <param title="lower">ラベルの開始時点</param>
        /// <param title="upper">ラベルの終了時点</param>
        /// <param title="labelText">ラベル名</param>
        public void SetLabel(decimal beginTime, decimal endTime, string labelText) {
            this.SetLabel(new ICSLabel(beginTime, endTime, labelText));
        }

        /// <summary>
        /// 範囲にラベルを設定します。
        /// </summary>
        /// <param title="label">設定されるラベル</param>
        public void SetLabel(ICSLabel label) {
            this.SetLabel(label, false);
        }

        /// <summary>
        /// 範囲にラベルを設定します。
        /// </summary>
        /// <param title="lower">ラベルの開始時点</param>
        /// <param title="upper">ラベルの終了時点</param>
        /// <param title="labelText">ラベル名</param>
        /// <param title="keepWrittenLabel">すでに設定されたラベルを保持するか</param>
        public void SetLabel(decimal beginTime, decimal endTime, string labelText, bool keepWrittenLabel) {
            this.SetLabel(new ICSLabel(beginTime, endTime, labelText), keepWrittenLabel);
        }

        /// <summary>
        /// 範囲にラベルを設定します。
        /// </summary>
        /// <param title="label">設定されるラベル</param>
        /// <param title="keepWrittenLabel">すでに設定されたラベルを保持するか</param>
        public void SetLabel(ICSLabel label, bool keepWrittenLabel) {
            int beforeEndIndex = GetLabelIndexAt(label.EndTime);
            int beforeBeginIndex = GetLabelIndexAt(label.BeginTime);
            string endLabel = "";
            if(beforeEndIndex != -1) {
                endLabel = _labelBorders.Values[beforeEndIndex];
            }
            if(keepWrittenLabel) {
                var innerEmpty = _labelBorders.Skip(beforeBeginIndex + 1).Take(beforeEndIndex - beforeBeginIndex).Where(p => p.Value == null || p.Value == "").Select(p => p.Key).ToList();
                foreach(var empty in innerEmpty) {
                    if(empty < label.EndTime) {
                        _labelBorders[empty] = label.LabelText;
                    }
                }
                if(endLabel == "" || endLabel == null) {
                    _labelBorders[label.EndTime] = endLabel;
                }
                if(beforeBeginIndex == -1 || _labelBorders.Values[beforeBeginIndex] == "" || _labelBorders.Values[beforeBeginIndex] == null) {
                    _labelBorders[label.BeginTime] = label.LabelText;
                }
            } else {
                if(beforeEndIndex != beforeBeginIndex) {
                    var removes = _labelBorders.Keys.Skip(beforeBeginIndex + 1).Take(beforeEndIndex - beforeBeginIndex).ToList();
                    foreach(var rm in removes) {
                        _labelBorders.Remove(rm);
                    }
                }
                _labelBorders[label.BeginTime] = label.LabelText;
                _labelBorders[label.EndTime] = endLabel;
            }
        }

        /// <summary>
        /// 秒数を"分:秒"の形で取得します
        /// </summary>
        /// <param title="time">秒数</param>
        /// <returns></returns>
        public static string ConvertTimeToString(decimal time) {
            if(time < 0)
                throw new ArgumentOutOfRangeException("time", "'time' cannot be negative");
            int minutes = (int)(time / 60);
            return minutes.ToString() + ":" + (time % 60).ToString();
        }

        /// <summary>
        /// "分:秒"の文字列を秒数の形で取得します
        /// </summary>
        /// <param title="time">文字列</param>
        /// <returns></returns>
        public static bool TryConvertStringToTime(string timeString, out decimal time) {
            time = 0;
            Regex regex = new Regex(@"(?:(\d+)\s*:)?\s*(\d+)(\.\d+)?");
            Match match = regex.Match(timeString);
            if(!match.Success) {
                return false;
            }
            if(!decimal.TryParse(match.Groups[2].Value, out time))
                return false;
            if(match.Groups[1].Success) {
                // 分
                decimal minutes;
                if(!decimal.TryParse(match.Groups[1].Value, out minutes))
                    return false;
                time += minutes * 60;
            }
            if(match.Groups[3].Success) {
                // 秒未満
                decimal second;
                if(!decimal.TryParse("0" + match.Groups[3].Value, out second))
                    return false;
                time += second;
            }
            return true;
        }

        /// <summary>
        /// ラベルシーケンスをCSV形式で出力します。
        /// </summary>
        /// <param title="writer"></param>
        public void WriteTo(TextWriter writer) {
            foreach(var label in this.EnumerateLabels()) {
                if(!label.IsEmptyLabel) {
                    writer.WriteLine(label.ToString());
                }
            }
        }

        /// <summary>
        /// CSV形式からラベルシーケンスを生成します。
        /// </summary>
        /// <param title="reader"></param>
        public void ReadFrom(Stream stream) {
            using(CSVReader reader = new CSVReader(stream)) {
                while(!reader.EndOfStream) {
                    string[] items = reader.ReadValues();
                    if(items.Length < 3)
                        continue;
                    decimal beginTime, endTime;
                    if(!TryConvertStringToTime(items[0], out beginTime))
                        continue;
                    if(!TryConvertStringToTime(items[1], out endTime))
                        continue;
                    if(beginTime < endTime)
                        SetLabel(beginTime, endTime, items[2].Trim(), true);
                }
            }
        }

        /// <summary>
        /// 現在のオブジェクトからPlugin.DataRowオブジェクトを生成します。
        /// </summary>
        /// <param title="host">生成先のPlugin.IPluginHost</param>
        /// <returns>新しいPlugin.DataRow</returns>
        public Plugin.DataRow CreateDataRow(Plugin.IPluginHost host) {
            IList<string> labelList = GetLabelTexts();
            // 自動色付け
            Dictionary<string, Color> colors = new Dictionary<string, Color>();
            for(int i = 0; i < labelList.Count; i++) {
                colors[labelList[i]] = ColorEx.ColorFromHSV((float)(Math.PI * 2 * i / labelList.Count), 0.1f, (i % 2) == 0 ? 1f : 0.9f);
            }
            return CreateDataRow(host, colors);
        }

        /// <summary>
        /// 現在のオブジェクトからPlugin.DataRowオブジェクトを生成します。
        /// </summary>
        /// <param title="host">生成先のPlugin.IPluginHost</param>
        /// <returns>新しいPlugin.DataRow</returns>
        public Plugin.DataRow CreateDataRow(Plugin.IPluginHost host, Dictionary<string, Color> colorDictionary) {
            Plugin.DataRow datarow = new Plugin.DataRow(host);
            IList<string> labelList = GetLabelTexts();
            // プロパティ生成
            foreach(var labelText in labelList) {
                if(labelText != null && labelText != "") {
                    Color fillColor = Color.White;
                    if(colorDictionary.ContainsKey(labelText))
                        fillColor = colorDictionary[labelText];
                    datarow.Properties.Add(new Plugin.RowProperty(labelText, datarow, fillColor));
                }
            }
            foreach(var label in this.EnumerateLabels()) {
                Color fillColor = Color.White;
                if(colorDictionary.ContainsKey(label.LabelText))
                    fillColor = colorDictionary[label.LabelText];
                datarow.DataLabels.Add(new Plugin.DataLabel(host, datarow, (double)label.BeginTime, (double)label.EndTime, label.LabelText, fillColor));
            }
            return datarow;
        }

        /// <summary>
        /// Plugin.DataRowからラベルデータを読み込みます．
        /// </summary>
        /// <param name="datarow"></param>
        public void FromDataRow(Plugin.DataRow datarow) {
            _labelBorders.Clear();
            _duration = 0;
            var labels = datarow.DataLabels;
            foreach(var label in labels) {
                if(label.StartTime < label.EndTime) {
                    SetLabel((decimal)label.StartTime, (decimal)label.EndTime, label.Value, true);
                }
            }
        }

        /// <summary>
        /// 同一ラベル間のラベル境界を除去します．
        /// </summary>
        public void RemoveBorderSameAsPrevious() {
            string prevLabel = "";
            bool removeLast = false;
            decimal last = 0;
            foreach(var label in _labelBorders.ToList()) {
                if(label.Value == prevLabel) {
                    _labelBorders.Remove(label.Key);
                    removeLast = true;
                } else {
                    removeLast = false;
                }
                last = label.Key;
                prevLabel = label.Value;
            }
            if(removeLast && prevLabel != "") {
                // 最後の境界が消えるとラベルが減るので
                _labelBorders[last] = "";
            }
        }

        /// <summary>
        /// ラベルのクリッピングの方式
        /// </summary>
        public enum ClipLabelsMode {
            /// <summary>
            /// 境界でラベルを区切ります
            /// </summary>
            CutBorder,
            /// <summary>
            /// 境界上にあるラベルを含めます
            /// </summary>
            IncludeBorder,
            /// <summary>
            /// 境界上にあるラベルを除きます
            /// </summary>
            ExcludeBorder,
        }

        /// <summary>
        /// ラベル列から一定の期間のラベルを取得します．
        /// </summary>
        /// <param name="lower">期間の開始時間</param>
        /// <param name="upper">期間の終了時間</param>
        /// <param name="mode">クリップ方法</param>
        /// <returns></returns>
        public List<ICSLabel> ClipLabelsBetween(decimal beginTime, decimal endTime, ClipLabelsMode mode) {
            List<ICSLabel> ret = new List<ICSLabel>();
            int beginIndex = this.GetLabelIndexAt(beginTime);
            int endIndex = this.GetLabelIndexAt(endTime);
            if(beginIndex == -1)
                beginIndex = 0;
            for(int i = beginIndex; i <= endIndex; i++) {
                var label = this.GetLabelByIndex(i);
                if(label == null)
                    continue;
                if(label.BeginTime < beginTime) {
                    switch(mode) {
                    case ClipLabelsMode.CutBorder:
                        if(beginTime >= label.EndTime)
                            continue;
                        label = label.ChangeTime(beginTime, label.EndTime);
                        break;
                    case ClipLabelsMode.ExcludeBorder:
                        continue;
                    }
                }
                if(label.EndTime > endTime) {
                    switch(mode) {
                    case ClipLabelsMode.CutBorder:
                        if(endTime <= label.BeginTime)
                            continue;
                        label = label.ChangeTime(label.BeginTime, endTime);
                        break;
                    case ClipLabelsMode.ExcludeBorder:
                        continue;
                    }
                }
                ret.Add(label);
            }
            return ret;
        }
    }

}
