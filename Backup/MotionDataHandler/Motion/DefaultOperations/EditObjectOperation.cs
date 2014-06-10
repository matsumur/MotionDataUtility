using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionDataHandler.Motion.DefaultOperations {
    // IMotionOperationEditObjectを実装するファイル

    using Operation;
    using Script;
    using Sequence;
    using Misc;
    public class OperationOffsetObject : IMotionOperationEditObject {
        #region IMotionOperationEditObject メンバ

        public IList<MotionObject> EditObject(IList<MotionObjectInfo> targetInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            NumberParameter x = args[0] as NumberParameter;
            NumberParameter y = args[1] as NumberParameter;
            NumberParameter z = args[2] as NumberParameter;
            List<MotionObject> ret = new List<MotionObject>();
            foreach(MotionObjectInfo info in targetInfoList) {
                MotionObject obj = frame[info];
                if(obj == null) {
                    ret.Add(null);
                } else {
                    ret.Add(obj.CloneOffsetObject(new Microsoft.DirectX.Vector3((float)x.Value, (float)y.Value, (float)z.Value)));
                }
            }
            return ret;
        }
        public Type CreatedType { get { return null; } }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return true;
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count > 0)
                return true;
            errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_PleaseSelectObject;
            return false;
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
            return "OffsetPosition";
        }

        public string GetTitle() {
            return "平行移動 / offset position";
        }

        public string GetDescription() {
            return "オブジェクトをすべてのフレームで一定量平行移動します";
        }

        public System.Drawing.Bitmap IconBitmap {
            get { return null; }
        }

        #endregion
    }

    public class OperationReverseLineDirection : IMotionOperationEditObject {

        #region IMotionOperationEditObject メンバ

        public IList<MotionObject> EditObject(IList<MotionObjectInfo> targetInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            SingleSelectParameter mode = args[0] as SingleSelectParameter;
            List<MotionObject> ret = new List<MotionObject>();
            foreach(MotionObjectInfo info in targetInfoList) {
                LineObject line = frame[info] as LineObject;
                if(line != null) {
                    switch(mode.Value) {
                    case 0:
                        ret.Add(new LineObject(line.Position + line.Edge, -line.Edge));
                        break;
                    case 1:
                        ret.Add(new LineObject(line.Position, -line.Edge));
                        break;
                    default:
                        ret.Add((LineObject)line.Clone());
                        break;
                    }
                } else {
                    ret.Add(null);
                }
            }
            return ret;
        }

        public Type CreatedType {
            get { return typeof(LineObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf(typeof(LineObject));
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count > 0)
                return true;
            errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_PleaseSelectLineObject;
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            SingleSelectParameter mode = new SingleSelectParameter("Mode", new string[] { "始点と終点を反転", "終点の向きを反転" });
            return new ProcParam<MotionProcEnv>[] { mode };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            SingleSelectParameter mode = args[0] as SingleSelectParameter;
            return true;
        }

        public string GetCommandName() {
            return "ReverseLineDirction";
        }

        public string GetTitle() {
            return "線分の向きを反転 / Reverse Line Direction";
        }

        public string GetDescription() {
            return "選択された線分オブジェクトの向きを反転します";
        }

        public System.Drawing.Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.reverse; }
        }

        #endregion
    }

    public class OperationResizeSphere : IMotionOperationEditObject {

        #region IMotionOperationEditObject メンバ

        public IList<MotionObject> EditObject(IList<MotionObjectInfo> targetInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            SingleSelectParameter mode = args[0] as SingleSelectParameter;
            NumberParameter value = args[1] as NumberParameter;
            List<MotionObject> ret = new List<MotionObject>();
            foreach(MotionObjectInfo info in targetInfoList) {
                SphereObject sphere = frame[info] as SphereObject;
                if(sphere != null) {
                    float radius = sphere.Radius;
                    try {
                        switch(mode.Value) {
                        case 0:
                            radius *= (float)value.Value;
                            break;
                        case 1:
                            radius = (float)value.Value;
                            break;
                        case 2:
                            radius += (float)value.Value;
                            break;
                        }
                    } catch(ArithmeticException) { }
                    ret.Add(new SphereObject(sphere.Position, radius));
                } else {
                    ret.Add(null);
                }
            }
            return ret;
        }

        public Type CreatedType {
            get { return typeof(SphereObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf(typeof(SphereObject));
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count > 0)
                return true;
            errorMessage = "球オブジェクト選択してください";
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            SingleSelectParameter mode = new SingleSelectParameter("Mode", new string[] { "倍率", "半径指定", "半径加算" });
            NumberParameter value = new NumberParameter("Value", -10000, 10000, 3);
            value.Value = 1;
            return new ProcParam<MotionProcEnv>[] { mode, value };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            SingleSelectParameter mode = args[0] as SingleSelectParameter;
            NumberParameter value = args[1] as NumberParameter;
            if(mode.Value == 0 || mode.Value == 1) {
                if(value.Value < 0) {
                    errorMessage = "負のサイズを指定できません";
                    return false;
                }
            }
            return true;
        }

        public string GetCommandName() {
            return "ResizeSphere";
        }

        public string GetTitle() {
            return "球のサイズ変更 / Resize Sphere";
        }

        public string GetDescription() {
            return "球オブジェクトの半径を拡大または縮小します";
        }

        public System.Drawing.Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.pointSphere; }
        }

        #endregion
    }
    public class OperationResizeCylinder : IMotionOperationEditObject {

        #region IMotionOperationEditObject メンバ

        public IList<MotionObject> EditObject(IList<MotionObjectInfo> targetInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            SingleSelectParameter modeRad = args[0] as SingleSelectParameter;
            NumberParameter valueRad = args[1] as NumberParameter;
            SingleSelectParameter modeAxis = args[2] as SingleSelectParameter;
            NumberParameter valueAxis = args[3] as NumberParameter;
            List<MotionObject> ret = new List<MotionObject>();
            foreach(MotionObjectInfo info in targetInfoList) {
                CylinderObject cylinder = frame[info] as CylinderObject;
                if(cylinder != null) {
                    float radius = cylinder.Radius;
                    float axis = cylinder.AxisLength();
                    try {
                        switch(modeRad.Value) {
                        case 0:
                            radius *= (float)valueRad.Value;
                            break;
                        case 1:
                            radius = (float)valueRad.Value;
                            break;
                        case 2:
                            radius += (float)valueRad.Value;
                            break;
                        }
                    } catch(ArithmeticException) { }
                    try {
                        switch(modeAxis.Value) {
                        case 0:
                            axis *= (float)valueAxis.Value;
                            break;
                        case 1:
                            axis = (float)valueAxis.Value;
                            break;
                        case 2:
                            axis += (float)valueAxis.Value;
                            break;
                        }
                    } catch(ArithmeticException) { }
                    ret.Add(new CylinderObject(cylinder.Position, cylinder.AxisDirection() * axis, radius));
                } else {
                    ret.Add(null);
                }

            }
            return ret;
        }

        public Type CreatedType {
            get { return typeof(CylinderObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf(typeof(CylinderObject));
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count > 0)
                return true;
            errorMessage = "円筒オブジェクト選択してください";
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            SingleSelectParameter modeRad = new SingleSelectParameter("Mode for Radius", new string[] { "倍率", "半径指定", "半径加算" });
            NumberParameter valueRad = new NumberParameter("Value for Radius", -10000, 10000, 3);
            valueRad.Value = 1;
            SingleSelectParameter modeAxis = new SingleSelectParameter("Mode for Thickness", new string[] { "倍率", "厚さ指定", "厚さ加算" });
            NumberParameter valueAxis = new NumberParameter("Value for Thickness", -10000, 10000, 3);
            valueAxis.Value = 1;
            return new ProcParam<MotionProcEnv>[] { modeRad, valueRad, modeAxis, valueAxis };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            SingleSelectParameter modeRad = args[0] as SingleSelectParameter;
            NumberParameter valueRad = args[1] as NumberParameter;
            SingleSelectParameter modeAxis = args[2] as SingleSelectParameter;
            NumberParameter valueAxis = args[3] as NumberParameter;
            if(modeRad.Value == 0 || modeRad.Value == 1) {
                if(valueRad.Value < 0) {
                    errorMessage = "半径に負のサイズを指定できません";
                    return false;
                }
            }
            return true;
        }

        public string GetCommandName() {
            return "ResizeCylinder";
        }

        public string GetTitle() {
            return "円筒のサイズ変更 / Resize Cylinder";
        }

        public string GetDescription() {
            return "円筒オブジェクトの半径及び厚さを拡大または縮小します";
        }

        public System.Drawing.Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.obj_cylinder; }
        }

        #endregion
    }

    public class OperationResizeLine : IMotionOperationEditObject {

        #region IMotionOperationEditObject メンバ

        public IList<MotionObject> EditObject(IList<MotionObjectInfo> targetInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            SingleSelectParameter mode = args[0] as SingleSelectParameter;
            NumberParameter value = args[1] as NumberParameter;
            List<MotionObject> ret = new List<MotionObject>();
            foreach(MotionObjectInfo info in targetInfoList) {
                LineObject line = frame[info] as LineObject;
                if(line != null) {
                    float length = line.Edge.Length();
                    try {
                        switch(mode.Value) {
                        case 0:
                            length *= (float)value.Value;
                            break;
                        case 1:
                            length = (float)value.Value;
                            break;
                        case 2:
                            length += (float)value.Value;
                            break;
                        }
                    } catch(ArithmeticException) { }
                    ret.Add(new LineObject(line.Position, line.Direction() * length));
                } else {
                    ret.Add(null);
                }
            }
            return ret;
        }

        public Type CreatedType {
            get { return typeof(LineObject); }
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf(typeof(LineObject));
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count > 0)
                return true;
            errorMessage = "線分オブジェクト選択してください";
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            SingleSelectParameter mode = new SingleSelectParameter("Mode ", new string[] { "倍率", "長さ指定", "長さ加算" });
            NumberParameter value = new NumberParameter("Value", -100000, 100000, 3);
            value.Value = 1;
            return new ProcParam<MotionProcEnv>[] { mode, value };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            SingleSelectParameter mode = args[0] as SingleSelectParameter;
            NumberParameter value = args[1] as NumberParameter;
            return true;
        }

        public string GetCommandName() {
            return "ResizeLine";
        }

        public string GetTitle() {
            return "線分のサイズ変更 / Resize Line";
        }

        public string GetDescription() {
            return "線分オブジェクトを伸縮します";
        }

        public System.Drawing.Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.obj_line; }
        }

        #endregion
    }

    public class OperationLineParallelToFloor : IMotionOperationEditObject {
        #region IMotionOperationEditObject メンバ

        public IList<MotionObject> EditObject(IList<MotionObjectInfo> targetInfoList, IList<ProcParam<MotionProcEnv>> args, ReadOnlyMotionFrame frame, bool previewMode) {
            List<MotionObject> ret = new List<MotionObject>();
            foreach(MotionObjectInfo info in targetInfoList) {
                MotionObject created = null;
                LineObject line = frame[info] as LineObject;
                if(line != null) {
                    created = new LineObject(line.Position, new Microsoft.DirectX.Vector3(line.Edge.X, 0, line.Edge.Z));
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
            return info.IsTypeOf<LineObject>();
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count > 0)
                return true;
            errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_PleaseSelectLineObject;
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            return null;
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            return true;
        }

        public string GetCommandName() {
            return "LineParallelToFloor";
        }

        public string GetTitle() {
            return "線分を床と平行にする / Line parallel to Floor";
        }

        public string GetDescription() {
            return "線分オブジェクトの方向成分のうちy成分の値を0にします";
        }

        public System.Drawing.Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.lineHoriz; }
        }

        #endregion
    }
    
}
