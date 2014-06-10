namespace MotionDataHandler.Motion {
    partial class MotionDataViewer {
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
            this.components = new System.ComponentModel.Container();
            this.contextRightClick = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.loadCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.topMostToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.keepVerticalAxisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.traceSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTraceLine = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.transparentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopTimerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dialogOpenCamera = new System.Windows.Forms.OpenFileDialog();
            this.dialogSaveCamera = new System.Windows.Forms.SaveFileDialog();
            this.dialogBackgroundColor = new System.Windows.Forms.ColorDialog();
            this.bgRender = new System.ComponentModel.BackgroundWorker();
            this.timerRender = new System.Windows.Forms.Timer(this.components);
            this.bgwWaitResize = new System.ComponentModel.BackgroundWorker();
            this.contextRightClick.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextRightClick
            // 
            this.contextRightClick.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadCameraToolStripMenuItem,
            this.saveCameraToolStripMenuItem,
            this.toolStripSeparator1,
            this.topMostToolStripMenuItem,
            this.keepVerticalAxisToolStripMenuItem,
            this.traceSelectedToolStripMenuItem,
            this.menuTraceLine,
            this.toolStripSeparator2,
            this.transparentToolStripMenuItem,
            this.backgroundColorToolStripMenuItem,
            this.stopTimerToolStripMenuItem});
            this.contextRightClick.Name = "contextRightClick";
            this.contextRightClick.Size = new System.Drawing.Size(179, 236);
            // 
            // loadCameraToolStripMenuItem
            // 
            this.loadCameraToolStripMenuItem.Name = "loadCameraToolStripMenuItem";
            this.loadCameraToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.loadCameraToolStripMenuItem.Text = "&Load Camera";
            this.loadCameraToolStripMenuItem.Click += new System.EventHandler(this.loadCameraToolStripMenuItem_Click);
            // 
            // saveCameraToolStripMenuItem
            // 
            this.saveCameraToolStripMenuItem.Name = "saveCameraToolStripMenuItem";
            this.saveCameraToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.saveCameraToolStripMenuItem.Text = "&Save Camera";
            this.saveCameraToolStripMenuItem.Click += new System.EventHandler(this.saveCameraToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(175, 6);
            // 
            // topMostToolStripMenuItem
            // 
            this.topMostToolStripMenuItem.Name = "topMostToolStripMenuItem";
            this.topMostToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.topMostToolStripMenuItem.Text = "&Top Most";
            this.topMostToolStripMenuItem.Click += new System.EventHandler(this.topMostToolStripMenuItem_Click);
            // 
            // keepVerticalAxisToolStripMenuItem
            // 
            this.keepVerticalAxisToolStripMenuItem.Name = "keepVerticalAxisToolStripMenuItem";
            this.keepVerticalAxisToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.keepVerticalAxisToolStripMenuItem.Text = "Keep &Vertical Axis";
            this.keepVerticalAxisToolStripMenuItem.Click += new System.EventHandler(this.keepVerticalAxisToolStripMenuItem_Click);
            // 
            // traceSelectedToolStripMenuItem
            // 
            this.traceSelectedToolStripMenuItem.Name = "traceSelectedToolStripMenuItem";
            this.traceSelectedToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.traceSelectedToolStripMenuItem.Text = "Trace Selected";
            this.traceSelectedToolStripMenuItem.Click += new System.EventHandler(this.traceSelectedToolStripMenuItem_Click);
            // 
            // menuTraceLine
            // 
            this.menuTraceLine.Name = "menuTraceLine";
            this.menuTraceLine.Size = new System.Drawing.Size(178, 22);
            this.menuTraceLine.Text = "Trace Line as Sight";
            this.menuTraceLine.Click += new System.EventHandler(this.menuTraceLine_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(175, 6);
            // 
            // transparentToolStripMenuItem
            // 
            this.transparentToolStripMenuItem.Checked = global::MotionDataHandler.Properties.Settings.Default.Motion_TransparentChecked;
            this.transparentToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.transparentToolStripMenuItem.Name = "transparentToolStripMenuItem";
            this.transparentToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.transparentToolStripMenuItem.Text = "Transparent";
            this.transparentToolStripMenuItem.CheckedChanged += new System.EventHandler(this.transparentToolStripMenuItem_CheckedChanged);
            this.transparentToolStripMenuItem.Click += new System.EventHandler(this.transparentToolStripMenuItem_Click);
            // 
            // backgroundColorToolStripMenuItem
            // 
            this.backgroundColorToolStripMenuItem.Name = "backgroundColorToolStripMenuItem";
            this.backgroundColorToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.backgroundColorToolStripMenuItem.Text = "Background Color";
            this.backgroundColorToolStripMenuItem.Click += new System.EventHandler(this.backgroundColorToolStripMenuItem_Click);
            // 
            // stopTimerToolStripMenuItem
            // 
            this.stopTimerToolStripMenuItem.Name = "stopTimerToolStripMenuItem";
            this.stopTimerToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.stopTimerToolStripMenuItem.Text = "Stop Timer";
            this.stopTimerToolStripMenuItem.Click += new System.EventHandler(this.stopTimerToolStripMenuItem_Click);
            // 
            // dialogOpenCamera
            // 
            this.dialogOpenCamera.DefaultExt = "cam.xml";
            this.dialogOpenCamera.FileName = "position";
            this.dialogOpenCamera.Filter = "Camera Position (*.cam.xml)|*.cam.xml";
            // 
            // dialogSaveCamera
            // 
            this.dialogSaveCamera.DefaultExt = "cam.xml";
            this.dialogSaveCamera.FileName = "position";
            this.dialogSaveCamera.Filter = "Camera Position (*.cam.xml)|*.cam.xml";
            // 
            // dialogBackgroundColor
            // 
            this.dialogBackgroundColor.AnyColor = true;
            this.dialogBackgroundColor.Color = System.Drawing.Color.Blue;
            // 
            // bgRender
            // 
            this.bgRender.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgRender_DoWork);
            this.bgRender.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgRender_RunWorkerCompleted);
            // 
            // timerRender
            // 
            this.timerRender.Tick += new System.EventHandler(this.timerRender_Tick);
            // 
            // bgwWaitResize
            // 
            this.bgwWaitResize.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwWaitResize_DoWork);
            this.bgwWaitResize.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwWaitResize_RunWorkerCompleted);
            // 
            // MotionDataViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Blue;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Name = "MotionDataViewer";
            this.Size = new System.Drawing.Size(256, 224);
            this.Load += new System.EventHandler(this.MotionDataViewer_Load);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.MotionDataViewer_MouseWheel);
            this.MouseLeave += new System.EventHandler(this.MotionDataViewer_MouseLeave);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.MotionDataViewer_PreviewKeyDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MotionDataViewer_MouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MotionDataViewer_MouseDown);
            this.Resize += new System.EventHandler(this.MotionDataViewer_Resize);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MotionDataViewer_MouseUp);
            this.MouseEnter += new System.EventHandler(this.MotionDataViewer_MouseEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MotionDataViewer_KeyDown);
            this.contextRightClick.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextRightClick;
        private System.Windows.Forms.ToolStripMenuItem topMostToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem keepVerticalAxisToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveCameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadCameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.OpenFileDialog dialogOpenCamera;
        private System.Windows.Forms.SaveFileDialog dialogSaveCamera;
        private System.Windows.Forms.ToolStripMenuItem traceSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem transparentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backgroundColorToolStripMenuItem;
        private System.Windows.Forms.ColorDialog dialogBackgroundColor;
        private System.ComponentModel.BackgroundWorker bgRender;
        private System.Windows.Forms.Timer timerRender;
        private System.Windows.Forms.ToolStripMenuItem menuTraceLine;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem stopTimerToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker bgwWaitResize;
    }
}
