using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.DirectX;

namespace MotionDataHandler.Motion.Old {
    /// <summary>
    /// モーションデータのフレーム群を保持するクラス
    /// </summary>
    public class MotionDataSet : IXmlSerializable, IDisposable {

        /// <summary>
        /// オブジェクトがバックグラウンド処理の経過を報告するときに呼び出されるイベント。
        /// </summary>
        public event System.ComponentModel.ProgressChangedEventHandler ProgressChanged = new System.ComponentModel.ProgressChangedEventHandler((s, o) => { });

        /// <summary>
        /// オブジェクトを破棄します。
        /// </summary>
        public void Dispose() {
            Clear();
        }

        /// <summary>
        /// モーションデータオブジェクトの基本上を保持するヘッダを取得または変更します。
        /// </summary>
        [XmlIgnore]
        public MotionDataHeader Header {
            get {
                if(_header == null) {
                    Header = new MotionDataHeader();
                }
                return _header;
            }
            set {
                if(_header != null && _header.Parent == this && _header != value) {
                    MotionDataHeader tmp = _header;
                    _header = value;
                    tmp.Parent = null;
                } else {
                    _header = value;
                }
                if(_header != null && _header.Parent != this) {
                    _header.Parent = this;
                }
            }
        }
        private MotionDataHeader _header;

        SortedList<decimal, MotionDataFrame> _frames;

        /// <summary>
        /// モーションデータのフレーム数を取得します。
        /// </summary>
        public int FrameLength { get { if(_frames == null) return 0; return _frames.Count; } }
        /// <summary>
        /// モーションデータの最終フレーム時間を取得します。
        /// </summary>
        public decimal Duration {
            get { if(FrameLength == 0) return 0; return _frames.Keys.Last(); }
        }

        /// <summary>
        /// フレーム時間の列挙を取得します。
        /// </summary>
        public IEnumerable<decimal> FrameTimes {
            get { foreach(var time in _frames.Keys) { yield return time; } }
        }
        /// <summary>
        /// フレームインデックスの列挙を取得します。
        /// </summary>
        public IEnumerable<int> FrameIndices {
            get { for(int i = 0; i < FrameLength; i++) { yield return i; } }
        }

        /// <summary>
        /// フレーム時間とフレームのペアの列挙を取得します。
        /// </summary>
        public IEnumerable<KeyValuePair<decimal, MotionDataFrame>> Frames {
            get { foreach(var pair in _frames) { yield return pair; } }
        }


        public void Clear() {
            _frames.Clear();
            this.Header.Clear();
        }

        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        public MotionDataSet() {
            Header = new MotionDataHeader();
            _frames = new SortedList<decimal, MotionDataFrame>();
        }
        
        private void doProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e) {
            if(ProgressChanged != null)
                ProgressChanged.Invoke(sender, e);
        }

        public void ReportProgress(int progressPercentage, object userState) {
            System.ComponentModel.ProgressChangedEventArgs e = new System.ComponentModel.ProgressChangedEventArgs(progressPercentage, userState);
            doProgressChanged(this, e);
        }

        
        /// <summary>
        /// 指定されたフレーム時間にフレームを追加します。
        /// </summary>
        /// <param title="frame">追加されるフレーム</param>
        /// <param title="time">追加されるフレーム時間の位置</param>
        public void AddFrame(decimal time, MotionDataFrame frame) {
            _frames[time] = frame;
        }

       
        #region Serialize

        public System.Xml.Schema.XmlSchema GetSchema() {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// バイナリ形式からMotionDataSetを読み込みます。
        /// </summary>
        /// <param title="reader">読み込み元</param>
        public void ReadBinary(BinaryReader reader) {
            string mdsb = reader.ReadString();
            if(mdsb != "MDSB") {
                throw new InvalidDataException("Invalid file format");
            }
            this.ReportProgress(0, "Load Data Set: Header");
            string headerString = reader.ReadString();

            using(MemoryStream headerStream = new MemoryStream(Encoding.UTF8.GetBytes(headerString)))
            using(XmlReader headerReader = XmlReader.Create(headerStream)) {
                Header = MotionDataHeader.Deserialize(headerReader);
            }

            string version = reader.ReadString();
            if(version != "BinaryVersion:1") {
                throw new InvalidDataException("binary version mismatch");
            }
            int length = reader.ReadInt32();
            for(int i = 0; i < length; i++) {
                this.ReportProgress(100 * i / length, string.Format("Load Data Set: {0} / {1}", i, length));

                decimal time = reader.ReadDecimal();
                MotionDataFrame frame = new MotionDataFrame();
                frame.ReadBinary(reader);
                AddFrame(time, frame);
            }
        }

        /// <summary>
        /// MotionDataSetをバイナリ形式で出力します。
        /// </summary>
        /// <param title="writer">書き込み先</param>
        public void WriteBinary(BinaryWriter writer) {
            writer.Write("MDSB");

            this.ReportProgress(0, "Save Data Set: Header");
            MemoryStream headerStream = new MemoryStream();
            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Indent = true;
            using(XmlWriter headerWriter = XmlWriter.Create(headerStream, setting)) {
                Header.Serialize(headerWriter);
            }
            headerStream.Seek(0, SeekOrigin.Begin);
            StreamReader headerReader = new StreamReader(headerStream);
            string headerString = headerReader.ReadToEnd();
            writer.Write(headerString);

            writer.Write("BinaryVersion:1");

            writer.Write(_frames.Count);
            int length = _frames.Count;
            int count = 0;
            foreach(var pair in _frames) {
                this.ReportProgress(100 * count / length, string.Format("Save Data Set: {0} / {1}", count, length));
                count++;

                writer.Write(pair.Key);
                pair.Value.WriteBinary(writer);
            }
        }

        /// <summary>
        /// データをXML形式から読み込みます。
        /// </summary>
        /// <param title="reader"></param>
        public void ReadXml(XmlReader reader) {
            reader.MoveToContent();
            if(reader.IsEmptyElement) {
                reader.Skip();
                return;
            }
            reader.ReadStartElement("MotionDataSet");
            int frameLength = 0;
            int count = 0;
            for(reader.MoveToContent(); reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None; reader.MoveToContent()) {
                switch(reader.Name) {
                case "MotionDataHeader":
                    lock(this) {
                        Header = MotionDataHeader.Deserialize(reader);
                    }
                    break;
                case "EnumerateFrame()":
                    if(reader.IsEmptyElement) {
                        reader.Skip();
                        continue;
                    }
                    string lengthStr;
                    if((lengthStr = reader.GetAttribute("Length")) != null) {
                        int length;
                        if(int.TryParse(lengthStr, out length)) {
                            frameLength = length;
                        }
                    }
                    reader.ReadStartElement("EnumerateFrame()");
                    while(reader.NodeType != XmlNodeType.None) {
                        if(reader.NodeType == XmlNodeType.EndElement)
                            break;
                        string timeStr;
                        switch(reader.Name) {
                        case "Frame":
                            if(reader.IsEmptyElement || (timeStr = reader.GetAttribute("Time")) == null) {
                                reader.Skip();
                                continue;
                            }
                            reader.ReadStartElement("Frame");
                            decimal time;
                            if(decimal.TryParse(timeStr, out time) && reader.IsStartElement("MotionDataFrame")) {
                                MotionDataFrame frame = MotionDataFrame.Deserialize(reader);
                                AddFrame(time, frame);
                                if(frameLength != 0) {
                                    this.ReportProgress(100 * count / frameLength, string.Format("Load Data Set: {0} / {1}", count, frameLength));
                                } else {
                                    this.ReportProgress(100 - (10000 / (count + 100)), string.Format("Load Data Set: {0}", count));
                                }
                                count++;
                            }
                            while(reader.NodeType != XmlNodeType.None && reader.NodeType != XmlNodeType.EndElement)
                                reader.Skip();
                            reader.ReadEndElement();

                            break;
                        default:
                            reader.Skip();
                            break;
                        }
                    }
                    while(reader.NodeType != XmlNodeType.None && reader.NodeType != XmlNodeType.EndElement)
                        reader.Skip();
                    reader.ReadEndElement();

                    break;
                default:
                    reader.Skip();
                    break;
                }
            }
            reader.ReadEndElement();
        }

        /// <summary>
        /// オブジェクトのデータをXML形式でかき出します。
        /// </summary>
        /// <param title="writer"></param>
        public void WriteXml(XmlWriter writer) {
            this.ReportProgress(0, "Save Data Set: Header");
            Header.Serialize(writer);
            writer.WriteStartElement("EnumerateFrame()");
            writer.WriteStartAttribute("Length");
            writer.WriteValue(_frames.Count);
            writer.WriteEndAttribute();

            int length = FrameLength;
            int count = 0;
            foreach(var pair in _frames) {
                this.ReportProgress(100 * count / length, string.Format("Save Data Set: {0} / {1}", count, length));
                count++;

                writer.WriteStartElement("Frame");
                writer.WriteStartAttribute("Time");
                writer.WriteValue(pair.Key);
                writer.WriteEndAttribute();
                pair.Value.Serialize(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// 現在のオブジェクトをストリームからのデータで上書きします。
        /// </summary>
        /// <param title="stream"></param>
        public void RetrieveXml(Stream stream) {
            _frames.Clear();
            this.ReadXml(XmlReader.Create(stream));
        }

        /// <summary>
        /// 現在のオブジェクトをストリームからのデータで上書きします。
        /// </summary>
        /// <param title="stream"></param>
        public void RetrieveBinary(Stream stream) {
            _frames.Clear();
            Header.Clear();
            BinaryReader reader = new BinaryReader(stream);
            this.ReadBinary(reader);
        }

        #endregion

    }
}
