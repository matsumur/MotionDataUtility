using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace MotionDataHandler.Script {
    using Misc;
    using Parse;
    /// <summary>
    /// 
    /// </summary>
    public class ScriptMessageEventArgs : EventArgs {
        public string Message { get; set; }
        public ScriptMessageEventArgs(string message) {
            this.Message = message;
        }
    }
    /// <summary>
    /// 関数呼び出し履歴の一要素となる構造体
    /// </summary>
    public struct FunctionCallHistory {
        /// <summary>
        /// 呼び出された関数
        /// </summary>
        public IScriptFunction Function;
        /// <summary>
        /// 呼び出された時の実引数
        /// </summary>
        public IList<ScriptVariable> Params;
        /// <summary>
        /// 呼び出された時の戻り値
        /// </summary>
        public ScriptVariable Result;
        /// <summary>
        /// 呼び出された時の実時刻
        /// </summary>
        public DateTime CreatedTime;
        /// <summary>
        /// 補助コンストラクタ．作成時刻は自動で設定されます
        /// </summary>
        /// <param name="function">呼び出された関数</param>
        /// <param name="params">呼び出された時の実引数</param>
        /// <param name="result">呼び出された時の戻り値</param>
        public FunctionCallHistory(IScriptFunction function, IList<ScriptVariable> @params, ScriptVariable result) {
            this.Function = function;
            this.Params = @params;
            this.Result = result;
            this.CreatedTime = DateTime.Now;
        }
    }



    /// <summary>
    /// オブジェクトタイプと単一のオブジェクトを関連付けるクラス．
    /// </summary>
    public class ObjectCorkBoard {
        readonly Dictionary<Type, object> _objects = new Dictionary<Type, object>();
        /// <summary>
        /// オブジェクトをタイプに関連付けます．
        /// </summary>
        /// <typeparam name="T">関連付けするタイプ</typeparam>
        /// <param name="object">関連付けされるオブジェクト</param>
        public void Fasten<T>(T @object) where T : class {
            _objects[typeof(T)] = @object;
        }
        /// <summary>
        /// タイプからオブジェクトを取得します
        /// </summary>
        /// <typeparam name="T">タイプ</typeparam>
        /// <returns></returns>
        public T PickUp<T>() where T : class {
            T ret = null;
            object tmp;
            if(_objects.TryGetValue(typeof(T), out tmp)) {
                ret = tmp as T;
            }
            return ret;
        }
    }

    /// <summary>
    /// スクリプト処理の制御用クラス
    /// </summary>
    public class ScriptConsole {
        private bool _constructed = false;
        private readonly List<string> _debugInfoList = new List<string>();
        /// <summary>
        /// デバッグ情報をメッセージ領域に出力します．
        /// </summary>
        /// <param name="message"></param>
        public void WriteDebugInfo(string message) {
            _debugInfoList.Add(message);
            this.Print(message);
        }
        public Collection<string> GetDebugInfoList() {
            return new Collection<string>(_debugInfoList.ToList());
        }
        public void ClearDebugInfo() {
            _debugInfoList.Clear();
        }


        readonly ObjectCorkBoard _corkBoard = new ObjectCorkBoard();
        /// <summary>
        /// 型に対してオブジェクトを関連付けるためのオブジェクトリストを取得します．
        /// </summary>
        public ObjectCorkBoard CorkBoard { get { return _corkBoard; } }
        /// <summary>
        /// スクリプトからの呼び出しのための，現在アクティブなモーションデータセットを取得または設定します
        /// </summary>
        public Motion.MotionDataSet MotionDataSet {
            get { return _corkBoard.PickUp<Motion.MotionDataSet>(); }
            set { _corkBoard.Fasten(value); }
        }
        /// <summary>
        /// スクリプトからの呼び出しのための，現在アクティブなシーケンスコントローラを取得または設定します
        /// </summary>
        public Sequence.SequenceViewerController SequenceController {
            get { return _corkBoard.PickUp<Sequence.SequenceViewerController>(); }
            set { _corkBoard.Fasten(value); }
        }
        /// <summary>
        /// スクリプトからの呼び出しのための，現在アクティブなコントロールを取得または設定します．
        /// </summary>
        public Control ParentControl {
            get { return _corkBoard.PickUp<Control>(); }
            set { _corkBoard.Fasten(value); }
        }

        public static readonly StringComparer StringComparer = StringComparer.CurrentCultureIgnoreCase;
        public static readonly StringComparison StringComparison = StringComparison.CurrentCultureIgnoreCase;

        private ScriptConsole() {
            this.LoadFunctions(Assembly.GetAssembly(typeof(ScriptConsole)));
        }
        /// <summary>
        /// IScriptFunctionを抽出済みのアセンブリの集合
        /// </summary>
        HashSet<Assembly> _registeredAssemblies = new HashSet<Assembly>();

        /// <summary>
        /// アセンブリからIScriptFunctionを実装するクラスのインスタンスを作成して関数に追加します．
        /// </summary>
        /// <param name="assembly">探索対象のアセンブリ</param>
        public void LoadFunctions(Assembly assembly) {
            if(_registeredAssemblies.Contains(assembly))
                return;
            _registeredAssemblies.Add(assembly);
            foreach(var module in assembly.GetModules()) {
                Type[] types;
                try {
                    types = module.GetTypes();
                } catch(ReflectionTypeLoadException ex) {
                    types = ex.Types;
                }
                foreach(var type in types.Where(t => t != null)) {
                    if(type.IsInterface || type.IsAbstract)
                        continue;
                    if(!type.GetInterfaces().Contains(typeof(IScriptFunction)))
                        continue;
                    ConstructorInfo constructor = type.GetConstructor(new Type[0]);
                    if(constructor == null)
                        continue;
                    IScriptFunction function = (IScriptFunction)constructor.Invoke(new object[0]);
                    this.RegisterFunction(function);
                }
            }
        }

        private static ScriptConsole _singleton = new ScriptConsole();
        /// <summary>
        /// 唯一のScriptConsoleインスタンスを取得します
        /// </summary>
        public static ScriptConsole Singleton { get { return _singleton; } }

        /// <summary>
        /// コンソールにメッセージを表示します
        /// </summary>
        /// <param name="message"></param>
        public void Print(string message) {
            EventHandler<ScriptMessageEventArgs> @out = this.Out;
            if(@out != null) {
                @out.Invoke(this, new ScriptMessageEventArgs(message));
            }
        }

        /// <summary>
        /// 実行時情報付きで警告メッセージを作成します
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string formatWarning(string message) {
            SyntaxElement caller;
            try {
                caller = _executedSyntaxElementStack.Peek();
            } catch(InvalidOperationException) {
                return "Unexpected Warn Call\r\n" + message;
            }
            if(_executedFunctionStack.Count > 0) {
                try {
                    return string.Format("{0} :: In Function '{1}', Column {2} at Line {3}", message, _executedFunctionStack.Peek().Name, caller.LexAtStart.Column, caller.LexAtStart.Line);
                } catch(InvalidOperationException) { }
            }
            return string.Format("{0} :: Column {1} at Line {2}", message, caller.LexAtStart.Column, caller.LexAtStart.Line);
        }
        /// <summary>
        /// 実行時情報付きで警告メッセージをコンソールに表示します
        /// </summary>
        /// <param name="message"></param>
        public void Warn(string message) {
            this.Print(formatWarning(message) + "\r\n");
        }
        /// <summary>
        /// 実行中の構文要素のスタック
        /// </summary>
        private Stack<SyntaxElement> _executedSyntaxElementStack = new Stack<SyntaxElement>();
        /// <summary>
        /// 関数呼び出しのスタック
        /// </summary>
        private Stack<IScriptFunction> _executedFunctionStack = new Stack<IScriptFunction>();

        public void PushSyntaxStack(SyntaxElement element) {
            _executedSyntaxElementStack.Push(element);
        }
        public void PopSyntaxStack() {
            _executedSyntaxElementStack.Pop();
        }

        /// <summary>
        /// 文字列の出力先
        /// </summary>
        public EventHandler<ScriptMessageEventArgs> Out;

        readonly private Dictionary<string, IScriptFunction> _functions = new Dictionary<string, IScriptFunction>(ScriptConsole.StringComparer);

        public static string NormalizeIdentifier(string name) {
            name = name.Replace('\r', ' ');
            name = name.Replace('\n', ' ');
            name = name.Replace('\t', ' ');
            name = name.Replace('\f', ' ');
            name = name.Replace('\b', ' ');
            name = name.Replace("　", "");
            name = name.Replace(" ", "");
            return name;
        }
        /// <summary>
        /// 名前から呼べるよう関数をリストに追加します．
        /// </summary>
        /// <param name="subroutine"></param>
        public void RegisterFunction(IScriptFunction function) {
            try {
                if(function == null)
                    throw new ArgumentNullException("function", "'function' cannot be null");
                string name = NormalizeIdentifier(function.Name);
                if(string.IsNullOrEmpty(name))
                    throw new ArgumentException("function.Name cannot be empty", "function");
                string usage = function.Usage;
                if(_functions.ContainsKey(name)) {
                    this.WriteDebugInfo("Function overwritten: " + name);
                }
                _functions[name] = function;
            } catch {
                if(_constructed)
                    throw;
                try {
                    throw;
                } catch(NotImplementedException) {
                    this.WriteDebugInfo(function.GetType().ToString() + " not implemented");
                } catch(Exception ex) {
                    this.WriteDebugInfo(ex.GetType().ToString() + ": " + ex.Message);
                }
            }
        }
        /// <summary>
        /// 関数をリストから削除します．
        /// </summary>
        /// <param name="name"></param>
        public void UnregisterFunction(string name) {
            _functions.Remove(NormalizeIdentifier(name));
        }
        /// <summary>
        /// 関数を名前をもとに呼びます
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public ScriptVariable CallFunction(string name, IList<ScriptVariable> args) {
            IScriptFunction function;
            name = NormalizeIdentifier(name);
            if(!_functions.TryGetValue(name, out function)) {
                _lastSyntaxOnError = _executedSyntaxElementStack.Count > 0 ? _executedSyntaxElementStack.Peek() : null;
                throw new ArgumentException(string.Format("Function '{0}' not found", name));
            }
            return this.Invoke(function, args);
        }

        SyntaxElement _lastSyntaxOnError = null;
        string _lastSubroutineOnError = null;

        public ScriptVariable Invoke(IScriptFunction function, IList<ScriptVariable> args) {
            if(function == null)
                throw new ArgumentNullException("function", "'function' cannot be null");
            if(args == null)
                args = new List<ScriptVariable>();
            bool topLevelInvoke = _executedFunctionStack.Count == 0;
            _executedFunctionStack.Push(function);
            try {
                ScriptVariable result = function.Call(args, this);
                if(topLevelInvoke) {
                    this.PushHistory(new FunctionCallHistory(function, args, result));
                }
                return result;
            } catch {
                _lastSyntaxOnError = _executedSyntaxElementStack.Count > 0 ? _executedSyntaxElementStack.Peek() : null;
                _lastSubroutineOnError = _executedFunctionStack.Peek().Name;
                throw;
            } finally {
                _executedFunctionStack.Pop();
            }
        }


        Queue<FunctionCallHistory> _callHistory = new Queue<FunctionCallHistory>();
        int _historyCapacity = global::MotionDataHandler.Properties.Settings.Default.Script_HistoryCapacity;
        /// <summary>
        /// 関数呼び出しの履歴サイズを取得または設定します．
        /// </summary>
        public int HistoryCapacity {
            get { return _historyCapacity; }
            set {
                if(value < 0)
                    value = 0;
                _historyCapacity = value;
                global::MotionDataHandler.Properties.Settings.Default.Script_HistoryCapacity = value;
            }
        }
        public int HistoryCount { get { return _callHistory.Count; } }
        private bool _suspendHistoryChanged = false;
        /// <summary>
        /// 関数呼び出しを履歴に追加します
        /// </summary>
        /// <param name="call"></param>
        public void PushHistory(FunctionCallHistory call) {
            if(!_suspendHistoryChanged) {
                pushHistoryInternal(call);
                this.DoHistoryChanged();
            }
        }

        void pushHistoryInternal(FunctionCallHistory call) {
            lock(_callHistory) {
                _callHistory.Enqueue(call);
            }
        }

        public void DoHistoryChanged() {
            lock(_callHistory) {
                while(_callHistory.Count > _historyCapacity) {
                    _callHistory.Dequeue();
                }
            }
            EventHandler tmp = HistoryChanged;
            if(tmp != null)
                tmp.Invoke(this, new EventArgs());
        }

        public event EventHandler HistoryChanged;

        /// <summary>
        /// 関数呼び出しの履歴を列挙します．
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FunctionCallHistory> EnumerateCallHistory() {
            lock(_callHistory) {
                return _callHistory.Select(history => history);
            }
        }

        public string paramReplace(ScriptVariable param, Func<ScriptVariable, string> replacer) {
            if(param == null)
                return "null";
            string referResult = replacer(param);
            if(referResult != null) {
                return referResult;
            } else {
                if(param.Type == ScriptVariableType.List) {
                    StringBuilder listStr = new StringBuilder();
                    foreach(ScriptVariable item in param.ToList()) {
                        if(listStr.Length != 0)
                            listStr.Append(", ");
                        listStr.Append(paramReplace(item, replacer));
                    }
                    return "{ " + listStr.ToString() + " }";
                } else {
                    return param.Serialize();
                }
            }
        }

        private void setMap(ScriptVariable resultElem, VariableReplaceMap map, string resultName) {
            if(resultElem != null && (resultElem.Type == ScriptVariableType.List || resultElem.Type == ScriptVariableType.String)) {
                map[resultElem] = resultName;
                if(resultElem.Type == ScriptVariableType.List) {
                    int index = 0;
                    foreach(ScriptVariable elem in resultElem.ToList()) {
                        setMap(elem, map, string.Format("{0}[{1}]", resultName, index));
                        index++;
                    }
                }
            }
        }

        /// <summary>
        /// 関数呼び出しの履歴をスクリプト形式で列挙します．
        /// </summary>
        /// <param name="useReturnValueAsArguments">呼び出しの引数がそれ以前の戻り値と一致する場合に，その戻り値が入った変数を引数として用いるかどうか</param>
        /// <param name="pauseSpan">前回の処理との間隔がこの値以上あいている場合に開始時刻がコメントとして表示される</param>
        /// <returns></returns>
        public string GetSerializedCallHistory(bool useReturnValueAsArguments, bool pickUpCommonArgument, TimeSpan pauseSpan) {
            ParameterizedHistories history = new ParameterizedHistories(this.EnumerateCallHistory(), new char[] { ' ', '　', '/', '_' });
            VariableReplaceMap pickUpReplaces = new VariableReplaceMap();
            ICollection<string> usedVariableNames = new HashSet<string>();
            if(useReturnValueAsArguments) {
                history.ReplaceByResult(ScriptVariableType.List | ScriptVariableType.String, "result", true, VariableNamedState.AllElementsNamed);
            }
            if(pickUpCommonArgument) {
                // 複数回発生した引数を変数に置き換える
                pickUpReplaces = history.GetDefaultReplacement(2, ScriptVariableType.List | ScriptVariableType.String, 32);
                history.ReplaceByName(pickUpReplaces, VariableNamedState.AllElementsNamed);
            }
            usedVariableNames = history.GetUsedVariableNames();

            StringBuilder str = new StringBuilder();
            if(pickUpReplaces.Count > 0) {
                // 複数回発生した引数を変数に置き換えたものの，変数の宣言
                str.AppendLine("// Common Arguments");
                VariableReplaceMap pickupDependencyResolved = new VariableReplaceMap();
                foreach(var pair in from p in pickUpReplaces where usedVariableNames.Contains(p.Value) orderby p.Value select p) {
                    pickupDependencyResolved.Add(pair);
                }
                foreach(var pair in pickupDependencyResolved.GetRecursivelyRenamedVariables(ScriptVariableType.List | ScriptVariableType.String)) {
                    str.AppendLine(string.Format("var {0} = {1};", pair.Value, pair.Key.Serialize()));
                }

                str.AppendLine();
            }
            // 関数呼び出しの出力
            DateTime prev = DateTime.MinValue;
            int index = 0;
            foreach(var call in history.Enumerate()) {
                // 間があいたら実行日時をコメントとして出力
                bool showDate = call.CreatedTime - prev >= pauseSpan;
                if(call.CreatedTime - prev >= pauseSpan) {
                    str.AppendLine("// " + call.CreatedTime.ToString());
                }

                string format = call.GetScriptExpression() + ";";
                index++;
                if(useReturnValueAsArguments) {
                    format = string.Format("var result{0} = {1}", index, format);
                }
                str.AppendLine(format);
                prev = call.CreatedTime;
            }
            return str.ToString();
        }

        /// <summary>
        /// 関数呼び出しの履歴を消去します．
        /// </summary>
        public void ClearHistory() {
            lock(_callHistory) {
                _callHistory.Clear();
            }
            this.DoHistoryChanged();
        }

        /// <summary>
        /// 関数の使用法を取得します．
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetUsage(string name) {
            IScriptFunction subroutine;
            if(!_functions.TryGetValue(name, out subroutine)) {
                throw new ArgumentException(string.Format("SubRoutine '{0}' not found", name));
            }
            return subroutine.Usage;
        }

        /// <summary>
        /// 指定された文字列から始まる関数名のリストを取得します．
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public List<string> GetFunctionNames(string prefix) {
            return (from string name in _functions.Keys where name.StartsWith(prefix, ScriptConsole.StringComparison) orderby name select name).ToList();
        }

        /// <summary>
        /// 関数名のリストを取得します．
        /// </summary>
        /// <returns></returns>
        public List<string> GetFunctionNames() {
            return new List<string>(_functions.Keys.OrderBy(name => name));
        }

        /// <summary>
        /// スクリプトの処理をします．
        /// </summary>
        /// <param name="reader"></param>
        public string Execute(TextReader reader) {
            this.SequenceController.SuspendAllocationChanged();
            try {
                _suspendHistoryChanged = true;
                try {
                    ScriptParser p = new ScriptParser();
                    return p.Execute(reader, new ScriptExecutionEnvironment(this, new VariableStorage(this)));
#if DEBUG
                } catch(ParseException ex) {
                    if(_lastSubroutineOnError == null)
                        _lastSubroutineOnError = "Script Body";
                    if(_lastSyntaxOnError == null)
                        _lastSyntaxOnError = new NullSyntaxElement(new LexicalElement("", LexType.Null, 0, 0, 0, 0));
                    return string.Format("{0}\r\nIn {1}, Column {2} at Line {3}", ex.Message, _lastSubroutineOnError, _lastSyntaxOnError.LexAtStart.Column, _lastSyntaxOnError.LexAtStart.Line);
#else
                } catch(Exception ex) {
                    if(_lastSubroutineOnError == null)
                        _lastSubroutineOnError = "Script Body";
                    if(_lastSyntaxOnError == null)
                        _lastSyntaxOnError = new NullSyntaxElement(new LexicalElement("", LexType.Null, 0, 0, 0, 0));
                    return string.Format("{0}\r\nIn {1}, Column {2} at Line {3}", ex.Message, _lastSubroutineOnError, _lastSyntaxOnError.LexAtStart.Column, _lastSyntaxOnError.LexAtStart.Line);
#endif
                } finally {
                    _suspendHistoryChanged = false;
                    this.DoHistoryChanged();
                }
            } finally {
                this.SequenceController.ResumeAllocationChanged();
            }
        }


        public string ExecuteThread(TextReader reader) {
            string ret = "";
            WaitForForm _waitForForm = new WaitForForm(ctrl => {
                // 実行処理
                try {
                    ret = this.Execute(reader);
                } finally {
                    ctrl.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
            }, () => {
                try {
                    // どの関数にいるか
                    string call = "Script Body";
                    if(_executedFunctionStack.Count > 0) {
                        IScriptFunction subroutine = null;
                        try {
                            _executedFunctionStack.Peek();
                        } catch(InvalidOperationException) { }
                        if(subroutine != null) {
                            ITimeConsumingScriptFunction timeConsume = subroutine as ITimeConsumingScriptFunction;
                            if(timeConsume != null) {
                                return timeConsume.GetProgress();
                            }
                            call = subroutine.Name;
                        }
                    }
                    // どの行にいるか
                    string position = "";
                    if(_executedSyntaxElementStack.Count > 0) {
                        SyntaxElement syntax = _executedSyntaxElementStack.Peek();
                        if(syntax == null) {
                            position = ", ...";
                        } else {
                            position = string.Format(", Column {0} at Line {1}", syntax.LexAtStart.Column, syntax.LexAtStart.Line);
                        }
                    }
                    return new System.ComponentModel.ProgressChangedEventArgs(0, "Process at " + call + position);
                } catch(InvalidOperationException) {
                    return new System.ComponentModel.ProgressChangedEventArgs(0, "");
                }
            });
            _waitForForm.Icon = global::MotionDataHandler.Properties.Resources.script;
            try {
                _waitForForm.CancelEnabled = true;
                if(_waitForForm.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    return ret;
                }
            } finally {
                if(_executedFunctionStack.Count > 0)
                    _executedFunctionStack.Clear();
                if(_executedSyntaxElementStack.Count > 0)
                    _executedSyntaxElementStack.Clear();
            }
            return null;
        }
    }

    /// <summary>
    /// スクリプトの実行環境
    /// </summary>
    public class ScriptExecutionEnvironment {
        public readonly VariableStorage Variables;
        public readonly ScriptConsole Console;
        public ScriptExecutionEnvironment(ScriptConsole console, VariableStorage variables) {
            this.Console = console;
            this.Variables = variables;
        }
    }

    /// <summary>
    /// スクリプト上の関数として機能するためのインターフェース
    /// </summary>
    public interface IScriptFunction {
        /// <summary>
        /// 関数名を取得します．
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 指定された引数をもとに関数を呼びます．
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        ScriptVariable Call(IList<ScriptVariable> args, ScriptConsole console);
        /// <summary>
        /// 関数の使用法を取得します．
        /// </summary>
        string Usage { get; }
    }
    /// <summary>
    /// 処理に時間を要する，スクリプト上の関数として機能するためのインターフェース
    /// </summary>
    public interface ITimeConsumingScriptFunction : IScriptFunction {
        System.ComponentModel.ProgressChangedEventArgs GetProgress();
    }


    /// <summary>
    /// 変数の保持及び名前解決用クラス
    /// </summary>
    public class VariableStorage {
        /// <summary>
        /// 変数の属性保持クラス
        /// </summary>
        public struct FieldProperty {
            public bool Readonly;
            public static FieldProperty Default { get { return new FieldProperty(); } }
        }
        /// <summary>
        /// 変数の属性とその値の組を保持するクラス
        /// </summary>
        public class VariableField {
            public FieldProperty Property;
            public ScriptVariable Variable;
            public VariableField(ScriptVariable variable, FieldProperty property) {
                this.Variable = variable;
                this.Property = property;
            }
        }
        /// <summary>
        /// 親となるScriptConsole
        /// </summary>
        public readonly ScriptConsole Parent;
        /// <summary>
        /// スコープごとの変数名と値の対応表
        /// </summary>
        IList<Dictionary<string, VariableField>> _variableScopes = new List<Dictionary<string, VariableField>>();
        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        /// <param name="scriptEnv">スクリプト実行環境</param>
        public VariableStorage(ScriptConsole parent)
            : this() {
            if(parent == null)
                throw new ArgumentNullException("parent", "'parent' cannot be null");
            this.Parent = parent;
            // Soncoleに登録されている関数を読み取り専用として環境に登録
            FieldProperty readonlyProperty = new FieldProperty();
            readonlyProperty.Readonly = true;
            foreach(string function in parent.GetFunctionNames()) {
                this.Declare(function, new RegisteredFunctionVariable(function), readonlyProperty);
            }
        }

        private VariableStorage() {
            // グローバル変数環境としてひとつスコープに入っておく
            this.EnterScope();
        }
        /// <summary>
        /// 新しいスコープに入ります．
        /// </summary>
        public void EnterScope() {
            _variableScopes.Add(new Dictionary<string, VariableField>(ScriptConsole.StringComparer));
        }
        /// <summary>
        /// 最後のスコープから出ます．
        /// </summary>
        public void ExitScope() {
            if(_variableScopes.Count == 0)
                throw new InvalidOperationException("No Block Exists");
            _variableScopes.RemoveAt(_variableScopes.Count - 1);
        }

        /// <summary>
        /// 指定された名前の変数を新しいスコープから順に探します．見つかった場合にtrue
        /// </summary>
        /// <param name="name">変数名</param>
        /// <param name="variable">変数の格納先</param>
        /// <returns></returns>
        public bool TryLookup(string name, out VariableField field) {
            if(_variableScopes.Count == 0)
                this.EnterScope();
            // 逆順に探す
            foreach(var block in _variableScopes.Reverse()) {
                if(block.TryGetValue(name, out field)) {
                    return true;
                }
            }
            field = null;
            return false;
        }
        /// <summary>
        /// 指定された名前の変数を新しいスコープから順に探します．見つからなければnull
        /// </summary>
        /// <param name="name">変数名</param>
        /// <returns></returns>
        public ScriptVariable LookUp(string name) {
            VariableField field;
            if(!this.TryLookup(name, out field)) {
                ScriptConsole.Singleton.Warn(string.Format("Undefined variable: {0}", name));
                return null;
            }
            return field.Variable;
        }
        /// <summary>
        /// 最新のスコープで変数を宣言します．そのスコープから出た時点でその変数は取り除かれます．
        /// </summary>
        /// <param name="name">変数名</param>
        /// <param name="variable">初期値</param>
        public void Declare(string name, ScriptVariable variable, FieldProperty property) {
            // スコープが空だったらとりあえずひとつ入っておく
            if(_variableScopes.Count == 0)
                this.EnterScope();
            // 既に宣言されていたらコンソールに警告を表示して値は変えない
            if(_variableScopes[_variableScopes.Count - 1].ContainsKey(name)) {
                this.Parent.Warn("Cannot declare already declared variable in the block: " + name);
                return;
            }
            _variableScopes[_variableScopes.Count - 1][name] = new VariableField(variable, property);
        }
        /// <summary>
        /// 指定された名前の変数を新しいスコープから順に探し，値を代入します．
        /// </summary>
        /// <param name="name">変数名</param>
        /// <param name="value">値</param>
        public void Set(string name, ScriptVariable value) {
            // スコープが空だったらとりあえずひとつ入っておく
            if(_variableScopes.Count == 0)
                this.EnterScope();

            VariableField field;
            if(!this.TryLookup(name, out field)) {
                // ない場合にはグローバル変数に突っ込む
                _variableScopes[0][name] = new VariableField(value, FieldProperty.Default);
                return;
            }
            if(field.Property.Readonly) {
                // 読み取り専用だったら変更しない
                this.Parent.Warn(string.Format("Cannot modify readonly variable: {0}", name));
                return;
            }
            // あった場合には新しいスコープから探して最初に見つかったところの値を変える
            foreach(var block in _variableScopes.Reverse()) {
                if(block.ContainsKey(name)) {
                    field.Variable = value;
                    block[name] = field;
                    return;
                }
            }
        }
    }
}
