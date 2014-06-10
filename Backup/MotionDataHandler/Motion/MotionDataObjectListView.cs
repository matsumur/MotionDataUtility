using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
namespace MotionDataHandler.Motion {
    using Misc;

    public partial class MotionDataObjectListView : UserControl {
        private readonly object _lockDataSet = new object();
        MotionDataSet _dataSet;
        string _currentGroup = "";
        Dictionary<string, TreeNode> _nodeByPath = new Dictionary<string, TreeNode>();
        readonly Mutex _mutex = new Mutex();
        bool _selectionChanging = false;
        bool _infoContentChanging = false;
        readonly object _lockListView = new object();
        readonly string _defaultName = "...";

        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        public MotionDataObjectListView() {
            InitializeComponent();
            this.Disposed += MotionDataObjectListView_Disposed;
        }

        /// <summary>
        /// オブジェクトにMotionDataSetを関連付けます
        /// </summary>
        /// <param name="dataSet"></param>
        public void AttachDataSet(MotionDataSet dataSet) {
            lock(_lockDataSet) {
                DetachDataSet();
                _dataSet = dataSet;
                if(_dataSet != null) {
                    _dataSet.ObjectSelectionChanged += OnSelectionChanged;
                    _dataSet.ObjectInfoSetChanged += OnObjectInfoSetChanged;
                }
                OnObjectInfoSetChanged(this, null);
            }
        }
        /// <summary>
        /// オブジェクトからMotionDataSetの関連を解きます
        /// </summary>
        public void DetachDataSet() {
            lock(_lockDataSet) {
                if(_dataSet != null) {
                    _dataSet.ObjectSelectionChanged -= OnSelectionChanged;
                    _dataSet.ObjectInfoSetChanged -= OnObjectInfoSetChanged;
                    _dataSet = null;
                }
            }
        }
        /// <summary>
        /// オブジェクトが破棄されたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MotionDataObjectListView_Disposed(object sender, EventArgs e) {
            DetachDataSet();
        }


        /// <summary>
        /// 外部でMotionDataSetのMotionObjectInfoの内容が変更されたので，ツリー及びオブジェクトの選択を変更する．
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnObjectInfoSetChanged(object sender, EventArgs e) {
            // フォーム以外のスレッドから呼び出された場合にフォームのスレッドで処理する
            if(treeViewInfo.InvokeRequired) {
                treeViewInfo.BeginInvoke(new EventHandler(OnObjectInfoSetChanged), sender, e);
                return;
            }
            MotionDataSet dataSet = _dataSet;
            if(dataSet == null)
                return;
            // 多重呼び出しの禁止
            if(!_mutex.WaitOne(0))
                return;
            try {
                if(_selectionChanging)
                    return;
                _selectionChanging = true;
                try {
                    HashSet<string> newGroups = new HashSet<string>();
                    newGroups.Add("");
                    foreach(var info in dataSet.GetObjectInfoList()) {
                        string path = PathEx.DirName(info.Name);
                        while(path != "") {
                            if(!newGroups.Contains(path)) {
                                newGroups.Add(path);
                            }
                            path = PathEx.DirName(path);
                        }
                    }
                    HashSet<string> oldGroups = new HashSet<string>(_nodeByPath.Keys);
                    if(newGroups.All(g => oldGroups.Contains(g)) && oldGroups.All(g => newGroups.Contains(g))) {
                        // 変化なし
                    } else {
                        treeViewInfo.SuspendLayout();
                        try {
                            treeViewInfo.Nodes.Clear();
                            _nodeByPath.Clear();
                            _nodeByPath[""] = new TreeNode("/");
                            foreach(var info in dataSet.GetObjectInfoList()) {
                                string path = PathEx.DirName(info.Name);
                                while(path != "") {
                                    if(!_nodeByPath.ContainsKey(path)) {
                                        _nodeByPath[path] = new TreeNode(PathEx.BaseName(path));
                                    }
                                    path = PathEx.DirName(path);
                                }
                            }
                            foreach(var pair in _nodeByPath) {
                                string path = pair.Key;
                                if(path != "") {
                                    string parent = PathEx.DirName(path);
                                    _nodeByPath[parent].Nodes.Add(pair.Value);
                                }
                            }
                            treeViewInfo.Nodes.Add(_nodeByPath[""]);

                        } finally { treeViewInfo.ResumeLayout(); }
                    }
                    this.DoSelectedGroupChanged();
                    this.DoUpdateObjectInfoControls();
                } finally { _selectionChanging = false; }
            } finally { _mutex.ReleaseMutex(); }
        }

        private void setText(Control control, string text) {
            if(this.InvokeRequired) {
                this.BeginInvoke(new Action<Control, string>(setText), control, text);
                return;
            }
            control.Text = text;
        }

        private void setEnabled(Control control, bool enabled) {
            if(this.InvokeRequired) {
                this.Invoke(new Action<Control, bool>(setEnabled), control, enabled);
                return;
            }
            control.Enabled = enabled;
        }

        private string formatNameText(MotionObjectInfo info) {
            MotionDataSet dataSet = _dataSet;
            if(dataSet == null)
                return "";
            string name = info.Name;
            if(_currentGroup != "") {
                name = name.Substring(_currentGroup.Length + 1);
            }
            return formatNameText(name, dataSet.IsSelecting(info), info.IsVisible);
        }
        private static string formatNameText(string name, bool selected, bool visible) {
            if(selected && visible) {
                return "> " + name;
            } else if(selected) {
                return ">* " + name;
            } else if(visible) {
                return name;
            } else {
                return "* " + name;
            }
        }



        private void invokeControl(Control control, Action action) {
            if(control.InvokeRequired) {
                control.BeginInvoke(new Action<Control, Action>(invokeControl), control, action);
                return;
            }
            action();
        }

        private void DoSelectedGroupChanged() {
            // フォーム以外のスレッドから呼び出された場合にフォームのスレッドで処理する
            if(this.InvokeRequired) {
                this.Invoke(new Action(this.DoSelectedGroupChanged));
                return;
            }
            MotionDataSet dataSet = _dataSet;
            if(dataSet == null)
                return;
            if(!_mutex.WaitOne(0))
                return;
            try {
                // グループの選択
                if(_currentGroup == "") {
                    textGroup.Text = PathEx.PathSeparator.ToString();
                    groupBoxInformation.Text = "<Object Information>";
                } else {
                    textGroup.Text = _currentGroup;
                    groupBoxInformation.Text = string.Format("<Object Information - {0}>", _currentGroup);
                }

                // グループ外のオブジェクトの選択をやめる
                foreach(var info in dataSet.GetObjectInfoList()) {
                    if(!PathEx.IsSubPath(info.Name, _currentGroup)) {
                        dataSet.SelectObjects(false, info);
                    }
                }
                // 前回のフォーカスを持ったオブジェクト
                uint? prevFocusedId = null;
                ListViewItem focusedItem = listObjectInfo.FocusedItem;
                if(focusedItem != null) {
                    prevFocusedId = uint.Parse(focusedItem.SubItems[1].Text);
                }
                // 現在の属するグループにEnsureVisible
                TreeNode currentNode;
                if(_nodeByPath.TryGetValue(_currentGroup, out currentNode)) {
                    invokeControl(treeViewInfo, () => {
                        treeViewInfo.SelectedNode = currentNode;
                        currentNode.EnsureVisible();
                    });
                }
                listObjectInfo.SuspendLayout();
                try {
                    listObjectInfo.Items.Clear();
                    var infoList = dataSet.GetObjectInfoListByGroup(_currentGroup);
                    ListViewItem lastSelected = null;
                    ListViewItem lastFocused = null;
                    foreach(var info in infoList) {
                        ListViewItem item = new ListViewItem(formatNameText(info));
                        item.UseItemStyleForSubItems = false;
                        ListViewItem.ListViewSubItem id = new ListViewItem.ListViewSubItem(item, info.Id.ToString());
                        ListViewItem.ListViewSubItem type = new ListViewItem.ListViewSubItem(item, info.ObjectType.Name);
                        item.SubItems.Add(id);
                        item.SubItems.Add(type);
                        id.BackColor = Color.FromArgb(255, info.Color);
                        id.ForeColor = ColorEx.GetComplementaryColor(id.BackColor);
                        item.Selected = dataSet.IsSelecting(info);
                        listObjectInfo.Items.Add(item);
                        if(dataSet.IsSelecting(info)) {
                            lastSelected = item;
                        }
                        if(prevFocusedId.HasValue && info.Id == prevFocusedId.Value) {
                            lastFocused = item;
                        }
                    }
                    if(lastSelected != null) {
                        lastSelected.EnsureVisible();
                    }
                    if(lastFocused != null) {
                        lastFocused.Focused = true;
                    }

                } finally { listObjectInfo.ResumeLayout(); }
                dataSet.DoObjectSelectionChanged();
            } finally { _mutex.ReleaseMutex(); }
        }

        public void OnSelectionChanged(object sender, EventArgs e) {
            // フォーム以外のスレッドから呼び出された場合にフォームのスレッドで処理する
            if(this.InvokeRequired) {
                this.BeginInvoke(new EventHandler(OnSelectionChanged), sender, e);
                return;
            }
            // _dataSetの途中変更を防ぐ
            MotionDataSet dataSet = _dataSet;
            if(dataSet == null)
                return;
            // 多重呼び出しの禁止
            if(!_mutex.WaitOne(0))
                return;
            try {
                if(_selectionChanging)
                    return;
                _selectionChanging = true;
                try {
                    // リストビューの更新
                    listObjectInfo.SuspendLayout();
                    try {
                        var infoList = dataSet.GetSelectedObjectInfoList();
                        // 選択されたオブジェクトが現在の選択グループ以外のものを含む場合にグループの選択を上位に変更
                        if(infoList.Count != 0) {
                            string common = PathEx.GetCommonDir(infoList.Select(info => info.Name).ToList());
                            if(_currentGroup != common && !PathEx.IsSubPath(common, _currentGroup)) {
                                _currentGroup = common;
                                this.DoSelectedGroupChanged();
                                return;
                            }
                        }
                        ListViewItem lastSelected = null;
                        // 選択が変更されたオブジェクトについてのリストビューのアイテムを更新
                        foreach(ListViewItem item in listObjectInfo.Items) {
                            uint id = uint.Parse(item.SubItems[1].Text);
                            var info = dataSet.GetObjectInfoById(id);
                            if(info != null) {
                                if(item.Selected != dataSet.IsSelecting(info)) {
                                    if(dataSet.IsSelecting(info)) {
                                        lastSelected = item;
                                    }
                                    item.Text = formatNameText(info);
                                    item.Selected = dataSet.IsSelecting(info);
                                    item.Font = new Font(item.Font, dataSet.IsSelecting(info) ? FontStyle.Bold : FontStyle.Regular);
                                }
                            }
                        }
                        if(lastSelected != null) {
                            lastSelected.EnsureVisible();
                        }
                    } finally { listObjectInfo.ResumeLayout(); }
                    this.DoUpdateObjectInfoControls();
                } finally { _selectionChanging = false; }
            } finally { _mutex.ReleaseMutex(); }
        }

        private void DoUpdateObjectInfoControls() {
            // フォーム以外のスレッドから呼び出された場合にフォームのスレッドで処理する
            if(this.InvokeRequired) {
                this.BeginInvoke(new Action(DoUpdateObjectInfoControls));
                return;
            }
            // 多重呼び出しの禁止
            if(!_mutex.WaitOne(0))
                return;
            try {
                if(_infoContentChanging)
                    return;
                _infoContentChanging = true;
                try {
                    // _dataSetの途中変更を防ぐ
                    MotionDataSet dataSet = _dataSet;
                    if(dataSet == null)
                        return;
                    var infoList = dataSet.GetSelectedObjectInfoList();
                    // その他のコントロールの更新
                    if(infoList.Count == 0) {
                        // 選択なしのとき
                        textColor.Enabled = false;
                        textName.Enabled = false;
                        numericColorAlpha.Enabled = false;
                        checkVisible.Enabled = false;
                        textGroup.Enabled = false;
                        textColor.BackColor = Color.White;
                        textColor.ForeColor = Color.Black;
                        textColor.Text = "Color";
                        textName.Text = "Nothing selected";
                        numericColorAlpha.Value = 0;
                        checkVisible.Checked = false;
                    } else {
                        textColor.Enabled = true;
                        textName.Enabled = true;
                        numericColorAlpha.Enabled = true;
                        checkVisible.Enabled = true;
                        textGroup.Enabled = true;

                        // 名前
                        if(infoList.Count == 1) {
                            textName.Text = PathEx.GetRelativePath(infoList[0].Name, _currentGroup);
                        } else {
                            textName.Text = _defaultName;
                        }
                        // 色
                        Color color = Color.FromArgb(255, infoList[0].Color);
                        if(infoList.All(info => color == Color.FromArgb(255, info.Color))) {
                            textColor.BackColor = color;
                            textColor.ForeColor = ColorEx.GetComplementaryColor(color);
                            textColor.Text = "Color: " + ColorTranslator.ToHtml(color);
                        } else {
                            textColor.BackColor = Color.White;
                            textColor.ForeColor = Color.Black;
                            textColor.Text = "Color: ...";
                        }
                        // visible
                        bool visible = infoList[0].IsVisible;
                        if(infoList.All(info => visible == info.IsVisible)) {
                            checkVisible.ThreeState = false;
                            checkVisible.Checked = visible;
                        } else {
                            checkVisible.ThreeState = true;
                            checkVisible.CheckState = CheckState.Indeterminate;
                        }
                        // 色のアルファ
                        int alpha = infoList[0].Color.A;
                        if(infoList.All(info => info.Color.A == alpha)) {
                            numericColorAlpha.Value = (int)Math.Round(100.0 * alpha / 255);
                            numericColorAlpha.ForeColor = SystemColors.ControlText;
                        } else {
                            numericColorAlpha.Value = (int)Math.Round(100.0 * alpha / 255);
                            numericColorAlpha.ForeColor = SystemColors.GrayText;
                        }
                    }
                } finally { _infoContentChanging = false; }
            } finally { _mutex.ReleaseMutex(); }
        }


        private void removeToolStripMenuItem_Click(object sender, EventArgs e) {
            // _dataSetの途中変更を防ぐ
            MotionDataSet dataSet = _dataSet;
            if(dataSet == null)
                return;
            dataSet.RemoveSelectedObjects(true, this);
        }

        private void buttonGather_Click(object sender, EventArgs e) {
            // _dataSetの途中変更を防ぐ
            MotionDataSet dataSet = _dataSet;
            if(dataSet == null)
                return;
            dataSet.GatherObjectInfoOrder(dataSet.GetSelectedObjectInfoList());
        }

        private void buttonUp_Click(object sender, EventArgs e) {
            // _dataSetの途中変更を防ぐ
            MotionDataSet dataSet = _dataSet;
            if(dataSet == null)
                return;
            dataSet.ReorderObjectInfo(dataSet.GetSelectedObjectInfoList(), -1);
        }

        private void buttonDown_Click(object sender, EventArgs e) {
            // _dataSetの途中変更を防ぐ
            MotionDataSet dataSet = _dataSet;
            if(dataSet == null)
                return;
            dataSet.ReorderObjectInfo(dataSet.GetSelectedObjectInfoList(), 1);
        }


        private void showToolStripMenuItem_Click(object sender, EventArgs e) {
            // _dataSetの途中変更を防ぐ
            MotionDataSet dataSet = _dataSet;
            if(dataSet == null)
                return;
            foreach(var info in dataSet.GetSelectedObjectInfoList()) {
                info.IsVisible = true;
            }
            dataSet.DoObjectInfoSetChanged();
        }

        private void askColor() {
            // 多重呼び出しの禁止
            if(!_mutex.WaitOne(0))
                return;
            try {
                // _dataSetの途中変更を防ぐ
                MotionDataSet dataSet = _dataSet;
                if(dataSet == null)
                    return;
                var infoList = dataSet.GetSelectedObjectInfoList();
                if(infoList.Count > 0) {
                    dialogColor.Color = Color.FromArgb(255, infoList[0].Color);
                    if(dialogColor.ShowDialog() == DialogResult.OK) {
                        foreach(var info in infoList) {
                            info.Color = Color.FromArgb(info.Color.A, dialogColor.Color);
                        }
                        dataSet.DoObjectInfoSetChanged();
                    }
                }
            } finally {
                _mutex.ReleaseMutex();
            }
        }

        private void textColor_Click(object sender, EventArgs e) {
            askColor();
        }

        private void textName_Validated(object sender, EventArgs e) {
            // _dataSetの途中変更を防ぐ
            MotionDataSet dataSet = _dataSet;
            if(dataSet == null)
                return;
            string text = PathEx.NormalizePath(textName.Text);
            if(text == _defaultName)
                return;
            string newName = PathEx.CombineName(_currentGroup, text);
            dataSet.RenameSelectedObjects(newName, this);
        }

        private void textGroup_Validated(object sender, EventArgs e) {
            // _dataSetの途中変更を防ぐ
            MotionDataSet dataSet = _dataSet;
            if(dataSet == null)
                return;
            var infoList = dataSet.GetSelectedObjectInfoList();
            string to = PathEx.NormalizePath(textGroup.Text);

            if(to == _currentGroup)
                return;
            bool found = false;
            foreach(var info in infoList) {
                if(PathEx.IsSubPath(info.Name, _currentGroup)) {
                    string subName = PathEx.GetRelativePath(info.Name, _currentGroup);
                    _dataSet.RenameObjectInfo(info, PathEx.CombineName(to, subName), this);
                    found = true;
                }
            }
            _currentGroup = to;
            if(found) {
                dataSet.DoObjectInfoSetChanged();
            }
        }

        private void treeViewInfo_AfterSelect(object sender, TreeViewEventArgs e) {
            // _dataSetの途中変更を防ぐ
            MotionDataSet dataSet = _dataSet;
            if(dataSet == null)
                return;
            var pairList = _nodeByPath.Where(p => p.Value == e.Node).ToList();
            if(pairList.Count > 0) {
                string path = pairList[0].Key;
                if(path != _currentGroup) {
                    _currentGroup = path;
                    if(!string.IsNullOrEmpty(_currentGroup)) {
                        dataSet.SelectObjects(false);
                        dataSet.SelectObjects(true, info => PathEx.IsSubPath(info.Name, _currentGroup));
                    }
                    this.DoSelectedGroupChanged();
                    if(!string.IsNullOrEmpty(_currentGroup)) {
                        if(listObjectInfo.Items.Count > 0) {
                            listObjectInfo.Items[0].EnsureVisible();
                        }
                    }
                }
            }
        }

        private void numericColorAlpha_ValueChanged(object sender, EventArgs e) {
            // 多重呼び出しの禁止
            if(!_mutex.WaitOne(0))
                return;
            try {
                if(_infoContentChanging)
                    return;
                _infoContentChanging = true;
                try {
                    // _dataSetの途中変更を防ぐ
                    MotionDataSet dataSet = _dataSet;
                    if(dataSet == null)
                        return;
                    var infoList = dataSet.GetSelectedObjectInfoList();
                    int alpha = (int)(Math.Round(2.55 * (int)numericColorAlpha.Value));
                    if(infoList.Count > 0) {
                        foreach(var info in infoList) {
                            info.Color = Color.FromArgb(alpha, info.Color);
                        }
                        dataSet.DoObjectInfoSetChanged();
                    }
                } finally { _infoContentChanging = false; }
            } finally { _mutex.ReleaseMutex(); }
        }

        private void checkVisible_CheckStateChanged(object sender, EventArgs e) {
            // _dataSetの途中変更を防ぐ
            MotionDataSet dataSet = _dataSet;
            if(dataSet == null)
                return;
            // 多重呼び出しの禁止
            if(!_mutex.WaitOne(0))
                return;
            try {
                if(_infoContentChanging)
                    return;
                _infoContentChanging = true;
                try {
                    var infoList = dataSet.GetSelectedObjectInfoList();
                    checkVisible.ThreeState = false;
                    bool visible = checkVisible.Checked;
                    if(infoList.Count > 0) {
                        foreach(var info in infoList) {
                            info.IsVisible = visible;
                        }
                        dataSet.DoObjectInfoSetChanged();
                    }
                } finally { _infoContentChanging = false; }
            } finally { _mutex.ReleaseMutex(); }
        }

        private void numericColorAlpha_MouseClick(object sender, MouseEventArgs e) {
            // 複数選択時に値が異なる場合には無効状態になるけどクリックしたら有効にする仕様
            if(!numericColorAlpha.Enabled) {
                MotionDataSet dataSet = _dataSet;
                if(dataSet == null)
                    return;
                var infoList = dataSet.GetSelectedObjectInfoList();
                if(infoList.Count > 0)
                    numericColorAlpha.Enabled = true;
            }
        }

        private void listObjectInfo_SelectedIndexChanged(object sender, EventArgs e) {
            // 多重呼び出しの禁止
            if(!_mutex.WaitOne(0))
                return;
            try {
                if(_selectionChanging)
                    return;
                _selectionChanging = true;
                try {
                    // _dataSetの途中変更を防ぐ
                    MotionDataSet dataSet = _dataSet;
                    if(dataSet == null)
                        return;
                    HashSet<uint> selectedIds = new HashSet<uint>();
                    foreach(ListViewItem item in listObjectInfo.SelectedItems) {
                        uint id = uint.Parse(item.SubItems[1].Text);
                        selectedIds.Add(id);
                    }
                    HashSet<uint> changedIds = new HashSet<uint>();
                    foreach(var info in dataSet.GetObjectInfoList()) {
                        bool selected = selectedIds.Contains(info.Id);
                        if(dataSet.IsSelecting(info) != selected) {
                            changedIds.Add(info.Id);
                            dataSet.SelectObjects(selected, info);
                        }
                    }
                    foreach(ListViewItem item in listObjectInfo.Items) {
                        uint id = uint.Parse(item.SubItems[1].Text);
                        if(changedIds.Contains(id)) {
                            MotionObjectInfo info = dataSet.GetObjectInfoById(id);
                            if(info != null) {
                                item.Text = formatNameText(info);
                                if(dataSet.IsSelecting(info)) {
                                    item.EnsureVisible();
                                    item.Font = new Font(item.Font, FontStyle.Bold);
                                } else {
                                    item.Font = new Font(item.Font, FontStyle.Regular);
                                }
                            }
                        }
                    }
                    dataSet.DoObjectSelectionChanged();
                    this.DoUpdateObjectInfoControls();
                } finally { _selectionChanging = false; }
            } finally { _mutex.ReleaseMutex(); }
        }

        private void textGroup_KeyDown(object sender, KeyEventArgs e) {
            switch(e.KeyCode) {
            case Keys.Enter:
                listObjectInfo.Focus();
                break;
            }
        }

        private void textName_KeyDown(object sender, KeyEventArgs e) {
            switch(e.KeyCode) {
            case Keys.Enter:
                listObjectInfo.Focus();
                break;
            }
        }

        private void textColor_KeyDown(object sender, KeyEventArgs e) {
            switch(e.KeyCode) {
            case Keys.Enter:
                askColor();
                break;
            }
        }

        private void contextRightClick_Opening(object sender, CancelEventArgs e) {

        }

        private void replaceFirstToToolStripMenuItem_Click(object sender, EventArgs e) {
            // _dataSetの途中変更を防ぐ
            MotionDataSet dataSet = _dataSet;
            if(dataSet == null)
                return;
            foreach(var info in dataSet.GetSelectedObjectInfoList()) {
                int ub = info.Name.IndexOf('_');
                if(ub >= 0) {
                    dataSet.RenameObjectInfo(info, info.Name.Substring(0, ub) + "/" + info.Name.Substring(ub + 1), this);
                }
            }
        }
    }
}
