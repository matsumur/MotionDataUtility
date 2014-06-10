using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MotionDataHandler.Script.Parse {

    public class ProgramSyntaxElement : Collection<SyntaxElement> {
        public ProgramSyntaxElement() : base() { }
        public ProgramSyntaxElement(IList<SyntaxElement> list) : base(list) { }
    }
    /// <summary>
    /// 構文要素の基底クラス
    /// </summary>
    public abstract class SyntaxElement {
        /// <summary>
        /// 構文要素の始まる位置の字句要素
        /// </summary>
        public LexicalElement LexAtStart;
        /// <summary>
        /// 内部のコンストラクタ
        /// </summary>
        /// <param name="lexAtStart"></param>
        protected SyntaxElement(LexicalElement lexAtStart) {
            this.LexAtStart = lexAtStart;
        }
        /// <summary>
        /// 実際の実行内容を処理します．
        /// </summary>
        /// <param name="env">変数環境</param>
        /// <param name="returnValue">式の戻り値</param>
        /// <returns>制御情報を返します</returns>
        protected abstract RunControlType ExecuteInternal(ScriptExecutionEnvironment env, out ScriptVariable returnValue);
        /// <summary>
        /// 構文要素からプログラムを復元します．
        /// </summary>
        /// <param name="writer">出力先のライタ</param>
        public abstract void Serialize(TextWriter writer);
        /// <summary>
        /// 実行時情報を設定しつつ，実行処理を呼び出します．
        /// </summary>
        /// <param name="env">変数環境</param>
        /// <param name="returnValue">式の戻り値</param>
        /// <returns>制御情報を返します</returns>
        public RunControlType Execute(ScriptExecutionEnvironment env, out ScriptVariable returnValue) {
            env.Console.PushSyntaxStack(this);
            try {
                return this.ExecuteInternal(env, out returnValue);
            } finally {
                env.Console.PopSyntaxStack();
            }
        }
    }

    /// <summary>
    /// 式要素の基底クラス．型チェック用
    /// </summary>
    public abstract class ExpressionSyntaxElement : SyntaxElement {
        protected ExpressionSyntaxElement(LexicalElement lexAtStart)
            : base(lexAtStart) {
        }
        /// <summary>
        /// 式の値を返します．
        /// </summary>
        /// <param name="env">変数環境</param>
        /// <returns></returns>
        protected abstract ScriptVariable CalculateInternal(ScriptExecutionEnvironment env);
        protected sealed override RunControlType ExecuteInternal(ScriptExecutionEnvironment env, out ScriptVariable returnValue) {
            returnValue = CalculateInternal(env);
            return RunControlType.None;
        }
        /// <summary>
        /// 実行処理を呼び出し，値を返します
        /// </summary>
        /// <param name="env">変数環境</param>
        /// <returns>式の値</returns>
        public ScriptVariable Calculate(ScriptExecutionEnvironment env) {
            ScriptVariable returnValue;
            this.Execute(env, out returnValue);
            return returnValue;
        }
    }
    /// <summary>
    /// 数値リテラル要素
    /// </summary>
    public class NumberSyntaxElement : ExpressionSyntaxElement {
        public readonly decimal Value;
        public NumberSyntaxElement(LexicalElement lexAtStart, decimal value)
            : base(lexAtStart) {
            this.Value = value;
        }
        protected override ScriptVariable CalculateInternal(ScriptExecutionEnvironment env) {
            return new NumberVariable(this.Value);
        }
        public override void Serialize(TextWriter writer) {
            writer.Write(this.Value);
        }
    }
    /// <summary>
    /// 文字列リテラル要素
    /// </summary>
    public class StringSyntaxElement : ExpressionSyntaxElement {
        public readonly string Value;
        public static string UnescapeString(string escapedString) {
            StringWriter ret = new StringWriter();
            if(!escapedString.StartsWith("\"")) {
                throw new ArgumentException("'escapedString' must be start with '\"'", "escapedString");
            }
            if(!escapedString.EndsWith("\"")) {
                throw new ArgumentException("'escapedString' must be end with '\"'", "escapedString");
            }
            for(int i = 1; i < escapedString.Length - 1; i++) {
                if(escapedString[i] == '\\') {
                    i++;
                    switch(escapedString[i]) {
                    case 'n':
                        ret.Write("\n");
                        break;
                    case 'f':
                        ret.Write("\f");
                        break;
                    case 't':
                        ret.Write("\t");
                        break;
                    case 'r':
                        ret.Write("\r");
                        break;
                    case 'b':
                        ret.Write("\b");
                        break;
                    default:
                        ret.Write(escapedString[i]);
                        break;
                    }
                } else {
                    ret.Write(escapedString[i]);
                }
            }
            return ret.ToString();
        }

        public static string EscapeString(string unescapedString) {
            StringWriter ret = new StringWriter();
            ret.Write('"');
            foreach(char c in unescapedString) {
                switch(c) {
                case '\n':
                    ret.Write("\\n");
                    break;
                case '\r':
                    ret.Write("\\r");
                    break;
                case '\f':
                    ret.Write("\\f");
                    break;
                case '\b':
                    ret.Write("\\b");
                    break;
                case '\t':
                    ret.Write("\\t");
                    break;
                case '\\':
                case '"':
                    ret.Write("\\");
                    ret.Write(c);
                    break;
                default:
                    ret.Write(c);
                    break;
                }
            }
            ret.Write('"');
            return ret.ToString();
        }
        public StringSyntaxElement(LexicalElement lexAtStart, string value)
            : base(lexAtStart) {
            this.Value = UnescapeString(value);
        }
        protected override ScriptVariable CalculateInternal(ScriptExecutionEnvironment env) {
            return new StringVariable(this.Value);
        }

        public override void Serialize(TextWriter writer) {
            writer.Write(EscapeString(this.Value));
        }
    }
    /// <summary>
    /// 真理値リテラル要素
    /// </summary>
    public class BooleanSyntaxElement : ExpressionSyntaxElement {
        public readonly bool Value;
        public BooleanSyntaxElement(LexicalElement lexAtStart, bool value)
            : base(lexAtStart) {
            this.Value = value;
        }

        protected override ScriptVariable CalculateInternal(ScriptExecutionEnvironment env) {
            return new BooleanVariable(this.Value);
        }

        public override void Serialize(TextWriter writer) {
            writer.Write(this.Value ? "true" : "false");
        }
    }
    /// <summary>
    /// nullリテラル要素
    /// </summary>
    public class NullSyntaxElement : ExpressionSyntaxElement {
        public NullSyntaxElement(LexicalElement lexAtStart)
            : base(lexAtStart) { }
        protected override ScriptVariable CalculateInternal(ScriptExecutionEnvironment env) {
            return null;
        }
        public override void Serialize(TextWriter writer) {
            writer.Write("null");
        }
    }
    /// <summary>
    /// リストコンストラクタ要素
    /// </summary>
    public class ListSyntaxElement : ExpressionSyntaxElement {
        public readonly ExpressionSyntaxElement[] Elements;
        public ListSyntaxElement(LexicalElement lexAtStart, IList<ExpressionSyntaxElement> elems)
            : base(lexAtStart) {
            this.Elements = elems.ToArray();
        }
        protected override ScriptVariable CalculateInternal(ScriptExecutionEnvironment env) {
            List<ScriptVariable> ret = new List<ScriptVariable>();
            foreach(var elem in this.Elements) {
                ret.Add(elem.Calculate(env));
            }
            return new ListVariable(ret);
        }

        public override void Serialize(TextWriter writer) {
            writer.Write("{ ");
            bool first = true;
            foreach(var elem in this.Elements) {
                if(first)
                    first = false;
                else
                    writer.Write(", ");
                elem.Serialize(writer);
            }
            if(this.Elements.Length > 0)
                writer.Write(" ");
            writer.Write("}");
        }
    }
    /// <summary>
    /// 二項演算要素
    /// </summary>
    public class BinarySyntaxElement : ExpressionSyntaxElement {
        public enum OperatorType {
            Plus,
            Minus,
            Cross,
            Slash,
            Percent,
            Eq,
            Ne,
            Gt,
            Ge,
            Lt,
            Le,
            And,
            Or,
        }
        public readonly ExpressionSyntaxElement Left, Right;
        public readonly OperatorType Operator;
        public BinarySyntaxElement(LexicalElement lexAtStart, OperatorType @operator, ExpressionSyntaxElement left, ExpressionSyntaxElement right)
            : base(lexAtStart) {
            this.Operator = @operator;
            this.Left = left;
            this.Right = right;
        }
        protected override ScriptVariable CalculateInternal(ScriptExecutionEnvironment env) {
            ScriptVariable left = this.Left.Calculate(env);
            // ショートサーキット演算子
            switch(this.Operator) {
            case OperatorType.And:
                if(left == null) {
                    return null;
                }
                if(!left.ToBoolean()) {
                    return new BooleanVariable(false);
                }
                return this.Right.Calculate(env);

            case OperatorType.Or:
                if(left == null) {
                    return null;
                }
                if(left.ToBoolean()) {
                    return new BooleanVariable(true);
                }
                return this.Right.Calculate(env);
            }
            // 右辺
            ScriptVariable right = this.Right.Calculate(env);

            if(left == null || right == null) {
                // 片方nullの場合は等価比較演算以外はnull
                switch(this.Operator) {
                case OperatorType.Eq:
                    return new BooleanVariable(left == right);
                case OperatorType.Ne:
                    return new BooleanVariable(left != right);
                default:
                    return null;
                }
            }
            // 普通の二項演算
            switch(this.Operator) {
            case OperatorType.Cross:
                return left.Multiply(right);
            case OperatorType.Plus:
                return left.Add(right);
            case OperatorType.Minus:
                return left.Subtract(right);
            case OperatorType.Slash:
                return left.Divide(right);
            case OperatorType.Percent:
                return left.Remainder(right);
            }
            // 比較演算子
            int? tmp = left.CompareTo(right);
            if(tmp.HasValue) {
                int value = tmp.Value;
                switch(this.Operator) {
                case OperatorType.Eq:
                    return new BooleanVariable(value == 0);
                case OperatorType.Ne:
                    return new BooleanVariable(value != 0);
                case OperatorType.Gt:
                    return new BooleanVariable(value > 0);
                case OperatorType.Ge:
                    return new BooleanVariable(value >= 0);
                case OperatorType.Lt:
                    return new BooleanVariable(value < 0);
                case OperatorType.Le:
                    return new BooleanVariable(value <= 0);
                }
            }
            return null;
        }
        public override void Serialize(TextWriter writer) {
            if(writer == null)
                throw new ArgumentNullException("writer", "'writer' cannot be null");
            this.Left.Serialize(writer);
            switch(this.Operator) {
            case OperatorType.And:
                writer.Write(" && ");
                break;
            case OperatorType.Cross:
                writer.Write(" * ");
                break;
            case OperatorType.Eq:
                writer.Write(" == ");
                break;
            case OperatorType.Ge:
                writer.Write(" >= ");
                break;
            case OperatorType.Gt:
                writer.Write(" > ");
                break;
            case OperatorType.Le:
                writer.Write(" <= ");
                break;
            case OperatorType.Lt:
                writer.Write(" < ");
                break;
            case OperatorType.Minus:
                writer.Write(" - ");
                break;
            case OperatorType.Ne:
                writer.Write(" != ");
                break;
            case OperatorType.Or:
                writer.Write(" || ");
                break;
            case OperatorType.Percent:
                writer.Write(" % ");
                break;
            case OperatorType.Plus:
                writer.Write(" + ");
                break;
            case OperatorType.Slash:
                writer.Write(" / ");
                break;
            }
            this.Right.Serialize(writer);
        }
    }
    /// <summary>
    /// 変数宣言のうち，一つの変数についての宣言部分を示す要素
    /// </summary>
    public class DeclareSyntaxElement : ExpressionSyntaxElement {
        public readonly string Identifier;
        public readonly ExpressionSyntaxElement InitialValue;
        public readonly VariableStorage.FieldProperty Property;
        public DeclareSyntaxElement(LexicalElement lexAtStart, string identifier, VariableStorage.FieldProperty property, ExpressionSyntaxElement initialValue)
            : base(lexAtStart) {
            this.Identifier = identifier;
            this.InitialValue = initialValue;
            this.Property = property;
        }
        public DeclareSyntaxElement(LexicalElement lexAtStart, string identifier, VariableStorage.FieldProperty property) : this(lexAtStart, identifier, property, null) { }
        protected override ScriptVariable CalculateInternal(ScriptExecutionEnvironment env) {
            ScriptVariable value = null;
            if(this.InitialValue != null) {
                value = this.InitialValue.Calculate(env);
            }
            env.Variables.Declare(this.Identifier, value, this.Property);
            return value == null ? null : value.Clone();
        }
        public override void Serialize(TextWriter writer) {
            writer.Write(this.Identifier);
            if(this.InitialValue != null) {
                writer.Write(" = ");
                this.InitialValue.Serialize(writer);
            }
        }
    }
    /// <summary>
    /// 代入及び演算代入の要素
    /// </summary>
    public class SubstituteSyntaxElement : ExpressionSyntaxElement {
        public enum OperatorType {
            Add,
            Subtract,
            Multiply,
            Divide,
            Remainder,
            Substitute
        }
        public readonly OperatorType Operator;
        public readonly AccessorSyntaxElement Accessor;
        public readonly ExpressionSyntaxElement RightValue;
        public SubstituteSyntaxElement(LexicalElement lexAtStart, AccessorSyntaxElement accessor, OperatorType @operator, ExpressionSyntaxElement rightValue)
            : base(lexAtStart) {
            this.Accessor = accessor;
            this.Operator = @operator;
            this.RightValue = rightValue;
        }

        private static ScriptVariable operate(ScriptVariable leftValue, ScriptVariable rightValue, OperatorType @operator) {
            switch(@operator) {
            case OperatorType.Add:
                return leftValue.Add(rightValue);
            case OperatorType.Subtract:
                return leftValue.Subtract(rightValue);
            case OperatorType.Multiply:
                return leftValue.Multiply(rightValue);
            case OperatorType.Divide:
                return leftValue.Divide(rightValue);
            case OperatorType.Remainder:
                return leftValue.Remainder(rightValue);
            }
            return rightValue;
        }
        protected override ScriptVariable CalculateInternal(ScriptExecutionEnvironment env) {
            ScriptVariable value = null;
            if(this.RightValue != null) {
                value = this.RightValue.Calculate(env);
            }
            IVariableAccessor access = this.Accessor.GetAccessor(env);
            if(this.Operator == OperatorType.Substitute) {
                access.Set(env, value == null ? null : value.Clone());
                return value;
            } else {
                ScriptVariable leftValue = access.Get(env);
                ScriptVariable returnValue = operate(leftValue, value, this.Operator);
                access.Set(env, returnValue);
                return returnValue;
            }
        }
        public override void Serialize(TextWriter writer) {
            this.Accessor.Serialize(writer);
            switch(this.Operator) {
            case OperatorType.Substitute:
                writer.Write(" = ");
                break;
            case OperatorType.Add:
                writer.Write(" += ");
                break;
            case OperatorType.Subtract:
                writer.Write(" -= ");
                break;
            case OperatorType.Multiply:
                writer.Write(" *= ");
                break;
            case OperatorType.Divide:
                writer.Write(" /= ");
                break;
            case OperatorType.Remainder:
                writer.Write(" %= ");
                break;
            }
            this.RightValue.Serialize(writer);
        }
    }
    /// <summary>
    /// 変数宣言の列を表す要素
    /// </summary>
    public class MultiDeclareSyntaxElement : ExpressionSyntaxElement {
        public readonly DeclareSyntaxElement[] Declarations;
        public readonly VariableStorage.FieldProperty Property;
        public MultiDeclareSyntaxElement(LexicalElement lexAtStart, VariableStorage.FieldProperty property, IList<DeclareSyntaxElement> declarations)
            : base(lexAtStart) {
            this.Declarations = declarations.ToArray();
            this.Property = property;
        }
        protected override ScriptVariable CalculateInternal(ScriptExecutionEnvironment env) {
            ScriptVariable returnValue = null;
            foreach(var declare in this.Declarations) {
                returnValue = declare.Calculate(env);
            }
            return returnValue;
        }

        public override void Serialize(TextWriter writer) {
            if(this.Declarations.Length > 0) {
                if(this.Property.Readonly) {
                    writer.Write("readonly ");
                }
                writer.Write("var ");
                bool first = true;
                foreach(var declare in this.Declarations) {
                    if(first)
                        first = false;
                    else
                        writer.Write(", ");
                    declare.Serialize(writer);
                }
            }
        }
    }
    /// <summary>
    /// 変数へアクセスする構文要素のための抽象クラス
    /// </summary>
    public abstract class AccessorSyntaxElement : ExpressionSyntaxElement {
        /// <summary>
        /// 開始位置の字句要素を指定するコンストラクタ
        /// </summary>
        /// <param name="lexAtStart"></param>
        protected AccessorSyntaxElement(LexicalElement lexAtStart) : base(lexAtStart) { }
        /// <summary>
        /// 変数へのアクセスのためインターフェースを取得します
        /// </summary>
        /// <param name="env">スクリプト環境</param>
        /// <returns></returns>
        public abstract IVariableAccessor GetAccessor(ScriptExecutionEnvironment env);
        /// <summary>
        /// 変数へアクセスし，値を取得します．
        /// </summary>
        /// <param name="env">スクリプト環境</param>
        /// <returns></returns>
        protected override ScriptVariable CalculateInternal(ScriptExecutionEnvironment env) {
            IVariableAccessor accessor = this.GetAccessor(env);
            return accessor.Get(env);
        }
    }
    /// <summary>
    /// 識別子を用いて変数にアクセスする構文要素
    /// </summary>
    public class IdentifierSyntaxElement : AccessorSyntaxElement {
        public readonly string Identifier;
        public override void Serialize(TextWriter writer) {
            writer.Write(this.Identifier);
        }
        public override IVariableAccessor GetAccessor(ScriptExecutionEnvironment env) {
            return new IdentifierAccessor(this.Identifier);
        }
        public IdentifierSyntaxElement(LexicalElement lexAtStart, string identifier)
            : base(lexAtStart) {
            this.Identifier = identifier;
        }
        public IdentifierSyntaxElement(LexicalElement identifierElement)
            : this(identifierElement, identifierElement.Text) {
        }
    }
    /// <summary>
    /// インデックスを用いて変数にアクセスする構文要素
    /// </summary>
    public class IndexerSyntaxElement : AccessorSyntaxElement {
        public readonly ExpressionSyntaxElement Variable;
        public readonly ExpressionSyntaxElement Index;
        public IndexerSyntaxElement(LexicalElement lexAtStart, ExpressionSyntaxElement variable, ExpressionSyntaxElement index)
            : base(lexAtStart) {
            this.Variable = variable;
            this.Index = index;
        }
        public IndexerSyntaxElement(LexicalElement lexAtStart, ExpressionSyntaxElement variable)
            : this(lexAtStart, variable, null) { }
        public override IVariableAccessor GetAccessor(ScriptExecutionEnvironment env) {
            ScriptVariable variable = this.Variable.Calculate(env);
            ScriptVariable index;
            if(this.Index == null) {
                ListVariable list = variable as ListVariable;
                if(list != null) {
                    index = new NumberVariable(list.Value.Count);
                } else {
                    env.Console.Warn("Cannot use empty index except for List");
                    return new NullAccessor();
                }
            } else {
                index = this.Index.Calculate(env);
            }
            return new IndexedVariableAccessor(variable, index);
        }

        public override void Serialize(TextWriter writer) {
            this.Variable.Serialize(writer);
            writer.Write("[");
            if(this.Index != null)
                this.Index.Serialize(writer);
            writer.Write("]");
        }
    }
    /// <summary>
    /// 関数型を作成する構文要素
    /// </summary>
    public class FunctionSyntaxElement : ExpressionSyntaxElement {
        public readonly bool IsParams;
        public readonly IList<IdentifierSyntaxElement> Parameters;
        public readonly SyntaxElement Body;
        public FunctionSyntaxElement(LexicalElement lexAtStart, IList<IdentifierSyntaxElement> parameters, SyntaxElement body, bool isParams)
            : base(lexAtStart) {
            this.Parameters = parameters;
            this.Body = body;
            this.IsParams = isParams;
        }
        protected override ScriptVariable CalculateInternal(ScriptExecutionEnvironment env) {
            return new FunctionVariable(this.Parameters, this.Body, this.IsParams);
        }

        public override void Serialize(TextWriter writer) {
            ScriptVariable returnValue = new FunctionVariable(this.Parameters, this.Body, this.IsParams);
            writer.Write(returnValue.Serialize());
        }
    }
    /// <summary>
    /// 関数を呼び出す構文要素
    /// </summary>
    public class InvokeSyntaxElement : ExpressionSyntaxElement {
        public readonly ExpressionSyntaxElement Function;
        public readonly ExpressionSyntaxElement[] Parameters;
        public readonly bool IsDotAccess;
        public InvokeSyntaxElement(LexicalElement lexAtStart, ExpressionSyntaxElement function, IList<ExpressionSyntaxElement> parameters)
            : base(lexAtStart) {
            this.Function = function;
            this.Parameters = parameters.ToArray();
            this.IsDotAccess = false;
        }
        public InvokeSyntaxElement(LexicalElement lexAtStart, ExpressionSyntaxElement function, ExpressionSyntaxElement firstParameter, IList<ExpressionSyntaxElement> parameters)
            : base(lexAtStart) {
            this.Function = function;
            this.Parameters = new[] { firstParameter }.Union(parameters).ToArray();
            this.IsDotAccess = true;
        }
        protected override ScriptVariable CalculateInternal(ScriptExecutionEnvironment env) {
            ScriptVariable function = this.Function.Calculate(env);
            if(function == null) {
                env.Console.Warn("Cannot call null as function");
                return null;
            }
            List<ScriptVariable> args = new List<ScriptVariable>();
            foreach(var param in this.Parameters) {
                args.Add(param.Calculate(env));
            }
            return function.Invoke(env, args);
        }

        public override void Serialize(TextWriter writer) {
            if(this.IsDotAccess) {
                this.Parameters[0].Serialize(writer);
                writer.Write(".");
                this.Function.Serialize(writer);
                writer.Write("(");
                for(int i = 1; i < this.Parameters.Length; i++) {
                    if(i != 1)
                        writer.Write(", ");
                    this.Parameters[i].Serialize(writer);
                }
                writer.Write(")");
            } else {
                this.Function.Serialize(writer);
                writer.Write("(");
                bool first = true;
                foreach(var arg in this.Parameters) {
                    if(first)
                        first = false;
                    else
                        writer.Write(", ");
                    arg.Serialize(writer);
                }
                writer.Write(")");
            }
        }
    }
    /// <summary>
    /// Break, Continue, 及びReturnを扱う構文要素
    /// </summary>
    public class ControlSyntaxElement : SyntaxElement {
        public readonly RunControlType Type;
        public readonly SyntaxElement ReturnValue = null;
        public ControlSyntaxElement(LexicalElement lexAtStart, SyntaxElement returnValue)
            : base(lexAtStart) {
            this.Type = RunControlType.Return;
            this.ReturnValue = returnValue;
        }
        public ControlSyntaxElement(LexicalElement lexAtStart, RunControlType type)
            : base(lexAtStart) {
            this.Type = type;
        }
        protected override RunControlType ExecuteInternal(ScriptExecutionEnvironment env, out ScriptVariable returnValue) {
            if(this.ReturnValue == null) {
                returnValue = null;
            } else {
                this.ReturnValue.Execute(env, out returnValue);
            }
            return this.Type;
        }
        public override void Serialize(TextWriter writer) {
            switch(this.Type) {
            case RunControlType.Break:
                writer.Write("break");
                break;
            case RunControlType.Continue:
                writer.Write("continue");
                break;
            case RunControlType.Return:
                writer.Write("return");
                if(this.ReturnValue != null) {
                    writer.Write(" ");
                    this.ReturnValue.Serialize(writer);
                }
                break;
            }
            writer.WriteLine(";");
        }
    }
    /// <summary>
    /// 単項演算要素
    /// </summary>
    public class AnarySyntaxElement : ExpressionSyntaxElement {
        public enum OperatorType { Plus, Minus, Not }
        public readonly OperatorType Operator;
        public readonly ExpressionSyntaxElement Operand;
        public AnarySyntaxElement(LexicalElement lexAtStart, OperatorType @operator, ExpressionSyntaxElement operand)
            : base(lexAtStart) {
            this.Operator = @operator;
            this.Operand = operand;
        }
        protected override ScriptVariable CalculateInternal(ScriptExecutionEnvironment env) {
            ScriptVariable operand = this.Operand.Calculate(env);
            switch(this.Operator) {
            case OperatorType.Minus:
                return new NumberVariable(-operand.ToNumber());
            case OperatorType.Plus:
                return new NumberVariable(operand.ToNumber());
            case OperatorType.Not:
                return new BooleanVariable(!operand.ToBoolean());
            }
            throw new NotImplementedException(this.Operator.ToString());
        }

        public override void Serialize(TextWriter writer) {
            switch(this.Operator) {
            case OperatorType.Minus:
                writer.Write("-");
                break;
            case OperatorType.Plus:
                writer.Write("+");
                break;
            case OperatorType.Not:
                writer.Write("!");
                break;
            }
            if(this.Operand is AnarySyntaxElement && (this.Operator == OperatorType.Minus || this.Operator == OperatorType.Plus)) {
                writer.Write(" ");
            }
            this.Operand.Serialize(writer);
        }
    }
    /// <summary>
    /// 変数のインクリメント及びデクリメントを行う構文要素
    /// </summary>
    public class IncrementSyntaxElement : ExpressionSyntaxElement {
        public enum OperatorType {
            PreInc,
            PostInc,
            PreDec,
            PostDec,
        }
        public readonly OperatorType Operator;
        public readonly AccessorSyntaxElement Accessor;
        public IncrementSyntaxElement(LexicalElement lexAtStart, AccessorSyntaxElement accessor, OperatorType @operator)
            : base(lexAtStart) {
            this.Operator = @operator;
            this.Accessor = accessor;
        }
        private ScriptVariable incOrDec(ScriptVariable value) {
            switch(this.Operator) {
            case OperatorType.PostDec:
            case OperatorType.PreDec:
                return value.Subtract(new NumberVariable(1M));
            case OperatorType.PostInc:
            case OperatorType.PreInc:
                return value.Add(new NumberVariable(1M));
            }
            throw new NotImplementedException();
        }
        protected override ScriptVariable CalculateInternal(ScriptExecutionEnvironment env) {
            ScriptVariable returnValue = null;
            IVariableAccessor access = this.Accessor.GetAccessor(env);
            ScriptVariable value = access.Get(env);
            switch(this.Operator) {
            case OperatorType.PostInc:
            case OperatorType.PostDec:
                returnValue = value;
                break;
            }
            value = incOrDec(value);
            switch(this.Operator) {
            case OperatorType.PreInc:
            case OperatorType.PreDec:
                returnValue = value;
                break;
            }
            access.Set(env, value);
            return returnValue;
        }
        public override void Serialize(TextWriter writer) {
            switch(this.Operator) {
            case OperatorType.PreInc:
                writer.Write("++");
                break;
            case OperatorType.PreDec:
                writer.Write("--");
                break;
            }

            this.Accessor.Serialize(writer);

            switch(this.Operator) {
            case OperatorType.PostInc:
                writer.Write("++");
                break;
            case OperatorType.PostDec:
                writer.Write("--");
                break;
            }
        }
    }
    /// <summary>
    /// 三項演算要素
    /// </summary>
    public class TernarySyntaxElement : ExpressionSyntaxElement {
        public enum OperatorType { Condition }
        public readonly OperatorType Operator;
        public readonly ExpressionSyntaxElement First, Second, Third;
        public TernarySyntaxElement(LexicalElement lexAtStart, OperatorType @operator, ExpressionSyntaxElement first, ExpressionSyntaxElement second, ExpressionSyntaxElement third)
            : base(lexAtStart) {
            this.Operator = @operator;
            this.First = first;
            this.Second = second;
            this.Third = third;
        }
        protected override ScriptVariable CalculateInternal(ScriptExecutionEnvironment env) {
            switch(this.Operator) {
            case OperatorType.Condition:
                ScriptVariable p1 = this.First.Calculate(env);
                if(p1 != null && p1.ToBoolean()) {
                    return this.Second.Calculate(env);
                } else {
                    return this.Third.Calculate(env);
                }
            default:
                throw new NotImplementedException(this.Operator.ToString());
            }
        }
        public override void Serialize(TextWriter writer) {
            switch(this.Operator) {
            case OperatorType.Condition:
                this.First.Serialize(writer);
                writer.Write(" ? ");
                this.Second.Serialize(writer);
                writer.Write(" : ");
                this.Third.Serialize(writer);
                break;
            }
        }
    }
    /// <summary>
    /// if文要素
    /// </summary>
    public class IfSyntaxElement : SyntaxElement {
        public readonly ExpressionSyntaxElement Condition;
        public readonly SyntaxElement Then;
        public readonly SyntaxElement Else = null;
        public IfSyntaxElement(LexicalElement lexAtStart, ExpressionSyntaxElement condition, SyntaxElement then, SyntaxElement @else)
            : base(lexAtStart) {
            this.Condition = condition;
            this.Then = then;
            this.Else = @else;
        }
        public IfSyntaxElement(LexicalElement lexAtStart, ExpressionSyntaxElement condition, SyntaxElement then)
            : this(lexAtStart, condition, then, null) { }

        protected override RunControlType ExecuteInternal(ScriptExecutionEnvironment env, out ScriptVariable returnValue) {
            returnValue = null;
            ScriptVariable condition = this.Condition.Calculate(env);
            if(condition != null && condition.ToBoolean()) {
                if(this.Then == null)
                    return RunControlType.None;
                RunControlType ctrl = this.Then.Execute(env, out returnValue);
                return ctrl;
            } else {
                if(this.Else == null)
                    return RunControlType.None;
                RunControlType ctrl = this.Else.Execute(env, out returnValue);
                return ctrl;
            }
        }
        public override void Serialize(TextWriter writer) {
            writer.Write("if(");
            this.Condition.Serialize(writer);
            writer.Write(") ");
            if(this.Then != null) {
                this.Then.Serialize(writer);
            } else {
                writer.WriteLine(";");
            }
            if(this.Else != null) {
                writer.Write("else ");
                this.Else.Serialize(writer);
            }
        }
    }
    /// <summary>
    /// while文またはdo-while文要素
    /// </summary>
    public class DoWhileSyntaxElement : SyntaxElement {
        public readonly bool Do;
        public readonly ExpressionSyntaxElement Condition;
        public readonly SyntaxElement Statement;
        public DoWhileSyntaxElement(LexicalElement lexAtStart, ExpressionSyntaxElement condition, SyntaxElement statement, bool @do)
            : base(lexAtStart) {
            this.Condition = condition;
            this.Statement = statement;
            this.Do = @do;
        }
        protected override RunControlType ExecuteInternal(ScriptExecutionEnvironment env, out ScriptVariable returnValue) {
            returnValue = null;
            if(!this.Do) {
                ScriptVariable condition = this.Condition.Calculate(env);
                if(condition == null || !condition.ToBoolean())
                    return RunControlType.None;
            }
            while(true) {
                RunControlType ctrl = this.Statement.Execute(env, out returnValue);
                if(ctrl == RunControlType.Break) {
                    return RunControlType.None;
                }
                if(ctrl == RunControlType.Return) {
                    return RunControlType.Return;
                }

                ScriptVariable condition = this.Condition.Calculate(env);
                if(condition == null || !condition.ToBoolean())
                    return RunControlType.None;
            }
        }
        public override void Serialize(TextWriter writer) {
            if(this.Do) {
                writer.Write("do ");
                this.Statement.Serialize(writer);
                writer.Write(" while(");
                this.Condition.Serialize(writer);
                writer.WriteLine(");");
            } else {
                writer.Write("while(");
                this.Condition.Serialize(writer);
                writer.Write(") ");
                this.Statement.Serialize(writer);
            }
        }
    }
    /// <summary>
    /// for文要素
    /// </summary>
    public class ForSyntaxElement : SyntaxElement {
        public readonly ExpressionSyntaxElement Initialize, Condition, Continue;
        public readonly SyntaxElement Statement;
        public ForSyntaxElement(LexicalElement lexAtStart, ExpressionSyntaxElement initialize, ExpressionSyntaxElement condition, ExpressionSyntaxElement @continue, SyntaxElement statement)
            : base(lexAtStart) {
            this.Initialize = initialize;
            this.Condition = condition;
            this.Continue = @continue;
            this.Statement = statement;
        }
        protected override RunControlType ExecuteInternal(ScriptExecutionEnvironment env, out ScriptVariable returnValue) {
            returnValue = null;
            env.Variables.EnterScope();
            try {
                if(this.Initialize != null) {
                    this.Initialize.Calculate(env);
                }
                while(true) {
                    if(this.Condition != null) {
                        ScriptVariable condition = this.Condition.Calculate(env);
                        if(condition == null || !condition.ToBoolean()) {
                            return RunControlType.None;
                        }
                    }
                    if(this.Statement != null) {
                        RunControlType ctrl = this.Statement.Execute(env, out returnValue);
                        if(ctrl == RunControlType.Break) {
                            return RunControlType.None;
                        }
                        if(ctrl == RunControlType.Return) {
                            return RunControlType.Return;
                        }
                    }
                    if(this.Continue != null) {
                        this.Continue.Calculate(env);
                    }
                }
            } finally { env.Variables.ExitScope(); }
        }
        public override void Serialize(TextWriter writer) {
            writer.Write("for(");
            if(this.Initialize != null)
                this.Initialize.Serialize(writer);
            writer.Write("; ");
            if(this.Condition != null)
                this.Condition.Serialize(writer);
            writer.Write("; ");
            if(this.Continue != null)
                this.Continue.Serialize(writer);
            writer.Write(") ");
            this.Statement.Serialize(writer);
        }
    }
    /// <summary>
    /// foreach文要素
    /// </summary>
    public class ForeachSyntaxElement : SyntaxElement {
        public readonly string Identifier;
        public readonly bool Declare;
        public readonly ExpressionSyntaxElement Enumerator;
        public readonly SyntaxElement Statement;
        public ForeachSyntaxElement(LexicalElement lexAtStart, string identifier, bool declare, ExpressionSyntaxElement enumerator, SyntaxElement statement)
            : base(lexAtStart) {
            this.Identifier = identifier;
            this.Declare = declare;
            this.Enumerator = enumerator;
            this.Statement = statement;
        }
        protected override RunControlType ExecuteInternal(ScriptExecutionEnvironment env, out ScriptVariable returnValue) {
            returnValue = null;
            env.Variables.EnterScope();
            try {
                env.Variables.Declare(this.Identifier, null, VariableStorage.FieldProperty.Default);
                ScriptVariable list = this.Enumerator.Calculate(env) ?? new ListVariable();
                foreach(var tmp in list.ToList()) {
                    env.Variables.Set(this.Identifier, tmp);
                    RunControlType ctrl = this.Statement.Execute(env, out returnValue);
                    if(ctrl == RunControlType.Break) {
                        return RunControlType.None;
                    }
                    if(ctrl == RunControlType.Return) {
                        return RunControlType.Return;
                    }
                }
            } finally { env.Variables.ExitScope(); }
            return RunControlType.None;
        }
        public override void Serialize(TextWriter writer) {
            writer.Write("foreach(");
            if(this.Declare)
                writer.Write("var ");
            writer.Write(this.Identifier);
            writer.Write(" in ");
            this.Enumerator.Serialize(writer);
            writer.Write(") ");
            this.Statement.Serialize(writer);
        }
    }
    /// <summary>
    /// 式を文とするための構文要素
    /// </summary>
    public class TerminatedExpressionSyntaxElement : SyntaxElement {
        public readonly ExpressionSyntaxElement Expression;
        public TerminatedExpressionSyntaxElement(LexicalElement lexAtStart)
            : this(lexAtStart, null) { }
        public TerminatedExpressionSyntaxElement(LexicalElement lexAtStart, ExpressionSyntaxElement expression)
            : base(lexAtStart) {
            this.Expression = expression;
        }
        protected override RunControlType ExecuteInternal(ScriptExecutionEnvironment env, out ScriptVariable returnValue) {
            if(this.Expression == null) {
                returnValue = null;
                return RunControlType.None;
            }
            return this.Expression.Execute(env, out returnValue);
        }
        public override void Serialize(TextWriter writer) {
            if(this.Expression != null) {
                this.Expression.Serialize(writer);
            }
            writer.WriteLine(";");
        }
    }

    /// <summary>
    /// 制御ブロック要素
    /// </summary>
    public class BlockSyntaxElement : SyntaxElement {
        public readonly SyntaxElement[] Statements;
        public readonly bool NoNewLineAtEnd;
        public BlockSyntaxElement(LexicalElement lexAtStart, IList<SyntaxElement> statements, bool noNewLineAtEnd)
            : base(lexAtStart) {
            this.Statements = statements.ToArray();
            this.NoNewLineAtEnd = noNewLineAtEnd;
        }
        protected override RunControlType ExecuteInternal(ScriptExecutionEnvironment env, out ScriptVariable returnValue) {
            returnValue = null;
            env.Variables.EnterScope();
            try {
                foreach(var statement in this.Statements) {
                    RunControlType ctrl = statement.Execute(env, out returnValue);
                    if(ctrl != RunControlType.None) {
                        return ctrl;
                    }
                }
            } finally {
                env.Variables.ExitScope();
            }
            return RunControlType.None;
        }
        public override void Serialize(TextWriter writer) {
            if(this.Statements.Length == 0) {
                writer.Write("{ ");
            } else {
                writer.WriteLine("{");
            }
            foreach(var statement in this.Statements) {
                statement.Serialize(writer);
            }
            if(this.NoNewLineAtEnd) {
                writer.Write("}");
            } else {
                writer.WriteLine("}");
            }
        }
    }
    /// <summary>
    /// 複数式を保持する構文要素
    /// </summary>
    public class MultiExpressionSyntaxElement : ExpressionSyntaxElement {
        public readonly ExpressionSyntaxElement[] Expressions;
        public MultiExpressionSyntaxElement(LexicalElement lexAtStart, IList<ExpressionSyntaxElement> expressions)
            : base(lexAtStart) {
            this.Expressions = expressions.ToArray();
        }
        protected override ScriptVariable CalculateInternal(ScriptExecutionEnvironment env) {
            ScriptVariable returnValue = null;
            foreach(var expression in this.Expressions) {
                returnValue = expression.Calculate(env);
            }
            return returnValue;
        }
        public override void Serialize(TextWriter writer) {
            bool first = true;
            foreach(var expression in this.Expressions) {
                if(first)
                    first = false;
                else
                    writer.Write(", ");
                expression.Serialize(writer);
            }
        }
    }
    public class ArgumentsSyntaxElement : List<ExpressionSyntaxElement> {
        public ArgumentsSyntaxElement()
            : base() {
        }
        public ArgumentsSyntaxElement(IEnumerable<ExpressionSyntaxElement> collection)
            : base(collection) {
        }
    }
    class DotInvocationSyntaxElement {
        public LexicalElement Identifier;
        public ArgumentsSyntaxElement Arguments;
        public DotInvocationSyntaxElement(LexicalElement identifier, ArgumentsSyntaxElement arguments) {
            this.Identifier = identifier;
            this.Arguments = arguments;
        }
    }
    /// <summary>
    /// 式の括弧括りを示す構文要素
    /// </summary>
    public class ParenthesisSyntaxElement : ExpressionSyntaxElement {
        public readonly ExpressionSyntaxElement Expression;
        public ParenthesisSyntaxElement(LexicalElement lexAtStart, ExpressionSyntaxElement expression)
            : base(lexAtStart) {
            this.Expression = expression;
        }

        protected override ScriptVariable CalculateInternal(ScriptExecutionEnvironment env) {
            return this.Expression.Calculate(env);
        }

        public override void Serialize(TextWriter writer) {
            writer.Write("(");
            this.Expression.Serialize(writer);
            writer.Write(")");
        }
    }
}
