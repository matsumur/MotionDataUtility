namespace MotionDataHandler.Misc {
    partial class TimePlayer {
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.labelSpan = new System.Windows.Forms.Label();
            this.numTime = new System.Windows.Forms.NumericUpDown();
            this.panel3 = new System.Windows.Forms.Panel();
            this.buttonFirst = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel11 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel9 = new System.Windows.Forms.Panel();
            this.numIndex = new System.Windows.Forms.NumericUpDown();
            this.panel4 = new System.Windows.Forms.Panel();
            this.labelIndexFrame = new System.Windows.Forms.Label();
            this.labelIndex = new System.Windows.Forms.Label();
            this.panel10 = new System.Windows.Forms.Panel();
            this.numFPS = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.trackIndex = new System.Windows.Forms.TrackBar();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panel1.SuspendLayout();
            this.panel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTime)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel11.SuspendLayout();
            this.panel6.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numIndex)).BeginInit();
            this.panel4.SuspendLayout();
            this.panel10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFPS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackIndex)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel8);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(252, 32);
            this.panel1.TabIndex = 0;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.labelSpan);
            this.panel8.Controls.Add(this.numTime);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(64, 0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(188, 32);
            this.panel8.TabIndex = 5;
            // 
            // labelSpan
            // 
            this.labelSpan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelSpan.Location = new System.Drawing.Point(0, 0);
            this.labelSpan.Name = "labelSpan";
            this.labelSpan.Size = new System.Drawing.Size(188, 13);
            this.labelSpan.TabIndex = 0;
            this.labelSpan.Text = "Time: 0 / 0";
            this.toolTip.SetToolTip(this.labelSpan, "現在の時間/総時間");
            // 
            // numTime
            // 
            this.numTime.DecimalPlaces = 3;
            this.numTime.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.numTime.Location = new System.Drawing.Point(0, 13);
            this.numTime.Name = "numTime";
            this.numTime.Size = new System.Drawing.Size(188, 19);
            this.numTime.TabIndex = 1;
            this.numTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip.SetToolTip(this.numTime, "現在の時間の変更");
            this.numTime.ValueChanged += new System.EventHandler(this.numTime_ValueChanged);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.buttonFirst);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(32, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(32, 32);
            this.panel3.TabIndex = 1;
            // 
            // buttonFirst
            // 
            this.buttonFirst.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonFirst.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonFirst.Location = new System.Drawing.Point(0, 0);
            this.buttonFirst.Name = "buttonFirst";
            this.buttonFirst.Size = new System.Drawing.Size(32, 32);
            this.buttonFirst.TabIndex = 0;
            this.buttonFirst.Text = "<<";
            this.toolTip.SetToolTip(this.buttonFirst, "巻き戻す");
            this.buttonFirst.UseVisualStyleBackColor = true;
            this.buttonFirst.Click += new System.EventHandler(this.buttonFirst_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.buttonPlay);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(32, 32);
            this.panel2.TabIndex = 0;
            // 
            // buttonPlay
            // 
            this.buttonPlay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonPlay.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonPlay.Location = new System.Drawing.Point(0, 0);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(32, 32);
            this.buttonPlay.TabIndex = 0;
            this.buttonPlay.Text = ">";
            this.toolTip.SetToolTip(this.buttonPlay, "再生/停止");
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.panel7);
            this.panel5.Controls.Add(this.trackIndex);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 32);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(252, 54);
            this.panel5.TabIndex = 3;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.panel11);
            this.panel7.Controls.Add(this.panel10);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel7.Location = new System.Drawing.Point(0, 35);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(252, 19);
            this.panel7.TabIndex = 2;
            // 
            // panel11
            // 
            this.panel11.Controls.Add(this.panel6);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel11.Location = new System.Drawing.Point(0, 0);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(166, 19);
            this.panel11.TabIndex = 1;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.splitContainer1);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(166, 19);
            this.panel6.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel9);
            this.splitContainer1.Panel1.Controls.Add(this.panel4);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.labelIndex);
            this.splitContainer1.Size = new System.Drawing.Size(166, 19);
            this.splitContainer1.SplitterDistance = 104;
            this.splitContainer1.TabIndex = 0;
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.numIndex);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel9.Location = new System.Drawing.Point(39, 0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(65, 19);
            this.panel9.TabIndex = 1;
            // 
            // numIndex
            // 
            this.numIndex.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numIndex.Location = new System.Drawing.Point(0, 0);
            this.numIndex.Name = "numIndex";
            this.numIndex.Size = new System.Drawing.Size(65, 19);
            this.numIndex.TabIndex = 0;
            this.numIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip.SetToolTip(this.numIndex, "現在のフレーム番号の変更");
            this.numIndex.ValueChanged += new System.EventHandler(this.numIndex_ValueChanged);
            // 
            // panel4
            // 
            this.panel4.AutoSize = true;
            this.panel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel4.Controls.Add(this.labelIndexFrame);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(39, 19);
            this.panel4.TabIndex = 0;
            // 
            // labelIndexFrame
            // 
            this.labelIndexFrame.AutoSize = true;
            this.labelIndexFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelIndexFrame.Location = new System.Drawing.Point(0, 0);
            this.labelIndexFrame.Name = "labelIndexFrame";
            this.labelIndexFrame.Size = new System.Drawing.Size(39, 12);
            this.labelIndexFrame.TabIndex = 0;
            this.labelIndexFrame.Text = "Frame:";
            this.labelIndexFrame.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelIndex
            // 
            this.labelIndex.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelIndex.Location = new System.Drawing.Point(0, 0);
            this.labelIndex.Name = "labelIndex";
            this.labelIndex.Size = new System.Drawing.Size(58, 19);
            this.labelIndex.TabIndex = 0;
            this.labelIndex.Text = "/0";
            this.labelIndex.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTip.SetToolTip(this.labelIndex, "フレーム数");
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.numFPS);
            this.panel10.Controls.Add(this.label1);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel10.Location = new System.Drawing.Point(166, 0);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(86, 19);
            this.panel10.TabIndex = 0;
            // 
            // numFPS
            // 
            this.numFPS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numFPS.DecimalPlaces = 2;
            this.numFPS.Location = new System.Drawing.Point(34, 1);
            this.numFPS.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.numFPS.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numFPS.Name = "numFPS";
            this.numFPS.Size = new System.Drawing.Size(54, 19);
            this.numFPS.TabIndex = 1;
            this.numFPS.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip.SetToolTip(this.numFPS, "一秒間のフレーム数の変更");
            this.numFPS.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numFPS.ValueChanged += new System.EventHandler(this.numFPS_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "FPS:";
            // 
            // trackIndex
            // 
            this.trackIndex.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackIndex.Location = new System.Drawing.Point(0, 0);
            this.trackIndex.Maximum = 1000;
            this.trackIndex.Name = "trackIndex";
            this.trackIndex.Size = new System.Drawing.Size(252, 54);
            this.trackIndex.TabIndex = 0;
            this.trackIndex.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.toolTip.SetToolTip(this.trackIndex, "現在のフレーム番号の変更");
            this.trackIndex.Scroll += new System.EventHandler(this.trackIndex_Scroll);
            // 
            // TimePlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel1);
            this.Name = "TimePlayer";
            this.Size = new System.Drawing.Size(252, 86);
            this.Load += new System.EventHandler(this.TimePlayer_Load);
            this.panel1.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numTime)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel11.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numIndex)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel10.ResumeLayout(false);
            this.panel10.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFPS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackIndex)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelSpan;
        private System.Windows.Forms.NumericUpDown numTime;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button buttonFirst;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TrackBar trackIndex;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label labelIndex;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.NumericUpDown numIndex;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label labelIndexFrame;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.NumericUpDown numFPS;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
