using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MotionDataHandler;
using MotionDataHandler.Script;
using MotionDataHandler.Script.Parse;
using System.IO;
using MotionDataHandler.Misc;

namespace MotionDataUtil {
    public partial class ScriptControlForm : Form {
        public ScriptControlForm() {
            MotionDataUtilSettings.Singleton.Initialize();
            ScriptConsole.Singleton.HistoryChanged += new EventHandler(Singleton_HistoryChanged);
            InitializeComponent();
        }

        void Singleton_HistoryChanged(object sender, EventArgs e) {
            if(this.InvokeRequired) {
                this.Invoke(new EventHandler(Singleton_HistoryChanged), sender, e);
                return;
            }
            UpdateHistory();
        }

        public void UpdateHistory() {
            /*
            StringBuilder str = new StringBuilder();
            foreach(string row in ScriptConsole.Singleton.EnumerateSerializedCallHistory(!checkNoUseReturnValue.Checked, new TimeSpan(0, 5, 0))) {
                str.AppendLine(row);
            }
            */
            string ret = ScriptConsole.Singleton.GetSerializedCallHistory(!checkNoUseReturnValue.Checked, false, new TimeSpan(0, 5, 0));
            try {
                textHistory.MaxLength = ret.Length;
            } catch(ArgumentOutOfRangeException) { }
            textHistory.Text = ret;
            textHistory.Select(textHistory.TextLength, 0);
            textHistory.ScrollToCaret();
        }

        private void ScriptControlForm_Load(object sender, EventArgs e) {
            numHistoryCapacity.Value = ScriptConsole.Singleton.HistoryCapacity;
            this.UpdateHistory();
            setScriptChanged(false);
            IList<string> debugs = ScriptConsole.Singleton.GetDebugInfoList();
            if(debugs.Count > 0) {
                textResult.AppendText("*** Debug Message ***");
                textResult.AppendText("\r\n");
                foreach(string debug in debugs) {
                    textResult.AppendText(debug);
                    textResult.AppendText("\r\n");
                }
            }
        }

        private void writeToOutput(string text) {
            if(this.InvokeRequired) {
                this.Invoke(new Action<string>(writeToOutput), text);
                return;
            }
            if(text == null)
                return;
            textResult.AppendText(text);
        }
        private void writeToOutput(object sender, ScriptMessageEventArgs e) {
            writeToOutput(e.Message);
        }

        private void buttonRun_Click(object sender, EventArgs e) {
            StringReader reader = new StringReader(textScript.Text);
            textResult.Text = "";
            ScriptConsole.Singleton.Out += new EventHandler<ScriptMessageEventArgs>(writeToOutput);
            try {
                ScriptConsole.Singleton.ParentControl = this;
                writeToOutput(ScriptConsole.Singleton.ExecuteThread(reader));
            } finally {
                ScriptConsole.Singleton.Out -= new EventHandler<ScriptMessageEventArgs>(writeToOutput);
            }
        }

        private void numHistoryCapacity_Validated(object sender, EventArgs e) {
            ScriptConsole.Singleton.HistoryCapacity = (int)numHistoryCapacity.Value;
        }

        private void ScriptControlForm_FormClosed(object sender, FormClosedEventArgs e) {
            MotionDataUtilSettings.Singleton.Save();
            ScriptConsole.Singleton.HistoryChanged -= Singleton_HistoryChanged;
        }

        private void buttonClear_Click(object sender, EventArgs e) {
            ScriptConsole.Singleton.ClearHistory();
        }

        private void checkNoUseReturnValue_CheckedChanged(object sender, EventArgs e) {
            this.UpdateHistory();
        }

        private void menuClose_Click(object sender, EventArgs e) {
            this.Close();
        }

        private string _savePath = null;
        private bool _scriptChanged = false;
        private void setScriptChanged(bool changed) {
            _scriptChanged = changed;
            if(_savePath != null) {
                this.Text = global::MotionDataUtil.Properties.Settings.Default.ScriptBoardTitle + " - " + Path.GetFileNameWithoutExtension(_savePath);
            }
            buttonSave.Enabled = menuSave.Enabled = _scriptChanged;
        }

        private void menuOpen_Click(object sender, EventArgs e) {
            if(openFileDialog.ShowDialog() == DialogResult.OK) {
                openFile(openFileDialog.FileName);
            }
        }

        private void openFile(string path) {
            if(closeScript()) {
                try {
                    using(StreamReader reader = new StreamReader(path)) {
                        textScript.Text = "";
                        while(true) {
                            string line = reader.ReadLine();
                            if(line == null)
                                break;
                            line += "\r\n";
                            if(textScript.Text.Length + line.Length > textScript.MaxLength) {
                                textScript.MaxLength = Math.Max(textScript.MaxLength * 2, textScript.MaxLength + line.Length);
                            }
                            textScript.AppendText(line);
                        }
                    }
                    _savePath = path;
                    saveFileDialog.InitialDirectory = Path.GetDirectoryName(path);
                    saveFileDialog.FileName = Path.GetFileName(path);
                    setScriptChanged(false);
                } catch(Exception ex) {
                    ErrorLogger.Tell(ex, "ファイルが開けません");
                }
            }
        }

        private void newFile() {
            closeScript();
        }

        private void menuSaveAs_Click(object sender, EventArgs e) {
            askSaveFile(false);
        }

        private bool closeScript() {
            bool ret = true;
            if(_scriptChanged) {
                DialogResult result = MessageBox.Show("スクリプトが変更されています．保存しますか", typeof(ScriptControlForm).Name, MessageBoxButtons.YesNoCancel);
                switch(result) {
                case DialogResult.Yes:
                    if(!askSaveFile(true)) {
                        ret = false;
                    }
                    break;
                case DialogResult.No:
                    break;
                case DialogResult.Cancel:
                    ret = false;
                    break;
                }
            }
            textScript.Text = "";
            setScriptChanged(!ret);
            _savePath = null;
            return ret;
        }

        private void ScriptControlForm_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = !closeScript();
        }

        private bool askSaveFile(bool overwrite) {
            try {
                if(overwrite && _savePath != null && _scriptChanged) {
                    saveFile(_savePath);
                } else {
                    if(saveFileDialog.ShowDialog() == DialogResult.OK) {
                        saveFile(saveFileDialog.FileName);
                    }
                }
                return true;
            } catch(Exception ex) {
                ErrorLogger.Tell(ex, "ファイルを保存できません");
                return false;
            }
        }
        private void saveFile(string path) {
            using(StreamWriter writer = new StreamWriter(path)) {
                writer.Write(textScript.Text);
            }
            saveFileDialog.InitialDirectory = Path.GetDirectoryName(path);
            saveFileDialog.FileName = Path.GetFileName(path);
            _savePath = path;
            setScriptChanged(false);
        }

        private void menuSave_Click(object sender, EventArgs e) {
            askSaveFile(true);
        }

        private void textScript_TextChanged(object sender, EventArgs e) {
            setScriptChanged(true);
        }

        private void buttonCopyHistory_Click(object sender, EventArgs e) {
            Clipboard.SetData(DataFormats.Text, textHistory.Text);
        }

        private void menuNewFile_Click(object sender, EventArgs e) {
            closeScript();
        }

        private void testToolStripMenuItem1_Click(object sender, EventArgs e) {
        }

        private void textScript_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
        }

        private void timerCaretPos_Tick(object sender, EventArgs e) {
            if(textScript.SelectionStart != -1) {
                int pos = textScript.SelectionStart;
                int line = textScript.GetLineFromCharIndex(pos);
                int head = textScript.GetFirstCharIndexFromLine(line);
                labelCursor.Text = string.Format("{1} 行 {0} 列", pos - head + 1, line + 1);
            }
        }


    }
}
