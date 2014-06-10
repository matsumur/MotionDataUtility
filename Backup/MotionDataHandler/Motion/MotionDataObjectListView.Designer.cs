namespace MotionDataHandler.Motion {
    partial class MotionDataObjectListView {
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("/");
            this.panelName = new System.Windows.Forms.Panel();
            this.textName = new System.Windows.Forms.TextBox();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.buttonGather = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.panelGroup = new System.Windows.Forms.Panel();
            this.listObjectInfo = new System.Windows.Forms.ListView();
            this.columnNames = new System.Windows.Forms.ColumnHeader();
            this.columnNumber = new System.Windows.Forms.ColumnHeader();
            this.columnType = new System.Windows.Forms.ColumnHeader();
            this.contextRightClick = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceFirstToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dialogColor = new System.Windows.Forms.ColorDialog();
            this.panelProperty = new System.Windows.Forms.Panel();
            this.numericColorAlpha = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.checkVisible = new System.Windows.Forms.CheckBox();
            this.textColor = new System.Windows.Forms.TextBox();
            this.splitAll = new System.Windows.Forms.SplitContainer();
            this.panelGroupTree = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.treeViewInfo = new System.Windows.Forms.TreeView();
            this.groupBoxInformation = new System.Windows.Forms.GroupBox();
            this.panelListView = new System.Windows.Forms.Panel();
            this.panelGroupName = new System.Windows.Forms.Panel();
            this.panelGroupText = new System.Windows.Forms.Panel();
            this.textGroup = new System.Windows.Forms.TextBox();
            this.panel11 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panelName.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.contextRightClick.SuspendLayout();
            this.panelProperty.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericColorAlpha)).BeginInit();
            this.splitAll.Panel1.SuspendLayout();
            this.splitAll.Panel2.SuspendLayout();
            this.splitAll.SuspendLayout();
            this.panelGroupTree.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBoxInformation.SuspendLayout();
            this.panelListView.SuspendLayout();
            this.panelGroupName.SuspendLayout();
            this.panelGroupText.SuspendLayout();
            this.panel11.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelName
            // 
            this.panelName.Controls.Add(this.textName);
            this.panelName.Controls.Add(this.panelButtons);
            this.panelName.Controls.Add(this.panelGroup);
            this.panelName.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelName.Location = new System.Drawing.Point(3, 230);
            this.panelName.Name = "panelName";
            this.panelName.Size = new System.Drawing.Size(279, 19);
            this.panelName.TabIndex = 2;
            // 
            // textName
            // 
            this.textName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textName.Location = new System.Drawing.Point(0, 0);
            this.textName.Name = "textName";
            this.textName.Size = new System.Drawing.Size(158, 19);
            this.textName.TabIndex = 0;
            this.toolTip.SetToolTip(this.textName, "選択中のオブジェクト名の変更");
            this.textName.Validated += new System.EventHandler(this.textName_Validated);
            this.textName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textName_KeyDown);
            // 
            // panelButtons
            // 
            this.panelButtons.AutoSize = true;
            this.panelButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelButtons.Controls.Add(this.buttonGather);
            this.panelButtons.Controls.Add(this.buttonUp);
            this.panelButtons.Controls.Add(this.buttonDown);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelButtons.Location = new System.Drawing.Point(158, 0);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(121, 19);
            this.panelButtons.TabIndex = 3;
            // 
            // buttonGather
            // 
            this.buttonGather.AutoSize = true;
            this.buttonGather.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonGather.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonGather.Location = new System.Drawing.Point(0, 0);
            this.buttonGather.Name = "buttonGather";
            this.buttonGather.Size = new System.Drawing.Size(49, 19);
            this.buttonGather.TabIndex = 0;
            this.buttonGather.Text = "Gather";
            this.toolTip.SetToolTip(this.buttonGather, "選択中のオブジェクトの並び順を一か所に集める");
            this.buttonGather.UseVisualStyleBackColor = true;
            this.buttonGather.Click += new System.EventHandler(this.buttonGather_Click);
            // 
            // buttonUp
            // 
            this.buttonUp.AutoSize = true;
            this.buttonUp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonUp.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonUp.Location = new System.Drawing.Point(49, 0);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(29, 19);
            this.buttonUp.TabIndex = 1;
            this.buttonUp.Text = "Up";
            this.toolTip.SetToolTip(this.buttonUp, "選択中のオブジェクトの並び順を一つ上に");
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // buttonDown
            // 
            this.buttonDown.AutoSize = true;
            this.buttonDown.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonDown.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonDown.Location = new System.Drawing.Point(78, 0);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(43, 19);
            this.buttonDown.TabIndex = 2;
            this.buttonDown.Text = "Down";
            this.toolTip.SetToolTip(this.buttonDown, "選択中のオブジェクトの並び順を一つ下に");
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // panelGroup
            // 
            this.panelGroup.AutoSize = true;
            this.panelGroup.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelGroup.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelGroup.Location = new System.Drawing.Point(0, 0);
            this.panelGroup.Name = "panelGroup";
            this.panelGroup.Size = new System.Drawing.Size(0, 19);
            this.panelGroup.TabIndex = 0;
            // 
            // listObjectInfo
            // 
            this.listObjectInfo.AllowColumnReorder = true;
            this.listObjectInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnNames,
            this.columnNumber,
            this.columnType});
            this.listObjectInfo.ContextMenuStrip = this.contextRightClick;
            this.listObjectInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listObjectInfo.FullRowSelect = true;
            this.listObjectInfo.GridLines = true;
            this.listObjectInfo.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listObjectInfo.HideSelection = false;
            this.listObjectInfo.Location = new System.Drawing.Point(0, 0);
            this.listObjectInfo.Name = "listObjectInfo";
            this.listObjectInfo.Size = new System.Drawing.Size(279, 195);
            this.listObjectInfo.TabIndex = 0;
            this.toolTip.SetToolTip(this.listObjectInfo, "現在のグループのオブジェクト");
            this.listObjectInfo.UseCompatibleStateImageBehavior = false;
            this.listObjectInfo.View = System.Windows.Forms.View.Details;
            this.listObjectInfo.SelectedIndexChanged += new System.EventHandler(this.listObjectInfo_SelectedIndexChanged);
            // 
            // columnNames
            // 
            this.columnNames.Text = "Name";
            this.columnNames.Width = 160;
            // 
            // columnNumber
            // 
            this.columnNumber.Text = "#";
            this.columnNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnNumber.Width = 25;
            // 
            // columnType
            // 
            this.columnType.Text = "Type";
            this.columnType.Width = 79;
            // 
            // contextRightClick
            // 
            this.contextRightClick.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeToolStripMenuItem,
            this.showToolStripMenuItem,
            this.replaceFirstToToolStripMenuItem});
            this.contextRightClick.Name = "contextRightClick";
            this.contextRightClick.Size = new System.Drawing.Size(190, 70);
            this.contextRightClick.Opening += new System.ComponentModel.CancelEventHandler(this.contextRightClick_Opening);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.showToolStripMenuItem.Text = "Show";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // replaceFirstToToolStripMenuItem
            // 
            this.replaceFirstToToolStripMenuItem.Name = "replaceFirstToToolStripMenuItem";
            this.replaceFirstToToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.replaceFirstToToolStripMenuItem.Text = "Replace first \'_\' to \'/\'";
            this.replaceFirstToToolStripMenuItem.Click += new System.EventHandler(this.replaceFirstToToolStripMenuItem_Click);
            // 
            // panelProperty
            // 
            this.panelProperty.Controls.Add(this.numericColorAlpha);
            this.panelProperty.Controls.Add(this.label3);
            this.panelProperty.Controls.Add(this.label4);
            this.panelProperty.Controls.Add(this.checkVisible);
            this.panelProperty.Controls.Add(this.textColor);
            this.panelProperty.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelProperty.Location = new System.Drawing.Point(3, 249);
            this.panelProperty.Name = "panelProperty";
            this.panelProperty.Size = new System.Drawing.Size(279, 20);
            this.panelProperty.TabIndex = 3;
            // 
            // numericColorAlpha
            // 
            this.numericColorAlpha.AllowDrop = true;
            this.numericColorAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericColorAlpha.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericColorAlpha.Location = new System.Drawing.Point(213, 1);
            this.numericColorAlpha.Name = "numericColorAlpha";
            this.numericColorAlpha.Size = new System.Drawing.Size(46, 19);
            this.numericColorAlpha.TabIndex = 3;
            this.numericColorAlpha.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip.SetToolTip(this.numericColorAlpha, "オブジェクトの不透明度を変更");
            this.numericColorAlpha.ValueChanged += new System.EventHandler(this.numericColorAlpha_ValueChanged);
            this.numericColorAlpha.MouseClick += new System.Windows.Forms.MouseEventHandler(this.numericColorAlpha_MouseClick);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(265, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(11, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "%";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(171, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "Alpha:";
            // 
            // checkVisible
            // 
            this.checkVisible.AutoSize = true;
            this.checkVisible.Location = new System.Drawing.Point(3, 2);
            this.checkVisible.Name = "checkVisible";
            this.checkVisible.Size = new System.Drawing.Size(59, 16);
            this.checkVisible.TabIndex = 0;
            this.checkVisible.Text = "Visible";
            this.toolTip.SetToolTip(this.checkVisible, "3Dビュー内で表示するかどうか");
            this.checkVisible.UseVisualStyleBackColor = true;
            this.checkVisible.CheckStateChanged += new System.EventHandler(this.checkVisible_CheckStateChanged);
            // 
            // textColor
            // 
            this.textColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textColor.Location = new System.Drawing.Point(68, 0);
            this.textColor.Name = "textColor";
            this.textColor.ReadOnly = true;
            this.textColor.Size = new System.Drawing.Size(97, 19);
            this.textColor.TabIndex = 1;
            this.textColor.Text = "White";
            this.toolTip.SetToolTip(this.textColor, "クリックでオブジェクトの色を変更");
            this.textColor.Click += new System.EventHandler(this.textColor_Click);
            this.textColor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textColor_KeyDown);
            // 
            // splitAll
            // 
            this.splitAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitAll.Location = new System.Drawing.Point(0, 0);
            this.splitAll.Name = "splitAll";
            this.splitAll.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitAll.Panel1
            // 
            this.splitAll.Panel1.Controls.Add(this.panelGroupTree);
            // 
            // splitAll.Panel2
            // 
            this.splitAll.Panel2.Controls.Add(this.groupBoxInformation);
            this.splitAll.Size = new System.Drawing.Size(285, 366);
            this.splitAll.SplitterDistance = 90;
            this.splitAll.TabIndex = 0;
            // 
            // panelGroupTree
            // 
            this.panelGroupTree.Controls.Add(this.groupBox2);
            this.panelGroupTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGroupTree.Location = new System.Drawing.Point(0, 0);
            this.panelGroupTree.Name = "panelGroupTree";
            this.panelGroupTree.Size = new System.Drawing.Size(285, 90);
            this.panelGroupTree.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.treeViewInfo);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(285, 90);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "<Object Group>";
            // 
            // treeViewInfo
            // 
            this.treeViewInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewInfo.HideSelection = false;
            this.treeViewInfo.Location = new System.Drawing.Point(3, 15);
            this.treeViewInfo.Name = "treeViewInfo";
            treeNode1.Name = "root";
            treeNode1.Text = "/";
            this.treeViewInfo.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.treeViewInfo.Size = new System.Drawing.Size(279, 72);
            this.treeViewInfo.TabIndex = 0;
            this.toolTip.SetToolTip(this.treeViewInfo, "グループ");
            this.treeViewInfo.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewInfo_AfterSelect);
            // 
            // groupBoxInformation
            // 
            this.groupBoxInformation.Controls.Add(this.panelListView);
            this.groupBoxInformation.Controls.Add(this.panelGroupName);
            this.groupBoxInformation.Controls.Add(this.panelName);
            this.groupBoxInformation.Controls.Add(this.panelProperty);
            this.groupBoxInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxInformation.Location = new System.Drawing.Point(0, 0);
            this.groupBoxInformation.Name = "groupBoxInformation";
            this.groupBoxInformation.Size = new System.Drawing.Size(285, 272);
            this.groupBoxInformation.TabIndex = 0;
            this.groupBoxInformation.TabStop = false;
            this.groupBoxInformation.Text = "<Object Information>";
            // 
            // panelListView
            // 
            this.panelListView.Controls.Add(this.listObjectInfo);
            this.panelListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelListView.Location = new System.Drawing.Point(3, 15);
            this.panelListView.Name = "panelListView";
            this.panelListView.Size = new System.Drawing.Size(279, 195);
            this.panelListView.TabIndex = 0;
            // 
            // panelGroupName
            // 
            this.panelGroupName.Controls.Add(this.panelGroupText);
            this.panelGroupName.Controls.Add(this.panel11);
            this.panelGroupName.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelGroupName.Location = new System.Drawing.Point(3, 210);
            this.panelGroupName.Name = "panelGroupName";
            this.panelGroupName.Size = new System.Drawing.Size(279, 20);
            this.panelGroupName.TabIndex = 1;
            // 
            // panelGroupText
            // 
            this.panelGroupText.Controls.Add(this.textGroup);
            this.panelGroupText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGroupText.Location = new System.Drawing.Point(48, 0);
            this.panelGroupText.Name = "panelGroupText";
            this.panelGroupText.Size = new System.Drawing.Size(231, 20);
            this.panelGroupText.TabIndex = 1;
            // 
            // textGroup
            // 
            this.textGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textGroup.Location = new System.Drawing.Point(0, 0);
            this.textGroup.Name = "textGroup";
            this.textGroup.Size = new System.Drawing.Size(231, 19);
            this.textGroup.TabIndex = 0;
            this.toolTip.SetToolTip(this.textGroup, "選択中のオブジェクトの所属グループの変更");
            this.textGroup.Validated += new System.EventHandler(this.textGroup_Validated);
            this.textGroup.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textGroup_KeyDown);
            // 
            // panel11
            // 
            this.panel11.Controls.Add(this.label5);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel11.Location = new System.Drawing.Point(0, 0);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(48, 20);
            this.panel11.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "Group:";
            // 
            // MotionDataObjectListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitAll);
            this.Name = "MotionDataObjectListView";
            this.Size = new System.Drawing.Size(285, 366);
            this.panelName.ResumeLayout(false);
            this.panelName.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.panelButtons.PerformLayout();
            this.contextRightClick.ResumeLayout(false);
            this.panelProperty.ResumeLayout(false);
            this.panelProperty.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericColorAlpha)).EndInit();
            this.splitAll.Panel1.ResumeLayout(false);
            this.splitAll.Panel2.ResumeLayout(false);
            this.splitAll.ResumeLayout(false);
            this.panelGroupTree.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBoxInformation.ResumeLayout(false);
            this.panelListView.ResumeLayout(false);
            this.panelGroupName.ResumeLayout(false);
            this.panelGroupText.ResumeLayout(false);
            this.panelGroupText.PerformLayout();
            this.panel11.ResumeLayout(false);
            this.panel11.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColorDialog dialogColor;
        private System.Windows.Forms.Panel panelName;
        private System.Windows.Forms.Panel panelGroup;
        private System.Windows.Forms.ListView listObjectInfo;
        private System.Windows.Forms.ColumnHeader columnNames;
        private System.Windows.Forms.ColumnHeader columnNumber;
        private System.Windows.Forms.ColumnHeader columnType;
        private System.Windows.Forms.ContextMenuStrip contextRightClick;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button buttonGather;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.TextBox textName;
        private System.Windows.Forms.Panel panelProperty;
        private System.Windows.Forms.SplitContainer splitAll;
        private System.Windows.Forms.TreeView treeViewInfo;
        private System.Windows.Forms.Panel panelGroupTree;
        private System.Windows.Forms.CheckBox checkVisible;
        private System.Windows.Forms.NumericUpDown numericColorAlpha;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textColor;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panelListView;
        private System.Windows.Forms.Panel panelGroupName;
        private System.Windows.Forms.Panel panelGroupText;
        private System.Windows.Forms.TextBox textGroup;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBoxInformation;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripMenuItem replaceFirstToToolStripMenuItem;
    }
}
