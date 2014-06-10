using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
namespace LLParserGenerator {
    /// <summary>
    /// 汎用字句解析クラス
    /// </summary>
    /// <typeparam name="TTermType">それぞれの字句の種類に対応する列挙型</typeparam>
    /// <typeparam name="TOutput">各終端記号に対して出力される型</typeparam>
    public abstract class GenericLexParser<TLexisType, TOutput> {
        /// <summary>
        /// 字句解析のための正規表現/字句の種類のペアによるルール要素
        /// </summary>
        public struct RuleElement {
            /// <summary>
            /// 字句の正規表現．複数行字句の場合は開始行の正規表現
            /// </summary>
            public Regex Expression;
            /// <summary>
            /// 複数行字句の終了行の正規表現．単一行の場合はnull
            /// </summary>
            public Regex MultilineEndExpression;
            /// <summary>
            /// 字句の種類
            /// </summary>
            public TLexisType Type;
            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="regex">正規表現の元となる文字列</param>
            /// <param name="type">字句の種類</param>
            public RuleElement(string regex, TLexisType type)
                : this(new Regex(regex), type) {
            }
            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="regex">正規表現</param>
            /// <param name="type">字句の種類</param>
            public RuleElement(Regex regex, TLexisType type)
                : this(regex, null, type) {
            }
            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="regex">複数行開始の正規表現の元となる文字列</param>
            /// <param name="regex">複数行終了の正規表現の元となる文字列</param>
            /// <param name="type">字句の種類</param>
            public RuleElement(string multilineStartRegex, string multilineEndRegex, TLexisType type)
                : this(new Regex(multilineStartRegex), new Regex(multilineEndRegex), type) {
            }
            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="regex">複数行開始の正規表現</param>
            /// <param name="regex">正規表現</param>
            /// <param name="type">複数行終了の字句の種類</param>
            public RuleElement(Regex multilineStartRegex, Regex multilineEndRegex, TLexisType type) {
                this.Expression = multilineStartRegex;
                this.MultilineEndExpression = multilineEndRegex;
                this.Type = type;
            }
        }
        /// <summary>
        /// ルール要素の集合
        /// </summary>
        public class RuleSet : Collection<RuleElement> {
            /// <summary>
            /// デフォルトコンストラクタ
            /// </summary>
            public RuleSet() : base() { }
            /// <summary>
            /// デフォルトコンストラクタ
            /// </summary>
            /// <param name="list">このコレクションにラップされるリスト</param>
            public RuleSet(IList<RuleElement> list) : base(list) { }
            /// <summary>
            /// 字句解析のルールを追加します
            /// </summary>
            /// <param name="regex">正規表現文字列</param>
            /// <param name="type">字句の種類</param>
            public void AddRule(string regex, TLexisType type) {
                this.Add(new GenericLexParser<TLexisType, TOutput>.RuleElement(regex, type));
            }
            /// <summary>
            /// 字句解析のルールを追加します
            /// </summary>
            /// <param name="regex">正規表現</param>
            /// <param name="type">字句の種類</param>
            public void AddRule(Regex regex, TLexisType type) {
                this.Add(new GenericLexParser<TLexisType, TOutput>.RuleElement(regex, type));
            }
            /// <summary>
            /// 複数行の字句解析のルールを追加しました．
            /// </summary>
            /// <param name="multilineStartRegex">開始行の正規表現文字列</param>
            /// <param name="multilineEndRegex">終了行の正規表現文字列</param>
            /// <param name="type">字句の種類</param>
            public void AddMultilineRule(string multilineStartRegex, string multilineEndRegex, TLexisType type) {
                this.Add(new GenericLexParser<TLexisType, TOutput>.RuleElement(multilineStartRegex, multilineEndRegex, type));
            }
            /// <summary>
            /// 複数行の字句解析のルールを追加しました．
            /// </summary>
            /// <param name="multilineStartRegex">開始行の正規表現</param>
            /// <param name="multilineEndRegex">終了行の正規表現</param>
            /// <param name="type">字句の種類</param>
            public void AddMultilineRule(Regex multilineStartRegex, Regex multilineEndRegex, TLexisType type) {
                this.Add(new GenericLexParser<TLexisType, TOutput>.RuleElement(multilineStartRegex, multilineEndRegex, type));
            }
        }
        /// <summary>
        /// キーワードとなる識別子を定める構造体
        /// </summary>
        public struct KeywordElement {
            /// <summary>
            /// キーワードとなる文字列
            /// </summary>
            public string Keyword;
            /// <summary>
            /// キーワードのタイプ
            /// </summary>
            public TLexisType Type;
            /// <summary>
            /// デフォルトコンストラクタ
            /// </summary>
            /// <param name="keyword">キーワードとなる文字列</param>
            /// <param name="type">キーワードのタイプ</param>
            public KeywordElement(string keyword, TLexisType type) {
                this.Keyword = keyword;
                this.Type = type;
            }
        }
        /// <summary>
        /// キーワードのルールの集合
        /// </summary>
        public class KeywordSet : Collection<KeywordElement> {
            /// <summary>
            /// デフォルトコンストラクタ
            /// </summary>
            public KeywordSet() : base() { }
            /// <summary>
            /// デフォルトコンストラクタ
            /// </summary>
            /// <param name="list">コレクションにラップされるリスト</param>
            public KeywordSet(IList<KeywordElement> list) : base(list) { }
            /// <summary>
            /// キーワードを追加します
            /// </summary>
            /// <param name="keyword">キーワード</param>
            /// <param name="type">キーワードの種類</param>
            public void AddKeyword(string keyword, TLexisType type) {
                this.Add(new GenericLexParser<TLexisType, TOutput>.KeywordElement(keyword, type));
            }
            /// <summary>
            /// キーワードの種類を取得または設定します．
            /// </summary>
            /// <param name="keyword">対象となるキーワード</param>
            /// <returns></returns>
            public TLexisType this[string keyword] {
                get { return this.First(p => p.Keyword == keyword).Type; }
                set { this.AddKeyword(keyword, value); }
            }
        }
        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        protected GenericLexParser() { }
        /// <summary>
        /// 字句解析のルールのセットを返します．ルールはセットのうち先に追加されたものから判定されます．
        /// </summary>
        /// <returns>ルールのセット</returns>
        protected abstract RuleSet GetPerseRules();
        /// <summary>
        /// キーワードのセットを返します．
        /// </summary>
        /// <returns>キーワードのセット</returns>
        protected abstract KeywordSet GetKeywords();
        /// <summary>
        /// 字句要素から出力すべきオブジェクトを引数にセットし，trueを返します．要素が無視される場合にはfalseを返します．
        /// </summary>
        /// <param name="termText">字句要素の文字列</param>
        /// <param name="termType">字句要素の種類</param>
        /// <param name="lineStart">字句要素の開始行</param>
        /// <param name="columnStart">字句要素の開始列</param>
        /// <param name="lineEnd">字句要素の終了行</param>
        /// <param name="columnEnd">字句要素の終了列</param>
        /// <param name="output">出力されるオブジェクト</param>
        /// <returns>有効なオブジェクトが返される場合にtrue</returns>
        protected abstract bool GetOutputFromTerminal(string lexis, TLexisType lexicalType, int lineStart, int columnStart, int lineEnd, int columnEnd, out TOutput output);
        /// <summary>
        /// 識別子とキーワードの比較器を返します
        /// </summary>
        /// <returns></returns>
        public abstract IEqualityComparer<string> GetKeywordComparer();
        /// <summary>
        /// 字句解析をして各要素を列挙します
        /// </summary>
        /// <param name="reader">パースされるテキストのリーダ</param>
        /// <returns></returns>
        public IEnumerable<TOutput> ParseText(TextReader reader) {
            //StringBuilder newLineBuf = new StringBuilder();
            //newLineBuf.AppendLine();
            //string newLine = newLineBuf.ToString();

            // 準備
            IList<RuleElement> _lexes = this.GetPerseRules();
            IList<KeywordElement> _keys = this.GetKeywords();
            Dictionary<string, TLexisType> _keywords = new Dictionary<string, TLexisType>(this.GetKeywordComparer());
            foreach(KeywordElement key in _keys) {
                _keywords.Add(key.Keyword, key.Type);
            }
            // 現在行の残り部分
            string lineBuffer = "";
            // 行番号と列番号
            int columnPosition = 1;
            int linePosition = 0;

            while(true) {
                // 今の行を読み終えていたら次の行を読み込む．次の行がなければ終了
                while(lineBuffer == "") {
                    lineBuffer = reader.ReadLine();
                    if(lineBuffer == null)
                        break;
                    linePosition++;
                    columnPosition = 0;
                }
                if(lineBuffer == null)
                    break;
                // 字句要素の中で最初にマッチする物を探す．
                int beginLine = linePosition;
                int beginColumn = columnPosition;
                Match match = null;
                RuleElement rule = default(RuleElement);
                foreach(var elem in _lexes) {
                    Match m = elem.Expression.Match(lineBuffer);
                    if(m.Success && m.Index == 0) {
                        match = m;
                        rule = elem;
                        break;
                    }
                }
                if(match == null) {
                    // パターンなし
                    throw new ParseException("Invalid charactor", lineBuffer, linePosition, columnPosition);
                }
                string term = match.Groups[0].Value;
                if(rule.MultilineEndExpression != null) {
                    // 複数行ルールは残りを探す
                    StringBuilder termBuffer = new StringBuilder(lineBuffer);
                    termBuffer.AppendLine();
                    while(true) {
                        lineBuffer = reader.ReadLine();
                        if(lineBuffer == null) {
                            throw new ParseException(string.Format("End-of-Stream while parsing {0}", rule.Type), lineBuffer, beginLine, beginColumn);
                        }
                        linePosition++;
                        Match matchEnd = rule.MultilineEndExpression.Match(lineBuffer);
                        if(matchEnd.Success) {
                            string termEnd = matchEnd.Groups[0].Value;
                            termBuffer.Append(termEnd);
                            columnPosition = termEnd.Length;
                            lineBuffer = lineBuffer.Substring(termEnd.Length);
                            break;
                        }
                        termBuffer.AppendLine(lineBuffer);
                    }

                    term = termBuffer.ToString();
                } else {
                    // 次の位置へ
                    columnPosition += term.Length;
                    lineBuffer = lineBuffer.Substring(term.Length);
                }
                // キーワードかどうかをチェック
                TLexisType resultType = rule.Type;
                TLexisType keywordType;
                if(_keywords.TryGetValue(term, out keywordType)) {
                    resultType = keywordType;
                }
                // 出力オブジェクトに変換
                TOutput output;
                if(this.GetOutputFromTerminal(term, resultType, beginLine, beginColumn, linePosition, columnPosition, out output)) {
                    yield return output;
                }
            }
        }
    }
}
