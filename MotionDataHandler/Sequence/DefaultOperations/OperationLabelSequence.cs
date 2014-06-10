using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MotionDataHandler.Sequence.DefaultOperations {
    using Operation;
    using Misc;
    public class OperationLabelMerge : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            // 実引数取得
            SequenceData anotherSequence = (args[0] as SequenceSingleSelectParameter).Value;
            bool appendTitle = ((BooleanParameter)args[1]).Value;
            // ラベル化
            ICSLabelSequence one = env.SelectedSequence.GetLabelSequence();
            ICSLabelSequence another = anotherSequence.GetLabelSequence();
            if(appendTitle) {
                // タイトル付与
                foreach(var label in one.EnumerateLabels().ToList()) {
                    one.SetLabel(label.BeginTime, label.EndTime, env.SelectedSequence.Title + "-" + label.LabelText);
                }
            }
            // 他方のラベルを追加
            foreach(var label in another.EnumerateLabels()) {
                if(appendTitle) {
                    one.SetLabel(label.BeginTime, label.EndTime, anotherSequence.Title + "-" + label.LabelText, true);
                } else {
                    one.SetLabel(label, true);
                }
            }
            // SequenceDataとして返す
            return SequenceData.FromLabelSequence(one, env.SelectedSequence.Title + "-merge-" + anotherSequence.Title);
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            // 仮引数作成
            var label = new SequenceSingleSelectParameter("Target", viewer => (viewer.Type & SequenceType.Label) != 0);
            var append = new BooleanParameter("Append Title to label");
            return new ProcParam<SequenceProcEnv>[] { label, append };
        }

        public string GetDescription() {
            return "ラベルをマージします";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Label; }
        }

        public string GetTitle() {
            return "マージ / Merge Label";
        }

        public bool ReplacesInternalData {
            get { return false; }
        }

        public string GetCommandName() {
            return "MergeLabel";
        }

        #endregion
    }

    public class OperationLabelIntersect : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            var operand = (args[0] as SequenceSingleSelectParameter).Value;
            var sameOnly = ((BooleanParameter)args[1]).Value;
            var one = env.SelectedSequence.GetLabelSequence();
            var another = operand.GetLabelSequence();
            var l1 = one.EnumerateLabels().GetEnumerator();
            var l2 = another.EnumerateLabels().GetEnumerator();
            if(!(l1.MoveNext() && l2.MoveNext()))
                return null;
            ICSLabelSequence tmp = new ICSLabelSequence();
            while(true) {
                // (not A) or B === A => B
                if((!sameOnly || l1.Current.LabelText == l2.Current.LabelText) && !l1.Current.IsEmptyLabel && !l2.Current.IsEmptyLabel) {
                    decimal begin = Math.Max(l1.Current.BeginTime, l2.Current.BeginTime);
                    decimal end = Math.Min(l1.Current.EndTime, l2.Current.EndTime);
                    if(end > begin) {
                        tmp.SetLabel(begin, end, l1.Current.LabelText);
                    }
                }
                if(l1.Current.EndTime < l2.Current.EndTime) {
                    if(!l1.MoveNext())
                        break;
                } else {
                    if(!l2.MoveNext())
                        break;
                }
            }

            return SequenceData.FromLabelSequence(tmp, env.SelectedSequence.Title + "-intersect-" + operand.Title);
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            List<ProcParam<SequenceProcEnv>> ret = new List<ProcParam<SequenceProcEnv>>();
            var label = new SequenceSingleSelectParameter("Target");
            ret.Add(label);
            var sameOnly = new BooleanParameter("Same Label Text Only");
            sameOnly.Value = true;
            ret.Add(sameOnly);
            return ret;
        }

        public string GetDescription() {
            return "ラベルの共通部分を取得します";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Label; }
        }

        public string GetTitle() {
            return "共通部分 / Intersect Label";
        }

        public bool ReplacesInternalData {
            get { return false; }
        }

        public string GetCommandName() {
            return "IntersectLabel";
        }

        #endregion
    }
    public class OperationRemoveBorderSameAsPrevious : ISequenceOperation {

        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            env.SelectedSequence.Borders.RemoveBorderSameToPrevious();
            return null;
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            return new ProcParam<SequenceProcEnv>[0];
        }

        public string GetDescription() {
            return "前後のラベル名が等しいBorderを取り除きます";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.NumericLabel; }
        }

        public string GetTitle() {
            return "前後が同じラベルの境界を取り除く / Remove ineffective border";
        }

        public bool ReplacesInternalData {
            get { return true; }
        }

        public string GetCommandName() {
            return "RemoveIneffectiveLabelBorder";
        }

        #endregion
    }
    public class OperationLabelRename : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            LabelReplaceParameter replace = args[0] as LabelReplaceParameter;
            env.SelectedSequence.Borders.ReplaceBorderName(replace.Value);
            return null;
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            List<ProcParam<SequenceProcEnv>> ret = new List<ProcParam<SequenceProcEnv>>();
            ret.Add(new LabelReplaceParameter("Replacement"));
            return ret;
        }

        public string GetTitle() {
            return "ラベル名置換 / Rename Label Text";
        }

        public string GetDescription() {
            return "ラベル名を一斉に置き換えます。";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.NumericLabel; }
        }

        public bool ReplacesInternalData {
            get { return true; }
        }
        public string GetCommandName() {
            return "RenameLabel";
        }

        #endregion
    }

    public class OperationLabelToNDimensionalBinarySequence : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            ICSLabelSequence labelSequence = env.SelectedSequence.GetLabelSequence();
            if(labelSequence == null) {
                MessageBox.Show("ラベルが空でした", this.GetType().ToString());
                return null;
            }
            if(env.Controller.WholeEndTime > 0) {
                labelSequence.SetLabel(0, env.Controller.WholeEndTime, "", true);
            }
            Dictionary<string, int> indexInfo = new Dictionary<string, int>();
            var labelTexts = labelSequence.GetLabelTexts(true);
            TimeSeriesValues sequence = new TimeSeriesValues(labelTexts);
            int count = 0;
            foreach(var labelText in labelTexts) {
                indexInfo[labelText] = count;
                count++;
            }
            sequence.SetValue(0, new decimal?[count]);
            sequence.SetValue(labelSequence.Duration, new decimal?[count]);
            foreach(var label in labelSequence.EnumerateLabels()) {
                var value = labelTexts.Select(x => (decimal?)0).ToArray();
                value[indexInfo[label.LabelText]] = 1;
                sequence.SetValue(label.BeginTime, value);
            }
            SequenceData ret = new SequenceData(sequence, null, PathEx.GiveName("label", env.SelectedSequence.Title));
            ret.Type = SequenceType.Numeric;
            return ret;
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            return new ProcParam<SequenceProcEnv>[0];
        }

        public string GetTitle() {
            return "ベクトル化 / Label to N-Dimensional Binary Sequence";
        }

        public string GetDescription() {
            return "ラベルをn次元の0/1の値のシーケンスとして出力します。";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Label; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }
        public string GetCommandName() {
            return "LabelToVector";
        }

        #endregion
    }
    public class OperationLabelToNumberSequence : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            ICSLabelSequence labelSequence = env.SelectedSequence.GetLabelSequence();
            if(labelSequence == null) {
                MessageBox.Show("ラベルが空でした", this.GetType().ToString());
                return null;
            }
            if(env.Controller.WholeEndTime > 0) {
                labelSequence.SetLabel(0, env.Controller.WholeEndTime, "", true);
            }
            TimeSeriesValues sequence = new TimeSeriesValues("value");
            Dictionary<string, int> indexInfo = new Dictionary<string, int>();
            var labelTexts = labelSequence.GetLabelTexts(true);
            int count = 0;
            foreach(var labelText in labelTexts) {
                indexInfo[labelText] = count;
                count++;
            }
            sequence.SetValue(0, new decimal?[1]);
            sequence.SetValue(labelSequence.Duration, new decimal?[1]);
            foreach(var label in labelSequence.EnumerateLabels()) {
                var value = new decimal?[1] { indexInfo[label.LabelText] };
                sequence.SetValue(label.BeginTime, value);
            }
            SequenceData ret = new SequenceData(sequence, null, PathEx.GiveName("labelNum", env.SelectedSequence.Title));
            ret.Type = SequenceType.Numeric;
            return ret;
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            return new ProcParam<SequenceProcEnv>[0];
        }

        public string GetTitle() {
            return "値化 / Label to Number Sequence";
        }

        public string GetDescription() {
            return "ラベルを整数に置き換えた値のシーケンスを出力します";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Label; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }
        public string GetCommandName() {
            return "LabelToNumber";
        }

        #endregion
    }

    public class OperationLabelExtract : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            var labels = ((LabelSelectParameter)args[0]);
            SequenceData ret = new SequenceData(new TimeSeriesValues(env.SelectedSequence.Values), new LabelingBorders(env.SelectedSequence.Borders), PathEx.GiveName("extract", env.SelectedSequence.Title));

            ret.Type = SequenceType.Label;
            if(!labels.Value.Contains(ret.Borders.DefaultName)) {
                env.SelectedSequence.Borders.DefaultName = "";
            }
            foreach(var pair in ret.Borders.Enumerate().ToList()) {
                if(!labels.Value.Contains(pair.Value)) {
                    ret.Borders.SetBorder(pair.Key, "");
                }
            }
            return ret;
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            List<ProcParam<SequenceProcEnv>> ret = new List<ProcParam<SequenceProcEnv>>();
            ret.Add(new LabelSelectParameter("Extract Target", true));
            return ret;
        }

        public string GetTitle() {
            return "選択抽出 / Extract Label by Name";
        }

        public string GetDescription() {
            return "選択されたラベル名を持つラベルのみを抽出します";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Label; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }
        public string GetCommandName() {
            return "ExtractLabelByName";
        }

        #endregion
    }

    public class OperationLabelExtend : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            var border = ((LabelSelectParameter)args[0]).Value;
            var left = ((NumberParameter)args[1]).Value;
            var right = ((NumberParameter)args[2]).Value;
            var percent = ((BooleanParameter)args[3]).Value;
            var withEmpty = ((BooleanParameter)args[4]).Value;
            ICSLabelSequence current = env.SelectedSequence.GetLabelSequence();
            ICSLabelSequence ret = env.SelectedSequence.GetLabelSequence();
            var currentLabels = current.EnumerateLabels().ToList();
            for(int i = 0; i < currentLabels.Count; i++) {
                if(border.Contains(currentLabels[i].LabelText)) {
                    decimal begin, end;
                    if(percent) {
                        begin = currentLabels[i].BeginTime - left * currentLabels[i].Duration / 100;
                        end = currentLabels[i].EndTime + right * currentLabels[i].Duration / 100;
                    } else {
                        begin = currentLabels[i].BeginTime - left;
                        end = currentLabels[i].EndTime + right;
                    }
                    if(begin > end) {
                        begin += (end - begin) / 2;
                        end = begin;
                    }
                    if(begin < end) {
                        ret.SetLabel(begin, end, currentLabels[i].LabelText);
                    }
                    if(begin > currentLabels[i].BeginTime) {
                        string prev = "";
                        if(i > 0 && !withEmpty) {
                            prev = currentLabels[i - 1].LabelText;
                        }
                        ret.SetLabel(currentLabels[i].BeginTime, begin, prev);
                    }
                    if(end < currentLabels[i].EndTime) {
                        string post = "";
                        if(i < currentLabels.Count - 1 && !withEmpty) {
                            post = currentLabels[i + 1].LabelText;
                        }
                        ret.SetLabel(end, currentLabels[i].EndTime, post);
                    }
                }
            }
            return SequenceData.FromLabelSequence(ret, PathEx.GiveName("extend", env.SelectedSequence.Title));
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            LabelSelectParameter border = new LabelSelectParameter("Target Label", true);
            NumberParameter left = new NumberParameter("Extend second before Begin", -1000, 1000, 3);
            NumberParameter right = new NumberParameter("Extend second after End", -1000, 1000, 3);
            BooleanParameter percent = new BooleanParameter("Values as Percentage of Label length");
            BooleanParameter withEmpty = new BooleanParameter("Fill with Empty label when shrink");
            return new ProcParam<SequenceProcEnv>[] { border, left, right, percent, withEmpty };
        }

        public string GetTitle() {
            return "伸縮 / Extend label";
        }

        public string GetDescription() {
            return "ラベルの幅を伸縮します";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Label; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }
        public string GetCommandName() {
            return "ExtendLabel";
        }

        #endregion
    }

    public class OperationExtractEmptyLabel : ISequenceOperation {

        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            int check = ((SingleSelectParameter)args[0]).Value;
            ICSLabelSequence current = env.SelectedSequence.GetLabelSequence();
            ICSLabelSequence ret = new ICSLabelSequence();
            ICSLabel prev = null;
            foreach(var label in current.EnumerateLabels()) {
                switch(check) {
                case 0:
                    if(label.IsEmptyLabel) {
                        ret.SetLabel(label.BeginTime, label.EndTime, "On");
                    }
                    break;
                case 1:
                    if(label.IsEmptyLabel && prev != null) {
                        ret.SetLabel(label.BeginTime, label.EndTime, prev.LabelText);
                    }
                    break;
                case 2:
                    if(prev != null && prev.IsEmptyLabel) {
                        ret.SetLabel(prev.BeginTime, prev.EndTime, label.LabelText);
                    }
                    break;
                }
                prev = label;
            }
            return SequenceData.FromLabelSequence(ret, PathEx.GiveName("Empty-of", env.SelectedSequence.Title));
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            var check = new SingleSelectParameter("抽出結果", new[] { "「On」ラベル", "直前のラベル名", "直後のラベル名" });
            return new[] { check };
        }

        public string GetTitle() {
            return "空ラベル抽出 / Extract Empty Label";
        }

        public string GetDescription() {
            return "ラベル列の空のラベルの部分を抽出して出力します。";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Label; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }
        public string GetCommandName() {
            return "ExtractEmptyLabel";
        }
        #endregion
    }

    public class OperationRemoveLabelByLength : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            var target = ((LabelSelectParameter)args[0]).Value;
            var length = ((NumberParameter)args[1]).Value;
            var action = ((SingleSelectParameter)args[2]).Value;
            var border = ((BooleanParameter)args[3]).Value;
            var glue = ((SingleSelectParameter)args[4]).Value;
            ICSLabelSequence ret = env.SelectedSequence.GetLabelSequence();
            var names = ret.GetLabelTexts();
            string tmpLabel = "tmpToRemove";
            while(names.Contains(tmpLabel)) {
                tmpLabel += "-";
            }
            foreach(var label in ret.EnumerateLabels().Where(l => target.Contains(l.LabelText)).ToList()) {
                if(action == 0) {
                    if(label.Duration < length || (border && label.Duration == length)) {
                        ret.SetLabel(label.BeginTime, label.EndTime, tmpLabel);
                    }
                } else {
                    if(label.Duration > length || (border && label.Duration == length)) {
                        ret.SetLabel(label.BeginTime, label.EndTime, tmpLabel);
                    }
                }
            }
            ret.RemoveBorderSameAsPrevious();
            var labels = ret.EnumerateLabels().ToList();
            for(int i = 0; i < labels.Count; i++) {
                if(labels[i].LabelText == tmpLabel) {
                    string to = "";
                    switch(glue) {
                    case 1:
                        if(i > 0 && i < labels.Count - 1) {
                            if(labels[i - 1].LabelText == labels[i + 1].LabelText) {
                                to = labels[i - 1].LabelText;
                            }
                        }
                        break;
                    case 2:
                        if(i > 0) {
                            to = labels[i - 1].LabelText;
                        }
                        break;
                    case 3:
                        if(i < labels.Count - 1) {
                            to = labels[i + 1].LabelText;
                        }
                        break;
                    }
                    ret.SetLabel(labels[i].BeginTime, labels[i].EndTime, to);
                }
            }
            ret.RemoveBorderSameAsPrevious();
            return SequenceData.FromLabelSequence(ret, PathEx.GiveName("Filtered", env.SelectedSequence.Title, (action == 0 ? "Lt" : "Gt") + length.ToString()));
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            var target = new LabelSelectParameter("対象のラベル", true);
            var length = new NumberParameter("秒数", 0, 10000, 3);
            var action = new SingleSelectParameter("大小", new[] { "「秒数」より短いものを置換", "「秒数」より長いものを置換" });
            var border = new BooleanParameter("「秒数」に一致するものを置換");
            var glue = new SingleSelectParameter("置換結果", new[] { "空ラベル", "前後のラベルが等しければそのラベル", "直前のラベル", "直後のラベル" });
            return new ProcParam<SequenceProcEnv>[] { target, length, action, border, glue };
        }

        public string GetTitle() {
            return "長さによるフィルタ / Replace Labels by Length";
        }

        public string GetDescription() {
            return "ラベルを長さに応じて置換します";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Label; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }
        public string GetCommandName() {
            return "ReplaceLabelByLength";
        }

        #endregion
    }

    public class OperationExtractLabelContaining : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            var another = ((SequenceSingleSelectParameter)args[0]).Value;
            var labels = ((LabelSelectParameter)args[1]).Value;
            var row = another.GetLabelSequence();
            var current = env.SelectedSequence.GetLabelSequence();
            var ret = new ICSLabelSequence();
            var l1 = current.EnumerateLabels().GetEnumerator();
            var l2 = row.EnumerateLabels().GetEnumerator();
            if(l1.MoveNext() && l2.MoveNext()) {
                while(true) {
                    if(labels.Contains(l2.Current.LabelText)) {
                        var begin1 = l1.Current.BeginTime <= l2.Current.BeginTime && l2.Current.BeginTime < l1.Current.EndTime;
                        var end1 = l1.Current.BeginTime <= l2.Current.EndTime && l2.Current.EndTime < l1.Current.EndTime;
                        var begin2 = l2.Current.BeginTime <= l1.Current.BeginTime && l1.Current.BeginTime < l2.Current.EndTime;
                        var end2 = l2.Current.BeginTime <= l1.Current.EndTime && l1.Current.EndTime < l2.Current.EndTime;
                        if(begin1 || end1 || begin2 || end2) {
                            ret.SetLabel(l1.Current);
                        }
                    }
                    if(l1.Current.EndTime < l2.Current.EndTime) {
                        if(!l1.MoveNext())
                            break;
                    } else {
                        if(!l2.MoveNext())
                            break;
                    }
                }
            }
            return SequenceData.FromLabelSequence(ret, PathEx.GiveName("Contains", env.SelectedSequence.Title, another.Title));
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            var row = new SequenceSingleSelectParameter("Sequence for Target Label", x => x.Type != SequenceType.Numeric);
            var labels = new LabelSelectParameter("Target Labels", true, row);
            return new ProcParam<SequenceProcEnv>[] { row, labels };
        }

        public string GetTitle() {
            return "共通部分を含む / Extract Label by labels containing";
        }

        public string GetDescription() {
            return "現在の行のラベルのうち、選択されたラベルを含んでいるものを抽出します";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Label; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }
        public string GetCommandName() {
            return "LabelContainingIntersection";
        }
        #endregion
    }

    public class OperationLabelDirectProductPerSelected : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            var rows = ((SequenceMultiSelectParameter)args[0]).Value;
            var others = rows.Select(p => p.GetLabelSequence()).ToList();
            var sep = (StringParameter)args[1];
            var multi = (BooleanParameter)args[2];
            var useEmpty = (BooleanParameter)args[3];
            if(others.Count == 0)
                return null;
            var current = env.SelectedSequence.GetLabelSequence();
            var ret = new ICSLabelSequence();
            foreach(var label in current.EnumerateLabels()) {
                if(label.LabelText == "" && !useEmpty.Value)
                    continue;
                var clips = others.Select(r => r.ClipLabelsBetween(label.BeginTime, label.EndTime, ICSLabelSequence.ClipLabelsMode.CutBorder));
                string name = label.LabelText;
                foreach(var clip in clips) {
                    var texts = clip.Where(c => useEmpty.Value || c.LabelText != "").ToList();
                    if(multi.Value) {
                        foreach(var text in texts) {
                            name += sep.Value + text.LabelText;
                        }
                    } else {
                        if(texts.Count > 0) {
                            name += sep.Value + texts[0].LabelText;
                        }
                    }
                    ret.SetLabel(label.ChangeText(name));
                }
            }
            return SequenceData.FromLabelSequence(ret, PathEx.GiveName("ContainsProduct", new string[] { env.SelectedSequence.Title }.Union(rows.Select(p => p.Title))));
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            var row = new SequenceMultiSelectParameter("Sequence for Target Label", x => x.Type != SequenceType.Numeric);
            var sep = new StringParameter("Separator");
            sep.Value = "+";
            var multi = new BooleanParameter("Allow Multi-Label per one row");
            var useEmpty = new BooleanParameter("Use Empty Label");
            return new ProcParam<SequenceProcEnv>[] { row, sep, multi, useEmpty };
        }

        public string GetTitle() {
            return "選択ラベル列に対する直積 / Label Direct Product for Selected";
        }

        public string GetDescription() {
            return "現在の行のそれぞれのラベルに対し、他のラベル列との直積を求めます";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Label; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }
        public string GetCommandName() {
            return "LabelDirectProduct2";
        }

        #endregion
    }

    public class OperationAppendAdjacentLabelName : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            var glue = ((StringParameter)args[0]).Value;
            var type = ((SingleSelectParameter)args[1]).Value;
            var skip = ((BooleanParameter)args[2]).Value;
            var labelSequence = env.SelectedSequence.GetLabelSequence();
            ICSLabelSequence ret = new ICSLabelSequence();
            ICSLabel prev = null;
            foreach(var label in labelSequence.EnumerateLabels()) {
                if(skip && label.IsEmptyLabel)
                    continue;
                string name = label.LabelText;
                if(prev != null) {
                    name = prev.LabelText + glue + label.LabelText;
                }
                if(prev != null || type == 0) {
                    if(type == 0) {
                        ret.SetLabel(label.BeginTime, label.EndTime, name);
                    } else {
                        ret.SetLabel(prev.BeginTime, prev.EndTime, name);
                    }
                }
                prev = label;
            }
            return SequenceData.FromLabelSequence(ret, PathEx.GiveName("AppendPrev", env.SelectedSequence.Title));
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            var glue = new StringParameter("Text to Concat");
            glue.Value = "-TO-";
            var type = new SingleSelectParameter("Type", new[] { "Previous-Current", "Current-Next" });
            var skip = new BooleanParameter("Skip Empty Label");
            return new ProcParam<SequenceProcEnv>[] { glue, type, skip };
        }

        public string GetTitle() {
            return "隣接ラベル名連結 / Append Adjacent Label Name";
        }

        public string GetDescription() {
            return "ラベルに前後のラベルのテキストをつなげます";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Label; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }
        public string GetCommandName() {
            return "AppendAdjacentLabel";
        }
        #endregion
    }
    public class OperationPrecisionRecall : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            var mode = args[0] as SingleSelectParameter;
            var threshold = args[1] as NumberParameter;
            var correct = args[2] as SequenceSingleSelectParameter;
            var correctSequence = correct.Value;
            ICSLabelSequence ret = new ICSLabelSequence();
            decimal correctCount = 0, obtainCount = 0, matchCount = 0;
            switch(mode.Value) {
            case 0:
                var common = TimeSeriesValuesCalculation.MergeTimeList(new[] { env.SelectedSequence.Values, correctSequence.Values }).ToList();
                for(int i = 0; i < common.Count - 1; i++) {
                    string o = env.SelectedSequence.GetLabelAt(common[i]);
                    string c = correctSequence.GetLabelAt(common[i]);
                    if(o == "" && c == "")
                        continue;
                    decimal interval = common[i + 1] - common[i];
                    if(o != "")
                        obtainCount += interval;
                    if(c != "")
                        correctCount += interval;
                    if(o == c)
                        matchCount += interval;
                    ret.SetLabel(common[i], common[i + 1], o == c ? "Match" : o == "" ? "False Negative" : c == "" ? "False Positive" : "Mismatch");
                }
                break;
            case 1:
            case 2:
                SequenceData main, sub;
                if(mode.Value == 1) {
                    main = correctSequence;
                    sub = env.SelectedSequence;
                } else {
                    main = env.SelectedSequence;
                    sub = correctSequence;
                }
                var subLabel = sub.GetLabelSequence();
                foreach(var label in main.GetLabelSequence().EnumerateLabels()) {
                    decimal denom = label.Duration;
                    if(label.LabelText != "") {
                        var clip = subLabel.ClipLabelsBetween(label.BeginTime, label.EndTime, ICSLabelSequence.ClipLabelsMode.CutBorder);
                        decimal numer = clip.Where(l => l.LabelText == label.LabelText).Select(l => l.Duration).Sum();
                        if(numer >= denom * threshold.Value) {
                            matchCount++;
                            ret.SetLabel(label.ChangeText("Match"));
                        } else {
                            ret.SetLabel(label.ChangeText(mode.Value == 1 ? "False Negative" : "False Positive"));
                        }
                        obtainCount++;
                        correctCount++;
                    } else {
                        var clip = subLabel.ClipLabelsBetween(label.BeginTime, label.EndTime, ICSLabelSequence.ClipLabelsMode.CutBorder);
                        var missClip = clip.Where(l => l.LabelText != "").ToList();
                        var missCount = missClip.Count;
                        if(mode.Value == 1) {
                            obtainCount += missCount;
                        } else {
                            correctCount += missCount;
                        }
                        foreach(var m in missClip) {
                            ret.SetLabel(m.ChangeText(mode.Value == 1 ? "False Positive" : "False Negative"));
                        }
                    }
                }
                break;
            }
            return SequenceData.FromLabelSequence(ret, PathEx.GiveName("PrecisionRecall", env.SelectedSequence.Title, correctSequence.Title));
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            var mode = new SingleSelectParameter("Mode", new[] { "Per Time", "Per Correct Label Count", "Per Obtained Label Count" });
            var threshold = new NumberParameter("Threshold for Ratio of Label Coverage for Matching", 0.001M, 1, 3);
            threshold.Value = 0.001M;
            var correct = new SequenceSingleSelectParameter("Correct Label", (v) => v.Type != SequenceType.Numeric);
            return new ProcParam<SequenceProcEnv>[] { mode, threshold, correct };
        }

        public string GetTitle() {
            return "適合率・再現率計算 / Calculate Precision/Recall";
        }

        public string GetDescription() {
            StringBuilder ret = new StringBuilder();
            ret.AppendLine("適合率/再現率を計算します。");
            ret.AppendLine("Per Timeモードは適合率/再現率を時間の占める割合で求めます。");
            ret.AppendLine("Per Correct Label Countモードは正解ラベルごとにマッチしているかを計数します。");
            ret.AppendLine("Per Obtained Label Countモードは検出ラベルごとにマッチしているかを計数します。");
            ret.AppendLine("Threshold for Ratio of Label Coverage for Matchingの値は、");
            ret.AppendLine("モードで指定されたラベルのうち、共通部分のラベルが占める割合の閾値を指定します。");
            ret.AppendLine("値が低いほど少しの共通部分でマッチと判定します。");
            return ret.ToString();
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Label; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }
        public string GetCommandName() {
            return "CalculateLabelPrecisionRecall";
        }
        #endregion
    }

    public class OperationLabelStatistics : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            BooleanParameter includeEmpty = args[0] as BooleanParameter;
            ICSLabelSequence labelSeq = env.SelectedSequence.GetLabelSequence();
            Dictionary<string, List<decimal>> labelInfo = new Dictionary<string, List<decimal>>();
            foreach(var label in labelSeq.EnumerateLabels()) {
                if(!includeEmpty.Value && label.LabelText == "")
                    continue;
                if(!labelInfo.ContainsKey(label.LabelText)) {
                    labelInfo[label.LabelText] = new List<decimal>();
                }
                labelInfo[label.LabelText].Add(label.Duration);
            }
            decimal sumTime = labelInfo.Select(l => l.Value.Sum()).Sum();
            int sumCount = labelInfo.Select(l => l.Value.Count).Sum();
            if(sumTime == 0 || sumCount == 0) {
                MessageBox.Show("Empty Label Sequence", this.GetTitle());
            } else {
                Misc.DialogOKDataGrid dialog = new MotionDataHandler.Misc.DialogOKDataGrid();
                var columns = new DataGridViewColumn[9].Select(p => new DataGridViewTextBoxColumn()).ToArray();
                columns[0].HeaderText = "Label";
                columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                columns[1].HeaderText = "Time";
                columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                columns[1].DefaultCellStyle.Format = "0.000";
                columns[2].HeaderText = "Time[%]";
                columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                columns[2].DefaultCellStyle.Format = "0.0000";
                columns[3].HeaderText = "Count";
                columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                columns[3].DefaultCellStyle.Format = "0";
                columns[4].HeaderText = "Count[%]";
                columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                columns[4].DefaultCellStyle.Format = "0.0000";
                columns[5].HeaderText = "Min Len.";
                columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                columns[5].DefaultCellStyle.Format = "0.000";
                columns[6].HeaderText = "Max Len.";
                columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                columns[6].DefaultCellStyle.Format = "0.000";
                columns[7].HeaderText = "Avg Len.";
                columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                columns[7].DefaultCellStyle.Format = "0.000";
                columns[8].HeaderText = "Median Len.";
                columns[8].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                columns[8].DefaultCellStyle.Format = "0.000";
                dialog.DataGrid.Columns.AddRange(columns);
                foreach(var labelName in labelInfo.Keys.OrderBy(k => k)) {
                    decimal time = labelInfo[labelName].Sum();
                    int count = labelInfo[labelName].Count;
                    decimal min = labelInfo[labelName].Min();
                    decimal max = labelInfo[labelName].Max();
                    decimal avg = labelInfo[labelName].Average();
                    var ordered = labelInfo[labelName].OrderBy(x => x).ToList();
                    decimal median = ((count % 2) == 0) ? (ordered[count / 2 - 1] + ordered[count / 2]) / 2 : ordered[count / 2];
                    dialog.DataGrid.Rows.Add(labelName, time, 100M * time / sumTime, count, 100M * count / sumCount, min, max, avg, median);
                }
                dialog.ShowDialog();
            }
            return null;
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            var includeEmpty = new BooleanParameter("Include Empty Label");
            return new ProcParam<SequenceProcEnv>[] { includeEmpty };
        }

        public string GetTitle() {
            return "ラベル統計 / Label Statistics";
        }

        public string GetDescription() {
            return "ラベルの統計値を表示します";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Label; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }

        public string GetCommandName() {
            return "LabelStatistics";
        }
        #endregion
    }

    public class OperationLabelDirectProduct : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            var operand = args[0] as SequenceMultiSelectParameter;
            var sep = args[1] as StringParameter;
            var useEmpty = args[2] as BooleanParameter;
            List<SequenceData> sequences = new List<SequenceData>();
            sequences.Add(env.SelectedSequence);
            sequences.AddRange(operand.Value);
            ICSLabelSequence ret = new ICSLabelSequence();
            var timeList = TimeSeriesValuesCalculation.MergeTimeList(sequences.Select(p => p.Values)).ToList();
            if(timeList.Count != 0) {
                var lastTime = timeList.Last();
                foreach(var time in timeList) {
                    if(time < lastTime) {
                        StringBuilder tmp = new StringBuilder();
                        bool empty = true;
                        foreach(var sequence in sequences) {
                            var label = sequence.GetLabelAt(time);
                            if(label != "" || useEmpty.Value) {
                                if(!empty)
                                    tmp.Append(sep.Value);
                                tmp.Append(label);
                                empty = false;
                            }
                        }
                        ret.SetLabel(time, lastTime, tmp.ToString());
                    }
                }
            }
            return SequenceData.FromLabelSequence(ret, PathEx.GiveName("Product", sequences.Select(p => p.Title)));
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            var operand = new SequenceMultiSelectParameter("Other operands", (v) => v.Type != SequenceType.Numeric);
            var sep = new StringParameter("Separator");
            sep.Value = "+";
            var useEmpty = new BooleanParameter("Use Empty Label");
            return new ProcParam<SequenceProcEnv>[] { operand, sep, useEmpty };
        }

        public string GetTitle() {
            return "ラベル直積 / Label Direct Product";
        }

        public string GetDescription() {
            return "複数のラベル列を掛け合わせます";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Label; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }
        public string GetCommandName() {
            return "LabelDirectProduct";
        }
        #endregion
    }
    /*
    public class OperationLabelSubtract : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Call(IList<OperationParameterBase<SequenceEnvironment>> args, OperationEnvironment env) {
            throw new NotImplementedException();
        }

        public IList<OperationParameterBase<SequenceEnvironment>> GetParameters() {
            SequenceSingleSelectParameter p = new SequenceSingleSelectParameter("operand", v => (v.Type & SequenceType.Label) != 0);
            return new[] { p };
        }

        public string GetTitle() {
            throw new NotImplementedException();
        }

        public string Explain() {
            throw new NotImplementedException();
        }

        public SequenceType OperationTargetType {
            get { throw new NotImplementedException(); }
        }

        public bool CanChangeSelectedDataContent {
            get { throw new NotImplementedException(); }
        }

        public string GetCommandName() {
            throw new NotImplementedException();
        }

        #endregion
    }*/
}
