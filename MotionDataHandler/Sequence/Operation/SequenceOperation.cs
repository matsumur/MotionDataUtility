using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.ObjectModel;

namespace MotionDataHandler.Sequence.Operation {
    using Script;
    using Misc;

    #region Parameters

    /// <summary>
    /// 真理値を受け取る引数
    /// </summary>
    public class BooleanParameter : BooleanProcParam<SequenceProcEnv> {
        public BooleanParameter(string paramName)
            : base(paramName) {
        }
    }

    /// <summary>
    /// 実数値を受け取る引数
    /// </summary>
    public class NumberParameter : NumberProcParam<SequenceProcEnv> {
        public NumberParameter(string paramName, decimal minimum, decimal maximum, int decimalPlaces)
            : base(paramName, minimum, maximum, decimalPlaces) {
        }
        public NumberParameter(string paramName, decimal minimum, decimal maximum, int decimalPlaces, decimal increment)
            : base(paramName, minimum, maximum, decimalPlaces, increment) {
        }
    }

    /// <summary>
    /// 文字列を受け取る引数
    /// </summary>
    public class StringParameter : StringProcParam<SequenceProcEnv> {
        public StringParameter(string paramName)
            : base(paramName) {
        }
    }

    /// <summary>
    /// 少数の候補のうち一つを選択する引数
    /// </summary>
    public class SingleSelectParameter : SingleSelectProcParam<SequenceProcEnv> {
        public SingleSelectParameter(string paramName, IList<string> radioTexts)
            : base(paramName, radioTexts) {
        }
    }

    /// <summary>
    /// 複数のシーケンス列を選択する引数
    /// </summary>
    public class SequenceMultiSelectParameter : ProcParam<SequenceProcEnv> {
        public override string TypeName { get { return "sequence name list"; } }
        public IList<SequenceData> Value;
        private Func<SequenceView, bool> _conditionToShowOnList;

        public SequenceMultiSelectParameter(string paramName) : this(paramName, x => true) { }
        public SequenceMultiSelectParameter(string paramName, Func<SequenceView, bool> conditionToShowOnList)
            : base(paramName) {
            Value = new List<SequenceData>();
            _conditionToShowOnList = conditionToShowOnList;
        }

        public override Panel CreatePanel(SequenceProcEnv environment) {
            Panel ret = new Panel();
            ret.Height = 160;
            GroupBox group = new GroupBox();
            group.Text = ParamName;
            group.Dock = DockStyle.Fill;
            SequenceSelectionControl list = new SequenceSelectionControl();
            list.AttachController(environment.Controller, _conditionToShowOnList);
            list.SelectionMode = SelectionMode.MultiExtended;
            list.SelectedIndexChanged += new EventHandler((s, e) => {
                Value = list.SelectedItems;
                DoValueChanged();
            });
            int index = 0;
            list.SelectedIndices.Clear();
            foreach(var viewer in environment.Controller.GetViewList()) {
                if(Value.Contains(viewer.Sequence)) {
                    list.SelectedIndices.Add(index);
                }
                if(_conditionToShowOnList(viewer)) {
                    index++;
                }
            }
            list.Dock = DockStyle.Fill;
            group.Controls.Add(list);
            ret.Controls.Add(group);
            return ret;
        }
        public override bool FromScriptVariable(SequenceProcEnv environment, ScriptVariable variable, ref string errorMessage) {
            IList<ScriptVariable> list = variable.ToList();
            List<SequenceData> sequences = new List<SequenceData>();
            foreach(var str in list) {
                string name = str.ToString();
                SequenceData sequence = environment.GetSequenceByTitle(name);
                if(sequence == null) {
                    errorMessage = MotionDataHandler.Properties.Settings.Default.Msg_SequenceNotFound + ": " + name;
                    return false;
                }
                sequences.Add(sequence);
            }
            this.Value = sequences;
            return true;
        }
        public override ScriptVariable ToScriptVariable(SequenceProcEnv environment) {
            return new ListVariable(this.Value.Select(d => (ScriptVariable)new StringVariable(d.Title)));
        }
    }
    /// <summary>
    /// 一つのシーケンス列を選択する引数
    /// </summary>
    public class SequenceSingleSelectParameter : ProcParam<SequenceProcEnv> {
        public override string TypeName { get { return "sequence name list"; } }
        public SequenceData Value;
        private Func<SequenceView, bool> _conditionToShowOnList;

        public SequenceSingleSelectParameter(string paramName) : this(paramName, x => true) { }
        public SequenceSingleSelectParameter(string paramName, Func<SequenceView, bool> conditionToShowOnList)
            : base(paramName) {
            this.Value = null;
            _conditionToShowOnList = conditionToShowOnList;
        }

        public override Panel CreatePanel(SequenceProcEnv environment) {
            Panel ret = new Panel();
            ret.Height = 160;
            GroupBox group = new GroupBox();
            group.Text = ParamName;
            group.Dock = DockStyle.Fill;
            SequenceSelectionControl list = new SequenceSelectionControl();
            list.AttachController(environment.Controller, _conditionToShowOnList);
            list.SelectionMode = SelectionMode.One;
            list.SelectedIndexChanged += new EventHandler((s, e) => {
                IList<SequenceData> items = list.SelectedItems;
                if(items.Count == 1) {
                    this.Value = items[0];
                } else {
                    this.Value = null;
                }
                this.DoValueChanged();
            });
            int index = 0;
            list.SelectedIndices.Clear();
            foreach(var viewer in environment.Controller.GetViewList()) {
                if(Value == viewer.Sequence) {
                    list.SelectedIndices.Add(index);
                }
                if(_conditionToShowOnList(viewer)) {
                    index++;
                }
            }
            list.Dock = DockStyle.Fill;
            group.Controls.Add(list);
            ret.Controls.Add(group);
            return ret;
        }
        public override bool FromScriptVariable(SequenceProcEnv environment, ScriptVariable variable, ref string errorMessage) {
            IList<ScriptVariable> list = variable.ToList();
            if(list.Count != 1) {
                errorMessage = MotionDataHandler.Properties.Settings.Default.Msg_SpecifyOneSequenceName;
                return false;
            }
            var str = list[0];
            string name = str.ToString();
            SequenceData sequence = environment.GetSequenceByTitle(name);
            if(sequence == null) {
                errorMessage = MotionDataHandler.Properties.Settings.Default.Msg_SequenceNotFound + ": " + name;
                return false;
            }
            this.Value = sequence;
            return true;
        }

        public override ScriptVariable ToScriptVariable(SequenceProcEnv environment) {
            return new StringVariable(this.Value.Title);
        }
    }
    /// <summary>
    /// シーケンスの中の一つの時系列データを選択する引数
    /// </summary>
    public class SequenceColumnSelectParameter : ProcParam<SequenceProcEnv> {
        public override string TypeName { get { return "sequence column index"; } }
        public int Value;
        public readonly SequenceSingleSelectParameter Parent;

        public SequenceColumnSelectParameter(string paramName, SequenceSingleSelectParameter parent)
            : base(paramName) {
            this.Value = 0;
            this.Parent = parent;
        }
        public SequenceColumnSelectParameter(string paramName) : this(paramName, null) { }

        public override Panel CreatePanel(SequenceProcEnv environment) {
            Panel ret = new Panel();
            GroupBox group = new GroupBox();
            group.Text = ParamName;
            group.Dock = DockStyle.Fill;
            TargetSequenceIndexControl target = new TargetSequenceIndexControl();
            if(this.Parent == null) {
                target.AttachSequence(environment.SelectedSequence);
            } else {
                this.Parent.ValueChanged += new EventHandler((s, e) => {
                    SequenceSingleSelectParameter parent = s as SequenceSingleSelectParameter;
                    var sequence = parent.Value;
                    if(sequence != null) {
                        target.AttachSequence(sequence);
                    }
                });
            }
            if(Value >= -1 && Value < target.ItemCount) {
                target.SelectedIndex = Value;
            }
            target.SelectedIndexChanged += new EventHandler((s, e) => {
                this.Value = target.SelectedIndex;
                this.DoValueChanged();
            });
            target.Dock = DockStyle.Fill;
            group.Controls.Add(target);
            ret.Controls.Add(group);
            ret.Height = 48;
            return ret;
        }

        public override bool FromScriptVariable(SequenceProcEnv environment, ScriptVariable variable, ref string errorMessage) {
            SequenceData parent = environment.SelectedSequence;
            if(this.Parent != null && this.Parent.Value != null) {
                parent = this.Parent.Value;
            }
            switch(variable.Type) {
            case ScriptVariableType.Number:
                int index = variable.ToInteger();
                if(index < 0 || index >= parent.Values.ColumnCount) {
                    errorMessage = MotionDataHandler.Properties.Settings.Default.Msg_IndexOutOfRange + ": " + index.ToString();
                    return false;
                }
                this.Value = index;
                return true;
            case ScriptVariableType.String:
                string name = variable.ToString();
                int index2 = -1;
                for(int i = 0; i < parent.Values.ColumnNames.Length; i++) {
                    if(parent.Values.ColumnNames[i] == name) {
                        index2 = i;
                        break;
                    }
                }
                if(index2 == -1) {
                    errorMessage = MotionDataHandler.Properties.Settings.Default.Msg_ColumnNameNotFound;
                    return false;
                }
                this.Value = index2;
                return true;
            default:
                errorMessage = MotionDataHandler.Properties.Settings.Default.Msg_SpecifyIndexOfColumnOrColumnName;
                return false;
            }
        }
        public override ScriptVariable ToScriptVariable(SequenceProcEnv environment) {
            return new NumberVariable(this.Value);
        }
    }

    /// <summary>
    /// シーケンスの中の複数の時系列データを選択する引数
    /// </summary>
    public class SequenceColumnMultiSelectParameter : ProcParam<SequenceProcEnv> {
        public override string TypeName { get { return "sequence column index list"; } }
        public IList<int> Value;
        public readonly SequenceSingleSelectParameter Parent;

        public SequenceColumnMultiSelectParameter(string paramName, SequenceSingleSelectParameter parent)
            : base(paramName) {
            Value = new List<int>();
            Parent = parent;
        }
        public SequenceColumnMultiSelectParameter(string paramName) : this(paramName, null) { }

        public override Panel CreatePanel(SequenceProcEnv environment) {
            Panel ret = new Panel();
            GroupBox group = new GroupBox();
            group.Text = this.ParamName;
            group.Dock = DockStyle.Fill;
            SequenceIndexSelectControl ctrl = new SequenceIndexSelectControl();
            ctrl.SelectionMode = SelectionMode.MultiExtended;
            if(Parent == null) {
                ctrl.SetItemsFromSequence(environment.SelectedSequence);
            } else {
                Parent.ValueChanged += new EventHandler((s, e) => {
                    SequenceSingleSelectParameter parent = s as SequenceSingleSelectParameter;
                    var sequence = parent.Value;
                    if(sequence != null) {
                        ctrl.SetItemsFromSequence(sequence);
                    }
                });
            }
            if(this.Value != null && this.Value.Count > 0) {
                ctrl.SelectedIndices.Clear();
                foreach(int index in this.Value) {
                    if(index < 0 || index >= ctrl.Items.Count)
                        continue;
                    ctrl.SelectedIndices.Add(index);
                }
            }
            this.Value = new List<int>(ctrl.SelectedIndices.OfType<int>());
            ctrl.SelectedIndexChanged += new EventHandler((s, e) => {
                this.Value = new List<int>(ctrl.SelectedIndices.OfType<int>());
                DoValueChanged();
            });
            ctrl.Dock = DockStyle.Fill;
            group.Controls.Add(ctrl);
            ret.Controls.Add(group);
            ret.Height = 128;
            return ret;
        }

        public override bool FromScriptVariable(SequenceProcEnv environment, ScriptVariable variable, ref string errorMessage) {
            SequenceData parent = environment.SelectedSequence;
            if(this.Parent != null && this.Parent.Value != null) {
                parent = this.Parent.Value;
            }
            IList<ScriptVariable> list = variable.ToList();
            List<int> ret = new List<int>();
            foreach(ScriptVariable v in list) {
                switch(v.Type) {
                case ScriptVariableType.Number:
                    int index = v.ToInteger();
                    if(index < 0 || index >= parent.Values.ColumnCount) {
                        errorMessage = MotionDataHandler.Properties.Settings.Default.Msg_IndexOutOfRange + ": " + index.ToString();
                        return false;
                    }
                    ret.Add(index);
                    break;
                case ScriptVariableType.String:
                    string name = v.ToString();
                    int index2 = -1;
                    for(int i = 0; i < parent.Values.ColumnNames.Length; i++) {
                        if(parent.Values.ColumnNames[i] == name) {
                            index2 = i;
                            break;
                        }
                    }
                    if(index2 == -1) {
                        errorMessage = MotionDataHandler.Properties.Settings.Default.Msg_ColumnNameNotFound;
                        return false;
                    }
                    ret.Add(index2);
                    return true;
                default:
                    errorMessage = MotionDataHandler.Properties.Settings.Default.Msg_SpecifyIndexOfColumnOrColumnName;
                    return false;
                }
            }
            this.Value = ret;
            return true;
        }
        public override ScriptVariable ToScriptVariable(SequenceProcEnv environment) {
            return new ListVariable(this.Value.Select(v => (ScriptVariable)new NumberVariable(v)));
        }
    }

    /// <summary>
    /// ラベル名を選択する引数
    /// </summary>
    public class LabelSelectParameter : ProcParam<SequenceProcEnv> {
        public override string TypeName { get { return "label text list"; } }
        public IList<string> Value;
        public readonly SequenceSingleSelectParameter Parent;
        public readonly bool MultiSelect;
        public LabelSelectParameter(string paramName, bool multiSelect, SequenceSingleSelectParameter parent)
            : base(paramName) {
            Value = new List<string>();
            Parent = parent;
            MultiSelect = multiSelect;
        }
        public LabelSelectParameter(string paramName, bool multiSelect) : this(paramName, multiSelect, null) { }

        public override Panel CreatePanel(SequenceProcEnv environment) {
            Panel ret = new Panel();
            GroupBox group = new GroupBox();
            group.Text = ParamName;
            group.Dock = DockStyle.Fill;
            BorderSelectionControl border = new BorderSelectionControl();
            border.SelectionMode = MultiSelect ? SelectionMode.MultiExtended : SelectionMode.One;
            if(Parent == null) {
                border.AttachBorder(environment.SelectedSequence.Borders);
            } else {
                var sequences = Parent.Value;
                if(sequences != null) {
                    border.AttachBorder(sequences.Borders);
                }
                Parent.ValueChanged += new EventHandler((s, e) => {
                    SequenceSingleSelectParameter parent = s as SequenceSingleSelectParameter;
                    var sequences2 = parent.Value;
                    if(sequences2 != null) {
                        border.AttachBorder(sequences2.Borders);
                    }
                });
            }
            border.SelectRange(Value);

            border.SelectedIndexChanged += new EventHandler((s, e) => {
                var labels = new List<string>();
                foreach(var item in border.SelectedItems) {
                    labels.Add((string)item);
                }
                Value = labels;
                DoValueChanged();
            });
            border.Dock = DockStyle.Fill;
            group.Controls.Add(border);
            ret.Controls.Add(group);
            ret.Height = 160;
            return ret;
        }
        public override bool FromScriptVariable(SequenceProcEnv environment, ScriptVariable variable, ref string errorMessage) {
            IList<ScriptVariable> list = variable.ToList();
            SequenceData parent = environment.SelectedSequence;
            if(this.Parent != null && this.Parent.Value != null) {
                parent = this.Parent.Value;
            }
            IList<string> borderNames = parent.Borders.GetLabelNames(true);
            List<string> ret = new List<string>();
            foreach(ScriptVariable v in list) {
                string name = v.ToString();
                if(!borderNames.Contains(name) && parent.Borders.DefaultName != name) {
                    errorMessage = MotionDataHandler.Properties.Settings.Default.Msg_LabelNameNotFound + ": " + name;
                    return false;
                }
                ret.Add(name);
            }
            this.Value = ret;
            return true;
        }
        public override ScriptVariable ToScriptVariable(SequenceProcEnv environment) {
            return new ListVariable(this.Value.Select(v => (ScriptVariable)new StringVariable(v)));
        }
    }
    public class LabelReplaceParameter : ProcParam<SequenceProcEnv> {
        public IDictionary<string, string> Value = new Dictionary<string, string>();
        public override string TypeName { get { return "string map"; } }
        public readonly SequenceSingleSelectParameter Parent;

        public LabelReplaceParameter(string paramName, SequenceSingleSelectParameter parent)
            : base(paramName) {
            this.Parent = parent;
        }
        public LabelReplaceParameter(string paramName)
            : this(paramName, null) {
        }


        public override Panel CreatePanel(SequenceProcEnv environment) {
            Panel ret = new Panel();
            GroupBox group = new GroupBox();
            group.Text = ParamName;
            group.Dock = DockStyle.Fill;
            LabelReplaceControl border = new LabelReplaceControl();
            if(this.Parent == null) {
                border.AttachSequenceData(environment.SelectedSequence);
            } else {
                var sequences = this.Parent.Value;
                if(sequences != null) {
                    border.AttachSequenceData(sequences);
                }
                this.Parent.ValueChanged += new EventHandler((s, e) => {
                    SequenceSingleSelectParameter parent = s as SequenceSingleSelectParameter;
                    var sequences2 = parent.Value;
                    if(sequences2 != null) {
                        border.AttachSequenceData(sequences2);
                    }
                });
            }
            border.AddReplaceMap(this.Value);

            border.ReplaceMapChanged += new EventHandler((s, e) => {
                this.Value = border.GetReplaceMap().Where(pair => pair.Key != pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
                this.DoValueChanged();
            });
            border.Dock = DockStyle.Fill;
            group.Controls.Add(border);
            ret.Controls.Add(group);
            ret.Height = 360;
            return ret;
        }

        public override bool FromScriptVariable(SequenceProcEnv environment, ScriptVariable variable, ref string errorMessage) {
            if(variable.IsNull()) {
                errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_CannotSpecifyNull + ": " + this.ParamName;
                return false;
            }
            this.Value = new Dictionary<string, string>();
            IList<ScriptVariable> list = variable.ToList();
            foreach(ScriptVariable row in list) {
                if(row.IsNull())
                    continue;
                IList<ScriptVariable> pair = row.ToList();
                if(pair.Count < 2) {
                    errorMessage = "各要素に二つの文字列が必要です";
                    return false;
                }
                if(pair[0].IsNull() || pair[1].IsNull()) {
                    errorMessage = "要素の文字列にnullを指定できません";
                    return false;
                }
                this.Value[pair[0].ToString()] = pair[1].ToString();
            }
            return true;
        }

        public override ScriptVariable ToScriptVariable(SequenceProcEnv environment) {
            List<ScriptVariable> list = new List<ScriptVariable>();
            foreach(var pair in this.Value) {
                StringVariable key = new StringVariable(pair.Key);
                StringVariable value = new StringVariable(pair.Value);
                list.Add(new ListVariable(key, value));
            }
            return new ListVariable(list);
        }
    }
    public class NumberListParameter : ProcParam<SequenceProcEnv> {
        public List<decimal> Value = new List<decimal>();
        public int NumberOfArgs;
        public decimal Maximum, Minimum, Increment;
        public int DecimalPlaces;
        public override string TypeName { get { return "number list"; } }
        public NumberListParameter(string paramName, int numberOfArgs, decimal minimum, decimal maximum, int decimalPlaces, decimal increment)
            : base(paramName) {
            this.NumberOfArgs = numberOfArgs;
            this.Maximum = maximum;
            this.Minimum = minimum;
            this.DecimalPlaces = decimalPlaces;
            this.Increment = increment;
        }

        public override Panel CreatePanel(SequenceProcEnv environment) {
            Panel ret = new Panel();
            GroupBox group = new GroupBox();
            group.Text = ParamName;
            group.Dock = DockStyle.Fill;
            for(int i = this.NumberOfArgs - 1; i >= 0; i--) {
                int index = i;
                while(this.Value.Count <= index)
                    this.Value.Add(0);
                NumericUpDown num = new NumericUpDown();
                num.DecimalPlaces = this.DecimalPlaces;
                num.Minimum = this.Minimum;
                num.Maximum = this.Maximum;
                if(Value[index] < this.Minimum)
                    Value[index] = this.Minimum;
                if(Value[index] > this.Maximum)
                    Value[index] = this.Maximum;
                num.Value = this.Value[index];
                num.TextAlign = HorizontalAlignment.Right;
                num.ValueChanged += new EventHandler((s, e) => {
                    while(this.Value.Count <= index)
                        this.Value.Add(0);
                    this.Value[index] = num.Value;
                    DoValueChanged();
                });
                num.Validated += new EventHandler((s, e) => {
                    while(this.Value.Count <= index)
                        this.Value.Add(0);
                    this.Value[index] = num.Value;
                    DoValueChanged();
                });

                num.Increment = this.Increment;
                num.Dock = DockStyle.Top;
                group.Controls.Add(num);
            }
            ret.Controls.Add(group);
            return ret;
        }

        public override bool FromScriptVariable(SequenceProcEnv environment, ScriptVariable variable, ref string errorMessage) {
            if(variable.IsNull()) {
                errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_CannotSpecifyNull + ": " + this.ParamName;
                return false;
            }
            IList<ScriptVariable> list = variable.ToList();

            while(this.Value.Count <= list.Count)
                this.Value.Add(0);

            int index = 0;
            foreach(ScriptVariable row in list) {
                if(row.IsNull()) {
                    this.Value[index] = 0;
                } else {
                    decimal value = row.ToNumber();
                    this.Value[index] = value;
                }
                index++;
            }
            if(index < this.NumberOfArgs) {
                errorMessage = string.Format("{0} 個の数値が必要です", this.NumberOfArgs);
                return false;
            }
            return true;
        }

        public override ScriptVariable ToScriptVariable(SequenceProcEnv environment) {
            List<NumberVariable> values = new List<NumberVariable>();
            while(this.Value.Count < this.NumberOfArgs)
                this.Value.Add(0);

            for(int i = 0; i < this.NumberOfArgs; i++) {
                values.Add(new NumberVariable(this.Value[i]));
            }
            return new ListVariable(values);
        }
    }
    #endregion


    /// <summary>
    /// SequenceOperationの実行内容を定めるInterface
    /// </summary>
    public interface ISequenceOperation {
        /// <summary>
        /// 処理を実行し，SequenceDataを返します
        /// </summary>
        /// <param title="args">GetParametersで返されたパラメータリスト。キャストして用いる。</param>
        /// <param title="env">実行環境変数</param>
        /// <returns>作成されたシーケンスのリスト</returns>
        SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env);

        /// <summary>
        /// パラメータリストを取得します。
        /// </summary>
        /// <returns>パラメータの配列</returns>
        IList<ProcParam<SequenceProcEnv>> GetParameters();
        /// <summary>
        /// 項目として表示されるタイトルを返します。
        /// </summary>
        /// <returns>タイトル</returns>
        string GetTitle();
        /// <summary>
        /// ダイアログに説明として表示されるテキストを返します。
        /// </summary>
        /// <returns>説明</returns>
        string GetDescription();
        /// <summary>
        /// ビューアのタイプがどの場合に項目として表示するかを取得します。
        /// </summary>
        SequenceType OperationTargetType { get; }
        /// <summary>
        /// 処理が選択中のシーケンスデータの中身を変更する可能性があるかを取得します．
        /// </summary>
        bool ReplacesInternalData { get; }
        /// <summary>
        /// スクリプト用コマンド名を返します．
        /// </summary>
        /// <returns></returns>
        string GetCommandName();
    }

    /// <summary>
    /// ISequenceOperationをIScriptFunctionとして実行するためのラッパークラス
    /// </summary>
    public class SequenceOperationScriptFunction : IScriptFunction {
        public readonly ISequenceOperation Operation;

        public SequenceOperationScriptFunction(ISequenceOperation operation) {
            if(operation == null)
                throw new ArgumentNullException("'operation' cannot be null", "operation");
            this.Operation = operation;
        }

        #region IScriptFunction メンバ

        public string Name {
            get { return string.Format("Sequence_{0}", this.Operation.GetCommandName()); }
        }

        public ScriptVariable Call(IList<ScriptVariable> args, ScriptConsole console) {

            if(args == null)
                throw new ArgumentNullException("args", "args cannot be null");
            if(args.Count < 1)
                throw new ArgumentException("args must have one or more elements", "args");
            if(args[0] == null)
                throw new ArgumentException("first element of args cannot be null", "args");
            string viewerName = args[0].ToString();
            SequenceView viewer = SequenceViewerController.Singleton.GetViewByTitle(viewerName);
            if(viewer == null)
                throw new ArgumentException("first element of args must be resultSequence name : " + viewerName, "args");

            SequenceProcEnv env2 = new SequenceProcEnv(console.SequenceController, viewer.Sequence);

            IList<ProcParam<SequenceProcEnv>> parameters = this.Operation.GetParameters() ?? new ProcParam<SequenceProcEnv>[0];
            if(args.Count != parameters.Count + 1)
                throw new ArgumentException(string.Format(global::MotionDataHandler.Properties.Settings.Default.Msg_NumberOfArgumentsRequired, parameters.Count + 1));
            for(int i = 0; i < parameters.Count; i++) {
                string errorStr = "";
                if(!parameters[i].FromScriptVariable(env2, args[i + 1], ref errorStr)) {
                    throw new ArgumentException(string.Format(global::MotionDataHandler.Properties.Settings.Default.Msg_InvalidNthArgument + ": {1}", i + 1, errorStr ?? ""), "args");
                }
            }
            SequenceData resultSequence = this.Operation.Operate(parameters, env2);
            ScriptVariable ret = null;
            if(resultSequence != null) {
                console.SequenceController.AddSequence(resultSequence, viewer.IsLocked);
                ret = new StringVariable(resultSequence.Title);
            }
            console.SequenceController.DoAllocationChanged();

            if(this.Operation.ReplacesInternalData) {
                env2.SelectedSequence.IsDataChanged = true;
            }

            return ret;
        }

        public string Usage {
            get {
                IList<ProcParam<SequenceProcEnv>> parameters = this.Operation.GetParameters() ?? new ProcParam<SequenceProcEnv>[0];
                StringBuilder ret = new StringBuilder("(target sequence");
                foreach(ProcParam<SequenceProcEnv> parameter in parameters) {
                    ret.Append(", ");
                    if(parameter == null) {
                        ret.Append("null");
                    } else {
                        ret.Append("(" + (parameter.TypeName ?? "undefined") + ") " + (parameter.ParamName ?? "value"));
                    }
                }
                ret.Append(")");
                return ret.ToString();
            }
        }

        #endregion
    }

    /// <summary>
    /// SequenceOperationを実行するときの実行環境
    /// </summary>
    public class SequenceProcEnv {
        readonly SequenceViewerController _controller;
        /// <summary>
        /// シーケンス群を統括するコントローラを取得します。
        /// </summary>
        public SequenceViewerController Controller { get { return _controller; } }
        readonly SequenceData _selectedSequence;
        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        /// <param title="controller">コントローラ</param>
        /// <param title="current">選択されたシーケンス</param>
        public SequenceProcEnv(SequenceViewerController controller, SequenceData selected) {
            if(controller == null) {
                throw new ArgumentNullException("controller", "'controller' cannot be null");
            }
            _controller = controller;
            _selectedSequence = selected;
        }
        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        /// <param title="controller">コントローラ</param>
        /// <param title="current">選択されたシーケンス</param>
        public SequenceProcEnv(SequenceViewerController controller)
            : this(controller, null) {
            SequenceView tmp = controller.GetFocusedView();
            if(tmp != null) {
                _selectedSequence = tmp.Sequence;
            }
        }

        /// <summary>
        /// 選択されたシーケンスのデータを取得します。
        /// </summary>
        public SequenceData SelectedSequence { get { return _selectedSequence; } }
        /// <summary>
        /// コントローラに関連付けられているシーケンスのデータの個数を返します。
        /// </summary>
        /// <returns></returns>
        public int SequenceCount() { return _controller.GetSequenceList().Count; }
        /// <summary>
        /// コントローラに関連付けられているシーケンスのデータのうち一致するタイトルのものを返します．ない場合はnullです，
        /// </summary>
        /// <param name="title">シーケンスデータのタイトル</param>
        /// <returns></returns>
        public SequenceData GetSequenceByTitle(string title) { return _controller.GetSequenceByTitle(title); }
        /// <summary>
        /// コントローラに関連付けられているシーケンスビューのうち一致するタイトルのものを返します．ない場合はnullです，
        /// </summary>
        /// <param name="title">シーケンスデータのタイトル</param>
        /// <returns></returns>
        public SequenceView GetViewerByTitle(string title) { return _controller.GetViewByTitle(title); }
    }

    public class OperationExecution {
        private SequenceViewerController _controller;
        private ISequenceOperation _operation;
        private SequenceData _current;
        public SequenceProcEnv GetEnvironment() {
            SequenceProcEnv env = new SequenceProcEnv(_controller, _current);
            return env;
        }
        /// <summary>
        /// 処理に用いられる引数のリストを取得または設定します。
        /// </summary>
        public IList<ProcParam<SequenceProcEnv>> Parameters = null;

        public OperationExecution(ISequenceOperation operation, SequenceViewerController controller, SequenceData current) {
            if(operation == null)
                throw new ArgumentNullException("operation", "'operation' cannot be null");
            _operation = operation;
            _controller = controller;
            _current = current;
        }

        public Panel GetPanel() {
            SequenceProcEnv env = this.GetEnvironment();

            Panel ret = new Panel();
            ret.Padding = new Padding(8, 8, 8, 8);
            if(Parameters == null) {
                Parameters = _operation.GetParameters() ?? new ProcParam<SequenceProcEnv>[0];
            }
            foreach(var param in Parameters) {
                Panel sub = param.CreatePanel(env);
                sub.Dock = DockStyle.Top;
                ret.Controls.Add(sub);
                sub.BringToFront();
            }
            if(Parameters.Count == 0) {
                Label empty = new Label();
                empty.Dock = DockStyle.Top;
                empty.Text = "No parameter";
                ret.Controls.Add(empty);
            }
            ret.AutoScroll = true;
            return ret;
        }

        public void Run(Control parentControl) {
            SequenceOperationScriptFunction ope = new SequenceOperationScriptFunction(_operation);
            ScriptConsole.Singleton.Invoke(ope, GetVariablesFromParameters(this.Parameters));
        }

        /// <summary>
        /// Sequence用パラメータをScript変数に変換します
        /// </summary>
        /// <param name="paramaters"></param>
        /// <returns></returns>
        public IList<ScriptVariable> GetVariablesFromParameters(IList<ProcParam<SequenceProcEnv>> paramaters) {
            SequenceProcEnv env = this.GetEnvironment();
            Collection<ScriptVariable> ret = new Collection<ScriptVariable>();
            // 先頭は選択列
            ret.Add(new StringVariable(env.SelectedSequence.Title));
            // 残りのパラメータを変数に変換
            foreach(var param in paramaters) {
                ScriptVariable variable = null;
                if(param != null) {
                    variable = param.ToScriptVariable(env);
                }
                ret.Add(variable);
            }

            return ret;
        }
    }
}
