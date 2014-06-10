using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
namespace MotionDataHandler.Sequence.DefaultOperations {
    using Operation;
    using Misc;
    public class OperationMovingAverage : ISequenceOperation {

        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            var timeBefore = ((NumberParameter)args[0]).Value;
            var timeAfter = ((NumberParameter)args[1]).Value;
            var exclude = ((NumberParameter)args[2]).Value;
            var sequence = TimeSeriesValuesCalculation.TimeSmooth(env.SelectedSequence.Values, timeBefore + timeAfter, -timeAfter, exclude);
            SequenceData ret = new SequenceData(sequence, new LabelingBorders(env.SelectedSequence.Borders), PathEx.GiveName("Smooth", env.SelectedSequence.Title));
            ret.Type = SequenceType.Numeric;
            return ret;
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            var timeBefore = new NumberParameter("前方の時間範囲", 0, 1000, 3);
            timeBefore.Value = 0.1M;
            var timeAfter = new NumberParameter("後方の時間範囲", 0, 1000, 3);
            timeAfter.Value = 0.1M;
            var exclude = new NumberParameter("欠損があっても値を出力する限界値(0～1)", 0, 1, 3);
            return new[] { timeBefore, timeAfter, exclude };
        }

        public string GetTitle() {
            return "移動平均 / Moving Average of Values";
        }

        public string GetDescription() {
            return "指定された時間幅で移動平均を施した値のシーケンスを出力します。";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Numeric; }
        }


        public bool ReplacesInternalData {
            get { return false; }
        }

        public string GetCommandName() {
            return "MovingAverage";
        }

        #endregion
    }

    public class OperationToLabel : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            SequenceColumnSelectParameter column = args[0] as SequenceColumnSelectParameter;
            int tmp = env.SelectedSequence.Borders.TargetColumnIndex;
            env.SelectedSequence.Borders.TargetColumnIndex = column.Value;
            ICSLabelSequence labelSeq = env.SelectedSequence.GetLabelSequence();
            if(tmp != column.Value) {
                env.SelectedSequence.Borders.TargetColumnIndex = tmp;
            }
            return SequenceData.FromLabelSequence(labelSeq, PathEx.GiveName("LabelOf", env.SelectedSequence.Title));
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            SequenceColumnSelectParameter column = new SequenceColumnSelectParameter("Target Column");
            return new ProcParam<SequenceProcEnv>[] { column };
        }

        public string GetTitle() {
            return "ラベル化 / Get Label Sequence";
        }

        public string GetDescription() {
            return "時系列データとラベル境界からラベルを作成します";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Numeric; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }

        public string GetCommandName() {
            return "CreateLabel";
        }

        #endregion
    }

    public class OperationClipByLabel : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            var row = ((SequenceSingleSelectParameter)args[0]).Value;
            var labels = ((LabelSelectParameter)args[1]).Value;
            TimeSeriesValues newSeq = new TimeSeriesValues(env.SelectedSequence.Values.ColumnNames);
            foreach(var label in row.GetLabelSequence().EnumerateLabels()) {
                newSeq.SetValue(label.BeginTime, new decimal?[env.SelectedSequence.Values.ColumnCount]);
                newSeq.SetValue(label.EndTime, new decimal?[env.SelectedSequence.Values.ColumnCount]);
            }
            foreach(var pair in env.SelectedSequence.Values.Enumerate()) {
                if(labels.Contains(row.GetLabelAt(pair.Key))) {
                    newSeq.SetValue(pair.Key, pair.Value);
                }
            }
            SequenceData ret = new SequenceData(newSeq, new LabelingBorders(env.SelectedSequence.Borders), PathEx.GiveName("ClipBy", new string[] { env.SelectedSequence.Title }.Union(labels)));
            ret.Type = SequenceType.Numeric;
            return ret;
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            var row = new SequenceSingleSelectParameter("Source Label Row", x => x.Type != SequenceType.Numeric);
            var labels = new LabelSelectParameter("Labels where Value is to keep existing", true, row);
            return new ProcParam<SequenceProcEnv>[] { row, labels };
        }

        public string GetTitle() {
            return "ラベルでクリップ / Clip sequence by Label";
        }

        public string GetDescription() {
            return "指定されたラベルが存在するところの値を切り出します";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Numeric; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }
        public string GetCommandName() {
            return "ClipByLabel";
        }

        #endregion
    }

    public class OperationNormalizeAsVector : ISequenceOperation {

        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            TimeSeriesValues values = new TimeSeriesValues(env.SelectedSequence.Values.ColumnNames);
            foreach(var pair in env.SelectedSequence.Values.Enumerate()) {
                decimal?[] tmp = new decimal?[env.SelectedSequence.Values.ColumnCount];
                if(pair.Value.All(x => x.HasValue)) {
                    try {
                        decimal sum = pair.Value.Sum(x => x.Value * x.Value);
                        decimal sq = (decimal)Math.Sqrt((double)sum);
                        if(sq != 0) {
                            for(int i = 0; i < tmp.Length; i++) {
                                tmp[i] = pair.Value[i].Value / sq;
                            }
                        }
                    } catch(OverflowException) {
                        tmp = new decimal?[env.SelectedSequence.Values.ColumnCount];
                    }
                }
                values.SetValue(pair.Key, tmp);
            }
            SequenceData ret = new SequenceData(values, null, PathEx.GiveName("Normal", env.SelectedSequence.Title));
            return ret;
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            return new ProcParam<SequenceProcEnv>[0];
        }

        public string GetTitle() {
            return "ベクトル正規化 / Normalize As Vector";
        }

        public string GetDescription() {
            return "値のシーケンス群の各時点をベクトルとして正規化します";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Numeric; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }
        public string GetCommandName() {
            return "NormalizeAsVector";
        }

        #endregion
    }

    public class OperationNormalizeBySum : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            TimeSeriesValues values = new TimeSeriesValues(env.SelectedSequence.Values.ColumnNames);
            foreach(var pair in env.SelectedSequence.Values.Enumerate()) {
                decimal?[] tmp = new decimal?[env.SelectedSequence.Values.ColumnCount];
                if(pair.Value.All(x => x.HasValue)) {
                    try {
                        decimal sum = pair.Value.Sum(x => x.Value);
                        if(sum != 0) {
                            for(int i = 0; i < tmp.Length; i++) {
                                tmp[i] = pair.Value[i].Value / sum;
                            }
                        }
                    } catch(OverflowException) {
                        tmp = new decimal?[env.SelectedSequence.Values.ColumnCount];
                    }
                }
                values.SetValue(pair.Key, tmp);
            }
            SequenceData ret = new SequenceData(values, null, PathEx.GiveName("Normal", env.SelectedSequence.Title));
            ret.Type = SequenceType.Numeric;
            return ret;
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            return new ProcParam<SequenceProcEnv>[0];
        }

        public string GetTitle() {
            return "和による正規化 / Normalize By Sum";
        }

        public string GetDescription() {
            return "値のシーケンス群の各時点の和が1になるように正規化します";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Numeric; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }
        public string GetCommandName() {
            return "NormalizeBySum";
        }

        #endregion
    }

    public class OperationStatistics : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            StringBuilder build = new StringBuilder();
            build.AppendFormat("Data: {0}", env.SelectedSequence.Title);
            build.AppendLine();
            build.AppendFormat(" Total Count: {0}", env.SelectedSequence.Values.SequenceLength);
            build.AppendLine();
            build.AppendFormat(" Column Length: {0}", env.SelectedSequence.Values.ColumnCount);
            build.AppendLine();
            for(int i = 0; i < env.SelectedSequence.Values.ColumnCount; i++) {
                build.AppendFormat("Sequence: {0}", env.SelectedSequence.Values.ColumnNames[i]);
                build.AppendLine();
                int count = 0;
                try {
                    decimal sum = 0M;
                    decimal max = decimal.MinValue, min = decimal.MaxValue;
                    foreach(var values in env.SelectedSequence.Values.Enumerate()) {
                        if(values.Value[i].HasValue) {
                            count++;
                            decimal v = values.Value[i].Value;
                            sum += v;
                            if(max < v)
                                max = v;
                            if(v < min)
                                min = v;
                        }
                    }
                    if(count == 0) {
                        build.AppendLine(" No data");
                        continue;
                    }
                    build.AppendFormat(" Count: {0}", count);
                    build.AppendLine();
                    build.AppendFormat(" Max: {0}", max);
                    build.AppendLine();
                    build.AppendFormat(" Min: {0}", min);
                    build.AppendLine();
                    decimal mean = sum / count;
                    build.AppendFormat(" Mean: {0}", mean);
                    build.AppendLine();
                    decimal sum2 = 0M;

                    foreach(var values in env.SelectedSequence.Values.Enumerate()) {
                        if(values.Value[i].HasValue) {
                            decimal tmp = values.Value[i].Value - mean;
                            sum2 += tmp * tmp;
                        }
                    }
                    decimal variance = sum2 / count;
                    build.AppendFormat(" Variance: {0}", variance);
                    build.AppendLine();

                    try {
                        decimal stddev = (decimal)Math.Sqrt((double)variance);
                        build.AppendFormat(" StdDev: {0}", stddev);
                        build.AppendLine();
                    } catch(OverflowException) {
                        build.AppendLine(" StdDev: Overflow");
                        build.AppendLine();
                    }
                } catch(OverflowException) { }
            }
            DialogOKMessage dialog = new DialogOKMessage(build.ToString(), "Statistics");
            dialog.ShowDialog();
            return null;
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            return new ProcParam<SequenceProcEnv>[0];
        }

        public string GetTitle() {
            return "統計値表示 / Sequence Statistics";
        }

        public string GetDescription() {
            return "統計データを取得します";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Numeric; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }
        public string GetCommandName() {
            return "ValueStatistics";
        }
        #endregion
    }

    public class OperationResampling : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            decimal hz = ((NumberParameter)args[0]).Value;
            SequenceData ret = new SequenceData(TimeSeriesValuesCalculation.GetResampled(env.SelectedSequence.Values, 1M / hz), new LabelingBorders(env.SelectedSequence.Borders), PathEx.GiveName("Resampled", env.SelectedSequence.Title, hz.ToString()));
            ret.Type = SequenceType.Numeric;
            return ret;
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            NumberParameter hz = new NumberParameter("Sampling Frequency(Hz)", 0.01M, 10000, 2);
            hz.Value = 60M;
            return new ProcParam<SequenceProcEnv>[] { hz };
        }

        public string GetTitle() {
            return "リサンプリング / Resample sequence";
        }

        public string GetDescription() {
            return "シーケンスを一定間隔で再サンプリングします";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Numeric; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }
        public string GetCommandName() {
            return "Resample";
        }

        #endregion
    }

    public class OperationGaussian : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            var stddev = ((NumberParameter)args[0]).Value;
            var details = (int)((NumberParameter)args[1]).Value;
            var exclude = ((NumberParameter)args[2]).Value;
            var sequence = TimeSeriesValuesCalculation.Gaussian(env.SelectedSequence.Values, stddev, details, exclude);
            sequence.SetColumnNames(env.SelectedSequence.Values.ColumnNames.Select(n => "Gauss " + n).ToArray());
            SequenceData ret = new SequenceData(sequence, new LabelingBorders(env.SelectedSequence.Borders), PathEx.GiveName("Gaussian", env.SelectedSequence.Title, stddev.ToString()));
            ret.Type = SequenceType.Numeric;
            return ret;
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            var stddev = new NumberParameter("StdDev", 0, 1000, 3);
            stddev.Value = 0.05M;
            var details = new NumberParameter("Detail", 1, 100, 0);
            details.Value = 10;
            var exclude = new NumberParameter("Limit rate to contain empty values", 0, 1, 3);
            return new[] { stddev, details, exclude };
        }

        public string GetTitle() {
            return "ガウスフィルタ / Gaussian Filter";
        }

        public string GetDescription() {
            return "Gaussianフィルタを施します";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Numeric; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }
        public string GetCommandName() {
            return "Gaussian";
        }
        #endregion
    }

    public class OperationArithmeticOperation : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            var arith = args[0] as SingleSelectParameter;
            var operand = args[1] as SequenceMultiSelectParameter;

            string opText = "null";
            TimeSeriesValues result = env.SelectedSequence.Values;
            string second = "";
            foreach(var sequence in operand.Value) {
                if(second == "") {
                    second = sequence.Title;
                } else {
                    second += "-etc";
                }
                var operandData = sequence;
                switch(arith.Value) {
                case 0:
                    result = TimeSeriesValuesCalculation.OperateWith(result, operandData.Values, (x, y) => x + y);
                    opText = "Add";
                    break;
                case 1:
                    result = TimeSeriesValuesCalculation.OperateWith(result, operandData.Values, (x, y) => x - y);
                    opText = "Subtract";
                    break;
                case 2:
                    result = TimeSeriesValuesCalculation.OperateWith(result, operandData.Values, (x, y) => x * y);
                    opText = "Multiply";
                    break;
                case 3:
                    result = TimeSeriesValuesCalculation.OperateWith(result, operandData.Values, (x, y) => y == 0 ? null : x / y);
                    opText = "Divide";
                    break;
                case 4:
                    result = TimeSeriesValuesCalculation.OperateWith(result, operandData.Values, (x, y) => x > y ? x : y);
                    opText = "Max";
                    break;
                case 5:
                    result = TimeSeriesValuesCalculation.OperateWith(result, operandData.Values, (x, y) => x < y ? x : y);
                    opText = "Min";
                    break;
                default:
                    result = new TimeSeriesValues();
                    break;
                }
            }
            SequenceData ret = new SequenceData(result, null, PathEx.GiveName(opText, env.SelectedSequence.Title, second));
            ret.Type = SequenceType.Numeric;
            return ret;
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            var arith = new SingleSelectParameter("Operation", new[] { "Add", "Subtract", "Multiply", "Divide", "Max", "Min" });
            var operand = new SequenceMultiSelectParameter("Other operands", (v) => v.Type != SequenceType.Label);
            return new ProcParam<SequenceProcEnv>[] { arith, operand };
        }

        public string GetTitle() {
            return "四則演算 / Arithmetic Operation";
        }

        public string GetDescription() {
            return "Add, Subtract, Multiply or Divide 2 Numeric Sequences";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Numeric; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }
        public string GetCommandName() {
            return "Arithmeric";
        }
        #endregion
    }

    public class OperationInnerProduct : ISequenceOperation {

        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            SequenceSingleSelectParameter operand = args[0] as SequenceSingleSelectParameter;
            TimeSeriesValues ret = new TimeSeriesValues("InnerProduct");

            int count = Math.Min(env.SelectedSequence.Values.ColumnCount, operand.Value.Values.ColumnCount);

            using(IEnumerator<KeyValuePair<decimal, decimal?[]>> one = env.SelectedSequence.Values.Enumerate().GetEnumerator())
            using(IEnumerator<KeyValuePair<decimal, decimal?[]>> another = operand.Value.Values.Enumerate().GetEnumerator()) {
                bool oneAlive = one.MoveNext();
                bool anotherAlive = another.MoveNext();
                while(oneAlive && anotherAlive) {
                    decimal? value = 0;
                    for(int i = 0; i < count; i++) {
                        if(one.Current.Value[i].HasValue && another.Current.Value[i].HasValue) {
                            value = value.Value + one.Current.Value[i].Value * another.Current.Value[i].Value;
                        } else {
                            value = null;
                            break;
                        }
                    }
                    // 後にある時刻のところに値を入れる
                    // 前にある方を進める
                    if(one.Current.Key < another.Current.Key) {
                        ret.SetValue(another.Current.Key, value);
                        oneAlive = one.MoveNext();
                    } else {
                        ret.SetValue(one.Current.Key, value);
                        anotherAlive = another.MoveNext();
                    }
                }
            }
            return new SequenceData(ret, null, PathEx.GiveName("InnerProduct", env.SelectedSequence.Title, operand.Value.Title));
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            SequenceSingleSelectParameter operand = new SequenceSingleSelectParameter("Other operand", (v) => (v.Type & SequenceType.Numeric) != 0);
            return new ProcParam<SequenceProcEnv>[] { operand };
        }

        public string GetTitle() {
            return "内積 / Inner Product";
        }

        public string GetDescription() {
            return "二つの時系列データの内積を取ります";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Numeric; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }

        public string GetCommandName() {
            return "InnerProduct";
        }

        #endregion
    }

    public class OperationAbsolute : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            var sequence = TimeSeriesValuesCalculation.AbsoluteLength(env.SelectedSequence.Values);
            sequence.ColumnNames[0] = "Abs Value";
            SequenceData ret = new SequenceData(sequence, null, PathEx.GiveName("Abs", env.SelectedSequence.Title));
            ret.Type = SequenceType.Numeric;
            return ret;
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            return null;
        }

        public string GetTitle() {
            return "ベクトル長 / Absolute Length of Vectors";
        }

        public string GetDescription() {
            return "Return Length of each Vector";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Numeric; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }
        public string GetCommandName() {
            return "VectorLength";
        }

        #endregion
    }
    public class OperationDifferentiate : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            var sequence = TimeSeriesValuesCalculation.Differentiate(env.SelectedSequence.Values);
            sequence.SetColumnNames(env.SelectedSequence.Values.ColumnNames.Select(n => "Diff " + n));
            SequenceData ret = new SequenceData(sequence, null, PathEx.GiveName("Diff", env.SelectedSequence.Title));
            ret.Type = SequenceType.Numeric;
            return ret;
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            return null;
        }

        public string GetTitle() {
            return "微分 / Differentiate";
        }

        public string GetDescription() {
            return "Defferentiate value sequence";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Numeric; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }
        public string GetCommandName() {
            return "Differentiate";
        }
        #endregion
    }
    public class OperationAverageFlat : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            var keepEmpty = args[0] as BooleanParameter;
            var sequence = TimeSeriesValuesCalculation.AverageFlat(env.SelectedSequence.Values, keepEmpty.Value);
            sequence.SetColumnNames(env.SelectedSequence.Values.ColumnNames.Select(n => "Avg " + n));
            SequenceData ret = new SequenceData(sequence, null, PathEx.GiveName("Average", env.SelectedSequence.Title));
            ret.Type = SequenceType.Numeric;
            return ret;
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            var keepEmpty = new BooleanParameter("Keep Empty Values");
            return new ProcParam<SequenceProcEnv>[] { keepEmpty };
        }

        public string GetTitle() {
            return "平均値 / Average Flat Sequence";
        }

        public string GetDescription() {
            return "Get single value sequence of average";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Numeric; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }
        public string GetCommandName() {
            return "TotalAverage";
        }

        #endregion
    }
    public class OperationGreatest : ISequenceOperation {
        public static ICSLabelSequence ToLabelByCondition(IList<SequenceData> sequences, Func<decimal?[], int> cond) {
            var timeList = TimeSeriesValuesCalculation.MergeTimeList(sequences.Select(p => p.Values)).ToList();
            var ret = new ICSLabelSequence();
            int prevKey = 0;
            int prevIndex = -1;
            for(int key = 1; key < timeList.Count - 1; key++) {
                var values = sequences.Select(s => s.Values[timeList[key]][s.Borders.TargetColumnIndex]).ToArray();
                int index = cond(values);
                if(prevIndex != index) {
                    if(prevIndex >= 0 && prevIndex < sequences.Count) {
                        ret.SetLabel(timeList[prevKey], timeList[key], sequences[prevIndex].Title);
                    } else {
                        ret.SetLabel(timeList[prevKey], timeList[key], "");
                    }
                    prevKey = key;
                    prevIndex = index;
                }
            }
            if(timeList.Count >= 1) {
                if(prevIndex >= 0 && prevIndex < sequences.Count) {
                    ret.SetLabel(timeList[prevKey], timeList[timeList.Count - 1], sequences[prevIndex].Title);
                } else {
                    ret.SetLabel(timeList[prevKey], timeList[timeList.Count - 1], "");
                }
            }
            return ret;
        }

        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            var cond = args[0] as SingleSelectParameter;
            var operand = args[1] as SequenceMultiSelectParameter;
            List<SequenceData> targets = new List<SequenceData>();
            targets.Add(env.SelectedSequence);
            foreach(var sequence in operand.Value) {
                if(sequence != env.SelectedSequence)
                    targets.Add(sequence);
            }
            Func<decimal?[], int> condFunc = (x) => -1;
            string opName = "";
            switch(cond.Value) {
            case 0:
                condFunc = (x) => {
                    if(!x.Any(v => v.HasValue))
                        return -1;
                    return x.Select((v, i) => new KeyValuePair<int, decimal?>(i, v)).Aggregate((a, b) => (!a.Value.HasValue ? b : !b.Value.HasValue ? a : a.Value.Value > b.Value.Value ? a : b)).Key;
                };
                opName = "Greatest";
                break;
            case 1:
                condFunc = (x) => {
                    if(!x.Any(v => v.HasValue))
                        return -1;
                    return x.Select((v, i) => new KeyValuePair<int, decimal?>(i, v)).Aggregate((a, b) => (!a.Value.HasValue ? b : !b.Value.HasValue ? a : a.Value.Value < b.Value.Value ? a : b)).Key;
                };
                opName = "Least";
                break;
            case 2:
                condFunc = (x) => {
                    for(int i = 0; i < x.Length; i++) {
                        if(x[i].HasValue)
                            return i;
                    }
                    return -1;
                };
                opName = "NotEmpty";
                break;
            }
            var labels = ToLabelByCondition(targets, condFunc);
            return SequenceData.FromLabelSequence(labels, PathEx.GiveName(opName, targets.Select(p => p.Title)));
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            var cond = new SingleSelectParameter("Condition", new[] { "Greatest", "Least", "First Not Empty" });
            var operand = new SequenceMultiSelectParameter("Other Candidates", (v) => v.Type != SequenceType.Label);

            return new ProcParam<SequenceProcEnv>[] { cond, operand };
        }

        public string GetTitle() {
            return "他の値列と比較してラベル化 / Get Label By Comparation";
        }

        public string GetDescription() {
            return "最大、最小などの条件でラベルを作成します";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Numeric; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }
        public string GetCommandName() {
            return "Greatest";
        }

        #endregion
    }

    public class OperationExtractSequence : ISequenceOperation {

        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            SequenceColumnMultiSelectParameter si = args[0] as SequenceColumnMultiSelectParameter;
            List<int> indices = si.Value.Where(i => 0 <= i && i < env.SelectedSequence.Values.ColumnNames.Length).ToList();
            TimeSeriesValues val = new TimeSeriesValues(indices.Select(i => env.SelectedSequence.Values.ColumnNames[i]));
            foreach(var pair in env.SelectedSequence.Values.Enumerate()) {
                val.SetValue(pair.Key, indices.Select(i => pair.Value[i]).ToArray());
            }
            SequenceData ret = new SequenceData(val, new LabelingBorders(env.SelectedSequence.Borders), PathEx.GiveName("Extracted", env.SelectedSequence.Title));
            ret.Type = SequenceType.Numeric;
            return ret;
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            SequenceColumnMultiSelectParameter si = new SequenceColumnMultiSelectParameter("Indices Extracted");
            return new ProcParam<SequenceProcEnv>[] { si };
        }

        public string GetTitle() {
            return "列の取りだし / Extract one or more Sequence Values";
        }

        public string GetDescription() {
            return "データ系列から一部の列だけを複製します";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Numeric; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }

        public string GetCommandName() {
            return "ExtractValues";
        }
        #endregion
    }
    public class OperationMergeSequence : ISequenceOperation {

        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            SequenceMultiSelectParameter others = args[0] as SequenceMultiSelectParameter;
            List<TimeSeriesValues> sequences = new List<TimeSeriesValues>();
            sequences.Add(env.SelectedSequence.Values);
            sequences.AddRange(others.Value.Select(d => d.Values));
            List<string> columnNames = new List<string>();
            int sequenceIndex = 0;
            foreach(TimeSeriesValues sequence in sequences) {
                sequenceIndex++;
                foreach(string name in sequence.ColumnNames) {
                    columnNames.Add(string.Format("{0}.{1}", sequenceIndex, name));
                }
            }
            TimeSeriesValues values = new TimeSeriesValues(columnNames);
            foreach(var pair in TimeSeriesValuesCalculation.EnumerateCompositeSequences(sequences)) {
                values[pair.Key] = pair.Value;
            }
            return new SequenceData(values, null, PathEx.GiveName("Composite", new string[] { env.SelectedSequence.Title }.Union(others.Value.Select(d => d.Title))));
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            SequenceMultiSelectParameter others = new SequenceMultiSelectParameter("Other Sequences", v => (v.Type & SequenceType.Numeric) != 0);
            return new ProcParam<SequenceProcEnv>[] { others };
        }

        public string GetTitle() {
            return "系列のマージ / Merge Sequence Values";
        }

        public string GetDescription() {
            return "複数の時系列データの列を一つにまとめます";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Numeric; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }

        public string GetCommandName() {
            return "ComposeValues";
        }

        #endregion
    }

    public class OperationUnaryOperation : ISequenceOperation {

        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            SingleSelectParameter mode = args[0] as SingleSelectParameter;
            Func<decimal?, decimal?> ope = new Func<decimal?, decimal?>(x => x);
            string title = "";
            switch(mode.Value) {
            case 0:
                ope = x => x.HasValue ? x * x : null;
                title = "Sq";
                break;
            case 1:
                ope = x => x.HasValue ? (decimal?)Math.Sqrt((double)x.Value) : null;
                title = "Sqrt";
                break;
            case 2:
                ope = x => x.HasValue ? -x : null;
                title = "Neg";
                break;
            case 3:
                ope = x => x.HasValue && x.Value != 0 ? (decimal?)1M / x.Value : null;
                title = "Inv";
                break;
            case 4:
                ope = x => x.HasValue ? (decimal?)Math.Sin((double)x.Value) : null;
                title = "Sin";
                break;
            case 5:
                ope = x => x.HasValue ? (decimal?)Math.Cos((double)x.Value) : null;
                title = "Cos";
                break;
            case 6:
                ope = x => x.HasValue ? (decimal?)Math.Tan((double)x.Value) : null;
                title = "Tan";
                break;
            case 7:
                ope = x => x.HasValue && -1M <= x.Value && x.Value <= 1M ? (decimal?)Math.Asin((double)x.Value) : null;
                title = "Asin";
                break;
            case 8:
                ope = x => x.HasValue && -1M <= x.Value && x.Value <= 1M ? (decimal?)Math.Acos((double)x.Value) : null;
                title = "Acos";
                break;
            case 9:
                ope = x => x.HasValue ? (decimal?)Math.Atan((double)x.Value) : null;
                title = "Atan";
                break;
            case 10:
                ope = x => x.HasValue ? (decimal?)Math.Exp((double)x.Value) : null;
                title = "Exp";
                break;
            case 11:
                ope = x => x.HasValue && x.Value > 0M ? (decimal?)Math.Log((double)x.Value) : null;
                title = "Log";
                break;
            }
            TimeSeriesValues ret = new TimeSeriesValues(env.SelectedSequence.Values.ColumnNames.Select(n => title + " " + n).ToArray());

            foreach(var pair in env.SelectedSequence.Values.Enumerate()) {
                decimal?[] row = new decimal?[ret.ColumnCount];
                for(int i = 0; i < row.Length; i++) {
                    try {
                        row[i] = ope(pair.Value[i]);
                    } catch(ArithmeticException) { }
                }
                ret[pair.Key] = row;
            }
            return new SequenceData(ret, null, PathEx.GiveName(title, env.SelectedSequence.Title));
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            SingleSelectParameter mode = new SingleSelectParameter("Operator", new string[] { "Square", "Square root", "Negate", "Inverse", "Sine", "Cosine", "Tangent", "Arcsine", "Arccosine", "Arctangent", "Exponential", "Logarithm" });
            return new ProcParam<SequenceProcEnv>[] { mode };
        }

        public string GetTitle() {
            return "単項演算 / Unary Operation";
        }

        public string GetDescription() {
            return "時系列データの各シーケンスに対し演算を行います";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Numeric; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }

        public string GetCommandName() {
            return "UnaryOperation";
        }

        #endregion
    }

    public class OperationOffsetScale : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            NumberParameter offset = args[0] as NumberParameter;
            NumberParameter scale = args[1] as NumberParameter;
            SingleSelectParameter mode = args[2] as SingleSelectParameter;

            bool offsetFirst = mode.Value == 0;

            TimeSeriesValues ret = new TimeSeriesValues(env.SelectedSequence.Values.ColumnNames);
            foreach(var pair in env.SelectedSequence.Values.Enumerate()) {
                decimal?[] row = new decimal?[pair.Value.Length];
                for(int i = 0; i < row.Length; i++) {
                    decimal? tmp = pair.Value[i];
                    if(tmp.HasValue) {
                        if(offsetFirst) {
                            tmp = tmp.Value + offset.Value;
                        }
                        tmp = tmp.Value * scale.Value;
                        if(!offsetFirst) {
                            tmp = tmp.Value + offset.Value;
                        }
                    }
                    row[i] = tmp;
                }
                ret[pair.Key] = row;
            }
            return new SequenceData(ret, null, PathEx.GiveName("OffsetScale", env.SelectedSequence.Title));
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            NumberParameter offset = new NumberParameter("Offset", -10000000, 10000000, 3, 1);
            NumberParameter scale = new NumberParameter("Scale", -10000000, 10000000, 3, 1);
            scale.Value = 1M;
            SingleSelectParameter mode = new SingleSelectParameter("Mode", new String[] { "offset後にscale", "scale後にoffset" });
            return new ProcParam<SequenceProcEnv>[] { offset, scale, mode };
        }

        public string GetTitle() {
            return "値のオフセット及びスケーリング / offset and scale values";
        }

        public string GetDescription() {
            return "時系列データの各時点の値を定数倍・定数値加算します";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Numeric; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }

        public string GetCommandName() {
            return "OffsetScale";
        }

        #endregion
    }

    public class OperationStandardNormalDistribution : ISequenceOperation {

        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            BooleanParameter useAvg = args[0] as BooleanParameter;
            BooleanParameter useStddev = args[1] as BooleanParameter;
            TimeSeriesValues ret = new TimeSeriesValues(env.SelectedSequence.Values.ColumnNames);


            decimal[] avg = new decimal[ret.ColumnCount];
            decimal[] stddev = new decimal[ret.ColumnCount];

            int[] count = new int[ret.ColumnCount];
            foreach(var pair in env.SelectedSequence.Values.Enumerate()) {
                for(int i = 0; i < ret.ColumnCount; i++) {
                    if(pair.Value[i].HasValue) {
                        avg[i] += pair.Value[i].Value;
                        count[i]++;
                    }
                }
            }
            for(int i = 0; i < ret.ColumnCount; i++) {
                if(count[i] > 0) {
                    avg[i] /= count[i];
                }
            }
            foreach(var pair in env.SelectedSequence.Values.Enumerate()) {
                for(int i = 0; i < ret.ColumnCount; i++) {
                    if(pair.Value[i].HasValue) {
                        decimal diff = pair.Value[i].Value - avg[i];
                        stddev[i] += diff * diff;
                    }
                }
            }
            for(int i = 0; i < ret.ColumnCount; i++) {
                if(count[i] > 0) {
                    stddev[i] = (decimal)Math.Sqrt((double)(stddev[i] / count[i]));
                }
            }
            foreach(var pair in env.SelectedSequence.Values.Enumerate()) {
                decimal?[] row = new decimal?[ret.ColumnCount];
                for(int i = 0; i < ret.ColumnCount; i++) {
                    decimal? value = pair.Value[i];
                    if(value.HasValue) {
                        if(useAvg.Value) {
                            value = value.Value - avg[i];
                        }
                        if(useStddev.Value && stddev[i] != 0) {
                            value = value.Value / stddev[i];
                        }
                    }
                    row[i] = value;
                }
                ret[pair.Key] = row;
            }
            return new SequenceData(ret, null, PathEx.GiveName("Std", env.SelectedSequence.Title));
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            BooleanParameter useAvg = new BooleanParameter("平均を引く");
            useAvg.Value = true;
            BooleanParameter useStddev = new BooleanParameter("標準偏差で割る");
            useStddev.Value = true;
            return new ProcParam<SequenceProcEnv>[] { useAvg, useStddev };
        }

        public string GetTitle() {
            return "値を平均・標準偏差で正規化(標準正規分布) / normalize by average and stddev";
        }

        public string GetDescription() {
            return "時系列データの分布が標準正規分布に従うように正規化します";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Numeric; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }

        public string GetCommandName() {
            return "StandardNormalDistribution";
        }

        #endregion
    }

}
