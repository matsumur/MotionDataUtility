// Auto generated with LLParserGenerator.ScriptParserGenerator
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace LLParserGenerator {
    abstract class ScriptParserParserBase {
        protected ScriptParserParserBase() { }
        public SourceElement StartParse(IEnumerable<GeneratorLexElement> source) {
            _Reader = new Reader<GeneratorLexElement, GeneratorLexType>(source);
            return this.ParseSource();
        }
        protected Reader<GeneratorLexElement, GeneratorLexType> _Reader;
        /// <summary>
        /// source = {definition|"Comment"} ;
        /// </summary>
        /// <param name="repetition">{definition|"Comment"}</param>
        protected abstract SourceElement ReturnSource(Selection<DefinitionElement, GeneratorLexElement>[] repetition);

        /// <summary>
        /// definition = 'nonterminal', "=", expressions, ";" ;
        /// </summary>
        /// <param name="nonterminal">'nonterminal'</param>
        /// <param name="equal">"="</param>
        /// <param name="expressions">expressions</param>
        /// <param name="semicolon">";"</param>
        protected abstract DefinitionElement ReturnDefinition(GeneratorLexElement nonterminal, GeneratorLexElement equal, ExpressionsElement expressions, GeneratorLexElement semicolon);

        /// <summary>
        /// expressions = selection ;
        /// </summary>
        /// <param name="selection">selection</param>
        protected abstract ExpressionsElement ReturnExpressions(SelectionElement selection);

        /// <summary>
        /// selection = elements, {'|', elements} ;
        /// </summary>
        /// <param name="elements">elements</param>
        /// <param name="repetition">{'|', elements}</param>
        protected abstract SelectionElement ReturnSelection(ElementsElement elements, FixedList<GeneratorLexElement, ElementsElement>[] repetition);

        /// <summary>
        /// elements = element, {',', element} ;
        /// </summary>
        /// <param name="element">element</param>
        /// <param name="repetition">{',', element}</param>
        protected abstract ElementsElement ReturnElements(ElementElement element, FixedList<GeneratorLexElement, ElementElement>[] repetition);

        /// <summary>
        /// element = repeat ;
        /// </summary>
        /// <param name="repeat">repeat</param>
        protected abstract ElementElement ReturnElement(RepeatElement repeat);

        /// <summary>
        /// element = option ;
        /// </summary>
        /// <param name="option">option</param>
        protected abstract ElementElement ReturnElement(OptionElement option);

        /// <summary>
        /// element = group ;
        /// </summary>
        /// <param name="group">group</param>
        protected abstract ElementElement ReturnElement(GroupElement group);

        /// <summary>
        /// element = literal ;
        /// </summary>
        /// <param name="literal">literal</param>
        protected abstract ElementElement ReturnElement(LiteralElement literal);

        /// <summary>
        /// repeat = '{', expressions, '}' ;
        /// </summary>
        /// <param name="openBrace">'{'</param>
        /// <param name="expressions">expressions</param>
        /// <param name="closeBrace">'}'</param>
        protected abstract RepeatElement ReturnRepeat(GeneratorLexElement openBrace, ExpressionsElement expressions, GeneratorLexElement closeBrace);

        /// <summary>
        /// option = '[', expressions, ']' ;
        /// </summary>
        /// <param name="openBracket">'['</param>
        /// <param name="expressions">expressions</param>
        /// <param name="closeBracket">']'</param>
        protected abstract OptionElement ReturnOption(GeneratorLexElement openBracket, ExpressionsElement expressions, GeneratorLexElement closeBracket);

        /// <summary>
        /// group = '(', expressions, ')' ;
        /// </summary>
        /// <param name="openPar">'('</param>
        /// <param name="expressions">expressions</param>
        /// <param name="closePar">')'</param>
        protected abstract GroupElement ReturnGroup(GeneratorLexElement openPar, ExpressionsElement expressions, GeneratorLexElement closePar);

        /// <summary>
        /// literal = 'terminal' ;
        /// </summary>
        /// <param name="terminal">'terminal'</param>
        protected abstract LiteralElement ReturnLiteral_Terminal(GeneratorLexElement terminal);

        /// <summary>
        /// literal = 'nonterminal' ;
        /// </summary>
        /// <param name="nonterminal">'nonterminal'</param>
        protected abstract LiteralElement ReturnLiteral_Nonterminal(GeneratorLexElement nonterminal);

        protected SourceElement ParseSource() {
            List<Selection<DefinitionElement, GeneratorLexElement>> var1 = new List<Selection<DefinitionElement, GeneratorLexElement>>();
            for(Optional<GeneratorLexElement> var2 = _Reader.Peek(); var2.HasValue && (var2.Value.Type == GeneratorLexType.Comment || var2.Value.Type == GeneratorLexType.Nonterminal); var2 = _Reader.Peek()) {
                Selection<DefinitionElement, GeneratorLexElement> var3 = default(Selection<DefinitionElement, GeneratorLexElement>);
                GeneratorLexElement var4 = _Reader.PeekOrThrow("source", GeneratorLexType.Nonterminal, GeneratorLexType.Comment);
                if(var4.Type == GeneratorLexType.Nonterminal) {
                    DefinitionElement var5 = this.ParseDefinition();
                    Selection<DefinitionElement, GeneratorLexElement> var6 = new Selection<DefinitionElement, GeneratorLexElement>(var5);
                    var3 = var6;
                } else if(var4.Type == GeneratorLexType.Comment) {
                    GeneratorLexElement var7 = _Reader.ReadOrThrow("source", GeneratorLexType.Comment);
                    Selection<DefinitionElement, GeneratorLexElement> var8 = new Selection<DefinitionElement, GeneratorLexElement>(var7);
                    var3 = var8;
                } else {
                    throw _Reader.Error("source", GeneratorLexType.Nonterminal, GeneratorLexType.Comment);
                }
                var1.Add(var3);
            }
            Selection<DefinitionElement, GeneratorLexElement>[] var9;
            var9 = var1.ToArray();
            return this.ReturnSource(var9);
        }
        protected DefinitionElement ParseDefinition() {
            GeneratorLexElement var1 = _Reader.ReadOrThrow("definition", GeneratorLexType.Nonterminal);
            GeneratorLexElement var2 = _Reader.ReadOrThrow("definition", GeneratorLexType.Equal);
            ExpressionsElement var3 = this.ParseExpressions();
            GeneratorLexElement var4 = _Reader.ReadOrThrow("definition", GeneratorLexType.Semicolon);
            return this.ReturnDefinition(var1, var2, var3, var4);
        }
        protected ExpressionsElement ParseExpressions() {
            SelectionElement var1 = this.ParseSelection();
            return this.ReturnExpressions(var1);
        }
        protected SelectionElement ParseSelection() {
            ElementsElement var1 = this.ParseElements();
            List<FixedList<GeneratorLexElement, ElementsElement>> var2 = new List<FixedList<GeneratorLexElement, ElementsElement>>();
            for(Optional<GeneratorLexElement> var3 = _Reader.Peek(); var3.HasValue && (var3.Value.Type == GeneratorLexType.VSlash); var3 = _Reader.Peek()) {
                FixedList<GeneratorLexElement, ElementsElement> var4 = default(FixedList<GeneratorLexElement, ElementsElement>);
                GeneratorLexElement var5 = _Reader.ReadOrThrow("selection", GeneratorLexType.VSlash);
                ElementsElement var6 = this.ParseElements();
                FixedList<GeneratorLexElement, ElementsElement> var7 = new FixedList<GeneratorLexElement, ElementsElement>(var5, var6);
                var4 = var7;
                var2.Add(var4);
            }
            FixedList<GeneratorLexElement, ElementsElement>[] var8;
            var8 = var2.ToArray();
            return this.ReturnSelection(var1, var8);
        }
        protected ElementsElement ParseElements() {
            ElementElement var1 = this.ParseElement();
            List<FixedList<GeneratorLexElement, ElementElement>> var2 = new List<FixedList<GeneratorLexElement, ElementElement>>();
            for(Optional<GeneratorLexElement> var3 = _Reader.Peek(); var3.HasValue && (var3.Value.Type == GeneratorLexType.Comma); var3 = _Reader.Peek()) {
                FixedList<GeneratorLexElement, ElementElement> var4 = default(FixedList<GeneratorLexElement, ElementElement>);
                GeneratorLexElement var5 = _Reader.ReadOrThrow("elements", GeneratorLexType.Comma);
                ElementElement var6 = this.ParseElement();
                FixedList<GeneratorLexElement, ElementElement> var7 = new FixedList<GeneratorLexElement, ElementElement>(var5, var6);
                var4 = var7;
                var2.Add(var4);
            }
            FixedList<GeneratorLexElement, ElementElement>[] var8;
            var8 = var2.ToArray();
            return this.ReturnElements(var1, var8);
        }
        protected ElementElement ParseElement() {
            GeneratorLexElement var1 = _Reader.PeekOrThrow("element", GeneratorLexType.OpenBrace, GeneratorLexType.OpenBracket, GeneratorLexType.OpenPar, GeneratorLexType.Terminal, GeneratorLexType.Nonterminal);
            if(var1.Type == GeneratorLexType.OpenBrace) {
                RepeatElement var2 = this.ParseRepeat();
                return this.ReturnElement(var2);
            } else if(var1.Type == GeneratorLexType.OpenBracket) {
                OptionElement var3 = this.ParseOption();
                return this.ReturnElement(var3);
            } else if(var1.Type == GeneratorLexType.OpenPar) {
                GroupElement var4 = this.ParseGroup();
                return this.ReturnElement(var4);
            } else if(var1.Type == GeneratorLexType.Terminal || var1.Type == GeneratorLexType.Nonterminal) {
                LiteralElement var5 = this.ParseLiteral();
                return this.ReturnElement(var5);
            } else {
                throw _Reader.Error("element", GeneratorLexType.OpenBrace, GeneratorLexType.OpenBracket, GeneratorLexType.OpenPar, GeneratorLexType.Terminal, GeneratorLexType.Nonterminal);
            }
        }
        protected RepeatElement ParseRepeat() {
            GeneratorLexElement var1 = _Reader.ReadOrThrow("repeat", GeneratorLexType.OpenBrace);
            ExpressionsElement var2 = this.ParseExpressions();
            GeneratorLexElement var3 = _Reader.ReadOrThrow("repeat", GeneratorLexType.CloseBrace);
            return this.ReturnRepeat(var1, var2, var3);
        }
        protected OptionElement ParseOption() {
            GeneratorLexElement var1 = _Reader.ReadOrThrow("option", GeneratorLexType.OpenBracket);
            ExpressionsElement var2 = this.ParseExpressions();
            GeneratorLexElement var3 = _Reader.ReadOrThrow("option", GeneratorLexType.CloseBracket);
            return this.ReturnOption(var1, var2, var3);
        }
        protected GroupElement ParseGroup() {
            GeneratorLexElement var1 = _Reader.ReadOrThrow("group", GeneratorLexType.OpenPar);
            ExpressionsElement var2 = this.ParseExpressions();
            GeneratorLexElement var3 = _Reader.ReadOrThrow("group", GeneratorLexType.ClosePar);
            return this.ReturnGroup(var1, var2, var3);
        }
        protected LiteralElement ParseLiteral() {
            GeneratorLexElement var1 = _Reader.PeekOrThrow("literal", GeneratorLexType.Terminal, GeneratorLexType.Nonterminal);
            if(var1.Type == GeneratorLexType.Terminal) {
                GeneratorLexElement var2 = _Reader.ReadOrThrow("literal", GeneratorLexType.Terminal);
                return this.ReturnLiteral_Terminal(var2);
            } else if(var1.Type == GeneratorLexType.Nonterminal) {
                GeneratorLexElement var3 = _Reader.ReadOrThrow("literal", GeneratorLexType.Nonterminal);
                return this.ReturnLiteral_Nonterminal(var3);
            } else {
                throw _Reader.Error("literal", GeneratorLexType.Terminal, GeneratorLexType.Nonterminal);
            }
        }
    }
}
