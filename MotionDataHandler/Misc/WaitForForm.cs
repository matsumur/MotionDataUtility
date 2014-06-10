using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace MotionDataHandler.Misc {
    /// <summary>
    /// プログレスバーとバックグラウンドワーカーからなる，バックグラウンド処理用のダイアログ
    /// </summary>
    /// <example>
    /// WaitForForm waitForm = new WaitForForm(delegate(WaitForForm.WorkerController ctrl){
    ///     ctrl.OperationTitle = "処理名";
    ///     // ... バックグラウンド処理の内容 
    ///     ctrl.DialogResult = DialogResult.OK;
    /// }, delegate(){ return new ProgressChangedEventArgs(25, "処理中"); });
    /// if(waitForm.ShowDialog() == DialogResult.OK) {
    ///     // ...
    /// }
    /// </example>
    public partial class WaitForForm : Form {
        public class WorkerController {
            BackgroundWorker _worker;
            WaitForForm _form;
            DoWorkEventArgs _args;
            public WorkerController(BackgroundWorker worker, WaitForForm form, DoWorkEventArgs args) {
                _worker = worker;
                _form = form;
                _args = args;
            }
            public DialogResult DialogResult { get { return _form.DialogResult; } set { _form.DialogResult = value; } }
            public string FormTitle { get { return _form.Text; } set { _form.SetTitle(value); } }
            public bool Cancel { get { return _args.Cancel; } set { _args.Cancel = value; } }
            public bool CancelEnabled { get { return _form.CancelEnabled; } set { _form.CancelEnabled = value; } }
            public string OperationTitle { get { return _form.labelTitle.Text; } set { _form.SetOperationTitle(value); } }
            public void ReportProgress(int percentProgress) { _form.ReportProgress(percentProgress); }
            public void ReportProgress(int percentProgress, object userState) { _form.ReportProgress(percentProgress, userState.ToString()); }
            public ProgressChangedEventHandler OnProgressChanged { get { return _form.OnProgressChanged; } }
            //public Func<ProgressChangedEventArgs> PollingFunction { get { return _form._polling; } set { _form._polling = value; } }
        }
        Func<ProgressChangedEventArgs> _polling;
        DateTime _openTime = DateTime.MinValue;
        /// <summary>
        /// 標準のコンストラクタ。
        /// BackgoundWorkerからReportProgressを呼び出すことでプログレスバーが更新されます。
        /// </summary>
        /// <param title="backgoundWorkerWork">バックグラウンドワーカーの実行内容。バックグラウンドワーカー自身、WaitForForm自身(this)、バックグラウンドワーカー実行時のDoWorkEventArgsを引数にとります。</param>
        private WaitForForm(Action<BackgroundWorker, WaitForForm, DoWorkEventArgs> backgroundWorkerWork, Func<ProgressChangedEventArgs> polling)
            : this(backgroundWorkerWork, polling, false) {
        }
        bool _debug;
        private WaitForForm(Action<BackgroundWorker, WaitForForm, DoWorkEventArgs> backgroundWorkerWork, Func<ProgressChangedEventArgs> polling, bool debug) {
            InitializeComponent();
            if(backgroundWorkerWork == null)
                throw new ArgumentNullException("backgroundWorkerWork", "'backgroundWorkerWork' cannot be null");
            this.work = backgroundWorkerWork;
            this.labelWaitFor.Text = "Start Up";
            this.ProgressBar.Value = this.ProgressBar.Minimum;
            _polling = polling;
            this.CancelEnabled = false;
            _debug = debug;
        }
        /// <summary>
        /// 背景処理中に表示するダイアログのコンストラクタ
        /// </summary>
        /// <param name="backgoundWorkerWork"></param>
        public WaitForForm(Action<WaitForForm.WorkerController> backgoundWorkerWork)
            : this((bgw, form, e) => { backgoundWorkerWork.Invoke(new WorkerController(bgw, form, e)); }, null) { }
        public WaitForForm(Action<WaitForForm.WorkerController> backgoundWorkerWork, Func<ProgressChangedEventArgs> polling)
            : this((bgw, form, e) => { backgoundWorkerWork.Invoke(new WorkerController(bgw, form, e)); }, polling) { }
        public WaitForForm(Action<WaitForForm.WorkerController> backgoundWorkerWork, Func<ProgressChangedEventArgs> polling, bool debug)
            : this((bgw, form, e) => { backgoundWorkerWork.Invoke(new WorkerController(bgw, form, e)); }, polling, debug) { }

        private Action<BackgroundWorker, WaitForForm, DoWorkEventArgs> work;
        private bool _cancelEnabled;
        /// <summary>
        /// キャンセルボタンが有効かどうかを取得または設定します。
        /// </summary>
        public bool CancelEnabled {
            get { return _cancelEnabled; }
            set {
                _cancelEnabled = value;
                setButtonCancelEnabled(value);
            }
        }

        private void setButtonCancelEnabled(bool enabled) {
            if(this.InvokeRequired) {
                this.BeginInvoke(new Action<bool>(setButtonCancelEnabled), enabled);
                return;
            }
            buttonCancel.Enabled = enabled;
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            if(bgWorker.IsBusy) {
                bgWorker.CancelAsync();
            } else {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e) {
            if(work == null) {
                this.DialogResult = DialogResult.Abort;
                this.Close();
                return;
            }
            try {
                work(this.bgWorker, this, e);
            } catch(Exception ex) {
                if(_debug)
                    throw;
                MessageBox.Show("処理中にエラーが発生しました:\n" + ex.Message + "\n" + ex.StackTrace, ex.GetType().ToString());
                ErrorLogger.Log(ex, "Error");
            }
        }

        bool _notifyRequested = false;
        int _notifyPercentage;
        string _notifyString;
        public void OnProgressChanged(object sender, ProgressChangedEventArgs e) {
            if(e.UserState != null) {
                _notifyString = e.UserState.ToString();
            } else {
                _notifyString = "...";
            }
            _notifyPercentage = e.ProgressPercentage;
            _notifyRequested = true;
        }

        public void ReportProgress(int percentage) {
            this.ReportProgress(percentage, null);
        }
        public void ReportProgress(int percentage, object state) {
            _notifyRequested = true;
            if(state != null) {
                _notifyString = state.ToString();
            } else {
                _notifyString = "...";
            }
            _notifyPercentage = percentage;
            _notifyRequested = true;
        }

        bool setProgressRequested = false;
        private void setProgress(int percentage, string state) {
            if(this.IsDisposed)
                return;
            if(this.InvokeRequired) {
                if(setProgressRequested)
                    return;
                setProgressRequested = true;
                this.BeginInvoke(new Action<int, string>(setProgress), percentage, state);
                return;
            }
            setProgressRequested = false;
            this.labelWaitFor.Text = state;
            if(percentage < this.ProgressBar.Minimum)
                percentage = this.ProgressBar.Minimum;
            if(percentage > this.ProgressBar.Maximum)
                percentage = this.ProgressBar.Maximum;
            this.ProgressBar.Value = percentage;
        }

        private void WaitForForm_FormClosing(object sender, FormClosingEventArgs e) {
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            timerNotify.Stop();
            this.Close();
        }

        private void WaitForForm_Load(object sender, EventArgs e) {
            timerNotify.Start();
            labelWaitFor.Text = "Processing...";
            _openTime = DateTime.Now;
            timerBegin.Start();
        }

        private void timerBegin_Tick(object sender, EventArgs e) {
            timerBegin.Stop();
            bgWorker.RunWorkerAsync();
        }

        /// <summary>
        /// ダイアログの実行内容を表示させます。
        /// </summary>
        /// <param title="text">メッセージ</param>
        public void SetOperationTitle(string text) {
            if(this.InvokeRequired) {
                this.BeginInvoke(new Action<string>(SetOperationTitle), text);
                return;
            }
            labelTitle.Text = text;
        }

        /// <summary>
        /// ダイアログの実行内容を表示させます。
        /// </summary>
        /// <param title="text">メッセージ</param>
        public void SetTitle(string text) {
            setText(this, text);
        }

        private void setText(Control control, string text) {
            if(this.InvokeRequired) {
                this.BeginInvoke(new Action<Control, string>(setText), control, text);
                return;
            }
            control.Text = text;
        }

        private void timerNotify_Tick(object sender, EventArgs e) {
            Func<ProgressChangedEventArgs> polling = _polling;
            if(polling != null) {
                if(bgWorker.IsBusy) {
                    ProgressChangedEventArgs arg = polling();
                    object userState = arg.UserState;
                    if(userState == null)
                        userState = "...";
                    setProgress(arg.ProgressPercentage, userState.ToString());
                }
            }
            if(_notifyRequested) {
                setProgress(_notifyPercentage, _notifyString);
            }
            if(_openTime != DateTime.MinValue) {
                TimeSpan elapse = DateTime.Now - _openTime;
                if(elapse.TotalHours < 1) {
                    setText(labelElapse, string.Format("{0}:{1}", elapse.Minutes, elapse.Seconds.ToString("00")));
                } else {
                    setText(labelElapse, string.Format("{0}:{1}:{2}", Math.Floor(elapse.TotalHours), elapse.Minutes.ToString("00"), elapse.Seconds.ToString("00")));
                }
            }
        }

    }
}
