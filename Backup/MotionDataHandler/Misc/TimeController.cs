using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace MotionDataHandler.Misc {
    /// <summary>
    /// 再生時間の制御や，開始・終了時間，カーソル位置，表示範囲，選択範囲を管理するためのクラス
    /// </summary>
    public class TimeController {
        Mutex _mutex = new Mutex();
        /// <summary>
        /// ITimeIntervalを実装しない時間幅データがある場合に時間幅リストに加えるためのクラス
        /// </summary>
        class InternalTimeInterval : ITimeInterval {
            readonly decimal _duration;
            public InternalTimeInterval(decimal duration) {
                _duration = duration;
            }
            #region ITimeInterval メンバ

            public decimal BeginTime { get { return 0; } }
            public decimal EndTime { get { return _duration; } }
            public event EventHandler TimeIntervalChanged = (s, e) => { };


            #endregion
        }

        #region Properties
        private static readonly TimeController _singleton = new TimeController();
        private readonly List<InternalTimeInterval> _internalTimeIntervals = new List<InternalTimeInterval>();
        private readonly WeakCollection<ITimeInterval> _timeIntervals = new WeakCollection<ITimeInterval>();
        private readonly System.Windows.Forms.Timer timerPlay = new System.Windows.Forms.Timer();
        private decimal _prevBeginTime, _prevEndTime;
        private decimal _beginVisibleTime = 0M, _endVisibleTime = 60M;
        private bool _isSelecting = false;
        private decimal _beginSelectTime = 0M, _endSelectTime = 60M;

        private decimal _fps = 60;
        private DateTime _previousNow = DateTime.MinValue;
        private decimal _previousCurrentTime = -1;
        private decimal _previousTimeChangedInvoked = 0;
        protected decimal _currentTime = 0;
        private double _previousMovieTime;
        private bool _isPlaying = false;
        private Plugin.IPluginHost _pluginHost = null;
        private decimal _timeChangedEpsilon = 0;
        private bool _isAutoScroll = true;
        private bool _isLoopEnabled = false;


        /// <summary>
        /// コントローラが時間を進めているかを取得または設定します。
        /// </summary>
        public bool IsPlaying {
            get { return _isPlaying; }
            set {
                if(value) {
                    if(!this.IsPlaying) {
                        Play();
                    }
                } else {
                    if(this.IsPlaying) {
                        Stop();
                    }
                }
            }
        }
        /// <summary>
        /// 現在の時間が選択範囲外に達したときに選択範囲が追随するかを取得または設定します。
        /// </summary>
        public bool IsAutoScroll {
            get { return _isAutoScroll; }
            set {
                if(_isAutoScroll != value) {
                    _isAutoScroll = value;
                    if(value) {
                        this.setCurrentTimeInternal(this.CurrentTime, true);
                    }
                    DoSettingsChanged();
                }
            }
        }
        /// <summary>
        /// 再生時にループを有効にするかを取得または設定します。
        /// </summary>
        public bool IsLoopEnabled { get { return _isLoopEnabled; } set { _isLoopEnabled = value; } }
        /// <summary>
        /// 内部タイマーの更新間隔をミリ秒で取得または設定します。
        /// </summary>
        public int TickInterval {
            get { return timerPlay.Interval; }
            set { timerPlay.Interval = value; }
        }




        /// <summary>
        /// 時間幅オブジェクトのコレクションの中で最小の開始時間を返します。
        /// </summary>
        public decimal BeginTime {
            get {
                var tmp = _timeIntervals.Where(t => t != null).ToList();
                return tmp.Any() ? tmp.Min(i => i.BeginTime) : 0;
            }
        }
        /// <summary>
        /// 時間幅オブジェクトのコレクションの中で最大の終了時間を返します。
        /// </summary>
        public decimal EndTime {
            get {
                var tmp = _timeIntervals.Where(t => t != null).ToList();
                return tmp.Any() ? tmp.Max(i => i.EndTime) : 0;
            }
        }
        /// <summary>
        /// 表示範囲の開始時間を取得または設定します。
        /// </summary>
        public decimal VisibleBeginTime {
            get { return _beginVisibleTime; }
            set { this.SetVisibleTime(value, value < _endVisibleTime ? _endVisibleTime : value); }
        }
        /// <summary>
        /// 表示範囲の終了時間を取得または設定します。
        /// </summary>
        public decimal VisibleEndTime {
            get { return _endVisibleTime; }
            set { this.SetVisibleTime(value < _beginVisibleTime ? value : _beginVisibleTime, value); }
        }
        /// <summary>
        /// 表示範囲の長さを取得します。
        /// </summary>
        public decimal VisibleDuration { get { return VisibleEndTime - VisibleBeginTime; } }

        /// <summary>
        /// 選択範囲の開始時間を取得または設定します。
        /// </summary>
        public decimal SelectBeginTime {
            get { return _beginSelectTime; }
        }
        /// <summary>
        /// 選択範囲の終了時間を取得または設定します。
        /// </summary>
        public decimal SelectEndTime {
            get { return _endSelectTime; }
        }
        /// <summary>
        /// 選択範囲の長さを取得します。
        /// </summary>
        public decimal SelectDuration { get { return SelectEndTime - SelectBeginTime; } }
        public bool IsSelecting { get { return _isSelecting; } }

        private decimal? _cursorTime = null;
        /// <summary>
        /// タイムバーなどのカーソルがある位置の時間を取得または設定します。
        /// </summary>
        public decimal? CursorTime {
            get { return _cursorTime; }
            set {
                _cursorTime = value;
                if(this.CursorTimeChanged != null) {
                    this.CursorTimeChanged.Invoke(this, new EventArgs());
                }
            }
        }



        /// <summary>
        /// 唯一のTimeControllerオブジェクトを取得します。
        /// </summary>
        public static TimeController Singleton { get { return _singleton; } }
        /// <summary>
        /// コントローラの終端時間を秒単位で取得します。
        /// </summary>
        public decimal Duration { get { return EndTime - BeginTime; } }
        /// <summary>
        /// 1秒中のインデックスの数を取得または設定します。
        /// </summary>
        public decimal FPS {
            get {
                Debug.Assert(_fps > 0);
                return _fps;
            }
            set {
                if(value <= 0)
                    throw new ArgumentOutOfRangeException("value", "'FPS' must be positive");
                if(_fps != value) {
                    _fps = value;
                    int tick = (int)(1000 / _fps / 2);
                    if(tick > 0)
                        TickInterval = tick;
                    DoSettingsChanged();
                }
            }
        }
        /// <summary>
        /// 現在のFPSでのインデックスの数を取得します。
        /// </summary>
        public int IndexCount { get { return (int)(Duration * FPS) + 1; } }
        /// <summary>
        /// 現在のFPSでの現在の時間のインデックスを取得または設定します。
        /// </summary>
        public int CurrentIndex {
            get { return this.GetIndexFromTime(this.CurrentTime); }
            set { setCurrentTimeInternal(this.GetTimeFromIndex(value), true, true); }
        }
        /// <summary>
        /// 現在の時間を取得または設定します。
        /// </summary>
        public decimal CurrentTime {
            get { return _currentTime; }
            set { setCurrentTimeInternal(value, true); }
        }
        /// <summary>
        /// 現在の時間をdouble型にキャストして取得または設定します。
        /// 設定にもちいた値と設定された値が異なる場合があります。
        /// </summary>
        public double CurrentTimeAsDouble {
            get { return (double)this.CurrentTime; }
            set { this.CurrentTime = (decimal)value; }
        }

        /// <summary>
        /// TimeChangedイベントが起こる時間の最小幅を取得または設定します。
        /// 0ならば値が変更された場合に必ずイベントを起こします。
        /// 負の数が設定された場合は値が変わらない場合でもイベントを起こします。
        /// </summary>
        public decimal TimeChangedEpsilon {
            get { return _timeChangedEpsilon; }
            set { _timeChangedEpsilon = value; }
        }

        #endregion
        #region Events
        /// <summary>
        /// CurrentTimeにTimeChangedEpsilonより大きな変更がなされた場合に呼び出されるイベント。
        /// </summary>
        public event EventHandler CurrentTimeChanged;
        /// <summary>
        /// 時間の表示範囲が変更された場合に呼び出されるイベント。
        /// </summary>
        public event EventHandler VisibleRangeChanged;
        /// <summary>
        /// 時間の選択範囲が変更された場合に呼び出されるイベント。
        /// </summary>
        public event EventHandler SelectedRangeChanged;
        /// <summary>
        /// IsPlayingの値が変更された場合に呼び出されるイベント。
        /// </summary>
        public event EventHandler IsPlayingChanged;
        /// <summary>
        /// Duration, FPSの値が変更された場合に呼び出されるイベント。
        /// </summary>
        public event EventHandler SettingsChanged;
        /// <summary>
        /// 外部から時間が変更された場合に呼び出されるイベント。
        /// </summary>
        public event EventHandler TimeSeeking;
        /// <summary>
        /// 現在のカーソルがある位置の時間が変更された場合に呼び出されるイベント。
        /// </summary>
        public event EventHandler CursorTimeChanged;

        #endregion

        /// <summary>
        /// 時間幅を持つオブジェクトをコレクションに追加します。
        /// </summary>
        /// <param name="interval"></param>
        public void AddTimeInterval(ITimeInterval interval) {
            if(interval == null)
                throw new ArgumentNullException("interval", "'interval' cannot be null");
            var intervals = _timeIntervals.ToList();
            if(!intervals.Contains(interval)) {
                _timeIntervals.Add(interval);
                interval.TimeIntervalChanged += new EventHandler(interval_TimeIntervalChanged);
                DoSettingsChanged();
            }
        }

        /// <summary>
        /// 時間幅を持つオブジェクトをコレクションから削除します。
        /// </summary>
        /// <param name="interval"></param>
        public void RemoveTimeInterval(ITimeInterval interval) {
            if(interval == null)
                throw new ArgumentNullException("interval", "'interval' cannot be null");
            var intervals = _timeIntervals.ToList();
            if(intervals.Contains(interval)) {
                _timeIntervals.Remove(interval);
                interval.TimeIntervalChanged -= new EventHandler(interval_TimeIntervalChanged);
                DoSettingsChanged();
            }
        }


        void interval_TimeIntervalChanged(object sender, EventArgs e) {
            decimal beginTime = BeginTime;
            decimal endTime = EndTime;
            if(beginTime != _prevBeginTime || endTime != _prevEndTime) {
                DoSettingsChanged();
                _prevBeginTime = beginTime;
                _prevEndTime = endTime;
            }
        }
        /// <summary>
        /// 時間の表示範囲を変更します。
        /// </summary>
        /// <param name="begin">表示範囲の開始時間</param>
        /// <param name="end">表示範囲の終了時間</param>
        public void SetVisibleTime(decimal begin, decimal end) {
            if(begin > end) {
                decimal tmp = begin;
                begin = end;
                end = tmp;
            }
            _beginVisibleTime = begin;
            _endVisibleTime = end;
            if(VisibleRangeChanged != null) {
                VisibleRangeChanged.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// 時間の選択範囲を変更します。
        /// </summary>
        /// <param name="begin">選択範囲の開始時間</param>
        /// <param name="end">選択範囲の終了時間</param>
        public void SelectRange(decimal begin, decimal end) {
            if(begin > end) {
                decimal tmp = begin;
                begin = end;
                end = tmp;
            }
            _beginSelectTime = begin;
            _endSelectTime = end;
            _isSelecting = true;
            EventHandler selectedRangeChanged = this.SelectedRangeChanged;
            if(selectedRangeChanged != null) {
                selectedRangeChanged.Invoke(this, new EventArgs());
            }
        }
        /// <summary>
        /// 時間範囲の選択を解除します
        /// </summary>
        public void DeselectRange() {
            _isSelecting = false;
            EventHandler selectedRangeChanged = this.SelectedRangeChanged;
            if(selectedRangeChanged != null) {
                selectedRangeChanged.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// 表示時間範囲を選択します
        /// </summary>
        public void SelectVisibleRange() {
            this.SelectRange(this.VisibleBeginTime, this.VisibleEndTime);
        }
        /// <summary>
        /// 選択された時間範囲に表示範囲を合わせます
        /// </summary>
        public void ZoomSelectedRange() {
            if(this.IsSelecting) {
                this.SetVisibleTime(this.SelectBeginTime, this.SelectEndTime);
                this.DeselectRange();
            }
        }
        /// <summary>
        /// (選択範囲/現在の表示範囲)=(現在の表示範囲/新しい表示範囲)になるように表示範囲を変更します
        /// </summary>
        public void UnzoomSelectedRange() {
            if(this.IsSelecting) {
                decimal beginVisible = this.VisibleBeginTime;
                decimal left = this.VisibleBeginTime;
                decimal right = this.VisibleEndTime;
                decimal end = this.SelectEndTime;
                decimal begin = this.SelectBeginTime;
                if(end - begin > 0) {
                    decimal scale = (right - left) / (end - begin);
                    decimal newLeft = (left - begin) * scale + left;
                    decimal newRight = (right - end) * scale + right;
                    left = newLeft;
                    right = newRight;
                }
                this.SetVisibleTime(left, right);
                this.DeselectRange();
            }
        }

        /// <summary>
        /// 範囲を選択している場合には開始端か終了端の内，指定された値に近いほうの値を指定された値に変更します．
        /// 選択していない場合には指定された値の点を選択します．
        /// </summary>
        /// <param name="time"></param>
        public void AdjustSelectedRange(decimal time) {
            if(this.IsSelecting) {
                decimal center = (this.SelectBeginTime + this.SelectEndTime) / 2;
                if(center < time) {
                    this.SelectRange(this.SelectBeginTime, time);
                } else {
                    this.SelectRange(time, this.SelectEndTime);
                }
            } else {
                this.SelectRange(time, time);
            }
        }

        /// <summary>
        /// 指定された値が現在のDurationより大きい場合にDurationの値を延長します。
        /// </summary>
        /// <param title="duration">新しい終端時間を秒単位で指定します。</param>
        public void ExtendDuration(decimal duration) {
            var tmp = new InternalTimeInterval(duration);
            _internalTimeIntervals.Add(tmp);
            this.AddTimeInterval(tmp);
        }

        /// <summary>
        /// 指定された時間を含むインデックスを求めます
        /// </summary>
        /// <param name="time">もとになる時間</param>
        /// <returns>インデックス</returns>
        public int GetIndexFromTime(decimal time) {
            return (int)(decimal.Floor(time * FPS));
        }
        /// <summary>
        /// 指定されたインデックスの開始時間を求めます。
        /// </summary>
        /// <param name="index">もとになるインデックス</param>
        /// <returns>時間</returns>
        public decimal GetTimeFromIndex(int index) {
            return ((decimal)index + 0.001M) / FPS;
        }
        /// <summary>
        /// 設定変更を伝播します
        /// </summary>
        public void DoSettingsChanged() {
            if(SettingsChanged != null) {
                SettingsChanged.Invoke(this, new EventArgs());
            }
        }
        /// <summary>
        /// 再生状態の変更を伝播します
        /// </summary>
        public void DoIsPlayingChanged() {
            if(IsPlayingChanged != null) {
                IsPlayingChanged.Invoke(this, new EventArgs());
            }
        }
        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        private TimeController() {
            this.TickInterval = 5;
            this.TimeChangedEpsilon = 0;
            timerPlay.Tick += timerPlay_Tick;
            timerPlay.Start();
        }
        /// <summary>
        /// 再生を開始します。
        /// </summary>
        public void Play() {
            if(_isPlaying) {
                return;
            }
            _isPlaying = true;
            if(_pluginHost != null) {
                if(!_pluginHost.IsRunning) {
                    _pluginHost.MoviePlay();
                }
            }
            DoIsPlayingChanged();
        }
        /// <summary>
        /// 再生を停止します。
        /// </summary>
        public void Stop() {
            if(!_isPlaying) {
                return;
            }
            _isPlaying = false;
            if(_pluginHost != null) {
                if(_pluginHost.IsRunning) {
                    _pluginHost.MoviePause();
                }
            }
            DoIsPlayingChanged();
        }

        public void AttachIPluginHost(Plugin.IPluginHost pluginHost) {
            _pluginHost = pluginHost;
            if(_pluginHost != null) {
                IsPlaying = _pluginHost.IsRunning;
            }
        }
        public void DetachIPluginHost() {
            _pluginHost = null;
        }

        private void setCurrentTimeInternal(decimal time, bool isSeeking) {
            setCurrentTimeInternal(time, isSeeking, false);
        }
        /// <summary>
        /// 時間を変更する処理
        /// </summary>
        /// <param name="time">設定される時間</param>
        /// <param name="isSeeking">シークの場合はtrue．再生の場合はfalse</param>
        /// <param name="isSliding"></param>
        private void setCurrentTimeInternal(decimal time, bool isSeeking, bool isRelativeSeeking) {
            _mutex.WaitOne();
            try {
                decimal visibleDuration = this.VisibleDuration;
                decimal epsilon = visibleDuration / 8640000M;
                decimal prevTime = _currentTime;
                if(this.IsLoopEnabled && !isSeeking && this.IsSelecting) {
                    // 再生時のループの処理
                    if(prevTime < this.SelectEndTime && this.SelectEndTime <= time) {
                        // 順方向
                        setCurrentTimeInternal(this.SelectBeginTime + epsilon, true, isRelativeSeeking);
                        return;
                    } else if(time <= this.SelectBeginTime && this.SelectBeginTime < prevTime) {
                        // 逆方向
                        setCurrentTimeInternal(this.SelectEndTime - epsilon, true, isRelativeSeeking);
                        return;
                    }
                }
                if(_pluginHost != null && isSeeking) {
                    // iCorpusStudioから呼び出されているときにシークする処理
                    // 再生中のときは止めてからシークしてまた再生
                    bool hasBeenPlaying = this.IsPlaying;
                    if(this.IsPlaying) {
                        this.Stop();
                    }
                    if(isRelativeSeeking) {
                        // 動かしたときに 1/fps 以下しか時間が変更されていないとiCorpusStudio側で変わりがないのでもっと動かす
                        decimal movieTime = (decimal)_pluginHost.MovieCurrentTime;
                        decimal originalTime = time;
                        _pluginHost.MovieCurrentTime = (double)time;
                        time = (decimal)(_previousMovieTime = _pluginHost.MovieCurrentTime);
                        while(movieTime == time && Math.Abs(originalTime - time) < 1) {
                            originalTime = (originalTime - movieTime) * 2 + movieTime;
                            _pluginHost.MovieCurrentTime = (double)originalTime;
                            time = (decimal)(_previousMovieTime = _pluginHost.MovieCurrentTime);
                        }
                        _previousCurrentTime = time;
                    } else {
                        _pluginHost.MovieCurrentTime = (double)time;
                        time = (decimal)(_previousMovieTime = _pluginHost.MovieCurrentTime);
                        _previousCurrentTime = time;
                    }
                    if(hasBeenPlaying) {
                        this.Play();
                    }
                }
                // 範囲外禁止
                if(time > this.EndTime)
                    time = this.EndTime;
                if(time < this.BeginTime)
                    time = this.BeginTime;
                // TimeChangedを呼ぶ処理
                // 前回からEpsilon以上経過したか確認
                decimal delta = Math.Abs(_previousTimeChangedInvoked - time);
                _currentTime = time;
                if(delta > _timeChangedEpsilon) {
                    EventHandler timeChanged = this.CurrentTimeChanged;
                    if(timeChanged != null) {
                        timeChanged.Invoke(this, new EventArgs());
                    }
                    _previousTimeChangedInvoked = time;
                }
                if(this.IsAutoScroll) {
                    if(this.IsSelecting && this.IsLoopEnabled && !isSeeking && this.VisibleBeginTime < this.SelectBeginTime && this.SelectEndTime < this.VisibleEndTime) {
                        // ループがオンで選択範囲がすべて見えるときはスクロールしない
                    } else {
                        // オートスクロールの処理
                        if(this.CurrentTime < this.VisibleBeginTime || this.VisibleEndTime < this.CurrentTime) {
                            // 画面外に飛んだ時
                            this.SetVisibleTime(time - this.VisibleDuration * 0.125M, time + this.VisibleDuration * 0.875M);
                        } else if(time < prevTime && !isSeeking && time < this.VisibleBeginTime + visibleDuration * 0.125M) {
                            // 逆再生で左の方に行った時
                            this.SetVisibleTime(time - this.VisibleDuration + epsilon, time + epsilon);
                        } else if(time > prevTime && !isSeeking && time > this.VisibleBeginTime + visibleDuration * 0.875M) {
                            // 順再生で右の方に行った時
                            this.SetVisibleTime(time - epsilon, time + this.VisibleDuration - epsilon);
                        }
                    }
                }
                if(isSeeking) {
                    EventHandler timeSeeking = this.TimeSeeking;
                    if(timeSeeking != null) {
                        timeSeeking.Invoke(this, new EventArgs());
                    }
                }
            } finally { _mutex.ReleaseMutex(); }
        }
        private void timerPlay_Tick(object sender, EventArgs e) {
            if(!_mutex.WaitOne(0))
                return;
            try {
                DateTime now = DateTime.Now;
                if(_previousNow != DateTime.MinValue) {
                    if(_pluginHost != null) {
                        if(_previousMovieTime != _pluginHost.MovieCurrentTime) {
                            double currentTime = _pluginHost.MovieCurrentTime;
                            setCurrentTimeInternal((decimal)currentTime, !_pluginHost.IsRunning);
                        } else if(_previousCurrentTime != this.CurrentTime) {
                            decimal currentTime = this.CurrentTime;
                            _pluginHost.MovieCurrentTime = (double)currentTime;
                        }
                        _previousMovieTime = _pluginHost.MovieCurrentTime;
                        _previousCurrentTime = this.CurrentTime;
                        this.IsPlaying = _pluginHost.IsRunning;
                    } else {
                        if(IsPlaying) {
                            long elapse = now.Ticks - _previousNow.Ticks;
                            setCurrentTimeInternal(CurrentTime + (decimal)elapse / 10000000, false);
                            if(CurrentTime == Duration) {
                                Stop();
                            }
                        }
                    }
                }
                _previousNow = now;
            } finally { _mutex.ReleaseMutex(); }
        }
    }
}
