namespace MotionDataHandler.Sequence {
    partial class DialogCloseSequenceViewers {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.controlViewers = new MotionDataHandler.Sequence.SequenceSelectionControl();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(217, 217);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(136, 217);
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.controlViewers);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(8, 3, 8, 3);
            this.groupBox1.Size = new System.Drawing.Size(280, 199);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "閉じるデータ行を選択";
            // 
            // controlViewers
            // 
            this.controlViewers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlViewers.Location = new System.Drawing.Point(8, 15);
            this.controlViewers.Name = "controlViewers";
            this.controlViewers.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.controlViewers.Size = new System.Drawing.Size(264, 181);
            this.controlViewers.TabIndex = 0;
            this.controlViewers.SelectedIndexChanged += new System.EventHandler(this.controlViewers_SelectedIndexChanged);
            // 
            // DialogCloseSequenceViewers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 252);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "DialogCloseSequenceViewers";
            this.Text = "Close Sequences";
            this.Load += new System.EventHandler(this.DialogCloseSequenceViewers_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DialogCloseSequenceViewers_FormClosed);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.buttonCancel, 0);
            this.Controls.SetChildIndex(this.buttonOK, 0);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private SequenceSelectionControl controlViewers;
    }
}