namespace MotionDataHandler.Sequence {
    partial class DialogSequenceBorder {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogSequenceBorder));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listBorders = new System.Windows.Forms.ListView();
            this.columnLowerEnd = new System.Windows.Forms.ColumnHeader();
            this.columnName = new System.Windows.Forms.ColumnHeader();
            this.panel4 = new System.Windows.Forms.Panel();
            this.buttonClose = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.buttonOutput = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.buttonReplace = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.comboName = new System.Windows.Forms.ComboBox();
            this.numLowerEnd = new System.Windows.Forms.NumericUpDown();
            this.labelSelected = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.dialogOpenBorder = new System.Windows.Forms.OpenFileDialog();
            this.dialogSaveBorder = new System.Windows.Forms.SaveFileDialog();
            this.targetColumnIndexControl1 = new MotionDataHandler.Sequence.TargetSequenceIndexControl();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLowerEnd)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1MinSize = 64;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel4);
            this.splitContainer1.Panel2.Controls.Add(this.panel3);
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Panel2MinSize = 64;
            this.splitContainer1.Size = new System.Drawing.Size(616, 345);
            this.splitContainer1.SplitterDistance = 238;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBorders);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(238, 345);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "一覧";
            // 
            // listBorders
            // 
            this.listBorders.AllowDrop = true;
            this.listBorders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnLowerEnd,
            this.columnName});
            this.listBorders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBorders.GridLines = true;
            this.listBorders.Location = new System.Drawing.Point(3, 15);
            this.listBorders.MultiSelect = false;
            this.listBorders.Name = "listBorders";
            this.listBorders.Size = new System.Drawing.Size(232, 327);
            this.listBorders.TabIndex = 0;
            this.listBorders.UseCompatibleStateImageBehavior = false;
            this.listBorders.View = System.Windows.Forms.View.Details;
            this.listBorders.SelectedIndexChanged += new System.EventHandler(this.listBorders_SelectedIndexChanged);
            this.listBorders.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBorders_DragDrop);
            this.listBorders.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBorders_DragEnter);
            // 
            // columnLowerEnd
            // 
            this.columnLowerEnd.Text = "開始値";
            this.columnLowerEnd.Width = 75;
            // 
            // columnName
            // 
            this.columnName.Text = "ラベル名";
            this.columnName.Width = 127;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.buttonClose);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 245);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(374, 100);
            this.panel4.TabIndex = 4;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Location = new System.Drawing.Point(287, 65);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 3;
            this.buttonClose.Text = "閉じる";
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.groupBox4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 181);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(374, 64);
            this.panel3.TabIndex = 2;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.targetColumnIndexControl1);
            this.groupBox4.Controls.Add(this.buttonOutput);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(0, 0);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(374, 64);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "ラベル化して出力";
            // 
            // buttonOutput
            // 
            this.buttonOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOutput.AutoSize = true;
            this.buttonOutput.Location = new System.Drawing.Point(275, 18);
            this.buttonOutput.Name = "buttonOutput";
            this.buttonOutput.Size = new System.Drawing.Size(87, 23);
            this.buttonOutput.TabIndex = 2;
            this.buttonOutput.Text = "出力";
            this.buttonOutput.UseVisualStyleBackColor = true;
            this.buttonOutput.Click += new System.EventHandler(this.buttonOutput_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "ラベル化対象の系列名";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 54);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(374, 127);
            this.panel2.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.buttonRemove);
            this.groupBox2.Controls.Add(this.buttonReplace);
            this.groupBox2.Controls.Add(this.buttonAdd);
            this.groupBox2.Controls.Add(this.comboName);
            this.groupBox2.Controls.Add(this.numLowerEnd);
            this.groupBox2.Controls.Add(this.labelSelected);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(374, 127);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "設定";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "ラベル名";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "開始値";
            // 
            // buttonRemove
            // 
            this.buttonRemove.Location = new System.Drawing.Point(165, 88);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(75, 23);
            this.buttonRemove.TabIndex = 5;
            this.buttonRemove.Text = "削除";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonReplace
            // 
            this.buttonReplace.Location = new System.Drawing.Point(84, 88);
            this.buttonReplace.Name = "buttonReplace";
            this.buttonReplace.Size = new System.Drawing.Size(75, 23);
            this.buttonReplace.TabIndex = 4;
            this.buttonReplace.Text = "置き換え";
            this.buttonReplace.UseVisualStyleBackColor = true;
            this.buttonReplace.Click += new System.EventHandler(this.buttonReplace_Click);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(3, 88);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonAdd.TabIndex = 3;
            this.buttonAdd.Text = "追加";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // comboName
            // 
            this.comboName.FormattingEnabled = true;
            this.comboName.Location = new System.Drawing.Point(77, 61);
            this.comboName.Name = "comboName";
            this.comboName.Size = new System.Drawing.Size(121, 20);
            this.comboName.TabIndex = 2;
            this.comboName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboName_KeyDown);
            // 
            // numLowerEnd
            // 
            this.numLowerEnd.DecimalPlaces = 3;
            this.numLowerEnd.Location = new System.Drawing.Point(78, 36);
            this.numLowerEnd.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numLowerEnd.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numLowerEnd.Name = "numLowerEnd";
            this.numLowerEnd.Size = new System.Drawing.Size(120, 19);
            this.numLowerEnd.TabIndex = 1;
            this.numLowerEnd.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numLowerEnd.KeyDown += new System.Windows.Forms.KeyEventHandler(this.numLowerEnd_KeyDown);
            // 
            // labelSelected
            // 
            this.labelSelected.AutoSize = true;
            this.labelSelected.Location = new System.Drawing.Point(6, 15);
            this.labelSelected.Name = "labelSelected";
            this.labelSelected.Size = new System.Drawing.Size(53, 12);
            this.labelSelected.TabIndex = 0;
            this.labelSelected.Text = "選択行: -";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(374, 54);
            this.panel1.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonSave);
            this.groupBox3.Controls.Add(this.buttonLoad);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(374, 54);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "設定の保存";
            // 
            // buttonSave
            // 
            this.buttonSave.AutoSize = true;
            this.buttonSave.Location = new System.Drawing.Point(89, 18);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(78, 23);
            this.buttonSave.TabIndex = 0;
            this.buttonSave.Text = "保存";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonLoad
            // 
            this.buttonLoad.AutoSize = true;
            this.buttonLoad.Location = new System.Drawing.Point(6, 18);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(77, 23);
            this.buttonLoad.TabIndex = 0;
            this.buttonLoad.Text = "読み込み";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // dialogOpenBorder
            // 
            this.dialogOpenBorder.DefaultExt = "seqbdr";
            this.dialogOpenBorder.Filter = "Sequence Border file (*.seqbdr)|*.seqbdr";
            // 
            // dialogSaveBorder
            // 
            this.dialogSaveBorder.DefaultExt = "seqbdr";
            this.dialogSaveBorder.Filter = "Sequence Border file (*.seqbdr)|*.seqbdr";
            // 
            // targetColumnIndexControl1
            // 
            this.targetColumnIndexControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.targetColumnIndexControl1.Location = new System.Drawing.Point(8, 35);
            this.targetColumnIndexControl1.MaximumSize = new System.Drawing.Size(1024, 20);
            this.targetColumnIndexControl1.MinimumSize = new System.Drawing.Size(0, 20);
            this.targetColumnIndexControl1.Name = "targetColumnIndexControl1";
            this.targetColumnIndexControl1.SelectedIndex = -1;
            this.targetColumnIndexControl1.Size = new System.Drawing.Size(129, 20);
            this.targetColumnIndexControl1.TabIndex = 5;
            this.targetColumnIndexControl1.SelectedIndexChanged += new System.EventHandler(this.targetColumnIndexControl1_SelectedIndexChanged);
            // 
            // DialogSequenceBorder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(616, 345);
            this.ControlBox = false;
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(528, 350);
            this.Name = "DialogSequenceBorder";
            this.Text = "ラベル化しきい値の設定";
            this.Load += new System.EventHandler(this.DialogTimeValueBorder_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLowerEnd)).EndInit();
            this.panel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView listBorders;
        private System.Windows.Forms.ColumnHeader columnLowerEnd;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelSelected;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Button buttonReplace;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.ComboBox comboName;
        private System.Windows.Forms.NumericUpDown numLowerEnd;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.OpenFileDialog dialogOpenBorder;
        private System.Windows.Forms.SaveFileDialog dialogSaveBorder;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button buttonOutput;
        private TargetSequenceIndexControl targetColumnIndexControl1;
    }
}