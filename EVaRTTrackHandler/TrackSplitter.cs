using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EVaRTTrackHandler {
    class TrackSplitter : TrackIO {
        public TrackSplitter()
            : base() {
        }
        
        /// <summary>
        /// 既定の分割ファイルの出力先を取得します。
        /// </summary>
        /// <param name="filename">元となるファイル名</param>
        /// <param name="index">出力インデックス</param>
        /// <returns>ファイル名</returns>
        /// <exception cref="System.ArgumentException"></exception>
        public string GetSplitFilename(string filename, int index) {
            string dir = Path.GetDirectoryName(filename);
            string basename = Path.GetFileNameWithoutExtension(filename);
            string ext = Path.GetExtension(filename);
            return Path.Combine(dir, new StringBuilder().AppendFormat("{0}.part{1}{2}", basename, index, ext).ToString());
        }

        /// <summary>
        /// Tracked ASCIIファイルを指定されたフレーム数以下のフレームを含む複数のファイルに分割します。
        /// </summary>
        /// <param name="filename">分割ファイル名</param>
        /// <param name="limit">各出力ファイルのフレーム数の上限</param>
        /// <returns>ファイルを分割する必要があったか</returns>
        /// <exception cref="System.IO.InvalidDataException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.IO.PathTooLongException"></exception>
        /// <exception cref="System.IO.DirectoryNotFoundException"></exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public bool SplitTrackFile(string filename, int limit) {
            _state = "Loading";
            using (StreamReader reader = new StreamReader(filename)) {
                this.LoadTrack(reader);
                int split = (int)Math.Ceiling((double)this.NumFrames / limit);
                if (split <= 1) return false;

                int lineCnt = 0;
                int restLines = this.NumFrames;
                int onceLines = (int)Math.Ceiling((double)restLines / split);
                for (int i = 0; i < split && restLines > 0; i++) {
                    string outfile = GetSplitFilename(filename, i + 1);
                    using (StreamWriter writer = new StreamWriter(outfile)) {
                        TrackIO track = this.Clone() as TrackIO;
                        if (onceLines > restLines) onceLines = restLines;
                        track.NumFrames = onceLines;
                        track.FilePath = outfile;

                        track.WritePreHeader(writer);
                        track.WriteMarkerHeader(writer);
                        int j = 0;
                        try {
                            for (j = 0; j < onceLines; j++) {
                                lineCnt++;
                                _state = new StringBuilder().AppendFormat("Line {0} / {1}", lineCnt.ToString(), this.NumFrames.ToString()).ToString();
                                writer.WriteLine(reader.ReadLine());
                            }
                        } catch (IOException) {
                            restLines = 0;
                        }
                        restLines -= j;
                    }
                }
            }
            _state = "Finished";
            return true;
        }

        public bool CutFramesTrackFile(string inputFile, string outputFile, int cutFrames, bool onTail) {
            _state = "Loading";
            bool ret = true;
            TrackIO trackIn = new TrackIO();
            using (StreamReader reader = new StreamReader(inputFile)) {
                trackIn.LoadTrack(reader);
                TrackIO trackOut = trackIn.Clone() as TrackIO;
                trackOut.NumFrames -= cutFrames;
                if (trackOut.NumFrames < 0) {
                    trackOut.NumFrames = 0;
                }
                using (StreamWriter writer = new StreamWriter(outputFile)) {
                    trackOut.WritePreHeader(writer);
                    trackOut.WriteMarkerHeader(writer);
                    if (onTail) {
                        for (int i = 0; i < trackOut.NumFrames && !reader.EndOfStream; i++) {
                            writer.WriteLine(reader.ReadLine());
                        }
                    } else {
                        for (int i = 0; i < cutFrames && !reader.EndOfStream; i++) reader.ReadLine();
                        while (!reader.EndOfStream) {
                            writer.WriteLine(reader.ReadLine());
                        }
                    }
                }
            }
            _state = "Finished";
            return ret;
        }
        public bool AddFramesTrackFile(string inputFile, string outputFile, int addFrames, bool onTail) {
            _state = "Loading";
            bool ret = true;
            TrackIO trackIn = new TrackIO();
            int trackWidth = 0;
            using (StreamReader reader = new StreamReader(inputFile)) {
                trackIn.LoadTrack(reader);
                try {
                    // 無名マーカーも含めたマーカー数
                    // 実際には配列長より\tの数のほうが正しいらしい
                    trackWidth = reader.ReadLine().Split('\t').Length / 3;
                } catch (IOException) { }
            }
            trackWidth = Math.Max(trackWidth, trackIn.NumMarkers);
            TrackIO trackOut = trackIn.Clone() as TrackIO;
            trackOut.NumFrames += addFrames;
            // 一行読んだのでもう一回
            using (StreamReader reader = new StreamReader(inputFile)) {
                trackIn.LoadTrack(reader);
                using (StreamWriter writer = new StreamWriter(outputFile)) {
                    trackOut.WritePreHeader(writer);
                    trackOut.WriteMarkerHeader(writer);
                    if (onTail) {
                        while (!reader.EndOfStream) {
                            writer.WriteLine(reader.ReadLine());
                        }
                    }
                        for (int i = 0; i < addFrames; i++) {
                            writer.Write("0\t0\t"); // 適当に
                            for (int j = 0; j < trackWidth; j++) writer.Write("\t\t\t");
                            writer.WriteLine();
                        }
                        if( !onTail){
                        while (!reader.EndOfStream) {
                            writer.WriteLine(reader.ReadLine());
                        }
                    }
                }
            }
            _state = "Finished";
            return ret;
        }

        /// <summary>
        /// 複数のTracked ASCIIファイルを縦につなげます。
        /// </summary>
        /// <param name="inputFiles">入力ファイルの配列</param>
        /// <param name="outputFile">出力先ファイル名</param>
        /// <returns></returns>
        public bool ConcatTrackFile(string[] inputFiles, string outputFile) {
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
                outTrack.NumFrames = 0;
                foreach (var track in tracks) outTrack.NumFrames += track.NumFrames;
                // マーカー数チェック
                for (int i = 0; i < inputFiles.Length; i++) {
                    if (tracks[i].NumMarkers != outTrack.NumMarkers) {
                        throw new InvalidDataException("Marker Length of Track Files mismatch. :" + inputFiles[i]);
                    }
                }
                using (StreamWriter writer = new StreamWriter(outputFile)) {
                    outTrack.WritePreHeader(writer);
                    outTrack.WriteMarkerHeader(writer);
                    int readerCnt = 0;
                    foreach (var reader in readers) {
                        int lineCnt = 0;
                        while (!reader.EndOfStream) {
                            lineCnt++;
                            _state = new StringBuilder().AppendFormat("Line {0} / {1}, File {2} / {3}", lineCnt.ToString(), this.NumFrames.ToString(), (readerCnt + 1).ToString(), readers.Length.ToString()).ToString();
                            writer.WriteLine(reader.ReadLine());
                        }
                        readerCnt++;
                    }
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
    }
}

