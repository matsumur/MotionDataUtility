using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace MotionDataHandler.Motion {
    using Misc;

    /// <summary>
    /// モーションデータの各オブジェクトの欠損がどのように分布しているかを表示するためのコントロール
    /// </summary>
    public partial class ObjectExistenceView : UserControl {
        public ObjectExistenceView() {
            InitializeComponent();
            _existenceList = new Dictionary<uint, RangeSet<int>>();
            this.Disposed += new EventHandler(ObjectExistenceView_Disposed);
        }

        void ObjectExistenceView_Disposed(object sender, EventArgs e) {
            DetachDataSet();
            AttachTimeController(null);
        }

        public void AttachDataSet(MotionDataSet dataSet) {
            lock(_lockAccessDataSet) {
                if(_dataSet != null) {
                    _dataSet.ObjectSelectionChanged -= OnSelectedChanged;
                    _dataSet.FrameListChanged -= _dataSet_FrameListChanged;
                }
                _dataSet = dataSet;
                if(_dataSet != null) {
                    _dataSet.ObjectSelectionChanged += OnSelectedChanged;
                    _dataSet.FrameListChanged += _dataSet_FrameListChanged;
                }
            }
            render();
        }

        void _dataSet_FrameListChanged(object sender, EventArgs e) {
            _existenceList.Clear();
            render();
        }

        public void DetachDataSet() {
            this.AttachDataSet(null);
        }

        public void AttachTimeController(TimeController timeController) {
            lock(_lockAccessTimeController) {
                if(_timeController != null) {
                    _timeController.CurrentTimeChanged -= _timeController_TimeChanged;
                    _timeController.VisibleRangeChanged -= new EventHandler(_timeController_SelectTimeChanged);
                    timeSelectionControl1.AttachTimeController(null);
                }
                _timeController = timeController;
                if(_timeController != null) {
                    _timeController.CurrentTimeChanged += _timeController_TimeChanged;
                    _timeController.VisibleRangeChanged += new EventHandler(_timeController_SelectTimeChanged);
                    timeSelectionControl1.AttachTimeController(_timeController);
                }
            }
        }


        MotionDataSet _dataSet;
        TimeController _timeController;
        readonly Dictionary<uint, RangeSet<int>> _existenceList = new Dictionary<uint, RangeSet<int>>();
        bool _renderRequested = false;
        Image _graphImage = null;
        int _width = 0, _height = 0;
        readonly object _lockAccessGraphImage = new object();
        readonly object _lockAccessDataSet = new object();
        readonly object _lockAccessTimeController = new object();

        void setPictureImage(PictureBox picture, Image img) {
            if(this.InvokeRequired) {
                this.BeginInvoke(new Action<PictureBox, Image>(setPictureImage), picture, img);
                return;
            }
            picture.Image = img;
        }
        void setPictureImageImmediate(PictureBox picture, Image img) {
            if(this.InvokeRequired) {
                this.Invoke(new Action<PictureBox, Image>(setPictureImageImmediate), picture, img);
                return;
            }
            picture.Image = img;
        }

        void setText(Control control, string text) {
            if(this.InvokeRequired) {
                this.BeginInvoke(new Action<Control, string>(setText), control, text);
                return;
            }
            control.Text = text;
        }

        /// <summary>
        /// モーションオブジェクトの欠損/被欠損の情報をフレーム単位で取得する
        /// </summary>
        /// <param name="id">情報を取得するさきのオブジェクトのId</param>
        /// <returns></returns>
        private RangeSet<int> getExistenceList(uint id) {
            MotionDataSet dataSet = _dataSet;
            if(dataSet == null)
                return null;
            RangeSet<int> ret;
            if(_existenceList.TryGetValue(id, out ret)) {
                return ret;
            }
            try {
                ret = new RangeSet<int>();
                int index = 0;
                foreach(var frame in dataSet.EnumerateFrame()) {
                    if(frame[id] != null) {
                        ret.Add(new RangeSet<int>.Range(index, index + 1));
                    }
                    index++;
                }
                _existenceList[id] = ret;
                return ret;
            } catch { return null; }
        }

        /// <summary>
        /// モーションデータの時間の値からグラフ上のx座標の値を求めます．
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private float timeToPosition(decimal time) {
            TimeController timeController = _timeController;
            if(timeController == null)
                return 0;
            decimal duration = timeController.VisibleDuration;
            decimal begin = timeController.VisibleBeginTime;
            if(duration == 0)
                return 0;
            try {
                return (float)((decimal)_width * (time - begin) / duration);
            } catch(OverflowException) { return 0; }
        }

        /// <summary>
        /// グラフ上のx座標の値からモーションデータの時間の値を求めます
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private decimal positionToTime(float x) {
            TimeController timeController = _timeController;
            if(timeController == null)
                return 0;
            decimal duration = timeController.VisibleDuration;
            decimal begin = timeController.VisibleBeginTime;
            int width = _width;
            if(width == 0)
                return 0;
            try {
                return duration * (decimal)x / width + begin;
            } catch(OverflowException) { return 0; }
        }

        private void OnSelectedChanged(object sender, EventArgs e) {
            render();
        }

        private void render() {
            if(bgwRender.IsBusy) {
                if(!_renderRequested) {
                    _renderRequested = true;
                    bgwRender.CancelAsync();
                }
                return;
            }
            lock(_lockAccessGraphImage) {
                if(!bgwRender.IsBusy) {
                    _graphImage = null;
                    _width = pictGraph.Width;
                    _height = pictGraph.Height;

                    _renderRequested = false;
                    bgwRender.RunWorkerAsync();
                }
            }
        }

        void _timeController_SelectTimeChanged(object sender, EventArgs e) {
            render();
        }
        private void _timeController_TimeChanged(object sender, EventArgs e) {
            this.DoPaint();
        }

        public void DoPaint() {
            if(_graphImage == null) {
                return;
            }
            lock(_lockAccessGraphImage) {
                if(_graphImage == null) {
                    return;
                }
                Bitmap tmp = new Bitmap(_graphImage);
                if(_timeController != null) {
                    float pos = timeToPosition(_timeController.CurrentTime);
                    using(Graphics gfx = Graphics.FromImage(tmp)) {
                        gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        gfx.DrawLine(new Pen(Color.FromArgb(128, Color.Blue)), new PointF(pos, 0), new PointF(pos, _height));
                    }
                }
                setPictureImage(pictGraph, tmp);
            }
        }

        private void ObjectExistenceView_Resize(object sender, EventArgs e) {
            render();
        }

        private void bgwRender_DoWork(object sender, DoWorkEventArgs e) {
            TimeController timeController = _timeController;
            MotionDataSet dataSet = _dataSet;
            if(dataSet == null || timeController == null) {
                Bitmap infoImage = new Bitmap(_width, _height);
                using(Graphics gfx = Graphics.FromImage(infoImage)) {
                    gfx.Clear(Color.DimGray);
                    gfx.DrawString("Error...", this.Font, Brushes.LightGray, new PointF());
                }
                setPictureImage(pictGraph, infoImage);
                return;
            }

            RangeSet<int> existAll = null; // 選択されたオブジェクトが被欠損なインデックス
            List<int> clipIndices = new List<int>(); // グラフ上の各x座標のインデックスの範囲
            Collection<MotionObjectInfo> infoList;
            // メッセージを表示
            setText(labelInfo, "データ欠損情報");
            infoList = dataSet.GetSelectedObjectInfoList();
            if(infoList.Count == 0) {
                Bitmap infoImage = new Bitmap(_width, _height);
                using(Graphics gfx = Graphics.FromImage(infoImage)) {
                    gfx.Clear(Color.DimGray);
                    gfx.DrawString("選択オブジェクトなし", this.Font, Brushes.Ivory, new PointF());
                }
                setPictureImage(pictGraph, infoImage);
                return;
            } else {
                Bitmap infoImage = new Bitmap(_width, _height);
                using(Graphics gfx = Graphics.FromImage(infoImage)) {
                    gfx.Clear(Color.DimGray);
                    gfx.DrawString("読み込み中...", this.Font, Brushes.LightGray, new PointF());
                }
                setPictureImageImmediate(pictGraph, infoImage);
            }
            // 選択された全オブジェクトの非欠損のandを取る
            foreach(var info in infoList) {
                if(bgwRender.CancellationPending) {
                    e.Cancel = true;
                    return;
                }
                if(existAll == null) {
                    existAll = getExistenceList(info.Id);
                } else {
                    existAll = existAll.GetIntersect(getExistenceList(info.Id));
                }
            }
            // 各x座標に対応する時間に対応するフレームのインデックスを求める
            // clipIndices[0] から (clipIndices[1] - 1)には，[x=0に対応する時刻]から[x=1に対応する時刻の直前]までに含まれるインデックスが入るようにする
            for(int x = 0; x <= _width; x++) {
                decimal time = positionToTime(x);
                int index = dataSet.GetFrameIndexAt(time);
                // ちょうどtimeの値がフレームの時間と一緒のときだけ特別対応
                Motion.MotionFrame frame = dataSet.GetFrameByIndex(index);
                if(frame == null || frame.Time != time)
                    index++; // 取得されたインデックスは，今回のループのx座標に対応する時間範囲に入らない
                clipIndices.Add(index);
            }
            lock(_lockAccessGraphImage) {
                // グラフの作成
                _graphImage = new Bitmap(_width, _height);
                using(Graphics gfx = Graphics.FromImage(_graphImage)) {
                    gfx.Clear(Color.Black);

                    for(int x = 0; x < _width; x++) {
                        // 今回のx座標に対応するフレームインデックスの範囲
                        int clipCount = clipIndices[x + 1] - clipIndices[x];
                        RangeSet<int>.Range clipRange = new RangeSet<int>.Range(clipIndices[x], clipIndices[x + 1]);
                        // 今回の範囲の被欠損情報を取得
                        RangeSet<int> existClipped = existAll.GetClipped(clipRange);
                        int existCount = existClipped.Total();

                        Pen pen = Pens.LightGreen;
                        if(existCount == clipCount)
                            pen = Pens.YellowGreen;
                        if(existCount > 0)
                            gfx.DrawLine(pen, new Point(x, _height - 1), new Point(x, _height - 1 - _height * existCount / clipCount));
                    }
                    // 選択範囲の欠損割合の計算
                    decimal selectBegin = timeController.VisibleBeginTime;
                    decimal selectEnd = timeController.VisibleEndTime;
                    int selectBeginIndex = dataSet.GetFrameIndexAt(selectBegin);
                    int selectEndIndex = dataSet.GetFrameIndexAt(selectEnd);
                    MotionFrame selectBeginFrame = dataSet.GetFrameByIndex(selectBeginIndex);
                    MotionFrame selectEndFrame = dataSet.GetFrameByIndex(selectEndIndex);
                    if(selectBeginFrame == null || selectBeginFrame.Time != selectBegin)
                        selectBeginIndex++; // clipIndicesを求めるときと同じ処理
                    if(selectEndFrame == null || selectEndFrame.Time != selectEnd)
                        selectEndIndex++;
                    RangeSet<int> whole = existAll.GetClipped(new RangeSet<int>.Range(selectBeginIndex, selectEndIndex));
                    int selectClipCount = selectEndIndex - selectBeginIndex;
                    int selectExistCount = whole.Total();
                    int missingCount = selectClipCount - selectExistCount;
                    double percentage = selectClipCount == 0 ? 0 : (100.0 - 100.0 * selectExistCount / selectClipCount);
                    setText(labelInfo, string.Format("欠落フレーム数: {0} (選択フレーム数 {1}, {2}% 欠損) ({3} オブジェクト)", missingCount, selectClipCount, Math.Round(percentage, 2), infoList.Count));
                }
            }
        }

        private void bgwRender_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if(!e.Cancelled)
                DoPaint();
            if(_renderRequested) {
                // 描画中にさらに他の描画要求が来ていたらもう一度描画
                render();
            }
        }

        private void pictGraph_MouseDown(object sender, MouseEventArgs e) {
            TimeController timeController = _timeController;
            if(timeController == null)
                return;
            decimal cursor = positionToTime(e.X);
            timeController.CurrentTime = cursor;
            if(e.Button == MouseButtons.Left) {
                cursor = Math.Min(timeController.VisibleEndTime, Math.Max(timeController.VisibleBeginTime, cursor));
            }
        }

        private void pictGraph_MouseMove(object sender, MouseEventArgs e) {
            TimeController timeController = _timeController;
            if(timeController == null)
                return;
            decimal cursor = positionToTime(e.X);
            if((e.Button & MouseButtons.Left) != 0) {
                timeController.CurrentTime = cursor;
            }
            timeController.CursorTime = cursor;
        }

        private void pictGraph_MouseLeave(object sender, EventArgs e) {
            TimeController timeController = _timeController;
            if(timeController == null)
                return;
            timeController.CursorTime = null;
        }

    }
}
