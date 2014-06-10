using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MotionDataHandler.Misc;

namespace MotionDataHandler.Sequence {
    public partial class LabelJumpForm : Form {
        ListViewItemComparer _listResultSorter = new ListViewItemComparer();
        public LabelJumpForm() {
            InitializeComponent();
            listResult.Enabled = false;
            setViewMode();
            _listResultSorter.ColumnModes = new ListViewItemComparer.ComparerMode[]{
 ListViewItemComparer.ComparerMode.Decimal ,
 ListViewItemComparer.ComparerMode.Decimal ,
 ListViewItemComparer.ComparerMode.Decimal ,
};
            listResult.ListViewItemSorter = _listResultSorter;
        }
        string _prevSequenceName = null;

        private SequenceViewerController _controller;
        public void AttachController(SequenceViewerController controller) {
            _controller = controller;
            labelSequenceSelectControl1.AttachController(_controller, v => v.Type != SequenceType.Numeric);
        }
        public void DetachController() {
            _controller = null;
        }

        private void listResult_SelectedIndexChanged(object sender, EventArgs e) {
            if(_controller == null)
                return;
            var items = listResult.SelectedItems;
            if(items.Count != 0) {
                var item = items[0];
                decimal borderBegin, borderEnd;
                if(decimal.TryParse(item.SubItems[0].Text, out borderBegin)
                    && decimal.TryParse(item.SubItems[1].Text, out borderEnd)) {
                    decimal borderDuration = borderEnd - borderBegin;
                    decimal rangeBegin, rangeEnd;
                    if(radioButtonMarginRatio.Checked) {
                        decimal margin = numericUpDownMarginRatio.Value;
                        rangeBegin = borderBegin - borderDuration * margin / 200M;
                        rangeEnd = borderEnd + borderDuration * margin / 200M;
                    } else if(radioButtonMarginSec.Checked) {
                        decimal margin = numericUpDownMarginSec.Value;
                        rangeBegin = borderBegin - margin;
                        rangeEnd = borderEnd + margin;
                    } else {
                        decimal sec = numericUpDownFixedSec.Value;
                        rangeBegin = borderBegin - sec / 8M;
                        rangeEnd = rangeBegin + sec;
                    }
                    decimal rangeDuration = rangeEnd - rangeBegin;
                    _controller.BroadcastVisibleRange(rangeBegin, rangeDuration);
                    _controller.SelectRange(borderBegin, borderEnd);
                    _controller.CurrentTime = borderBegin;
                }
            }
        }

        private void labelSequenceSelectControl1_SelectedIndexChanged(object sender, EventArgs e) {
            var items = labelSequenceSelectControl1.SelectedItems;
            bool enable = items.Count > 0;
            borderSelectControl1.Enabled = enable;
            listResult.Enabled = false;
            buttonNext.Enabled = false;
            if(!enable) {
                return;
            }
            _prevSequenceName = items[0].Title;
            borderSelectControl1.AttachBorder(items[0].Borders);
        }

        private void borderSelectControl1_SelectedIndexChanged(object sender, EventArgs e) {
            var labelText = (string)borderSelectControl1.SelectedItem;
            var items = labelSequenceSelectControl1.SelectedItems;
            if(labelText != null && items.Count != 0) {
                var labelSequence = items[0].GetLabelSequence();
                List<int> prevSelectedIndex = new List<int>();
                foreach(int prevIndex in listResult.SelectedIndices) {
                    prevSelectedIndex.Add(prevIndex);
                }
                listResult.Items.Clear();
                int index = 0;
                foreach(var label in labelSequence.EnumerateLabels()) {
                    if(label.LabelText == labelText) {
                        ListViewItem item = new ListViewItem(label.BeginTime.ToString("F3"));
                        ListViewItem.ListViewSubItem end = new ListViewItem.ListViewSubItem(item, label.EndTime.ToString("F3"));
                        ListViewItem.ListViewSubItem duration = new ListViewItem.ListViewSubItem(item, label.Duration.ToString("F3"));
                        item.SubItems.Add(end);
                        item.SubItems.Add(duration);
                        listResult.Items.Add(item);
                        if(prevSelectedIndex.Contains(index)) {
                            item.Selected = true;
                        }
                        index++;
                    }
                }
                listResult.Enabled = true;
                buttonNext.Enabled = true;
            } else {
                listResult.Enabled = false;
                buttonNext.Enabled = false;
            }
        }

        private void buttonNext_Click(object sender, EventArgs e) {
            var indices = listResult.SelectedIndices;
            if(indices.Count == 0) {
                if(listResult.Items.Count > 0) {
                    listResult.SelectedIndices.Add(0);
                }
            } else {
                int first = indices[0];
                int next = first + 1;
                if(next < listResult.Items.Count) {
                    listResult.SelectedIndices.Clear();
                    listResult.SelectedIndices.Add(next);
                    listResult.SelectedItems[0].EnsureVisible();
                } else {
                    MessageBox.Show("No next label");
                    listResult.SelectedIndices.Clear();
                }
            }
        }


        private void LabelJumpForm_Activated(object sender, EventArgs e) {
            AttachController(_controller);
        }

        private void borderSelectControl1_Click(object sender, EventArgs e) {
            listResult.SelectedIndices.Clear();
        }

        private void radioButtonMarginRatio_CheckedChanged(object sender, EventArgs e) {
            setViewMode();
        }

        private void radioButtonMarginSec_CheckedChanged(object sender, EventArgs e) {
            setViewMode();
        }

        private void radioButtonFixedSec_CheckedChanged(object sender, EventArgs e) {
            setViewMode();
        }
        private void setViewMode() {
            numericUpDownMarginRatio.Enabled = radioButtonMarginRatio.Checked;
            numericUpDownMarginSec.Enabled = radioButtonMarginSec.Checked;
            numericUpDownFixedSec.Enabled = radioButtonFixedSec.Checked;
        }

        private void listResult_ColumnClick(object sender, ColumnClickEventArgs e) {
            _listResultSorter.Column = e.Column;
            listResult.Sort();
        }
    }
}
