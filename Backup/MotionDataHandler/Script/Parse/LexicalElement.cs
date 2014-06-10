using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MotionDataHandler.Script.Parse {
    /// <summary>
    /// 字句要素のタイプ
    /// </summary>
    public enum LexType {
        /// <summary>
        /// 無視する字句要素．字句解析中にのみ用いられる
        /// </summary>
        Ignore,

        Number,
        String,
        OpenPar,
        ClosePar,
        OpenBraces,
        CloseBraces,
        OpenBracket,
        CloseBracket,
        Params,
        Readonly,
        Var,
        If,
        Else,
        While,
        For,
        Foreach,
        In,
        Do,
        Func,
        Break,
        Continue,
        Return,
        Identifier,
        Dot,
        Semicolon,
        Plus,
        PlusEqual,
        PlusPlus,
        Minus,
        MinusEqual,
        MinusMinus,
        Cross,
        CrossEqual,
        Slash,
        SlashEqual,
        Percent,
        PercentEqual,
        Equal,
        Comma,
        True,
        False,
        Null,
        Question,
        Colon,
        Gt,
        Ge,
        Lt,
        Le,
        Eq,
        Ne,
        Not,
        And,
        Or,
        /// <summary>
        /// 式．実際には出現しない
        /// </summary>
        Expression,
        /// <summary>
        /// 文．実際には出現しない
        /// </summary>
        Statement,
    }

    /// <summary>
    /// 分解された字句要素
    /// </summary>
    public struct LexicalElement : ILexicalElement<LexType> {
        /// <summary>
        /// 字句の内容
        /// </summary>
        public String Text { get; set; }
        /// <summary>
        /// 字句の種類
        /// </summary>
        public LexType Type { get; set; }
        /// <summary>
        /// 字句位置の行番号
        /// </summary>
        public int Line { get; set; }
        /// <summary>
        /// 字句位置の列番号
        /// </summary>
        public int Column { get; set; }

        public int LineEnd;
        public int ColumnEnd;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="text">字句の内容</param>
        /// <param name="type">字句の種類</param>
        /// <param name="lineIndex">字句位置の行番号</param>
        /// <param name="columnIndex">字句位置の列番号</param>
        public LexicalElement(string text, LexType type, int lineIndex, int columnIndex, int lineEndIndex, int columnEndIndex)
            : this() {
            this.Line = lineIndex;
            this.Column = columnIndex;
            this.Type = type;
            this.Text = text;
            this.LineEnd = lineEndIndex;
            this.ColumnEnd = columnEndIndex;
        }

        public static string GetStringFromLexType(LexType lexType) {
            switch(lexType) {
            case LexType.And:
                return "'&&'";
            case LexType.Break:
                return "'break'";
            case LexType.CloseBraces:
                return "'}'";
            case LexType.CloseBracket:
                return "']'";
            case LexType.ClosePar:
                return "')'";
            case LexType.Colon:
                return "':'";
            case LexType.Comma:
                return "','";
            case LexType.Continue:
                return "'continue'";
            case LexType.Cross:
                return "'*'";
            case LexType.CrossEqual:
                return "'*='";
            case LexType.Do:
                return "'do'";
            case LexType.Dot:
                return "'.'";
            case LexType.Else:
                return "'else'";
            case LexType.Eq:
                return "'=='";
            case LexType.Equal:
                return "'='";
            case LexType.False:
                return "'false'";
            case LexType.For:
                return "'for'";
            case LexType.Foreach:
                return "''foreach'";
            case LexType.Func:
                return "'func'";
            case LexType.Ge:
                return "'>='";
            case LexType.Gt:
                return "'>'";
            case LexType.Identifier:
                return "Identifier";
            case LexType.If:
                return "'if'";
            case LexType.In:
                return "'in'";
            case LexType.Le:
                return "'<='";
            case LexType.Lt:
                return "'<'";
            case LexType.Minus:
                return "'-'";
            case LexType.MinusEqual:
                return "'-='";
            case LexType.MinusMinus:
                return "'--'";
            case LexType.Ne:
                return "'!='";
            case LexType.Not:
                return "'!'";
            case LexType.Null:
                return "'null'";
            case LexType.Number:
                return "Number";
            case LexType.OpenBraces:
                return "'{'";
            case LexType.OpenBracket:
                return "'['";
            case LexType.OpenPar:
                return "'('";
            case LexType.Or:
                return "'||'";
            case LexType.Params:
                return "'params'";
            case LexType.Percent:
                return "'%'";
            case LexType.PercentEqual:
                return "'%='";
            case LexType.Plus:
                return "'+'";
            case LexType.PlusEqual:
                return "'+='";
            case LexType.PlusPlus:
                return "'++'";
            case LexType.Question:
                return "'?'";
            case LexType.Readonly:
                return "readonly";
            case LexType.Return:
                return "'return'";
            case LexType.Semicolon:
                return "';'";
            case LexType.Slash:
                return "'/'";
            case LexType.SlashEqual:
                return "'/='";
            case LexType.String:
                return "String";
            case LexType.True:
                return "'true'";
            case LexType.Var:
                return "'var'";
            case LexType.While:
                return "'while'";
            }
            return Enum.GetName(typeof(LexType), lexType);
        }

    }
}
