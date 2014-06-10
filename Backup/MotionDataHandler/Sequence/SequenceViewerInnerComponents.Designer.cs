namespace MotionDataHandler.Sequence {
    partial class SequenceViewerInnerComponents {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent() {
            this.DialogOpenSequence = new System.Windows.Forms.OpenFileDialog();
            this.DialogOpenLabel = new System.Windows.Forms.OpenFileDialog();
            this.DialogLoadViewPanel = new System.Windows.Forms.OpenFileDialog();
            // 
            // DialogOpenSequence
            // 
            this.DialogOpenSequence.DefaultExt = "seq";
            this.DialogOpenSequence.FileName = "sequence";
            this.DialogOpenSequence.Filter = "Sequence File (*.seq;*.csv)|*.seq;*.csv";
            this.DialogOpenSequence.Multiselect = true;
            // 
            // DialogOpenLabel
            // 
            this.DialogOpenLabel.DefaultExt = "csv";
            this.DialogOpenLabel.FileName = "label file";
            this.DialogOpenLabel.Filter = "Label Data (*.csv;*.lab)|*.csv;*.lab";
            this.DialogOpenLabel.Multiselect = true;
            // 
            // DialogLoadViewPanel
            // 
            this.DialogLoadViewPanel.DefaultExt = "tvsv";
            this.DialogLoadViewPanel.Filter = "View Panel State (*.tvsv)|*.tvsv";
            this.DialogLoadViewPanel.Multiselect = true;

        }

        #endregion

        public System.Windows.Forms.OpenFileDialog DialogOpenSequence;
        public System.Windows.Forms.OpenFileDialog DialogOpenLabel;
        public System.Windows.Forms.OpenFileDialog DialogLoadViewPanel;
    }
}
