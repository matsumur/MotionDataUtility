using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.ObjectModel;


namespace MotionDataHandler.Sequence {
    using Misc;

    public static class TimeSeriesValuesCalculation {

        public static TimeSeriesValues Differentiate(TimeSeriesValues seq) {
            using(seq.Lock.GetReadLock()) {
                TimeSeriesValues ret = new TimeSeriesValues(seq.ColumnNames.Select(n => "diff " + n).ToArray());
                for(int i = 0; i < seq.SequenceLength - 1; i++) {
                    decimal preTime, postTime;
                    if(!seq.TryGetTimeFromIndex(i, out preTime) || !seq.TryGetTimeFromIndex(i + 1, out postTime))
                        continue;
                    decimal elapse = postTime - preTime;
                    if(elapse > 0) {
                        decimal?[] pre = seq[i];
                        decimal?[] post = seq[i + 1];
                        decimal?[] diff = new decimal?[seq.ColumnCount];
                        for(int j = 0; j < seq.ColumnCount; j++) {
                            if(pre[j].HasValue && post[j].HasValue) {
                                diff[j] = (post[j].Value - pre[j].Value) / elapse;
                            }
                        }
                        ret[preTime] = diff;
                    }
                }
                return ret;
            }
        }

        public static TimeSeriesValues AverageFlat(TimeSeriesValues seq, bool keepEmpty) {
            using(seq.Lock.GetReadLock()) {
                TimeSeriesValues ret = new TimeSeriesValues(seq.ColumnNames.Select(n => "avg " + n).ToArray());
                decimal?[] tmp1 = new decimal?[seq.ColumnCount];
                int[] counts = new int[seq.ColumnCount];
                for(int i = 0; i < seq.ColumnCount; i++)
                    counts[i] = 0;
                for(int i = 0; i < seq.ColumnCount; i++)
                    tmp1[i] = 0;
                try {
                    foreach(var pair in seq.Enumerate()) {
                        for(int i = 0; i < pair.Value.Length; i++) {
                            if(pair.Value[i].HasValue) {
                                tmp1[i] += pair.Value[i].Value;
                                counts[i]++;
                            }
                        }
                    }
                    for(int i = 0; i < seq.ColumnCount; i++) {
                        if(counts[i] == 0) {
                            tmp1[i] = null;
                        } else {
                            tmp1[i] /= counts[i];
                        }
                    }
                } catch(OverflowException) {
                    // doubleで集計してdecimalにキャスト
                    double?[] tmp2 = new double?[seq.ColumnCount];
                    for(int i = 0; i < seq.ColumnCount; i++)
                        tmp2[i] = 0;
                    for(int i = 0; i < seq.ColumnCount; i++)
                        counts[i] = 0;
                    foreach(var pair in seq.Enumerate()) {
                        for(int i = 0; i < pair.Value.Length; i++) {
                            if(pair.Value[i].HasValue) {
                                tmp2[i] += (double)pair.Value[i].Value;
                                counts[i]++;
                            }
                        }
                    }
                    for(int i = 0; i < seq.ColumnCount; i++) {
                        if(counts[i] == 0) {
                            tmp1[i] = null;
                        } else {
                            try {
                                tmp1[i] = (decimal)(tmp2[i] / counts[i]);
                            } catch(OverflowException) {
                                tmp1[i] = null;
                            }
                        }
                    }
                }
                foreach(var pair in seq.Enumerate()) {
                    decimal?[] tmp3 = new decimal?[seq.ColumnCount];
                    for(int i = 0; i < seq.ColumnCount; i++) {
                        if(pair.Value[i].HasValue || !keepEmpty) {
                            tmp3[i] = tmp1[i];
                        }
                    }
                    ret[pair.Key] = tmp3;
                }
                return ret;
            }
        }

        public static TimeSeriesValues Aggregate(TimeSeriesValues seq, decimal? initialValue, Func<decimal?, decimal?, decimal?> aggregator, Func<decimal?, decimal?> finalizer) {
            using(seq.Lock.GetReadLock()) {
                TimeSeriesValues ret = new TimeSeriesValues(1);
                foreach(var pair in seq.Enumerate()) {
                    decimal? aggregation = initialValue;
                    foreach(var value in pair.Value) {
                        aggregation = aggregator(aggregation, value);
                    }
                    ret[pair.Key] = new decimal?[] { finalizer(aggregation) };
                }
                return ret;
            }
        }

        public static TimeSeriesValues AbsoluteLength(TimeSeriesValues seq) {
            return Aggregate(seq, 0M,
                (a, n) => (a.HasValue && n.HasValue) ? (decimal?)(a.Value + n.Value * n.Value) : null,
                a => a.HasValue ? (decimal?)Math.Sqrt((double)a.Value) : null);
        }


        public static TimeSeriesValues OperateWith(TimeSeriesValues seq, TimeSeriesValues operand, Func<decimal?, decimal?, decimal?> operation) {
            if(operand == null)
                throw new ArgumentNullException("operand", "'operand' cannot be null");
            int count = Math.Min(seq.ColumnCount, operand.ColumnCount);
            TimeSeriesValues ret = new TimeSeriesValues(seq.ColumnNames.Take(count).ToArray());

            using(IEnumerator<KeyValuePair<decimal, decimal?[]>> one = seq.Enumerate().GetEnumerator())
            using(IEnumerator<KeyValuePair<decimal, decimal?[]>> another = operand.Enumerate().GetEnumerator()) {
                bool oneAlive = one.MoveNext();
                bool anotherAlive = another.MoveNext();
                while(oneAlive && anotherAlive) {
                    decimal?[] values = new decimal?[count];
                    for(int i = 0; i < count; i++) {
                        if(one.Current.Value[i].HasValue && another.Current.Value[i].HasValue) {
                            values[i] = operation(one.Current.Value[i].Value, another.Current.Value[i].Value);
                        }
                    }
                    // 後にある時刻のところに値を入れる
                    // 前にある方を進める
                    if(one.Current.Key < another.Current.Key) {
                        ret.SetValue(another.Current.Key, values);
                        oneAlive = one.MoveNext();
                    } else {
                        ret.SetValue(one.Current.Key, values);
                        anotherAlive = another.MoveNext();
                    }
                }
            }
            return ret;
        }

        public static TimeSeriesValues FrameMean(TimeSeriesValues seq, int meanLength) {
            using(seq.Lock.GetReadLock()) {
                if(meanLength <= 0)
                    throw new ArgumentOutOfRangeException("meanLength", "meanLength > 0");
                TimeSeriesValues ret = new TimeSeriesValues(seq.ColumnNames.Select(n => "mean " + n).ToArray());
                for(int i = 0; i < seq.SequenceLength - meanLength; i++) {
                    decimal?[] mean = new decimal?[seq.ColumnCount];
                    for(int j = 0; j < seq.ColumnCount; j++) { mean[j] = 0; }
                    for(int j = 0; j < seq.ColumnCount; j++) {
                        mean[j] = 0;
                        for(int k = 0; k < meanLength; k++) {
                            decimal? value = seq[i + k][j];
                            if(value.HasValue) {
                                mean[j] += value.Value;
                            } else {
                                mean[j] = null;
                                break;
                            }
                        }
                        if(mean[j].HasValue) {
                            mean[j] /= meanLength;
                        }
                    }
                    decimal time;
                    if(seq.TryGetTimeFromIndex(i, out time))
                        ret.SetValue(time, mean);
                }
                return ret;
            }
        }

        /// <summary>
        /// シーケンスの移動平均を求めます
        /// </summary>
        /// <param name="seq">シーケンス</param>
        /// <param name="smoothSpan">移動平均を取る時間範囲</param>
        /// <param name="timeOffset">出力の時間のオフセット．0で時間範囲の終了点の時間，smoothSpanの値で時間範囲の開始点の時間のところに出力される</param>
        /// <param name="limitRatioToContainEmptyValue">データの欠損があっても値を出力する限界率(0～1)</param>
        /// <returns></returns>
        public static TimeSeriesValues TimeSmooth(TimeSeriesValues seq, decimal smoothSpan, decimal timeOffset, decimal limitRatioToContainEmptyValue) {
            using(seq.Lock.GetReadLock()) {
                if(smoothSpan < 0)
                    throw new ArgumentOutOfRangeException("time", "'time' cannot be negative");
                TimeSeriesValues ret = new TimeSeriesValues(seq.ColumnNames);
                int[] nullCount = new int[seq.ColumnCount];
                decimal[] sum = new decimal[seq.ColumnCount];
                int count = 0;
                int beginIndex = 0;
                for(int endIndex = 0; endIndex < seq.SequenceLength; endIndex++) {
                    decimal endTime;
                    if(!seq.TryGetTimeFromIndex(endIndex, out endTime))
                        continue;

                    for(int j = 0; j < seq.ColumnCount; j++) {
                        if(seq[endIndex][j].HasValue) {
                            sum[j] += seq[endIndex][j].Value;
                        } else {
                            nullCount[j]++;
                        }
                    }
                    count++;

                    decimal beginTime;
                    while(beginIndex < endIndex && seq.TryGetTimeFromIndex(beginIndex, out beginTime)) {
                        if(beginTime > endTime - smoothSpan) {
                            break;
                        }
                        for(int j = 0; j < seq.ColumnCount; j++) {
                            if(seq[beginIndex][j].HasValue) {
                                sum[j] -= seq[beginIndex][j].Value;
                            } else {
                                nullCount[j]--;
                            }
                        }
                        count--;
                        beginIndex++;
                    }
                    Debug.Assert(nullCount.Max() <= count);
                    Debug.Assert(count > 0);
                    decimal?[] values = new decimal?[seq.ColumnCount];
                    for(int j = 0; j < seq.ColumnCount; j++) {
                        decimal rate = (decimal)nullCount[j] / count;
                        int validCount = count - nullCount[j];
                        if(rate <= limitRatioToContainEmptyValue && validCount > 0) {
                            values[j] = sum[j] / validCount;
                        } else {
                            values[j] = null;
                        }
                    }
                    ret[endTime + timeOffset] = values;
                }
                return ret;
            }
        }

        public static TimeSeriesValues Gaussian(TimeSeriesValues seq, decimal stddev, int trialCount, decimal limitRatioToContainEmptyValue) {
            // stddev = time * sqrt(trial / 4)
            decimal time = stddev / (decimal)Math.Sqrt(0.25 * trialCount);
            TimeSeriesValues tmp = new TimeSeriesValues(seq);
            for(int i = 0; i < trialCount; i++) {
                tmp = TimeSmooth(tmp, time * 2, -time, limitRatioToContainEmptyValue);
            }
            return tmp;
        }
        /*
        public static TimeSeriesValueData Gaussian(TimeSeriesValueData seq, decimal stddev, decimal limitRatioToContainEmptyValue) {
            TimeSeriesValueData tmp = new TimeSeriesValueData(seq.ColumnNames);
            double dLimit = (double)limitRatioToContainEmptyValue;
            double dStddev = (double)stddev;
            foreach(var pair in seq.Enumerate()) {
                double[] valueSum = new double[seq.ColumnCount];
                double[] timeSum = new double[seq.ColumnCount];
                double[] lackSum = new double[seq.ColumnCount];
                foreach(var row in seq.EnumerateRows(pair.Key - stddev * 3, pair.Key + stddev * 3)) {
                    double dDuration = (double)row.Duration();
                    decimal centerTime = (row.BeginTime + row.EndTime) / 2;
                    double diffTime = (double)(centerTime - pair.Key);
                    double gauss = 1;
                    //(double)Math.Exp(-diffTime * diffTime / (2.0 * dStddev * dStddev));
                    for(int i = 0; i < seq.ColumnCount; i++) {
                        double coef = dDuration * gauss;
                        timeSum[i] += coef;
                        if(row.Values[i].HasValue) {
                            valueSum[i] += (double)row.Values[i].Value * coef;
                        } else {
                            lackSum[i] += coef;
                        }
                    }
                }
                decimal?[] values = new decimal?[seq.ColumnCount];
                for(int i = 0; i < seq.ColumnCount; i++) {
                    if(timeSum[i] - lackSum[i] > 0 && lackSum[i] <= dLimit * timeSum[i]) {
                        try {
                            values[i] = (decimal)(valueSum[i] / (timeSum[i] - lackSum[i]));
                        } catch(ArithmeticException) { }
                    }
                }
                tmp[pair.Key] = values;
            }
            return tmp;
        }
        */
        public static TimeSeriesValues GetResampled(TimeSeriesValues seq, decimal interval) {
            using(seq.Lock.GetReadLock()) {
                decimal min = seq.BeginTime;
                decimal max = seq.EndTime;
                TimeSeriesValues ret = new TimeSeriesValues(seq.ColumnNames);
                for(decimal time = min; time < max + interval; time += interval) {
                    var values = seq[time];
                    if(values != null)
                        ret[time] = values;
                }
                return ret;
            }
        }

        /// <summary>
        /// 複数の時系列データから，時間の早い順に値を列挙します
        /// </summary>
        /// <param name="sequenceList"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<int, KeyValuePair<decimal, decimal?[]>>> EnumerateSequences(IEnumerable<TimeSeriesValues> sequenceList) {
            var enumerators = sequenceList.Select(s => s.Enumerate().GetEnumerator()).ToList();
            bool[] endList = Enumerable.Repeat(false, enumerators.Count).ToArray();
            try {
                int index2 = 0;
                foreach(var e in enumerators) {
                    if(!e.MoveNext()) {
                        endList[index2] = true;
                    }
                    index2++;
                }

                while(enumerators.Count > 0) {
                    IEnumerator<KeyValuePair<decimal, decimal?[]>> minEnu = null;
                    KeyValuePair<decimal, decimal?[]> value = new KeyValuePair<decimal, decimal?[]>(decimal.MaxValue, null);
                    int minIndex = 0;
                    int index = 0;
                    foreach(var e in enumerators) {
                        if(!endList[index] && e.Current.Key < value.Key) {
                            minEnu = e;
                            value = e.Current;
                            minIndex = index;
                        }
                        index++;
                    }
                    if(minEnu == null)
                        break;
                    if(!minEnu.MoveNext()) {
                        endList[minIndex] = true;
                    }
                    yield return new KeyValuePair<int, KeyValuePair<decimal, decimal?[]>>(minIndex, value);
                }
            } finally {
                enumerators.ForEach(e => e.Dispose());
            }
        }

        /// <summary>
        /// 複数の時系列データをマージして列挙します
        /// </summary>
        /// <param name="sequenceList"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<decimal, decimal?[]>> EnumerateCompositeSequences(IEnumerable<TimeSeriesValues> sequenceList) {
            List<TimeSeriesValues> sequences = sequenceList.ToList();
            int columnCount = 0;
            List<int> startIndices = new List<int>();
            foreach(TimeSeriesValues sequence in sequences) {
                startIndices.Add(columnCount);
                columnCount += sequence.ColumnCount;
            }
            decimal?[] row = new decimal?[columnCount];
            foreach(var pair in EnumerateSequences(sequences)) {
                int startIndex = startIndices[pair.Key];
                for(int i = 0; i < pair.Value.Value.Length; i++) {
                    row[i + startIndex] = pair.Value.Value[i];
                }
                yield return new KeyValuePair<decimal, decimal?[]>(pair.Value.Key, row.ToArray());
            }
        }
        /// <summary>
        /// 各系列の時間を早い順に列挙します。重複する時間は取り除かれます。
        /// </summary>
        /// <param name="sequenceList">系列のリスト</param>
        /// <returns></returns>
        public static IEnumerable<Decimal> MergeTimeList(IEnumerable<TimeSeriesValues> sequenceList) {
            decimal prev = decimal.MinValue;
            foreach(var pair in EnumerateSequences(sequenceList)) {
                decimal time = pair.Value.Key;
                if(time != prev)
                    yield return time;
                prev = time;
            }
        }

    }
}
