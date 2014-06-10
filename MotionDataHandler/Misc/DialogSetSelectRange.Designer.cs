namespace MotionDataHandler.Misc {
    partial class DialogSetSelectRange {
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numBeginFrame = new System.Windows.Forms.NumericUpDown();
            this.numBeginSec = new System.Windows.Forms.NumericUpDown();
            this.radioBeginFrame = new System.Windows.Forms.RadioButton();
            this.radioBeginSec = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numEndFrame = new System.Windows.Forms.NumericUpDown();
            this.numEndSec = new System.Windows.Forms.NumericUpDown();
            this.radioEndFrame = new System.Windows.Forms.RadioButton();
            this.radioEndSec = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBeginFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBeginSec)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numEndFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEndSec)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(146, 163);
            this.buttonCancel.TabIndex = 1;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(65, 163);
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(209, 145);
            this.panel1.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1MinSize = 72;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel2MinSize = 72;
            this.splitContainer1.Size = new System.Drawing.Size(209, 145);
            this.splitContainer1.SplitterDistance = 72;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numBeginFrame);
            this.groupBox1.Controls.Add(this.numBeginSec);
            this.groupBox1.Controls.Add(this.radioBeginFrame);
            this.groupBox1.Controls.Add(this.radioBeginSec);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(209, 72);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "BeginTime";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(236, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 12);
            this.label1.TabIndex = 2;
            // 
            // numBeginFrame
            // 
            this.numBeginFrame.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numBeginFrame.Location = new System.Drawing.Point(112, 43);
            this.numBeginFrame.Name = "numBeginFrame";
            this.numBeginFrame.Size = new System.Drawing.Size(91, 19);
            this.numBeginFrame.TabIndex = 3;
            this.numBeginFrame.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numBeginFrame.ValueChanged += new System.EventHandler(this.numBeginFrame_ValueChanged);
            // 
            // numBeginSec
            // 
            this.numBeginSec.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numBeginSec.DecimalPlaces = 3;
            this.numBeginSec.Location = new System.Drawing.Point(112, 18);
            this.numBeginSec.Name = "numBeginSec";
            this.numBeginSec.Size = new System.Drawing.Size(91, 19);
            this.numBeginSec.TabIndex = 2;
            this.numBeginSec.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numBeginSec.ValueChanged += new System.EventHandler(this.numBeginSec_ValueChanged);
            // 
            // radioBeginFrame
            // 
            this.radioBeginFrame.AutoSize = true;
            this.radioBeginFrame.Location = new System.Drawing.Point(6, 43);
            this.radioBeginFrame.Name = "radioBeginFrame";
            this.radioBeginFrame.Size = new System.Drawing.Size(55, 16);
            this.radioBeginFrame.TabIndex = 1;
            this.radioBeginFrame.Text = "Frame";
            this.radioBeginFrame.UseVisualStyleBackColor = true;
            this.radioBeginFrame.CheckedChanged += new System.EventHandler(this.radioBeginFrame_CheckedChanged);
            // 
            // radioBeginSec
            // 
            this.radioBeginSec.AutoSize = true;
            this.radioBeginSec.Checked = true;
            this.radioBeginSec.Location = new System.Drawing.Point(6, 18);
            this.radioBeginSec.Name = "radioBeginSec";
            this.radioBeginSec.Size = new System.Drawing.Size(44, 16);
            this.radioBeginSec.TabIndex = 0;
            this.radioBeginSec.TabStop = true;
            this.radioBeginSec.Text = "Sec.";
            this.radioBeginSec.UseVisualStyleBackColor = true;
            this.radioBeginSec.CheckedChanged += new System.EventHandler(this.radioBeginSec_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numEndFrame);
            this.groupBox2.Controls.Add(this.numEndSec);
            this.groupBox2.Controls.Add(this.radioEndFrame);
            this.groupBox2.Controls.Add(this.radioEndSec);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(209, 72);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "EndTime";
            // 
            // numEndFrame
            // 
            this.numEndFrame.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numEndFrame.Location = new System.Drawing.Point(112, 43);
            this.numEndFrame.Name = "numEndFrame";
            this.numEndFrame.Size = new System.Drawing.Size(91, 19);
            this.numEndFrame.TabIndex = 3;
            this.numEndFrame.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numEndFrame.ValueChanged += new System.EventHandler(this.numEndFrame_ValueChanged);
            // 
            // numEndSec
            // 
            this.numEndSec.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numEndSec.DecimalPlaces = 3;
            this.numEndSec.Location = new System.Drawing.Point(112, 18);
            this.numEndSec.Name = "numEndSec";
            this.numEndSec.Size = new System.Drawing.Size(91, 19);
            this.numEndSec.TabIndex = 2;
            this.numEndSec.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numEndSec.ValueChanged += new System.EventHandler(this.numEndSec_ValueChanged);
            // 
            // radioEndFrame
            // 
            this.radioEndFrame.AutoSize = true;
            this.radioEndFrame.Location = new System.Drawing.Point(6, 43);
            this.radioEndFrame.Name = "radioEndFrame";
            this.radioEndFrame.Size = new System.Drawing.Size(55, 16);
            this.radioEndFrame.TabIndex = 1;
            this.radioEndFrame.Text = "Frame";
            this.radioEndFrame.UseVisualStyleBackColor = true;
            this.radioEndFrame.CheckedChanged += new System.EventHandler(this.radioEndFrame_CheckedChanged);
            // 
            // radioEndSec
            // 
            this.radioEndSec.AutoSize = true;
            this.radioEndSec.Checked = true;
            this.radioEndSec.Location = new System.Drawing.Point(6, 18);
            this.radioEndSec.Name = "radioEndSec";
            this.radioEndSec.Size = new System.Drawing.Size(44, 16);
            this.radioEndSec.TabIndex = 0;
            this.radioEndSec.TabStop = true;
            this.radioEndSec.Text = "Sec.";
            this.radioEndSec.UseVisualStyleBackColor = true;
            this.radioEndSec.CheckedChanged += new System.EventHandler(this.radioEndSec_CheckedChanged);
            // 
            // DialogSetSelectRange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(233, 198);
            this.Controls.Add(this.panel1);
            this.Name = "DialogSetSelectRange";
            this.Text = "Select Range";
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.buttonCancel, 0);
            this.Controls.SetChildIndex(this.buttonOK, 0);
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBeginFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBeginSec)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numEndFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEndSec)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioBeginSec;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numBeginFrame;
        private System.Windows.Forms.NumericUpDown numBeginSec;
        private System.Windows.Forms.RadioButton radioBeginFrame;
        private System.Windows.Forms.NumericUpDown numEndFrame;
        private System.Windows.Forms.NumericUpDown numEndSec;
        private System.Windows.Forms.RadioButton radioEndFrame;
        private System.Windows.Forms.RadioButton radioEndSec;
    }
}