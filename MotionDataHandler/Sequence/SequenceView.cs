using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Drawing.Drawing2D;

namespace MotionDataHandler.Sequence {
    using Misc;
    using ViewerFunction;
    using Script;
    public partial class SequenceView : UserControl, IXmlSerializable, IDataModified {
        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        public SequenceView()
            : this(new SequenceData()) {
            this.Type = SequenceType.None;
        }
        /// <summary>
        /// シーケンスデータから作成するコンストラクタ
        /// </summary>
        /// <param name="sequence"></param>
        public SequenceView(SequenceData sequence) {
            InitializeComponent();
            panelView.AllowDrop = true;
            this.Disposed += onDisposed;
            // 引数のデータをこのビューのデータにする
            this.AttachSequenceData(sequence, false);
            // 表示をリセット
            this.ResetTimeAndValue(false);
            // データ変更をしていないことにする
            this.IsDataModified = false;
        }

        #region IDataChanged
        /// <summary>
        /// データが変更された時に呼び出されるイベント
        /// </summary>
        public event EventHandler DataModified = null;
        private void doDataChanged() {
            if(!_isDataChanged) {
                _isDataChanged = true;
                if(this.DataModified != null) {
                    this.DataModified.Invoke(this, new EventArgs());
                }
            }
            setStateColor();
        }
        private bool _isDataChanged = false;
        public bool IsDataModified {
            get { return _isDataChanged; }
            set {
                if(value) {
                    doDataChanged();
                } else {
                    _isDataChanged = false;
                }
                setStateColor();
            }
        }
        private void onDataChanged(object sender, EventArgs e) {
            doDataChanged();
        }
        #endregion

        private void setStateColor() {
            if(this.InvokeRequired) {
                this.BeginInvoke(new Action(setStateColor));
                return;
            }
            if(_sequence.IsDataChanged) {
                panelStateIcon.BackColor = Color.Yellow;
                if(this.Parent != null) {
                    toolTipState.SetToolTip(this.panelStateIcon, "Data Changed");
                }
            } else if(IsDataModified) {
                panelStateIcon.BackColor = Color.PaleGreen;
                if(this.Parent != null) {
                    toolTipState.SetToolTip(this.panelStateIcon, "State Changed");
                }
            } else {
                panelStateIcon.BackColor = Color.White;
                if(this.Parent != null) {
                    toolTipState.SetToolTip(this.panelStateIcon, "Unchanged");
                }
            }
        }
        #region Property
        RegulatedMouseControl _mouse = new RegulatedMouseControl(0, RegulatedMouseButton.CtrlLeft, RegulatedMouseButton.AltLeft, RegulatedMouseButton.ShiftLeft);
        private readonly object _lockAllocateImage = new object();
        private readonly object _lockDisplayImage = new object();
        bool _suspendRefresh = true;
        bool _enterTextTitle = false;
        private int _viewHeight = -1;

        const int _heightHiddenAtLabel = 16;
        const int _heightPanelTop = 16;
        const int _heightPanelTopHiddenAtLabel = 16;
        const int _heightHiddenAtGraph = 16;
        public const int MinimumHeight = 36;
        public const int SmallHeight = 48;
        public const int MediumHeight = 80;
        public const int LargeHeight = 128;
        public const int MaximumHeight = 4096;
        /// <summary>
        /// 種類に基づいてタイトルラベルの背景色を取得します
        /// </summary>
        public Color AppearanceColor {
            get {
                if(this.Type == SequenceType.Label) {
                    return Color.FromArgb(255, 255, 192);
                } else if(this.Type != SequenceType.NumericLabel) {
                    return Color.FromArgb(192, 255, 192);
                } else {
                    if(this.StackGraphMode) {
                        return Color.FromArgb(192, 192, 255);
                    }
                    return Color.FromArgb(192, 255, 255);
                }
            }
        }
        /// <summary>
        /// ビューの要求する高さを取得または設定します．
        /// </summary>
        public int RequestedHeight {
            get {
                if(this.IsHidden && !_isFocused) {
                    if(this.Type == SequenceType.Label) {
                        return _heightHiddenAtLabel;
                    } else {
                        return _heightHiddenAtGraph;
                    }
                }
                return Math.Max(MinimumHeight, _viewHeight);
            }
            set {
                _viewHeight = Math.Max(MinimumHeight, Math.Min(value, MaximumHeight));
                if(_controller != null)
                    _controller.DoAllocationChanged();
                doDataChanged();
            }
        }
        private bool? _isLocked = null;
        /// <summary>
        /// ビューのラベル編集がロックされているかを取得または設定します．
        /// </summary>
        public bool IsLocked {
            get { return _isLocked ?? false; }
            set {
                setIsLocked(value);
            }
        }
        private void setIsLocked(bool locked) {
            if(this.InvokeRequired) {
                this.Invoke(new Action<bool>(setIsLocked), locked);
                return;
            }
            _isLocked = locked;
            menuLockEdit.Checked = locked;
            if(locked) {
                panelLock.BackgroundImage = global::MotionDataHandler.Properties.Resources._lock;

            } else {
                panelLock.BackgroundImage = global::MotionDataHandler.Properties.Resources.unlock;
            }
        }
        private bool _isHidden;
        /// <summary>
        /// ビューの内容が非表示になっているかを取得または設定します．
        /// </summary>
        public bool IsHidden {
            get { return _isHidden; }
            set {
                menuHide.Checked = _isHidden = value;
                if(this.Parent != null) {
                    // Loadが呼ばれるまで設定しない
                    toolTipState.Hide(panelHideIcon);
                    toolTipState.SetToolTip(panelHideIcon, _isHidden ? "表示" : "隠す");
                }
                if(_isHidden) {
                    Bitmap show = new Bitmap(Properties.Resources.hideView);
                    using(Graphics gfx = Graphics.FromImage(show)) {
                        gfx.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.White)), new Rectangle(Point.Empty, show.Size));
                    }
                    panelHideIcon.BackgroundImage = show;
                } else {
                    panelHideIcon.BackgroundImage = Properties.Resources.hideView;
                }
                doRefreshPanelHeader();
                doDataChanged();
                var p = Properties.Resources.question;
            }
        }
        private SequenceViewerController _controller = null;
        /// <summary>
        /// このビューを所有するコントローラを取得または設定します．
        /// </summary>
        public SequenceViewerController ParentController { get { return _controller; } }
        private SequenceData _sequence = new SequenceData();
        /// <summary>
        /// このビューが持つデータを取得または設定します．
        /// </summary>
        public SequenceData Sequence {
            get {
                Debug.Assert(_sequence != null);
                return _sequence;
            }
            set { this.AttachSequenceData(value, false); }
        }
        decimal _timeBegin;
        decimal _timeLength;
        decimal _valueBegin;
        decimal _valueHeight;
        decimal _prevTimeBegin, _prevTimeLength;
        decimal _prevValueBegin, _prevValueHeight;
        /// <summary>
        /// 表示範囲の開始時間を取得します。
        /// </summary>
        public decimal TimeBegin { get { lock(_lockTimeValue) { return _timeBegin; } } }
        /// <summary>
        /// 表示範囲の時間幅を取得します。
        /// </summary>
        public decimal TimeLength { get { lock(_lockTimeValue) { return _timeLength; } } }
        /// <summary>
        /// 表示範囲の終了時間を取得します。
        /// </summary>
        public decimal TimeEnd { get { lock(_lockTimeValue) { return _timeBegin + _timeLength; } } }
        /// <summary>
        /// 表示範囲の値の下限を取得します。
        /// </summary>
        public decimal ValueBegin { get { lock(_lockTimeValue) { return _valueBegin; } } }
        /// <summary>
        /// 表示範囲の値の幅を取得します。
        /// </summary>
        public decimal ValueHeight { get { lock(_lockTimeValue) { return _valueHeight; } } }
        /// <summary>
        /// 表示範囲の値の上限を取得します。
        /// </summary>
        public decimal ValueEnd { get { lock(_lockTimeValue) { return _valueBegin + _valueHeight; } } }
        private MinMaxTester<decimal> _minmax = new MinMaxTester<decimal>();

        private Bitmap _bitmapView = null;
        private bool _isMouseEnter = false;
        private readonly object _lockTimeValue = new object();
        private Plugin.IPluginHost _pluginHost = null;
        private bool _isFocused = false;
        /// <summary>
        /// ビューがフォーカスされているかを返します
        /// </summary>
        public bool IsFocused { get { return _isFocused; } }

        /// <summary>
        /// ビューの種類(時系列データ/ラベル列)を取得または設定します
        /// </summary>
        public SequenceType Type {
            get { return this.Sequence.Type; }
            set { this.Sequence.Type = value; }
        }

        public int LeftRegionWidth { get { return panelSelect.Width; } set { panelSelect.Width = value; } }
        bool _stackGraphMode = false;

        public bool StackGraphMode {
            get { return _stackGraphMode; }
            set {
                _stackGraphMode = value;
                calculateMinMax();
                doRefreshPanelHeader();
                ResetTimeAndValue(true);
                DoRefreshView();
                doDataChanged();
            }
        }

        protected double _fastModeThreshold {
            get {
                SequenceViewerController controller = _controller;
                if(controller != null) {
                    return controller.FastRenderingMode ? 1.5 : 16777216;
                }
                return 1.5;
            }
        }

        bool _isContextMenuOpened = false;
        bool _isDefaultImage = true;

        public event EventHandler<StringEventArgs> StatusMessageChanged;
        #endregion
        private void doRefreshPanelHeader() {
            panelAll.SuspendLayout();
            Color topColor = AppearanceColor;
            if(_isFocused) {
                textTitle.BackColor = topColor;
            } else {
                textTitle.BackColor = Color.FromArgb(topColor.R / 2 + 96, topColor.G / 2 + 96, topColor.B / 2 + 96);
            }
            labelInfo.BackColor = Color.FromArgb(255 - (255 - topColor.R) / 2, 255 - (255 - topColor.G) / 2, 255 - (255 - topColor.B) / 2);
            if(_isHidden) {
                if(this.Type == SequenceType.Label) {
                    panelHeader.Height = _heightPanelTopHiddenAtLabel;
                } else {
                    panelHeader.Height = _heightPanelTop;
                }
            } else {
                panelHeader.Height = _heightPanelTop;
            }
            panelAll.ResumeLayout();
            textTitle.Text = this.Sequence.Title;
            if(_controller != null)
                _controller.DoAllocationChanged();
        }

        public void AttachPluginHost(Plugin.IPluginHost host) {
            _pluginHost = host;
        }
        private void onPluginHostChanged(object sender, IPluginHostChangedEventArgs e) {
            AttachPluginHost(e.PluginHost);
        }
        public void AttachController(SequenceViewerController controller) {
            if(_controller != null) {
                _controller.RemoveView(this);
                if(_controller != null) {
                    _controller.IPluginHostChanged -= onPluginHostChanged;
                    _controller.CurrentTimeChanged -= onCurrentTimeChanged;
                    _controller.RefreshView -= onRefreshView;
                    _controller.VisibleRangeChanged -= onTimeRangeBroadcasted;
                    _controller.CursorTimeChanged -= _controller_CursorTimeChanged;
                    _controller.SelectedRangeChanged -= _controller_SelectedRangeChanged;
                    _controller = null;
                }
            }
            _controller = controller;
            if(_controller != null) {
                _controller.AddView(this);
                _controller.IPluginHostChanged += onPluginHostChanged;
                _controller.CurrentTimeChanged += onCurrentTimeChanged;
                _controller.RefreshView += onRefreshView;
                _controller.VisibleRangeChanged += onTimeRangeBroadcasted;
                _controller.SelectedRangeChanged += _controller_SelectedRangeChanged;
                _controller.CursorTimeChanged += _controller_CursorTimeChanged;
                _controller.BroadcastVisibleRange();
            }
        }

        public void DetachController() {
            this.AttachController(null);
        }

        void _controller_SelectedRangeChanged(object sender, EventArgs e) {
            SequenceViewerController controller = _controller;
            if(controller == null)
                return;
            if(controller.IsSelecting) {
                selectTimeRangeInternal(controller.SelectBeginTime, controller.SelectEndTime);
            } else {
                deselectTimeRangeInternal();
            }
        }

        void _controller_CursorTimeChanged(object sender, EventArgs e) {
            if(_controller != null) {
                setCursorTimeBar(_controller.CursorTime);
            }
        }



        private void onDisposed(object sender, EventArgs e) {
            DetachController();
            if(!Sequence.IsDisposed) {
                Sequence.DataChanged -= onDataChanged;
                Sequence.Dispose();
            }
        }


        private void onTimeRangeBroadcasted(object sender, EventArgs e) {
            SequenceViewerController ctrl = sender as SequenceViewerController;
            SetVisibleTimeRange(ctrl.VisibleBeginTime, ctrl.VisibleDuration);
        }

        private void setStatusMessage(string text) {
            EventHandler<StringEventArgs> tmp = this.StatusMessageChanged;
            if(tmp != null) {
                tmp.Invoke(this, new StringEventArgs(text));
            }
        }

        IList<string> prevNames = new List<string>();
        private void setupComboBorder() {
            if(this.InvokeRequired) {
                this.Invoke(new Action(setupComboBorder));
                return;
            }
            _comboBorderEntered = false;
            comboBorder.Text = global::MotionDataHandler.Properties.Settings.Default.Menu_SetLabelHere;
            comboBorder.ForeColor = Color.DarkGray;

            var names = Sequence.Borders.GetLabelNames(true);
            if(names.SequenceEqual(prevNames))
                return;
            prevNames = names;

            comboBorder.Items.Clear();
            //comboBorder.Items.Add(global::MotionDataHandler.Properties.Settings.Default.SetLabelHere);

            foreach(var name in names) {
                comboBorder.Items.Add(name);
            }
        }

        private decimal getCursorTime() {
            SequenceViewerController controller = _controller;
            if(controller != null) {
                decimal? cursor = controller.CursorTime;
                if(cursor.HasValue)
                    return cursor.Value;
                return controller.CurrentTime;
            }
            return offsetToTime(_mouse.Location.X);
        }

        private decimal getCursorValue() {
            return offsetToValue(_mouse.Location.Y);
        }

        private string _currentLabel = null;
        private decimal?[] _currentValues = null;
        string _prevLabel = null;
        int _prevIndex = -1;
        private void onCurrentTimeChanged(object sender, EventArgs e) {
            TimeController time = sender as TimeController;
            if(this.Type != SequenceType.Numeric) {
                _currentLabel = this.Sequence.GetLabelAt(time.CurrentTime);
                if(_prevLabel != _currentLabel) {
                    setInformationText();
                    _prevLabel = _currentLabel;
                }
            } else {
                int index = Sequence.Values.GetIndexAt(time.CurrentTime);
                if(index != _prevIndex) {
                    _currentValues = Sequence.Values.GetValueAt(time.CurrentTime);
                    setInformationText();
                    _prevIndex = index;
                }
            }
            setCurrentTimeBar();
        }

        private void setInformationText() {
            if(this.InvokeRequired) {
                this.Invoke(new Action(setInformationText));
                return;
            }
            SequenceViewerController controller = _controller;
            if(controller == null)
                return;

            string ret = " Info: ";
            if(this.Type != SequenceType.Numeric) {
                if(_currentLabel != null)
                    ret += "label: " + _currentLabel;
            } else {
                if(_currentValues == null) {
                    ret += "no data";
                } else {
                    for(int i = 0; i < this.Sequence.Values.ColumnCount; i++) {
                        ret += this.Sequence.Values.ColumnNames[i] + ":" + (_currentValues[i].HasValue ? _currentValues[i].Value.ToString("F3") : "empty") + " ";
                    }
                }
            }
            labelInfo.Text = ret;
        }



        public void SetValues(TimeSeriesValues values) {
            if(values == null)
                throw new ArgumentNullException("values", "'values' cannot be null");
            Sequence.Values = values;
            calculateMinMax();
            ResetTimeAndValue(true);
        }

        public void SetTitle(string title) {
            if(this.InvokeRequired) {
                this.BeginInvoke(new Action<string>(SetTitle), title);
                return;
            }
            if(title == null)
                title = "unnamed";

            if(_controller != null) {
                _controller.RenamePanelTitle(this, title);
            }

            if(textTitle.Text != this.Sequence.Title) {
                textTitle.Text = this.Sequence.Title;
            }
        }

        public void AttachSequenceData(SequenceData sequence, bool changeHeight) {
            _sequence.DataChanged -= onDataChanged;
            _sequence.PropertyTypeChanged -= Sequence_PropertyChanged;

            _sequence = sequence ?? new SequenceData();

            _sequence.DataChanged += onDataChanged;
            _sequence.PropertyTypeChanged += Sequence_PropertyChanged;
            if(!_isLocked.HasValue) {
                bool locked = false;
                if(_sequence.Type == SequenceType.Label) {
                    if(_sequence.Borders.BorderCount > 0 && _sequence.Values.SequenceLength > 0)
                        locked = true;
                }
                this.IsLocked = locked;
            }
            this.SetValues(_sequence.Values);
            this.SetTitle(_sequence.Title);
            this.SetDefaultHeight(changeHeight);
            doDataChanged();
        }

        public void SetDefaultHeight(bool forceChange) {
            if(_viewHeight < 0 || forceChange) {
                if(this.IsHidden) {
                    if(this.Type == SequenceType.Label) {
                        this.RequestedHeight = _heightHiddenAtLabel;
                    } else {
                        this.RequestedHeight = _heightHiddenAtGraph;
                    }
                } else {
                    if(this.Type == SequenceType.Label) {
                        this.RequestedHeight = MinimumHeight;
                    } else {
                        this.RequestedHeight = LargeHeight;
                    }
                }
            }
        }

        void Sequence_PropertyChanged(object sender, SequenceData.PropertyTypeEventArgs e) {
            SequenceViewerController controller = _controller;
            if(controller == null)
                return;
            switch(e.PropertyType) {
            case SequenceData.PropertyType.Type:
                controller.SetUniqueTitle(this);
                SetDefaultHeight(true);
                doRefreshPanelHeader();
                DoRefreshView();
                menuGraphStack.Enabled = this.Type == SequenceType.Numeric;
                if((this.Type & SequenceType.Numeric) != 0) {
                    calculateMinMax();
                }
                break;
            case SequenceData.PropertyType.Title:
                doRefreshPanelHeader();
                break;
            case SequenceData.PropertyType.Borders:
            case SequenceData.PropertyType.Values:
                this.DoRefreshView();
                break;
            }
        }


        public void calculateMinMax() {
            MinMaxTester<decimal> minmax = new MinMaxTester<decimal>();
            foreach(var pair in Sequence.Values.Enumerate()) {
                if(StackGraphMode) {
                    if(pair.Value.All(x => x.HasValue)) {
                        decimal sum = 0;
                        minmax.TestValue(sum);
                        foreach(var value in pair.Value) {
                            sum += value.Value;
                            minmax.TestValue(sum);
                        }
                    }
                } else {
                    foreach(var value in pair.Value) {
                        if(value.HasValue) {
                            minmax.TestValue(value.Value);
                        }
                    }
                }
            }
            _minmax = minmax;
        }

        private void setCommonTimeRangeChecked(bool check) {
            if(this.InvokeRequired) {
                this.BeginInvoke(new Action<bool>(setCommonTimeRangeChecked), check);
                return;
            }
            menuCommonTimeRange.Checked = check;
        }

        public void ResetTimeAndValue(bool valueOnly) {
            if(!valueOnly) {
                if(_controller == null) {
                    this.SetVisibleTimeRange(Sequence.Values.BeginTime, Sequence.Values.Duration);
                } else {
                    this.SetVisibleTimeRange(_controller.BeginTime, _controller.WholeEndTime);
                }
            }
            if(_minmax.IsValid) {
                this.SetVisibleValueRange(_minmax.Min, 0);
            } else {
                this.SetVisibleValueRange(0, 0);
            }
        }

        public void SetVisibleTimeRange(decimal begin) {
            SetVisibleTimeRange(begin, _timeLength);
        }
        public void SetVisibleTimeRange(decimal begin, decimal length) {
            _prevTimeBegin = _timeBegin;
            _prevTimeLength = _timeLength;
            if(length == 0)
                length = 1;
            if(length < 0)
                length = -length;

            lock(_lockTimeValue) {
                if(_timeBegin == begin && _timeLength == length)
                    return;
                _timeBegin = begin;
                _timeLength = length;
            }
            //doDataChanged();
            this.DoRefreshView();
            if(_controller != null) {
                _controller.BroadcastVisibleRange(this);
            }
        }
        public void SetVisibleValueRange(decimal begin) {
            this.SetVisibleValueRange(begin, _valueHeight);
        }

        public void SetVisibleValueRange(decimal begin, decimal height) {
            _prevValueBegin = _valueBegin;
            _prevValueHeight = _valueHeight;
            if(_minmax.IsValid) {
                decimal range = _minmax.Max - _minmax.Min;
                decimal inf = _minmax.Min - range / 30M;
                decimal sup = _minmax.Max + range / 30M;
                if(height <= 0 || height > sup - inf)
                    height = sup - inf;
                if(begin < inf)
                    begin = inf;
                if(begin + height > sup)
                    begin = sup - height;
            } else {
                begin = -1;
                height = 2;
            }
            lock(_lockTimeValue) {
                if(_valueBegin == begin && _valueHeight == height)
                    return;
                _valueBegin = begin;
                _valueHeight = height;
            }
            //doDataChanged();
            this.DoRefreshView();
        }

        /// <summary>
        /// 時間および値の範囲からビュー内の選択範囲を求めます．
        /// </summary>
        /// <returns></returns>
        private RectangleF getSelectedRectangle() {
            float left = 0f, right = 0f, top = 0f, bottom = 0f;
            if(_isTimeRangeSelected) {
                left = timeToOffset(_selectTimeBegin);
                right = timeToOffset(_selectTimeEnd);
            }
            if(_isValueRangeSelected) {
                top = valueToOffset(_selectValueMax);
                bottom = valueToOffset(_selectValueMin);
            }
            return new RectangleF(left, top, right - left, bottom - top);
        }
        /// <summary>
        /// データからデータ画像を作成するよう要求します。
        /// </summary>
        private void requestAllocateImage() {
            if(_isDefaultImage) {
                pictView.Image = null;
                _isDefaultImage = false;
            }
            if(bgAllocateImage.IsBusy) {
                _imageAllocateRequested = true;
                return;
            }
            bgAllocateImage.RunWorkerAsync();
        }

        /// <summary>
        /// 始点と終点からドラッグ領域を示す矩形を返します．
        /// </summary>
        /// <param name="begin">ドラッグの視点</param>
        /// <param name="end">ドラッグの終点</param>
        /// <param name="lengthLimitAsZero">ドラッグとみなさない各軸の移動幅</param>
        /// <param name="isXNegative">水平方向に関して負の方向にドラッグされたかどうか</param>
        /// <param name="isYNegative">垂直方向に関して負の方向にドラッグされたかどうか</param>
        /// <param name="isXZero">水平方向にドラッグされていないかどうか</param>
        /// <param name="isYZero">垂直方向にドラッグされていないかどうか</param>
        /// <returns></returns>
        static Rectangle getDraggedRectangle(Point begin, Point end, int lengthLimitAsZero, out bool isXNegative, out bool isYNegative, out bool isXZero, out bool isYZero) {
            int x = begin.X;
            int y = begin.Y;
            int dx = end.X - begin.X;
            int dy = end.Y - begin.Y;
            isXNegative = false;
            isYNegative = false;
            isXZero = false;
            isYZero = false;
            if(dx < 0) {
                isXNegative = true;
                x += dx;
                dx = -dx;
            }
            if(dy < 0) {
                isYNegative = true;
                y += dy;
                dy = -dy;
            }

            if(Math.Abs(dx) <= lengthLimitAsZero)
                isXZero = true;
            if(Math.Abs(dy) <= lengthLimitAsZero)
                isYZero = true;
            return new Rectangle(x, y, dx, dy);
        }

        int _lengthAsDragging = 8;
        /// <summary>
        /// ドラッグした矩形を記録します
        /// </summary>
        /// <param title="begin">ドラッグの開始位置</param>
        /// <param title="end">ドラッグの終了位置</param>
        public void DoDragViewArea(Point begin, Point end, int lengthAsDrag) {
            bool negX, negY, zeroX, zeroY;

            Rectangle area = getDraggedRectangle(begin, end, lengthAsDrag, out negX, out negY, out zeroX, out zeroY);
            bool valueSelect = !zeroY;
            bool timeSelect = !zeroX;
            if(this.Type == SequenceType.Label) {
                valueSelect = false;
                timeSelect = area.Width != 0;
            }

            if(valueSelect) {
                this.SelectValueRange(offsetToValue(area.Bottom), offsetToValue(area.Top));
            } else {
                this.DeselectValueRange();
            }
            if(timeSelect) {
                this.SelectTimeRange(offsetToTime(area.Left), offsetToTime(area.Right));
            } else {
                this.DeselectTimeRange();
            }
        }

        volatile bool _imageDisplayRequested = false;
        /// <summary>
        /// データ画像から表示画像を作成して表示します。
        /// </summary>
        private void displayImage() {
            lock(_lockDisplayImage) {
                if(!_imageDisplayRequested) {
                    _imageDisplayRequested = true;
                    if(!bgDisplayImage.IsBusy) {
                        bgDisplayImage.RunWorkerAsync();
                    }
                }
            }
        }
        /// <summary>
        /// カーソル位置の情報等を描画します．
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgDisplayImage_DoWork(object sender, DoWorkEventArgs e) {
            lock(_lockDisplayImage) {
                _imageDisplayRequested = false;
                if(_imageAllocateRequested)
                    return;
            }
            Bitmap bmpDisp = null;
            lock(_lockAllocateImage) {
                Bitmap tmp = _bitmapView;
                if(tmp == null)
                    return;
                bmpDisp = new Bitmap(tmp);
            }

            lock(_lockDisplayImage) {
                int width = bmpDisp.Width;
                int height = bmpDisp.Height;
                Brush brushBgText = new SolidBrush(Color.FromArgb(224, 248, 248, 248));
                using(Graphics gfx = Graphics.FromImage(bmpDisp)) {
                    Font font = new Font(this.Font, FontStyle.Regular);
                    if(_isSelecting) {
                        Pen pen = new Pen(Color.FromArgb(128, 255, 0, 0));
                        Brush brushStrBg = new SolidBrush(Color.FromArgb(196, 128, 0, 0));
                        Brush brushArea = new SolidBrush(Color.FromArgb(32, 0, 0, 0));

                        string strSelectWidth = (_selectTimeEnd - _selectTimeBegin).ToString("F3");
                        string strSelectHeight = (_selectValueMax - _selectValueMin).ToString("F3");
                        SizeF sizeSelectWidth = gfx.MeasureString(strSelectWidth, font);
                        SizeF sizeSelectHeight = gfx.MeasureString(strSelectHeight, font);
                        RectangleF area = getSelectedRectangle();

                        if(_isTimeRangeSelected && _isValueRangeSelected && this.Type != SequenceType.Label) {
                            // 矩形範囲選択
                            gfx.FillRectangle(brushArea, area);
                            gfx.DrawRectangle(pen, area.Left, area.Top, area.Width, area.Height);
                            if(_isFocused) {
                                PointF pointSelectHeight = new PointF(area.X - sizeSelectHeight.Width, area.Top + (area.Height - sizeSelectHeight.Height) / 2);
                                gfx.FillRectangle(brushBgText, new RectangleF(pointSelectHeight, sizeSelectHeight));
                                gfx.DrawString(strSelectHeight, font, brushStrBg, pointSelectHeight);
                                PointF pointSelectWidth = new PointF(area.X + (area.Width - sizeSelectWidth.Width) / 2 + 1, area.Y - sizeSelectHeight.Height - 1);
                                gfx.FillRectangle(brushBgText, new RectangleF(pointSelectWidth, sizeSelectWidth));
                                gfx.DrawString(strSelectWidth, font, brushStrBg, pointSelectWidth);
                            }
                        } else if(_isTimeRangeSelected) {
                            // 時間範囲選択
                            gfx.FillRectangle(brushArea, new RectangleF(area.X, 0, area.Width, height));
                            if(_isFocused) {
                                gfx.DrawRectangle(pen, area.X, 0, area.Width, height);
                                gfx.DrawLine(pen, new PointF(area.X, _mouse.DownLocation.Y), new PointF(area.Right, _mouse.DownLocation.Y));
                                float pointY = _mouse.DownLocation.Y - sizeSelectHeight.Height - 1;
                                PointF pointSelectWidth = new PointF(area.X + (area.Width - sizeSelectWidth.Width) / 2 + 1, pointY < 0 ? 0 : pointY);
                                if(area.Width < sizeSelectWidth.Width)
                                    pointSelectWidth.X = area.Right + 2;
                                gfx.FillRectangle(brushBgText, new RectangleF(pointSelectWidth, sizeSelectWidth));
                                gfx.DrawString(strSelectWidth, font, brushStrBg, pointSelectWidth);
                            }
                        } else if(_isValueRangeSelected && this.Type != SequenceType.Label) {
                            // 値範囲選択
                            gfx.FillRectangle(brushArea, new RectangleF(0, area.Y, width, area.Height));
                            if(_isFocused) {
                                gfx.DrawRectangle(pen, 0, area.Y, width, area.Height);
                                gfx.DrawLine(pen, new PointF(_mouse.DownLocation.X, area.Y), new PointF(_mouse.DownLocation.X, area.Bottom));
                                PointF pointSelectHeight = new PointF(_mouse.DownLocation.X, area.Top + (area.Height - sizeSelectHeight.Height) / 2);
                                gfx.FillRectangle(brushBgText, new RectangleF(pointSelectHeight, sizeSelectHeight));
                                gfx.DrawString(strSelectHeight, font, brushStrBg, pointSelectHeight);
                            }
                        }
                    }
                    if(_isMouseEnter) {
                        Brush brushBg = new SolidBrush(Color.FromArgb(224, 248, 248, 248));
                        using(this.Sequence.Lock.GetReadLock()) {
                            decimal time = getCursorTime();
                            int index = Sequence.Values.GetIndexAt(time);
                            decimal seqTime;
                            decimal?[] values;
                            if(this.Sequence.Values.TryGetTimeFromIndex(index, out seqTime) && Sequence.Values.TryGetValueFromIndex(index, out values)) {
                                Pen penCross = new Pen(Color.FromArgb(128, 255, 0, 0));
                                Point axis = _mouse.Location;

                                if(this.Type != SequenceType.Label) {
                                    gfx.DrawLine(penCross, new Point(0, axis.Y), new Point(pictView.Width, axis.Y));
                                    string valueString = string.Format("Value: {0}", getCursorValue().ToString("F3"));
                                    SizeF sizeValue = gfx.MeasureString(valueString, font);
                                    PointF posValue = new PointF(0, axis.Y < sizeValue.Height + 8 ? axis.Y : axis.Y - sizeValue.Height);
                                    gfx.FillRectangle(brushBgText, new RectangleF(posValue, sizeValue));
                                    gfx.DrawString(valueString, font, Brushes.SlateBlue, posValue);
                                }

                                gfx.DrawLine(penCross, new Point(axis.X, 0), new Point(axis.X, pictView.Height));
                                if(!_isSelecting && this.Type != SequenceType.Label) {
                                    for(int i = 0; i < Sequence.Values.ColumnCount; i++) {
                                        if(!values[i].HasValue)
                                            continue;
                                        Brush brush = new SolidBrush(ColorEx.ColorFromHSV((float)(Math.PI * 2 * i / Sequence.Values.ColumnCount), 0.8f, 0.5f));
                                        string dataString = Sequence.Values.ColumnNames[i] + ":" + values[i].Value.ToString("F3");
                                        SizeF sizeData = gfx.MeasureString(dataString, font);
                                        int top = (int)(valueToOffset(values[i].Value) - sizeData.Height / 2);
                                        top = Math.Min(Math.Max(top, 0), (int)(height - sizeData.Height));

                                        int left = axis.X + i * 2;
                                        if(sizeData.Width > pictView.Width - axis.X - 16) {
                                            left = (int)(_mouse.Location.X - sizeData.Width - i * 2);
                                        }
                                        gfx.FillRectangle(brushBgText, new RectangleF(left, top, sizeData.Width, sizeData.Height));
                                        gfx.DrawString(dataString, font, brush, new PointF(left, top));

                                    }
                                }

                            }
                        }


                        Brush brushRange = new SolidBrush(Color.FromArgb(196, 0, 0, 0));
                        if(this.Type != SequenceType.Label) {
                            string strValueMin = ValueBegin.ToString("F3");
                            string strValueMax = ValueEnd.ToString("F3");
                            SizeF sizeValueMin = gfx.MeasureString(strValueMin, font);
                            SizeF sizeValueMax = gfx.MeasureString(strValueMax, font);
                            PointF posValueMin = new PointF((width - sizeValueMin.Width) / 2, height - sizeValueMin.Height);
                            PointF posValueMax = new PointF((width - sizeValueMax.Width) / 2, 0);
                            gfx.FillRectangle(brushBg, new RectangleF(posValueMin, sizeValueMin));
                            gfx.FillRectangle(brushBg, new RectangleF(posValueMax, sizeValueMax));
                            gfx.DrawString(strValueMin, font, brushRange, posValueMin);
                            gfx.DrawString(strValueMax, font, brushRange, posValueMax);
                        }
                        string strTimeMin = TimeBegin.ToString("F3");
                        string strTimeMax = TimeEnd.ToString("F3");
                        SizeF sizeTimeMin = gfx.MeasureString(strTimeMin, font);
                        SizeF sizeTimeMax = gfx.MeasureString(strTimeMax, font);
                        PointF posTimeMin = new PointF(0, (height - sizeTimeMin.Height) / 2);
                        PointF posTimeMax = new PointF(width - sizeTimeMax.Width, (height - sizeTimeMax.Height) / 2);
                        gfx.FillRectangle(brushBg, new RectangleF(posTimeMin, sizeTimeMin));
                        gfx.FillRectangle(brushBg, new RectangleF(posTimeMax, sizeTimeMax));
                        gfx.DrawString(strTimeMin, font, brushRange, posTimeMin);
                        gfx.DrawString(strTimeMax, font, brushRange, posTimeMax);
                    }
                    setPictViewImage(bmpDisp);
                }
            }

        }

        private void bgDisplayImage_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            lock(_lockDisplayImage) {
                if(_imageDisplayRequested) {
                    _imageDisplayRequested = false;
                    displayImage();
                }
            }
        }

        private void setPictViewImage(Image img) {
            if(this.InvokeRequired) {
                if(this.ParentForm != null && !this.IsDisposed) {
                    try {
                        this.BeginInvoke(new Action<Image>(setPictViewImage), img);
                    } catch(ObjectDisposedException) { }
                }
                return;
            }
            pictView.Image = img;
        }

        private int valueToOffset(decimal value) {
            if(this.ValueHeight <= 0)
                return 0;
            try {
                return (pictView.Height - 1) - (int)decimal.Floor((pictView.Height - 1) * (value - ValueBegin) / ValueHeight);
            } catch(ArithmeticException) {
                return 0;
            }
        }

        private int timeToOffset(decimal time) {
            if(this.TimeLength <= 0)
                return 0;
            try {
                return (int)decimal.Floor((pictView.Width - 1) * (time - TimeBegin) / TimeLength);
            } catch(ArithmeticException) {
                return 0;
            }
        }

        private decimal offsetToTime(float offset) {
            return offsetDeltaToTime(offset) + this.TimeBegin;
        }
        private decimal offsetDeltaToTime(float delta) {
            if(pictView.Width <= 1)
                return 0;
            try {
                return this.TimeLength * (decimal)delta / (pictView.Width - 1);
            } catch(ArithmeticException) {
                return 0;
            }
        }

        private decimal offsetToValue(float offset) {
            return this.ValueEnd - offsetDeltaToValue(offset);
        }

        private decimal offsetDeltaToValue(float delta) {
            if(pictView.Height <= 1)
                return 0;
            try {
                return this.ValueHeight * (decimal)delta / (pictView.Height - 1);
            } catch(ArithmeticException) {
                return 0;
            }
        }

        /// <summary>
        /// 現在の再生位置のバーを描画します
        /// </summary>
        private void setCurrentTimeBar() {
            if(this.InvokeRequired) {
                this.BeginInvoke(new Action(setCurrentTimeBar));
                return;
            }
            if(_controller == null) {
                pictCurrent.Visible = false;
            } else {
                int offset = timeToOffset(_controller.CurrentTime);
                if(offset < 0 || offset > pictView.Width) {
                    pictCurrent.Visible = false;
                } else {
                    pictCurrent.Visible = true;
                    pictCurrent.Location = new Point(offset, pictView.Location.Y);
                    pictCurrent.Size = new Size(1, pictView.Height);
                }
            }
        }

        private void setCursorTimeBar(decimal? time) {
            if(this.InvokeRequired) {
                this.BeginInvoke(new Action<decimal?>(setCursorTimeBar), time);
                return;
            }
            if(time.HasValue) {
                int offset = timeToOffset(time.Value);
                if(offset < 0 || offset > pictView.Width) {
                    pictCursor.Visible = false;
                } else {
                    pictCursor.Visible = true;
                    pictCursor.Location = new Point(offset, pictView.Location.Y);
                    pictCursor.Size = new Size(1, pictView.Height);
                }
            } else {
                pictCursor.Visible = false;
            }
        }
        /// <summary>
        /// ビューの更新を抑制します
        /// </summary>
        public void SuspendRefresh() {
            _suspendRefresh = true;
        }
        /// <summary>
        /// ビューの更新の抑制を解除します
        /// </summary>
        /// <param name="refreshImmediate">ビューをただちに更新します</param>
        public void ResumeRefresh(bool refreshImmediate) {
            _suspendRefresh = false;
            if(refreshImmediate) {
                this.DoRefreshView();
            }
        }

        /// <summary>
        /// ビューを再描画します
        /// </summary>
        public void DoRefreshView() {
            if(!_suspendRefresh) {
                requestAllocateImage();
                setCurrentTimeBar();
                SequenceViewerController controller = _controller;
                if(controller != null) {
                    setCursorTimeBar(controller.CursorTime);
                    controller.DoTimeIntervalChanged();
                }
            }
        }

        public void onRefreshView(object sender, EventArgs e) {
            DoRefreshView();
        }

        /// <summary>
        /// 範囲選択を解除します
        /// </summary>
        public void DeselectRange() {
            this.DeselectTimeRange();
            this.DeselectValueRange();
        }
        /// <summary>
        /// 表示範囲が選択範囲のサイズになるように縮小した新しい表示範囲を返します
        /// </summary>
        /// <param name="draggedArea"></param>
        /// <returns></returns>
        private RectangleF GetUnzoomRectangle(RectangleF draggedArea) {
            float left = 0;
            float top = 0;
            float right = pictView.Width;
            float bottom = pictView.Height;
            if(draggedArea.Width > 0) {
                left = -pictView.Width * draggedArea.Left / draggedArea.Width;
                right = pictView.Width * (pictView.Width - draggedArea.Left) / draggedArea.Width;
            }
            if(draggedArea.Height > 0) {
                top = -pictView.Height * draggedArea.Top / draggedArea.Height;
                bottom = pictView.Height * (pictView.Height - draggedArea.Top) / draggedArea.Height;
            }
            return new RectangleF(left, top, right - left, bottom - top);
        }

        private void menuOpen_Click(object sender, EventArgs e) {
            if(_controller == null)
                return;
            _controller.OpenTimeSeriesValuesWithDialog();
        }

        private void SequenceView_Load(object sender, EventArgs e) {
            this.SetTitle(this.Sequence.Title);
            setTextTitleWidth();
            this.ResumeRefresh(true);
        }

        private bool selectedAreaContains(Point location) {
            RectangleF area = getSelectedRectangle();
            if(!_isTimeRangeSelected) {
                if(!_isValueRangeSelected) {
                    return false;
                } else {
                    return area.Top <= location.Y && location.Y < area.Bottom;
                }
            } else {
                if(!_isValueRangeSelected) {
                    return area.Left <= location.X && location.X < area.Right;
                } else {
                    return area.Left <= location.X && location.X < area.Right && area.Top <= location.Y && location.Y < area.Bottom;
                }
            }
        }

        readonly ToolTip _toolTipValue = new ToolTip();
        Point _pointPrevToolTip = Point.Empty;
        bool _usePanelToolTip = false;
        private void pictView_MouseMove(object sender, MouseEventArgs e) {
            SequenceViewerController controller = _controller;
            if(controller == null)
                return;
            _isMouseEnter = true;
            RegulatedMouseInfo info = _mouse.MouseMove(e);
            decimal cursorTime = offsetToTime(e.X);
            _cursorLabel = this.Sequence.GetLabelAt(cursorTime);
            //cursorTime = Math.Min(_timeBegin + _timeLength, Math.Max(_timeBegin, cursorTime));

            switch(info.Button) {
            case RegulatedMouseButton.Left:
                this.DoDragViewArea(_mouse.DownLocation, _mouse.Location, _lengthAsDragging);
                break;
            case RegulatedMouseButton.CtrlLeft:
                this.DoDragViewArea(_mouse.DownLocation, _mouse.Location, _lengthAsDragging * 2);
                break;

            case RegulatedMouseButton.AltLeft:
                if(_mouse.MoveDelta != Point.Empty) {
                    this.SetVisibleTimeRange(this.TimeBegin - offsetDeltaToTime(_mouse.MoveDelta.X));
                    this.SetVisibleValueRange(this.ValueBegin + offsetDeltaToValue(_mouse.MoveDelta.Y));
                }
                break;
            case RegulatedMouseButton.ShiftLeft:
                controller.AdjustSelectedRange(cursorTime);
                break;
            }

            displayImage();
            _controller.CursorTime = cursorTime;
            setCurrentTimeBar();

            if(selectedAreaContains(e.Location)) {
                panelView.ContextMenuStrip = contextMenuArea;
            } else {
                panelView.ContextMenuStrip = contextMenu;
            }

            if(_usePanelToolTip) {
                if(_pointPrevToolTip != e.Location) {
                    _toolTipValue.UseFading = false;
                    string title, text;
                    if(this.Type == SequenceType.Label) {
                        title = cursorTime.ToString("0.000");
                        string label = this.Sequence.GetLabelAt(cursorTime) ?? "";
                        text = "label: " + label;
                    } else {
                        decimal value = offsetToValue(e.Y);
                        title = cursorTime.ToString("0.000") + ", " + value.ToString("0.000");
                        decimal?[] values = this.Sequence.Values.GetValueAt(cursorTime);
                        StringBuilder textStr = new StringBuilder();
                        for(int i = 0; i < this.Sequence.Values.ColumnCount; i++) {
                            if(i != 0)
                                textStr.AppendLine();
                            textStr.AppendFormat("{0}: {1}", this.Sequence.Values.ColumnNames[i], (values[i].HasValue ? values[i].Value.ToString("0.000") : "empty"));
                        }
                        text = textStr.ToString();
                    }
                    _toolTipValue.ToolTipTitle = title;
                    _toolTipValue.Show(text, this, Point.Add(e.Location, new Size(16, 0)));

                    _pointPrevToolTip = e.Location;
                }
            }

        }

        private void pictView_MouseDoubleClick(object sender, MouseEventArgs e) {
            SequenceViewerController controller = _controller;
            if(controller == null)
                return;
            decimal cursorTime = offsetToTime(e.X);
            if((Control.ModifierKeys & Keys.Control) != 0) {
            } else {
                controller.CurrentTime = cursorTime;
            }
        }


        private void pictView_SizeChanged(object sender, EventArgs e) {
            this.DoRefreshView();
        }

        private void pictView_Resize(object sender, EventArgs e) {
            this.DoRefreshView();
        }

        private void pictView_MouseLeave(object sender, EventArgs e) {
            _toolTipValue.Hide(this);
            // コンテキストメニューが開いている場合にはビューからカーソルを消さない
            if(!_isContextMenuOpened) {
                leavePictView();
            }
        }
        private void pictView_MouseUp(object sender, MouseEventArgs e) {
            SequenceViewerController controller = _controller;
            if(controller == null)
                return;
            RegulatedMouseInfo info = _mouse.MouseUp(e);
            decimal time = offsetToTime(e.X);
            if(info.State == RegulatedMouseClickState.Click) {
                bool downInSelectedArea = selectedAreaContains(e.Location);
                switch(info.Button) {
                case RegulatedMouseButton.Left:
                    if(downInSelectedArea || (time < _controller.BeginTime || _controller.EndTime < time)) {
                        this.SelectTimeRange(time, time);
                    } else {
                        if((this.Type & SequenceType.Label) != 0) {
                            decimal rangeBegin = this.Sequence.GetLabelStartTimeAt(time) ?? TimeController.Singleton.BeginTime;
                            decimal rangeEnd = this.Sequence.GetLabelStartTimeAt(time, 1) ?? TimeController.Singleton.EndTime;
                            this.SelectTimeRange(rangeBegin, rangeEnd);
                        } else {
                            decimal value = offsetToValue(e.Y);
                            int index = this.Sequence.Borders.GetIndexFromValue(value);
                            if(this.Sequence.Borders.IsValidIndex(index) && this.Sequence.Borders.IsValidIndex(index + 1)) {
                                decimal rangeBegin = this.Sequence.Borders.GetBorderByIndex(index).Key;
                                decimal rangeEnd = this.Sequence.Borders.GetBorderByIndex(index + 1).Key;
                                this.DeselectTimeRange();
                                this.SelectValueRange(rangeBegin, rangeEnd);
                            }
                        }
                    }
                    break;
                case RegulatedMouseButton.CtrlLeft:
                    if(!downInSelectedArea) {
                        if((this.Type & SequenceType.Label) != 0) {
                            decimal rangeBegin = this.Sequence.GetLabelStartTimeAt(time) ?? TimeController.Singleton.BeginTime;
                            decimal rangeEnd = this.Sequence.GetLabelStartTimeAt(time, 1) ?? TimeController.Singleton.EndTime;
                            this.SelectTimeRange(Math.Min(_selectTimeBegin, rangeBegin), Math.Max(_selectTimeEnd, rangeEnd));
                        } else {
                            decimal value = offsetToValue(e.Y);
                            int index = this.Sequence.Borders.GetIndexFromValue(value);
                            if(this.Sequence.Borders.IsValidIndex(index) && this.Sequence.Borders.IsValidIndex(index + 1)) {
                                decimal rangeBegin = this.Sequence.Borders.GetBorderByIndex(index).Key;
                                decimal rangeEnd = this.Sequence.Borders.GetBorderByIndex(index + 1).Key;
                                this.DeselectTimeRange();
                                this.SelectValueRange(Math.Min(_selectValueMin, rangeBegin), Math.Max(_selectValueMax, rangeEnd));
                            }
                        }
                    }
                    break;
                }
            }
        }


        private void pictView_MouseDown(object sender, MouseEventArgs e) {
            SequenceViewerController controller = _controller;
            if(controller == null)
                return;
            panelView.Select();
            decimal cursorTime = offsetToTime(e.X);
            bool downInSelectedArea = selectedAreaContains(e.Location);
            //if(e.Button == MouseButtons.Right && !downInSelectedArea) {
            //    this.DeselectRange();
            //}
            RegulatedMouseButton button = _mouse.MouseDown(e);
            switch(button) {
            case RegulatedMouseButton.Left:
            case RegulatedMouseButton.CtrlLeft:
                break;
            case RegulatedMouseButton.AltLeft:
                break;
            case RegulatedMouseButton.ShiftLeft:
                controller.AdjustSelectedRange(cursorTime);
                break;
            }

            //if(button != RegulatedMouseButton.None && (Control.ModifierKeys & Keys.Shift) != 0) {
            //    controller.CurrentTime = cursorTime;
            //}
        }

        bool _isSelecting { get { return _isTimeRangeSelected || _isValueRangeSelected; } }
        decimal _selectTimeBegin, _selectTimeEnd, _selectValueMin, _selectValueMax;
        /// <summary>
        /// グラフの時間範囲を選択します
        /// </summary>
        /// <param name="begin">範囲の開始時間</param>
        /// <param name="end">範囲の終了時間</param>
        public void SelectTimeRange(decimal begin, decimal end) {
            SequenceViewerController controller = _controller;
            if(controller != null) {
                controller.SelectRange(begin, end);
            } else {
                selectTimeRangeInternal(begin, end);
            }
        }
        private void selectTimeRangeInternal(decimal begin, decimal end) {
            _isTimeRangeSelected = true;
            if(begin > end) {
                decimal tmp = begin;
                begin = end;
                end = tmp;
            }
            _selectTimeBegin = begin;
            _selectTimeEnd = end;
            displayImage();
        }
        public void DeselectTimeRange() {
            SequenceViewerController controller = _controller;
            if(controller != null) {
                controller.DeselectRange();
            } else {
                deselectTimeRangeInternal();
            }
        }
        private void deselectTimeRangeInternal() {
            _isTimeRangeSelected = false;
            displayImage();
        }
        /// <summary>
        /// グラフの値に関して範囲選択を行います
        /// </summary>
        /// <param name="min">範囲の開始値</param>
        /// <param name="max">範囲の終了値</param>
        public void SelectValueRange(decimal min, decimal max) {
            _isValueRangeSelected = true;
            if(min > max) {
                decimal tmp = min;
                min = max;
                max = tmp;
            }
            _selectValueMin = min;
            _selectValueMax = max;
            displayImage();
        }
        /// <summary>
        /// 値方向の範囲選択を解除します
        /// </summary>
        public void DeselectValueRange() {
            _isValueRangeSelected = false;
            displayImage();
        }
        bool _isTimeRangeSelected = false;
        /// <summary>
        /// 時間範囲が選択されているかを取得します
        /// </summary>
        public bool IsTimeRangeSelected {
            get { return _isTimeRangeSelected; }
        }
        bool _isValueRangeSelected = false;
        /// <summary>
        /// 値方向の範囲選択がなされているかを取得します
        /// </summary>
        public bool IsValueRangeSelected {
            get { return _isValueRangeSelected; }
        }


        struct displayLabel {
            public int Begin, Width;
            public int End { get { return this.Begin + this.Width; } }
            public string Name;
            public displayLabel(int begin, int width, string name) {
                this.Begin = begin;
                this.Width = width;
                this.Name = name;
            }
        }

        volatile bool _imageAllocateRequested = false;

        private void bgRenderLabel(Graphics gfx, Size viewSize, Font drawFont, DoWorkEventArgs e) {
            // ラベルとして描画
            if(this.Sequence.Borders.TargetColumnIndex < this.Sequence.Values.ColumnCount) {
                List<displayLabel> displayLabels = new List<displayLabel>();
                // ひとつ前のx位置のラベル名
                string prevLabel = null;
                // 今のラベルの開始x位置
                const int margin = 2;
                int beginOffset = -margin;
                // 前回のラベルインデックス．同じラベル名で違う境界の場合用
                int prevBorderIndex = -1;
                // 1ピクセルごとに境界を確認
                for(int x = -margin; x < viewSize.Width + margin; x++) {
                    // 評価対象の値を取得
                    decimal time = offsetToTime(x);
                    decimal?[] values = this.Sequence.Values.GetValueAt(time);
                    decimal? value = null;
                    if(values != null) {
                        value = values[this.Sequence.Borders.TargetColumnIndex];
                    }
                    // 評価
                    int borderIndex = Sequence.Borders.GetIndexFromValue(value);
                    string newLabel = Sequence.Borders.GetLabelByValue(value);
                    if(prevLabel != newLabel || prevBorderIndex != borderIndex) {
                        // ラベルが変わったら記録
                        if(prevLabel != null) {
                            displayLabels.Add(new displayLabel(beginOffset, x - beginOffset, prevLabel));
                        }
                        beginOffset = x;
                        prevLabel = newLabel;
                        prevBorderIndex = borderIndex;
                    }
                    // 次のラベルまで間があるときは飛ばす
                    int index = this.Sequence.Values.GetIndexAt(time);
                    decimal nextTime;
                    if(this.Sequence.Values.TryGetTimeFromIndex(index + 1, out nextTime)) {
                        int nextX = timeToOffset(nextTime);
                        if(x + 1 < nextX) {
                            x = nextX - 1;
                        }
                    } else {
                        if(index >= this.Sequence.Values.SequenceLength) {
                            break;
                        }
                    }
                }
                // 最後のラベルを記録
                if(prevLabel != null) {
                    displayLabels.Add(new displayLabel(beginOffset, viewSize.Width + margin - beginOffset, prevLabel));
                }
                IList<string> borders = this.Sequence.Borders.GetLabelNames(true);
                if(borders.Count != 0) {
                    foreach(var label in displayLabels) {
                        int lineY = viewSize.Height - 2;
                        // ラベル名のY位置
                        float labelPosY = lineY - this.Font.Height - 1;
                        if(labelPosY < 1)
                            labelPosY = 1;
                        // 背景
                        Color fillColor = this.Sequence.Borders.GetLabelColor(label.Name);
                        if(fillColor == Color.Empty)
                            fillColor = Color.White;
                        Color lightColor = ColorEx.AddColor(fillColor, Color.FromArgb(32, 32, 32));
                        Color shadowColor = ColorEx.SubtractColor(fillColor, Color.FromArgb(32, 32, 32));
                        Color halfLightColor = ColorEx.AddColor(fillColor, Color.FromArgb(16, 16, 16));
                        Color halfShadowColor = ColorEx.SubtractColor(fillColor, Color.FromArgb(16, 16, 16));
                        if(false) {
                            gfx.FillRectangle(new SolidBrush(fillColor), new RectangleF(label.Begin, 0, label.Width, viewSize.Height));
                        } else {
                            int subHeight = viewSize.Height / 4;
                            int restHeight = viewSize.Height - subHeight;
                            gfx.FillRectangle(new LinearGradientBrush(new Point(0, 0), new Point(0, subHeight), lightColor, fillColor), new RectangleF(label.Begin, 0, label.Width, subHeight));
                            gfx.FillRectangle(new LinearGradientBrush(new Point(0, 0), new Point(0, viewSize.Height), shadowColor, fillColor), new RectangleF(label.Begin, subHeight, label.Width, restHeight));
                        }
                        if(label.Width >= 4) {
                            // 下線を引く
                            gfx.DrawLine(new Pen(ColorEx.GetComplementaryColor(fillColor)), label.Begin, lineY, label.End - 1, lineY);
                            if(label.Width >= 6) {
                                // 矢印
                                gfx.DrawLine(new Pen(ColorEx.GetComplementaryColor(fillColor)), label.End - 6, lineY - 3, label.End - 3, lineY);
                            }
                        }
                        // ラベル名                        
                        //for(int dx = -1; dx <= 1; dx++) {
                        //    for(int dy = -1; dy <= 1; dy++) {
                        //        gfx.DrawString(label.Name, this.Font, new SolidBrush(Color.White), new RectangleF(label.Begin+dx, labelPosY+dy, label.Width, this.Font.Height));
                        //    }
                        //}
                        //gfx.DrawString(label.Name, this.Font, new SolidBrush(Color.Black), new RectangleF(label.Begin, labelPosY, label.Width, this.Font.Height));
                        gfx.DrawString(label.Name, this.Font, new SolidBrush(ColorEx.GetComplementaryColor(fillColor)), new RectangleF(label.Begin, labelPosY, label.Width, this.Font.Height));
                        // 縁を装飾
                        if(label.Width >= 3) {
                            float h, s, v;
                            ColorEx.ColorToHSV(fillColor, out h, out s, out v);
                            gfx.DrawLine(new Pen(lightColor), new Point(label.Begin, 0), new Point(label.Begin, pictView.Height));
                            gfx.DrawLine(new Pen(shadowColor), new Point(label.End - 1, 0), new Point(label.End - 1, pictView.Height));
                            if(label.Width >= 5) {
                                Color borderColor2 = ColorEx.ColorFromHSV(h, s, Math.Min(1.0f, v + 0.05f));
                                Color borderColor3 = ColorEx.ColorFromHSV(h, s, Math.Max(0.0f, v - 0.05f));
                                gfx.DrawLine(new Pen(halfLightColor), new Point(label.Begin + 1, 0), new Point(label.Begin + 1, pictView.Height));
                                gfx.DrawLine(new Pen(halfShadowColor), new Point(label.End - 2, 0), new Point(label.End - 2, pictView.Height));
                            }
                        }
                    }
                }
            }
        }

        private void bgRenderBorder(Graphics gfx, Size viewSize, Font drawFont, DoWorkEventArgs e) {
            // 評価用の境界を描画
            Color borderColorPrev = Color.Empty;
            foreach(var pair in Sequence.Borders.Enumerate()) {
                if(bgAllocateImage.CancellationPending) {
                    e.Cancel = true;
                    return;
                }
                try {
                    // 低い値から描画していく
                    Color borderColor = this.Sequence.Borders.GetLabelColor(pair.Value);
                    if(borderColor == Color.Empty)
                        borderColor = Color.White;
                    Color fillColor = borderColor;
                    Color shadowColor = ColorEx.SubtractColor(borderColor, Color.FromArgb(32, 32, 32));
                    int borderHeight = valueToOffset(pair.Key);
                    if(borderHeight < 0)
                        continue;
                    if(borderHeight > viewSize.Height)
                        borderHeight = viewSize.Height;
                    if(this.Type == SequenceType.Numeric) {
                        gfx.FillRectangle(new SolidBrush(fillColor), new Rectangle(new Point(0, 0), new Size(viewSize.Width, borderHeight)));
                    }
                    gfx.DrawLine(new Pen(shadowColor), new Point(0, borderHeight), new Point(viewSize.Width, borderHeight));
                    gfx.DrawLine(new Pen(borderColorPrev), new Point(0, borderHeight + 1), new Point(viewSize.Width, borderHeight + 1));
                    borderColorPrev = ColorEx.AddColor(borderColor, Color.FromArgb(32, 32, 32));
                } catch(OverflowException) { }
            }
        }

        private IList<decimal> generateTicks(decimal minValue, decimal maxValue, decimal div) {
            decimal diff = (maxValue - minValue) / div;
            decimal unitValue = 1;
            while(unitValue < diff) {
                decimal ten = unitValue * 10;
                if(ten < unitValue)
                    return new decimal[0];
                unitValue = ten;
            }
            while(unitValue > diff) {
                if(unitValue / 10 * 5 <= diff) {
                    unitValue = unitValue / 10 * 5;
                    break;
                }
                if(unitValue / 10 * 2.5M <= diff) {
                    unitValue = unitValue / 10 * 2.5M;
                    break;
                }
                unitValue /= 10;
            }
            if(unitValue == 0)
                return new decimal[0];
            decimal tmpUnit = unitValue;
            while(tmpUnit < diff) {
                decimal ten = tmpUnit * 10;
                if(ten < tmpUnit)
                    return new decimal[0];
                tmpUnit = ten;
            }
            decimal minMultipleUnit = 0;
            while(tmpUnit >= unitValue) {
                if(minMultipleUnit + tmpUnit < Math.Abs(minValue)) {
                    minMultipleUnit += tmpUnit;
                } else {
                    tmpUnit /= 10;
                }
            }
            if(minValue < 0)
                minMultipleUnit = -minMultipleUnit;
            List<decimal> ret = new List<decimal>();
            for(decimal multipleUnit = minMultipleUnit; multipleUnit < maxValue; multipleUnit += unitValue) {
                if(multipleUnit > minValue)
                    ret.Add(multipleUnit);
            }
            return ret;
        }

        private void bgRenderTick(Graphics gfx, Size viewSize, Font drawFont, Pen[] pens, DoWorkEventArgs e) {
            Color tickColor = Color.FromArgb(192, Color.Black);
            Color thinTickColor = Color.FromArgb(64, Color.Black);
            Color bgColor = Color.FromArgb(128, Color.White);
            Brush tickBrush = new SolidBrush(tickColor);
            Pen tickPen = new Pen(tickColor);
            Pen thinTickPen = new Pen(thinTickColor);
            Brush bgBrush = new SolidBrush(bgColor);
            //gfx.DrawString(this.ValueEnd.ToString("0.000"), drawFont, tickBrush, new PointF(0, 0));
            //gfx.DrawString(this.ValueBegin.ToString("0.000"), drawFont, tickBrush, new PointF(0, viewSize.Height - drawFont.Height));
            decimal div = Math.Min(Math.Max(viewSize.Height / drawFont.Height / 2, 1), 10);
            IList<decimal> ticks = generateTicks(this.ValueBegin, this.ValueEnd, div);
            int decimalPlaces;
            for(decimalPlaces = 0; decimalPlaces < 5; decimalPlaces++) {
                decimal coef = 1M;
                for(int j = 0; j < decimalPlaces; j++)
                    coef *= 10M;
                if(ticks.All(t => decimal.Round(t * coef) == t * coef))
                    break;
            }
            string tickFormat = "0";
            if(decimalPlaces > 0) {
                tickFormat = "0.";
                for(int j = 0; j < decimalPlaces; j++)
                    tickFormat += "0";
            }
            int maxWidth = 0;
            foreach(decimal tick in ticks) {
                SizeF size = gfx.MeasureString(tick.ToString(tickFormat), drawFont);
                maxWidth = Math.Max(maxWidth, (int)Math.Ceiling(size.Width));
            }
            foreach(decimal tick in ticks) {
                SizeF size = gfx.MeasureString(tick.ToString(tickFormat), drawFont);
                int width = (int)Math.Ceiling(size.Width);
                int y = valueToOffset(tick);
                gfx.DrawLine(tickPen, new PointF(0, y), new PointF(10, y));
                gfx.DrawLine(thinTickPen, new PointF(10, y), new PointF(viewSize.Width, y));
                PointF pos = new PointF(10, y - (float)drawFont.Height / 2);
                gfx.FillRectangle(bgBrush, new RectangleF(pos, new SizeF(maxWidth, size.Height)));
                gfx.DrawString(tick.ToString(tickFormat), drawFont, tickBrush, PointF.Add(pos, new Size(maxWidth - width, 0)));
            }
        }

        private void bgRenderValues(Graphics gfx, Size viewSize, Font drawFont, Pen[] pens, DoWorkEventArgs e) {
            decimal leftTime = offsetToTime(0);
            decimal rightTime = offsetToTime(viewSize.Width);
            int beginIndex = Sequence.Values.GetIndexAt(leftTime);
            int endIndex = Sequence.Values.GetIndexAt(rightTime);

            // 描画処理用変数
            decimal?[] prevValue = new decimal?[Sequence.Values.ColumnCount];
            int prevOffsetX = -1;
            Rectangle bitmapRectangle = new Rectangle(Point.Empty, _bitmapView.Size);
            // 二か所で使うので描画処理内容を保存
            Action<int, decimal?[]> render = (offsetX, values) => {
                try {
                    decimal stack = 0;
                    decimal prevValue_prevColumn = 0;
                    bool prevAll = prevValue.All(x => x.HasValue);
                    bool valueAll = values.All(x => x.HasValue);
                    for(int j = 0; j < prevValue.Length; j++) {
                        if(!this.StackGraphMode || valueAll) {
                            if(this.StackGraphMode) {
                                // 積み重ねの計算
                                values[j] = values[j].Value + stack;
                            }
                            if(prevValue[j].HasValue) {
                                // 前回の値があれば水平線を引く
                                int prevY = valueToOffset(prevValue[j].Value);
                                if(prevOffsetX != offsetX) {
                                    gfx.DrawLine(pens[j], new Point(prevOffsetX, prevY), new Point(offsetX, prevY));
                                } else {
                                    if(bitmapRectangle.Contains(new Point(offsetX, prevY)))
                                        _bitmapView.SetPixel(offsetX, prevY, pens[j].Color);
                                }
                                if(values[j].HasValue) {
                                    // 前回と今回の両方値があれば垂直線を引く
                                    int curY = valueToOffset(values[j].Value);
                                    if(prevY != curY) {
                                        gfx.DrawLine(pens[j], new Point(offsetX, prevY), new Point(offsetX, curY));
                                    } else {
                                        if(new Rectangle(Point.Empty, _bitmapView.Size).Contains(new Point(offsetX, curY)))
                                            _bitmapView.SetPixel(offsetX, curY, pens[j].Color);
                                    }
                                }
                            } else if(values[j].HasValue) {
                                // 今回だけ値があれば点を打つ
                                int curY = valueToOffset(values[j].Value);
                                if(bitmapRectangle.Contains(new Point(offsetX, curY)))
                                    _bitmapView.SetPixel(offsetX, curY, pens[j].Color);
                            }

                            if(this.StackGraphMode) {
                                // 積み重ねの塗りつぶし
                                if(prevAll) {
                                    int prevY = valueToOffset(prevValue[j].Value);
                                    int prevY_prevColumn = valueToOffset(prevValue_prevColumn);
                                    int beginY = Math.Min(prevY, prevY_prevColumn);
                                    int endY = Math.Max(prevY_prevColumn, prevY);
                                    if(prevOffsetX != offsetX && beginY != endY) {
                                        gfx.FillRectangle(new SolidBrush(Color.FromArgb(128, pens[j].Color)), new Rectangle(new Point(prevOffsetX, beginY), new Size(offsetX - prevOffsetX, endY - beginY)));
                                    }
                                }
                                stack = values[j].Value;
                            }
                        }
                        if(prevValue[j].HasValue) {
                            // 次の塗りつぶしの下限
                            prevValue_prevColumn = prevValue[j].Value;
                        }
                        prevValue[j] = values[j];
                    }

                    prevOffsetX = offsetX;
                } catch(OverflowException) { }
            };

            if(_fastModeThreshold > 0 && endIndex - beginIndex > _fastModeThreshold * viewSize.Width) {
                // 高速モード
                for(int offsetX = 0; offsetX < viewSize.Width; offsetX++) {
                    if(bgAllocateImage.CancellationPending) {
                        e.Cancel = true;
                        return;
                    }
                    int index = this.Sequence.Values.GetIndexAt(offsetToTime(offsetX + 0.9999f));
                    decimal?[] values;
                    if(index != -1 && this.Sequence.Values.TryGetValueFromIndex(index, out values)) {
                        render(offsetX, values);
                    }
                    if(index >= this.Sequence.Values.SequenceLength) {
                        break;
                    }
                }
            } else {
                // 実表示モード
                for(int index = beginIndex - 1; index <= endIndex + 1; index++) {
                    if(bgAllocateImage.CancellationPending) {
                        e.Cancel = true;
                        return;
                    }
                    decimal time;
                    decimal?[] values;
                    if(this.Sequence.Values.TryGetTimeFromIndex(index, out time) && this.Sequence.Values.TryGetValueFromIndex(index, out values)) {
                        try {
                            int offsetX = timeToOffset(time);
                            render(offsetX, values);
                        } catch(OverflowException) { }
                    } else {
                        for(int i = 0; i < prevValue.Length; i++)
                            prevValue[i] = null;
                    }
                }
            }
        }

        private void bgRenderLegend(Graphics gfx, Size viewSize, Font drawFont, Pen[] pens, DoWorkEventArgs e) {
            Brush brushBgLegend = new SolidBrush(Color.FromArgb(224, 248, 248, 248));
            if(this.Sequence.Values.ColumnCount > 1) {
                SizeF[] sizes = new SizeF[this.Sequence.Values.ColumnCount];
                for(int i = 0; i < this.Sequence.Values.ColumnCount; i++) {
                    sizes[i] = gfx.MeasureString(this.Sequence.Values.ColumnNames[i], drawFont);
                }
                float widthMax = sizes.Max(x => x.Width);
                int count = 0;
                for(int index = 0; index < this.Sequence.Values.ColumnCount; index++) {
                    Brush fillBrush = new SolidBrush(pens[index].Color);
                    float x = viewSize.Width - widthMax - 16;
                    gfx.FillRectangle(fillBrush, new RectangleF(x, count * 16 + 4, 8, 8));
                    PointF pointStr = new PointF(x + 10, count * 16 + 2);
                    gfx.FillRectangle(brushBgLegend, new RectangleF(pointStr, sizes[index]));
                    gfx.DrawString(Sequence.Values.ColumnNames[index], drawFont, fillBrush, pointStr);
                    count++;
                }
            }
        }

        /// <summary>
        /// グラフまたはラベルを描画します．
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgAllocateImage_DoWork(object sender, DoWorkEventArgs e) {
            // サイズチェック
            int width = pictView.Width;
            int height = pictView.Height;
            if(width <= 0 || height <= 0)
                return;
            Size viewSize = new Size(width, height);
            // 背景画像確保
            lock(_lockAllocateImage) {
                _bitmapView = new Bitmap(width, height);
                using(Graphics gfx = Graphics.FromImage(_bitmapView)) {
                    // 背景初期化
                    var bgColor = this.Sequence.Borders.GetLabelColor(this.Sequence.Borders.DefaultName);
                    if(bgColor == Color.Empty)
                        bgColor = Color.White;
                    gfx.Clear(bgColor);

                    if(bgAllocateImage.CancellationPending) {
                        e.Cancel = true;
                        return;
                    }
                    // 普通のフォント
                    Font font = new Font(this.Font, FontStyle.Regular);
                    // 境界の描画 
                    if(this.Type != SequenceType.Numeric) {
                        bgRenderLabel(gfx, viewSize, font, e);
                        if(e.Cancel)
                            return;
                    }
                    if(this.Type != SequenceType.Label) {
                        // 値方向のラベル化境界を描画
                        bgRenderBorder(gfx, viewSize, font, e);
                        if(e.Cancel)
                            return;
                        // 時系列データ描画用のペン
                        Pen[] pens = new Pen[this.Sequence.Values.ColumnCount];
                        for(int i = 0; i < pens.Length; i++) {
                            Color pensColor = ColorEx.ColorFromHSV((float)(Math.PI * 2 * i / pens.Length), 0.8f, 1f);
                            pens[i] = new Pen(pensColor);
                        }
                        // 時系列データ描画
                        bgRenderValues(gfx, viewSize, font, pens, e);
                        if(e.Cancel)
                            return;
                        bgRenderTick(gfx, viewSize, font, pens, e);
                        if(e.Cancel)
                            return;
                        // 凡例
                        bgRenderLegend(gfx, viewSize, font, pens, e);
                    }
                }
            }
            if(bgAllocateImage.CancellationPending) {
                e.Cancel = true;
                return;
            }
            displayImage();
            if(bgAllocateImage.CancellationPending) {
                e.Cancel = true;
                return;
            }
        }

        private void menuResetScale_Click(object sender, EventArgs e) {
            ResetTimeAndValue(false);
        }



        private void menuCommonTimeRange_Click(object sender, EventArgs e) {
            if(_controller != null) {
                _controller.BroadcastVisibleRange(this);
            }
        }

        public void onDragEnter(object sender, DragEventArgs e) {
            if(e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.Copy;
            }
        }

        public void onDragDrop(object sender, DragEventArgs e) {
            if(_controller == null)
                return;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach(var file in files) {
                _controller.OpenFileByExtension(file);
            }
        }

        private void bgAllocateImage_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if(_imageAllocateRequested) {
                requestAllocateImage();
                _imageAllocateRequested = false;
            } else {
                pictView.Location = Point.Empty;
            }
        }


        private void zoomToolStripMenuItem_Click(object sender, EventArgs e) {
            this.ZoomSelected();
        }
        /// <summary>
        /// 現在の選択範囲が表示範囲になるように拡大します．
        /// </summary>
        public void ZoomSelected() {
            if(_isTimeRangeSelected) {
                SetVisibleTimeRange(_selectTimeBegin, _selectTimeEnd - _selectTimeBegin);
            }
            if(_isValueRangeSelected) {
                SetVisibleValueRange(_selectValueMin, _selectValueMax - _selectValueMin);
            }
            this.DeselectRange();
        }

        private void unzoomToolStripMenuItem_Click(object sender, EventArgs e) {
            this.UnzoomSelected();
        }
        /// <summary>
        /// 現在の表示範囲が選択範囲と同じになるように表示範囲を縮小します．
        /// </summary>
        public void UnzoomSelected() {
            RectangleF _selectedArea = getSelectedRectangle();
            RectangleF unzoom = this.GetUnzoomRectangle(_selectedArea);
            if(_isTimeRangeSelected) {
                decimal left = offsetToTime(unzoom.Left);
                decimal right = offsetToTime(unzoom.Right);
                this.SetVisibleTimeRange(left, right - left);
            }
            if(_isValueRangeSelected) {
                decimal top = offsetToValue(unzoom.Top);
                decimal bottom = offsetToValue(unzoom.Bottom);
                this.SetVisibleValueRange(bottom, top - bottom);
            }
            this.DeselectRange();
        }

        private void menuClip_Click(object sender, EventArgs e) {
            if(MessageBox.Show("指定された範囲にデータを切り取ります。", this.GetType().Name, MessageBoxButtons.OKCancel) == DialogResult.OK) {
                if(this.Type == SequenceType.Label) {
                    SequenceViewerController controller = _controller;
                    if(controller != null) {
                        this.Sequence.SetLabelAt(controller.BeginTime, _selectTimeBegin, "");
                        this.Sequence.SetLabelAt(_selectTimeEnd, controller.EndTime, "");
                        this.DoRefreshView();
                    }
                } else {
                    if(_isTimeRangeSelected) {
                        this.Sequence.Values.ClipTime(_selectTimeBegin, _selectTimeEnd);
                        this.Sequence.IsDataChanged = true;
                    }
                    if(_isValueRangeSelected) {
                        this.Sequence.Values.ClipValue(_selectValueMin, _selectValueMax);
                        this.Sequence.IsDataChanged = true;
                        calculateMinMax();
                    }
                    if(!(!_isTimeRangeSelected && !_isValueRangeSelected)) {
                        this.DoRefreshView();
                    }
                }
            }
        }
        /// <summary>
        /// 現在のビューのデータをラベル列で上書きします
        /// </summary>
        /// <param name="labelSequence">ラベル列</param>
        /// <param name="title">新しいタイトル</param>
        public void AttachLabelSequence(ICSLabelSequence labelSequence, string title) {
            AttachLabelSequence(labelSequence, title, null);
        }
        /// <summary>
        /// 現在のビューのデータをラベル列で上書きします
        /// </summary>
        /// <param name="labelSequence">ラベル列</param>
        /// <param name="title">新しいタイトル</param>
        /// <param name="colorPalette">ラベル名とラベル色の対応</param>
        public void AttachLabelSequence(ICSLabelSequence labelSequence, string title, IDictionary<string, Color> colorPalette) {
            this.Sequence = SequenceData.FromLabelSequence(labelSequence, title, colorPalette);
            this.Type = SequenceType.Label;
            this.SetDefaultHeight(true);
        }


        bool _isMenuLoadLabelUpdated = false;
        /// <summary>
        /// menuLoadLabelアイテムのドロップダウンアイテムを生成します
        /// </summary>
        /// <param title="sender"></param>
        /// <param title="e"></param>
        private void menuLoadLabel_MouseEnter(object sender, EventArgs e) {
        }

        private void saveValueDataToolStripMenuItem_Click(object sender, EventArgs e) {
            if(dialogSaveSequence.ShowDialog() == DialogResult.OK) {
                this.Sequence.Values.Serialize(dialogSaveSequence.FileName);
            }
        }

        private void comboBorder_KeyDown(object sender, KeyEventArgs e) {
            if(e.KeyData == Keys.Enter) {
                setLabelFromCombobox();
                contextMenuArea.Close();
            }
        }

        private void comboBorder_SelectedIndexChanged(object sender, EventArgs e) {
            if(comboBorder.SelectedIndex != -1 && _comboBorderEntered) {
                comboBorder.DroppedDown = false;
            }
        }

        private void comboBorder_DropDownClosed(object sender, EventArgs e) {
            if(comboBorder.SelectedIndex != -1) {
                setLabelFromCombobox();
                contextMenuArea.Close();
            }
        }


        private void setLabelFromCombobox() {
            if(!comboBorder.Enabled)
                return;
            if(comboBorder.SelectedIndex != -1) {
                SetLabelForSelected(comboBorder.SelectedItem.ToString());
            } else if(comboBorder.Text != global::MotionDataHandler.Properties.Settings.Default.Menu_SetLabelHere) {
                SetLabelForSelected(comboBorder.Text);
            }

        }
        /// <summary>
        /// 選択されている範囲にラベルを設定します．
        /// </summary>
        /// <param name="label"></param>
        public void SetLabelForSelected(string label) {
            if(label == null)
                label = "";
            if(_isSelecting && !this.IsLocked) {
                if(this.Type == SequenceType.Label) {
                    this.Sequence.SetLabelAt(_selectTimeBegin, _selectTimeEnd, label);
                    this.DoRefreshView();
                } else if(_isValueRangeSelected) {
                    // 選択縦範囲のボーダーを変える
                    this.Sequence.Borders.SetBorderRange(_selectValueMin, _selectValueMax, label);
                    this.Sequence.IsDataChanged = true;
                    this.DoRefreshView();
                }
            }
        }


        bool _comboBorderEntered = false;

        private void comboBorder_MouseDown(object sender, MouseEventArgs e) {
            if(!_comboBorderEntered) {
                comboBorder.ForeColor = SystemColors.ControlText;
                // 入力欄の初期値の設定
                if(comboBorder.Text == MotionDataHandler.Properties.Settings.Default.Menu_SetLabelHere) {
                    try {
                        decimal keyBegin, keyEnd;
                        // シーケンスタイプに応じて選択ラベル範囲は異なる
                        if(this.Type == SequenceType.Label && _isTimeRangeSelected) {
                            decimal? keyBeginTmp = this.Sequence.GetTargetColumnValueAt(_selectTimeBegin);
                            decimal? keyEndTmp = this.Sequence.GetTargetColumnValueAt(_selectTimeEnd);
                            if(!keyEndTmp.HasValue || !keyBeginTmp.HasValue) {
                                throw new ApplicationException();
                            }
                            keyBegin = keyBeginTmp.Value;
                            keyEnd = keyEndTmp.Value;
                        } else if(this.Type != SequenceType.Label && _isValueRangeSelected) {
                            keyBegin = _selectValueMin;
                            keyEnd = _selectValueMax;
                        } else {
                            comboBorder.Text = "";
                            return;
                        }

                        int beginIndex = this.Sequence.Borders.GetIndexFromValue(keyBegin);
                        int endIndex = this.Sequence.Borders.GetIndexFromValue(keyEnd);
                        if(beginIndex < 0 || endIndex >= this.Sequence.Borders.BorderCount) {
                            throw new ApplicationException();
                        }
                        bool isUnique = true;

                        // 選択領域が全部同じラベルなら入力欄の初期値をそれにする．
                        string labelText = this.Sequence.Borders[beginIndex] ?? "";
                        for(int i = beginIndex; i <= endIndex; i++) {
                            var value = this.Sequence.Borders.GetBorderByIndex(i);
                            // ラベル名が異なる部分があれば空に
                            if(value.Value != labelText) {
                                isUnique = false;
                                break;
                            }
                        }
                        if(!isUnique) {
                            throw new ApplicationException();
                        }
                        comboBorder.Text = labelText;
                        comboBorder.Select(0, comboBorder.Text.Length);
                    } catch(ApplicationException) {
                        comboBorder.Select(0, comboBorder.Text.Length);
                    }
                }
                _comboBorderEntered = true;
            }
        }



        private void removeBorderHereToolStripMenuItem_Click(object sender, EventArgs e) {
            if(!this.IsLocked) {
                decimal here;
                if(this.Type != SequenceType.Numeric) {
                    here = getCursorTime();
                } else {
                    here = getCursorValue();
                }
                int index = Sequence.Borders.GetIndexFromValue(here);
                if(Sequence.Borders.IsValidIndex(index)) {
                    Sequence.Borders.RemoveAt(index);
                }
                Sequence.IsDataChanged = true;
                DoRefreshView();
            }
        }

        private void borderPropertyToolStripMenuItem_Click(object sender, EventArgs e) {
            DialogSequenceBorder dialog = new DialogSequenceBorder(Sequence, this);
            dialog.SetNumLowerEnd(getCursorValue());
            dialog.AttachIPluginHost(_pluginHost);
            dialog.ShowDialog();
            this.DoRefreshView();
        }


        private void outputLabelToolStripMenuItem_Click(object sender, EventArgs e) {
            OutputLabel();
        }
        /// <summary>
        /// ラベル列をエクスポートします．iCorpusStudioが開いている場合にはそちらに出力することも可能です．
        /// </summary>
        public void OutputLabel() {
            using(this.Sequence.Lock.GetReadLock()) {
                ICSLabelSequence labelSequence = Sequence.GetLabelSequence();
                if(labelSequence == null) {
                    MessageBox.Show("ラベルが作成できませんでした");
                    return;
                }
                bool toICorpusStudio = false;
                bool toFile = false;
                if(_pluginHost != null) {
                    DialogResult res = MessageBox.Show("iCorpusStudioが開いています．iCorpusStudioに出力しますか?", typeof(SequenceView).Name, MessageBoxButtons.YesNoCancel);
                    switch(res) {
                    case DialogResult.Yes:
                        toICorpusStudio = true;
                        break;
                    case DialogResult.No:
                        toFile = true;
                        break;
                    case DialogResult.Cancel:
                        break;
                    }
                } else {
                    toFile = true;
                }
                if(toICorpusStudio) {
                    Plugin.DataRow datarow = labelSequence.CreateDataRow(_pluginHost, Sequence.Borders.GetColorPalette());
                    datarow.Title = Sequence.Title;
                    _pluginHost.DataRows.Add(datarow);
                } else if(toFile) {
                    dialogSaveLabel.FileName = Sequence.Title + "." + dialogSaveLabel.DefaultExt;
                    if(dialogSaveLabel.ShowDialog() == DialogResult.OK) {
                        using(StreamWriter writer = new StreamWriter(dialogSaveLabel.OpenFile())) {
                            labelSequence.WriteTo(writer);
                        }
                    }
                }
            }
        }

        private void textTitle_TextChanged(object sender, EventArgs e) {
            lock(Sequence) {
                if(Sequence.Title == textTitle.Text)
                    return;
                if(Path.GetInvalidPathChars().Any(c => textTitle.Text.Contains(c))) {
                    ToolTip tip = new ToolTip();
                    tip.IsBalloon = true;
                    tip.Show("タイトルに以下の文字を使用できません\n" + CollectionEx.Join(" ", Path.GetInvalidPathChars()), textTitle, textTitle.Location);
                    textTitle.Text = Sequence.Title;
                }
            }
        }


        private void panelReorder_MouseClick(object sender, MouseEventArgs e) {
            ContextMenuStrip menu = refreshMenuOrder();
            menu.Show(panelReorderIcon, e.Location);
        }


        private ContextMenuStrip refreshMenuOrder() {
            if(_controller == null)
                return null;
            contextMenuOrder.Items.Clear();
            contextMenuOrder.Items.AddRange(_controller.GetReorderMenus(this).ToArray());
            return contextMenuOrder;
        }

        private void textUp_MouseClick(object sender, MouseEventArgs e) {
            if(_controller != null) {
                _controller.ReorderView(this, -1);
            }
        }

        private void hideTitleToolStripMenuItem_Click(object sender, EventArgs e) {
            IsHidden = !IsHidden;
        }


        private void panelToLarge_MouseClick(object sender, MouseEventArgs e) {
            setTextResize(this.RequestedHeight);
            contextMenuSize.Show(panelToLargeIcon, e.Location);
        }

        private void panelToSmall_MouseWheel(object sender, MouseEventArgs e) {
            RequestedHeight -= e.Delta / 30;
        }

        private void panelHide_MouseClick(object sender, MouseEventArgs e) {
            IsHidden = !IsHidden;
        }

        #region Serialize

        public void Serialize(Stream stream) {
            Serialize(stream, false);
        }

        public void Serialize(Stream stream, bool apartSequence) {
            XmlWriter writer = XmlWriter.Create(stream);
            Serialize(writer, apartSequence);
        }
        public void Serialize(string fileName) {
            Serialize(fileName, false);
        }
        public void Serialize(string fileName, bool apartSequence) {
            string path = fileName + "~";
            using(FileStream stream = new FileStream(path, FileMode.Create)) {
                Serialize(stream, apartSequence);
            }
            if(File.Exists(fileName)) {
                File.Replace(path, fileName, path + "~");
                File.Delete(path + "~");
            } else {
                File.Move(path, fileName);
            }
        }

        public void Serialize(XmlWriter writer) {
            Serialize(writer, false);
        }
        public void Serialize(XmlWriter writer, bool apartSequence) {
            writer.WriteStartElement(typeof(SequenceView).Name);
            WriteXml(writer, apartSequence);
            writer.WriteEndElement();
            writer.Flush();
        }

        public static SequenceView Deserialize(Stream stream) {
            XmlReader reader = XmlReader.Create(stream);
            return Deserialize(reader);
        }

        public static SequenceView Deserialize(string fileName) {
            using(FileStream stream = new FileStream(fileName, FileMode.Open)) {
                return Deserialize(stream);
            }
        }
        public static SequenceView Deserialize(XmlReader reader) {
            SequenceView ret = new SequenceView();
            ret.ReadXml(reader);
            return ret;
        }
        public System.Xml.Schema.XmlSchema GetSchema() {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// SequenceTypeをSequenceViewerからSequenceDataに移した時の互換性用
        /// </summary>
        bool _apartSequence = false;

        public void ReadXml(XmlReader reader) {
            reader.MoveToContent();
            if(reader.IsEmptyElement) {
                reader.Skip();
                return;
            }
            if(reader.Name != typeof(SequenceView).Name && reader.Name != "TimeValueSequenceViewer" && reader.Name != "SequenceViewer" && reader.Name != "SequenceView")
                throw new XmlException("Node not for " + typeof(SequenceView).Name);
            reader.ReadStartElement();
            bool? isLabelViewMode = null;
            bool? isFreeLabelingMode = null;
            while(reader.NodeType != XmlNodeType.None && reader.NodeType != XmlNodeType.EndElement) {
                reader.MoveToContent();
                switch(reader.Name) {
                case "DataPair":
                case "Sequence":
                    reader.ReadStartElement();
                    Sequence = SequenceData.Deserialize(reader.ReadSubtree());
                    reader.Skip();
                    reader.ReadEndElement();
                    isLabelViewMode = this.Type != SequenceType.Numeric;
                    isFreeLabelingMode = this.Type == SequenceType.Label;
                    _apartSequence = false;
                    break;
                case "ApartSequence":
                case "ApartDataPair":
                case "ClearDataPair":
                    if(reader.ReadElementContentAsBoolean()) {
                        SequenceType typeBefore = this.Type;
                        Sequence = new SequenceData();
                        this.Type = typeBefore;
                    }
                    _apartSequence = true;
                    break;
                case "RequestHeight":
                case "RequestedHeight":
                    this.RequestedHeight = reader.ReadElementContentAsInt();
                    break;
                case "RequestHide":
                case "IsHidden":
                    this.IsHidden = reader.ReadElementContentAsBoolean();
                    break;
                case "IsLocked":
                    this.IsLocked = reader.ReadElementContentAsBoolean();
                    break;
                case "LabelViewMode":
                    isLabelViewMode = reader.ReadElementContentAsBoolean();
                    break;
                case "FreeLabelingMode":
                    isFreeLabelingMode = reader.ReadElementContentAsBoolean();
                    break;
                case "StackGraphMode":
                    this.StackGraphMode = reader.ReadElementContentAsBoolean();
                    break;
                case "Type":
                    string sequenceTypeStr = reader.ReadElementContentAsString();
                    this.Type = (SequenceType)Enum.Parse(typeof(SequenceType), sequenceTypeStr);
                    break;
                case "TimeBegin":
                    SetVisibleTimeRange(reader.ReadElementContentAsDecimal());
                    break;
                case "TimeLength":
                    SetVisibleTimeRange(TimeBegin, reader.ReadElementContentAsDecimal());
                    break;
                case "ValueBegin":
                    SetVisibleValueRange(reader.ReadElementContentAsDecimal());
                    break;
                case "ValueHeight":
                    SetVisibleValueRange(ValueBegin, reader.ReadElementContentAsDecimal());
                    break;

                default:
                    reader.Skip();
                    break;
                }
            }
            if(isFreeLabelingMode ?? false) {
                this.Type = SequenceType.Label;
            } else if(isLabelViewMode ?? false) {
                this.Type = SequenceType.NumericLabel;
            } else if(isFreeLabelingMode.HasValue || isLabelViewMode.HasValue) {
                this.Type = SequenceType.Numeric;
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer) {
            WriteXml(writer, false);
        }

        public void WriteXml(XmlWriter writer, bool apartSequence) {
            writer.WriteStartElement("StackGraphMode");
            writer.WriteValue(StackGraphMode);
            writer.WriteEndElement();
            writer.WriteStartElement("RequestedHeight");
            writer.WriteValue(RequestedHeight);
            writer.WriteEndElement();
            writer.WriteStartElement("IsHidden");
            writer.WriteValue(IsHidden);
            writer.WriteEndElement();
            writer.WriteStartElement("IsLocked");
            writer.WriteValue(this.IsLocked);
            writer.WriteEndElement();
            if(!apartSequence) {
                writer.WriteStartElement("Sequence");
                this.Sequence.Serialize(writer);
                writer.WriteEndElement();
            } else {
                writer.WriteStartElement("ApartSequence");
                writer.WriteValue(true);
                writer.WriteEndElement();
            }
        }

        public readonly static string DefaultExtension = ".tvsv";

        public void SaveState(string path) {
            string xmlPath = string.Format("{0}{1}", path, DefaultExtension);
            if(IsDataModified || !File.Exists(xmlPath)) {
                this.Serialize(xmlPath, true);
            }
            this.Sequence.SaveState(path);
            this.IsDataModified = false;
        }

        public static SequenceView RetrieveState(string path) {
            try {
                if(!File.Exists(path + DefaultExtension)) {
                    string ext = Path.GetExtension(path);
                    if(ext == DefaultExtension) {
                        path = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
                        if(!File.Exists(path + DefaultExtension)) {
                            throw new FileNotFoundException();
                        }
                    } else {
                        throw new FileNotFoundException();
                    }
                }
                string xmlPath = string.Format("{0}{1}", path, DefaultExtension);
                SequenceView ret = SequenceView.Deserialize(xmlPath);
                if(ret._apartSequence) {
                    SequenceType typeInView = ret.Type;
                    ret.Sequence = SequenceData.RetrieveState(path);
                    if(typeInView != SequenceType.None) {
                        ret.Type = typeInView;
                        ret.Sequence.IsDataChanged = false;
                    }
                }
                ret.IsDataModified = false;

                return ret;
            } catch(Exception ex) {
                ErrorLogger.Tell(ex, "Cannot load " + Path.GetFileName(path));
                return null;
            }
        }

        #endregion

        private void exportToolStripMenuItem_Click(object sender, EventArgs e) {
            dialogSaveViewerState.FileName = Sequence.Title;
            if(dialogSaveViewerState.ShowDialog() == DialogResult.OK) {
                using(Stream stream = dialogSaveViewerState.OpenFile()) {
                    this.Serialize(stream);
                }
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e) {
            if(_controller == null)
                return;
            _controller.OpenSequenceWithDialog();
        }

        private void pictView_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            SequenceViewerController controller = _controller;
            if(controller == null)
                return;
            int addend = 1;
            if(e.Control)
                addend = 8;
            switch(e.KeyCode) {
            case Keys.Right:
                controller.CurrentIndex += addend;
                controller.CursorTime = controller.CurrentTime;
                if(e.Shift) {
                    controller.AdjustSelectedRange(controller.CurrentTime);
                } else {
                    controller.SelectRange(controller.CurrentTime, controller.CurrentTime);
                }
                e.IsInputKey = true;
                break;
            case Keys.Left:
                controller.CurrentIndex -= addend;
                controller.CursorTime = controller.CurrentTime;
                if(e.Shift) {
                    controller.AdjustSelectedRange(controller.CurrentTime);
                } else {
                    controller.SelectRange(controller.CurrentTime, controller.CurrentTime);
                }
                e.IsInputKey = true;
                break;
            case Keys.Up:
                this.RequestedHeight -= addend;
                e.IsInputKey = true;
                break;
            case Keys.Down:
                this.RequestedHeight += addend;
                e.IsInputKey = true;
                break;
            }
        }

        private void closeThisPanelToolStripMenuItem_Click(object sender, EventArgs e) {
            this.CloseView();
        }

        public void CloseView() {
            if(_controller != null) {
                if(MessageBox.Show("このビューを閉じます。", this.GetType().ToString(), MessageBoxButtons.OKCancel) == DialogResult.OK) {
                    _controller.RemoveView(this);
                    this.Dispose();
                }
            }
        }

        private void menuGraphStack_Click(object sender, EventArgs e) {
            menuGraphStack.Checked = StackGraphMode = !StackGraphMode;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e) {
            this.Sequence.Borders.RemoveBorderSameToPrevious();
            this.Sequence.IsDataChanged = true;
            DoRefreshView();
        }


        private void setTextTitleWidth() {
            if(this.InvokeRequired) {
                this.BeginInvoke(new Action(setTextTitleWidth));
                return;
            }
            if(_enterTextTitle) {
                panelTitle.Width = panelTitleInfo.Width;
            } else {
                int candid = panelTitleInfo.Width - panelInfo.MinimumSize.Width * 2;
                if(candid < textTitle.MinimumSize.Width)
                    candid = textTitle.MinimumSize.Width;
                panelTitle.Width = candid;
            }
        }

        private void textTitle_Enter(object sender, EventArgs e) {
            _enterTextTitle = true;
            setTextTitleWidth();
        }

        private void textTitle_Leave(object sender, EventArgs e) {
            _enterTextTitle = false;
            this.SetTitle(textTitle.Text);
            setTextTitleWidth();
        }

        private void minimumToolStripMenuItem_Click(object sender, EventArgs e) {
            RequestedHeight = MinimumHeight;
        }

        private void smallToolStripMenuItem_Click(object sender, EventArgs e) {
            RequestedHeight = SmallHeight;
        }

        private void middleToolStripMenuItem_Click(object sender, EventArgs e) {
            RequestedHeight = MediumHeight;
        }

        private void largeToolStripMenuItem_Click(object sender, EventArgs e) {
            RequestedHeight = LargeHeight;
        }


        int _textViewHeightText = 0;
        bool _textResize_textChangeDisabled = false;
        private void setTextResize(int value) {
            if(this.InvokeRequired) {
                this.Invoke(new Action<int>(setTextResize), value);
                return;
            }
            _textResize_textChangeDisabled = true;
            textResize.Text = (_textViewHeightText = value).ToString();
            _textResize_textChangeDisabled = false;
        }

        private void textResize_TextChanged(object sender, EventArgs e) {
            if(_textResize_textChangeDisabled)
                return;
            int value;
            if(int.TryParse(textResize.Text, out value)) {
                if(value > 1024) {
                    value = 1024;
                    RequestedHeight = value;
                    setTextResize(value);
                } else if(value > 0) {
                    RequestedHeight = value;
                    _textViewHeightText = value;
                } else {
                    setTextResize(_textViewHeightText);
                }
            } else {
                setTextResize(_textViewHeightText);
            }
        }



        private void panelResetScale_MouseClick(object sender, MouseEventArgs e) {
            this.ResetTimeAndValue(false);
        }

        /// <summary>
        /// このビュー以外の他のビューにフォーカスが当てられているかを返します．
        /// </summary>
        /// <returns></returns>
        private bool isOtherViewFocused() {
            if(_isFocused)
                return false;
            SequenceViewerController controller = _controller;
            if(controller == null)
                return false;
            return controller.GetFocusedView() == null;
        }

        private void SequenceView_Leave(object sender, EventArgs e) {
            bool prev = _isFocused;
            _isFocused = false;
            pictureBoxSelect.Visible = false;
            if(prev != _isFocused) {
                this.DeselectValueRange();
                displayImage();
                doRefreshPanelHeader();
                if(!isOtherViewFocused()) {
                    setStatusMessageAsSequenceTitle();
                    SequenceViewerController controller = _controller;
                    if(controller != null) {
                        controller.CursorTime = null;
                    }
                }
            }
        }
        /// <summary>
        /// 選択マーカー用のピクチャボックスの画像を設定します．
        /// </summary>
        void setupPictureBoxSelect() {
            if(pictureBoxSelect.Width > 0 && pictureBoxSelect.Height > 0) {
                if(pictureBoxSelect.Image == null || pictureBoxSelect.Image.Size != pictureBoxSelect.Size) {
                    Bitmap img = new Bitmap(pictureBoxSelect.Width, pictureBoxSelect.Height);
                    using(Graphics gfx = Graphics.FromImage(img)) {
                        gfx.SmoothingMode = SmoothingMode.AntiAlias;
                        gfx.Clear(SystemColors.Highlight);
                        int triangleWidth = img.Width - 3;
                        int center = img.Height / 2;
                        gfx.FillPolygon(new SolidBrush(SystemColors.Control), new PointF[] { new PointF(2, center - triangleWidth * 2f / 3), new PointF(2, center + triangleWidth * 2f / 3), new PointF(1 + triangleWidth, center) });
                    }
                    pictureBoxSelect.Image = img;
                }
            }
        }
        private void pictureBoxSelect_Resize(object sender, EventArgs e) {

        }

        private void SequenceView_Enter(object sender, EventArgs e) {
            bool prev = _isFocused;
            _isFocused = true;
            this.setupPictureBoxSelect();
            pictureBoxSelect.Visible = true;
            if(prev != _isFocused) {
                setStatusMessageAsSequenceTitle();
                displayImage();
                doRefreshPanelHeader();
                SequenceViewerController controller = _controller;
                if(controller != null) {
                    controller.DoFocusedViewChanged();
                }
            }
        }

        private void setStatusMessageAsSequenceTitle() {
            SequenceViewerController controller = _controller;
            if(controller != null) {
                SequenceView focused = controller.GetFocusedView();
                if(focused == null) {
                    this.setStatusMessage("Ready.");
                } else {
                    this.setStatusMessage(focused.Sequence.Title);
                }
            }
        }

        private void contextMenu_Opening(object sender, CancelEventArgs e) {
            setupComboBorder();
            comboBorder.Enabled = !this.IsLocked;
            removeBorderHereToolStripMenuItem.Enabled = !this.IsLocked;

            saveValueDataToolStripMenuItem.Visible = this.Type != SequenceType.Label;
            if(_controller != null) {
                if(_controller.LastOperation != null) {
                    var labelMode = (_controller.LastOperation.OperationTargetType & SequenceType.Label) != 0 && this.Type != SequenceType.Numeric;
                    var valueMode = (_controller.LastOperation.OperationTargetType & SequenceType.Numeric) != 0 && this.Type != SequenceType.Label;
                    if(labelMode || valueMode) {
                        menuLastOperation.DropDown.Items.Clear();
                        foreach(var item in _controller.GetLastOperationToolStripItems()) {
                            menuLastOperation.DropDown.Items.Add(item);
                        }
                        menuLastOperation.Enabled = true;
                    } else {
                        menuLastOperation.Enabled = false;
                    }
                } else {
                    menuLastOperation.Enabled = false;
                }

                if(this.Type != SequenceType.Numeric) {
                    menuOperateLabelSequence.DropDownItems.Clear();
                    foreach(var item in _controller.GetOperationToolStripItems(SequenceType.Label)) {
                        menuOperateLabelSequence.DropDownItems.Add(item);
                    }
                    menuOperateLabelSequence.Enabled = true;
                } else {
                    menuOperateLabelSequence.Enabled = false;
                }
                if(this.Type != SequenceType.Label) {
                    menuOperateValueSequence.DropDownItems.Clear();
                    foreach(var item in _controller.GetOperationToolStripItems(SequenceType.Numeric)) {
                        menuOperateValueSequence.DropDownItems.Add(item);
                    }
                    menuOperateValueSequence.Enabled = true;
                } else {
                    menuOperateValueSequence.Enabled = false;
                }
            }
            contextMenu.Update();
        }

        private void contextMenu_Closed(object sender, ToolStripDropDownClosedEventArgs e) {
            _isMenuLoadLabelUpdated = false;
        }

        private void textTitle_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            if(e.KeyCode == Keys.Escape) {
                SetTitle(this.Sequence.Title);
            }
        }

        private void panelTitleInfo_Resize(object sender, EventArgs e) {
            setTextTitleWidth();
        }

        private void contextMenu_Opened(object sender, EventArgs e) {
            // コンテキストメニューを開いたときにビュー上のカーソルを消さないようにするフラグ．
            _isContextMenuOpened = true;
        }

        private void contextMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e) {
            _isContextMenuOpened = false;
            leavePictView();
        }

        private void leavePictView() {
            _isMouseEnter = false;
            displayImage();
        }


        string _cursorLabel = null;
        private void contextMenuArea_Opening(object sender, CancelEventArgs e) {
            comboBorder.Enabled = false;
            comboBorder.SelectedIndex = -1;
            setupComboBorder();
            comboBorder.Select();
            string label = _cursorLabel ?? "";
            if(label.Length > 24)
                label = label.Substring(0, 21) + "...";
            menuSetLabel.Text = string.Format(global::MotionDataHandler.Properties.Settings.Default.Menu_SetLabelWithName, label);

            menuSetLabel.Enabled = comboBorder.Enabled = (_isValueRangeSelected || this.Type == SequenceType.Label) && !this.IsLocked;

            menuClip.Enabled = !this.IsLocked;
        }

        private void contextMenuArea_Closed(object sender, ToolStripDropDownClosedEventArgs e) {
            displayImage();
        }

        private void menuSetLabelColorDefault_Click(object sender, EventArgs e) {
            _sequence.Borders.ClearColorPalette();
            this.DoRefreshView();
        }

        private void panelLock_MouseClick(object sender, MouseEventArgs e) {
            this.IsLocked = !this.IsLocked;
            doDataChanged();
        }

        private void menuAddBorderHere_Click(object sender, EventArgs e) {
            if(!this.IsLocked) {
                if(this.Type != SequenceType.Numeric) {
                    decimal time = getCursorTime();
                    string labelHere = this.Sequence.GetLabelAt(time);
                    this.Sequence.SetLabelAt(time, labelHere);
                } else {
                    decimal value = getCursorValue();
                    string labelHere = this.Sequence.Borders.GetLabelByValue(value);
                    this.Sequence.Borders.SetBorder(value, labelHere);
                    this.Sequence.DoDataChanged();
                }
                this.DoRefreshView();
            }
        }

        private void menuLockEdit_Click(object sender, EventArgs e) {
            this.IsLocked = !this.IsLocked;
            doDataChanged();
        }

        private void menuOrder_Click(object sender, EventArgs e) {
        }

        private void menuOrder_DropDownOpening(object sender, EventArgs e) {
            ContextMenuStrip tmp = refreshMenuOrder();
            menuOrder.DropDownItems.Clear();
            List<ToolStripItem> items = new List<ToolStripItem>();
            foreach(ToolStripItem item in tmp.Items) {
                items.Add(item);
            }
            items.ForEach(item => menuOrder.DropDownItems.Add(item));
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e) {

        }

        private void menuJumpHead_Click(object sender, EventArgs e) {
            SequenceViewerController controller = _controller;
            if(controller == null)
                return;
            if(controller.IsSelecting) {
                controller.CurrentTime = controller.SelectBeginTime;
            }
        }

        private void menuJumpTail_Click(object sender, EventArgs e) {
            SequenceViewerController controller = _controller;
            if(controller == null)
                return;
            if(controller.IsSelecting) {
                controller.CurrentTime = controller.SelectEndTime;
            }
        }

        private void menuSetLabel_Click(object sender, EventArgs e) {
            SetLabelForSelected(_cursorLabel);
        }

        private void panelToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
            if(!_isMenuLoadLabelUpdated) {
                _isMenuLoadLabelUpdated = true;
                menuLoadLabel.DropDownItems.Clear();
                menuLoadLabel.DropDownItems.AddRange(_controller.CreateLabelOpenMenu().ToArray());
            }
        }

        private void pictureBoxSelect_SizeChanged(object sender, EventArgs e) {
            setupPictureBoxSelect();
        }

    }
}


