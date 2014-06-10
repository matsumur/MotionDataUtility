using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;
using System.Text.RegularExpressions;

namespace MotionDataHandler.Sequence {
    using Misc;
    using Operation;
    using DefaultOperations;
    using ViewerFunction;
    using Script;

    /// <summary>
    /// SequenceViewerのリストを保持して制御をするためのクラス
    /// </summary>
    public class SequenceViewerController : ITimeInterval {
        #region ビューパネル操作用IViewerFunctionの定義
        /// <summary>
        /// パネルを追加するViewerFunction
        /// </summary>
        public class FunctionNewPanel : IViewerFunction {
            #region IViewerFunction メンバ

            public ScriptVariable Call(IList<ScriptVariable> args, SequenceViewerController controller) {
                SequenceData newData = new SequenceData();
                newData.Title = "scratch";
                newData.Type = SequenceType.Label;
                controller.AddSequence(newData);
                return new StringVariable(newData.Title);
            }

            public string GetCommandName() { return "NewViewPanel"; }
            public string Usage { get { return "()"; } }

            #endregion
        }
        /// <summary>
        /// パネルを閉じるViewerFunction
        /// </summary>
        public class FunctionClosePanel : IViewerFunction {
            #region IViewerFunction メンバ

            public ScriptVariable Call(IList<ScriptVariable> args, SequenceViewerController controller) {
                if(args.Count == 0)
                    return null;
                if(args[0].IsNull())
                    return null;
                IList<ScriptVariable> nameVars = args[0].ToList();
                List<ScriptVariable> ret = new List<ScriptVariable>();
                controller.SuspendAllocationChanged();
                try {
                    foreach(var nameVar in nameVars) {
                        string name = nameVar.IsNull() ? "" : nameVar.ToString();
                        SequenceView viewer = controller.GetViewByTitle(name);
                        if(viewer != null) {
                            controller.removeViewInternal(viewer);
                            ret.Add(new BooleanVariable(true));
                        } else {
                            ret.Add(new BooleanVariable(false));
                        }
                    }
                } finally { controller.ResumeAllocationChanged(); }
                return new ListVariable(ret);
            }

            public string GetCommandName() { return "CloseViewPanel"; }
            public string Usage { get { return "({panel title, ...})"; } }

            #endregion
        }
        /// <summary>
        /// パネルのタイトルを変更するViewerFunction
        /// </summary>
        public class FunctionRenamePanelTitle : IViewerFunction {
            #region IViewerFunction メンバ

            public ScriptVariable Call(IList<ScriptVariable> args, SequenceViewerController controller) {
                if(args.Count < 2)
                    throw new ArgumentException("2 arguments required", "args");
                if(args[0].IsNull())
                    throw new ArgumentNullException("args", "first argument cannot be null");
                if(args[1].IsNull())
                    throw new ArgumentNullException("args", "second argument cannot be null");
                string current = args[0].ToString();
                string replace = args[1].ToString();
                string result = controller.renamePanelTitleInternal(current, replace);
                if(result == null) {
                    throw new InvalidOperationException("Panel View not found: " + current);
                }
                return new StringVariable(result);
            }

            public string GetCommandName() {
                return "RenamePanelTitle";
            }

            public string Usage {
                get { return "(current, replace)"; }
            }

            #endregion
        }
        #endregion

        Plugin.IPluginHost _pluginHost;
        /// <summary>
        /// iCorpusStudioのPluginHostを取得します
        /// </summary>
        public Plugin.IPluginHost PluginHost { get { return _pluginHost; } }

        #region IDataChanged
        /// <summary>
        /// データが変更された時に呼び出されるイベント
        /// </summary>
        public event EventHandler DataChanged = null;
        private void doDataChanged() {
            if(!_isDataChanged) {
                _isDataChanged = true;
                if(DataChanged != null) {
                    DataChanged.Invoke(this, new EventArgs());
                }
            }
        }
        private bool _isDataChanged = false;
        public bool IsDataChanged {
            get { return _isDataChanged; }
            set {
                if(value) {
                    doDataChanged();
                } else {
                    _isDataChanged = false;
                }
            }
        }
        private void onDataChanged(object sender, EventArgs e) {
            doDataChanged();
        }
        #endregion

        #region IPluginHost
        /// <summary>
        /// iCorpusStudioのPluginHostを割り当てます
        /// </summary>
        /// <param name="host"></param>
        public void AttachPluginHost(Plugin.IPluginHost host) {
            _pluginHost = host;
            EventHandler<IPluginHostChangedEventArgs> tmp = IPluginHostChanged;
            if(tmp != null) {
                tmp.Invoke(this, new IPluginHostChangedEventArgs(_pluginHost));
            }
        }
        /// <summary>
        /// iCorpusStudioのPluginHostを取り除きます
        /// </summary>
        public void DetachPluginHost() {
            this.AttachPluginHost(null);
        }
        #endregion

        #region 全般的なメソッド・プロパティ

        private static readonly SequenceViewerController _singleton = new SequenceViewerController();
        /// <summary>
        /// 唯一のオブジェクトを取得します
        /// </summary>
        public static SequenceViewerController Singleton { get { return _singleton; } }
        /// <summary>
        /// 内部コンストラクタ
        /// </summary>
        private SequenceViewerController() {
            _timeController = null;
            ScriptConsole.Singleton.SequenceController = this;
            this.GetSequenceOperations(SequenceType.None);
            this.AttachTimeController(TimeController.Singleton);
        }

        /// <summary>
        /// 親コントロール
        /// </summary>
        private Control _parentControl;
        /// <summary>
        /// 親コントロールを取得します
        /// </summary>
        public Control ParentControl { get { return _parentControl; } }

        public void AttachTimeController(TimeController timeController) {
            DetachTimeController();
            if(timeController == null)
                throw new ArgumentNullException("timeController", "'timeController' cannot be null");
            _timeController = timeController;
            _timeController.CurrentTimeChanged += _timeController_CurrentTimeChanged;
            _timeController.VisibleRangeChanged += _timeController_VisibleRangeChanged;
            _timeController.SelectedRangeChanged += new EventHandler(_timeController_SelectedRangeChanged);
            _timeController.CursorTimeChanged += _timeController_CursorTimeChanged;
            _timeController.AddTimeInterval(this);
        }


        public void AttachParentControl(Control parentControl) {
            if(_parentControl != null) {
                _parentControl.Disposed -= new EventHandler(_parentControl_Disposed);
            }
            _parentControl = parentControl;
            if(_parentControl != null) {
                _parentControl.Disposed -= new EventHandler(_parentControl_Disposed);
            }
            createViewForStoredData();
        }

        void _parentControl_Disposed(object sender, EventArgs e) {
            ClearViewList();
        }

        #endregion

        #region ビューパネル全般のプロパティ
        /// <summary>
        /// 内包するビューパネル一覧
        /// </summary>
        private readonly List<SequenceView> _viewList = new List<SequenceView>();
        /// <summary>
        /// 親フォーム未作成時に追加されたシーケンスデータの一覧
        /// </summary>
        private readonly List<KeyValuePair<SequenceData, bool?>> _notViewCreatedData = new List<KeyValuePair<SequenceData, bool?>>();

        #endregion

        #region ビューパネルの追加・削除・順序変更の基礎的な操作
        /// <summary>
        /// ビューパネル一覧の同期オブジェクト
        /// </summary>
        readonly object _lockAccessViewList = new object();

        public void AddEmptyView() {
            if(this.ParentControl != null) {
                ViewerFunctionScriptFunction.Invoke(this.ParentControl, this, new FunctionNewPanel());
            } else {
                SequenceData newData = new SequenceData();
                newData.Title = "scratch";
                this.AddSequence(newData);
            }
        }

        /// <summary>
        /// 指定されたビューパネルを追加します．
        /// </summary>
        /// <param name="view"></param>
        public void AddView(SequenceView view) {
            if(view == null)
                return;
            // このオブジェクトのビュー一覧に追加
            lock(_lockAccessViewList) {
                if(_viewList.Contains(view))
                    return;
                this.SetUniqueTitle(view);
                _viewList.Add(view);
            }
            // ビューの状態が変わった時に，メッセージを受け取って処理を投げるよう設定
            view.StatusMessageChanged += onStatusMessageChanged;
            // ビューの親を設定
            if(view.ParentController != this) {
                view.AttachController(this);
            }
            // iCorpusStudioのPluginHostをこのオブジェクトと同じにする
            view.AttachPluginHost(_pluginHost);
            // ビューパネルのデータが更新されたときに，更新を上に伝播させる処理を投げるよう設定
            view.DataModified += onDataChanged;
            // ビューの種類がちゃんと設定されてなかった場合にはラベル列モードに
            if(view.Type == SequenceType.None) {
                view.Type = SequenceType.Label;
            }
            // データの更新を上に伝播させる
            doDataChanged();
            // ビューパネルの配置の変更を上に伝播させる
            this.DoAllocationChanged();
        }

        /// <summary>
        /// 指定されたシーケンスデータを持つビューパネルを作成して追加します．
        /// </summary>
        /// <param name="data">シーケンスデータ</param>
        public void AddSequence(SequenceData data) {
            this.AddSequence(data, null);
        }
        /// <summary>
        /// 指定されたシーケンスデータを持つビューパネルを作成して追加します
        /// </summary>
        /// <param name="data">シーケンスデータ</param>
        /// <param name="locked">ビューをロックするかどうか</param>
        public void AddSequence(SequenceData data, bool? locked) {
            lock(_notViewCreatedData) {
                _notViewCreatedData.Add(new KeyValuePair<SequenceData, bool?>(data, locked));
            }
            createViewForStoredData();
        }

        public void OpenSequenceWithDialog() {
            try {
                if(_components.DialogLoadViewPanel.ShowDialog() == DialogResult.OK) {
                    foreach(var path in _components.DialogLoadViewPanel.FileNames) {
                        string statePath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
                        this.AddView(SequenceView.RetrieveState(statePath));
                    }
                    this.DoAllocationChanged();
                }
            } catch(Exception ex) {
                ErrorLogger.Tell(ex, "ファイルを読み込めませんでした");
            }
        }

        /// <summary>
        /// 親コントロールがない時に追加されたシーケンスデータがある場合に，それら用のビューパネルを作成して追加します
        /// </summary>
        private void createViewForStoredData() {
            Control parent = this.ParentControl;
            if(parent == null)
                return;
            if(parent.InvokeRequired) {
                parent.Invoke(new Action(createViewForStoredData));
                return;
            }
            lock(_notViewCreatedData) {
                this.SuspendAllocationChanged();
                try {
                    foreach(var pair in _notViewCreatedData) {
                        SequenceView view = new SequenceView(pair.Key);
                        this.AddView(view);
                        if(pair.Value.HasValue) {
                            view.IsLocked = pair.Value.Value;
                        }
                    }
                } finally {
                    this.ResumeAllocationChanged();
                    _notViewCreatedData.Clear();
                }
            }
        }
        /// <summary>
        /// ビューパネルを取り除く処理をする
        /// </summary>
        /// <param name="view"></param>
        private void removeViewInternal(SequenceView view) {
            if(view == null)
                return;
            // このオブジェクトのビュー一覧から要素を取り除く
            lock(_lockAccessViewList) {
                if(!_viewList.Contains(view))
                    return;
                _viewList.Remove(view);
            }
            view.StatusMessageChanged -= onStatusMessageChanged;
            if(view.ParentController == this) {
                view.DetachController();
            }
            view.AttachPluginHost(null);
            view.DataModified -= onDataChanged;
            if(view.ParentForm != null && !view.IsDisposed) {
                try {
                    view.Invoke(new Action(view.Dispose));
                } catch(ObjectDisposedException) { }
            }
            doDataChanged();
            this.DoAllocationChanged();
        }
        /// <summary>
        /// 指定されたビューパネルを取り除きます
        /// </summary>
        /// <param name="view"></param>
        public void RemoveView(SequenceView view) {
            this.RemoveView(new SequenceView[] { view });
        }
        /// <summary>
        /// 指定されたビューパネル群を取り除きます
        /// </summary>
        /// <param name="viewList"></param>
        public void RemoveView(IList<SequenceView> viewList) {
            this.RemoveView(viewList.Select(v => v.Sequence.Title).ToList());
        }
        /// <summary>
        /// 指定されたタイトルを持つビューパネル群を取り除きます
        /// </summary>
        /// <param name="titles"></param>
        public void RemoveView(IList<string> titles) {
            // フォームを閉じたときなどParentControlが空だとViewerFunctionがちゃんと動かないので処理を分ける
            if(this.ParentControl != null) {
                try {
                    ViewerFunction.ViewerFunctionScriptFunction.Invoke(this.ParentControl, this, new FunctionClosePanel(), new ListVariable(titles.Select(p => (ScriptVariable)new StringVariable(p))));
                    // 成功したら脱出する
                    return;
                } catch(InvalidOperationException) { }
            }
            // ViewerFunctionが失敗したら普通に閉じる
            foreach(string title in titles) {
                SequenceView view = this.GetViewByTitle(title);
                removeViewInternal(view);
            }
        }

        /// <summary>
        /// 指定されたビューパネルの順序を指定された値だけ動かします
        /// </summary>
        /// <param name="view">動かす対象のビューパネル</param>
        /// <param name="moveOffset">順序の移動幅．-1だとひとつ前に，+1だとひとつ後ろに移動します</param>
        public void ReorderView(SequenceView view, int moveOffset) {
            lock(_lockAccessViewList) {
                if(_viewList.Contains(view) && moveOffset != 0) {
                    // 現在位置から取り除く
                    int index = _viewList.IndexOf(view);
                    _viewList.RemoveAt(index);
                    // 新しい位置に割り込み
                    int to = index + moveOffset;
                    if(to < 0)
                        to = 0;
                    if(to > _viewList.Count)
                        to = _viewList.Count;
                    _viewList.Insert(to, view);
                    // タイトル略称がかぶっているときに順番が変わると保存先がずれそうなので，かぶったビューパネルの更新フラグを立てておく
                    var slice = from v in _viewList select new { Title = v.Sequence.Title, Slice = getSlicedName(v.Sequence.Title) };
                    var dupTitle = (from g in
                                        from s in slice group s.Title by s.Slice
                                    where g.Count() > 1
                                    select g.ToList()).SelectMany(g => g).ToList();
                    foreach(var v in _viewList) {
                        if(dupTitle.Contains(v.Sequence.Title)) {
                            v.Sequence.IsDataChanged = true;
                        }
                    }
                }
            }
            doDataChanged();
            this.DoAllocationChanged();
        }

        /// <summary>
        /// 内包するビューパネルをすべて取り除きます
        /// </summary>
        public void ClearViewList() {
            IList<SequenceView> viewList = this.GetViewList();
            this.SuspendAllocationChanged();
            try {
                foreach(SequenceView view in viewList) {
                    this.removeViewInternal(view);
                }
            } finally { this.ResumeAllocationChanged(); }
            EventHandler tmp = this.TimeIntervalChanged;
            if(tmp != null)
                tmp.Invoke(this, new EventArgs());
        }

        #endregion


        SequenceViewerInnerComponents _components = new SequenceViewerInnerComponents();

        #region 外部データからのビューパネルの追加
        /// <summary>
        /// ダイアログを表示して時系列データを開きます
        /// </summary>
        public void OpenTimeSeriesValuesWithDialog() {
            if(_components.DialogOpenSequence.ShowDialog() == DialogResult.OK) {
                try {
                    foreach(var file in _components.DialogOpenSequence.FileNames) {
                        if(this.OpenTimeSeriesValues(file) == null) {
                            MessageBox.Show("ファイルが有効ではありませんでした．", typeof(InvalidDataException).Name);
                        }
                    }
                } catch(Exception ex) {
                    ErrorLogger.Tell(ex, "ファイルを読み込めませんでした");
                }
            }
        }
        /// <summary>
        /// 指定されたファイルを時系列データとして開きます
        /// </summary>
        /// <param name="fileName">ファイルのパス</param>
        /// <returns></returns>
        public SequenceView OpenTimeSeriesValues(string fileName) {
            using(FileStream stream = new FileStream(fileName, FileMode.Open)) {
                TimeSeriesValues sequence = TimeSeriesValues.Deserialize(stream);
                if(sequence == null) {
                    return null;
                }
                return this.OpenTimeSeriesValues(sequence, Path.GetFileNameWithoutExtension(fileName));
            }
        }

        /// <summary>
        /// 指定された時系列データを持つビューを作成して一覧に追加します．
        /// </summary>
        /// <param name="sequence">時系列データ</param>
        /// <param name="title">ビューのタイトル</param>
        /// <returns></returns>
        public SequenceView OpenTimeSeriesValues(TimeSeriesValues sequence, string title) {
            SequenceView view = new SequenceView();
            view.SuspendRefresh();
            try {
                SequenceData newData = new SequenceData(sequence, null, title);
                newData.Type = SequenceType.Numeric;
                view.AttachSequenceData(newData, true);
                this.AddView(view);
            } finally {
                view.ResumeRefresh(true);
            }
            return view;
        }
        /// <summary>
        /// 指定されたラベル列を持つビューを作成して一覧に追加します．
        /// </summary>
        /// <param name="labelSequence">ラベル列</param>
        /// <param name="title">ビューのタイトル</param>
        /// <returns></returns>
        public SequenceView OpenLabelSequence(ICSLabelSequence labelSequence, string title) {
            SequenceView view = new SequenceView();
            view.SuspendRefresh();
            try {
                view.AttachLabelSequence(labelSequence, title);
                this.AddView(view);
            } finally {
                view.ResumeRefresh(true);
            }
            return view;
        }

        public IList<ToolStripItem> CreateLabelOpenMenu() {
            List<ToolStripItem> ret = new List<ToolStripItem>();
            ToolStripItem itemFile = new ToolStripMenuItem(Properties.Settings.Default.Menu_LoadFromFile, null, menuLabelLoadFromFile_Click, "menuLoadFromFile");
            ret.Add(itemFile);
            if(_pluginHost != null) {
                ToolStripItem itemSep = new ToolStripSeparator();
                ret.Add(itemSep);
                int index = 0;
                foreach(var row in _pluginHost.DataRows) {
                    ToolStripItem item = new ToolStripMenuItem(row.Title, null, menuLoadLabelDropDown_Click, index.ToString());
                    ret.Add(item);
                    index++;
                }
            }
            return ret;
        }

        private void menuLoadLabelDropDown_Click(object sender, EventArgs e) {
            if(_pluginHost == null)
                return;
            ToolStripMenuItem button = sender as ToolStripMenuItem;
            if(button == null)
                return;
            int index;
            if(!int.TryParse(button.Name, out index))
                return;
            if(index >= 0 && index < _pluginHost.DataRows.Count) {
                var datarow = _pluginHost.DataRows[index];
                ICSLabelSequence labelSequence = new ICSLabelSequence();
                labelSequence.FromDataRow(datarow);
                Dictionary<string, Color> palette = new Dictionary<string, Color>();
                foreach(var prop in datarow.Properties) {
                    palette[prop.Value] = prop.Color;
                }
                this.OpenLabelSequence(labelSequence, datarow.Title, palette);
            }
        }


        private void menuLabelLoadFromFile_Click(object sender, EventArgs e) {
            if(_components.DialogOpenLabel.ShowDialog() == DialogResult.OK) {
                foreach(var path in _components.DialogOpenLabel.FileNames) {
                    this.OpenLabelSequence(path);
                }
            }
        }


        /// <summary>
        /// 指定されたラベル列を持つビューを作成して一覧に追加します．
        /// </summary>
        /// <param name="labelSequence">ラベル列</param>
        /// <param name="title">ビューのタイトル</param>
        /// <param name="colorPaletter">ラベルと表示する色の対応</param>
        /// <returns></returns>
        public SequenceView OpenLabelSequence(ICSLabelSequence labelSequence, string title, IDictionary<string, Color> colorPaletter) {
            SequenceView view = new SequenceView();
            view.SuspendRefresh();
            try {
                view.AttachLabelSequence(labelSequence, title, colorPaletter);
                this.AddView(view);
            } finally {
                view.ResumeRefresh(true);
            }
            return view;
        }
        /// <summary>
        /// 指定されたデータ行のデータからラベル列を持つビューを作成して一覧に追加します．
        /// </summary>
        /// <param name="datarow">データ行</param>
        /// <returns></returns>
        public SequenceView OpenLabelSequence(Plugin.DataRow datarow) {
            ICSLabelSequence labelSequence = new ICSLabelSequence();
            labelSequence.FromDataRow(datarow);
            return OpenLabelSequence(labelSequence, datarow.Title);
        }
        /// <summary>
        /// 指定されたファイルをラベル列として開きます
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public SequenceView OpenLabelSequence(string path) {
            ICSLabelSequence labelSequence = new ICSLabelSequence();
            using(FileStream stream = new FileStream(path, FileMode.Open)) {
                labelSequence.ReadFrom(stream);
            }
            return this.OpenLabelSequence(labelSequence, Path.GetFileNameWithoutExtension(path));
        }

        public void OpenFileByExtension(string path) {
            string ext = Path.GetExtension(path).ToLower();
            try {
                switch(ext) {
                case ".csv":
                    OpenLabelSequence(path);
                    break;
                case ".seq":
                    OpenTimeSeriesValues(path);
                    break;
                case ".tvsv":
                    string path2 = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
                    SequenceView newView = SequenceView.RetrieveState(path2);
                    AddView(newView);
                    break;
                default:
                    MessageBox.Show("未知の形式です: " + path, this.GetType().Name);
                    break;
                }
            } catch(Exception ex) {
                ErrorLogger.Tell(ex, "ファイルが読み込めません: " + path);
            }
        }

        #endregion

        #region ビューパネルの内部データの変更
        /// <summary>
        /// ViewerFunctionの文字列引数に応じてパネルのタイトルを変更する用のメソッド
        /// </summary>
        /// <param name="current"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        private string renamePanelTitleInternal(string current, string replace) {
            SequenceView view = this.GetViewByTitle(current);
            if(view == null)
                return null;
            return this.setUniqueTitleInternal(view, replace);
        }
        /// <summary>
        /// 指定されたビューのタイトルを他と重複しないように変更します
        /// </summary>
        /// <param name="view"></param>
        public void SetUniqueTitle(SequenceView view) {
            this.setUniqueTitleInternal(view, view.Sequence.Title);
        }
        /// <summary>
        /// 指定されたビューのタイトルを他と重複しないように指定されたタイトルにしてそのタイトル文字列を返す
        /// </summary>
        /// <param name="view"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private string setUniqueTitleInternal(SequenceView view, string title) {
            lock(_lockAccessViewList) {
                string original = title;
                original = original.Trim();
                foreach(char c in Path.GetInvalidPathChars()) {
                    original = original.Replace(c, '_');
                }
                string tmp = original;
                for(int count = 2; _viewList.Any(v => v != view && v.Sequence.Title == tmp); count++) {
                    tmp = string.Format("{0} ({1})", original, count);
                }
                if(tmp != view.Sequence.Title) {
                    view.Sequence.Title = tmp;
                    view.Sequence.IsDataChanged = true;
                }
                return view.Sequence.Title;
            }
        }
        /// <summary>
        /// ViewerFunctionを経由して指定されたビューパネルのタイトルを変更します
        /// </summary>
        /// <param name="view">変更対象のビューパネル</param>
        /// <param name="title">新しいタイトル</param>
        public void RenamePanelTitle(SequenceView view, string title) {
            title = title.Trim();
            if(view.Sequence.Title == title || this.ParentControl == null) {
                setUniqueTitleInternal(view, title);
            } else {
                ViewerFunctionScriptFunction.Invoke(this.ParentControl, this, new FunctionRenamePanelTitle(), new StringVariable(view.Sequence.Title), new StringVariable(title));
            }
        }

        /// <summary>
        /// 指定されたタイトルをもとに，タイトルリストに含まれないような一意のタイトルを返します．
        /// </summary>
        /// <param name="title">新しいタイトル名</param>
        /// <param name="prevTitles">他のタイトルリスト</param>
        /// <returns></returns>
        string getUniqueName(string title, List<string> prevTitles) {
            foreach(var c in Path.GetInvalidPathChars()) {
                title = title.Replace(c, '_');
            }
            string ret = getSlicedName(title);
            int num = 1;
            while(prevTitles.Contains(ret)) {
                ret = title + "(" + num.ToString() + ")";
                num++;
            }
            return ret;
        }

        #endregion

        #region ビューパネルの表示関連と時間情報と範囲選択情報
        /// <summary>
        /// 表示範囲や選択範囲を前ビューパネルに反映させる用の同期オブジェクト
        /// </summary>
        readonly Mutex _mutexTimeRangeBroadCasting = new Mutex();
        /// <summary>
        /// AllocationChangedがサスペンドされているときにDoAllocationChangedが呼ばれたかどうか
        /// </summary>
        bool _doAllocationChangedRequested = false;
        /// <summary>
        /// SuspendAllocationChangedが呼ばれた回数 - ResumeAllocationChangedが呼ばれた回数．
        /// </summary>
        int _doAllocationChangedSuspended = 0;

        /// <summary>
        /// ビューパネルの再描画を抑制します
        /// </summary>
        public void SuspendAllocationChanged() {
            Interlocked.Increment(ref _doAllocationChangedSuspended);
        }
        /// <summary>
        /// ビューパネルの再描画の抑制を解除します
        /// </summary>
        public void ResumeAllocationChanged() {
            if(Interlocked.Decrement(ref _doAllocationChangedSuspended) == 0) {
                if(_doAllocationChangedRequested) {
                    _doAllocationChangedRequested = false;
                    this.DoAllocationChanged();
                }
            }
        }



        /// <summary>
        /// パネルの配置の変更を反映させます
        /// </summary>
        public void DoAllocationChanged() {
            if(_doAllocationChangedSuspended != 0) {
                _doAllocationChangedRequested = true;
            } else {

                EventHandler tmp = AllocationChanged;
                if(tmp != null) {
                    tmp.Invoke(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// コントローラの状態を表す文字列が変更されたときに呼ばれるイベント
        /// </summary>
        public event EventHandler<StringEventArgs> StatusMessageChanged;
        /// <summary>
        /// 状態を表す文字列を設定する
        /// </summary>
        /// <param name="text"></param>
        private void setStatusMessage(string text) {
            EventHandler<StringEventArgs> tmp = this.StatusMessageChanged;
            if(tmp != null) {
                tmp.Invoke(this, new StringEventArgs(text));
            }
        }
        /// <summary>
        /// 内包するビューパネルの状態を表す文字列が変更されたときに実行されるメソッド
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onStatusMessageChanged(object sender, StringEventArgs e) {
            EventHandler<StringEventArgs> tmp = this.StatusMessageChanged;
            if(tmp != null) {
                tmp.Invoke(sender, e);
            }
        }
        TimeController _timeController;

        /// <summary>
        /// 現在のカーソル位置の時間を取得または設定します．
        /// </summary>
        public decimal? CursorTime {
            get {
                if(_timeController == null) { return null; }
                return _timeController.CursorTime;
            }
            set {
                if(_timeController != null) {
                    _timeController.CursorTime = value;
                }
            }
        }

        /// <summary>
        /// 現在の時間を取得または設定します．
        /// </summary>
        public decimal CurrentTime {
            get {
                if(_timeController == null)
                    return 0;
                return _timeController.CurrentTime;
            }
            set {
                if(_timeController == null)
                    return;
                _timeController.CurrentTime = value;
            }
        }
        /// <summary>
        /// 現在の時間に対応するフレームインデックスを取得または設定します．
        /// </summary>
        public int CurrentIndex {
            get {
                if(_timeController == null)
                    return 0;
                return _timeController.CurrentIndex;
            }
            set {
                if(_timeController == null)
                    return;
                _timeController.CurrentIndex = value;
            }
        }

        /// <summary>
        /// TimeControllerの終了時間を返します．
        /// </summary>
        public decimal WholeEndTime {
            get {
                decimal tmp = this.EndTime;
                if(_timeController != null && tmp < _timeController.Duration)
                    tmp = _timeController.Duration;
                return tmp;
            }
        }
        /// <summary>
        /// 時間の表示範囲の開始位置を取得または設定します
        /// </summary>
        public decimal VisibleBeginTime {
            get {
                if(_timeController == null)
                    return 0;
                return _timeController.VisibleBeginTime;
            }
            set {
                if(_timeController == null)
                    return;
                _timeController.VisibleBeginTime = value;
            }
        }
        /// <summary>
        /// 時間の表示範囲の終了位置を取得または設定します
        /// </summary>
        public decimal VisibleEndTime {
            get {
                if(_timeController == null)
                    return 0;
                return _timeController.VisibleEndTime;
            }
            set {
                if(_timeController == null)
                    return;
                _timeController.VisibleEndTime = value;
            }
        }
        /// <summary>
        /// 時間の表示範囲の大きさを取得します
        /// </summary>
        public decimal VisibleDuration {
            get {
                if(_timeController == null)
                    return 0;
                return _timeController.VisibleEndTime - _timeController.VisibleBeginTime;
            }
        }
        /// <summary>
        /// 時間範囲が選択されているかどうかを取得します
        /// </summary>
        public bool IsSelecting { get { return _timeController == null ? false : _timeController.IsSelecting; } }
        /// <summary>
        /// 選択されている時間範囲の開始位置を取得します
        /// </summary>
        public decimal SelectBeginTime { get { return _timeController == null ? 0 : _timeController.SelectBeginTime; } }
        /// <summary>
        /// 選択されている時間範囲の終了位置を取得します
        /// </summary>
        public decimal SelectEndTime { get { return _timeController == null ? 0 : _timeController.SelectEndTime; } }
        /// <summary>
        /// 選択されている時間範囲の大きさを取得します
        /// </summary>
        public decimal SelectDuration { get { return this.SelectEndTime - this.SelectBeginTime; } }

        public bool FastRenderingMode { get; set; }
        void _timeController_CursorTimeChanged(object sender, EventArgs e) {
            EventHandler tmp = this.CursorTimeChanged;
            if(tmp != null) {
                tmp.Invoke(this, new EventArgs());
            }
        }

        void _timeController_VisibleRangeChanged(object sender, EventArgs e) {
            EventHandler tmp = this.VisibleRangeChanged;
            if(tmp != null) {
                tmp.Invoke(this, new EventArgs());
            }
        }

        private void _timeController_CurrentTimeChanged(object sender, EventArgs e) {
            EventHandler tmp = this.CurrentTimeChanged;
            if(tmp != null) {
                tmp.Invoke(sender, e);
            }
        }

        void _timeController_SelectedRangeChanged(object sender, EventArgs e) {
            EventHandler tmp = this.SelectedRangeChanged;
            if(tmp != null) {
                tmp.Invoke(sender, e);
            }
        }

        public void DetachTimeController() {
            if(_timeController != null) {
                _timeController.CurrentTimeChanged -= _timeController_CurrentTimeChanged;
                _timeController.VisibleRangeChanged -= _timeController_VisibleRangeChanged;
                _timeController.RemoveTimeInterval(this);
                _timeController.CursorTimeChanged -= _timeController_CursorTimeChanged;
                _timeController.SelectedRangeChanged -= _timeController_SelectedRangeChanged;
                _timeController = null;
            }
        }

        /// <summary>
        /// ビューの再描画を要求します
        /// </summary>
        public void DoRefreshView() {
            EventHandler tmp = this.RefreshView;
            if(tmp != null) {
                tmp.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// 表示時間範囲を設定します
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        public void SetVisibleTime(decimal beginTime, decimal endTime) {
            if(_timeController == null)
                return;
            _timeController.SetVisibleTime(beginTime, endTime);
        }

        /// <summary>
        /// 選択時間範囲を設定します
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        public void SelectRange(decimal beginTime, decimal endTime) {
            if(_timeController == null)
                return;
            _timeController.SelectRange(beginTime, endTime);
        }
        /// <summary>
        /// 選択時間範囲を微調整します．時間範囲の開始位置・終了位置のうち指定された値に近いほうの値を指定された値に設定します
        /// </summary>
        /// <param name="time"></param>
        public void AdjustSelectedRange(decimal time) {
            if(_timeController == null)
                return;
            _timeController.AdjustSelectedRange(time);
        }
        /// <summary>
        /// 時間範囲の選択を解除します
        /// </summary>
        public void DeselectRange() {
            if(_timeController == null)
                return;
            _timeController.DeselectRange();
        }

        /// <summary>
        /// 時間表示範囲を指定されたビューパネルと同じになるようにします
        /// </summary>
        /// <param name="view"></param>
        public void BroadcastVisibleRange(SequenceView view) {
            if(view == null)
                return;
            BroadcastVisibleRange(view.TimeBegin, view.TimeLength);
        }
        public void BroadcastVisibleRange() {
            BroadcastVisibleRange(this.VisibleBeginTime, this.VisibleDuration);
        }
        public void BroadcastVisibleRange(decimal timeBegin, decimal timeLength) {
            if(_mutexTimeRangeBroadCasting.WaitOne(0)) {
                try {
                    if(_timeController != null) {
                        _timeController.SetVisibleTime(timeBegin, timeBegin + timeLength);
                    }
                    doDataChanged();
                } finally {
                    _mutexTimeRangeBroadCasting.ReleaseMutex();
                }
            }
        }
        /// <summary>
        /// 時間選択範囲を指定されたビューパネルと同じになるようにします
        /// </summary>
        /// <param name="view"></param>
        public void BroadcastSelectRange(SequenceView view) {
            if(view == null)
                return;
            this.BroadcastSelectRange(view.TimeBegin, view.TimeLength);
        }
        public void BroadcastSelectRange() {
            this.BroadcastSelectRange(this.SelectBeginTime, this.SelectDuration);
        }
        public void BroadcastSelectRange(decimal timeBegin, decimal timeLength) {
            if(_mutexTimeRangeBroadCasting.WaitOne(0)) {
                try {
                    if(_timeController != null) {
                        _timeController.SelectRange(timeBegin, timeBegin + timeLength);
                    }
                    doDataChanged();
                } finally {
                    _mutexTimeRangeBroadCasting.ReleaseMutex();
                }
            }
        }

        public void SelectAllVisibleRange() {
            _timeController.SelectVisibleRange();
        }
        public void ZoomSelectedRange() {
            _timeController.ZoomSelectedRange();
        }


        #region ITimeInterval メンバ

        public decimal BeginTime { get { return 0; } }

        public decimal EndTime {
            get {
                decimal tmp = 1;
                var subViewList = this.GetViewList();
                if(subViewList.Count != 0) {
                    tmp = subViewList.Max(x => x.Sequence.Values.EndTime);
                }
                return tmp;
            }
        }
        public void DoFocusedViewChanged() {
            EventHandler tmp = this.FocusedViewChanged;
            if(tmp != null) {
                tmp.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler FocusedViewChanged = (s, e) => { };
        public event EventHandler TimeIntervalChanged = (s, e) => { };

        #endregion

        public void DoTimeIntervalChanged() {
            EventHandler tmp = this.TimeIntervalChanged;
            if(tmp != null) {
                tmp.Invoke(this, new EventArgs());
            }
        }

        #endregion

        #region ビューパネルの取得・列挙
        /// <summary>
        /// 内包する各ビューパネルのシーケンスデータのリストを返します
        /// </summary>
        /// <returns></returns>
        public IList<SequenceData> GetSequenceList() {
            lock(_lockAccessViewList) {
                return _viewList.Select(x => x.Sequence).ToList();
            }
        }
        /// <summary>
        /// 内包するビューパネルのリストを返します
        /// </summary>
        /// <returns></returns>
        public IList<SequenceView> GetViewList() {
            lock(_lockAccessViewList) {
                return _viewList.ToList();
            }
        }
        /// <summary>
        /// 指定されたタイトルのビューパネルを返します．ない場合にはnull
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public SequenceView GetViewByTitle(string title) {
            lock(_lockAccessViewList) {
                var list = _viewList.Where(v => v.Sequence.Title == title).ToList();
                if(list.Count == 0)
                    return null;
                return list.First();
            }
        }
        /// <summary>
        /// 指定されたタイトルのビューパネルのシーケンスデータを返します．ない場合にはnull
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public SequenceData GetSequenceByTitle(string title) {
            var view = this.GetViewByTitle(title);
            if(view == null)
                return null;
            return view.Sequence;
        }
        /// <summary>
        /// 現在フォーカスの合っているビューパネルを返します
        /// </summary>
        /// <returns></returns>
        public SequenceView GetFocusedView() {
            foreach(var view in _viewList) {
                if(view.IsFocused)
                    return view;
            }
            return null;
        }

        #endregion

        #region イベント
        /// <summary>
        /// アタッチされたTimeControllerの現在の時間が変更された時に呼び出されるイベント。
        /// </summary>
        public event EventHandler CurrentTimeChanged;
        /// <summary>
        /// アタッチされたTimeControllerのカーソル位置の時間が変更されたときに呼び出されるイベント。
        /// </summary>
        public event EventHandler CursorTimeChanged;
        /// <summary>
        /// ビューアの内容の更新要求がなされた時に呼び出されるイベント。
        /// </summary>
        public event EventHandler RefreshView;
        /// <summary>
        /// ビューアの表示時間幅の変更要求がなされた時に呼び出されるイベント。
        /// </summary>
        public event EventHandler VisibleRangeChanged;
        /// <summary>
        /// 選択時間幅の変更要求がなされた時に呼び出されるイベント。
        /// </summary>
        public event EventHandler SelectedRangeChanged;
        /// <summary>
        /// アタッチされたIPluginHostが変更された時に呼び出されるイベント。
        /// </summary>
        public event EventHandler<IPluginHostChangedEventArgs> IPluginHostChanged = (s, e) => { };
        /// <summary>
        /// 付属しているビューアが追加・移動・削除された時に呼び出されるイベント。
        /// </summary>
        public event EventHandler AllocationChanged = (s, e) => { };
        #endregion

        #region 保存関連
        /// <summary>
        /// シリアライズ時の経過情報を保持する．
        /// </summary>
        readonly ProgressInformation _serializeProgress = new ProgressInformation();

        /// <summary>
        /// 状態を読み込むときのビューパネルのタイトルを保持する．
        /// </summary>
        List<string> _viewNameListForLoadState;
        /// <summary>
        /// 指定されたパスの位置にビューパネル一覧データを保存します
        /// </summary>
        /// <param name="path">保存先のパス</param>
        /// <param name="hasExtension">パスが拡張子を含むかどうか</param>
        public void SaveState(string path, bool hasExtension) {

            if(hasExtension) {
                path = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
            }

            _serializeProgress.Initialize(0, "Initializing...");
            // ヘッダ
            string svsPath = string.Format("{0}.svs", path);
            if(this.IsDataChanged || !File.Exists(svsPath)) {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                using(XmlWriter writer = XmlWriter.Create(svsPath, settings)) {
                    writer.WriteStartElement(typeof(SequenceViewerController).Name);
                    WriteXml(writer, true);
                    writer.WriteEndElement();
                }
            }
            // 本体
            string dir = GetStateDataDirectoryPath(path, false);
            if(!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            IList<SequenceView> viewList = this.GetViewList();
            _serializeProgress.Initialize(viewList.Count, "");
            List<string> usedList = new List<string>();
            foreach(var view in viewList) {
                _serializeProgress.Message = view.Sequence.Title;
                string name = getUniqueName(view.Sequence.Title, usedList);
                string viewPath = Path.Combine(dir, name);
                view.SaveState(viewPath);
                usedList.Add(name);
                _serializeProgress.CurrentValue++;
            }
            this.IsDataChanged = false;
        }

        public void LoadState(string path, bool addPanels) {
            _serializeProgress.Initialize(0, "Initializing...");
            this.SuspendAllocationChanged();
            try {
                if(!addPanels) {
                    this.ClearViewList();
                }
                _serializeProgress.Message = "Loading...";
                Thread.Sleep(10);
                if(!File.Exists(path + ".svs")) {
                    string ext = Path.GetExtension(path);
                    if(ext == ".svs") {
                        path = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
                        if(!File.Exists(path + ".svs")) {
                            throw new FileNotFoundException(path + ".svs: Not found");
                        }
                    } else {
                        throw new FileNotFoundException(path + ".svs: Not found");
                    }
                }
                string svsPath = string.Format("{0}.svs", path);
                using(XmlReader reader = XmlReader.Create(svsPath)) {
                    this.ReadXml(reader);
                }
                _serializeProgress.Initialize(_viewNameListForLoadState.Count, "");

                string dir = GetStateDataDirectoryPath(path, false);
                foreach(var name in _viewNameListForLoadState) {
                    _serializeProgress.Message = name;
                    retrievePanel(dir, name);
                    _serializeProgress.CurrentValue++;
                    // プログレスバーの更新時間を確保
                    Thread.Sleep(10);
                }
                this.DoAllocationChanged();
            } finally { this.ResumeAllocationChanged(); }
            this.IsDataChanged = false;
        }

        /// <summary>
        /// 指定されたパスのビューパネルを追加する．
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="name"></param>
        private void retrievePanel(string dir, string name) {
            if(_parentControl.InvokeRequired) {
                _parentControl.Invoke(new Action<string, string>(retrievePanel), dir, name);
                return;
            }
            string viewPath = Path.Combine(dir, name);
            var view = SequenceView.RetrieveState(viewPath);
            if(view != null) {
                this.AddView(view);
                view.IsDataModified = false;
            }
        }
        public ProgressChangedEventArgs GetSerializeProgress() {
            int percent = _serializeProgress.GetProgressPercentage();
            string message = string.Format("{0} / {1}: {2}", (int)_serializeProgress.CurrentValue, (int)_serializeProgress.MaxValue, _serializeProgress.Message);
            return new ProgressChangedEventArgs(percent, message);
        }

        /// <summary>
        /// 状態保存時のシーケンスのタイトルの保存用の名前を返します．
        /// </summary>
        /// <param name="title">シーケンスのタイトル</param>
        /// <returns></returns>
        string getSlicedName(string title) {
            if(title == "")
                title = "empty~";
            if(title.Length > 48)
                title = title.Substring(0, 48) + "~";
            foreach(var c in Path.GetInvalidFileNameChars()) {
                if(title.Contains(c)) {
                    title = title.Replace(c, '_');
                }
            }
            return title;
        }
        public void WriteXml(XmlWriter writer, bool apartViewList) {
            IList<SequenceView> viewList = this.GetViewList();
            _serializeProgress.Initialize(viewList.Count, "");
            if(!apartViewList) {
                writer.WriteStartElement("Length");
                writer.WriteValue(viewList.Count);
                writer.WriteEndElement();
                foreach(var view in viewList) {
                    _serializeProgress.Message = view.Sequence.Title;
                    writer.WriteStartElement("View");
                    view.Serialize(writer);
                    writer.WriteEndElement();
                    _serializeProgress.CurrentValue++;
                }
            } else {
                List<string> doneList = new List<string>();
                writer.WriteStartElement("ViewNames");
                foreach(var view in viewList) {
                    string name = getUniqueName(view.Sequence.Title, doneList);
                    writer.WriteElementString("Name", name);
                    doneList.Add(name);
                }
                writer.WriteEndElement();
            }
            writer.WriteStartElement("VisibleBeginTime");
            writer.WriteValue(this.VisibleBeginTime);
            writer.WriteEndElement();
            writer.WriteStartElement("VisibleEndTime");
            writer.WriteValue(this.VisibleEndTime);
            writer.WriteEndElement();
        }

        public void Serialize(XmlWriter writer) {
            XmlSerializer sel = new XmlSerializer(typeof(SequenceViewerController));
            sel.Serialize(writer, this);
        }

        public void ReadXml(XmlReader reader) {
            _serializeProgress.Initialize(0, "Initializing...");
            reader.MoveToContent();
            if(reader.IsEmptyElement) {
                reader.Skip();
                return;
            }
            if(reader.Name != typeof(SequenceViewerController).Name
                && reader.Name != "TimeValueSequenceForm"
                && reader.Name != "TimeValueSequenceViewerController")
                throw new XmlException("Node not for " + typeof(SequenceViewerController).Name);
            reader.ReadStartElement();
            reader.MoveToContent();
            _viewNameListForLoadState = new List<string>();
            while(reader.NodeType != XmlNodeType.None && reader.NodeType != XmlNodeType.EndElement) {
                switch(reader.Name) {
                case "Length":
                    _serializeProgress.MaxValue = reader.ReadElementContentAsInt();
                    break;
                case "Viewer":
                case "View":
                    reader.ReadStartElement();
                    var viewer = SequenceView.Deserialize(reader.ReadSubtree());
                    _serializeProgress.Message = viewer.Sequence.Title;
                    this.AddView(viewer);
                    reader.Skip();
                    reader.ReadEndElement();

                    _serializeProgress.CurrentValue++;
                    break;
                case "ViewerNames":
                case "ViewNames":
                    reader.ReadStartElement();
                    reader.MoveToContent();
                    while(reader.NodeType != XmlNodeType.None && reader.NodeType != XmlNodeType.EndElement) {
                        if(reader.Name == "Name") {
                            _viewNameListForLoadState.Add(reader.ReadElementContentAsString());
                        } else {
                            reader.Skip();
                        }
                        reader.MoveToContent();
                    }
                    reader.ReadEndElement();
                    break;
                case "TimeBegin":
                case "VisibleTimeBegin":
                case "VisibleBeginTime":
                    this.VisibleBeginTime = reader.ReadElementContentAsDecimal();
                    break;
                case "VisibleEndTime":
                    this.VisibleEndTime = reader.ReadElementContentAsDecimal();
                    break;
                default:
                    reader.Skip();
                    break;
                }
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public System.Xml.Schema.XmlSchema GetSchema() {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// 指定された保存ファイルから，シーケンスデータの保存ディレクトリを返します．
        /// </summary>
        /// <param name="stateFilePath">保存ファイルのパス</param>
        /// <param name="hasExtension">保存ファイルが拡張子を含むパスであるかどうか</param>
        /// <returns></returns>
        public static string GetStateDataDirectoryPath(string stateFilePath, bool hasExtension) {
            if(hasExtension) {
                stateFilePath = Path.Combine(Path.GetDirectoryName(stateFilePath), Path.GetFileNameWithoutExtension(stateFilePath));
            }
            return string.Format("{0}.Files", stateFilePath);
        }


        #endregion

        #region SequenceOperation 関連
        private readonly object _lockGetSequenceOperation = new object();
        Dictionary<System.Reflection.Assembly, List<ISequenceOperation>> _sequenceOperations = new Dictionary<System.Reflection.Assembly, List<ISequenceOperation>>();
        ISequenceOperation _lastOperation = null;
        IList<ProcParam<SequenceProcEnv>> _lastOperationArgs = null;
        /// <summary>
        /// 前回のシーケンス処理を取得します．
        /// </summary>
        public ISequenceOperation LastOperation { get { return _lastOperation; } }
        /// <summary>
        /// 前回のシーケンス処理の引数を取得します．
        /// </summary>
        public IList<ProcParam<SequenceProcEnv>> LastOperationArgs {
            get {
                if(_lastOperationArgs == null)
                    return null;
                return _lastOperationArgs.ToList();
            }
        }

        /// <summary>
        /// 現在のアセンブリからOperation.ISequenceOperationを実装するクラスを取得します
        /// </summary>
        /// <param name="target">取得するISequenceOperationのOperationTargetType</param>
        /// <returns></returns>
        public IList<ISequenceOperation> GetSequenceOperations(SequenceType target) {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            return this.GetSequenceOperations(asm, target);
        }
        /// <summary>
        /// 指定されたアセンブリからOperation.ISequenceOperationを実装するクラスを取得します．
        /// </summary>
        /// <param name="asm">クラスを取得する元のアセンブリ</param>
        /// <param name="target">取得するISequenceOperationのOperationTargetType</param>
        /// <returns></returns>
        public IList<ISequenceOperation> GetSequenceOperations(System.Reflection.Assembly asm, SequenceType target) {
            lock(_lockGetSequenceOperation) {
                if(!_sequenceOperations.ContainsKey(asm)) {
                    System.Reflection.Module[] modules = asm.GetModules();
                    List<Type> iOperationList = new List<Type>();
                    List<Type> iFunctionList = new List<Type>();
                    foreach(var module in modules) {
                        Type[] types;
                        try {
                            types = module.GetTypes();
                        } catch(System.Reflection.ReflectionTypeLoadException ex) {
                            types = ex.Types.Where(t => t != null).ToArray();
                        }
                        iOperationList.AddRange(from type in types
                                                where type.GetInterfaces().Contains(typeof(ISequenceOperation)) && !type.IsAbstract && !type.IsInterface
                                                select type);
                        iFunctionList.AddRange(from type in types
                                               where type.GetInterfaces().Contains(typeof(IViewerFunction)) && !type.IsAbstract && !type.IsInterface
                                               select type);
                    }
                    List<ISequenceOperation> operations = new List<ISequenceOperation>();
                    foreach(var iOpe in iOperationList) {
                        var ctor = iOpe.GetConstructor(new Type[0]);
                        if(ctor != null) {
                            var obj = ctor.Invoke(new object[0]) as ISequenceOperation;
                            if(obj != null) {
                                operations.Add(obj);
                                SequenceOperationScriptFunction ope = new SequenceOperationScriptFunction(obj);
                                Script.ScriptConsole.Singleton.RegisterFunction(ope);
                            }
                        }
                    }
                    foreach(var iFunc in iFunctionList) {
                        var ctor = iFunc.GetConstructor(new Type[0]);
                        if(ctor != null) {
                            var obj = ctor.Invoke(new object[0]) as IViewerFunction;
                            if(obj != null) {
                                ViewerFunctionScriptFunction ope = new ViewerFunctionScriptFunction(obj);
                                Script.ScriptConsole.Singleton.RegisterFunction(ope);
                            }
                        }
                    }
                    _sequenceOperations[asm] = operations;
                }
            }
            return (from ope
                        in _sequenceOperations.SelectMany(so => so.Value)
                    where (ope.OperationTargetType & target) != 0
                    orderby ope.GetTitle()
                    select ope).ToList();
        }

        /// <summary>
        /// SequenceTypeに応じたデータ処理のメニュー項目のリストを返します
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IList<ToolStripItem> GetOperationToolStripItems(SequenceType type) {
            List<ToolStripItem> ret = new List<ToolStripItem>();
            IList<ISequenceOperation> labelOpes = this.GetSequenceOperations(type);
            foreach(bool replace in new bool[] { false, true }) {
                if(replace) {
                    ret.Add(new ToolStripSeparator());
                }
                foreach(var ope in labelOpes) {
                    if(ope.ReplacesInternalData != replace)
                        continue;
                    var nonIterator = ope;
                    ToolStripItem item = new ToolStripMenuItem(ope.GetTitle(), null, new EventHandler((s, e2) => {
                        try {
                            SequenceView focusedView = this.GetFocusedView();
                            DialogSequenceOperation dialog = new DialogSequenceOperation(nonIterator, this, focusedView.Sequence);
                            if(dialog.ShowDialog() == DialogResult.OK) {
                                this.SetLastOperation(nonIterator, dialog.Args);
                            }
                            focusedView.DoRefreshView();
                        } catch(Exception ex) {
                            ErrorLogger.Tell(ex, Properties.Settings.Default.Msg_FailedToOperate);
                        }
                    }));
                    ret.Add(item);
                }
            }
            return ret;
        }
        /// <summary>
        /// 直前のデータ処理を繰り返す用のメニュー項目のリストを返します．
        /// </summary>
        /// <returns></returns>
        public IList<ToolStripItem> GetLastOperationToolStripItems() {
            List<ToolStripItem> ret = new List<ToolStripItem>();
            // デリゲート用一時変数
            ISequenceOperation lastOpe = this.LastOperation;
            IList<ProcParam<SequenceProcEnv>> lastArgs = this.LastOperationArgs;
            ToolStripItem itemRedisplay = new ToolStripMenuItem(global::MotionDataHandler.Properties.Settings.Default.Menu_ReView + " - " + lastOpe.GetTitle(), null, new EventHandler((s, e2) => {
                try {
                    SequenceView focusedView = this.GetFocusedView();
                    DialogSequenceOperation dialog = new DialogSequenceOperation(lastOpe, this, focusedView.Sequence, lastArgs);
                    if(dialog.ShowDialog() == DialogResult.OK) {
                        this.SetLastOperation(lastOpe, dialog.Args);
                    }
                    focusedView.DoRefreshView();
                } catch(Exception ex) {
                    ErrorLogger.Tell(ex, Properties.Settings.Default.Msg_FailedToOperate);
                }
            }));
            ToolStripItem itemImmediate = new ToolStripMenuItem(global::MotionDataHandler.Properties.Settings.Default.Menu_ReApply + " - " + lastOpe.GetTitle(), null, new EventHandler((s, e2) => {
                try {
                    SequenceView focusedView = this.GetFocusedView();
                    Operation.OperationExecution exec = new Operation.OperationExecution(lastOpe, this, focusedView.Sequence);
                    exec.Parameters = lastArgs;
                    exec.Run(focusedView);
                    focusedView.DoRefreshView();
                } catch(Exception ex) {
                    ErrorLogger.Tell(ex, Properties.Settings.Default.Msg_FailedToOperate);
                }
            }));
            ret.Add(itemRedisplay);
            ret.Add(itemImmediate);
            return ret;
        }

        public void SetLastOperation(ISequenceOperation operation, IList<ProcParam<SequenceProcEnv>> args) {
            _lastOperation = operation;
            _lastOperationArgs = args;
        }

        #endregion

        public IList<ToolStripItem> GetReorderMenus(SequenceView selected) {
            List<ToolStripItem> ret = new List<ToolStripItem>();
            int thisPos = 0;
            foreach(var viewer in this.GetViewList()) {
                if(viewer == selected)
                    break;
                thisPos++;
            }
            int toPos = -thisPos;
            bool before = true;
            foreach(var viewer in this.GetViewList()) {
                int tmpTo = toPos;
                ToolStripItem item;
                if(viewer != selected) {
                    item = new ToolStripMenuItem("Move " + (before ? "before" : "after") + ": " + viewer.Sequence.Title, null, new EventHandler((s, e) => {
                        this.ReorderView(selected, tmpTo);
                    }));
                } else {
                    item = new ToolStripSeparator();
                    before = false;
                }
                ret.Add(item);
                toPos++;
            }
            return ret;
        }













    }
}
