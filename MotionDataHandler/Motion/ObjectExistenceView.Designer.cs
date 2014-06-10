namespace MotionDataHandler.Motion {
    partial class ObjectExistenceView {
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
            this.pictGraph = new System.Windows.Forms.PictureBox();
            this.panelLabel = new System.Windows.Forms.Panel();
            this.labelInfo = new System.Windows.Forms.Label();
            this.bgwRender = new System.ComponentModel.BackgroundWorker();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.timeSelectionControl1 = new MotionDataHandler.Misc.TimeSelectionControl();
            ((System.ComponentModel.ISupportInitialize)(this.pictGraph)).BeginInit();
            this.panelLabel.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictGraph
            // 
            this.pictGraph.BackColor = System.Drawing.Color.Gray;
            this.pictGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictGraph.Location = new System.Drawing.Point(0, 16);
            this.pictGraph.Name = "pictGraph";
            this.pictGraph.Size = new System.Drawing.Size(146, 109);
            this.pictGraph.TabIndex = 0;
            this.pictGraph.TabStop = false;
            this.toolTip.SetToolTip(this.pictGraph, "欠損のないデータ");
            this.pictGraph.MouseLeave += new System.EventHandler(this.pictGraph_MouseLeave);
            this.pictGraph.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictGraph_MouseMove);
            this.pictGraph.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictGraph_MouseDown);
            // 
            // panelLabel
            // 
            this.panelLabel.Controls.Add(this.labelInfo);
            this.panelLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLabel.Location = new System.Drawing.Point(0, 0);
            this.panelLabel.Name = "panelLabel";
            this.panelLabel.Size = new System.Drawing.Size(146, 16);
            this.panelLabel.TabIndex = 1;
            // 
            // labelInfo
            // 
            this.labelInfo.AutoSize = true;
            this.labelInfo.Location = new System.Drawing.Point(4, 3);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(56, 12);
            this.labelInfo.TabIndex = 0;
            this.labelInfo.Text = "No Object";
            // 
            // bgwRender
            // 
            this.bgwRender.WorkerSupportsCancellation = true;
            this.bgwRender.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwRender_DoWork);
            this.bgwRender.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwRender_RunWorkerCompleted);
            // 
            // timeSelectionControl1
            // 
            this.timeSelectionControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.timeSelectionControl1.InvertY = false;
            this.timeSelectionControl1.IsGlobalTimeLineMode = false;
            this.timeSelectionControl1.Location = new System.Drawing.Point(0, 125);
            this.timeSelectionControl1.MajorRuleHeight = 10;
            this.timeSelectionControl1.MinorRuleHeight = 6;
            this.timeSelectionControl1.Name = "timeSelectionControl1";
            this.timeSelectionControl1.ShowsCursorTimeLabel = true;
            this.timeSelectionControl1.ShowsFrameIndex = false;
            this.timeSelectionControl1.Size = new System.Drawing.Size(146, 21);
            this.timeSelectionControl1.SubMajorRuleHeight = 8;
            this.timeSelectionControl1.TabIndex = 2;
            this.toolTip.SetToolTip(this.timeSelectionControl1, "選択範囲");
            // 
            // ObjectExistsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.pictGraph);
            this.Controls.Add(this.panelLabel);
            this.Controls.Add(this.timeSelectionControl1);
            this.Name = "ObjectExistsView";
            this.Size = new System.Drawing.Size(146, 146);
            this.Resize += new System.EventHandler(this.ObjectExistenceView_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictGraph)).EndInit();
            this.panelLabel.ResumeLayout(false);
            this.panelLabel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private MotionDataHandler.Misc.TimeSelectionControl timeSelectionControl1;
        private System.Windows.Forms.Panel panelLabel;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.PictureBox pictGraph;
        private System.ComponentModel.BackgroundWorker bgwRender;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
