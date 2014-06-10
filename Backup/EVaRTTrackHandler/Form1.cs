using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MotionDataHandler;
using MotionDataHandler.Misc;
using MotionDataHandler.DataIO;
namespace EVaRTTrackHandler {
    public partial class Form1 : Form {
        string currentFolder;

        public Form1() {
            InitializeComponent();
            selectCurrentFolder(Directory.GetCurrentDirectory());
            addTrackSecond(new string[0]);
            buttonEnamble();
        }

        /// <summary>
        /// ファイル一覧を更新する。
        /// </summary>
        private void refreshFileList() {
            listFiles.Items.Clear();
            string[] files = Directory.GetFiles(currentFolder, "*.trc");
            foreach (var file in files) {
                listFiles.Items.Add(Path.GetFileName(file));
            }
        }

        /// <summary>
        /// カレントフォルダを変更する。ついでにファイル一覧を更新する。
        /// </summary>
        /// <param title="folder"></param>
        private void selectCurrentFolder(string folder) {
            dialogSelectFolder.SelectedPath = folder;
            dialogTrackFile.InitialDirectory = folder;
            dialogTrackSecond.InitialDirectory = folder;
            dialogCombineOutput.InitialDirectory = folder;
            dialogTrimOutput.InitialDirectory = folder;
            textFolder.Text = folder;
            currentFolder = folder;
            refreshFileList();
            buttonEnamble();
        }

        private void buttonListRefresh_Click(object sender, EventArgs e) {
            refreshFileList();
        }


        private void buttonSelectFolder_Click(object sender, EventArgs e) {
            if (dialogSelectFolder.ShowDialog() == DialogResult.OK) {
                selectCurrentFolder(dialogSelectFolder.SelectedPath);
            }
        }

        private void listFiles_SelectedValueChanged(object sender, EventArgs e) {

        }

        private void buttonSetMain_Click(object sender, EventArgs e) {
            if (listFiles.SelectedItem != null) {
                textTrack.Text = Path.Combine(currentFolder, listFiles.SelectedItem as string);
            }
        }

        private void buttonSetSecond_Click(object sender, EventArgs e) {
            if (listFiles.SelectedItem != null) {
                addTrackSecond(new string[1]{ Path.Combine(currentFolder, listFiles.SelectedItem as string)});
            }

        }
        private void buttonBrowseMain_Click(object sender, EventArgs e) {
            if (dialogTrackFile.ShowDialog() == DialogResult.OK) {
                textTrack.Text = dialogTrackFile.FileName;
            }
        }

        private void refreshDialogDefaultPath() {
            if (textTrack.Text != "" && listTrackSecond.Items.Count > 0) {
                try {
                    string combineOutput = Path.Combine(currentFolder, new StringBuilder().AppendFormat("{0}.Merged.trc", Path.GetFileNameWithoutExtension(textTrack.Text)).ToString());
                    textOutput.Text = combineOutput;
                    dialogCombineOutput.FileName = Path.GetFileName(combineOutput);
                    dialogConcatOutput.FileName = new StringBuilder().AppendFormat("{0}.Concat.trc", Path.GetFileNameWithoutExtension(textTrack.Text)).ToString();
                    dialogTrimOutput.FileName = new StringBuilder().AppendFormat("{0}.Trimmed.trc", Path.GetFileNameWithoutExtension(textTrack.Text)).ToString();
                } catch (ArgumentException) { }
            }
            if (textTrack.Text != "") {
                dialogPhaseSpaceOutput.InitialDirectory = currentFolder;
                dialogPhaseSpaceOutput.FileName = Path.GetFileNameWithoutExtension(textTrack.Text) + ".csv";
            }
        }

        /// <summary>
        /// ファイルパスが変更されるとついでにTracked ASCIIファイルのヘッダをパースします。
        /// </summary>
        /// <param title="sender"></param>
        /// <param title="e"></param>
        private void textTrack_TextChanged(object sender, EventArgs e) {
            viewTrack.Items.Clear();
            if (File.Exists(textTrack.Text)) {
                using (TrcReader reader = new TrcReader(textTrack.Text)) {
                    try {
                        autoAddViewTrack("Loaded", Path.GetFileName(textTrack.Text));
                        autoAddViewTrack("PathFileType", reader.Header.PathFileType.ToString());
                        autoAddViewTrack("AxisOrder", reader.Header.AxisOrder.ToString());
                        autoAddViewTrack("FilePath", reader.Header.FilePath.ToString());
                        autoAddViewTrack("DataRate", reader.Header.DataRate.ToString());
                        autoAddViewTrack("CameraRate", reader.Header.CameraRate.ToString());
                        autoAddViewTrack("NumFrames", reader.Header.NumFrames.ToString());
                        autoAddViewTrack("NumMarkers", reader.Header.NumMarkers.ToString());
                        autoAddViewTrack("Units", reader.Header.Units.ToString());
                        autoAddViewTrack("OrigDataRate", reader.Header.OrigDataRate.ToString());
                        autoAddViewTrack("OrigDataStartFrame", reader.Header.OrigDataStartFrame.ToString());
                        autoAddViewTrack("OrigNumFrames", reader.Header.OrigNumFrames.ToString());
                        StringBuilder builder = new StringBuilder();
                        foreach (var marker in reader.Header.Markers) { builder.Append(marker).Append(" "); }
                        autoAddViewTrack("Markers", builder.ToString());
                    } catch (Exception ex) {
                        autoAddViewTrack("Load Error", Path.GetFileName(textTrack.Text));
                        autoAddViewTrack("Cause", ex.Message);
                    }
                }
            } else {
                autoAddViewTrack("File Not Found", Path.GetFileName(textTrack.Text));
            }
            refreshDialogDefaultPath();
            buttonEnamble();
        }

        private void autoAddViewTrack(string data, string value) {
            ListViewItem lData = new ListViewItem(data);
            ListViewItem.ListViewSubItem lValue = new ListViewItem.ListViewSubItem(lData, value);
            lData.ToolTipText = value;
            lData.SubItems.Add(lValue);
            viewTrack.Items.Add(lData);
        }

        private void buttonSplit_Click(object sender, EventArgs e) {
            try {
                bool ok = true;
                // 上書きチェック。
                if (File.Exists(TrackHandler.GetSplitFilename(textTrack.Text, 1))) {
                    if (MessageBox.Show("Splitted File Already Exists. Ok to Overwrite?", "Overwrite", MessageBoxButtons.OKCancel) == DialogResult.Cancel) {
                        ok = false;
                    }
                }
                if (ok) {
                    if (TrackHandler.SplitTrackFile(textTrack.Text, (int)numSplit.Value)) {
                        MessageBox.Show("Succeeded to Split", "Notice");
                        refreshFileList();
                    } else {
                        MessageBox.Show("No Need to Split", "Notice");
                    }
                }
            } catch (Exception ex) {
                ErrorLogger.Tell(ex, "Failed");
            }
        }

        private void addTrackSecond(string[] items) {
            addTrackSecond(items, 0);
        }
        private void addTrackSecond(string[] items, int offset) {
            int index = listTrackSecond.Items.Count;
            if (listTrackSecond.SelectedIndices.Count > 0) {
                foreach (var i in listTrackSecond.SelectedIndices) {
                    int index2 = (int)i;
                    if (index2 < index) index = index2;
                }
            }
            List<object> removes = new List<object>();
            foreach (var item in listTrackSecond.SelectedItems) removes.Add(item);
            foreach (var item in removes) {
                listTrackSecond.Items.Remove(item);
            }
            if (index > listTrackSecond.Items.Count) index = listTrackSecond.Items.Count;
            index += offset;
            if (index > listTrackSecond.Items.Count) index = listTrackSecond.Items.Count;
            if (index < 0) index = 0;
            foreach (var item in items) {
                if (!listTrackSecond.Items.Contains(item)) {
                    listTrackSecond.Items.Insert(index, item);
                    listTrackSecond.SetSelected(index, true);
                index++;
                }
            }
            if (listTrackSecond.Items.Contains("")) {
                listTrackSecond.Items.Remove("");
            }
            listTrackSecond.Items.Add("");
        }

        /// <summary>
        /// MainのトラックファイルとSecondaryのトラックファイル群をまとめたstring配列を取得します
        /// </summary>
        /// <returns>string[]</returns>
        private string[] getTrackFiles() {
            List<string> inputs = new List<string>();
            inputs.Add(textTrack.Text);
            foreach (var file in listTrackSecond.Items) {
                string item = file as string;
                if (item != "") {
                    inputs.Add(item);
                }
            }
            return inputs.ToArray();
        }

        private void buttonBrowseSecond_Click(object sender, EventArgs e) {
            if (dialogTrackSecond.ShowDialog() == DialogResult.OK) {
                addTrackSecond(dialogTrackSecond.FileNames);
            }
        }

        private void enableConmine() {
            if (listTrackSecond.Items.Count > 0 && File.Exists(textTrack.Text)) {
                buttonCombine.Enabled = true;
            } else {
                buttonCombine.Enabled = false;
            }
        }

        private void buttonCombine_Click(object sender, EventArgs e) {
            if (dialogCombineOutput.ShowDialog() == DialogResult.OK) {
                textOutput.Text = dialogCombineOutput.FileName;
                try {
                    if (TrackHandler.JoinTracks(getTrackFiles(), textOutput.Text)) {
                        MessageBox.Show("Succeeded to Combine", "Notice");
                        refreshFileList();
                    } else {
                        MessageBox.Show("Error", "Failed");
                    }
                } catch (Exception ex) {
                    ErrorLogger.Tell(ex, "Failed");
                }
            }
        }

        private void textTrackSecond_TextChanged(object sender, EventArgs e) {
            refreshDialogDefaultPath();
        }

        private void listFiles_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (listFiles.SelectedItem != null) {
                textTrack.Text = Path.Combine(currentFolder, listFiles.SelectedItem as string);
            }
        }

        private void buttonConcat_Click(object sender, EventArgs e) {
            if (dialogConcatOutput.ShowDialog() == DialogResult.OK) {
                textOutput.Text = dialogConcatOutput.FileName;
                try {
                    if (TrackHandler.ConcatTrackFile(getTrackFiles(), textOutput.Text)) {
                        MessageBox.Show("Succeeded to Concatinate", "Notice");
                        refreshFileList();
                    } else {
                        MessageBox.Show("Error", "Failed");
                    }
                } catch (Exception ex) {
                    ErrorLogger.Tell(ex, "Failed");
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e) {

        }

        private void listTrackSecond_SelectedIndexChanged(object sender, EventArgs e) {
            
        }

        private void buttonDelete_Click(object sender, EventArgs e) {
            addTrackSecond(new string[0]);
        }

        private void buttonUp_Click(object sender, EventArgs e) {
            List<string> items = new List<string>();
            foreach (var item in listTrackSecond.SelectedItems) items.Add(item as string);
            addTrackSecond(items.ToArray(), -1);
        }

        private void buttonDown_Click(object sender, EventArgs e) {
            List<string> items = new List<string>();
            foreach (var item in listTrackSecond.SelectedItems) items.Add(item as string);
            addTrackSecond(items.ToArray(), 1);
        }

        private void buttonPhaseSpace_Click(object sender, EventArgs e) {
            if (File.Exists(textTrack.Text)) {
                if (dialogPhaseSpaceOutput.ShowDialog() == DialogResult.OK) {
                    try {
                        TrackHandler.OutputAsPhaseSpace(textTrack.Text, dialogPhaseSpaceOutput.FileName, numLocaltime.Value);
                        MessageBox.Show("Succeeded to Output", "Notice");
                        refreshFileList();
                    } catch (Exception ex) {
                        ErrorLogger.Tell(ex, "Failed");
                    }
                }
            }
        }



        private void numLocaltime_ValueChanged(object sender, EventArgs e) {
        }


        private void buttonEnamble() {
            bool condFileList = listFiles.SelectedItem == null;
            buttonSetMain.Enabled = !condFileList;
            buttonSetSecond.Enabled = !condFileList;
            bool condMainExists = File.Exists(textTrack.Text);
            buttonSplit.Enabled = condMainExists;
            buttonCombine.Enabled = condMainExists;
            buttonConcat.Enabled = condMainExists;
            buttonPhaseSpace.Enabled = condMainExists;
        }

        private void listFiles_SelectedIndexChanged(object sender, EventArgs e) {
            buttonEnamble();
        }

        private void buttonCutAdd_Click(object sender, EventArgs e) {
            if (File.Exists(textTrack.Text)) {
                dialogCutAddOutput.FileName = new StringBuilder().AppendFormat("{0}.{1}{2}.trc", Path.GetFileNameWithoutExtension(textTrack.Text), radioCut.Checked ? "Cut" : "Add", radioHead.Checked ? "Head" : "Tail").ToString();
                if (dialogCutAddOutput.ShowDialog() == DialogResult.OK) {
                    if (radioCut.Checked) {
                        TrackHandler.CutFramesTrackFile(textTrack.Text, dialogCutAddOutput.FileName, (int)numCutAdd.Value, radioTail.Checked);
                    } else {
                        TrackHandler.AddFramesTrackFile(textTrack.Text, dialogCutAddOutput.FileName, (int)numCutAdd.Value, radioTail.Checked);
                    }
                }
            }
        }

        private void textTrack_DragDrop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0) {
                    this.textTrack.Text = files[0];
                }
            }
        }
    

        private void textTrack_DragEnter_1(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void textFolder_TextChanged(object sender, EventArgs e) {

        }

        private void textFolder_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void textFolder_DragDrop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0) {
                    selectCurrentFolder(Path.GetDirectoryName(files[0]));
                }
            }
        }

        private void listFiles_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void listFiles_DragDrop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0) {
                    selectCurrentFolder(Path.GetDirectoryName(files[0]));
                }
            }
        }

        private void buttonTrimUnnamed_Click(object sender, EventArgs e) {
            if (dialogTrimOutput.ShowDialog() == DialogResult.OK) {
                textOutput.Text = dialogTrimOutput.FileName;
                try {
                    if (TrackHandler.TrimUnnamedFile(textTrack.Text, dialogTrimOutput.FileName)) {
                        MessageBox.Show("Succeeded to Trim", "Notice");
                        refreshFileList();
                    } else {
                        MessageBox.Show("Error", "Failed");
                    }
                } catch (Exception ex) {
                    ErrorLogger.Tell(ex, "Failed");
                }
            }
        }

        private void dialogTrimOutput_FileOk(object sender, CancelEventArgs e) {

        }

    }
}
