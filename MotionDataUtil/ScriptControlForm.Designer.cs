namespace MotionDataUtil {
    partial class ScriptControlForm {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptControlForm));
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNewFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.splitHorizontal = new System.Windows.Forms.SplitContainer();
            this.splitVertical = new System.Windows.Forms.SplitContainer();
            this.panelScriptBody = new System.Windows.Forms.Panel();
            this.groupScript = new System.Windows.Forms.GroupBox();
            this.textScript = new System.Windows.Forms.TextBox();
            this.panelScriptControl = new System.Windows.Forms.Panel();
            this.labelCursor = new System.Windows.Forms.Label();
            this.buttonRun = new System.Windows.Forms.Button();
            this.groupResult = new System.Windows.Forms.GroupBox();
            this.textResult = new System.Windows.Forms.TextBox();
            this.groupHistory = new System.Windows.Forms.GroupBox();
            this.panelHistoryBody = new System.Windows.Forms.Panel();
            this.textHistory = new System.Windows.Forms.TextBox();
            this.panelHistoryControl = new System.Windows.Forms.Panel();
            this.checkNoUseReturnValue = new System.Windows.Forms.CheckBox();
            this.buttonCopyHistory = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.numHistoryCapacity = new System.Windows.Forms.NumericUpDown();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.buttonNewFile = new System.Windows.Forms.ToolStripButton();
            this.buttonOpen = new System.Windows.Forms.ToolStripButton();
            this.buttonSave = new System.Windows.Forms.ToolStripButton();
            this.panelBody = new System.Windows.Forms.Panel();
            this.timerCaretPos = new System.Windows.Forms.Timer(this.components);
            this.menuStrip.SuspendLayout();
            this.splitHorizontal.Panel1.SuspendLayout();
            this.splitHorizontal.Panel2.SuspendLayout();
            this.splitHorizontal.SuspendLayout();
            this.splitVertical.Panel1.SuspendLayout();
            this.splitVertical.Panel2.SuspendLayout();
            this.splitVertical.SuspendLayout();
            this.panelScriptBody.SuspendLayout();
            this.groupScript.SuspendLayout();
            this.panelScriptControl.SuspendLayout();
            this.groupResult.SuspendLayout();
            this.groupHistory.SuspendLayout();
            this.panelHistoryBody.SuspendLayout();
            this.panelHistoryControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHistoryCapacity)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.panelBody.SuspendLayout();
            this.SuspendLayout();
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuNewFile,
            this.menuOpen,
            this.toolStripSeparator1,
            this.menuSave,
            this.menuSaveAs,
            this.toolStripSeparator2,
            this.menuClose});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(38, 21);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // menuNewFile
            // 
            this.menuNewFile.Name = "menuNewFile";
            this.menuNewFile.Size = new System.Drawing.Size(191, 22);
            this.menuNewFile.Text = "新規作成(&N)";
            this.menuNewFile.Click += new System.EventHandler(this.menuNewFile_Click);
            // 
            // menuOpen
            // 
            this.menuOpen.Name = "menuOpen";
            this.menuOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuOpen.Size = new System.Drawing.Size(191, 22);
            this.menuOpen.Text = "開く(&O)";
            this.menuOpen.Click += new System.EventHandler(this.menuOpen_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(188, 6);
            // 
            // menuSave
            // 
            this.menuSave.Name = "menuSave";
            this.menuSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuSave.Size = new System.Drawing.Size(191, 22);
            this.menuSave.Text = "上書き保存(&S)";
            this.menuSave.Click += new System.EventHandler(this.menuSave_Click);
            // 
            // menuSaveAs
            // 
            this.menuSaveAs.Name = "menuSaveAs";
            this.menuSaveAs.Size = new System.Drawing.Size(191, 22);
            this.menuSaveAs.Text = "名前を付けて保存(&A)";
            this.menuSaveAs.Click += new System.EventHandler(this.menuSaveAs_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(188, 6);
            // 
            // menuClose
            // 
            this.menuClose.Name = "menuClose";
            this.menuClose.Size = new System.Drawing.Size(191, 22);
            this.menuClose.Text = "閉じる(&X)";
            this.menuClose.Click += new System.EventHandler(this.menuClose_Click);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(683, 25);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // splitHorizontal
            // 
            this.splitHorizontal.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitHorizontal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitHorizontal.Location = new System.Drawing.Point(0, 0);
            this.splitHorizontal.Name = "splitHorizontal";
            // 
            // splitHorizontal.Panel1
            // 
            this.splitHorizontal.Panel1.Controls.Add(this.splitVertical);
            // 
            // splitHorizontal.Panel2
            // 
            this.splitHorizontal.Panel2.Controls.Add(this.groupHistory);
            this.splitHorizontal.Panel2.Padding = new System.Windows.Forms.Padding(3);
            this.splitHorizontal.Size = new System.Drawing.Size(683, 397);
            this.splitHorizontal.SplitterDistance = 399;
            this.splitHorizontal.TabIndex = 2;
            // 
            // splitVertical
            // 
            this.splitVertical.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitVertical.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitVertical.Location = new System.Drawing.Point(0, 0);
            this.splitVertical.Name = "splitVertical";
            this.splitVertical.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitVertical.Panel1
            // 
            this.splitVertical.Panel1.Controls.Add(this.panelScriptBody);
            this.splitVertical.Panel1.Controls.Add(this.panelScriptControl);
            this.splitVertical.Panel1.Padding = new System.Windows.Forms.Padding(3);
            // 
            // splitVertical.Panel2
            // 
            this.splitVertical.Panel2.Controls.Add(this.groupResult);
            this.splitVertical.Panel2.Padding = new System.Windows.Forms.Padding(3);
            this.splitVertical.Size = new System.Drawing.Size(399, 397);
            this.splitVertical.SplitterDistance = 234;
            this.splitVertical.TabIndex = 0;
            // 
            // panelScriptBody
            // 
            this.panelScriptBody.Controls.Add(this.groupScript);
            this.panelScriptBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelScriptBody.Location = new System.Drawing.Point(3, 3);
            this.panelScriptBody.Name = "panelScriptBody";
            this.panelScriptBody.Size = new System.Drawing.Size(389, 188);
            this.panelScriptBody.TabIndex = 1;
            // 
            // groupScript
            // 
            this.groupScript.Controls.Add(this.textScript);
            this.groupScript.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupScript.Location = new System.Drawing.Point(0, 0);
            this.groupScript.Name = "groupScript";
            this.groupScript.Size = new System.Drawing.Size(389, 188);
            this.groupScript.TabIndex = 0;
            this.groupScript.TabStop = false;
            this.groupScript.Text = global::MotionDataUtil.Properties.Settings.Default.Script;
            // 
            // textScript
            // 
            this.textScript.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textScript.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textScript.Location = new System.Drawing.Point(3, 15);
            this.textScript.MaxLength = 1048576;
            this.textScript.Multiline = true;
            this.textScript.Name = "textScript";
            this.textScript.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textScript.Size = new System.Drawing.Size(383, 170);
            this.textScript.TabIndex = 0;
            this.textScript.Text = "if(true) {\r\n    Usage();\r\n} else {\r\n    var texts = Usage({\"Usage\", \"Print\"}, tru" +
                "e);\r\n    foreach(var text in texts) {\r\n        PrintLn(text);\r\n    }\r\n}";
            this.textScript.TextChanged += new System.EventHandler(this.textScript_TextChanged);
            this.textScript.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.textScript_PreviewKeyDown);
            // 
            // panelScriptControl
            // 
            this.panelScriptControl.Controls.Add(this.labelCursor);
            this.panelScriptControl.Controls.Add(this.buttonRun);
            this.panelScriptControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelScriptControl.Location = new System.Drawing.Point(3, 191);
            this.panelScriptControl.Name = "panelScriptControl";
            this.panelScriptControl.Size = new System.Drawing.Size(389, 36);
            this.panelScriptControl.TabIndex = 0;
            // 
            // labelCursor
            // 
            this.labelCursor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCursor.Location = new System.Drawing.Point(90, 11);
            this.labelCursor.Name = "labelCursor";
            this.labelCursor.Size = new System.Drawing.Size(296, 18);
            this.labelCursor.TabIndex = 1;
            this.labelCursor.Text = "1, 1";
            this.labelCursor.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // buttonRun
            // 
            this.buttonRun.Location = new System.Drawing.Point(9, 6);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(75, 23);
            this.buttonRun.TabIndex = 0;
            this.buttonRun.Text = global::MotionDataUtil.Properties.Settings.Default.Run;
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // groupResult
            // 
            this.groupResult.Controls.Add(this.textResult);
            this.groupResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupResult.Location = new System.Drawing.Point(3, 3);
            this.groupResult.Name = "groupResult";
            this.groupResult.Size = new System.Drawing.Size(389, 149);
            this.groupResult.TabIndex = 0;
            this.groupResult.TabStop = false;
            this.groupResult.Text = global::MotionDataUtil.Properties.Settings.Default.RunResult;
            // 
            // textResult
            // 
            this.textResult.BackColor = System.Drawing.SystemColors.Control;
            this.textResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textResult.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textResult.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textResult.Location = new System.Drawing.Point(3, 15);
            this.textResult.MaxLength = 131072;
            this.textResult.Multiline = true;
            this.textResult.Name = "textResult";
            this.textResult.ReadOnly = true;
            this.textResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textResult.Size = new System.Drawing.Size(383, 131);
            this.textResult.TabIndex = 0;
            // 
            // groupHistory
            // 
            this.groupHistory.Controls.Add(this.panelHistoryBody);
            this.groupHistory.Controls.Add(this.panelHistoryControl);
            this.groupHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupHistory.Location = new System.Drawing.Point(3, 3);
            this.groupHistory.Name = "groupHistory";
            this.groupHistory.Size = new System.Drawing.Size(270, 387);
            this.groupHistory.TabIndex = 0;
            this.groupHistory.TabStop = false;
            this.groupHistory.Text = global::MotionDataUtil.Properties.Settings.Default.CallHistory;
            // 
            // panelHistoryBody
            // 
            this.panelHistoryBody.Controls.Add(this.textHistory);
            this.panelHistoryBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelHistoryBody.Location = new System.Drawing.Point(3, 15);
            this.panelHistoryBody.Name = "panelHistoryBody";
            this.panelHistoryBody.Size = new System.Drawing.Size(264, 316);
            this.panelHistoryBody.TabIndex = 1;
            // 
            // textHistory
            // 
            this.textHistory.BackColor = System.Drawing.SystemColors.Control;
            this.textHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textHistory.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textHistory.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textHistory.Location = new System.Drawing.Point(0, 0);
            this.textHistory.MaxLength = 131072;
            this.textHistory.Multiline = true;
            this.textHistory.Name = "textHistory";
            this.textHistory.ReadOnly = true;
            this.textHistory.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textHistory.Size = new System.Drawing.Size(264, 316);
            this.textHistory.TabIndex = 0;
            // 
            // panelHistoryControl
            // 
            this.panelHistoryControl.Controls.Add(this.checkNoUseReturnValue);
            this.panelHistoryControl.Controls.Add(this.buttonCopyHistory);
            this.panelHistoryControl.Controls.Add(this.buttonClear);
            this.panelHistoryControl.Controls.Add(this.label1);
            this.panelHistoryControl.Controls.Add(this.numHistoryCapacity);
            this.panelHistoryControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelHistoryControl.Location = new System.Drawing.Point(3, 331);
            this.panelHistoryControl.Name = "panelHistoryControl";
            this.panelHistoryControl.Size = new System.Drawing.Size(264, 53);
            this.panelHistoryControl.TabIndex = 0;
            // 
            // checkNoUseReturnValue
            // 
            this.checkNoUseReturnValue.AutoSize = true;
            this.checkNoUseReturnValue.Checked = global::MotionDataUtil.Properties.Settings.Default.checkNoUseReturnValue;
            this.checkNoUseReturnValue.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::MotionDataUtil.Properties.Settings.Default, "checkNoUseReturnValue", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkNoUseReturnValue.Location = new System.Drawing.Point(5, 31);
            this.checkNoUseReturnValue.Name = "checkNoUseReturnValue";
            this.checkNoUseReturnValue.Size = new System.Drawing.Size(107, 16);
            this.checkNoUseReturnValue.TabIndex = 3;
            this.checkNoUseReturnValue.Text = global::MotionDataUtil.Properties.Settings.Default.NoUseReturnValue;
            this.checkNoUseReturnValue.UseVisualStyleBackColor = true;
            this.checkNoUseReturnValue.CheckedChanged += new System.EventHandler(this.checkNoUseReturnValue_CheckedChanged);
            // 
            // buttonCopyHistory
            // 
            this.buttonCopyHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCopyHistory.Location = new System.Drawing.Point(136, 26);
            this.buttonCopyHistory.Name = "buttonCopyHistory";
            this.buttonCopyHistory.Size = new System.Drawing.Size(122, 23);
            this.buttonCopyHistory.TabIndex = 2;
            this.buttonCopyHistory.Text = "クリップボードにコピー";
            this.buttonCopyHistory.UseVisualStyleBackColor = true;
            this.buttonCopyHistory.Click += new System.EventHandler(this.buttonCopyHistory_Click);
            // 
            // buttonClear
            // 
            this.buttonClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClear.Location = new System.Drawing.Point(185, 3);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(73, 23);
            this.buttonClear.TabIndex = 2;
            this.buttonClear.Text = global::MotionDataUtil.Properties.Settings.Default.ClearHistory;
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "履歴数";
            // 
            // numHistoryCapacity
            // 
            this.numHistoryCapacity.Location = new System.Drawing.Point(64, 6);
            this.numHistoryCapacity.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numHistoryCapacity.Name = "numHistoryCapacity";
            this.numHistoryCapacity.Size = new System.Drawing.Size(55, 19);
            this.numHistoryCapacity.TabIndex = 1;
            this.numHistoryCapacity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numHistoryCapacity.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numHistoryCapacity.Validated += new System.EventHandler(this.numHistoryCapacity_Validated);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "mdus";
            this.openFileDialog.Filter = "MotionDataUtility Script (*.mdus)|*.mdus";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "mdus";
            this.saveFileDialog.FileName = "script";
            this.saveFileDialog.Filter = "MotionDataUtility Script (*.mdus)|*.mdus";
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonNewFile,
            this.buttonOpen,
            this.buttonSave});
            this.toolStrip.Location = new System.Drawing.Point(0, 25);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(683, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // buttonNewFile
            // 
            this.buttonNewFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonNewFile.Image = global::MotionDataUtil.Properties.Resources.newFile;
            this.buttonNewFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonNewFile.Name = "buttonNewFile";
            this.buttonNewFile.Size = new System.Drawing.Size(23, 22);
            this.buttonNewFile.Text = "新規スクリプト";
            this.buttonNewFile.Click += new System.EventHandler(this.menuNewFile_Click);
            // 
            // buttonOpen
            // 
            this.buttonOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonOpen.Image = global::MotionDataUtil.Properties.Resources.open;
            this.buttonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(23, 22);
            this.buttonOpen.Text = "スクリプトを開く";
            this.buttonOpen.Click += new System.EventHandler(this.menuOpen_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonSave.Image = global::MotionDataUtil.Properties.Resources.save;
            this.buttonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(23, 22);
            this.buttonSave.Text = "スクリプトを保存";
            this.buttonSave.Click += new System.EventHandler(this.menuSave_Click);
            // 
            // panelBody
            // 
            this.panelBody.Controls.Add(this.splitHorizontal);
            this.panelBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBody.Location = new System.Drawing.Point(0, 50);
            this.panelBody.Name = "panelBody";
            this.panelBody.Size = new System.Drawing.Size(683, 397);
            this.panelBody.TabIndex = 4;
            // 
            // timerCaretPos
            // 
            this.timerCaretPos.Enabled = true;
            this.timerCaretPos.Tick += new System.EventHandler(this.timerCaretPos_Tick);
            // 
            // ScriptControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 447);
            this.Controls.Add(this.panelBody);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ScriptControlForm";
            this.Text = global::MotionDataUtil.Properties.Settings.Default.ScriptBoardTitle;
            this.Load += new System.EventHandler(this.ScriptControlForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ScriptControlForm_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScriptControlForm_FormClosing);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.splitHorizontal.Panel1.ResumeLayout(false);
            this.splitHorizontal.Panel2.ResumeLayout(false);
            this.splitHorizontal.ResumeLayout(false);
            this.splitVertical.Panel1.ResumeLayout(false);
            this.splitVertical.Panel2.ResumeLayout(false);
            this.splitVertical.ResumeLayout(false);
            this.panelScriptBody.ResumeLayout(false);
            this.groupScript.ResumeLayout(false);
            this.groupScript.PerformLayout();
            this.panelScriptControl.ResumeLayout(false);
            this.groupResult.ResumeLayout(false);
            this.groupResult.PerformLayout();
            this.groupHistory.ResumeLayout(false);
            this.panelHistoryBody.ResumeLayout(false);
            this.panelHistoryBody.PerformLayout();
            this.panelHistoryControl.ResumeLayout(false);
            this.panelHistoryControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHistoryCapacity)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.panelBody.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuSave;
        private System.Windows.Forms.ToolStripMenuItem menuSaveAs;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem menuClose;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.SplitContainer splitHorizontal;
        private System.Windows.Forms.SplitContainer splitVertical;
        private System.Windows.Forms.Panel panelScriptBody;
        private System.Windows.Forms.Panel panelScriptControl;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.GroupBox groupResult;
        private System.Windows.Forms.TextBox textResult;
        private System.Windows.Forms.GroupBox groupHistory;
        private System.Windows.Forms.Panel panelHistoryControl;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numHistoryCapacity;
        private System.Windows.Forms.Panel panelHistoryBody;
        private System.Windows.Forms.GroupBox groupScript;
        private System.Windows.Forms.TextBox textScript;
        private System.Windows.Forms.TextBox textHistory;
        private System.Windows.Forms.CheckBox checkNoUseReturnValue;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Panel panelBody;
        private System.Windows.Forms.ToolStripButton buttonOpen;
        private System.Windows.Forms.ToolStripButton buttonSave;
        private System.Windows.Forms.ToolStripButton buttonNewFile;
        private System.Windows.Forms.ToolStripMenuItem menuNewFile;
        private System.Windows.Forms.Button buttonCopyHistory;
        private System.Windows.Forms.Label labelCursor;
        private System.Windows.Forms.Timer timerCaretPos;




    }
}