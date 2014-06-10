using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Xml;
using System.Threading;
using System.IO;

//using Microsoft.DirectX.AudioVideoPlayback;
namespace MotionDataHandler.Motion {
    using Misc;

    public partial class MotionDataViewer : UserControl {
        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        public MotionDataViewer() {
            InitializeComponent();
            this.BackColor = Properties.Settings.Default.Motion_DefaultBackgroundColor;
            keepVerticalAxisToolStripMenuItem.Checked = KeepVerticalAxis = true;
            transparentToolStripMenuItem.Checked = true;

            this.ViewCamera = new DxCamera(this.Width, this.Height);

            this.ViewCamera.SetTarget(new Vector3(0, 1000, 0), true);
            this.ViewCamera.SetSightLine(new Vector3(3000, 0, 0), true);
            this.ViewCamera.UpVector = new Vector3(0, 1, 0);
            this.ViewCamera.ZnearPlane = 100f;
            this.ViewCamera.ZfarPlane = 20000f;

            this.Disposed += new EventHandler(MotionDataViewer_Disposed);

        }

        #region ロック用オブジェクト
        private readonly object _lockPreview = new object();
        private readonly object _lockAttachDataSet = new object();
        private readonly object _lockAttachTimeController = new object();
        private readonly object _lockAccessDxDevice = new object();
        private readonly object _lockFloor = new object();
        private readonly object _lockResize = new object();
        #endregion

        #region 描画用継続変数
        /// <summary>
        /// DirectXデバイス
        /// </summary>
        Device _dxDevice;
        /// <summary>
        /// データセット描画用の頂点バッファ
        /// </summary>
        VertexBuffer _dataSetVertices = null;
        /// <summary>
        /// 文字列描画用のスプライト
        /// </summary>
        Sprite _fontSprite;
        /// <summary>
        /// 描画用のフォント
        /// </summary>
        Microsoft.DirectX.Direct3D.Font _dxFont;
        /// <summary>
        /// 描画用のカメラ情報を取得または設定します．
        /// </summary>
        public DxCamera ViewCamera;
        /// <summary>
        /// 床面描画用の頂点バッファ
        /// </summary>
        VertexBuffer _floorVertices;
        /// <summary>
        /// 床面のマテリアル
        /// </summary>
        Material _floorMaterial;
        /// <summary>
        /// 図形描画用のスプライト
        /// </summary>
        Sprite _drawSprite;
        /// <summary>
        /// 白線を引く用のテクスチャ
        /// </summary>
        Texture _textureWhite;
        /// <summary>
        /// Deviceの描画領域を拡大することで疑似的にアンチエイリアスをかけるための係数
        /// </summary>
        private int _renderScale = 1;
        /// <summary>
        /// 描画用IDisposableオブジェクトの廃棄管理用リスト
        /// </summary>
        readonly List<IDisposable> _disposeTable = new List<IDisposable>();

        /// <summary>
        /// 床面描画時の一ブロックのサイズ
        /// </summary>
        float _floorRectLength = 500f;
        /// <summary>
        /// 床面描画時のブロック間の間隔
        /// </summary>
        float _floorRectGap = 5f;
        /// <summary>
        /// 空間情報を保持するオブジェクト
        /// </summary>
        MotionFieldState _prevFieldState;
        /// <summary>
        /// 床面の色
        /// </summary>
        Color _floorColor = Color.FromArgb(255, 96, 96, 96);

        #endregion

        #region MotionDataSet関連
        /// <summary>
        /// 関連付けられたデータセット
        /// </summary>
        MotionDataSet _dataSet = null;
        /// <summary>
        /// プレビュー用オブジェクト
        /// </summary>
        readonly List<KeyValuePair<MotionObject, bool>> _previewObjects = new List<KeyValuePair<MotionObject, bool>>();
        /// <summary>
        /// 削除プレビュー用オブジェクト
        /// </summary>
        readonly List<MotionObjectInfo> _previewRemovedObjects = new List<MotionObjectInfo>();
        #endregion

        #region イベント用継続変数
        /// <summary>
        /// 時間管理オブジェクト
        /// </summary>
        TimeController _timeController;

        /// <summary>
        /// マウス操作を標準化する用のオブジェクト
        /// </summary>
        readonly RegulatedMouseControl _mouse = new RegulatedMouseControl(4, RegulatedMouseButton.CtrlLeft, RegulatedMouseButton.Right, RegulatedMouseButton.AltLeft);
        #endregion

        #region 一時変数
        /// <summary>
        /// オブジェクトにマウスオーバーしているときのオブジェクトID
        /// </summary>
        uint? _mouseOverId = null;
        /// <summary>
        /// カメラが線分を追跡する際の，オブジェクトID
        /// </summary>
        uint? _traceLineId = null;
        /// <summary>
        /// デバイスが復元できない状態に陥ったかどうか
        /// </summary>
        bool _deviceCannotRestore = false;
        /// <summary>
        /// デバイスが復元されたかどうか
        /// </summary>
        bool _resetDone = false;
        /// <summary>
        /// 操作用ヒントを表示するかどうか
        /// </summary>
        bool _showKeyboardHint = false;
        /// <summary>
        /// 描画時の前回の時刻
        /// </summary>
        long _prevRenderTick = DateTime.Now.Ticks;
        /// <summary>
        /// 描画時の経過時間
        /// </summary>
        float _elapse = 0f;
        /// <summary>
        /// 描画時の漸近係数
        /// </summary>
        float _asymptotic = 0f;
        /// <summary>
        /// カメラが線分を追跡する際の，最後にオブジェクトが存在したフレームを描画した時刻
        /// </summary>
        long _lastTraceLineExistsTicks = 0;
        /// <summary>
        /// カメラが線分を追跡する際の，前回の線分の開始位置
        /// </summary>
        Vector3 _lastTraceLinePosition = Vector3.Empty;
        /// <summary>
        /// カメラが線分を追跡する際の，前回の線分の終了位置
        /// </summary>
        Vector3 _lastTraceLineAnotherEnd = Vector3.Empty;
        /// <summary>
        /// 確保した頂点バッファのサイズ
        /// </summary>
        int _vertexNums = -1;
        /// <summary>
        /// 画面の描画が要求されたかどうか
        /// </summary>
        bool _requestRender = false;
        /// <summary>
        /// 画面サイズの変更時にデバイスのサイズを変更する様要求されたかどうか
        /// </summary>
        bool _requestAutoResetOnResize = false;
        /// <summary>
        /// 画面サイズの変更に伴い現在デバイスのサイズを変更しているかどうか
        /// </summary>
        bool _autoResettingOnResize = false;
        /// <summary>
        /// オブジェクトの描画時に面を持つオブジェクトをデフォルトで半透明表示するかどうか
        /// </summary>
        bool _transparentEnabled = false;
        /// <summary>
        /// デバッグモード用状態変数
        /// </summary>
        int _debug = 0;
        /// <summary>
        /// デバッグモードであるか否か
        /// </summary>
        bool _debugMode = false;
        #endregion

        #region 外部公開プロパティ
        private bool _canChangeSelection = true;
        /// <summary>
        /// クリック時のオブジェクトの選択を有効にするかを取得または設定します。
        /// </summary>
        public bool CanChangeSelection { get { return _canChangeSelection; } set { _canChangeSelection = value; } }

        private bool _keepVerticalAxis = false;
        /// <summary>
        /// カメラの上方ベクトルを固定するかを取得または設定します．
        /// </summary>
        public bool KeepVerticalAxis {
            get { return _keepVerticalAxis; }
            set {
                _keepVerticalAxis = value;
                MotionDataSet dataSet = _dataSet;
                if(ViewCamera != null && dataSet != null) {
                    ViewCamera.UpVector = dataSet.FieldState.FloorUpper;
                }
            }
        }
        #endregion

        #region Events

        private void bgRender_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if(_requestRender) {
                requestRenderInternal();
            }
        }

        private void timerRender_Tick(object sender, EventArgs e) {
            requestRenderInternal();
            if(timerRender.Interval < 1000) {
                timerRender.Interval += 10;
                if(timerRender.Interval > 1000) {
                    timerRender.Interval = 1000;
                }
            }
        }

        private void menuTraceLine_Click(object sender, EventArgs e) {
            MotionDataSet dataSet = _dataSet;
            if(dataSet == null)
                return;
            if(menuTraceLine.Checked || _traceLineId.HasValue) {
                _traceLineId = null;
                menuTraceLine.Checked = false;
                return;
            }
            this.EnableTraceSelectedObjects(false);
            var infoList = dataSet.GetSelectedObjectInfoList(typeof(LineObject));
            if(infoList.Count != 1) {
                MessageBox.Show("線分を一つ選択してください");
                return;
            }
            _traceLineId = infoList[0].Id;
            menuTraceLine.Checked = true;
        }

        private void stopTimerToolStripMenuItem_Click(object sender, EventArgs e) {
            if(timerRender.Enabled) {
                timerRender.Stop();
                stopTimerToolStripMenuItem.Checked = true;
            } else {
                timerRender.Start();
                stopTimerToolStripMenuItem.Checked = false;
            }
        }

        private void MotionDataViewer_Resize(object sender, EventArgs e) {
            if(!_autoResettingOnResize) {
                lock(_lockResize) {
                    _requestAutoResetOnResize = true;
                    if(!bgwWaitResize.IsBusy)
                        bgwWaitResize.RunWorkerAsync();
                }
            }
        }

        private void bgwWaitResize_DoWork(object sender, DoWorkEventArgs e) {
            _requestAutoResetOnResize = false;
            Thread.Sleep(1000);
        }

        private void bgwWaitResize_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            lock(_lockResize) {
                if(_requestAutoResetOnResize) {
                    if(!bgwWaitResize.IsBusy)
                        bgwWaitResize.RunWorkerAsync();
                    return;
                }
                MotionDataSet dataSet = _dataSet;
                if(dataSet != null) {
                    _autoResettingOnResize = true;
                    try {
                        this.disposeDirect3D();
                        this.MaximumSize = new Size();
                        this.MinimumSize = new Size();
                        this.Dock = DockStyle.None;
                        this.Dock = DockStyle.Fill;
                        this.initializeDirect3D();
                    } finally {
                        _autoResettingOnResize = false;
                    }
                }
            }
        }

        private void MotionDataViewer_MouseLeave(object sender, EventArgs e) {
            _mouseOverId = null;
        }
        private void MotionDataViewer_KeyDown(object sender, KeyEventArgs e) {
            switch(e.KeyData) {
            case Keys.H:
                _showKeyboardHint = !_showKeyboardHint;
                break;
            case Keys.D:
                _debug = 1;
                break;
            case Keys.E:
                _debug = _debug == 1 ? 2 : 0;
                break;
            case Keys.B:
                _debug = _debug == 2 ? 3 : 0;
                break;
            case Keys.U:
                _debug = _debug == 3 ? 4 : 0;
                break;
            case Keys.G:
                _debug = _debug == 4 ? 5 : 0;
                break;
            case Keys.Escape:
                if(_debug == 5)
                    _debugMode = true;
                break;
            }
        }

        private void transparentToolStripMenuItem_CheckedChanged(object sender, EventArgs e) {
            global::MotionDataHandler.Properties.Settings.Default.Motion_TransparentChecked = transparentToolStripMenuItem.Checked;
        }

        void MotionDataViewer_Disposed(object sender, EventArgs e) {
            disposeDirect3D();
            this.DetachDataSet();
            this.AttachTimeController(null);
        }

        private void MotionDataViewer_MouseMove(object sender, MouseEventArgs e) {
            MotionFieldState fieldState;

            MotionDataSet dataSet = _dataSet;
            if(dataSet == null)
                return;
            fieldState = dataSet.FieldState;

            RegulatedMouseInfo state = _mouse.MouseMove(e);
            Vector3 updir = this.KeepVerticalAxis ? fieldState.FloorUpper : this.ViewCamera.UpVector;
            Matrix rotX = Matrix.RotationAxis(updir, -(float)(Math.PI * _mouse.MoveDelta.X / this.ViewCamera.ViewLongEdge));
            Matrix rotY = Matrix.RotationAxis(this.ViewCamera.RightDirection(), -(float)(Math.PI * _mouse.MoveDelta.Y / ViewCamera.ViewLongEdge));
            switch(state.Button) {
            case RegulatedMouseButton.Left:
            case RegulatedMouseButton.Right:
                // 回転
                bool keepTarget = (state.Button == RegulatedMouseButton.Left);
                if(this.KeepVerticalAxis) {
                    // 向きが真下や真上を通りすぎるとカメラが反転してうっとうしいので
                    // 通り過ぎる場合は縦回転しないようする

                    Vector3 prevSight = this.ViewCamera.SightDirection();
                    Vector3 nextSight = prevSight;
                    nextSight.TransformNormal(rotY);
                    Vector3 prevRight = Vector3.Cross(prevSight, fieldState.FloorUpper);
                    Vector3 nextRight = Vector3.Cross(nextSight, fieldState.FloorUpper);
                    // 通り過ぎる場合は右ベクトルが前と後で逆を向く
                    if(Vector3.Dot(prevRight, nextRight) < -0.0f) {
                        this.ViewCamera.TransformSightLine(rotX, keepTarget);
                    } else {
                        this.ViewCamera.TransformSightLine(rotX * rotY, keepTarget);
                    }
                    this.ViewCamera.UpVector = fieldState.FloorUpper;
                } else {
                    this.ViewCamera.TransformSightLine(rotX * rotY, keepTarget);
                }
                break;
            case RegulatedMouseButton.CtrlLeft:
                // 平行移動
                Vector3 diff = updir * _mouse.MoveDelta.Y - this.ViewCamera.RightDirection() * _mouse.MoveDelta.X;
                this.ViewCamera.Offset(diff * (this.ViewCamera.SightLine.Length() / this.ViewCamera.ViewLongEdge));
                break;
            case RegulatedMouseButton.AltLeft:
                // 拡大
                this.ViewCamera.SetTarget(ViewCamera.TargetPosition - ViewCamera.SightLine * 0.005f * _mouse.MoveDelta.Y, false);
                this.ViewCamera.ScaleSightLine(1.0f + 0.005f * _mouse.MoveDelta.X, true);

                break;
            }

            Vector2 clickPos = calcClickPos(e.Location);
            _mouseOverId = testCursorRay(clickPos);
            this.RequestRender();
        }

        private void MotionDataViewer_MouseWheel(object sender, MouseEventArgs e) {
            if(e.Delta != 0) {
                if((Control.ModifierKeys & Keys.Control) != 0) {
                    this.ViewCamera.SetTarget(ViewCamera.TargetPosition - ViewCamera.SightLine * (e.Delta * 0.0005f), false);
                } else {
                    this.ViewCamera.ScaleSightLine(1.0f + 0.0005f * e.Delta, true);
                }

                RequestRender();
            }
        }


        private void MotionDataViewer_MouseDown(object sender, MouseEventArgs e) {
            _mouse.MouseDown(e);
        }

        private void MotionDataViewer_MouseUp(object sender, MouseEventArgs e) {
            RegulatedMouseInfo state = _mouse.MouseUp(e);
            switch(state.Button) {
            case RegulatedMouseButton.Left:
                if(state.State == RegulatedMouseClickState.Click) {
                    selectOnClick(e.Location, false);
                }
                break;
            case RegulatedMouseButton.CtrlLeft:
                if(state.State == RegulatedMouseClickState.Click) {
                    selectOnClick(e.Location, true);
                }
                break;
            case RegulatedMouseButton.Right:
                if(state.State == RegulatedMouseClickState.Click) {
                    if(this.CanChangeSelection) {
                        contextRightClick.Show(System.Windows.Forms.Control.MousePosition);
                    }
                }
                break;
            case RegulatedMouseButton.AltLeft:
                break;
            }
        }

        private void keepVerticalAxisToolStripMenuItem_Click(object sender, EventArgs e) {
            this.KeepVerticalAxis = !this.KeepVerticalAxis;
            keepVerticalAxisToolStripMenuItem.Checked = this.KeepVerticalAxis;
        }


        private void doTopMostChanged() {
            if(this.ParentForm != null) {
                if(this.ParentForm.InvokeRequired) {
                    this.Invoke(new Action(doTopMostChanged));
                    return;
                }
                if(topMostToolStripMenuItem.Checked) {
                    this.ParentForm.TopMost = true;
                } else {
                    this.ParentForm.TopMost = false;
                }
            }
        }


        private void topMostToolStripMenuItem_Click(object sender, EventArgs e) {
            topMostToolStripMenuItem.Checked = !topMostToolStripMenuItem.Checked;
            doTopMostChanged();
        }

        private void saveCameraToolStripMenuItem_Click(object sender, EventArgs e) {
            if(dialogSaveCamera.ShowDialog() == DialogResult.OK) {
                using(FileStream stream = new FileStream(dialogSaveCamera.FileName, FileMode.Create)) {
                    this.ViewCamera.Serialize(stream);
                }
            }
        }

        private void loadCameraToolStripMenuItem_Click(object sender, EventArgs e) {
            if(dialogOpenCamera.ShowDialog() == DialogResult.OK) {
                using(FileStream stream = new FileStream(dialogOpenCamera.FileName, FileMode.Open)) {
                    this.ViewCamera = DxCamera.Deserialize(stream);
                    this.ViewCamera.SetViewSize(this.Size);
                    dialogSaveCamera.FileName = dialogOpenCamera.FileName;
                    dialogOpenCamera.InitialDirectory = dialogOpenCamera.InitialDirectory;
                }
            }
        }

        private void MotionDataViewer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            TimeController timeController = _timeController;
            switch(e.KeyCode) {
            case Keys.Right:
                if(timeController != null) {
                    if(timeController.CurrentIndex < timeController.IndexCount - 1)
                        timeController.CurrentIndex++;
                }
                break;
            case Keys.Left:
                if(timeController != null) {
                    if(timeController.CurrentIndex > 0)
                        timeController.CurrentIndex--;
                }
                break;
            }
        }

        private void traceSelectedToolStripMenuItem_Click(object sender, EventArgs e) {
            traceSelectedToolStripMenuItem.Checked = !traceSelectedToolStripMenuItem.Checked;
        }

        private void MotionDataViewer_MouseEnter(object sender, EventArgs e) {
            try {
                if(!_resetDone && this.Enabled && _dxDevice != null) {
                    initializeDirect3D();
                }
                _dataSet_ObjectChanged(sender, e);
            } catch(Exception) {
                disposeDirect3D();
            }
        }

        private void transparentToolStripMenuItem_Click(object sender, EventArgs e) {
            _transparentEnabled = transparentToolStripMenuItem.Checked = !transparentToolStripMenuItem.Checked;
        }

        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e) {
            if(dialogBackgroundColor.ShowDialog() == DialogResult.OK) {
                global::MotionDataHandler.Properties.Settings.Default.Motion_DefaultBackgroundColor = this.BackColor = dialogBackgroundColor.Color;
            }
        }

        private void MotionDataViewer_Load(object sender, EventArgs e) {
            _transparentEnabled = transparentToolStripMenuItem.Checked;
            this.ParentForm.FormClosed += new FormClosedEventHandler(ParentForm_FormClosed);

            dialogBackgroundColor.Color = this.BackColor;
            timerRender.Start();
        }

        void ParentForm_FormClosed(object sender, FormClosedEventArgs e) {
            disposeDirect3D();
            this.DetachDataSet();
        }

        private void bgRender_DoWork(object sender, DoWorkEventArgs e) {
            _requestRender = false;
            lock(_lockAccessDxDevice) {
                if(_dxDevice == null)
                    return;
                MotionDataSet dataSet = _dataSet;
                if(dataSet == null)
                    return;
                TimeController timeController = _timeController;
                if(timeController == null)
                    return;
                // 時間の処理
                long nowTicks = DateTime.Now.Ticks;
                _elapse = (float)(nowTicks - _prevRenderTick) / 10000000F;
                _asymptotic = 1f - (float)Math.Pow(0.75, _elapse / 0.05F);
                _prevRenderTick = nowTicks;
                // 選択オブジェクトの追跡
                if(traceSelectedToolStripMenuItem.Checked) {
                    MotionFrame frame = dataSet.GetFrameAt(timeController.CurrentTime);
                    if(frame != null) {
                        Vector3 sumGravityPoint = Vector3.Empty;
                        var selectedInfoList = dataSet.GetSelectedObjectInfoList();
                        int existsCount = 0;
                        foreach(var info in selectedInfoList) {
                            if(frame[info] != null) {
                                sumGravityPoint += frame[info].GravityPoint;
                                existsCount++;
                            }
                        }
                        if(existsCount > 0) {
                            Vector3 avgGravityPoint = sumGravityPoint * (1f / existsCount);
                            this.ViewCamera.SetTarget(Vector3.Lerp(this.ViewCamera.TargetPosition, avgGravityPoint, _asymptotic), false);
                        }
                    }
                } else if(_traceLineId.HasValue) {
                    // 選択線分の追跡
                    MotionFrame frame = dataSet.GetFrameAt(timeController.CurrentTime);
                    if(frame != null && _traceLineId.HasValue) {
                        LineObject line = (LineObject)frame[_traceLineId.Value];
                        if(line != null && line.Edge != Vector3.Empty) {
                            _lastTraceLineExistsTicks = _prevRenderTick;
                            _lastTraceLineAnotherEnd = line.AnotherEnd;
                            _lastTraceLinePosition = line.Position;
                        }
                        if(_prevRenderTick < _lastTraceLineExistsTicks + 10000000) {
                            this.ViewCamera.SetPosition(Vector3.Lerp(ViewCamera.Position, _lastTraceLinePosition, _asymptotic), true);
                            this.ViewCamera.SetTarget(Vector3.Lerp(ViewCamera.TargetPosition, _lastTraceLineAnotherEnd, _asymptotic), true);
                            this.ViewCamera.UpVector = dataSet.FieldState.FloorUpper;
                        }
                    }
                }
                // カメラがバグった時用
                if(!VectorEx.IsFinite(ViewCamera.Position))
                    ViewCamera.Position = Vector3.Empty;
                if(!VectorEx.IsFinite(ViewCamera.TargetPosition))
                    ViewCamera.TargetPosition = Vector3.Empty;

                // DirectXによるレンダリング
                try {
                    if(!_resetDone) {
                        onDeviceLostException();
                        return;
                    }
                    // 背景クリア
                    _dxDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, this.BackColor, 1.0f, 0);
                    // 光源の位置設定
                    _dxDevice.Lights[1].Direction = -dataSet.FieldState.FloorUpper;
                    _dxDevice.Lights[1].Update();
                    _dxDevice.Lights[0].Position = ViewCamera.Position;
                    _dxDevice.Lights[0].Range = ViewCamera.ZfarPlane;
                    _dxDevice.Lights[0].Update();
                    // 描画開始
                    _dxDevice.BeginScene();
                    // カメラの設定
                    setupMatrices();
                    // オブジェクトの描画
                    renderFloor();
                    renderDataSet();
                    // メッセージの描画
                    try {
                        if(_timeController == null)
                            return;
                        MotionFrame frame = dataSet.GetFrameAt(timeController.CurrentTime);
                        decimal time = 0;
                        if(frame != null) {
                            time = frame.Time;
                        }
                        int frameIndex = dataSet.GetFrameIndexAt(timeController.CurrentTime);
                        int lineSpace = 15 * _renderScale;
                        _fontSprite.Begin(SpriteFlags.AlphaBlend);
                        try {
                            if(_mouseOverId.HasValue) {
                                // マウスオーバー時は名前を表示
                                MotionObjectInfo info = dataSet.GetObjectInfoById(_mouseOverId.Value);
                                if(info != null) {
                                    int baseX = _mouse.Location.X + 12;
                                    Rectangle rect = _dxFont.MeasureString(_fontSprite, info.Name, DrawTextFormat.NoClip, Color.LightGreen);

                                    _drawSprite.Begin(SpriteFlags.AlphaBlend);
                                    //drawLineWithSplite(_drawSprite, new Vector2(baseX - rect.Height / 4, _mouse.Location.Y), new Vector2(baseX + rect.Width + rect.Height, _mouse.Location.Y), Color.GreenYellow, false);
                                    MotionObject @object = frame[info];
                                    if(@object != null) {
                                        Vector3 gravityPoint = @object.GravityPoint;
                                        Vector3 centerPoint = ViewCamera.Project(gravityPoint, dataSet.FieldState.LeftHanded);
                                        drawLineWithSplite(_drawSprite, new Vector2(baseX - rect.Height / 4, _mouse.Location.Y), new Vector2(centerPoint.X, centerPoint.Y), Color.GreenYellow, false);
                                    }

                                    drawStringWithRectangle(_dxFont, info.Name, new Point(baseX, _mouse.Location.Y - rect.Height - 2), Color.White, Color.FromArgb(196, Color.DarkGreen), 2);
                                    _drawSprite.End();

                                    if(_debugMode) {
                                        float minDistance, maxDistance;
                                        if(@object.GetDistanceDirectional(ViewCamera.Position, ViewCamera.SightDirection(), out minDistance, out maxDistance)) {
                                            _dxFont.DrawText(_fontSprite, "Min: " + minDistance.ToString("0.000"), new Rectangle(baseX, _mouse.Location.Y + 2, 0, 0), DrawTextFormat.NoClip, Color.White.ToArgb());
                                            _dxFont.DrawText(_fontSprite, "Max: " + maxDistance.ToString("0.000"), new Rectangle(baseX, _mouse.Location.Y + rect.Height + 2, 0, 0), DrawTextFormat.NoClip, Color.White.ToArgb());
                                        }
                                    }
                                }
                            }
                            int offset = 0;
                            _dxFont.DrawText(_fontSprite, string.Format("Time: {0} / {1}", time.ToString("F3"), timeController.EndTime.ToString("F3")), new Rectangle(0, (offset++) * lineSpace, 0, 0), DrawTextFormat.NoClip, Color.Cyan.ToArgb());
                            _dxFont.DrawText(_fontSprite, string.Format("Frame: {0} / {1}", frameIndex, dataSet.FrameLength), new Rectangle(0, (offset++) * lineSpace, 0, 0), DrawTextFormat.NoClip, Color.Cyan.ToArgb());
                            if(_showKeyboardHint) {
                                string[] hints = new string[] { "Left click: Select an object", "Middle click or Ctrl + Left click: Add/Remove Selection", "Right click: Menu", "Left drag: Move around", "Middle drag or Ctrl + Left Drag: Parallel shift", "Right drag: Turn around", "Mouse wheel or Alt + Left Horizontal Drag : Zoom in/out", "Ctrl + Mouse wheel or Alt + Left Vertical Drag : Go forward/back" };
                                foreach(var hint in hints) {
                                    _dxFont.DrawText(_fontSprite, hint, new Rectangle(0, (offset++) * lineSpace, 0, 0), DrawTextFormat.NoClip, Color.Yellow.ToArgb());
                                }
                            } else {
                                _dxFont.DrawText(_fontSprite, "Hit 'h' to show Hint", new Rectangle(0, (offset++) * lineSpace, 0, 0), DrawTextFormat.NoClip, Color.White.ToArgb());
                            }
                            offset = 1;

                            _dxFont.DrawText(_fontSprite, string.Format("VertexBuffer: {0}", _vertexNums), new Rectangle(0, this.ViewCamera.Viewport.Height - (offset++) * lineSpace - 3, 0, 0), DrawTextFormat.NoClip, Color.FromArgb(128, Color.Cyan).ToArgb());
                            _dxFont.DrawText(_fontSprite, string.Format("Elapse: {0}", _elapse.ToString("F3")), new Rectangle(0, this.ViewCamera.Viewport.Height - (offset++) * lineSpace - 3, 0, 0), DrawTextFormat.NoClip, Color.FromArgb(128, Color.Cyan).ToArgb());
                        } finally {
                            _fontSprite.End();
                        }
                    } catch(AccessViolationException) {
                        // たまにおこる
                    } finally {
                        _dxDevice.EndScene();
                    }
                    _dxDevice.Present();
                } catch(DeviceLostException) {
                    onDeviceLostException();
                } catch(InvalidCallException) {
                    Thread.Sleep(20);
                }
            }
        }

        private void drawStringWithRectangle(Microsoft.DirectX.Direct3D.Font font, string text, Point location, Color foreColor, Color backColor, int distance) {
            Rectangle rect = font.MeasureString(_drawSprite, text, DrawTextFormat.None, foreColor);
            _drawSprite.Draw2D(_textureWhite, new Rectangle(0, 0, 1, 1), new SizeF(rect.Width + distance * 2, rect.Height + distance * 2), new PointF(location.X - distance, location.Y - distance), backColor);
            /*
            // 発光エフェクト
            double distanceSq = distance * distance;
            for(int i = -distance; i <= distance; i++) {
                double iSq = i * i;
                for(int j = -distance; j <= distance; j++) {
                    double dSq = iSq + j * j;
                    if(dSq >= distanceSq)
                        continue;
                    double ratio = 1.0 - Math.Sqrt(Math.Sqrt(dSq) / distance);
                    int a = (int)(ratio * backColor.A);
                    a = Math.Min(Math.Max(a, 0), 255);
                    font.DrawText(_fontSprite, text, new Rectangle(Point.Add(location, new Size(i, j)), Size.Empty), DrawTextFormat.NoClip, Color.FromArgb(a, backColor));
                }
            }
            */
            font.DrawText(_fontSprite, text, new Rectangle(location, Size.Empty), DrawTextFormat.NoClip, foreColor);
        }


        #endregion

        #region DataHandle

        public void DetachDataSet() {
            this.AttachDataSet(null);
        }

        /// <summary>
        /// オブジェクトにデータセットを関連付けます．
        /// </summary>
        /// <param name="dataSet"></param>
        public void AttachDataSet(MotionDataSet dataSet) {
            lock(_lockAttachDataSet) {
                if(_dataSet != null) {
                    _dataSet.ObjectInfoSetChanged -= _dataSet_ObjectChanged;
                    _dataSet.ObjectSelectionChanged -= _dataSet_ObjectChanged;
                }
                _dataSet = dataSet;
                if(_dataSet != null) {
                    _dataSet.ObjectInfoSetChanged += _dataSet_ObjectChanged;
                    _dataSet.ObjectSelectionChanged += _dataSet_ObjectChanged;
                }
            }
            RequestRender();
        }

        /// <summary>
        /// オブジェクトにTimeControllerを関連付けます．
        /// </summary>
        /// <param name="timeController"></param>
        public void AttachTimeController(TimeController timeController) {
            lock(_lockAttachTimeController) {
                if(_timeController != null) {
                    _timeController.CurrentTimeChanged -= _timeController_CurrentTimeChanged;                    
                }
                _timeController = timeController;
                if(_timeController != null) {
                    _timeController.CurrentTimeChanged += _timeController_CurrentTimeChanged;
                }
            }
        }

        void _timeController_CurrentTimeChanged(object sender, EventArgs e) {
            if(this.InvokeRequired) {
                this.BeginInvoke(new EventHandler(_timeController_CurrentTimeChanged), sender, e);
                return;
            }
            this.RequestRender();
        }



        #endregion

        #region Graphics

        /// <summary>
        /// 指定された画面上のポイントに対応する空間位置とカメラ位置を結ぶ直線上の最も近いオブジェクトの
        /// MotionDataObjectInfoのMotionDataHeaderにおけるインデックスを返します。        
        /// </summary>
        /// <param title="point">画面上のポイント</param>
        /// <returns>オブジェクトのID。失敗した場合はnull。</returns>
        private uint? testCursorRay(Vector2 location) {
            MotionDataSet dataSet = _dataSet;
            float marginSize = Properties.Settings.Default.Motion_MarginSize;
            if(dataSet == null)
                return null;
            TimeController timeController = _timeController;
            if(timeController == null)
                return null;
            // クリック座標とカメラ位置からベクトルを作成
            Vector3 cameraPos, cameraSight, cameraTarget;
            cameraPos = this.ViewCamera.GetClickedPositionAndDirection(location, dataSet.FieldState.LeftHanded, out cameraSight);
            cameraTarget = cameraPos + cameraSight;
            // データセットからデータをもらう
            var infoList = dataSet.GetObjectInfoList();
            MotionFrame frame = dataSet.GetFrameAt(timeController.CurrentTime);
            if(frame == null)
                return null;

            // 距離を測る用の変数
            Dictionary<uint, float> distances = new Dictionary<uint, float>();
            float minDistance;
            float distance;

            // 面を持たないオブジェクトを先にテストする
            foreach(var info in infoList.Where(info => info.IsVisible)) {
                MotionObject @object = frame[info];
                if(@object == null)
                    continue;
                if(!@object.HasArea()) {
                    if(@object.DetectLineCollision(cameraPos, cameraTarget, marginSize, out distance)) {
                        distances[info.Id] = distance;
                    }
                }
            }
            if(distances.Count > 0) {
                // 一番近いものを選択対象に
                minDistance = distances.Min(d => d.Value);
                return (from d in distances where d.Value == minDistance select d.Key).First();
            }
            // 範囲を広げながら何回かテスト
            for(int i = 0; i < 2; i++) {
                foreach(var info in infoList.Where(info => info.IsVisible)) {
                    MotionObject @object = frame[info];
                    if(@object == null)
                        continue;
                    float enlargement = marginSize * (1 + i);
                    if(!@object.HasArea()) {
                        // 面を持たないオブジェクトは範囲を広げておく
                        enlargement += marginSize;
                    }
                    if(@object.DetectLineCollision(cameraPos, cameraTarget, enlargement, out distance)) {
                        distances[info.Id] = distance;
                    }
                }
                if(distances.Count > 0) {
                    // 一番近いものを選択対象に
                    minDistance = distances.Min(d => d.Value);
                    return (from d in distances where d.Value == minDistance select d.Key).First();
                }
            }
            return null;
        }

        /// <summary>
        /// 実際の表示の上のスクリーン座標からDevice内のスクリーン座標に変換する
        /// </summary>
        /// <param name="pos">マウスのカーソルの位置</param>
        /// <returns></returns>
        private Vector2 calcClickPos(Point pos) {
            Vector2 clickPos = new Vector2(pos.X, pos.Y);
            if(this.Size != ViewCamera.ViewRectangle.Size) {
                if(this.Width <= 0 || this.Height <= 0)
                    return Vector2.Empty;
                clickPos = new Vector2(clickPos.X * this.ViewCamera.ViewRectangle.Width / this.Width, clickPos.Y * this.ViewCamera.ViewRectangle.Height / this.Height);
            }
            return clickPos;
        }

        /// <summary>
        /// クリック個所に応じてオブジェクトの選択を行います．
        /// </summary>
        /// <param name="position">クリック座標</param>
        /// <param name="ctrl">コントロールキーの押下</param>
        /// <returns></returns>
        private bool selectOnClick(Point position, bool ctrl) {
            if(!this.CanChangeSelection)
                return false;
            if(!this.Enabled)
                return false;
            lock(_lockAccessDxDevice) {
                if(_dxDevice == null)
                    return false;
                MotionDataSet dataSet = _dataSet;
                if(dataSet == null)
                    return false;
                Vector2 clickPos = calcClickPos(position);

                uint? selectId = testCursorRay(clickPos);
                var currentSelects = dataSet.GetSelectedObjectInfoList();
                if(ctrl) {
                    // コントロールの時
                    if(selectId.HasValue) {
                        dataSet.SelectObjects(!dataSet.IsSelecting(selectId.Value), selectId.Value);
                    }
                } else {
                    // 一つしか選択してないとき
                    // コントロールの時
                    if(selectId.HasValue) {
                        dataSet.SelectObjects(false);
                        dataSet.SelectObjects(true, selectId.Value);
                    } else {
                        dataSet.SelectObjects(false);
                    }
                }

                dataSet.DoObjectSelectionChanged();
                return dataSet.GetSelectedObjectInfoList().Count > 0;
            }
        }





        /// <summary>
        /// DirectXのオブジェクト及びDeviceをDisposeします．
        /// </summary>
        private void disposeDirect3D() {
            lock(_lockAccessDxDevice) {
                lock(_disposeTable) {
                    if(_dxDevice != null) {
                        disposeDirect3DExceptDevice();
                        while(_disposeTable.Count > 0) {
                            int index = _disposeTable.Count - 1;
                            disposeFromTable(_disposeTable[index]);
                        }
                        if(!_dxDevice.Disposed) {
                            _dxDevice.Dispose();
                        }
                        _dxDevice = null;
                        _resetDone = false;
                    }
                }
            }
        }

        /// <summary>
        /// DirectXのDevice以外のオブジェクトをDisposeします．
        /// </summary>
        protected void disposeDirect3DExceptDevice() {
            lock(_disposeTable) {
                List<IDisposable> tmp = new List<IDisposable>(_disposeTable);
                foreach(IDisposable obj in tmp) {
                    if(!(obj is Device)) {
                        disposeFromTable(obj);
                    }
                }
                _fontSprite = null;
                _drawSprite = null;
                _textureWhite = null;
                _dataSetVertices = null;
                _floorVertices = null;
                _dxFont = null;
            }
        }
        /// <summary>
        /// オブジェクトをDisposeし，_disposeTableから取り除きます．
        /// </summary>
        /// <param name="obj"></param>
        void disposeFromTable(IDisposable obj) {
            lock(_disposeTable) {
                obj.Dispose();
                _disposeTable.RemoveAll(x => object.ReferenceEquals(obj, x));
            }
        }

        private void initializeDirect3D() {
            if(_deviceCannotRestore)
                return;
            lock(_lockAccessDxDevice) {
                disposeDirect3D();

                Point l = this.Location;
                Size s = this.Size;
                DockStyle d = this.Dock;
                this.Dock = DockStyle.None;
                this.Location = l;
                this.Size = new Size(s.Width * _renderScale, s.Height * _renderScale);


                PresentParameters presentParameters = new PresentParameters();
                presentParameters.DeviceWindow = this;
                presentParameters.Windowed = true;
                presentParameters.SwapEffect = SwapEffect.Discard;
                presentParameters.AutoDepthStencilFormat = DepthFormat.D16;
                presentParameters.EnableAutoDepthStencil = true;
                // これのおかげでリサイズしても落ちない
                // イベントハンドラを使っていると リサイズ->自前リセット->古いオブジェクト破棄->内部リセット->古いオブジェクト参照 となって落ちる
                Device.IsUsingEventHandlers = false;

                try {
                    _dxDevice = new Device(0, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing | CreateFlags.MultiThreaded, presentParameters);
                } catch(InvalidCallException) {
                    try {
                        _dxDevice = new Device(0, DeviceType.Reference, this, CreateFlags.SoftwareVertexProcessing | CreateFlags.MultiThreaded, presentParameters);
                    } catch(InvalidCallException) {
                        throw new InvalidCallException("Cannot initialize Direct3D Device");
                    }
                }

                _disposeTable.Add(_dxDevice);

                onResetDevice(_dxDevice, null);
                this.Dock = d;
            }
        }

        private void onResetDevice(object sender, EventArgs e) {
            if(_deviceCannotRestore)
                return;
            lock(_lockAccessDxDevice) {
                disposeDirect3DExceptDevice();

                _dxDevice.RenderState.AlphaBlendEnable = true;
                _dxDevice.RenderState.SourceBlend = Blend.SourceAlpha;
                _dxDevice.RenderState.DestinationBlend = Blend.InvSourceAlpha;
                _dxDevice.RenderState.ZBufferEnable = true;
                _dxDevice.RenderState.CullMode = Cull.CounterClockwise;

                _fontSprite = new Sprite(_dxDevice);
                _disposeTable.Add(_fontSprite);
                FontDescription desc;
                desc.FaceName = "Tahoma";
                desc.Height = 14 * _renderScale;
                desc.CharSet = CharacterSet.ShiftJIS;
                _dxFont = new Microsoft.DirectX.Direct3D.Font(_dxDevice, desc);
                _disposeTable.Add(_dxFont);

                _disposeTable.Add(_drawSprite = new Sprite(_dxDevice));
                _disposeTable.Add(_textureWhite = new Texture(_dxDevice, Properties.Resources.white, Usage.None, Pool.Managed));

                ViewCamera.SetViewSize(this.Size);
                ViewCamera.FieldOfView = (float)Math.PI / 3;
                _dxDevice.Viewport = ViewCamera.Viewport;

                _dxDevice.Lights[1].Ambient = Color.White;
                _dxDevice.Lights[1].Type = LightType.Directional;
                _dxDevice.Lights[1].Diffuse = Color.White;
                _dxDevice.Lights[1].Specular = Color.White;
                _dxDevice.Lights[1].Enabled = true;

                _dxDevice.Lights[0].Ambient = Color.White;
                _dxDevice.Lights[0].Diffuse = Color.White;
                _dxDevice.Lights[0].Enabled = true;
                _dxDevice.Lights[0].Specular = Color.White;

                _dxDevice.Lights[0].Attenuation0 = 1f;
                _dxDevice.Lights[0].Attenuation1 = 0.0001f;
                _dxDevice.Lights[0].Type = LightType.Point;

                _resetDone = true;
            }
        }


        private bool setupVertexBuffer(IList<PolygonDatum> renderer) {
            lock(_lockAccessDxDevice) {
                if(_dxDevice == null)
                    return false;
                int vertexNum = renderer.Sum(i => i.Vertices.Length);
                if(_dataSetVertices != null && vertexNum <= _vertexNums)
                    return true;
                if(_dataSetVertices != null) {
                    lock(_disposeTable) {
                        disposeFromTable(_dataSetVertices);
                        _dataSetVertices = null;
                    }
                }
                _dataSetVertices = new VertexBuffer(typeof(CustomVertex.PositionNormalColored), vertexNum, _dxDevice, Usage.WriteOnly, CustomVertex.PositionNormalColored.Format, Pool.Managed);
                if(_dataSetVertices != null) {
                    _vertexNums = vertexNum;
                }
                _disposeTable.Add(_dataSetVertices);
                return true;
            }
        }

        private void setupMatrices() {
            lock(_lockAccessDxDevice) {
                if(_dxDevice == null)
                    return;
                MotionDataSet dataSet = _dataSet;
                if(dataSet == null)
                    return;
                _dxDevice.Transform.World = Matrix.Identity;

                MotionFieldState fieldState = dataSet.FieldState;
                _dxDevice.Transform.View = this.ViewCamera.LookAt(fieldState.LeftHanded);
                _dxDevice.Transform.Projection = this.ViewCamera.PerspectiveFov(fieldState.LeftHanded);
            }
        }

        private void renderObject(MotionObject @object, Color color, bool asSelect) {
            // オブジェクトのポリゴン情報をもらってくる
            IList<PolygonDatum> vertexSet;
            PolygonRenderHint hint = new PolygonRenderHint();
            hint.MarginSize = Properties.Settings.Default.Motion_MarginSize;
            if(asSelect) {
                vertexSet = @object.RenderSelectionMark(color.ToArgb(), this.ViewCamera, hint);
            } else {
                vertexSet = @object.Render(color.ToArgb(), this.ViewCamera, hint);
            }
            // DirectXの頂点の準備
            if(setupVertexBuffer(vertexSet)) {
                CustomVertex.PositionNormalColored[] vertices = (CustomVertex.PositionNormalColored[])_dataSetVertices.Lock(0, 0);
                try {
                    List<CustomVertex.PositionNormalColored> tmpVertices = new List<CustomVertex.PositionNormalColored>();
                    foreach(PolygonDatum info in vertexSet) {
                        tmpVertices.AddRange(info.Vertices.Select(v => new CustomVertex.PositionNormalColored(v.Position, v.Normal, v.Color)));
                    }
                    tmpVertices.CopyTo(vertices);
                } finally {
                    _dataSetVertices.Unlock();
                }
                _dxDevice.VertexFormat = CustomVertex.PositionNormalColored.Format;
                // 描画
                int startVertex = 0;
                _dxDevice.SetStreamSource(0, _dataSetVertices, 0);
                foreach(PolygonDatum info in vertexSet) {
                    try {
                        _dxDevice.DrawPrimitives((PrimitiveType)info.Type, startVertex, info.GetPrimitiveCount());
                        startVertex += info.Vertices.Length;
                    } catch(AccessViolationException) { }
                }
            }
        }

        private void renderPreview() {
            int bright = Math.Abs((DateTime.Now.Millisecond % 500) - 250);
            Color subColor = Color.FromArgb(96, bright, bright, bright);
            Color mainColor = Color.FromArgb(bright, bright, bright);
            lock(_lockPreview) {
                if(_previewObjects != null && _previewObjects.Count > 0) {
                    foreach(var pair in _previewObjects) {
                        MotionObject @object = pair.Key;
                        bool sub = pair.Value;
                        Color color;
                        if(sub) {
                            color = subColor;
                        } else {
                            color = mainColor;
                        }
                        if(@object.HasArea() && _transparentEnabled) {
                            color = Color.FromArgb(color.A / 2, color);
                        }
                        renderObject(@object, color, false);
                    }
                }
            }
        }


        private void renderDataSet() {
            if(!this.Enabled)
                return;
            lock(_lockAccessDxDevice) {
                if(_dxDevice == null)
                    return;
                MotionDataSet dataSet = _dataSet;
                if(dataSet == null)
                    return;
                TimeController timeController = _timeController;
                if(timeController == null)
                    return;
                // プレビュー処理
                renderPreview();

                MotionFrame currentFrame;
                if(null != (currentFrame = dataSet.GetFrameAt(timeController.CurrentTime))) {
                    // オブジェクトと最小距離を列挙
                    Dictionary<uint, float> distances = new Dictionary<uint, float>();
                    var infoList = dataSet.GetObjectInfoList();
                    float maxDistance, minDistance;
                    foreach(var info in infoList) {
                        MotionObject @object = currentFrame[info];
                        if(@object == null)
                            continue;
                        if(!@object.GetDistanceDirectional(ViewCamera.Position, ViewCamera.SightDirection(), out minDistance, out maxDistance))
                            continue;
                        // 近すぎるのを無視
                        if(maxDistance < ViewCamera.ZnearPlane)
                            continue;
                        distances[info.Id] = minDistance;
                    }
                    // 面を持たないものから描く
                    foreach(bool hasArea in new bool[] { false, true }) {
                        // 遠いものから描く                     
                        foreach(var info in from pair
                                                in distances
                                            orderby pair.Value descending
                                            select dataSet.GetObjectInfoById(pair.Key)) {
                            if(info == null)
                                continue;
                            MotionObject @object = currentFrame[info];
                            if(@object != null) {
                                // 面を持つかチェック
                                if(@object.HasArea() != hasArea)
                                    continue;
                                // オブジェクト自身を描く
                                if(info.IsVisible) {
                                    lock(_lockPreview) {
                                        if(_previewRemovedObjects.Contains(info))
                                            continue;
                                    }
                                    Color color = info.Color;
                                    if(@object.HasArea() && _transparentEnabled) {
                                        color = Color.FromArgb(color.A / 2, color);
                                    }
                                    renderObject(@object, color, false);
                                }
                                // マウスオーバーと選択を描く
                                uint? mouseOverId = _mouseOverId;
                                if(mouseOverId.HasValue && mouseOverId.Value == info.Id) {
                                    if(dataSet.IsSelecting(info)) {
                                        renderObject(@object, Color.FromArgb(128, Color.Orange), true);
                                    } else {
                                        renderObject(@object, Color.FromArgb(128, Color.YellowGreen), true);
                                    }
                                } else {
                                    if(dataSet.IsSelecting(info)) {
                                        renderObject(@object, Color.FromArgb(128, Color.Red), true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void disposeFloor() {
            lock(_lockFloor) {
                if(_floorVertices != null) {
                    disposeFromTable(_floorVertices);
                    _floorVertices = null;
                }
            }
        }

        private void renderFloor() {
            lock(_lockAccessDxDevice) {
                if(_dxDevice == null)
                    return;
                MotionDataSet dataSet = _dataSet;
                if(dataSet == null)
                    return;
                lock(_lockFloor) {
                    // 床の設定が変わったかチェック
                    if(!_prevFieldState.Equals(dataSet.FieldState)) {
                        disposeFloor();
                        _prevFieldState = dataSet.FieldState;
                    }
                    if(_floorVertices == null) {
                        // 床のポリゴンを作成
                        _floorMaterial = new Material();
                        _floorMaterial.Diffuse = _floorColor;
                        _floorMaterial.Specular = _floorColor;

                        _dxDevice.VertexFormat = CustomVertex.PositionNormal.Format;
                        _disposeTable.Add(_floorVertices = new VertexBuffer(typeof(CustomVertex.PositionNormal), 4 * 32 * 32, _dxDevice, Usage.WriteOnly, CustomVertex.PositionNormal.Format, Pool.Default));
                        CustomVertex.PositionNormal[] verts = (CustomVertex.PositionNormal[])_floorVertices.Lock(0, 0);
                        Vector3 floorVert = dataSet.FieldState.GetFloorSecondaryParallel();
                        for(int i = 0; i < 32; i++) {
                            for(int j = 0; j < 32; j++) {
                                int offset = (i * 32 + j) * 4;
                                float x = ((i - 16) * 2 + 1) * _floorRectLength * 0.5f;
                                float y = ((j - 16) * 2 + 1) * _floorRectLength * 0.5f;
                                for(int k = 0; k < 2; k++) {
                                    for(int l = 0; l < 2; l++) {
                                        float dx = (k * 2 - 1) * (_floorRectLength - _floorRectGap) * 0.5f;
                                        float dy = (l * 2 - 1) * (_floorRectLength - _floorRectGap) * 0.5f;
                                        verts[offset + k * 2 + l].Position = dataSet.FieldState.FloorCenter + (x + dx) * dataSet.FieldState.FloorParallel + (y + dy) * floorVert;
                                        verts[offset + k * 2 + l].Normal = dataSet.FieldState.FloorUpper;
                                    }
                                }
                            }
                        }
                        _floorVertices.Unlock();
                    }
                    // 元の材質設定を保存
                    Material oldMaterial = _dxDevice.Material;
                    // 床を描画
                    _dxDevice.SetStreamSource(0, _floorVertices, 0);
                    _dxDevice.VertexFormat = CustomVertex.PositionNormal.Format;
                    _dxDevice.Material = _floorMaterial;
                    for(int i = 0; i < 32; i++) {
                        for(int j = 0; j < 32; j++) {
                            int offset = (i * 32 + j) * 4;
                            _dxDevice.DrawPrimitives(PrimitiveType.TriangleStrip, offset, 2);
                        }
                    }
                    _dxDevice.Material = oldMaterial;
                }
            }
        }

        private void drawLineWithSplite(Sprite sprite, Vector2 begin, Vector2 end, Color color, bool doBeginAndEnd) {
            if(doBeginAndEnd) {
                sprite.Begin(SpriteFlags.AlphaBlend);
            }
            try {
                Vector2 d = end - begin;
                float angle = (float)Math.Atan2(d.Y, d.X);
                float length = d.Length();
                Vector2 c = begin + d * 0.5f;

                sprite.Draw2D(_textureWhite, new Rectangle(0, 0, 1, 1), new SizeF(length, 1), new PointF(0.5f, 0.5f), angle, new PointF(c.X, c.Y), color);
            } finally {
                if(doBeginAndEnd) {
                    sprite.End();
                }
            }
        }

        /// <summary>
        /// デバイスのリセットを行う
        /// </summary>
        private void onDeviceLostException() {
            int result;
            // リセット可能かどうかをチェック
            lock(_lockAccessDxDevice) {
                if(_dxDevice == null)
                    return;
                if(!_dxDevice.CheckCooperativeLevel(out result)) {
                    _resetDone = false;
                    // リセット可能ならリセット
                    if(result == (int)ResultCode.DeviceNotReset) {
                        try {
                            _dxDevice.Reset(_dxDevice.PresentationParameters);
                            onResetDevice(_dxDevice, new EventArgs());
                        } catch(InvalidCallException) { System.Threading.Thread.Sleep(20); }
                    } else if(result == (int)ResultCode.DeviceLost) {
                        // まだリセットできなければ、しばらくスリープ
                        System.Threading.Thread.Sleep(20);
                    }
                }
            }
        }

        private void _dataSet_ObjectChanged(object sender, EventArgs e) {
            if(this.InvokeRequired) {
                this.BeginInvoke(new EventHandler(_dataSet_ObjectChanged), sender, e);
                return;
            }
            this.RequestRender();
        }


        /// <summary>
        /// 再描画を行います。
        /// </summary>
        public bool RequestRender() {
            timerRender.Interval = 5;
            return requestRenderInternal();
        }

        private bool requestRenderInternal() {
            _requestRender = true;
            if(bgRender.IsBusy)
                return false;
            lock(_lockAccessDxDevice) {
                MotionDataSet dataSet = _dataSet;
                if(dataSet == null)
                    return false;
                if(_deviceCannotRestore)
                    return false;
                if(_dxDevice == null) {
                    if(this.Enabled) {
                        try {
                            initializeDirect3D();
                        } catch(Exception ex) {
                            _deviceCannotRestore = true;
                            ErrorLogger.Tell(ex, "DirectXの初期化に失敗しました");
                        }
                    }
                }
            }
            lock(bgRender) {
                if(!bgRender.IsBusy) {
                    bgRender.RunWorkerAsync();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// ビュー内にオブジェクトを仮に配置します
        /// </summary>
        /// <param name="object">オブジェクトのデータ</param>
        /// <param name="sub">補足的なオブジェクトの場合はtrue</param>
        public void AddPreviewObject(MotionObject @object, bool sub) {
            lock(_lockPreview) {
                _previewObjects.Add(new KeyValuePair<MotionObject, bool>(@object, sub));
            }
        }
        /// <summary>
        /// ビューからオブジェクトを仮に取り除きます
        /// </summary>
        /// <param name="info">取り除くオブジェクトのMotionObjectInfo</param>
        public void AddPreviewRemoveObject(MotionObjectInfo info) {
            lock(_lockPreview) {
                _previewRemovedObjects.Add(info);
            }
        }


        public void ClearPreviewObjects() {
            lock(_lockPreview) {
                _previewObjects.Clear();
                _previewRemovedObjects.Clear();
            }
        }


        public void EnableTraceSelectedObjects(bool enable) {
            if(this.InvokeRequired) {
                this.Invoke(new Action<bool>(EnableTraceSelectedObjects), enable);
                return;
            }
            traceSelectedToolStripMenuItem.Checked = enable;
        }


        #endregion


    }
}

