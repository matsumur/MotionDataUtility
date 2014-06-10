namespace MotionDataUtil {
    partial class MotionDataUtilityForm {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MotionDataUtilityForm));
            this.dialogOpen = new System.Windows.Forms.OpenFileDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.labelStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.panelMotionDataMarkerList = new System.Windows.Forms.Panel();
            this.motionDataMarkerList1 = new MotionDataHandler.Motion.MotionDataObjectListView();
            this.panelTimePlayer = new System.Windows.Forms.Panel();
            this.TimePlayerMotionData = new MotionDataHandler.Misc.TimePlayer();
            this.panelMotionDataViewer = new System.Windows.Forms.Panel();
            this.motionDataViewer1 = new MotionDataHandler.Motion.MotionDataViewer();
            this.panel3 = new System.Windows.Forms.Panel();
            this.objectExistsView1 = new MotionDataHandler.Motion.ObjectExistenceView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOpenMotionData = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveMotionData = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveMotionDataAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.loadTrcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadCsvToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAddPhaseSpace = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.loadEyeSightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.motionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.sequenceViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuScriptBoard = new System.Windows.Forms.ToolStripMenuItem();
            this.旧エディットToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invertSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuVersion = new System.Windows.Forms.ToolStripMenuItem();
            this.dialogSaveMotionData = new System.Windows.Forms.SaveFileDialog();
            this.dialogOpenMotionData = new System.Windows.Forms.OpenFileDialog();
            this.dialogSaveTsv = new System.Windows.Forms.SaveFileDialog();
            this.dialogLoadEyeSight = new System.Windows.Forms.OpenFileDialog();
            this.dialogOpenTrc = new System.Windows.Forms.OpenFileDialog();
            this.dialogSaveLabel = new System.Windows.Forms.SaveFileDialog();
            this.dialogSaveSequence = new System.Windows.Forms.SaveFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panelSideBar = new System.Windows.Forms.Panel();
            this.pictureSideBar = new System.Windows.Forms.PictureBox();
            this.statusStrip1.SuspendLayout();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.panelMotionDataMarkerList.SuspendLayout();
            this.panelTimePlayer.SuspendLayout();
            this.panelMotionDataViewer.SuspendLayout();
            this.panel3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panelSideBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureSideBar)).BeginInit();
            this.SuspendLayout();
            // 
            // dialogOpen
            // 
            this.dialogOpen.FileName = "openFileDialog1";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 461);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(755, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // labelStatus
            // 
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(41, 17);
            this.labelStatus.Text = "ready.";
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(20, 25);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.panelMotionDataMarkerList);
            this.splitMain.Panel1.Controls.Add(this.panelTimePlayer);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.panelMotionDataViewer);
            this.splitMain.Panel2.Controls.Add(this.panel3);
            this.splitMain.Size = new System.Drawing.Size(735, 436);
            this.splitMain.SplitterDistance = 278;
            this.splitMain.TabIndex = 6;
            // 
            // panelMotionDataMarkerList
            // 
            this.panelMotionDataMarkerList.Controls.Add(this.motionDataMarkerList1);
            this.panelMotionDataMarkerList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMotionDataMarkerList.Location = new System.Drawing.Point(0, 101);
            this.panelMotionDataMarkerList.Name = "panelMotionDataMarkerList";
            this.panelMotionDataMarkerList.Size = new System.Drawing.Size(278, 335);
            this.panelMotionDataMarkerList.TabIndex = 7;
            // 
            // motionDataMarkerList1
            // 
            this.motionDataMarkerList1.AllowDrop = true;
            this.motionDataMarkerList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.motionDataMarkerList1.Location = new System.Drawing.Point(0, 0);
            this.motionDataMarkerList1.Name = "motionDataMarkerList1";
            this.motionDataMarkerList1.Size = new System.Drawing.Size(278, 335);
            this.motionDataMarkerList1.TabIndex = 0;
            this.motionDataMarkerList1.DragDrop += new System.Windows.Forms.DragEventHandler(this.controls_DragDrop);
            this.motionDataMarkerList1.DragEnter += new System.Windows.Forms.DragEventHandler(this.controls_DragEnter);
            // 
            // panelTimePlayer
            // 
            this.panelTimePlayer.Controls.Add(this.TimePlayerMotionData);
            this.panelTimePlayer.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTimePlayer.Location = new System.Drawing.Point(0, 0);
            this.panelTimePlayer.Name = "panelTimePlayer";
            this.panelTimePlayer.Size = new System.Drawing.Size(278, 101);
            this.panelTimePlayer.TabIndex = 6;
            // 
            // TimePlayerMotionData
            // 
            this.TimePlayerMotionData.AllowDrop = true;
            this.TimePlayerMotionData.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.TimePlayerMotionData.Dock = System.Windows.Forms.DockStyle.Top;
            this.TimePlayerMotionData.Location = new System.Drawing.Point(0, 0);
            this.TimePlayerMotionData.Name = "TimePlayerMotionData";
            this.TimePlayerMotionData.Size = new System.Drawing.Size(278, 95);
            this.TimePlayerMotionData.TabIndex = 0;
            this.TimePlayerMotionData.DragDrop += new System.Windows.Forms.DragEventHandler(this.controls_DragDrop);
            this.TimePlayerMotionData.DragEnter += new System.Windows.Forms.DragEventHandler(this.controls_DragEnter);
            // 
            // panelMotionDataViewer
            // 
            this.panelMotionDataViewer.Controls.Add(this.motionDataViewer1);
            this.panelMotionDataViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMotionDataViewer.Location = new System.Drawing.Point(0, 0);
            this.panelMotionDataViewer.Name = "panelMotionDataViewer";
            this.panelMotionDataViewer.Size = new System.Drawing.Size(453, 364);
            this.panelMotionDataViewer.TabIndex = 6;
            // 
            // motionDataViewer1
            // 
            this.motionDataViewer1.AllowDrop = true;
            this.motionDataViewer1.AutoSize = true;
            this.motionDataViewer1.BackColor = System.Drawing.Color.Blue;
            this.motionDataViewer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.motionDataViewer1.CanChangeSelection = true;
            this.motionDataViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.motionDataViewer1.KeepVerticalAxis = true;
            this.motionDataViewer1.Location = new System.Drawing.Point(0, 0);
            this.motionDataViewer1.Name = "motionDataViewer1";
            this.motionDataViewer1.Size = new System.Drawing.Size(453, 364);
            this.motionDataViewer1.TabIndex = 4;
            this.motionDataViewer1.DragDrop += new System.Windows.Forms.DragEventHandler(this.controls_DragDrop);
            this.motionDataViewer1.DragEnter += new System.Windows.Forms.DragEventHandler(this.controls_DragEnter);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.objectExistsView1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 364);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(453, 72);
            this.panel3.TabIndex = 5;
            // 
            // objectExistsView1
            // 
            this.objectExistsView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.objectExistsView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectExistsView1.Location = new System.Drawing.Point(0, 0);
            this.objectExistsView1.Name = "objectExistsView1";
            this.objectExistsView1.Size = new System.Drawing.Size(453, 72);
            this.objectExistsView1.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.viewToolStripMenuItem,
            this.旧エディットToolStripMenuItem,
            this.menuHelp});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(755, 25);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuOpenMotionData,
            this.menuSaveMotionData,
            this.menuSaveMotionDataAs,
            this.toolStripSeparator1,
            this.loadTrcToolStripMenuItem,
            this.loadCsvToolStripMenuItem,
            this.menuAddPhaseSpace,
            this.toolStripSeparator7,
            this.loadEyeSightToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(80, 21);
            this.menuFile.Text = global::MotionDataUtil.Properties.Settings.Default.File;
            this.menuFile.Click += new System.EventHandler(this.menuFile_Click);
            // 
            // menuOpenMotionData
            // 
            this.menuOpenMotionData.Name = "menuOpenMotionData";
            this.menuOpenMotionData.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuOpenMotionData.Size = new System.Drawing.Size(326, 22);
            this.menuOpenMotionData.Text = "開く(&O)";
            this.menuOpenMotionData.Click += new System.EventHandler(this.menuOpenMotionData_Click);
            // 
            // menuSaveMotionData
            // 
            this.menuSaveMotionData.Name = "menuSaveMotionData";
            this.menuSaveMotionData.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuSaveMotionData.Size = new System.Drawing.Size(326, 22);
            this.menuSaveMotionData.Text = "保存(&S)";
            this.menuSaveMotionData.Click += new System.EventHandler(this.menuSaveMotionData_Click);
            // 
            // menuSaveMotionDataAs
            // 
            this.menuSaveMotionDataAs.Name = "menuSaveMotionDataAs";
            this.menuSaveMotionDataAs.Size = new System.Drawing.Size(326, 22);
            this.menuSaveMotionDataAs.Text = "名前を付けて保存(&A)";
            this.menuSaveMotionDataAs.Click += new System.EventHandler(this.menuSaveMotionDataAs_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(323, 6);
            // 
            // loadTrcToolStripMenuItem
            // 
            this.loadTrcToolStripMenuItem.Name = "loadTrcToolStripMenuItem";
            this.loadTrcToolStripMenuItem.Size = new System.Drawing.Size(326, 22);
            this.loadTrcToolStripMenuItem.Text = "MAC3D &Trcファイルを読み込む";
            this.loadTrcToolStripMenuItem.Click += new System.EventHandler(this.loadTrcToolStripMenuItem_Click);
            // 
            // loadCsvToolStripMenuItem
            // 
            this.loadCsvToolStripMenuItem.Name = "loadCsvToolStripMenuItem";
            this.loadCsvToolStripMenuItem.Size = new System.Drawing.Size(326, 22);
            this.loadCsvToolStripMenuItem.Text = "PhaseSpace &Csvファイルを読み込む";
            this.loadCsvToolStripMenuItem.Click += new System.EventHandler(this.loadCsvToolStripMenuItem_Click);
            // 
            // menuAddPhaseSpace
            // 
            this.menuAddPhaseSpace.Name = "menuAddPhaseSpace";
            this.menuAddPhaseSpace.Size = new System.Drawing.Size(326, 22);
            this.menuAddPhaseSpace.Text = "PhaseSpace Csvファイルからオブジェクトを追加";
            this.menuAddPhaseSpace.Click += new System.EventHandler(this.menuAddPhaseSpace_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(323, 6);
            // 
            // loadEyeSightToolStripMenuItem
            // 
            this.loadEyeSightToolStripMenuItem.Name = "loadEyeSightToolStripMenuItem";
            this.loadEyeSightToolStripMenuItem.Size = new System.Drawing.Size(326, 22);
            this.loadEyeSightToolStripMenuItem.Text = "E&ye Sightファイルを読み込む";
            this.loadEyeSightToolStripMenuItem.Click += new System.EventHandler(this.loadEyeSightToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(323, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(326, 22);
            this.exitToolStripMenuItem.Text = "閉じる(&X)";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.motionToolStripMenuItem,
            this.toolStripMenuItem1,
            this.sequenceViewerToolStripMenuItem,
            this.menuScriptBoard});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(60, 21);
            this.viewToolStripMenuItem.Text = "表示(&V)";
            // 
            // motionToolStripMenuItem
            // 
            this.motionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("motionToolStripMenuItem.Image")));
            this.motionToolStripMenuItem.Name = "motionToolStripMenuItem";
            this.motionToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.motionToolStripMenuItem.Text = "サブ3Dビューを追加";
            this.motionToolStripMenuItem.Click += new System.EventHandler(this.motionToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(178, 6);
            // 
            // sequenceViewerToolStripMenuItem
            // 
            this.sequenceViewerToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("sequenceViewerToolStripMenuItem.Image")));
            this.sequenceViewerToolStripMenuItem.Name = "sequenceViewerToolStripMenuItem";
            this.sequenceViewerToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.sequenceViewerToolStripMenuItem.Text = "TSeqViewer";
            this.sequenceViewerToolStripMenuItem.Click += new System.EventHandler(this.sequenceViewerToolStripMenuItem_Click);
            // 
            // menuScriptBoard
            // 
            this.menuScriptBoard.Name = "menuScriptBoard";
            this.menuScriptBoard.Size = new System.Drawing.Size(181, 22);
            this.menuScriptBoard.Text = "スクリプトボード(&S)";
            this.menuScriptBoard.Click += new System.EventHandler(this.menuScriptBoard_Click);
            // 
            // 旧エディットToolStripMenuItem
            // 
            this.旧エディットToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem});
            this.旧エディットToolStripMenuItem.Name = "旧エディットToolStripMenuItem";
            this.旧エディットToolStripMenuItem.Size = new System.Drawing.Size(41, 21);
            this.旧エディットToolStripMenuItem.Text = "Edit";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.noneToolStripMenuItem,
            this.invertSelectedToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.selectAllToolStripMenuItem.Text = "Select &All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // noneToolStripMenuItem
            // 
            this.noneToolStripMenuItem.Name = "noneToolStripMenuItem";
            this.noneToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.noneToolStripMenuItem.Text = "&None";
            this.noneToolStripMenuItem.Click += new System.EventHandler(this.noneToolStripMenuItem_Click);
            // 
            // invertSelectedToolStripMenuItem
            // 
            this.invertSelectedToolStripMenuItem.Name = "invertSelectedToolStripMenuItem";
            this.invertSelectedToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.invertSelectedToolStripMenuItem.Text = "&Invert Selected";
            this.invertSelectedToolStripMenuItem.Click += new System.EventHandler(this.invertSelectedToolStripMenuItem_Click);
            // 
            // menuHelp
            // 
            this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuVersion});
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(44, 21);
            this.menuHelp.Text = "&Help";
            // 
            // menuVersion
            // 
            this.menuVersion.Name = "menuVersion";
            this.menuVersion.Size = new System.Drawing.Size(116, 22);
            this.menuVersion.Text = "&Version";
            this.menuVersion.Click += new System.EventHandler(this.menuVersion_Click);
            // 
            // dialogSaveMotionData
            // 
            this.dialogSaveMotionData.DefaultExt = "mdsb2";
            this.dialogSaveMotionData.Filter = "Binary MotionData ver.2 (*.mdsb2)|*.mdsb2|Xml MotionData ver.2 (*.mdsx2)|*.mdsx2";
            // 
            // dialogOpenMotionData
            // 
            this.dialogOpenMotionData.DefaultExt = "mdsb2";
            this.dialogOpenMotionData.Filter = "MotionData (*.mdsb; *.mdsx; *.mdsb2; *.mdsx2)|*.mdsb; *.mdsx; *.mdsb2; *.mdsx2";
            // 
            // dialogSaveTsv
            // 
            this.dialogSaveTsv.DefaultExt = "txt";
            this.dialogSaveTsv.Filter = "Tab text (*.txt)|*.txt";
            // 
            // dialogLoadEyeSight
            // 
            this.dialogLoadEyeSight.DefaultExt = "csv";
            this.dialogLoadEyeSight.FileName = "openFileDialog1";
            this.dialogLoadEyeSight.Filter = "Eye Sight CSV(*.csv)|*.csv";
            this.dialogLoadEyeSight.Multiselect = true;
            // 
            // dialogOpenTrc
            // 
            this.dialogOpenTrc.DefaultExt = "trc";
            this.dialogOpenTrc.Filter = "Tracked ASCII (*.trc)|*.trc";
            // 
            // dialogSaveLabel
            // 
            this.dialogSaveLabel.DefaultExt = "csv";
            this.dialogSaveLabel.Filter = "CSV (*.csv)|*csv";
            // 
            // dialogSaveSequence
            // 
            this.dialogSaveSequence.DefaultExt = "csv";
            this.dialogSaveSequence.FileName = "sequence";
            this.dialogSaveSequence.Filter = "Sequence file (*.csv)|*csv";
            // 
            // panelSideBar
            // 
            this.panelSideBar.Controls.Add(this.pictureSideBar);
            this.panelSideBar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSideBar.Location = new System.Drawing.Point(0, 25);
            this.panelSideBar.Name = "panelSideBar";
            this.panelSideBar.Size = new System.Drawing.Size(20, 436);
            this.panelSideBar.TabIndex = 8;
            // 
            // pictureSideBar
            // 
            this.pictureSideBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureSideBar.Location = new System.Drawing.Point(0, 0);
            this.pictureSideBar.Name = "pictureSideBar";
            this.pictureSideBar.Size = new System.Drawing.Size(20, 50);
            this.pictureSideBar.TabIndex = 0;
            this.pictureSideBar.TabStop = false;
            this.pictureSideBar.Click += new System.EventHandler(this.pictureSideBar_Click);
            // 
            // MotionDataUtilityForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(755, 483);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.panelSideBar);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MotionDataUtilityForm";
            this.Text = global::MotionDataUtil.Properties.Settings.Default.MotionDataUtilityTitle;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MotionDataUtilityForm_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MotionDataUtilityForm_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            this.splitMain.ResumeLayout(false);
            this.panelMotionDataMarkerList.ResumeLayout(false);
            this.panelTimePlayer.ResumeLayout(false);
            this.panelMotionDataViewer.ResumeLayout(false);
            this.panelMotionDataViewer.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelSideBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureSideBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog dialogOpen;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private MotionDataHandler.Motion.MotionDataViewer motionDataViewer1;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.Panel panelTimePlayer;
        private System.Windows.Forms.Panel panelMotionDataMarkerList;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem loadTrcToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadCsvToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog dialogSaveMotionData;
        private System.Windows.Forms.OpenFileDialog dialogOpenMotionData;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog dialogSaveTsv;
        private System.Windows.Forms.OpenFileDialog dialogLoadEyeSight;
        private System.Windows.Forms.ToolStripMenuItem menuSaveMotionData;
        private System.Windows.Forms.OpenFileDialog dialogOpenTrc;
        private System.Windows.Forms.ToolStripMenuItem menuOpenMotionData;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem invertSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem motionToolStripMenuItem;
        public MotionDataHandler.Misc.TimePlayer TimePlayerMotionData;
        protected System.Windows.Forms.ToolStripStatusLabel labelStatus;
        private System.Windows.Forms.Panel panelMotionDataViewer;
        private System.Windows.Forms.Panel panel3;
        private MotionDataHandler.Motion.ObjectExistenceView objectExistsView1;
        private System.Windows.Forms.SaveFileDialog dialogSaveLabel;
        private System.Windows.Forms.ToolStripMenuItem loadEyeSightToolStripMenuItem;
        private MotionDataHandler.Motion.MotionDataObjectListView motionDataMarkerList1;
        private System.Windows.Forms.ToolStripMenuItem menuSaveMotionDataAs;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.SaveFileDialog dialogSaveSequence;
        private System.Windows.Forms.ToolStripMenuItem sequenceViewerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuAddPhaseSpace;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem 旧エディットToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolStripMenuItem menuVersion;
        private System.Windows.Forms.Panel panelSideBar;
        private System.Windows.Forms.PictureBox pictureSideBar;
        private System.Windows.Forms.ToolStripMenuItem menuScriptBoard;
    }
}

