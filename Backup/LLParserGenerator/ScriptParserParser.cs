using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
namespace LLParserGenerator {
    class ScriptParserParser : ScriptParserParserBase {
        /// <summary>
        /// source = {definition|"Comment"} ;
        /// </summary>
        /// <param name="repetition">{definition|"Comment"}</param>
        protected override SourceElement ReturnSource(Selection<DefinitionElement, GeneratorLexElement>[] repetition) {
            SourceElement ret = new SourceElement();
            Dictionary<string, List<DefinitionElement>> defSet = new Dictionary<string, List<DefinitionElement>>();
            foreach(var selection in repetition) {
                if(selection.Element1.HasValue) {
                    List<DefinitionElement> defList;
                    if(!defSet.TryGetValue(selection.Element1.Value.DefinitionName, out defList)) {
                        defSet[selection.Element1.Value.DefinitionName] = defList = new List<DefinitionElement>();
                    }
                    defList.Add(selection.Element1.Value);
                } else {
                    Debug.Assert(selection.Element2.HasValue);
                    ret.Comments.Add(selection.Element2.Value);
                }
            }
            foreach(List<DefinitionElement> defs in defSet.Values) {
                Debug.Assert(defs.Count > 0);
                List<ElementsElement> elements = new List<ElementsElement>();
                foreach(DefinitionElement def in defs) {
                    elements.AddRange(def.Expression.Selection.Candidates);
                }
                SelectionElement select = new SelectionElement(elements);
                ret.Defs.Add(new DefinitionElement(defs.First().Left, new ExpressionsElement(select)));
            }
            return ret;
        }

        /// <summary>
        /// definition = 'nonterminal', "=", expressions, ";" ;
        /// </summary>
        /// <param name="nonterminal">'nonterminal'</param>
        /// <param name="equal">"="</param>
        /// <param name="expressions">expressions</param>
        /// <param name="semicolon">";"</param>
        protected override DefinitionElement ReturnDefinition(GeneratorLexElement nonterminal, GeneratorLexElement equal, ExpressionsElement expressions, GeneratorLexElement semicolon) {
            DefinitionElement ret = new DefinitionElement(nonterminal, expressions);
            ret.Expression.setRootDefinition(ret);
            return ret;
        }

        /// <summary>
        /// expressions = selection ;
        /// </summary>
        /// <param name="selection">selection</param>
        protected override ExpressionsElement ReturnExpressions(SelectionElement selection) {
            return new ExpressionsElement(selection);
        }

        /// <summary>
        /// selection = elements, {'|', elements} ;
        /// </summary>
        /// <param name="elements">elements</param>
        /// <param name="repetition">{'|', elements}</param>
        protected override ElementsElement ReturnElements(ElementElement element, FixedList<GeneratorLexElement, ElementElement>[] repetition) {
            List<ElementElement> list = new List<ElementElement>();
            list.Add(element);
            list.AddRange(repetition.Select(p => p.Element2));
            return new ElementsElement(list);
        }

        /// <summary>
        /// element = repeat ;
        /// </summary>
        /// <param name="repeat">repeat</param>
        protected override ElementElement ReturnElement(RepeatElement repeat) {
            return repeat;
        }

        /// <summary>
        /// element = option ;
        /// </summary>
        /// <param name="option">option</param>
        protected override ElementElement ReturnElement(OptionElement option) {
            return option;
        }

        /// <summary>
        /// element = group ;
        /// </summary>
        /// <param name="group">group</param>
        protected override ElementElement ReturnElement(GroupElement group) {
            return group;
        }

        /// <summary>
        /// element = literal ;
        /// </summary>
        /// <param name="literal">literal</param>
        protected override ElementElement ReturnElement(LiteralElement literal) {
            return literal;
        }

        /// <summary>
        /// selection = elements, {'|', elements} ;
        /// </summary>
        /// <param name="elements">elements</param>
        /// <param name="repetition">{'|', elements}</param>
        protected override SelectionElement ReturnSelection(ElementsElement elements, FixedList<GeneratorLexElement, ElementsElement>[] repetition) {
            List<ElementsElement> list = new List<ElementsElement>();
            list.Add(elements);
            list.AddRange(repetition.Select(p => p.Element2));
            return new SelectionElement(list);
        }

        /// <summary>
        /// repeat = '{', expressions, '}' ;
        /// </summary>
        /// <param name="openBrace">'{'</param>
        /// <param name="expressions">expressions</param>
        /// <param name="closeBrace">'}'</param>
        protected override RepeatElement ReturnRepeat(GeneratorLexElement openBrace, ExpressionsElement expressions, GeneratorLexElement closeBrace) {
            return new RepeatElement(expressions);
        }

        /// <summary>
        /// option = '[', expressions, ']' ;
        /// </summary>
        /// <param name="openBracket">'['</param>
        /// <param name="expressions">expressions</param>
        /// <param name="closeBracket">']'</param>
        protected override OptionElement ReturnOption(GeneratorLexElement openBrace, ExpressionsElement expressions, GeneratorLexElement closeBrace) {
            return new OptionElement(expressions);
        }

        /// <summary>
        /// group = '(', expressions, ')' ;
        /// </summary>
        /// <param name="openPar">'('</param>
        /// <param name="expressions">expressions</param>
        /// <param name="closePar">')'</param>
        protected override GroupElement ReturnGroup(GeneratorLexElement openBrace, ExpressionsElement expressions, GeneratorLexElement closeBrace) {
            return new GroupElement(expressions);
        }

        public ScriptParserParser()
            : base() {
        }

        /// <summary>
        /// literal = 'terminal' ;
        /// </summary>
        /// <param name="terminal">'terminal'</param>
        protected override LiteralElement ReturnLiteral_Terminal(GeneratorLexElement terminal) {
            return new LiteralElement(terminal);
        }

        /// <summary>
        /// literal = 'nonterminal' ;
        /// </summary>
        /// <param name="nonterminal">'nonterminal'</param>
        protected override LiteralElement ReturnLiteral_Nonterminal(GeneratorLexElement nonterminal) {
            return new LiteralElement(nonterminal);
        }
    }

}
