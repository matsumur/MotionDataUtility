using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EVaRTTrackHandler {
    class TrackLinker : TrackIO {
        public TrackLinker()
            : base() {
            //PrefixMain = "";
            //PrefixSecond = "";

        }


        /// <summary>
        /// 複数のTracked ASCIIファイルを横に結合します。
        /// マーカーセット名がつけられてない部分は放棄されます。
        /// </summary>
        /// <param name="inputFiles">入力ファイル名の配列</param>
        /// <param name="outputFile">出力先ファイル名</param>
        /// <returns>エラーがないか</returns>
        /// <exception cref="System.IO.InvalidDataException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.IO.PathTooLongException"></exception>
        /// <exception cref="System.IO.DirectoryNotFoundException"></exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public  bool LinkFiles(string[] inputFiles, string outputFile) {
            _state = "Loading";
            bool ret = true;
            if (inputFiles == null) throw new ArgumentNullException("inputFiles is null");
            if (inputFiles.Length == 0) throw new ArgumentException("inputFiles has no element");
            StreamReader[] readers = new StreamReader[inputFiles.Length];
            try {
                // 入力ファイルを開く
                TrackIO[] tracks = new TrackIO[inputFiles.Length];
                for (int i = 0; i < inputFiles.Length; i++) {
                    readers[i] = new StreamReader(inputFiles[i]);
                    tracks[i] = new TrackIO();
                    tracks[i].LoadTrack(readers[i]);
                }
                // 出力トラックファイルのヘッダーを設定
                TrackIO outTrack = tracks[0].Clone() as TrackIO;
                outTrack.FilePath = outputFile;
                outTrack.NumMarkers = 0;
                foreach (var track in tracks) outTrack.NumMarkers += track.NumMarkers;
                List<string> outMarkers = new List<string>();
                for (int i = 0; i < inputFiles.Length; i++) {
                    string prefix = Path.GetFileNameWithoutExtension(inputFiles[i]) + "_";
                    foreach (var marker in tracks[i].Markers)
                        outMarkers.Add(prefix + marker);
                }
                outTrack.Markers = outMarkers.ToArray();
                outTrack.NumMarkers = outTrack.Markers.Length;
                // フレーム数チェック
                for (int i = 0; i < inputFiles.Length; i++) {
                    if (tracks[i].NumFrames != outTrack.NumFrames) {
                        throw new InvalidDataException("Frame Length of Track Files mismatch. :" + inputFiles[i]);
                    }
                }
                // 書き込み
                using (StreamWriter writer = new StreamWriter(outputFile)) {
                    outTrack.WritePreHeader(writer);
                    outTrack.WriteMarkerHeader(writer);
                    try {
                        int lineCnt = 0;
                        while (true) {
                            // 全部EOFなら終わり
                            if (readers.All(x => { return x.EndOfStream; })) {
                                break;
                            }
                            lineCnt++;
                            _state = new StringBuilder().AppendFormat("Line {0} / {1}", lineCnt.ToString(), outTrack.NumFrames.ToString()).ToString();
                            // 各行の出力
                            string[] markerValues = new string[outTrack.NumMarkers * 3];
                            for (int i = 0; i < markerValues.Length; i++) markerValues[i] = "";
                            string frame = "", time = "";
                            int markerOffset = 0;
                            for (int i = 0; i < inputFiles.Length; i++) {
                                if (!readers[i].EndOfStream) {
                                    string[] lines = readers[i].ReadLine().Split('\t');
                                    if (frame == "" && lines.Length >= 2) {
                                        frame = lines[0];
                                        time = lines[1];
                                    }
                                    try {
                                        for (int j = 0; j < tracks[i].NumMarkers * 3; j++) {
                                            markerValues[j + markerOffset] = lines[j + 2];
                                        }
                                    } catch (IndexOutOfRangeException) { }
                                }
                                markerOffset += tracks[i].NumMarkers * 3;
                            }
                            if (frame != "") {
                                writer.Write("{0}\t{1}\t", frame, time);
                                foreach (var value in markerValues) writer.Write("{0}\t", value);
                                writer.WriteLine();
                            }
                        }
                    } catch (IOException) { ret = false; }
                }
            } finally {
                foreach (var reader in readers) {
                    if (reader != null) {
                        reader.Dispose();
                    }
                }
            }
            _state = "Finished";
            return ret;
        }
        /*
        public bool LinkFiles(string inputFile1, string inputFile2, string outputFile) {
            bool ret = true;
            using (StreamReader read1 = new StreamReader(inputFile1)) {
                using (StreamReader read2 = new StreamReader(inputFile2)) {
                    this.LoadTrack(read1);
                    TrackIO another = new TrackIO();
                    another.LoadTrack(read2);
                    if (this.NumFrames != another.NumFrames) {
                        ret = false;
                        LinkErrorMessage = "NumFrames mismatch";
                    }
                    TrackIO outTrack = this.Clone() as TrackIO;
                    outTrack.FilePath = outputFile;
                    outTrack.NumMarkers += another.NumMarkers;
                    List<string> markers = new List<string>();
                    foreach (var marker in this.Markers) markers.Add(PrefixMain + marker);
                    foreach (var marker in another.Markers) markers.Add(PrefixSecond + marker);
                    outTrack.Markers = markers.ToArray();
                    using (StreamWriter writer = new StreamWriter(outputFile)) {
                        outTrack.WritePreHeader(writer);
                        outTrack.WriteMarkerHeader(writer);
                        try {
                            while (!read1.EndOfStream || !read2.EndOfStream) {
                                string frame = "";
                                string time = "";
                                string[] markers1 = new string[this.NumMarkers * 3];
                                string[] markers2 = new string[another.NumMarkers * 3];
                                if (!read1.EndOfStream) {
                                    string[] lines = read1.ReadLine().Split('\t');
                                    if (lines.Length >= 2) {
                                        frame = lines[0];
                                        time = lines[1];
                                    }
                                    try {
                                        for (int i = 0; i < this.NumMarkers * 3; i++) {
                                            markers1[i] = lines[i + 2];
                                        }
                                    } catch (IndexOutOfRangeException) { }
                                }
                                if (!read2.EndOfStream) {
                                    string[] lines = read2.ReadLine().Split('\t');
                                    if (lines.Length >= 2 && frame == "") {
                                        frame = lines[0];
                                        time = lines[1];
                                    }
                                    try {
                                        for (int i = 0; i < another.NumMarkers * 3; i++) {
                                            markers2[i] = lines[i + 2];
                                        }
                                    } catch (IndexOutOfRangeException) { }
                                }
                                for (int i = 0; i < this.NumMarkers * 3; i++) {
                                    if (markers1[i] == null) markers1[i] = "";
                                }
                                for (int i = 0; i < another.NumMarkers * 3; i++) {
                                    if (markers2[i] == null) markers2[i] = "";
                                }
                                writer.Write("{0}\t{1}\t", frame, time);
                                foreach (var marker in markers1) writer.Write("{0}\t", marker);
                                foreach (var marker in markers2) writer.Write("{0}\t", marker);
                                writer.WriteLine();
                            }
                        } catch (IOException) { }
                    }
                }
            }
            return ret;
        }
         * */
    }
}

