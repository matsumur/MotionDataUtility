using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MotionDataHandler.DataIO {
    using Misc;
    /// <summary>
    /// PhaseSpaceのある時点でのあるマーカーの座標
    /// </summary>
    public struct PhaseSpaceMarker {
        public int Condition;
        public float X, Y, Z;
        public PhaseSpaceMarker(int condition, float x, float y, float z) {
            Condition = condition;
            X = x;
            Y = y;
            Z = z;
        }
        public void WriteTo(TextWriter writer) {
            writer.Write("{0},{1},{2},{3}", Condition, X.ToString("R"), Y.ToString("R"), Z.ToString("R"));
        }
    }
    /// <summary>
    /// PhaseSpaceのある時点でのマーカー群の座標
    /// </summary>
    public struct PhaseSpaceFrame {
        public decimal Time;
        public PhaseSpaceMarker[] Markers;
        public void ReadFrom(TextReader reader) {
            string line = reader.ReadLine();
            if(line == null)
                throw new InvalidDataException("reader read empty line");
            string[] values = CharacterSeparatedValues.FromString(line, ',');
            if(values.Length < 1)
                throw new InvalidDataException("reader read insufficient data line");
            Time = decimal.Parse(values[0]);
            Markers = new PhaseSpaceMarker[(values.Length - 1) / 4];
            for(int i = 0; i < Markers.Length; i++) {
                int condition;
                float x, y, z;
                if(int.TryParse(values[i * 4 + 1], out condition)
                && float.TryParse(values[i * 4 + 2], out x)
                && float.TryParse(values[i * 4 + 3], out y)
                && float.TryParse(values[i * 4 + 4], out z)) {
                    Markers[i] = new PhaseSpaceMarker(condition, x, y, z);
                } else {
                    throw new InvalidDataException("invalid marker data");
                }
            }
        }
        public void WriteTo(TextWriter writer) {
            writer.Write((Math.Floor(Time * 1000000M) / 1000000M).ToString("F6"));
            foreach(var marker in Markers) {
                writer.Write(",");
                marker.WriteTo(writer);
            }
            writer.WriteLine();
        }
    }
    /// <summary>
    /// PhaseSpaceのデータを読み込むためのクラス
    /// </summary>
    public class PhaseSpaceDataReader : StreamReader {
        int _frames;
        /// <summary>
        /// 読み込んだフレーム数を取得します。
        /// </summary>
        public int Frames { get { return _frames; } }
        private void init() {
            _frames = 0;
        }
        public PhaseSpaceDataReader(string path) : base(path) { init(); }
        public PhaseSpaceDataReader(Stream stream) : base(stream) { init(); }

        public PhaseSpaceFrame ReadFrame() {
            PhaseSpaceFrame ret = new PhaseSpaceFrame();
            ret.ReadFrom(this);
            _frames++;
            return ret;
        }
    }
    /// <summary>
    /// PhaseSpaceのデータを書き込むためのクラス
    /// </summary>
    public class PhaseSpaceDataWriter : StreamWriter {
        int _frames;
        /// <summary>
        /// 書き込んだフレーム数を取得します。
        /// </summary>
        public int Frames { get { return _frames; } }

        protected void init() {
            _frames = 0;
        }
        public PhaseSpaceDataWriter(string path) : base(path) { init(); }
        public PhaseSpaceDataWriter(Stream stream) : base(stream) { init(); }

        public void WriteFrame(PhaseSpaceFrame frame) {
            frame.WriteTo(this);
            _frames++;
        }
    }
}

