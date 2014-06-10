using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MotionDataHandler.Script.Parse {
    public class LexParser : GenericLexParser<LexType, LexicalElement> {
        protected override GenericLexParser<LexType, LexicalElement>.RuleSet GetPerseRules() {
            GenericLexParser<LexType, LexicalElement>.RuleSet ret = new GenericLexParser<LexType, LexicalElement>.RuleSet();
            // 字句解析の設定．上から順にテストされる
            ret.AddRule(@"^[0-9]+(\.[0-9]+)?", LexType.Number);
            ret.AddRule("^\"([^\\\\]|\\\\.)*?\"", LexType.String);
            ret.AddMultilineRule("^\"", "^([^\\\\]|\\\\.)*?\"", LexType.String);
            ret.AddRule(@"^/\*([^*]*\*)+?/", LexType.Ignore);
            ret.AddMultilineRule(@"^/\*", @"^([^*]*\*)+?/", LexType.Ignore);
            ret.AddRule("^//.*", LexType.Ignore);
            ret.AddRule(@"^\(", LexType.OpenPar);
            ret.AddRule(@"^\)", LexType.ClosePar);
            ret.AddRule(@"^\{", LexType.OpenBraces);
            ret.AddRule(@"^\}", LexType.CloseBraces);
            ret.AddRule(@"^\[", LexType.OpenBracket);
            ret.AddRule(@"^\]", LexType.CloseBracket);
            ret.AddRule(@"^<=", LexType.Le);
            ret.AddRule(@"^<", LexType.Lt);
            ret.AddRule(@"^>=", LexType.Ge);
            ret.AddRule(@"^>", LexType.Gt);
            ret.AddRule(@"^!=", LexType.Ne);
            ret.AddRule(@"^==", LexType.Eq);
            ret.AddRule(@"^!", LexType.Not);
            ret.AddRule(@"^&&", LexType.And);
            ret.AddRule(@"^\|\|", LexType.Or);
            ret.AddRule(@"^\+=", LexType.PlusEqual);
            ret.AddRule(@"^-=", LexType.MinusEqual);
            ret.AddRule(@"^\*=", LexType.CrossEqual);
            ret.AddRule(@"^/=", LexType.SlashEqual);
            ret.AddRule(@"^%=", LexType.PercentEqual);
            ret.AddRule(@"^=", LexType.Equal);
            ret.AddRule(@"^\+\+", LexType.PlusPlus);
            ret.AddRule(@"^--", LexType.MinusMinus);
            ret.AddRule(@"^\+", LexType.Plus);
            ret.AddRule(@"^,", LexType.Comma);
            ret.AddRule(@"^-", LexType.Minus);
            ret.AddRule(@"^\*", LexType.Cross);
            ret.AddRule(@"^/", LexType.Slash);
            ret.AddRule(@"^%", LexType.Percent);
            ret.AddRule(@"^\?", LexType.Question);
            ret.AddRule(@"^:", LexType.Colon);
            ret.AddRule(@"^;", LexType.Semicolon);
            ret.AddRule(@"^\.", LexType.Dot);
            const string marks = @"""#$%&'()=^~\\|@`\[\{;+:*\]\},<.>/? \s　";
            ret.AddRule("^[^-0-9" + marks + "][^-" + marks + "]*", LexType.Identifier);
            ret.AddRule(@"^[\s　]+", LexType.Ignore);
            return ret;
        }

        protected override GenericLexParser<LexType, LexicalElement>.KeywordSet GetKeywords() {
            GenericLexParser<LexType, LexicalElement>.KeywordSet ret = new GenericLexParser<LexType, LexicalElement>.KeywordSet();
            // キーワードの登録
            ret.AddKeyword("params", LexType.Params);
            ret.AddKeyword("readonly", LexType.Readonly);
            ret.AddKeyword("var", LexType.Var);
            ret.AddKeyword("true", LexType.True);
            ret.AddKeyword("false", LexType.False);
            ret.AddKeyword("null", LexType.Null);
            ret.AddKeyword("if", LexType.If);
            ret.AddKeyword("else", LexType.Else);
            ret.AddKeyword("while", LexType.While);
            ret.AddKeyword("for", LexType.For);
            ret.AddKeyword("foreach", LexType.Foreach);
            ret.AddKeyword("in", LexType.In);
            ret.AddKeyword("do", LexType.Do);
            ret.AddKeyword("break", LexType.Break);
            ret.AddKeyword("continue", LexType.Continue);
            ret.AddKeyword("return", LexType.Return);
            ret.AddKeyword("func", LexType.Func);
            return ret;
        }


        public override IEqualityComparer<string> GetKeywordComparer() {
            return ScriptConsole.StringComparer;
        }

        protected override bool GetOutputFromTerminal(string termText, LexType termType, int lineStart, int columnStart, int lineEnd, int columnEnd, out LexicalElement output) {
            if(termType == LexType.Ignore) {
                output = default(LexicalElement);
                return false;
            }
            output = new LexicalElement(termText, termType, lineStart, columnStart, lineEnd, columnEnd);
            return true;
        }
    }
}
