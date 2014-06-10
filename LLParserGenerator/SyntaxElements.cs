using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace LLParserGenerator {
    class SourceElement {
        List<DefinitionElement> _defs = new List<DefinitionElement>();

        public List<DefinitionElement> Defs { get { return _defs; } }
        List<GeneratorLexElement> _comments = new List<GeneratorLexElement>();

        internal List<GeneratorLexElement> Comments { get { return _comments; } }
    }


    class DefinitionElement {
        static HashSet<string> _keywords = new HashSet<string>(new[] { "abstract", "event", "new", "struct", "as", "explicit", "null", "switch", "base", "extern", "object", "this", "bool", "false", "operator", "throw", "break", "finally", "out", "true", "byte", "fixed", "override", "try", "case", "float", "params", "typeof", "catch", "for", "private", "uint", "char", "foreach", "protected", "ulong", "checked", "goto", "public", "unchecked", "class", "if", "readonly", "unsafe", "const", "implicit", "ref", "ushort", "continue", "in", "return", "using", "decimal", "int", "sbyte", "virtual", "default", "interface", "sealed", "volatile", "delegate", "internal", "short", "void", "do", "is", "sizeof", "while", "double", "lock", "stackalloc", "else", "long", "static", "enum", "namespace", "string", "get", "partial", "set", "value", "where", "yield ", });
        /// <summary>
        /// 定義される非終端記号名を取得します
        /// </summary>
        public string DefinitionName { get { return _left.Text; } }
        GeneratorLexElement _left;
        /// <summary>
        /// 定義される非終端記号の元となる字句を取得します
        /// </summary>
        public GeneratorLexElement Left { get { return _left; } }
        ExpressionsElement _expression;
        /// <summary>
        /// 定義される式を返します
        /// </summary>
        public ExpressionsElement Expression { get { return _expression; } }
        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        /// <param name="left">非終端記号</param>
        /// <param name="expression">式</param>
        public DefinitionElement(GeneratorLexElement left, ExpressionsElement expression) {
            _left = left;
            _expression = expression;
        }
        public override string ToString() {
            return string.Format("{0}={1};", _left.Text, _expression);
        }
        string[] _firstTerminalsOfSelf = null;
        string[] _firstDependenciesOfSelf = null;
        /// <summary>
        /// この定義の中で先頭にくる可能性のある終端記号を返します
        /// </summary>
        /// <param name="generator"></param>
        /// <returns></returns>
        public IList<String> GetFirstTerminals(ScriptParserGenerator generator) {
            return this.Expression.GetFirstTerminals(generator);
        }
        protected internal IList<string> GetFirstTerminalsOfSelf(out string[] dependedDefinitions) {
            if(_firstDependenciesOfSelf != null && _firstDependenciesOfSelf != null) {
                dependedDefinitions = _firstDependenciesOfSelf;
                return _firstTerminalsOfSelf;
            }
            _firstTerminalsOfSelf = this.Expression.GetFirstTerminalsOfSelf(out dependedDefinitions).ToArray();
            _firstDependenciesOfSelf = dependedDefinitions;
            return _firstTerminalsOfSelf;
        }
        /// <summary>
        /// 各ルールから生成される結果用メソッドのシグネチャのリストを返します
        /// </summary>
        /// <param name="generator"></param>
        /// <returns></returns>
        public IList<MethodSignature> GetReturnParameterSignatures(ScriptParserGenerator generator) {
            IDictionary<ElementsElement, MethodSignature> tmp = this.GetReturnParameterSignaturesAndElements(generator);
            return tmp.Values.ToArray();
        }

        /// <summary>
        /// 各ルールから生成される結果用メソッドのシグネチャを，ルールの要素から牽けるようにしてを返します
        /// </summary>
        /// <param name="generator"></param>
        /// <returns></returns>
        public IDictionary<ElementsElement, MethodSignature> GetReturnParameterSignaturesAndElements(ScriptParserGenerator generator) {
            Dictionary<ElementsElement, MethodSignature> methodList = new Dictionary<ElementsElement, MethodSignature>();            
            foreach(ElementsElement elems in this.Expression.Selection.Candidates) {
                // ひとつのルールに対してメソッドを一個生成する
                MethodSignature method = new MethodSignature();
                foreach(ElementElement elem in elems.Elements) {
                    ParameterSignature sig = elem.GetReturnParameterSignature(generator);
                    sig.ParamName = sig.ParamName.TrimStart('_');
                    sig.ParamName = sig.ParamName.TrimEnd('_');
                    if(sig.ParamName.Length >= 1) {
                        if(char.IsUpper(sig.ParamName[0])) {
                            sig.ParamName = sig.ParamName[0].ToString().ToLower() + sig.ParamName.Substring(1);
                        }
                    }
                    method.Parameters.Add(sig);
                    method.AddEbnfString(elem.ToString());
                }
                // おなじParamNameを持つものを探す
                HashSet<string> duplicativeNames = new HashSet<string>(from sig in method.Parameters
                                                                       group sig.ParamName by sig.ParamName into nameSet
                                                                       where nameSet.Count() >= 2
                                                                       select nameSet.Key);
                Dictionary<string, int> countUpPerName = duplicativeNames.ToDictionary(n => n, n => 1);
                foreach(ParameterSignature sig in method.Parameters) {
                    if(duplicativeNames.Contains(sig.ParamName)) {
                        string name = sig.ParamName;
                        sig.ParamName = string.Format("{0}_{1}", name, countUpPerName[name]);
                        countUpPerName[name]++;
                    }
                }
                method.MethodName = generator.GetReturnMethodIdentifier(this.DefinitionName);
                methodList[elems] = method;
            }
            // 引数の重複性を確認する
            var groupBy = methodList.Values.GroupBy(method => method, new MethodSignatureParametersComparer());
            Dictionary<MethodSignature, List<MethodSignature>> duplicativeMethods = groupBy.ToDictionary(m => m.Key, m => m.ToList());
            // 同じ引数のメソッドがある場合にはそれぞれにパラメータから作成される識別子を後置する．
            foreach(List<MethodSignature> dupMethods in duplicativeMethods.Values) {
                if(dupMethods.Count >= 2) {
                    foreach(MethodSignature dupMethod in dupMethods) {
                        dupMethod.MethodName += "_" + dupMethod.GetMangledName();
                    }
                }
            }
            // C#のキーワードが識別子になる場合には頭に@を付ける
            foreach(MethodSignature method in methodList.Values) {
                foreach(ParameterSignature sig in method.Parameters) {
                    if(_keywords.Contains(sig.ParamName)) {
                        sig.ParamName = "@" + sig.ParamName;
                    }
                }
            }
            return methodList;
        }

        public void WriteParseCode(TextWriter writer, ScriptParserGenerator generator) {
            writer.Write(ScriptParserGenerator.GetAccessibilityString(generator.Settings.ParseMethodAccessibility));
            writer.Write(" ");
            writer.Write(generator.GetReturnClassIdentifier(this.DefinitionName));
            writer.Write(" ");
            writer.Write(generator.GetParseMethodIdentifier(this.DefinitionName));
            writer.WriteLine("() {");
            generator.IndentCount = 1;
            generator.InitializeVariableIndex(1);
            IDictionary<ElementsElement, MethodSignature> methods = this.GetReturnParameterSignaturesAndElements(generator);

            if(false) {
                // 普通のコード
                if(this.Expression.Selection.Candidates.Count >= 2) {
                    IList<string> firsts = this.Expression.Selection.GetFirstTerminals(generator);
                    string peekVar;
                    writer.WriteLine(generator.GetCodeOfPeekOrThrow(this.DefinitionName, firsts, out peekVar));
                    Dictionary<string, string> usedFirsts = new Dictionary<string, string>();
                    foreach(ElementsElement elems in this.Expression.Selection.Candidates) {
                        IList<string> innerFirsts = elems.GetFirstTerminals(generator);
                        foreach(string first in innerFirsts) {
                            string usingPoint;
                            if(usedFirsts.TryGetValue(first, out usingPoint)) {
                                string context = string.Format("'{0}' の定義", this.DefinitionName);
                                string message = string.Format("<{1}> 内の <{0}> は <{2}> によって隠されます", first, elems.ToString(), usingPoint);
                                generator.Warn(context, message);
                            } else {
                                usedFirsts[first] = elems.ToString();
                            }
                        }
                        writer.WriteLine(generator.GetCodeOfIfLexisIn(peekVar, innerFirsts));

                        List<string> arguments = new List<string>();
                        foreach(ElementElement elem in elems.Elements) {
                            string var = elem.WriteParseCode(writer, generator);
                            arguments.Add(var);
                        }
                        writer.WriteLine(generator.GetCodeOfReturnMethod(methods[elems].MethodName, arguments));
                        writer.Write(generator.GetCodeOfCloseBlock());
                        writer.Write(generator.GetCodeOfSingleElse());
                    }
                    writer.WriteLine(generator.GetCodeOfOpenBlock());

                    writer.WriteLine(generator.GetCodeOfThrowNew("System.NotImplementedException", ScriptParserGenerator.EscapeString(this.DefinitionName)));
                    writer.WriteLine(generator.GetCodeOfCloseBlock());
                } else {
                    ElementsElement elems = this.Expression.Selection.Candidates.First();
                    IList<string> innerFirsts = elems.GetFirstTerminals(generator);

                    List<string> arguments = new List<string>();
                    foreach(ElementElement elem in elems.Elements) {
                        string var = elem.WriteParseCode(writer, generator);
                        arguments.Add(var);
                    }
                    writer.WriteLine(generator.GetCodeOfReturnMethod(methods[elems].MethodName, arguments));
                }
            } else {
                // 前半部分が同じSelectionをツリー状に探索するようにするために
                // SelectionSubCandidateを作る
                List<SelectionSubCandidate> subCandidates = new List<SelectionSubCandidate>();
                foreach(ElementsElement candidate in this.Expression.Selection.Candidates) {
                    subCandidates.Add(new SelectionSubCandidate(candidate));
                }

                SelectionSubCandidate.writeParseCodeAux(writer, generator, subCandidates, (candidateAtEmpty, writer2, generator2) => {
                    writer.WriteLine(generator.GetCodeOfReturnMethod(methods[candidateAtEmpty.OriginalElements].MethodName, candidateAtEmpty.TemporaryVariables));
                });
            }
            writer.Write("}");
        }
        /// <summary>
        /// この要素に後続し得る終端記号．この要素が他の定義の末尾にあるときの依存関係を解決済み
        /// </summary>
        private string[] _followingTerminals;
        /// <summary>
        /// この要素に後続し得る終端記号．
        /// </summary>
        private List<string> _followingTerminalsOfSelf = new List<string>();
        /// <summary>
        /// この要素が末尾にくる可能性のある他の定義の名前
        /// </summary>
        private List<string> _followingDependencies = new List<string>();
        /// <summary>
        /// この定義に後続する可能性のある終端記号を追加します
        /// </summary>
        /// <param name="followings"></param>
        internal void AddFollowingTerminals(IEnumerable<string> followings) {
            _followingTerminalsOfSelf.AddRange(followings);
        }
        /// <summary>
        /// この定義に対応する非終端記号が末尾にくる可能性のある他の定義を追加します
        /// </summary>
        /// <param name="nonterminal"></param>
        internal void AddNonterminalContainingThisAtTail(string nonterminal) {
            _followingDependencies.Add(nonterminal);
        }
        /// <summary>
        /// 後続終端記号の情報を設定します
        /// </summary>
        public void SetupFollowingTerminals(ScriptParserGenerator generator) {
            if(_followingTerminalsSetupDone)
                return;
            _followingTerminalsSetupDone = true;
            _expression.SetupFollowingTerminals(generator, new string[] { null });
        }
        bool _followingTerminalsSetupDone = false;
        /// <summary>
        /// この定義に後続する終端記号を返します．
        /// </summary>
        /// <param name="generator"></param>
        /// <returns></returns>
        public IList<string> GetFollowingTerminals(ScriptParserGenerator generator) {
            generator.InitializeFollowingTerminals();
            if(_followingTerminals != null) {
                return new ReadOnlyCollection<string>(_followingTerminals);
            }
            List<string> ret = new List<string>(_followingTerminalsOfSelf.Where(f => f != null));
            // 循環対策のために一時的に入れておく
            _followingTerminals = ret.ToArray();
            foreach(string dependency in _followingDependencies) {
                DefinitionElement def = generator.LookUpDefinition(dependency);
                ret.AddRange(def.GetFollowingTerminals(generator));
            }
            _followingTerminals = ret.Distinct().ToArray();
            return new ReadOnlyCollection<string>(_followingTerminals);
        }
    }

    class ExpressionsElement : DefinitionContent {
        SelectionElement _selection;
        public SelectionElement Selection { get { return _selection; } }
        public ExpressionsElement(SelectionElement selection) {
            _selection = selection;
        }
        public override string ToString() {
            return _selection.ToString();
        }

        protected internal override IList<string> GetFirstTerminalsOfSelf(out string[] dependedDefinitions) {
            return this.Selection.GetFirstTerminalsOfSelf(out dependedDefinitions);
        }

        public override ParameterSignature GetReturnParameterSignature(ScriptParserGenerator generator) {
            return Selection.GetReturnParameterSignature(generator);
        }

        public override string WriteParseCode(TextWriter writer, ScriptParserGenerator generator) {
            return _selection.WriteParseCode(writer, generator);
        }


        protected override void SetupFollowingTerminalsOfSelf(ScriptParserGenerator generator, string[] followings) {
            _selection.SetupFollowingTerminals(generator, followings);
        }

        public override IEnumerable<DefinitionContent> EnumerateChildren() {
            return new DefinitionContent[] { _selection };
        }
    }
    class SelectionElement : DefinitionContent {
        List<ElementsElement> _candidates;
        public List<ElementsElement> Candidates { get { return _candidates; } }

        public SelectionElement(IList<ElementsElement> candidates) {
            _candidates = new List<ElementsElement>(candidates);
        }
        public override string ToString() {
            return _candidates.Select(p => p.ToString()).Aggregate((a, b) => a + "|" + b);
        }

        protected internal override IList<string> GetFirstTerminalsOfSelf(out string[] dependedDefinitions) {
            List<string> ret = new List<string>();
            List<string> dep = new List<string>();
            foreach(Optional<ElementsElement> elem in this.Candidates) {
                if(elem.HasValue) {
                    string[] depTmp;
                    ret.AddRange(elem.Value.GetFirstTerminalsOfSelf(out depTmp));
                    dep.AddRange(depTmp);
                } else {
                    ret.Add(null);
                }
            }
            dependedDefinitions = dep.ToArray();
            return ret;
        }
        public override ParameterSignature GetReturnParameterSignature(ScriptParserGenerator generator) {
            bool tmp;
            return this.GetReturnParameterSignature(generator, out tmp);
        }

        public ParameterSignature GetReturnParameterSignature(ScriptParserGenerator generator, out bool oneClassType) {
            Debug.Assert(_candidates.Count >= 1);
            List<ParameterSignature> sigList = new List<ParameterSignature>();
            HashSet<ParameterSignature> exists = new HashSet<ParameterSignature>(new ParameterSignatureComparer());
            foreach(ElementsElement elems in _candidates) {
                ParameterSignature sig = elems.GetReturnParameterSignature(generator);
                if(!exists.Contains(sig)) {
                    sigList.Add(sig);
                    exists.Add(sig);
                }
            }
            Debug.Assert(sigList.Count >= 1);
            if(sigList.Count == 1) {
                oneClassType = true;
                return sigList.First();
            }
            oneClassType = false;
            ParameterSignature ret = new ParameterSignature("selection", SignatureType.Selection, sigList);
            return ret;
        }


        public override string WriteParseCode(TextWriter writer, ScriptParserGenerator generator) {
            if(false) {
                // 普通のコード
                Debug.Assert(_candidates.Count >= 1);
                if(_candidates.Count == 1) {
                    return _candidates.First().WriteParseCode(writer, generator);
                } else {
                    IList<string> firsts = this.GetFirstTerminals(generator);
                    string peekVar;
                    writer.WriteLine(generator.GetCodeOfPeekOrThrow(this.RootDefinition.DefinitionName, firsts, out peekVar));
                    bool oneClassType;
                    ParameterSignature returnParam = GetReturnParameterSignature(generator, out oneClassType);
                    string returnType = returnParam.GetTypeName();
                    string returnVar;
                    writer.WriteLine(generator.GetCodeOfDeclareDefault(returnType, out returnVar));
                    Dictionary<string, string> usedFirsts = new Dictionary<string, string>();
                    foreach(ElementsElement elems in _candidates) {
                        IList<string> innerFirsts = elems.GetFirstTerminals(generator);
                        foreach(string first in innerFirsts) {
                            string usingPoint;
                            if(usedFirsts.TryGetValue(first, out usingPoint)) {
                                string context = string.Format("'{0}' の定義", this.RootDefinition.DefinitionName);
                                string message = string.Format("<{1}> 内の <{0}> は <{2}> によって隠されます", first, this.ToString(), usingPoint);
                                generator.Warn(context, message);
                            } else {
                                usedFirsts[first] = elems.ToString();
                            }
                        }
                        writer.WriteLine(generator.GetCodeOfIfLexisIn(peekVar, innerFirsts));
                        string candidateVar = elems.WriteParseCode(writer, generator);
                        if(oneClassType) {
                            writer.WriteLine(generator.GetCodeOfSubstitution(returnVar, candidateVar));
                        } else {
                            string selectVar;
                            writer.WriteLine(generator.GetCodeOfDeclareNew(returnType, new[] { candidateVar }, out selectVar));

                            writer.WriteLine(generator.GetCodeOfSubstitution(returnVar, selectVar));
                        }
                        writer.Write(generator.GetCodeOfCloseBlock());
                        writer.Write(generator.GetCodeOfSingleElse());
                    }
                    writer.Write(generator.GetCodeOfOpenBlock());
                    writer.WriteLine(generator.GetCodeOfCloseBlock());
                    return returnVar;
                }
            } else {
                // 前半部分が同じSelectionをツリー状に探索するようにするために
                // SelectionSubCandidateを作る
                List<SelectionSubCandidate> subCandidates = new List<SelectionSubCandidate>();
                foreach(ElementsElement candidate in _candidates) {
                    subCandidates.Add(new SelectionSubCandidate(candidate));
                }
                bool oneClassType;
                ParameterSignature returnParam = this.GetReturnParameterSignature(generator, out oneClassType);
                string returnVariableName;
                writer.WriteLine(generator.GetCodeOfDeclareDefault(returnParam.GetTypeName(), out returnVariableName));

                SelectionSubCandidate.writeParseCodeAux(writer, generator, subCandidates, (candidateAtEmpty, writer2, generator2) => {
                    // 候補の要素が単一要素の場合はFixedListをnewしなくてよいので分ける
                    string resultVar;
                    if(candidateAtEmpty.OriginalElements.Elements.Count == 1) {
                        resultVar = candidateAtEmpty.TemporaryVariables.First();
                    } else {
                        ParameterSignature returnParam2 = candidateAtEmpty.OriginalElements.GetReturnParameterSignature(generator2);
                        string typename = returnParam2.GetTypeName();
                        writer2.WriteLine(generator2.GetCodeOfDeclareNew(typename, candidateAtEmpty.TemporaryVariables, out resultVar));
                    }
                    bool oneClassReturn;
                    ParameterSignature tmpReturnParam = this.GetReturnParameterSignature(generator2, out oneClassReturn);
                    if(!oneClassReturn) {
                        // 一回Selection<>を作成しないといけない
                        string tmpTypename = tmpReturnParam.GetTypeName();
                        string tmpResultVar;
                        writer.WriteLine(generator2.GetCodeOfDeclareNew(tmpTypename, new[] { resultVar }, out tmpResultVar));
                        resultVar = tmpResultVar;
                    }
                    writer2.WriteLine(generator.GetCodeOfSubstitution(returnVariableName, resultVar));
                });
                return returnVariableName;
            }
        }



        protected override void SetupFollowingTerminalsOfSelf(ScriptParserGenerator generator, string[] followings) {
            foreach(ElementsElement elems in _candidates) {
                elems.SetupFollowingTerminals(generator, followings);
            }
        }

        public override IEnumerable<DefinitionContent> EnumerateChildren() {
            return _candidates.Select(c => (DefinitionContent)c);
        }
    }
    class ElementsElement : DefinitionContent {
        List<ElementElement> _elements;
        public List<ElementElement> Elements { get { return _elements; } }

        public ElementsElement(IList<ElementElement> elements) {
            _elements = new List<ElementElement>(elements);
        }
        public override string ToString() {
            return _elements.Select(p => p.ToString()).Aggregate((a, b) => a + ", " + b);
        }


        protected internal override IList<string> GetFirstTerminalsOfSelf(out string[] dependedDefinitions) {
            List<string> ret = new List<string>();
            List<string> dep = new List<string>();
            foreach(ElementElement elem in this.Elements) {
                string[] depTmp;
                IList<string> res = elem.GetFirstTerminalsOfSelf(out depTmp);
                dep.AddRange(depTmp);
                ret.AddRange(res.Where(r => r != null));
                if(!res.Contains(null)) {
                    dependedDefinitions = dep.ToArray();
                    return ret;
                }
            }
            ret.Add(null);
            dependedDefinitions = dep.ToArray();
            return ret;
        }

        public override ParameterSignature GetReturnParameterSignature(ScriptParserGenerator generator) {
            Debug.Assert(_elements.Count >= 1);
            if(_elements.Count == 1) {
                return _elements.First().GetReturnParameterSignature(generator);
            } else {
                List<ParameterSignature> list = new List<ParameterSignature>();
                foreach(ElementElement elems in _elements) {
                    list.Add(elems.GetReturnParameterSignature(generator));
                }
                string name = list.Select(s => s.ParamName).Aggregate((a, b) => a + "_" + b);
                return new ParameterSignature(name, SignatureType.FixedList, list);
            }
        }


        public override string WriteParseCode(TextWriter writer, ScriptParserGenerator generator) {
            Debug.Assert(_elements.Count >= 1);
            if(_elements.Count == 1) {
                return _elements.First().WriteParseCode(writer, generator);
            } else {
                List<string> innerVars = new List<string>();
                foreach(ElementElement elem in _elements) {
                    string var = elem.WriteParseCode(writer, generator);
                    innerVars.Add(var);
                }
                ParameterSignature returnParam = this.GetReturnParameterSignature(generator);
                string returnType = returnParam.GetTypeName();
                string returnVar;
                writer.WriteLine(generator.GetCodeOfDeclareNew(returnType, innerVars, out returnVar));
                return returnVar;
            }
        }

        protected override void SetupFollowingTerminalsOfSelf(ScriptParserGenerator generator, string[] followings) {
            for(int i = _elements.Count - 1; i >= 0; i--) {
                // 後ろからfollowingsを設定
                _elements[i].SetupFollowingTerminals(generator, followings);
                IList<string> tmp = _elements[i].GetFirstTerminals(generator);
                // 注目対象のElementがnullになりうるときは後続のfollowingsを加える
                if(tmp.Contains(null)) {
                    // followingsにはnullが入っていることもあるけど除かない
                    followings = tmp.Where(t => t != null).Union(followings).ToArray();
                } else {
                    followings = tmp.ToArray();
                }
            }
        }

        public override IEnumerable<DefinitionContent> EnumerateChildren() {
            return _elements.Select(e => (DefinitionContent)e);
        }
    }
    abstract class ElementElement : DefinitionContent {
    }
    class RepeatElement : ElementElement {
        ExpressionsElement _innerExpression;
        public ExpressionsElement InnerExpression { get { return _innerExpression; } }
        public RepeatElement(ExpressionsElement innerExpression) {
            _innerExpression = innerExpression;
        }
        public override string ToString() {
            return "{" + _innerExpression.ToString() + "}";
        }

        protected internal override IList<string> GetFirstTerminalsOfSelf(out string[] dependedDefinitions) {
            List<string> ret = new List<string>(_innerExpression.GetFirstTerminalsOfSelf(out dependedDefinitions));
            ret.Add(null);
            return ret;
        }
        public override ParameterSignature GetReturnParameterSignature(ScriptParserGenerator generator) {
            ParameterSignature inner = _innerExpression.GetReturnParameterSignature(generator);

            return new ParameterSignature("repetition", SignatureType.Array, inner);
        }


        public override string WriteParseCode(TextWriter writer, ScriptParserGenerator generator) {
            IList<string> firsts = _innerExpression.GetFirstTerminals(generator);
            IList<string> followings = this.GetFollowingTerminals(generator);
            HashSet<string> firstSet = new HashSet<string>(firsts);
            foreach(string following in followings) {
                if(firstSet.Contains(following)) {
                    string context = string.Format("'{0}' の定義", this.RootDefinition.DefinitionName);
                    string message = string.Format("<{1}> の直後の <{0}> は <{1}> によって隠されます", following, this.ToString());
                    generator.Warn(context, message);
                }
            }
            ParameterSignature returnParam = this.GetReturnParameterSignature(generator);
            ParameterSignature innerParam = _innerExpression.GetReturnParameterSignature(generator);
            string innerType = innerParam.GetTypeName();
            string innerListType = string.Format("List<{0}>", innerType);
            string innerListVar;
            writer.WriteLine(generator.GetCodeOfDeclareNew(innerListType, out innerListVar));
            string peek;

            writer.WriteLine(generator.GetCodeOfForPeekLexisIn(firsts, out peek));
            string innerVar = _innerExpression.WriteParseCode(writer, generator);
            writer.WriteLine(generator.GetCodeOfInvoke(innerListVar, "Add", innerVar));
            writer.WriteLine(generator.GetCodeOfCloseBlock());
            string returnType = returnParam.GetTypeName();
            string returnVar;
            writer.WriteLine(generator.GetCodeOfDeclare(returnType, out returnVar));
            writer.WriteLine(generator.GetCodeOfInvokeSubstitute(returnVar, innerListVar, "ToArray"));
            return returnVar;
        }

        protected override void SetupFollowingTerminalsOfSelf(ScriptParserGenerator generator, string[] followings) {
            followings = followings.Union(this.GetFirstTerminals(generator).Where(f => f != null)).ToArray();
            _innerExpression.SetupFollowingTerminals(generator, followings);
        }

        public override IEnumerable<DefinitionContent> EnumerateChildren() {
            return new DefinitionContent[] { _innerExpression };
        }
    }
    class OptionElement : ElementElement {
        ExpressionsElement _innerExpression;
        public ExpressionsElement InnerExpression { get { return _innerExpression; } }
        public OptionElement(ExpressionsElement innerExpression) {
            _innerExpression = innerExpression;
        }
        public override string ToString() {
            return "[" + _innerExpression.ToString() + "]";
        }


        protected internal override IList<string> GetFirstTerminalsOfSelf(out string[] dependedDefinitions) {
            List<string> ret = new List<string>(_innerExpression.GetFirstTerminalsOfSelf(out dependedDefinitions));
            ret.Add(null);
            return ret;
        }
        public override ParameterSignature GetReturnParameterSignature(ScriptParserGenerator generator) {
            ParameterSignature inner = _innerExpression.GetReturnParameterSignature(generator);
            string name = string.Format("{0}_opt", inner.ParamName);
            return new ParameterSignature(name, SignatureType.Optional, inner);
        }


        public override string WriteParseCode(TextWriter writer, ScriptParserGenerator generator) {
            IList<string> firsts = _innerExpression.GetFirstTerminals(generator);
            IList<string> followings = this.GetFollowingTerminals(generator);
            HashSet<string> firstSet = new HashSet<string>(firsts);
            foreach(string following in followings) {
                if(firstSet.Contains(following)) {
                    string context = string.Format("'{0}' の定義", this.RootDefinition.DefinitionName);
                    string message = string.Format("省略可能な <{1}> の直後に <{0}> がありますが <{1}> の方が優先されます", following, this.ToString());
                    generator.Warn(context, message);
                }
            }

            ParameterSignature returnParam = this.GetReturnParameterSignature(generator);
            string returnType = returnParam.GetTypeName();
            string returnVar;
            writer.WriteLine(generator.GetCodeOfDeclareDefault(returnType, out returnVar));
            string peekVar;
            writer.WriteLine(generator.GetCodeOfPeek(out peekVar));
            writer.WriteLine(generator.GetCodeOfIfOptionalLexisIn(peekVar, firsts));
            string innerVar = _innerExpression.WriteParseCode(writer, generator);
            writer.WriteLine(generator.GetCodeOfSubstitution(returnVar, innerVar));
            writer.WriteLine(generator.GetCodeOfCloseBlock());
            return returnVar;
        }

        protected override void SetupFollowingTerminalsOfSelf(ScriptParserGenerator generator, string[] followings) {
            _innerExpression.SetupFollowingTerminals(generator, followings);
        }

        public override IEnumerable<DefinitionContent> EnumerateChildren() {
            return new DefinitionContent[] { _innerExpression };
        }
    }
    class GroupElement : ElementElement {
        ExpressionsElement _innerExpression;
        public ExpressionsElement InnerExpression { get { return _innerExpression; } }
        public GroupElement(ExpressionsElement innerExpression) {
            _innerExpression = innerExpression;
        }
        public override string ToString() {
            return "(" + _innerExpression.ToString() + ")";
        }

        protected internal override IList<string> GetFirstTerminalsOfSelf(out string[] dependedDefinitions) {
            List<string> ret = new List<string>(_innerExpression.GetFirstTerminalsOfSelf(out dependedDefinitions));
            return ret;
        }
        public override ParameterSignature GetReturnParameterSignature(ScriptParserGenerator generator) {
            ParameterSignature inner = _innerExpression.GetReturnParameterSignature(generator);
            return inner;
        }

        public override string WriteParseCode(TextWriter writer, ScriptParserGenerator generator) {
            return _innerExpression.WriteParseCode(writer, generator);
        }
        protected override void SetupFollowingTerminalsOfSelf(ScriptParserGenerator generator, string[] followings) {
            _innerExpression.SetupFollowingTerminals(generator, followings);
        }

        public override IEnumerable<DefinitionContent> EnumerateChildren() {
            return new DefinitionContent[] { _innerExpression };
        }
    }
    class LiteralElement : ElementElement {
        GeneratorLexElement _literal;
        /// <summary>
        /// 内部の文字列を取得します
        /// </summary>
        public string InnerWord {
            get {
                switch(_literal.Type) {
                case GeneratorLexType.Terminal:
                    return _literal.Text.Substring(1, _literal.Text.Length - 2);
                case GeneratorLexType.Nonterminal:
                    return _literal.Text;
                default:
                    throw new NotSupportedException();
                }
            }
        }
        /// <summary>
        /// 基となる字句要素を取得します
        /// </summary>
        public GeneratorLexElement Literal { get { return _literal; } }
        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        /// <param name="literal"></param>
        public LiteralElement(GeneratorLexElement literal) {
            _literal = literal;
        }
        public override string ToString() {
            return _literal.Text;
        }

        protected internal override IList<string> GetFirstTerminalsOfSelf(out string[] dependedDefinitions) {
            List<string> ret = new List<string>();
            List<string> dep = new List<string>();
            switch(this.Literal.Type) {
            case GeneratorLexType.Terminal:
                ret.Add(this.InnerWord);
                break;
            case GeneratorLexType.Nonterminal:
                dep.Add(this.InnerWord);
                break;
            default:
                throw new NotSupportedException();
            }
            dependedDefinitions = dep.ToArray();
            return ret.ToArray();
        }
        public override ParameterSignature GetReturnParameterSignature(ScriptParserGenerator generator) {
            switch(_literal.Type) {
            case GeneratorLexType.Terminal:
                return new ParameterSignature(generator.GetTerminalName(this.InnerWord), generator.Settings.InputClass);
            case GeneratorLexType.Nonterminal:
                return new ParameterSignature(ScriptParserGenerator.ConvertCase(this.InnerWord, generator.Settings.NonterminalCaseConversion), generator.GetReturnClassIdentifier(this.InnerWord));
            default:
                throw new NotSupportedException();
            }
        }


        public override string WriteParseCode(TextWriter writer, ScriptParserGenerator generator) {
            string var;
            switch(_literal.Type) {
            case GeneratorLexType.Terminal:
                writer.WriteLine(generator.GetCodeOfReadOrThrow(this.RootDefinition.DefinitionName, new string[] { this.InnerWord }, out var));
                return var;
            case GeneratorLexType.Nonterminal:
                writer.WriteLine(generator.GetCodeOfParseNonterminal(this.InnerWord, out var));
                return var;
            default:
                throw new NotSupportedException();
            }
        }

        protected override void SetupFollowingTerminalsOfSelf(ScriptParserGenerator generator, string[] followings) {
            switch(_literal.Type) {
            case GeneratorLexType.Terminal:
                break;
            case GeneratorLexType.Nonterminal:
                DefinitionElement def = generator.LookUpDefinition(this.InnerWord);
                def.AddFollowingTerminals(followings);
                if(followings.Contains(null)) {
                    def.AddNonterminalContainingThisAtTail(this.RootDefinition.DefinitionName);
                }
                break;
            default:
                throw new NotSupportedException();
            }
        }

        public override IEnumerable<DefinitionContent> EnumerateChildren() {
            return new DefinitionContent[0];
        }
    }




}