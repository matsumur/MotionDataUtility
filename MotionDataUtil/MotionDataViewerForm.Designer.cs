namespace MotionDataUtil {
    partial class MotionDataViewerForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MotionDataViewerForm));
            this.motionDataViewer1 = new MotionDataHandler.Motion.MotionDataViewer();
            this.SuspendLayout();
            // 
            // motionDataViewer1
            // 
            this.motionDataViewer1.AutoSize = true;
            this.motionDataViewer1.BackColor = System.Drawing.Color.Blue;
            this.motionDataViewer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.motionDataViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.motionDataViewer1.Location = new System.Drawing.Point(0, 0);
            this.motionDataViewer1.Name = "motionDataViewer1";
            this.motionDataViewer1.Size = new System.Drawing.Size(292, 272);
            this.motionDataViewer1.TabIndex = 0;
            this.motionDataViewer1.Load += new System.EventHandler(this.motionDataViewer1_Load);
            // 
            // MotionDataViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 272);
            this.Controls.Add(this.motionDataViewer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MotionDataViewerForm";
            this.Text = "MotionDataViewerForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MotionDataViewerForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MotionDataHandler.Motion.MotionDataViewer motionDataViewer1;
    }
}