using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MotionDataHandler;
using MotionDataHandler.DataIO;
using System.IO;

namespace EVaRTTrackHandler {
    public class TrackHandler {
        public static void OutputAsPhaseSpace(string inputFile, string outputFile) {
            OutputAsPhaseSpace(inputFile, outputFile, 0);
        }
        public static void OutputAsPhaseSpace(string inputFile, string outputFile, decimal offset) {
            using (TrcReader reader = new TrcReader(inputFile)) {
                using (PhaseSpaceDataWriter writer = new PhaseSpaceDataWriter(outputFile)) {
                    decimal ratio = 1 / decimal.Parse(reader.Header.DataRate);
                    while (!reader.EndOfStream) {
                        TrcFrame trc = reader.ReadFrame();
                        trc = trc.TrimUnnamed(reader.Header);
              PhaseSpaceFrame motion = FrameConverter.GetMotionFrame(trc);
                        motion.Time = offset + ratio * writer.Frames;
                        writer.WriteFrame(motion);
                    }
                }
            }
        }

        public static bool JoinTracks(string[] inputFiles, string outputFile) {
            if (inputFiles == null) throw new ArgumentNullException("inputFiles is null");
            if (inputFiles.Length == 0) throw new ArgumentException("inputFiles has no element");

            TrcReader[] readers = new TrcReader[inputFiles.Length];
            try {
                for (int i = 0; i < inputFiles.Length; i++) {
                    readers[i] = new TrcReader(inputFiles[i]);
                }
                int numFrames = readers[0].Header.NumFrames;
                for (int i = 1; i < readers.Length; i++) {
                    if (readers[i].Header.NumFrames != numFrames) {
                        throw new InvalidDataException("Frame Length of Track Files mismatch. :" + inputFiles[i]);
                    }
                }
                TrcHeader outHeader = readers[0].Header;
                outHeader.FilePath = outputFile;
                List<string> outMarkers = new List<string>();
                for (int i = 0; i < inputFiles.Length; i++) {
                    string prefix = Path.GetFileNameWithoutExtension(inputFiles[i]) + "_";
                    foreach (var marker in readers[i].Header.Markers)
                        outMarkers.Add(prefix + marker);
                }
                outHeader.Markers = outMarkers.ToArray();
                outHeader.NumMarkers = outHeader.Markers.Length;
                using (TrcWriter writer = new TrcWriter(outHeader, outputFile)) {
                    while (true) {
                        // 全部EOFなら終わり
                        if (readers.All(x => { return x.EndOfStream; }))
                            break;
                        // 半端にEOFならエラー
                        if (readers.Any(x => { return x.EndOfStream; }))
                            throw new InvalidDataException("Actual Frame Length mismatch");
                        // 各行の入力
                        TrcFrame[] frames = new TrcFrame[readers.Length];
                        for (int i = 0; i < readers.Length; i++)
                            frames[i] = readers[i].ReadFrame().TrimUnnamed(readers[i].Header);
                        // 各行の出力
                        TrcFrame outFrame = new TrcFrame();
                        outFrame.Markers = new TrcMarker?[0];
                        outFrame = outFrame.JoinAll(frames);
                        writer.WriteFrame(outFrame);
                    }
                }
            } finally {
                foreach (var reader in readers) {
                    try {
                        if (reader != null) reader.Dispose();
                    } catch (Exception) { }
                }
            }
            return true;
        }


        /// <summary>
        /// 既定の分割ファイルの出力先を取得します。
        /// </summary>
        /// <param title="filename">元となるファイル名</param>
        /// <param title="index">出力インデックス</param>
        /// <returns>ファイル名</returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static string GetSplitFilename(string filename, int index) {
            string dir = Path.GetDirectoryName(filename);
            string basename = Path.GetFileNameWithoutExtension(filename);
            string ext = Path.GetExtension(filename);
            return Path.Combine(dir, new StringBuilder().AppendFormat("{0}.part{1}{2}", basename, index, ext).ToString());
        }

        /// <summary>
        /// Tracked ASCIIファイルを指定されたフレーム数以下のフレームを含む複数のファイルに分割します。
        /// </summary>
        /// <param title="filename">分割ファイル名</param>
        /// <param title="limit">各出力ファイルのフレーム数の上限</param>
        /// <returns>ファイルを分割する必要があったか</returns>
        /// <exception cref="System.IO.InvalidDataException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.IO.PathTooLongException"></exception>
        /// <exception cref="System.IO.DirectoryNotFoundException"></exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public static bool SplitTrackFile(string filename, int limit) {
            using (TrcReader reader = new TrcReader(filename)) {
                int split = (int)Math.Ceiling((double)reader.Header.NumFrames / limit);
                if (split <= 1) return false;

                int restLines = reader.Header.NumFrames;
                int onceLines = (int)Math.Ceiling((double)restLines / split);
                for (int i = 0; i < split && restLines > 0; i++) {
                    string outfile = GetSplitFilename(filename, i + 1);
                    TrcHeader outHeader = reader.Header;
                    outHeader.FilePath = outfile;
                    if (onceLines > restLines) onceLines = restLines;
                    outHeader.NumFrames = onceLines;
                    using (TrcWriter writer = new TrcWriter(outHeader, outfile)) {
                        try {
                            for (int j = 0; j < onceLines; j++) {
                                writer.WriteFrame(reader.ReadFrame());
                            }
                            restLines -= onceLines;
                        } catch (Exception) {
                            restLines = 0;
                        }
                    }
                }
            }
            return true;
        }

        public static bool CutFramesTrackFile(string inputFile, string outputFile, int cutFrames, bool onTail) {
            if (cutFrames < 0) return false;
            bool ret = true;
            using (TrcReader reader = new TrcReader(inputFile)) {
                TrcHeader outHeader = reader.Header;
                outHeader.FilePath = outputFile;
                outHeader.NumFrames -= cutFrames;
                if (outHeader.NumFrames < 0) {
                    outHeader.NumFrames = 0;
                }
                using (TrcWriter writer = new TrcWriter(outHeader, outputFile)) {
                    if (onTail) {
                        for (int i = 0; i < outHeader.NumFrames && !reader.EndOfStream; i++) {
                            writer.WriteFrame(reader.ReadFrame());
                        }
                    } else {
                        for (int i = 0; i < cutFrames && !reader.EndOfStream; i++) reader.ReadFrame();
                        while (!reader.EndOfStream) {
                            writer.WriteFrame(reader.ReadFrame());
                        }
                    }
                }
            }
            return ret;
        }
        public static bool AddFramesTrackFile(string inputFile, string outputFile, int addFrames, bool onTail) {
            if (addFrames < 0) return false;
            bool ret = true;
            // 一行読んだのでもう一回
            using (TrcReader reader = new TrcReader(inputFile)) {
                TrcHeader outHeader = reader.Header;
                outHeader.FilePath = outputFile;
                outHeader.NumFrames += addFrames;
                using (TrcWriter writer = new TrcWriter(outHeader, outputFile)) {
                    if (onTail) {
                        while (!reader.EndOfStream) {
                            writer.WriteFrame(reader.ReadFrame());
                        }
                    }
                    for (int i = 0; i < addFrames; i++) {
                        writer.WriteFrame(new TrcFrame());
                        if (!onTail) {
                            while (!reader.EndOfStream) {
                                writer.WriteFrame(reader.ReadFrame());
                            }
                        }
                    }
                }
                return ret;
            }

        }

        public static bool TrimUnnamedFile(string inputFile, string outputFile) {
            if (inputFile == null) throw new ArgumentNullException("inputFiles is null");
            if (outputFile == null) throw new ArgumentNullException("outputFile is null");
            using (TrcReader reader = new TrcReader(inputFile)) {
                TrcHeader outHeader = reader.Header;
                outHeader.FilePath = outputFile;
                using (TrcWriter writer = new TrcWriter(outHeader, outputFile)) {
                    while (!reader.EndOfStream) {
                        writer.WriteFrame(reader.ReadFrame().TrimUnnamed(reader.Header));
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 複数のTracked ASCIIファイルを縦につなげます。
        /// </summary>
        /// <param title="inputFiles">入力ファイルの配列</param>
        /// <param title="outputFile">出力先ファイル名</param>
        /// <returns></returns>
        public static bool ConcatTrackFile(string[] inputFiles, string outputFile) {
            bool ret = true;
            if (inputFiles == null) throw new ArgumentNullException("inputFiles is null");
            if (inputFiles.Length == 0) throw new ArgumentException("inputFiles has no element");
            TrcReader[] readers = new TrcReader[inputFiles.Length];
            try {
                // 入力ファイルを開く
                for (int i = 0; i < inputFiles.Length; i++) {
                    readers[i] = new TrcReader(inputFiles[i]);
                }
                // 出力トラックファイルのヘッダーを設定
                TrcHeader outHeader = readers[0].Header;
                outHeader.FilePath = outputFile;
                outHeader.NumFrames = readers.Sum(x => x.Header.NumFrames);
                // マーカー数チェック
                for (int i = 0; i < inputFiles.Length; i++) {
                    if (readers[i].Header.NumMarkers != outHeader.NumMarkers) {
                        throw new InvalidDataException("Marker Length of Track Files mismatch. :" + inputFiles[i]);
                    }
                }
                using (TrcWriter writer = new TrcWriter(outHeader, outputFile)) {
                    foreach (var reader in readers) {
                        while (!reader.EndOfStream) {
                            writer.WriteFrame(reader.ReadFrame());
                        }
                    }
                }
            } finally {
                foreach (var reader in readers) {
                    if (reader != null) 
                        reader.Dispose();
                }
            }
            return ret;
        }
    }
}