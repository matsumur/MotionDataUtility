using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LLParserGenerator {
    abstract class DefinitionContent {
        private DefinitionElement _rootDefinition;
        public DefinitionElement RootDefinition { get { return _rootDefinition; } }

        internal void setRootDefinition(DefinitionElement definition) {
            _rootDefinition = definition;
            foreach(DefinitionContent child in this.EnumerateChildren()) {
                child.setRootDefinition(definition);
            }
        }
        /// <summary>
        /// 子要素のDefinitionContentを列挙します
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<DefinitionContent> EnumerateChildren();
        /// <summary>
        /// 先頭に来得る終端記号群．先頭に非終端記号が来得る場合の依存関係も解決済み
        /// </summary>
        private string[] _firstTerminals;
        /// <summary>
        /// この要素の直後に来得る終端記号群．この要素が末尾にある場合の依存関係も解決済み
        /// </summary>
        private string[] _followingTerminals;
        /// <summary>
        /// この要素の中で先頭に来る可能性のある終端記号を求めます．先頭に非終端記号がある場合には後で解決します．
        /// </summary>
        /// <param name="dependedDefinitions">先頭に来る可能性のある非終端記号の定義名の出力先</param>
        /// <returns></returns>
        protected internal abstract IList<string> GetFirstTerminalsOfSelf(out string[] dependedDefinitions);
        /// <summary>
        /// この要素に対応する構文解析結果から結果要素を作成するメソッドへの入力パラメータの型を返します．
        /// </summary>
        /// <param name="generator"></param>
        /// <returns></returns>
        public abstract ParameterSignature GetReturnParameterSignature(ScriptParserGenerator generator);
        /// <summary>
        /// この要素に対応する構文解析メソッドの部分コードを記述します
        /// </summary>
        /// <param name="writer">出力先</param>
        /// <param name="generator"></param>
        /// <returns></returns>
        public abstract string WriteParseCode(TextWriter writer, ScriptParserGenerator generator);
        /// <summary>
        /// この要素の中で先頭にくる可能性のある終端記号．先頭に非終端記号がある場合の依存関係は未解決
        /// </summary>
        List<string> _followingTerminalsOfSelf = new List<string>();
        /// <summary>
        /// この要素が定義の末尾にくる可能性があるかどうか
        /// </summary>
        bool CanBeTailOfDefinition;
        /// <summary>
        /// 要素をスキャンして各要素の後続し得る終端記号を設定します．
        /// </summary>
        /// <param name="root">この要素を含む定義</param>
        /// <param name="followings">この要素に後続する可能性のある終端記号</param>
        protected abstract void SetupFollowingTerminalsOfSelf(ScriptParserGenerator generator, string[] followings);
        internal void SetupFollowingTerminals(ScriptParserGenerator generator, string[] followings) {
            _followingTerminalsOfSelf.AddRange(followings.Where(f => f != null));
            this.CanBeTailOfDefinition = followings.Any(f => f == null);
            this.SetupFollowingTerminalsOfSelf(generator, followings);
        }
        /// <summary>
        /// この要素に後続する可能性のある終端記号群を返します
        /// </summary>
        /// <param name="generator"></param>
        /// <returns></returns>
        public IList<string> GetFollowingTerminals(ScriptParserGenerator generator) {
            generator.InitializeFollowingTerminals();
            if(_followingTerminals != null)
                return new ReadOnlyCollection<string>(_followingTerminals);
            List<string> ret = new List<string>(_followingTerminalsOfSelf);
            // 循環対策に一時的入れていく
            _followingTerminals = ret.ToArray();
            // 末尾にある要素の場合はこの要素を含む定義の後続を追加
            if(this.CanBeTailOfDefinition) {
                ret.AddRange(this.RootDefinition.GetFollowingTerminals(generator));
            }
            _followingTerminals = ret.Distinct().ToArray();
            return new ReadOnlyCollection<string>(_followingTerminals);
        }
        /// <summary>
        /// この要素の先頭にくる可能性のある終端記号群を返します．
        /// </summary>
        /// <param name="generator"></param>
        /// <returns></returns>
        public IList<string> GetFirstTerminals(ScriptParserGenerator generator) {
            if(_firstTerminals != null) {
                return new ReadOnlyCollection<string>(_firstTerminals);
            }
            HashSet<string> ret = new HashSet<string>();
            HashSet<string> dependencies = new HashSet<string>();
            Queue<string> dependencyCandidates = new Queue<string>();
            string[] subDeps;
            foreach(string first in this.GetFirstTerminalsOfSelf(out subDeps)) {
                ret.Add(first);
            }
            foreach(string dep in subDeps) {
                dependencyCandidates.Enqueue(dep);
            }
            while(dependencyCandidates.Count > 0) {
                string dep = dependencyCandidates.Dequeue();
                if(dependencies.Contains(dep))
                    continue;
                dependencies.Add(dep);
                DefinitionElement depDef = generator.LookUpDefinition(dep);
                IList<string> firsts = depDef.GetFirstTerminalsOfSelf(out subDeps);
                foreach(string subDep in subDeps) {
                    dependencyCandidates.Enqueue(subDep);
                }
                foreach(string first in firsts) {
                    ret.Add(first);
                }
            }
            if(ret.Contains(null)) {
                ret.RemoveWhere(r => r == null);
                ret.Add(null);
            }
            _firstTerminals = ret.ToArray();
            return new ReadOnlyCollection<string>(_firstTerminals);
        }
    }

    class DefinitionContentComparer : IEqualityComparer<DefinitionContent> {
        #region IEqualityComparer<DefinitionContent> メンバ

        public bool Equals(DefinitionContent x, DefinitionContent y) {
            GroupElement x1 = x as GroupElement;
            if(x1 != null) {
                return this.Equals(x1.InnerExpression, y);
            }
            GroupElement y1 = y as GroupElement;
            if(y1 != null) {
                return this.Equals(x, y1.InnerExpression);
            }
            ExpressionsElement x2 = x as ExpressionsElement;
            if(x2 != null) {
                return this.Equals(x2.Selection, y);
            }
            ExpressionsElement y2 = y as ExpressionsElement;
            if(y2 != null) {
                return this.Equals(x, y2.Selection);
            }
            SelectionElement x3 = x as SelectionElement;
            SelectionElement y3 = y as SelectionElement;
            if(x3 != null && y3 != null) {
                // ∀x0(∈ x) x0 ∈ y かつ ∀y0(∈ y) y0 ∈ x ならば x = y 
                return x3.Candidates.All(x0 => y3.Candidates.Any(y0 => this.Equals(x0, y0))) && y3.Candidates.All(y0 => x3.Candidates.Any(x0 => this.Equals(x0, y0)));
            }
            ElementsElement x4 = x as ElementsElement;
            ElementsElement y4 = y as ElementsElement;
            if(x4 != null && y4 != null) {
                if(x4.Elements.Count != y4.Elements.Count)
                    return false;
                return CollectionEx.Zip(x4.Elements, y4.Elements, (x0, y0) => this.Equals(x0, y0)).All(eq => eq);
            }
            RepeatElement x5 = x as RepeatElement;
            RepeatElement y5 = y as RepeatElement;
            if(x5 != null && y5 != null) {
                return this.Equals(x5.InnerExpression, y5.InnerExpression);
            }
            OptionElement x6 = x as OptionElement;
            OptionElement y6 = y as OptionElement;
            if(x6 != null && y6 != null) {
                return this.Equals(x6.InnerExpression, y6.InnerExpression);
            }
            LiteralElement x7 = x as LiteralElement;
            LiteralElement y7 = y as LiteralElement;
            if(x7 != null && y7 != null) {
                return x7.Literal.Type == y7.Literal.Type && x7.InnerWord == y7.InnerWord;
            }
            if(x == null && y == null)
                return true;
            if(x == null || y == null)
                return false;
            return false;
        }

        public int GetHashCode(DefinitionContent obj) {
            SelectionElement s = obj as SelectionElement;
            if(s != null) {
                // ハッシュ値計算できないので適当
                return 15773;
            }
            ExpressionsElement ex = obj as ExpressionsElement;
            if(ex != null) {
                return this.GetHashCode(ex.Selection);
            }
            ElementsElement es = obj as ElementsElement;
            if(es != null) {
                return es.Elements.Sum(e => this.GetHashCode(e));
            }
            RepeatElement r = obj as RepeatElement;
            if(r != null) {
                return 1744579 + this.GetHashCode(r.InnerExpression);
            }
            OptionElement o = obj as OptionElement;
            if(o != null) {
                return 3234551 + this.GetHashCode(o.InnerExpression);
            }
            GroupElement g = obj as GroupElement;
            if(g != null) {
                return this.GetHashCode(g.InnerExpression);
            }
            LiteralElement l = obj as LiteralElement;
            if(l != null) {
                return l.InnerWord.GetHashCode() + l.Literal.Type.GetHashCode();
            }
            Debug.Assert(false);
            return 7;
        }

        #endregion
    }

    class SelectionSubCandidate {
        ElementsElement _originalElements;
        string[] _temporaryVariables;
        List<ElementElement> _restElements = new List<ElementElement>();

        public DefinitionElement RootElement { get { return _originalElements.RootDefinition; } }
        public ElementsElement OriginalElements { get { return _originalElements; } }
        public string[] TemporaryVariables { get { return _temporaryVariables; } }
        public List<ElementElement> RestElements { get { return _restElements; } }
        public bool IsEmpty { get { return _restElements.Count == 0; } }
        public ElementElement Head { get { return _restElements.First(); } }

        public SelectionSubCandidate(ElementsElement elements) {
            _originalElements = elements;
            _temporaryVariables = new string[0];
            _restElements = elements.Elements.ToList();
        }
        private SelectionSubCandidate(ElementsElement original, IEnumerable<string> temporaryVariables, IEnumerable<ElementElement> restElements) {
            _originalElements = original;
            _temporaryVariables = temporaryVariables.ToArray();
            _restElements = new List<ElementElement>(restElements);
        }
        public SelectionSubCandidate NextElement(string temporaryVariableName) {
            return new SelectionSubCandidate(_originalElements, _temporaryVariables.Union(new[] { temporaryVariableName }), _restElements.Skip(1));
        }
        public IList<string> GetFirstTerminals(ScriptParserGenerator generator) {
            List<string> ret = new List<string>();
            foreach(ElementElement elem in _restElements) {
                IList<string> res = elem.GetFirstTerminals(generator);
                ret.AddRange(res.Where(r => r != null));
                if(!res.Contains(null)) {
                    return ret;
                }
            }
            //ret.Add(null);
            return ret;
        }
        public static void writeParseCodeAux(TextWriter writer, ScriptParserGenerator generator, IList<SelectionSubCandidate> subCandidates, Action<SelectionSubCandidate, TextWriter, ScriptParserGenerator> returnCodeGenerate) {
            Debug.Assert(subCandidates.Count >= 1);
            DefinitionElement rootDefinition = subCandidates.First().RootElement;

            // 先頭のDefinitionContentが同じである選択候補の集合
            Dictionary<DefinitionContent, List<SelectionSubCandidate>> sameHeadCandidates = new Dictionary<DefinitionContent, List<SelectionSubCandidate>>(new DefinitionContentComparer());
            // 優先順位を保つために順番を覚えておく
            List<DefinitionContent> headOrder = new List<DefinitionContent>();
            // 後続の要素がない選択候補の集合
            List<SelectionSubCandidate> noElementCandidates = new List<SelectionSubCandidate>();
            // 選択候補を先頭の要素ごとに振り分け
            foreach(SelectionSubCandidate sub in subCandidates) {
                List<SelectionSubCandidate> list;
                if(sub.IsEmpty) {
                    noElementCandidates.Add(sub);
                } else {
                    if(!sameHeadCandidates.TryGetValue(sub.Head, out list)) {
                        sameHeadCandidates[sub.Head] = list = new List<SelectionSubCandidate>();
                        headOrder.Add(sub.Head);
                    }
                    list.Add(sub);
                }
            }
            Dictionary<string, string> usedFirsts = new Dictionary<string, string>();
            Dictionary<DefinitionContent, string[]> firstTerminals = new Dictionary<DefinitionContent, string[]>();
            foreach(DefinitionContent head in headOrder) {
                // 先頭が同じ候補の集合
                List<SelectionSubCandidate> list;
                bool getListFromHeadSucceeded = sameHeadCandidates.TryGetValue(head, out list);
                Debug.Assert(getListFromHeadSucceeded);
                // 現在の先頭要素の中での頭にくる可能性のある終端記号
                HashSet<string> firstsAtThisHead = new HashSet<string>();
                foreach(SelectionSubCandidate sub in list) {
                    IList<string> firsts = sub.GetFirstTerminals(generator);
                    foreach(string first in firsts) {
                        // もう追加した場合はスルー
                        if(firstsAtThisHead.Contains(first))
                            continue;
                        firstsAtThisHead.Add(first);
                        // 他の先頭のときに使っている終端記号が頭にくる可能性がある場合は警告
                        string usingPoint;
                        if(usedFirsts.TryGetValue(first, out usingPoint)) {
                            string context = string.Format("'{0}' の定義", rootDefinition.DefinitionName);
                            string message = string.Format("<{1}> 内の <{0}> は <{2}> によって隠されます", first, sub.OriginalElements.ToString(), usingPoint);
                            generator.Warn(context, message);
                        } else {
                            usedFirsts[first] = sub.OriginalElements.ToString();
                        }
                    }
                }
                firstTerminals[head] = firstsAtThisHead.ToArray();
            }
            bool isSingleCandidate = sameHeadCandidates.Count == 1 && noElementCandidates.Count == 0;
            if(isSingleCandidate) {
                List<SelectionSubCandidate> list = sameHeadCandidates.Values.First();
                string[] firstsAtThisHead = firstTerminals.Values.First();
                DefinitionContent head = list.First().Head;
                string headVar = head.WriteParseCode(writer, generator);
                List<SelectionSubCandidate> next = new List<SelectionSubCandidate>();
                foreach(SelectionSubCandidate sub in list) {
                    next.Add(sub.NextElement(headVar));
                }
                writeParseCodeAux(writer, generator, next, returnCodeGenerate);
                return;
            }
            bool needsBlockAtEmpty = headOrder.Count >= 1;
            bool usePeekOrThrow = noElementCandidates.Count == 0;
            // 後続の要素があるものは再帰的に処理
            if(headOrder.Count >= 1) {
                string peek;
                if(usePeekOrThrow) {
                    IList<string> validTerminals = firstTerminals.Values.SelectMany(terminals => terminals).Distinct().ToList();
                    writer.WriteLine(generator.GetCodeOfPeekOrThrow(rootDefinition.DefinitionName, validTerminals, out peek));
                } else {
                    // 後続の要素がない場合もあるのでPeekOrThrowではなくPeek
                    writer.WriteLine(generator.GetCodeOfPeek(out peek));
                }
                // 順番通りに
                foreach(DefinitionContent head in headOrder) {
                    // 先頭が同じ候補の集合
                    List<SelectionSubCandidate> list;
                    bool getListFromHeadSucceeded = sameHeadCandidates.TryGetValue(head, out list);
                    Debug.Assert(getListFromHeadSucceeded);
                    // 現在の先頭要素の中での頭にくる可能性のある終端記号
                    string[] firstsAtThisHead;
                    bool getFirstsFromHeadSucceeded = firstTerminals.TryGetValue(head, out firstsAtThisHead);
                    Debug.Assert(getFirstsFromHeadSucceeded);
                    // 頭の終端記号が対象のであるか
                    if(usePeekOrThrow) {
                        writer.WriteLine(generator.GetCodeOfIfLexisIn(peek, firstsAtThisHead));
                    } else {
                        writer.WriteLine(generator.GetCodeOfIfOptionalLexisIn(peek, firstsAtThisHead));
                    }
                    // 先頭の要素をパース
                    string headVar = head.WriteParseCode(writer, generator);
                    // 後続を再帰的に処理
                    List<SelectionSubCandidate> next = new List<SelectionSubCandidate>();
                    foreach(SelectionSubCandidate sub in list) {
                        next.Add(sub.NextElement(headVar));
                    }
                    writeParseCodeAux(writer, generator, next, returnCodeGenerate);
                    writer.Write(generator.GetCodeOfCloseBlock());
                    writer.Write(generator.GetCodeOfSingleElse());
                }
            }
            // 同じ内容なのが二つあった時は警告を出す
            if(noElementCandidates.Count >= 2) {
                SelectionSubCandidate oneOfCandidate = noElementCandidates.First();
                foreach(SelectionSubCandidate emptyElse in noElementCandidates.Skip(1)) {
                    string context = string.Format("'{0}' の定義", rootDefinition.DefinitionName);
                    string message = string.Format("<{0}> は <{1}> によって隠されます", emptyElse.OriginalElements.ToString(), oneOfCandidate.OriginalElements.ToString());
                    generator.Warn(context, message);
                }
            }
            if(needsBlockAtEmpty) {
                writer.WriteLine(generator.GetCodeOfOpenBlock());
            }
            if(noElementCandidates.Count >= 1) {
                SelectionSubCandidate elements = noElementCandidates.First();
                returnCodeGenerate(elements, writer, generator);
            } else {
                IList<string> availableTerminals = new List<string>(usedFirsts.Keys);
                writer.WriteLine(generator.GetCodeOfThrowError(rootDefinition.DefinitionName, availableTerminals));
            }
            if(needsBlockAtEmpty) {
                writer.WriteLine(generator.GetCodeOfCloseBlock());
            }
        }

    }

}

