using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionDataHandler.Misc {
    /// <summary>
    /// 時区間のデータ構造．TimeControllerがすべてのデータの開始時間と終了時間を知るために用いる
    /// </summary>
    public interface ITimeInterval {
        /// <summary>
        /// 時区間の開始時間を取得します。
        /// </summary>
        decimal BeginTime { get; }
        /// <summary>
        /// 時区間の終了時間を取得します。
        /// </summary>
        decimal EndTime { get; }

        /// <summary>
        /// 時区間の変更を通知するイベント
        /// </summary>
        event EventHandler TimeIntervalChanged;
    }

}
