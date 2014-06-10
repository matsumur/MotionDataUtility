using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace MotionDataHandler.Misc {
    /// <summary>
    /// 最大値と最小値を保持・更新するための構造体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    struct MinMaxTester<T> {
        public T Min;
        public T Max;
        public bool IsValid;
        public void TestValue(T value) {
            if(!IsValid) {
                Min = Max = value;
                IsValid = true;
            } else {
                int minCmp = Comparer<T>.Default.Compare(Min, value);
                int maxCmp = Comparer<T>.Default.Compare(Max, value);
                if(minCmp > 0)
                    Min = value;
                if(maxCmp < 0)
                    Max = value;
            }
        }
    }
}
