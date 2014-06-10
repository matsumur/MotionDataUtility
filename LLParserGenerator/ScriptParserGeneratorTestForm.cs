using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace LLParserGenerator {
    public partial class ScriptParserGeneratorTestForm : Form {
        public ScriptParserGeneratorTestForm() {
            InitializeComponent();
        }

        bool _modified;
        string _filename = null;
        string DisplayFilename { get { return _filename == null ? "<untitled>" : Path.GetFileName(_filename); } }
        ScriptParserGenerator _generator;
        const int _colorizeWait = 10;
        int _colorizeElapse = 0;
        private void timerColorize_Tick(object sender, EventArgs e) {
            if(_colorizeElapse > 0) {
                _colorizeElapse--;
                if(_colorizeElapse == 0) {
                    colorizeText();
                }
            }
        }

        private void textScript_TextChanged(object sender, EventArgs e) {
            _colorizeElapse = _colorizeWait;
            _modified = true;
            setTitle();
        }

        void colorizeText() {
        }

        void setText(ToolStripItem item, string text) {
            if(this.InvokeRequired) {
                this.Invoke(new Action<ToolStripItem, string>(setText), item, text);
                return;
            }
            item.Text = text;
        }
        void setText(Control control, string text) {
            if(this.InvokeRequired) {
                this.Invoke(new Action<Control, string>(setText), control, text);
                return;
            }
            control.Text = text;
        }


        private void ScriptParserGeneratorTestForm_Load(object sender, EventArgs e) {
            _colorizeElapse = _colorizeWait;
            timerCursor.Start();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            if(_modified) {
                if(MessageBox.Show(Properties.Settings.Default.Msg_FileModifiedAskSave, DisplayFilename, MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    if(!save(false)) {
                        return;
                    }
                }
            }
            if(dialogOpen.ShowDialog() == DialogResult.OK) {
                _filename = dialogOpen.FileName;
                using(StreamReader reader = new StreamReader(_filename)) {
                    textScript.Text = reader.ReadToEnd();
                    _colorizeElapse = _colorizeWait;
                }
            }
            setTitle();
        }
        bool save(bool overwrite) {
            if(!overwrite) {
                if(dialogSave.ShowDialog() != DialogResult.OK) {
                    return false;
                }
                _filename = dialogSave.FileName;
            }
            if(_filename != null) {
                try {
                    using(StreamWriter writer = new StreamWriter(_filename)) {
                        writer.Write(textScript.Text);
                    }
                    _modified = false;
                    setTitle();
                } catch(Exception ex) {
                    setText(textStatus, ex.Message);
                    return false;
                }
            }
            return true;
        }
        void setTitle() {
            setText(this, string.Format("{0}{1} - {2}", DisplayFilename, (_modified ? "*" : ""), typeof(ScriptParserGenerator).Name));
        }
        private void buttonCompile_Click(object sender, EventArgs e) {
            GeneratorLexer lexer = new GeneratorLexer(false);
            try {
                using(StringReader reader = new StringReader(textScript.Text)) {
                    _generator = new ScriptParserGenerator(lexer.ParseText(reader));
                }
            } catch(Exception ex) {
                setText(textMessage, ex.Message);
                setText(textParser, "");
                setText(textUtility, "");
                setText(textTemplates, "");
                return;
            }
            try {
                listSettings.Items.Clear();
                foreach(var pair in _generator.GetSettingsContent()) {
                    ListViewItem item = new ListViewItem(new string[] { pair.Key, pair.Value });
                    listSettings.Items.Add(item);
                }
                StringWriter outputParser = new StringWriter();
                StringWriter outputUtility = new StringWriter();
                StringWriter outputTemplate = new StringWriter();
                StringWriter message = new StringWriter();
                foreach(string line in _generator.GetHeader()) {
                    outputParser.WriteLine(line);
                }
                foreach(string line in _generator.GetBaseOutputClassHeader()) {
                    outputParser.WriteLine(line);
                }
                foreach(string line in _generator.GetAbstractReturnMethods()) {
                    outputParser.WriteLine(line);
                }
                foreach(var def in _generator.Source.Defs) {
                    def.WriteParseCode(outputParser, _generator);
                    outputParser.WriteLine();
                }
                foreach(string line in _generator.GetOutputClassFooter()) {
                    outputParser.WriteLine(line);
                }
                foreach(string line in _generator.GetFooter()) {
                    outputParser.WriteLine(line);
                }

                foreach(string line in _generator.GetHeader()) {
                    outputUtility.WriteLine(line);
                }
                foreach(string line in _generator.GetUtility()) {
                    outputUtility.WriteLine(line);
                }
                foreach(string line in _generator.GetFooter()) {
                    outputUtility.WriteLine(line);
                }
                foreach(string line in _generator.GetHeader()) {
                    outputTemplate.WriteLine(line);
                }
                foreach(string line in _generator.GetReturnClassTemplates()) {
                    outputTemplate.WriteLine(line);
                }
                foreach(string line in _generator.GetOutputClassHeader()) {
                    outputTemplate.WriteLine(line);
                }
                foreach(string line in _generator.GetNotImplementedReturnMethods()) {
                    outputTemplate.WriteLine(line);
                }
                foreach(string line in _generator.GetOutputClassFooter()) {
                    outputTemplate.WriteLine(line);
                }
                foreach(string line in _generator.GetFooter()) {
                    outputTemplate.WriteLine(line);
                }
                message.Write(_generator.ReadWarningMessage());
                message.WriteLine("Done.");
                setText(textMessage, message.ToString());
                setText(textParser, outputParser.ToString());
                setText(textUtility, outputUtility.ToString());
                setText(textTemplates, outputTemplate.ToString());
            } catch(NotSupportedException ex) {
                setText(textStatus, ex.Message);
                textMessage.AppendText(ex.StackTrace);
            }
        }
        string joinStr(IList<string> strs) {
            if(strs.Count == 0)
                return "";
            return strs.Aggregate((a, b) => "'" + a + "', '" + b + "'");
        }
        void firstFollowing(ScriptParserGenerator gen, DefinitionElement root, TextWriter writer, DefinitionContent content) {
            writer.WriteLine(string.Format("   {0}: first:<{1}> following:<{2}>", content, joinStr(content.GetFirstTerminals(gen)), joinStr(content.GetFollowingTerminals(gen))));
            SelectionElement s = content as SelectionElement;
            if(s != null) {
                foreach(ElementsElement elems in s.Candidates) {
                    firstFollowing(gen, root, writer, elems);
                }
            }
            ExpressionsElement ex = content as ExpressionsElement;
            if(ex != null) {
                foreach(ElementsElement elems in ex.Selection.Candidates) {
                    firstFollowing(gen, root, writer, elems);
                }
            }
            ElementsElement es = content as ElementsElement;
            if(es != null) {
                foreach(ElementElement elem in es.Elements) {
                    firstFollowing(gen, root, writer, elem);
                }
            }
            RepeatElement r = content as RepeatElement;
            if(r != null)
                firstFollowing(gen, root, writer, r.InnerExpression);
            OptionElement o = content as OptionElement;
            if(o != null)
                firstFollowing(gen, root, writer, o.InnerExpression);
            GroupElement g = content as GroupElement;
            if(g != null)
                firstFollowing(gen, root, writer, g.InnerExpression);
            LiteralElement l = content as LiteralElement;
        }

        private void textParser_Click(object sender, EventArgs e) {
            textParser.SelectAll();
            textParser.Copy();
        }

        private void textUtility_Click(object sender, EventArgs e) {
            textUtility.SelectAll();
            textUtility.Copy();
        }

        private void textTemplates_Click(object sender, EventArgs e) {
            textTemplates.SelectAll();
            textTemplates.Copy();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            save(true);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            save(false);
        }

        private void ScriptParserGeneratorTestForm_FormClosing(object sender, FormClosingEventArgs e) {
            if(_modified) {
                switch(MessageBox.Show(Properties.Settings.Default.Msg_FileModifiedAskSave, DisplayFilename, MessageBoxButtons.YesNoCancel)) {
                case DialogResult.Yes:
                    if(!save(false)) {
                        e.Cancel = true;
                        return;
                    }
                    break;
                case DialogResult.No:
                    break;
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;
                }
            }
        }

        private void timerCursor_Tick(object sender, EventArgs e) {
            int pos = textScript.SelectionStart;
            int line = textScript.GetLineFromCharIndex(pos);
            int lineHeadPos = textScript.GetFirstCharIndexFromLine(line);
            labelCursorPos.Text = string.Format("Line {0}, Column {1}", line + 1, pos - lineHeadPos + 1);
        }
    }

}
