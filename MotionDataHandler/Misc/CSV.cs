using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MotionDataHandler.Misc {
    /// <summary>
    /// CSV形式のデータを処理するためのクラス
    /// </summary>
    public static class CharacterSeparatedValues {
        /// <summary>
        /// CSVデータの一行が改行により途中で切れているかを返す。
        /// 改行を含むなら改行文字と次の行をlineに連結させるべきである。
        /// </summary>
        /// <param title="line">csv文字列</param>
        /// <param title="delimiter">区切り文字</param>
        /// <returns>改行により中断されているならば真</returns>
        public static bool IsLineInterruptedByNewLine(string line) {
            return IsLineInterruptedByNewLine(line, ',');
        }
        /// <summary>
        /// CSVデータの一行が改行により途中で切れているかを返す。
        /// 改行を含むなら次の行をlineに連結させるべきである。
        /// </summary>
        /// <param title="line">csv文字列</param>
        /// <param title="delimiter">区切り文字</param>
        /// <returns>改行により中断されているならば真</returns>
        public static bool IsLineInterruptedByNewLine(string line, char delimiter) {
            // とりあえず分割
            string[] lines = line.Split(delimiter);
            for(int index = 0; index < lines.Length; index++) {
                if(lines[index].StartsWith("\"")) { // 二重引用符で始まる
                    int continues; // 結合回数
                    for(continues = 0; index + continues + 1 < lines.Length; continues++) {
                        // 二重引用符で終わるところを探す。
                        // 始まったところが二重引用符一文字だった場合はすぐには抜けない様にする
                        if(lines[index + continues].EndsWith("\"")
                            && (continues != 0 || lines[index].Length != 1))
                            break;
                    }
                    if(index + continues + 1 == lines.Length) {
                        // 最後の断片でもちゃんと二重引用符で終わっていたらfalse
                        if(lines[index + continues].EndsWith("\"")
                            && (continues != 0 || lines[index].Length != 1))
                            return false;
                        // 改行の途中である
                        return true;
                    }
                    index += continues;
                }
            }
            return false;
        }
        /// <summary>
        /// CSVデータの一行を文字列の配列に変換します
        /// </summary>
        /// <param title="line">csv文字列</param>
        /// <returns>csvデータ</returns>
        public static string[] FromString(string line) {
            return FromString(line, ',');
        }

        /// <summary>
        /// 区切り文字を指定してCSVデータの一行を文字列の配列に変換します
        /// </summary>
        /// <param title="line">csv文字列</param>
        /// <param title="delimiter">区切り文字</param>
        /// <returns>csvデータ</returns>
        public static string[] FromString(string line, char delimiter) {
            List<string> ret = new List<string>();
            // とりあえず分割
            string[] lines = line.Split(delimiter);
            for(int index = 0; index < lines.Length; index++) {
                if(lines[index].StartsWith("\"")) { // 二重引用符で始まる
                    int continues; // 結合回数
                    for(continues = 0; index + continues + 1 < lines.Length; continues++) {
                        // 二重引用符で終わるところを探す。
                        // 始まったところが二重引用符一文字だった場合はすぐには抜けない様にする
                        if(lines[index + continues].EndsWith("\"")
                            && (continues != 0 || lines[index].Length != 1))
                            break;
                    }
                    // join $delimiter, @lines[markerIndex..markerIndex+continues];
                    StringBuilder tmp = new StringBuilder();
                    for(int j = 0; j <= continues; j++) {
                        if(j != 0)
                            tmp.Append(delimiter);
                        tmp.Append(lines[index + j]);
                    }
                    // 頭の二重引用符を除く
                    tmp.Remove(0, 1);
                    // 末尾の二重引用符を除く
                    if(lines[index + continues].EndsWith("\"")
                        && (continues != 0 || lines[index].Length != 1)) {
                        tmp.Length--;
                    }
                    // 二つの二重引用符は一つの二重引用符とみなす
                    tmp.Replace("\"\"", "\"");
                    // 出力へ
                    ret.Add(tmp.ToString());
                    // 結合した分を飛ばす。forのindex++は別
                    index += continues;
                } else {
                    // 出力へ
                    ret.Add(lines[index]);
                }
            }
            return ret.ToArray();
        }
        /// <summary>
        /// 文字列の配列からCSV文字列を取得します
        /// </summary>
        /// <param title="values">連結される文字列の配列</param>
        /// <returns>連結された文字列</returns>
        public static string ToString(string[] values) {
            return ToString(values, ',');
        }
        /// <summary>
        /// 連結文字を指定して文字列の配列からCSV文字列を取得します
        /// </summary>
        /// <param title="values">連結される文字列の配列</param>
        /// <param title="delimiter">連結の用いる文字</param>
        /// <returns>連結された文字列</returns>
        public static string ToString(string[] values, char glue) {
            // join $delimiter, @values;
            StringBuilder ret = new StringBuilder();
            for(int i = 0; i < values.Length; i++) {
                if(i != 0)
                    ret.Append(glue);
                if(values[i].Contains(glue.ToString()) || values[i].Contains("\r") || values[i].Contains("\n") || values[i].Contains("\"")) {
                    // 文字列中の二重引用符を二重にする
                    string tmp = values[i].Replace("\"", "\"\"");

                    // 文字列を二重引用符で囲む
                    tmp = new StringBuilder().AppendFormat("\"{0}\"", tmp).ToString();
                    // 出力する
                    ret.Append(tmp);
                } else {
                    // 出力する
                    ret.Append(values[i]);
                }
            }
            return ret.ToString();
        }
    }

    /// <summary>
    /// CSV形式のファイルを読み込むためのクラス
    /// </summary>
    public class CSVReader : StreamReader {
        /// <summary>
        /// カンマで区切られたファイルを読み込みます．
        /// </summary>
        /// <param name="path">ファイル名</param>
        public CSVReader(string path)
            : this(path, ',') {
        }
        /// <summary>
        /// 指定された文字で区切られたファイルを読み込みます
        /// </summary>
        /// <param name="path">ファイル名</param>
        /// <param name="delimiter">区切り文字</param>
        public CSVReader(string path, char delimiter)
            : this(new FileStream(path, FileMode.Open), delimiter) {
        }
        /// <summary>
        /// カンマで区切られたストリームを読み込みます，
        /// </summary>
        /// <param name="stream">入力ストリーム</param>
        public CSVReader(Stream stream)
            : this(stream, ',') {
        }
        /// <summary>
        /// 指定された文字で区切られたファイルを読み込みます．
        /// </summary>
        /// <param name="stream">入力ストリーム</param>
        /// <param name="delimiter">区切り文字</param>
        public CSVReader(Stream stream, char delimiter)
            : base(stream) {
            this.delimiter = delimiter;
        }

        private char delimiter;
        /// <summary>
        /// 区切り文字を取得します．
        /// </summary>
        public char Delimiter { get { return delimiter; } }
        private static string[] newLines = new string[] { "\r\n", "\n\r", "\r", "\n" };

        /// <summary>
        /// 次の行を読み込み，区切り文字で分解して文字列の配列を返します．
        /// </summary>
        /// <returns></returns>
        public string[] ReadValues() {
            if(this.EndOfStream)
                return null;
            string line = this.ReadLine();
            while(CharacterSeparatedValues.IsLineInterruptedByNewLine(line, delimiter)) {
                if(this.EndOfStream)
                    return null;
                line += Environment.NewLine + this.ReadLine();
            }
            return CharacterSeparatedValues.FromString(line, delimiter);
            /*
            List<string> tmp1 = new List<string>();
            int beginOffset = bufferOffset;
            if (bufferOffset > buffer.Length) {
                char[] buf = new char[BufferLength];
                int length = ReadBlock(buf, 0, BufferLength);
                buffer = buffer.Substring(beginOffset) + new string(buf);
            }
             * */
        }
    }
}

