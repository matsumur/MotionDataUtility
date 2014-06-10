using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MotionDataHandler.Script {
    /// <summary>
    /// Paramの型
    /// </summary>
    [Flags()]
    public enum ScriptVariableType {
        None = 0,
        String = 1,
        Number = 2,
        Boolean = 4,
        List = 8,
        RegisteredFunction = 16,
        Function = 32,
    }

    /// <summary>
    /// スクリプト内部変数の基底クラス
    /// </summary>
    public abstract class ScriptVariable {
        public bool IsTypeOf(ScriptVariableType types) {
            return (types & this.Type) != 0;
        }

        protected ScriptVariableType _type;
        /// <summary>
        /// 変数の型
        /// </summary>
        public ScriptVariableType Type { get { return _type; } }
        /// <summary>
        /// 型を指定するコンストラクタ
        /// </summary>
        /// <param name="type">変数の型</param>
        protected ScriptVariable(ScriptVariableType type) { _type = type; }
        /// <summary>
        /// 変数にインデックスを指定して値を設定します
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <returns></returns>
        public abstract bool SetIndexedValue(int index, ScriptVariable value);
        /// <summary>
        /// 変数にインデックスを指定して値を取得します.
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <returns></returns>
        public abstract bool GetIndexedValue(int index, out ScriptVariable value);
        /// <summary>
        /// 変数を数値に変換します．
        /// </summary>
        /// <returns></returns>
        public abstract decimal ToNumber();
        /// <summary>
        /// 変数を整数に変換します．
        /// </summary>
        /// <returns></returns>
        public int ToInteger() {
            int ret = 0;
            decimal number = this.ToNumber();
            try {
                ret = (int)number;
            } catch(ArithmeticException) {
                return 0;
            }
            return ret;
        }
        /// <summary>
        /// 変数を真理値に変換します
        /// </summary>
        /// <returns></returns>
        public abstract bool ToBoolean();
        /// <summary>
        /// 変数をリストに変換します．
        /// </summary>
        /// <returns></returns>
        public abstract IList<ScriptVariable> ToList();
        /// <summary>
        /// 変数を複製します．
        /// </summary>
        /// <returns></returns>
        public abstract ScriptVariable Clone();
        /// <summary>
        /// 変数をスクリプト文字列に変換します
        /// </summary>
        /// <returns></returns>
        public abstract string Serialize();

        public abstract ScriptVariable Invoke(ScriptExecutionEnvironment env, IList<ScriptVariable> args);
        /// <summary>
        /// ListParamの循環参照をnullに置き換えて取り除く．
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static List<ScriptVariable> RemoveParamRecursion(IList<ScriptVariable> args) {
            if(args == null)
                throw new ArgumentNullException("args", "'args' cannot be null");
            return removeParamRecursionAux(args, new Dictionary<ScriptVariable, ScriptVariable>());
        }

        private static List<ScriptVariable> removeParamRecursionAux(IList<ScriptVariable> args, Dictionary<ScriptVariable, ScriptVariable> cloneMap) {
            List<ScriptVariable> ret = new List<ScriptVariable>();
            foreach(var arg in args) {
                ScriptVariable cloned;
                if(arg == null) {
                    ret.Add(null);
                    continue;
                }
                if(cloneMap.TryGetValue(arg, out cloned)) {
                    ret.Add(cloned);
                    continue;
                }
                ListVariable list = arg as ListVariable;
                if(list != null) {
                    ListVariable tmp = new ListVariable();
                    cloneMap[list] = tmp;
                    tmp.AddRange(removeParamRecursionAux(list.Value, cloneMap));
                    ret.Add(tmp);
                } else {
                    cloned = arg.Clone();
                    cloneMap[arg] = cloned;
                    ret.Add(cloned);
                }
            }

            return ret;
        }
    }
    /// <summary>
    /// 文字列を保持するスクリプト内部変数
    /// </summary>
    public class StringVariable : ScriptVariable {
        public readonly string Value = "";

        public StringVariable(string value)
            : base(ScriptVariableType.String) {
            this.Value = value ?? "";
        }
        public override string ToString() {
            return this.Value;
        }

        public override decimal ToNumber() {
            decimal ret;
            if(decimal.TryParse(this.Value, out ret)) {
                return ret;
            }
            return 0;
        }

        public override bool ToBoolean() {
            return this.Value.Length != 0;
        }

        public override IList<ScriptVariable> ToList() {
            return new ScriptVariable[] { this.Clone() }.ToList();
        }

        public override ScriptVariable Clone() {
            return new StringVariable(this.Value);
        }
        public override bool GetIndexedValue(int index, out ScriptVariable value) {
            value = null;
            if(index < 0 || index > this.Value.Length) {
                return false;
            }
            value = new StringVariable(this.Value.Substring(index, 1));
            return true;
        }
        public override bool SetIndexedValue(int index, ScriptVariable value) {
            return false;
        }
        public override string Serialize() {
            return Parse.StringSyntaxElement.EscapeString(this.Value);
        }
        public override ScriptVariable Invoke(ScriptExecutionEnvironment env, IList<ScriptVariable> args) {
            env.Console.Warn("Meaningless Function Call for " + this.GetType().Name);
            return this;
        }
    }
    /// <summary>
    /// 数値を保持するスクリプト内部変数
    /// </summary>
    public class NumberVariable : ScriptVariable {
        public readonly decimal Value;
        public NumberVariable(decimal value)
            : base(ScriptVariableType.Number) {
            this.Value = value;
        }
        public override string ToString() {
            return this.Value.ToString();
        }

        public override decimal ToNumber() {
            return this.Value;
        }

        public override bool ToBoolean() {
            return this.Value != 0M;
        }

        public override IList<ScriptVariable> ToList() {
            return new List<ScriptVariable>(new ScriptVariable[] { this.Clone() });
        }

        public override ScriptVariable Clone() {
            return new NumberVariable(this.Value);
        }


        public override bool GetIndexedValue(int index, out ScriptVariable value) {
            value = null;
            return false;
        }
        public override bool SetIndexedValue(int index, ScriptVariable value) {
            return false;
        }
        public override string Serialize() {
            return this.Value.ToString();
        }
        public override ScriptVariable Invoke(ScriptExecutionEnvironment env, IList<ScriptVariable> args) {
            env.Console.Warn("Meaningless Function Call for " + this.GetType().Name);
            return this;
        }
    }
    /// <summary>
    /// 真理値を保持するスクリプト内部変数
    /// </summary>
    public class BooleanVariable : ScriptVariable {
        public readonly bool Value;
        public BooleanVariable(bool value)
            : base(ScriptVariableType.Boolean) {
            this.Value = value;
        }
        public override string ToString() {
            if(this.Value)
                return "true";
            else
                return "false";
        }

        public override decimal ToNumber() {
            return this.Value ? 1M : 0M;
        }

        public override bool ToBoolean() {
            return this.Value;
        }

        public override IList<ScriptVariable> ToList() {
            return new ScriptVariable[] { this.Clone() }.ToList();
        }

        public override ScriptVariable Clone() {
            return new BooleanVariable(this.Value);
        }

        public override bool GetIndexedValue(int index, out ScriptVariable value) {
            value = null;
            return false;
        }
        public override bool SetIndexedValue(int index, ScriptVariable value) {
            return false;
        }

        public override string Serialize() {
            if(this.Value)
                return "true";
            else
                return "false";
        }
        public override ScriptVariable Invoke(ScriptExecutionEnvironment env, IList<ScriptVariable> args) {
            env.Console.Warn("Meaningless Function Call for " + this.GetType().Name);
            return this;
        }
    }
    /// <summary>
    /// スクリプト内部変数のリストを保持するスクリプト内部変数
    /// </summary>
    public class ListVariable : ScriptVariable {
        private readonly List<ScriptVariable> _value = new List<ScriptVariable>();
        public ReadOnlyCollection<ScriptVariable> Value { get { return new ReadOnlyCollection<ScriptVariable>(_value); } }

        public ListVariable(IEnumerable<ScriptVariable> value)
            : base(ScriptVariableType.List) {
            _value = ScriptVariable.RemoveParamRecursion(value.ToList());
        }
        public ListVariable(IEnumerable<NumberVariable> value)
            : this(value.Select(v => (ScriptVariable)v)) {
        }
        public ListVariable(IEnumerable<StringVariable> value)
            : this(value.Select(v => (ScriptVariable)v)) {
        }
        public ListVariable(IEnumerable<BooleanVariable> value)
            : this(value.Select(v => (ScriptVariable)v)) {
        }
        public ListVariable(IEnumerable<ListVariable> value)
            : this(value.Select(v => (ScriptVariable)v)) {
        }
        public ListVariable(params ScriptVariable[] value)
            : this((IList<ScriptVariable>)value) {
        }

        public override string ToString() {
            return this.Serialize();
        }

        public override decimal ToNumber() {
            return this.Value.Count;
        }

        public override bool ToBoolean() {
            return this.Value.Count > 0;
        }

        public override IList<ScriptVariable> ToList() {
            return this.Value;
        }

        public override ScriptVariable Clone() {
            List<ScriptVariable> ret = new List<ScriptVariable>();
            foreach(var elem in ScriptVariable.RemoveParamRecursion(this.Value)) {
                if(elem == null) {
                    ret.Add(null);
                } else {
                    ret.Add(elem.Clone());
                }
            }
            return new ListVariable(ret);
        }

        public override bool GetIndexedValue(int index, out ScriptVariable value) {
            value = null;
            if(index < 0 || index >= this.Value.Count) {
                return false;
            }
            value = this.Value[index];
            return true;
        }
        public override bool SetIndexedValue(int index, ScriptVariable value) {
            if(index < 0) {
                return false;
            }
            if(index >= _value.Count) {
                while(_value.Count < index) {
                    _value.Add(null);
                }
                _value.Add(value.Clone());
            } else {
                _value[index] = value.Clone();
            }
            return true;
        }


        public ScriptVariable this[string key] {
            get {
                foreach(ScriptVariable variable in _value) {
                    ListVariable list = variable as ListVariable;
                    if(list != null && list.Value.Count >= 2) {
                        if(list.Value[0].IsNull())
                            continue;
                        if(list.Value[0].ToString() == key)
                            return list.Value[1];
                    }
                }
                return null;
            }
            set {
                foreach(ScriptVariable variable in _value) {
                    ListVariable list = variable as ListVariable;
                    if(list != null && list.Value.Count >= 2) {
                        if(list.Value[0].IsNull())
                            continue;
                        if(list.Value[0].ToString() == key) {
                            list.SetIndexedValue(1, value);
                        }
                    }
                }
                _value.Add(new ListVariable(new StringVariable(key), value));
            }
        }

        /// <summary>
        /// リストの末尾にオブジェクトを追加します．
        /// </summary>
        /// <param name="values"></param>
        public void AddRange(IEnumerable<ScriptVariable> values) {
            _value.AddRange(values);
        }

        public override string Serialize() {
            StringBuilder str = new StringBuilder("{ ");
            bool first = true;
            foreach(ScriptVariable item in this.Value) {
                if(first)
                    first = false;
                else
                    str.Append(", ");
                if(item.IsNull()) {
                    str.Append("null");
                } else {
                    str.Append(item.Serialize());
                }
            }
            str.Append(" }");
            return str.ToString();
        }
        public override ScriptVariable Invoke(ScriptExecutionEnvironment env, IList<ScriptVariable> args) {
            env.Console.Warn("Meaningless Function Call for " + this.GetType().Name);
            return this;
        }
    }

    public class RegisteredFunctionVariable : ScriptVariable {
        string _name;
        public RegisteredFunctionVariable(string name)
            : base(ScriptVariableType.RegisteredFunction) {
            _name = name;
        }
        public string Name { get { return _name; } }
        public override bool SetIndexedValue(int index, ScriptVariable value) {
            return false;
        }

        public override bool GetIndexedValue(int index, out ScriptVariable value) {
            value = null;
            return false;
        }

        public override decimal ToNumber() {
            return 0M;
        }

        public override bool ToBoolean() {
            return false;
        }

        public override IList<ScriptVariable> ToList() {
            return new ScriptVariable[] { this };
        }

        public override ScriptVariable Clone() {
            return new RegisteredFunctionVariable(_name);
        }
        public override string ToString() {
            return _name;
        }

        public override string Serialize() {
            return _name;
        }

        public override ScriptVariable Invoke(ScriptExecutionEnvironment env, IList<ScriptVariable> args) {
            return env.Console.CallFunction(_name, args);
        }
    }
    public class FunctionVariable : ScriptVariable {
        public bool IsParams;
        public IList<Parse.IdentifierSyntaxElement> Parameters;
        public Parse.SyntaxElement Body;

        public FunctionVariable(IList<Parse.IdentifierSyntaxElement> parameters, Parse.SyntaxElement body, bool isParams)
            : base(ScriptVariableType.Function) {
            this.Parameters = parameters;
            this.Body = body;
            this.IsParams = isParams;
        }
        public override bool SetIndexedValue(int index, ScriptVariable value) {
            return false;
        }

        public override bool GetIndexedValue(int index, out ScriptVariable value) {
            value = null;
            return false;
        }

        public override decimal ToNumber() {
            return 0M;
        }

        public override bool ToBoolean() {
            return false;
        }

        public override IList<ScriptVariable> ToList() {
            return new ScriptVariable[] { this };
        }


        public override ScriptVariable Clone() {
            return new FunctionVariable(this.Parameters, this.Body, this.IsParams);
        }

        public override string Serialize() {
            System.IO.StringWriter writer = new System.IO.StringWriter();
            writer.Write("Func(");
            int index = 0;
            foreach(var p in this.Parameters) {
                if(index != 0) {
                    writer.Write(", ");
                }
                if(this.IsParams && index == this.Parameters.Count - 1) {
                    writer.Write("params ");
                }
                writer.Write("var ");
                p.Serialize(writer);
                index++;
            }
            writer.Write(") ");
            this.Body.Serialize(writer);
            return writer.ToString();
        }

        public override ScriptVariable Invoke(ScriptExecutionEnvironment env, IList<ScriptVariable> args) {
            env.Variables.EnterScope();
            try {
                int index = 0;
                foreach(var p in this.Parameters) {
                    if(this.IsParams && index == this.Parameters.Count - 1) {
                        List<ScriptVariable> @params = new List<ScriptVariable>();
                        for(; index < args.Count; index++) {
                            @params.Add(args[index]);
                        }
                        env.Variables.Declare(p.Identifier, new ListVariable(@params), VariableStorage.FieldProperty.Default);
                        break;
                    } else if(index < args.Count) {
                        env.Variables.Declare(p.Identifier, args[index], VariableStorage.FieldProperty.Default);
                    } else {
                        env.Variables.Declare(p.Identifier, null, VariableStorage.FieldProperty.Default);
                    }
                    index++;
                }
                ScriptVariable ret;
                this.Body.Execute(env, out ret);
                return ret;
            } finally {
                env.Variables.ExitScope();
            }
        }
        public override string ToString() {
            System.IO.StringWriter writer = new System.IO.StringWriter();
            writer.Write("Func(");
            bool first = true;
            foreach(var p in this.Parameters) {
                if(first) {
                    first = false;
                } else {
                    writer.Write(", ");
                }
                p.Serialize(writer);
            }
            writer.Write(") ");
            return writer.ToString();
        }
    }

    /// <summary>
    /// ScriptParamの拡張メソッドを定義するクラス
    /// </summary>
    public static class ScriptVariableExtension {
        /// <summary>
        /// 値がnullであるかを返します．
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool IsNull(this ScriptVariable param) {
            return param == null;
        }

        public static int? CompareTo(this ScriptVariable self, ScriptVariable other) {
            if(self.IsNull() && other.IsNull())
                return 0;
            if(self.IsNull() || other.IsNull())
                return null;
            if(self.Type == ScriptVariableType.Number || other.Type == ScriptVariableType.Number) {
                if(self.Type == ScriptVariableType.String || other.Type == ScriptVariableType.String) {
                    return self.ToString().CompareTo(other.ToString());
                }
            }
            if(self.Type != other.Type)
                return null;
            switch(self.Type) {
            case ScriptVariableType.Boolean:
                return self.ToBoolean().CompareTo(other.ToBoolean());
            case ScriptVariableType.Number:
                return self.ToNumber().CompareTo(other.ToNumber());
            case ScriptVariableType.String:
                return self.ToString().CompareTo(other.ToString());
            case ScriptVariableType.List:
                var list1 = self.ToList();
                var list2 = other.ToList();
                int length = Math.Min(list1.Count, list2.Count);
                for(int i = 0; i < length; i++) {
                    int? c = list1[i].CompareTo(list2[i]);
                    if(!c.HasValue)
                        return null;
                    if(c.Value != 0)
                        return c.Value;
                }
                return list1.Count.CompareTo(list2.Count);
            case ScriptVariableType.RegisteredFunction:
                RegisteredFunctionVariable regSelf = (RegisteredFunctionVariable)self;
                RegisteredFunctionVariable regOther = (RegisteredFunctionVariable)other;
                return string.Compare(regSelf.Name, regSelf.Name);
            case ScriptVariableType.Function:
                FunctionVariable funcSelf = (FunctionVariable)self;
                FunctionVariable funcOther = (FunctionVariable)other;
                if(funcSelf.Body == funcOther.Body)
                    return 0;
                return null;
            }
            return null;
        }

        public static bool EqualTo(this ScriptVariable self, ScriptVariable other) {
            return self.CompareTo(other) == 0;
        }

        public static ScriptVariable ConvertTo(this ScriptVariable self, ScriptVariableType type) {
            if(self.IsNull())
                return null;
            if(self.Type == type)
                return self;
            switch(type) {
            case ScriptVariableType.Boolean:
                return new BooleanVariable(self.ToBoolean());
            case ScriptVariableType.List:
                return new ListVariable(self.ToList());
            case ScriptVariableType.Number:
                return new NumberVariable(self.ToNumber());
            case ScriptVariableType.String:
                return new StringVariable(self.ToString());
            //case ScriptVariableType.Object:
            //return new ObjectVariable(self.ToObject());
            }
            return null;
        }
        public static ScriptVariable Add(this ScriptVariable self, ScriptVariable other) {
            if(self == null)
                return null;
            other = ConvertTo(other, self.Type);
            switch(self.Type) {
            case ScriptVariableType.Boolean:
                if(other == null)
                    return null;
                return new BooleanVariable(self.ToBoolean() || other.ToBoolean());
            case ScriptVariableType.List:
                List<ScriptVariable> list = new List<ScriptVariable>(self.ToList());
                if(other == null)
                    list.Add(null);
                else
                    list.AddRange(other.ToList());
                return new ListVariable(list);
            case ScriptVariableType.Number:
                if(other == null)
                    return self.Clone();
                try {
                    return new NumberVariable(self.ToNumber() + other.ToNumber());
                } catch(ArithmeticException ex) {
                    ScriptConsole.Singleton.Warn(ex.GetType().Name);
                    return new NumberVariable(0);
                }
            /*
        case ScriptVariableType.Object:
            ScriptSub.Singleton.Warn("cannot add " + self.Type.ToString());
            return self.Clone();
            */
            case ScriptVariableType.String:
                if(other == null)
                    return self.Clone();
                return new StringVariable(self.ToString() + other.ToString());
            }
            return null;
        }
        public static ScriptVariable Subtract(this ScriptVariable self, ScriptVariable other) {
            if(self == null)
                return null;
            other = ConvertTo(other, self.Type);
            switch(self.Type) {
            case ScriptVariableType.Boolean:
                if(other == null)
                    return null;
                return new BooleanVariable(self.ToBoolean() && !other.ToBoolean());
            case ScriptVariableType.List:
            case ScriptVariableType.String:
                //case ScriptVariableType.Object:
                ScriptConsole.Singleton.Warn("cannot subtract " + self.Type.ToString());
                return self.Clone();
            case ScriptVariableType.Number:
                if(other == null)
                    return self.Clone();
                try {
                    return new NumberVariable(self.ToNumber() - other.ToNumber());
                } catch(ArithmeticException ex) {
                    ScriptConsole.Singleton.Warn(ex.GetType().Name);
                    return new NumberVariable(0);
                }
            }
            return null;
        }
        public static ScriptVariable Multiply(this ScriptVariable self, ScriptVariable other) {
            if(self == null)
                return null;
            other = ConvertTo(other, self.Type);
            switch(self.Type) {
            case ScriptVariableType.Boolean:
                if(other == null)
                    return null;
                return new BooleanVariable(self.ToBoolean() && other.ToBoolean());
            case ScriptVariableType.List:
            case ScriptVariableType.String:
                //case ScriptVariableType.Object:
                ScriptConsole.Singleton.Warn("cannot multiply " + self.Type.ToString());
                return self.Clone();
            case ScriptVariableType.Number:
                if(other == null)
                    return self.Clone();
                try {
                    return new NumberVariable(self.ToNumber() * other.ToNumber());
                } catch(ArithmeticException ex) {
                    ScriptConsole.Singleton.Warn(ex.GetType().Name);
                    return new NumberVariable(0);
                }
            }
            return null;
        }
        public static ScriptVariable Divide(this ScriptVariable self, ScriptVariable other) {
            if(self == null)
                return null;
            other = ConvertTo(other, self.Type);
            switch(self.Type) {
            case ScriptVariableType.Boolean:
                if(other == null)
                    return null;
                return new BooleanVariable(self.ToBoolean() || !other.ToBoolean());
            case ScriptVariableType.List:
            case ScriptVariableType.String:
                //case ScriptVariableType.Object:
                ScriptConsole.Singleton.Warn("cannot divide " + self.Type.ToString());
                return self.Clone();
            case ScriptVariableType.Number:
                if(other == null)
                    return self.Clone();
                try {
                    return new NumberVariable(self.ToNumber() / other.ToNumber());
                } catch(ArithmeticException ex) {
                    ScriptConsole.Singleton.Warn(ex.GetType().Name);
                    return new NumberVariable(0);
                }
            }
            return null;
        }
        public static ScriptVariable Remainder(this ScriptVariable self, ScriptVariable other) {
            if(self == null || other == null)
                return null;
            try {

                return new NumberVariable(decimal.Remainder(self.ToNumber(), other.ToNumber()));
            } catch(ArithmeticException ex) {
                ScriptConsole.Singleton.Warn(ex.GetType().Name);
                return null;
            }
        }
    }


}
