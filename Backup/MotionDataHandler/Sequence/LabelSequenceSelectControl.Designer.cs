namespace MotionDataHandler.Sequence {
    partial class SequenceSelectionControl {
        /// <summary> 
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param title="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
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
            this.listLabelSequence = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listLabelSequence
            // 
            this.listLabelSequence.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listLabelSequence.FormattingEnabled = true;
            this.listLabelSequence.ItemHeight = 12;
            this.listLabelSequence.Location = new System.Drawing.Point(0, 0);
            this.listLabelSequence.Name = "listLabelSequence";
            this.listLabelSequence.Size = new System.Drawing.Size(150, 148);
            this.listLabelSequence.TabIndex = 0;
            this.listLabelSequence.SelectedIndexChanged += new System.EventHandler(this.listLabelSequence_SelectedIndexChanged);
            // 
            // SequenceSelectionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listLabelSequence);
            this.Name = "SequenceSelectionControl";
            this.Load += new System.EventHandler(this.LabelSequenceSelectControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listLabelSequence;
    }
}
