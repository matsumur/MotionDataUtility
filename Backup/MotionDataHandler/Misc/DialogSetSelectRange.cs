using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MotionDataHandler.Misc {
    /// <summary>
    /// 時間の選択範囲を数値で入力するためのダイアログ
    /// </summary>
    public partial class DialogSetSelectRange : DialogOKCancel {
        private readonly TimeController _timeController;
        public DialogSetSelectRange(TimeController timeController, bool checkFrame) {
            InitializeComponent();
            if(timeController == null)
                throw new ArgumentNullException("timeController", "'timeController' cannot be null");
            _timeController = timeController;
            initControls(checkFrame);
        }

        void initControls(bool checkFrame) {
            numEndSec.Minimum = numBeginSec.Minimum = new decimal[] { _timeController.BeginTime, _timeController.SelectBeginTime, 0 }.Min();
            numEndSec.Maximum = numBeginSec.Maximum = new decimal[] { _timeController.EndTime, _timeController.SelectBeginTime }.Max();
            var beginIndex = _timeController.GetIndexFromTime(_timeController.VisibleBeginTime);
            var endIndex = _timeController.GetIndexFromTime(_timeController.VisibleEndTime);
            numEndFrame.Minimum = numBeginFrame.Minimum = new decimal[] { 0, beginIndex }.Min();
            numEndFrame.Maximum = numBeginFrame.Maximum = new decimal[] { _timeController.IndexCount, endIndex }.Max();
            try {
                numBeginSec.Value = _timeController.SelectBeginTime;
                numEndSec.Value = _timeController.SelectEndTime;
                numBeginFrame.Value = beginIndex;
                numEndFrame.Value = endIndex;
            } catch(ArgumentOutOfRangeException) { }
            radioBeginSec.Checked = radioEndSec.Checked = !checkFrame;
            radioBeginFrame.Checked = radioEndFrame.Checked = checkFrame;
            setNumEnabled();
            propagateValue();
        }

        void setNumEnabled() {
            numBeginSec.Enabled = radioBeginSec.Checked;
            numBeginFrame.Enabled = radioBeginFrame.Checked;
            numEndSec.Enabled = radioEndSec.Checked;
            numEndFrame.Enabled = radioEndFrame.Checked;
        }

        void propagateValue() {
            if(radioBeginSec.Checked) {
                decimal value = _timeController.GetIndexFromTime(numBeginSec.Value);
                value = numBeginFrame.Minimum < value ? value < numBeginFrame.Maximum ? value : numBeginFrame.Maximum : numBeginFrame.Minimum;
                try {
                    numBeginFrame.Value = value;
                } catch(ArgumentOutOfRangeException) { }
            } else if(radioBeginFrame.Checked) {
                decimal value = _timeController.GetTimeFromIndex((int)numBeginFrame.Value);
                value = numBeginSec.Minimum < value ? value < numBeginSec.Maximum ? value : numBeginSec.Maximum : numBeginSec.Minimum;
                try {
                    numBeginSec.Value = value;
                } catch(ArgumentOutOfRangeException) { }
            }
            if(radioEndSec.Checked) {
                decimal value = _timeController.GetIndexFromTime(numEndSec.Value);
                value = numEndFrame.Minimum < value ? value < numEndFrame.Maximum ? value : numEndFrame.Maximum : numEndFrame.Minimum;
                try {
                    numEndFrame.Value = value;
                } catch(ArgumentOutOfRangeException) { }
            } else if(radioEndFrame.Checked) {
                decimal value = _timeController.GetTimeFromIndex((int)numEndFrame.Value);
                value = numEndSec.Minimum < value ? value < numEndSec.Maximum ? value : numEndSec.Maximum : numEndSec.Minimum;
                try {
                    numEndSec.Value = value;
                } catch(ArgumentOutOfRangeException) { }
            }
        }

        private void radioBeginSec_CheckedChanged(object sender, EventArgs e) {
            setNumEnabled();
            numBeginSec.Focus();
        }

        private void radioBeginFrame_CheckedChanged(object sender, EventArgs e) {
            setNumEnabled();
            numBeginFrame.Focus();
        }

        private void radioEndSec_CheckedChanged(object sender, EventArgs e) {
            setNumEnabled();
            numEndSec.Focus();
        }

        private void radioEndFrame_CheckedChanged(object sender, EventArgs e) {
            setNumEnabled();
            numEndFrame.Focus();
        }

        private void numBeginSec_ValueChanged(object sender, EventArgs e) {
            propagateValue();
        }

        private void numBeginFrame_ValueChanged(object sender, EventArgs e) {
            propagateValue();
        }

        private void numEndSec_ValueChanged(object sender, EventArgs e) {
            propagateValue();
        }

        private void numEndFrame_ValueChanged(object sender, EventArgs e) {
            propagateValue();
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            decimal begin = 0M, end = 0M;
            bool checkBegin = false, checkEnd = false;
            if(radioBeginSec.Checked) {
                begin = numBeginSec.Value;
                checkBegin = true;
            } else if(radioBeginFrame.Checked) {
                begin = _timeController.GetTimeFromIndex((int)numBeginFrame.Value);
                checkBegin = true;
            }
            if(radioEndSec.Checked) {
                end = numEndSec.Value;
                checkEnd = true;
            } else if(radioEndFrame.Checked) {
                end = _timeController.GetTimeFromIndex((int)numEndFrame.Value);
                checkEnd = true;
            }
            if(checkBegin && checkEnd) {
                _timeController.SelectRange(begin, end);
            } else if(checkBegin) {
                if(_timeController.IsSelecting) {
                    _timeController.SelectRange(begin, _timeController.SelectEndTime);
                } else {
                    _timeController.SelectRange(begin, _timeController.EndTime);
                }
            } else if(checkEnd) {
                if(_timeController.IsSelecting) {
                    _timeController.SelectRange(_timeController.SelectBeginTime, end);
                } else {
                    _timeController.SelectRange(_timeController.BeginTime, end);
                }
            }
            this.DialogResult = DialogResult.OK;
        }
    }
}
