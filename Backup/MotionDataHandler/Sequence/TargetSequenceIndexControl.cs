using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MotionDataHandler.Sequence {
    public partial class TargetSequenceIndexControl : UserControl {
        public TargetSequenceIndexControl() {
            InitializeComponent();
        }
        SequenceData _sequence;
        /// <summary>
        /// カラムを選択する対象となるSequenceValuesを関連付けます。
        /// </summary>
        /// <param title="resultSequence"></param>
        public void AttachSequence(SequenceData sequence) {
            if(sequence == null)
                throw new ArgumentNullException("sequence", "'sequence' cannot be null");
            _sequence = sequence;
            setContents();
        }

        private void setContents() {
            if(_sequence == null || _sequence.Values.ColumnCount == 0) {
                comboIndices.Enabled = false;
            } else {
                comboIndices.Items.Clear();
                if(_sequence.Values.ColumnCount > 0) {
                    comboIndices.Enabled = true;
                    foreach(var name in _sequence.Values.ColumnNames) {
                        comboIndices.Items.Add(name);
                    }
                    comboIndices.SelectedIndex = _sequence.Borders.TargetColumnIndex;
                } else {
                    comboIndices.Enabled = false;
                }
            }
        }
        private void TargetColumnIndexControl_Load(object sender, EventArgs e) {
        }

        /// <summary>
        /// 選択されたカラムのインデックスを取得または設定します。選択されていない場合は-1が返されます。
        /// </summary>
        public int SelectedIndex {
            get {
                if(!comboIndices.Enabled)
                    return -1;
                return comboIndices.SelectedIndex;
            }
            set { comboIndices.SelectedIndex = value; }
        }

        public int ItemCount {
            get { return comboIndices.Items.Count; }
        }
        /// <summary>
        /// SelectedIndexの値が変更された時に呼び出されるイベント
        /// </summary>
        [Category("動作"), Description("SelectedIndexの値が変更された時に呼び出されるイベント")]
        public event EventHandler SelectedIndexChanged;

        private void comboIndices_SelectedIndexChanged(object sender, EventArgs e) {
            if(_sequence != null && SelectedIndex != -1 && _sequence.Borders.TargetColumnIndex != SelectedIndex) {
                _sequence.Borders.TargetColumnIndex = SelectedIndex;
                if(this.SelectedIndexChanged != null) {
                    this.SelectedIndexChanged(this, e);
                }
            }
        }
    }
}
