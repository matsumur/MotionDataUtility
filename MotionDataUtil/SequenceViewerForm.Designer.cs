namespace MotionDataUtil {
    partial class SequenceViewerForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param title="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SequenceViewerForm));
            this.dialogSaveState = new System.Windows.Forms.SaveFileDialog();
            this.dialogOpenState = new System.Windows.Forms.OpenFileDialog();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelTimeSelectionGlobal = new System.Windows.Forms.Panel();
            this.timeSelectionControlGlobal = new MotionDataHandler.Misc.TimeSelectionControl();
            this.panelREsetScale = new System.Windows.Forms.Panel();
            this.pictResetScale = new System.Windows.Forms.PictureBox();
            this.panelViewerList = new System.Windows.Forms.Panel();
            this.panelViewerMain = new System.Windows.Forms.Panel();
            this.panelTimeSelection = new System.Windows.Forms.Panel();
            this.panelTimeSelectionLocal = new System.Windows.Forms.Panel();
            this.timeSelectionControlLocal = new MotionDataHandler.Misc.TimeSelectionControl();
            this.panelTimeSelectionMargin = new System.Windows.Forms.Panel();
            this.vScrollBarViewer = new System.Windows.Forms.VScrollBar();
            this.panelHScroll = new System.Windows.Forms.Panel();
            this.scrollTime = new System.Windows.Forms.HScrollBar();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonScrollMode = new System.Windows.Forms.ToolStripSplitButton();
            this.menuAutoScroll = new System.Windows.Forms.ToolStripMenuItem();
            this.menuManualScroll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonLoad = new System.Windows.Forms.ToolStripButton();
            this.buttonSave = new System.Windows.Forms.ToolStripButton();
            this.buttonRun = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonNewPanel = new System.Windows.Forms.ToolStripButton();
            this.buttonClosePanels = new System.Windows.Forms.ToolStripButton();
            this.buttonDust = new System.Windows.Forms.ToolStripButton();
            this.buttonLoopMode = new System.Windows.Forms.ToolStripSplitButton();
            this.noLoopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonRenderingMode = new System.Windows.Forms.ToolStripSplitButton();
            this.fastRenderingModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trueRenderingModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.splitContainerViewer = new System.Windows.Forms.SplitContainer();
            this.groupBoxDustBox = new System.Windows.Forms.GroupBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.listBoxDustBox = new System.Windows.Forms.ListBox();
            this.panelNoteForDustBox = new System.Windows.Forms.Panel();
            this.labelNoteForDustBox = new System.Windows.Forms.Label();
            this.panelControlDustBox = new System.Windows.Forms.Panel();
            this.buttonRefreshDustBox = new System.Windows.Forms.Button();
            this.buttonLoadDust = new System.Windows.Forms.Button();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLoadState = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveState = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveStateAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.menuAddPanelFromState = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuOpenTimeSeriesValues = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOpenLabel = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOpenViewState = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.closewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewPanelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeMultiplePanelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSetLabel = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLockEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.menuLabelingBorder = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.menuJumpLabel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.menuChangeLabelColors = new System.Windows.Forms.ToolStripMenuItem();
            this.menuView = new System.Windows.Forms.ToolStripMenuItem();
            this.menuActiveView = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.menuResetScale = new System.Windows.Forms.ToolStripMenuItem();
            this.menuZoomSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUnzoomSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.menuHideView = new System.Windows.Forms.ToolStripMenuItem();
            this.menuReorderView = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewHeight = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMinimumViewHeight = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSmallViewHeight = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMediumViewHeight = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLargeViewHeight = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.textViewHeight = new System.Windows.Forms.ToolStripTextBox();
            this.menuCaptureViewArea = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.menuFastRenderingEnabled = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.dustBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.motionDataUtilToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuScriptBoard = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.menuPlay = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPlayPause = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuJumpStart = new System.Windows.Forms.ToolStripMenuItem();
            this.menuJumpEnd = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.menuAutoScrollEnabled = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLoopEnabled = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLastOperation = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOperateValueSequence = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOperateLabelSequence = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.labelStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.labelCurrentTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelTop.SuspendLayout();
            this.panelTimeSelectionGlobal.SuspendLayout();
            this.panelREsetScale.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictResetScale)).BeginInit();
            this.panelViewerList.SuspendLayout();
            this.panelTimeSelection.SuspendLayout();
            this.panelTimeSelectionLocal.SuspendLayout();
            this.panelHScroll.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.splitContainerViewer.Panel1.SuspendLayout();
            this.splitContainerViewer.Panel2.SuspendLayout();
            this.splitContainerViewer.SuspendLayout();
            this.groupBoxDustBox.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panelNoteForDustBox.SuspendLayout();
            this.panelControlDustBox.SuspendLayout();
            this.menuMain.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dialogSaveState
            // 
            this.dialogSaveState.DefaultExt = "svs";
            this.dialogSaveState.FileName = "Seqv";
            this.dialogSaveState.Filter = "Sequence Visualizer State file(*.svs)|*.svs";
            // 
            // dialogOpenState
            // 
            this.dialogOpenState.DefaultExt = "svs";
            this.dialogOpenState.FileName = "seqv";
            this.dialogOpenState.Filter = "Sequence Visualizer State file(*.svs)|*.svs";
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.panelTimeSelectionGlobal);
            this.panelTop.Controls.Add(this.panelREsetScale);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 50);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(611, 24);
            this.panelTop.TabIndex = 1;
            // 
            // panelTimeSelectionGlobal
            // 
            this.panelTimeSelectionGlobal.Controls.Add(this.timeSelectionControlGlobal);
            this.panelTimeSelectionGlobal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTimeSelectionGlobal.Location = new System.Drawing.Point(0, 0);
            this.panelTimeSelectionGlobal.Name = "panelTimeSelectionGlobal";
            this.panelTimeSelectionGlobal.Size = new System.Drawing.Size(587, 24);
            this.panelTimeSelectionGlobal.TabIndex = 2;
            // 
            // timeSelectionControlGlobal
            // 
            this.timeSelectionControlGlobal.CurrentTimeTabSize = 10;
            this.timeSelectionControlGlobal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeSelectionControlGlobal.InvertY = false;
            this.timeSelectionControlGlobal.IsGlobalTimeLineMode = true;
            this.timeSelectionControlGlobal.Location = new System.Drawing.Point(0, 0);
            this.timeSelectionControlGlobal.MajorRuleHeight = 10;
            this.timeSelectionControlGlobal.MinorRuleHeight = 6;
            this.timeSelectionControlGlobal.Name = "timeSelectionControlGlobal";
            this.timeSelectionControlGlobal.ShowsCursorTimeLabel = true;
            this.timeSelectionControlGlobal.ShowsFrameIndex = false;
            this.timeSelectionControlGlobal.Size = new System.Drawing.Size(587, 24);
            this.timeSelectionControlGlobal.SubMajorRuleHeight = 8;
            this.timeSelectionControlGlobal.TabIndex = 0;
            // 
            // panelREsetScale
            // 
            this.panelREsetScale.Controls.Add(this.pictResetScale);
            this.panelREsetScale.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelREsetScale.Location = new System.Drawing.Point(587, 0);
            this.panelREsetScale.Name = "panelREsetScale";
            this.panelREsetScale.Size = new System.Drawing.Size(24, 24);
            this.panelREsetScale.TabIndex = 1;
            // 
            // pictResetScale
            // 
            this.pictResetScale.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictResetScale.Image = ((System.Drawing.Image)(resources.GetObject("pictResetScale.Image")));
            this.pictResetScale.Location = new System.Drawing.Point(0, 0);
            this.pictResetScale.Name = "pictResetScale";
            this.pictResetScale.Size = new System.Drawing.Size(24, 24);
            this.pictResetScale.TabIndex = 0;
            this.pictResetScale.TabStop = false;
            this.pictResetScale.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictResetScale_MouseClick);
            // 
            // panelViewerList
            // 
            this.panelViewerList.AllowDrop = true;
            this.panelViewerList.AutoScroll = true;
            this.panelViewerList.Controls.Add(this.panelViewerMain);
            this.panelViewerList.Controls.Add(this.panelTimeSelection);
            this.panelViewerList.Controls.Add(this.vScrollBarViewer);
            this.panelViewerList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelViewerList.Location = new System.Drawing.Point(0, 0);
            this.panelViewerList.Name = "panelViewerList";
            this.panelViewerList.Size = new System.Drawing.Size(609, 168);
            this.panelViewerList.TabIndex = 2;
            this.panelViewerList.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelViewer_DragDrop);
            this.panelViewerList.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelViewer_DragEnter);
            // 
            // panelViewerMain
            // 
            this.panelViewerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelViewerMain.Location = new System.Drawing.Point(0, 24);
            this.panelViewerMain.Name = "panelViewerMain";
            this.panelViewerMain.Size = new System.Drawing.Size(592, 144);
            this.panelViewerMain.TabIndex = 2;
            this.panelViewerMain.Resize += new System.EventHandler(this.panelViewer_Resize);
            // 
            // panelTimeSelection
            // 
            this.panelTimeSelection.Controls.Add(this.panelTimeSelectionLocal);
            this.panelTimeSelection.Controls.Add(this.panelTimeSelectionMargin);
            this.panelTimeSelection.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTimeSelection.Location = new System.Drawing.Point(0, 0);
            this.panelTimeSelection.Name = "panelTimeSelection";
            this.panelTimeSelection.Size = new System.Drawing.Size(592, 24);
            this.panelTimeSelection.TabIndex = 0;
            // 
            // panelTimeSelectionLocal
            // 
            this.panelTimeSelectionLocal.Controls.Add(this.timeSelectionControlLocal);
            this.panelTimeSelectionLocal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTimeSelectionLocal.Location = new System.Drawing.Point(12, 0);
            this.panelTimeSelectionLocal.Name = "panelTimeSelectionLocal";
            this.panelTimeSelectionLocal.Size = new System.Drawing.Size(580, 24);
            this.panelTimeSelectionLocal.TabIndex = 1;
            // 
            // timeSelectionControlLocal
            // 
            this.timeSelectionControlLocal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.timeSelectionControlLocal.CurrentTimeTabSize = 10;
            this.timeSelectionControlLocal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeSelectionControlLocal.InvertY = true;
            this.timeSelectionControlLocal.IsGlobalTimeLineMode = false;
            this.timeSelectionControlLocal.Location = new System.Drawing.Point(0, 0);
            this.timeSelectionControlLocal.MajorRuleHeight = 10;
            this.timeSelectionControlLocal.MinorRuleHeight = 6;
            this.timeSelectionControlLocal.Name = "timeSelectionControlLocal";
            this.timeSelectionControlLocal.ShowsCursorTimeLabel = true;
            this.timeSelectionControlLocal.ShowsFrameIndex = false;
            this.timeSelectionControlLocal.Size = new System.Drawing.Size(580, 24);
            this.timeSelectionControlLocal.SubMajorRuleHeight = 8;
            this.timeSelectionControlLocal.TabIndex = 0;
            // 
            // panelTimeSelectionMargin
            // 
            this.panelTimeSelectionMargin.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelTimeSelectionMargin.Location = new System.Drawing.Point(0, 0);
            this.panelTimeSelectionMargin.Name = "panelTimeSelectionMargin";
            this.panelTimeSelectionMargin.Size = new System.Drawing.Size(12, 24);
            this.panelTimeSelectionMargin.TabIndex = 0;
            // 
            // vScrollBarViewer
            // 
            this.vScrollBarViewer.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBarViewer.Location = new System.Drawing.Point(592, 0);
            this.vScrollBarViewer.Name = "vScrollBarViewer";
            this.vScrollBarViewer.Size = new System.Drawing.Size(17, 168);
            this.vScrollBarViewer.SmallChange = 5;
            this.vScrollBarViewer.TabIndex = 0;
            this.vScrollBarViewer.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBarViewer_Scroll);
            // 
            // panelHScroll
            // 
            this.panelHScroll.AutoSize = true;
            this.panelHScroll.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelHScroll.Controls.Add(this.scrollTime);
            this.panelHScroll.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelHScroll.Location = new System.Drawing.Point(0, 168);
            this.panelHScroll.Name = "panelHScroll";
            this.panelHScroll.Size = new System.Drawing.Size(609, 17);
            this.panelHScroll.TabIndex = 3;
            // 
            // scrollTime
            // 
            this.scrollTime.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.scrollTime.LargeChange = 5;
            this.scrollTime.Location = new System.Drawing.Point(0, 0);
            this.scrollTime.Maximum = 10;
            this.scrollTime.Name = "scrollTime";
            this.scrollTime.Size = new System.Drawing.Size(609, 17);
            this.scrollTime.TabIndex = 0;
            this.scrollTime.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrollTime_Scroll);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // buttonScrollMode
            // 
            this.buttonScrollMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonScrollMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAutoScroll,
            this.menuManualScroll});
            this.buttonScrollMode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonScrollMode.Name = "buttonScrollMode";
            this.buttonScrollMode.Size = new System.Drawing.Size(82, 22);
            this.buttonScrollMode.Text = "scrollMode";
            this.buttonScrollMode.ButtonClick += new System.EventHandler(this.buttonScrollMode_ButtonClick);
            // 
            // menuAutoScroll
            // 
            this.menuAutoScroll.Name = "menuAutoScroll";
            this.menuAutoScroll.Size = new System.Drawing.Size(145, 22);
            this.menuAutoScroll.Text = "AutoScroll";
            this.menuAutoScroll.Click += new System.EventHandler(this.menuAutoScroll_Click);
            // 
            // menuManualScroll
            // 
            this.menuManualScroll.Name = "menuManualScroll";
            this.menuManualScroll.Size = new System.Drawing.Size(145, 22);
            this.menuManualScroll.Text = "ManualScroll";
            this.menuManualScroll.Click += new System.EventHandler(this.menuManualScroll_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonLoad,
            this.buttonSave,
            this.toolStripSeparator2,
            this.buttonRun,
            this.toolStripSeparator7,
            this.buttonNewPanel,
            this.buttonClosePanels,
            this.buttonDust,
            this.toolStripSeparator3,
            this.buttonScrollMode,
            this.buttonLoopMode,
            this.buttonRenderingMode});
            this.toolStrip1.Location = new System.Drawing.Point(0, 25);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(611, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // buttonLoad
            // 
            this.buttonLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonLoad.Image = ((System.Drawing.Image)(resources.GetObject("buttonLoad.Image")));
            this.buttonLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(23, 22);
            this.buttonLoad.Text = "状態を開く";
            this.buttonLoad.Click += new System.EventHandler(this.menuLoadState_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonSave.Image = ((System.Drawing.Image)(resources.GetObject("buttonSave.Image")));
            this.buttonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(23, 22);
            this.buttonSave.Text = "状態を保存";
            this.buttonSave.Click += new System.EventHandler(this.saveStateToolStripMenuItem_Click);
            // 
            // buttonRun
            // 
            this.buttonRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonRun.Image = ((System.Drawing.Image)(resources.GetObject("buttonRun.Image")));
            this.buttonRun.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(23, 22);
            this.buttonRun.Text = "再生・停止";
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 25);
            // 
            // buttonNewPanel
            // 
            this.buttonNewPanel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonNewPanel.Image = ((System.Drawing.Image)(resources.GetObject("buttonNewPanel.Image")));
            this.buttonNewPanel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonNewPanel.Name = "buttonNewPanel";
            this.buttonNewPanel.Size = new System.Drawing.Size(23, 22);
            this.buttonNewPanel.Text = "データ行を追加します";
            this.buttonNewPanel.Click += new System.EventHandler(this.addNewPanelToolStripMenuItem_Click);
            // 
            // buttonClosePanels
            // 
            this.buttonClosePanels.CheckOnClick = true;
            this.buttonClosePanels.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonClosePanels.Image = ((System.Drawing.Image)(resources.GetObject("buttonClosePanels.Image")));
            this.buttonClosePanels.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonClosePanels.Name = "buttonClosePanels";
            this.buttonClosePanels.Size = new System.Drawing.Size(23, 22);
            this.buttonClosePanels.Text = "複数のデータ行を選択して閉じます";
            this.buttonClosePanels.Click += new System.EventHandler(this.closeMultiplePanelsToolStripMenuItem_Click);
            // 
            // buttonDust
            // 
            this.buttonDust.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDust.Image = ((System.Drawing.Image)(resources.GetObject("buttonDust.Image")));
            this.buttonDust.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDust.Name = "buttonDust";
            this.buttonDust.Size = new System.Drawing.Size(23, 22);
            this.buttonDust.Text = "ごみ箱";
            this.buttonDust.Click += new System.EventHandler(this.buttonDust_Click);
            // 
            // buttonLoopMode
            // 
            this.buttonLoopMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonLoopMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noLoopToolStripMenuItem,
            this.loopToolStripMenuItem});
            this.buttonLoopMode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonLoopMode.Name = "buttonLoopMode";
            this.buttonLoopMode.Size = new System.Drawing.Size(77, 22);
            this.buttonLoopMode.Text = "loopMode";
            this.buttonLoopMode.ButtonClick += new System.EventHandler(this.buttonLoopMode_ButtonClick);
            // 
            // noLoopToolStripMenuItem
            // 
            this.noLoopToolStripMenuItem.Name = "noLoopToolStripMenuItem";
            this.noLoopToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.noLoopToolStripMenuItem.Text = "No Loop";
            this.noLoopToolStripMenuItem.Click += new System.EventHandler(this.noLoopToolStripMenuItem_Click);
            // 
            // loopToolStripMenuItem
            // 
            this.loopToolStripMenuItem.Name = "loopToolStripMenuItem";
            this.loopToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.loopToolStripMenuItem.Text = "Loop";
            this.loopToolStripMenuItem.Click += new System.EventHandler(this.loopToolStripMenuItem_Click);
            // 
            // buttonRenderingMode
            // 
            this.buttonRenderingMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonRenderingMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fastRenderingModeToolStripMenuItem,
            this.trueRenderingModeToolStripMenuItem});
            this.buttonRenderingMode.Image = ((System.Drawing.Image)(resources.GetObject("buttonRenderingMode.Image")));
            this.buttonRenderingMode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRenderingMode.Name = "buttonRenderingMode";
            this.buttonRenderingMode.Size = new System.Drawing.Size(106, 22);
            this.buttonRenderingMode.Text = "renderingMode";
            this.buttonRenderingMode.ButtonClick += new System.EventHandler(this.buttonRenderingMode_ButtonClick);
            // 
            // fastRenderingModeToolStripMenuItem
            // 
            this.fastRenderingModeToolStripMenuItem.Name = "fastRenderingModeToolStripMenuItem";
            this.fastRenderingModeToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.fastRenderingModeToolStripMenuItem.Text = "Fast Rendering Mode";
            this.fastRenderingModeToolStripMenuItem.Click += new System.EventHandler(this.fastRenderingModeToolStripMenuItem_Click);
            // 
            // trueRenderingModeToolStripMenuItem
            // 
            this.trueRenderingModeToolStripMenuItem.Name = "trueRenderingModeToolStripMenuItem";
            this.trueRenderingModeToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.trueRenderingModeToolStripMenuItem.Text = "True Rendering Mode";
            this.trueRenderingModeToolStripMenuItem.Click += new System.EventHandler(this.trueRenderingModeToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // splitContainerViewer
            // 
            this.splitContainerViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainerViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerViewer.Location = new System.Drawing.Point(0, 74);
            this.splitContainerViewer.Name = "splitContainerViewer";
            this.splitContainerViewer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerViewer.Panel1
            // 
            this.splitContainerViewer.Panel1.Controls.Add(this.panelViewerList);
            this.splitContainerViewer.Panel1.Controls.Add(this.panelHScroll);
            // 
            // splitContainerViewer.Panel2
            // 
            this.splitContainerViewer.Panel2.Controls.Add(this.groupBoxDustBox);
            this.splitContainerViewer.Panel2.Padding = new System.Windows.Forms.Padding(3);
            this.splitContainerViewer.Size = new System.Drawing.Size(611, 320);
            this.splitContainerViewer.SplitterDistance = 187;
            this.splitContainerViewer.TabIndex = 5;
            // 
            // groupBoxDustBox
            // 
            this.groupBoxDustBox.Controls.Add(this.panel6);
            this.groupBoxDustBox.Controls.Add(this.panelNoteForDustBox);
            this.groupBoxDustBox.Controls.Add(this.panelControlDustBox);
            this.groupBoxDustBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxDustBox.Location = new System.Drawing.Point(3, 3);
            this.groupBoxDustBox.Name = "groupBoxDustBox";
            this.groupBoxDustBox.Size = new System.Drawing.Size(603, 121);
            this.groupBoxDustBox.TabIndex = 0;
            this.groupBoxDustBox.TabStop = false;
            this.groupBoxDustBox.Text = "ごみ箱";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.listBoxDustBox);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(3, 15);
            this.panel6.Name = "panel6";
            this.panel6.Padding = new System.Windows.Forms.Padding(3);
            this.panel6.Size = new System.Drawing.Size(474, 81);
            this.panel6.TabIndex = 2;
            // 
            // listBoxDustBox
            // 
            this.listBoxDustBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxDustBox.FormattingEnabled = true;
            this.listBoxDustBox.ItemHeight = 12;
            this.listBoxDustBox.Location = new System.Drawing.Point(3, 3);
            this.listBoxDustBox.Name = "listBoxDustBox";
            this.listBoxDustBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxDustBox.Size = new System.Drawing.Size(468, 64);
            this.listBoxDustBox.TabIndex = 0;
            this.listBoxDustBox.SelectedIndexChanged += new System.EventHandler(this.listBoxDustBox_SelectedIndexChanged);
            // 
            // panelNoteForDustBox
            // 
            this.panelNoteForDustBox.Controls.Add(this.labelNoteForDustBox);
            this.panelNoteForDustBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelNoteForDustBox.Location = new System.Drawing.Point(3, 96);
            this.panelNoteForDustBox.Name = "panelNoteForDustBox";
            this.panelNoteForDustBox.Size = new System.Drawing.Size(474, 22);
            this.panelNoteForDustBox.TabIndex = 1;
            // 
            // labelNoteForDustBox
            // 
            this.labelNoteForDustBox.AutoSize = true;
            this.labelNoteForDustBox.Location = new System.Drawing.Point(5, 3);
            this.labelNoteForDustBox.Name = "labelNoteForDustBox";
            this.labelNoteForDustBox.Size = new System.Drawing.Size(257, 12);
            this.labelNoteForDustBox.TabIndex = 0;
            this.labelNoteForDustBox.Text = "保存されたビューのうち，閉じられたものが保管されます";
            // 
            // panelControlDustBox
            // 
            this.panelControlDustBox.Controls.Add(this.buttonRefreshDustBox);
            this.panelControlDustBox.Controls.Add(this.buttonLoadDust);
            this.panelControlDustBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelControlDustBox.Location = new System.Drawing.Point(477, 15);
            this.panelControlDustBox.Name = "panelControlDustBox";
            this.panelControlDustBox.Size = new System.Drawing.Size(123, 103);
            this.panelControlDustBox.TabIndex = 0;
            // 
            // buttonRefreshDustBox
            // 
            this.buttonRefreshDustBox.Location = new System.Drawing.Point(6, 3);
            this.buttonRefreshDustBox.Name = "buttonRefreshDustBox";
            this.buttonRefreshDustBox.Size = new System.Drawing.Size(75, 23);
            this.buttonRefreshDustBox.TabIndex = 1;
            this.buttonRefreshDustBox.Text = "一覧を更新";
            this.buttonRefreshDustBox.UseVisualStyleBackColor = true;
            this.buttonRefreshDustBox.Click += new System.EventHandler(this.buttonRefreshDustBox_Click);
            // 
            // buttonLoadDust
            // 
            this.buttonLoadDust.Location = new System.Drawing.Point(6, 32);
            this.buttonLoadDust.Name = "buttonLoadDust";
            this.buttonLoadDust.Size = new System.Drawing.Size(75, 23);
            this.buttonLoadDust.TabIndex = 0;
            this.buttonLoadDust.Text = "読み込む";
            this.buttonLoadDust.UseVisualStyleBackColor = true;
            this.buttonLoadDust.Click += new System.EventHandler(this.buttonLoadDust_Click);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuLoadState,
            this.menuSaveState,
            this.menuSaveStateAs,
            this.toolStripSeparator4,
            this.menuAddPanelFromState,
            this.toolStripSeparator1,
            this.menuOpenTimeSeriesValues,
            this.menuOpenLabel,
            this.menuOpenViewState,
            this.toolStripSeparator11,
            this.closewToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(80, 21);
            this.fileToolStripMenuItem.Text = "ファイル(&F)";
            this.fileToolStripMenuItem.DropDownOpening += new System.EventHandler(this.fileToolStripMenuItem_DropDownOpening);
            // 
            // menuLoadState
            // 
            this.menuLoadState.Name = "menuLoadState";
            this.menuLoadState.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuLoadState.Size = new System.Drawing.Size(247, 22);
            this.menuLoadState.Text = "ビュー一覧を開く(&O)";
            this.menuLoadState.Click += new System.EventHandler(this.menuLoadState_Click);
            // 
            // menuSaveState
            // 
            this.menuSaveState.Image = ((System.Drawing.Image)(resources.GetObject("menuSaveState.Image")));
            this.menuSaveState.Name = "menuSaveState";
            this.menuSaveState.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuSaveState.Size = new System.Drawing.Size(247, 22);
            this.menuSaveState.Text = "ビュー一覧を保存(&S)";
            this.menuSaveState.Click += new System.EventHandler(this.saveStateToolStripMenuItem_Click);
            // 
            // menuSaveStateAs
            // 
            this.menuSaveStateAs.Name = "menuSaveStateAs";
            this.menuSaveStateAs.Size = new System.Drawing.Size(247, 22);
            this.menuSaveStateAs.Text = "名前を付けてビュー一覧を保存(&A)";
            this.menuSaveStateAs.Click += new System.EventHandler(this.menuSaveStateAs_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(244, 6);
            // 
            // menuAddPanelFromState
            // 
            this.menuAddPanelFromState.Name = "menuAddPanelFromState";
            this.menuAddPanelFromState.Size = new System.Drawing.Size(247, 22);
            this.menuAddPanelFromState.Text = "ビュー一覧を開いて追加(&I)";
            this.menuAddPanelFromState.Click += new System.EventHandler(this.menuAddPanelFromState_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(244, 6);
            // 
            // menuOpenTimeSeriesValues
            // 
            this.menuOpenTimeSeriesValues.Name = "menuOpenTimeSeriesValues";
            this.menuOpenTimeSeriesValues.Size = new System.Drawing.Size(247, 22);
            this.menuOpenTimeSeriesValues.Text = "時系列データをインポート";
            this.menuOpenTimeSeriesValues.Click += new System.EventHandler(this.menuOpenTimeSeriesValues_Click);
            // 
            // menuOpenLabel
            // 
            this.menuOpenLabel.Name = "menuOpenLabel";
            this.menuOpenLabel.Size = new System.Drawing.Size(247, 22);
            this.menuOpenLabel.Text = "ラベルデータをインポート";
            // 
            // menuOpenViewState
            // 
            this.menuOpenViewState.Name = "menuOpenViewState";
            this.menuOpenViewState.Size = new System.Drawing.Size(247, 22);
            this.menuOpenViewState.Text = "保存されたビューを追加";
            this.menuOpenViewState.Click += new System.EventHandler(this.menuOpenViewState_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(244, 6);
            // 
            // closewToolStripMenuItem
            // 
            this.closewToolStripMenuItem.Name = "closewToolStripMenuItem";
            this.closewToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
            this.closewToolStripMenuItem.Text = "終了(&X)";
            this.closewToolStripMenuItem.Click += new System.EventHandler(this.closewToolStripMenuItem_Click);
            // 
            // menuEdit
            // 
            this.menuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewPanelToolStripMenuItem,
            this.closeMultiplePanelsToolStripMenuItem,
            this.toolStripSeparator6,
            this.menuSetLabel,
            this.menuLockEdit,
            this.toolStripSeparator14,
            this.menuLabelingBorder,
            this.toolStripSeparator13,
            this.menuJumpLabel,
            this.toolStripSeparator10,
            this.menuChangeLabelColors});
            this.menuEdit.Name = "menuEdit";
            this.menuEdit.Size = new System.Drawing.Size(59, 21);
            this.menuEdit.Text = "編集(&E)";
            this.menuEdit.DropDownOpening += new System.EventHandler(this.menuEdit_DropDownOpening);
            // 
            // addNewPanelToolStripMenuItem
            // 
            this.addNewPanelToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addNewPanelToolStripMenuItem.Image")));
            this.addNewPanelToolStripMenuItem.Name = "addNewPanelToolStripMenuItem";
            this.addNewPanelToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.addNewPanelToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.addNewPanelToolStripMenuItem.Text = "空のラベルビューを追加(&N)";
            this.addNewPanelToolStripMenuItem.Click += new System.EventHandler(this.addNewPanelToolStripMenuItem_Click);
            // 
            // closeMultiplePanelsToolStripMenuItem
            // 
            this.closeMultiplePanelsToolStripMenuItem.Image = global::MotionDataUtil.Properties.Resources.closeButton;
            this.closeMultiplePanelsToolStripMenuItem.Name = "closeMultiplePanelsToolStripMenuItem";
            this.closeMultiplePanelsToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.closeMultiplePanelsToolStripMenuItem.Text = "ビューをまとめて閉じる(&M)";
            this.closeMultiplePanelsToolStripMenuItem.Click += new System.EventHandler(this.closeMultiplePanelsToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(299, 6);
            // 
            // menuSetLabel
            // 
            this.menuSetLabel.Name = "menuSetLabel";
            this.menuSetLabel.Size = new System.Drawing.Size(302, 22);
            this.menuSetLabel.Text = "選択範囲のラベルを設定(&S)";
            // 
            // menuLockEdit
            // 
            this.menuLockEdit.Name = "menuLockEdit";
            this.menuLockEdit.Size = new System.Drawing.Size(302, 22);
            this.menuLockEdit.Text = "選択中のビューの編集をロック(&L)";
            this.menuLockEdit.Click += new System.EventHandler(this.menuLockEdit_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(299, 6);
            // 
            // menuLabelingBorder
            // 
            this.menuLabelingBorder.Name = "menuLabelingBorder";
            this.menuLabelingBorder.Size = new System.Drawing.Size(302, 22);
            this.menuLabelingBorder.Text = "選択中のビューのラベル化しきい値の設定(&T)";
            this.menuLabelingBorder.Click += new System.EventHandler(this.menuLabelingBorder_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(299, 6);
            // 
            // menuJumpLabel
            // 
            this.menuJumpLabel.Name = "menuJumpLabel";
            this.menuJumpLabel.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.menuJumpLabel.Size = new System.Drawing.Size(302, 22);
            this.menuJumpLabel.Text = "ラベル検索(&F)";
            this.menuJumpLabel.Click += new System.EventHandler(this.jumpToolStripMenuItem_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(299, 6);
            // 
            // menuChangeLabelColors
            // 
            this.menuChangeLabelColors.Name = "menuChangeLabelColors";
            this.menuChangeLabelColors.Size = new System.Drawing.Size(302, 22);
            this.menuChangeLabelColors.Text = "ラベル色の一括変更(&C)";
            this.menuChangeLabelColors.Click += new System.EventHandler(this.colorsToolStripMenuItem_Click);
            // 
            // menuView
            // 
            this.menuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuActiveView,
            this.toolStripSeparator17,
            this.menuResetScale,
            this.menuZoomSelected,
            this.menuUnzoomSelected,
            this.toolStripSeparator15,
            this.menuHideView,
            this.menuReorderView,
            this.menuViewHeight,
            this.menuCaptureViewArea,
            this.toolStripSeparator12,
            this.menuFastRenderingEnabled,
            this.toolStripSeparator9,
            this.dustBoxToolStripMenuItem,
            this.toolStripSeparator8,
            this.motionDataUtilToolStripMenuItem,
            this.menuScriptBoard});
            this.menuView.Name = "menuView";
            this.menuView.Size = new System.Drawing.Size(60, 21);
            this.menuView.Text = "表示(&V)";
            this.menuView.DropDownOpening += new System.EventHandler(this.menuView_DropDownOpening);
            // 
            // menuActiveView
            // 
            this.menuActiveView.Name = "menuActiveView";
            this.menuActiveView.Size = new System.Drawing.Size(241, 22);
            this.menuActiveView.Text = "ビューを選択(&V)";
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            this.toolStripSeparator17.Size = new System.Drawing.Size(238, 6);
            // 
            // menuResetScale
            // 
            this.menuResetScale.Image = global::MotionDataUtil.Properties.Resources.resetScale;
            this.menuResetScale.Name = "menuResetScale";
            this.menuResetScale.Size = new System.Drawing.Size(241, 22);
            this.menuResetScale.Text = "全範囲を表示(&A)";
            this.menuResetScale.Click += new System.EventHandler(this.menuResetScale_Click);
            // 
            // menuZoomSelected
            // 
            this.menuZoomSelected.Image = global::MotionDataUtil.Properties.Resources.zoomup;
            this.menuZoomSelected.Name = "menuZoomSelected";
            this.menuZoomSelected.Size = new System.Drawing.Size(241, 22);
            this.menuZoomSelected.Text = "選択範囲を拡大(&Z)";
            this.menuZoomSelected.Click += new System.EventHandler(this.menuZoomSelected_Click);
            // 
            // menuUnzoomSelected
            // 
            this.menuUnzoomSelected.Image = global::MotionDataUtil.Properties.Resources.zoomdown;
            this.menuUnzoomSelected.Name = "menuUnzoomSelected";
            this.menuUnzoomSelected.Size = new System.Drawing.Size(241, 22);
            this.menuUnzoomSelected.Text = "選択範囲へ縮小(&U)";
            this.menuUnzoomSelected.Click += new System.EventHandler(this.menuUnzoomSelected_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(238, 6);
            // 
            // menuHideView
            // 
            this.menuHideView.Image = global::MotionDataUtil.Properties.Resources.hideView16;
            this.menuHideView.Name = "menuHideView";
            this.menuHideView.Size = new System.Drawing.Size(241, 22);
            this.menuHideView.Text = "選択中のビューを隠す(&H)";
            this.menuHideView.Click += new System.EventHandler(this.menuHideView_Click);
            // 
            // menuReorderView
            // 
            this.menuReorderView.Image = global::MotionDataUtil.Properties.Resources.reorder;
            this.menuReorderView.Name = "menuReorderView";
            this.menuReorderView.Size = new System.Drawing.Size(241, 22);
            this.menuReorderView.Text = "ビューの位置を変更(&O)";
            // 
            // menuViewHeight
            // 
            this.menuViewHeight.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuMinimumViewHeight,
            this.menuSmallViewHeight,
            this.menuMediumViewHeight,
            this.menuLargeViewHeight,
            this.toolStripSeparator18,
            this.textViewHeight});
            this.menuViewHeight.Image = global::MotionDataUtil.Properties.Resources.tolarge;
            this.menuViewHeight.Name = "menuViewHeight";
            this.menuViewHeight.Size = new System.Drawing.Size(241, 22);
            this.menuViewHeight.Text = "ビューのサイズを変更";
            this.menuViewHeight.DropDownOpening += new System.EventHandler(this.menuViewHeight_DropDownOpening);
            // 
            // menuMinimumViewHeight
            // 
            this.menuMinimumViewHeight.Name = "menuMinimumViewHeight";
            this.menuMinimumViewHeight.Size = new System.Drawing.Size(160, 22);
            this.menuMinimumViewHeight.Text = "最小(&Z)";
            this.menuMinimumViewHeight.Click += new System.EventHandler(this.menuMinimumViewHeight_Click);
            // 
            // menuSmallViewHeight
            // 
            this.menuSmallViewHeight.Name = "menuSmallViewHeight";
            this.menuSmallViewHeight.Size = new System.Drawing.Size(160, 22);
            this.menuSmallViewHeight.Text = "小(&S)";
            this.menuSmallViewHeight.Click += new System.EventHandler(this.menuSmallViewHeight_Click);
            // 
            // menuMediumViewHeight
            // 
            this.menuMediumViewHeight.Name = "menuMediumViewHeight";
            this.menuMediumViewHeight.Size = new System.Drawing.Size(160, 22);
            this.menuMediumViewHeight.Text = "中(&M)";
            this.menuMediumViewHeight.Click += new System.EventHandler(this.menuMediumViewHeight_Click);
            // 
            // menuLargeViewHeight
            // 
            this.menuLargeViewHeight.Name = "menuLargeViewHeight";
            this.menuLargeViewHeight.Size = new System.Drawing.Size(160, 22);
            this.menuLargeViewHeight.Text = "大(&L)";
            this.menuLargeViewHeight.Click += new System.EventHandler(this.menuLargeViewHeight_Click);
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            this.toolStripSeparator18.Size = new System.Drawing.Size(157, 6);
            // 
            // textViewHeight
            // 
            this.textViewHeight.Name = "textViewHeight";
            this.textViewHeight.Size = new System.Drawing.Size(100, 24);
            this.textViewHeight.TextChanged += new System.EventHandler(this.textViewHeight_TextChanged);
            // 
            // menuCaptureViewArea
            // 
            this.menuCaptureViewArea.Name = "menuCaptureViewArea";
            this.menuCaptureViewArea.Size = new System.Drawing.Size(241, 22);
            this.menuCaptureViewArea.Text = "ビューエリアを画像としてコピー";
            this.menuCaptureViewArea.Click += new System.EventHandler(this.menuCaptureViewArea_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(238, 6);
            // 
            // menuFastRenderingEnabled
            // 
            this.menuFastRenderingEnabled.Name = "menuFastRenderingEnabled";
            this.menuFastRenderingEnabled.Size = new System.Drawing.Size(241, 22);
            this.menuFastRenderingEnabled.Text = "グラフを簡易描画する(&R)";
            this.menuFastRenderingEnabled.Click += new System.EventHandler(this.menuFastRenderingEnabled_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(238, 6);
            // 
            // dustBoxToolStripMenuItem
            // 
            this.dustBoxToolStripMenuItem.Image = global::MotionDataUtil.Properties.Resources.dustBoxButton;
            this.dustBoxToolStripMenuItem.Name = "dustBoxToolStripMenuItem";
            this.dustBoxToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.dustBoxToolStripMenuItem.Text = "ごみ箱(&D)";
            this.dustBoxToolStripMenuItem.Click += new System.EventHandler(this.buttonDust_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(238, 6);
            // 
            // motionDataUtilToolStripMenuItem
            // 
            this.motionDataUtilToolStripMenuItem.Name = "motionDataUtilToolStripMenuItem";
            this.motionDataUtilToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.motionDataUtilToolStripMenuItem.Text = "&Motion Data Utility";
            this.motionDataUtilToolStripMenuItem.Click += new System.EventHandler(this.motionDataUtilToolStripMenuItem_Click);
            // 
            // menuScriptBoard
            // 
            this.menuScriptBoard.Name = "menuScriptBoard";
            this.menuScriptBoard.Size = new System.Drawing.Size(241, 22);
            this.menuScriptBoard.Text = "スクリプトボード(&S)";
            this.menuScriptBoard.Click += new System.EventHandler(this.menuScriptBoard_Click);
            // 
            // menuMain
            // 
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.menuEdit,
            this.menuView,
            this.menuPlay,
            this.toolStripMenuItem1});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Size = new System.Drawing.Size(611, 25);
            this.menuMain.TabIndex = 0;
            this.menuMain.Text = "menuStrip1";
            // 
            // menuPlay
            // 
            this.menuPlay.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuPlayPause,
            this.toolStripMenuItem2,
            this.menuJumpStart,
            this.menuJumpEnd,
            this.toolStripSeparator16,
            this.menuAutoScrollEnabled,
            this.menuLoopEnabled});
            this.menuPlay.Name = "menuPlay";
            this.menuPlay.Size = new System.Drawing.Size(59, 21);
            this.menuPlay.Text = "再生(&P)";
            this.menuPlay.DropDownOpening += new System.EventHandler(this.menuPlay_DropDownOpening);
            // 
            // menuPlayPause
            // 
            this.menuPlayPause.Image = global::MotionDataUtil.Properties.Resources.run;
            this.menuPlayPause.Name = "menuPlayPause";
            this.menuPlayPause.Size = new System.Drawing.Size(226, 22);
            this.menuPlayPause.Text = "再生(&P)";
            this.menuPlayPause.Click += new System.EventHandler(this.menuPlayPause_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(226, 22);
            this.toolStripMenuItem2.Text = "停止(&S)";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // menuJumpStart
            // 
            this.menuJumpStart.Name = "menuJumpStart";
            this.menuJumpStart.Size = new System.Drawing.Size(226, 22);
            this.menuJumpStart.Text = "選択範囲の先頭へジャンプ(&H)";
            this.menuJumpStart.Click += new System.EventHandler(this.menuJumpStart_Click);
            // 
            // menuJumpEnd
            // 
            this.menuJumpEnd.Name = "menuJumpEnd";
            this.menuJumpEnd.Size = new System.Drawing.Size(226, 22);
            this.menuJumpEnd.Text = "選択範囲の末尾へジャンプ(&T)";
            this.menuJumpEnd.Click += new System.EventHandler(this.menuJumpEnd_Click);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(223, 6);
            // 
            // menuAutoScrollEnabled
            // 
            this.menuAutoScrollEnabled.Name = "menuAutoScrollEnabled";
            this.menuAutoScrollEnabled.Size = new System.Drawing.Size(226, 22);
            this.menuAutoScrollEnabled.Text = "自動スクロール(&A)";
            this.menuAutoScrollEnabled.Click += new System.EventHandler(this.menuAutoScrollEnabled_Click);
            // 
            // menuLoopEnabled
            // 
            this.menuLoopEnabled.Name = "menuLoopEnabled";
            this.menuLoopEnabled.Size = new System.Drawing.Size(226, 22);
            this.menuLoopEnabled.Text = "再生時に選択範囲をループ(&L)";
            this.menuLoopEnabled.Click += new System.EventHandler(this.menuLoopEnabled_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuLastOperation,
            this.menuOperateValueSequence,
            this.menuOperateLabelSequence});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(93, 21);
            this.toolStripMenuItem1.Text = "データ処理(&O)";
            this.toolStripMenuItem1.DropDownOpening += new System.EventHandler(this.toolStripMenuItem1_DropDownOpening);
            // 
            // menuLastOperation
            // 
            this.menuLastOperation.Enabled = false;
            this.menuLastOperation.Image = ((System.Drawing.Image)(resources.GetObject("menuLastOperation.Image")));
            this.menuLastOperation.Name = "menuLastOperation";
            this.menuLastOperation.Size = new System.Drawing.Size(192, 22);
            this.menuLastOperation.Text = "直前の処理を再適用(&A)";
            // 
            // menuOperateValueSequence
            // 
            this.menuOperateValueSequence.Image = ((System.Drawing.Image)(resources.GetObject("menuOperateValueSequence.Image")));
            this.menuOperateValueSequence.Name = "menuOperateValueSequence";
            this.menuOperateValueSequence.Size = new System.Drawing.Size(192, 22);
            this.menuOperateValueSequence.Text = "時系列データの処理(&T)";
            // 
            // menuOperateLabelSequence
            // 
            this.menuOperateLabelSequence.Image = ((System.Drawing.Image)(resources.GetObject("menuOperateLabelSequence.Image")));
            this.menuOperateLabelSequence.Name = "menuOperateLabelSequence";
            this.menuOperateLabelSequence.Size = new System.Drawing.Size(192, 22);
            this.menuOperateLabelSequence.Text = "ラベル列の処理(&L)";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelStatus,
            this.labelCurrentTime});
            this.statusStrip1.Location = new System.Drawing.Point(0, 394);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(611, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // labelStatus
            // 
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(476, 17);
            this.labelStatus.Spring = true;
            this.labelStatus.Text = "Ready";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelCurrentTime
            // 
            this.labelCurrentTime.AutoSize = false;
            this.labelCurrentTime.Name = "labelCurrentTime";
            this.labelCurrentTime.Size = new System.Drawing.Size(120, 17);
            this.labelCurrentTime.Text = "0.000 秒";
            this.labelCurrentTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SequenceViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 416);
            this.Controls.Add(this.splitContainerViewer);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuMain);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuMain;
            this.Name = "SequenceViewerForm";
            this.Text = "TSeqViewer";
            this.Load += new System.EventHandler(this.TimeValueSequenceForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TimeValueSequenceForm_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SequenceViewerForm_FormClosing);
            this.Resize += new System.EventHandler(this.TimeValueSequenceForm_Resize);
            this.panelTop.ResumeLayout(false);
            this.panelTimeSelectionGlobal.ResumeLayout(false);
            this.panelREsetScale.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictResetScale)).EndInit();
            this.panelViewerList.ResumeLayout(false);
            this.panelTimeSelection.ResumeLayout(false);
            this.panelTimeSelectionLocal.ResumeLayout(false);
            this.panelHScroll.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainerViewer.Panel1.ResumeLayout(false);
            this.splitContainerViewer.Panel1.PerformLayout();
            this.splitContainerViewer.Panel2.ResumeLayout(false);
            this.splitContainerViewer.ResumeLayout(false);
            this.groupBoxDustBox.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panelNoteForDustBox.ResumeLayout(false);
            this.panelNoteForDustBox.PerformLayout();
            this.panelControlDustBox.ResumeLayout(false);
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SaveFileDialog dialogSaveState;
        private System.Windows.Forms.OpenFileDialog dialogOpenState;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelViewerList;
        private System.Windows.Forms.Panel panelHScroll;
        private System.Windows.Forms.HScrollBar scrollTime;
        private MotionDataHandler.Misc.TimeSelectionControl timeSelectionControlGlobal;
        private System.Windows.Forms.VScrollBar vScrollBarViewer;
        private System.Windows.Forms.Panel panelTimeSelectionLocal;
        private MotionDataHandler.Misc.TimeSelectionControl timeSelectionControlLocal;
        private System.Windows.Forms.Panel panelViewerMain;
        private System.Windows.Forms.ToolStripButton buttonLoad;
        private System.Windows.Forms.ToolStripButton buttonSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton buttonRun;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSplitButton buttonScrollMode;
        private System.Windows.Forms.ToolStripMenuItem menuAutoScroll;
        private System.Windows.Forms.ToolStripMenuItem menuManualScroll;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSplitButton buttonLoopMode;
        private System.Windows.Forms.ToolStripMenuItem noLoopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loopToolStripMenuItem;
        private System.Windows.Forms.ToolStripSplitButton buttonRenderingMode;
        private System.Windows.Forms.ToolStripMenuItem fastRenderingModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trueRenderingModeToolStripMenuItem;
        private System.Windows.Forms.Panel panelTimeSelectionGlobal;
        private System.Windows.Forms.Panel panelREsetScale;
        private System.Windows.Forms.PictureBox pictResetScale;
        private System.Windows.Forms.ToolStripButton buttonNewPanel;
        private System.Windows.Forms.ToolStripButton buttonClosePanels;
        private System.Windows.Forms.SplitContainer splitContainerViewer;
        private System.Windows.Forms.ToolStripButton buttonDust;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.GroupBox groupBoxDustBox;
        private System.Windows.Forms.Panel panelControlDustBox;
        private System.Windows.Forms.Button buttonLoadDust;
        private System.Windows.Forms.Button buttonRefreshDustBox;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.ListBox listBoxDustBox;
        private System.Windows.Forms.Panel panelNoteForDustBox;
        private System.Windows.Forms.Label labelNoteForDustBox;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuLoadState;
        private System.Windows.Forms.ToolStripMenuItem menuSaveState;
        private System.Windows.Forms.ToolStripMenuItem menuSaveStateAs;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem menuAddPanelFromState;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuEdit;
        private System.Windows.Forms.ToolStripMenuItem addNewPanelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeMultiplePanelsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuView;
        private System.Windows.Forms.ToolStripMenuItem motionDataUtilToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuScriptBoard;
        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem dustBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuChangeLabelColors;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.Panel panelTimeSelection;
        private System.Windows.Forms.Panel panelTimeSelectionMargin;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel labelStatus;
        private System.Windows.Forms.ToolStripStatusLabel labelCurrentTime;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem menuLastOperation;
        private System.Windows.Forms.ToolStripMenuItem menuOperateValueSequence;
        private System.Windows.Forms.ToolStripMenuItem menuOperateLabelSequence;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem menuFastRenderingEnabled;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem menuOpenTimeSeriesValues;
        private System.Windows.Forms.ToolStripMenuItem menuOpenLabel;
        private System.Windows.Forms.ToolStripMenuItem menuOpenViewState;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripMenuItem closewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuResetScale;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripMenuItem menuZoomSelected;
        private System.Windows.Forms.ToolStripMenuItem menuPlay;
        private System.Windows.Forms.ToolStripMenuItem menuLoopEnabled;
        private System.Windows.Forms.ToolStripMenuItem menuUnzoomSelected;
        private System.Windows.Forms.ToolStripMenuItem menuSetLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.ToolStripMenuItem menuJumpLabel;
        private System.Windows.Forms.ToolStripMenuItem menuLockEdit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripMenuItem menuPlayPause;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem menuJumpStart;
        private System.Windows.Forms.ToolStripMenuItem menuJumpEnd;
        private System.Windows.Forms.ToolStripMenuItem menuAutoScrollEnabled;
        private System.Windows.Forms.ToolStripMenuItem menuActiveView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        private System.Windows.Forms.ToolStripMenuItem menuHideView;
        private System.Windows.Forms.ToolStripMenuItem menuReorderView;
        private System.Windows.Forms.ToolStripMenuItem menuLabelingBorder;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripMenuItem menuViewHeight;
        private System.Windows.Forms.ToolStripMenuItem menuMinimumViewHeight;
        private System.Windows.Forms.ToolStripMenuItem menuSmallViewHeight;
        private System.Windows.Forms.ToolStripMenuItem menuMediumViewHeight;
        private System.Windows.Forms.ToolStripMenuItem menuLargeViewHeight;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
        private System.Windows.Forms.ToolStripTextBox textViewHeight;
        private System.Windows.Forms.ToolStripMenuItem menuCaptureViewArea;
    }
}