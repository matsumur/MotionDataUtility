namespace MotionDataHandler.Sequence {
    partial class LabelJumpForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param title="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.borderSelectControl1 = new MotionDataHandler.Sequence.BorderSelectionControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelSequenceSelectControl1 = new MotionDataHandler.Sequence.SequenceSelectionControl();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel7 = new System.Windows.Forms.Panel();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.panel9 = new System.Windows.Forms.Panel();
            this.listResult = new System.Windows.Forms.ListView();
            this.columnBegin = new System.Windows.Forms.ColumnHeader();
            this.columnEnd = new System.Windows.Forms.ColumnHeader();
            this.columnDuration = new System.Windows.Forms.ColumnHeader();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.buttonNext = new System.Windows.Forms.Button();
            this.panel5 = new System.Windows.Forms.Panel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownFixedSec = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownMarginSec = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownMarginRatio = new System.Windows.Forms.NumericUpDown();
            this.radioButtonMarginSec = new System.Windows.Forms.RadioButton();
            this.radioButtonFixedSec = new System.Windows.Forms.RadioButton();
            this.radioButtonMarginRatio = new System.Windows.Forms.RadioButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel7.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel10.SuspendLayout();
            this.panel5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFixedSec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMarginSec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMarginRatio)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.borderSelectControl1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(251, 109);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "対象のラベル";
            // 
            // borderSelectControl1
            // 
            this.borderSelectControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.borderSelectControl1.FormattingEnabled = true;
            this.borderSelectControl1.ItemHeight = 12;
            this.borderSelectControl1.Location = new System.Drawing.Point(3, 15);
            this.borderSelectControl1.Name = "borderSelectControl1";
            this.borderSelectControl1.Size = new System.Drawing.Size(245, 88);
            this.borderSelectControl1.TabIndex = 0;
            this.borderSelectControl1.SelectedIndexChanged += new System.EventHandler(this.borderSelectControl1_SelectedIndexChanged);
            this.borderSelectControl1.Click += new System.EventHandler(this.borderSelectControl1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelSequenceSelectControl1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(226, 109);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "対象のビュー";
            // 
            // labelSequenceSelectControl1
            // 
            this.labelSequenceSelectControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelSequenceSelectControl1.Location = new System.Drawing.Point(3, 15);
            this.labelSequenceSelectControl1.Name = "labelSequenceSelectControl1";
            this.labelSequenceSelectControl1.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.labelSequenceSelectControl1.Size = new System.Drawing.Size(220, 91);
            this.labelSequenceSelectControl1.TabIndex = 0;
            this.labelSequenceSelectControl1.SelectedIndexChanged += new System.EventHandler(this.labelSequenceSelectControl1_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.panel7);
            this.groupBox3.Controls.Add(this.panel5);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(481, 191);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "検索";
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.groupBox5);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(155, 15);
            this.panel7.Name = "panel7";
            this.panel7.Padding = new System.Windows.Forms.Padding(4);
            this.panel7.Size = new System.Drawing.Size(323, 173);
            this.panel7.TabIndex = 1;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.panel9);
            this.groupBox5.Controls.Add(this.panel8);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Location = new System.Drawing.Point(4, 4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(315, 165);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "検索結果";
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.listResult);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel9.Location = new System.Drawing.Point(3, 15);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(254, 147);
            this.panel9.TabIndex = 1;
            // 
            // listResult
            // 
            this.listResult.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnBegin,
            this.columnEnd,
            this.columnDuration});
            this.listResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listResult.FullRowSelect = true;
            this.listResult.GridLines = true;
            this.listResult.HideSelection = false;
            this.listResult.Location = new System.Drawing.Point(0, 0);
            this.listResult.MultiSelect = false;
            this.listResult.Name = "listResult";
            this.listResult.Size = new System.Drawing.Size(254, 147);
            this.listResult.TabIndex = 0;
            this.listResult.UseCompatibleStateImageBehavior = false;
            this.listResult.View = System.Windows.Forms.View.Details;
            this.listResult.SelectedIndexChanged += new System.EventHandler(this.listResult_SelectedIndexChanged);
            this.listResult.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listResult_ColumnClick);
            // 
            // columnBegin
            // 
            this.columnBegin.Text = "開始";
            // 
            // columnEnd
            // 
            this.columnEnd.Text = "終了";
            // 
            // columnDuration
            // 
            this.columnDuration.Text = "長さ";
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.panel10);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel8.Location = new System.Drawing.Point(257, 15);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(55, 147);
            this.panel8.TabIndex = 0;
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.buttonNext);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel10.Location = new System.Drawing.Point(0, 93);
            this.panel10.Name = "panel10";
            this.panel10.Padding = new System.Windows.Forms.Padding(4);
            this.panel10.Size = new System.Drawing.Size(55, 54);
            this.panel10.TabIndex = 0;
            // 
            // buttonNext
            // 
            this.buttonNext.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonNext.Location = new System.Drawing.Point(4, 4);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(47, 46);
            this.buttonNext.TabIndex = 0;
            this.buttonNext.Text = "次を 検索";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.groupBox4);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel5.Location = new System.Drawing.Point(3, 15);
            this.panel5.Name = "panel5";
            this.panel5.Padding = new System.Windows.Forms.Padding(4);
            this.panel5.Size = new System.Drawing.Size(152, 173);
            this.panel5.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.numericUpDownFixedSec);
            this.groupBox4.Controls.Add(this.numericUpDownMarginSec);
            this.groupBox4.Controls.Add(this.numericUpDownMarginRatio);
            this.groupBox4.Controls.Add(this.radioButtonMarginSec);
            this.groupBox4.Controls.Add(this.radioButtonFixedSec);
            this.groupBox4.Controls.Add(this.radioButtonMarginRatio);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(4, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(144, 165);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "表示モード";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(69, 136);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "Sec.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(69, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Sec.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(69, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(11, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "%";
            // 
            // numericUpDownFixedSec
            // 
            this.numericUpDownFixedSec.DecimalPlaces = 1;
            this.numericUpDownFixedSec.Location = new System.Drawing.Point(17, 134);
            this.numericUpDownFixedSec.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numericUpDownFixedSec.Name = "numericUpDownFixedSec";
            this.numericUpDownFixedSec.Size = new System.Drawing.Size(46, 19);
            this.numericUpDownFixedSec.TabIndex = 1;
            this.numericUpDownFixedSec.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownFixedSec.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this.numericUpDownFixedSec.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // numericUpDownMarginSec
            // 
            this.numericUpDownMarginSec.DecimalPlaces = 1;
            this.numericUpDownMarginSec.Location = new System.Drawing.Point(17, 87);
            this.numericUpDownMarginSec.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numericUpDownMarginSec.Name = "numericUpDownMarginSec";
            this.numericUpDownMarginSec.Size = new System.Drawing.Size(46, 19);
            this.numericUpDownMarginSec.TabIndex = 1;
            this.numericUpDownMarginSec.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownMarginSec.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this.numericUpDownMarginSec.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // numericUpDownMarginRatio
            // 
            this.numericUpDownMarginRatio.Location = new System.Drawing.Point(17, 40);
            this.numericUpDownMarginRatio.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDownMarginRatio.Name = "numericUpDownMarginRatio";
            this.numericUpDownMarginRatio.Size = new System.Drawing.Size(46, 19);
            this.numericUpDownMarginRatio.TabIndex = 1;
            this.numericUpDownMarginRatio.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownMarginRatio.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this.numericUpDownMarginRatio.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // radioButtonMarginSec
            // 
            this.radioButtonMarginSec.AutoSize = true;
            this.radioButtonMarginSec.Location = new System.Drawing.Point(6, 65);
            this.radioButtonMarginSec.Name = "radioButtonMarginSec";
            this.radioButtonMarginSec.Size = new System.Drawing.Size(110, 16);
            this.radioButtonMarginSec.TabIndex = 0;
            this.radioButtonMarginSec.Text = "固定余白を加える";
            this.radioButtonMarginSec.UseVisualStyleBackColor = true;
            this.radioButtonMarginSec.CheckedChanged += new System.EventHandler(this.radioButtonMarginSec_CheckedChanged);
            // 
            // radioButtonFixedSec
            // 
            this.radioButtonFixedSec.AutoSize = true;
            this.radioButtonFixedSec.Location = new System.Drawing.Point(6, 112);
            this.radioButtonFixedSec.Name = "radioButtonFixedSec";
            this.radioButtonFixedSec.Size = new System.Drawing.Size(71, 16);
            this.radioButtonFixedSec.TabIndex = 0;
            this.radioButtonFixedSec.Text = "固定時間";
            this.radioButtonFixedSec.UseVisualStyleBackColor = true;
            this.radioButtonFixedSec.CheckedChanged += new System.EventHandler(this.radioButtonFixedSec_CheckedChanged);
            // 
            // radioButtonMarginRatio
            // 
            this.radioButtonMarginRatio.AutoSize = true;
            this.radioButtonMarginRatio.Checked = true;
            this.radioButtonMarginRatio.Location = new System.Drawing.Point(6, 18);
            this.radioButtonMarginRatio.Name = "radioButtonMarginRatio";
            this.radioButtonMarginRatio.Size = new System.Drawing.Size(86, 16);
            this.radioButtonMarginRatio.TabIndex = 0;
            this.radioButtonMarginRatio.TabStop = true;
            this.radioButtonMarginRatio.Text = "余白を加える";
            this.radioButtonMarginRatio.UseVisualStyleBackColor = true;
            this.radioButtonMarginRatio.CheckedChanged += new System.EventHandler(this.radioButtonMarginRatio_CheckedChanged);
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
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox3);
            this.splitContainer1.Panel2MinSize = 100;
            this.splitContainer1.Size = new System.Drawing.Size(481, 304);
            this.splitContainer1.SplitterDistance = 109;
            this.splitContainer1.TabIndex = 2;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer2.Size = new System.Drawing.Size(481, 109);
            this.splitContainer2.SplitterDistance = 226;
            this.splitContainer2.TabIndex = 1;
            // 
            // LabelJumpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 304);
            this.Controls.Add(this.splitContainer1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LabelJumpForm";
            this.Text = "ラベルの検索";
            this.Activated += new System.EventHandler(this.LabelJumpForm_Activated);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFixedSec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMarginSec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMarginRatio)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private SequenceSelectionControl labelSequenceSelectControl1;
        private BorderSelectionControl borderSelectControl1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.ListView listResult;
        private System.Windows.Forms.ColumnHeader columnBegin;
        private System.Windows.Forms.ColumnHeader columnEnd;
        private System.Windows.Forms.ColumnHeader columnDuration;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.RadioButton radioButtonMarginRatio;
        private System.Windows.Forms.NumericUpDown numericUpDownMarginRatio;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownMarginSec;
        private System.Windows.Forms.RadioButton radioButtonMarginSec;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownFixedSec;
        private System.Windows.Forms.RadioButton radioButtonFixedSec;
    }
}