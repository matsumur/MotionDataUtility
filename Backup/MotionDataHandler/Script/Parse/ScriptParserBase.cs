// Auto generated with LLParserGenerator.ScriptParserGenerator
using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MotionDataHandler.Script.Parse {
    abstract class ScriptParserBase {
        protected ScriptParserBase() { }
        public ProgramSyntaxElement StartParse(IEnumerable<LexicalElement> source) {
            _Reader = new Reader<LexicalElement, LexType>(source);
            return this.parseProgram();
        }
        protected Reader<LexicalElement, LexType> _Reader;
        /// <summary>
        /// program = {statement} ;
        /// </summary>
        /// <param name="repetition">{statement}</param>
        protected abstract ProgramSyntaxElement ReturnProgram(SyntaxElement[] repetition);

        /// <summary>
        /// statement = foreach ;
        /// </summary>
        /// <param name="@foreach">foreach</param>
        protected abstract SyntaxElement ReturnStatement(ForeachSyntaxElement @foreach);

        /// <summary>
        /// statement = for ;
        /// </summary>
        /// <param name="@for">for</param>
        protected abstract SyntaxElement ReturnStatement(ForSyntaxElement @for);

        /// <summary>
        /// statement = while ;
        /// </summary>
        /// <param name="@while">while</param>
        protected abstract SyntaxElement ReturnStatement_While(DoWhileSyntaxElement @while);

        /// <summary>
        /// statement = do ;
        /// </summary>
        /// <param name="@do">do</param>
        protected abstract SyntaxElement ReturnStatement_Do(DoWhileSyntaxElement @do);

        /// <summary>
        /// statement = if ;
        /// </summary>
        /// <param name="@if">if</param>
        protected abstract SyntaxElement ReturnStatement(IfSyntaxElement @if);

        /// <summary>
        /// statement = block ;
        /// </summary>
        /// <param name="block">block</param>
        protected abstract SyntaxElement ReturnStatement(SyntaxElement block);

        /// <summary>
        /// statement = control, ';' ;
        /// </summary>
        /// <param name="control">control</param>
        /// <param name="semicolon">';'</param>
        protected abstract SyntaxElement ReturnStatement(ControlSyntaxElement control, LexicalElement semicolon);

        /// <summary>
        /// statement = [multi_expression], ';' ;
        /// </summary>
        /// <param name="multiExpression_opt">[multi_expression]</param>
        /// <param name="semicolon">';'</param>
        protected abstract SyntaxElement ReturnStatement(Optional<MultiExpressionSyntaxElement> multiExpression_opt, LexicalElement semicolon);

        /// <summary>
        /// statement = declare, ';' ;
        /// </summary>
        /// <param name="declare">declare</param>
        /// <param name="semicolon">';'</param>
        protected abstract SyntaxElement ReturnStatement(MultiDeclareSyntaxElement declare, LexicalElement semicolon);

        /// <summary>
        /// foreach = 'foreach', '(', ['var'], identifier, 'in', expression, ')', statement ;
        /// </summary>
        /// <param name="@foreach">'foreach'</param>
        /// <param name="openPar">'('</param>
        /// <param name="var_opt">['var']</param>
        /// <param name="identifier">identifier</param>
        /// <param name="@in">'in'</param>
        /// <param name="expression">expression</param>
        /// <param name="closePar">')'</param>
        /// <param name="statement">statement</param>
        protected abstract ForeachSyntaxElement ReturnForeach(LexicalElement @foreach, LexicalElement openPar, Optional<LexicalElement> var_opt, IdentifierSyntaxElement identifier, LexicalElement @in, ExpressionSyntaxElement expression, LexicalElement closePar, SyntaxElement statement);

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
        protected abstract ForSyntaxElement ReturnFor(LexicalElement @for, LexicalElement openPar, Optional<Selection<ExpressionSyntaxElement, MultiDeclareSyntaxElement>> selection_opt, LexicalElement semicolon_1, Optional<ExpressionSyntaxElement> expression_opt_1, LexicalElement semicolon_2, Optional<ExpressionSyntaxElement> expression_opt_2, LexicalElement closePar, SyntaxElement statement);

        /// <summary>
        /// while = 'while', '(', expression, ')', statement ;
        /// </summary>
        /// <param name="@while">'while'</param>
        /// <param name="openPar">'('</param>
        /// <param name="expression">expression</param>
        /// <param name="closePar">')'</param>
        /// <param name="statement">statement</param>
        protected abstract DoWhileSyntaxElement ReturnWhile(LexicalElement @while, LexicalElement openPar, ExpressionSyntaxElement expression, LexicalElement closePar, SyntaxElement statement);

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
        protected abstract DoWhileSyntaxElement ReturnDo(LexicalElement @do, SyntaxElement statement, LexicalElement @while, LexicalElement openPar, ExpressionSyntaxElement expression, LexicalElement closePar, LexicalElement semicolon);

        /// <summary>
        /// if = 'if', '(', expression, ')', statement, ['else', statement] ;
        /// </summary>
        /// <param name="@if">'if'</param>
        /// <param name="openPar">'('</param>
        /// <param name="expression">expression</param>
        /// <param name="closePar">')'</param>
        /// <param name="statement">statement</param>
        /// <param name="else_Statement_opt">['else', statement]</param>
        protected abstract IfSyntaxElement ReturnIf(LexicalElement @if, LexicalElement openPar, ExpressionSyntaxElement expression, LexicalElement closePar, SyntaxElement statement, Optional<FixedList<LexicalElement, SyntaxElement>> else_Statement_opt);

        /// <summary>
        /// block = '{', {statement}, '}' ;
        /// </summary>
        /// <param name="openBraces">'{'</param>
        /// <param name="repetition">{statement}</param>
        /// <param name="closeBraces">'}'</param>
        protected abstract SyntaxElement ReturnBlock(LexicalElement openBraces, SyntaxElement[] repetition, LexicalElement closeBraces);

        /// <summary>
        /// control = 'break' ;
        /// </summary>
        /// <param name="@break">'break'</param>
        protected abstract ControlSyntaxElement ReturnControl_Break(LexicalElement @break);

        /// <summary>
        /// control = 'continue' ;
        /// </summary>
        /// <param name="@continue">'continue'</param>
        protected abstract ControlSyntaxElement ReturnControl_Continue(LexicalElement @continue);

        /// <summary>
        /// control = 'return', [expression] ;
        /// </summary>
        /// <param name="@return">'return'</param>
        /// <param name="expression_opt">[expression]</param>
        protected abstract ControlSyntaxElement ReturnControl(LexicalElement @return, Optional<ExpressionSyntaxElement> expression_opt);

        /// <summary>
        /// multi_expression = expression, {',', expression} ;
        /// </summary>
        /// <param name="expression">expression</param>
        /// <param name="repetition">{',', expression}</param>
        protected abstract MultiExpressionSyntaxElement ReturnMultiExpression(ExpressionSyntaxElement expression, FixedList<LexicalElement, ExpressionSyntaxElement>[] repetition);

        /// <summary>
        /// declare = ('readonly', ['var']|'var'), identifier, ['=', expression], {',', identifier, ['=', expression]} ;
        /// </summary>
        /// <param name="selection">('readonly', ['var']|'var')</param>
        /// <param name="identifier">identifier</param>
        /// <param name="equal_Expression_opt">['=', expression]</param>
        /// <param name="repetition">{',', identifier, ['=', expression]}</param>
        protected abstract MultiDeclareSyntaxElement ReturnDeclare(Selection<FixedList<LexicalElement, Optional<LexicalElement>>, LexicalElement> selection, IdentifierSyntaxElement identifier, Optional<FixedList<LexicalElement, ExpressionSyntaxElement>> equal_Expression_opt, FixedList<LexicalElement, IdentifierSyntaxElement, Optional<FixedList<LexicalElement, ExpressionSyntaxElement>>>[] repetition);

        /// <summary>
        /// expression = substitute ;
        /// </summary>
        /// <param name="substitute">substitute</param>
        protected abstract ExpressionSyntaxElement ReturnExpression(ExpressionSyntaxElement substitute);

        /// <summary>
        /// substitute = left_value, [('+='|'-='|'*='|'/='|'%='|'='), substitute] ;
        /// </summary>
        /// <param name="leftValue">left_value</param>
        /// <param name="plusEqual_Substitute_opt">[('+='|'-='|'*='|'/='|'%='|'='), substitute]</param>
        protected abstract ExpressionSyntaxElement ReturnSubstitute(ExpressionSyntaxElement leftValue, Optional<FixedList<LexicalElement, ExpressionSyntaxElement>> plusEqual_Substitute_opt);

        /// <summary>
        /// left_value = ternary ;
        /// </summary>
        /// <param name="ternary">ternary</param>
        protected abstract ExpressionSyntaxElement ReturnLeftValue(ExpressionSyntaxElement ternary);

        /// <summary>
        /// ternary = or, ['?', ternary, ':', ternary] ;
        /// </summary>
        /// <param name="or">or</param>
        /// <param name="question_Ternary_Colon_Ternary_opt">['?', ternary, ':', ternary]</param>
        protected abstract ExpressionSyntaxElement ReturnTernary(ExpressionSyntaxElement or, Optional<FixedList<LexicalElement, ExpressionSyntaxElement, LexicalElement, ExpressionSyntaxElement>> question_Ternary_Colon_Ternary_opt);

        /// <summary>
        /// or = and, {'||', and} ;
        /// </summary>
        /// <param name="and">and</param>
        /// <param name="repetition">{'||', and}</param>
        protected abstract ExpressionSyntaxElement ReturnOr(ExpressionSyntaxElement and, FixedList<LexicalElement, ExpressionSyntaxElement>[] repetition);

        /// <summary>
        /// and = cmp, {'&amp;&amp;', cmp} ;
        /// </summary>
        /// <param name="cmp">cmp</param>
        /// <param name="repetition">{'&amp;&amp;', cmp}</param>
        protected abstract ExpressionSyntaxElement ReturnAnd(ExpressionSyntaxElement cmp, FixedList<LexicalElement, ExpressionSyntaxElement>[] repetition);

        /// <summary>
        /// cmp = add, [('&lt;='|'&gt;='|'=='|'!='|'&lt;'|'&gt;'), add] ;
        /// </summary>
        /// <param name="add">add</param>
        /// <param name="le_Add_opt">[('&lt;='|'&gt;='|'=='|'!='|'&lt;'|'&gt;'), add]</param>
        protected abstract ExpressionSyntaxElement ReturnCmp(ExpressionSyntaxElement add, Optional<FixedList<LexicalElement, ExpressionSyntaxElement>> le_Add_opt);

        /// <summary>
        /// add = mul, {('+'|'-'), mul} ;
        /// </summary>
        /// <param name="mul">mul</param>
        /// <param name="repetition">{('+'|'-'), mul}</param>
        protected abstract ExpressionSyntaxElement ReturnAdd(ExpressionSyntaxElement mul, FixedList<LexicalElement, ExpressionSyntaxElement>[] repetition);

        /// <summary>
        /// mul = unary, {('*'|'/'|'%'), unary} ;
        /// </summary>
        /// <param name="unary">unary</param>
        /// <param name="repetition">{('*'|'/'|'%'), unary}</param>
        protected abstract ExpressionSyntaxElement ReturnMul(ExpressionSyntaxElement unary, FixedList<LexicalElement, ExpressionSyntaxElement>[] repetition);

        /// <summary>
        /// unary = {'!'|'+'|'-'}, inc ;
        /// </summary>
        /// <param name="repetition">{'!'|'+'|'-'}</param>
        /// <param name="inc">inc</param>
        protected abstract ExpressionSyntaxElement ReturnUnary(LexicalElement[] repetition, ExpressionSyntaxElement inc);

        /// <summary>
        /// inc = mod_property ;
        /// </summary>
        /// <param name="modProperty">mod_property</param>
        protected abstract ExpressionSyntaxElement ReturnInc(ExpressionSyntaxElement modProperty);

        /// <summary>
        /// inc = ('++'|'--'), mod_property ;
        /// </summary>
        /// <param name="plusPlus">('++'|'--')</param>
        /// <param name="modProperty">mod_property</param>
        protected abstract ExpressionSyntaxElement ReturnInc(LexicalElement plusPlus, ExpressionSyntaxElement modProperty);

        /// <summary>
        /// inc = mod_property, ('++'|'--') ;
        /// </summary>
        /// <param name="modProperty">mod_property</param>
        /// <param name="plusPlus">('++'|'--')</param>
        protected abstract ExpressionSyntaxElement ReturnInc(ExpressionSyntaxElement modProperty, LexicalElement plusPlus);

        /// <summary>
        /// mod_property = property, {arguments|indexing|dot_invocation} ;
        /// </summary>
        /// <param name="property">property</param>
        /// <param name="repetition">{arguments|indexing|dot_invocation}</param>
        protected abstract ExpressionSyntaxElement ReturnModProperty(ExpressionSyntaxElement property, Selection<ArgumentsSyntaxElement, ExpressionSyntaxElement, DotInvocationSyntaxElement>[] repetition);

        /// <summary>
        /// indexing = '[', expression, ']' ;
        /// </summary>
        /// <param name="openBracket">'['</param>
        /// <param name="expression">expression</param>
        /// <param name="closeBracket">']'</param>
        protected abstract ExpressionSyntaxElement ReturnIndexing(LexicalElement openBracket, ExpressionSyntaxElement expression, LexicalElement closeBracket);

        /// <summary>
        /// dot_invocation = '.', identifier, arguments ;
        /// </summary>
        /// <param name="dot">'.'</param>
        /// <param name="identifier">identifier</param>
        /// <param name="arguments">arguments</param>
        protected abstract DotInvocationSyntaxElement ReturnDotInvocation(LexicalElement dot, IdentifierSyntaxElement identifier, ArgumentsSyntaxElement arguments);

        /// <summary>
        /// arguments = '(', [expression, {',', expression}], ')' ;
        /// </summary>
        /// <param name="openPar">'('</param>
        /// <param name="expression_repetition_opt">[expression, {',', expression}]</param>
        /// <param name="closePar">')'</param>
        protected abstract ArgumentsSyntaxElement ReturnArguments(LexicalElement openPar, Optional<FixedList<ExpressionSyntaxElement, FixedList<LexicalElement, ExpressionSyntaxElement>[]>> expression_repetition_opt, LexicalElement closePar);

        /// <summary>
        /// property = "Number" ;
        /// </summary>
        /// <param name="number">"Number"</param>
        protected abstract ExpressionSyntaxElement ReturnProperty_Number(LexicalElement number);

        /// <summary>
        /// property = "String" ;
        /// </summary>
        /// <param name="@string">"String"</param>
        protected abstract ExpressionSyntaxElement ReturnProperty_String(LexicalElement @string);

        /// <summary>
        /// property = "true" ;
        /// </summary>
        /// <param name="@true">"true"</param>
        protected abstract ExpressionSyntaxElement ReturnProperty_True(LexicalElement @true);

        /// <summary>
        /// property = "false" ;
        /// </summary>
        /// <param name="@false">"false"</param>
        protected abstract ExpressionSyntaxElement ReturnProperty_False(LexicalElement @false);

        /// <summary>
        /// property = 'null' ;
        /// </summary>
        /// <param name="@null">'null'</param>
        protected abstract ExpressionSyntaxElement ReturnProperty_Null(LexicalElement @null);

        /// <summary>
        /// property = identifier ;
        /// </summary>
        /// <param name="identifier">identifier</param>
        protected abstract ExpressionSyntaxElement ReturnProperty(IdentifierSyntaxElement identifier);

        /// <summary>
        /// property = list ;
        /// </summary>
        /// <param name="list">list</param>
        protected abstract ExpressionSyntaxElement ReturnProperty(ListSyntaxElement list);

        /// <summary>
        /// property = func ;
        /// </summary>
        /// <param name="func">func</param>
        protected abstract ExpressionSyntaxElement ReturnProperty(ExpressionSyntaxElement func);

        /// <summary>
        /// property = parenthesis ;
        /// </summary>
        /// <param name="parenthesis">parenthesis</param>
        protected abstract ExpressionSyntaxElement ReturnProperty(ParenthesisSyntaxElement parenthesis);

        /// <summary>
        /// parenthesis = '(', multi_expression, ')' ;
        /// </summary>
        /// <param name="openPar">'('</param>
        /// <param name="multiExpression">multi_expression</param>
        /// <param name="closePar">')'</param>
        protected abstract ParenthesisSyntaxElement ReturnParenthesis(LexicalElement openPar, MultiExpressionSyntaxElement multiExpression, LexicalElement closePar);

        /// <summary>
        /// identifier = "Identifier" ;
        /// </summary>
        /// <param name="identifier">"Identifier"</param>
        protected abstract IdentifierSyntaxElement ReturnIdentifier(LexicalElement identifier);

        /// <summary>
        /// list = '{', [expression, {',', expression}], '}' ;
        /// </summary>
        /// <param name="openBraces">'{'</param>
        /// <param name="expression_repetition_opt">[expression, {',', expression}]</param>
        /// <param name="closeBraces">'}'</param>
        protected abstract ListSyntaxElement ReturnList(LexicalElement openBraces, Optional<FixedList<ExpressionSyntaxElement, FixedList<LexicalElement, ExpressionSyntaxElement>[]>> expression_repetition_opt, LexicalElement closeBraces);

        /// <summary>
        /// func = 'func', '(', [['params'], ['var'], identifier, {',', ['params'], ['var'], identifier}], ')', block ;
        /// </summary>
        /// <param name="func">'func'</param>
        /// <param name="openPar">'('</param>
        /// <param name="params_opt_Var_opt_Identifier_repetition_opt">[['params'], ['var'], identifier, {',', ['params'], ['var'], identifier}]</param>
        /// <param name="closePar">')'</param>
        /// <param name="block">block</param>
        protected abstract ExpressionSyntaxElement ReturnFunc(LexicalElement func, LexicalElement openPar, Optional<FixedList<Optional<LexicalElement>, Optional<LexicalElement>, IdentifierSyntaxElement, FixedList<LexicalElement, Optional<LexicalElement>, Optional<LexicalElement>, IdentifierSyntaxElement>[]>> params_opt_Var_opt_Identifier_repetition_opt, LexicalElement closePar, SyntaxElement block);

        protected ProgramSyntaxElement parseProgram() {
            List<SyntaxElement> var1 = new List<SyntaxElement>();
            for(Optional<LexicalElement> var2 = _Reader.Peek(); var2.HasValue && (var2.Value.Type == LexType.Semicolon || var2.Value.Type == LexType.Foreach || var2.Value.Type == LexType.For || var2.Value.Type == LexType.While || var2.Value.Type == LexType.Do || var2.Value.Type == LexType.If || var2.Value.Type == LexType.OpenBraces || var2.Value.Type == LexType.Break || var2.Value.Type == LexType.Continue || var2.Value.Type == LexType.Return || var2.Value.Type == LexType.Readonly || var2.Value.Type == LexType.Var || var2.Value.Type == LexType.Not || var2.Value.Type == LexType.Plus || var2.Value.Type == LexType.Minus || var2.Value.Type == LexType.PlusPlus || var2.Value.Type == LexType.MinusMinus || var2.Value.Type == LexType.Number || var2.Value.Type == LexType.String || var2.Value.Type == LexType.True || var2.Value.Type == LexType.False || var2.Value.Type == LexType.Null || var2.Value.Type == LexType.Identifier || var2.Value.Type == LexType.Func || var2.Value.Type == LexType.OpenPar); var2 = _Reader.Peek()) {
                SyntaxElement var3 = default(SyntaxElement);
                SyntaxElement var4 = this.parseStatement();
                var3 = var4;
                var1.Add(var3);
            }
            SyntaxElement[] var5;
            var5 = var1.ToArray();
            return this.ReturnProgram(var5);
        }
        protected SyntaxElement parseStatement() {
            LexicalElement var1 = _Reader.PeekOrThrow("statement", LexType.Foreach, LexType.For, LexType.While, LexType.Do, LexType.If, LexType.OpenBraces, LexType.Break, LexType.Continue, LexType.Return, LexType.Not, LexType.Plus, LexType.Minus, LexType.PlusPlus, LexType.MinusMinus, LexType.Number, LexType.String, LexType.True, LexType.False, LexType.Null, LexType.Identifier, LexType.Func, LexType.OpenPar, LexType.Semicolon, LexType.Readonly, LexType.Var);
            if(var1.Type == LexType.Foreach) {
                ForeachSyntaxElement var2 = this.parseForeach();
                return this.ReturnStatement(var2);
            } else if(var1.Type == LexType.For) {
                ForSyntaxElement var3 = this.parseFor();
                return this.ReturnStatement(var3);
            } else if(var1.Type == LexType.While) {
                DoWhileSyntaxElement var4 = this.parseWhile();
                return this.ReturnStatement_While(var4);
            } else if(var1.Type == LexType.Do) {
                DoWhileSyntaxElement var5 = this.parseDo();
                return this.ReturnStatement_Do(var5);
            } else if(var1.Type == LexType.If) {
                IfSyntaxElement var6 = this.parseIf();
                return this.ReturnStatement(var6);
            } else if(var1.Type == LexType.OpenBraces) {
                SyntaxElement var7 = this.parseBlock();
                return this.ReturnStatement(var7);
            } else if(var1.Type == LexType.Break || var1.Type == LexType.Continue || var1.Type == LexType.Return) {
                ControlSyntaxElement var8 = this.parseControl();
                LexicalElement var9 = _Reader.ReadOrThrow("statement", LexType.Semicolon);
                return this.ReturnStatement(var8, var9);
            } else if(var1.Type == LexType.Not || var1.Type == LexType.Plus || var1.Type == LexType.Minus || var1.Type == LexType.PlusPlus || var1.Type == LexType.MinusMinus || var1.Type == LexType.Number || var1.Type == LexType.String || var1.Type == LexType.True || var1.Type == LexType.False || var1.Type == LexType.Null || var1.Type == LexType.Identifier || var1.Type == LexType.OpenBraces || var1.Type == LexType.Func || var1.Type == LexType.OpenPar || var1.Type == LexType.Semicolon) {
                Optional<MultiExpressionSyntaxElement> var10 = default(Optional<MultiExpressionSyntaxElement>);
                Optional<LexicalElement> var11 = _Reader.Peek();
                if(var11.HasValue && (var11.Value.Type == LexType.Not || var11.Value.Type == LexType.Plus || var11.Value.Type == LexType.Minus || var11.Value.Type == LexType.PlusPlus || var11.Value.Type == LexType.MinusMinus || var11.Value.Type == LexType.Number || var11.Value.Type == LexType.String || var11.Value.Type == LexType.True || var11.Value.Type == LexType.False || var11.Value.Type == LexType.Null || var11.Value.Type == LexType.Identifier || var11.Value.Type == LexType.OpenBraces || var11.Value.Type == LexType.Func || var11.Value.Type == LexType.OpenPar)) {
                    MultiExpressionSyntaxElement var12 = default(MultiExpressionSyntaxElement);
                    MultiExpressionSyntaxElement var13 = this.parseMultiExpression();
                    var12 = var13;
                    var10 = var12;
                }
                LexicalElement var14 = _Reader.ReadOrThrow("statement", LexType.Semicolon);
                return this.ReturnStatement(var10, var14);
            } else if(var1.Type == LexType.Readonly || var1.Type == LexType.Var) {
                MultiDeclareSyntaxElement var15 = this.parseDeclare();
                LexicalElement var16 = _Reader.ReadOrThrow("statement", LexType.Semicolon);
                return this.ReturnStatement(var15, var16);
            } else {
                throw _Reader.Error("statement", LexType.Foreach, LexType.For, LexType.While, LexType.Do, LexType.If, LexType.OpenBraces, LexType.Break, LexType.Continue, LexType.Return, LexType.Not, LexType.Plus, LexType.Minus, LexType.PlusPlus, LexType.MinusMinus, LexType.Number, LexType.String, LexType.True, LexType.False, LexType.Null, LexType.Identifier, LexType.Func, LexType.OpenPar, LexType.Semicolon, LexType.Readonly, LexType.Var);
            }
        }
        protected ForeachSyntaxElement parseForeach() {
            LexicalElement var1 = _Reader.ReadOrThrow("foreach", LexType.Foreach);
            LexicalElement var2 = _Reader.ReadOrThrow("foreach", LexType.OpenPar);
            Optional<LexicalElement> var3 = default(Optional<LexicalElement>);
            Optional<LexicalElement> var4 = _Reader.Peek();
            if(var4.HasValue && (var4.Value.Type == LexType.Var)) {
                LexicalElement var5 = default(LexicalElement);
                LexicalElement var6 = _Reader.ReadOrThrow("foreach", LexType.Var);
                var5 = var6;
                var3 = var5;
            }
            IdentifierSyntaxElement var7 = this.parseIdentifier();
            LexicalElement var8 = _Reader.ReadOrThrow("foreach", LexType.In);
            ExpressionSyntaxElement var9 = this.parseExpression();
            LexicalElement var10 = _Reader.ReadOrThrow("foreach", LexType.ClosePar);
            SyntaxElement var11 = this.parseStatement();
            return this.ReturnForeach(var1, var2, var3, var7, var8, var9, var10, var11);
        }
        protected ForSyntaxElement parseFor() {
            LexicalElement var1 = _Reader.ReadOrThrow("for", LexType.For);
            LexicalElement var2 = _Reader.ReadOrThrow("for", LexType.OpenPar);
            Optional<Selection<ExpressionSyntaxElement, MultiDeclareSyntaxElement>> var3 = default(Optional<Selection<ExpressionSyntaxElement, MultiDeclareSyntaxElement>>);
            Optional<LexicalElement> var4 = _Reader.Peek();
            if(var4.HasValue && (var4.Value.Type == LexType.Readonly || var4.Value.Type == LexType.Var || var4.Value.Type == LexType.Not || var4.Value.Type == LexType.Plus || var4.Value.Type == LexType.Minus || var4.Value.Type == LexType.PlusPlus || var4.Value.Type == LexType.MinusMinus || var4.Value.Type == LexType.Number || var4.Value.Type == LexType.String || var4.Value.Type == LexType.True || var4.Value.Type == LexType.False || var4.Value.Type == LexType.Null || var4.Value.Type == LexType.Identifier || var4.Value.Type == LexType.OpenBraces || var4.Value.Type == LexType.Func || var4.Value.Type == LexType.OpenPar)) {
                Selection<ExpressionSyntaxElement, MultiDeclareSyntaxElement> var5 = default(Selection<ExpressionSyntaxElement, MultiDeclareSyntaxElement>);
                LexicalElement var6 = _Reader.PeekOrThrow("for", LexType.Not, LexType.Plus, LexType.Minus, LexType.PlusPlus, LexType.MinusMinus, LexType.Number, LexType.String, LexType.True, LexType.False, LexType.Null, LexType.Identifier, LexType.OpenBraces, LexType.Func, LexType.OpenPar, LexType.Readonly, LexType.Var);
                if(var6.Type == LexType.Not || var6.Type == LexType.Plus || var6.Type == LexType.Minus || var6.Type == LexType.PlusPlus || var6.Type == LexType.MinusMinus || var6.Type == LexType.Number || var6.Type == LexType.String || var6.Type == LexType.True || var6.Type == LexType.False || var6.Type == LexType.Null || var6.Type == LexType.Identifier || var6.Type == LexType.OpenBraces || var6.Type == LexType.Func || var6.Type == LexType.OpenPar) {
                    ExpressionSyntaxElement var7 = this.parseExpression();
                    Selection<ExpressionSyntaxElement, MultiDeclareSyntaxElement> var8 = new Selection<ExpressionSyntaxElement, MultiDeclareSyntaxElement>(var7);
                    var5 = var8;
                } else if(var6.Type == LexType.Readonly || var6.Type == LexType.Var) {
                    MultiDeclareSyntaxElement var9 = this.parseDeclare();
                    Selection<ExpressionSyntaxElement, MultiDeclareSyntaxElement> var10 = new Selection<ExpressionSyntaxElement, MultiDeclareSyntaxElement>(var9);
                    var5 = var10;
                } else {
                    throw _Reader.Error("for", LexType.Not, LexType.Plus, LexType.Minus, LexType.PlusPlus, LexType.MinusMinus, LexType.Number, LexType.String, LexType.True, LexType.False, LexType.Null, LexType.Identifier, LexType.OpenBraces, LexType.Func, LexType.OpenPar, LexType.Readonly, LexType.Var);
                }
                var3 = var5;
            }
            LexicalElement var11 = _Reader.ReadOrThrow("for", LexType.Semicolon);
            Optional<ExpressionSyntaxElement> var12 = default(Optional<ExpressionSyntaxElement>);
            Optional<LexicalElement> var13 = _Reader.Peek();
            if(var13.HasValue && (var13.Value.Type == LexType.Not || var13.Value.Type == LexType.Plus || var13.Value.Type == LexType.Minus || var13.Value.Type == LexType.PlusPlus || var13.Value.Type == LexType.MinusMinus || var13.Value.Type == LexType.Number || var13.Value.Type == LexType.String || var13.Value.Type == LexType.True || var13.Value.Type == LexType.False || var13.Value.Type == LexType.Null || var13.Value.Type == LexType.Identifier || var13.Value.Type == LexType.OpenBraces || var13.Value.Type == LexType.Func || var13.Value.Type == LexType.OpenPar)) {
                ExpressionSyntaxElement var14 = default(ExpressionSyntaxElement);
                ExpressionSyntaxElement var15 = this.parseExpression();
                var14 = var15;
                var12 = var14;
            }
            LexicalElement var16 = _Reader.ReadOrThrow("for", LexType.Semicolon);
            Optional<ExpressionSyntaxElement> var17 = default(Optional<ExpressionSyntaxElement>);
            Optional<LexicalElement> var18 = _Reader.Peek();
            if(var18.HasValue && (var18.Value.Type == LexType.Not || var18.Value.Type == LexType.Plus || var18.Value.Type == LexType.Minus || var18.Value.Type == LexType.PlusPlus || var18.Value.Type == LexType.MinusMinus || var18.Value.Type == LexType.Number || var18.Value.Type == LexType.String || var18.Value.Type == LexType.True || var18.Value.Type == LexType.False || var18.Value.Type == LexType.Null || var18.Value.Type == LexType.Identifier || var18.Value.Type == LexType.OpenBraces || var18.Value.Type == LexType.Func || var18.Value.Type == LexType.OpenPar)) {
                ExpressionSyntaxElement var19 = default(ExpressionSyntaxElement);
                ExpressionSyntaxElement var20 = this.parseExpression();
                var19 = var20;
                var17 = var19;
            }
            LexicalElement var21 = _Reader.ReadOrThrow("for", LexType.ClosePar);
            SyntaxElement var22 = this.parseStatement();
            return this.ReturnFor(var1, var2, var3, var11, var12, var16, var17, var21, var22);
        }
        protected DoWhileSyntaxElement parseWhile() {
            LexicalElement var1 = _Reader.ReadOrThrow("while", LexType.While);
            LexicalElement var2 = _Reader.ReadOrThrow("while", LexType.OpenPar);
            ExpressionSyntaxElement var3 = this.parseExpression();
            LexicalElement var4 = _Reader.ReadOrThrow("while", LexType.ClosePar);
            SyntaxElement var5 = this.parseStatement();
            return this.ReturnWhile(var1, var2, var3, var4, var5);
        }
        protected DoWhileSyntaxElement parseDo() {
            LexicalElement var1 = _Reader.ReadOrThrow("do", LexType.Do);
            SyntaxElement var2 = this.parseStatement();
            LexicalElement var3 = _Reader.ReadOrThrow("do", LexType.While);
            LexicalElement var4 = _Reader.ReadOrThrow("do", LexType.OpenPar);
            ExpressionSyntaxElement var5 = this.parseExpression();
            LexicalElement var6 = _Reader.ReadOrThrow("do", LexType.ClosePar);
            LexicalElement var7 = _Reader.ReadOrThrow("do", LexType.Semicolon);
            return this.ReturnDo(var1, var2, var3, var4, var5, var6, var7);
        }
        protected IfSyntaxElement parseIf() {
            LexicalElement var1 = _Reader.ReadOrThrow("if", LexType.If);
            LexicalElement var2 = _Reader.ReadOrThrow("if", LexType.OpenPar);
            ExpressionSyntaxElement var3 = this.parseExpression();
            LexicalElement var4 = _Reader.ReadOrThrow("if", LexType.ClosePar);
            SyntaxElement var5 = this.parseStatement();
            Optional<FixedList<LexicalElement, SyntaxElement>> var6 = default(Optional<FixedList<LexicalElement, SyntaxElement>>);
            Optional<LexicalElement> var7 = _Reader.Peek();
            if(var7.HasValue && (var7.Value.Type == LexType.Else)) {
                FixedList<LexicalElement, SyntaxElement> var8 = default(FixedList<LexicalElement, SyntaxElement>);
                LexicalElement var9 = _Reader.ReadOrThrow("if", LexType.Else);
                SyntaxElement var10 = this.parseStatement();
                FixedList<LexicalElement, SyntaxElement> var11 = new FixedList<LexicalElement, SyntaxElement>(var9, var10);
                var8 = var11;
                var6 = var8;
            }
            return this.ReturnIf(var1, var2, var3, var4, var5, var6);
        }
        protected SyntaxElement parseBlock() {
            LexicalElement var1 = _Reader.ReadOrThrow("block", LexType.OpenBraces);
            List<SyntaxElement> var2 = new List<SyntaxElement>();
            for(Optional<LexicalElement> var3 = _Reader.Peek(); var3.HasValue && (var3.Value.Type == LexType.Semicolon || var3.Value.Type == LexType.Foreach || var3.Value.Type == LexType.For || var3.Value.Type == LexType.While || var3.Value.Type == LexType.Do || var3.Value.Type == LexType.If || var3.Value.Type == LexType.OpenBraces || var3.Value.Type == LexType.Break || var3.Value.Type == LexType.Continue || var3.Value.Type == LexType.Return || var3.Value.Type == LexType.Readonly || var3.Value.Type == LexType.Var || var3.Value.Type == LexType.Not || var3.Value.Type == LexType.Plus || var3.Value.Type == LexType.Minus || var3.Value.Type == LexType.PlusPlus || var3.Value.Type == LexType.MinusMinus || var3.Value.Type == LexType.Number || var3.Value.Type == LexType.String || var3.Value.Type == LexType.True || var3.Value.Type == LexType.False || var3.Value.Type == LexType.Null || var3.Value.Type == LexType.Identifier || var3.Value.Type == LexType.Func || var3.Value.Type == LexType.OpenPar); var3 = _Reader.Peek()) {
                SyntaxElement var4 = default(SyntaxElement);
                SyntaxElement var5 = this.parseStatement();
                var4 = var5;
                var2.Add(var4);
            }
            SyntaxElement[] var6;
            var6 = var2.ToArray();
            LexicalElement var7 = _Reader.ReadOrThrow("block", LexType.CloseBraces);
            return this.ReturnBlock(var1, var6, var7);
        }
        protected ControlSyntaxElement parseControl() {
            LexicalElement var1 = _Reader.PeekOrThrow("control", LexType.Break, LexType.Continue, LexType.Return);
            if(var1.Type == LexType.Break) {
                LexicalElement var2 = _Reader.ReadOrThrow("control", LexType.Break);
                return this.ReturnControl_Break(var2);
            } else if(var1.Type == LexType.Continue) {
                LexicalElement var3 = _Reader.ReadOrThrow("control", LexType.Continue);
                return this.ReturnControl_Continue(var3);
            } else if(var1.Type == LexType.Return) {
                LexicalElement var4 = _Reader.ReadOrThrow("control", LexType.Return);
                Optional<ExpressionSyntaxElement> var5 = default(Optional<ExpressionSyntaxElement>);
                Optional<LexicalElement> var6 = _Reader.Peek();
                if(var6.HasValue && (var6.Value.Type == LexType.Not || var6.Value.Type == LexType.Plus || var6.Value.Type == LexType.Minus || var6.Value.Type == LexType.PlusPlus || var6.Value.Type == LexType.MinusMinus || var6.Value.Type == LexType.Number || var6.Value.Type == LexType.String || var6.Value.Type == LexType.True || var6.Value.Type == LexType.False || var6.Value.Type == LexType.Null || var6.Value.Type == LexType.Identifier || var6.Value.Type == LexType.OpenBraces || var6.Value.Type == LexType.Func || var6.Value.Type == LexType.OpenPar)) {
                    ExpressionSyntaxElement var7 = default(ExpressionSyntaxElement);
                    ExpressionSyntaxElement var8 = this.parseExpression();
                    var7 = var8;
                    var5 = var7;
                }
                return this.ReturnControl(var4, var5);
            } else {
                throw _Reader.Error("control", LexType.Break, LexType.Continue, LexType.Return);
            }
        }
        protected MultiExpressionSyntaxElement parseMultiExpression() {
            ExpressionSyntaxElement var1 = this.parseExpression();
            List<FixedList<LexicalElement, ExpressionSyntaxElement>> var2 = new List<FixedList<LexicalElement, ExpressionSyntaxElement>>();
            for(Optional<LexicalElement> var3 = _Reader.Peek(); var3.HasValue && (var3.Value.Type == LexType.Comma); var3 = _Reader.Peek()) {
                FixedList<LexicalElement, ExpressionSyntaxElement> var4 = default(FixedList<LexicalElement, ExpressionSyntaxElement>);
                LexicalElement var5 = _Reader.ReadOrThrow("multi_expression", LexType.Comma);
                ExpressionSyntaxElement var6 = this.parseExpression();
                FixedList<LexicalElement, ExpressionSyntaxElement> var7 = new FixedList<LexicalElement, ExpressionSyntaxElement>(var5, var6);
                var4 = var7;
                var2.Add(var4);
            }
            FixedList<LexicalElement, ExpressionSyntaxElement>[] var8;
            var8 = var2.ToArray();
            return this.ReturnMultiExpression(var1, var8);
        }
        protected MultiDeclareSyntaxElement parseDeclare() {
            Selection<FixedList<LexicalElement, Optional<LexicalElement>>, LexicalElement> var1 = default(Selection<FixedList<LexicalElement, Optional<LexicalElement>>, LexicalElement>);
            LexicalElement var2 = _Reader.PeekOrThrow("declare", LexType.Readonly, LexType.Var);
            if(var2.Type == LexType.Readonly) {
                LexicalElement var3 = _Reader.ReadOrThrow("declare", LexType.Readonly);
                Optional<LexicalElement> var4 = default(Optional<LexicalElement>);
                Optional<LexicalElement> var5 = _Reader.Peek();
                if(var5.HasValue && (var5.Value.Type == LexType.Var)) {
                    LexicalElement var6 = default(LexicalElement);
                    LexicalElement var7 = _Reader.ReadOrThrow("declare", LexType.Var);
                    var6 = var7;
                    var4 = var6;
                }
                FixedList<LexicalElement, Optional<LexicalElement>> var8 = new FixedList<LexicalElement, Optional<LexicalElement>>(var3, var4);
                Selection<FixedList<LexicalElement, Optional<LexicalElement>>, LexicalElement> var9 = new Selection<FixedList<LexicalElement, Optional<LexicalElement>>, LexicalElement>(var8);
                var1 = var9;
            } else if(var2.Type == LexType.Var) {
                LexicalElement var10 = _Reader.ReadOrThrow("declare", LexType.Var);
                Selection<FixedList<LexicalElement, Optional<LexicalElement>>, LexicalElement> var11 = new Selection<FixedList<LexicalElement, Optional<LexicalElement>>, LexicalElement>(var10);
                var1 = var11;
            } else {
                throw _Reader.Error("declare", LexType.Readonly, LexType.Var);
            }
            IdentifierSyntaxElement var12 = this.parseIdentifier();
            Optional<FixedList<LexicalElement, ExpressionSyntaxElement>> var13 = default(Optional<FixedList<LexicalElement, ExpressionSyntaxElement>>);
            Optional<LexicalElement> var14 = _Reader.Peek();
            if(var14.HasValue && (var14.Value.Type == LexType.Equal)) {
                FixedList<LexicalElement, ExpressionSyntaxElement> var15 = default(FixedList<LexicalElement, ExpressionSyntaxElement>);
                LexicalElement var16 = _Reader.ReadOrThrow("declare", LexType.Equal);
                ExpressionSyntaxElement var17 = this.parseExpression();
                FixedList<LexicalElement, ExpressionSyntaxElement> var18 = new FixedList<LexicalElement, ExpressionSyntaxElement>(var16, var17);
                var15 = var18;
                var13 = var15;
            }
            List<FixedList<LexicalElement, IdentifierSyntaxElement, Optional<FixedList<LexicalElement, ExpressionSyntaxElement>>>> var19 = new List<FixedList<LexicalElement, IdentifierSyntaxElement, Optional<FixedList<LexicalElement, ExpressionSyntaxElement>>>>();
            for(Optional<LexicalElement> var20 = _Reader.Peek(); var20.HasValue && (var20.Value.Type == LexType.Comma); var20 = _Reader.Peek()) {
                FixedList<LexicalElement, IdentifierSyntaxElement, Optional<FixedList<LexicalElement, ExpressionSyntaxElement>>> var21 = default(FixedList<LexicalElement, IdentifierSyntaxElement, Optional<FixedList<LexicalElement, ExpressionSyntaxElement>>>);
                LexicalElement var22 = _Reader.ReadOrThrow("declare", LexType.Comma);
                IdentifierSyntaxElement var23 = this.parseIdentifier();
                Optional<FixedList<LexicalElement, ExpressionSyntaxElement>> var24 = default(Optional<FixedList<LexicalElement, ExpressionSyntaxElement>>);
                Optional<LexicalElement> var25 = _Reader.Peek();
                if(var25.HasValue && (var25.Value.Type == LexType.Equal)) {
                    FixedList<LexicalElement, ExpressionSyntaxElement> var26 = default(FixedList<LexicalElement, ExpressionSyntaxElement>);
                    LexicalElement var27 = _Reader.ReadOrThrow("declare", LexType.Equal);
                    ExpressionSyntaxElement var28 = this.parseExpression();
                    FixedList<LexicalElement, ExpressionSyntaxElement> var29 = new FixedList<LexicalElement, ExpressionSyntaxElement>(var27, var28);
                    var26 = var29;
                    var24 = var26;
                }
                FixedList<LexicalElement, IdentifierSyntaxElement, Optional<FixedList<LexicalElement, ExpressionSyntaxElement>>> var30 = new FixedList<LexicalElement, IdentifierSyntaxElement, Optional<FixedList<LexicalElement, ExpressionSyntaxElement>>>(var22, var23, var24);
                var21 = var30;
                var19.Add(var21);
            }
            FixedList<LexicalElement, IdentifierSyntaxElement, Optional<FixedList<LexicalElement, ExpressionSyntaxElement>>>[] var31;
            var31 = var19.ToArray();
            return this.ReturnDeclare(var1, var12, var13, var31);
        }
        protected ExpressionSyntaxElement parseExpression() {
            ExpressionSyntaxElement var1 = this.parseSubstitute();
            return this.ReturnExpression(var1);
        }
        protected ExpressionSyntaxElement parseSubstitute() {
            ExpressionSyntaxElement var1 = this.parseLeftValue();
            Optional<FixedList<LexicalElement, ExpressionSyntaxElement>> var2 = default(Optional<FixedList<LexicalElement, ExpressionSyntaxElement>>);
            Optional<LexicalElement> var3 = _Reader.Peek();
            if(var3.HasValue && (var3.Value.Type == LexType.PlusEqual || var3.Value.Type == LexType.MinusEqual || var3.Value.Type == LexType.CrossEqual || var3.Value.Type == LexType.SlashEqual || var3.Value.Type == LexType.PercentEqual || var3.Value.Type == LexType.Equal)) {
                FixedList<LexicalElement, ExpressionSyntaxElement> var4 = default(FixedList<LexicalElement, ExpressionSyntaxElement>);
                LexicalElement var5 = default(LexicalElement);
                LexicalElement var6 = _Reader.PeekOrThrow("substitute", LexType.PlusEqual, LexType.MinusEqual, LexType.CrossEqual, LexType.SlashEqual, LexType.PercentEqual, LexType.Equal);
                if(var6.Type == LexType.PlusEqual) {
                    LexicalElement var7 = _Reader.ReadOrThrow("substitute", LexType.PlusEqual);
                    var5 = var7;
                } else if(var6.Type == LexType.MinusEqual) {
                    LexicalElement var8 = _Reader.ReadOrThrow("substitute", LexType.MinusEqual);
                    var5 = var8;
                } else if(var6.Type == LexType.CrossEqual) {
                    LexicalElement var9 = _Reader.ReadOrThrow("substitute", LexType.CrossEqual);
                    var5 = var9;
                } else if(var6.Type == LexType.SlashEqual) {
                    LexicalElement var10 = _Reader.ReadOrThrow("substitute", LexType.SlashEqual);
                    var5 = var10;
                } else if(var6.Type == LexType.PercentEqual) {
                    LexicalElement var11 = _Reader.ReadOrThrow("substitute", LexType.PercentEqual);
                    var5 = var11;
                } else if(var6.Type == LexType.Equal) {
                    LexicalElement var12 = _Reader.ReadOrThrow("substitute", LexType.Equal);
                    var5 = var12;
                } else {
                    throw _Reader.Error("substitute", LexType.PlusEqual, LexType.MinusEqual, LexType.CrossEqual, LexType.SlashEqual, LexType.PercentEqual, LexType.Equal);
                }
                ExpressionSyntaxElement var13 = this.parseSubstitute();
                FixedList<LexicalElement, ExpressionSyntaxElement> var14 = new FixedList<LexicalElement, ExpressionSyntaxElement>(var5, var13);
                var4 = var14;
                var2 = var4;
            }
            return this.ReturnSubstitute(var1, var2);
        }
        protected ExpressionSyntaxElement parseLeftValue() {
            ExpressionSyntaxElement var1 = this.parseTernary();
            return this.ReturnLeftValue(var1);
        }
        protected ExpressionSyntaxElement parseTernary() {
            ExpressionSyntaxElement var1 = this.parseOr();
            Optional<FixedList<LexicalElement, ExpressionSyntaxElement, LexicalElement, ExpressionSyntaxElement>> var2 = default(Optional<FixedList<LexicalElement, ExpressionSyntaxElement, LexicalElement, ExpressionSyntaxElement>>);
            Optional<LexicalElement> var3 = _Reader.Peek();
            if(var3.HasValue && (var3.Value.Type == LexType.Question)) {
                FixedList<LexicalElement, ExpressionSyntaxElement, LexicalElement, ExpressionSyntaxElement> var4 = default(FixedList<LexicalElement, ExpressionSyntaxElement, LexicalElement, ExpressionSyntaxElement>);
                LexicalElement var5 = _Reader.ReadOrThrow("ternary", LexType.Question);
                ExpressionSyntaxElement var6 = this.parseTernary();
                LexicalElement var7 = _Reader.ReadOrThrow("ternary", LexType.Colon);
                ExpressionSyntaxElement var8 = this.parseTernary();
                FixedList<LexicalElement, ExpressionSyntaxElement, LexicalElement, ExpressionSyntaxElement> var9 = new FixedList<LexicalElement, ExpressionSyntaxElement, LexicalElement, ExpressionSyntaxElement>(var5, var6, var7, var8);
                var4 = var9;
                var2 = var4;
            }
            return this.ReturnTernary(var1, var2);
        }
        protected ExpressionSyntaxElement parseOr() {
            ExpressionSyntaxElement var1 = this.parseAnd();
            List<FixedList<LexicalElement, ExpressionSyntaxElement>> var2 = new List<FixedList<LexicalElement, ExpressionSyntaxElement>>();
            for(Optional<LexicalElement> var3 = _Reader.Peek(); var3.HasValue && (var3.Value.Type == LexType.Or); var3 = _Reader.Peek()) {
                FixedList<LexicalElement, ExpressionSyntaxElement> var4 = default(FixedList<LexicalElement, ExpressionSyntaxElement>);
                LexicalElement var5 = _Reader.ReadOrThrow("or", LexType.Or);
                ExpressionSyntaxElement var6 = this.parseAnd();
                FixedList<LexicalElement, ExpressionSyntaxElement> var7 = new FixedList<LexicalElement, ExpressionSyntaxElement>(var5, var6);
                var4 = var7;
                var2.Add(var4);
            }
            FixedList<LexicalElement, ExpressionSyntaxElement>[] var8;
            var8 = var2.ToArray();
            return this.ReturnOr(var1, var8);
        }
        protected ExpressionSyntaxElement parseAnd() {
            ExpressionSyntaxElement var1 = this.parseCmp();
            List<FixedList<LexicalElement, ExpressionSyntaxElement>> var2 = new List<FixedList<LexicalElement, ExpressionSyntaxElement>>();
            for(Optional<LexicalElement> var3 = _Reader.Peek(); var3.HasValue && (var3.Value.Type == LexType.And); var3 = _Reader.Peek()) {
                FixedList<LexicalElement, ExpressionSyntaxElement> var4 = default(FixedList<LexicalElement, ExpressionSyntaxElement>);
                LexicalElement var5 = _Reader.ReadOrThrow("and", LexType.And);
                ExpressionSyntaxElement var6 = this.parseCmp();
                FixedList<LexicalElement, ExpressionSyntaxElement> var7 = new FixedList<LexicalElement, ExpressionSyntaxElement>(var5, var6);
                var4 = var7;
                var2.Add(var4);
            }
            FixedList<LexicalElement, ExpressionSyntaxElement>[] var8;
            var8 = var2.ToArray();
            return this.ReturnAnd(var1, var8);
        }
        protected ExpressionSyntaxElement parseCmp() {
            ExpressionSyntaxElement var1 = this.parseAdd();
            Optional<FixedList<LexicalElement, ExpressionSyntaxElement>> var2 = default(Optional<FixedList<LexicalElement, ExpressionSyntaxElement>>);
            Optional<LexicalElement> var3 = _Reader.Peek();
            if(var3.HasValue && (var3.Value.Type == LexType.Le || var3.Value.Type == LexType.Ge || var3.Value.Type == LexType.Eq || var3.Value.Type == LexType.Ne || var3.Value.Type == LexType.Lt || var3.Value.Type == LexType.Gt)) {
                FixedList<LexicalElement, ExpressionSyntaxElement> var4 = default(FixedList<LexicalElement, ExpressionSyntaxElement>);
                LexicalElement var5 = default(LexicalElement);
                LexicalElement var6 = _Reader.PeekOrThrow("cmp", LexType.Le, LexType.Ge, LexType.Eq, LexType.Ne, LexType.Lt, LexType.Gt);
                if(var6.Type == LexType.Le) {
                    LexicalElement var7 = _Reader.ReadOrThrow("cmp", LexType.Le);
                    var5 = var7;
                } else if(var6.Type == LexType.Ge) {
                    LexicalElement var8 = _Reader.ReadOrThrow("cmp", LexType.Ge);
                    var5 = var8;
                } else if(var6.Type == LexType.Eq) {
                    LexicalElement var9 = _Reader.ReadOrThrow("cmp", LexType.Eq);
                    var5 = var9;
                } else if(var6.Type == LexType.Ne) {
                    LexicalElement var10 = _Reader.ReadOrThrow("cmp", LexType.Ne);
                    var5 = var10;
                } else if(var6.Type == LexType.Lt) {
                    LexicalElement var11 = _Reader.ReadOrThrow("cmp", LexType.Lt);
                    var5 = var11;
                } else if(var6.Type == LexType.Gt) {
                    LexicalElement var12 = _Reader.ReadOrThrow("cmp", LexType.Gt);
                    var5 = var12;
                } else {
                    throw _Reader.Error("cmp", LexType.Le, LexType.Ge, LexType.Eq, LexType.Ne, LexType.Lt, LexType.Gt);
                }
                ExpressionSyntaxElement var13 = this.parseAdd();
                FixedList<LexicalElement, ExpressionSyntaxElement> var14 = new FixedList<LexicalElement, ExpressionSyntaxElement>(var5, var13);
                var4 = var14;
                var2 = var4;
            }
            return this.ReturnCmp(var1, var2);
        }
        protected ExpressionSyntaxElement parseAdd() {
            ExpressionSyntaxElement var1 = this.parseMul();
            List<FixedList<LexicalElement, ExpressionSyntaxElement>> var2 = new List<FixedList<LexicalElement, ExpressionSyntaxElement>>();
            for(Optional<LexicalElement> var3 = _Reader.Peek(); var3.HasValue && (var3.Value.Type == LexType.Plus || var3.Value.Type == LexType.Minus); var3 = _Reader.Peek()) {
                FixedList<LexicalElement, ExpressionSyntaxElement> var4 = default(FixedList<LexicalElement, ExpressionSyntaxElement>);
                LexicalElement var5 = default(LexicalElement);
                LexicalElement var6 = _Reader.PeekOrThrow("add", LexType.Plus, LexType.Minus);
                if(var6.Type == LexType.Plus) {
                    LexicalElement var7 = _Reader.ReadOrThrow("add", LexType.Plus);
                    var5 = var7;
                } else if(var6.Type == LexType.Minus) {
                    LexicalElement var8 = _Reader.ReadOrThrow("add", LexType.Minus);
                    var5 = var8;
                } else {
                    throw _Reader.Error("add", LexType.Plus, LexType.Minus);
                }
                ExpressionSyntaxElement var9 = this.parseMul();
                FixedList<LexicalElement, ExpressionSyntaxElement> var10 = new FixedList<LexicalElement, ExpressionSyntaxElement>(var5, var9);
                var4 = var10;
                var2.Add(var4);
            }
            FixedList<LexicalElement, ExpressionSyntaxElement>[] var11;
            var11 = var2.ToArray();
            return this.ReturnAdd(var1, var11);
        }
        protected ExpressionSyntaxElement parseMul() {
            ExpressionSyntaxElement var1 = this.parseUnary();
            List<FixedList<LexicalElement, ExpressionSyntaxElement>> var2 = new List<FixedList<LexicalElement, ExpressionSyntaxElement>>();
            for(Optional<LexicalElement> var3 = _Reader.Peek(); var3.HasValue && (var3.Value.Type == LexType.Cross || var3.Value.Type == LexType.Slash || var3.Value.Type == LexType.Percent); var3 = _Reader.Peek()) {
                FixedList<LexicalElement, ExpressionSyntaxElement> var4 = default(FixedList<LexicalElement, ExpressionSyntaxElement>);
                LexicalElement var5 = default(LexicalElement);
                LexicalElement var6 = _Reader.PeekOrThrow("mul", LexType.Cross, LexType.Slash, LexType.Percent);
                if(var6.Type == LexType.Cross) {
                    LexicalElement var7 = _Reader.ReadOrThrow("mul", LexType.Cross);
                    var5 = var7;
                } else if(var6.Type == LexType.Slash) {
                    LexicalElement var8 = _Reader.ReadOrThrow("mul", LexType.Slash);
                    var5 = var8;
                } else if(var6.Type == LexType.Percent) {
                    LexicalElement var9 = _Reader.ReadOrThrow("mul", LexType.Percent);
                    var5 = var9;
                } else {
                    throw _Reader.Error("mul", LexType.Cross, LexType.Slash, LexType.Percent);
                }
                ExpressionSyntaxElement var10 = this.parseUnary();
                FixedList<LexicalElement, ExpressionSyntaxElement> var11 = new FixedList<LexicalElement, ExpressionSyntaxElement>(var5, var10);
                var4 = var11;
                var2.Add(var4);
            }
            FixedList<LexicalElement, ExpressionSyntaxElement>[] var12;
            var12 = var2.ToArray();
            return this.ReturnMul(var1, var12);
        }
        protected ExpressionSyntaxElement parseUnary() {
            List<LexicalElement> var1 = new List<LexicalElement>();
            for(Optional<LexicalElement> var2 = _Reader.Peek(); var2.HasValue && (var2.Value.Type == LexType.Not || var2.Value.Type == LexType.Plus || var2.Value.Type == LexType.Minus); var2 = _Reader.Peek()) {
                LexicalElement var3 = default(LexicalElement);
                LexicalElement var4 = _Reader.PeekOrThrow("unary", LexType.Not, LexType.Plus, LexType.Minus);
                if(var4.Type == LexType.Not) {
                    LexicalElement var5 = _Reader.ReadOrThrow("unary", LexType.Not);
                    var3 = var5;
                } else if(var4.Type == LexType.Plus) {
                    LexicalElement var6 = _Reader.ReadOrThrow("unary", LexType.Plus);
                    var3 = var6;
                } else if(var4.Type == LexType.Minus) {
                    LexicalElement var7 = _Reader.ReadOrThrow("unary", LexType.Minus);
                    var3 = var7;
                } else {
                    throw _Reader.Error("unary", LexType.Not, LexType.Plus, LexType.Minus);
                }
                var1.Add(var3);
            }
            LexicalElement[] var8;
            var8 = var1.ToArray();
            ExpressionSyntaxElement var9 = this.parseInc();
            return this.ReturnUnary(var8, var9);
        }
        protected ExpressionSyntaxElement parseInc() {
            LexicalElement var1 = _Reader.PeekOrThrow("inc", LexType.Number, LexType.String, LexType.True, LexType.False, LexType.Null, LexType.Identifier, LexType.OpenBraces, LexType.Func, LexType.OpenPar, LexType.PlusPlus, LexType.MinusMinus);
            if(var1.Type == LexType.Number || var1.Type == LexType.String || var1.Type == LexType.True || var1.Type == LexType.False || var1.Type == LexType.Null || var1.Type == LexType.Identifier || var1.Type == LexType.OpenBraces || var1.Type == LexType.Func || var1.Type == LexType.OpenPar) {
                ExpressionSyntaxElement var2 = this.parseModProperty();
                Optional<LexicalElement> var3 = _Reader.Peek();
                if(var3.HasValue && (var3.Value.Type == LexType.PlusPlus || var3.Value.Type == LexType.MinusMinus)) {
                    LexicalElement var4 = default(LexicalElement);
                    LexicalElement var5 = _Reader.PeekOrThrow("inc", LexType.PlusPlus, LexType.MinusMinus);
                    if(var5.Type == LexType.PlusPlus) {
                        LexicalElement var6 = _Reader.ReadOrThrow("inc", LexType.PlusPlus);
                        var4 = var6;
                    } else if(var5.Type == LexType.MinusMinus) {
                        LexicalElement var7 = _Reader.ReadOrThrow("inc", LexType.MinusMinus);
                        var4 = var7;
                    } else {
                        throw _Reader.Error("inc", LexType.PlusPlus, LexType.MinusMinus);
                    }
                    return this.ReturnInc(var2, var4);
                } else {
                    return this.ReturnInc(var2);
                }
            } else if(var1.Type == LexType.PlusPlus || var1.Type == LexType.MinusMinus) {
                LexicalElement var8 = default(LexicalElement);
                LexicalElement var9 = _Reader.PeekOrThrow("inc", LexType.PlusPlus, LexType.MinusMinus);
                if(var9.Type == LexType.PlusPlus) {
                    LexicalElement var10 = _Reader.ReadOrThrow("inc", LexType.PlusPlus);
                    var8 = var10;
                } else if(var9.Type == LexType.MinusMinus) {
                    LexicalElement var11 = _Reader.ReadOrThrow("inc", LexType.MinusMinus);
                    var8 = var11;
                } else {
                    throw _Reader.Error("inc", LexType.PlusPlus, LexType.MinusMinus);
                }
                ExpressionSyntaxElement var12 = this.parseModProperty();
                return this.ReturnInc(var8, var12);
            } else {
                throw _Reader.Error("inc", LexType.Number, LexType.String, LexType.True, LexType.False, LexType.Null, LexType.Identifier, LexType.OpenBraces, LexType.Func, LexType.OpenPar, LexType.PlusPlus, LexType.MinusMinus);
            }
        }
        protected ExpressionSyntaxElement parseModProperty() {
            ExpressionSyntaxElement var1 = this.parseProperty();
            List<Selection<ArgumentsSyntaxElement, ExpressionSyntaxElement, DotInvocationSyntaxElement>> var2 = new List<Selection<ArgumentsSyntaxElement, ExpressionSyntaxElement, DotInvocationSyntaxElement>>();
            for(Optional<LexicalElement> var3 = _Reader.Peek(); var3.HasValue && (var3.Value.Type == LexType.OpenPar || var3.Value.Type == LexType.OpenBracket || var3.Value.Type == LexType.Dot); var3 = _Reader.Peek()) {
                Selection<ArgumentsSyntaxElement, ExpressionSyntaxElement, DotInvocationSyntaxElement> var4 = default(Selection<ArgumentsSyntaxElement, ExpressionSyntaxElement, DotInvocationSyntaxElement>);
                LexicalElement var5 = _Reader.PeekOrThrow("mod_property", LexType.OpenPar, LexType.OpenBracket, LexType.Dot);
                if(var5.Type == LexType.OpenPar) {
                    ArgumentsSyntaxElement var6 = this.parseArguments();
                    Selection<ArgumentsSyntaxElement, ExpressionSyntaxElement, DotInvocationSyntaxElement> var7 = new Selection<ArgumentsSyntaxElement, ExpressionSyntaxElement, DotInvocationSyntaxElement>(var6);
                    var4 = var7;
                } else if(var5.Type == LexType.OpenBracket) {
                    ExpressionSyntaxElement var8 = this.parseIndexing();
                    Selection<ArgumentsSyntaxElement, ExpressionSyntaxElement, DotInvocationSyntaxElement> var9 = new Selection<ArgumentsSyntaxElement, ExpressionSyntaxElement, DotInvocationSyntaxElement>(var8);
                    var4 = var9;
                } else if(var5.Type == LexType.Dot) {
                    DotInvocationSyntaxElement var10 = this.parseDotInvocation();
                    Selection<ArgumentsSyntaxElement, ExpressionSyntaxElement, DotInvocationSyntaxElement> var11 = new Selection<ArgumentsSyntaxElement, ExpressionSyntaxElement, DotInvocationSyntaxElement>(var10);
                    var4 = var11;
                } else {
                    throw _Reader.Error("mod_property", LexType.OpenPar, LexType.OpenBracket, LexType.Dot);
                }
                var2.Add(var4);
            }
            Selection<ArgumentsSyntaxElement, ExpressionSyntaxElement, DotInvocationSyntaxElement>[] var12;
            var12 = var2.ToArray();
            return this.ReturnModProperty(var1, var12);
        }
        protected ExpressionSyntaxElement parseIndexing() {
            LexicalElement var1 = _Reader.ReadOrThrow("indexing", LexType.OpenBracket);
            ExpressionSyntaxElement var2 = this.parseExpression();
            LexicalElement var3 = _Reader.ReadOrThrow("indexing", LexType.CloseBracket);
            return this.ReturnIndexing(var1, var2, var3);
        }
        protected DotInvocationSyntaxElement parseDotInvocation() {
            LexicalElement var1 = _Reader.ReadOrThrow("dot_invocation", LexType.Dot);
            IdentifierSyntaxElement var2 = this.parseIdentifier();
            ArgumentsSyntaxElement var3 = this.parseArguments();
            return this.ReturnDotInvocation(var1, var2, var3);
        }
        protected ArgumentsSyntaxElement parseArguments() {
            LexicalElement var1 = _Reader.ReadOrThrow("arguments", LexType.OpenPar);
            Optional<FixedList<ExpressionSyntaxElement, FixedList<LexicalElement, ExpressionSyntaxElement>[]>> var2 = default(Optional<FixedList<ExpressionSyntaxElement, FixedList<LexicalElement, ExpressionSyntaxElement>[]>>);
            Optional<LexicalElement> var3 = _Reader.Peek();
            if(var3.HasValue && (var3.Value.Type == LexType.Not || var3.Value.Type == LexType.Plus || var3.Value.Type == LexType.Minus || var3.Value.Type == LexType.PlusPlus || var3.Value.Type == LexType.MinusMinus || var3.Value.Type == LexType.Number || var3.Value.Type == LexType.String || var3.Value.Type == LexType.True || var3.Value.Type == LexType.False || var3.Value.Type == LexType.Null || var3.Value.Type == LexType.Identifier || var3.Value.Type == LexType.OpenBraces || var3.Value.Type == LexType.Func || var3.Value.Type == LexType.OpenPar)) {
                FixedList<ExpressionSyntaxElement, FixedList<LexicalElement, ExpressionSyntaxElement>[]> var4 = default(FixedList<ExpressionSyntaxElement, FixedList<LexicalElement, ExpressionSyntaxElement>[]>);
                ExpressionSyntaxElement var5 = this.parseExpression();
                List<FixedList<LexicalElement, ExpressionSyntaxElement>> var6 = new List<FixedList<LexicalElement, ExpressionSyntaxElement>>();
                for(Optional<LexicalElement> var7 = _Reader.Peek(); var7.HasValue && (var7.Value.Type == LexType.Comma); var7 = _Reader.Peek()) {
                    FixedList<LexicalElement, ExpressionSyntaxElement> var8 = default(FixedList<LexicalElement, ExpressionSyntaxElement>);
                    LexicalElement var9 = _Reader.ReadOrThrow("arguments", LexType.Comma);
                    ExpressionSyntaxElement var10 = this.parseExpression();
                    FixedList<LexicalElement, ExpressionSyntaxElement> var11 = new FixedList<LexicalElement, ExpressionSyntaxElement>(var9, var10);
                    var8 = var11;
                    var6.Add(var8);
                }
                FixedList<LexicalElement, ExpressionSyntaxElement>[] var12;
                var12 = var6.ToArray();
                FixedList<ExpressionSyntaxElement, FixedList<LexicalElement, ExpressionSyntaxElement>[]> var13 = new FixedList<ExpressionSyntaxElement, FixedList<LexicalElement, ExpressionSyntaxElement>[]>(var5, var12);
                var4 = var13;
                var2 = var4;
            }
            LexicalElement var14 = _Reader.ReadOrThrow("arguments", LexType.ClosePar);
            return this.ReturnArguments(var1, var2, var14);
        }
        protected ExpressionSyntaxElement parseProperty() {
            LexicalElement var1 = _Reader.PeekOrThrow("property", LexType.Number, LexType.String, LexType.True, LexType.False, LexType.Null, LexType.Identifier, LexType.OpenBraces, LexType.Func, LexType.OpenPar);
            if(var1.Type == LexType.Number) {
                LexicalElement var2 = _Reader.ReadOrThrow("property", LexType.Number);
                return this.ReturnProperty_Number(var2);
            } else if(var1.Type == LexType.String) {
                LexicalElement var3 = _Reader.ReadOrThrow("property", LexType.String);
                return this.ReturnProperty_String(var3);
            } else if(var1.Type == LexType.True) {
                LexicalElement var4 = _Reader.ReadOrThrow("property", LexType.True);
                return this.ReturnProperty_True(var4);
            } else if(var1.Type == LexType.False) {
                LexicalElement var5 = _Reader.ReadOrThrow("property", LexType.False);
                return this.ReturnProperty_False(var5);
            } else if(var1.Type == LexType.Null) {
                LexicalElement var6 = _Reader.ReadOrThrow("property", LexType.Null);
                return this.ReturnProperty_Null(var6);
            } else if(var1.Type == LexType.Identifier) {
                IdentifierSyntaxElement var7 = this.parseIdentifier();
                return this.ReturnProperty(var7);
            } else if(var1.Type == LexType.OpenBraces) {
                ListSyntaxElement var8 = this.parseList();
                return this.ReturnProperty(var8);
            } else if(var1.Type == LexType.Func) {
                ExpressionSyntaxElement var9 = this.parseFunc();
                return this.ReturnProperty(var9);
            } else if(var1.Type == LexType.OpenPar) {
                ParenthesisSyntaxElement var10 = this.parseParenthesis();
                return this.ReturnProperty(var10);
            } else {
                throw _Reader.Error("property", LexType.Number, LexType.String, LexType.True, LexType.False, LexType.Null, LexType.Identifier, LexType.OpenBraces, LexType.Func, LexType.OpenPar);
            }
        }
        protected ParenthesisSyntaxElement parseParenthesis() {
            LexicalElement var1 = _Reader.ReadOrThrow("parenthesis", LexType.OpenPar);
            MultiExpressionSyntaxElement var2 = this.parseMultiExpression();
            LexicalElement var3 = _Reader.ReadOrThrow("parenthesis", LexType.ClosePar);
            return this.ReturnParenthesis(var1, var2, var3);
        }
        protected IdentifierSyntaxElement parseIdentifier() {
            LexicalElement var1 = _Reader.ReadOrThrow("identifier", LexType.Identifier);
            return this.ReturnIdentifier(var1);
        }
        protected ListSyntaxElement parseList() {
            LexicalElement var1 = _Reader.ReadOrThrow("list", LexType.OpenBraces);
            Optional<FixedList<ExpressionSyntaxElement, FixedList<LexicalElement, ExpressionSyntaxElement>[]>> var2 = default(Optional<FixedList<ExpressionSyntaxElement, FixedList<LexicalElement, ExpressionSyntaxElement>[]>>);
            Optional<LexicalElement> var3 = _Reader.Peek();
            if(var3.HasValue && (var3.Value.Type == LexType.Not || var3.Value.Type == LexType.Plus || var3.Value.Type == LexType.Minus || var3.Value.Type == LexType.PlusPlus || var3.Value.Type == LexType.MinusMinus || var3.Value.Type == LexType.Number || var3.Value.Type == LexType.String || var3.Value.Type == LexType.True || var3.Value.Type == LexType.False || var3.Value.Type == LexType.Null || var3.Value.Type == LexType.Identifier || var3.Value.Type == LexType.OpenBraces || var3.Value.Type == LexType.Func || var3.Value.Type == LexType.OpenPar)) {
                FixedList<ExpressionSyntaxElement, FixedList<LexicalElement, ExpressionSyntaxElement>[]> var4 = default(FixedList<ExpressionSyntaxElement, FixedList<LexicalElement, ExpressionSyntaxElement>[]>);
                ExpressionSyntaxElement var5 = this.parseExpression();
                List<FixedList<LexicalElement, ExpressionSyntaxElement>> var6 = new List<FixedList<LexicalElement, ExpressionSyntaxElement>>();
                for(Optional<LexicalElement> var7 = _Reader.Peek(); var7.HasValue && (var7.Value.Type == LexType.Comma); var7 = _Reader.Peek()) {
                    FixedList<LexicalElement, ExpressionSyntaxElement> var8 = default(FixedList<LexicalElement, ExpressionSyntaxElement>);
                    LexicalElement var9 = _Reader.ReadOrThrow("list", LexType.Comma);
                    ExpressionSyntaxElement var10 = this.parseExpression();
                    FixedList<LexicalElement, ExpressionSyntaxElement> var11 = new FixedList<LexicalElement, ExpressionSyntaxElement>(var9, var10);
                    var8 = var11;
                    var6.Add(var8);
                }
                FixedList<LexicalElement, ExpressionSyntaxElement>[] var12;
                var12 = var6.ToArray();
                FixedList<ExpressionSyntaxElement, FixedList<LexicalElement, ExpressionSyntaxElement>[]> var13 = new FixedList<ExpressionSyntaxElement, FixedList<LexicalElement, ExpressionSyntaxElement>[]>(var5, var12);
                var4 = var13;
                var2 = var4;
            }
            LexicalElement var14 = _Reader.ReadOrThrow("list", LexType.CloseBraces);
            return this.ReturnList(var1, var2, var14);
        }
        protected ExpressionSyntaxElement parseFunc() {
            LexicalElement var1 = _Reader.ReadOrThrow("func", LexType.Func);
            LexicalElement var2 = _Reader.ReadOrThrow("func", LexType.OpenPar);
            Optional<FixedList<Optional<LexicalElement>, Optional<LexicalElement>, IdentifierSyntaxElement, FixedList<LexicalElement, Optional<LexicalElement>, Optional<LexicalElement>, IdentifierSyntaxElement>[]>> var3 = default(Optional<FixedList<Optional<LexicalElement>, Optional<LexicalElement>, IdentifierSyntaxElement, FixedList<LexicalElement, Optional<LexicalElement>, Optional<LexicalElement>, IdentifierSyntaxElement>[]>>);
            Optional<LexicalElement> var4 = _Reader.Peek();
            if(var4.HasValue && (var4.Value.Type == LexType.Params || var4.Value.Type == LexType.Var || var4.Value.Type == LexType.Identifier)) {
                FixedList<Optional<LexicalElement>, Optional<LexicalElement>, IdentifierSyntaxElement, FixedList<LexicalElement, Optional<LexicalElement>, Optional<LexicalElement>, IdentifierSyntaxElement>[]> var5 = default(FixedList<Optional<LexicalElement>, Optional<LexicalElement>, IdentifierSyntaxElement, FixedList<LexicalElement, Optional<LexicalElement>, Optional<LexicalElement>, IdentifierSyntaxElement>[]>);
                Optional<LexicalElement> var6 = default(Optional<LexicalElement>);
                Optional<LexicalElement> var7 = _Reader.Peek();
                if(var7.HasValue && (var7.Value.Type == LexType.Params)) {
                    LexicalElement var8 = default(LexicalElement);
                    LexicalElement var9 = _Reader.ReadOrThrow("func", LexType.Params);
                    var8 = var9;
                    var6 = var8;
                }
                Optional<LexicalElement> var10 = default(Optional<LexicalElement>);
                Optional<LexicalElement> var11 = _Reader.Peek();
                if(var11.HasValue && (var11.Value.Type == LexType.Var)) {
                    LexicalElement var12 = default(LexicalElement);
                    LexicalElement var13 = _Reader.ReadOrThrow("func", LexType.Var);
                    var12 = var13;
                    var10 = var12;
                }
                IdentifierSyntaxElement var14 = this.parseIdentifier();
                List<FixedList<LexicalElement, Optional<LexicalElement>, Optional<LexicalElement>, IdentifierSyntaxElement>> var15 = new List<FixedList<LexicalElement, Optional<LexicalElement>, Optional<LexicalElement>, IdentifierSyntaxElement>>();
                for(Optional<LexicalElement> var16 = _Reader.Peek(); var16.HasValue && (var16.Value.Type == LexType.Comma); var16 = _Reader.Peek()) {
                    FixedList<LexicalElement, Optional<LexicalElement>, Optional<LexicalElement>, IdentifierSyntaxElement> var17 = default(FixedList<LexicalElement, Optional<LexicalElement>, Optional<LexicalElement>, IdentifierSyntaxElement>);
                    LexicalElement var18 = _Reader.ReadOrThrow("func", LexType.Comma);
                    Optional<LexicalElement> var19 = default(Optional<LexicalElement>);
                    Optional<LexicalElement> var20 = _Reader.Peek();
                    if(var20.HasValue && (var20.Value.Type == LexType.Params)) {
                        LexicalElement var21 = default(LexicalElement);
                        LexicalElement var22 = _Reader.ReadOrThrow("func", LexType.Params);
                        var21 = var22;
                        var19 = var21;
                    }
                    Optional<LexicalElement> var23 = default(Optional<LexicalElement>);
                    Optional<LexicalElement> var24 = _Reader.Peek();
                    if(var24.HasValue && (var24.Value.Type == LexType.Var)) {
                        LexicalElement var25 = default(LexicalElement);
                        LexicalElement var26 = _Reader.ReadOrThrow("func", LexType.Var);
                        var25 = var26;
                        var23 = var25;
                    }
                    IdentifierSyntaxElement var27 = this.parseIdentifier();
                    FixedList<LexicalElement, Optional<LexicalElement>, Optional<LexicalElement>, IdentifierSyntaxElement> var28 = new FixedList<LexicalElement, Optional<LexicalElement>, Optional<LexicalElement>, IdentifierSyntaxElement>(var18, var19, var23, var27);
                    var17 = var28;
                    var15.Add(var17);
                }
                FixedList<LexicalElement, Optional<LexicalElement>, Optional<LexicalElement>, IdentifierSyntaxElement>[] var29;
                var29 = var15.ToArray();
                FixedList<Optional<LexicalElement>, Optional<LexicalElement>, IdentifierSyntaxElement, FixedList<LexicalElement, Optional<LexicalElement>, Optional<LexicalElement>, IdentifierSyntaxElement>[]> var30 = new FixedList<Optional<LexicalElement>, Optional<LexicalElement>, IdentifierSyntaxElement, FixedList<LexicalElement, Optional<LexicalElement>, Optional<LexicalElement>, IdentifierSyntaxElement>[]>(var6, var10, var14, var29);
                var5 = var30;
                var3 = var5;
            }
            LexicalElement var31 = _Reader.ReadOrThrow("func", LexType.ClosePar);
            SyntaxElement var32 = this.parseBlock();
            return this.ReturnFunc(var1, var2, var3, var31, var32);
        }
    }
}
