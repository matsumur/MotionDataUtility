using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.DirectX;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace MotionDataHandler.Misc {

    /// <summary>
    /// 巡回セールスマン問題を力づくで解く(平面オブジェクトの外枠を求めるのに使う)
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class TravelingSalesmanProblem<TKey> {
        public IList<TKey> Answer;
        public static TravelingSalesmanProblem<TKey> Solve(IList<TKey> subjects, Func<TKey, TKey, float> getDistance) {
            TravelingSalesmanProblem<TKey> ret = new TravelingSalesmanProblem<TKey>();
            var comparer = Comparer<TKey>.Default;
            if(subjects == null)
                throw new ArgumentNullException("subjects", "'subjects' cannot be null");
            if(getDistance == null)
                throw new ArgumentNullException("getDistance", "'getDistance' cannot be null");
            if(subjects.Count == 0) {
                ret.Answer = new List<TKey>();
                return ret;
            }
            Dictionary<KeyValuePair<int, int>, float> distanceList = new Dictionary<KeyValuePair<int, int>, float>();
            var rest = subjects.Skip(1).ToList();
            Permutation perm = new Permutation(rest.Count, rest.Count, 1);
            IList<int> minList = new List<int>();
            float minDistance = float.PositiveInfinity;
            foreach(var order in perm) {
                List<int> loop = new List<int>();
                loop.Add(0);
                loop.AddRange(order);
                loop.Add(0);
                float distance = 0;
                for(int i = 0; i < loop.Count - 1; i++) {
                    if(!distanceList.ContainsKey(new KeyValuePair<int, int>(loop[i], loop[i + 1]))) {
                        distanceList[new KeyValuePair<int, int>(loop[i], loop[i + 1])] = getDistance(subjects[loop[i]], subjects[loop[i + 1]]);
                    }
                    distance += distanceList[new KeyValuePair<int, int>(loop[i], loop[i + 1])];
                }
                if(distance < minDistance) {
                    minDistance = distance;
                    List<int> min = new List<int>();
                    min.Add(0);
                    min.AddRange(order);
                    minList = min;
                }
            }
            ret.Answer = new List<TKey>();
            foreach(var key in minList) {
                ret.Answer.Add(subjects[key]);

            }
            return ret;
        }
    }

    public class Permutation : PermutationAux, IEnumerable<IList<int>> {
        int _given, _taken, _offset;
        public Permutation(int given, int taken) : this(given, taken, 0) { }
        public Permutation(int given, int taken, int offset) {
            _given = given;
            _taken = taken;
            _offset = offset;
        }
        public new IEnumerator<IList<int>> GetEnumerator() {
            return new PermutationEnumerator(_given, _taken, _offset);
        }
    }

    public class PermutationAux : IEnumerable {
        public IEnumerator GetEnumerator() {
            throw new NotImplementedException();
        }
    }

    public class PermutationEnumeratorAux : IEnumerator {
        public object Current {
            get {
                throw new NotImplementedException();
            }
        }
        public bool MoveNext() {
            throw new NotImplementedException();
        }
        public void Reset() {
            throw new NotImplementedException();
        }
    }

    public class PermutationEnumerator : PermutationEnumeratorAux, IEnumerator<IList<int>> {
        int _given = 0;
        int _taken = 0;
        int _offset = 0;
        IList<int> _state = null;
        readonly List<int> _cards;
        IList<int> _current = null;
        bool _end;
        public PermutationEnumerator(int given, int taken, int offset) {
            if(taken < 0)
                throw new ArgumentOutOfRangeException("taken", "'taken' cannot be negative");
            if(given < 0)
                throw new ArgumentOutOfRangeException("given", "'given' cannot be negative");
            _given = given;
            _taken = taken;
            _offset = offset;
            _state = null;
            _cards = new List<int>();
            for(int i = 0; i < _given; i++)
                _cards.Add(i + _offset);
            _current = null;
            _end = false;
        }
        public new IList<int> Current {
            get { lock(_cards) { return new List<int>(_current); } }
        }
        public new bool MoveNext() {
            lock(_cards) {
                if(_end)
                    return false;
                if(_taken > _given)
                    return false;
                if(_state == null) {
                    _state = new List<int>();
                    for(int j = 0; j < _taken; j++) {
                        _state.Add(0);
                    }
                    _current = new List<int>(_cards).Take(_taken).ToList();
                    return true;
                }
                int i;
                for(i = 0; i < _taken; i++) {
                    _state[i]++;
                    if(_state[i] < _taken - i)
                        break;
                    _state[i] = 0;
                }
                if(i == _taken) {
                    _end = true;
                    return false;
                }
                List<int> cards = new List<int>(_cards);
                _current = new List<int>();
                foreach(var index in _state) {
                    _current.Add(cards[index]);
                    cards.RemoveAt(index);
                }

                return true;
            }
        }
        public new void Reset() {
            lock(_cards) {
                _state = null;
                _end = false;
            }
        }
        public void Dispose() {
        }
    }

    public static class Combination {
        public static IEnumerable<IList<int>> EnumerateCombinations(int given, int taken) {
            if(given < 0)
                throw new ArgumentOutOfRangeException("", "'' cannot be negative");
            if(taken < 0)
                throw new ArgumentOutOfRangeException("", "'' cannot be negative");
            if(taken == 1) {
                yield return new List<int>();
                yield break;
            }
            if(given < taken)
                yield break;
            int[] ret = new int[taken];
            for(int i = 0; i < taken; i++) {
                ret[i] = i;
            }
            while(true) {
                yield return ret.ToList();
                // 最後の組み合わせか確認
                if(ret[0] == given - taken)
                    break;
                // 繰り上がる直前の桁を探す
                int j;
                for(j = 1; j < taken; j++) {
                    if(ret[j] == given - (taken - j)) {
                        break;
                    }
                }
                // 繰り上がる
                ret[j - 1]++;
                // 繰り上がった後の桁たちは直前の桁+1
                for(; j < taken; j++) {
                    ret[j] = ret[j - 1] + 1;
                }
            }
        }
    }
    /// <summary>
    /// Ax = b を解くためのクラス
    /// </summary>
    public class SimultaneousEquations {
        /// <summary>
        /// 得られた解を取得します。解がない場合には長さ0の配列が返されます．
        /// </summary>
        public float[] Answers;
        /// <summary>
        /// 方程式を解きます
        /// </summary>
        /// <param title="matrix">左辺の行列</param>
        /// <param title="vector">右辺のベクトル</param>
        /// <returns>得られた解</returns>
        public static SimultaneousEquations Solve(float[,] matrix, float[] vector) {
            float[,] mat = (float[,])matrix.Clone();
            float[] vec = (float[])vector.Clone();
            SimultaneousEquations ret = new SimultaneousEquations();

            if(mat.GetLength(0) != mat.GetLength(1))
                throw new ArgumentException( "'mat' must be square matrix", "matrix");
            if(mat.GetLength(0) != vec.Length)
                throw new ArgumentException( "'vec' must have same length of 'matrix'", "vector");
            List<int> leftIndex = new List<int>();
            // 行列の左端から、各ベクトルを一つの値を残して0にしていく
            for(int j = 0; j < vec.Length; j++) {
                // 対象の列のベクトルの最大値を取得
                int maxHeadIndex = -1;
                float maxHead = 0;
                for(int i = 0; i < vec.Length; i++) {
                    if(leftIndex.Contains(i))
                        continue;
                    float head = Math.Abs(mat[i, j]);
                    if(head > maxHead) {
                        maxHead = head;
                        maxHeadIndex = i;
                    }
                }
                if(maxHeadIndex == -1) {
                    ret.Answers = new float[0];
                    return ret;
                }
                // 各行に最大係数の行を足し引きして0にしていく
                for(int i = 0; i < vec.Length; i++) {
                    if(i == maxHeadIndex)
                        continue;
                    double coeff = (double)mat[i, j] / mat[maxHeadIndex, j];
                    mat[i, j] = 0;
                    for(int j2 = j + 1; j2 < vec.Length; j2++) {
                        mat[i, j2] = (float)(mat[i, j2] - mat[maxHeadIndex, j2] * coeff);
                    }
                    vec[i] = (float)(vec[i] - vec[maxHeadIndex] * coeff);
                }
                // 最大係数の行の位置; 0でない位置を覚えておく
                leftIndex.Add(maxHeadIndex);
            }
            // 各列残っている行で右辺の係数を割る
            ret.Answers = new float[vec.Length];
            for(int j = 0; j < vec.Length; j++) {
                ret.Answers[j] = vec[leftIndex[j]] / mat[leftIndex[j], j];
            }
            return ret;
        }
    }

    /// <summary>
    /// Ax^2+Bx+C = 0を解くためのクラス
    /// </summary>
    public class QuadraticEquation {
        /// <summary>
        /// 得られた解
        /// </summary>
        public float[] Answers;
        /// <summary>
        /// 解が不定であるか
        /// </summary>
        public bool Indefinite;
        /// <summary>
        /// 方程式を解きます。
        /// </summary>
        /// <param title="A">二次の係数</param>
        /// <param title="B">一次の係数</param>
        /// <param title="C">定数項</param>
        /// <returns>得られた解</returns>
        public static QuadraticEquation Solve(float A, float B, float C) {
            if(A < 0) {
                // 解の順序が小さい順になるように
                A = -A;
                B = -B;
                C = -C;
            }
            QuadraticEquation ret = new QuadraticEquation();
            ret.Indefinite = false;
            if(A != 0) {
                double det = B * B - 4 * A * C;
                if(det > 0) {
                    ret.Answers = new float[2];
                    ret.Answers[0] = (float)((-B - Math.Sqrt(det)) / (2 * A));
                    ret.Answers[1] = (float)((-B + Math.Sqrt(det)) / (2 * A));
                } else if(det == 0) {
                    ret.Answers = new float[1];
                    ret.Answers[0] = (-B) / (2 * A);
                } else {
                    ret.Answers = new float[0];
                }
            } else if(B != 0) {
                ret.Answers = new float[1];
                ret.Answers[0] = -C / B;
            } else if(C != 0) {
                ret.Answers = new float[0];
            } else {
                ret.Answers = new float[0];
                ret.Indefinite = true;
            }

            return ret;

        }
    }
}
