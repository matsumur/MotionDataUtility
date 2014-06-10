using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace MotionDataHandler.Motion {
    using Misc;
    using Operation;
    public class MotionDataSet : ITimeInterval {

        private static MotionDataSet _singleton = new MotionDataSet();
        public static MotionDataSet Singleton { get { return _singleton; } }
        /// <summary>
        /// フレームの列挙中にフレームが追加または削除されるのを防ぐためのロック用オブジェクトを取得します．
        /// </summary>
        private readonly ReaderWriterLockSlim _rwLockFrame = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly ReaderWriterLockSlim _rwLockInfo = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        /// <summary>
        /// idまたはインデックスでアクセスできるMotionObjectInfoのリスト
        /// </summary>
        readonly MotionObjectInfoList _infoList = new MotionObjectInfoList();

        /// <summary>
        /// 時間でソートされたMotionFrameのリスト
        /// </summary>
        readonly SortedList<decimal, MotionFrame> _frameList = new SortedList<decimal, MotionFrame>();
        /// <summary>
        /// 新しいオブジェクトに付与される一意のIdを保持する
        /// </summary>
        private uint _nextId;
        /// <summary>
        /// フレームの検索時の前回のインデックス番号
        /// </summary>
        private int _prevIndex;

        /// <summary>
        /// 時間がかかる処理の経過具合
        /// </summary>
        private int _progressValue;
        /// <summary>
        /// 時間がかかる処理の予定処理数
        /// </summary>
        private int _progressMax;
        /// <summary>
        /// 時間がかかる処理時の処理の経過割合を取得します．
        /// </summary>
        public int ProgressPercentage { get { return _progressMax > 0 ? 100 * _progressValue / _progressMax : 0; } }
        /// <summary>
        /// 時間がかかる処理のメッセージ
        /// </summary>
        private string _progressMessage;
        /// <summary>
        /// 時間がかかる処理時のメッセージを取得します
        /// </summary>
        public string ProgressMessage { get { return _progressMessage; } }
        /// <summary>
        /// 時間がかかる処理時の経過情報を取得します
        /// </summary>
        public ProgressChangedEventArgs ProgressChangedEventArgs { get { return new ProgressChangedEventArgs(this.ProgressPercentage, this.ProgressMessage); } }

        public MotionFieldState FieldState;

        private readonly HashSet<uint> _selectionSet = new HashSet<uint>();

        private MotionDataSet() {
            this.LoadMotionOperations();
            this.FrameListChanged += new EventHandler(MotionDataSet_FrameListChanged);
        }

        void MotionDataSet_FrameListChanged(object sender, EventArgs e) {
            EventHandler tmp = this.TimeIntervalChanged;
            if(tmp != null) {
                tmp.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// MotionObjectInfoのための重複しないIdを取得します．
        /// </summary>
        /// <returns></returns>
        public virtual uint GetNextId() {
            _rwLockInfo.EnterWriteLock();
            try {
                List<uint> idList = (from info in _infoList select info.Id).ToList();
                if(idList.Count != 0) {
                    uint maxId = idList.Max();
                    _nextId = Math.Max(_nextId, maxId + 1);
                }
                return _nextId++;
            } finally { _rwLockInfo.ExitWriteLock(); }
        }
        /// <summary>
        /// MotionObjectInfoのための重複しない名前を取得します
        /// </summary>
        /// <param name="name">名前</param>
        /// <param name="target">名前を決める対象のMotionObjectInfo．このオブジェクトの名前は重複チェックの比較対象から除かれます．</param>
        /// <returns></returns>
        public virtual string GetUniqueName(string name, MotionObjectInfo target) {
            _rwLockInfo.EnterReadLock();
            try {
                string tmp = PathEx.NormalizePath(name);
                for(int count = 1; _infoList.Where(info => info != target).Select(info => info.Name).Contains(tmp); count++) {
                    tmp = string.Format("{0} ({1})", name, count);
                }
                return tmp;
            } finally { _rwLockInfo.ExitReadLock(); }
        }
        /// <summary>
        /// MotionObjectInfoのための重複しない名前を取得します
        /// </summary>
        /// <param name="name">名前</param>
        /// <returns></returns>
        public virtual string GetUniqueName(string name) {
            return this.GetUniqueName(name, null);
        }

        /// <summary>
        /// 名前に基づいてMotionObjectInfoを取得します
        /// </summary>
        /// <param name="name">名前</param>
        /// <returns></returns>
        public MotionObjectInfo GetObjectInfoByName(string name) {
            _rwLockInfo.EnterReadLock();
            try {
                name = PathEx.NormalizePath(name);
                var tmp = _infoList.Where(info => info.Name == name).ToList();
                if(tmp.Count == 0)
                    return null;
                return tmp[0];
            } finally { _rwLockInfo.ExitReadLock(); }
        }

        /// <summary>
        /// 指定されたグループに属するMotionObjectInfoの一覧を返します．
        /// </summary>
        /// <param name="group">グループ</param>
        /// <returns></returns>
        public Collection<MotionObjectInfo> GetObjectInfoListByGroup(string group) {
            group = PathEx.NormalizePath(group);
            return this.GetObjectInfoList(info => PathEx.IsSubPath(info.Name, group));
        }

        /// <summary>
        /// Idに基づいてMotionObjectInfoを取得します．
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public MotionObjectInfo GetObjectInfoById(uint id) {
            _rwLockInfo.EnterReadLock();
            try {
                MotionObjectInfo ret;
                if(_infoList.TryGetObjectInfo(id, out ret))
                    return ret;
                return null;
            } finally { _rwLockInfo.ExitReadLock(); }
        }

        /// <summary>
        /// Idのリストに基づいてMotionObjectInfoのリストを取得します．
        /// </summary>
        /// <param name="idList">Idのリスト</param>
        /// <returns></returns>
        public Collection<MotionObjectInfo> GetObjectInfoById(IEnumerable<uint> idList) {
            return new Collection<MotionObjectInfo>(idList.Select(id => this.GetObjectInfoById(id)).ToList());
        }
        /// <summary>
        /// 指定されたIdのMotionObjectInfoの順序インデックスを返します．
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetObjectInfoIndex(uint id) {
            return this.GetObjectInfoIndex(this.GetObjectInfoById(id));
        }
        /// <summary>
        /// 指定されたMotionObjectInfoの順序インデックスを返します．
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public int GetObjectInfoIndex(MotionObjectInfo info) {
            return _infoList.IndexOf(info);
        }
        /// <summary>
        /// MotionObjectInfoの一覧を返します．
        /// </summary>
        /// <returns></returns>
        public Collection<MotionObjectInfo> GetObjectInfoList() {
            _rwLockInfo.EnterReadLock();
            try {
                return new Collection<MotionObjectInfo>(_infoList.ToList());
            } finally { _rwLockInfo.ExitReadLock(); }
        }
        /// <summary>
        /// 指定された条件を満たすMotionObjectInfoの一覧を返します．
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public Collection<MotionObjectInfo> GetObjectInfoList(Predicate<MotionObjectInfo> predicate) {
            _rwLockInfo.EnterReadLock();
            try {
                return new Collection<MotionObjectInfo>(_infoList.Where(info => predicate(info)).ToList());
            } finally { _rwLockInfo.ExitReadLock(); }
        }
        /// <summary>
        /// 指定されたタイプのMotionObjectInfoの一覧を返します．
        /// </summary>
        /// <param name="objectType">MotionObjectのタイプ</param>
        /// <returns></returns>
        public Collection<MotionObjectInfo> GetObjectInfoList(Type objectType) {
            return this.GetObjectInfoList(info => info.ObjectType == objectType || info.ObjectType.IsSubclassOf(objectType));
        }
        /// <summary>
        /// 指定された条件を満たす，選択されているMotionObjectInfoの一覧を返します．
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public Collection<MotionObjectInfo> GetSelectedObjectInfoList(Predicate<MotionObjectInfo> predicate) {
            return this.GetObjectInfoList(info => this.IsSelecting(info) && predicate(info));
        }
        /// <summary>
        /// 選択されているMotionObjectInfoの一覧を返します．
        /// </summary>
        /// <returns></returns>
        public Collection<MotionObjectInfo> GetSelectedObjectInfoList() {
            return this.GetObjectInfoList(info => this.IsSelecting(info));
        }
        /// <summary>
        /// 指定されたタイプの，選択されているMotionObjectInfoの一覧を返します．
        /// </summary>
        /// <param name="objectType">MotionObjectのタイプ</param>
        /// <returns></returns>
        public Collection<MotionObjectInfo> GetSelectedObjectInfoList(Type objectType) {
            return this.GetSelectedObjectInfoList(info => info.ObjectType == objectType || info.ObjectType.IsSubclassOf(objectType));
        }

        /// <summary>
        /// 指定されたオブジェクトがこのデータセットのものであるかを返します．
        /// </summary>
        /// <param name="objectInfo"></param>
        /// <returns></returns>
        public bool ContainsObject(MotionObjectInfo objectInfo) {
            if(objectInfo == null)
                throw new ArgumentNullException("objectInfo", "'objectInfo' cannot be null");
            MotionObjectInfo info = this.GetObjectInfoById(objectInfo.Id);
            if(info == null)
                return false;
            return info.Equals(objectInfo);
        }

        /// <summary>
        /// 指定されたオブジェクト情報をデータセットに追加します．
        /// </summary>
        /// <param name="objectInfo">オブジェクト情報</param>
        /// <return></return>
        public bool AddObject(MotionObjectInfo objectInfo) {
            _rwLockInfo.EnterWriteLock();
            try {
                if(objectInfo.Parent != this)
                    objectInfo.Parent = this;
                if(objectInfo.IsIdSet) {
                    // 二重に追加しない
                    if(this.ContainsObject(objectInfo))
                        return false;
                } else {
                    // Idがnullなら設定
                    objectInfo.Id = this.GetNextId();
                }
                // Nameがnullなら設定
                if(objectInfo.Name == null)
                    objectInfo.Name = "unnamed";
                // Id重複チェック
                if(_infoList.Select(info => info.Id).Contains(objectInfo.Id))
                    throw new ArgumentException(string.Format("{0} already contains Id:{1}", typeof(MotionDataSet).Name, objectInfo.Id));
                // Name重複チェック
                objectInfo.Name = this.GetUniqueName(objectInfo.Name);

                _infoList.Add(objectInfo);
                return false;
            } finally { _rwLockInfo.ExitWriteLock(); }
        }
        /// <summary>
        /// 指定されたオブジェクト情報をデータセットから削除します．
        /// </summary>
        /// <param name="objectInfo"></param>
        /// <returns></returns>
        public bool RemoveObject(MotionObjectInfo objectInfo) {
            _rwLockInfo.EnterWriteLock();
            try {
                if(!this.ContainsObject(objectInfo))
                    return false;
                try {
                    return _infoList.Remove(objectInfo);
                } finally {
                    _selectionSet.Remove(objectInfo.Id);
                }
            } finally { _rwLockInfo.ExitWriteLock(); }
        }
        /// <summary>
        /// オブジェクト情報をすべて消去します．
        /// </summary>
        public void ClearObject() {
            _rwLockInfo.EnterWriteLock();
            try {
                _infoList.Clear();
            } finally { _rwLockInfo.ExitWriteLock(); }
        }
        /// <summary>
        /// 指定された時刻におけるモーションフレームを返します．
        /// </summary>
        /// <param name="time">時刻</param>
        /// <returns></returns>
        public MotionFrame GetFrameAt(decimal time) {
            return this.GetFrameAt(time, 0);
        }
        /// <summary>
        /// 指定された時刻におけるモーションフレームから指定されたフレーム数を加算したフレームを返します．
        /// </summary>
        /// <param name="time"></param>
        /// <param name="indexOffset"></param>
        /// <returns></returns>
        public MotionFrame GetFrameAt(decimal time, int indexOffset) {
            _rwLockFrame.EnterReadLock();
            try {
                int index = this.GetFrameIndexAt(time);
                index += indexOffset;
                if(index < 0 || index >= _frameList.Count)
                    return null;
                try {
                    return _frameList.Values[index];
                } catch(IndexOutOfRangeException) {
                    // ごく稀に
                    return null;
                }
            } finally { _rwLockFrame.ExitReadLock(); }
        }
        /// <summary>
        /// 指定された時刻のフレーム番号を返します．
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public int GetFrameIndexAt(decimal time) {
            _rwLockFrame.EnterReadLock();
            try {
                return _prevIndex = CollectionEx.GetLastIndexBeforeKey<decimal, MotionFrame>(_frameList, time, _prevIndex, 1);
            } finally { _rwLockFrame.ExitReadLock(); }
        }
        /// <summary>
        /// フレームを追加します．すでにその時刻にフレームがある場合は置き換えます．
        /// </summary>
        /// <param name="frame"></param>
        public void AddFrame(MotionFrame frame) {
            _rwLockFrame.EnterWriteLock();
            try {
                _frameList[frame.Time] = frame;
            } finally { _rwLockFrame.ExitWriteLock(); }
        }

        public void RemoveFrame(decimal time) {
            _rwLockFrame.EnterWriteLock();
            try {
                _frameList.Remove(time);
            } finally { _rwLockFrame.ExitWriteLock(); }
        }
        public void RemoveFrameAt(int index) {
            _rwLockFrame.EnterWriteLock();
            try {
                _frameList.RemoveAt(index);
            } finally { _rwLockFrame.ExitWriteLock(); }
        }
        public void RemoveFrameAll(Func<MotionFrame, int, bool> match) {
            _rwLockFrame.EnterWriteLock();
            try {
                foreach(decimal time in _frameList.Values.Where((f, i) => match(f, i)).Select(f => f.Time).ToArray()) {
                    _frameList.Remove(time);
                }
            } finally { _rwLockFrame.ExitWriteLock(); }
        }
        /// <summary>
        /// すべてのフレームを消去します．
        /// </summary>
        public void ClearFrame() {
            _rwLockFrame.EnterWriteLock();
            try {
                _frameList.Clear();
                GC.Collect();
            } finally { _rwLockFrame.ExitWriteLock(); }
        }
        /// <summary>
        /// フレーム時刻の一覧を返します．
        /// </summary>
        /// <returns></returns>
        public IEnumerable<decimal> EnumerateFrameTime() {
            _rwLockFrame.EnterReadLock();
            try {
                foreach(decimal time in _frameList.Keys) {
                    yield return time;
                }
            } finally { _rwLockFrame.ExitReadLock(); }
        }
        /// <summary>
        /// フレームの一覧を返します．
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MotionFrame> EnumerateFrame() {
            _rwLockFrame.EnterReadLock();
            try {
                foreach(MotionFrame frame in _frameList.Values) {
                    yield return frame;
                }
            } finally { _rwLockFrame.ExitReadLock(); }
        }
        /// <summary>
        /// フレーム数を取得します．
        /// </summary>
        public int FrameLength { get { return _frameList.Count; } }
        /// <summary>
        /// オブジェクト情報が追加または削除された，またはオブジェクト情報の中身が変更されたときに呼ばれるイベント
        /// </summary>
        public event EventHandler ObjectInfoSetChanged = new EventHandler((o, e) => { });
        /// <summary>
        /// フレームが追加または変更または削除されたときに呼ばれるイベント
        /// </summary>
        public event EventHandler FrameListChanged = new EventHandler((o, e) => { });
        /// <summary>
        /// オブジェクトが選択または選択解除されたときに呼ばれるイベント
        /// </summary>
        public event EventHandler ObjectSelectionChanged = new EventHandler((o, e) => { });
        /// <summary>
        /// オブジェクト情報の追加及び削除，及びオブジェクト情報の中身の変更を通知します．
        /// </summary>
        public void DoObjectInfoSetChanged() {
            EventHandler tmp = this.ObjectInfoSetChanged;
            if(tmp != null)
                tmp.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// フレームの追加及変更及び削除を通知します．
        /// </summary>
        public void DoFrameListChanged() {
            EventHandler tmp = this.FrameListChanged;
            if(tmp != null)
                tmp.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// オブジェクトの選択及び選択解除を通知します．
        /// </summary>
        public void DoObjectSelectionChanged() {
            EventHandler tmp = this.ObjectSelectionChanged;
            if(tmp != null)
                tmp.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// すべてのオブジェクトを選択または選択解除します．
        /// </summary>
        /// <param name="select"></param>
        public void SelectObjects(bool select) {
            _rwLockInfo.EnterReadLock();
            try {
                _selectionSet.Clear();
                if(select) {
                    foreach(var info in _infoList) {
                        _selectionSet.Add(info.Id);
                    }
                }
            } finally { _rwLockInfo.ExitReadLock(); }
        }
        /// <summary>
        /// 指定された条件を満たすオブジェクト情報を持つオブジェクトを選択または選択解除します．
        /// </summary>
        /// <param name="select"></param>
        /// <param name="predicate"></param>
        public void SelectObjects(bool select, Predicate<MotionObjectInfo> predicate) {
            this.SelectObjects(select, _infoList.Where(info => predicate(info)));
        }

        /// <summary>
        /// 指定されたMotionObjectInfoを選択または選択解除します．
        /// </summary>
        /// <param name="select"></param>
        /// <param name="infoList"></param>
        public void SelectObjects(bool select, IEnumerable<MotionObjectInfo> infoList) {
            _rwLockInfo.EnterReadLock();
            try {
                foreach(var info in infoList) {
                    if(select) {
                        _selectionSet.Add(info.Id);
                    } else {
                        _selectionSet.Remove(info.Id);
                    }
                }
            } finally { _rwLockInfo.ExitReadLock(); }
        }
        /// <summary>
        /// 指定されたMotionObjectInfoを選択または選択解除します．
        /// </summary>
        /// <param name="select"></param>
        /// <param name="infoList"></param>
        public void SelectObjects(bool select, params MotionObjectInfo[] infoList) {
            this.SelectObjects(select, (IList<MotionObjectInfo>)infoList);
        }
        /// <summary>
        /// 指定されたIdを持つMotionObjectInfoを選択または選択解除します．
        /// </summary>
        /// <param name="select"></param>
        /// <param name="idList"></param>
        public void SelectObjects(bool select, IEnumerable<uint> idList) {
            this.SelectObjects(select, this.GetObjectInfoById(idList));
        }
        /// <summary>
        /// 指定されたIdを持つMotionObjectInfoを選択または選択解除します．
        /// </summary>
        /// <param name="select"></param>
        /// <param name="idList"></param>
        public void SelectObjects(bool select, params uint[] idList) {
            this.SelectObjects(select, (IList<uint>)idList);
        }
        /// <summary>
        /// 指定されたIdを持つMotionObjectInfoが選択されているかを返します．
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsSelecting(uint id) {
            _rwLockInfo.EnterReadLock();
            try {
                return _selectionSet.Contains(id);
            } finally { _rwLockInfo.ExitReadLock(); }
        }
        /// <summary>
        ///指定されたMotionObjectInfoが選択されているかを返します．
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool IsSelecting(MotionObjectInfo info) {
            if(info == null)
                throw new ArgumentNullException("info", "'info' cannot be null");
            return this.IsSelecting(info.Id);
        }


        /// <summary>
        /// 指定されたタイプのオブジェクトを選択または選択解除します．
        /// </summary>
        /// <param name="select"></param>
        /// <param name="type"></param>
        public void SelectObjects(bool select, Type type) {
            this.SelectObjects(select, info => info.ObjectType == type || info.ObjectType.IsSubclassOf(type));
        }
        /// <summary>
        /// 指定されたインデックスのモーションフレームを返します．
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public MotionFrame GetFrameByIndex(int index) {
            _rwLockFrame.EnterReadLock();
            try {
                if(index < 0 || index >= _frameList.Count)
                    return null;
                return _frameList.Values[index];
            } finally { _rwLockFrame.ExitReadLock(); }
        }
        /// <summary>
        /// オブジェクト情報をシリアライズします．
        /// </summary>
        /// <param name="writer"></param>
        public void SerializeInfoList(XmlWriter writer) {
            _rwLockInfo.EnterReadLock();
            try {
                XmlSerializer ser = new XmlSerializer(typeof(MotionObjectInfo));
                writer.WriteStartElement("InfoList");
                foreach(var info in _infoList) {
                    ser.Serialize(writer, info);
                }
                writer.WriteEndElement();
            } finally { _rwLockInfo.ExitReadLock(); }
        }
        /// <summary>
        /// シリアライズされたオブジェクト情報を読み込みます．
        /// </summary>
        /// <param name="reader"></param>
        public void RetrieveInfoList(XmlReader reader) {
            ClearObject();
            ClearFrame();
            XmlSerializer ser = new XmlSerializer(typeof(MotionObjectInfo));
            reader.MoveToContent();
            if(reader.Name != "InfoList")
                throw new XmlException("Element name is not 'InfoList'");
            if(reader.IsEmptyElement) {
                reader.Skip();
            } else {
                reader.ReadStartElement();
                for(reader.MoveToElement(); reader.NodeType != XmlNodeType.None && reader.NodeType != XmlNodeType.EndElement; reader.MoveToElement()) {
                    if(reader.Name == typeof(MotionObjectInfo).Name) {
                        MotionObjectInfo info = (MotionObjectInfo)ser.Deserialize(reader);
                        this.AddObject(info);
                    } else {
                        reader.Skip();
                    }
                }
                reader.ReadEndElement();
            }
        }
        /// <summary>
        /// データセットをXML形式でシリアライズします．
        /// </summary>
        /// <param name="writer"></param>
        public void SerializeXml(XmlWriter writer) {
            _progressMessage = "Saving...";
            _progressMax = this.GetObjectInfoList().Count;
            _progressValue = 0;

            writer.WriteStartElement(typeof(MotionDataSet).Name);

            SerializeInfoList(writer);

            XmlSerializer ser = new XmlSerializer(typeof(MotionFieldState));
            ser.Serialize(writer, this.FieldState);

            writer.WriteStartElement("EnumerateFrameTime");
            foreach(decimal time in this.EnumerateFrameTime()) {
                writer.WriteStartElement("FrameTime");
                writer.WriteValue(time);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("ObjectList");
            var infoList = this.GetObjectInfoList();
            foreach(var info in infoList) {
                writer.WriteStartElement("Object");
                writer.WriteAttributeString("Id", info.Id.ToString());

                Array array = this.GetObjectSequenceAsArray(info);
                XmlSerializer ser2 = new XmlSerializer(info.ObjectType.MakeArrayType());
                ser2.Serialize(writer, array);

                writer.WriteEndElement();
                _progressMessage = string.Format("Saving {0} ({1}/{2})", info.Name, _progressValue, _progressMax);
                _progressValue++;
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
            this.TrimExcess();
        }
        /// <summary>
        /// XMLでシリアライズされたデータセットを読み込みます．
        /// </summary>
        /// <param name="reader"></param>
        public void RetrieveXml(XmlReader reader) {
            _progressMessage = "Loading...";
            _progressMax = 0;
            _progressValue = 0;

            this.ClearFrame();
            this.ClearObject();
            _progressMessage = "Loading Object Information";

            reader.MoveToContent();
            reader.ReadStartElement(typeof(MotionDataSet).Name);
            for(reader.MoveToContent(); reader.NodeType != XmlNodeType.None && reader.NodeType != XmlNodeType.EndElement; reader.MoveToContent()) {
                if(reader.Name == "InfoList") {
                    this.RetrieveInfoList(reader);
                    break;
                } else {
                    reader.Skip();
                }
            }
            for(reader.MoveToContent(); reader.NodeType != XmlNodeType.None && reader.NodeType != XmlNodeType.EndElement; reader.MoveToContent()) {
                if(reader.Name == typeof(MotionFieldState).Name) {
                    XmlSerializer ser = new XmlSerializer(typeof(MotionFieldState));
                    this.FieldState = (MotionFieldState)ser.Deserialize(reader);
                    break;
                } else {
                    reader.Skip();
                }
            }
            for(reader.MoveToContent(); reader.NodeType != XmlNodeType.None && reader.NodeType != XmlNodeType.EndElement; reader.MoveToContent()) {
                if(reader.Name == "EnumerateFrameTime") {
                    if(reader.IsEmptyElement) {
                        reader.Skip();
                    } else {
                        reader.ReadStartElement();
                        for(reader.MoveToContent(); reader.NodeType != XmlNodeType.None && reader.NodeType != XmlNodeType.EndElement; reader.MoveToContent()) {
                            if(reader.Name == "FrameTime") {
                                MotionFrame frame = new MotionFrame(this, reader.ReadElementContentAsDecimal());
                                _progressMessage = string.Format("Loading Frame at {0}", frame.Time);

                                this.AddFrame(frame);
                            } else {
                                reader.Skip();
                            }
                        }
                        reader.ReadEndElement();
                    }
                    break;
                } else {
                    reader.Skip();
                }
            }
            _progressMessage = "Loading Each Object";
            _progressMax = this.GetObjectInfoList().Count;
            for(reader.MoveToContent(); reader.NodeType != XmlNodeType.None && reader.NodeType != XmlNodeType.EndElement; reader.MoveToContent()) {
                if(reader.Name == "ObjectList") {
                    if(reader.IsEmptyElement) {
                        reader.Skip();
                    } else {
                        reader.ReadStartElement();
                        for(reader.MoveToContent(); reader.NodeType != XmlNodeType.None && reader.NodeType != XmlNodeType.EndElement; reader.MoveToContent()) {
                            if(reader.Name == "Object") {

                                string idStr;
                                uint id;
                                MotionObjectInfo info;
                                if((idStr = reader.GetAttribute("Id")) != null && uint.TryParse(idStr, out id) && (info = this.GetObjectInfoById(id)) != null) {
                                    _progressValue++;
                                    _progressMessage = string.Format("Loading {0} ({1}/{2})", info.Name, _progressValue, _progressMax);

                                    if(reader.IsEmptyElement) {
                                        reader.Skip();
                                    } else {
                                        reader.ReadStartElement();
                                        XmlSerializer ser = new XmlSerializer(info.ObjectType.MakeArrayType());
                                        Array array = (Array)ser.Deserialize(reader);
                                        int index = 0;
                                        foreach(var frame in this.EnumerateFrame()) {
                                            frame[info] = (MotionObject)array.GetValue(index);
                                            index++;
                                        }
                                        reader.ReadEndElement();
                                    }
                                } else {
                                    reader.Skip();
                                }
                            } else {
                                reader.Skip();
                            }
                        }
                        reader.ReadEndElement();
                    }
                    break;
                } else {
                    reader.Skip();
                }
            }
            reader.ReadEndElement();
        }
        /// <summary>
        /// バイナリ形式でデータセットをシリアライズします．
        /// </summary>
        /// <param name="writer"></param>
        public void SerializeBinary(BinaryWriter writer) {
            _progressMessage = "Saving...";
            _progressMax = this.FrameLength;
            _progressValue = 0;

            StringWriter strWriter = new StringWriter();
            using(XmlWriter xmlWriter = XmlWriter.Create(strWriter)) {
                this.SerializeInfoList(xmlWriter);
            }
            writer.Write(strWriter.ToString());

            XmlSerializer ser = new XmlSerializer(typeof(MotionFieldState));
            strWriter = new StringWriter();
            using(XmlWriter xmlWriter = XmlWriter.Create(strWriter)) {
                ser.Serialize(xmlWriter, this.FieldState);
            }
            writer.Write(strWriter.ToString());

            writer.Write(this.FrameLength);
            foreach(var frame in this.EnumerateFrame()) {
                frame.SerializeBinary(writer);
                _progressMessage = string.Format("Saving {0}/{1}", _progressValue, this.FrameLength);
                _progressValue++;
            }
        }
        /// <summary>
        /// バイナリ形式でシリアライズされたデータセットを読み込みます．
        /// </summary>
        /// <param name="reader"></param>
        public void RetrieveBinary(BinaryReader reader) {
            _progressMessage = "Loading...";
            _progressMax = 0;
            _progressValue = 0;

            this.ClearFrame();
            this.ClearObject();
            string infoStr = reader.ReadString();
            using(StringReader strReader = new StringReader(infoStr))
            using(XmlReader xmlReader = XmlReader.Create(strReader)) {
                this.RetrieveInfoList(xmlReader);
            }
            string fieldStateStr = reader.ReadString();
            using(StringReader strReader = new StringReader(fieldStateStr))
            using(XmlReader xmlReader = XmlReader.Create(strReader)) {
                XmlSerializer ser = new XmlSerializer(typeof(MotionFieldState));
                this.FieldState = (MotionFieldState)ser.Deserialize(xmlReader);
            }
            int frameLength = reader.ReadInt32();
            _frameList.Capacity = frameLength;
            _progressMax = frameLength;

            bool playing = TimeController.Singleton.IsPlaying;
            decimal timeStart = TimeController.Singleton.CurrentTime;

            for(int i = 0; i < frameLength; i++) {
                MotionFrame frame = MotionFrame.DeserializeBinary(reader, this);
                this.AddFrame(frame);
                _progressMessage = string.Format("Loading {0}/{1}", i, frameLength);
                _progressValue = i;
                if(!playing && (i % 5000) == 0) {
                    if(TimeController.Singleton.BeginTime <= frame.Time && frame.Time <= TimeController.Singleton.EndTime) {
                        TimeController.Singleton.CurrentTime = frame.Time;
                    }
                }
            }
            if(!playing) {
                TimeController.Singleton.CurrentTime = timeStart;
            }
        }
        public void TrimExcess() {
            if(_frameList.Count * 10 < _frameList.Capacity * 9) {
                _frameList.Capacity = _frameList.Count;
                GC.Collect();
            }
        }

        /// <summary>
        /// 旧バージョンのMotionDataSetを現在のMotionDataSetに読み込みます．
        /// </summary>
        /// <param name="dataSet"></param>
        public void FromOldVersion(Old.MotionDataSet dataSet) {
            _progressMessage = "Converting...";
            _progressMax = dataSet.FrameLength;
            _progressValue = 0;

            this.ClearObject();
            this.ClearFrame();
            Dictionary<int, uint> idMapPoint = new Dictionary<int, uint>();
            Dictionary<int, uint> idMapLine = new Dictionary<int, uint>();
            Dictionary<int, uint> idMapCylinder = new Dictionary<int, uint>();
            Dictionary<int, uint> idMapSphere = new Dictionary<int, uint>();
            Dictionary<int, uint> idMapPlane = new Dictionary<int, uint>();

            _progressMessage = "Converting Object Information...";
            foreach(var info in dataSet.Header.Infos) {
                MotionObjectInfo newInfo = new MotionObjectInfo();
                newInfo.Name = string.Format("{0}/{1}", info.GroupName, info.Name);
                newInfo.Id = this.GetNextId();
                switch(info.ObjectType) {
                case Old.MotionDataObjectType.Point:
                    newInfo.ObjectType = typeof(PointObject);
                    idMapPoint[info.ObjectIndex] = newInfo.Id;
                    break;
                case Old.MotionDataObjectType.Line:
                    newInfo.ObjectType = typeof(LineObject);
                    idMapLine[info.ObjectIndex] = newInfo.Id;
                    break;
                case Old.MotionDataObjectType.Cylinder:
                    newInfo.ObjectType = typeof(CylinderObject);
                    idMapCylinder[info.ObjectIndex] = newInfo.Id;
                    break;
                case Old.MotionDataObjectType.Sphere:
                    newInfo.ObjectType = typeof(SphereObject);
                    idMapSphere[info.ObjectIndex] = newInfo.Id;
                    break;
                case Old.MotionDataObjectType.Plane:
                    newInfo.ObjectType = typeof(PlaneObject);
                    idMapPlane[info.ObjectIndex] = newInfo.Id;
                    break;
                }
                newInfo.IsVisible = info.Visible;
                newInfo.Color = info.Color;
                this.AddObject(newInfo);
            }
            _progressMessage = "Converting Frames...";
            _progressMax = dataSet.FrameLength;

            foreach(var pair in dataSet.Frames) {
                _progressValue++;
                _progressMessage = string.Format("Converting Frames {0}/{1}", _progressValue, _progressMax);

                MotionFrame frame = new MotionFrame(this, pair.Key);
                for(int i = 0; i < pair.Value.Points.Length; i++) {
                    if(pair.Value.Points[i].Exists) {
                        uint id = idMapPoint[i];
                        frame[id] = new PointObject(pair.Value.Points[i].Position);
                    }
                }
                for(int i = 0; i < pair.Value.Lines.Length; i++) {
                    if(pair.Value.Lines[i].Exists) {
                        uint id = idMapLine[i];
                        frame[id] = new LineObject(pair.Value.Lines[i].End, pair.Value.Lines[i].DirectionAndLength);
                    }
                }
                for(int i = 0; i < pair.Value.Cylinders.Length; i++) {
                    if(pair.Value.Cylinders[i].Exists) {
                        uint id = idMapCylinder[i];
                        frame[id] = new CylinderObject(pair.Value.Cylinders[i].End, pair.Value.Cylinders[i].Axis, pair.Value.Cylinders[i].Radius);
                    }
                }
                for(int i = 0; i < pair.Value.Spheres.Length; i++) {
                    if(pair.Value.Spheres[i].Exists) {
                        uint id = idMapSphere[i];
                        frame[id] = new SphereObject(pair.Value.Spheres[i].Center, pair.Value.Spheres[i].Radius);
                    }
                }
                for(int i = 0; i < pair.Value.Planes.Length; i++) {
                    if(pair.Value.Planes[i].Exists) {
                        uint id = idMapPlane[i];
                        frame[id] = new PlaneObject(pair.Value.Planes[i].Points);
                    }
                }
                this.AddFrame(frame);
            }
        }


        #region ITimeInterval メンバ
        /// <summary>
        /// 最初のフレームのフレーム時刻を取得します．フレームがない場合は0
        /// </summary>
        public decimal BeginTime {
            get {
                _rwLockFrame.EnterReadLock();
                try {
                    if(this.FrameLength == 0)
                        return 0;
                    MotionFrame frame = this.GetFrameByIndex(0);
                    if(frame == null)
                        return 0;
                    return frame.Time;
                } finally { _rwLockFrame.ExitReadLock(); }
            }
        }
        /// <summary>
        /// 最後のフレームのフレーム時刻を取得します．フレームがない場合は0
        /// </summary>
        public decimal EndTime {
            get {
                _rwLockFrame.EnterReadLock();
                try {
                    if(this.FrameLength == 0)
                        return 0;
                    MotionFrame frame = this.GetFrameByIndex(this.FrameLength - 1);
                    if(frame == null)
                        return 0;
                    return frame.Time;
                } finally { _rwLockFrame.ExitReadLock(); }
            }
        }

        public event EventHandler TimeIntervalChanged;

        #endregion
        /// <summary>
        /// 選択されているオブジェクトをデータセットから取り除きます．
        /// </summary>
        /// <param name="askUser"></param>
        public void RemoveSelectedObjects(bool askUser, Control callerControl) {
            var infoList = this.GetSelectedObjectInfoList();
            if(infoList.Count != 0) {
                if(askUser) {
                    string targetName;
                    const int limit = 16;
                    if(infoList.Count <= limit) {
                        targetName = CollectionEx.Join("\n", infoList.Select(info => info.Name + "(" + info.ObjectType.Name + ")"));
                    } else {
                        targetName = CollectionEx.Join("\n", infoList.Take(limit).Select(info => info.Name + "(" + info.ObjectType.Name + ")"));
                        targetName += "...\n";
                    }
                    if(MessageBox.Show("Remove these objects?\n" + targetName, "info", MessageBoxButtons.OKCancel) != DialogResult.OK) {
                        return;
                    }
                }
                MotionOperationExecution exec = new MotionOperationExecution(new DefaultOperations.OperationRemoveObjects(), Script.ScriptConsole.Singleton);
                exec.OperateThread(callerControl);
            }
        }

        public void RenameSelectedObjects(string newName, Control callerControl) {
            IList<MotionObjectInfo> infoList = this.GetSelectedObjectInfoList();
            if(infoList.Count == 1 && infoList[0].Name == newName)
                return;
            MotionOperationExecution exec = new MotionOperationExecution(new DefaultOperations.OperationRename(), Script.ScriptConsole.Singleton);
            StringParameter name = exec.Parameters[0] as StringParameter;
            name.Value = newName;
            try {
                exec.Operate(callerControl);
            } catch(Exception ex) {
                ErrorLogger.Tell(ex, "スクリプト処理系エラー");
            }
        }

        public void RenameObjectInfo(MotionObjectInfo info, string newName, Control callerControl) {
            if(info.Name == newName)
                return;
            try {
                MotionOperationScriptFunction func = new MotionOperationScriptFunction(new DefaultOperations.OperationRename());
                List<Script.ScriptVariable> args = new List<MotionDataHandler.Script.ScriptVariable>();
                args.Add(new Script.ListVariable(new Script.StringVariable(info.Name)));
                args.Add(new Script.StringVariable(newName));
                Script.ScriptConsole.Singleton.MotionDataSet = this;
                Script.ScriptConsole.Singleton.Invoke(func, args);
            } catch(Exception ex) {
                ErrorLogger.Tell(ex, "スクリプト処理系エラー");
            }
        }

        /// <summary>
        /// オブジェクト情報の順序を入れ替えます．
        /// </summary>
        /// <param name="id2index">各Idを持つオブジェクト情報の位置．この値に基づいて昇順で並べ替えます．この中に含まれないIdを持つオブジェクト情報は現在の位置番号が使われます．</param>
        public void ReorderObjectInfo(IDictionary<uint, double> id2index) {
            _rwLockInfo.EnterWriteLock();
            try {
                // 現在の順序をそのまま { Id => newOrder }
                Dictionary<uint, double> baseOrder = new Dictionary<uint, double>();
                for(int index = 0; index < _infoList.Count; index++) {
                    baseOrder[_infoList[index].Id] = index;
                }
                // 指定された引数の順序で上書き
                Dictionary<uint, double> order = new Dictionary<uint, double>(baseOrder);
                foreach(var pair in id2index) {
                    order[pair.Key] = pair.Value;
                }
                // 新しい順序でソート．値が同じものがあれば元の順序で比較
                List<KeyValuePair<uint, double>> orderByValue = order.ToList();
                orderByValue.Sort(new Comparison<KeyValuePair<uint, double>>((x, y) => {
                    int c = x.Value.CompareTo(y.Value);
                    if(c != 0)
                        return c;
                    return baseOrder[x.Key].CompareTo(baseOrder[y.Key]);
                }));
                // ソートされた順序からIdの順序を取り出す．
                var infoById = _infoList.ToDictionary(info => info.Id);
                _infoList.Clear();
                foreach(var id in orderByValue.Select(pair => pair.Key)) {
                    _infoList.Add(infoById[id]);
                }
            } finally { _rwLockInfo.ExitWriteLock(); }
            // 変更を反映しておく
            this.DoObjectInfoSetChanged();
        }
        /// <summary>
        /// 指定されたIdを持つオブジェクト情報の位置を指定された値だけずらします．
        /// </summary>
        /// <param name="idList">順序を変えるIdのリスト．</param>
        /// <param name="offsetIndex">ずらす量</param>
        public void ReorderObjectInfo(IEnumerable<uint> idList, int offsetIndex) {
            Dictionary<uint, double> order = new Dictionary<uint, double>();
            _rwLockInfo.EnterReadLock();
            try {
                RangeSet<int> indexRangeSet = new RangeSet<int>();
                var indices = idList.Select(id => this.GetObjectInfoIndex(id)).Where(index => index != -1).ToList();
                if(indices.Count == 0)
                    return;
                foreach(var index in indices) {
                    indexRangeSet.Add(new RangeSet<int>.Range(index, index + 1));
                }

                foreach(var indexRange in indexRangeSet) {
                    double toIndex;
                    if(offsetIndex > 0) {
                        toIndex = indexRange.End - 0.5 + offsetIndex;
                    } else {
                        toIndex = indexRange.Start - 0.5 + offsetIndex;
                    }
                    for(int index = indexRange.Start; index < indexRange.End; index++) {
                        order[_infoList[index].Id] = toIndex;
                    }
                }
            } finally { _rwLockInfo.ExitReadLock(); }
            this.ReorderObjectInfo(order);
        }
        public void ReorderObjectInfo(IEnumerable<MotionObjectInfo> infoList, int offsetIndex) {
            this.ReorderObjectInfo(infoList.Select(info => info.Id), offsetIndex);
        }

        public void GatherObjectInfoOrder(IEnumerable<uint> idList) {
            Dictionary<uint, double> order = new Dictionary<uint, double>();
            _rwLockInfo.EnterReadLock();
            try {
                var indices = idList.Select(id => this.GetObjectInfoIndex(id)).Where(index => index != -1).ToList();
                if(indices.Count == 0)
                    return;
                int toIndex = indices.Min();
                foreach(var index in indices) {
                    order[_infoList[index].Id] = toIndex;
                }
            } finally { _rwLockInfo.ExitReadLock(); }
            this.ReorderObjectInfo(order);
        }

        public void GatherObjectInfoOrder(IEnumerable<MotionObjectInfo> infoList) {
            this.GatherObjectInfoOrder(infoList.Select(info => info.Id));
        }

        public Array GetObjectSequenceAsArray(uint id) {
            MotionObjectInfo info = this.GetObjectInfoById(id);
            if(info == null)
                return null;
            return this.GetObjectSequenceAsArray(info);
        }

        public Array GetObjectSequenceAsArray(IReadableMotionObjectInfo info) {
            _rwLockFrame.EnterReadLock();
            try {
                if(info == null)
                    throw new ArgumentNullException("info", "'info' cannot be null");
                Type arrayType = info.ObjectType.MakeArrayType();
                Array array = (Array)Activator.CreateInstance(arrayType, this.FrameLength);
                int index = 0;
                foreach(var frame in this.EnumerateFrame()) {
                    array.SetValue(frame[info], index);
                    index++;
                }
                return array;
            } finally { _rwLockFrame.ExitReadLock(); }
        }

        public void DeleteSelectedObjectOfFrames(decimal beginTimeRange, decimal endTimeRange, bool ask) {
            var infoList = this.GetSelectedObjectInfoList();
            if(ask) {
                string msg = string.Format("Delete these Objects where Frame between {0}s and {1}s.\n", beginTimeRange.ToString("0.000"), endTimeRange.ToString("0.000"));
                foreach(var info in infoList) {
                    msg += string.Format("{0}({1})\n", info.Name, info.ObjectType.Name);
                }
                if(MessageBox.Show(msg, "Delete", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    return;
            }
            WaitForForm form = new WaitForForm((ctrl) => {
                try {
                    _rwLockFrame.EnterReadLock();
                    try {
                        int beginIndex = this.GetFrameIndexAt(beginTimeRange);
                        int endIndex = this.GetFrameIndexAt(endTimeRange);
                        for(int i = beginIndex; i <= endIndex; i++) {
                            MotionFrame frame = this.GetFrameByIndex(i);
                            if(frame != null && beginTimeRange <= frame.Time && frame.Time <= endTimeRange) {
                                foreach(var info in infoList) {
                                    frame.RemoveObject(info);
                                }
                            }
                        }
                    } finally { _rwLockFrame.ExitReadLock(); }
                    ctrl.DialogResult = DialogResult.OK;
                } catch(Exception ex) {
                    ErrorLogger.Tell(ex, "An error occured between deletion");
                } finally {
                    this.DoFrameListChanged();
                }
            });
            form.ShowDialog();
        }

        private readonly Dictionary<System.Reflection.Assembly, IList<IMotionOperationBase>> _operations = new Dictionary<System.Reflection.Assembly, IList<IMotionOperationBase>>();
        private object _lockLoadMotionOperations = new object();
        /// <summary>
        /// MotionDataHandlerに含まれるIMotionOperationインターフェースを継承するクラスのインスタンスを処理リストに追加します
        /// </summary>
        public void LoadMotionOperations() {
            this.LoadMotionOperations(System.Reflection.Assembly.GetExecutingAssembly());
        }
        /// <summary>
        /// 指定されたアセンブリからIMotionOperationインターフェースを継承するクラスのインスタンスを処理リストに追加します
        /// </summary>
        /// <param name="asm"></param>
        public void LoadMotionOperations(System.Reflection.Assembly asm) {
            lock(_lockLoadMotionOperations) {
                if(!_operations.ContainsKey(asm)) {
                    System.Reflection.Module[] modules = asm.GetModules();
                    List<Type> iOperationList = new List<Type>();
                    foreach(var module in modules) {
                        iOperationList.AddRange(from type in module.GetTypes()
                                                where type.GetInterfaces().Contains(typeof(IMotionOperationBase)) && !type.IsAbstract && !type.IsInterface
                                                select type);
                    }
                    List<IMotionOperationBase> operations = new List<IMotionOperationBase>();
                    foreach(var iOpe in iOperationList) {
                        var opeCtor = iOpe.GetConstructor(new Type[0]);
                        if(opeCtor != null) {
                            var opeObj = opeCtor.Invoke(new object[0]) as IMotionOperationBase;
                            if(opeObj != null) {
                                operations.Add(opeObj);
                                MotionOperationScriptFunction opeFunc = new MotionOperationScriptFunction(opeObj);
                                Script.ScriptConsole.Singleton.RegisterFunction(opeFunc);
                                IMotionOperationEditObject editOpe = opeObj as IMotionOperationEditObject;
                                if(editOpe != null) {
                                    IMotionOperationCreateObject opeObj2 = new MotionOperationEditToCreateWrapper(editOpe);
                                    operations.Add(opeObj2);
                                    MotionOperationScriptFunction opeFunc2 = new MotionOperationScriptFunction(opeObj2);
                                    Script.ScriptConsole.Singleton.RegisterFunction(opeFunc2);
                                }
                            }
                        }
                    }
                    _operations[asm] = operations;
                }
            }
        }

        private Collection<T> getOperation<T>() where T : class, IMotionOperationBase {
            return new Collection<T>((from ope in
                                          from ope2 in _operations.SelectMany(o => o.Value)
                                          select ope2 as T
                                      where ope != null
                                      orderby ope.GetTitle()
                                      select ope).ToList());
        }
        public Collection<IMotionOperationEditObject> GetOperationEditObject() {
            return getOperation<IMotionOperationEditObject>();
        }
        public Collection<IMotionOperationCreateObject> GetOperationCreateObject() {
            return getOperation<IMotionOperationCreateObject>();
        }
        public Collection<IMotionOperationGeneral> GetOperationGeneral() {
            return getOperation<IMotionOperationGeneral>();
        }
        public Collection<IMotionOperationOutputSequence> GetOperationOutputSequence() {
            return getOperation<IMotionOperationOutputSequence>();
        }

    }
}
