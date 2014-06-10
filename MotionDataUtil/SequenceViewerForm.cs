using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MotionDataHandler;
using MotionDataHandler.Misc;
using MotionDataHandler.Sequence;
using MotionDataHandler.Script;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing.Drawing2D;

namespace MotionDataUtil {
    public partial class SequenceViewerForm : Form {
        #region Singleton
        static SequenceViewerForm _singleton = null;
        public static SequenceViewerForm Singleton {
            get {
                if(_singleton == null || _singleton.IsDisposed) {
                    _singleton = new SequenceViewerForm();
                }
                return _singleton;
            }
        }
        #endregion

        readonly SequenceViewerController _viewerController;
        Plugin.IPluginHost _pluginHost;
        bool _formInitialized = false;
        private SequenceViewerForm() {
            MotionDataUtilSettings.Singleton.Initialize();

            InitializeComponent();

            _viewerController = SequenceViewerController.Singleton;
            ScriptConsole.Singleton.ParentControl = this;
            ScriptConsole.Singleton.SequenceController = _viewerController;
            this.Disposed += new EventHandler(TimeValueSequenceForm_Disposed);
            _formInitialized = true;
        }

        private void TimeValueSequenceForm_Load(object sender, EventArgs e) {
            showDustBox(false);

            _viewerController.AttachParentControl(this);
            _viewerController.AllocationChanged += onAllocateViewer;
            _viewerController.DataChanged += onDataChanged;
            _viewerController.VisibleRangeChanged += onVisibleTimeChanged;
            _viewerController.GetSequenceOperations(System.Reflection.Assembly.GetExecutingAssembly(), SequenceType.None);
            _viewerController.StatusMessageChanged += onStatusMessageChanged;
            _viewerController.FocusedViewChanged += _viewerController_FocusedViewChanged;

            if(_viewerController.GetSequenceList().Count == 0) {
                var scratch = new SequenceView();
                scratch.Type = SequenceType.Label;
                scratch.SetDefaultHeight(true);

                scratch.SuspendRefresh();
                _viewerController.AddView(scratch);
                _viewerController.IsDataChanged = false;
                scratch.Sequence.IsDataChanged = false;
                scratch.IsDataModified = false;
                scratch.ResumeRefresh(true);
            } else {
                _viewerController.DoAllocationChanged();
            }

            TimeController.Singleton.SettingsChanged += onSelectionAutoScrollChanged;
            TimeController.Singleton.SettingsChanged += onLoopModeChanged;
            TimeController.Singleton.IsPlayingChanged += onPlayingChanged;
            TimeController.Singleton.CurrentTimeChanged += onCurrentTimeChanged;
            _viewerController.AttachTimeController(TimeController.Singleton);

            changeSaveStateEnabled();
            timeSelectionControlGlobal.AttachTimeController(TimeController.Singleton);
            timeSelectionControlLocal.AttachTimeController(TimeController.Singleton);
            TimeController.Singleton.SetVisibleTime(TimeController.Singleton.VisibleBeginTime, TimeController.Singleton.VisibleEndTime);
            TimeController.Singleton.DoSettingsChanged();
            TimeController.Singleton.DoIsPlayingChanged();
            setRenderingMode(true);
            ToolTip tip = new ToolTip();
            tip.SetToolTip(pictResetScale, "Reset Scale");
        }

        void _viewerController_FocusedViewChanged(object sender, EventArgs e) {
            this.EnsureFocusedViewVisible();
        }

        void onCurrentTimeChanged(object sender, EventArgs e) {
            if(this.InvokeRequired) {
                this.BeginInvoke(new EventHandler(onCurrentTimeChanged), sender, e);
                return;
            }
            labelCurrentTime.Text = string.Format("{0} 秒", TimeController.Singleton.CurrentTime.ToString("0.000"));
        }

        void TimeValueSequenceForm_Disposed(object sender, EventArgs e) {
            TimeController.Singleton.SettingsChanged -= onSelectionAutoScrollChanged;
            TimeController.Singleton.SettingsChanged -= onLoopModeChanged;
            TimeController.Singleton.IsPlayingChanged -= onPlayingChanged;
            TimeController.Singleton.CurrentTimeChanged -= onCurrentTimeChanged;
            _viewerController.DetachTimeController();
        }
        private void TimeValueSequenceForm_FormClosed(object sender, FormClosedEventArgs e) {
            _viewerController.ClearViewList();
            _viewerController.AllocationChanged -= onAllocateViewer;
            _viewerController.DataChanged -= onDataChanged;
            _viewerController.VisibleRangeChanged -= onVisibleTimeChanged;
            _viewerController.AttachParentControl(null);
            _viewerController.StatusMessageChanged -= onStatusMessageChanged;
            MotionDataUtilSettings.Singleton.Save();
        }


        private void onAllocateViewer(object sender, EventArgs e) {
            // 各SequenceViewerを描画する
            if(this.InvokeRequired) {
                this.BeginInvoke(new EventHandler(onAllocateViewer), sender, e);
                return;
            }
            IList<SequenceView> viewers = _viewerController.GetViewList();
            // スクロールバーの長さ設定
            int sumHeight = 16 + viewers.Sum(v => v.RequestedHeight);
            if(vScrollBarViewer.Value < 0)
                vScrollBarViewer.Value = 0;
            int supValue = sumHeight - panelViewerMain.Height;
            supValue = Math.Min(Math.Max(supValue, 0), vScrollBarViewer.Maximum);
            if(vScrollBarViewer.Value > supValue)
                vScrollBarViewer.Value = supValue;
            vScrollBarViewer.Minimum = 0;
            vScrollBarViewer.Maximum = sumHeight;
            vScrollBarViewer.LargeChange = panelViewerMain.Height;
            vScrollBarViewer.Enabled = sumHeight > panelViewerMain.Height;

            panelViewerList.SuspendLayout();
            panelViewerMain.SuspendLayout();
            try {
                // 前回にあって今回ないViewerを取り除く
                List<Control> prevControls = new List<Control>();
                foreach(var control in panelViewerMain.Controls) {
                    prevControls.Add(control as Control);
                }
                foreach(var prev in prevControls) {
                    SequenceView prevViewer = prev as SequenceView;
                    if(prevViewer == null || !viewers.Contains(prevViewer)) {
                        panelViewerMain.Controls.Remove(prev);
                    }
                }
                // 位置を設定しながら配置
                int top = panelViewerMain.DisplayRectangle.Top - vScrollBarViewer.Value;
                foreach(var view in viewers) {
                    view.SuspendLayout();
                    try {
                        view.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                        view.Location = new Point(0, top);
                        view.Size = new Size(panelViewerMain.ClientSize.Width, view.RequestedHeight);
                        top += view.RequestedHeight;
                        if(!panelViewerMain.Controls.Contains(view)) {
                            panelViewerMain.Controls.Add(view);
                        }
                    } finally {
                        view.ResumeLayout();
                    }
                }
            } finally {
                panelViewerList.ResumeLayout();
                panelViewerMain.ResumeLayout();
            }
        }

        public void AttachIPluginHost(Plugin.IPluginHost pluginHost) {
            _pluginHost = pluginHost;
            if(_viewerController != null) {
                _viewerController.AttachPluginHost(pluginHost);
            }
        }

        public void AutoLoadICSLabelSequence(ICSLabelSequence labelSequence, string title) {
            _viewerController.OpenLabelSequence(labelSequence, title);
        }

        public void AutoLoadSequence(string fileName) {
            _viewerController.OpenTimeSeriesValues(fileName);
        }
        public void AutoLoadSequence(TimeSeriesValues sequence, string title) {
            _viewerController.OpenTimeSeriesValues(sequence, title);
        }


        private void closewToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }


        private void addNewPanelToolStripMenuItem_Click(object sender, EventArgs e) {
            _viewerController.AddEmptyView();
            GC.Collect();
        }

        private void loadState(bool addPanels) {
            try {
                if(dialogOpenState.ShowDialog() == DialogResult.OK) {
                    string path = dialogOpenState.FileName;
                    path = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
                    WaitForForm waitForm = new WaitForForm(ctrl => {
                        ctrl.OperationTitle = string.Format("Load: {0}", Path.GetFileName(path));
                        _viewerController.LoadState(path, addPanels);
                        ctrl.DialogResult = DialogResult.OK;
                    }, () => _viewerController.GetSerializeProgress());
                    if(waitForm.ShowDialog() == DialogResult.OK) {
                        if(!addPanels) {
                            dialogSaveState.FileName = dialogOpenState.FileName;
                            _isSavePathSet = true;
                            changeSaveStateEnabled();
                        }
                    }
                }
            } catch(Exception ex) {
                ErrorLogger.Tell(ex, "ファイルの読み込みに失敗しました");
            }
        }

        private void menuLoadState_Click(object sender, EventArgs e) {
            loadState(false);
        }

        private void menuAddPanelFromState_Click(object sender, EventArgs e) {
            loadState(true);
        }

        private void setStatusLabel(string text) {
            if(this.InvokeRequired) {
                this.Invoke(new Action<string>(setStatusLabel), text);
                return;
            }
            labelStatus.Text = text;
        }
        private void onStatusMessageChanged(object sender, StringEventArgs e) {
            if(e == null)
                return;
            setStatusLabel(e.Text);
        }

        private bool _isSavePathSet = false;

        private bool trySaveState(bool ask) {
            try {
                if((!ask && _isSavePathSet) || dialogSaveState.ShowDialog() == DialogResult.OK) {
                    string path = dialogSaveState.FileName;
                    WaitForForm waitForm = new WaitForForm(ctrl => {
                        ctrl.OperationTitle = string.Format("Save: {0}", Path.GetFileName(path));
                        _viewerController.SaveState(path, true);
                        ctrl.DialogResult = DialogResult.OK;
                    }, () => _viewerController.GetSerializeProgress());
                    if(waitForm.ShowDialog() == DialogResult.OK) {
                        _isSavePathSet = true;
                        changeSaveStateEnabled();
                        setStatusLabel("ビュー一覧が保存されました");
                        return true;
                    }
                    return false;
                } else {
                    return false;
                }
            } catch(Exception ex) {
                ErrorLogger.Tell(ex, "ファイルの書き込みに失敗しました");
                return false;
            }
        }

        private void menuSaveStateAs_Click(object sender, EventArgs e) {
            trySaveState(true);
        }

        private void saveStateToolStripMenuItem_Click(object sender, EventArgs e) {
            trySaveState(false);
        }

        const string _formName = "TSeqViewer";

        void changeSaveStateEnabled() {
            menuSaveState.Enabled = _viewerController.IsDataChanged;
            buttonSave.Enabled = _viewerController.IsDataChanged;
            if(_isSavePathSet) {
                this.Text = _formName + " - " + Path.GetFileNameWithoutExtension(dialogSaveState.FileName);
            } else {
                this.Text = _formName;
            }
        }

        const int _precisionScroll = 64;
        void onDataChanged(object sender, EventArgs e) {
            if(this.InvokeRequired) {
                this.BeginInvoke(new EventHandler(onDataChanged), sender, e);
                return;
            }
            try {
                changeSaveStateEnabled();
                scrollTime.Maximum = (int)(_viewerController.WholeEndTime * _precisionScroll - _viewerController.VisibleDuration * _precisionScroll);
            } catch(OverflowException) { }
        }

        void onVisibleTimeChanged(object sender, EventArgs e) {
            if(this.InvokeRequired) {
                this.BeginInvoke(new EventHandler(onVisibleTimeChanged), sender, e);
                return;
            }
            try {
                scrollTime.Minimum = 0;
                scrollTime.Maximum = (int)(_viewerController.WholeEndTime * _precisionScroll);
                scrollTime.LargeChange = (int)(_viewerController.VisibleDuration * _precisionScroll);
                if(scrollTime.LargeChange < 10)
                    scrollTime.LargeChange = 10;
                int value = (int)(_viewerController.VisibleBeginTime * _precisionScroll);
                if(value < scrollTime.Minimum)
                    value = scrollTime.Minimum;
                if(value > scrollTime.Maximum)
                    value = scrollTime.Maximum;
                scrollTime.Value = value;
                scrollTime.SmallChange = scrollTime.LargeChange / 10;
            } catch(OverflowException) { }
        }

        private void SequenceViewerForm_FormClosing(object sender, FormClosingEventArgs e) {
            if(_viewerController.IsDataChanged) {
                switch(MessageBox.Show("状態が変更されています。保存しますか。", this.GetType().ToString(), MessageBoxButtons.YesNoCancel)) {
                case DialogResult.Yes:
                    e.Cancel = !trySaveState(true);
                    break;
                case DialogResult.No:
                    break;
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;
                }
            }
        }

        private void scrollTime_Scroll(object sender, ScrollEventArgs e) {
            try {
                decimal timeLength = _viewerController.VisibleDuration;
                decimal begin = (decimal)e.NewValue / _precisionScroll;
                _viewerController.SetVisibleTime(begin, begin + timeLength);
            } catch(OverflowException) { }
        }

        private void panelViewer_DragEnter(object sender, DragEventArgs e) {
            if(e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void panelViewer_DragDrop(object sender, DragEventArgs e) {
            if(_viewerController == null)
                return;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach(var file in files) {
                _viewerController.OpenFileByExtension(file);
            }
        }

        LabelJumpForm _jumpForm;

        private void jumpToolStripMenuItem_Click(object sender, EventArgs e) {
            if(_jumpForm == null || _jumpForm.IsDisposed) {
                _jumpForm = new LabelJumpForm();
                _jumpForm.AttachController(_viewerController);
            }
            _jumpForm.Show();
            _jumpForm.BringToFront();
        }

        private void playerToolStripMenuItem_Click(object sender, EventArgs e) {
            TimeControllerPlayerForm form = new TimeControllerPlayerForm();
            form.Show();
        }


        private void vScrollBarViewer_Scroll(object sender, ScrollEventArgs e) {
            vScrollBarViewer.Value = e.NewValue;
            this.Invoke(new EventHandler(onAllocateViewer), sender, e);
        }

        private void panelViewer_Resize(object sender, EventArgs e) {

        }

        private void buttonScrollMode_ButtonClick(object sender, EventArgs e) {
            setTimeScrollMode(!TimeController.Singleton.IsAutoScroll);
        }

        private void setTimeScrollMode(bool auto) {
            TimeController.Singleton.IsAutoScroll = auto;
            onSelectionAutoScrollChanged(this, new EventArgs());
        }

        private void onSelectionAutoScrollChanged(object sender, EventArgs e) {
            if(this.InvokeRequired) {
                this.BeginInvoke(new EventHandler(onSelectionAutoScrollChanged), sender, e);
                return;
            }
            buttonScrollMode.Text = TimeController.Singleton.IsAutoScroll ? menuAutoScroll.Text : menuManualScroll.Text;
            menuAutoScrollEnabled.Checked = TimeController.Singleton.IsAutoScroll;
        }

        private void menuAutoScroll_Click(object sender, EventArgs e) {
            setTimeScrollMode(true);
        }

        private void menuManualScroll_Click(object sender, EventArgs e) {
            setTimeScrollMode(false);
        }

        private void buttonLoopMode_ButtonClick(object sender, EventArgs e) {
            setLoopMode(!TimeController.Singleton.IsLoopEnabled);
        }

        private void setLoopMode(bool loop) {
            TimeController.Singleton.IsLoopEnabled = loop;
            onLoopModeChanged(this, new EventArgs());
        }
        private void onLoopModeChanged(object sender, EventArgs e) {
            if(this.InvokeRequired) {
                this.BeginInvoke(new EventHandler(onLoopModeChanged), sender, e);
                return;
            }
            buttonLoopMode.Text = TimeController.Singleton.IsLoopEnabled ? loopToolStripMenuItem.Text : noLoopToolStripMenuItem.Text;
            menuLoopEnabled.Checked = TimeController.Singleton.IsLoopEnabled;
        }

        private void noLoopToolStripMenuItem_Click(object sender, EventArgs e) {
            setLoopMode(false);
        }

        private void loopToolStripMenuItem_Click(object sender, EventArgs e) {
            setLoopMode(true);
        }

        private void buttonRun_Click(object sender, EventArgs e) {
            if(TimeController.Singleton.IsPlaying) {
                TimeController.Singleton.Stop();
            } else {
                TimeController.Singleton.Play();
            }
        }

        private void onPlayingChanged(object sender, EventArgs e) {
            if(TimeController.Singleton.IsPlaying) {
                buttonRun.Image = MotionDataUtil.Properties.Resources.pause;
                buttonRun.Text = "Pause";
                menuPlayPause.Checked = true;
            } else {
                buttonRun.Image = MotionDataUtil.Properties.Resources.run;
                buttonRun.Text = "Run";
                menuPlayPause.Checked = false;
            }
        }

        private void TimeValueSequenceForm_Resize(object sender, EventArgs e) {
            if(_formInitialized) {
                this.Invoke(new EventHandler(onAllocateViewer), sender, e);
            }
        }

        private void buttonRenderingMode_ButtonClick(object sender, EventArgs e) {
            setRenderingMode(!_viewerController.FastRenderingMode);
        }

        private void setRenderingMode(bool fast) {
            if(this.InvokeRequired) {
                this.BeginInvoke(new Action<bool>(setRenderingMode), fast);
                return;
            }
            _viewerController.FastRenderingMode = fast;
            buttonRenderingMode.Text = fast ? fastRenderingModeToolStripMenuItem.Text : trueRenderingModeToolStripMenuItem.Text;
            _viewerController.DoRefreshView();
        }

        private void fastRenderingModeToolStripMenuItem_Click(object sender, EventArgs e) {
            setRenderingMode(true);
        }

        private void trueRenderingModeToolStripMenuItem_Click(object sender, EventArgs e) {
            setRenderingMode(false);
        }

        private void pictResetScale_MouseClick(object sender, MouseEventArgs e) {
            TimeController.Singleton.SetVisibleTime(TimeController.Singleton.BeginTime, TimeController.Singleton.EndTime);
        }


        private void closeMultiplePanelsToolStripMenuItem_Click(object sender, EventArgs e) {
            DialogCloseSequenceViewers dialog = new DialogCloseSequenceViewers(_viewerController);
            dialog.ShowDialog();
        }


        private void motionDataUtilToolStripMenuItem_Click(object sender, EventArgs e) {
            MotionDataUtil.MotionDataUtilityForm.Singleton.Show();
        }


        private void colorsToolStripMenuItem_Click(object sender, EventArgs e) {
            DialogLabelColorSet d = new DialogLabelColorSet(_viewerController);
            d.ShowDialog();
            _viewerController.DoRefreshView();
        }

        private void menuScriptBoard_Click(object sender, EventArgs e) {
            ScriptControlForm form = new ScriptControlForm();
            form.Show();
        }

        bool _isDustBoxVisible = false;

        private void showDustBox(bool show) {
            if(show) {
                updateDustBox();
                buttonLoadDust.Enabled = listBoxDustBox.SelectedIndices.Count > 0;
            }
            buttonDust.Checked = show;
            dustBoxToolStripMenuItem.Checked = show;
            splitContainerViewer.Panel2Collapsed = !show;
            _isDustBoxVisible = show;
            this.Invoke(new EventHandler(onAllocateViewer), this, new EventArgs());
        }

        private void buttonDust_Click(object sender, EventArgs e) {
            showDustBox(!_isDustBoxVisible);
        }

        readonly Dictionary<string, string> _dustBoxTitleAndPath = new Dictionary<string, string>();
        private void updateDustBox() {
            listBoxDustBox.Items.Clear();
            _dustBoxTitleAndPath.Clear();
            if(_isSavePathSet) {
                string dir = SequenceViewerController.GetStateDataDirectoryPath(dialogSaveState.FileName, true);
                string[] txtFiles = Directory.GetFiles(dir, "*" + SequenceData.DefaultExtensionForHeader);
                foreach(string txtFile in txtFiles) {
                    string basename = Path.GetFileNameWithoutExtension(txtFile);
                    string valuesFile = Path.Combine(dir, basename + SequenceData.DefaultExtensionForValues);
                    string borderFile = Path.Combine(dir, basename + SequenceData.DefaultExtensionForBorder);
                    string viewerFile = Path.Combine(dir, basename + SequenceView.DefaultExtension);
                    if(File.Exists(valuesFile) && File.Exists(borderFile) && File.Exists(viewerFile)) {
                        string title;
                        using(FileStream stream = new FileStream(txtFile, FileMode.Open)) {
                            SequenceData tmp = new SequenceData();
                            tmp.RetrieveDataHeader(stream);
                            title = tmp.Title;
                        }
                        if(null == _viewerController.GetSequenceByTitle(title)) {
                            listBoxDustBox.Items.Add(title);
                            _dustBoxTitleAndPath[title] = txtFile;
                        }
                    }
                }
            }
            listBoxDustBox.SelectedIndices.Clear();
        }

        private void listBoxDustBox_SelectedIndexChanged(object sender, EventArgs e) {
            buttonLoadDust.Enabled = listBoxDustBox.SelectedIndices.Count > 0;
        }

        private void buttonLoadDust_Click(object sender, EventArgs e) {
            bool loaded = false;
            foreach(string title in listBoxDustBox.SelectedItems) {
                string txtFile;
                if(_dustBoxTitleAndPath.TryGetValue(title, out txtFile)) {
                    string dir = Path.GetDirectoryName(txtFile);
                    string filename = Path.GetFileNameWithoutExtension(txtFile);
                    string panelFile = Path.Combine(dir, filename + SequenceView.DefaultExtension);
                    _viewerController.OpenFileByExtension(panelFile);
                    loaded = true;
                }
            }
            if(loaded) {
                updateDustBox();
            }
        }

        private void buttonRefreshDustBox_Click(object sender, EventArgs e) {
            updateDustBox();
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e) {
            System.Threading.ReaderWriterLockSlim rw = new System.Threading.ReaderWriterLockSlim(System.Threading.LockRecursionPolicy.SupportsRecursion);
            LockDisposable l = new LockDisposable(rw);
            int c = 0;
            const int r = 1000000;
            StringBuilder s = new StringBuilder();
            DateTime p;
            for(int j = 0; j < 3; j++) {
                p = DateTime.Now;
                for(int i = 0; i < r; i++) {
                    c++;
                }
                s.AppendLine(((DateTime.Now.Ticks - p.Ticks) / 1000).ToString());
                p = DateTime.Now;
                for(int i = 0; i < r; i++) {
                    lock(rw) {
                        c++;
                    }
                }
                s.AppendLine(((DateTime.Now.Ticks - p.Ticks) / 1000).ToString());
                p = DateTime.Now;
                for(int i = 0; i < r; i++) {
                    rw.EnterReadLock();
                    try {
                        c++;
                    } finally { rw.ExitReadLock(); }
                }
                s.AppendLine(((DateTime.Now.Ticks - p.Ticks) / 1000).ToString());
                p = DateTime.Now;
                for(int i = 0; i < r; i++) {
                    using(l.GetReadLock()) {
                        c++;
                    }
                }
                s.AppendLine(((DateTime.Now.Ticks - p.Ticks) / 1000).ToString());

            }
            MessageBox.Show(s.ToString());
        }

        private void toolStripMenuItem1_DropDownOpening(object sender, EventArgs e) {
            SequenceViewerController controller = _viewerController;
            if(controller == null)
                return;
            SequenceView focusedView = controller.GetFocusedView();
            if(focusedView == null) {
                menuLastOperation.Enabled = false;
                menuOperateLabelSequence.Enabled = false;
                menuOperateValueSequence.Enabled = false;
                return;
            }
            if(controller.LastOperation != null) {
                bool labelMode = (controller.LastOperation.OperationTargetType & SequenceType.Label) != 0 && focusedView.Type != SequenceType.Numeric;
                bool valueMode = (controller.LastOperation.OperationTargetType & SequenceType.Numeric) != 0 && focusedView.Type != SequenceType.Label;
                if(labelMode || valueMode) {
                    menuLastOperation.DropDown.Items.Clear();
                    foreach(var item in controller.GetLastOperationToolStripItems()) {
                        menuLastOperation.DropDown.Items.Add(item);
                    }
                    menuLastOperation.Enabled = true;
                } else {
                    menuLastOperation.Enabled = false;
                }
            } else {
                menuLastOperation.Enabled = false;
            }

            if(focusedView.Type != SequenceType.Numeric) {
                menuOperateLabelSequence.DropDownItems.Clear();
                foreach(var item in controller.GetOperationToolStripItems(SequenceType.Label)) {
                    menuOperateLabelSequence.DropDownItems.Add(item);
                }
                menuOperateLabelSequence.Enabled = true;
            } else {
                menuOperateLabelSequence.Enabled = false;
            }
            if(focusedView.Type != SequenceType.Label) {
                menuOperateValueSequence.DropDownItems.Clear();
                foreach(var item in controller.GetOperationToolStripItems(SequenceType.Numeric)) {
                    menuOperateValueSequence.DropDownItems.Add(item);
                }
                menuOperateValueSequence.Enabled = true;
            } else {
                menuOperateValueSequence.Enabled = false;
            }
        }

        private void menuOpenTimeSeriesValues_Click(object sender, EventArgs e) {
            _viewerController.OpenTimeSeriesValuesWithDialog();
        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
            menuOpenLabel.DropDownItems.Clear();
            menuOpenLabel.DropDownItems.AddRange(_viewerController.CreateLabelOpenMenu().ToArray());
        }

        private void menuOpenViewState_Click(object sender, EventArgs e) {
            _viewerController.OpenSequenceWithDialog();
        }


        private void menuSelection_DropDownOpening(object sender, EventArgs e) {
        }

        private void menuAutoScrollEnabled_Click(object sender, EventArgs e) {
            setTimeScrollMode(!TimeController.Singleton.IsAutoScroll);
        }

        private void menuLoopEnabled_Click(object sender, EventArgs e) {
            setLoopMode(!TimeController.Singleton.IsLoopEnabled);
        }

        private void menuZoomSelected_Click(object sender, EventArgs e) {
            SequenceView view = _viewerController.GetFocusedView();
            if(view != null) {
                view.ZoomSelected();
            } else if(TimeController.Singleton.IsSelecting) {
                TimeController.Singleton.ZoomSelectedRange();
            }
        }

        private void menuUnzoomSelected_Click(object sender, EventArgs e) {
            SequenceView view = _viewerController.GetFocusedView();
            if(view != null) {
                view.UnzoomSelected();
            } else if(TimeController.Singleton.IsSelecting) {
                TimeController.Singleton.UnzoomSelectedRange();
            }
        }

        private void menuView_DropDownOpening(object sender, EventArgs e) {
            SequenceView selectedView = _viewerController.GetFocusedView();
            bool zoom = selectedView != null && (selectedView.IsTimeRangeSelected || selectedView.IsValueRangeSelected);
            zoom = zoom || TimeController.Singleton.IsSelecting;
            menuZoomSelected.Enabled = zoom;
            menuUnzoomSelected.Enabled = zoom;
            menuHideView.Enabled = selectedView != null;
            menuViewHeight.Enabled = selectedView != null;
            menuHideView.Checked = selectedView != null && selectedView.IsHidden;
            menuReorderView.Enabled = selectedView != null && _viewerController.GetViewList().Count >= 2;
            if(selectedView != null) {
                menuReorderView.DropDownItems.Clear();
                menuReorderView.DropDownItems.AddRange(_viewerController.GetReorderMenus(selectedView).ToArray());
            }
            menuFastRenderingEnabled.Checked = _viewerController.FastRenderingMode;

            menuActiveView.DropDownItems.Clear();
            foreach(SequenceView view in _viewerController.GetViewList()) {
                SequenceView tmpView = view;
                Image img = null;
                switch(view.Sequence.Type) {
                case SequenceType.Label:
                    img = Properties.Resources.label_mini;
                    break;
                case SequenceType.Numeric:
                    img = Properties.Resources.graph_mini;
                    break;
                }
                ToolStripMenuItem item = new ToolStripMenuItem(view.Sequence.Title, img, (s, e2) => { tmpView.Focus(); });
                item.Checked = view.IsFocused;
                menuActiveView.DropDownItems.Add(item);
            }
        }

        const int _ensureVisibleMargin = 16;

        public void EnsureFocusedViewVisible() {
            SequenceView selectedView = _viewerController.GetFocusedView();
            if(selectedView != null) {
                int panelHeight = panelViewerMain.DisplayRectangle.Height;
                int offset = vScrollBarViewer.Value;
                int panelY = panelViewerMain.DisplayRectangle.Top;
                int y = selectedView.Location.Y;
                int height = selectedView.Height;
                int newOffset = offset;
                if(y < panelY + _ensureVisibleMargin) {
                    newOffset += y - (panelY + _ensureVisibleMargin);
                } else if(y + height > panelY + panelHeight - _ensureVisibleMargin) {
                    newOffset += y + height - (panelY + panelHeight - _ensureVisibleMargin);
                }
                newOffset = Math.Max(vScrollBarViewer.Minimum, Math.Min(newOffset, vScrollBarViewer.Maximum));
                if(newOffset != offset) {
                    vScrollBarViewer.Value = newOffset;
                    this.Invoke(new EventHandler(onAllocateViewer), this, new EventArgs());
                }
            }

        }

        private void menuEdit_DropDownOpening(object sender, EventArgs e) {
            SequenceView view = _viewerController.GetFocusedView();
            if(view == null || view.IsLocked) {
                menuSetLabel.Enabled = false;
            } else {
                menuSetLabel.DropDownItems.Clear();
                ToolStripTextBox text = new ToolStripTextBox("textLabel");
                text.TextChanged += (sender2, e2) => {
                    view.SetLabelForSelected(text.Text);
                };
                text.KeyDown += (sender2, e2) => {
                    if(e2.KeyCode == Keys.Enter) {
                        menuEdit.DropDown.Close();
                    }
                };
                menuSetLabel.DropDownItems.Add(text);
                menuSetLabel.DropDownItems.Add(new ToolStripSeparator());
                foreach(string label in view.Sequence.Borders.GetLabelNames(true)) {
                    string tmpLabel = label;
                    ToolStripMenuItem item = new ToolStripMenuItem(string.Format("「{0}」", label), null, (sender2, e2) => {
                        view.SetLabelForSelected(tmpLabel);
                    });
                    menuSetLabel.DropDownItems.Add(item);
                }
                menuSetLabel.Enabled = true;
            }
            menuLabelingBorder.Enabled = view != null && (view.Sequence.Type & SequenceType.Numeric) != 0;

            menuLockEdit.Enabled = view != null;
            menuLockEdit.Checked = view != null && view.IsLocked;
        }

        private void menuLockEdit_Click(object sender, EventArgs e) {
            SequenceView view = _viewerController.GetFocusedView();
            if(view == null)
                return;
            view.IsLocked = !view.IsLocked;
        }

        private void menuHideView_Click(object sender, EventArgs e) {
            SequenceView view = _viewerController.GetFocusedView();
            if(view == null)
                return;
            view.IsHidden = !view.IsHidden;
        }

        private void menuResetScale_Click(object sender, EventArgs e) {
            TimeController.Singleton.SetVisibleTime(TimeController.Singleton.BeginTime, TimeController.Singleton.EndTime);
        }

        private void menuPlay_DropDownOpening(object sender, EventArgs e) {
            menuJumpStart.Enabled = TimeController.Singleton.IsSelecting;
            menuJumpEnd.Enabled = TimeController.Singleton.IsSelecting;
            menuPlayPause.Image = TimeController.Singleton.IsPlaying ? Properties.Resources.pause : Properties.Resources.run;
        }

        private void menuPlayPause_Click(object sender, EventArgs e) {
            TimeController.Singleton.IsPlaying = !TimeController.Singleton.IsPlaying;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e) {
            TimeController.Singleton.Stop();
            TimeController.Singleton.CurrentTime = TimeController.Singleton.BeginTime;
        }

        private void menuJumpStart_Click(object sender, EventArgs e) {
            TimeController.Singleton.CurrentTime = TimeController.Singleton.SelectBeginTime;
        }

        private void menuJumpEnd_Click(object sender, EventArgs e) {
            TimeController.Singleton.CurrentTime = TimeController.Singleton.SelectEndTime;
        }

        private void menuFastRenderingEnabled_Click(object sender, EventArgs e) {
            setRenderingMode(!_viewerController.FastRenderingMode);
        }

        private void menuLabelingBorder_Click(object sender, EventArgs e) {
            SequenceView view = _viewerController.GetFocusedView();
            if(view == null)
                return;
            DialogSequenceBorder dialog = new DialogSequenceBorder(view.Sequence, view);
            dialog.AttachIPluginHost(_pluginHost);
            dialog.ShowDialog();
            view.DoRefreshView();
        }

        private void textViewHeight_TextChanged(object sender, EventArgs e) {
            SequenceView view = _viewerController.GetFocusedView();
            if(view == null)
                return;
            int height;
            if(int.TryParse(textViewHeight.Text, out height)) {
                view.RequestedHeight = height;
            }
        }

        private void menuMinimumViewHeight_Click(object sender, EventArgs e) {
            SequenceView view = _viewerController.GetFocusedView();
            if(view == null)
                return;
            view.RequestedHeight = SequenceView.MinimumHeight;
        }

        private void menuSmallViewHeight_Click(object sender, EventArgs e) {
            SequenceView view = _viewerController.GetFocusedView();
            if(view == null)
                return;
            view.RequestedHeight = SequenceView.SmallHeight;
        }

        private void menuMediumViewHeight_Click(object sender, EventArgs e) {
            SequenceView view = _viewerController.GetFocusedView();
            if(view == null)
                return;
            view.RequestedHeight = SequenceView.MediumHeight;
        }

        private void menuLargeViewHeight_Click(object sender, EventArgs e) {
            SequenceView view = _viewerController.GetFocusedView();
            if(view == null)
                return;
            view.RequestedHeight = SequenceView.LargeHeight;
        }

        private void menuViewHeight_DropDownOpening(object sender, EventArgs e) {
            SequenceView view = _viewerController.GetFocusedView();
            if(view == null)
                return;
            menuMinimumViewHeight.Checked = view.Height == SequenceView.MinimumHeight;
            menuSmallViewHeight.Checked = view.Height == SequenceView.SmallHeight;
            menuMediumViewHeight.Checked = view.Height == SequenceView.MediumHeight;
            menuLargeViewHeight.Checked = view.Height == SequenceView.LargeHeight;
            textViewHeight.Text = view.Height.ToString();
        }

        private void menuCaptureViewArea_Click(object sender, EventArgs e) {
            Bitmap img = new Bitmap(panelViewerList.Width, panelViewerList.Height);
            panelViewerList.DrawToBitmap(img, new Rectangle(Point.Empty, panelViewerList.Size));
            Clipboard.SetData(System.Windows.Forms.DataFormats.Bitmap, img);
        }
    }
}
