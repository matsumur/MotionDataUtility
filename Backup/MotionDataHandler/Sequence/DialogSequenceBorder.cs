using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MotionDataHandler.Sequence {
    using Misc;

    public partial class DialogSequenceBorder : Form {
        SequenceData _sequence;
        public DialogSequenceBorder(SequenceData sequence, SequenceView viewer) {
            InitializeComponent();
            if(sequence == null)
                throw new ArgumentNullException("sequence", "'sequence' cannot be null");
            _sequence = sequence;
            _viewer = viewer;
            refreshList();
            refreshOutput();
            setDialogName();
            targetColumnIndexControl1.AttachSequence(sequence);
            
            //switch(sequence.Type) {
            //case SequenceType.Numeric:
            //    comboBoxSequenceType.SelectedIndex = 0;
            //    break;
            //case SequenceType.NumericLabel:
            //    comboBoxSequenceType.SelectedIndex = 1;
            //    break;
            //case SequenceType.Label:
            //    comboBoxSequenceType.SelectedIndex = 2;
            //    break;
            //}
        }
        SequenceView _viewer = null;
        Plugin.IPluginHost _pluginHost = null;
        public void AttachIPluginHost(Plugin.IPluginHost pluginHost) {
            _pluginHost = pluginHost;
        }

        private void setDialogName() {
            if(_sequence.Title == null || _sequence.Title == "")
                return;
            dialogOpenBorder.FileName = "border-" + _sequence.Title + "." + dialogOpenBorder.DefaultExt;
            dialogSaveBorder.FileName = "border-" + _sequence.Title + "." + dialogSaveBorder.DefaultExt;
        }

        private void numLowerEnd_KeyDown(object sender, KeyEventArgs e) {
            if(e.KeyData == Keys.Enter) {
                comboName.Focus();
            }
        }

        private void listBorders_SelectedIndexChanged(object sender, EventArgs e) {
            refreshEdit();
        }

        private void refreshList() {
            listBorders.Items.Clear();

            ListViewItem defaultItem = new ListViewItem(new string[] { "Default", _sequence.Borders.DefaultName });
            listBorders.Items.Add(defaultItem);
            foreach(var pair in _sequence.Borders.Enumerate()) {
                ListViewItem item = new ListViewItem(new string[] { pair.Key.ToString("F3"), pair.Value });
                listBorders.Items.Add(item);
            }
            comboName.Items.Clear();
            comboName.Items.AddRange(_sequence.Borders.GetLabelNames().ToArray());
            refreshEdit();
        }

        private void refreshEdit() {
            if(listBorders.SelectedIndices.Count == 0) {
                labelSelected.Text = "選択行: -";
                buttonRemove.Enabled = false;
                buttonReplace.Enabled = false;
                buttonAdd.Enabled = true;
                numLowerEnd.Enabled = true;
            } else {
                buttonAdd.Enabled = true;
                lock(_sequence) {
                    int index = listBorders.SelectedIndices[0];
                    if(index == 0) {
                        labelSelected.Text = "選択行: Default / " + _sequence.Borders.DefaultName;
                        comboName.Text = _sequence.Borders.DefaultName;
                        buttonRemove.Enabled = false;
                        buttonReplace.Enabled = true;
                        buttonAdd.Enabled = false;
                        numLowerEnd.Enabled = false;
                    } else {
                        if(index - 1 >= 0 && index - 1 < _sequence.Borders.BorderCount) {
                            var pair = _sequence.Borders.GetBorderByIndex(index - 1);
                            labelSelected.Text = string.Format("選択行: {0} / {1}", pair.Key.ToString("F3"), pair.Value);
                            try {
                                numLowerEnd.Value = pair.Key;
                                comboName.Text = pair.Value;
                            } catch(ArgumentOutOfRangeException) { }
                        }
                        buttonRemove.Enabled = true;
                        buttonReplace.Enabled = true;
                        buttonAdd.Enabled = true;
                        numLowerEnd.Enabled = true;
                    }
                }
            }
        }
        private void refreshOutput() {
            if(!_sequence.Values.HasColumns) {
                buttonOutput.Enabled = false;
            } else {
                buttonOutput.Enabled = true;
            }
        }
        private void addBorder() {
            using(_sequence.Lock.GetWriteLock()) {
                decimal key = numLowerEnd.Value;
                string name = comboName.Text;
                _sequence.Borders.SetBorder(key, name);
                if(_sequence.Type == SequenceType.Label) {
                    int borderIndex = _sequence.Borders.GetIndexFromValue(key);
                    decimal next = 0;
                    if(borderIndex + 1 >= _sequence.Borders.BorderCount) {
                        next = _sequence.Values.EndTime;
                    } else {
                        next = _sequence.Borders.GetBorderByIndex(borderIndex + 1).Key;
                    }
                    decimal?[] tmp = _sequence.Values.GetValueAt(next);
                    if(_sequence.Borders.TargetColumnIndex >= 0 && _sequence.Borders.TargetColumnIndex < _sequence.Values.ColumnCount) {
                        tmp[_sequence.Borders.TargetColumnIndex] = key;
                        _sequence.Values.SetRange(key, next, tmp);
                    }
                }
            }
            _sequence.IsDataChanged = true;
            refreshList();
        }

        private void replaceBorder() {
            if(listBorders.SelectedIndices.Count == 0)
                return;
            int index = listBorders.SelectedIndices[0];

            using(_sequence.Lock.GetWriteLock()) {
                decimal key = numLowerEnd.Value;
                string name = comboName.Text;
                if(index == 0) {
                    _sequence.Borders.DefaultName = name;
                } else if(index - 1 >= 0 && index - 1 < _sequence.Borders.BorderCount) {
                    _sequence.Borders.RemoveAt(index - 1);
                    _sequence.Borders.SetBorder(key, name);
                    if(_sequence.Type == SequenceType.Label) {
                        int borderIndex = _sequence.Borders.GetIndexFromValue(key);
                        decimal next = 0;
                        if(borderIndex + 1 >= _sequence.Borders.BorderCount) {
                            next = _sequence.Values.EndTime;
                        } else {
                            next = _sequence.Borders.GetBorderByIndex(borderIndex + 1).Key;
                        }
                        decimal?[] tmp = _sequence.Values.GetValueAt(next);
                        if(_sequence.Borders.TargetColumnIndex >= 0 && _sequence.Borders.TargetColumnIndex < _sequence.Values.ColumnCount) {
                            tmp[_sequence.Borders.TargetColumnIndex] = key;
                            _sequence.Values.SetRange(key, next, tmp);
                        }
                    }
                }
            }
            _sequence.IsDataChanged = true;
            refreshList();
        }

        private void removeBorder() {
            if(listBorders.SelectedIndices.Count == 0)
                return;
            int index = listBorders.SelectedIndices[0];
            if(index == 0)
                return;
            using(_sequence.Lock.GetWriteLock()) {
                if(index - 1 >= 0 && index - 1 < _sequence.Borders.BorderCount) {
                    _sequence.Borders.RemoveAt(index - 1);
                }
            }
            _sequence.IsDataChanged = true;
            refreshList();
        }

        private void buttonClose_Click(object sender, EventArgs e) {

        }

        private void buttonLoad_Click(object sender, EventArgs e) {
            try {
                if(dialogOpenBorder.ShowDialog() == DialogResult.OK) {
                    LabelingBorders newBorder = LabelingBorders.Deserialize(dialogOpenBorder.FileName);
                    _sequence.Borders = newBorder;
                    refreshList();
                    dialogSaveBorder.FileName = dialogOpenBorder.FileName;
                }
            } catch(Exception ex) {
                ErrorLogger.Tell(ex, "ファイルが読み込めませんでした");
            }
        }

        private void buttonSave_Click(object sender, EventArgs e) {

            try {
                if(dialogSaveBorder.ShowDialog() == DialogResult.OK) {
                    _sequence.Borders.Serialize(dialogSaveBorder.FileName);
                }
            } catch(Exception ex) {
                ErrorLogger.Tell(ex, "ファイルが書き込めませんでした");
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e) {
            addBorder();
        }

        private void buttonReplace_Click(object sender, EventArgs e) {
            replaceBorder();
        }

        private void buttonRemove_Click(object sender, EventArgs e) {
            removeBorder();
        }

        private void DialogTimeValueBorder_Load(object sender, EventArgs e) {

        }

        private void buttonOutput_Click(object sender, EventArgs e) {
            if(_viewer != null) {
                _viewer.OutputLabel();
            }
        }

        private void listBorders_DragEnter(object sender, DragEventArgs e) {
            if(e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void listBorders_DragDrop(object sender, DragEventArgs e) {
            if(e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if(files.Length > 0) {
                    try {
                        _sequence.Borders = LabelingBorders.Deserialize(files[0]);
                        refreshList();
                        dialogSaveBorder.FileName = dialogOpenBorder.FileName = files[0];
                    } catch(Exception ex) {
                        ErrorLogger.Tell(ex, "ファイルが読み込めませんでした");
                    }
                }
            }
        }

        public void SetNumLowerEnd(decimal value) {
            if(this.InvokeRequired) {
                this.Invoke(new Action<decimal>(SetNumLowerEnd), value);
                return;
            }
            if(value > numLowerEnd.Maximum)
                numLowerEnd.Maximum = value;
            if(value < numLowerEnd.Minimum)
                numLowerEnd.Minimum = value;
            numLowerEnd.Value = value;
        }

        private void comboName_KeyDown(object sender, KeyEventArgs e) {
            if(e.KeyData == Keys.Enter) {
                if(buttonReplace.Enabled) {
                    replaceBorder();
                } else if(buttonAdd.Enabled) {
                    addBorder();
                }
            }
        }

        private void numColumn_ValueChanged(object sender, EventArgs e) {
            if(targetColumnIndexControl1.SelectedIndex >= 0) {
                _sequence.Borders.TargetColumnIndex = targetColumnIndexControl1.SelectedIndex;
            }
        }

        private void targetColumnIndexControl1_SelectedIndexChanged(object sender, EventArgs e) {
            _sequence.IsDataChanged = true;
        }

        private void comboBoxSequenceType_SelectedIndexChanged(object sender, EventArgs e) {
            //switch(comboBoxSequenceType.SelectedIndex) {
            //case 0:
            //    _sequence.Type = SequenceType.Numeric;
            //    break;
            //case 1:
            //    _sequence.Type = SequenceType.NumericLabel;
            //    break;
            //case 2:
            //    _sequence.Type = SequenceType.Label;
            //    break;
            //}
        }

        private void label1_Click(object sender, EventArgs e) {

        }

    }
}
