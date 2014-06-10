namespace MotionDataHandler.Misc {
    partial class WaitForForm {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WaitForForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelTitle = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labelElapse = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.labelWaitFor = new System.Windows.Forms.Label();
            this.bgWorker = new System.ComponentModel.BackgroundWorker();
            this.timerNotify = new System.Windows.Forms.Timer(this.components);
            this.timerBegin = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Controls.Add(this.labelTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(344, 30);
            this.panel1.TabIndex = 0;
            // 
            // labelTitle
            // 
            this.labelTitle.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Image = global::MotionDataHandler.Properties.Resources.circle_bar;
            this.labelTitle.Location = new System.Drawing.Point(0, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Padding = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.labelTitle.Size = new System.Drawing.Size(344, 30);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Operation";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.Controls.Add(this.labelElapse);
            this.panel2.Controls.Add(this.buttonCancel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 70);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(344, 47);
            this.panel2.TabIndex = 1;
            // 
            // labelElapse
            // 
            this.labelElapse.AutoSize = true;
            this.labelElapse.Location = new System.Drawing.Point(12, 26);
            this.labelElapse.Name = "labelElapse";
            this.labelElapse.Size = new System.Drawing.Size(25, 12);
            this.labelElapse.TabIndex = 1;
            this.labelElapse.Text = "0:00";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Enabled = false;
            this.buttonCancel.Location = new System.Drawing.Point(257, 12);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 30);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(16, 40);
            this.panel3.TabIndex = 2;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.panel7);
            this.panel5.Controls.Add(this.panel4);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel5.Location = new System.Drawing.Point(16, 54);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(328, 16);
            this.panel5.TabIndex = 4;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.ProgressBar);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(0, 0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(312, 16);
            this.panel7.TabIndex = 5;
            // 
            // ProgressBar
            // 
            this.ProgressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProgressBar.Location = new System.Drawing.Point(0, 0);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(312, 16);
            this.ProgressBar.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(312, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(16, 16);
            this.panel4.TabIndex = 4;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.labelWaitFor);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(16, 30);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(328, 24);
            this.panel6.TabIndex = 5;
            // 
            // labelWaitFor
            // 
            this.labelWaitFor.AutoSize = true;
            this.labelWaitFor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelWaitFor.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelWaitFor.Location = new System.Drawing.Point(0, 0);
            this.labelWaitFor.Name = "labelWaitFor";
            this.labelWaitFor.Padding = new System.Windows.Forms.Padding(3, 5, 0, 0);
            this.labelWaitFor.Size = new System.Drawing.Size(65, 18);
            this.labelWaitFor.TabIndex = 0;
            this.labelWaitFor.Text = "Start Up";
            this.labelWaitFor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // bgWorker
            // 
            this.bgWorker.WorkerReportsProgress = true;
            this.bgWorker.WorkerSupportsCancellation = true;
            this.bgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorker_DoWork);
            this.bgWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorker_RunWorkerCompleted);
            this.bgWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.OnProgressChanged);
            // 
            // timerNotify
            // 
            this.timerNotify.Interval = 50;
            this.timerNotify.Tick += new System.EventHandler(this.timerNotify_Tick);
            // 
            // timerBegin
            // 
            this.timerBegin.Interval = 10;
            this.timerBegin.Tick += new System.EventHandler(this.timerBegin_Tick);
            // 
            // WaitForForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(344, 117);
            this.ControlBox = false;
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WaitForForm";
            this.Text = "Processing...";
            this.Load += new System.EventHandler(this.WaitForForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WaitForForm_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label labelWaitFor;
        private System.ComponentModel.BackgroundWorker bgWorker;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Timer timerNotify;
        private System.Windows.Forms.Label labelElapse;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Timer timerBegin;


    }
}