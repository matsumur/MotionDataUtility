using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace MotionDataHandler.Script {

    /// <summary>
    /// スクリプト変数の名前付けの状態
    /// </summary>
    public enum VariableNamedState {
        None,
        AllElementsNamed,
        AnyElementNamed,
    }

    /// <summary>
    /// 名前付き変数．データ処理の実行履歴に対応するスクリプトを生成する中で，処理の戻り値を以降の処理の引数に利用するためのクラス
    /// </summary>
    public class RefScriptVariable {
        /// <summary>
        /// この変数に名前が付けられているかを取得します
        /// </summary>
        public bool IsNamed { get { return this.Name != null; } }
        /// <summary>
        /// この変数，もしくはこの変数に含まれるすべての要素に名前が付けられているかを取得します
        /// </summary>
        public virtual bool IsAllNamed { get { return this.IsNamed; } }
        /// <summary>
        /// この変数，もしくはこの変数に含まれるいずれかの要素に名前が付けられているかを取得します
        /// </summary>
        public virtual bool IsAnyNamed { get { return this.IsNamed; } }
        /// <summary>
        /// この変数の名前を取得または設定します．
        /// </summary>
        public string Name { get; set; }

        ScriptVariable _value;
        /// <summary>
        /// この変数の値を取得します
        /// </summary>        
        public ScriptVariable Value { get { return _value; } }
        /// <summary>
        /// 内部コンストラクタ
        /// </summary>
        /// <param name="variable">変数の値</param>
        protected RefScriptVariable(ScriptVariable value) { _value = value; }
        /// <summary>
        /// 変数の値の型に応じて名前付きスクリプト変数を作成します
        /// </summary>
        /// <param name="variable">変数の値</param>
        /// <param name="stringDelimiter">文字列を分割する区切り文字</param>
        /// <returns></returns>
        public static RefScriptVariable Create(ScriptVariable value, IEnumerable<char> stringDelimiter) {
            StringVariable str = value as StringVariable;
            if(str != null)
                return new RefConcatStringVariable(str, stringDelimiter);
            ListVariable list = value as ListVariable;
            if(list != null)
                return new RefListVariable(list, stringDelimiter);
            return new RefScriptVariable(value);
        }
        /// <summary>
        /// この変数の名前もしくは内容を示すスクリプト文字列を取得します
        /// </summary>
        /// <returns></returns>
        public virtual string Serialize() {
            if(this.IsNamed)
                return this.Name;
            if(this.Value == null)
                return "null";
            return this.Value.Serialize();
        }
        /// <summary>
        /// この変数の名前付けが指定された状態を満たすかを返します
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool SatisfiesNamedState(VariableNamedState state) {
            if(state == VariableNamedState.AllElementsNamed && this.IsAllNamed)
                return true;
            if(state == VariableNamedState.AnyElementNamed && this.IsAnyNamed)
                return true;
            return false;
        }
        /// <summary>
        /// 指定された変換を用いて変数に名前をつけ，変換された名前の一覧を返します
        /// </summary>
        /// <param name="map">変数の値と，その変数に付ける名前のディクショナリ</param>
        /// <param name="ignoreState">名前付けをしない条件</param>
        public virtual IList<string> ReplaceByName(IDictionary<ScriptVariable, string> map, VariableNamedState ignoreState) {
            List<string> ret = new List<string>();
            if(this.SatisfiesNamedState(ignoreState))
                return ret;
            if(this.Value == null)
                return ret;
            string name;
            if(map.TryGetValue(this.Value, out name)) {
                this.Name = name;
                ret.Add(name);
            }
            return ret;
        }
        /// <summary>
        /// コレクションにこの変数の名前を追加します
        /// </summary>
        /// <param name="names"></param>
        public virtual void AddUsedNameTo(ICollection<string> names) {
            if(this.IsNamed) {
                names.Add(this.Name);
            }
        }

        public virtual void Rename(IDictionary<string, string> renameMap) {
            if(this.IsNamed) {
                string replace;
                if(renameMap.TryGetValue(this.Name, out replace))
                    this.Name = replace;
            }
        }
    }

    /// <summary>
    /// 区切り文字によって分解された文字列の，語と区切り文字のペアを保持するクラス．区切り文字は語の直前にある区切り文字の方を対象にする
    /// </summary>
    public class ConcatString {
        /// <summary>
        /// 語部分の名前が付けられているかを取得します
        /// </summary>
        public bool IsNamed { get { return this.Name != null; } }
        /// <summary>
        /// 区切り文字列を取得します
        /// </summary>
        public readonly string Delimiter;
        /// <summary>
        /// 語部分の文字列を取得します
        /// </summary>
        public readonly string Word;
        /// <summary>
        /// 語部分の名前を取得または設定します
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        /// <param name="delimiter">区切り文字列</param>
        /// <param name="word">語</param>
        public ConcatString(string delimiter, string word) {
            if(delimiter == null)
                throw new ArgumentNullException("delimiter", "'delimiter' cannot be null");
            if(word == null)
                throw new ArgumentNullException("word", "'word' cannot be null");
            this.Delimiter = delimiter;
            this.Word = word;
        }
        /// <summary>
        /// この変数の名前付けが指定された状態を満たすかを返します
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool SatisfiesNamedState(VariableNamedState state) {
            if(state == VariableNamedState.AllElementsNamed && this.IsNamed)
                return true;
            if(state == VariableNamedState.AnyElementNamed && this.IsNamed)
                return true;
            return false;
        }


        /// <summary>
        /// 指定された変換を用いて変数に名前をつけます
        /// </summary>
        /// <param name="map">変数の値と，その変数に付ける名前のディクショナリ</param>
        /// <param name="ignoreState">名前付けをしない条件</param>
        public IList<string> ReplaceByName(IDictionary<ScriptVariable, string> map, VariableNamedState ignoreState) {
            List<string> ret = new List<string>();
            if(this.SatisfiesNamedState(ignoreState))
                return ret;
            string name;
            if(map.TryGetValue(new StringVariable(this.Word), out name)) {
                this.Name = name;
                ret.Add(name);
            }
            return ret;
        }
    }

    /// <summary>
    /// 分解された文字列を保持する名前付き変数
    /// </summary>
    public class RefConcatStringVariable : RefScriptVariable {
        public override bool IsAllNamed { get { return this.IsNamed || this.Strings.All(c => c.IsNamed || c.Word == ""); } }
        public override bool IsAnyNamed { get { return this.IsNamed || this.Strings.Any(c => c.IsNamed && c.Word != ""); } }
        private readonly IList<ConcatString> _strings;
        public ReadOnlyCollection<ConcatString> Strings { get { return new ReadOnlyCollection<ConcatString>(_strings); } }
        public RefConcatStringVariable(StringVariable variable, IEnumerable<char> stringDelimiters)
            : base(variable) {
            List<char> delimiters = stringDelimiters.ToList();
            List<ConcatString> strings = new List<ConcatString>();
            string text = variable.Value;
            int index = 0;
            while(index < text.Length) {
                string delimiter, word;

                // 区切り文字を数えて切り取る
                int delimiterBeginIndex = index;
                while(index < text.Length && delimiters.Contains(text[index]))
                    index++;
                delimiter = text.Substring(delimiterBeginIndex, index - delimiterBeginIndex);

                // 非区切り文字を数えて切り取る
                int wordBeginIndex = index;
                while(index < text.Length && !delimiters.Contains(text[index]))
                    index++;
                word = text.Substring(wordBeginIndex, index - wordBeginIndex);

                strings.Add(new ConcatString(delimiter, word));
            }
            _strings = strings;
        }

        public override IList<string> ReplaceByName(IDictionary<ScriptVariable, string> map, VariableNamedState ignoreState) {
            List<string> ret = new List<string>();
            if(this.SatisfiesNamedState(ignoreState))
                return ret;
            if(this.Value == null)
                return ret;
            string name;
            if(map.TryGetValue(this.Value, out name)) {
                this.Name = name;
                ret.Add(name);
            }
            foreach(var sub in this.Strings) {
                ret.AddRange(sub.ReplaceByName(map, ignoreState));
            }
            return ret;
        }

        public override string Serialize() {
            if(this.IsNamed)
                return this.Name;
            if(this.Strings.Any(c => c.IsNamed)) {
                StringBuilder ret = new StringBuilder();
                StringBuilder substr = new StringBuilder();
                foreach(var c in this.Strings) {
                    substr.Append(c.Delimiter);
                    if(c.IsNamed) {
                        if(ret.Length > 0)
                            ret.Append(" + ");
                        if(substr.Length > 0) {
                            ret.Append(new StringVariable(substr.ToString()).Serialize());
                            ret.Append(" + ");
                        }
                        ret.Append(c.Name);
                        substr = new StringBuilder();
                    } else {
                        substr.Append(c.Word);
                    }
                }
                if(substr.Length > 0) {
                    if(ret.Length > 0) {
                        ret.Append(" + ");
                    }
                    ret.Append(new StringVariable(substr.ToString()).Serialize());
                }
                return ret.ToString();
            }
            return this.Value.Serialize();
        }


        public override void AddUsedNameTo(ICollection<string> names) {
            if(this.IsNamed) {
                names.Add(this.Name);
            } else {
                foreach(var c in this.Strings) {
                    if(c.IsNamed) {
                        names.Add(c.Name);
                    }
                }
            }
        }

        public override void Rename(IDictionary<string, string> renameMap) {
            base.Rename(renameMap);
            foreach(var c in this.Strings) {
                if(c.IsNamed) {
                    string replace;
                    if(renameMap.TryGetValue(c.Name, out replace))
                        c.Name = replace;
                }
            }
        }
    }

    /// <summary>
    /// 名前付き変数のリストを保持する名前付き変数クラス
    /// </summary>
    public class RefListVariable : RefScriptVariable {
        public override bool IsAllNamed { get { return this.IsNamed || this.List.All(i => i.IsAllNamed); } }
        public override bool IsAnyNamed { get { return this.IsNamed || this.List.Any(i => i.IsAnyNamed); } }
        private List<RefScriptVariable> _list;
        public ReadOnlyCollection<RefScriptVariable> List { get { return new ReadOnlyCollection<RefScriptVariable>(_list); } }
        public RefListVariable(ListVariable listVariable, IEnumerable<char> stringDelimiters)
            : base(listVariable) {
            _list = listVariable.Value.Select(v => RefScriptVariable.Create(v, stringDelimiters)).ToList();
        }

        public override string Serialize() {
            if(this.IsNamed)
                return this.Name;
            StringBuilder str = new StringBuilder("{ ");
            bool first = true;
            foreach(RefScriptVariable item in this.List) {
                if(first)
                    first = false;
                else
                    str.Append(", ");
                if(item == null) {
                    str.Append("null");
                } else {
                    str.Append(item.Serialize());
                }
            }
            str.Append(" }");
            return str.ToString();
        }

        public override IList<string> ReplaceByName(IDictionary<ScriptVariable, string> map, VariableNamedState ignoreState) {
            List<string> ret = new List<string>();
            if(this.SatisfiesNamedState(ignoreState))
                return ret;
            if(this.Value == null)
                return ret;
            string name;
            if(map.TryGetValue(this.Value, out name)) {
                this.Name = name;
                ret.Add(name);
            }

            foreach(var sub in this.List) {
                ret.AddRange(sub.ReplaceByName(map, ignoreState));
            }
            return ret;
        }

        public override void AddUsedNameTo(ICollection<string> names) {
            if(this.IsNamed) {
                names.Add(this.Name);
            } else {
                foreach(var sub in this.List) {
                    sub.AddUsedNameTo(names);
                }
            }
        }

        public override void Rename(IDictionary<string, string> renameMap) {
            base.Rename(renameMap);
            foreach(var sub in this.List) {
                sub.Rename(renameMap);
            }
        }
    }

    /// <summary>
    /// 名前付き引数を保持する関数呼び出し情報
    /// </summary>
    public struct RefFunctionCallHistory {
        public IScriptFunction Function;
        public IList<RefScriptVariable> Params;
        public ScriptVariable Result;
        public DateTime CreatedTime;
        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        /// <param name="history">基となる関数呼び出し履歴</param>
        /// <param name="stringDelimiters">文字列を分解する区切り文字</param>
        public RefFunctionCallHistory(FunctionCallHistory history, IEnumerable<char> stringDelimiters) {
            this.Function = history.Function;
            this.Params = history.Params.Select(p => RefScriptVariable.Create(p, stringDelimiters)).ToList();
            this.Result = history.Result;
            this.CreatedTime = history.CreatedTime;
        }
        /// <summary>
        /// 関数呼び出し式を表すスクリプト文字列を返します
        /// </summary>
        /// <returns></returns>
        public string GetScriptExpression() {
            StringBuilder args = new StringBuilder();
            foreach(var p in this.Params) {
                if(args.Length != 0)
                    args.Append(", ");
                args.Append(p.Serialize());
            }
            return string.Format("{0}({1})", this.Function.Name, args);
        }

        /// <summary>
        /// 指定された変換を用いて引数に名前をつけます
        /// </summary>
        /// <param name="map">変数の値と，その変数に付ける名前のディクショナリ</param>
        /// <param name="ignoreState">名前付けをしない条件</param>
        public void ReplaceByName(IDictionary<ScriptVariable, string> map, VariableNamedState ignoreState) {
            foreach(var param in this.Params) {
                param.ReplaceByName(map, ignoreState);
            }
        }
    }
    /// <summary>
    /// 変数の値を比較する比較器
    /// </summary>
    public class VariableValueEqualityComparer : IEqualityComparer<ScriptVariable> {
        #region IEqualityComparer<ScriptVariable> メンバ

        public bool Equals(ScriptVariable x, ScriptVariable y) {
            return x.EqualTo(y);
        }

        public int GetHashCode(ScriptVariable obj) {
            int ret = 0;
            if(obj != null) {
                switch(obj.Type) {
                case ScriptVariableType.Boolean:
                    return obj.ToBoolean().GetHashCode();
                case ScriptVariableType.Function:
                    return ((FunctionVariable)obj).Body.GetHashCode();
                case ScriptVariableType.List:
                    foreach(var v in obj.ToList()) {
                        ret += this.GetHashCode(v);
                    }
                    return ret;
                case ScriptVariableType.Number:
                    return obj.ToNumber().GetHashCode();
                case ScriptVariableType.RegisteredFunction:
                    return ((RegisteredFunctionVariable)obj).Name.GetHashCode();
                case ScriptVariableType.String:
                    return obj.ToString().GetHashCode();
                }
            }
            return ret;
        }

        #endregion
    }

    /// <summary>
    /// 変数の値と変数の名前の相互変換を保持するクラス
    /// </summary>
    public class VariableReplaceMap : Misc.BijectiveDictionary<ScriptVariable, string> {
        public VariableReplaceMap()
            : base(new VariableValueEqualityComparer(), StringComparer.CurrentCulture) {
        }
        /// <summary>
        /// 変数の名前が重複しないように変数名を変更して追加し，その名前を返します．
        /// </summary>
        /// <param name="variable">変数の値</param>
        /// <param name="name">変数名</param>
        /// <param name="maxIdentifierLength">長い変数名を切り捨てる文字列長．同じ変数名が複数ある場合，この値より長い変数名が生成される可能性があります</param>
        /// <returns></returns>
        public string AddWithRename(ScriptVariable value, string name, int maxIdentifierLength) {
            string tmp;
            if(this.TryGetValue(value, out tmp)) {
                return tmp;
            }
            name = Parse.ScriptParser.ConvertStringIntoIdentifier(name);
            name = name.Substring(0, Math.Min(maxIdentifierLength, name.Length));
            if(name == "")
                name = "_";
            string identifier = name;
            int index = 0;
            while(this.ContainsValue(identifier)) {
                index++;
                identifier = name + index.ToString();
            }
            this.Add(value, identifier);
            return identifier;
        }

        /// <summary>
        /// 変数名をキーにした名前付き変数のリスト
        /// </summary>
        private class RefVariableByName : KeyedCollection<string, RefScriptVariable> {
            protected override string GetKeyForItem(RefScriptVariable item) {
                return item.Name;
            }
        }

        /// <summary>
        /// 値を変数に置き換える部分の，置き換えられる変数の中身も変数で置き換えたものを返します
        /// </summary>
        /// <param name="targetTypes"></param>
        /// <returns></returns>
        public IList<KeyValuePair<RefScriptVariable, string>> GetRecursivelyRenamedVariables(ScriptVariableType targetTypes) {
            Dictionary<string, IList<string>> dependencies = new Dictionary<string, IList<string>>();
            // 変数一覧を作成
            RefVariableByName variables = new RefVariableByName();
            foreach(var pair in this) {
                RefScriptVariable refVar = RefScriptVariable.Create(pair.Key, "");
                refVar.Name = pair.Value;
                variables.Add(refVar);
            }
            // 変数の値がリストだったら，各要素を名前付き変数で置き換えられるものは置き換える
            foreach(var variable in variables) {
                RefListVariable list = variable as RefListVariable;
                if(list != null) {
                    foreach(var elem in list.List) {
                        IList<string> replacers = elem.ReplaceByName(this, VariableNamedState.None);
                        // 変換された変数から，変換に利用した変数を求められるようにする
                        foreach(string replacer in replacers) {
                            if(!dependencies.ContainsKey(variable.Name)) {
                                dependencies[variable.Name] = new List<string>();
                            }
                            dependencies[variable.Name].Add(replacer);
                        }
                    }
                }
            }
            HashSet<RefScriptVariable> used = new HashSet<RefScriptVariable>();
            Stack<RefScriptVariable> subVariables = new Stack<RefScriptVariable>(variables);
            List<KeyValuePair<RefScriptVariable, string>> ret = new List<KeyValuePair<RefScriptVariable, string>>();
            while(subVariables.Count > 0) {
                // 確認対象
                RefScriptVariable targetVar = subVariables.Peek();
                if(!used.Contains(targetVar) && targetVar.Name != null) {
                    string name = targetVar.Name;
                    // 対象の変数の部分または要素が他の変数で置き換えられているときには，
                    // その置き換える変数の方を先に確認する
                    IList<string> dependNames;
                    if(dependencies.TryGetValue(name, out dependNames)) {                        
                        bool dependExists = false;
                        foreach(string dependName in dependNames) {
                            RefScriptVariable dependVar = variables[dependName];
                            if(dependVar != null && !used.Contains(dependVar)) {
                                subVariables.Push(dependVar);
                                dependExists = true;
                            }
                        }
                        if(dependExists) {
                            continue;
                        }
                    }
                    // 
                    targetVar.Name = null;
                    ret.Add(new KeyValuePair<RefScriptVariable, string>(targetVar, name));
                }
                used.Add(targetVar);
                subVariables.Pop();
                continue;
            }
            return ret;
        }
    }

    /// <summary>
    /// 引数を変数で置き換えた関数呼び出し履歴を保持するクラス
    /// </summary>
    public class ParameterizedHistories {
        List<RefFunctionCallHistory> _histories = new List<RefFunctionCallHistory>();
        IList<char> _stringDelimiters;
        VariableReplaceMap _replacements = new VariableReplaceMap();
        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        /// <param name="histories">関数呼び出し履歴の列挙</param>
        /// <param name="stringDelimiters">引数の文字列を分割する区切り文字</param>
        public ParameterizedHistories(IEnumerable<FunctionCallHistory> histories, IEnumerable<char> stringDelimiters) {
            _stringDelimiters = stringDelimiters.ToArray();
            foreach(var history in histories) {
                _histories.Add(new RefFunctionCallHistory(history, _stringDelimiters));
            }
        }
        /// <summary>
        /// 履歴を列挙します
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RefFunctionCallHistory> Enumerate() {
            foreach(RefFunctionCallHistory call in _histories) {
                yield return call;
            }
        }

        private void replaceByResultAux(ScriptVariable resultElem, VariableReplaceMap map, string resultName, ScriptVariableType targetTypes) {
            if(resultElem != null && (resultElem.Type & targetTypes) != 0) {
                map[resultElem] = resultName;
                if(resultElem.Type == ScriptVariableType.List) {
                    int index = 0;
                    foreach(ScriptVariable elem in resultElem.ToList()) {
                        replaceByResultAux(elem, map, string.Format("{0}[{1}]", resultName, index), targetTypes);
                        index++;
                    }
                }
            }
        }

        /// <summary>
        /// プレフィックスと順序番号から戻り値受け取り用の変数名を作成します
        /// </summary>
        /// <param name="prefix">プレフィックス</param>
        /// <param name="index">順序番号</param>
        /// <returns></returns>
        public static string ConstructResultName(string prefix, int index) {
            return Parse.ScriptParser.ConvertStringIntoIdentifier(prefix + index.ToString());
        }

        /// <summary>
        /// 関数の戻り値を用いて引数を変数で置き換えます
        /// </summary>
        /// <param name="targetTypes">置き換える対象の変数の値の型</param>
        /// <param name="resultPrefix">戻り値を受け取る</param>
        /// <param name="useIndexerOfResult">戻り値のインデクサの値を用いるかどうか</param>
        /// <param name="ignoreState">置き換えの対象としない条件</param>
        public void ReplaceByResult(ScriptVariableType targetTypes, string resultPrefix, bool useIndexerOfResult, VariableNamedState ignoreState) {
            int index = 0;
            foreach(var refCall in _histories) {
                refCall.ReplaceByName(_replacements, ignoreState);
                index++;
                replaceByResultAux(refCall.Result, _replacements, ConstructResultName(resultPrefix, index), targetTypes);
            }
        }

        public void getOccuranceOfVariablesAux(IDictionary<ScriptVariable, int> map, IList<RefScriptVariable> variables, ScriptVariableType targetTypes, VariableNamedState ignoreState, ICollection<ScriptVariable> ignoreVariables) {
            foreach(var variable in variables) {
                if(variable.SatisfiesNamedState(ignoreState))
                    continue;
                if(variable.Value == null)
                    continue;
                if(!variable.Value.IsTypeOf(targetTypes))
                    continue;
                if(!ignoreVariables.Contains(variable.Value)) {
                    if(!map.ContainsKey(variable.Value)) {
                        map[variable.Value] = 0;
                    }
                    map[variable.Value]++;
                }
                RefListVariable list = variable as RefListVariable;
                RefConcatStringVariable concat = variable as RefConcatStringVariable;
                if(list != null) {
                    getOccuranceOfVariablesAux(map, list.List, targetTypes, ignoreState, ignoreVariables);
                } else if(concat != null) {
                    if(concat.Strings.Count == 1 && concat.Strings[0].Word == concat.Value.ToString())
                        continue;
                    foreach(var c in concat.Strings) {
                        if(c.SatisfiesNamedState(ignoreState))
                            continue;
                        StringVariable strVar = new StringVariable(c.Word);
                        if(!ignoreVariables.Contains(strVar)) {
                            if(!map.ContainsKey(strVar)) {
                                map[strVar] = 0;
                            }
                            map[strVar]++;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 変数の値と，その値が利用されている回数のペアのディクショナリを返します
        /// </summary>
        /// <param name="targetTypes">回数を数える変数の値の型</param>
        /// <param name="ignoreState">回数を数える対象としない条件</param>
        /// <param name="ignoreVariables">回数を数えない変数の値</param>
        /// <returns></returns>
        public IDictionary<ScriptVariable, int> GetOccuranceOfVariables(ScriptVariableType targetTypes, VariableNamedState ignoreState, ICollection<ScriptVariable> ignoreVariables) {
            Dictionary<ScriptVariable, int> ret = new Dictionary<ScriptVariable, int>(new VariableValueEqualityComparer());
            foreach(var refCall in _histories) {
                getOccuranceOfVariablesAux(ret, refCall.Params, targetTypes, ignoreState, ignoreVariables);
            }
            return ret;
        }

        /// <summary>
        /// 標準的な引数の置き換えのディクショナリを返します
        /// </summary>
        /// <param name="minimumOccurance">置き換え対象とする変数の値の生起回数の条件</param>
        /// <param name="targetTypes">置き換えの対象とする変数の値の型</param>
        /// <param name="maxIdentifierLength">置き換える名前のプレフィックス部の最大長</param>
        /// <returns></returns>
        public VariableReplaceMap GetDefaultReplacement(int minimumOccurance, ScriptVariableType targetTypes, int maxIdentifierLength) {
            HashSet<ScriptVariable> replacement = new HashSet<ScriptVariable>(new VariableValueEqualityComparer());

            bool replaced = true;
            while(replaced) {
                replaced = false;
                var none = this.GetOccuranceOfVariables(ScriptVariableType.List | targetTypes, VariableNamedState.AnyElementNamed, replacement);
                var sub = none.Where(p => p.Key.IsTypeOf(targetTypes) && p.Value >= minimumOccurance).ToList();
                if(sub.Any()) {
                    int maxOccur = sub.Max(p => p.Value);
                    foreach(var pair in sub.Where(p => p.Value == maxOccur)) {
                        if(replacement.Add(pair.Key)) {
                            replaced = true;
                        }
                    }
                }
            }

            var one = this.GetOccuranceOfVariables(ScriptVariableType.List | targetTypes, VariableNamedState.AllElementsNamed, replacement);
            var subOne = one.Where(p => p.Key.IsTypeOf(targetTypes) && !p.Key.IsTypeOf(ScriptVariableType.List) && p.Value >= minimumOccurance).ToList();
            foreach(var pair in subOne) {
                if(replacement.Add(pair.Key)) {
                    replaced = true;
                }
            }

            VariableReplaceMap ret = new VariableReplaceMap();
            foreach(ScriptVariable v in replacement) {
                string abbrev = Parse.ScriptParser.ConvertStringIntoIdentifier(v.ToString());
                string identifier = abbrev;
                int index = 0;
                while(ret.ContainsValue(identifier) || _replacements.ContainsValue(identifier)) {
                    index++;
                    identifier = abbrev + index.ToString();
                }
                ret.AddWithRename(v, identifier, maxIdentifierLength);
            }
            return ret;
        }
        /// <summary>
        /// 実際に置き換えの起こる変数の名前のコレクションを返します
        /// </summary>
        /// <returns></returns>
        public ICollection<string> GetUsedVariableNames() {
            HashSet<string> ret = new HashSet<string>();
            foreach(var refCall in _histories) {
                foreach(var param in refCall.Params) {
                    param.AddUsedNameTo(ret);
                }
            }
            return ret;
        }

        public void RenameVariableNames(IDictionary<string, string> renameMap) {
            foreach(var refCall in _histories) {
                foreach(var param in refCall.Params) {
                    param.Rename(renameMap);
                }
            }
        }

        /// <summary>
        /// 指定された変換を用いて引数に名前をつけます
        /// </summary>
        /// <param name="map">変数の値と，その変数に付ける名前のディクショナリ</param>
        /// <param name="ignoreState">名前付けをしない条件</param>
        public void ReplaceByName(IDictionary<ScriptVariable, string> map, VariableNamedState ignoreState) {
            foreach(var refCall in _histories) {
                refCall.ReplaceByName(map, ignoreState);
            }
        }

    }


}
