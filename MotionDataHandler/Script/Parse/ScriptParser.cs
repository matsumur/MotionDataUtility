using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
namespace MotionDataHandler.Script.Parse {
    /// <summary>
    /// スクリプトを字句解析・構文解析して実行するためのクラス
    /// </summary>
    internal class ScriptParser : ScriptParserBase {
        readonly LexParser _lexParser = new LexParser();
        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public ScriptParser() : base() { }
        /// <summary>
        /// 文字列を識別子として有効な文字列に変換する
        /// </summary>
        /// <param name="string"></param>
        /// <returns></returns>
        public static string ConvertStringIntoIdentifier(string @string) {
            if(string.IsNullOrEmpty(@string))
                @string = "_";
            if("0123456789".Contains(@string[0]))
                @string = "_" + @string;
            StringBuilder ret = new StringBuilder();
            char prev = @string[0];
            foreach(char c in @string) {
                // 半角記号と改行と全角・半角空白を_に変換
                if(!(@"""#$%&'()=^~\|@`[{;+:*]},<.>/? 　" + "\t\f\n\r").Contains(c)) {
                    ret.Append(c);
                    prev = c;
                } else {
                    if(prev != '_') {
                        ret.Append("_");
                    }
                    prev = '_';
                }
            }
            if(ret.Length == 0)
                ret.Append("_");
            return ret.ToString();
        }

        /// <summary>
        /// 字句解析をして結果を文字列に戻します
        /// </summary>
        /// <param name="reader">スクリプトの読み込み</param>
        /// <returns></returns>
        public string LexTest(TextReader reader) {
            try {
                List<LexicalElement> tmp = _lexParser.ParseText(reader).ToList();
                StringBuilder ret = new StringBuilder();
                foreach(var elem in tmp) {
                    ret.AppendLine(string.Format("{0}: {1}", elem.Text, elem.Type));
                }
                return ret.ToString();
            } catch(ParseException ex) {
                return string.Format("{0}\r\ncolumn {1} at line{2}: {3}", ex.Message, ex.Column, ex.Line, ex.ErrorText);
            }
        }
        /// <summary>
        /// 構文解析をして結果を文字列に戻します
        /// </summary>
        /// <param name="reader">スクリプトの読み込み</param>
        /// <returns></returns>
        public string ParseTest(TextReader reader) {
            try {
                List<LexicalElement> tmp = _lexParser.ParseText(reader).ToList();
                IList<SyntaxElement> ret = this.StartParse(tmp);
                StringWriter writer = new StringWriter();
                foreach(var syn in ret) {
                    syn.Serialize(writer);
                }
                return writer.ToString();
            } catch(ParseException ex) {
                return string.Format("{0}\r\nColumn {1} at Line {2}: {3}", ex.Message, ex.Column, ex.Line, ex.ErrorText);
            }
        }
        /// <summary>
        /// スクリプトを実行します
        /// </summary>
        /// <param name="reader">スクリプトの読み込み</param>
        /// <param name="env">実行環境</param>
        /// <returns></returns>
        public string Execute(TextReader reader, ScriptExecutionEnvironment env) {
            try {
                List<LexicalElement> tmp = _lexParser.ParseText(reader).ToList();
                IList<SyntaxElement> ret = this.StartParse(tmp);
                foreach(var syn in ret) {
                    ScriptVariable p;
                    syn.Execute(env, out p);
                }
                return "";
            } catch(ParseException ex) {
                return string.Format("{0}\r\nColumn {1} at Line {2}: {3}", ex.Message, ex.Column, ex.Line, ex.ErrorText);
            }
        }




        protected override ProgramSyntaxElement ReturnProgram(SyntaxElement[] repeatOfStatement) {
            return new ProgramSyntaxElement(repeatOfStatement);
        }

        protected override SyntaxElement ReturnBlock(LexicalElement openBraces, SyntaxElement[] repeatOfStatement, LexicalElement closeBraces) {
            return new BlockSyntaxElement(openBraces, repeatOfStatement, true);
        }

        protected override SyntaxElement ReturnStatement(ForeachSyntaxElement @foreach) {
            return @foreach;
        }

        protected override SyntaxElement ReturnStatement(ForSyntaxElement @for) {
            return @for;
        }

        protected override SyntaxElement ReturnStatement_While(DoWhileSyntaxElement @while) {
            return @while;
        }

        protected override SyntaxElement ReturnStatement_Do(DoWhileSyntaxElement @do) {
            return @do;
        }

        protected override SyntaxElement ReturnStatement(IfSyntaxElement @if) {
            return @if;
        }

        protected override SyntaxElement ReturnStatement(SyntaxElement block) {
            return block;
        }

        protected override SyntaxElement ReturnStatement(ControlSyntaxElement control, LexicalElement semicolon) {
            return control;
        }

        protected override SyntaxElement ReturnStatement(Optional<MultiExpressionSyntaxElement> optOfMultiExpression, LexicalElement semicolon) {
            if(optOfMultiExpression.HasValue) {
                return new TerminatedExpressionSyntaxElement(optOfMultiExpression.Value.LexAtStart, optOfMultiExpression.Value);
            }
            return new TerminatedExpressionSyntaxElement(semicolon);
        }

        protected override SyntaxElement ReturnStatement(MultiDeclareSyntaxElement declare, LexicalElement semicolon) {
            return new TerminatedExpressionSyntaxElement(declare.LexAtStart, declare);
        }


        /// <summary>
        /// for = 'for', '(', [expression|declare], ';', [expression], ';', [expression], ')', statement ;
        /// </summary>
        /// <param name="@for">'for'</param>
        /// <param name="openPar">'('</param>
        /// <param name="selection_opt">[expression|declare]</param>
        /// <param name="semicolon_1">';'</param>
        /// <param name="expression_opt_1">[expression]</param>
        /// <param name="semicolon_2">';'</param>
        /// <param name="expression_opt_2">[expression]</param>
        /// <param name="closePar">')'</param>
        /// <param name="statement">statement</param>
        protected override ForSyntaxElement ReturnFor(LexicalElement @for, LexicalElement openPar, Optional<Selection<ExpressionSyntaxElement, MultiDeclareSyntaxElement>> selection_opt, LexicalElement semicolon_1, Optional<ExpressionSyntaxElement> expression_opt_1, LexicalElement semicolon_2, Optional<ExpressionSyntaxElement> expression_opt_2, LexicalElement closePar, SyntaxElement statement) {
            ExpressionSyntaxElement init = null;
            if(selection_opt.HasValue) {
                if(selection_opt.Value.Element1.HasValue) {
                    init = selection_opt.Value.Element1.Value;
                }
                if(selection_opt.Value.Element2.HasValue) {
                    init = selection_opt.Value.Element2.Value;
                }
            }
            ExpressionSyntaxElement cond = null;
            if(expression_opt_1.HasValue) {
                cond = expression_opt_1.Value;
            }
            ExpressionSyntaxElement cont = null;
            if(expression_opt_2.HasValue) {
                cont = expression_opt_2.Value;
            }
            return new ForSyntaxElement(@for, init, cond, cont, statement);
        }

        /// <summary>
        /// while = 'while', '(', expression, ')', statement ;
        /// </summary>
        /// <param name="@while">'while'</param>
        /// <param name="openPar">'('</param>
        /// <param name="expression">expression</param>
        /// <param name="closePar">')'</param>
        /// <param name="statement">statement</param>
        protected override DoWhileSyntaxElement ReturnWhile(LexicalElement @while, LexicalElement openPar, ExpressionSyntaxElement expression, LexicalElement closePar, SyntaxElement statement) {
            return new DoWhileSyntaxElement(@while, expression, statement, false);
        }

        /// <summary>
        /// do = 'do', statement, 'while', '(', expression, ')', ';' ;
        /// </summary>
        /// <param name="@do">'do'</param>
        /// <param name="statement">statement</param>
        /// <param name="@while">'while'</param>
        /// <param name="openPar">'('</param>
        /// <param name="expression">expression</param>
        /// <param name="closePar">')'</param>
        /// <param name="semicolon">';'</param>
        protected override DoWhileSyntaxElement ReturnDo(LexicalElement @do, SyntaxElement statement, LexicalElement @while, LexicalElement openPar, ExpressionSyntaxElement expression, LexicalElement closePar, LexicalElement semicolon) {
            return new DoWhileSyntaxElement(@do, expression, statement, true);
        }

        /// <summary>
        /// if = 'if', '(', expression, ')', statement, ['else', statement] ;
        /// </summary>
        /// <param name="@if">'if'</param>
        /// <param name="openPar">'('</param>
        /// <param name="expression">expression</param>
        /// <param name="closePar">')'</param>
        /// <param name="statement">statement</param>
        /// <param name="else_Statement_opt">['else', statement]</param>
        protected override IfSyntaxElement ReturnIf(LexicalElement @if, LexicalElement openPar, ExpressionSyntaxElement expression, LexicalElement closePar, SyntaxElement statement, Optional<FixedList<LexicalElement, SyntaxElement>> else_Statement_opt) {
            if(else_Statement_opt.HasValue) {
                return new IfSyntaxElement(@if, expression, statement, else_Statement_opt.Value.Element2);
            }
            return new IfSyntaxElement(@if, expression, statement);
        }


        protected override ControlSyntaxElement ReturnControl(LexicalElement @return, Optional<ExpressionSyntaxElement> optOfExpression) {
            if(optOfExpression.HasValue) {
                return new ControlSyntaxElement(@return, optOfExpression.Value);
            }
            return new ControlSyntaxElement(@return, RunControlType.Return);
        }

        protected override MultiExpressionSyntaxElement ReturnMultiExpression(ExpressionSyntaxElement expression, FixedList<LexicalElement, ExpressionSyntaxElement>[] repeatOfCommaAndExpression) {
            List<ExpressionSyntaxElement> expressions = new List<ExpressionSyntaxElement>();
            expressions.Add(expression);
            foreach(var pair in repeatOfCommaAndExpression) {
                expressions.Add(pair.Element2);
            }
            return new MultiExpressionSyntaxElement(expression.LexAtStart, expressions);
        }

        protected override MultiDeclareSyntaxElement ReturnDeclare(Selection<FixedList<LexicalElement, Optional<LexicalElement>>, LexicalElement> readonlyAnd_OptOfVar_OrVar, IdentifierSyntaxElement identifier, Optional<FixedList<LexicalElement, ExpressionSyntaxElement>> optOfEqualAndExpression, FixedList<LexicalElement, IdentifierSyntaxElement, Optional<FixedList<LexicalElement, ExpressionSyntaxElement>>>[] repeatOfCommaAndIdentifierAnd_OptOfEqualAndExpression) {
            LexicalElement head = new LexicalElement();
            VariableStorage.FieldProperty property = new VariableStorage.FieldProperty();
            if(readonlyAnd_OptOfVar_OrVar.Element1.HasValue) {
                head = readonlyAnd_OptOfVar_OrVar.Element1.Value.Element1;
                property.Readonly = true;
            } else {
                head = readonlyAnd_OptOfVar_OrVar.Element2.Value;
            }
            List<DeclareSyntaxElement> varList = new List<DeclareSyntaxElement>();
            if(optOfEqualAndExpression.HasValue) {
                DeclareSyntaxElement var1 = new DeclareSyntaxElement(identifier.LexAtStart, identifier.Identifier, property, optOfEqualAndExpression.Value.Element2);
                varList.Add(var1);
            } else {
                DeclareSyntaxElement var1 = new DeclareSyntaxElement(identifier.LexAtStart, identifier.Identifier, property);
                varList.Add(var1);
            }
            foreach(var declare in repeatOfCommaAndIdentifierAnd_OptOfEqualAndExpression) {
                DeclareSyntaxElement var2;
                IdentifierSyntaxElement identifier2 = declare.Element2;
                if(declare.Element3.HasValue) {
                    ExpressionSyntaxElement expression2 = declare.Element3.Value.Element2;
                    var2 = new DeclareSyntaxElement(identifier2.LexAtStart, identifier2.Identifier, property, expression2);
                } else {
                    var2 = new DeclareSyntaxElement(identifier2.LexAtStart, identifier2.Identifier, property);
                }
                varList.Add(var2);
            }
            return new MultiDeclareSyntaxElement(head, property, varList);
        }


        protected override ExpressionSyntaxElement ReturnOr(ExpressionSyntaxElement and, FixedList<LexicalElement, ExpressionSyntaxElement>[] repeatOfOrAndAnd) {
            ExpressionSyntaxElement left = and;
            foreach(var right in repeatOfOrAndAnd) {
                left = new BinarySyntaxElement(left.LexAtStart, BinarySyntaxElement.OperatorType.Or, left, right.Element2);
            }
            return left;
        }

        protected override ExpressionSyntaxElement ReturnAnd(ExpressionSyntaxElement cmp, FixedList<LexicalElement, ExpressionSyntaxElement>[] repeatOfAndAndCmp) {
            ExpressionSyntaxElement left = cmp;
            foreach(var right in repeatOfAndAndCmp) {
                left = new BinarySyntaxElement(left.LexAtStart, BinarySyntaxElement.OperatorType.And, left, right.Element2);
            }
            return left;
        }

        protected override ExpressionSyntaxElement ReturnCmp(ExpressionSyntaxElement add, Optional<FixedList<LexicalElement, ExpressionSyntaxElement>> optOf_Le_AndAdd) {
            ExpressionSyntaxElement left = add;
            if(optOf_Le_AndAdd.HasValue) {
                BinarySyntaxElement.OperatorType op = BinarySyntaxElement.OperatorType.Eq;
                switch(optOf_Le_AndAdd.Value.Element1.Type) {
                case LexType.Eq:
                    op = BinarySyntaxElement.OperatorType.Eq;
                    break;
                case LexType.Ne:
                    op = BinarySyntaxElement.OperatorType.Ne;
                    break;
                case LexType.Gt:
                    op = BinarySyntaxElement.OperatorType.Gt;
                    break;
                case LexType.Ge:
                    op = BinarySyntaxElement.OperatorType.Ge;
                    break;
                case LexType.Lt:
                    op = BinarySyntaxElement.OperatorType.Lt;
                    break;
                case LexType.Le:
                    op = BinarySyntaxElement.OperatorType.Le;
                    break;
                default:
                    throw new NotSupportedException();
                }
                left = new BinarySyntaxElement(left.LexAtStart, op, left, optOf_Le_AndAdd.Value.Element2);
            }
            return left;
        }

        protected override ExpressionSyntaxElement ReturnAdd(ExpressionSyntaxElement mul, FixedList<LexicalElement, ExpressionSyntaxElement>[] repeatOf_Plus_AndMul) {
            ExpressionSyntaxElement left = mul;
            foreach(var right in repeatOf_Plus_AndMul) {
                BinarySyntaxElement.OperatorType op = BinarySyntaxElement.OperatorType.Plus;
                switch(right.Element1.Type) {
                case LexType.Plus:
                    op = BinarySyntaxElement.OperatorType.Plus;
                    break;
                case LexType.Minus:
                    op = BinarySyntaxElement.OperatorType.Minus;
                    break;
                default:
                    throw new NotSupportedException();
                }
                left = new BinarySyntaxElement(left.LexAtStart, op, left, right.Element2);
            }
            return left;
        }

        protected override ExpressionSyntaxElement ReturnMul(ExpressionSyntaxElement anary, FixedList<LexicalElement, ExpressionSyntaxElement>[] repeatOf_Cross_AndAnary) {
            ExpressionSyntaxElement left = anary;
            foreach(var right in repeatOf_Cross_AndAnary) {
                BinarySyntaxElement.OperatorType op = BinarySyntaxElement.OperatorType.Cross;
                switch(right.Element1.Type) {
                case LexType.Cross:
                    op = BinarySyntaxElement.OperatorType.Cross;
                    break;
                case LexType.Slash:
                    op = BinarySyntaxElement.OperatorType.Slash;
                    break;
                case LexType.Percent:
                    op = BinarySyntaxElement.OperatorType.Percent;
                    break;
                default:
                    throw new NotSupportedException();
                }
                left = new BinarySyntaxElement(left.LexAtStart, op, left, right.Element2);
            }
            return left;
        }

        protected override ExpressionSyntaxElement ReturnUnary(LexicalElement[] repeatOfNot, ExpressionSyntaxElement inc) {
            ExpressionSyntaxElement right = inc;
            foreach(LexicalElement left in repeatOfNot.Reverse()) {
                AnarySyntaxElement.OperatorType op = AnarySyntaxElement.OperatorType.Plus;
                switch(left.Type) {
                case LexType.Plus:
                    op = AnarySyntaxElement.OperatorType.Plus;
                    break;
                case LexType.Minus:
                    op = AnarySyntaxElement.OperatorType.Minus;
                    break;
                case LexType.Not:
                    op = AnarySyntaxElement.OperatorType.Not;
                    break;
                default:
                    throw new NotSupportedException();
                }
                right = new AnarySyntaxElement(left, op, right);
            }
            return right;
        }

        protected override ExpressionSyntaxElement ReturnInc(ExpressionSyntaxElement modProperty) {
            return modProperty;
        }

        protected override ExpressionSyntaxElement ReturnInc(LexicalElement plusPlus, ExpressionSyntaxElement modProperty) {
            IncrementSyntaxElement.OperatorType op = IncrementSyntaxElement.OperatorType.PostDec;
            switch(plusPlus.Type) {
            case LexType.PlusPlus:
                op = IncrementSyntaxElement.OperatorType.PreInc;
                break;
            case LexType.MinusMinus:
                op = IncrementSyntaxElement.OperatorType.PreDec;
                break;
            default:
                throw new NotSupportedException();
            }
            AccessorSyntaxElement accessor = modProperty as AccessorSyntaxElement;
            if(accessor == null) {
                throw new ParseException("Only identifier or indexer can be left value", plusPlus);
            }
            return new IncrementSyntaxElement(plusPlus, accessor, op);
        }

        protected override ExpressionSyntaxElement ReturnInc(ExpressionSyntaxElement modProperty, LexicalElement plusPlus) {
            IncrementSyntaxElement.OperatorType op = IncrementSyntaxElement.OperatorType.PostDec;
            switch(plusPlus.Type) {
            case LexType.PlusPlus:
                op = IncrementSyntaxElement.OperatorType.PostInc;
                break;
            case LexType.MinusMinus:
                op = IncrementSyntaxElement.OperatorType.PostDec;
                break;
            default:
                throw new NotSupportedException();
            }
            AccessorSyntaxElement accessor = modProperty as AccessorSyntaxElement;
            if(accessor == null) {
                throw new ParseException("Only identifier or indexer can be left value", modProperty.LexAtStart);
            }
            return new IncrementSyntaxElement(plusPlus, accessor, op);
        }

        protected override ExpressionSyntaxElement ReturnModProperty(ExpressionSyntaxElement property, Selection<ArgumentsSyntaxElement, ExpressionSyntaxElement, DotInvocationSyntaxElement>[] repeatOfArgumentsOrIndexingOrDotInvocation) {
            ExpressionSyntaxElement left = property;
            foreach(var selection in repeatOfArgumentsOrIndexingOrDotInvocation) {
                if(selection.Element1.HasValue) {
                    ArgumentsSyntaxElement args = selection.Element1.Value;
                    left = new InvokeSyntaxElement(left.LexAtStart, left, args);
                } else if(selection.Element2.HasValue) {
                    ExpressionSyntaxElement index = selection.Element2.Value;
                    left = new IndexerSyntaxElement(left.LexAtStart, left, index);
                } else if(selection.Element3.HasValue) {
                    DotInvocationSyntaxElement right = selection.Element3.Value;
                    List<ExpressionSyntaxElement> expressions = new List<ExpressionSyntaxElement>();
                    expressions.Add(left);
                    expressions.AddRange(right.Arguments);
                    left = new InvokeSyntaxElement(left.LexAtStart, new IdentifierSyntaxElement(right.Identifier), expressions);
                } else {
                    throw new NotSupportedException();
                }
            }
            return left;
        }


        protected override DotInvocationSyntaxElement ReturnDotInvocation(LexicalElement dot, IdentifierSyntaxElement identifier, ArgumentsSyntaxElement arguments) {
            return new DotInvocationSyntaxElement(identifier.LexAtStart, arguments);
        }

        protected override ArgumentsSyntaxElement ReturnArguments(LexicalElement openPar, Optional<FixedList<ExpressionSyntaxElement, FixedList<LexicalElement, ExpressionSyntaxElement>[]>> optOfExpressionAnd_RepeatOfCommaAndExpression, LexicalElement closePar) {
            List<ExpressionSyntaxElement> expressions = new List<ExpressionSyntaxElement>();
            if(optOfExpressionAnd_RepeatOfCommaAndExpression.HasValue) {
                expressions.Add(optOfExpressionAnd_RepeatOfCommaAndExpression.Value.Element1);
                foreach(var successor in optOfExpressionAnd_RepeatOfCommaAndExpression.Value.Element2) {
                    expressions.Add(successor.Element2);
                }
            }
            return new ArgumentsSyntaxElement(expressions);
        }


        protected override ExpressionSyntaxElement ReturnProperty(IdentifierSyntaxElement identifier) {
            return identifier;
        }

        protected override ExpressionSyntaxElement ReturnProperty(ListSyntaxElement list) {
            return list;
        }

        protected override ExpressionSyntaxElement ReturnProperty(ExpressionSyntaxElement func) {
            return func;
        }

        protected override IdentifierSyntaxElement ReturnIdentifier(LexicalElement identifier) {
            return new IdentifierSyntaxElement(identifier, identifier.Text);
        }

        protected override ListSyntaxElement ReturnList(LexicalElement openBraces, Optional<FixedList<ExpressionSyntaxElement, FixedList<LexicalElement, ExpressionSyntaxElement>[]>> optOfExpressionAnd_RepeatOfCommaAndExpression, LexicalElement closeBraces) {
            List<ExpressionSyntaxElement> expressions = new List<ExpressionSyntaxElement>();
            if(optOfExpressionAnd_RepeatOfCommaAndExpression.HasValue) {
                expressions.Add(optOfExpressionAnd_RepeatOfCommaAndExpression.Value.Element1);
                foreach(var list in optOfExpressionAnd_RepeatOfCommaAndExpression.Value.Element2) {
                    expressions.Add(list.Element2);
                }
            }
            return new ListSyntaxElement(openBraces, expressions);
        }


        protected override ForeachSyntaxElement ReturnForeach(LexicalElement @foreach, LexicalElement openPar, Optional<LexicalElement> optOfVar, IdentifierSyntaxElement identifier, LexicalElement @in, ExpressionSyntaxElement expression, LexicalElement closePar, SyntaxElement statement) {
            return new ForeachSyntaxElement(@foreach, identifier.Identifier, optOfVar.HasValue, expression, statement);
        }


        protected override ControlSyntaxElement ReturnControl_Break(LexicalElement @break) {
            return new ControlSyntaxElement(@break, RunControlType.Break);
        }

        protected override ControlSyntaxElement ReturnControl_Continue(LexicalElement @continue) {
            return new ControlSyntaxElement(@continue, RunControlType.Continue);
        }

        protected override ExpressionSyntaxElement ReturnProperty_Number(LexicalElement number) {
            decimal value;
            if(decimal.TryParse(number.Text, out value)) {
                return new NumberSyntaxElement(number, value);
            }
            return new NumberSyntaxElement(number, 0M);
        }

        protected override ExpressionSyntaxElement ReturnProperty_String(LexicalElement @string) {
            return new StringSyntaxElement(@string, @string.Text);
        }

        protected override ExpressionSyntaxElement ReturnProperty_True(LexicalElement @true) {
            return new BooleanSyntaxElement(@true, true);
        }

        protected override ExpressionSyntaxElement ReturnProperty_False(LexicalElement @false) {
            return new BooleanSyntaxElement(@false, false);
        }

        protected override ExpressionSyntaxElement ReturnProperty_Null(LexicalElement @null) {
            return new NullSyntaxElement(@null);
        }

        protected override ExpressionSyntaxElement ReturnExpression(ExpressionSyntaxElement substitute) {
            return substitute;
        }

        protected override ExpressionSyntaxElement ReturnSubstitute(ExpressionSyntaxElement leftValue, Optional<FixedList<LexicalElement, ExpressionSyntaxElement>> optOf_PlusEqual_AndSubstitute) {
            if(optOf_PlusEqual_AndSubstitute.HasValue) {
                SubstituteSyntaxElement.OperatorType op = SubstituteSyntaxElement.OperatorType.Substitute;
                switch(optOf_PlusEqual_AndSubstitute.Value.Element1.Type) {
                case LexType.Equal:
                    op = SubstituteSyntaxElement.OperatorType.Substitute;
                    break;
                case LexType.PlusEqual:
                    op = SubstituteSyntaxElement.OperatorType.Add;
                    break;
                case LexType.MinusEqual:
                    op = SubstituteSyntaxElement.OperatorType.Subtract;
                    break;
                case LexType.CrossEqual:
                    op = SubstituteSyntaxElement.OperatorType.Multiply;
                    break;
                case LexType.SlashEqual:
                    op = SubstituteSyntaxElement.OperatorType.Divide;
                    break;
                case LexType.PercentEqual:
                    op = SubstituteSyntaxElement.OperatorType.Remainder;
                    break;
                }
                AccessorSyntaxElement accessor = leftValue as AccessorSyntaxElement;
                if(accessor == null) {
                    throw new ParseException("Only identifier or indexer can be left value", leftValue.LexAtStart);
                }
                leftValue = new SubstituteSyntaxElement(leftValue.LexAtStart, accessor, op, optOf_PlusEqual_AndSubstitute.Value.Element2);
            }
            return leftValue;
        }

        protected override ExpressionSyntaxElement ReturnLeftValue(ExpressionSyntaxElement ternary) {
            return ternary;
        }

        protected override ExpressionSyntaxElement ReturnTernary(ExpressionSyntaxElement or, Optional<FixedList<LexicalElement, ExpressionSyntaxElement, LexicalElement, ExpressionSyntaxElement>> optOfQuestionAndTernaryAndColonAndTernary) {
            ExpressionSyntaxElement left = or;
            if(optOfQuestionAndTernaryAndColonAndTernary.HasValue) {
                ExpressionSyntaxElement atTrue = optOfQuestionAndTernaryAndColonAndTernary.Value.Element2;
                ExpressionSyntaxElement atFalse = optOfQuestionAndTernaryAndColonAndTernary.Value.Element4;
                left = new TernarySyntaxElement(left.LexAtStart, TernarySyntaxElement.OperatorType.Condition, left, atTrue, atFalse);
            }
            return left;
        }


        protected override ExpressionSyntaxElement ReturnIndexing(LexicalElement openBracket, ExpressionSyntaxElement expression, LexicalElement closeBracket) {
            return expression;
        }

        protected override ExpressionSyntaxElement ReturnProperty(ParenthesisSyntaxElement parenthesis) {
            return parenthesis;
        }

        protected override ParenthesisSyntaxElement ReturnParenthesis(LexicalElement openPar, MultiExpressionSyntaxElement multiExpression, LexicalElement closePar) {
            return new ParenthesisSyntaxElement(openPar, multiExpression);
        }

        protected override ExpressionSyntaxElement ReturnFunc(LexicalElement func, LexicalElement openPar, Optional<FixedList<Optional<LexicalElement>, Optional<LexicalElement>, IdentifierSyntaxElement, FixedList<LexicalElement, Optional<LexicalElement>, Optional<LexicalElement>, IdentifierSyntaxElement>[]>> optOf_OptOfParams_And_OptOfVar_AndIdentifierAnd_RepeatOfCommaAnd_OptOfParams_And_OptOfVar_AndIdentifier, LexicalElement closePar, SyntaxElement block) {
            List<IdentifierSyntaxElement> @params = new List<IdentifierSyntaxElement>();
            bool isParam = false;
            if(optOf_OptOfParams_And_OptOfVar_AndIdentifierAnd_RepeatOfCommaAnd_OptOfParams_And_OptOfVar_AndIdentifier.HasValue) {
                var tmp = optOf_OptOfParams_And_OptOfVar_AndIdentifierAnd_RepeatOfCommaAnd_OptOfParams_And_OptOfVar_AndIdentifier.Value;
                var param = tmp.Element1;
                var var = tmp.Element2;
                var identifier = tmp.Element3;
                var repeat = tmp.Element4;
                if(optOf_OptOfParams_And_OptOfVar_AndIdentifierAnd_RepeatOfCommaAnd_OptOfParams_And_OptOfVar_AndIdentifier.Value.Element1.HasValue) {
                    isParam = true;
                }
                @params.Add(identifier);
                foreach(var successor in repeat) {
                    var comma = successor.Element1;
                    var param2 = successor.Element2;
                    var var2 = successor.Element3;
                    var identifier2 = successor.Element4;
                    @params.Add(identifier2);
                }
            }
            return new FunctionSyntaxElement(func, @params, block, isParam);
        }
    }

    /// <summary>
    /// 実行制御用の戻り値
    /// </summary>
    public enum RunControlType {
        /// <summary>
        /// 通常通り
        /// </summary>
        None,
        Continue,
        Break,
        Return,
    }
}
