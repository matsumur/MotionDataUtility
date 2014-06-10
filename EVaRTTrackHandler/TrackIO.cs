using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EVaRTTrackHandler {
  
    class TrackIO : ICloneable {
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
        protected bool loaded;

        public TrackIO() {
            loaded = false;
        }

        protected string _state;

        public virtual string State {
            get {
                return _state;
            }
        }
        /// <summary>
        /// オブジェクトのコピーを作成します。
        /// </summary>
        /// <returns>クローンされたオブジェクト</returns>
        public object Clone() {
            TrackIO ret = new TrackIO();
            ret.PathFileType = this.PathFileType;
            ret.AxisOrder = this.AxisOrder;
            ret.FilePath = this.FilePath;
            ret.DataRate = this.DataRate;
            ret.CameraRate = this.CameraRate;
            ret.NumFrames = this.NumFrames;
            ret.NumMarkers = this.NumMarkers;
            ret.Units = this.Units;
            ret.OrigDataRate = this.OrigDataRate;
            ret.OrigDataStartFrame = this.OrigDataStartFrame;
            ret.OrigNumFrames = this.OrigNumFrames;
            ret.Markers = this.Markers;
            ret.loaded = this.loaded;
            ret._state = this._state;
            return ret;
        }

        /// <summary>
        /// ファイル名を指定してTracked ASCIIをロードします。
        /// </summary>
        /// <param name="filename">ロードするファイル名</param>
        public void LoadTrack(string filename) {
            using (StreamReader reader = new StreamReader(filename)) {
                LoadTrack(reader);
            }
        }

        /// <summary>
        /// TextReaderからTracked ASCIIをロードします。
        /// </summary>
        /// <param name="reader">ロードするTextReader</param>
        /// <exception cref="System.IO.InvalidDataException"></exception>
        public void LoadTrack(TextReader reader) {
            string positionText = ""; // エラーメッセージ用
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
                for (int i = 2; i < lines.Length; i += 3) {
                    if (lines[i] != "") // 空のマーカー名は無視するように。
                        tmpMarkers.Add(lines[i]);
                }
                Markers = tmpMarkers.ToArray();
                positionText = "Read fifth line";
                lines = reader.ReadLine().Split('\t');
                positionText = "Read empty line";
                reader.ReadLine();
                loaded = true;
            } catch (Exception e) {
                throw new InvalidDataException(positionText, e);
            }
        }

        /// <summary>
        /// Tracked ASCIIのヘッダのうちマーカーセット名を含まない部分を出力します。
        /// </summary>
        /// <param name="writer">出力するTextWriter</param>
        public void WritePreHeader(TextWriter writer) {
            writer.WriteLine("PathFileType\t{0}\t{1}\t{2}", PathFileType.ToString(), AxisOrder, FilePath);
            writer.WriteLine("DataRate\tCameraRate\tNumFrames\tNumMarkers\tUnits\tOrigDataRate\tOrigDataStartFrame\tOrigNumFrames");
            writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", DataRate.ToString(), CameraRate.ToString(), NumFrames.ToString(), NumMarkers.ToString(), Units, OrigDataRate.ToString(), OrigDataStartFrame.ToString(), OrigNumFrames.ToString());
        }

        /// <summary>
        /// Tracked ASCIIのヘッダのうちマーカーセット名の部分を出力します。
        /// </summary>
        /// <param name="writer">出力するTextWriter</param>
        public void WriteMarkerHeader(TextWriter writer) {
            writer.Write("Frame#\tTime\t");
            foreach (var marker in Markers) {
                writer.Write("{0}\t\t\t", marker);
            }
            writer.WriteLine();
            writer.Write("\t");
            for (int i = 0; i < Markers.Length; i++) {
                writer.Write("X{0}\tY{0}\tZ{0}\t", (i + 1).ToString());
            }
            writer.WriteLine();
            writer.WriteLine();
        }

        public void OutputAsPhaseSpace(string inputFile, string outputFile) {
            OutputAsPhaseSpace(inputFile, outputFile, new DateTime(621355968000000000).ToLocalTime());
        }

        public void OutputAsPhaseSpace(string inputFile, string outputFile, DateTime beginTime) {
            _state = "Loading";
            using (StreamReader reader = new StreamReader(inputFile)) {
                this.LoadTrack(reader);
                using (StreamWriter writer = new StreamWriter(outputFile)) {
                    decimal epoch = (decimal)(beginTime.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                    decimal offset = 1 / decimal.Parse(this.DataRate);
                    int lineCnt = 0;
                    while (!reader.EndOfStream) {
                        lineCnt++;
                        _state = new StringBuilder().AppendFormat("Line {0} / {1}", lineCnt.ToString(), this.NumFrames.ToString()).ToString();
                        string[] lines = reader.ReadLine().Split('\t');
                        writer.Write("{0}", epoch.ToString("F7"));
                        for (int i = 0; i < this.NumMarkers; i++) {
                            if (i * 3 + 2 >= lines.Length || lines[i * 3 + 2] == "") {
                                writer.Write(",-1,0,0,0");
                            } else {
                                writer.Write(",5,{0},{1},{2}", lines[i * 3 + 2], lines[i * 3 + 3], lines[i * 3 + 4]);
                            }
                        }
                        writer.WriteLine();
                        epoch += offset;
                    }
                }
            }
            _state = "Finished";
        }
    }
}
