namespace MotionDataHandler.Misc {
    partial class TimeSelectionControl {
        /// <summary> 
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
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
            this.contextMenuMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripSeparatorZoom = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.labelCursor = new System.Windows.Forms.Label();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.JumpHereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.pictCurrentTab = new System.Windows.Forms.PictureBox();
            this.pictCurrent = new System.Windows.Forms.PictureBox();
            this.pictCursor = new System.Windows.Forms.PictureBox();
            this.pictDisplay = new System.Windows.Forms.PictureBox();
            this.menuZoom = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUnzoom = new System.Windows.Forms.ToolStripMenuItem();
            this.resetScaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectVisibleRangeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inputSelectRangeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showFrameNumberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictCurrentTab)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictCurrent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictCursor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictDisplay)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuMain
            // 
            this.contextMenuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuZoom,
            this.menuUnzoom,
            this.resetScaleToolStripMenuItem,
            this.toolStripSeparatorZoom,
            this.selectVisibleRangeToolStripMenuItem,
            this.inputSelectRangeToolStripMenuItem,
            this.toolStripSeparator3,
            this.showFrameNumberToolStripMenuItem});
            this.contextMenuMain.Name = "contextMenuMain";
            this.contextMenuMain.Size = new System.Drawing.Size(236, 170);
            this.contextMenuMain.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuMain_Opening);
            // 
            // toolStripSeparatorZoom
            // 
            this.toolStripSeparatorZoom.Name = "toolStripSeparatorZoom";
            this.toolStripSeparatorZoom.Size = new System.Drawing.Size(232, 6);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(232, 6);
            // 
            // labelCursor
            // 
            this.labelCursor.AutoSize = true;
            this.labelCursor.Enabled = false;
            this.labelCursor.Location = new System.Drawing.Point(3, 0);
            this.labelCursor.Name = "labelCursor";
            this.labelCursor.Size = new System.Drawing.Size(19, 12);
            this.labelCursor.TabIndex = 4;
            this.labelCursor.Text = "0.0";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(263, 6);
            // 
            // JumpHereToolStripMenuItem
            // 
            this.JumpHereToolStripMenuItem.Name = "JumpHereToolStripMenuItem";
            this.JumpHereToolStripMenuItem.Size = new System.Drawing.Size(266, 22);
            this.JumpHereToolStripMenuItem.Text = "&Jump Here";
            this.JumpHereToolStripMenuItem.Click += new System.EventHandler(this.JumpHereToolStripMenuItem_Click);
            // 
            // pictCurrentTab
            // 
            this.pictCurrentTab.Image = global::MotionDataHandler.Properties.Resources.downTriangleTab;
            this.pictCurrentTab.Location = new System.Drawing.Point(273, 10);
            this.pictCurrentTab.Name = "pictCurrentTab";
            this.pictCurrentTab.Size = new System.Drawing.Size(8, 8);
            this.pictCurrentTab.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictCurrentTab.TabIndex = 5;
            this.pictCurrentTab.TabStop = false;
            this.pictCurrentTab.MouseLeave += new System.EventHandler(this.pictCurrentTab_MouseLeave);
            this.pictCurrentTab.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictCurrentTab_MouseMove);
            this.pictCurrentTab.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictCurrentTab_MouseDown);
            this.pictCurrentTab.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictCurrentTab_MouseUp);
            this.pictCurrentTab.MouseEnter += new System.EventHandler(this.pictCurrentTab_MouseEnter);
            // 
            // pictCurrent
            // 
            this.pictCurrent.BackColor = System.Drawing.Color.Blue;
            this.pictCurrent.Enabled = false;
            this.pictCurrent.Location = new System.Drawing.Point(66, 0);
            this.pictCurrent.Name = "pictCurrent";
            this.pictCurrent.Size = new System.Drawing.Size(1, 128);
            this.pictCurrent.TabIndex = 1;
            this.pictCurrent.TabStop = false;
            // 
            // pictCursor
            // 
            this.pictCursor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.pictCursor.Enabled = false;
            this.pictCursor.Location = new System.Drawing.Point(192, 0);
            this.pictCursor.Name = "pictCursor";
            this.pictCursor.Size = new System.Drawing.Size(1, 128);
            this.pictCursor.TabIndex = 2;
            this.pictCursor.TabStop = false;
            // 
            // pictDisplay
            // 
            this.pictDisplay.ContextMenuStrip = this.contextMenuMain;
            this.pictDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictDisplay.Image = global::MotionDataHandler.Properties.Resources.measure;
            this.pictDisplay.Location = new System.Drawing.Point(0, 0);
            this.pictDisplay.Name = "pictDisplay";
            this.pictDisplay.Size = new System.Drawing.Size(385, 41);
            this.pictDisplay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictDisplay.TabIndex = 0;
            this.pictDisplay.TabStop = false;
            this.pictDisplay.MouseLeave += new System.EventHandler(this.pictDisplay_MouseLeave);
            this.pictDisplay.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictDisplay_MouseMove);
            this.pictDisplay.Click += new System.EventHandler(this.pictDisplay_Click);
            this.pictDisplay.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pictDisplay_MouseDoubleClick);
            this.pictDisplay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictDisplay_MouseDown);
            this.pictDisplay.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictDisplay_MouseUp);
            this.pictDisplay.MouseEnter += new System.EventHandler(this.pictDisplay_MouseEnter);
            // 
            // zoomSelectedRangeToolStripMenuItem
            // 
            this.menuZoom.Image = global::MotionDataHandler.Properties.Resources.zoomin;
            this.menuZoom.Name = "zoomSelectedRangeToolStripMenuItem";
            this.menuZoom.Size = new System.Drawing.Size(235, 22);
            this.menuZoom.Text = global::MotionDataHandler.Properties.Settings.Default.Menu_ZoomSelectedRange;
            this.menuZoom.Click += new System.EventHandler(this.zoomSelectedRangeToolStripMenuItem_Click);
            // 
            // menuUnzoom
            // 
            this.menuUnzoom.Image = global::MotionDataHandler.Properties.Resources.zoomout;
            this.menuUnzoom.Name = "menuUnzoom";
            this.menuUnzoom.Size = new System.Drawing.Size(235, 22);
            this.menuUnzoom.Text = global::MotionDataHandler.Properties.Settings.Default.Menu_UnzoomSelectedRange;
            this.menuUnzoom.Click += new System.EventHandler(this.menuUnzoom_Click);
            // 
            // resetScaleToolStripMenuItem
            // 
            this.resetScaleToolStripMenuItem.Image = global::MotionDataHandler.Properties.Resources.resetScale;
            this.resetScaleToolStripMenuItem.Name = "resetScaleToolStripMenuItem";
            this.resetScaleToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.resetScaleToolStripMenuItem.Text = global::MotionDataHandler.Properties.Settings.Default.Menu_ResetScale;
            this.resetScaleToolStripMenuItem.Click += new System.EventHandler(this.resetScaleToolStripMenuItem_Click);
            // 
            // selectVisibleRangeToolStripMenuItem
            // 
            this.selectVisibleRangeToolStripMenuItem.Name = "selectVisibleRangeToolStripMenuItem";
            this.selectVisibleRangeToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.selectVisibleRangeToolStripMenuItem.Text = global::MotionDataHandler.Properties.Settings.Default.Menu_SelectVisibleRange;
            this.selectVisibleRangeToolStripMenuItem.Click += new System.EventHandler(this.selectVisibleRangeToolStripMenuItem_Click);
            // 
            // inputSelectRangeToolStripMenuItem
            // 
            this.inputSelectRangeToolStripMenuItem.Name = "inputSelectRangeToolStripMenuItem";
            this.inputSelectRangeToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.inputSelectRangeToolStripMenuItem.Text = global::MotionDataHandler.Properties.Settings.Default.Menu_InputSelectRange;
            this.inputSelectRangeToolStripMenuItem.Click += new System.EventHandler(this.inputSelectRangeToolStripMenuItem_Click);
            // 
            // showFrameNumberToolStripMenuItem
            // 
            this.showFrameNumberToolStripMenuItem.Name = "showFrameNumberToolStripMenuItem";
            this.showFrameNumberToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.showFrameNumberToolStripMenuItem.Text = global::MotionDataHandler.Properties.Settings.Default.Menu_ShowFrameIndex;
            this.showFrameNumberToolStripMenuItem.Click += new System.EventHandler(this.showFrameNumberToolStripMenuItem_Click);
            // 
            // TimeSelectionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictCurrentTab);
            this.Controls.Add(this.pictCurrent);
            this.Controls.Add(this.labelCursor);
            this.Controls.Add(this.pictCursor);
            this.Controls.Add(this.pictDisplay);
            this.Name = "TimeSelectionControl";
            this.Size = new System.Drawing.Size(385, 41);
            this.MouseLeave += new System.EventHandler(this.TimeSelectionControl_MouseLeave);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TimeSelectionControl_PreviewKeyDown);
            this.Leave += new System.EventHandler(this.TimeSelectionControl_Leave);
            this.Resize += new System.EventHandler(this.TimeSelectionControl_Resize);
            this.Enter += new System.EventHandler(this.TimeSelectionControl_Enter);
            this.contextMenuMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictCurrentTab)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictCurrent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictCursor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictDisplay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictDisplay;
        private System.Windows.Forms.PictureBox pictCurrent;
        private System.Windows.Forms.PictureBox pictCursor;
        private System.Windows.Forms.ContextMenuStrip contextMenuMain;
        private System.Windows.Forms.ToolStripMenuItem showFrameNumberToolStripMenuItem;
        private System.Windows.Forms.Label labelCursor;
        private System.Windows.Forms.ToolStripMenuItem inputSelectRangeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetScaleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuZoom;
        private System.Windows.Forms.ToolStripMenuItem selectVisibleRangeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorZoom;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem JumpHereToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripMenuItem menuUnzoom;
        private System.Windows.Forms.PictureBox pictCurrentTab;

    }
}
