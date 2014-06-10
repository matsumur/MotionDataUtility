namespace EVaRTTrackHandler {
    partial class Form1 {
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

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent() {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textFolder = new System.Windows.Forms.TextBox();
            this.buttonSelectFolder = new System.Windows.Forms.Button();
            this.buttonSetSecond = new System.Windows.Forms.Button();
            this.buttonSetMain = new System.Windows.Forms.Button();
            this.listFiles = new System.Windows.Forms.ListBox();
            this.buttonListRefresh = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.viewTrack = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.buttonBrowseMain = new System.Windows.Forms.Button();
            this.textTrack = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonSplit = new System.Windows.Forms.Button();
            this.numSplit = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.listTrackSecond = new System.Windows.Forms.ListBox();
            this.buttonConcat = new System.Windows.Forms.Button();
            this.buttonCombine = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textOutput = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonBrowseSecond = new System.Windows.Forms.Button();
            this.dialogSelectFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.dialogTrackFile = new System.Windows.Forms.OpenFileDialog();
            this.dialogCombineOutput = new System.Windows.Forms.SaveFileDialog();
            this.dialogTrackSecond = new System.Windows.Forms.OpenFileDialog();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.numLocaltime = new System.Windows.Forms.NumericUpDown();
            this.buttonPhaseSpace = new System.Windows.Forms.Button();
            this.dialogPhaseSpaceOutput = new System.Windows.Forms.SaveFileDialog();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.numCutAdd = new System.Windows.Forms.NumericUpDown();
            this.buttonCutAdd = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.radioTail = new System.Windows.Forms.RadioButton();
            this.radioHead = new System.Windows.Forms.RadioButton();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.radioAdd = new System.Windows.Forms.RadioButton();
            this.radioCut = new System.Windows.Forms.RadioButton();
            this.dialogConcatOutput = new System.Windows.Forms.SaveFileDialog();
            this.dialogCutAddOutput = new System.Windows.Forms.SaveFileDialog();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.buttonTrimUnnamed = new System.Windows.Forms.Button();
            this.dialogTrimOutput = new System.Windows.Forms.SaveFileDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSplit)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLocaltime)).BeginInit();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCutAdd)).BeginInit();
            this.groupBox8.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textFolder);
            this.groupBox1.Controls.Add(this.buttonSelectFolder);
            this.groupBox1.Controls.Add(this.buttonSetSecond);
            this.groupBox1.Controls.Add(this.buttonSetMain);
            this.groupBox1.Controls.Add(this.listFiles);
            this.groupBox1.Controls.Add(this.buttonListRefresh);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(333, 427);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File List";
            // 
            // textFolder
            // 
            this.textFolder.AllowDrop = true;
            this.textFolder.Enabled = false;
            this.textFolder.Location = new System.Drawing.Point(7, 45);
            this.textFolder.Multiline = true;
            this.textFolder.Name = "textFolder";
            this.textFolder.Size = new System.Drawing.Size(320, 45);
            this.textFolder.TabIndex = 4;
            this.textFolder.TextChanged += new System.EventHandler(this.textFolder_TextChanged);
            this.textFolder.DragDrop += new System.Windows.Forms.DragEventHandler(this.textFolder_DragDrop);
            this.textFolder.DragEnter += new System.Windows.Forms.DragEventHandler(this.textFolder_DragEnter);
            // 
            // buttonSelectFolder
            // 
            this.buttonSelectFolder.Location = new System.Drawing.Point(88, 18);
            this.buttonSelectFolder.Name = "buttonSelectFolder";
            this.buttonSelectFolder.Size = new System.Drawing.Size(97, 23);
            this.buttonSelectFolder.TabIndex = 3;
            this.buttonSelectFolder.Text = "Select Folder";
            this.buttonSelectFolder.UseVisualStyleBackColor = true;
            this.buttonSelectFolder.Click += new System.EventHandler(this.buttonSelectFolder_Click);
            // 
            // buttonSetSecond
            // 
            this.buttonSetSecond.Location = new System.Drawing.Point(230, 398);
            this.buttonSetSecond.Name = "buttonSetSecond";
            this.buttonSetSecond.Size = new System.Drawing.Size(97, 23);
            this.buttonSetSecond.TabIndex = 2;
            this.buttonSetSecond.Text = "Load Second";
            this.buttonSetSecond.UseVisualStyleBackColor = true;
            this.buttonSetSecond.Click += new System.EventHandler(this.buttonSetSecond_Click);
            // 
            // buttonSetMain
            // 
            this.buttonSetMain.Location = new System.Drawing.Point(252, 368);
            this.buttonSetMain.Name = "buttonSetMain";
            this.buttonSetMain.Size = new System.Drawing.Size(75, 23);
            this.buttonSetMain.TabIndex = 2;
            this.buttonSetMain.Text = "Load";
            this.buttonSetMain.UseVisualStyleBackColor = true;
            this.buttonSetMain.Click += new System.EventHandler(this.buttonSetMain_Click);
            // 
            // listFiles
            // 
            this.listFiles.AllowDrop = true;
            this.listFiles.FormattingEnabled = true;
            this.listFiles.HorizontalScrollbar = true;
            this.listFiles.ItemHeight = 12;
            this.listFiles.Location = new System.Drawing.Point(7, 96);
            this.listFiles.Name = "listFiles";
            this.listFiles.Size = new System.Drawing.Size(320, 268);
            this.listFiles.TabIndex = 1;
            this.listFiles.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listFiles_MouseDoubleClick);
            this.listFiles.SelectedIndexChanged += new System.EventHandler(this.listFiles_SelectedIndexChanged);
            this.listFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.listFiles_DragDrop);
            this.listFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.listFiles_DragEnter);
            this.listFiles.SelectedValueChanged += new System.EventHandler(this.listFiles_SelectedValueChanged);
            // 
            // buttonListRefresh
            // 
            this.buttonListRefresh.Location = new System.Drawing.Point(6, 18);
            this.buttonListRefresh.Name = "buttonListRefresh";
            this.buttonListRefresh.Size = new System.Drawing.Size(75, 23);
            this.buttonListRefresh.TabIndex = 0;
            this.buttonListRefresh.Text = "Refresh";
            this.buttonListRefresh.UseVisualStyleBackColor = true;
            this.buttonListRefresh.Click += new System.EventHandler(this.buttonListRefresh_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.viewTrack);
            this.groupBox2.Controls.Add(this.buttonBrowseMain);
            this.groupBox2.Controls.Add(this.textTrack);
            this.groupBox2.Location = new System.Drawing.Point(352, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(511, 194);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Primary Track File";
            // 
            // viewTrack
            // 
            this.viewTrack.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.viewTrack.GridLines = true;
            this.viewTrack.Location = new System.Drawing.Point(9, 44);
            this.viewTrack.Name = "viewTrack";
            this.viewTrack.Size = new System.Drawing.Size(496, 142);
            this.viewTrack.TabIndex = 2;
            this.viewTrack.UseCompatibleStateImageBehavior = false;
            this.viewTrack.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Data";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.Width = 280;
            // 
            // buttonBrowseMain
            // 
            this.buttonBrowseMain.AutoSize = true;
            this.buttonBrowseMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonBrowseMain.Location = new System.Drawing.Point(484, 16);
            this.buttonBrowseMain.Name = "buttonBrowseMain";
            this.buttonBrowseMain.Size = new System.Drawing.Size(21, 22);
            this.buttonBrowseMain.TabIndex = 1;
            this.buttonBrowseMain.Text = "...";
            this.buttonBrowseMain.UseVisualStyleBackColor = true;
            this.buttonBrowseMain.Click += new System.EventHandler(this.buttonBrowseMain_Click);
            // 
            // textTrack
            // 
            this.textTrack.AllowDrop = true;
            this.textTrack.Location = new System.Drawing.Point(7, 19);
            this.textTrack.Name = "textTrack";
            this.textTrack.Size = new System.Drawing.Size(471, 19);
            this.textTrack.TabIndex = 0;
            this.textTrack.TextChanged += new System.EventHandler(this.textTrack_TextChanged);
            this.textTrack.DragDrop += new System.Windows.Forms.DragEventHandler(this.textTrack_DragDrop);
            this.textTrack.DragEnter += new System.Windows.Forms.DragEventHandler(this.textTrack_DragEnter_1);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonSplit);
            this.groupBox3.Controls.Add(this.numSplit);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Location = new System.Drawing.Point(351, 212);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(348, 44);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Split";
            // 
            // buttonSplit
            // 
            this.buttonSplit.Location = new System.Drawing.Point(261, 14);
            this.buttonSplit.Name = "buttonSplit";
            this.buttonSplit.Size = new System.Drawing.Size(75, 23);
            this.buttonSplit.TabIndex = 2;
            this.buttonSplit.Text = "Split";
            this.buttonSplit.UseVisualStyleBackColor = true;
            this.buttonSplit.Click += new System.EventHandler(this.buttonSplit_Click);
            // 
            // numSplit
            // 
            this.numSplit.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numSplit.Location = new System.Drawing.Point(180, 17);
            this.numSplit.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numSplit.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numSplit.Name = "numSplit";
            this.numSplit.Size = new System.Drawing.Size(75, 19);
            this.numSplit.TabIndex = 1;
            this.numSplit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numSplit.ThousandsSeparator = true;
            this.numSplit.Value = new decimal(new int[] {
            70000,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(167, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Maximum Frames of Each File: ";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.buttonDelete);
            this.groupBox4.Controls.Add(this.buttonDown);
            this.groupBox4.Controls.Add(this.buttonUp);
            this.groupBox4.Controls.Add(this.listTrackSecond);
            this.groupBox4.Controls.Add(this.buttonConcat);
            this.groupBox4.Controls.Add(this.buttonCombine);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.textOutput);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.buttonBrowseSecond);
            this.groupBox4.Location = new System.Drawing.Point(352, 345);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(511, 245);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Combine";
            // 
            // buttonDelete
            // 
            this.buttonDelete.AutoSize = true;
            this.buttonDelete.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonDelete.Image = global::EVaRTTrackHandler.Properties.Resources.cross;
            this.buttonDelete.Location = new System.Drawing.Point(482, 117);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(22, 22);
            this.buttonDelete.TabIndex = 6;
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonDown
            // 
            this.buttonDown.AutoSize = true;
            this.buttonDown.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonDown.Image = global::EVaRTTrackHandler.Properties.Resources.darrow;
            this.buttonDown.Location = new System.Drawing.Point(482, 89);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(22, 22);
            this.buttonDown.TabIndex = 6;
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // buttonUp
            // 
            this.buttonUp.AutoSize = true;
            this.buttonUp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonUp.Image = global::EVaRTTrackHandler.Properties.Resources.uarrow;
            this.buttonUp.Location = new System.Drawing.Point(482, 61);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(22, 22);
            this.buttonUp.TabIndex = 6;
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // listTrackSecond
            // 
            this.listTrackSecond.FormattingEnabled = true;
            this.listTrackSecond.HorizontalScrollbar = true;
            this.listTrackSecond.ItemHeight = 12;
            this.listTrackSecond.Location = new System.Drawing.Point(7, 35);
            this.listTrackSecond.Name = "listTrackSecond";
            this.listTrackSecond.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listTrackSecond.Size = new System.Drawing.Size(472, 136);
            this.listTrackSecond.TabIndex = 5;
            this.listTrackSecond.SelectedIndexChanged += new System.EventHandler(this.listTrackSecond_SelectedIndexChanged);
            // 
            // buttonConcat
            // 
            this.buttonConcat.Location = new System.Drawing.Point(404, 214);
            this.buttonConcat.Name = "buttonConcat";
            this.buttonConcat.Size = new System.Drawing.Size(100, 23);
            this.buttonConcat.TabIndex = 4;
            this.buttonConcat.Text = "Concatinate";
            this.buttonConcat.UseVisualStyleBackColor = true;
            this.buttonConcat.Click += new System.EventHandler(this.buttonConcat_Click);
            // 
            // buttonCombine
            // 
            this.buttonCombine.Location = new System.Drawing.Point(430, 185);
            this.buttonCombine.Name = "buttonCombine";
            this.buttonCombine.Size = new System.Drawing.Size(75, 23);
            this.buttonCombine.TabIndex = 4;
            this.buttonCombine.Text = "Combine";
            this.buttonCombine.UseVisualStyleBackColor = true;
            this.buttonCombine.Click += new System.EventHandler(this.buttonCombine_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 174);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "Output File Name";
            // 
            // textOutput
            // 
            this.textOutput.Enabled = false;
            this.textOutput.Location = new System.Drawing.Point(9, 189);
            this.textOutput.Name = "textOutput";
            this.textOutput.Size = new System.Drawing.Size(415, 19);
            this.textOutput.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "Secondary Track Files";
            // 
            // buttonBrowseSecond
            // 
            this.buttonBrowseSecond.AutoSize = true;
            this.buttonBrowseSecond.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonBrowseSecond.Location = new System.Drawing.Point(482, 33);
            this.buttonBrowseSecond.Name = "buttonBrowseSecond";
            this.buttonBrowseSecond.Size = new System.Drawing.Size(21, 22);
            this.buttonBrowseSecond.TabIndex = 1;
            this.buttonBrowseSecond.Text = "...";
            this.buttonBrowseSecond.UseVisualStyleBackColor = true;
            this.buttonBrowseSecond.Click += new System.EventHandler(this.buttonBrowseSecond_Click);
            // 
            // dialogTrackFile
            // 
            this.dialogTrackFile.DefaultExt = "trc";
            this.dialogTrackFile.FileName = "openFileDialog1";
            // 
            // dialogTrackSecond
            // 
            this.dialogTrackSecond.FileName = "openFileDialog1";
            this.dialogTrackSecond.Multiselect = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.numLocaltime);
            this.groupBox5.Controls.Add(this.buttonPhaseSpace);
            this.groupBox5.Location = new System.Drawing.Point(352, 263);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(146, 76);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "PhaseSpace Format";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 5;
            this.label5.Text = "Begin Time:";
            // 
            // numLocaltime
            // 
            this.numLocaltime.Location = new System.Drawing.Point(78, 18);
            this.numLocaltime.Maximum = new decimal(new int[] {
            1410065408,
            2,
            0,
            0});
            this.numLocaltime.Name = "numLocaltime";
            this.numLocaltime.Size = new System.Drawing.Size(49, 19);
            this.numLocaltime.TabIndex = 4;
            this.numLocaltime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numLocaltime.ValueChanged += new System.EventHandler(this.numLocaltime_ValueChanged);
            // 
            // buttonPhaseSpace
            // 
            this.buttonPhaseSpace.AutoSize = true;
            this.buttonPhaseSpace.Location = new System.Drawing.Point(6, 43);
            this.buttonPhaseSpace.Name = "buttonPhaseSpace";
            this.buttonPhaseSpace.Size = new System.Drawing.Size(131, 23);
            this.buttonPhaseSpace.TabIndex = 0;
            this.buttonPhaseSpace.Text = "Output as PhaseSpace";
            this.buttonPhaseSpace.UseVisualStyleBackColor = true;
            this.buttonPhaseSpace.Click += new System.EventHandler(this.buttonPhaseSpace_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.numCutAdd);
            this.groupBox6.Controls.Add(this.buttonCutAdd);
            this.groupBox6.Controls.Add(this.groupBox8);
            this.groupBox6.Controls.Add(this.groupBox7);
            this.groupBox6.Location = new System.Drawing.Point(705, 212);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(157, 126);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Cut && Add Frames";
            // 
            // numCutAdd
            // 
            this.numCutAdd.Location = new System.Drawing.Point(19, 100);
            this.numCutAdd.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numCutAdd.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCutAdd.Name = "numCutAdd";
            this.numCutAdd.Size = new System.Drawing.Size(76, 19);
            this.numCutAdd.TabIndex = 2;
            this.numCutAdd.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numCutAdd.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // buttonCutAdd
            // 
            this.buttonCutAdd.Location = new System.Drawing.Point(101, 97);
            this.buttonCutAdd.Name = "buttonCutAdd";
            this.buttonCutAdd.Size = new System.Drawing.Size(49, 23);
            this.buttonCutAdd.TabIndex = 1;
            this.buttonCutAdd.Text = "Output";
            this.buttonCutAdd.UseVisualStyleBackColor = true;
            this.buttonCutAdd.Click += new System.EventHandler(this.buttonCutAdd_Click);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.radioTail);
            this.groupBox8.Controls.Add(this.radioHead);
            this.groupBox8.Location = new System.Drawing.Point(67, 18);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(59, 67);
            this.groupBox8.TabIndex = 0;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "H/T";
            // 
            // radioTail
            // 
            this.radioTail.AutoSize = true;
            this.radioTail.Location = new System.Drawing.Point(6, 41);
            this.radioTail.Name = "radioTail";
            this.radioTail.Size = new System.Drawing.Size(42, 16);
            this.radioTail.TabIndex = 0;
            this.radioTail.Text = "Tail";
            this.radioTail.UseVisualStyleBackColor = true;
            // 
            // radioHead
            // 
            this.radioHead.AutoSize = true;
            this.radioHead.Checked = true;
            this.radioHead.Location = new System.Drawing.Point(6, 18);
            this.radioHead.Name = "radioHead";
            this.radioHead.Size = new System.Drawing.Size(49, 16);
            this.radioHead.TabIndex = 0;
            this.radioHead.TabStop = true;
            this.radioHead.Text = "Head";
            this.radioHead.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.radioAdd);
            this.groupBox7.Controls.Add(this.radioCut);
            this.groupBox7.Location = new System.Drawing.Point(6, 18);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(55, 67);
            this.groupBox7.TabIndex = 0;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "C/A";
            // 
            // radioAdd
            // 
            this.radioAdd.AutoSize = true;
            this.radioAdd.Location = new System.Drawing.Point(6, 41);
            this.radioAdd.Name = "radioAdd";
            this.radioAdd.Size = new System.Drawing.Size(43, 16);
            this.radioAdd.TabIndex = 0;
            this.radioAdd.Text = "Add";
            this.radioAdd.UseVisualStyleBackColor = true;
            // 
            // radioCut
            // 
            this.radioCut.AutoSize = true;
            this.radioCut.Checked = true;
            this.radioCut.Location = new System.Drawing.Point(6, 18);
            this.radioCut.Name = "radioCut";
            this.radioCut.Size = new System.Drawing.Size(41, 16);
            this.radioCut.TabIndex = 0;
            this.radioCut.TabStop = true;
            this.radioCut.Text = "Cut";
            this.radioCut.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.buttonTrimUnnamed);
            this.groupBox9.Location = new System.Drawing.Point(504, 263);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(195, 75);
            this.groupBox9.TabIndex = 6;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Outputs";
            // 
            // buttonTrimUnnamed
            // 
            this.buttonTrimUnnamed.Location = new System.Drawing.Point(6, 18);
            this.buttonTrimUnnamed.Name = "buttonTrimUnnamed";
            this.buttonTrimUnnamed.Size = new System.Drawing.Size(96, 23);
            this.buttonTrimUnnamed.TabIndex = 0;
            this.buttonTrimUnnamed.Text = "TrimUnnamed";
            this.buttonTrimUnnamed.UseVisualStyleBackColor = true;
            this.buttonTrimUnnamed.Click += new System.EventHandler(this.buttonTrimUnnamed_Click);
            // 
            // dialogTrimOutput
            // 
            this.dialogTrimOutput.FileOk += new System.ComponentModel.CancelEventHandler(this.dialogTrimOutput_FileOk);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(875, 602);
            this.Controls.Add(this.groupBox9);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "EVaRTTrackForm";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSplit)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLocaltime)).EndInit();
            this.groupBox6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numCutAdd)).EndInit();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonSetSecond;
        private System.Windows.Forms.Button buttonSetMain;
        private System.Windows.Forms.ListBox listFiles;
        private System.Windows.Forms.Button buttonListRefresh;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonBrowseMain;
        private System.Windows.Forms.TextBox textTrack;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView viewTrack;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button buttonSplit;
        private System.Windows.Forms.NumericUpDown numSplit;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textOutput;
        private System.Windows.Forms.Button buttonBrowseSecond;
        private System.Windows.Forms.Button buttonCombine;
        private System.Windows.Forms.Button buttonSelectFolder;
        private System.Windows.Forms.FolderBrowserDialog dialogSelectFolder;
        private System.Windows.Forms.OpenFileDialog dialogTrackFile;
        private System.Windows.Forms.TextBox textFolder;
        private System.Windows.Forms.SaveFileDialog dialogCombineOutput;
        private System.Windows.Forms.Button buttonConcat;
        private System.Windows.Forms.ListBox listTrackSecond;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.OpenFileDialog dialogTrackSecond;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button buttonPhaseSpace;
        private System.Windows.Forms.SaveFileDialog dialogPhaseSpaceOutput;
        private System.Windows.Forms.NumericUpDown numLocaltime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.RadioButton radioTail;
        private System.Windows.Forms.RadioButton radioHead;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.RadioButton radioAdd;
        private System.Windows.Forms.RadioButton radioCut;
        private System.Windows.Forms.NumericUpDown numCutAdd;
        private System.Windows.Forms.Button buttonCutAdd;
        private System.Windows.Forms.SaveFileDialog dialogConcatOutput;
        private System.Windows.Forms.SaveFileDialog dialogCutAddOutput;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Button buttonTrimUnnamed;
        private System.Windows.Forms.SaveFileDialog dialogTrimOutput;
    }
}

