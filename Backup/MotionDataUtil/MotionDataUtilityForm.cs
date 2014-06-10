using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Drawing;

using System.Windows.Forms;
using System.Text;
using Microsoft.DirectX;
using MotionDataHandler;
using MotionDataHandler.DataIO;
using MotionDataHandler.Motion;
using MotionDataHandler.Misc;
using MotionDataHandler.Sequence;
using Script = MotionDataHandler.Script;
using System.Xml;

namespace MotionDataUtil {
    public partial class MotionDataUtilityForm : Form {
        static readonly object _lockSingleton = new object();
        static MotionDataUtilityForm _singleton = null;
        public static MotionDataUtilityForm Singleton {
            get {
                MotionDataUtilityForm ret = _singleton;
                if(ret != null)
                    return ret;
                lock(_lockSingleton) {
                    ret = _singleton;
                    if(ret != null)
                        return ret;

                    return _singleton = new MotionDataUtilityForm();
                }
            }
        }

        readonly MotionDataSet _dataSet = MotionDataSet.Singleton;
        Plugin.IPluginHost _pluginHost;

        bool __isDatasetChanged;
        bool _isDataSetModified {
            get { return __isDatasetChanged; }
            set { __isDatasetChanged = value; enableMenuSaveMotionData(); }
        }

        bool __isOverWritable;
        bool _isOverWritable {
            get { return __isOverWritable; }
            set { __isOverWritable = value; enableMenuSaveMotionData(); }
        }

        MotionDataHandler.Motion.Operation.OperationMenuCreator _operationMenu = null;
        ToolStripMenuItem _operationCreateMenuItem = null;
        ToolStripMenuItem _operationEditMenuItem = null;
        ToolStripMenuItem _operationGeneralMenuItem = null;
        ToolStripMenuItem _operationOutputMenuItem = null;

        private MotionDataUtilityForm() {
            MotionDataUtilSettings.Singleton.Initialize();
            InitializeComponent();

            _dataSet.ClearFrame();
            _dataSet.ClearObject();
            motionDataViewer1.AttachTimeController(TimeController.Singleton);
            motionDataViewer1.AttachDataSet(_dataSet);
            motionDataMarkerList1.AttachDataSet(_dataSet);
            objectExistsView1.AttachDataSet(_dataSet);
            objectExistsView1.AttachTimeController(TimeController.Singleton);
            _dataSet.FrameListChanged += onDataSetChanged;
            _dataSet.ObjectInfoSetChanged += onDataSetChanged;
            _isDataSetModified = false;

            TimePlayerMotionData.AttachTimeController(TimeController.Singleton);
            TimeController.Singleton.AddTimeInterval(_dataSet);

            Script.ScriptConsole.Singleton.ParentControl = this;
            Script.ScriptConsole.Singleton.MotionDataSet = _dataSet;
            registerScriptFunctions();

            this.UpdateOperationMenu();
            setSideBarVisible(true);
            this.Disposed += new EventHandler(MotionDataUtilityForm_Disposed);
            menuStrip1.Items.RemoveByKey("旧エディットToolStripMenuItem");
        }

        /// <summary>
        /// フォームのメニューに処理メニューを追加します
        /// </summary>
        public void UpdateOperationMenu() {
            int indexBeforeHelp = menuStrip1.Items.IndexOf(menuHelp);
            if(_operationMenu == null) {
                _operationMenu = new MotionDataHandler.Motion.Operation.OperationMenuCreator(_dataSet, SequenceViewerController.Singleton);
            }
            if(_operationCreateMenuItem != null) {
                menuStrip1.Items.Remove(_operationCreateMenuItem);
            }
            if(_operationEditMenuItem != null) {
                menuStrip1.Items.Remove(_operationEditMenuItem);
            }
            if(_operationOutputMenuItem != null) {
                menuStrip1.Items.Remove(_operationOutputMenuItem);
            }
            if(_operationGeneralMenuItem != null) {
                menuStrip1.Items.Remove(_operationGeneralMenuItem);
            }
            if((_operationCreateMenuItem = _operationMenu.GetCreateMenuItem()) != null) {
                _operationCreateMenuItem.Text = "作成(&C)";
                menuStrip1.Items.Insert(indexBeforeHelp++, _operationCreateMenuItem);
            }
            if((_operationEditMenuItem = _operationMenu.GetEditMenuItem()) != null) {
                _operationEditMenuItem.Text = "編集(&E)";
                menuStrip1.Items.Insert(indexBeforeHelp++, _operationEditMenuItem);
            }
            if((_operationOutputMenuItem = _operationMenu.GetOutputMenuItem()) != null) {
                _operationOutputMenuItem.Text = "出力(&O)";
                foreach(ToolStripItem item in _operationOutputMenuItem.DropDownItems) {
                    ToolStripMenuItem menuItem = item as ToolStripMenuItem;
                    if(menuItem != null) {
                        menuItem.Click += new EventHandler(menuOutputSequenceMenuItem_Click);
                    }
                }
                menuStrip1.Items.Insert(indexBeforeHelp++, _operationOutputMenuItem);
            }
            if((_operationGeneralMenuItem = _operationMenu.GetGeneralMenuItem()) != null) {
                _operationGeneralMenuItem.DropDownItems.Add(selectAllToolStripMenuItem);
                _operationGeneralMenuItem.DropDownItems.Add(noneToolStripMenuItem);
                _operationGeneralMenuItem.DropDownItems.Add(invertSelectedToolStripMenuItem);
                _operationGeneralMenuItem.Text = "操作(&A)";
                menuStrip1.Items.Insert(indexBeforeHelp++, _operationGeneralMenuItem);
            }
        }

        void menuOutputSequenceMenuItem_Click(object sender, EventArgs e) {
            if(this.InvokeRequired) {
                this.Invoke(new EventHandler(menuOutputSequenceMenuItem_Click), sender, e);
                return;
            }
            openSequenceForm();
        }
        void MotionDataUtilityForm_Disposed(object sender, EventArgs e) {
            lock(_lockSingleton) {
                _singleton = null;
            }
        }

        private void registerScriptFunctions() {
            Script.ScriptConsole.Singleton.LoadFunctions(System.Reflection.Assembly.GetAssembly(typeof(MotionDataUtilityForm)));
        }

        public void AttachIPlguinHost(Plugin.IPluginHost host) {
            _pluginHost = host;
            TimeController.Singleton.AttachIPluginHost(host);
        }


        private void onDataSetChanged(object sender, EventArgs e) {
            _isDataSetModified = true;
        }


        private void enableMenuSaveMotionData() {
            if(this.InvokeRequired) {
                this.BeginInvoke(new Action(enableMenuSaveMotionData));
                return;
            }
            menuSaveMotionData.Enabled = _isDataSetModified;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void loadEVaRTTrc(TrcReader reader) {
            WaitForForm waitForm = new WaitForForm(ctrl => {
                try {
                    _dataSet.ClearFrame();
                    _dataSet.ClearObject();
                    Dictionary<int, uint> index2id = new Dictionary<int, uint>();
                    int length = reader.Header.NumFrames;
                    int count = 0;
                    for(int i = 0; i < reader.Header.NumMarkers; i++) {
                        MotionObjectInfo newInfo = new MotionObjectInfo(typeof(PointObject));
                        newInfo.Name = reader.Header.Markers[i];
                        _dataSet.AddObject(newInfo);
                        index2id[i] = newInfo.Id;
                    }

                    while(!reader.EndOfStream) {
                        TrcFrame inFrame = reader.ReadFrame();

                        MotionFrame outFrame = new MotionFrame(_dataSet, inFrame.Time);
                        for(int i = 0; i < inFrame.Markers.Length; i++) {
                            uint id;
                            if(!index2id.TryGetValue(i, out id)) {
                                MotionObjectInfo newInfo = new MotionObjectInfo(typeof(PointObject));
                                newInfo.Name = PathEx.CombineName("unnamed", (i + 1).ToString());
                                _dataSet.AddObject(newInfo);
                                id = index2id[i] = newInfo.Id;
                            }
                            if(inFrame.Markers[i].HasValue) {
                                float x, y, z;
                                if(float.TryParse(inFrame.Markers[i].Value.X, out x) && float.TryParse(inFrame.Markers[i].Value.Y, out y) && float.TryParse(inFrame.Markers[i].Value.Z, out z)) {
                                    outFrame[id] = new PointObject(new Vector3(x, y, z));
                                }
                            }
                        }
                        _dataSet.AddFrame(outFrame);
                        if(length < count)
                            length = count;
                        if(length <= 0)
                            length = 1;
                        ctrl.ReportProgress(100 * count / length, string.Format("Load Frame: {0}/{1} ({2} sec)", count, length, inFrame.Time.ToString("0.00")));
                        count++;
                    }
                    ctrl.ReportProgress(100, string.Format("Done"));
                    ctrl.DialogResult = DialogResult.OK;
                } catch(Exception) {
                    _dataSet.ClearObject();
                    _dataSet.ClearFrame();
                    _dataSet.DoObjectInfoSetChanged();
                    _dataSet.DoFrameListChanged();
                    throw;
                }
            });
            if(waitForm.ShowDialog() == DialogResult.OK) {
                _dataSet.DoObjectInfoSetChanged();
                _dataSet.DoFrameListChanged();
            }

        }

        bool openTrcFile(string fileName) {
            if(fileName == null) {
                if(dialogOpenTrc.ShowDialog() != DialogResult.OK) {
                    return false;
                }
                fileName = dialogOpenTrc.FileName;
            }
            try {
                using(TrcReader reader = new TrcReader(fileName)) {
                    loadEVaRTTrc(reader);
                }
                setSaveMotionDataFileName(fileName);
                _isOverWritable = false;
                TimeController.Singleton.SetVisibleTime(TimeController.Singleton.BeginTime, TimeController.Singleton.EndTime);
                return true;
            } catch(Exception ex) {
                ErrorLogger.Tell(ex, fileName + ": ファイルを開けませんでした");
                return false;
            }
        }

        private void loadTrcToolStripMenuItem_Click(object sender, EventArgs e) {
            openTrcFile(null);
        }

        bool openPhaseSpaceCsvFile(string fileName) {
            if(fileName == null) {
                if(dialogOpen.ShowDialog() != DialogResult.OK) {
                    return false;
                }
                fileName = dialogOpen.FileName;
            }
            try {
                using(PhaseSpaceDataReader reader = new PhaseSpaceDataReader(fileName)) {
                    loadPhaseSpace(reader);
                    setSaveMotionDataFileName(fileName);
                    _isOverWritable = false;
                    TimeController.Singleton.SetVisibleTime(TimeController.Singleton.BeginTime, TimeController.Singleton.EndTime);
                    return true;
                }
            } catch(Exception ex) {
                ErrorLogger.Tell(ex, fileName + ": ファイルを開けませんでした");
                return false;
            }
        }

        private void loadCsvToolStripMenuItem_Click(object sender, EventArgs e) {
            openPhaseSpaceCsvFile(null);
        }

        private void MotionDataUtilityForm_FormClosing(object sender, FormClosingEventArgs e) {
            if(!askSaveMotionDataAndOK()) {
                e.Cancel = true;
            }
        }

        bool openEyeSight(string fileName) {
            if(fileName == null) {
                if(dialogLoadEyeSight.ShowDialog() != DialogResult.OK) {
                    return false;
                }
                // すべて開く
                return dialogLoadEyeSight.FileNames.All(path => openEyeSight(path));
            }
            WaitForForm waitForm = new WaitForForm(ctrl => {
                lock(_dataSet) {
                    try {
                        ctrl.OperationTitle = fileName;
                        MotionObjectInfo newInfo = new MotionObjectInfo(typeof(LineObject));
                        newInfo.Name = Path.GetFileNameWithoutExtension(fileName);
                        _dataSet.AddObject(newInfo);
                        using(CSVReader reader = new CSVReader(fileName)) {
                            int index = 0;
                            while(!reader.EndOfStream) {
                                if(index < 0 || index >= _dataSet.FrameLength)
                                    break;
                                MotionFrame frame = _dataSet.GetFrameByIndex(index);
                                int progress = 0;
                                if(_dataSet.FrameLength > 0)
                                    progress = Math.Min(99, 100 * index / _dataSet.FrameLength);
                                ctrl.ReportProgress(progress, "Loading...: " + index.ToString());

                                string[] values = reader.ReadValues();
                                if(values == null)
                                    break;

                                if(values.Length >= 8) {
                                    float[] floats = new float[8];
                                    bool failure = false;
                                    for(int i = 0; !failure && i < 8; i++) {
                                        if(!float.TryParse(values[i], out floats[i]))
                                            failure = true;
                                    }
                                    if(!failure) {
                                        if(floats[0] > 0 && floats[4] > 0) {
                                            Vector3 end = new Vector3(floats[1], floats[2], floats[3]);
                                            Vector3 dir = new Vector3(floats[5], floats[6], floats[7]);
                                            dir.Normalize();
                                            frame[newInfo] = new LineObject(end, dir * 1000);
                                        }
                                    }
                                }
                                index++;
                            }
                        }
                    } catch(Exception ex) {
                        ErrorLogger.Tell(ex, "読み込みに失敗しました");
                    } finally {
                        _dataSet.DoObjectInfoSetChanged();
                        _dataSet.DoFrameListChanged();
                    }
                }
            });
            if(waitForm.ShowDialog() == DialogResult.OK) {
                return true;
            }
            return false;
        }

        private void loadEyeSightToolStripMenuItem_Click(object sender, EventArgs e) {
            openEyeSight(null);
        }


        private void setSaveMotionDataFileName(string path) {
            string name = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);
            dialogSaveMotionData.InitialDirectory = Path.GetDirectoryName(path);
            if(ext == ".mdsb2" || ext == ".mdsx2") {
                dialogSaveMotionData.FileName = Path.GetFileName(path);
            } else {
                dialogSaveMotionData.FileName = string.Format("{0}.{1}", name, dialogSaveMotionData.DefaultExt);
            }
            this.Text = global::MotionDataUtil.Properties.Settings.Default.MotionDataUtilityTitle + " - " + name;
        }

        private void saveMotionData(string path) {
            string ext = Path.GetExtension(path);
            try {
                DialogResult result = DialogResult.None;
                using(Stream stream = new FileStream(path + "~", FileMode.Create)) {
                    WaitForForm waitForm = new WaitForForm(ctrl => {
                        try {
                            if(ext == ".mdsx2") {
                                using(System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(stream)) {
                                    _dataSet.SerializeXml(writer);
                                }
                            } else if(ext == ".mdsb2") {
                                using(BinaryWriter writer = new BinaryWriter(stream)) {
                                    _dataSet.SerializeBinary(writer);
                                }
                            } else {
                                throw new ArgumentException("Unknown format: " + ext);
                            }
                            ctrl.DialogResult = DialogResult.OK;
                        } catch(Exception ex) {
                            ErrorLogger.Tell(ex, path + ": ファイルを保存できませんでした");
                        }
                    }, () => _dataSet.ProgressChangedEventArgs);
                    waitForm.SetOperationTitle(path);
                    result = waitForm.ShowDialog();
                }
                if(result == DialogResult.OK) {
                    if(File.Exists(path)) {
                        File.Replace(path + "~", path, path + "~~");
                        File.Delete(path + "~~");
                    } else {
                        File.Move(path + "~", path);
                    }
                    _isDataSetModified = false;
                    _isOverWritable = true;
                }
            } catch(NullReferenceException ex) {
                ErrorLogger.Tell(ex, path + ": ファイルを保存できませんでした");
            }
        }

        private void menuSaveMotionData_Click(object sender, EventArgs e) {
            if(_dataSet == null) {
                MessageBox.Show("モーションデータがロードされていません");
            } else {
                if(_isOverWritable) {
                    saveMotionData(Path.Combine(dialogSaveMotionData.InitialDirectory, dialogSaveMotionData.FileName));
                } else {
                    if(dialogSaveMotionData.ShowDialog() == DialogResult.OK) {
                        string path = dialogSaveMotionData.FileName;
                        saveMotionData(path);
                    }
                }
            }
        }

        private void menuSaveMotionDataAs_Click(object sender, EventArgs e) {
            if(_dataSet == null) {
                MessageBox.Show("モーションデータがロードされていません");
            } else {
                if(dialogSaveMotionData.ShowDialog() == DialogResult.OK) {
                    string path = dialogSaveMotionData.FileName;
                    saveMotionData(path);
                }
            }
        }

        bool openMotionData(string fileName) {
            if(fileName == null) {
                if(dialogOpenMotionData.ShowDialog() != DialogResult.OK) {
                    return false;
                }
                fileName = dialogOpenMotionData.FileName;
            }
            string ext = Path.GetExtension(fileName);
            using(Stream stream = new FileStream(fileName, FileMode.Open)) {
                switch(ext) {
                case ".mdsx":
                case ".mdsb":
                    using(MotionDataHandler.Motion.Old.MotionDataSet newDataSet = new MotionDataHandler.Motion.Old.MotionDataSet()) {
                        // 古いデータをロード
                        WaitForForm waitForm2 = new WaitForForm(ctrl => {
                            newDataSet.ProgressChanged += ctrl.OnProgressChanged;
                            try {
                                switch(ext) {
                                case ".mdsx":
                                    newDataSet.RetrieveXml(stream);
                                    break;
                                case ".mdsb":
                                    newDataSet.RetrieveBinary(stream);
                                    break;
                                default:
                                    System.Diagnostics.Debug.Fail("format naming error");
                                    break;
                                }
                                ctrl.DialogResult = DialogResult.OK;
                            } catch(Exception ex) {
                                ErrorLogger.Tell(ex, fileName + ": ファイルを読み込めませんでした");
                            } finally {
                                newDataSet.ProgressChanged -= ctrl.OnProgressChanged;
                            }
                        });
                        waitForm2.SetOperationTitle(fileName);
                        if(waitForm2.ShowDialog() == DialogResult.OK) {
                            // データを変換
                            WaitForForm waitForm = new WaitForForm(ctrl => {
                                ctrl.OperationTitle = "Converting into New Data Format...";
                                _dataSet.FromOldVersion(newDataSet);
                                ctrl.DialogResult = DialogResult.OK;
                            }, () => new ProgressChangedEventArgs(_dataSet.ProgressPercentage, _dataSet.ProgressMessage));
                            if(waitForm.ShowDialog() == DialogResult.OK) {
                                TimeController.Singleton.SetVisibleTime(TimeController.Singleton.BeginTime, TimeController.Singleton.EndTime);
                                setSaveMotionDataFileName(fileName);
                                _dataSet.DoObjectInfoSetChanged();
                                _dataSet.DoFrameListChanged();
                                _isOverWritable = false;
                                return true;
                            }
                        }
                        return false;
                    }
                case ".mdsx2":
                case ".mdsb2": {
                        WaitForForm waitForm = new WaitForForm(ctrl => {
                            ctrl.OperationTitle = fileName;
                            try {
                                switch(ext) {
                                case ".mdsx2":
                                    using(XmlReader reader = XmlReader.Create(stream)) {
                                        _dataSet.RetrieveXml(reader);
                                    }
                                    break;
                                case ".mdsb2":
                                    using(BinaryReader reader = new BinaryReader(stream)) {
                                        _dataSet.RetrieveBinary(reader);
                                    }
                                    break;
                                default:
                                    System.Diagnostics.Debug.Fail("format naming error");
                                    break;
                                }
                                ctrl.DialogResult = DialogResult.OK;
                            } catch(Exception ex) {
                                ErrorLogger.Tell(ex, fileName + ": ファイルを読み込めませんでした");
                            }
                        }, () => _dataSet.ProgressChangedEventArgs);
                        if(waitForm.ShowDialog() == DialogResult.OK) {
                            TimeController.Singleton.SetVisibleTime(TimeController.Singleton.BeginTime, TimeController.Singleton.EndTime);
                            setSaveMotionDataFileName(fileName);
                            _dataSet.DoObjectInfoSetChanged();
                            _dataSet.DoFrameListChanged();
                            _isDataSetModified = false;
                            _isOverWritable = true;
                            return true;
                        }
                        return false;
                    }
                default:
                    try {
                        throw new ArgumentException("Unknown format: " + ext);
                    } catch(Exception ex) {
                        ErrorLogger.Tell(ex, fileName + ": ファイルを読み込めませんでした");
                    }
                    return false;
                }
            }
        }
        /// <summary>
        /// データが変更されている場合にデータの保存を行うかを尋ねます。保存が不要であるか、保存が成功した場合にtrueを返します。
        /// </summary>
        /// <returns></returns>
        bool askSaveMotionDataAndOK() {
            if(_isDataSetModified) {
                switch(MessageBox.Show("データが変更されています。保存しますか?", this.GetType().ToString(), MessageBoxButtons.YesNoCancel)) {
                case DialogResult.Yes:
                    if(dialogSaveMotionData.ShowDialog() == DialogResult.OK) {
                        string path = dialogSaveMotionData.FileName;
                        saveMotionData(path);
                        return true;
                    } else {
                        return false;
                    }
                case DialogResult.No:
                    return true;
                case DialogResult.Cancel:
                    return false;
                }
            }
            return true;
        }

        private void menuOpenMotionData_Click(object sender, EventArgs e) {
            if(askSaveMotionDataAndOK()) {
                openMotionData(null);
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e) {
            _dataSet.SelectObjects(true);
            _dataSet.DoObjectSelectionChanged();
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e) {
            _dataSet.SelectObjects(false);
            _dataSet.DoObjectSelectionChanged();
        }

        private void invertSelectedToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach(var info in _dataSet.GetObjectInfoList()) {
                _dataSet.SelectObjects(!_dataSet.IsSelecting(info), info);
            }
            _dataSet.DoObjectSelectionChanged();
        }

        private void motionToolStripMenuItem_Click(object sender, EventArgs e) {
            MotionDataViewerForm viewer = new MotionDataViewerForm(_dataSet);
            viewer.Show();
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e) {
            SequenceViewerForm form = SequenceViewerForm.Singleton;
            form.Show();
        }

        private void sequenceViewerToolStripMenuItem_Click(object sender, EventArgs e) {
            openSequenceForm();
        }

        private void openSequenceForm() {
            openSequenceForm(null);
        }

        private void openSequenceForm(string path) {
            SequenceViewerForm form = SequenceViewerForm.Singleton;
            if(!form.Visible) {
                form.AttachIPluginHost(_pluginHost);
                try {
                    Script.ScriptConsole.Singleton.MotionDataSet = _dataSet;
                    Script.ScriptConsole.Singleton.ParentControl = this;
                    Script.ScriptConsole.Singleton.Invoke(new Misc.FunctionOpenSequenceViewer(), null);
                } catch(Exception) {
                    SequenceViewerForm.Singleton.Show();
                }
            }

            form.BringToFront();

            if(path != null) {
                try {
                    form.AutoLoadSequence(path);
                } catch(Exception ex) {
                    ErrorLogger.Tell(ex, "ファイルが開けませんでした");
                }
            }
        }
        private void openSequenceForm(TimeSeriesValues sequence, string title) {
            SequenceViewerForm form = SequenceViewerForm.Singleton;
            if(!form.Visible) {
                form.AttachIPluginHost(_pluginHost);
                try {
                    Script.ScriptConsole.Singleton.MotionDataSet = _dataSet;
                    Script.ScriptConsole.Singleton.ParentControl = this;
                    Script.ScriptConsole.Singleton.Invoke(new Misc.FunctionOpenSequenceViewer(), null);
                } catch(Exception) {
                    SequenceViewerForm.Singleton.Show();
                }
            }

            form.BringToFront();
            form.AutoLoadSequence(sequence, title);
        }

        private void MotionDataUtilityForm_FormClosed(object sender, FormClosedEventArgs e) {
            _dataSet.ClearObject();
            _dataSet.ClearFrame();
            MotionDataUtilSettings.Singleton.Save();
        }

        private void menuAddPhaseSpace_Click(object sender, EventArgs e) {
            if(dialogOpen.ShowDialog() == DialogResult.OK) {
                using(PhaseSpaceDataReader reader = new PhaseSpaceDataReader(dialogOpen.FileName)) {
                    addPhaseSpace(reader);
                }
            }
        }

        private void loadPhaseSpace(PhaseSpaceDataReader reader) {
            WaitForForm waitForm = new WaitForForm(ctrl => {
                try {
                    _dataSet.ClearFrame();
                    _dataSet.ClearObject();
                    Dictionary<int, uint> index2id = new Dictionary<int, uint>();
                    int count = 1;
                    while(!reader.EndOfStream) {
                        PhaseSpaceFrame inFrame = reader.ReadFrame();

                        MotionFrame outFrame = new MotionFrame(_dataSet, inFrame.Time);
                        for(int i = 0; i < inFrame.Markers.Length; i++) {
                            uint id;
                            if(!index2id.TryGetValue(i, out id)) {
                                MotionObjectInfo newInfo = new MotionObjectInfo(typeof(PointObject));
                                newInfo.Name = PathEx.CombineName("unnamed", (i + 1).ToString());
                                _dataSet.AddObject(newInfo);
                                id = index2id[i] = newInfo.Id;
                            }
                            if(inFrame.Markers[i].Condition > 0) {
                                outFrame[id] = new PointObject(new Vector3(inFrame.Markers[i].X, inFrame.Markers[i].Y, inFrame.Markers[i].Z));
                            }
                        }
                        _dataSet.AddFrame(outFrame);

                        ctrl.ReportProgress(99 - 990000 / (count + 10000), string.Format("Load Frame: {0} ({1} sec)", count, inFrame.Time.ToString("0.00")));
                        count++;
                    }
                    ctrl.ReportProgress(100, string.Format("Done"));
                    ctrl.DialogResult = DialogResult.OK;
                } catch(Exception) {
                    _dataSet.ClearObject();
                    _dataSet.ClearFrame();
                    _dataSet.DoObjectInfoSetChanged();
                    _dataSet.DoFrameListChanged();
                    throw;
                }
            });
            if(waitForm.ShowDialog() == DialogResult.OK) {
                _dataSet.DoObjectInfoSetChanged();
                _dataSet.DoFrameListChanged();
            }
        }

        private void addPhaseSpace(PhaseSpaceDataReader reader) {
            WaitForForm waitForm = new WaitForForm(ctrl => {
                try {
                    Dictionary<int, uint> index2id = new Dictionary<int, uint>();
                    int count = 1;
                    int index = 0;
                    PhaseSpaceFrame prevInFrame = new PhaseSpaceFrame();
                    bool first = true;
                    Action<PhaseSpaceFrame, int> import = (inFrame, endIndex) => {
                        for(; index < endIndex && index < _dataSet.FrameLength; index++) {
                            MotionFrame frame = _dataSet.GetFrameByIndex(index);
                            if(frame == null)
                                continue;
                            for(int i = 0; i < inFrame.Markers.Length; i++) {
                                uint id;
                                if(!index2id.TryGetValue(i, out id)) {
                                    MotionObjectInfo newInfo = new MotionObjectInfo(typeof(PointObject));
                                    newInfo.Name = PathEx.CombineName("unnamed", (i + 1).ToString());
                                    _dataSet.AddObject(newInfo);
                                    id = index2id[i] = newInfo.Id;
                                }
                                if(inFrame.Markers[i].Condition > 0) {
                                    frame[id] = new PointObject(new Vector3(inFrame.Markers[i].X, inFrame.Markers[i].Y, inFrame.Markers[i].Z));
                                }
                            }
                        }
                    };
                    while(!reader.EndOfStream) {
                        PhaseSpaceFrame inFrame = reader.ReadFrame();
                        int inIndex = _dataSet.GetFrameIndexAt(inFrame.Time);
                        MotionFrame tmp = _dataSet.GetFrameByIndex(inIndex);
                        if(tmp == null || tmp.Time == inFrame.Time)
                            inIndex++;
                        if(first) {
                            first = false;
                        } else {
                            import(prevInFrame, inIndex);
                        }
                        prevInFrame = inFrame;
                        ctrl.ReportProgress(99 - 990000 / (count + 10000), string.Format("Load Frame Data: {0} ({1} sec)", count, inFrame.Time.ToString("0.00")));
                        count++;
                    }
                    if(!first) {
                        import(prevInFrame, _dataSet.FrameLength);
                    }
                    ctrl.ReportProgress(100, string.Format("Done"));
                    ctrl.DialogResult = DialogResult.OK;
                } catch(Exception) {
                    _dataSet.ClearObject();
                    _dataSet.ClearFrame();
                    _dataSet.DoObjectInfoSetChanged();
                    _dataSet.DoFrameListChanged();
                    throw;
                }
            });
            if(waitForm.ShowDialog() == DialogResult.OK) {
                _dataSet.DoObjectInfoSetChanged();
                _dataSet.DoFrameListChanged();
            }
        }

        private void menuFile_Click(object sender, EventArgs e) {
            GC.Collect();
        }



        private void menuScriptBoard_Click(object sender, EventArgs e) {
            ScriptControlForm form = new ScriptControlForm();
            form.Show();

        }

        private void menuVersion_Click(object sender, EventArgs e) {
            System.Reflection.AssemblyName asmName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
            MessageBox.Show(asmName.Version.ToString(), asmName.Name, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }


        private void setSideBarVisible(bool visible) {
            splitMain.Panel1Collapsed = !visible;
            SizeF textSize;
            string text = visible ? "サイドバーを隠す" : "サイドバーを表示";
            using(Bitmap test = new Bitmap(1, 1))
            using(Graphics gfxTest = Graphics.FromImage(test)) {
                textSize = gfxTest.MeasureString(text, this.Font);
            }
            Size textImgSize = new Size((int)(textSize.Width + 4), 18);
            Bitmap imgText = new Bitmap(textImgSize.Width, textImgSize.Height);
            using(Graphics gfx = Graphics.FromImage(imgText)) {
                gfx.Clear(this.BackColor);
                gfx.DrawString(text, this.Font, Brushes.Black, new PointF(2, 4));
                Pen framePen = new Pen(SystemColors.ControlDarkDark);
                gfx.DrawLines(framePen, new Point[] { new Point(0, imgText.Height), new Point(0, 0), new Point(imgText.Width - 1, 0), new Point(imgText.Width - 1, imgText.Height) });
            }

            Bitmap imgResult = new Bitmap(textImgSize.Height, textImgSize.Width);
            using(Graphics gfx = Graphics.FromImage(imgResult)) {
                gfx.DrawImage(imgText, new Point[] { new Point(imgResult.Width, 0), new Point(imgResult.Width, imgResult.Height), new Point(0, 0) });
            }
            pictureSideBar.Height = imgResult.Height;
            pictureSideBar.Image = imgResult;
        }

        private void pictureSideBar_Click(object sender, EventArgs e) {
            setSideBarVisible(splitMain.Panel1Collapsed);
        }

        private void controls_DragEnter(object sender, DragEventArgs e) {
            if(e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void controls_DragDrop(object sender, DragEventArgs e) {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            e.Effect = DragDropEffects.None;
            this.BeginInvoke(new Action(() => {
                foreach(string fileName in files) {
                    openGeneralFile(fileName);
                }
            }));
        }

        void openGeneralFile(string fileName) {
            string ext = Path.GetExtension(fileName);
            switch(ext.ToLower()) {
            case ".mdsx":
            case ".mdsb":
            case ".mdsx2":
            case ".mdsb2":
                if(askSaveMotionDataAndOK()) {
                    openMotionData(fileName);
                }
                break;
            case ".trc":
                if(askSaveMotionDataAndOK()) {
                    openTrcFile(fileName);
                }
                break;
            case ".csv": {
                    DialogSimpleSelect dialog = new DialogSimpleSelect(string.Format("あいまいな拡張子を開く({0})", Path.GetFileName(fileName)), "PhaseSpace CSV ファイルを新規に開く", "PhaseSpace CSV ファイルを追加で開く", "Eye Sight CSV ファイルを追加で開く");
                    int index;
                    if(dialog.ShowDialog(out index) == DialogResult.OK) {
                        switch(index) {
                        case 0:
                            if(askSaveMotionDataAndOK()) {
                                openPhaseSpaceCsvFile(fileName);
                            }
                            break;
                        case 1:
                            using(PhaseSpaceDataReader reader = new PhaseSpaceDataReader(fileName)) {
                                addPhaseSpace(reader);
                            }
                            break;
                        case 2:
                            openEyeSight(fileName);
                            break;
                        }
                    }
                }
                break;
            default:
                MessageBox.Show(string.Format("ファイルを開けません: {0}", fileName), this.Text);
                break;
            }
        }

    }
}

