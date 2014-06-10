using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MotionDataHandler.Misc {
    /// <summary>
    /// 再生，停止などを行うためのコントロール
    /// </summary>
    public partial class TimePlayer : UserControl {
        private TimeController _timeController = null;
        object _lockTimeChanging = new object();
        bool _isTimeChanging = false;
        object _lockFPSChanging = new object();
        bool _isFPSChanging = false;
        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        public TimePlayer() {
            InitializeComponent();
            AttachTimeController(TimeController.Singleton);
        }
        public void AttachTimeController(TimeController timeController) {
            DetachTimeController();
            if (timeController != null) {
                lock (_timeController = timeController) {
                    _timeController.CurrentTimeChanged += onTimeChanged;
                    _timeController.IsPlayingChanged += onPlayingChanged;
                    _timeController.SettingsChanged += onTimeControllerSettingsChanged;
                    onTimeControllerSettingsChanged(this, new EventArgs());
                }
            }
            enableControls(_timeController != null);
        }
        public void DetachTimeController() {
            if (_timeController != null) {
                lock (_timeController) {
                    _timeController.CurrentTimeChanged -= onTimeChanged;
                    _timeController.IsPlayingChanged -= onPlayingChanged;
                    _timeController.SettingsChanged -= onTimeControllerSettingsChanged;
                    _timeController = null;
                }
            }
        }

        private void onPlayingChanged(object sender, EventArgs e) {
            setButtonPlayText((sender as TimeController).IsPlaying ? "||" : ">");
        }
        private void setButtonPlayText(string text) {
            if (this.InvokeRequired) {
                this.Invoke(new Action<string>(setButtonPlayText), text);
                return;
            }
            buttonPlay.Text = text;
        }

        private void onTimeChanged(object sender, EventArgs e) {
            setNums();
        }

        private void onTimeControllerSettingsChanged(object sender, EventArgs e) {
            lock (_lockFPSChanging) {
                if (_isFPSChanging)
                    return;
                _isFPSChanging = true;
            }
            try {
                setTrackAndNumRange();
                setLabels();
            } finally {
                _isFPSChanging = false;
            }
        }

        private void setNums() {
            if (this.InvokeRequired) {
                this.Invoke(new Action(setNums));
                return;
            }
            if (_timeController == null)
                return;
            lock (_lockTimeChanging) {
                if (_isTimeChanging)
                    return;
                _isTimeChanging = true;
            }
            try {
                if (_timeController.CurrentIndex >= trackIndex.Minimum && _timeController.CurrentIndex <= trackIndex.Maximum)
                    trackIndex.Value = _timeController.CurrentIndex;
                if (_timeController.CurrentIndex >= numIndex.Minimum && _timeController.CurrentIndex <= numIndex.Maximum)
                    numIndex.Value = _timeController.CurrentIndex;
                if (_timeController.CurrentTime >= numTime.Minimum && _timeController.CurrentTime <= numTime.Maximum)
                    numTime.Value = _timeController.CurrentTime;
            } finally {
                _isTimeChanging = false;
            }
        }

        private void setLabels() {
            if (this.InvokeRequired) {
                this.Invoke(new Action(setLabels));
                return;
            }
            if (_timeController == null)
                return;
            lock (_timeController) {
                labelIndex.Text = string.Format("/ {0}", _timeController.IndexCount);
                labelSpan.Text = string.Format("Time: {0} / {1}", _timeController.CurrentTime.ToString("F3"), _timeController.Duration.ToString("F3"));
            }
        }

        private void setTrackAndNumRange() {
            if (this.InvokeRequired) {
                this.Invoke(new Action(setTrackAndNumRange));
                return;
            }
            lock (_timeController) {
                trackIndex.Maximum = _timeController.IndexCount - 1;
                trackIndex.Minimum = 0;
                numTime.Maximum = _timeController.Duration;
                numTime.Minimum = 0;
                numIndex.Maximum = _timeController.IndexCount - 1;
                numIndex.Minimum = 0;
            }
        }

        private void onDispose(object sender, EventArgs e) {
            DetachTimeController();
        }

        private void enableControls(bool enable) {
            if (this.InvokeRequired) {
                this.Invoke(new Action<bool>(enableControls), enable);
                return;
            }
            buttonFirst.Enabled = enable;
            buttonPlay.Enabled = enable;
            trackIndex.Enabled = enable;
            numTime.Enabled = enable;
        }


        private void trackIndex_Scroll(object sender, EventArgs e) {
            if (_timeController == null)
                return;
            lock (_lockTimeChanging) {
                if (_isTimeChanging)
                    return;
            }
            lock (_timeController) {
                _timeController.CurrentIndex = trackIndex.Value;
            }
        }

        private void numTime_ValueChanged(object sender, EventArgs e) {
            if (_timeController == null)
                return;
            lock (_lockTimeChanging) {
                if (_isTimeChanging)
                    return;
            }
            lock (_timeController) {
                _timeController.CurrentTime = numTime.Value;
            }
        }

        private void numIndex_ValueChanged(object sender, EventArgs e) {
            if (_timeController == null)
                return;
            lock (_lockTimeChanging) {
                if (_isTimeChanging)
                    return;
            }
            lock (_timeController) {
                _timeController.CurrentIndex = (int)numIndex.Value;
            }

        }

        private void buttonPlay_Click(object sender, EventArgs e) {
            if (_timeController == null)
                return;
            lock (_timeController) {
                _timeController.IsPlaying = !_timeController.IsPlaying;
            }
        }

        private void buttonFirst_Click(object sender, EventArgs e) {
            if (_timeController == null)
                return;
            lock (_timeController) {
                _timeController.CurrentTime = 0;
            }
        }

        private void TimePlayer_Load(object sender, EventArgs e) {
            this.Disposed += onDispose;
            this.Parent.Disposed += onDispose;
            AttachTimeController(TimeController.Singleton);
        }

        private void numFPS_ValueChanged(object sender, EventArgs e) {
            lock (_lockFPSChanging) {
                if (_isFPSChanging)
                    return;
            }
            if (_timeController == null)
                return;
            _timeController.FPS = numFPS.Value;
        }
    }
}
