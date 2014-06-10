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

using System.Xml.Schema;

namespace MotionDataHandler.Motion.Old {
    /// <summary>
    /// MotionDataObjectの種類を指定する列挙体
    /// </summary>
    [Flags]
    public enum MotionDataObjectType {
        /// <summary>
        /// 未定義のオブジェクト
        /// </summary>
        Undefined = 0x00,
        /// <summary>
        /// 単一点を示すオブジェクト
        /// </summary>
        Point = 0x01,
        /// <summary>
        /// 線分を示すオブジェクト
        /// </summary>
        Line = 0x02,
        /// <summary>
        /// 円筒を示すオブジェクト
        /// </summary>
        Cylinder = 0x04,
        /// <summary>
        /// 球を示すオブジェクト
        /// </summary>
        Sphere = 0x08,
        /// <summary>
        /// 面を示すオブジェクト
        /// </summary>
        Plane = 0x10,
        /// <summary>
        /// 任意のオブジェクト
        /// </summary>
        Any = 0x1F,
    }


    /// <summary>
    /// モーションデータオブジェクトの情報を保持するクラス
    /// </summary>
    public class MotionDataObjectInfo {
        readonly MotionDataHeader _parent = null;
        private string _name = "";
        /// <summary>
        /// オブジェクトの名前を取得または設定します。
        /// </summary>
        public string Name {
            get { if(_name == null)_name = ""; return _name; }
            set {
                if(value == null)
                    value = "unnamed";
                foreach(var c in new[] { '\\', ':', '?', '"', '|', '>', '<', '*', '$', '{', '}' }) {
                    value = value.Replace(c, '_');
                }
                List<string> names = _parent.Infos.Where(i => i != this).Select(i => i.Name).ToList();

                int count = 1;
                string tmp = value.Trim();
                while(names.Contains(tmp)) {
                    count++;
                    tmp = value + " (" + count.ToString() + ")";
                }
                _name = tmp;
            }
        }
        /// <summary>
        /// オブジェクトの色を取得または設定します。
        /// </summary>
        public Color Color = Color.White;
        /// <summary>
        /// オブジェクトを画面に表示するかどうかを取得または設定します。
        /// </summary>
        public bool Visible;
        /// <summary>
        /// オブジェくとが仮想オブジェクトかどうかを取得または設定します。
        /// </summary>
        public bool Virtual;

        private MotionDataObjectType _objectType;
        /// <summary>
        /// オブジェクトの種類を取得または設定します。
        /// </summary>
        public MotionDataObjectType ObjectType {
            get { return _objectType; }
            set {
                switch(value) {
                case MotionDataObjectType.Point:
                case MotionDataObjectType.Line:
                case MotionDataObjectType.Cylinder:
                case MotionDataObjectType.Sphere:
                case MotionDataObjectType.Plane:
                    _objectType = value;
                    break;
                default:
                    throw new ArgumentException("Type can be specified only single MotionDataObjectType");
                }
            }
        }
        private int _objectIndex;
        /// <summary>
        /// モーションデータフレーム内でのオブジェクトのインデックスを取得または設定します。
        /// </summary>
        public int ObjectIndex {
            get { return _objectIndex; }
            set {
                if(value < 0)
                    throw new ArgumentOutOfRangeException("value", "'ObjectIndex' cannot be negative");
                _objectIndex = value;
            }
        }

        private string _groupName;
        /// <summary>
        /// オブジェクトのグループ名を取得または設定します。
        /// </summary>
        public string GroupName {
            get { if(_groupName == null)_groupName = ""; return _groupName; }
            set {
                _groupName = value;
                _groupName = _groupName.Replace('>', '_');
                _groupName = _groupName.Replace('<', '_');
                _groupName = _groupName.Replace('*', '_');
                _groupName = _groupName.Replace('(', '_');
                _groupName = _groupName.Replace(')', '_');
            }
        }


        public void Serialize(XmlWriter writer) {
            writer.WriteStartElement(this.GetType().Name);
            writer.WriteElementString("Name", this.Name);
            writer.WriteElementString("GroupName", this.GroupName);
            writer.WriteElementString("ObjectType", this.ObjectType.ToString());
            writer.WriteElementString("ObjectIndex", this.ObjectIndex.ToString());
            writer.WriteStartElement("Visible");
            writer.WriteValue(this.Visible);
            writer.WriteEndElement();
            writer.WriteStartElement("Virtual");
            writer.WriteValue(this.Virtual);
            writer.WriteEndElement();
            writer.WriteElementString("Color", ColorTranslator.ToHtml(this.Color));
            writer.WriteEndElement();
        }
        public static MotionDataObjectInfo Deserialize(XmlReader reader, MotionDataHeader parent) {
            if(parent == null)
                throw new ArgumentNullException("parent", "'parent' cannot be null");
            reader.MoveToContent();
            if(reader.IsEmptyElement) {
                return null;
            } else {
                string name = "", groupName = "";
                Color color = Color.White;
                MotionDataObjectType type = MotionDataObjectType.Undefined;
                int index = -1;
                bool visible = true;
                bool isVirtual = false;
                reader.ReadStartElement(typeof(MotionDataObjectInfo).Name);
                for(reader.MoveToContent(); reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None; reader.MoveToContent()) {
                    if(reader.IsEmptyElement) {
                        reader.Skip();
                        continue;
                    }
                    switch(reader.Name) {
                    case "Name":
                        name = reader.ReadElementContentAsString();
                        break;
                    case "GroupName":
                        groupName = reader.ReadElementContentAsString();
                        break;
                    case "ObjectType":
                        type = (MotionDataObjectType)Enum.Parse(typeof(MotionDataObjectType), reader.ReadElementContentAsString());
                        break;
                    case "ObjectIndex":
                        index = reader.ReadElementContentAsInt();
                        break;
                    case "Show":
                    case "Visible":
                        visible = reader.ReadElementContentAsBoolean();
                        break;
                    case "Virtual":
                        isVirtual = reader.ReadElementContentAsBoolean();
                        break;
                    case "Color":
                        var cstr = reader.ReadElementContentAsString();
                        int cint;
                        if(int.TryParse(cstr, out cint)) {
                            color = Color.FromArgb(cint);
                        } else {
                            color = ColorTranslator.FromHtml(cstr);
                        }
                        break;
                    default:
                        reader.Skip();
                        break;
                    }
                }
                reader.ReadEndElement();
                if(type == MotionDataObjectType.Undefined || index == -1)
                    return null;
                var ret = new MotionDataObjectInfo(parent, type, index);
                ret.Name = name;
                ret.GroupName = groupName;
                ret.Color = color;
                ret.Visible = visible;
                ret.Virtual = isVirtual;
                return ret;
            }
        }
        public MotionDataObjectInfo(MotionDataHeader parent, MotionDataObjectType type, int index)
            : this(parent) {
            this.ObjectType = type;
            this.ObjectIndex = index;
        }
        public MotionDataObjectInfo(MotionDataHeader parent) {
            if(parent == null)
                throw new ArgumentNullException("parent", "'parent' cannot be null");
            _parent = parent;
            _objectType = MotionDataObjectType.Undefined;
        }
    }


    public class MotionDataHeader {

        public void Clear() {
            _infos = null;
        }

        private MotionDataObjectInfo[] _infos;

        public MotionDataObjectInfo[] Infos {
            get {
                if(_infos == null)
                    _infos = new MotionDataObjectInfo[0];
                return _infos;
            }
            set {
                _infos = value;
                try {
                    checkUnique();
                } catch(ArgumentException ) {
                    throw ;
                }
            }
        }

        private MotionDataSet _parent;
        [XmlIgnore]
        public MotionDataSet Parent {
            get { return _parent; }
            set {
                if(_parent != null && _parent.Header == this && _parent != value) {
                    MotionDataSet tmp = _parent;
                    _parent = value;
                    tmp.Header = null;
                } else {
                    _parent = value;
                }
                if(_parent != null && _parent.Header != this) {
                    _parent.Header = this;
                }
            }
        }

        public MotionFieldState FieldState;

        private void checkUnique() {
            for(int i = 0; i < Infos.Length; i++) {
                for(int j = i + 1; j < Infos.Length; j++) {
                    if(Infos[i].ObjectType == Infos[j].ObjectType && Infos[i].ObjectIndex == Infos[j].ObjectIndex)
                        throw new ArgumentException("Violate unique object index: Infos");
                }
            }
        }
        public void Add(MotionDataObjectInfo info) {
            if(info.ObjectType == MotionDataObjectType.Undefined)
                throw new ArgumentException("Cannot add Undefined type object info");
            List<MotionDataObjectInfo> newInfos = new List<MotionDataObjectInfo>(Infos);

            newInfos.Add(info);
            Infos = newInfos.ToArray();
        }

        public void Serialize(XmlWriter writer) {
            writer.WriteStartElement(typeof(MotionDataHeader).Name);
            XmlSerializer serFS = new XmlSerializer(typeof(MotionFieldState));
            serFS.Serialize(writer, this.FieldState);
            writer.WriteStartElement("Infos");
            foreach(var info in _infos) {
                info.Serialize(writer);
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        public static MotionDataHeader Deserialize(XmlReader reader) {
            reader.MoveToContent();
            reader.ReadStartElement(typeof(MotionDataHeader).Name);
            MotionDataHeader ret = new MotionDataHeader();
            List<MotionDataObjectInfo> infos = new List<MotionDataObjectInfo>();
            for(reader.MoveToContent(); reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None; reader.MoveToContent()) {
                if(reader.IsEmptyElement) {
                    reader.Skip();
                    continue;
                }
                switch(reader.Name) {
                case "MotionFieldState":
                    XmlSerializer ser = new XmlSerializer(typeof(MotionFieldState));
                    ret.FieldState = (MotionFieldState)ser.Deserialize(reader);
                    break;
                case "Infos":
                    reader.ReadStartElement("Infos");
                    for(reader.MoveToContent(); reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None; reader.MoveToContent()) {
                        if(reader.IsEmptyElement) {
                            reader.Skip();
                            continue;
                        }
                        switch(reader.Name) {
                        case "MotionDataObjectInfo":
                            infos.Add(MotionDataObjectInfo.Deserialize(reader, ret));
                            break;
                        default:
                            reader.Skip();
                            break;
                        }
                    }
                    reader.ReadEndElement();
                    break;
                default:
                    reader.Skip();
                    break;
                }
            }
            reader.ReadEndElement();
            ret._infos = infos.ToArray();
            return ret;
        }

    }
}
