using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MotionDataHandler.Misc {
    using Misc;
    /// <summary>
    /// 時間を示すためのルーラーを表示し，時間範囲を選択するためのコントロール
    /// </summary>
    public partial class TimeSelectionControl : UserControl {
        //enum EnumResizeArea {
        //    None,
        //    ResizeBegin,
        //    ResizeEnd,
        //    Move,
        //}
        public TimeSelectionControl() {
            InitializeComponent();
        }
        [Category("Mode")]
        [Description("タイムバーが選択された時間幅でなく全時間幅を表示するようにします")]
        public bool IsGlobalTimeLineMode { get { return _isGlobalTimeLineMode; } set { _isGlobalTimeLineMode = value; } }
        private bool _isGlobalTimeLineMode = false;
        private bool _invertY;
        [Category("表示")]
        [Description("メモリの位置を下にします")]
        public bool InvertY {
            get { return _invertY; }
            set { _invertY = value; }
        }
        private bool _showsFrameIndex;
        [Category("表示")]
        [Description("時間の代わりにフレーム番号を表示します")]
        public bool ShowsFrameIndex {
            get { return _showsFrameIndex; }
            set { _showsFrameIndex = value; }
        }

        private bool _showsCursorTimeLabel = true;

        [Category("表示")]
        [Description("カーソル位置の時間のラベルを表示します")]
        public bool ShowsCursorTimeLabel {
            get { return _showsCursorTimeLabel; }
            set { _showsCursorTimeLabel = value; }
        }


        [Category("表示")]
        [Description("現在時刻を操作するボタンのサイズを指定します")]
        public int CurrentTimeTabSize {
            get { return global::MotionDataHandler.Properties.Settings.Default.CurrentTimeTabSize; }
            set {
                global::MotionDataHandler.Properties.Settings.Default.CurrentTimeTabSize = Math.Max(6, value);
                setCurrentTimeBar();
            }
        }

        private decimal _beginTime, _endTime, _beginSelectTime, _endSelectTime;
        RegulatedMouseControl _mouse = new RegulatedMouseControl(0, RegulatedMouseButton.Left, RegulatedMouseButton.CtrlLeft, RegulatedMouseButton.AltLeft, RegulatedMouseButton.ShiftLeft, RegulatedMouseButton.ShiftCtrlLeft);
        //EnumResizeArea _resizingMode = EnumResizeArea.None;
        decimal _selectingTime;

        TimeController _timeController;
        bool _focued = false;

        int _majorRuleHeight = 10;
        int _subMajorRuleHeight = 8;
        int _minorRuleHeight = 6;

        public int MajorRuleHeight {
            get { return _majorRuleHeight; }
            set { _majorRuleHeight = value; }
        }
        public int SubMajorRuleHeight {
            get { return _subMajorRuleHeight; }
            set { _subMajorRuleHeight = value; }
        }
        public int MinorRuleHeight {
            get { return _minorRuleHeight; }
            set { _minorRuleHeight = value; }
        }

        public Color TimeLineBackColor = SystemColors.Window;
        public Color TimeLineSelectColor = SystemColors.Highlight;
        public Color TimeLineRulerColor = SystemColors.WindowText;
        public Color TimeLineCurrentColor = Color.Blue;
        Color _prevCurrentColor = Color.Blue;

        public void AttachTimeController(TimeController timeController) {
            if(_timeController != null) {
                _timeController.CurrentTimeChanged -= new EventHandler(_timeController_TimeChanged);
                _timeController.VisibleRangeChanged -= new EventHandler(_timeController_SelectTimeChanged);
                _timeController.SelectedRangeChanged -= new EventHandler(_timeController_SelectedRangeChanged);
                _timeController.SettingsChanged -= new EventHandler(_timeController_SettingsChanged);
                _timeController.CursorTimeChanged -= new EventHandler(_timeController_CursorTimeChanged);
            }
            _timeController = timeController;
            if(_timeController != null) {
                _timeController.CurrentTimeChanged += new EventHandler(_timeController_TimeChanged);
                _timeController.VisibleRangeChanged += new EventHandler(_timeController_SelectTimeChanged);
                _timeController.SelectedRangeChanged += new EventHandler(_timeController_SelectedRangeChanged);
                _timeController.SettingsChanged += new EventHandler(_timeController_SettingsChanged);
                _timeController.CursorTimeChanged += new EventHandler(_timeController_CursorTimeChanged);
                setTimeRange();
                setSelectTimeRange();
                setCurrentTimeBar();
                displayTimeBar();
            }
        }

        void _timeController_SelectedRangeChanged(object sender, EventArgs e) {
            TimeController timeController = _timeController;
            if(timeController == null)
                return;
            _beginSelectTime = timeController.SelectBeginTime;
            _endSelectTime = timeController.SelectEndTime;
            displayTimeBar();
        }

        void setTimeRange() {
            TimeController timeController = _timeController;
            if(timeController == null)
                return;
            if(this.IsGlobalTimeLineMode) {
                _beginTime = timeController.BeginTime;
                _endTime = timeController.EndTime;
            } else {
                _beginTime = timeController.VisibleBeginTime;
                _endTime = timeController.VisibleEndTime;
            }
        }

        void setSelectTimeRange() {
            TimeController timeController = _timeController;
            if(timeController == null)
                return;
            _beginSelectTime = timeController.SelectBeginTime;
            _endSelectTime = timeController.SelectEndTime;
        }

        void setCurrentTimeBar() {
            if(this.InvokeRequired) {
                this.BeginInvoke(new Action(setCurrentTimeBar));
                return;
            }
            TimeController timeController = _timeController;
            if(timeController == null)
                return;
            pictCurrent.Width = 1;
            pictCurrent.Height = this.Height;
            pictCurrent.Location = new Point((int)Math.Round(getDisplayPosition(timeController.CurrentTime)), 0);
            pictCurrentTab.Size = new Size(CurrentTimeTabSize, CurrentTimeTabSize);
            pictCurrentTab.Location = new Point(pictCurrent.Location.X - CurrentTimeTabSize / 2, 0);
        }

        void _timeController_SelectTimeChanged(object sender, EventArgs e) {
            setSelectTimeRange();
            setTimeRange();
            displayTimeBar();
            setCurrentTimeBar();
        }

        void _timeController_SettingsChanged(object sender, EventArgs e) {
            setSelectTimeRange();
            setTimeRange();
            displayTimeBar();
            setCurrentTimeBar();
        }

        void _timeController_TimeChanged(object sender, EventArgs e) {
            setCurrentTimeBar();
        }

        decimal? _prevCursor = null;
        void _timeController_CursorTimeChanged(object sender, EventArgs e) {
            if(this.InvokeRequired) {
                this.BeginInvoke(new EventHandler(_timeController_CursorTimeChanged), sender, e);
                return;
            }
            TimeController timeController = _timeController;
            if(timeController == null)
                return;
            decimal? cursor = timeController.CursorTime;
            if(_prevCursor != cursor) {
                if(cursor.HasValue) {
                    pictCursor.Location = new Point((int)Math.Round(getDisplayPosition(cursor.Value)), 0);
                    pictCursor.Size = new Size(1, pictDisplay.Height);
                    pictCursor.Visible = true;
                    if(this.ShowsCursorTimeLabel) {
                        labelCursor.Visible = true;
                        if(this.ShowsFrameIndex) {
                            labelCursor.Text = "[" + timeController.GetIndexFromTime(cursor.Value) + "]";
                        } else {
                            labelCursor.Text = cursor.Value.ToString("0.000");
                        }
                        if(pictCursor.Location.X + 5 + labelCursor.Width >= pictDisplay.Width) {
                            labelCursor.Location = new Point(pictCursor.Location.X - 5 - labelCursor.Width, (pictDisplay.Height - labelCursor.Height) / 2);
                        } else {
                            labelCursor.Location = new Point(pictCursor.Location.X + 5, (pictDisplay.Height - labelCursor.Height) / 2);
                        }
                    }
                } else {
                    pictCursor.Visible = false;
                    labelCursor.Visible = false;
                }
            }
            _prevCursor = cursor;
        }

        //private float _resizeGripLength = 2f;
        //EnumResizeArea testResizeArea(MouseEventArgs e) {
        //    TimeController timeController = _timeController;
        //    if(timeController == null)
        //        return EnumResizeArea.None;
        //    try {
        //        if(timeController.IsSelecting) {
        //            float leftIndex = getDisplayPosition(_beginSelectTime);
        //            float rightIndex = getDisplayPosition(_endSelectTime);
        //            if(leftIndex + _resizeGripLength <= e.X && e.X <= rightIndex - _resizeGripLength) {
        //                return EnumResizeArea.Move;
        //            } else if(leftIndex - _resizeGripLength <= e.X && e.X <= leftIndex + _resizeGripLength) {
        //                // 左
        //                return EnumResizeArea.ResizeBegin;
        //            } else if(rightIndex - _resizeGripLength <= e.X && e.X <= rightIndex + _resizeGripLength) {
        //                // 右
        //                return EnumResizeArea.ResizeEnd;
        //            } else {
        //                // 外
        //                return EnumResizeArea.None;
        //            }
        //        } else {
        //            return EnumResizeArea.None;
        //        }
        //    } catch(OverflowException) {
        //        return EnumResizeArea.None;
        //    }
        //}


        //void setCursorShape(EnumResizeArea resizeMode) {
        //    if((Control.ModifierKeys & Keys.Control) != 0) {
        //        switch(resizeMode) {
        //        case EnumResizeArea.None:
        //            pictDisplay.Cursor = Cursor.Current = Cursors.Default;
        //            break;
        //        case EnumResizeArea.ResizeBegin:
        //        case EnumResizeArea.ResizeEnd:
        //            pictDisplay.Cursor = Cursor.Current = Cursors.VSplit;
        //            break;
        //        case EnumResizeArea.Move:
        //            pictDisplay.Cursor = Cursor.Current = Cursors.SizeAll;
        //            break;
        //        }
        //    } else {
        //        pictDisplay.Cursor = Cursor.Current = Cursors.Default;
        //    }
        //}

        private void displayTimeBar() {
            if(this.InvokeRequired) {
                this.BeginInvoke(new Action(displayTimeBar));
                return;
            }

            int width, height;
            TimeController timeController = _timeController;
            if(timeController == null)
                return;
            width = pictDisplay.Width;
            height = pictDisplay.Height;
            if(width <= 0 || height <= 0)
                return;
            Image img = new Bitmap(width, height);
            using(Graphics gfx = Graphics.FromImage(img)) {
                gfx.Clear(getColorFocused(this.TimeLineBackColor));
                Pen pen = new Pen(getColorFocused(this.TimeLineRulerColor));
                Font font = new Font("UI Gothic", 10);
                Brush strBrush = new SolidBrush(getColorFocused(this.TimeLineRulerColor));
                decimal ruleInterval = getRuleInterval(_endTime - _beginTime, 4);
                if(ruleInterval > 0) {
                    int digit = (int)Math.Floor(Math.Log10((double)ruleInterval * 10));
                    string formatForSec = "0";
                    string formatForSec2 = "0.00";
                    if(digit < 0) {
                        formatForSec += ".";
                        for(int i = 0; i < -digit; i++) {
                            formatForSec += "0";
                            formatForSec2 += "0";
                        }
                    }

                    decimal visibleBeginIndex = getTimeFromPosition(0) / ruleInterval;
                    decimal visibleEndIndex = getTimeFromPosition(width) / ruleInterval;
                    decimal beginIndex = Math.Max(timeController.BeginTime / ruleInterval, visibleBeginIndex - 10);
                    decimal endIndex = Math.Min(timeController.EndTime / ruleInterval, visibleEndIndex + 10);
                    for(decimal index = decimal.Ceiling(beginIndex); index < endIndex; index++) {
                        decimal ruleTime = index * ruleInterval;
                        float rulePos = getDisplayPosition(ruleTime);
                        int ruleHeight = this.MinorRuleHeight;
                        if(decimal.Round(decimal.Remainder(index, 5M)) == 0) {
                            if(decimal.Round(decimal.Remainder(index, 10M)) == 0) {
                                // 主目盛
                                ruleHeight = this.MajorRuleHeight;
                                string ruleText;
                                if(this.ShowsFrameIndex) {
                                    ruleText = "[" + timeController.GetIndexFromTime(ruleTime) + "]";
                                } else {
                                    ruleText = decimal.Round(ruleTime, Math.Max(0, -digit)).ToString(formatForSec);
                                }
                                var timeTextSize = gfx.MeasureString(ruleText, font);
                                gfx.DrawString(ruleText, font, strBrush, new PointF(rulePos - timeTextSize.Width / 2f, _invertY ? (height - _subMajorRuleHeight - timeTextSize.Height) : _subMajorRuleHeight + 1));
                            } else {
                                // 副目盛
                                ruleHeight = this.SubMajorRuleHeight;
                            }
                        }
                        gfx.DrawLine(pen, new PointF(rulePos, _invertY ? height - 1 : 0), new PointF(rulePos, _invertY ? (height - ruleHeight - 1) : ruleHeight));
                    }
                    string beginStr = _beginTime.ToString(formatForSec2);
                    string endStr = _endTime.ToString(formatForSec2);
                    var beginSize = gfx.MeasureString(beginStr, font);
                    gfx.DrawString(beginStr, font, new SolidBrush(Color.Red), new PointF(0, _invertY ? (height - 1 - beginSize.Height) : 0));
                    var endSize = gfx.MeasureString(endStr, font);
                    gfx.DrawString(endStr, font, new SolidBrush(Color.Red), new PointF(width - endSize.Width, _invertY ? (height - 1 - beginSize.Height) : 0));
                }
                float beginVisiblePos = getDisplayPosition(timeController.VisibleBeginTime);
                float endVisiblePos = getDisplayPosition(timeController.VisibleEndTime);
                Brush invisibleBrush = new SolidBrush(Color.FromArgb(64, Color.Black));
                gfx.FillRectangle(invisibleBrush, new RectangleF(0, 0, beginVisiblePos, height));
                gfx.FillRectangle(invisibleBrush, new RectangleF(endVisiblePos, 0, width - endVisiblePos, height));
                if(timeController.IsSelecting) {
                    float beginSelectPos = getDisplayPosition(_beginSelectTime);
                    float endSelectPos = getDisplayPosition(_endSelectTime);
                    float markerheight = 5;
                    float markerWidth = markerheight * (float)Math.Sqrt(3) / 2;
                    float topY = this.InvertY ? height - 1 : 0;
                    float middleY = this.InvertY ? height - markerheight - 1 : markerheight;
                    float lineY = this.InvertY ? (height - markerheight) * 0.8f - 1 : markerheight + (height - markerheight) * 0.2f;
                    float bottomY = this.InvertY ? 0 : height - 1;
                    Color color = getColorFocused(this.TimeLineSelectColor);
                    Brush fillBgBrush = new SolidBrush(Color.FromArgb(64, color));
                    Brush fillBrush = new SolidBrush(color);
                    Pen fillPen = new Pen(color);

                    RectangleF fillRect = new RectangleF(beginSelectPos, 0, endSelectPos - beginSelectPos, height);
                    gfx.FillRectangle(fillBgBrush, fillRect);
                    foreach(float pos in new float[] { beginSelectPos, endSelectPos }) {
                        PointF[] triangle = new PointF[] { new PointF(pos, middleY), new PointF(pos + markerWidth, topY), new PointF(pos - markerWidth, topY) };
                        gfx.FillPolygon(fillBrush, triangle);
                        gfx.DrawPolygon(fillPen, triangle);
                        gfx.DrawLine(fillPen, new PointF(pos, middleY), new PointF(pos, bottomY));
                    }
                    gfx.DrawLine(fillPen, new PointF(beginSelectPos, lineY), new PointF(endSelectPos, lineY));
                }
            }
            pictDisplay.Image = img;
        }

        private Color getColorFocused(Color color) {
            if(_focued) {
                return color;
            } else {
                return Color.FromArgb(color.A, color.R / 2 + 96, color.G / 2 + 96, color.B / 2 + 96);
            }
        }

        private float getDisplayPosition(decimal time) {
            decimal duration = _endTime - _beginTime;
            decimal offset = time - _beginTime;
            if(duration <= 0)
                return 0;
            return (float)(offset / duration * (pictDisplay.Width - 1));
        }

        private decimal getTimeFromPosition(float pos) {
            return _beginTime + getTimeSpanFromDelta(pos);
        }

        private decimal getTimeSpanFromDelta(float delta) {
            if(this.Width <= 1)
                return 0;
            decimal duration = _endTime - _beginTime;
            decimal offset = duration * (decimal)delta / (pictDisplay.Width - 1);
            return offset;
        }

        private decimal getRuleInterval(decimal timeInterval, int minimumDisplayInterval) {
            TimeController timeController = _timeController;
            if(timeController == null)
                return 0;
            if(timeInterval <= 0)
                return 0;
            decimal ret;
            if(this.ShowsFrameIndex) {
                ret = 1M / timeController.FPS;
                while(ret < 5000000M) {
                    if((decimal)pictDisplay.Width * ret / timeInterval >= minimumDisplayInterval)
                        return ret;
                    ret *= 5M;
                    if(ret >= 5000000M)
                        break;
                    if((decimal)pictDisplay.Width * ret / timeInterval >= minimumDisplayInterval)
                        return ret;
                    ret *= 2M;
                }
                return ret;
            } else {
                for(decimal candid = 0.0000001M; candid < 100000000M; candid *= 10M) {
                    if((decimal)pictDisplay.Width * candid / timeInterval >= minimumDisplayInterval)
                        return candid;
                    if((decimal)pictDisplay.Width * candid * 5M / timeInterval >= minimumDisplayInterval)
                        return candid * 5M;
                }
                return timeController.Duration / 5M;
            }
        }

        private void TimeSelectionControl_Resize(object sender, EventArgs e) {
            setCurrentTimeBar();
            displayTimeBar();
        }

        private void TimeSelectionControl_Enter(object sender, EventArgs e) {
            bool prev = _focued;
            _focued = true;
            if(prev != _focued) {
                displayTimeBar();
            }
        }

        private void TimeSelectionControl_Leave(object sender, EventArgs e) {
            bool prev = _focued;
            _focued = false;
            if(prev != _focued) {
                displayTimeBar();
            }
        }



        private void pictDisplay_MouseDoubleClick(object sender, MouseEventArgs e) {
            decimal time = getTimeFromPosition(e.X);
            TimeController timeController = _timeController;
            if(timeController == null) {
                return;
            }
            timeController.CurrentTime = time;
        }


        private void TimeSelectionControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            TimeController timeController = _timeController;
            if(timeController != null) {
                int addend = 1;
                if(e.Control)
                    addend = 8;
                switch(e.KeyCode) {
                case Keys.Right:
                    timeController.CurrentIndex += addend;
                    timeController.CursorTime = timeController.CurrentTime;
                    if(e.Shift) {
                        timeController.AdjustSelectedRange(timeController.CurrentTime);
                    } else {
                        timeController.SelectRange(timeController.CurrentTime, timeController.CurrentTime);
                    }
                    e.IsInputKey = true;
                    break;
                case Keys.Left:
                    timeController.CurrentIndex -= addend;
                    timeController.CursorTime = timeController.CurrentTime;
                    if(e.Shift) {
                        timeController.AdjustSelectedRange(timeController.CurrentTime);
                    } else {
                        timeController.SelectRange(timeController.CurrentTime, timeController.CurrentTime);
                    }
                    e.IsInputKey = true;
                    break;
                case Keys.Down:
                    this.CurrentTimeTabSize = Math.Min(this.Height / 2, this.CurrentTimeTabSize + addend);
                    e.IsInputKey = true;
                    break;
                case Keys.Up:
                    this.CurrentTimeTabSize -= addend;
                    e.IsInputKey = true;
                    break;
                }

            }
        }

        private void TimeSelectionControl_MouseLeave(object sender, EventArgs e) {
            Cursor.Current = Cursors.Default;
        }

        private void pictDisplay_MouseMove(object sender, MouseEventArgs e) {
            TimeController timeController = _timeController;
            if(timeController == null)
                return;
            decimal cursorTime = getTimeFromPosition(e.X);
            //cursorTime = Math.Min(timeController.VisibleEndTime, Math.Max(timeController.VisibleBeginTime, cursorTime));
            timeController.CursorTime = cursorTime;


            RegulatedMouseInfo info = _mouse.MouseMove(e);
            decimal timeDelta = getTimeSpanFromDelta(_mouse.MoveDelta.X);
            switch(info.Button) {
            case RegulatedMouseButton.Left:
                timeController.CursorTime = cursorTime;
                timeController.SelectRange(Math.Min(cursorTime, _selectingTime), Math.Max(cursorTime, _selectingTime));
                break;
            case RegulatedMouseButton.CtrlLeft:
                timeController.CurrentTime = cursorTime;
                timeController.SelectRange(Math.Min(cursorTime, _selectingTime), Math.Max(cursorTime, _selectingTime));
                break;
            case RegulatedMouseButton.AltLeft:
                decimal timeOffset = timeDelta;
                if(!this.IsGlobalTimeLineMode) {
                    timeOffset = -timeOffset;
                }
                timeController.SetVisibleTime(timeController.VisibleBeginTime + timeOffset, timeController.VisibleEndTime + timeOffset);
                break;
            case RegulatedMouseButton.ShiftLeft:
                timeController.AdjustSelectedRange(cursorTime);
                break;
            case RegulatedMouseButton.ShiftCtrlLeft:
                timeController.CurrentTime = cursorTime;
                timeController.AdjustSelectedRange(cursorTime);
                break;

            }
        }

        private void pictDisplay_MouseDown(object sender, MouseEventArgs e) {
            TimeController timeController = _timeController;
            if(timeController == null)
                return;
            RegulatedMouseButton button = _mouse.MouseDown(e);
            decimal cursorTime = getTimeFromPosition(e.X);
            //cursorTime = Math.Min(timeController.VisibleEndTime, Math.Max(timeController.VisibleBeginTime, cursorTime));
            _selectingTime = cursorTime;

            switch(button) {
            case RegulatedMouseButton.Left:
                timeController.CursorTime = cursorTime;
                timeController.SelectRange(Math.Min(cursorTime, _selectingTime), Math.Max(cursorTime, _selectingTime));
                break;
            case RegulatedMouseButton.CtrlLeft:
                timeController.CurrentTime = cursorTime;
                timeController.SelectRange(Math.Min(cursorTime, _selectingTime), Math.Max(cursorTime, _selectingTime));
                break;
            case RegulatedMouseButton.ShiftCtrlLeft:
                timeController.AdjustSelectedRange(cursorTime);
                timeController.CurrentTime = cursorTime;
                break;
            case RegulatedMouseButton.ShiftLeft:
                timeController.AdjustSelectedRange(cursorTime);
                break;
            }

        }

        private void pictDisplay_MouseUp(object sender, MouseEventArgs e) {
            RegulatedMouseInfo info = _mouse.MouseUp(e);
        }

        private void pictDisplay_Click(object sender, EventArgs e) {
            this.Focus();
        }

        private void cursorLeave() {
            TimeController timeController = _timeController;
            if(timeController != null) {
                timeController.CursorTime = null;
            }
        }

        private void pictDisplay_MouseEnter(object sender, EventArgs e) {
        }

        private void pictDisplay_MouseLeave(object sender, EventArgs e) {
            cursorLeave();
        }

        private void showFrameNumberToolStripMenuItem_Click(object sender, EventArgs e) {
            this.ShowsFrameIndex = !this.ShowsFrameIndex;
            showFrameNumberToolStripMenuItem.Checked = this.ShowsFrameIndex;
            displayTimeBar();
        }

        private void inputSelectRangeToolStripMenuItem_Click(object sender, EventArgs e) {
            TimeController timeController = _timeController;
            if(timeController == null)
                return;
            Misc.DialogSetSelectRange dialog = new MotionDataHandler.Misc.DialogSetSelectRange(timeController, this.ShowsFrameIndex);
            dialog.ShowDialog();
        }

        private void resetScaleToolStripMenuItem_Click(object sender, EventArgs e) {
            TimeController timeController = _timeController;
            if(timeController == null)
                return;
            timeController.SetVisibleTime(timeController.BeginTime, timeController.EndTime);
        }

        private void zoomSelectedRangeToolStripMenuItem_Click(object sender, EventArgs e) {
            TimeController timeController = _timeController;
            if(timeController == null)
                return;
            timeController.ZoomSelectedRange();
        }

        private void menuUnzoom_Click(object sender, EventArgs e) {
            TimeController timeController = _timeController;
            if(timeController == null)
                return;
            timeController.UnzoomSelectedRange();

        }

        private void selectVisibleRangeToolStripMenuItem_Click(object sender, EventArgs e) {
            TimeController timeController = _timeController;
            if(timeController == null)
                return;
            timeController.SelectVisibleRange();
        }

        private void contextMenuMain_Opening(object sender, CancelEventArgs e) {
            resetScaleToolStripMenuItem.Visible = !this.IsGlobalTimeLineMode;
            selectVisibleRangeToolStripMenuItem.Visible = !this.IsGlobalTimeLineMode;
            menuUnzoom.Visible = !this.IsGlobalTimeLineMode;
            TimeController timeController = _timeController;
            if(timeController == null)
                return;
            menuZoom.Enabled = menuUnzoom.Enabled = timeController.IsSelecting && (timeController.SelectEndTime - timeController.SelectBeginTime) > 0;
        }

        private void JumpHereToolStripMenuItem_Click(object sender, EventArgs e) {
            TimeController timeController = _timeController;
            if(timeController == null)
                return;
            decimal? cursor = timeController.CursorTime;
            if(cursor.HasValue)
                timeController.CurrentTime = cursor.Value;
        }

        #region CurrentTimeTab

        decimal _startTimeOnDownTab = 0;
        bool _leftDownTab = false;
        bool _rightDownTab = false;
        bool _onTab = false;
        private void setPictCurrentTab() {
            Bitmap img = new Bitmap(pictCurrentTab.Width, pictCurrentTab.Height);
            using(Graphics gfx = Graphics.FromImage(img)) {
                gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                gfx.DrawImage(global::MotionDataHandler.Properties.Resources.downTriangleTab, new Rectangle(Point.Empty, pictCurrentTab.Size));
                if(_leftDownTab && !_rightDownTab) {
                    gfx.FillRectangle(new SolidBrush(Color.FromArgb(128, SystemColors.MenuHighlight)), new Rectangle(Point.Empty, pictCurrentTab.Size));
                } else if(_onTab) {
                    gfx.FillRectangle(new SolidBrush(Color.FromArgb(64, SystemColors.MenuHighlight)), new Rectangle(Point.Empty, pictCurrentTab.Size));
                }
            }
            pictCurrentTab.Image = img;
        }

        private void pictCurrentTab_MouseDown(object sender, MouseEventArgs e) {
            TimeController timeController = _timeController;
            if(timeController == null)
                return;
            switch(e.Button) {
            case MouseButtons.Left:
                if(!_rightDownTab) {
                    _leftDownTab = true;
                    _startTimeOnDownTab = getTimeFromPosition(e.X + pictCurrentTab.Location.X);
                }
                setPictCurrentTab();
                break;
            case MouseButtons.Right:
                if(_leftDownTab) {
                    timeController.CurrentTime = _startTimeOnDownTab;
                    _rightDownTab = true;
                }
                setPictCurrentTab();
                break;
            }
        }

        private void pictCurrentTab_MouseMove(object sender, MouseEventArgs e) {
            TimeController timeController = _timeController;
            if(timeController == null)
                return;
            if(_leftDownTab && !_rightDownTab) {
                timeController.CurrentTime = getTimeFromPosition(e.X + pictCurrentTab.Location.X);
                if((Control.ModifierKeys & Keys.Shift) != 0) {
                    timeController.AdjustSelectedRange(timeController.CurrentTime);
                }
            }
        }

        private void pictCurrentTab_MouseUp(object sender, MouseEventArgs e) {
            TimeController timeController = _timeController;
            if(timeController == null)
                return;
            switch(e.Button) {
            case MouseButtons.Left:
                _leftDownTab = false;
                setPictCurrentTab();
                break;
            case MouseButtons.Right:
                _rightDownTab = false;
                setPictCurrentTab();
                break;
            }
        }

        private void pictCurrentTab_MouseEnter(object sender, EventArgs e) {
            _onTab = true;
            setPictCurrentTab();
        }

        private void pictCurrentTab_MouseLeave(object sender, EventArgs e) {
            _onTab = false;
            setPictCurrentTab();
        }

        #endregion
    }
}
