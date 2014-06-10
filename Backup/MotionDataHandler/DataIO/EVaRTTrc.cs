using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MotionDataHandler.DataIO {
    /// <summary>
    /// EVaRT(Cortex) .trcファイルヘッダ情報
    /// </summary>
    public struct TrcHeader {
        public int PathFileType;
        public string AxisOrder;
        public string FilePath;
        public string DataRate;
        public string CameraRate;
        public int NumFrames;
        public int NumMarkers;
        public string Units;
        public string OrigDataRate;
        public int OrigDataStartFrame;
        public int OrigNumFrames;
        public string[] Markers;

        /// <summary>
        /// TextReaderからEVaRT trcファイルのヘッダを読み込みます。
        /// </summary>
        /// <param title="reader">読み込むTextReader</param>
        /// <exception cref="InvalidDataException"></exception>
        public void ReadFrom(TextReader reader) {
            // エラー時の読み込み個所を表示するためのもの
            string positionText = "";
            try {
                string[] lines;
                positionText = "Read first line";
                lines = reader.ReadLine().Split('\t');
                positionText = "Parse PathFileType";
                PathFileType = Convert.ToInt32(lines[1]);
                positionText = "Parse AxisOrder";
                AxisOrder = lines[2];
                positionText = "Parse FilePath";
                FilePath = lines[3];
                positionText = "Read second line";
                lines = reader.ReadLine().Split('\t');
                positionText = "Read third line";
                lines = reader.ReadLine().Split('\t');
                positionText = "Parse DataRate";
                DataRate = lines[0];
                positionText = "Parse CameraRate";
                CameraRate = lines[1];
                positionText = "Parse NumFrames";
                NumFrames = Convert.ToInt32(lines[2]);
                positionText = "Parse NumMarkers";
                NumMarkers = Convert.ToInt32(lines[3]);
                positionText = "Parse Units";
                Units = lines[4];
                positionText = "Parse OrigDataRate";
                OrigDataRate = lines[5];
                positionText = "Parse OrigDataStartFrame";
                OrigDataStartFrame = Convert.ToInt32(lines[6]);
                positionText = "Parse OrigNumFrames";
                OrigNumFrames = Convert.ToInt32(lines[7]);
                positionText = "Read fourth line";
                lines = reader.ReadLine().Split('\t');
                positionText = "Parse Markers";
                List<string> tmpMarkers = new List<string>();
                for(int i = 2; i < lines.Length; i += 3) {
                    if(lines[i] != "") // 空のマーカー名は無視するように。
                        tmpMarkers.Add(lines[i]);
                }
                Markers = tmpMarkers.ToArray();
                positionText = "Read fifth line";
                lines = reader.ReadLine().Split('\t');
                positionText = "Read empty line";
                reader.ReadLine(); // 空行を読み込む
            } catch(Exception e) {
                throw new InvalidDataException(positionText, e);
            }
        }
        /// <summary>
        /// StreamからEVaRT trcファイルのヘッダを読み込みます。
        /// </summary>
        /// <param title="stream">読み込むStream</param>
        /// <exception cref="InvalidDataException"></exception>
        public void ReadFrom(Stream stream) {
            using(StreamReader reader = new StreamReader(stream)) {
                ReadFrom(reader);
            }
        }
        /// <summary>
        /// ファイル名指定でEVaRT trcファイルのヘッダを読み込みます。
        /// </summary>
        /// <param title="path">読み込むファイル名</param>
        /// <exception cref="InvalidDataException"></exception>
        public void ReadFrom(string path) {
            using(StreamReader reader = new StreamReader(path)) {
                ReadFrom(reader);
            }
        }


        /// <summary>
        /// TextReaderからEVaRT trcファイルのヘッダを読み込みます。
        /// </summary>
        /// <param title="reader">読み込むTextReader</param>
        /// <exception cref="InvalidDataException"></exception>
        public TrcHeader(TextReader reader)
            : this() {
            ReadFrom(reader);
        }
        /// <summary>
        /// StreamからEVaRT trcファイルのヘッダを読み込みます。
        /// </summary>
        /// <param title="stream">読み込むStream</param>
        /// <exception cref="InvalidDataException"></exception>
        public TrcHeader(Stream stream)
            : this() {
            ReadFrom(stream);
        }
        /// <summary>
        /// ファイル名指定でEVaRT trcファイルのヘッダを読み込みます。
        /// </summary>
        /// <param title="path">読み込むファイル名</param>
        /// <exception cref="InvalidDataException"></exception>
        public TrcHeader(string path)
            : this() {
            ReadFrom(path);
        }

        /// <summary>
        /// Tracked ASCIIのヘッダを出力します。
        /// </summary>
        /// <param title="writer">出力するTextWriter</param>
        public void WriteTo(TextWriter writer) {
            NumMarkers = Markers.Length;
            writer.WriteLine("PathFileType\t{0}\t{1}\t{2}", PathFileType.ToString(), AxisOrder, FilePath);
            writer.WriteLine("DataRate\tCameraRate\tNumFrames\tNumMarkers\tUnits\tOrigDataRate\tOrigDataStartFrame\tOrigNumFrames");
            writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", DataRate.ToString(), CameraRate.ToString(), NumFrames.ToString(), NumMarkers.ToString(), Units, OrigDataRate.ToString(), OrigDataStartFrame.ToString(), OrigNumFrames.ToString());

            writer.Write("Frame#\tTime\t");
            foreach(var marker in Markers) {
                writer.Write("{0}\t\t\t", marker);
            }
            writer.WriteLine();
            writer.Write("\t\t");
            for(int i = 0; i < Markers.Length; i++) {
                writer.Write("X{0}\tY{0}\tZ{0}\t", (i + 1).ToString());
            }
            writer.WriteLine();
            writer.WriteLine();
        }

        public TrcHeader JoinWith(TrcHeader another, bool useAnothersSettings) {
            TrcHeader ret;
            if(useAnothersSettings) {
                ret = another;
            } else {
                ret = this;
            }
            List<string> markers = new List<string>();
            markers.AddRange(this.Markers);
            markers.AddRange(another.Markers);
            ret.Markers = markers.ToArray();
            ret.NumMarkers = markers.Count;
            return ret;
        }
    }
    /// <summary>
    /// .trcのある時点でのある点の座標データ
    /// </summary>
    public struct TrcMarker {
        public string X, Y, Z;
        public TrcMarker(string x, string y, string z) {
            X = x;
            Y = y;
            Z = z;
        }
        public void WriteTo(TextWriter writer) {
            writer.Write("{0}\t{1}\t{2}\t", X, Y, Z);
        }
        public override string ToString() {
            return new StringBuilder().AppendFormat("{0}: ({1}, {2}, {3})", this.GetType().ToString(), X, Y, Z).ToString();
        }
    }
    /// <summary>
    /// .trcのある時点での点群の座標データ
    /// </summary>
    public struct TrcFrame {
        public TrcMarker?[] Markers;
        public int Number;
        public decimal Time;
        /// <summary>
        /// 一行分のフレームをかき出します。
        /// </summary>
        /// <param title="writer">出力先のTextWriter</param>
        public void WriteTo(TextWriter writer) {
            writer.Write("{0}\t{1}\t", Number, (Math.Floor(Time * 1000M) / 1000M).ToString("F3"));
            if(this.Markers == null)
                this.Markers = new TrcMarker?[0];
            foreach(var marker in Markers) {
                if(marker.HasValue) {
                    marker.Value.WriteTo(writer);
                } else {
                    writer.Write("\t\t\t");
                }
            }
            writer.WriteLine();
        }
        /// <summary>
        /// TextReaderから一行分のフレームを取得します。
        /// </summary>
        /// <param title="reader">読み込むTextReader</param>
        /// <exception cref="InvalidDataException"></exception>
        public void ReadFrom(TextReader reader) {
            string line = reader.ReadLine();
            if(line == null)
                throw new InvalidDataException("reader read empty line");
            string[] values = line.Split('\t');
            if(values.Length < 2)
                throw new InvalidDataException("reader read insufficient data line");
            Markers = new TrcMarker?[(values.Length - 2) / 3]; // Length - 2: 末尾に\tがあるので2じゃなく3余計だけどintなので気にしない
            try {
                Number = int.Parse(values[0]);
            } catch(Exception ex) {
                throw new InvalidDataException("invalid frame#", ex);
            }
            try {
                Time = decimal.Parse(values[1]);
            } catch(Exception ex) {
                throw new InvalidDataException("invalid Time", ex);
            }
            for(int i = 0; i < Markers.Length; i++) {
                if(i * 3 + 4 < values.Length && values[i * 3 + 2] != "")
                    Markers[i] = new TrcMarker(values[i * 3 + 2], values[i * 3 + 3], values[i * 3 + 4]);
            }
        }

        /// <summary>
        /// 無名マーカーを除去したフレームを返します
        /// </summary>
        /// <param title="header">マーカー名情報を保持するtrc ヘッダ</param>
        /// <returns>新しいフレーム</returns>
        public TrcFrame TrimUnnamed(TrcHeader header) {
            return TrimMarkers(0, header.NumMarkers);
        }
        /// <summary>
        /// 指定範囲のマーカーのみを保持するフレームを返します。
        /// </summary>
        /// <param title="lower">範囲の開始インデックス</param>
        /// <param title="length">範囲の長さ</param>
        /// <returns>新しいフレーム</returns>
        public TrcFrame TrimMarkers(int begin, int length) {
            TrcFrame ret = this;
            ret.Markers = new TrcMarker?[length];
            if(this.Markers == null)
                this.Markers = new TrcMarker?[0];
            for(int i = 0; i < length; i++) {
                if(i + begin >= this.Markers.Length)
                    break;
                if(this.Markers[i + begin].HasValue)
                    ret.Markers[i] = this.Markers[i + begin].Value;
            }
            return ret;
        }

        /// <summary>
        /// ほかのフレームのマーカーをこのフレームの末尾に追加したフレームを返します。
        /// </summary>
        /// <param title="another">ほかのフレーム</param>
        /// <returns>新しいフレーム</returns>
        public TrcFrame JoinWith(TrcFrame another) {
            return JoinWith(another, false);
        }
        /// <summary>
        /// ほかのフレームのマーカーをこのフレームの末尾に追加したフレームを返します。
        /// </summary>
        /// <param title="another">ほかのフレーム</param>
        /// <param title="useAnothersNumberAndTime">ほかのフレームのフレーム番号と時間を新しいフレームに用いるか</param>
        /// <returns>新しいフレーム</returns>
        public TrcFrame JoinWith(TrcFrame another, bool useAnothersNumberAndTime) {
            TrcFrame ret;
            if(useAnothersNumberAndTime) {
                ret = another;
            } else {
                ret = this;
            }
            if(this.Markers == null)
                this.Markers = new TrcMarker?[0];
            if(another.Markers == null)
                another.Markers = new TrcMarker?[0];
            ret.Markers = new TrcMarker?[this.Markers.Length + another.Markers.Length];
            for(int i = 0; i < this.Markers.Length; i++) {
                if(this.Markers[i].HasValue)
                    ret.Markers[i] = this.Markers[i].Value;
            }
            for(int i = 0; i < another.Markers.Length; i++) {
                if(another.Markers[i].HasValue)
                    ret.Markers[i + this.Markers.Length] = another.Markers[i].Value;
            }
            return ret;
        }
        public TrcFrame JoinAll(TrcFrame[] others) {
            TrcFrame ret = this;
            if(this.Markers == null)
                this.Markers = new TrcMarker?[0];
            int length = this.Markers.Length;
            for(int i = 0; i < others.Length; i++) {
                if(others[i].Markers == null)
                    others[i].Markers = new TrcMarker?[0];
            }
            foreach(var frame in others) {
                length += frame.Markers.Length;
            }
            ret.Markers = new TrcMarker?[length];
            for(int i = 0; i < this.Markers.Length; i++) {
                if(this.Markers[i].HasValue)
                    ret.Markers[i] = this.Markers[i].Value;
            }
            int offset = this.Markers.Length;
            foreach(var frame in others) {
                for(int i = 0; i < frame.Markers.Length; i++) {
                    if(frame.Markers[i].HasValue)
                        ret.Markers[i + offset] = frame.Markers[i].Value;
                }
                offset += frame.Markers.Length;
            }
            return ret;
        }
        public static decimal GetTimeFromNumber(int number, TrcHeader header) {
            return GetTimeFromNumber(number, header, 0);
        }
        public static decimal GetTimeFromNumber(int number, TrcHeader header, decimal offset) {
            try {
                decimal ratio = 1 / decimal.Parse(header.DataRate);
                return (((decimal)number - 1) * ratio) + offset;
            } catch(OverflowException) {
                return offset;
            }
        }
    }
    /// <summary>
    /// .trcファイルの読み込みクラス
    /// </summary>
    public class TrcReader : StreamReader {
        TrcHeader _header;
        /// <summary>
        /// 読み込まれたEVaRT trcファイルのヘッダを取得します。
        /// </summary>
        public TrcHeader Header {
            get { return _header; }
        }
        int _frames;
        /// <summary>
        /// 読み込んだフレーム数を取得します。
        /// </summary>
        public int Frames { get { return _frames; } }

        private void init() {
            _header = new TrcHeader(this);
        }

        public TrcReader(string path) : base(path) { init(); }
        public TrcReader(Stream stream) : base(stream) { init(); }
        public TrcReader(string path, Encoding encoding) : base(path, encoding) { init(); }
        public TrcReader(Stream stream, Encoding encoding) : base(stream, encoding) { init(); }

        /// <summary>
        /// 次の1フレームを取得します。
        /// </summary>
        /// <returns>取得されたフレーム</returns>
        /// <exception cref="InvalidDataException"></exception>
        public TrcFrame ReadFrame() {
            TrcFrame ret = new TrcFrame();
            if(this.EndOfStream)
                throw new EndOfStreamException();
            ret.ReadFrom(this);
            _frames++;
            return ret;
        }
    }
    /// <summary>
    /// .trcファイルの書き込みクラス
    /// </summary>
    public class TrcWriter : StreamWriter {
        int _frames;
        /// <summary>
        /// 書き込んだフレーム数を取得します。
        /// </summary>
        public int Frames { get { return _frames; } }

        /// <summary>
        /// フレーム書き込み時の時間オフセットを取得または設定します。
        /// </summary>
        public decimal TimeOffset;

        TrcHeader _header;

        private void init(TrcHeader header) {
            TimeOffset = 0;
            _frames = 0;
            _header = header;
            _header.WriteTo(this);
        }

        public TrcWriter(TrcHeader header, string path) : base(path) { init(header); }
        public TrcWriter(TrcHeader header, Stream stream) : base(stream) { init(header); }
        public TrcWriter(TrcHeader header, Stream stream, Encoding encoding) : base(stream, encoding) { init(header); }

        /// <summary>
        /// ファイルにフレームを書き込みます。フレーム番号と時間は修正されます。
        /// </summary>
        /// <param title="frame">書き込むフレーム</param>
        /// <returns></returns>
        public bool WriteFrame(TrcFrame frame) {
            if(_frames >= _header.NumFrames) {
                return false;
            }
            _frames++;
            frame.Number = _frames;
            frame.Time = TrcFrame.GetTimeFromNumber(frame.Number, _header, TimeOffset);
            frame.WriteTo(this);
            return true;
        }
        /// <summary>
        /// フレーム番号と時間を維持してファイルにフレームを書き込みます。
        /// </summary>
        /// <param title="frame">書き込むフレーム</param>
        /// <returns></returns>
        public bool WriteRawFrame(TrcFrame frame) {
            if(_frames >= _header.NumFrames) {
                return false;
            }
            _frames++;
            frame.WriteTo(this);
            return true;
        }
    }
}

