using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MotionDataHandler.Misc {
    public partial class DialogSimpleSelect : DialogOKCancel {
        readonly List<RadioButton> _radios = new List<RadioButton>();
        public int SelectedIndex;
        public DialogSimpleSelect(string title, int initialSelectedIndex, IList<string> radioButtonTexts, IList<bool> enabledList) {
            InitializeComponent();
            if(enabledList == null) {
                // 全部true
                enabledList = radioButtonTexts.Select(x => true).ToList();
            }
            if(enabledList.Count < radioButtonTexts.Count) {
                // 数が足りなければfalseを追加
                IList<bool> newEnabledList = new bool[radioButtonTexts.Count];
                for(int i = 0; i < enabledList.Count; i++) {
                    newEnabledList[i] = enabledList[i];
                }
                enabledList = newEnabledList;
            }
            this.Text = title;
            buttonOK.Enabled = false;
            for(int i = radioButtonTexts.Count - 1; i >= 0; i--) {
                // デリゲート用一時変数
                int copy_i = i;
                RadioButton radio = new RadioButton();
                radio.Dock = DockStyle.Top;
                radio.Text = radioButtonTexts[i];
                radio.Enabled = enabledList[i];
                radio.CheckedChanged += (sender, e) => {
                    if(radio.Checked) {
                        this.SelectedIndex = copy_i;
                        buttonOK.Enabled = true;
                    }
                };
                if(i == initialSelectedIndex) {
                    radio.Checked = true;
                    this.SelectedIndex = i;
                    buttonOK.Enabled = true;
                }
                panel1.Controls.Add(radio);
            }
        }
        public DialogSimpleSelect(string title, params string[] radioButtonTexts)
            : this(title, 0, radioButtonTexts, null) {

        }
        public DialogResult ShowDialog(out int selectedIndex) {
            DialogResult ret = this.ShowDialog();
            selectedIndex = this.SelectedIndex;
            return ret;
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
