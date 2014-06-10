using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionDataHandler.Misc {
    /// <summary>
    /// データが更新された状態であるかを伝播するためのインターフェース
    /// </summary>
    public interface IDataModified {
        /// <summary>
        /// データが更新されているかどうかを取得または設定します．
        /// </summary>
        bool IsDataModified { get; set; }
        /// <summary>
        /// データが未更新状態から更新状態に変更されたときに呼び出されるイベント
        /// </summary>
        event EventHandler DataModified;
    }
}
