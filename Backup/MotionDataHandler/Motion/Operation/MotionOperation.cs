using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace MotionDataHandler.Motion.Operation {
    using Script;
    using Misc;
    /// <summary>
    /// 真理値を受け取る引数
    /// </summary>
    public class BooleanParameter : BooleanProcParam<MotionProcEnv> {
        public BooleanParameter(string paramName)
            : base(paramName) {
        }
    }

    /// <summary>
    /// 実数値を受け取る引数
    /// </summary>
    public class NumberParameter : NumberProcParam<MotionProcEnv> {
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
    public class StringParameter : StringProcParam<MotionProcEnv> {
        public StringParameter(string paramName)
            : base(paramName) {
        }
    }

    /// <summary>
    /// 少数の候補のうち一つを選択する引数
    /// </summary>
    public class SingleSelectParameter : SingleSelectProcParam<MotionProcEnv> {
        public SingleSelectParameter(string paramName, IList<string> radioTexts)
            : base(paramName, radioTexts) {
        }
    }

    public class MotionObjectSingleSelectParameter : ProcParam<MotionProcEnv> {
        public MotionObjectInfo Value;
        Predicate<MotionObjectInfo> _targetCondition;
        bool _selectedOnly;
        public MotionObjectSingleSelectParameter(string paramName, Predicate<MotionObjectInfo> targetCondition, bool selectedOnly)
            : base(paramName) {
            _targetCondition = targetCondition;
            _selectedOnly = selectedOnly;
        }
        public override string TypeName {
            get { return "object name"; }
        }

        public override Panel CreatePanel(MotionProcEnv environment) {
            Panel ret = new Panel();
            ret.Height = 120;
            GroupBox group = new GroupBox();
            group.Text = ParamName;
            group.Dock = DockStyle.Fill;
            MotionDataObjectSelectList list = new MotionDataObjectSelectList();
            list.SelectionMode = SelectionMode.One;
            bool seletctedOnly = _selectedOnly;
            Predicate<MotionObjectInfo> condition2 = new Predicate<MotionObjectInfo>(info => {
                return (_targetCondition == null || _targetCondition(info)) && (!_selectedOnly || info.IsSelected);
            });
            list.AttachDataSet(environment.DataSet, condition2, new Predicate<MotionObjectInfo>(info => info == this.Value));
            list.SelectedIndexChanged += new EventHandler((s, e) => {
                IList<MotionObjectInfo> infoList = list.GetListSelectedInfoIndices();
                if(infoList.Count == 0) {
                    this.Value = null;
                } else {
                    this.Value = infoList[0];
                }
                DoValueChanged();
            });
            IList<MotionObjectInfo> infoList2 = list.GetListSelectedInfoIndices();
            if(infoList2.Count == 0) {
                this.Value = null;
            } else {
                this.Value = infoList2[0];
            }

            list.Dock = DockStyle.Fill;
            group.Controls.Add(list);
            ret.Controls.Add(group);
            return ret;
        }

        public override bool FromScriptVariable(MotionProcEnv environment, ScriptVariable variable, ref string errorMessage) {
            if(variable.IsNull()) {
                errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_CannotSpecifyNull;
                return false;
            }
            string name = variable.ToString();

            MotionObjectInfo info = environment.DataSet.GetObjectInfoByName(name);
            if(info == null) {
                errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_ObjectNotFound + ": " + name;
                return false;
            }
            this.Value = info;
            return true;
        }

        public override ScriptVariable ToScriptVariable(MotionProcEnv environment) {
            if(this.Value == null)
                return null;
            return new StringVariable(this.Value.Name);
        }
    }

    #region sequence

    /// <summary>
    /// 一つのシーケンス列を選択する引数
    /// </summary>
    public class SequenceSingleSelectParameter : ProcParam<MotionProcEnv> {
        public override string TypeName { get { return "sequence name list"; } }
        public Sequence.SequenceData Value;
        private Func<Sequence.SequenceView, bool> _conditionToShowOnList;

        public SequenceSingleSelectParameter(string paramName) : this(paramName, x => true) { }
        public SequenceSingleSelectParameter(string paramName, Func<Sequence.SequenceView, bool> conditionToShowOnList)
            : base(paramName) {
            this.Value = null;
            _conditionToShowOnList = conditionToShowOnList;
        }

        public override Panel CreatePanel(MotionProcEnv environment) {
            Panel ret = new Panel();
            ret.Height = 160;
            GroupBox group = new GroupBox();
            group.Text = ParamName;
            group.Dock = DockStyle.Fill;
            Sequence.SequenceSelectionControl list = new MotionDataHandler.Sequence.SequenceSelectionControl();
            list.AttachController(environment.SequenceController, _conditionToShowOnList);
            list.SelectionMode = SelectionMode.One;
            list.SelectedIndexChanged += new EventHandler((s, e) => {
                IList<Sequence.SequenceData> items = list.SelectedItems;
                if(items.Count == 1) {
                    this.Value = items[0];
                } else {
                    this.Value = null;
                }
                this.DoValueChanged();
            });
            int index = 0;
            list.SelectedIndices.Clear();
            foreach(var viewer in environment.SequenceController.GetViewList()) {
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
        public override bool FromScriptVariable(MotionProcEnv environment, ScriptVariable variable, ref string errorMessage) {
            IList<ScriptVariable> list = variable.ToList();
            if(list.Count != 1) {
                errorMessage = MotionDataHandler.Properties.Settings.Default.Msg_SpecifyOneSequenceName;
                return false;
            }
            var str = list[0];
            string name = str.ToString();
            Sequence.SequenceData sequence = environment.SequenceController.GetSequenceByTitle(name);
            if(sequence == null) {
                errorMessage = MotionDataHandler.Properties.Settings.Default.Msg_SequenceNotFound + ": " + name;
                return false;
            }
            this.Value = sequence;
            return true;
        }

        public override ScriptVariable ToScriptVariable(MotionProcEnv environment) {
            return new StringVariable(this.Value.Title);
        }
    }

    /// <summary>
    /// シーケンスの中の一つの時系列データを選択する引数
    /// </summary>
    public class SequenceColumnSelectParameter : ProcParam<MotionProcEnv> {
        public override string TypeName { get { return "sequence column index"; } }
        public int Value;
        public readonly SequenceSingleSelectParameter Parent;

        public SequenceColumnSelectParameter(string paramName, SequenceSingleSelectParameter parent)
            : base(paramName) {
            this.Value = 0;
            if(parent == null)
                throw new ArgumentNullException("parent", "'parent' cannot be null");
            this.Parent = parent;
        }

        public override Panel CreatePanel(MotionProcEnv environment) {
            Panel ret = new Panel();
            GroupBox group = new GroupBox();
            group.Text = this.ParamName;
            group.Dock = DockStyle.Fill;
            Sequence.TargetSequenceIndexControl target = new MotionDataHandler.Sequence.TargetSequenceIndexControl();
            this.Parent.ValueChanged += new EventHandler((s, e) => {
                SequenceSingleSelectParameter parent = s as SequenceSingleSelectParameter;
                var sequence = parent.Value;
                if(sequence != null) {
                    target.AttachSequence(sequence);
                }
            });
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

        public override bool FromScriptVariable(MotionProcEnv environment, ScriptVariable variable, ref string errorMessage) {
            Sequence.SequenceData parent = this.Parent.Value;

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
                int index2 = Array.IndexOf(parent.Values.ColumnNames, name);
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
        public override ScriptVariable ToScriptVariable(MotionProcEnv environment) {
            return new NumberVariable(this.Value);
        }
    }

    #endregion

    public class MotionProcEnv {
        public readonly MotionDataSet DataSet;
        public readonly Sequence.SequenceViewerController SequenceController;
        public MotionProcEnv(ScriptConsole console) {
            this.DataSet = console.MotionDataSet;
            this.SequenceController = console.SequenceController;
        }
    }

    /// <summary>
    /// モーションデータに対する処理の汎用インターフェース
    /// </summary>
    public interface IMotionOperationBase {
        /// <summary>
        /// 引数のオブジェクトが処理の対象とされるかどうかを返します
        /// </summary>
        /// <param name="info">判断されるMotionObjectInfo</param>
        /// <returns>処理に関係するオブジェクト情報であればtrue</returns>
        bool FilterSelection(MotionObjectInfo info);

        /// <summary>
        /// オブジェクトの選択が有効であるかを返します．
        /// </summary>
        /// <param name="infoList">選択中のオブジェクト．あらかじめFilterSelection()により対象とならないものは除かれます</param>
        /// <param name="errorMessage">有効でない場合のメッセージの格納先．有効な場合はなんでも良い</param>
        /// <returns>有効であればtrue</returns>
        bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage);

        /// <summary>
        /// 処理用のパラメータリストを返します．パラメータが不要である場合には空のリストまたはnullを返します．
        /// </summary>
        /// <returns></returns>
        IList<ProcParam<MotionProcEnv>> GetParameters();

        /// <summary>
        /// 与えられた実引数が有効であるかを返します．
        /// </summary>
        /// <param name="args">実引数のリスト．GetParametersで返されたものと同様のリストを与える</param>
        /// <param name="errorMessage">有効でない場合のメッセージの格納先．有効な場合は何でもよい</param>
        /// <returns></returns>
        bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage);

        /// <summary>
        /// スクリプトボードで使用されるときのコマンド名を返します．"CreateFixObject"といった形式．
        /// </summary>
        /// <returns></returns>
        string GetCommandName();

        /// <summary>
        /// 処理のタイトルを返します．簡単な日本語名など
        /// </summary>
        /// <returns></returns>
        string GetTitle();

        /// <summary>
        /// 処理の長い説明を返します．複数行可．
        /// </summary>
        /// <returns></returns>
        string GetDescription();

        /// <summary>
        /// サイズが16x16の表示用のアイコンを取得します．ない場合はnull．
        /// </summary>
        Bitmap IconBitmap { get; }
    }

    /// <summary>
    /// 作成・編集・出力以外の処理や，フレーム全体を見渡して作成・出力処理を行うための汎用インターフェース
    /// </summary>
    public interface IMotionOperationGeneral : IMotionOperationBase {
        /// <summary>
        /// なんらかの処理を実行します
        /// </summary>
        /// <param name="selectedInfoList">選択中のオブジェクト</param>
        /// <param name="args">処理の実引数．GetParametersの戻り値を渡す</param>
        /// <param name="dataSet">処理対象のデータセット</param>
        /// <param name="progressInfo">処理の経過情報を書き出す先のクラス</param>
        void Operate(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, MotionDataSet dataSet, ProgressInformation progressInfo);
    }

    /// <summary>
    /// オブジェクトのフレームデータの編集処理を行うための汎用インターフェース
    /// </summary>
    public interface IMotionOperationEditObject : IMotionOperationBase {
        /// <summary>
        /// 各フレームの選択された各オブジェクトを編集した結果を返します．戻り値は，選択されたオブジェクトのリストと同じ順で並べられた，編集結果のオブジェクトのリストになります．
        /// </summary>
        /// <param name="targetInfoList">編集対象に選択されたオブジェクト情報のリスト</param>
        /// <param name="args">処理への実引数．GetParametersの戻り値を渡す</param>
        /// <param name="frame">編集元のフレーム</param>
        /// <param name="previewMode">プレビュー用に結果が要求されているかどうか．trueの場合に戻り値のリストに余分なオブジェクトを追加して返すと，それらのオブジェクトもプレビュー表示用に使われる</param>
        /// <returns></returns>
        IList<MotionObject> EditObject(IList<MotionObjectInfo> targetInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode);
        /// <summary>
        /// 作成もしくは編集されるオブジェクトの型を取得します．
        /// </summary>
        Type CreatedType { get; }
    }

    /// <summary>
    /// オブジェクトの作成処理を行うための汎用インターフェース
    /// </summary>
    public interface IMotionOperationCreateObject : IMotionOperationBase {
        /// <summary>
        /// 作成されるオブジェクト情報のリストを返します
        /// </summary>
        /// <param name="selectedInfoList">選択されているオブジェクト情報のリスト</param>
        /// <param name="args">処理への実引数</param>
        /// <returns></returns>
        IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args);
        /// <summary>
        /// 各フレームに追加されるオブジェクトのリストを返します．戻り値は，GetNewObjectInfoListの戻り値と同じ順で並べられた，作成結果のオブジェクトのリストになります．
        /// </summary>
        /// <param name="selectedInfoList">選択されているオブジェクト情報のリスト</param>
        /// <param name="args">処理への実引数．GetParametersの戻り値を渡す</param>
        /// <param name="frame">作成元のフレーム</param>
        /// <param name="previewMode"><プレビュー用に結果が要求されているかどうか．trueの場合に戻り値のリストに余分なオブジェクトを追加して返すと，それらのオブジェクトもプレビュー表示用に使われる</param>
        /// <returns></returns>
        IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode);
        /// <summary>
        /// 作成されるオブジェクトの型を取得します．
        /// </summary>
        Type CreatedType { get; }
    }

    public interface IMotionOperationOutputSequence : IMotionOperationBase {
        /// <summary>
        /// 出力されるシーケンスデータのリストを返します．
        /// </summary>
        /// <param name="selected">選択されているオブジェクト情報のリスト</param>
        /// <param name="args">処理への実引数．GetParametersの戻り値を渡す</param>
        /// <param name="frames">フレームデータの列挙</param>
        /// <param name="progressInfo">処理の経過を伝えるためのオブジェクト</param>
        /// <returns></returns>
        IList<Sequence.SequenceData> OutputSequence(IList<MotionObjectInfo> selected, IList<ProcParam<MotionProcEnv>> args, IEnumerable<ReadOnlyMotionFrame> frames, ProgressInformation progressInfo);
    }

    public class MotionOperationScriptFunction : ITimeConsumingScriptFunction {
        readonly ProgressInformation _progress = new ProgressInformation();
        readonly IMotionOperationBase _operation;
        public MotionOperationScriptFunction(IMotionOperationBase operation) {
            if(operation == null) {
                throw new ArgumentNullException("operation", "'operation' cannot be null");
            }
            List<Type> validTypes = new List<Type> { typeof(IMotionOperationGeneral), typeof(IMotionOperationEditObject), typeof(IMotionOperationCreateObject), typeof(IMotionOperationOutputSequence) };
            if(!validTypes.Any(t => t.IsInstanceOfType(operation))) {
                StringBuilder msg = new StringBuilder();
                for(int i = 0; i < validTypes.Count; i++) {
                    if(i == validTypes.Count - 1)
                        msg.Append(" or ");
                    else if(i != 0)
                        msg.Append(", ");
                    msg.Append(validTypes[i].Name);
                }
                throw new ArgumentException("'operation' must be " + msg, "operation");
            }
            _operation = operation;
        }


        #region IScriptFunction メンバ

        public string Name {
            get {
                if(_operation is IMotionOperationCreateObject) {
                    return "Motion_Create_" + _operation.GetCommandName();
                } else if(_operation is IMotionOperationEditObject) {
                    return "Motion_Edit_" + _operation.GetCommandName();
                } else if(_operation is IMotionOperationOutputSequence) {
                    return "Motion_Output_" + _operation.GetCommandName();
                } else {
                    return "Motion_" + _operation.GetCommandName();
                }
            }
        }

        public ScriptVariable Call(IList<ScriptVariable> args, ScriptConsole console) {
            _progress.Initialize(0, "Initializing...");
            if(args == null)
                throw new ArgumentNullException("args", "args cannot be null");
            if(args.Count < 1)
                args = new List<ScriptVariable> { null };
            if(args[0] == null)
                args[0] = new ListVariable();
            IList<ScriptVariable> objectNamesVar = args[0].ToList();
            if(objectNamesVar.Any(n => n.IsNull())) {
                throw new ArgumentException(global::MotionDataHandler.Properties.Settings.Default.Msg_ObjectNameCannotBeNull, "args");
            }
            List<string> objectNames = objectNamesVar.Select(n => n.ToString()).ToList();
            MotionDataSet dataSet = console.MotionDataSet;
            MotionProcEnv env2 = new MotionProcEnv(console);
            List<MotionObjectInfo> infoList = new List<MotionObjectInfo>();
            foreach(string objectName in objectNames) {
                MotionObjectInfo info = dataSet.GetObjectInfoByName(objectName);
                if(info == null)
                    throw new ArgumentException(global::MotionDataHandler.Properties.Settings.Default.Msg_ObjectNotFound + ": " + objectName, "args");
                infoList.Add(info);
            }
            foreach(MotionObjectInfo info in infoList) {
                if(!_operation.FilterSelection(info))
                    throw new ArgumentException(global::MotionDataHandler.Properties.Settings.Default.Msg_InvalidTargetObjectSpecified + ": " + info.Name, "args");
            }
            string errorMessage = "";
            if(!_operation.ValidateSelection(infoList, ref errorMessage)) {
                if(errorMessage == null)
                    errorMessage = "";
                throw new ArgumentException(global::MotionDataHandler.Properties.Settings.Default.Msg_ImproperObjectSelection + ": " + errorMessage, "args");
            }

            IList<ProcParam<MotionProcEnv>> parameters = _operation.GetParameters() ?? new ProcParam<MotionProcEnv>[0];
            if(args.Count != parameters.Count + 1)
                throw new ArgumentException(string.Format(global::MotionDataHandler.Properties.Settings.Default.Msg_NumberOfArgumentsRequired, parameters.Count + 1));
            for(int i = 0; i < parameters.Count; i++) {
                if(!parameters[i].FromScriptVariable(env2, args[i + 1], ref errorMessage)) {
                    throw new ArgumentException(string.Format(global::MotionDataHandler.Properties.Settings.Default.Msg_InvalidNthArgument + ": {1}", i + 1, errorMessage ?? ""), "args");
                }
            }
            if(!_operation.ValidateArguments(parameters, ref errorMessage)) {
                throw new ArgumentException(string.Format(global::MotionDataHandler.Properties.Settings.Default.Msg_InvalidArgument + ": {0}", errorMessage ?? ""), "args");
            }
            IMotionOperationGeneral general = _operation as IMotionOperationGeneral;
            if(general != null) {
                _progress.Initialize(0, "Operation");
                general.Operate(infoList, parameters, dataSet, _progress);
                return new ListVariable(infoList.Select(info => new StringVariable(info.Name)));
            }
            IMotionOperationEditObject edit = _operation as IMotionOperationEditObject;
            if(edit != null) {
                _progress.Initialize(dataSet.FrameLength, "Edit Object");
                foreach(MotionFrame frame in dataSet.EnumerateFrame()) {
                    IList<MotionObject> results = edit.EditObject(infoList, parameters, new ReadOnlyMotionFrame(frame), false);
                    int count = Math.Min(results.Count, infoList.Count);
                    for(int i = 0; i < count; i++) {
                        frame[infoList[i]] = results[i];
                    }
                    _progress.CurrentValue++;
                }
                dataSet.DoFrameListChanged();
                return new ListVariable(infoList.Select(info => new StringVariable(info.Name)));
            }
            IMotionOperationOutputSequence output = _operation as IMotionOperationOutputSequence;
            if(output != null) {
                _progress.Initialize(0, "Output");
                IList<Sequence.SequenceData> sequences = output.OutputSequence(infoList, parameters, dataSet.EnumerateFrame().Select(frame => new ReadOnlyMotionFrame(frame)), _progress);
                foreach(Sequence.SequenceData sequence in sequences) {
                    console.SequenceController.AddSequence(sequence);
                }
                return new ListVariable(sequences.Select(s => new StringVariable(s.Title)));
            }
            IMotionOperationCreateObject create = _operation as IMotionOperationCreateObject;
            if(create != null) {
                _progress.Initialize(dataSet.FrameLength, "Create Object");
                IList<MotionObjectInfo> newInfoList = create.GetNewObjectInfoList(infoList, parameters);
                MotionFrame firstFrame = dataSet.GetFrameByIndex(0);
                if(firstFrame != null) {
                    IList<MotionObject> newObjects = create.CreateObjects(infoList, parameters, new ReadOnlyMotionFrame(firstFrame), false) ?? new MotionObject[0];
                    if(newObjects.Count != newInfoList.Count) {
                        throw new InvalidOperationException(global::MotionDataHandler.Properties.Settings.Default.Msg_CreateObjectLengthMismatch);
                    }
                }
                foreach(MotionObjectInfo newInfo in newInfoList) {
                    dataSet.AddObject(newInfo);
                }
                foreach(MotionFrame frame in dataSet.EnumerateFrame()) {
                    IList<MotionObject> newObjects = create.CreateObjects(infoList, parameters, new ReadOnlyMotionFrame(frame), false) ?? new MotionObject[0];
                    int count = Math.Min(newObjects.Count, newInfoList.Count);
                    for(int i = 0; i < count; i++) {
                        frame[newInfoList[i]] = newObjects[i];
                    }
                    _progress.CurrentValue++;
                }
                dataSet.DoObjectInfoSetChanged();
                dataSet.DoFrameListChanged();
                return new ListVariable(newInfoList.Select(info => new StringVariable(info.Name)));
            }
            return null;
        }

        public string Usage {
            get {
                IList<ProcParam<MotionProcEnv>> parameters = _operation.GetParameters() ?? new ProcParam<MotionProcEnv>[0];
                StringBuilder ret = new StringBuilder("({target object name list}");
                foreach(ProcParam<MotionProcEnv> parameter in parameters) {
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

        #region ITimeConsumingScriptFunction メンバ

        public ProgressChangedEventArgs GetProgress() {
            string msg = _progress.Message ?? "";
            if(_progress.MaxValue != 0) {
                msg = string.Format("{0}: {1} / {2}", msg, _progress.CurrentValue, _progress.MaxValue);
            }
            return new ProgressChangedEventArgs(_progress.GetProgressPercentage(), msg);
        }

        #endregion
    }

    public class MotionOperationExecution {
        private ScriptConsole _console;
        private IMotionOperationBase _operation;
        public MotionProcEnv GetEnvironment() {
            MotionProcEnv env = new MotionProcEnv(_console);
            return env;
        }
        IList<ProcParam<MotionProcEnv>> _parameters = null;
        /// <summary>
        /// 処理に用いられる引数のリストを取得または設定します。
        /// </summary>
        public IList<ProcParam<MotionProcEnv>> Parameters {
            get {
                if(_parameters == null)
                    _parameters = _operation.GetParameters() ?? new ProcParam<MotionProcEnv>[0];
                return _parameters;
            }
            set { _parameters = value; }
        }
        public MotionOperationExecution(IMotionOperationBase operation, ScriptConsole console) {
            if(operation == null)
                throw new ArgumentNullException("operation", "'operation' cannot be null");
            if(console == null)
                throw new ArgumentNullException("console", "'console' cannot be null");
            _operation = operation;
            _console = console;
        }

        public Panel GetPanel() {
            MotionProcEnv env = this.GetEnvironment();

            Panel ret = new Panel();
            ret.Padding = new Padding(8, 8, 8, 8);
            foreach(var param in this.Parameters) {
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

        MotionOperationScriptFunction _opeFunc = null;
        public void Operate(Control parentControl) {
            _opeFunc = new MotionOperationScriptFunction(_operation);
            _console.Invoke(_opeFunc, this.ParameterToVariable(this.Parameters));
        }

        public IList<ScriptVariable> ParameterToVariable(IList<ProcParam<MotionProcEnv>> paramaters) {
            MotionProcEnv env = this.GetEnvironment();
            Collection<ScriptVariable> ret = new Collection<ScriptVariable>();
            ret.Add(new ListVariable(_console.MotionDataSet.GetSelectedObjectInfoList(info => _operation.FilterSelection(info)).Select(info => (ScriptVariable)new StringVariable(info.Name))));
            paramaters.Select(p => p == null ? null : p.ToScriptVariable(env)).ToList().ForEach(p => ret.Add(p));
            return ret;
        }

        public ProgressChangedEventArgs GetProgress() {
            MotionOperationScriptFunction opeFunc = _opeFunc;
            if(opeFunc == null) {
                return new ProgressChangedEventArgs(0, "Initializing...");
            }
            return opeFunc.GetProgress();
        }

        public bool OperateThread(Control parentControl) {
            WaitForForm form = new WaitForForm(ctrl => {
                ctrl.OperationTitle = _operation.GetDescription();
                ctrl.FormTitle = _operation.GetTitle();
                this.Operate(parentControl);
                ctrl.DialogResult = DialogResult.OK;
            }, () => this.GetProgress());
            return form.ShowDialog() == DialogResult.OK;
        }
    }

    /// <summary>
    /// IMotionOperationEditObjectを編集の代わりに作成にしてIMotionOperationCreateObjectとして働かせる用のラッパークラス
    /// </summary>
    public class MotionOperationEditToCreateWrapper : IMotionOperationCreateObject {
        /// <summary>
        /// プライベートコンストラクタ
        /// </summary>
        private MotionOperationEditToCreateWrapper() {

        }

        IMotionOperationEditObject _editOperation;
        public MotionOperationEditToCreateWrapper(IMotionOperationEditObject editOperation) {
            if(editOperation == null)
                throw new ArgumentNullException("editOperation", "'editOperation' cannot be null");
            _editOperation = editOperation;
        }

        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args) {
            List<MotionObjectInfo> ret = new List<MotionObjectInfo>();
            foreach(MotionObjectInfo info in selectedInfoList) {
                MotionObjectInfo newInfo = new MotionObjectInfo(info.ObjectType, info);
                newInfo.Name = PathEx.GiveName(_editOperation.GetCommandName(), info.Name);
                ret.Add(newInfo);
            }
            return ret;
        }

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            return _editOperation.EditObject(selectedInfoList, args, frame, previewMode);
        }

        public Type CreatedType {
            get { return _editOperation.CreatedType; }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return _editOperation.FilterSelection(info);
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            return _editOperation.ValidateSelection(infoList, ref errorMessage);
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            return _editOperation.GetParameters();
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            return _editOperation.ValidateArguments(args, ref errorMessage);
        }

        public string GetCommandName() {
            return _editOperation.GetCommandName();
        }

        public string GetTitle() {
            return "(複製)" + _editOperation.GetTitle();
        }

        public string GetDescription() {
            return "(複製)\r\n" + _editOperation.GetDescription();
        }

        public Bitmap IconBitmap {
            get { return _editOperation.IconBitmap; }
        }

        #endregion
    }
}
