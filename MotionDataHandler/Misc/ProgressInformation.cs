using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionDataHandler.Misc {
    /// <summary>
    /// ProgressChangedEventArgs用のデータを保持するクラス
    /// </summary>
    public class ProgressInformation {
        /// <summary>
        /// データ完了時の値を取得または設定します．
        /// </summary>
        public double MaxValue;
        /// <summary>
        /// 処理の完了度の値を取得または設定します．
        /// </summary>
        public double CurrentValue;
        /// <summary>
        /// 処理の状態を表す文字列を取得または設定します．
        /// </summary>
        public volatile string Message;
        /// <summary>
        /// 未設定の状態のオブジェクトを作成します．
        /// </summary>
        public ProgressInformation()
            : this(0, "Uninitialized") {
        }
        /// <summary>
        /// 初期化処理を伴うコンストラクタ
        /// </summary>
        /// <param name="maxValue">処理の量</param>
        /// <param name="initialMessage">未処理時のメッセージ</param>
        public ProgressInformation(double maxValue, string initialMessage) {
            this.Initialize(maxValue, initialMessage);
        }
        /// <summary>
        /// オブジェクトを初期化します
        /// </summary>
        /// <param name="maxValue">処理の量</param>
        /// <param name="initialMessage">未処理時のメッセージ</param>
        public void Initialize(double maxValue, string initialMessage) {
            this.CurrentValue = 0;
            this.MaxValue = maxValue;
            this.Message = initialMessage;
        }
        /// <summary>
        /// 現在の完了度を0から100の値で返します
        /// </summary>
        /// <param name="startPercentage">このオブジェクトが複数の処理のうちの一つの処理の情報である場合の，この処理が開始された時の全体の処理の完了割合．通常は0</param>
        /// <param name="percentageRange">このオブジェクトが複数の処理のうちの一つの処理の情報である場合の，この処理が全体に占める処理の量の割合．通常は100</param>
        /// <returns></returns>
        public int GetProgressPercentage(double startPercentage, double percentageRange) {
            double maxValue = this.MaxValue;
            double value = this.CurrentValue;
            if(maxValue == 0)
                return (int)Math.Floor(startPercentage);
            return (int)Math.Floor(startPercentage + percentageRange * value / maxValue);
        }
        /// <summary>
        /// 現在の完了度を0から100の値で返します
        /// </summary>
        /// <returns></returns>
        public int GetProgressPercentage() {
            return this.GetProgressPercentage(0, 100);
        }
    }
}
