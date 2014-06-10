using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace LLParserGenerator {
    enum GeneratorLexType {
        Comment,
        Nonterminal,
        Equal,
        Semicolon,
        Comma,
        VSlash,
        OpenBrace,
        CloseBrace,
        OpenBracket,
        CloseBracket,
        OpenPar,
        ClosePar,
        Terminal,
        Ignore,
        InvalidCharactors,
    }
    class GeneratorLexer : GenericLexParser<GeneratorLexType, GeneratorLexElement> {
        bool _outputWhiteSpaceLexis;
        public GeneratorLexer(bool outputWhiteSpaceLexis) {
            _outputWhiteSpaceLexis = outputWhiteSpaceLexis;
        }
        protected override GenericLexParser<GeneratorLexType, GeneratorLexElement>.RuleSet GetPerseRules() {
            GenericLexParser<GeneratorLexType, GeneratorLexElement>.RuleSet ret = new GenericLexParser<GeneratorLexType, GeneratorLexElement>.RuleSet();
            ret.AddRule(@"^\(\*([^*]*\*)+?\)", GeneratorLexType.Comment);
            ret.AddMultilineRule(@"^\(\*", @"^([^*]*\*)+?\)", GeneratorLexType.Comment);
            ret.AddRule(@"^'[^']+'", GeneratorLexType.Terminal);
            ret.AddRule("^\"[^\"]+\"", GeneratorLexType.Terminal);
            ret.AddRule(@"^;", GeneratorLexType.Semicolon);
            ret.AddRule(@"^,", GeneratorLexType.Comma);
            ret.AddRule(@"^=", GeneratorLexType.Equal);
            ret.AddRule(@"^\(", GeneratorLexType.OpenPar);
            ret.AddRule(@"^\)", GeneratorLexType.ClosePar);
            ret.AddRule(@"^\{", GeneratorLexType.OpenBrace);
            ret.AddRule(@"^\}", GeneratorLexType.CloseBrace);
            ret.AddRule(@"^\[", GeneratorLexType.OpenBracket);
            ret.AddRule(@"^\]", GeneratorLexType.CloseBracket);
            ret.AddRule(@"^\|", GeneratorLexType.VSlash);
            ret.AddRule(@"[a-zA-Z_][0-9a-zA-Z_]*", GeneratorLexType.Nonterminal);
            ret.AddRule(@"^\s+", GeneratorLexType.Ignore);
            ret.AddRule(@"^.+", GeneratorLexType.InvalidCharactors);
            return ret;
        }

        protected override GenericLexParser<GeneratorLexType, GeneratorLexElement>.KeywordSet GetKeywords() {
            return new GenericLexParser<GeneratorLexType, GeneratorLexElement>.KeywordSet();
        }

        protected override bool GetOutputFromTerminal(string termText, GeneratorLexType termType, int lineStart, int columnStart, int lineEnd, int columnEnd, out GeneratorLexElement output) {
            output = new GeneratorLexElement(termText, termType, lineStart, columnStart);
            if(termType == GeneratorLexType.Ignore && !_outputWhiteSpaceLexis)
                return false;
            return true;
        }

        public override IEqualityComparer<string> GetKeywordComparer() {
            return StringComparer.CurrentCulture;
        }
    }
    struct GeneratorLexElement : ILexicalElement<GeneratorLexType> {
        public GeneratorLexElement(string text, GeneratorLexType type, int line, int column) {
            _text = text;
            _type = type;
            _line = line;
            _column = column;
        }
        string _text;
        GeneratorLexType _type;
        int _line;
        int _column;


        public string Text { get { return _text; } }

        #region ILexicalElement<GeneratorLexType> メンバ

        public int Line {
            get { return _line; }
        }

        public int Column {
            get { return _column; }
        }

        public GeneratorLexType Type {
            get { return _type; }
        }

        #endregion
        public override string ToString() {
            return string.Format("'{0}' as {1} at ({2}, {3})", _text, _type, _line, _column);
        }
    }
}
