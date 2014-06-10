// Auto generated with LLParserGenerator.ScriptParserGenerator
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace LLParserGenerator {
    interface ILexicalElementBase {
        string Text { get; }
        int Line { get; }
        int Column { get; }
    }
    interface ILexicalElement<TLexisType> : ILexicalElementBase where TLexisType : struct {
        TLexisType Type { get; }
    }
    struct Optional<T> {
        public readonly bool HasValue;
        public readonly T Value;
        public static readonly Optional<T> Empty = new Optional<T>();
        public Optional(T value) {
            if(value == null) {
                this.Value = default(T);
                this.HasValue = false;
            } else {
                this.Value = value;
                this.HasValue = true;
            }
        }
        public static implicit operator Optional<T>(T value) {
            return new Optional<T>(value);
        }
        public override string ToString() {
            if(!this.HasValue) {
                return "null";
            } else {
                return this.Value.ToString();
            }
        }
    }
    class ParseException : Exception {
        public string ErrorText;
        public int Line, Column;
        public ParseException(string message, string errorText, int line, int column)
            : base(string.Format("{0} (Line {1}, Column {2})", message, line, column)) {
            this.ErrorText = errorText;
            this.Line = line;
            this.Column = column;
        }
        public ParseException(string message, ILexicalElementBase lexAtStart)
            : this(message, lexAtStart.Text, lexAtStart.Line, lexAtStart.Column) {
        }
        public ParseException(string message)
            : this(message, "", -1, 0) {
        }
    }
    class Reader<TLexisElement, TLexisType> : IDisposable
        where TLexisElement : ILexicalElement<TLexisType>
        where TLexisType : struct {
        readonly IEnumerator<TLexisElement> _enumerator;
        readonly List<TLexisElement> _history = new List<TLexisElement>();
        readonly Stack<int> _save = new Stack<int>();
        int _position = 0;
        public void PushSave() {
            _save.Push(_position);
        }
        public void PopSave() {
            _save.Pop();
        }
        public void Restore() {
            _position = _save.Peek();
        }
        public void RestoreAndPopSave() {
            _position = _save.Pop();
        }
        bool _end = false;

        Optional<TLexisElement> _last = Optional<TLexisElement>.Empty;
        Optional<TLexisElement> _prev = Optional<TLexisElement>.Empty;
        public Optional<TLexisElement> Previous { get { return _prev; } }

        public Reader(IEnumerable<TLexisElement> enumeration) {
            _enumerator = enumeration.GetEnumerator();
        }

        /// <summary>
        /// 先頭にある字句を削除せずに読み込んで返します．
        /// </summary>
        /// <returns></returns>
        public Optional<TLexisElement> Peek() {
            if(_position < _history.Count) {
                return _history[_position];
            }
            while(_position >= _history.Count) {
                if(_end)
                    return Optional<TLexisElement>.Empty;
                do {
                    if(!_enumerator.MoveNext()) {
                        _end = true;
                        return Optional<TLexisElement>.Empty;
                    }
                } while(_enumerator.Current == null);
                _history.Add(_enumerator.Current);
            }
            return _history[_position];
        }

        /// <summary>
        /// 次の字句を読み込んで返し，読み取りカーソルを一つ進めます．
        /// </summary>
        /// <returns></returns>
        public Optional<TLexisElement> Read() {
            Optional<TLexisElement> ret = this.Peek();
            if(!_end || _position < _history.Count) {
                _position++;
            }
            return ret;
        }
        public ParseException Error(string context, params TLexisType[] validTypes) {
            string valid = "";
            for(int i = 0; i < validTypes.Length; i++) {
                if(i != 0) {
                    if(i == validTypes.Length - 1) {
                        valid += " or ";
                    } else {
                        valid += ", ";
                    }
                }
                valid += validTypes[i].ToString();
            }
            string aux = "is";
            if(validTypes.Length == 0) {
                aux = "None is";
            } else if(validTypes.Length != 1) {
                aux = "are";
            }

            Optional<TLexisElement> peek = this.Peek();
            if(!peek.HasValue) {
                peek = this.Previous;
            }
            if(peek.HasValue) {
                throw new ParseException(string.Format("{0} {1} expected while reading {2}", valid, aux, context), "", peek.Value.Line, peek.Value.Column);
            } else {
                throw new ParseException("Empty Script", "", 0, 0);
            }
        }

        /// <summary>
        /// 先頭にある字句を削除せずに読み込んで返します．有効な種類でなければParseExceptionをスローします．
        /// </summary>
        /// <returns></returns>
        public TLexisElement PeekOrThrow(string context, params TLexisType[] validTypes) {
            Optional<TLexisElement> ret = this.Peek();
            if(!ret.HasValue) {
                throw Error(context, validTypes);
            }
            if(!validTypes.Contains(ret.Value.Type)) {
                throw Error(context, validTypes);
            }
            return ret.Value;
        }

        /// <summary>
        /// 次の字句を読み込んで返し，読み取りカーソルを一つ進めます．有効な種類でなければParseExceptionをスローします．
        /// </summary>
        /// <param name="errorMessage">読み込み中の構文</param>
        /// <param name="validTypes">有効な字句</param>
        /// <returns></returns>
        public TLexisElement ReadOrThrow(string context, params TLexisType[] validTypes) {
            this.PeekOrThrow(context, validTypes);
            Optional<TLexisElement> ret = this.Read();
            return ret.Value;
        }


        #region IDisposable メンバ

        public void Dispose() {
            _enumerator.Dispose();
        }

        #endregion
    }
    struct FixedList<T1, T2> {
        public T1 Element1;
        public T2 Element2;
        public FixedList(T1 element1, T2 element2) {
            this.Element1 = element1;
            this.Element2 = element2;
        }
        public override string ToString() {
            return string.Format("1: <{0}>, 2: <{1}>", this.Element1, this.Element2);
        }
    }
    struct Selection<T1, T2> {
        public readonly Optional<T1> Element1;
        public readonly Optional<T2> Element2;
        public Selection(T1 element) {
            this.Element1 = element;
            this.Element2 = Optional<T2>.Empty;
        }
        public Selection(T2 element) {
            this.Element1 = Optional<T1>.Empty;
            this.Element2 = element;
        }
        public override string ToString() {
            if(this.Element1.HasValue) {
                return string.Format("1: {0}", this.Element1.Value);
            }
            if(this.Element2.HasValue) {
                return string.Format("2: {0}", this.Element2.Value);
            }
            return "null: null";
        }
    }
}
