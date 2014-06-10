using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
namespace LLParserGenerator {
    /// <summary>
    /// 型がクラスであるか構造体であるかを保持するための列挙型
    /// </summary>
    enum ClassOrStruct {
        /// <summary>
        /// 構造体
        /// </summary>
        Struct,
        /// <summary>
        /// クラス
        /// </summary>
        Class,
    }
    /// <summary>
    /// 型またはプロパティのアクセスレベルを保持するための列挙型
    /// </summary>
    enum Accessibility {
        /// <summary>
        /// 既定値を仕様
        /// </summary>
        None,
        /// <summary>
        /// パブリック
        /// </summary>
        Public,
        /// <summary>
        /// プロテクテッド
        /// </summary>
        Protected,
        /// <summary>
        /// プライベート
        /// </summary>
        Private,
        /// <summary>
        /// 内部のみ
        /// </summary>
        Internal,
        /// <summary>
        /// プロテクテッドまたは内部のみ
        /// </summary>
        ProtectedInteral,
    }
    /// <summary>
    /// 識別子の大文字・小文字変換の設定を保持する列挙型
    /// </summary>
    enum CaseConversion {
        /// <summary>
        /// 変換なし
        /// </summary>
        None,
        /// <summary>
        /// すべて小文字
        /// </summary>
        Lower,
        /// <summary>
        /// 先頭のみ大文字
        /// </summary>
        Capitalized,
        /// <summary>
        /// すべて大文字
        /// </summary>
        Upper,
    }
    /// <summary>
    /// パーサ生成器の設定を保持するクラス
    /// </summary>
    class ScriptParserGeneratorSettings {
        /// <summary>
        /// 構文解析を開始するパースメソッドに対応する非終端記号
        /// </summary>
        public string Start = null;
        /// <summary>
        /// 出力結果の名前空間
        /// </summary>
        public string NameSpace = "Namespace1";
        /// <summary>
        /// 出力結果のusing対象のリスト
        /// </summary>
        public List<string> Using = new List<string>();
        /// <summary>
        /// 出力される本体クラス名
        /// </summary>
        public string OutputClass = "Parser1";
        /// <summary>
        /// 出力される本体クラスのアクセスレベル
        /// </summary>
        public Accessibility OutputClassAccessibility = Accessibility.None;
        /// <summary>
        /// 入力となる各字句要素の型
        /// </summary>
        public string InputClass = "Lexis1";
        /// <summary>
        /// 入力となる字句要素の型のタイプ
        /// </summary>
        public ClassOrStruct InputClassType = ClassOrStruct.Class;
        /// <summary>
        /// 入力となる字句要素が保持する要素の種類に対応する型
        /// </summary>
        public string InputEnum = "LexisType1";
        /// <summary>
        /// 非終端記号ごとの構文解析用メソッドの名前に前置される文字列
        /// </summary>
        public string ParseMethodPrefix = "Parse";
        /// <summary>
        /// 非終端記号ごとの構文解析用メソッドの名前に後置される文字列
        /// </summary>
        public string ParseMethodSuffix = "";
        /// <summary>
        /// 非終端記号ごとの構文解析用メソッドのアクセスレベル
        /// </summary>
        public Accessibility ParseMethodAccessibility = Accessibility.Private;
        /// <summary>
        /// 非終端記号ごとの構文要素を作成して返すメソッドの名前に前置される文字列
        /// </summary>
        public string ReturnMethodPrefix = "Return";
        /// <summary>
        /// 非終端記号ごとの構文要素を作成して返すメソッドの名前に後置される文字列
        /// </summary>
        public string ReturnMethodSuffix = "";
        /// <summary>
        /// 非終端記号ごとの構文要素を作成して返すメソッドのアクセスレベル
        /// </summary>
        public Accessibility ReturnMethodAccessibility = Accessibility.Protected;
        /// <summary>
        /// 非終端記号ごとの構文要素に対応する型の名前に前置される文字列
        /// </summary>
        public string ReturnClassPrefix = "";
        /// <summary>
        /// 非終端記号ごとの構文要素に対応する型の名前に後置される文字列
        /// </summary>
        public string ReturnClassSuffix = "Element";
        /// <summary>
        /// 非終端記号ごとの構文要素に対応する型のアクセスレベル
        /// </summary>
        public Accessibility ReturnClassAccessibility = Accessibility.None;
        /// <summary>
        /// 非終端記号ごとの構文要素に対応する型のタイプ
        /// </summary>
        public ClassOrStruct ReturnClassType = ClassOrStruct.Class;
        /// <summary>
        /// 非終端記号から対応する識別子を作成する場合の大文字・小文字変換
        /// </summary>
        public CaseConversion NonterminalCaseConversion = CaseConversion.Capitalized;
        /// <summary>
        /// 終端記号から対応する識別子を作成する場合の大文字・小文字変換
        /// </summary>
        public CaseConversion TerminalCaseConversion = CaseConversion.Capitalized;
        /// <summary>
        /// 非終端記号から既定ではない構文要素を返す場合の名前変換
        /// </summary>
        public Dictionary<string, string> Returns = new Dictionary<string, string>();
        /// <summary>
        /// 入力データの終端記号から既定ではない要素列挙型を返す場合の名前変換
        /// </summary>
        public Dictionary<string, string> Terminal = new Dictionary<string, string>();
        /// <summary>
        /// クラスのフィールドを取得します．
        /// </summary>
        /// <returns></returns>
        public static FieldInfo[] GetFields() {
            return typeof(ScriptParserGeneratorSettings).GetFields();
        }
    }
    /// <summary>
    /// メソッドのパラメータの型の種類を表す列挙型
    /// </summary>
    enum SignatureType {
        /// <summary>
        /// 固定長でさまざまな型を保持するリスト型
        /// </summary>
        FixedList,
        /// <summary>
        /// データがない場合がある型
        /// </summary>
        Optional,
        /// <summary>
        /// 異なる型のうちどれか一つだけ有効なデータを保持する型
        /// </summary>
        Selection,
        /// <summary>
        /// 同じ型を0個以上保持する型
        /// </summary>
        Array,
        /// <summary>
        /// 子を持たない特定の型
        /// </summary>
        TerminalClass,
    }
    /// <summary>
    /// メソッドのパラメータの型をツリー構造で保持するクラス
    /// </summary>
    class ParameterSignature {
        /// <summary>
        /// 子となる型のリスト
        /// </summary>
        List<ParameterSignature> _children;
        /// <summary>
        /// この型の種類
        /// </summary>
        SignatureType _type;
        /// <summary>
        /// この型に対応する識別子
        /// </summary>
        string _identifier;
        /// <summary>
        /// 生成された変数名
        /// </summary>
        string _paramName;
        /// <summary>
        /// この型に対応する識別子名を取得します
        /// </summary>
        public string Identifier { get { return _identifier; } }
        /// <summary>
        /// この型の種類を取得します．
        /// </summary>
        public SignatureType Type { get { return _type; } }
        /// <summary>
        /// この型が持つ子となる型の読み取り専用リストを取得します．
        /// </summary>
        public ReadOnlyCollection<ParameterSignature> Children { get { return new ReadOnlyCollection<ParameterSignature>(_children); } }
        /// <summary>
        /// 生成された変数名を取得または設定します．
        /// </summary>
        public string ParamName { get { return _paramName; } set { _paramName = value; } }
        /// <summary>
        /// 子要素を持たない型を作成するコンストラクタ
        /// </summary>
        /// <param name="paramName">変数名</param>
        /// <param name="type">型の種類</param>
        public ParameterSignature(string paramName, SignatureType type)
            : this(paramName, type, new List<ParameterSignature>()) {
        }
        /// <summary>
        /// 一定数の子要素を持つ型を作成するコンストラクタ
        /// </summary>
        /// <param name="paramName">変数名</param>
        /// <param name="type">型の種類</param>
        /// <param name="children">子要素</param>
        public ParameterSignature(string paramName, SignatureType type, params ParameterSignature[] children)
            : this(paramName, type, Enum.GetName(typeof(SignatureType), type), children.ToList()) {
        }
        /// <summary>
        /// 複数の子要素を持つ型を作成するコンストラクタ
        /// </summary>
        /// <param name="paramName">変数名</param>
        /// <param name="type">型の種類</param>
        /// <param name="children">子要素</param>
        public ParameterSignature(string paramName, SignatureType type, IList<ParameterSignature> children)
            : this(paramName, type, Enum.GetName(typeof(SignatureType), type), children) {
        }
        /// <summary>
        /// 子要素を持たない特定の型を作成するコンストラクタ
        /// </summary>
        /// <param name="paramName">変数名</param>
        /// <param name="terminalClass">型名</param>
        public ParameterSignature(string paramName, string terminalClass)
            : this(paramName, SignatureType.TerminalClass, terminalClass, null) {
        }
        /// <summary>
        /// 内部コンストラクタ
        /// </summary>
        /// <param name="paramName">変数名</param>
        /// <param name="type">型の種類</param>
        /// <param name="identifier">型名</param>
        /// <param name="children">子要素</param>
        private ParameterSignature(string paramName, SignatureType type, string identifier, IList<ParameterSignature> children) {
            _type = type;
            _children = (children ?? new List<ParameterSignature>()).ToList();
            _identifier = identifier;
            _paramName = paramName;
        }
        /// <summary>
        /// 全体の型名を作成して返します
        /// </summary>
        /// <returns></returns>
        public string GetTypeName() {
            switch(_type) {
            case SignatureType.TerminalClass:
                return _identifier;
            case SignatureType.Array:
                return string.Format("{0}[]", _children.First().GetTypeName());
            case SignatureType.FixedList:
                if(_children.Count == 0)
                    return "";
                if(_children.Count == 1)
                    return _children.First().GetTypeName();
                return string.Format("{0}<{1}>", _identifier, _children.Select(c => c.GetTypeName()).Aggregate((a, b) => a + ", " + b));
            case SignatureType.Optional:
                return string.Format("{0}<{1}>", _identifier, _children.First().GetTypeName());
            case SignatureType.Selection:
                if(_children.Count == 0)
                    return "";
                if(_children.Count == 1)
                    return _children.First().ToString();
                return string.Format("{0}<{1}>", _identifier, _children.Select(c => c.GetTypeName()).Aggregate((a, b) => a + ", " + b));
            default:
                throw new NotSupportedException();
            }
        }
        public string GetMangledName() {
            string[] parts = this.ParamName.Split('_');
            if(parts.Length == 0)
                return "";
            return parts.Select(p => p.Substring(0, 1).ToUpper() + p.Substring(1)).Aggregate((a, b) => a + b);
        }
    }
    /// <summary>
    /// 型が同じであるかを返す比較器クラス
    /// </summary>
    class ParameterSignatureComparer : IEqualityComparer<ParameterSignature> {
        #region IEqualityComparer<ClassNameSignature> メンバ

        public bool Equals(ParameterSignature x, ParameterSignature y) {
            if(x.Type != y.Type)
                return false;
            if(x.Identifier != y.Identifier)
                return false;
            if(x.Children.Count != y.Children.Count)
                return false;
            return CollectionEx.Zip(x.Children, y.Children, (a, b) => this.Equals(a, b)).All(eq => eq);
        }

        public int GetHashCode(ParameterSignature obj) {
            int ret = 0;
            ret ^= obj.Type.GetHashCode();
            ret ^= obj.Identifier.GetHashCode();
            ret ^= obj.Children.Count.GetHashCode();
            return ret;
        }

        #endregion
    }
    /// <summary>
    /// メソッドの全パラメータの型のリストを保持するクラス
    /// </summary>
    class MethodSignature {
        readonly List<string> _ebnfList = new List<string>();
        /// <summary>
        /// パラメータの元となったEBNF文字列のリストを返します．
        /// </summary>
        public IList<string> EbnfList { get { return new ReadOnlyCollection<string>(_ebnfList); } }
        /// <summary>
        /// パラメータ情報のリストを返します
        /// </summary>
        public readonly List<ParameterSignature> Parameters = new List<ParameterSignature>();
        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        public MethodSignature() { }
        /// <summary>
        /// パラメータ情報のリストを指定するコンストラクタ
        /// </summary>
        /// <param name="collection"></param>
        public MethodSignature(IEnumerable<ParameterSignature> collection) {
            this.Parameters.AddRange(collection);
        }
        /// <summary>
        /// メソッド名とパラメータ情報のリストを指定するコンストラクタ
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="collection"></param>
        public MethodSignature(string methodName, IEnumerable<ParameterSignature> collection)
            : this(collection) {
            this.MethodName = methodName;
        }
        /// <summary>
        /// 一意に特定できるような識別子名をパラメータリストから作成します．
        /// </summary>
        /// <returns></returns>
        public string GetMangledName() {
            if(this.Parameters.Count == 0)
                return "";
            return this.Parameters.Select(p => p.GetMangledName()).Aggregate((a, b) => a + "_" + b);
        }
        /// <summary>
        /// このメソッドの名前を取得または設定します．
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// パラメータの元となったEBNFの文字列をリストに追加します
        /// </summary>
        /// <param name="ruleString"></param>
        public void AddEbnfString(string ruleString) {
            _ebnfList.Add(ruleString);
        }
        public static string GetParameterString(IEnumerable<ParameterSignature> parameters) {
            StringBuilder ret = new StringBuilder();
            foreach(ParameterSignature param in parameters) {
                if(ret.Length != 0) {
                    ret.Append(", ");
                }
                ret.Append(param.GetTypeName());
                ret.Append(" ");
                ret.Append(param.ParamName);
            }
            return ret.ToString();
        }
    }
    /// <summary>
    /// メソッドの全パラメータの型が同じであるかを返すクラス
    /// </summary>
    class MethodSignatureParametersComparer : IEqualityComparer<MethodSignature> {
        ParameterSignatureComparer _sigComp = new ParameterSignatureComparer();
        #region IEqualityComparer<IList<ClassNameSignature>> メンバ

        public bool Equals(MethodSignature x, MethodSignature y) {
            if(x.Parameters.Count != y.Parameters.Count)
                return false;
            return CollectionEx.Zip(x.Parameters, y.Parameters, (a, b) => _sigComp.Equals(a, b)).All(eq => eq);
        }

        public int GetHashCode(MethodSignature obj) {
            int ret = 0;
            foreach(ParameterSignature sig in obj.Parameters) {
                ret += _sigComp.GetHashCode(sig);
            }
            return ret;
        }

        #endregion
    }
    /// <summary>
    /// スクリプトのパーサを生成するクラス
    /// </summary>
    class ScriptParserGenerator {
        #region Settings
        /// <summary>
        /// 設定情報
        /// </summary>
        ScriptParserGeneratorSettings _settings;
        /// <summary>
        /// 無効な設定とその値
        /// </summary>
        List<KeyValuePair<string, string>> _invalidSettings;
        /// <summary>
        /// 設定情報を保持するオブジェクトを取得します．
        /// </summary>
        public ScriptParserGeneratorSettings Settings { get { return _settings; } }
        /// <summary>
        /// 無効な設定値を保持する配列を返します．
        /// </summary>
        public KeyValuePair<string, string>[] InvalidSettings { get { return _invalidSettings.ToArray(); } }
        /// <summary>
        /// 設定の変数名から対応するフィールドを返すための辞書型
        /// </summary>
        Dictionary<string, FieldInfo> _settingNameToField;
        /// <summary>
        /// ひとつの設定を読み込みます
        /// </summary>
        /// <param name="comment">コメントデータ</param>
        void readSetting(GeneratorLexElement comment) {
            string[] commentParts = ParseComment(comment);
            if(commentParts.Length > 0) {
                // ひとつの*で囲まれた文字列は設定用の文字列
                string name = commentParts[0];
                if(name.StartsWith("*") && name.EndsWith("*") && !name.StartsWith("**") && !name.EndsWith("**")) {
                    name = name.Substring(1, name.Length - 2);
                    name = normalizeSettingsName(name);
                    FieldInfo field;
                    if(!_settingNameToField.TryGetValue(name, out field)) {
                        _invalidSettings.Add(new KeyValuePair<string, string>(comment.Text, "Invalid Key"));
                    } else {
                        if(field.FieldType == typeof(string)) {
                            string value = "";
                            if(commentParts.Length >= 2)
                                value = commentParts[1];
                            field.SetValue(_settings, value);
                        } else if(field.FieldType == typeof(ClassOrStruct) || field.FieldType == typeof(Accessibility) || field.FieldType == typeof(CaseConversion)) {
                            if(commentParts.Length < 2) {
                                _invalidSettings.Add(new KeyValuePair<string, string>(comment.Text, "Argument Missing"));
                                return;
                            }
                            try {
                                field.SetValue(_settings, Enum.Parse(field.FieldType, commentParts[1], true));
                            } catch(ArgumentException ex) {
                                _invalidSettings.Add(new KeyValuePair<string, string>(comment.Text, ex.Message));
                            }
                        } else if(field.FieldType == typeof(List<string>)) {
                            string value = "";
                            if(commentParts.Length >= 2)
                                value = commentParts[1];
                            List<string> list = (List<string>)field.GetValue(_settings);
                            list.Add(value);
                        } else if(field.FieldType == typeof(Dictionary<string, string>)) {
                            string value = "";
                            if(commentParts.Length < 2) {
                                _invalidSettings.Add(new KeyValuePair<string, string>(comment.Text, "Argument Missing"));
                                return;
                            }
                            string key = commentParts[1];
                            if(commentParts.Length >= 3)
                                value = commentParts[2];
                            Dictionary<string, string> dict = (Dictionary<string, string>)field.GetValue(_settings);
                            dict[key] = value;
                        } else {
                            Debug.Assert(false);
                            _invalidSettings.Add(new KeyValuePair<string, string>(comment.Text, "Unexpected Data Type"));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 設定の変数名を正規化します
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static string normalizeSettingsName(string name) {
            name = name.Replace("_", "");
            name = name.ToLower();
            return name;
        }
        /// <summary>
        /// コメント内部文字列を空白で区切って返します
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public static string[] ParseComment(GeneratorLexElement comment) {
            string text = comment.Text;
            if(text.StartsWith("(*")) {
                text = text.Substring(2);
            }
            if(text.EndsWith("*)")) {
                text = text.Substring(0, text.Length - 2);
            }
            return text.Split(new char[] { ' ', '\t', '\f', '\b', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        bool _followingTerminalsInitialized = false;
        public void InitializeFollowingTerminals() {
            if(_followingTerminalsInitialized) {
                return;
            }
            foreach(DefinitionElement def in _source.Defs) {
                def.SetupFollowingTerminals(this);
            }
            _followingTerminalsInitialized = true;
        }

        #endregion
        /// <summary>
        /// ソース要素
        /// </summary>
        SourceElement _source;
        /// <summary>
        /// ソースの構文解析結果の要素を取得します．
        /// </summary>
        public SourceElement Source { get { return _source; } }
        /// <summary>
        /// 非終端記号文字列から対応する定義要素を返す辞書型
        /// </summary>
        Dictionary<string, DefinitionElement> _nonterminalToDefinition;
        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        public ScriptParserGenerator() {
            FieldInfo[] settingFields = ScriptParserGeneratorSettings.GetFields();
            _settingNameToField = settingFields.ToDictionary(f => normalizeSettingsName(f.Name));
        }
        /// <summary>
        /// 字句要素列を読み込むコンストラクタ
        /// </summary>
        /// <param name="lexes">字句要素列</param>
        public ScriptParserGenerator(IEnumerable<GeneratorLexElement> lexes)
            : this() {
            LoadFromLexes(lexes);
        }
        /// <summary>
        /// 字句要素列を読み込んで構文解析を行います
        /// </summary>
        /// <param name="lexes">字句要素列</param>
        public void LoadFromLexes(IEnumerable<GeneratorLexElement> lexes) {
            _invalidSettings = new List<KeyValuePair<string, string>>();
            _settings = new ScriptParserGeneratorSettings();

            ScriptParserParser parser = new ScriptParserParser();
            _source = parser.StartParse(lexes);
            foreach(GeneratorLexElement comment in _source.Comments) {
                readSetting(comment);
            }
        }

        /// <summary>
        /// 非終端記号文字列から定義要素を返します
        /// </summary>
        /// <param name="nonterminal">非終端記号</param>
        /// <returns></returns>
        public DefinitionElement LookUpDefinition(string nonterminal) {
            DefinitionElement def;
            if(_nonterminalToDefinition == null) {
                _nonterminalToDefinition = this.Source.Defs.ToDictionary(d => d.DefinitionName);
            }
            if(!_nonterminalToDefinition.TryGetValue(nonterminal, out def)) {
                throw new ArgumentException(string.Format("'{0}' is not Defined", nonterminal), "nonterminal");
            }
            return def;
        }
        /// <summary>
        /// 大文字・小文字変換を行います
        /// </summary>
        /// <param name="text">変換元の文字列</param>
        /// <param name="convertsion">変換の種類</param>
        /// <returns></returns>
        public static string ConvertCase(string text, CaseConversion convertsion) {
            switch(convertsion) {
            case CaseConversion.Capitalized:
                string[] arr = text.Split('_');
                text = arr.Select(p => p.Length >= 1 ? (p.Substring(0, 1).ToUpper() + p.Substring(1)) : "").Aggregate((a, b) => a + b);
                break;
            case CaseConversion.Lower:
                text = text.ToLower();
                break;
            case CaseConversion.Upper:
                text = text.ToUpper();
                break;
            case CaseConversion.None:
                break;
            }
            return text;
        }
        /// <summary>
        /// 終端記号のテキストから終端記号名を返します
        /// </summary>
        /// <param name="terminal">終端記号</param>
        /// <returns></returns>
        public string GetTerminalName(string terminal) {
            string newString;
            if(_settings.Terminal.TryGetValue(terminal, out newString)) {
                terminal = newString;
            }
            return ConvertCase(terminal, _settings.TerminalCaseConversion);
        }
        /// <summary>
        /// 終端記号のテキストから対応する列挙型のメンバを返します．
        /// </summary>
        /// <param name="terminal">終端記号</param>
        /// <returns></returns>
        public string GetTerminalEnum(string terminal) {
            terminal = this.GetTerminalName(terminal);
            return string.Format("{0}.{1}", _settings.InputEnum, terminal);
        }
        /// <summary>
        /// 非終端記号の文字列から構文解析結果要素のクラス名を返します
        /// </summary>
        /// <param name="nonterminal">非終端記号</param>
        /// <returns></returns>
        public string GetReturnClassIdentifier(string nonterminal) {
            string tmp;
            if(_settings.Returns.TryGetValue(nonterminal, out tmp)) {
                nonterminal = tmp;
            }
            nonterminal = ConvertCase(nonterminal, _settings.NonterminalCaseConversion);
            return string.Format("{0}{1}{2}", _settings.ReturnClassPrefix, nonterminal, _settings.ReturnClassSuffix);
        }
        /// <summary>
        /// 非終端記号の文字列から構文解析の結果から結果要素を作成するメソッド名を返します．
        /// </summary>
        /// <param name="nonterminal">非終端記号</param>
        /// <returns></returns>
        public string GetReturnMethodIdentifier(string nonterminal) {
            nonterminal = ConvertCase(nonterminal, _settings.NonterminalCaseConversion);
            return string.Format("{0}{1}{2}", _settings.ReturnMethodPrefix, nonterminal, _settings.ReturnMethodSuffix);
        }
        /// <summary>
        /// 非終端記号の文字列から構文解析メソッド名を返します．
        /// </summary>
        /// <param name="nonterminal">非終端記号</param>
        /// <returns></returns>
        public string GetParseMethodIdentifier(string nonterminal) {
            nonterminal = ConvertCase(nonterminal, _settings.NonterminalCaseConversion);
            return string.Format("{0}{1}{2}", _settings.ParseMethodPrefix, nonterminal, _settings.ParseMethodSuffix);
        }
        /// <summary>
        /// コード生成時の変数番号の初期値を設定します
        /// </summary>
        /// <param name="init"></param>
        public void InitializeVariableIndex(int init) {
            _variableIndex = init;
        }
        int _variableIndex = 1;
        /// <summary>
        /// コード生成時のインデント文字列を取得
        /// </summary>
        string _indent {
            get {
                StringBuilder ret = new StringBuilder();
                for(int i = 0; i < _indentCount; i++) { ret.Append("  "); }
                return ret.ToString();
            }
        }
        int _indentCount = 0;
        /// <summary>
        /// コード生成時のインデントの深さを取得または設定します．
        /// </summary>
        public int IndentCount {
            get { return _indentCount; }
            set { _indentCount = value; }
        }
        /// <summary>
        /// 一時変数名を作成して返します．
        /// </summary>
        /// <returns></returns>
        public string GetTemporaryVariable() {
            return string.Format("var{0}", _variableIndex++);
        }
        /// <summary>
        /// 変数宣言のコードを返します
        /// </summary>
        /// <param name="type">変数の型</param>
        /// <param name="variableName">作成された変数名</param>
        /// <returns></returns>
        public string GetCodeOfDeclare(string type, out string variableName) {
            variableName = this.GetTemporaryVariable();
            return _indent + string.Format("{0} {1};", type, variableName);
        }
        /// <summary>
        /// デフォルト値で初期化する変数宣言のコードを返します
        /// </summary>
        /// <param name="type">変数の型</param>
        /// <param name="variableName">作成された変数名</param>
        /// <returns></returns>
        public string GetCodeOfDeclareDefault(string type, out string variableName) {
            variableName = this.GetTemporaryVariable();
            return _indent + string.Format("{0} {1} = default({0});", type, variableName);
        }
        /// <summary>
        /// newで初期化する変数宣言のコードを返します
        /// </summary>
        /// <param name="type">変数の型</param>
        /// <param name="arguments">newの引数列</param>
        /// <param name="variableName">作成された変数名</param>
        /// <returns></returns>
        public string GetCodeOfDeclareNew(string type, IList<string> arguments, out string variableName) {
            if(arguments == null || arguments.Count == 0)
                return this.GetCodeOfDeclareNew(type, out variableName);
            variableName = this.GetTemporaryVariable();
            return _indent + string.Format("{0} {1} = new {0}({2});", type, variableName, arguments.Aggregate((a, b) => a + ", " + b));
        }
        /// <summary>
        /// 引数なしのnewで初期化する変数のコードを返します
        /// </summary>
        /// <param name="type">変数の型</param>
        /// <param name="variableName">作成された変数名</param>
        /// <returns></returns>
        public string GetCodeOfDeclareNew(string type, out string variableName) {
            variableName = this.GetTemporaryVariable();
            return _indent + string.Format("{0} {1} = new {0}();", type, variableName);
        }
        /// <summary>
        /// 代入のコードを返します
        /// </summary>
        /// <param name="varName">代入先の変数名</param>
        /// <param name="rightValue">代入される値</param>
        /// <returns></returns>
        public string GetCodeOfSubstitution(string varName, string rightValue) {
            return _indent + string.Format("{0} = {1};", varName, rightValue);
        }
        /// <summary>
        /// 次の字句要素を先読みするコードを返します
        /// </summary>
        /// <param name="returnValueName">結果が代入される変数名</param>
        /// <returns></returns>
        public string GetCodeOfPeek(out string returnValueName) {
            returnValueName = this.GetTemporaryVariable();
            return _indent + string.Format("Optional<{0}> {1} = _Reader.Peek();", this.Settings.InputClass, returnValueName);
        }
        /// <summary>
        /// ブロック開始のコードを返します
        /// </summary>
        /// <returns></returns>
        public string GetCodeOfOpenBlock() {
            string ret = _indent + "{";
            _indentCount++;
            return ret;
        }
        /// <summary>
        /// ブロック終了のコードを返します
        /// </summary>
        /// <returns></returns>
        public string GetCodeOfCloseBlock() {
            _indentCount--;
            return _indent + "}";
        }
        /// <summary>
        /// 単体のelseのコードを返します
        /// </summary>
        /// <returns></returns>
        public string GetCodeOfSingleElse() {
            return _indent + "else ";
        }
        /// <summary>
        /// ブロック付きのelseのコードを返します
        /// </summary>
        /// <returns></returns>
        public string GetCodeOfBlockElse() {
            return _indent + "} else {";
        }
        /// <summary>
        /// ブロック付きのifのコードを返します
        /// </summary>
        /// <param name="condition">条件式</param>
        /// <returns></returns>
        public string GetCodeOfIf(string condition) {
            string ret = _indent + string.Format("if({0}) {{", condition);
            _indentCount++;
            return ret;
        }
        /// <summary>
        /// ブロック付きのwhileのコードを返します
        /// </summary>
        /// <param name="condition">条件式</param>
        /// <returns></returns>
        public string GetCodeOfWhile(string condition) {
            string ret = _indent + string.Format("while({0}) {{", condition);
            _indentCount++;
            return ret;
        }
        /// <summary>
        /// ブロック付きのforのコードを返します
        /// </summary>
        /// <param name="init">初期化式</param>
        /// <param name="condition">条件式</param>
        /// <param name="next">継続式</param>
        /// <returns></returns>
        public string GetCodeOfFor(string init, string condition, string next) {
            string ret = _indent + string.Format("for({0}; {1}; {2}) {{", init, condition, next);
            _indentCount++;
            return ret;
        }
        /// <summary>
        /// メソッドの呼び出しを行うコードを返します
        /// </summary>
        /// <param name="left">呼び出すオブジェクト</param>
        /// <param name="method">メソッド</param>
        /// <param name="arguments">引数列</param>
        /// <returns></returns>
        public string GetCodeOfInvoke(string left, string method, params string[] arguments) {
            string arg = "";
            if(arguments != null && arguments.Length > 0) {
                arg = arguments.Aggregate((a, b) => a + ", " + b);
            }
            return _indent + string.Format("{0}.{1}({2});", left, method, arg);
        }
        /// <summary>
        /// メソッドの呼び出しを行い結果を変数に代入するコードを返します
        /// </summary>
        /// <param name="varName">代入先の変数名</param>
        /// <param name="left">呼び出すオブジェクト</param>
        /// <param name="method">メソッド</param>
        /// <param name="arguments">引数列</param>
        /// <returns></returns>
        public string GetCodeOfInvokeSubstitute(string varName, string left, string method, params string[] arguments) {
            string arg = "";
            if(arguments != null && arguments.Length > 0) {
                arg = arguments.Aggregate((a, b) => a + ", " + b);
            }
            return _indent + string.Format("{0} = {1}.{2}({3});", varName, left, method, arg);
        }
        /// <summary>
        /// 字句要素の先読み結果が特定の値である限り継続するforのコードを返します
        /// </summary>
        /// <param name="terminalTypes">継続される列挙型の値</param>
        /// <param name="returnValueName">先読み結果が保持される変数名</param>
        /// <returns></returns>
        public string GetCodeOfForPeekLexisIn(IEnumerable<string> terminalTypes, out string returnValueName) {
            returnValueName = this.GetTemporaryVariable();
            string init = string.Format("Optional<{0}> {1} = _Reader.Peek()", this.Settings.InputClass, returnValueName);
            string condition = this.GetCodeOfConditionOptionalLexisIn(returnValueName, terminalTypes);
            string next = string.Format("{0} = _Reader.Peek()", returnValueName);
            return this.GetCodeOfFor(init, condition, next);
        }
        /// <summary>
        /// 指定された字句要素型の変数の型が特定の値であるかを返すコードを返します
        /// </summary>
        /// <param name="lexisVar">字句要素型である変数名</param>
        /// <param name="terminalTypes">trueを返す列挙型の値</param>
        /// <returns></returns>
        public string GetCodeOfConditionLexisIn(string lexisVar, IEnumerable<string> terminalTypes) {
            IEnumerable<string> enumTypes = terminalTypes.Where(t => t != null).Select(t => this.GetTerminalEnum(t));
            IList<string> equations = enumTypes.Select(t => lexisVar + ".Type == " + t).ToList();
            string condition = "true";
            if(equations.Count >= 1) {
                condition = equations.Aggregate((a, b) => a + " || " + b);
            }
            return condition;
        }
        /// <summary>
        /// 指定されたOptional付き字句要素型の変数の型が特定の値であるかを返すコードを返します
        /// </summary>
        /// <param name="lexisVar">Optional付き字句要素型である変数名</param>
        /// <param name="terminalTypes">trueを返す列挙型</param>
        /// <returns></returns>
        public string GetCodeOfConditionOptionalLexisIn(string lexisVar, IEnumerable<string> terminalTypes) {
            IEnumerable<string> enumTypes = terminalTypes.Where(t => t != null).Select(t => this.GetTerminalEnum(t));
            IList<string> equations = enumTypes.Select(t => lexisVar + ".Value.Type == " + t).ToList();
            string condition = string.Format("{0}.HasValue", lexisVar);
            if(equations.Count >= 1) {
                condition += " && ( " + equations.Aggregate((a, b) => a + " || " + b) + ")";
            }
            return condition;
        }
        /// <summary>
        /// 指定された字句要素型の変数の型が特定の値であるかを条件とするifのコードを返します
        /// </summary>
        /// <param name="lexisVar">字句要素型である変数名</param>
        /// <param name="terminalTypes">trueとなる列挙型</param>
        /// <returns></returns>
        public string GetCodeOfIfLexisIn(string lexisVar, IEnumerable<string> terminalTypes) {
            string condition = this.GetCodeOfConditionLexisIn(lexisVar, terminalTypes);
            return GetCodeOfIf(condition);
        }
        /// <summary>
        /// 指定されたOptional付き字句要素型の変数の型が特定の値であるかを条件とするifのコードを返します
        /// </summary>
        /// <param name="lexisVar">Optional付き字句要素型である変数名</param>
        /// <param name="terminalTypes">trueとなる列挙型</param>
        /// <returns></returns>
        public string GetCodeOfIfOptionalLexisIn(string lexisVar, IEnumerable<string> terminalTypes) {
            string condition = this.GetCodeOfConditionOptionalLexisIn(lexisVar, terminalTypes);
            return GetCodeOfIf(condition);
        }
        /// <summary>
        /// 字句要素を先読みして特定の型でなければ例外を投げるコードを返します．
        /// </summary>
        /// <param name="callerDef">呼び出し元の定義の名前</param>
        /// <param name="validTerminals">有効な列挙型の値</param>
        /// <param name="returnValueName">先読み結果を保持する変数名</param>
        /// <returns></returns>
        public string GetCodeOfPeekOrThrow(string callerDef, IEnumerable<string> validTerminals, out string returnValueName) {
            return _getCodeOfXOrThrow("Peek", callerDef, validTerminals, out returnValueName);
        }
        /// <summary>
        /// 字句要素を読み込んで特定の型でなければ例外を投げるコードを返します．
        /// </summary>
        /// <param name="callerDef">呼び出し元の定義の名前</param>
        /// <param name="validTerminals">有効な列挙型の値</param>
        /// <param name="returnValueName">読み込み結果を保持する変数名</param>
        /// <returns></returns>
        public string GetCodeOfReadOrThrow(string callerDef, IEnumerable<string> validTerminals, out string returnValueName) {
            return _getCodeOfXOrThrow("Read", callerDef, validTerminals, out returnValueName);
        }
        private string _getCodeOfXOrThrow(string X, string callerDef, IEnumerable<string> validTerminals, out string returnValueName) {
            validTerminals = validTerminals.Where(t => t != null).ToList();
            returnValueName = this.GetTemporaryVariable();
            string e_callerDef = EscapeString(callerDef);
            string e_valid = validTerminals.Select(t => this.GetTerminalEnum(t)).Aggregate((a, b) => a + ", " + b);
            return _indent + string.Format("{0} {1} = _Reader.{2}OrThrow({3}, {4});", this.Settings.InputClass, returnValueName, X, e_callerDef, e_valid);
        }
        public string GetCodeOfThrowError(string callerDef, IEnumerable<string> validTerminals) {
            validTerminals = validTerminals.Where(t => t != null).ToList();
            string e_callerDef = EscapeString(callerDef);
            string e_valid = validTerminals.Select(t => this.GetTerminalEnum(t)).Aggregate((a, b) => a + ", " + b);
            return _indent + string.Format("throw _Reader.Error({0}, {1});", e_callerDef, e_valid);
        }
        /// <summary>
        /// 字句要素を先読みして特定の型でなければ例外を投げ，そうでなければ先読み結果を破棄するコードを返します．
        /// </summary>
        /// <param name="callerDef">呼び出し元の定義の名前</param>
        /// <param name="validTerminals">有効な列挙型の値</param>
        /// <returns></returns>
        public string GetCodeOfPeekOrThrow(string callerDef, IEnumerable<string> validTerminals) {
            return _getCodeOfXOrThrow("Peek", callerDef, validTerminals);
        }
        /// <summary>
        /// 字句要素を読み込んで特定の型でなければ例外を投げ，そうでなければ先読み結果を破棄するコードを返します．
        /// </summary>
        /// <param name="callerDef">呼び出し元の定義の名前</param>
        /// <param name="validTerminals">有効な列挙型の値</param>
        /// <returns></returns>
        public string GetCodeOfReadOrThrow(string callerDef, IEnumerable<string> validTerminals) {
            return _getCodeOfXOrThrow("Read", callerDef, validTerminals);
        }
        private string _getCodeOfXOrThrow(string X, string callerDef, IEnumerable<string> validTerminals) {
            validTerminals = validTerminals.Where(t => t != null).ToList();
            string e_callerDef = EscapeString(callerDef);
            string e_valid = validTerminals.Select(t => this.GetTerminalEnum(t)).Aggregate((a, b) => a + ", " + b);
            return _indent + string.Format("_Reader.{0}OrThrow({1}, {2});", X, e_callerDef, e_valid);
        }
        /// <summary>
        /// 指定された非終端記号に対応する構文解析用メソッドを呼び出すコードを返します
        /// </summary>
        /// <param name="nonterminal">非終端記号文字列</param>
        /// <param name="returnValueName">構文解析結果を保持する先の変数名</param>
        /// <returns></returns>
        public string GetCodeOfParseNonterminal(string nonterminal, out string returnValueName) {
            returnValueName = this.GetTemporaryVariable();
            return _indent + string.Format("{0} {1} = this.{2}();", this.GetReturnClassIdentifier(nonterminal), returnValueName, GetParseMethodIdentifier(nonterminal));
        }
        /// <summary>
        /// 例外をnewして投げるコードを返します
        /// </summary>
        /// <param name="exception">例外クラス名</param>
        /// <param name="arguments">newの引数</param>
        /// <returns></returns>
        public string GetCodeOfThrowNew(string exception, params string[] arguments) {
            string arg = "";
            if(arguments != null && arguments.Length > 0) {
                arg = arguments.Aggregate((a, b) => a + ", " + b);
            }
            return _indent + string.Format("throw new {0}({1});", exception, arg);
        }
        /// <summary>
        /// メソッドを呼び出してreturnするコードを返します
        /// </summary>
        /// <param name="method">呼び出されるメソッド</param>
        /// <param name="arguments">引数列</param>
        /// <returns></returns>
        public string GetCodeOfReturnMethod(string method, IList<string> arguments) {
            string arg = "";
            if(arguments != null && arguments.Count > 0) {
                arg = arguments.Aggregate((a, b) => a + ", " + b);
            }
            return _indent + string.Format("return this.{0}({1});", method, arg);
        }
        public string GetCodeOfComment(string comment) {
            StringBuilder ret = new StringBuilder();
            using(StringReader reader = new StringReader(comment)) {
                string line;
                while((line = reader.ReadLine()) != null) {
                    if(ret.Length != 0) {
                        ret.AppendLine();
                    }
                    ret.Append(_indent + "// " + line);
                }
            }
            return ret.ToString();
        }
        /// <summary>
        /// 文字列をコード用にエスケープします
        /// </summary>
        /// <param name="aString">文字列</param>
        /// <returns></returns>
        public static string EscapeString(string aString) {
            return string.Format("\"{0}\"", aString.Replace("\\", "\\\\").Replace("\"", "\\\""));
        }
        /// <summary>
        /// 警告メッセージを出力します
        /// </summary>
        /// <param name="context">警告箇所を表す文字列</param>
        /// <param name="message">警告メッセージ</param>
        public void Warn(string context, string message) {
            _warnString.WriteLine(string.Format("{0} - {1}", context, message));
        }
        StringWriter _warnString = new StringWriter();
        /// <summary>
        /// 記録された警告メッセージを取得します．
        /// </summary>
        /// <returns></returns>
        public string ReadWarningMessage() {
            string ret = _warnString.ToString();
            _warnString = new StringWriter();
            return ret;
        }
        /// <summary>
        /// アクセスレベルの値から対応するコードを返します
        /// </summary>
        /// <param name="accessibility">アクセスレベル</param>
        /// <returns></returns>
        public static string GetAccessibilityString(Accessibility accessibility) {
            switch(accessibility) {
            case Accessibility.Internal:
                return "internal";
            case Accessibility.None:
                return "";
            case Accessibility.Private:
                return "private";
            case Accessibility.Protected:
                return "protected";
            case Accessibility.ProtectedInteral:
                return "protected internal";
            case Accessibility.Public:
                return "public";
            default:
                throw new NotSupportedException();
            }
        }
        /// <summary>
        /// 型の種類の値から対応するコードを返します．
        /// </summary>
        /// <param name="classOrStruct">型の種類</param>
        /// <returns></returns>
        public static string GetClassTypeString(ClassOrStruct classOrStruct) {
            switch(classOrStruct) {
            case ClassOrStruct.Class:
                return "class";
            case ClassOrStruct.Struct:
                return "struct";
            default:
                throw new NotSupportedException();
            }
        }

        string getReturnMethodXmlComment(DefinitionElement def, MethodSignature method) {
            StringBuilder str = new StringBuilder();
            string rule = ";";
            if(method.EbnfList.Count > 0) {
                rule = method.EbnfList.Aggregate((a, b) => a + ", " + b) + " ;";
            }
            str.AppendLine("/// <summary>");
            str.AppendLine("/// " + escapeXmlString(def.DefinitionName + " = " + rule));
            str.AppendLine("/// </summary>");
            for(int i = 0; i < method.EbnfList.Count && i < method.Parameters.Count; i++) {
                string name = escapeXmlAttributeValue(method.Parameters[i].ParamName);
                string value = escapeXmlString(method.EbnfList[i]);
                str.AppendFormat("/// <param name=\"{0}\">{1}</param>", name, value);
                str.AppendLine();
            }
            return str.ToString();
        }

        /// <summary>
        /// abstract付きの結果要素作成メソッドのコードを返します
        /// </summary>
        /// <returns></returns>
        public IList<string> GetAbstractReturnMethods() {
            List<string> ret = new List<string>();
            foreach(DefinitionElement def in _source.Defs) {
                foreach(MethodSignature method in def.GetReturnParameterSignatures(this)) {
                    StringBuilder str = new StringBuilder();
                    str.Append(getReturnMethodXmlComment(def, method));
                    str.Append(GetAccessibilityString(_settings.ReturnMethodAccessibility));
                    str.Append(" abstract ");
                    str.Append(this.GetReturnClassIdentifier(def.DefinitionName));
                    str.Append(" ");
                    str.Append(method.MethodName);
                    str.Append("(");
                    str.Append(MethodSignature.GetParameterString(method.Parameters));
                    str.AppendLine(");");
                    ret.Add(str.ToString());
                }
            }
            return ret;
        }
        /// <summary>
        /// NotImplemented付きの結果要素作成メソッドのコードを返します
        /// </summary>
        /// <returns></returns>
        public IList<string> GetNotImplementedReturnMethods() {
            List<string> ret = new List<string>();
            foreach(DefinitionElement def in _source.Defs) {
                foreach(MethodSignature method in def.GetReturnParameterSignatures(this)) {
                    StringBuilder str = new StringBuilder();
                    str.Append(getReturnMethodXmlComment(def, method));
                    str.Append(GetAccessibilityString(_settings.ReturnMethodAccessibility));
                    str.Append(" override ");
                    str.Append(this.GetReturnClassIdentifier(def.DefinitionName));
                    str.Append(" ");
                    str.Append(method.MethodName);
                    str.Append("(");
                    str.Append(MethodSignature.GetParameterString(method.Parameters));
                    str.AppendLine(") {");
                    str.AppendLine("    throw new NotImplementedException();");
                    str.AppendLine("}");
                    ret.Add(str.ToString());
                }
            }
            return ret;
        }
        /// <summary>
        /// 結果要素のスタブのコードを返します
        /// </summary>
        /// <returns></returns>
        public IList<string> GetReturnClassTemplates() {
            List<string> ret = new List<string>();
            HashSet<string> exists = new HashSet<string>();
            string type = GetClassTypeString(_settings.ReturnClassType);
            string access = GetAccessibilityString(_settings.ReturnClassAccessibility);
            foreach(DefinitionElement def in _source.Defs) {
                string className = this.GetReturnClassIdentifier(def.DefinitionName);
                if(!exists.Contains(className)) {
                    ret.Add(string.Format("{0} {1} {2} {{ }}", access, type, className));
                    exists.Add(className);
                }
            }
            return ret;
        }
        /// <summary>
        /// using文を設定から作成して返します．
        /// </summary>
        /// <returns></returns>
        private IList<string> getUsings() {
            HashSet<string> exists = new HashSet<string>();
            List<string> ret = new List<string>();
            foreach(string @using in _settings.Using.Union(new[] { "System", "System.IO", "System.Linq", "System.Text", "System.Collections.Generic" })) {
                if(!exists.Contains(@using)) {
                    ret.Add(string.Format("using {0};", @using));
                    exists.Add(@using);
                }
            }
            return ret;
        }
        /// <summary>
        /// 出力クラスの基底クラスの開始部分のコードを返します
        /// </summary>
        /// <returns></returns>
        public IList<string> GetBaseOutputClassHeader() {
            if(_settings.Start == null)
                throw new InvalidOperationException("構文解析の開始規則が指定されていません");
            List<string> ret = new List<string>();
            ret.Add(string.Format("  {0} abstract class {1}Base {{", GetAccessibilityString(_settings.OutputClassAccessibility), _settings.OutputClass));
            ret.Add(string.Format("  protected {0}Base() {{ }}", _settings.OutputClass));
            ret.Add(string.Format("  public {0} StartParse(IEnumerable<{1}> source) {{ \r\n _Reader = new Reader<{1}, {2}>(source);\r\n return this.{3}();\r\n }}", this.GetReturnClassIdentifier(_settings.Start), _settings.InputClass, _settings.InputEnum, this.GetParseMethodIdentifier(_settings.Start)));
            ret.Add(string.Format("  protected Reader<{0}, {1}> _Reader;", _settings.InputClass, _settings.InputEnum));
            return ret;
        }
        /// <summary>
        /// 出力クラスの開始部分のコードを返します
        /// </summary>
        /// <returns></returns>
        public IList<string> GetOutputClassHeader() {
            if(_settings.Start == null)
                throw new InvalidOperationException("構文解析の開始規則が指定されていません");
            List<string> ret = new List<string>();
            ret.Add(string.Format("  {0} class {1} : {1}Base {{", GetAccessibilityString(_settings.OutputClassAccessibility), _settings.OutputClass));
            ret.Add(string.Format("  public {0}() {{ }}", _settings.OutputClass));
            return ret;
        }
        /// <summary>
        /// 各出力ファイルの先頭部分のコードを返します．
        /// </summary>
        /// <returns></returns>
        public IList<string> GetHeader() {
            List<string> ret = new List<string>();
            ret.Add(string.Format("// Auto generated with {0}", typeof(ScriptParserGenerator)));
            ret.AddRange(getUsings());
            ret.Add("");
            ret.Add(string.Format("namespace {0} {{", _settings.NameSpace));
            return ret;
        }

        /// <summary>
        /// 出力クラスの基底クラスの終了部分のコードを返します
        /// </summary>
        /// <returns></returns>
        public IList<string> GetOutputClassFooter() {
            List<string> ret = new List<string>();
            ret.Add("  }");
            return ret;
        }
        /// <summary>
        /// 各出力ファイルの末尾部分のコードを返します．
        /// </summary>
        /// <returns></returns>
        public IList<string> GetFooter() {
            List<string> ret = new List<string>();
            ret.Add("}");
            return ret;
        }
        /// <summary>
        /// ユーティリティクラスのコードを返します．
        /// </summary>
        /// <returns></returns>
        public IList<string> GetUtility() {
            List<string> ret = new List<string>();
            // 先頭の固定テキストをリソースから読み込む
            using(StringReader reader = new StringReader(Properties.Resources.FixedUtility)) {
                string line;
                while((line = reader.ReadLine()) != null) {
                    ret.Add(line);
                }
            }
            // FixedListとSelectionのジェネリックパラメータの最大数を求める
            int longestFixedList = 0;
            int longestSelection = 0;
            foreach(DefinitionElement def in _source.Defs) {
                foreach(MethodSignature method in def.GetReturnParameterSignatures(this)) {
                    foreach(ParameterSignature sig in method.Parameters) {
                        countLengthOfGenericForUtility(sig, ref longestFixedList, ref longestSelection);
                    }
                }
            }
            // FixedList<T1,T2> , ... , FixedList<T1, T2, ..., Tn> のコード
            for(int i = 2; i <= longestFixedList; i++) {
                // クラス名
                StringBuilder genericParameter = new StringBuilder();
                for(int j = 1; j <= i; j++) {
                    if(j != 1) {
                        genericParameter.Append(", ");
                    }
                    genericParameter.AppendFormat("T{0}", j);
                }
                ret.Add(string.Format("struct FixedList<{0}> {{", genericParameter));
                // メンバ変数
                for(int j = 1; j <= i; j++) {
                    ret.Add(string.Format("  public T{0} Element{0};", j));
                }
                // コンストラクタ
                StringBuilder parameter = new StringBuilder();
                for(int j = 1; j <= i; j++) {
                    if(j != 1) {
                        parameter.Append(", ");
                    }
                    parameter.AppendFormat("T{0} element{0}", j);
                }
                ret.Add(string.Format("  public FixedList({0}) {{", parameter));
                for(int j = 1; j <= i; j++) {
                    ret.Add(string.Format("    this.Element{0} = element{0};", j));
                }
                ret.Add("  }");
                //return string.Format("1: <{0}>, 2: <{1}>, 3: <{2}>", this.Element1, this.Element2, this.Element3);
                // ToString
                ret.Add("  public override string ToString() {");
                StringBuilder toString = new StringBuilder("    return string.Format(\"");
                for(int j = 1; j <= i; j++) {
                    if(j != 1) {
                        toString.Append(", ");
                    }
                    toString.AppendFormat("{0}: <{{{1}}}>", j, j - 1);
                }
                toString.Append("\"");
                for(int j = 1; j <= i; j++) {
                    toString.AppendFormat(", this.Element{0}", j);
                }
                toString.Append(");");
                ret.Add(toString.ToString());
                ret.Add("  }");
                // クラス閉じ
                ret.Add("}");
            }
            // Selection<T1,T2> , ... , Selection<T1, T2, ..., Tn> のコード
            for(int i = 2; i <= longestSelection; i++) {
                // クラス名
                StringBuilder genericParameter = new StringBuilder();
                for(int j = 1; j <= i; j++) {
                    if(j != 1) {
                        genericParameter.Append(", ");
                    }
                    genericParameter.AppendFormat("T{0}", j);
                }
                ret.Add(string.Format("struct Selection<{0}> {{", genericParameter));
                // メンバ変数
                for(int j = 1; j <= i; j++) {
                    ret.Add(string.Format("  public readonly Optional<T{0}> Element{0};", j));
                }
                // コンストラクタ群
                for(int j = 1; j <= i; j++) {
                    // コンストラクタ
                    ret.Add(string.Format("  public Selection(T{0} element) {{", j));
                    // メンバ変数初期化
                    for(int k = 1; k <= i; k++) {
                        if(j == k) {
                            ret.Add(string.Format("    this.Element{0} = element;", k));
                        } else {
                            ret.Add(string.Format("    this.Element{0} = Optional<T{0}>.Empty;", k));
                        }
                    }
                    // コンストラクタ閉じ
                    ret.Add("    }");
                }

                // ToString
                ret.Add("  public override string ToString() {");
                for(int j = 1; j <= i; j++) {
                    ret.Add(string.Format("    if(this.Element{0}.HasValue) {{", j));
                    ret.Add(string.Format("      return string.Format(\"{0}: {{0}}\", this.Element{0}.Value);", j));
                    ret.Add(string.Format("    }}"));
                }
                ret.Add("      return \"null: null\";");
                ret.Add("  }");
                // クラス閉じ
                ret.Add("  }");
            }
            return ret;
        }
        /// <summary>
        /// 必要とされる固定長リストの最大長と選択の最大候補数を取得します．
        /// </summary>
        /// <param name="sig">調査対象のメソッドパラメータの型</param>
        /// <param name="fixedList">固定長リストの最大長の保持先</param>
        /// <param name="selection">選択の最大候補数の保持先</param>
        private void countLengthOfGenericForUtility(ParameterSignature sig, ref int fixedList, ref int selection) {
            switch(sig.Type) {
            case SignatureType.FixedList:
                fixedList = Math.Max(fixedList, sig.Children.Count);
                break;
            case SignatureType.Selection:
                selection = Math.Max(selection, sig.Children.Count);
                break;
            case SignatureType.Array:
            case SignatureType.Optional:
                break;
            case SignatureType.TerminalClass:
                return;
            }
            foreach(ParameterSignature child in sig.Children) {
                countLengthOfGenericForUtility(child, ref fixedList, ref selection);
            }
        }
        /// <summary>
        /// 設定内容を文字列化して返します．
        /// </summary>
        /// <returns></returns>
        public IList<KeyValuePair<string, string>> GetSettingsContent() {
            List<KeyValuePair<string, string>> ret = new List<KeyValuePair<string, string>>();
            FieldInfo[] settingFields = ScriptParserGeneratorSettings.GetFields();
            foreach(FieldInfo field in settingFields) {
                if(field.FieldType == typeof(string)) {
                    string value = (string)field.GetValue(_settings);
                    ret.Add(new KeyValuePair<string, string>(field.Name, value));
                } else if(field.FieldType == typeof(ClassOrStruct) || field.FieldType == typeof(Accessibility) || field.FieldType == typeof(CaseConversion)) {
                    object value = field.GetValue(_settings);
                    ret.Add(new KeyValuePair<string, string>(field.Name, value.ToString()));
                } else if(field.FieldType == typeof(List<string>)) {
                    List<string> value = (List<string>)field.GetValue(_settings);
                    foreach(string v in value) {
                        ret.Add(new KeyValuePair<string, string>(field.Name, v));
                    }
                } else if(field.FieldType == typeof(Dictionary<string, string>)) {
                    Dictionary<string, string> value = (Dictionary<string, string>)field.GetValue(_settings);
                    foreach(var pair in value) {
                        ret.Add(new KeyValuePair<string, string>(field.Name, string.Format("{0} {1}", pair.Key, pair.Value)));
                    }
                } else {
                    Debug.Assert(false);
                }
            }
            return ret;
        }
        /// <summary>
        /// XML内部で使用できない文字をHTMLエンティティに置き換える
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        static string escapeXmlString(string text) {
            StringBuilder ret = new StringBuilder();
            foreach(char c in text) {
                switch(c) {
                case '&':
                    ret.Append("&amp;");
                    break;
                case '<':
                    ret.Append("&lt;");
                    break;
                case '>':
                    ret.Append("&gt;");
                    break;
                default:
                    ret.Append(c);
                    break;
                }
            }
            return ret.ToString();
        }
        /// <summary>
        /// XMLのタグの属性値に使用できない文字をエスケープします．
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        static string escapeXmlAttributeValue(string text) {
            StringBuilder ret = new StringBuilder();
            foreach(char c in text) {
                switch(c) {
                case '&':
                    ret.Append("&amp;");
                    break;
                case '<':
                    ret.Append("&lt;");
                    break;
                case '>':
                    ret.Append("&gt;");
                    break;
                case '"':
                    ret.Append("\\\"");
                    break;
                default:
                    ret.Append(c);
                    break;
                }
            }
            return ret.ToString();
        }

    }
}

