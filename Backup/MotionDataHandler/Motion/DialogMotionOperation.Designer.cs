namespace MotionDataHandler.Motion {
    partial class DialogMotionOperation {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listBoxTarget = new System.Windows.Forms.ListBox();
            this.groupBoxPreview = new System.Windows.Forms.GroupBox();
            this.motionDataViewer = new MotionDataHandler.Motion.MotionDataViewer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.panelSettings = new System.Windows.Forms.Panel();
            this.panelPreview = new System.Windows.Forms.Panel();
            this.checkIgnoreForm = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBoxPreview.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panelSettings.SuspendLayout();
            this.panelPreview.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(489, 441);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(408, 441);
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxSettings.Location = new System.Drawing.Point(0, 115);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(213, 250);
            this.groupBoxSettings.TabIndex = 0;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Settings";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBoxTarget);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(213, 115);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Target Objects";
            // 
            // listBoxTarget
            // 
            this.listBoxTarget.BackColor = System.Drawing.SystemColors.Control;
            this.listBoxTarget.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxTarget.FormattingEnabled = true;
            this.listBoxTarget.HorizontalScrollbar = true;
            this.listBoxTarget.ItemHeight = 12;
            this.listBoxTarget.Location = new System.Drawing.Point(3, 15);
            this.listBoxTarget.Name = "listBoxTarget";
            this.listBoxTarget.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listBoxTarget.Size = new System.Drawing.Size(207, 88);
            this.listBoxTarget.TabIndex = 0;
            // 
            // groupBoxPreview
            // 
            this.groupBoxPreview.Controls.Add(this.motionDataViewer);
            this.groupBoxPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxPreview.Location = new System.Drawing.Point(0, 0);
            this.groupBoxPreview.Name = "groupBoxPreview";
            this.groupBoxPreview.Size = new System.Drawing.Size(339, 365);
            this.groupBoxPreview.TabIndex = 0;
            this.groupBoxPreview.TabStop = false;
            this.groupBoxPreview.Text = "Preview";
            // 
            // motionDataViewer
            // 
            this.motionDataViewer.BackColor = System.Drawing.Color.Blue;
            this.motionDataViewer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.motionDataViewer.CanChangeSelection = false;
            this.motionDataViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.motionDataViewer.KeepVerticalAxis = true;
            this.motionDataViewer.Location = new System.Drawing.Point(3, 15);
            this.motionDataViewer.Name = "motionDataViewer";
            this.motionDataViewer.Size = new System.Drawing.Size(333, 347);
            this.motionDataViewer.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 12);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.textBoxDescription);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panelSettings);
            this.splitContainer1.Panel2.Controls.Add(this.panelPreview);
            this.splitContainer1.Size = new System.Drawing.Size(552, 423);
            this.splitContainer1.SplitterDistance = 54;
            this.splitContainer1.TabIndex = 2;
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDescription.Location = new System.Drawing.Point(0, 0);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ReadOnly = true;
            this.textBoxDescription.Size = new System.Drawing.Size(552, 54);
            this.textBoxDescription.TabIndex = 0;
            // 
            // panelSettings
            // 
            this.panelSettings.Controls.Add(this.groupBoxSettings);
            this.panelSettings.Controls.Add(this.groupBox1);
            this.panelSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSettings.Location = new System.Drawing.Point(0, 0);
            this.panelSettings.Name = "panelSettings";
            this.panelSettings.Size = new System.Drawing.Size(213, 365);
            this.panelSettings.TabIndex = 1;
            // 
            // panelPreview
            // 
            this.panelPreview.Controls.Add(this.groupBoxPreview);
            this.panelPreview.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelPreview.Location = new System.Drawing.Point(213, 0);
            this.panelPreview.Name = "panelPreview";
            this.panelPreview.Size = new System.Drawing.Size(339, 365);
            this.panelPreview.TabIndex = 0;
            // 
            // checkIgnoreForm
            // 
            this.checkIgnoreForm.AutoSize = true;
            this.checkIgnoreForm.Checked = global::MotionDataHandler.Properties.Settings.Default.DialogMotionOperation_IgnoreForm;
            this.checkIgnoreForm.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::MotionDataHandler.Properties.Settings.Default, "DialogMotionOperation_IgnoreForm", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkIgnoreForm.Location = new System.Drawing.Point(15, 445);
            this.checkIgnoreForm.Name = "checkIgnoreForm";
            this.checkIgnoreForm.Size = new System.Drawing.Size(218, 16);
            this.checkIgnoreForm.TabIndex = 3;
            this.checkIgnoreForm.Text = global::MotionDataHandler.Properties.Settings.Default.Label_DoNotShowDialogIfNoSettings;
            this.checkIgnoreForm.UseVisualStyleBackColor = true;
            // 
            // DialogMotionOperation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 476);
            this.ControlBox = true;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.checkIgnoreForm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogMotionOperation";
            this.Text = "DialogCreateObject";
            this.Load += new System.EventHandler(this.DialogMotionOperation_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DialogMotionOperation_FormClosed);
            this.Controls.SetChildIndex(this.checkIgnoreForm, 0);
            this.Controls.SetChildIndex(this.splitContainer1, 0);
            this.Controls.SetChildIndex(this.buttonCancel, 0);
            this.Controls.SetChildIndex(this.buttonOK, 0);
            this.groupBox1.ResumeLayout(false);
            this.groupBoxPreview.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panelSettings.ResumeLayout(false);
            this.panelPreview.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.GroupBox groupBoxPreview;
        private System.Windows.Forms.GroupBox groupBox1;
        protected System.Windows.Forms.GroupBox groupBoxSettings;
        private MotionDataHandler.Motion.MotionDataViewer motionDataViewer;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.ListBox listBoxTarget;
        private System.Windows.Forms.Panel panelSettings;
        private System.Windows.Forms.Panel panelPreview;
        private System.Windows.Forms.CheckBox checkIgnoreForm;
    }
}