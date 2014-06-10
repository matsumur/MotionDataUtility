using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using System.Xml.Schema;

namespace MotionDataHandler.Motion.Old {
    using Misc;

    /// <summary>
    /// モーションデータオブジェクト群の位置フレームを保持するクラス
    /// </summary>
    public class MotionDataFrame : IXmlSerializable {
        private MotionDataPoint[] _points;
        private MotionDataLine[] _lines;
        private MotionDataSphere[] _spheres;
        private MotionDataCylinder[] _cylinders;
        private MotionDataPlane[] _planes;

        /// <summary>
        /// MotionDataPointの配列を取得または設定します。
        /// </summary>
        public MotionDataPoint[] Points {
            get { if(_points == null) { _points = new MotionDataPoint[0]; } return _points; }
            set { _points = value; }
        }
        /// <summary>
        /// MotionDataLineの配列を取得または設定します。
        /// </summary>
        public MotionDataLine[] Lines {
            get { if(_lines == null) { _lines = new MotionDataLine[0]; } return _lines; }
            set { _lines = value; }
        }
        /// <summary>
        /// MotionDataCylinderの配列を取得または設定します。
        /// </summary>
        public MotionDataCylinder[] Cylinders {
            get { if(_cylinders == null) { _cylinders = new MotionDataCylinder[0]; } return _cylinders; }
            set { _cylinders = value; }
        }
        /// <summary>
        /// MotionDataSphereの配列を取得または設定します。
        /// </summary>
        public MotionDataSphere[] Spheres {
            get { if(_spheres == null) { _spheres = new MotionDataSphere[0]; } return _spheres; }
            set { _spheres = value; }
        }
        /// <summary>
        /// MotionDataPlaneの配列を取得または設定します。
        /// </summary>
        public MotionDataPlane[] Planes {
            get { if(_planes == null) { _planes = new MotionDataPlane[0]; } return _planes; }
            set { _planes = value; }
        }

        public void AddObject<TMotionDataObject>(params TMotionDataObject[] obj) {
            Type tType = typeof(TMotionDataObject);
            if(tType == typeof(MotionDataPoint)) {
                AddPoint(obj as MotionDataPoint[]);
            } else if(tType == typeof(MotionDataLine)) {
                AddLine(obj as MotionDataLine[]);
            } else if(tType == typeof(MotionDataCylinder)) {
                AddCylinder(obj as MotionDataCylinder[]);
            } else if(tType == typeof(MotionDataSphere)) {
                AddSphere(obj as MotionDataSphere[]);
            } else if(tType == typeof(MotionDataPlane)) {
                AddPlane(obj as MotionDataPlane[]);
            } else {
                throw new ArgumentException("'TMotionDataObject' is not MotionDataObject");
            }
        }

        /// <summary>
        /// MotionDataPointをフレームに追加します。
        /// </summary>
        /// <param title="points">追加されるモーションデータオブジェクト</param>
        public void AddPoint(params MotionDataPoint[] points) {
            List<MotionDataPoint> tmp = new List<MotionDataPoint>(Points);
            tmp.AddRange(points);
            Points = tmp.ToArray();
        }
        /// <summary>
        /// MotionDataLineをフレームに追加します。
        /// </summary>
        /// <param title="points">追加されるモーションデータオブジェクト</param>
        public void AddLine(params MotionDataLine[] lines) {
            List<MotionDataLine> tmp = new List<MotionDataLine>(Lines);
            tmp.AddRange(lines);
            Lines = tmp.ToArray();
        }
        /// <summary>
        /// MotionDataSphereをフレームに追加します。
        /// </summary>
        /// <param title="points">追加されるモーションデータオブジェクト</param>
        public void AddSphere(params MotionDataSphere[] spheres) {
            List<MotionDataSphere> tmp = new List<MotionDataSphere>(Spheres);
            tmp.AddRange(spheres);
            Spheres = tmp.ToArray();
        }
        /// <summary>
        /// MotionDataCylinderをフレームに追加します。
        /// </summary>
        /// <param title="points">追加されるモーションデータオブジェクト</param>
        public void AddCylinder(params MotionDataCylinder[] cylinders) {
            List<MotionDataCylinder> tmp = new List<MotionDataCylinder>(Cylinders);
            tmp.AddRange(cylinders);
            Cylinders = tmp.ToArray();
        }
        /// <summary>
        /// MotionDataPlaneをフレームに追加します。
        /// </summary>
        /// <param title="points">追加されるモーションデータオブジェクト</param>
        public void AddPlane(params MotionDataPlane[] planes) {
            List<MotionDataPlane> tmp = new List<MotionDataPlane>(Planes);
            tmp.AddRange(planes);
            Planes = tmp.ToArray();
        }

        /// <summary>
        /// 指定されたMotionDataObjectInfoに対応するモーションデータオブジェクトをフレームから削除します。
        /// </summary>
        /// <param title="targetInfo">削除するモーションデータオブジェクトのMotionDataObjectInfo</param>
        public void RemoveAt(MotionDataObjectInfo targetInfo) {
            RemoveAt(targetInfo.ObjectType, targetInfo.ObjectIndex);
        }
        /// <summary>
        /// 指定されたモーションデータオブジェクトをフレームから削除します。
        /// </summary>
        /// <param title="targetType">削除するモーションデータオブジェクトのタイプ</param>
        /// <param title="targetIndex">削除するモーションデータオブジェクトのインデックス</param>
        public void RemoveAt(MotionDataObjectType targetType, int targetIndex) {
            switch(targetType) {
            case MotionDataObjectType.Point:
                if(targetIndex < 0 || targetIndex >= Points.Length)
                    throw new ArgumentOutOfRangeException("targetIndex", "'targetIndex' out of range");
                List<MotionDataPoint> tmpPoint = new List<MotionDataPoint>(Points);
                tmpPoint.RemoveAt(targetIndex);
                Points = tmpPoint.ToArray();
                break;
            case MotionDataObjectType.Line:
                if(targetIndex < 0 || targetIndex >= Lines.Length)
                    throw new ArgumentOutOfRangeException("targetIndex", "targetIndex", "'targetIndex' out of range");
                List<MotionDataLine> tmpLine = new List<MotionDataLine>(Lines);
                tmpLine.RemoveAt(targetIndex);
                Lines = tmpLine.ToArray();
                break;
            case MotionDataObjectType.Cylinder:
                if(targetIndex < 0 || targetIndex >= Cylinders.Length)
                    throw new ArgumentOutOfRangeException("targetIndex", "'targetIndex' out of range");
                List<MotionDataCylinder> tmpCylinder = new List<MotionDataCylinder>(Cylinders);
                tmpCylinder.RemoveAt(targetIndex);
                Cylinders = tmpCylinder.ToArray();
                break;
            case MotionDataObjectType.Sphere:
                if(targetIndex < 0 || targetIndex >= Spheres.Length)
                    throw new ArgumentOutOfRangeException("targetIndex", "'targetIndex' out of range");
                List<MotionDataSphere> tmpSphere = new List<MotionDataSphere>(Spheres);
                tmpSphere.RemoveAt(targetIndex);
                Spheres = tmpSphere.ToArray();
                break;
            case MotionDataObjectType.Plane:
                if(targetIndex < 0 || targetIndex >= Planes.Length)
                    throw new ArgumentOutOfRangeException("targetIndex", "'targetIndex' out of range");
                List<MotionDataPlane> tmpPlane = new List<MotionDataPlane>(Planes);
                tmpPlane.RemoveAt(targetIndex);
                Planes = tmpPlane.ToArray();
                break;
            default:
                throw new ArgumentException("'targetType' must be single object type", "targetType");
            }
        }

        
        #region IXmlSerializable

        public void Serialize(XmlWriter writer) {
            XmlSerializer sel = new XmlSerializer(typeof(MotionDataFrame));
            sel.Serialize(writer, this);
        }

        public static MotionDataFrame Deserialize(XmlReader reader) {
            XmlSerializer sel = new XmlSerializer(typeof(MotionDataFrame));
            return (MotionDataFrame)sel.Deserialize(reader);
        }

        public System.Xml.Schema.XmlSchema GetSchema() {
            throw new System.NotImplementedException();
        }

        public void ReadXml(XmlReader reader) {
            if(reader.IsEmptyElement) {
                reader.Skip();
                return;
            }
            reader.ReadStartElement("MotionDataFrame");
            string lengthStr;
            int length;
            while(reader.NodeType != XmlNodeType.None) {
                if(reader.NodeType == XmlNodeType.EndElement)
                    break;
                switch(reader.Name) {
                case "Points":
                    if(reader.IsEmptyElement || (lengthStr = reader.GetAttribute("Length")) == null) {
                        reader.Skip();
                    } else {
                        reader.ReadStartElement();
                        if(int.TryParse(lengthStr, out length)) {
                            string contents = reader.ReadString();
                            readPointsFromString(length, contents);
                        }
                        while(reader.NodeType != XmlNodeType.None && reader.NodeType != XmlNodeType.EndElement)
                            reader.Skip();
                        reader.ReadEndElement();
                    }
                    break;
                case "Lines":
                    if(reader.IsEmptyElement || (lengthStr = reader.GetAttribute("Length")) == null) {
                        reader.Skip();
                    } else {
                        reader.ReadStartElement();
                        if(int.TryParse(lengthStr, out length)) {
                            string contents = reader.ReadString();
                            readLinesFromString(length, contents);
                        }
                        while(reader.NodeType != XmlNodeType.None && reader.NodeType != XmlNodeType.EndElement)
                            reader.Skip();
                        reader.ReadEndElement();
                    }
                    break;
                case "Cylinders":
                    if(reader.IsEmptyElement || (lengthStr = reader.GetAttribute("Length")) == null) {
                        reader.Skip();
                    } else {
                        reader.ReadStartElement();
                        if(int.TryParse(lengthStr, out length)) {
                            string contents = reader.ReadString();
                            readCylindersFromString(length, contents);
                        }
                        while(reader.NodeType != XmlNodeType.None && reader.NodeType != XmlNodeType.EndElement)
                            reader.Skip();
                        reader.ReadEndElement();
                    }
                    break;
                case "Spheres":
                    if(reader.IsEmptyElement || (lengthStr = reader.GetAttribute("Length")) == null) {
                        reader.Skip();
                    } else {
                        reader.ReadStartElement();
                        if(int.TryParse(lengthStr, out length)) {
                            string contents = reader.ReadString();
                            ReadSpheresFromString(length, contents);
                        }
                        while(reader.NodeType != XmlNodeType.None && reader.NodeType != XmlNodeType.EndElement)
                            reader.Skip();
                        reader.ReadEndElement();
                    }
                    break;
                case "Planes":
                    if(reader.IsEmptyElement || (lengthStr = reader.GetAttribute("Length")) == null) {
                        reader.Skip();
                    } else {
                        reader.ReadStartElement();
                        if(int.TryParse(lengthStr, out length)) {
                            this.Planes = new MotionDataPlane[length];
                            int count = 0;
                            while(true) {
                                if(reader.NodeType == XmlNodeType.EndElement)
                                    break;
                                if(count == length)
                                    break;
                                switch(reader.Name) {
                                case "MotionDataPlane":
                                    this.Planes[count] = MotionDataPlane.Deserialize(reader);
                                    count++;
                                    break;
                                default:
                                    reader.Skip();
                                    break;
                                }
                            }
                        }
                        while(reader.NodeType != XmlNodeType.None && reader.NodeType != XmlNodeType.EndElement)
                            reader.Skip();
                        reader.ReadEndElement();
                    }
                    break;
                default:
                    reader.Skip();
                    break;
                }
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer) {
            writer.WriteStartElement("Points");
            writer.WriteStartAttribute("Length");
            writer.WriteValue(Points.Length);
            writer.WriteEndAttribute();
            writer.WriteString(writePointsToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Lines");
            writer.WriteStartAttribute("Length");
            writer.WriteValue(Lines.Length);
            writer.WriteEndAttribute();
            writer.WriteString(writeLinesToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Cylinders");
            writer.WriteStartAttribute("Length");
            writer.WriteValue(Cylinders.Length);
            writer.WriteEndAttribute();
            writer.WriteString(writeCylindersToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Spheres");
            writer.WriteStartAttribute("Length");
            writer.WriteValue(Spheres.Length);
            writer.WriteEndAttribute();
            writer.WriteString(WriteSpheresToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Planes");
            writer.WriteStartAttribute("Length");
            writer.WriteValue(Planes.Length);
            writer.WriteEndAttribute();
            foreach(var plane in Planes) {
                plane.Serialize(writer);
            }
            writer.WriteEndElement();
        }

        protected void readPointsFromString(int length, string value) {
            string[] values = value.Split('\t');
            this.Points = new MotionDataPoint[length];
            for(int i = 0; i < length; i++) {
                Vector3 tmp;
                if(VectorEx.TryParse(values, i * 3, out tmp)) {
                    this.Points[i].Exists = true;
                    this.Points[i].Position = tmp;
                }
            }
        }

        protected string writePointsToString() {
            StringBuilder builder = new StringBuilder();
            foreach(var point in Points) {
                if(point.Exists) {
                    builder.AppendFormat("{0}\t{1}\t{2}\t", point.X, point.Y, point.Z);
                } else {
                    builder.Append("\t\t\t");
                }
            }
            return builder.ToString();
        }

        protected void readLinesFromString(int length, string value) {
            string[] values = value.Split('\t');
            this.Lines = new MotionDataLine[length];
            for(int i = 0; i < length; i++) {
                Vector3 tmp1, tmp2;
                if(VectorEx.TryParse(values, i * 6, out tmp1)
                    && VectorEx.TryParse(values, i * 6 + 3, out tmp2)) {
                    this.Lines[i].Exists = true;
                    this.Lines[i].End = tmp1;
                    this.Lines[i].DirectionAndLength = tmp2;
                }
            }
        }

        protected string writeLinesToString() {
            StringBuilder builder = new StringBuilder();
            foreach(var line in Lines) {
                if(line.Exists) {
                    builder.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t", line.End.X, line.End.Y, line.End.Z, line.DirectionAndLength.X, line.DirectionAndLength.Y, line.DirectionAndLength.Z);
                } else {
                    builder.Append("\t\t\t\t\t\t");
                }
            }
            return builder.ToString();
        }

        protected void readCylindersFromString(int length, string value) {
            string[] values = value.Split('\t');
            this.Cylinders = new MotionDataCylinder[length];
            for(int i = 0; i < length; i++) {
                Vector3 tmp1, tmp2;
                float tmp3;
                if(VectorEx.TryParse(values, i * 7, out tmp1)
                    && VectorEx.TryParse(values, i * 7 + 3, out tmp2)
                    && values.Length >= i * 7 + 6
                    && float.TryParse(values[i * 7 + 6], out tmp3)) {
                    this.Cylinders[i].Exists = true;
                    this.Cylinders[i].End = tmp1;
                    this.Cylinders[i].Axis = tmp2;
                    this.Cylinders[i].Radius = tmp3;
                }
            }
        }

        protected string writeCylindersToString() {
            StringBuilder builder = new StringBuilder();
            foreach(var cylinder in Cylinders) {
                if(cylinder.Exists) {
                    builder.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t", cylinder.End.X, cylinder.End.Y, cylinder.End.Z, cylinder.Axis.X, cylinder.Axis.Y, cylinder.Axis.Z, cylinder.Radius);
                } else {
                    builder.Append("\t\t\t\t\t\t\t");
                }
            }
            return builder.ToString();
        }

        protected void ReadSpheresFromString(int length, string value) {
            string[] values = value.Split('\t');
            this.Spheres = new MotionDataSphere[length];
            for(int i = 0; i < length; i++) {
                Vector3 tmp1;
                float tmp2;
                if(VectorEx.TryParse(values, i * 4, out tmp1)
                    && values.Length >= i * 4 + 3
                    && float.TryParse(values[i * 4 + 3], out tmp2)) {
                    this.Spheres[i].Exists = true;
                    this.Spheres[i].Center = tmp1;
                    this.Spheres[i].Radius = tmp2;
                }
            }
        }

        protected string WriteSpheresToString() {
            StringBuilder builder = new StringBuilder();
            foreach(var sphere in Spheres) {
                if(sphere.Exists) {
                    builder.AppendFormat("{0}\t{1}\t{2}\t{3}\t", sphere.Center.X, sphere.Center.Y, sphere.Center.Z, sphere.Radius);
                } else {
                    builder.Append("\t\t\t\t");
                }
            }
            return builder.ToString();
        }

        #endregion

        #region BinaryIO
        /// <summary>
        /// バイナリデータからオブジェクトを読み込みます。
        /// </summary>
        /// <param title="reader">読み込み元</param>
        public void ReadBinary(BinaryReader reader) {
            Points = new MotionDataPoint[reader.ReadInt32()];
            for(int i = 0; i < Points.Length; i++) {
                Points[i].ReadBinary(reader);
            }
            Lines = new MotionDataLine[reader.ReadInt32()];
            for(int i = 0; i < Lines.Length; i++) {
                Lines[i].ReadBinary(reader);
            }
            Cylinders = new MotionDataCylinder[reader.ReadInt32()];
            for(int i = 0; i < Cylinders.Length; i++) {
                Cylinders[i].ReadBinary(reader);
            }
            Spheres = new MotionDataSphere[reader.ReadInt32()];
            for(int i = 0; i < Spheres.Length; i++) {
                Spheres[i].ReadBinary(reader);
            }
            Planes = new MotionDataPlane[reader.ReadInt32()];
            for(int i = 0; i < Planes.Length; i++) {
                Planes[i].ReadBinary(reader);
            }
        }

        /// <summary>
        /// オブジェクトをバイナリデータとして書き込みます。
        /// </summary>
        /// <param title="writer">書き込み先</param>
        public void WriteBinary(BinaryWriter writer) {
            writer.Write(Points.Length);
            foreach(var obj in Points)
                obj.WriteBinary(writer);
            writer.Write(Lines.Length);
            foreach(var obj in Lines)
                obj.WriteBinary(writer);
            writer.Write(Cylinders.Length);
            foreach(var obj in Cylinders)
                obj.WriteBinary(writer);
            writer.Write(Spheres.Length);
            foreach(var obj in Spheres)
                obj.WriteBinary(writer);
            writer.Write(Planes.Length);
            foreach(var obj in Planes)
                obj.WriteBinary(writer);
        }

        #endregion
    }

}
