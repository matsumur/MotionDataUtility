using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MotionDataHandler.Sequence {
    public partial class LabelReplaceControl : UserControl {
        public LabelReplaceControl() {
            InitializeComponent();
            dataGridView_SelectionChanged(this, new EventArgs());
        }

        SequenceData _sequence;
        public void AttachSequenceData(SequenceData sequence) {
            _sequence = sequence;
            _map.Clear();
            if(_sequence != null) {
                foreach(string label in _sequence.Borders.GetLabelNames(false)) {
                    _map.Add(label, label);
                }
            }
            updateDataGridView();
        }

        Dictionary<string, string> _map = new Dictionary<string, string>();

        public IDictionary<string, string> GetReplaceMap() {
            return _map.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public event EventHandler ReplaceMapChanged;

        private void doReplaceMapChanged() {
            EventHandler tmp = this.ReplaceMapChanged;
            if(tmp != null) {
                tmp.Invoke(this, new EventArgs());
            }
        }

        public void ClearReplaceMap() {
            foreach(string label in _map.Keys.ToList()) {
                _map[label] = label;
            }
            updateDataGridView();
            doReplaceMapChanged();
        }

        public void AddReplaceMap(IDictionary<string, string> map) {
            HashSet<string> labels = new HashSet<string>(_sequence.Borders.GetLabelNames(true));
             foreach(var pair in map) {
                _map[pair.Key] = pair.Value ?? "";
            }
            updateDataGridView();
            doReplaceMapChanged();
        }
        public void AddReplaceMap(string key, string value) {
            _map[key] = value;
            updateDataGridView();
            doReplaceMapChanged();
        }

        private void doDataGridViewChanged() {
            _map.Clear();
            foreach(DataGridViewRow row in dataGridView.Rows) {
                DataGridViewTextBoxCell org = row.Cells[0] as DataGridViewTextBoxCell;
                DataGridViewTextBoxCell to = row.Cells[1] as DataGridViewTextBoxCell;
                if(org != null && to != null) {
                    _map.Add(org.Value.ToString(), to.Value.ToString());
                }
            }
            doReplaceMapChanged();
        }
        private void updateDataGridView() {
            List<int> selectedIndices = new List<int>();
            foreach(DataGridViewRow row in dataGridView.SelectedRows) {
                selectedIndices.Add(row.Index);
            }
            dataGridView.Rows.Clear();
            int index = 0;
            foreach(var pair in _map) {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell org = new DataGridViewTextBoxCell();
                DataGridViewTextBoxCell to = new DataGridViewTextBoxCell();
                if(pair.Key != null) {
                    org.Value = pair.Key;
                    to.Value = pair.Value ?? pair.Key;
                }
                row.Cells.Add(org);
                row.Cells.Add(to);
                dataGridView.Rows.Add(row);
                row.Selected = selectedIndices.Contains(index);
                index++;
            }
        }
        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
            doDataGridViewChanged();
        }

        private const string _defaultTextForMultipleLabels = "...";

        private void dataGridView_SelectionChanged(object sender, EventArgs e) {
            string first = null;
            bool unique = true;
            foreach(DataGridViewRow row in dataGridView.SelectedRows) {
                DataGridViewTextBoxCell org = row.Cells[0] as DataGridViewTextBoxCell;
                DataGridViewTextBoxCell to = row.Cells[1] as DataGridViewTextBoxCell;
                string label = to.Value.ToString();
                if(first == null) {
                    first = label;
                } else {
                    if(first != label) {
                        unique = false;
                    }
                }
            }
            if(first == null) {
                textBoxReplace.Enabled = false;
                buttonReplace.Enabled = false;
                buttonRestore.Enabled = false;
            } else {
                textBoxReplace.Enabled = true;
                buttonReplace.Enabled = true;
                buttonRestore.Enabled = true;
                if(unique) {
                    textBoxReplace.Text = first ?? "";
                } else {
                    textBoxReplace.Text = _defaultTextForMultipleLabels;
                }
            }
        }

        private void buttonReplace_Click(object sender, EventArgs e) {
            foreach(DataGridViewRow row in dataGridView.SelectedRows) {
                DataGridViewTextBoxCell org = row.Cells[0] as DataGridViewTextBoxCell;
                DataGridViewTextBoxCell to = row.Cells[1] as DataGridViewTextBoxCell;
                string label = org.Value.ToString();
                _map[label] = textBoxReplace.Text;
            }
            updateDataGridView();
            doReplaceMapChanged();
        }

        private void buttonRestore_Click(object sender, EventArgs e) {
            foreach(DataGridViewRow row in dataGridView.SelectedRows) {
                DataGridViewTextBoxCell org = row.Cells[0] as DataGridViewTextBoxCell;
                DataGridViewTextBoxCell to = row.Cells[1] as DataGridViewTextBoxCell;
                string label = org.Value.ToString();
                _map[label] = label;
            }
            updateDataGridView();
            doReplaceMapChanged();
        }

        private void textBoxReplace_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            if(e.KeyData == Keys.Enter) {
                buttonReplace_Click(sender, e);
                e.IsInputKey = true;
            }
        }
    }
}
