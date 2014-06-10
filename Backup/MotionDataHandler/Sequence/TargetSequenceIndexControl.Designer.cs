namespace MotionDataHandler.Sequence {
    partial class TargetSequenceIndexControl {
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
            this.comboIndices = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // comboIndices
            // 
            this.comboIndices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboIndices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboIndices.FormattingEnabled = true;
            this.comboIndices.Location = new System.Drawing.Point(0, 0);
            this.comboIndices.Name = "comboIndices";
            this.comboIndices.Size = new System.Drawing.Size(129, 20);
            this.comboIndices.TabIndex = 0;
            this.comboIndices.SelectedIndexChanged += new System.EventHandler(this.comboIndices_SelectedIndexChanged);
            // 
            // TargetColumnIndexControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.comboIndices);
            this.MaximumSize = new System.Drawing.Size(1024, 20);
            this.MinimumSize = new System.Drawing.Size(0, 20);
            this.Name = "TargetColumnIndexControl";
            this.Size = new System.Drawing.Size(129, 20);
            this.Load += new System.EventHandler(this.TargetColumnIndexControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboIndices;
    }
}
