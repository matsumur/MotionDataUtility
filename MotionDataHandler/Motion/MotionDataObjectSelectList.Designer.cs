namespace MotionDataHandler .Motion{
    partial class MotionDataObjectSelectList {
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
            this.listSelect = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listSelect
            // 
            this.listSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listSelect.FormattingEnabled = true;
            this.listSelect.ItemHeight = 12;
            this.listSelect.Location = new System.Drawing.Point(0, 0);
            this.listSelect.Name = "listSelect";
            this.listSelect.Size = new System.Drawing.Size(150, 148);
            this.listSelect.TabIndex = 0;
            this.listSelect.SelectedIndexChanged += new System.EventHandler(this.listSelect_SelectedIndexChanged);
            // 
            // MotionDataObjectSelectList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listSelect);
            this.Name = "MotionDataObjectSelectList";
            this.Load += new System.EventHandler(this.MotionDataObjectSelectList_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listSelect;


    }
}
