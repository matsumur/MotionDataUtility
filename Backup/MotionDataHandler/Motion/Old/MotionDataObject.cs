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

    public interface IMotionDataObject {
        bool Exists { get; set; }
    }

    /// <summary>
    /// Point型のモーションデータを保持する構造体
    /// </summary>
    public struct MotionDataPoint : IMotionDataObject {
        bool _exists;
        /// <summary>
        /// 有効なデータが存在するかと取得または設定します。
        /// </summary>
        public bool Exists { get { return _exists; } set { _exists = value; } }

        /// <summary>
        /// Pointの空間座標取得または設定します。
        /// </summary>
        public Vector3 Position;
        /// <summary>
        /// MotionDataPointを作成します。
        /// </summary>
        /// <param title="position">Pointの空間座標</param>
        public MotionDataPoint(Vector3 position) {
            _exists = true;
            Position = position;
        }
        /// <summary>
        /// MotionDataPointを作成します。
        /// </summary>
        /// <param title="x">PointのX座標の値</param>
        /// <param title="y">PointのY座標の値</param>
        /// <param title="z">PointのZ座標の値</param>
        public MotionDataPoint(float x, float y, float z) : this(new Vector3(x, y, z)) { }
        /// <summary>
        /// PointのX座標の値を取得または設定します。
        /// </summary>
        [XmlIgnore]
        public float X { get { return Position.X; } set { Position.X = value; } }
        /// <summary>
        /// PointのY座標の値を取得または設定します。
        /// </summary>
        [XmlIgnore]
        public float Y { get { return Position.Y; } set { Position = new Vector3(X, value, Z); } }
        /// <summary>
        /// PointのZ座標の値を取得または設定します。
        /// </summary>
        [XmlIgnore]
        public float Z { get { return Position.Z; } set { Position = new Vector3(X, Y, value); } }

        #region BinaryIO
        /// <summary>
        /// バイナリデータからオブジェクトを読み込みます。
        /// </summary>
        /// <param title="reader">読み込み元</param>
        public void ReadBinary(BinaryReader reader) {
            Exists = reader.ReadBoolean();
            if(Exists) {
                Position = VectorEx.ReadVector3(reader);
            }
        }

        /// <summary>
        /// オブジェクトをバイナリデータとして書き込みます。
        /// </summary>
        /// <param title="writer">書き込み先</param>
        public void WriteBinary(BinaryWriter writer) {
            writer.Write(Exists);
            if(Exists) {
                VectorEx.WriteVector3(writer, Position);
            }
        }
        #endregion
    }
    /// <summary>
    /// Line型のモーションデータを保持する構造体
    /// </summary>

    public struct MotionDataLine : IMotionDataObject {
        bool _exists;
        /// <summary>
        /// 有効なデータが存在するかを取得または設定します。
        /// </summary>
        public bool Exists { get { return _exists; } set { _exists = value; } }
        /// <summary>
        /// 線分の開始点を取得または設定します。
        /// </summary>
        public Vector3 End;
        /// <summary>
        /// 線分の方向と長さを取得または設定します。
        /// </summary>
        public Vector3 DirectionAndLength;
        /// <summary>
        /// 線分の方向を返します。
        /// </summary>
        /// <returns>正規化されたベクトル</returns>
        public Vector3 Direction() { return Vector3.Normalize(DirectionAndLength); }
        /// <summary>
        /// 線分の長さを返します。
        /// </summary>
        /// <returns>線分長</returns>
        public float Length() { return DirectionAndLength.Length(); }
        /// <summary>
        /// MotionDataLineを作成します。
        /// </summary>
        /// <param title="end">線分の開始点</param>
        /// <param title="directionAndLength">線分の方向と長さ</param>
        public MotionDataLine(Vector3 end, Vector3 directionAndLength) {
            _exists = true;
            End = end;
            DirectionAndLength = directionAndLength;
        }
        /// <summary>
        /// 線分の終了点を取得または設定します。
        /// </summary>
        [XmlIgnore]
        public Vector3 AnotherEnd {
            get { return End + DirectionAndLength; }
            set { DirectionAndLength = value - End; }
        }

        #region BinaryIO
        /// <summary>
        /// バイナリデータからオブジェクトを読み込みます。
        /// </summary>
        /// <param title="reader">読み込み元</param>
        public void ReadBinary(BinaryReader reader) {
            Exists = reader.ReadBoolean();
            if(Exists) {
                End = VectorEx.ReadVector3(reader);
                DirectionAndLength = VectorEx.ReadVector3(reader);
            }
        }

        /// <summary>
        /// オブジェクトをバイナリデータとして書き込みます。
        /// </summary>
        /// <param title="writer">書き込み先</param>
        public void WriteBinary(BinaryWriter writer) {
            writer.Write(Exists);
            if(Exists) {
                VectorEx.WriteVector3(writer, End);
                VectorEx.WriteVector3(writer, DirectionAndLength);
            }
        }
        #endregion
    }
    /// <summary>
    /// Cylinder型のモーションデータを保持する構造体
    /// </summary>

    public struct MotionDataCylinder : IMotionDataObject {
        bool _exists;
        /// <summary>
        /// 有効なデータが存在するかと取得または設定します。
        /// </summary>
        public bool Exists { get { return _exists; } set { _exists = value; } }
        /// <summary>
        /// 円柱の底面の中心点を取得または設定します。
        /// </summary>
        public Vector3 End;
        /// <summary>
        /// 円柱の中心軸を取得または設定します。
        /// </summary>
        public Vector3 Axis;
        /// <summary>
        /// 中心軸の終了点を取得または設定します。
        /// </summary>
        [XmlIgnore]
        public Vector3 AnotherEnd {
            get { return End + Axis; }
            set { Axis = value - End; }
        }
        float _radius;
        /// <summary>
        /// 円柱の半径を取得または設定します。
        /// </summary>
        public float Radius {
            get { return _radius; }
            set {
                if(value < 0)
                    value = -value;
                _radius = value;
            }
        }
        /// <summary>
        /// MotionDataCylinderを作成します。
        /// </summary>
        /// <param title="end">円柱の底面の中心点</param>
        /// <param title="axis">円柱の軸</param>
        /// <param title="radius">円柱の半径</param>
        public MotionDataCylinder(Vector3 end, Vector3 axis, float radius) {
            _exists = true;
            End = end;
            Axis = axis;
            _radius = 0;
            Radius = radius;
        }

        #region BinaryIO
        /// <summary>
        /// バイナリデータからオブジェクトを読み込みます。
        /// </summary>
        /// <param title="reader">読み込み元</param>
        public void ReadBinary(BinaryReader reader) {
            Exists = reader.ReadBoolean();
            if(Exists) {
                End = VectorEx.ReadVector3(reader);
                Axis = VectorEx.ReadVector3(reader);
                Radius = reader.ReadSingle();
            }
        }

        /// <summary>
        /// オブジェクトをバイナリデータとして書き込みます。
        /// </summary>
        /// <param title="writer">書き込み先</param>
        public void WriteBinary(BinaryWriter writer) {
            writer.Write(Exists);
            if(Exists) {
                VectorEx.WriteVector3(writer, End);
                VectorEx.WriteVector3(writer, Axis);
                writer.Write(Radius);
            }
        }
        #endregion
    }
    /// <summary>
    /// Sphere型のモーションデータを保持する構造体
    /// </summary>

    public struct MotionDataSphere : IMotionDataObject {
        bool _exists;
        /// <summary>
        /// 有効なデータが存在するかと取得または設定します。
        /// </summary>
        public bool Exists { get { return _exists; } set { _exists = value; } }
        /// <summary>
        /// 球の中心点を取得または設定します。
        /// </summary>
        public Vector3 Center;
        float _radius;
        /// <summary>
        /// 球の半径を取得または設定します。
        /// </summary>
        public float Radius {
            get { return _radius; }
            set {
                if(value < 0)
                    value = -value;
                _radius = value;
            }
        }
        /// <summary>
        /// MotionDataSphereを作成します。
        /// </summary>
        /// <param title="center">球の中心点</param>
        /// <param title="radius">球の半径</param>
        public MotionDataSphere(Vector3 center, float radius) {
            _exists = true;
            Center = center;
            _radius = 0;
            Radius = radius;
        }


        #region BinaryIO
        /// <summary>
        /// バイナリデータからオブジェクトを読み込みます。
        /// </summary>
        /// <param title="reader">読み込み元</param>
        public void ReadBinary(BinaryReader reader) {
            Exists = reader.ReadBoolean();
            if(Exists) {
                Center = VectorEx.ReadVector3(reader);
                Radius = reader.ReadSingle();
            }
        }

        /// <summary>
        /// オブジェクトをバイナリデータとして書き込みます。
        /// </summary>
        /// <param title="writer">書き込み先</param>
        public void WriteBinary(BinaryWriter writer) {
            writer.Write(Exists);
            if(Exists) {
                VectorEx.WriteVector3(writer, Center);
                writer.Write(Radius);
            }
        }
        #endregion
    }
    /// <summary>
    /// Plane型のモーションデータを保持する構造体
    /// </summary>
    public struct MotionDataPlane : IMotionDataObject, IXmlSerializable {
        bool _exists;
        /// <summary>
        /// 有効なデータが存在するかと取得または設定します。
        /// </summary>
        public bool Exists { get { return _exists; } set { _exists = value; } }
        /// <summary>
        /// 平面を構成する点群の空間座標を取得または設定します。
        /// </summary>
        public Vector3[] Points;
        /// <summary>
        /// MotionDataPlaneを作成します。
        /// </summary>
        /// <param title="points"></param>
        public MotionDataPlane(Vector3[] points) {
            _exists = true;
            if(points == null)
                throw new ArgumentNullException("points", "'points' cannot benull");
            if(points.Length < 3)
                throw new ArgumentException("'points' must be have more than or equal to 3 points", "points");
            Points = points;
        }


        #region IXmlSerializable

        public System.Xml.Schema.XmlSchema GetSchema() {
            throw new System.NotImplementedException();
        }

        public void ReadXml(XmlReader reader) {
            if(reader.IsEmptyElement) {
                reader.Skip();
                return;
            }
            reader.ReadStartElement("MotionDataPlane");
            this.Exists = false;
            while(reader.NodeType != XmlNodeType.None) {
                if(reader.NodeType == XmlNodeType.EndElement)
                    break;
                switch(reader.Name) {
                case "Points":
                    string lengthStr;
                    if(reader.IsEmptyElement || (lengthStr = reader.GetAttribute("Length")) == null) {
                        reader.Skip();
                    } else {
                        reader.ReadStartElement("Points");
                        int length;
                        if(int.TryParse(lengthStr, out length) && length >= 3) {
                            this.Exists = true;
                            this.Points = new Vector3[length];
                            string[] values = reader.ReadString().Split('\t');
                            for(int i = 0; i < length; i++) {
                                Vector3 tmp;
                                if(VectorEx.TryParse(values, i * 3, out tmp)) {
                                    this.Points[i] = tmp;
                                } else {
                                    this.Exists = false;
                                    break;
                                }
                            }
                        }
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
            if(this.Exists && this.Points != null) {
                writer.WriteStartElement("Points");
                writer.WriteStartAttribute("Length");
                writer.WriteValue(Points.Length);
                writer.WriteEndAttribute();
                foreach(var point in this.Points) {
                    writer.WriteString(string.Format("{0}\t{1}\t{2}\t", point.X, point.Y, point.Z));
                }
                writer.WriteEndElement();
            }
        }


        public void Serialize(XmlWriter writer) {
            XmlSerializer sel = new XmlSerializer(typeof(MotionDataPlane));
            sel.Serialize(writer, this);
        }

        public static MotionDataPlane Deserialize(XmlReader reader) {
            XmlSerializer sel = new XmlSerializer(typeof(MotionDataPlane));
            return (MotionDataPlane)sel.Deserialize(reader);
        }

        #endregion

        #region BinaryIO
        /// <summary>
        /// バイナリデータからオブジェクトを読み込みます。
        /// </summary>
        /// <param title="reader">読み込み元</param>
        public void ReadBinary(BinaryReader reader) {
            Exists = reader.ReadBoolean();
            if(Exists) {
                Points = new Vector3[reader.ReadInt32()];
                for(int i = 0; i < Points.Length; i++) {
                    Points[i] = VectorEx.ReadVector3(reader);
                }
            }
        }

        /// <summary>
        /// オブジェクトをバイナリデータとして書き込みます。
        /// </summary>
        /// <param title="writer">書き込み先</param>
        public void WriteBinary(BinaryWriter writer) {
            writer.Write(Exists);
            if(Exists) {
                writer.Write(Points.Length);
                for(int i = 0; i < Points.Length; i++) {
                    VectorEx.WriteVector3(writer, Points[i]);
                }
            }
        }
        #endregion
    }

}
