namespace MotionDataHandler.Sequence {
    partial class DialogLabelColorSet {
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
            this.listViewSequence = new System.Windows.Forms.ListView();
            this.columnViewer = new System.Windows.Forms.ColumnHeader();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonRestore = new System.Windows.Forms.Button();
            this.buttonDistributeRGB = new System.Windows.Forms.Button();
            this.buttonChangeColor = new System.Windows.Forms.Button();
            this.buttonDefault = new System.Windows.Forms.Button();
            this.buttonUnify = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listViewLabel = new System.Windows.Forms.ListView();
            this.columnLabel = new System.Windows.Forms.ColumnHeader();
            this.columnColor = new System.Windows.Forms.ColumnHeader();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.buttonOK = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewSequence
            // 
            this.listViewSequence.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewSequence.CheckBoxes = true;
            this.listViewSequence.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnViewer});
            this.listViewSequence.GridLines = true;
            this.listViewSequence.Location = new System.Drawing.Point(12, 12);
            this.listViewSequence.MultiSelect = false;
            this.listViewSequence.Name = "listViewSequence";
            this.listViewSequence.Size = new System.Drawing.Size(245, 313);
            this.listViewSequence.TabIndex = 0;
            this.listViewSequence.UseCompatibleStateImageBehavior = false;
            this.listViewSequence.View = System.Windows.Forms.View.Details;
            this.listViewSequence.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listViewSequence_ItemChecked);
            this.listViewSequence.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewSequence_ItemSelectionChanged);
            // 
            // columnViewer
            // 
            this.columnViewer.Text = "Sequence";
            this.columnViewer.Width = 256;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonOK);
            this.panel1.Controls.Add(this.buttonRestore);
            this.panel1.Controls.Add(this.buttonDistributeRGB);
            this.panel1.Controls.Add(this.buttonChangeColor);
            this.panel1.Controls.Add(this.buttonDefault);
            this.panel1.Controls.Add(this.buttonUnify);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(520, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(161, 341);
            this.panel1.TabIndex = 1;
            // 
            // buttonRestore
            // 
            this.buttonRestore.Location = new System.Drawing.Point(6, 191);
            this.buttonRestore.Name = "buttonRestore";
            this.buttonRestore.Size = new System.Drawing.Size(143, 23);
            this.buttonRestore.TabIndex = 1;
            this.buttonRestore.Text = "変更を元に戻す";
            this.buttonRestore.UseVisualStyleBackColor = true;
            this.buttonRestore.Click += new System.EventHandler(this.buttonRestore_Click);
            // 
            // buttonDistributeRGB
            // 
            this.buttonDistributeRGB.Location = new System.Drawing.Point(6, 41);
            this.buttonDistributeRGB.Name = "buttonDistributeRGB";
            this.buttonDistributeRGB.Size = new System.Drawing.Size(143, 23);
            this.buttonDistributeRGB.TabIndex = 0;
            this.buttonDistributeRGB.Text = "コントラストを上げる";
            this.buttonDistributeRGB.UseVisualStyleBackColor = true;
            this.buttonDistributeRGB.Click += new System.EventHandler(this.buttonDistributeRGB_Click);
            // 
            // buttonChangeColor
            // 
            this.buttonChangeColor.Location = new System.Drawing.Point(6, 128);
            this.buttonChangeColor.Name = "buttonChangeColor";
            this.buttonChangeColor.Size = new System.Drawing.Size(143, 23);
            this.buttonChangeColor.TabIndex = 0;
            this.buttonChangeColor.Text = "色の変更";
            this.buttonChangeColor.UseVisualStyleBackColor = true;
            this.buttonChangeColor.Click += new System.EventHandler(this.buttonChangeColor_Click);
            // 
            // buttonDefault
            // 
            this.buttonDefault.Location = new System.Drawing.Point(6, 99);
            this.buttonDefault.Name = "buttonDefault";
            this.buttonDefault.Size = new System.Drawing.Size(143, 23);
            this.buttonDefault.TabIndex = 0;
            this.buttonDefault.Text = "デフォルトカラー";
            this.buttonDefault.UseVisualStyleBackColor = true;
            this.buttonDefault.Click += new System.EventHandler(this.buttonDefault_Click);
            // 
            // buttonUnify
            // 
            this.buttonUnify.Location = new System.Drawing.Point(6, 12);
            this.buttonUnify.Name = "buttonUnify";
            this.buttonUnify.Size = new System.Drawing.Size(143, 23);
            this.buttonUnify.TabIndex = 0;
            this.buttonUnify.Text = "同一ラベルの色を統一";
            this.buttonUnify.UseVisualStyleBackColor = true;
            this.buttonUnify.Click += new System.EventHandler(this.buttonUnify_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listViewSequence);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listViewLabel);
            this.splitContainer1.Size = new System.Drawing.Size(520, 341);
            this.splitContainer1.SplitterDistance = 264;
            this.splitContainer1.TabIndex = 2;
            // 
            // listViewLabel
            // 
            this.listViewLabel.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewLabel.CheckBoxes = true;
            this.listViewLabel.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnLabel,
            this.columnColor});
            this.listViewLabel.GridLines = true;
            this.listViewLabel.Location = new System.Drawing.Point(3, 12);
            this.listViewLabel.MultiSelect = false;
            this.listViewLabel.Name = "listViewLabel";
            this.listViewLabel.Size = new System.Drawing.Size(239, 313);
            this.listViewLabel.TabIndex = 0;
            this.listViewLabel.UseCompatibleStateImageBehavior = false;
            this.listViewLabel.View = System.Windows.Forms.View.Details;
            this.listViewLabel.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listViewLabel_ItemChecked);
            this.listViewLabel.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewLabel_ItemSelectionChanged);
            // 
            // columnLabel
            // 
            this.columnLabel.Text = "Label";
            this.columnLabel.Width = 154;
            // 
            // columnColor
            // 
            this.columnColor.Text = "Color";
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(66, 306);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(83, 23);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // DialogLabelColorSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(681, 341);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogLabelColorSet";
            this.Text = "DialogLabelColorSet";
            this.Load += new System.EventHandler(this.DialogLabelColorSet_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DialogLabelColorSet_FormClosing);
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewSequence;
        private System.Windows.Forms.ColumnHeader columnViewer;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView listViewLabel;
        private System.Windows.Forms.ColumnHeader columnLabel;
        private System.Windows.Forms.ColumnHeader columnColor;
        private System.Windows.Forms.Button buttonUnify;
        private System.Windows.Forms.Button buttonDistributeRGB;
        private System.Windows.Forms.Button buttonRestore;
        private System.Windows.Forms.Button buttonDefault;
        private System.Windows.Forms.Button buttonChangeColor;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Button buttonOK;
    }
}