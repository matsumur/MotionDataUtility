
namespace LLParserGenerator {
    partial class ScriptParserGeneratorTestForm {
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.textScript = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonCompile = new System.Windows.Forms.Button();
            this.tabCompile = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.textMessage = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listSettings = new System.Windows.Forms.ListView();
            this.columnName = new System.Windows.Forms.ColumnHeader();
            this.columnValue = new System.Windows.Forms.ColumnHeader();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.textParser = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.textUtility = new System.Windows.Forms.TextBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.textTemplates = new System.Windows.Forms.TextBox();
            this.timerCursor = new System.Windows.Forms.Timer(this.components);
            this.dialogOpen = new System.Windows.Forms.OpenFileDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.textStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.dialogSave = new System.Windows.Forms.SaveFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.labelCursorPos = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabCompile.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(496, 25);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(38, 21);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.saveToolStripMenuItem.Text = "&Save ";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(145, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.textScript);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabCompile);
            this.splitContainer1.Size = new System.Drawing.Size(496, 404);
            this.splitContainer1.SplitterDistance = 223;
            this.splitContainer1.TabIndex = 2;
            // 
            // textScript
            // 
            this.textScript.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textScript.Location = new System.Drawing.Point(0, 0);
            this.textScript.Multiline = true;
            this.textScript.Name = "textScript";
            this.textScript.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textScript.Size = new System.Drawing.Size(492, 188);
            this.textScript.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonCompile);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 188);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(492, 31);
            this.panel1.TabIndex = 0;
            // 
            // buttonCompile
            // 
            this.buttonCompile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCompile.Location = new System.Drawing.Point(413, 3);
            this.buttonCompile.Name = "buttonCompile";
            this.buttonCompile.Size = new System.Drawing.Size(75, 23);
            this.buttonCompile.TabIndex = 0;
            this.buttonCompile.Text = "Compile";
            this.buttonCompile.UseVisualStyleBackColor = true;
            this.buttonCompile.Click += new System.EventHandler(this.buttonCompile_Click);
            // 
            // tabCompile
            // 
            this.tabCompile.Controls.Add(this.tabPage1);
            this.tabCompile.Controls.Add(this.tabPage2);
            this.tabCompile.Controls.Add(this.tabPage3);
            this.tabCompile.Controls.Add(this.tabPage4);
            this.tabCompile.Controls.Add(this.tabPage5);
            this.tabCompile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabCompile.Location = new System.Drawing.Point(0, 0);
            this.tabCompile.Name = "tabCompile";
            this.tabCompile.SelectedIndex = 0;
            this.tabCompile.Size = new System.Drawing.Size(492, 173);
            this.tabCompile.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textMessage);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(484, 147);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Message";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // textMessage
            // 
            this.textMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textMessage.Location = new System.Drawing.Point(3, 3);
            this.textMessage.Multiline = true;
            this.textMessage.Name = "textMessage";
            this.textMessage.ReadOnly = true;
            this.textMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textMessage.Size = new System.Drawing.Size(478, 141);
            this.textMessage.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listSettings);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(484, 147);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Settings";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // listSettings
            // 
            this.listSettings.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnValue});
            this.listSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listSettings.Location = new System.Drawing.Point(3, 3);
            this.listSettings.Name = "listSettings";
            this.listSettings.Size = new System.Drawing.Size(478, 141);
            this.listSettings.TabIndex = 0;
            this.listSettings.UseCompatibleStateImageBehavior = false;
            this.listSettings.View = System.Windows.Forms.View.Details;
            // 
            // columnName
            // 
            this.columnName.Text = "パラメータ名";
            this.columnName.Width = 150;
            // 
            // columnValue
            // 
            this.columnValue.Text = "値";
            this.columnValue.Width = 200;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.textParser);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(484, 147);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Output Parser";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // textParser
            // 
            this.textParser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textParser.Location = new System.Drawing.Point(0, 0);
            this.textParser.Multiline = true;
            this.textParser.Name = "textParser";
            this.textParser.ReadOnly = true;
            this.textParser.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textParser.Size = new System.Drawing.Size(484, 147);
            this.textParser.TabIndex = 2;
            this.textParser.Click += new System.EventHandler(this.textParser_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.textUtility);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(484, 147);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Output Utility";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // textUtility
            // 
            this.textUtility.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textUtility.Location = new System.Drawing.Point(0, 0);
            this.textUtility.Multiline = true;
            this.textUtility.Name = "textUtility";
            this.textUtility.ReadOnly = true;
            this.textUtility.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textUtility.Size = new System.Drawing.Size(484, 147);
            this.textUtility.TabIndex = 2;
            this.textUtility.Click += new System.EventHandler(this.textUtility_Click);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.textTemplates);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(484, 147);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Output Templates";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // textTemplates
            // 
            this.textTemplates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textTemplates.Location = new System.Drawing.Point(3, 3);
            this.textTemplates.Multiline = true;
            this.textTemplates.Name = "textTemplates";
            this.textTemplates.ReadOnly = true;
            this.textTemplates.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textTemplates.Size = new System.Drawing.Size(478, 141);
            this.textTemplates.TabIndex = 3;
            this.textTemplates.Click += new System.EventHandler(this.textTemplates_Click);
            // 
            // timerCursor
            // 
            this.timerCursor.Interval = 150;
            this.timerCursor.Tick += new System.EventHandler(this.timerCursor_Tick);
            // 
            // dialogOpen
            // 
            this.dialogOpen.DefaultExt = "txt";
            this.dialogOpen.FileName = "EBNF.txt";
            this.dialogOpen.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textStatus,
            this.labelCursorPos});
            this.statusStrip1.Location = new System.Drawing.Point(0, 429);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(496, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // textStatus
            // 
            this.textStatus.Name = "textStatus";
            this.textStatus.Size = new System.Drawing.Size(348, 17);
            this.textStatus.Spring = true;
            this.textStatus.Text = "ready.";
            this.textStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dialogSave
            // 
            this.dialogSave.DefaultExt = "txt";
            this.dialogSave.FileName = "EBNF.txt";
            this.dialogSave.Filter = "Text Files (*.txt)|*.txt|All files|*.*";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 450);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "label1";
            // 
            // labelCursorPos
            // 
            this.labelCursorPos.Name = "labelCursorPos";
            this.labelCursorPos.Size = new System.Drawing.Size(102, 17);
            this.labelCursorPos.Text = "Line 0, Column 0";
            // 
            // ScriptParserGeneratorTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 451);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ScriptParserGeneratorTestForm";
            this.Text = "ScriptParserGenerator";
            this.Load += new System.EventHandler(this.ScriptParserGeneratorTestForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScriptParserGeneratorTestForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tabCompile.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Timer timerCursor;
        private System.Windows.Forms.OpenFileDialog dialogOpen;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel textStatus;
        private System.Windows.Forms.TextBox textScript;
        private System.Windows.Forms.Button buttonCompile;
        private System.Windows.Forms.TabControl tabCompile;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox textMessage;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.ListView listSettings;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnValue;
        private System.Windows.Forms.TextBox textUtility;
        private System.Windows.Forms.TextBox textParser;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TextBox textTemplates;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog dialogSave;
        private System.Windows.Forms.ToolStripStatusLabel labelCursorPos;
        private System.Windows.Forms.Label label1;
    }
}

