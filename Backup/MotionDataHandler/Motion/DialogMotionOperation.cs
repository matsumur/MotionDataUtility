using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MotionDataHandler.Sequence;
using MotionDataHandler.Misc;
using MotionDataHandler.Motion;
using MotionDataHandler.Motion.Operation;
using MotionDataHandler.Script;
using Microsoft.DirectX;

namespace MotionDataHandler.Motion {
    /// <summary>
    /// MotionObjectを作成する上でのテンプレート
    /// </summary>
    public partial class DialogMotionOperation : DialogOKCancel {
        private ScriptConsole _console;
        private IMotionOperationBase _operation;
        private MotionOperationExecution _exec;
        private IList<MotionObjectInfo> _targetInfoList;
        /// <summary>
        /// プライベートコンストラクタ
        /// </summary>
        private DialogMotionOperation() {
            InitializeComponent();
        }

        public DialogMotionOperation(ScriptConsole console, IMotionOperationBase operation)
            : this() {
            if(console == null)
                throw new ArgumentNullException("console", "'console' cannot be null");
            if(operation == null)
                throw new ArgumentNullException("operation", "'operation' cannot be null");
            _operation = operation;
            _console = console;
            _exec = new MotionOperationExecution(operation, _console);
            _targetInfoList = _console.MotionDataSet.GetSelectedObjectInfoList(info => _operation.FilterSelection(info));
        }

        private void DialogMotionOperation_FormClosed(object sender, FormClosedEventArgs e) {
            motionDataViewer.DetachDataSet();
            motionDataViewer.AttachTimeController(null);
        }

        private void DialogMotionOperation_Load(object sender, EventArgs e) {
            if(_console == null)
                return;
            if(_operation == null)
                return;
            // ダイアログのタイトルを適当に設定
            this.Text = _operation.GetTitle();
            // ビューの設定
            motionDataViewer.AttachDataSet(_console.MotionDataSet);
            motionDataViewer.AttachTimeController(TimeController.Singleton);
            // 引数の値が変わったらプレビューしなおすよう設定する
            foreach(ProcParam<MotionProcEnv> param in _exec.Parameters) {
                param.ValueChanged += new EventHandler(param_ValueChanged);
            }
            // 引数のパネルを作って表示
            Panel panel = _exec.GetPanel();
            panel.AutoScroll = true;
            panel.Dock = DockStyle.Fill;
            groupBoxSettings.Controls.Add(panel);
            
            // 選択されているオブジェクトが不適切だったらダイアログを閉じる
            string errorStr = "";
            if(!_operation.ValidateSelection(_targetInfoList, ref errorStr)) {
                MessageBox.Show("Error: " + (errorStr ?? "something wrong"));
                this.Close();
                return;
            }
            // 説明書きを処理オブジェクトからもらって表示
            textBoxDescription.Text = _operation.GetDescription();

            // 選択されているオブジェクトを表示
            listBoxTarget.Items.Clear();
            foreach(MotionObjectInfo info in _targetInfoList) {
                listBoxTarget.Items.Add(string.Format("{0} ({1})", info.Name, info.ObjectType.Name));
            }
            // プレビューを更新
            this.SetPreview();
            // 引数がない場合にダイアログを表示しないオプションが付いていたら勝手に実行する
            if(checkIgnoreForm.Checked && _exec.Parameters.Count == 0) {
                this.Invoke(new EventHandler(buttonOK_Click), sender, e);
            }
        }

        void param_ValueChanged(object sender, EventArgs e) {
            this.SetPreview();
        }

        /// <summary>
        /// CreateObjectの結果に基づいてプレビューを表示します．
        /// </summary>
        protected virtual void SetPreview() {
            if(_operation == null)
                return;
            // 処理の種類を判別したい
            IMotionOperationCreateObject createOpe = _operation as IMotionOperationCreateObject;
            IMotionOperationEditObject editOpe = _operation as IMotionOperationEditObject;
            IMotionOperationGeneral generalOpe = _operation as IMotionOperationGeneral;
            IMotionOperationOutputSequence outputOpe = _operation as IMotionOperationOutputSequence;

            // 前回のプレビュー用オブジェクトを消しておく
            motionDataViewer.ClearPreviewObjects();

            // 作成や編集でなければプレビューしない
            if(createOpe == null && editOpe == null) {
                panelPreview.Visible = false;
                return;
            }

            panelPreview.Visible = true;
            // 引数チェック．失敗したら表示しない
            string errorMessage = "";
            if(!_operation.ValidateArguments(_exec.Parameters, ref errorMessage))
                return;
            // フレーム取得．失敗したら表示しない
            MotionFrame frame = _console.MotionDataSet.GetFrameAt(TimeController.Singleton.CurrentTime);
            if(frame == null)
                return;
            // 作成/編集結果のオブジェクトと，プレビュー時のみに表示されるオブジェクトを分類する
            IList<MotionObject> previewObjs = null;
            IList<MotionObject> previewSubObjs = null;
            if(createOpe != null) {
                previewSubObjs = createOpe.CreateObjects(_targetInfoList, _exec.Parameters, new ReadOnlyMotionFrame(frame), true);
                previewObjs = createOpe.CreateObjects(_targetInfoList, _exec.Parameters, new ReadOnlyMotionFrame(frame), false);
            } else if(editOpe != null) {
                previewSubObjs = editOpe.EditObject(_targetInfoList, _exec.Parameters, new ReadOnlyMotionFrame(frame), true);
                previewObjs = editOpe.EditObject(_targetInfoList, _exec.Parameters, new ReadOnlyMotionFrame(frame), false);
            }
            if(previewObjs == null)
                return;
            if(previewSubObjs == null) {
                previewSubObjs = previewObjs;
            }
            if(previewSubObjs.Count >= previewObjs.Count) {
                previewSubObjs = previewSubObjs.Skip(previewObjs.Count).ToList();
            }
            // プレビュー対象をビューに登録しつつ，重心を求める
            Vector3 sum = Vector3.Empty;
            int count = 0;
            if(previewObjs != null) {
                foreach(var @object in previewObjs) {
                    if(@object != null) {
                        sum += @object.GravityPoint;
                        motionDataViewer.AddPreviewObject(@object, false);
                        count++;
                    }
                }
                foreach(var @object in previewSubObjs) {
                    if(@object != null) {
                        sum += @object.GravityPoint;
                        motionDataViewer.AddPreviewObject(@object, true);
                        count++;
                    }
                }
                if(editOpe != null) {
                    foreach(MotionObjectInfo info in _targetInfoList) {
                        motionDataViewer.AddPreviewRemoveObject(info);
                    }
                }
            }
            // ビューのカメラを重心の方に向ける
            if(count > 0) {
                sum *= 1f / count;
                motionDataViewer.ViewCamera.SetTarget(sum, false);
            }
        }


        private void buttonOK_Click(object sender, EventArgs e) {
            string errorMessage = "";
            if(!_operation.ValidateArguments(_exec.Parameters, ref errorMessage)) {
                MessageBox.Show(errorMessage ?? "Error");
                return;
            }
            if(_exec.OperateThread(this)) {
                this.DialogResult = DialogResult.OK;
            }
        }

    }
}
