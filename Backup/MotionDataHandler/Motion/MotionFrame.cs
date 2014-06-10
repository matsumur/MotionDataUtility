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

namespace MotionDataHandler.Motion {
    /// <summary>
    /// モーションデータのうち一フレーム内のオブジェクトの状態を保持するクラス
    /// </summary>
    public class MotionFrame {
        /// <summary>
        /// モーションデータ全体を取得します
        /// </summary>
        public readonly MotionDataSet Parent;
        /// <summary>
        /// このフレームのデータ内の時刻を取得します
        /// </summary>
        public readonly decimal Time;
        readonly SortedList<uint, MotionObject> _objectList = new SortedList<uint, MotionObject>();
        /// <summary>
        /// 内部データの余分な領域を取り除きます．
        /// </summary>
        public void TrimExcess() {
            if(_objectList.Count * 10 < _objectList.Capacity * 9) {
                _objectList.Capacity = _objectList.Count;
            }
        }
        /// <summary>
        /// データセットから削除されたオブジェクトが残っている場合に取り除きます
        /// </summary>
        public void CleanupRemoveObjects() {
            List<uint> removedIds = new List<uint>();
            foreach(var pair in _objectList) {
                if(this.Parent.GetObjectInfoById(pair.Key) == null) {
                    removedIds.Add(pair.Key);
                }
            }
            foreach(uint id in removedIds) {
                this.RemoveObject(id);
            }
            this.TrimExcess();
        }
        /// <summary>
        /// 内部コンストラクタ
        /// </summary>
        private MotionFrame() {
        }
        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        /// <param name="parent">親となるデータセット</param>
        /// <param name="time">フレーム時刻</param>
        public MotionFrame(MotionDataSet parent, decimal time)
            : this() {
            this.Parent = parent;
            this.Time = time;
        }
        /// <summary>
        /// 初期容量を指定するコンストラクタ
        /// </summary>
        /// <param name="parent">親となるデータセット</param>
        /// <param name="time">フレーム時刻</param>
        /// <param name="initialCapacity">初期容量</param>
        public MotionFrame(MotionDataSet parent, decimal time, int initialCapacity)
            : this(parent, time) {
            _objectList.Capacity = initialCapacity;
        }
        /// <summary>
        /// フレームからオブジェクトを取り除きます
        /// </summary>
        /// <param name="id">取り除くオブジェクトのID</param>
        /// <returns></returns>
        public bool RemoveObject(uint id) {
            lock(this) {
                return _objectList.Remove(id);
            }
        }
        /// <summary>
        /// フレームからオブジェクトを取り除きます
        /// </summary>
        /// <param name="info">取り除くオブジェクトの情報</param>
        /// <returns></returns>
        public bool RemoveObject(MotionObjectInfo info) {
            return this.RemoveObject(info.Id);
        }
        /// <summary>
        /// フレームにオブジェクトを追加します
        /// </summary>
        /// <param name="id">追加するオブジェクトのID</param>
        /// <param name="object">オブジェクトの状態</param>
        public void SetObject(uint id, MotionObject @object) {
            lock(this) {
                if(@object == null) {
                    this.RemoveObject(id);
                } else {
                    MotionObjectInfo info = this.Parent.GetObjectInfoById(id);
                    if(!@object.IsTypeOf(info.ObjectType))
                        throw new ArgumentException(string.Format("cannot assign '{0}' to frame as '{1}'", @object.GetType(), info.ObjectType));
                    _objectList[id] = @object;
                }
            }
        }

        /// <summary>
        /// フレームにオブジェクトを追加します
        /// </summary>
        /// <param name="info">追加するオブジェクトの情報</param>
        /// <param name="object">オブジェクトの状態</param>
        public void SetObject(IReadableMotionObjectInfo info, MotionObject @object) {
            lock(this) {
                //if(info.Parent != this.Parent)
                //    throw new ArgumentException("Parent mismatch");
                if(@object == null) {
                    RemoveObject(info.Id);
                } else {
                    if(!@object.IsTypeOf(info.ObjectType))
                        throw new ArgumentException(string.Format("cannot assign '{0}' to frame as '{1}'", @object.GetType(), info.ObjectType));
                    _objectList[info.Id] = @object;
                }
            }
        }
        /// <summary>
        /// フレーム内のオブジェクトを取得します．
        /// </summary>
        /// <param name="id">取得するオブジェクトのID</param>
        /// <returns></returns>
        public MotionObject GetObject(uint id) {
            MotionObject ret;
            if(_objectList.TryGetValue(id, out ret))
                return ret;
            return null;
        }
        /// <summary>
        /// フレーム内のオブジェクトを取得します
        /// </summary>
        /// <param name="info">取得するオブジェクトの情報</param>
        /// <returns></returns>
        public MotionObject GetObject(IReadableMotionObjectInfo info) {
            if(info == null)
                throw new ArgumentNullException("info", "'info' cannot be null");
            //if(info.Parent != this.Parent)
            //    throw new ArgumentException("Parent mismatch");
            return GetObject(info.Id);
        }
        /// <summary>
        /// フレームからすべてのオブジェクトを取り除きます
        /// </summary>
        public void Clear() {
            _objectList.Clear();
        }
        /// <summary>
        /// IDを用いてオブジェクトを取得または設定します．
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MotionObject this[uint id] {
            get { return this.GetObject(id); }
            set { this.SetObject(id, value); }
        }
        /// <summary>
        /// オブジェクト情報を用いてオブジェクトを取得または設定します．
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public MotionObject this[IReadableMotionObjectInfo info] {
            get { return this.GetObject(info); }
            set { this.SetObject(info, value); }
        }

        /// <summary>
        /// フレームデータをバイナリ形式で出力します
        /// </summary>
        /// <param name="writer"></param>
        public void SerializeBinary(BinaryWriter writer) {
            writer.Write(this.Time);
            lock(this) {
                List<MotionObjectInfo> existInfo;
                lock(this.Parent) {
                    existInfo = (from info in this.Parent.GetObjectInfoList() where this[info] != null select info).ToList();
                }
                writer.Write(existInfo.Count);
                foreach(var info in existInfo) {
                    System.Diagnostics.Debug.Assert(this[info] != null);
                    writer.Write(info.Id);
                    this[info].WriteBinary(writer);
                }
            }
        }
        /// <summary>
        /// フレームデータをバイナリ形式から取得します
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static MotionFrame DeserializeBinary(BinaryReader reader, MotionDataSet parent) {
            decimal time = reader.ReadDecimal();
            int count = reader.ReadInt32();
            MotionFrame ret = new MotionFrame(parent, time, count);
            for(int i = 0; i < count; i++) {
                uint id = reader.ReadUInt32();
                MotionObjectInfo info = parent.GetObjectInfoById(id);
                if(info == null)
                    throw new InvalidDataException(string.Format("unexpected id:{0} in frame at:{1}", id, time));
                MotionObject @object = info.GetEmptyObject();
                @object.ReadBinary(reader);

                ret[info] = @object;
            }
            return ret;
        }
    }

    /// <summary>
    /// 読み取り専用のフレームデータを保持するクラス
    /// </summary>
    public class ReadOnlyMotionFrame {
        private MotionFrame _frame;
        /// <summary>
        /// このフレームのデータ内の時刻を取得します
        /// </summary>
        public decimal Time { get { return _frame.Time; } }
        /// <summary>
        /// IDを指定してオブジェクトを取得します．
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MotionObject this[uint id] {
            get {
                MotionObject tmp = _frame.GetObject(id);
                return tmp == null ? null : (MotionObject)tmp.Clone();
            }
        }
        /// <summary>
        /// オブジェクト情報を指定してオブジェクトを取得します
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public MotionObject this[IReadableMotionObjectInfo info] {
            get {
                MotionObject tmp = _frame.GetObject(info);
                return tmp == null ? null : (MotionObject)tmp.Clone();
            }
        }
        /// <summary>
        /// IDを指定してオブジェクトを取得します．
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MotionObject GetObject(uint id) {
            MotionObject tmp = _frame.GetObject(id);
            return tmp == null ? null : (MotionObject)tmp.Clone();
        }
        /// <summary>
        /// オブジェクト情報を指定してオブジェクトを取得します
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public MotionObject GetObject(IReadableMotionObjectInfo info) {
            MotionObject tmp = _frame.GetObject(info);
            return tmp == null ? null : (MotionObject)tmp.Clone();
        }
        public ReadOnlyMotionFrame(MotionFrame frame) {
            if(frame == null)
                throw new ArgumentNullException("frame", "'frame' cannot be null");
            _frame = frame;
        }
    }
}
