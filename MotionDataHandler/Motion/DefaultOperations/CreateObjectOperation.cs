using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.DirectX;
namespace MotionDataHandler.Motion.DefaultOperations {
    // IMotionOperationCreateObjectを実装するファイル
    using Script;
    using Operation;
    using Misc;

    public class OperationFixedPoint : IMotionOperationCreateObject {
        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<MotionDataHandler.Misc.ProcParam<MotionProcEnv>> args) {
            MotionObjectInfo newInfo = new MotionObjectInfo(typeof(PointObject));
            newInfo.Name = "FixedPoint";
            return new MotionObjectInfo[] { newInfo };
        }

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            NumberParameter x = args[0] as NumberParameter;
            NumberParameter y = args[1] as NumberParameter;
            NumberParameter z = args[2] as NumberParameter;
            MotionObject ret = new PointObject(new Vector3((float)x.Value, (float)y.Value, (float)z.Value));
            return new[] { ret };
        }

        public Type CreatedType {
            get { return typeof(PointObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return false;
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            return true;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            NumberParameter x = new NumberParameter("X", -100000, 100000, 3);
            NumberParameter y = new NumberParameter("Y", -100000, 100000, 3);
            NumberParameter z = new NumberParameter("Z", -100000, 100000, 3);
            return new ProcParam<MotionProcEnv>[] { x, y, z };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            NumberParameter x = args[0] as NumberParameter;
            NumberParameter y = args[1] as NumberParameter;
            NumberParameter z = args[2] as NumberParameter;
            return true;
        }

        public string GetCommandName() {
            return "FixedPoint";
        }

        public string GetTitle() {
            return "固定点 / FixedPoint";
        }

        public string GetDescription() {
            return "空間内の絶対位置に点を作成します";
        }

        public Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.obj_point; }
        }

        #endregion
    }

    public class OperationLineCollisionPoint : IMotionOperationCreateObject {
        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args) {
            MotionObjectSingleSelectParameter main = args[0] as MotionObjectSingleSelectParameter;
            NumberParameter enlarge = args[1] as NumberParameter;
            MotionObjectInfo newInfo = new MotionObjectInfo(typeof(PointObject));
            newInfo.Name = PathEx.GiveName("Collision", main.Value.Name);
            return new MotionObjectInfo[] { newInfo };
        }

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            MotionObjectSingleSelectParameter main = args[0] as MotionObjectSingleSelectParameter;
            NumberParameter enlarge = args[1] as NumberParameter;
            float enlargement = 0;
            try { enlargement = (float)enlarge.Value; } catch(ArithmeticException) { }
            IList<MotionObjectInfo> targetInfoList = selectedInfoList.Where(info => info != main.Value).ToList();

            LineObject line = (LineObject)frame[main.Value];
            if(line == null || targetInfoList.Any(info => frame[info] == null)) {
                return new MotionObject[] { null };
            }
            float minDistance = line.Length();
            bool found = false;
            foreach(var targetInfo in targetInfoList) {
                float distance;
                if(frame[targetInfo].DetectLineCollision(line.Position, line.AnotherEnd, enlargement, out distance)) {
                    if(distance >= 0 && distance < minDistance) {
                        found = true;
                        minDistance = distance;
                    }
                }
            }
            if(!found) {
                return new MotionObject[] { null };
            }
            return new MotionObject[] { new PointObject(line.Position + line.Direction() * minDistance) };
        }

        public Type CreatedType {
            get { return typeof(PointObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return true;
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Where(info => info.IsTypeOf<LineObject>()).Count() == 0 || infoList.Count < 2) {
                errorMessage = "線分オブジェクトを一つと衝突検出対象オブジェクトを一つ以上選択してください";
                return false;
            }
            return true;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            MotionObjectSingleSelectParameter main = new MotionObjectSingleSelectParameter("Main Line", info => info.IsTypeOf<LineObject>(), true);
            NumberParameter enlarge = new NumberParameter("Enlarge targets", -10000, 10000, 3);
            return new ProcParam<MotionProcEnv>[] { main, enlarge };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            MotionObjectSingleSelectParameter main = args[0] as MotionObjectSingleSelectParameter;
            if(main.Value == null) {
                errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_CannotSpecifyNull + ": " + args[0].ParamName;
                return false;
            }
            NumberParameter enlarge = args[1] as NumberParameter;
            return true;
        }

        public string GetCommandName() {
            return "DetectCollisionPoint";
        }

        public string GetTitle() {
            return "線分の衝突点検出 / Detect Collision Point of Line and other Objects";
        }

        public string GetDescription() {
            return "指定された線分オブジェクトと他のオブジェクトとの交点を求めます．複数のオブジェクトと交差する場合には線分オブジェクトの始点に最も近い位置に交点が作成されます";
        }

        public Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.line_collision; }
        }

        #endregion
    }

    public class OperationSphereAroundPointFromSequence : IMotionOperationCreateObject {
        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args) {
            SequenceSingleSelectParameter sequence = args[0] as SequenceSingleSelectParameter;
            SequenceColumnSelectParameter column = args[1] as SequenceColumnSelectParameter;
            List<MotionObjectInfo> ret = new List<MotionObjectInfo>();
            foreach(MotionObjectInfo info in selectedInfoList) {
                MotionObjectInfo newInfo = new MotionObjectInfo(typeof(SphereObject), info);
                newInfo.Name = PathEx.GiveName("Around", info.Name);
                ret.Add(newInfo);
            }
            return ret;
        }

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            SequenceSingleSelectParameter sequence = args[0] as SequenceSingleSelectParameter;
            SequenceColumnSelectParameter column = args[1] as SequenceColumnSelectParameter;
            List<MotionObject> ret = new List<MotionObject>();
            foreach(MotionObjectInfo info in selectedInfoList) {
                MotionObject created = null;
                PointObject point = frame[info] as PointObject;
                if(point != null) {
                    decimal?[] row = sequence.Value.Values.GetValueAt(frame.Time);
                    decimal? value = row[column.Value];
                    if(value.HasValue) {
                        try {
                            created = new SphereObject(point.Position, (float)value.Value);
                        } catch(ArithmeticException) { }
                    }
                }
                ret.Add(created);
            }
            return ret;
        }

        public Type CreatedType {
            get { return typeof(SphereObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf<PointObject>();
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count > 0)
                return true;
            errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_PleaseSelectPointObject;
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            SequenceSingleSelectParameter sequence = new SequenceSingleSelectParameter("Time-Series data For radius", view => (view.Type & MotionDataHandler.Sequence.SequenceType.Numeric) != 0);
            SequenceColumnSelectParameter column = new SequenceColumnSelectParameter("Column", sequence);
            return new ProcParam<MotionProcEnv>[] { sequence, column };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            SequenceSingleSelectParameter sequence = args[0] as SequenceSingleSelectParameter;
            if(sequence.Value == null) {
                errorMessage = "シーケンス名を指定してください";
                return false;
            }
            SequenceColumnSelectParameter column = args[1] as SequenceColumnSelectParameter;
            return true;
        }

        public string GetCommandName() {
            return "SphereAroundPointFromSequence";
        }

        public string GetTitle() {
            return "点を中心とした球(時系列データから半径を設定) / Sphere around Point with Radius from Sequence";
        }

        public string GetDescription() {
            return "点オブジェクトを中心とし，時系列データの対応する値を半径とする球オブジェクトを作成します";
        }

        public Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.pointSphere; }
        }

        #endregion
    }
    public class OperationSphereAroundPoint : IMotionOperationCreateObject {
        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args) {
            List<MotionObjectInfo> ret = new List<MotionObjectInfo>();
            foreach(MotionObjectInfo info in selectedInfoList) {
                MotionObjectInfo newInfo = new MotionObjectInfo(typeof(SphereObject), info);
                newInfo.Name = PathEx.GiveName("Around", info.Name);
                ret.Add(newInfo);
            }
            return ret;
        }

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            NumberParameter radius = args[0] as NumberParameter;

            List<MotionObject> ret = new List<MotionObject>();
            foreach(MotionObjectInfo info in selectedInfoList) {
                MotionObject created = null;
                PointObject point = frame[info] as PointObject;
                if(point != null) {
                    try {
                        created = new SphereObject(point.Position, (float)radius.Value);
                    } catch(ArithmeticException) { }
                }
                ret.Add(created);
            }
            return ret;
        }

        public Type CreatedType {
            get { return typeof(SphereObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf<PointObject>();
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count > 0)
                return true;
            errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_PleaseSelectPointObject;
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            NumberParameter radius = new NumberParameter("Radius", 0, 100000, 3);
            return new ProcParam<MotionProcEnv>[] { radius };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            NumberParameter radius = args[0] as NumberParameter;
            return true;
        }

        public string GetCommandName() {
            return "SphereAroundPoint";
        }

        public string GetTitle() {
            return "点を中心とした球 / Sphere around Point";
        }

        public string GetDescription() {
            return "点オブジェクトを中心とした半径固定の球オブジェクトを作成します";
        }

        public Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.pointSphere; }
        }

        #endregion
    }

    public class OperationPointBetweenPoints : IMotionOperationCreateObject {
        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args) {
            MotionObjectInfo info1 = selectedInfoList[0];
            MotionObjectInfo info2 = selectedInfoList[1];
            MotionObjectInfo newInfo = new MotionObjectInfo(typeof(PointObject), info1);
            newInfo.Name = PathEx.GiveName("Between", info1.Name, info2.Name);
            return new MotionObjectInfo[] { newInfo };
        }

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            NumberParameter interpolater = args[0] as NumberParameter;

            MotionObjectInfo info1 = selectedInfoList[0];
            MotionObjectInfo info2 = selectedInfoList[1];

            PointObject point1 = frame[info1] as PointObject;
            PointObject point2 = frame[info2] as PointObject;

            MotionObject ret = null;
            if(point1 != null && point2 != null) {
                try {
                    ret = new PointObject(Vector3.Lerp(point1.Position, point2.Position, (float)interpolater.Value));
                } catch(ArithmeticException) { }
            }
            return new MotionObject[] { ret };
        }

        public Type CreatedType {
            get { return typeof(PointObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf<PointObject>();
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count == 2)
                return true;
            errorMessage = "二つの点オブジェクトを選択してください";
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            NumberParameter interpolater = new NumberParameter("Interpolater", -1000, 1000, 3, 0.01M);
            return new ProcParam<MotionProcEnv>[] { interpolater };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            NumberParameter interpolater = args[0] as NumberParameter;
            return true;
        }

        public string GetCommandName() {
            return "PointBetweenTwoPoints";
        }

        public string GetTitle() {
            return "二点を結ぶ直線上の点 / Point between two Points";
        }

        public string GetDescription() {
            return "二つの点オブジェクトを内分または外分する点オブジェクトを作成します";
        }

        public Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.linePoint; }
        }

        #endregion
    }

    public class OperationPointOnLine : IMotionOperationCreateObject {
        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args) {
            List<MotionObjectInfo> ret = new List<MotionObjectInfo>();
            foreach(MotionObjectInfo info in selectedInfoList) {
                MotionObjectInfo newInfo = new MotionObjectInfo(typeof(PointObject), info);
                newInfo.Name = PathEx.GiveName("On", info.Name);
                ret.Add(newInfo);
            }
            return ret;
        }

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            NumberParameter interpolater = args[0] as NumberParameter;
            List<MotionObject> ret = new List<MotionObject>();
            foreach(MotionObjectInfo info in selectedInfoList) {
                MotionObject created = null;
                LineObject line = frame[info] as LineObject;
                if(line != null) {
                    try {
                        created = new PointObject(Vector3.Lerp(line.Position, line.AnotherEnd, (float)interpolater.Value));
                    } catch(ArithmeticException) { }
                }
                ret.Add(created);
            }
            return ret;
        }

        public Type CreatedType {
            get { return typeof(PointObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf<LineObject>();
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count > 0)
                return true;
            errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_PleaseSelectLineObject;
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            NumberParameter interpolater = new NumberParameter("Interpolater", -1000, 1000, 3, 0.01M);
            return new ProcParam<MotionProcEnv>[] { interpolater };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            NumberParameter interpolater = args[0] as NumberParameter;
            return true;
        }

        public string GetCommandName() {
            return "PointOnLine";
        }

        public string GetTitle() {
            return "線分上の点 / Point on Line";
        }

        public string GetDescription() {
            return "線分オブジェクトの両端の点を内分または外分する点オブジェクトを作成します";
        }

        public Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.pointLine; }
        }

        #endregion
    }

    public class OperationPointCenterOfSphere : IMotionOperationCreateObject {
        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args) {
            List<MotionObjectInfo> ret = new List<MotionObjectInfo>();
            foreach(MotionObjectInfo info in selectedInfoList) {
                MotionObjectInfo newInfo = new MotionObjectInfo(typeof(PointObject), info);
                newInfo.Name = PathEx.GiveName("Center", info.Name);
                ret.Add(newInfo);
            }
            return ret;
        }

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            List<MotionObject> ret = new List<MotionObject>();
            foreach(MotionObjectInfo info in selectedInfoList) {
                MotionObject created = null;
                SphereObject sphere = frame[info] as SphereObject;
                if(sphere != null) {
                    created = new PointObject(sphere.Position);
                }
                ret.Add(created);
            }
            return ret;
        }

        public Type CreatedType {
            get { return typeof(PointObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf<SphereObject>();
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count > 0)
                return true;
            errorMessage = "球オブジェクトを選択してください";
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            return null;
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            return true;
        }

        public string GetCommandName() {
            return "PointCenterOfSphere";
        }

        public string GetTitle() {
            return "球の中心点 / Point Center of Sphere";
        }

        public string GetDescription() {
            return "球オブジェクトの中心点となる点オブジェクトを作成します";
        }

        public Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.pointSphere; }
        }

        #endregion
    }

    public class OperationPointCoordinateByTwoLines : IMotionOperationCreateObject {

        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args) {
            MotionObjectSingleSelectParameter first = args[0] as MotionObjectSingleSelectParameter;

            MotionObjectInfo info1 = selectedInfoList[0];
            MotionObjectInfo info2 = selectedInfoList[1];

            MotionObjectInfo firstAxis = first.Value;
            MotionObjectInfo secondAxis = selectedInfoList.Where(info => info.IsTypeOf(typeof(LineObject)) && info != firstAxis).First();
            MotionObjectInfo newInfo = new MotionObjectInfo(typeof(PointObject), firstAxis);
            newInfo.Name = PathEx.GiveName("Coord", firstAxis.Name, secondAxis.Name);
            return new MotionObjectInfo[] { newInfo };
        }

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            MotionObjectSingleSelectParameter first = args[0] as MotionObjectSingleSelectParameter;
            NumberParameter x = args[1] as NumberParameter;
            NumberParameter y = args[2] as NumberParameter;
            NumberParameter z = args[3] as NumberParameter;
            SingleSelectParameter unit = args[4] as SingleSelectParameter;

            bool useRatio = unit.Value == 0;
            MotionObjectInfo firstAxis = first.Value;
            MotionObjectInfo secondAxis = selectedInfoList.Where(info => info.IsTypeOf(typeof(LineObject)) && info != firstAxis).First();

            LineObject line1 = frame[firstAxis] as LineObject;
            LineObject line2 = frame[secondAxis] as LineObject;
            if(line1 == null || line2 == null)
                return new MotionObject[] { null };
            Vector3 anchor = line1.Position;
            Vector3 axis1 = line1.Edge;
            Vector3 axis2 = line2.Edge;
            Vector3 axis1norm = Vector3.Normalize(axis1);
            Vector3 axis2norm = Vector3.Normalize(axis2);
            Vector3 axis3 = Vector3.Cross(axis1norm, axis2norm) * (float)Math.Sqrt(axis1.Length() * axis2.Length());

            if(!useRatio) {
                axis1 = Vector3.Normalize(axis1);
                axis2 = Vector3.Normalize(axis2);
                axis3 = Vector3.Normalize(axis3);
            }
            List<MotionObject> ret = new List<MotionObject>();
            try {
                Vector3 edge1 = axis1 * (float)x.Value;
                Vector3 edge2 = axis2 * (float)y.Value;
                Vector3 edge3 = axis3 * (float)z.Value;
                PointObject point = new PointObject(anchor + edge1 + edge2 + edge3);
                ret.Add(point);
                if(previewMode) {
                    ret.Add(new BoxFrameObject(anchor, edge1, edge2, edge3));
                }
            } catch(ArithmeticException) { }
            return ret;
        }

        public Type CreatedType {
            get { return typeof(PointObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf<LineObject>();
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count != 2) {
                errorMessage = "二つの線分オブジェクトを選択してください";
                return false;
            }
            return true;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            MotionObjectSingleSelectParameter first = new MotionObjectSingleSelectParameter("First Axis", info => info.IsTypeOf<LineObject>(), true);
            NumberParameter x = new NumberParameter("First Axis Value", -10000, 10000, 3);
            NumberParameter y = new NumberParameter("Second Axis Value", -10000, 10000, 3);
            NumberParameter z = new NumberParameter("Third Axis Value", -10000, 10000, 3);
            SingleSelectParameter unit = new SingleSelectParameter("Unit", new string[] { "線分長", "単位長さ" });
            return new ProcParam<MotionProcEnv>[] { first, x, y, z, unit };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            MotionObjectSingleSelectParameter first = args[0] as MotionObjectSingleSelectParameter;
            NumberParameter x = args[1] as NumberParameter;
            NumberParameter y = args[2] as NumberParameter;
            NumberParameter z = args[3] as NumberParameter;
            SingleSelectParameter unit = args[4] as SingleSelectParameter;
            return true;
        }

        public string GetCommandName() {
            return "PointCoordinateByTwoLines";
        }

        public string GetTitle() {
            return "二線分の成す座標系内の点 / Point at Coordinate by two Lines";
        }

        public string GetDescription() {
            return "二つの線分とそれらに垂直な線分からなる座標系を作り，その座標系上の指定された位置に点オブジェクトを作成します．\r\n(二線分の始点から終点へのベクトルx, yと，xとyの外積からなるベクトルz (|z| = √|x×y|，右手系)，一方の線分の始点s，及び引数a, b, cから，p = s + ax + by + czとなるp点上に点オブジェクトを作成します)";
        }

        public Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.pointCoordinate; }
        }

        #endregion
    }

    public class OperationPointCoordinateByTwoLinesFromSequence : IMotionOperationCreateObject {

        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args) {
            MotionObjectSingleSelectParameter first = args[0] as MotionObjectSingleSelectParameter;

            MotionObjectInfo info1 = selectedInfoList[0];
            MotionObjectInfo info2 = selectedInfoList[1];

            MotionObjectInfo firstAxis = first.Value;
            MotionObjectInfo secondAxis = selectedInfoList.Where(info => info.IsTypeOf(typeof(LineObject)) && info != firstAxis).First();
            MotionObjectInfo newInfo = new MotionObjectInfo(typeof(PointObject), firstAxis);
            newInfo.Name = PathEx.GiveName("Coord", firstAxis.Name, secondAxis.Name);
            return new MotionObjectInfo[] { newInfo };
        }

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            MotionObjectSingleSelectParameter first = args[0] as MotionObjectSingleSelectParameter;
            SequenceSingleSelectParameter seq = args[1] as SequenceSingleSelectParameter;
            SequenceColumnSelectParameter column1 = args[2] as SequenceColumnSelectParameter;
            SequenceColumnSelectParameter column2 = args[3] as SequenceColumnSelectParameter;
            SequenceColumnSelectParameter column3 = args[4] as SequenceColumnSelectParameter;
            SingleSelectParameter unit = args[5] as SingleSelectParameter;

            bool useRatio = unit.Value == 0;
            MotionObjectInfo firstAxis = first.Value;
            MotionObjectInfo secondAxis = selectedInfoList.Where(info => info.IsTypeOf(typeof(LineObject)) && info != firstAxis).First();

            LineObject line1 = frame[firstAxis] as LineObject;
            LineObject line2 = frame[secondAxis] as LineObject;
            if(line1 == null || line2 == null)
                return new MotionObject[] { null };
            Vector3 anchor = line1.Position;
            Vector3 axis1 = line1.Edge;
            Vector3 axis2 = line2.Edge;
            Vector3 axis1norm = Vector3.Normalize(axis1);
            Vector3 axis2norm = Vector3.Normalize(axis2);
            Vector3 axis3 = Vector3.Cross(axis1norm, axis2norm) * (float)Math.Sqrt(axis1.Length() * axis2.Length());

            if(!useRatio) {
                axis1 = Vector3.Normalize(axis1);
                axis2 = Vector3.Normalize(axis2);
                axis3 = Vector3.Normalize(axis3);
            }
            List<MotionObject> ret = new List<MotionObject>();
            try {
                decimal?[] row = seq.Value.Values.GetValueAt(frame.Time);
                if(row != null && row[column1.Value].HasValue && row[column2.Value].HasValue && row[column3.Value].HasValue) {
                    float x = 0, y = 0, z = 0;
                    try {
                        x = (float)row[column1.Value].Value;
                        y = (float)row[column2.Value].Value;
                        z = (float)row[column3.Value].Value;
                    } catch(ArithmeticException) { }
                    Vector3 edge1 = axis1 * x;
                    Vector3 edge2 = axis2 * y;
                    Vector3 edge3 = axis3 * z;
                    PointObject point = new PointObject(anchor + edge1 + edge2 + edge3);
                    ret.Add(point);
                    if(previewMode) {
                        ret.Add(new BoxFrameObject(anchor, edge1, edge2, edge3));
                    }
                } else {
                    ret.Add(null);
                    if(previewMode) {
                        ret.Add(null);
                    }
                }
            } catch(ArithmeticException) { }
            return ret;
        }

        public Type CreatedType {
            get { return typeof(PointObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf<LineObject>();
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count != 2) {
                errorMessage = "二つの線分オブジェクトを選択してください";
                return false;
            }
            return true;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            MotionObjectSingleSelectParameter first = new MotionObjectSingleSelectParameter("First Axis", info => info.IsTypeOf<LineObject>(), true);
            SequenceSingleSelectParameter seq = new SequenceSingleSelectParameter("Source", v => (v.Type & MotionDataHandler.Sequence.SequenceType.Numeric) != 0);
            SequenceColumnSelectParameter column1 = new SequenceColumnSelectParameter("Source Column1", seq);
            SequenceColumnSelectParameter column2 = new SequenceColumnSelectParameter("Source Column2", seq);
            SequenceColumnSelectParameter column3 = new SequenceColumnSelectParameter("Source Column3", seq);
            seq.ValueChanged += (s, e) => {
                column1.Value = 0;
                column2.Value = 1;
                column3.Value = 2;
            };
            SingleSelectParameter unit = new SingleSelectParameter("Unit", new string[] { "線分長", "単位長さ" });

            return new ProcParam<MotionProcEnv>[] { first, seq, column1, column2, column3, unit };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            MotionObjectSingleSelectParameter first = args[0] as MotionObjectSingleSelectParameter;
            SequenceSingleSelectParameter seq = args[1] as SequenceSingleSelectParameter;
            SequenceColumnSelectParameter column1 = args[2] as SequenceColumnSelectParameter;
            SequenceColumnSelectParameter column2 = args[3] as SequenceColumnSelectParameter;
            SequenceColumnSelectParameter column3 = args[4] as SequenceColumnSelectParameter;
            SingleSelectParameter unit = args[5] as SingleSelectParameter;
            if(seq.Value == null) {
                errorMessage = "シーセンスが選択されていません";
                return false;
            }
            return true;
        }

        public string GetCommandName() {
            return "PointCoordinateByTwoLinesFromSequence";
        }

        public string GetTitle() {
            return "二線分の成す座標系内の点(時系列データから座標を設定) / Point at Coordinate by two Lines From Sequence";
        }

        public string GetDescription() {
            return "二つの線分とそれらに垂直な線分からなる座標系を作り，その座標系上の時系列データの値の位置に点オブジェクトを作成します．\r\n(二線分の始点から終点へのベクトルx, yと，xとyの外積からなるベクトルz (|z| = √|x×y|，右手系)，一方の線分の始点s，及び時系列データの値a, b, cから，p = s + ax + by + czとなるp点上に点オブジェクトを作成します)";
        }

        public Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.pointCoordinate; }
        }

        #endregion
    }

    public class OperationPointCoordinateByThreePoints : IMotionOperationCreateObject {

        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args) {
            MotionObjectSingleSelectParameter first = args[0] as MotionObjectSingleSelectParameter;
            MotionObjectSingleSelectParameter second = args[1] as MotionObjectSingleSelectParameter;

            MotionObjectInfo firstAxis = first.Value;
            MotionObjectInfo secondAxis = second.Value;
            MotionObjectInfo thirdAxis = selectedInfoList.Where(info => info.IsTypeOf(typeof(PointObject)) && info != firstAxis && info != secondAxis).First();
            MotionObjectInfo newInfo = new MotionObjectInfo(typeof(PointObject), firstAxis);
            newInfo.Name = PathEx.GiveName("Coord", firstAxis.Name, secondAxis.Name, thirdAxis.Name);
            return new MotionObjectInfo[] { newInfo };
        }

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            MotionObjectSingleSelectParameter first = args[0] as MotionObjectSingleSelectParameter;
            MotionObjectSingleSelectParameter second = args[1] as MotionObjectSingleSelectParameter;
            NumberParameter x = args[2] as NumberParameter;
            NumberParameter y = args[3] as NumberParameter;
            NumberParameter z = args[4] as NumberParameter;
            SingleSelectParameter unit = args[5] as SingleSelectParameter;

            bool useRatio = unit.Value == 0;
            MotionObjectInfo firstAxis = first.Value;
            MotionObjectInfo secondAxis = second.Value;
            MotionObjectInfo thirdAxis = selectedInfoList.Where(info => info.IsTypeOf(typeof(PointObject)) && info != firstAxis && info != secondAxis).First();

            PointObject point1 = frame[firstAxis] as PointObject;
            PointObject point2 = frame[secondAxis] as PointObject;
            PointObject point3 = frame[thirdAxis] as PointObject;
            if(point1 == null || point2 == null || point3 == null)
                return new MotionObject[] { null };
            Vector3 anchor = point1.Position;
            Vector3 axis1 = point2.Position - anchor;
            Vector3 axis2 = point3.Position - anchor;
            Vector3 axis1norm = Vector3.Normalize(axis1);
            Vector3 axis2norm = Vector3.Normalize(axis2);
            Vector3 axis3 = Vector3.Cross(axis1norm, axis2norm) * (float)Math.Sqrt(axis1.Length() * axis2.Length());

            if(!useRatio) {
                axis1 = Vector3.Normalize(axis1);
                axis2 = Vector3.Normalize(axis2);
                axis3 = Vector3.Normalize(axis3);
            }
            List<MotionObject> ret = new List<MotionObject>();
            try {
                Vector3 edge1 = axis1 * (float)x.Value;
                Vector3 edge2 = axis2 * (float)y.Value;
                Vector3 edge3 = axis3 * (float)z.Value;
                PointObject point = new PointObject(anchor + edge1 + edge2 + edge3);
                ret.Add(point);
                if(previewMode) {
                    ret.Add(new BoxFrameObject(anchor, edge1, edge2, edge3));
                }
            } catch(ArithmeticException) { }
            return ret;
        }

        public Type CreatedType {
            get { return typeof(PointObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf<PointObject>();
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count != 3) {
                errorMessage = "三つの点オブジェクトを選択してください";
                return false;
            }
            return true;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            MotionObjectSingleSelectParameter first = new MotionObjectSingleSelectParameter("First Anchor", info => info.IsTypeOf<PointObject>(), true);
            MotionObjectSingleSelectParameter second = new MotionObjectSingleSelectParameter("Second Anchor", info => info.IsTypeOf<PointObject>(), true);
            NumberParameter x = new NumberParameter("First Axis Value", -10000, 10000, 3);
            NumberParameter y = new NumberParameter("Second Axis Value", -10000, 10000, 3);
            NumberParameter z = new NumberParameter("Third Axis Value", -10000, 10000, 3);
            SingleSelectParameter unit = new SingleSelectParameter("Unit", new string[] { "線分長", "単位長さ" });
            return new ProcParam<MotionProcEnv>[] { first, second, x, y, z, unit };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            MotionObjectSingleSelectParameter first = args[0] as MotionObjectSingleSelectParameter;
            if(first.Value == null) {
                errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_CannotSpecifyNull + ": " + args[0].ParamName;
                return false;
            }
            MotionObjectSingleSelectParameter second = args[1] as MotionObjectSingleSelectParameter;
            if(second.Value == null) {
                errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_CannotSpecifyNull + ": " + args[1].ParamName;
                return false;
            }
            if(first.Value == second.Value) {
                errorMessage = "第一基準点と第二基準点に同じオブジェクトを指定できません";
                return false;
            }
            NumberParameter x = args[2] as NumberParameter;
            NumberParameter y = args[3] as NumberParameter;
            NumberParameter z = args[4] as NumberParameter;
            SingleSelectParameter unit = args[5] as SingleSelectParameter;
            return true;
        }

        public string GetCommandName() {
            return "PointCoordinateByThreePoints";
        }

        public string GetTitle() {
            return "三点の成す座標系内の点 / Point at Coordinate by three Points";
        }

        public string GetDescription() {
            return "三つの点オブジェクトから二つの線分を作り，それらに垂直な線分からなる座標系を作り，その座標系上の指定された位置に点オブジェクトを作成します．\r\n(三点の位置ベクトルs, t, u からベクトルx = t-s, y = u-sと，xとyの外積からなるベクトルz (|z| = √|x×y|，右手系)を求め，引数a, b, cから，p = s + ax + by + czとなるp点上に点オブジェクトを作成します)";
        }

        public Bitmap IconBitmap {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }

    public class OperationLineBetweenPoints : IMotionOperationCreateObject {
        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args) {
            MotionObjectSingleSelectParameter origination = args[0] as MotionObjectSingleSelectParameter;
            IList<MotionObjectInfo> targetInfoList = selectedInfoList.Where(info => info != origination.Value).ToList();
            List<MotionObjectInfo> ret = new List<MotionObjectInfo>();
            foreach(MotionObjectInfo info in targetInfoList) {
                MotionObjectInfo newInfo = new MotionObjectInfo(typeof(LineObject), info);
                newInfo.Name = PathEx.GiveName("LineTo", origination.Value.Name, info.Name);
                ret.Add(newInfo);
            }
            return ret;
        }

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            MotionObjectSingleSelectParameter origination = args[0] as MotionObjectSingleSelectParameter;
            IList<MotionObjectInfo> targetInfoList = selectedInfoList.Where(info => info != origination.Value).ToList();
            PointObject origin = frame[origination.Value] as PointObject;
            if(origin == null)
                return targetInfoList.Select(info => (MotionObject)null).ToList();
            List<MotionObject> ret = new List<MotionObject>();

            foreach(MotionObjectInfo info in targetInfoList) {
                MotionObject created = null;
                PointObject dest = frame[info] as PointObject;
                if(dest != null) {
                    created = new LineObject(origin.Position, dest.Position - origin.Position);
                }
                ret.Add(created);
            }
            return ret;
        }

        public Type CreatedType {
            get { return typeof(LineObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf<PointObject>();
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count >= 2)
                return true;
            errorMessage = "二つ以上の点オブジェクトを選択してください";
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            MotionObjectSingleSelectParameter origination = new MotionObjectSingleSelectParameter("Origination", info => info.IsTypeOf<PointObject>(), true);
            return new ProcParam<MotionProcEnv>[] { origination };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            MotionObjectSingleSelectParameter origination = args[0] as MotionObjectSingleSelectParameter;
            if(origination == null) {
                errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_CannotSpecifyNull + ": " + args[0].ParamName;
                return false;
            }
            return true;
        }

        public string GetCommandName() {
            return "LineBetweenPoints";
        }

        public string GetTitle() {
            return "二点を結ぶ線分 / Line between points";
        }

        public string GetDescription() {
            return "二つの点オブジェクトのあいた二線分オブジェクトを作成します";
        }

        public Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.linePoint; }
        }

        #endregion
    }

    public class OperationAxisLineOfCylinder : IMotionOperationCreateObject {
        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args) {
            List<MotionObjectInfo> ret = new List<MotionObjectInfo>();
            foreach(MotionObjectInfo info in selectedInfoList) {
                MotionObjectInfo newInfo = new MotionObjectInfo(typeof(LineObject), info);
                newInfo.Name = PathEx.GiveName("Axis", info.Name);
                ret.Add(newInfo);
            }
            return ret;
        }

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            List<MotionObject> ret = new List<MotionObject>();
            foreach(MotionObjectInfo info in selectedInfoList) {
                MotionObject created = null;
                CylinderObject cylinder = frame[info] as CylinderObject;
                if(cylinder != null) {
                    created = new LineObject(cylinder.Position, cylinder.Axis);
                }
                ret.Add(created);
            }
            return ret;
        }

        public Type CreatedType {
            get { return typeof(LineObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf<CylinderObject>();
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count > 0)
                return true;
            errorMessage = "円筒オブジェクトを選択してください";
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            return null;
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            return true;
        }

        public string GetCommandName() {
            return "AxisOfCylinder";
        }

        public string GetTitle() {
            return "円筒の中心軸 / Axis Line of Cylinder";
        }

        public string GetDescription() {
            return "円筒オブジェクトの中心軸として線分オブジェクトを作成します";
        }

        public Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.cylinderLine; }
        }

        #endregion
    }
    public class OperationLineBetweenPointAndNearestObject : IMotionOperationCreateObject {
        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args) {
            MotionObjectSingleSelectParameter main = args[0] as MotionObjectSingleSelectParameter;
            MotionObjectInfo newInfo = new MotionObjectInfo(typeof(LineObject), main.Value);
            return new MotionObjectInfo[] { newInfo };
        }

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            MotionObjectSingleSelectParameter main = args[0] as MotionObjectSingleSelectParameter;
            PointObject origin = frame[main.Value] as PointObject;
            if(origin == null)
                return new MotionObject[1];
            List<MotionObjectInfo> targetInfoList = selectedInfoList.Where(info => info != main.Value).ToList();
            Vector3 point = origin.Position;
            float minDistace = float.MaxValue;
            foreach(MotionObjectInfo info in targetInfoList) {
                MotionObject @object = frame[info];
                if(@object == null)
                    return new MotionObject[1];
                Vector3 to = @object.GetNearestFrom(origin.Position);
                float distance = (to - origin.Position).LengthSq();
                if(distance < minDistace) {
                    point = to;
                    minDistace = distance;
                }
            }
            if(minDistace != float.MaxValue) {
                return new MotionObject[] { new LineObject(origin.Position, point - origin.Position) };
            }
            return new MotionObject[1];
        }

        public Type CreatedType {
            get { return typeof(LineObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return true;
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Any(info => info.IsTypeOf<PointObject>()) && infoList.Count >= 2) {
                return true;
            }
            errorMessage = "点オブジェクトを含む，二つ以上のオブジェクトを選択してください";
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            MotionObjectSingleSelectParameter main = new MotionObjectSingleSelectParameter("Origination", info => info.IsTypeOf<PointObject>(), true);
            return new ProcParam<MotionProcEnv>[] { main };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            MotionObjectSingleSelectParameter main = args[0] as MotionObjectSingleSelectParameter;
            if(main.Value == null) {
                errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_CannotSpecifyNull + ": " + main.ParamName;
                return false;
            }
            return true;
        }

        public string GetCommandName() {
            return "LineBetweenPointAndNearest";
        }

        public string GetTitle() {
            return "点と点からの最近点を結ぶ線分 / Line between Point and Nearest Object";
        }

        public string GetDescription() {
            return "基準となる点オブジェクトから，他のオブジェクトの中で一番近い位置までを結ぶ線分オブジェクトを作成します";
        }

        public Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.lineNearest; }
        }

        #endregion
    }

    public class OperationLineBetweenClosestPointsOfTwoLines : IMotionOperationCreateObject {
        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args) {
            MotionObjectSingleSelectParameter origination = args[0] as MotionObjectSingleSelectParameter;
            MotionObjectInfo destination = selectedInfoList.Where(info => info != origination.Value).First();
            MotionObjectInfo newInfo = new MotionObjectInfo(typeof(LineObject), origination.Value);
            return new MotionObjectInfo[] { newInfo };
        }

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            MotionObjectSingleSelectParameter origination = args[0] as MotionObjectSingleSelectParameter;
            MotionObjectInfo destination = selectedInfoList.Where(info => info != origination.Value).First();
            LineObject origin = frame[origination.Value] as LineObject;
            LineObject dest = frame[destination] as LineObject;
            if(origin == null || dest == null)
                return new MotionObject[1];
            Vector3 end1, end2;
            if(GeometryCalc.GetNearestPointsInLinesWithEdges(origin.Position, origin.Edge, dest.Position, dest.Edge, out end1, out end2)) {
                return new MotionObject[] { new LineObject(end1, end2 - end1) };
            }
            return new MotionObject[1];
        }

        public Type CreatedType {
            get { return typeof(LineObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf<LineObject>();
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count == 2)
                return true;
            errorMessage = "二つの線分オブジェクトを選択してください";
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            MotionObjectSingleSelectParameter origination = new MotionObjectSingleSelectParameter("Origination", info => info.IsTypeOf<LineObject>(), true);
            return new ProcParam<MotionProcEnv>[] { origination };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            MotionObjectSingleSelectParameter origination = args[0] as MotionObjectSingleSelectParameter;
            if(origination.Value == null) {
                errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_CannotSpecifyNull + ": " + origination.ParamName;
                return false;
            }
            return true;
        }

        public string GetCommandName() {
            return "LineBetweenClosestPointsOfTwoLines";
        }

        public string GetTitle() {
            return "二線分の最も近づく二点間を結ぶ線分 / Line between closest points of two Lines";
        }

        public string GetDescription() {
            return "二つの線分オブジェクト上の二点を結ぶ上で，最も長さが短くなるような二点間を結ぶ線分オブジェクトを作成します";
        }

        public Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.lineClosest; }
        }

        #endregion
    }

    public class OperationLineTranslateToPoint : IMotionOperationCreateObject {
        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args) {
            MotionObjectSingleSelectParameter anchor = args[0] as MotionObjectSingleSelectParameter;
            SingleSelectParameter alignment = args[1] as SingleSelectParameter;
            List<MotionObjectInfo> targetInfoList = selectedInfoList.Where(info => info.IsTypeOf<LineObject>()).ToList();

            List<MotionObjectInfo> ret = new List<MotionObjectInfo>();
            foreach(MotionObjectInfo info in targetInfoList) {
                MotionObjectInfo newInfo = new MotionObjectInfo(typeof(LineObject), info);
                newInfo.Name = PathEx.GiveName("TransTo", info.Name, anchor.Value.Name);
                ret.Add(newInfo);
            }
            return ret;
        }

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            MotionObjectSingleSelectParameter anchor = args[0] as MotionObjectSingleSelectParameter;
            SingleSelectParameter alignment = args[1] as SingleSelectParameter;
            bool orig = alignment.Value == 0;
            List<MotionObjectInfo> targetInfoList = selectedInfoList.Where(info => info.IsTypeOf<LineObject>()).ToList();

            List<MotionObject> ret = new List<MotionObject>();
            PointObject point = frame[anchor.Value] as PointObject;
            if(point == null)
                return new MotionObject[targetInfoList.Count];
            foreach(MotionObjectInfo info in targetInfoList) {
                MotionObject created = null;
                LineObject line = frame[info] as LineObject;
                if(line != null) {
                    if(orig) {
                        created = new LineObject(point.Position, line.Edge);
                    } else {
                        created = new LineObject(point.Position - line.Edge, line.Edge);
                    }
                }
                ret.Add(created);
            }
            return ret;
        }

        public Type CreatedType {
            get { return typeof(LineObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf<LineObject>() || info.IsTypeOf<PointObject>();
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count >= 2 && infoList.Any(info => info.IsTypeOf<PointObject>()) && infoList.Any(info => info.IsTypeOf<LineObject>()))
                return true;
            errorMessage = "一つ以上の線分オブジェクトと，一つの点オブジェクトを選択してください";
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            MotionObjectSingleSelectParameter anchor = new MotionObjectSingleSelectParameter("Anchor Point", info => info.IsTypeOf<PointObject>(), true);
            SingleSelectParameter alignment = new SingleSelectParameter("Alignment", new string[] { "始点", "終点" });
            return new ProcParam<MotionProcEnv>[] { anchor, alignment };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            MotionObjectSingleSelectParameter anchor = args[0] as MotionObjectSingleSelectParameter;
            if(anchor.Value == null) {
                errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_CannotSpecifyNull + ": " + anchor.ParamName;
                return false;
            }
            SingleSelectParameter alignment = args[1] as SingleSelectParameter;
            return true;
        }

        public string GetCommandName() {
            return "TranslateLineToPoint";
        }

        public string GetTitle() {
            return "線分を点上へ平行移動 / Translate Line to Point";
        }

        public string GetDescription() {
            return "線分オブジェクトの始点または終点が指定された点オブジェクトの位置にくるように平行移動します";
        }

        public Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.lineTranslate; }
        }

        #endregion
    }

    public class OperationAddLines : IMotionOperationCreateObject {

        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args) {
            MotionObjectSingleSelectParameter origination = args[0] as MotionObjectSingleSelectParameter;
            List<MotionObjectInfo> subInfoList = selectedInfoList.Where(info => info != origination.Value).ToList();
            List<MotionObjectInfo> reorderedList = new List<MotionObjectInfo>();
            reorderedList.Add(origination.Value);
            reorderedList.AddRange(subInfoList);

            MotionObjectInfo newInfo = new MotionObjectInfo(typeof(LineObject), origination.Value);
            newInfo.Name = PathEx.GiveName("Sum", reorderedList.Select(i => i.Name));
            return new MotionObjectInfo[] { newInfo };
        }

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            MotionObjectSingleSelectParameter origination = args[0] as MotionObjectSingleSelectParameter;
            List<MotionObjectInfo> subInfoList = selectedInfoList.Where(info => info != origination.Value).ToList();

            LineObject origin = frame[origination.Value] as LineObject;
            if(origin == null)
                return new MotionObject[1];
            Vector3 edge = origin.Edge;
            foreach(MotionObjectInfo info in subInfoList) {
                LineObject line = frame[info] as LineObject;
                if(line == null)
                    return new MotionObject[1];
                edge += line.Edge;
            }
            MotionObject created = new LineObject(origin.Position, edge);
            return new MotionObject[] { created };
        }

        public Type CreatedType {
            get { return typeof(LineObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf<LineObject>();
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count > 0)
                return true;
            errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_PleaseSelectLineObject;
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            MotionObjectSingleSelectParameter origination = new MotionObjectSingleSelectParameter("Origination", info => info.IsTypeOf<LineObject>(), true);
            return new ProcParam<MotionProcEnv>[] { origination };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            MotionObjectSingleSelectParameter origination = args[0] as MotionObjectSingleSelectParameter;
            if(origination == null) {
                errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_CannotSpecifyNull + ": " + origination.ParamName;
                return false;
            }
            return true;
        }

        public string GetCommandName() {
            return "AddLineVector";
        }

        public string GetTitle() {
            return "線分のベクトルの和 / Add Line Vectors";
        }

        public string GetDescription() {
            return "線分オブジェクトのエッジベクトルの和をエッジベクトルとする線分オブジェクトを作成します";
        }

        public Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.lineAdd; }
        }

        #endregion
    }

    public class OperationCylinderAroundLineFromSequence : IMotionOperationCreateObject {
        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args) {
            SequenceSingleSelectParameter sequence = args[0] as SequenceSingleSelectParameter;
            SequenceColumnSelectParameter column = args[1] as SequenceColumnSelectParameter;
            List<MotionObjectInfo> ret = new List<MotionObjectInfo>();
            foreach(MotionObjectInfo info in selectedInfoList) {
                MotionObjectInfo newInfo = new MotionObjectInfo(typeof(CylinderObject), info);
                newInfo.Name = PathEx.GiveName("Around", info.Name);
                ret.Add(newInfo);
            }
            return ret;
        }

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            SequenceSingleSelectParameter sequence = args[0] as SequenceSingleSelectParameter;
            SequenceColumnSelectParameter column = args[1] as SequenceColumnSelectParameter;
            List<MotionObject> ret = new List<MotionObject>();
            foreach(MotionObjectInfo info in selectedInfoList) {
                MotionObject created = null;
                LineObject line = frame[info] as LineObject;
                if(line != null) {
                    decimal?[] row = sequence.Value.Values.GetValueAt(frame.Time);
                    decimal? value = row[column.Value];
                    if(value.HasValue) {
                        try {
                            created = new CylinderObject(line.Position, line.Edge, (float)value.Value);
                        } catch(ArithmeticException) { }
                    }
                }
                ret.Add(created);
            }
            return ret;
        }

        public Type CreatedType {
            get { return typeof(CylinderObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf<LineObject>();
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count > 0)
                return true;
            errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_PleaseSelectLineObject;
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            SequenceSingleSelectParameter sequence = new SequenceSingleSelectParameter("Time-Series data For radius", view => (view.Type & MotionDataHandler.Sequence.SequenceType.Numeric) != 0);
            SequenceColumnSelectParameter column = new SequenceColumnSelectParameter("Column", sequence);
            return new ProcParam<MotionProcEnv>[] { sequence, column };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            SequenceSingleSelectParameter sequence = args[0] as SequenceSingleSelectParameter;
            if(sequence.Value == null) {
                errorMessage = "シーケンス名を指定してください";
                return false;
            }
            SequenceColumnSelectParameter column = args[1] as SequenceColumnSelectParameter;
            return true;
        }

        public string GetCommandName() {
            return "CylinderAroundLineFromSequence";
        }

        public string GetTitle() {
            return "線分を軸とした円筒(時系列データから半径を設定) / Cylinder around Line with Radius from Sequence";
        }

        public string GetDescription() {
            return "線分オブジェクトを軸とし，時系列データの対応する値を半径とする円筒オブジェクトを作成します";
        }

        public Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.cylinderLine; }
        }

        #endregion
    }
    public class OperationCylinderAroundLine : IMotionOperationCreateObject {
        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args) {
            List<MotionObjectInfo> ret = new List<MotionObjectInfo>();
            foreach(MotionObjectInfo info in selectedInfoList) {
                MotionObjectInfo newInfo = new MotionObjectInfo(typeof(CylinderObject), info);
                newInfo.Name = PathEx.GiveName("Around", info.Name);
                ret.Add(newInfo);
            }
            return ret;
        }

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            NumberParameter radius = args[0] as NumberParameter;

            List<MotionObject> ret = new List<MotionObject>();
            foreach(MotionObjectInfo info in selectedInfoList) {
                MotionObject created = null;
                LineObject line = frame[info] as LineObject;
                if(line != null) {
                    try {
                        created = new CylinderObject(line.Position, line.Edge, (float)radius.Value);
                    } catch(ArithmeticException) { }
                }
                ret.Add(created);
            }
            return ret;
        }

        public Type CreatedType {
            get { return typeof(CylinderObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf<LineObject>();
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count > 0)
                return true;
            errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_PleaseSelectLineObject;
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            NumberParameter radius = new NumberParameter("Radius", 0, 100000, 3);
            return new ProcParam<MotionProcEnv>[] { radius };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            NumberParameter radius = args[0] as NumberParameter;
            return true;
        }

        public string GetCommandName() {
            return "CylinderAroundLine";
        }

        public string GetTitle() {
            return "線分を軸とした円筒 / Cylinder around Line";
        }

        public string GetDescription() {
            return "線分オブジェクトを軸とした半径固定の円筒オブジェクトを作成します";
        }

        public Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.cylinderLine; }
        }

        #endregion
    }

    public class OperationSphereContainingPoints : IMotionOperationCreateObject {
        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args) {
            MotionObjectInfo newInfo = new MotionObjectInfo(typeof(SphereObject), selectedInfoList[0]);
            newInfo.Name = PathEx.GiveName("Contain", selectedInfoList.Select(i => i.Name));
            return new MotionObjectInfo[] { newInfo };
        }

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            SphereObject created = null;
            List<Vector3> points = new List<Vector3>();
            foreach(var info in selectedInfoList) {
                PointObject point = frame[info] as PointObject;
                if(point == null)
                    return new MotionObject[1];

                points.Add(point.Position);
            }
            Vector3 center;
            float radius;
            if(GeometryCalc.GetContainingSphere(out center, out radius, points.ToArray())) {
                created = new SphereObject(center, radius);
            }
            return new MotionObject[] { created };
        }

        public Type CreatedType {
            get { return typeof(SphereObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf<PointObject>();
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count >= 2)
                return true;
            errorMessage = "二つ以上の点オブジェクトを選択してください";
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            return null;
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            return true;
        }

        public string GetCommandName() {
            return "SphereContainingPoints";
        }

        public string GetTitle() {
            return "点群を含む最小の球 / Sphere Containing Points";
        }

        public string GetDescription() {
            return "指定された点オブジェクトをすべて含むような最小の球オブジェクトを作成します";
        }

        public Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.sphereContain; }
        }

        #endregion
    }

    public class OperationSphereContacting4Points : IMotionOperationCreateObject {
        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args) {
            MotionObjectInfo newInfo = new MotionObjectInfo(typeof(SphereObject), selectedInfoList[0]);
            newInfo.Name = PathEx.GiveName("Contact", selectedInfoList.Select(i => i.Name));
            return new MotionObjectInfo[] { newInfo };
        }

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            SphereObject created = null;
            List<Vector3> points = new List<Vector3>();
            foreach(var info in selectedInfoList) {
                PointObject point = frame[info] as PointObject;
                if(point == null)
                    return new MotionObject[1];

                points.Add(point.Position);
            }

            if(points.Count >= 4) {
                Vector3 center;
                if(GeometryCalc.GetCircumscribedSphere(points[0], points[1], points[2], points[3], out center)) {
                    float radius = 0;
                    foreach(Vector3 point in points) {
                        float radiusCandidate = (center - point).Length();
                        if(radiusCandidate > radius)
                            radius = radiusCandidate;
                    }
                    created = new SphereObject(center, radius);
                }
            }
            return new MotionObject[] { created };
        }

        public Type CreatedType {
            get { return typeof(SphereObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf<PointObject>();
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count == 4)
                return true;
            errorMessage = "4つの点オブジェクトを選択してください";
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            return null;
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            return true;
        }

        public string GetCommandName() {
            return "SphereContact4Points";
        }

        public string GetTitle() {
            return "4点に接する球 / Sphere Constacting 4 Points";
        }

        public string GetDescription() {
            return "指定された点オブジェクトに接する球オブジェクトを作成します";
        }

        public Bitmap IconBitmap {
            get { return null; }
        }

        #endregion
    }

    public class OperationCylinderToFloowContainingPoints : IMotionOperationCreateObject {
        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args) {
            MotionObjectInfo newInfo = new MotionObjectInfo(typeof(CylinderObject), selectedInfoList[0]);
            newInfo.Name = PathEx.GiveName("ContainToFloor", selectedInfoList.Select(i => i.Name));
            return new MotionObjectInfo[] { newInfo };
        }

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            MotionObject created = null;
            List<Vector3> points = new List<Vector3>();
            foreach(var info in selectedInfoList) {
                PointObject point = frame[info] as PointObject;
                if(point == null)
                    return new MotionObject[] { null };

                points.Add(point.Position);
            }
            Vector3 center;
            float radius;
            if(GeometryCalc.GetContainingSphere(out center, out radius, points.ToArray())) {
                created = new CylinderObject(center, new Vector3(0, -center.Y, 0), radius);
            }
            return new MotionObject[] { created };
        }

        public Type CreatedType {
            get { return typeof(CylinderObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf<PointObject>();
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count >= 2)
                return true;
            errorMessage = "二つ以上の点オブジェクトを選択してください";
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            return null;
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            return true;
        }

        public string GetCommandName() {
            return "CylinderToFloorContainingPoints";
        }

        public string GetTitle() {
            return "点群を含む最小の球を基とする床までの円筒 / Cylinder to Floor Containing Points";
        }

        public string GetDescription() {
            return "指定された点オブジェクトをすべて含むような最小の球を求め，その中心点から床へ下ろした垂線を軸とし，球の半径を半径とする円筒を作成します";
        }

        public Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.cylinderContain; }
        }

        #endregion
    }

    public class OperationPlaneOverPoints : IMotionOperationCreateObject {
        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args) {
            MotionObjectInfo newInfo = new MotionObjectInfo(typeof(PlaneObject), selectedInfoList[0]);
            newInfo.Name = PathEx.GiveName("Polygon", selectedInfoList.Select(i => i.Name));
            return new MotionObjectInfo[] { newInfo };
        }

        IList<MotionObjectInfo> _prevInfoList = null;
        IList<MotionObjectInfo> _order;

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            MotionObject created = null;
            if(selectedInfoList.All(info => frame[info] != null)) {
                if(_order == null || _prevInfoList != selectedInfoList) {
                    IList<uint> idOrder = GeometryCalc.OptimizePlaneOrder(selectedInfoList, frame);
                    _order = new List<MotionObjectInfo>();
                    foreach(uint id in idOrder) {
                        _order.Add(selectedInfoList.Where(info => info.Id == id).First());
                    }
                    _prevInfoList = selectedInfoList;
                }
                created = new PlaneObject(_order.Select(info => frame[info].GravityPoint));
            }
            return new MotionObject[] { created };
        }

        public Type CreatedType {
            get { return typeof(PlaneObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf<PointObject>();
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count >= 3)
                return true;
            errorMessage = "三つ以上の点オブジェクトを選択してください";
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            return null;
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            return true;
        }

        public string GetCommandName() {
            return "PlaneOverPoints";
        }

        public string GetTitle() {
            return "点群を覆う平面 / Plane over Points";
        }

        public string GetDescription() {
            return "指定された点オブジェクトの位置を頂点とする多角形平面オブジェクトを作成します";
        }

        public Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.planePoint; }
        }

        #endregion
    }

    public class OperationLineFromPoint : IMotionOperationCreateObject {
        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args) {
            List<MotionObjectInfo> ret = new List<MotionObjectInfo>();
            foreach(MotionObjectInfo info in selectedInfoList) {
                MotionObjectInfo newInfo = new MotionObjectInfo(typeof(LineObject), info);
                newInfo.Name = PathEx.GiveName("From", info.Name);
                ret.Add(newInfo);
            }
            return ret;
        }

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            NumberParameter x = args[0] as NumberParameter;
            NumberParameter y = args[1] as NumberParameter;
            NumberParameter z = args[2] as NumberParameter;
            float dx, dy, dz;
            try {
                dx = (float)x.Value;
                dy = (float)y.Value;
                dz = (float)z.Value;
            } catch(ArithmeticException) {
                return selectedInfoList.Select(info => (MotionObject)null).ToList();
            }
            List<MotionObject> ret = new List<MotionObject>();
            foreach(MotionObjectInfo info in selectedInfoList) {
                MotionObject created = null;
                PointObject point = frame[info] as PointObject;
                if(point != null) {
                    created = new LineObject(point.Position, new Vector3(dx, dy, dz));
                }
                ret.Add(created);
            }
            return ret;
        }

        public Type CreatedType {
            get { return typeof(LineObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf<PointObject>();
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count > 0)
                return true;
            errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_PleaseSelectPointObject;
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            NumberParameter x = new NumberParameter("Edge X", -100000, 100000, 3, 1);
            NumberParameter y = new NumberParameter("Edge Y", -100000, 100000, 3, 1);
            NumberParameter z = new NumberParameter("Edge Z", -100000, 100000, 3, 1);
            return new ProcParam<MotionProcEnv>[] { x, y, z };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            NumberParameter x = args[0] as NumberParameter;
            NumberParameter y = args[1] as NumberParameter;
            NumberParameter z = args[2] as NumberParameter;
            return true;
        }

        public string GetCommandName() {
            return "FixedLineFromPoint";
        }

        public string GetTitle() {
            return "点を起点とする固定長線分 / Fixed Line from Point";
        }

        public string GetDescription() {
            return "点オブジェクトを起点とし，指定されたサイズの枝を持つ線分オブジェクトを作成します";
        }

        public Bitmap IconBitmap {
            get { return null; }
        }

        #endregion
    }
    public class OperationLineFromPointFromSequence : IMotionOperationCreateObject {
        #region IMotionOperationCreateObject メンバ

        public IList<MotionObjectInfo> GetNewObjectInfoList(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args) {
            List<MotionObjectInfo> ret = new List<MotionObjectInfo>();
            foreach(MotionObjectInfo info in selectedInfoList) {
                MotionObjectInfo newInfo = new MotionObjectInfo(typeof(LineObject), info);
                newInfo.Name = PathEx.GiveName("From", info.Name);
                ret.Add(newInfo);
            }
            return ret;
        }

        public IList<MotionObject> CreateObjects(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            SequenceSingleSelectParameter seq = args[0] as SequenceSingleSelectParameter;
            SequenceColumnSelectParameter columnX = args[1] as SequenceColumnSelectParameter;
            SequenceColumnSelectParameter columnY = args[2] as SequenceColumnSelectParameter;
            SequenceColumnSelectParameter columnZ = args[3] as SequenceColumnSelectParameter;
            float dx, dy, dz;
            decimal?[] row = seq.Value.Values.GetValueAt(frame.Time);
            if(row != null && row[columnX.Value].HasValue && row[columnY.Value].HasValue && row[columnZ.Value].HasValue) {
                try {
                    dx = (float)row[columnX.Value].Value;
                    dy = (float)row[columnY.Value].Value;
                    dz = (float)row[columnZ.Value].Value;
                } catch(ArithmeticException) {
                    return selectedInfoList.Select(info => (MotionObject)null).ToList();
                }
            } else {
                return selectedInfoList.Select(info => (MotionObject)null).ToList();
            }
            List<MotionObject> ret = new List<MotionObject>();
            foreach(MotionObjectInfo info in selectedInfoList) {
                MotionObject created = null;
                PointObject point = frame[info] as PointObject;
                if(point != null) {
                    created = new LineObject(point.Position, new Vector3(dx, dy, dz));
                }
                ret.Add(created);
            }
            return ret;
        }

        public Type CreatedType {
            get { return typeof(LineObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf<PointObject>();
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count > 0)
                return true;
            errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_PleaseSelectPointObject;
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            SequenceSingleSelectParameter seq = new SequenceSingleSelectParameter("Source", v => (v.Type & MotionDataHandler.Sequence.SequenceType.Numeric) != 0);
            SequenceColumnSelectParameter columnX = new SequenceColumnSelectParameter("Source Column X", seq);
            SequenceColumnSelectParameter columnY = new SequenceColumnSelectParameter("Source Column Y", seq);
            SequenceColumnSelectParameter columnZ = new SequenceColumnSelectParameter("Source Column X", seq);
            return new ProcParam<MotionProcEnv>[] { seq, columnX, columnY, columnZ };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            SequenceSingleSelectParameter seq = args[0] as SequenceSingleSelectParameter;
            SequenceColumnSelectParameter columnX = args[1] as SequenceColumnSelectParameter;
            SequenceColumnSelectParameter columnY = args[2] as SequenceColumnSelectParameter;
            SequenceColumnSelectParameter columnZ = args[3] as SequenceColumnSelectParameter;
            if(seq.Value == null) {
                errorMessage = "シーセンスが選択されていません";
                return false;
            }
            return true;
        }

        public string GetCommandName() {
            return "FixedLineFromPointFromSequence";
        }

        public string GetTitle() {
            return "点を起点とする線分(時系列データを枝サイズとする) / Fixed Line from Point from Sequence";
        }

        public string GetDescription() {
            return "点オブジェクトを起点とし，時系列データの値のサイズの枝を持つ線分オブジェクトを作成します";
        }

        public Bitmap IconBitmap {
            get { return null; }
        }

        #endregion
    }

}
