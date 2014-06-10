namespace MotionDataHandler.Sequence {
    partial class DialogSequenceOperation {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param title="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent() {
            this.panelMain = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textExplain = new System.Windows.Forms.TextBox();
            this.textSelected = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(325, 489);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(244, 489);
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // panelMain
            // 
            this.panelMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMain.AutoScroll = true;
            this.panelMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelMain.Location = new System.Drawing.Point(13, 92);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(387, 391);
            this.panelMain.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.textExplain);
            this.panel1.Location = new System.Drawing.Point(13, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(387, 48);
            this.panel1.TabIndex = 2;
            // 
            // textExplain
            // 
            this.textExplain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textExplain.Location = new System.Drawing.Point(0, 0);
            this.textExplain.Multiline = true;
            this.textExplain.Name = "textExplain";
            this.textExplain.ReadOnly = true;
            this.textExplain.Size = new System.Drawing.Size(387, 48);
            this.textExplain.TabIndex = 0;
            // 
            // textSelected
            // 
            this.textSelected.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textSelected.Location = new System.Drawing.Point(13, 67);
            this.textSelected.Name = "textSelected";
            this.textSelected.ReadOnly = true;
            this.textSelected.Size = new System.Drawing.Size(387, 19);
            this.textSelected.TabIndex = 3;
            // 
            // DialogSequenceOperation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.ClientSize = new System.Drawing.Size(412, 524);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.textSelected);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "DialogSequenceOperation";
            this.Text = "Sequence Operation";
            this.Load += new System.EventHandler(this.DialogSequenceOperation_Load);
            this.Controls.SetChildIndex(this.textSelected, 0);
            this.Controls.SetChildIndex(this.panelMain, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.buttonCancel, 0);
            this.Controls.SetChildIndex(this.buttonOK, 0);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textExplain;
        private System.Windows.Forms.TextBox textSelected;
    }
}
