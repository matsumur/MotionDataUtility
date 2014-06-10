using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionDataHandler.Misc {
    /// <summary>
    /// 文字列データを保持するイベント変数クラス
    /// </summary>
    public class StringEventArgs : EventArgs {
        /// <summary>
        /// 文字列データを取得または設定します．
        /// </summary>
        public string Text { get; set; }
        public StringEventArgs(string text) {
            this.Text = text;
        }
    }
}
