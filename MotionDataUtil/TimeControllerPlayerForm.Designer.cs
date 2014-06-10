namespace MotionDataUtil {
    partial class TimeControllerPlayerForm {
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
            this.timePlayer1 = new MotionDataHandler.Misc.TimePlayer();
            this.SuspendLayout();
            // 
            // timePlayer1
            // 
            this.timePlayer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.timePlayer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timePlayer1.Location = new System.Drawing.Point(0, 0);
            this.timePlayer1.Name = "timePlayer1";
            this.timePlayer1.Size = new System.Drawing.Size(219, 89);
            this.timePlayer1.TabIndex = 0;
            // 
            // TimeControllerPlayerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(219, 89);
            this.Controls.Add(this.timePlayer1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1024, 120);
            this.MinimumSize = new System.Drawing.Size(192, 96);
            this.Name = "TimeControllerPlayerForm";
            this.Text = "TimePlayer";
            this.ResumeLayout(false);

        }

        #endregion

        private MotionDataHandler.Misc.TimePlayer timePlayer1;
    }
}