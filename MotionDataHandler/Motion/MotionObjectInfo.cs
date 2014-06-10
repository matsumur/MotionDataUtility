using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.DirectX;
using System.Drawing;
using System.Threading;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace MotionDataHandler.Motion {
    using Misc;

    public class MotionObjectInfo : MotionDataHandler.Motion.IReadableMotionObjectInfo {
        /// <summary>
        /// XmlSerializer用コンストラクタ
        /// </summary>
        public MotionObjectInfo() { }

        public MotionObjectInfo(Type objectType) {
            this.ObjectType = objectType;
        }

        public MotionObjectInfo(Type objectType, MotionObjectInfo original) {
            _parent = original._parent;
            this.ObjectType = objectType;
            this.Id = original._parent.GetNextId();
            this.Name = original.Name;
            this.Color = original.Color;
        }

        private MotionDataSet _parent = null;
        /// <summary>
        /// オブジェクト情報の属するデータセットを取得または設定します．設定は一度しかできません．
        /// </summary>
        [XmlIgnore]
        public MotionDataSet Parent {
            get { return _parent; }
            set {
                if(_parent != null)
                    throw new InvalidOperationException("cannot change 'Parent' value");
                _parent = value;
            }
        }

        private Type _objectType = null;
        /// <summary>
        /// モーションオブジェクトのタイプを取得または設定します．設定は一度しかできません．
        /// </summary>
        [XmlIgnore]
        public Type ObjectType {
            get { return _objectType; }
            set {
                if(_objectType != null)
                    throw new InvalidOperationException("cannot change 'ObjectType' value");
                IsMotionObjectTypeOrThrow(value);
                _objectType = value;
            }
        }

        public bool IsTypeOf(Type type) {
            return this.ObjectType == type || this.ObjectType.IsSubclassOf(type);
        }

        public bool IsTypeOf<T>() where T : MotionObject {
            return this.IsTypeOf(typeof(T));
        }
        [XmlIgnore]
        public bool IsSelected {
            get {
                if(_parent == null)
                    return false;
                return _parent.IsSelecting(this);
            }
            set {
                if(_parent == null)
                    throw new InvalidOperationException("Parent is not set");
                _parent.SelectObjects(true, this);
            }
        }
        /// <summary>
        /// オブジェクトのタイプ名を取得します．または，タイプ名に基づいてタイプを設定します．
        /// </summary>
        public string ObjectTypeFullName {
            get { return this.ObjectType.FullName; }
            set { this.ObjectType = Type.GetType(value); }
        }

        private MotionObject _emptyObject = null;
        /// <summary>
        /// 現在のモーションオブジェクトのタイプから，オブジェクトのインスタンスを作成して返します．
        /// </summary>
        public MotionObject GetEmptyObject() {
            if(ObjectType == null)
                throw new InvalidOperationException("cannot create MotionObject of null Type");
            if(_emptyObject == null) {
                lock(this) {
                    _emptyObject = (MotionObject)Activator.CreateInstance(ObjectType);
                    return (MotionObject)_emptyObject.Clone();
                }
            }
            return (MotionObject)_emptyObject.Clone();
        }

        /// <summary>
        /// 指定された方がモーションオブジェクトのタイプであるかを検証し，検証に失敗すれば例外をスローします．
        /// </summary>
        /// <param name="motionObjectType"></param>
        public static void IsMotionObjectTypeOrThrow(Type motionObjectType) {
            if(motionObjectType == null)
                throw new ArgumentNullException("motionObjectType", "'motionObjectType' cannot be null");
            if(!motionObjectType.IsSubclassOf(typeof(MotionObject)))
                throw new ArgumentException(string.Format("{0} is not subclass of MotionObject", motionObjectType));
            if(motionObjectType.IsAbstract)
                throw new ArgumentException(string.Format("{0} is Abstract Type", motionObjectType));

            if(motionObjectType.GetConstructor(System.Type.EmptyTypes) == null)
                throw new ArgumentException(string.Format("{0} does not have constructor with no parameter", motionObjectType));
            MotionObject @object = (MotionObject)Activator.CreateInstance(motionObjectType);
            Object clone = @object.Clone();
            if(clone.GetType() != motionObjectType)
                throw new ArgumentException(string.Format("{0}.Clone() returns not {0} but {1}", motionObjectType, clone.GetType()));
            MemoryStream stream = new MemoryStream();
            try {
                using(BinaryWriter writer = new BinaryWriter(stream)) {
                    @object.WriteBinary(writer);
                }
            } catch(Exception ex) {
                throw new ArgumentException(string.Format("{0} failed to WriteBinary with default data", motionObjectType), ex);
            }
            stream = new MemoryStream(stream.ToArray());
            try {
                using(BinaryReader reader = new BinaryReader(stream)) {
                    ((MotionObject)clone).ReadBinary(reader);
                }
            } catch(Exception ex) {
                throw new ArgumentException(string.Format("{0} failed to WriteBinary and ReadBinary with default data", motionObjectType), ex);
            }
        }

        private uint? _id = null;
        /// <summary>
        /// モーションオブジェクトのIDを取得します．データセットへ追加時に自動設定されます．
        /// </summary>
        ///<exception cref="System.InvalidOperationException"></exception>
        public uint Id {
            get {
                if(!_id.HasValue)
                    throw new InvalidOperationException("Id has not been set");
                return _id.Value;
            }
            set {
                if(_id.HasValue)
                    throw new InvalidOperationException("Id cannot be changed");
                _id = value;
            }
        }

        /// <summary>
        /// モーションオブジェクトのIDが設定されているかを取得します．
        /// </summary>
        public bool IsIdSet { get { return _id.HasValue; } }

        private string _name = "";
        /// <summary>
        /// モーションオブジェクトの名前を取得または設定します．
        /// </summary>
        public string Name {
            get { return _name; }
            set {
                value = PathEx.NormalizePath(value);
                if(_parent != null) {
                    value = _parent.GetUniqueName(value, this);
                }
                _name = value;
            }
        }

        private Color _color = Color.White;
        /// <summary>
        /// オブジェクトの色を取得または設定します．
        /// </summary>
        [XmlIgnore]
        public Color Color { get { return _color; } set { _color = value; } }

        public int ColorValue { get { return this.Color.ToArgb(); } set { this.Color = Color.FromArgb(value); } }

        private bool _visible = true;
        public bool IsVisible { get { return _visible; } set { _visible = value; } }

    }

    public class MotionObjectInfoList : System.Collections.ObjectModel.KeyedCollection<uint, MotionObjectInfo> {
        public MotionObjectInfoList() { }
        public MotionObjectInfoList(IEnumerable<MotionObjectInfo> infoList)
            : this() {
            foreach(MotionObjectInfo info in infoList) {
                this.Add(info);
            }
        }
        protected override uint GetKeyForItem(MotionObjectInfo item) {
            return item.Id;
        }
        public bool TryGetObjectInfo(uint id, out MotionObjectInfo objectInfo) {
            var dictionary = this.Dictionary;
            if(dictionary != null) {
                return dictionary.TryGetValue(id, out objectInfo);
            } else {
                foreach(var info in this.Items) {
                    if(info.Id == id) {
                        objectInfo = info;
                        return true;
                    }
                }
                objectInfo = null;
                return false;
            }
        }
    }
}
