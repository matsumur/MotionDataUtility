using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;

namespace MotionDataHandler.Motion.DefaultOperations {
    // IMotionOperationOutputSequenceを実装するファイル
    using Operation;
    using Script;
    using Sequence;
    using Misc;
    public class OperationOutputPointPosition : IMotionOperationOutputSequence {
        #region IMotionOperationOutputSequence メンバ

        public IList<SequenceData> OutputSequence(IList<MotionObjectInfo> selected, IList<ProcParam<MotionProcEnv>> args, IEnumerable<ReadOnlyMotionFrame> frames, ProgressInformation progressInfo) {
            List<SequenceData> ret = new List<SequenceData>();
            foreach(MotionObjectInfo info in selected) {
                TimeSeriesValues values = new TimeSeriesValues("x", "y", "z");
                foreach(var frame in frames) {
                    PointObject point = frame[info] as PointObject;
                    if(point != null) {
                        try {
                            values[frame.Time] = new decimal?[] { (decimal)point.Position.X, (decimal)point.Position.Y, (decimal)point.Position.Z };
                        } catch(ArithmeticException) {
                            values[frame.Time] = null;
                        }
                    } else {
                        values[frame.Time] = null;
                    }
                }
                SequenceData data = new SequenceData(values, null, PathEx.GiveName("Position", info.Name));
                ret.Add(data);
            }
            return ret;
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf(typeof(PointObject));
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count > 0)
                return true;
            errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_PleaseSelectPointObject;
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            return new ProcParam<MotionProcEnv>[0];
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            return true;
        }

        public string GetCommandName() {
            return "PointPosition";
        }

        public string GetTitle() {
            return "点の座標 / Point Position";
        }

        public string GetDescription() {
            return "点オブジェクトの座標を時系列データとして出力します";
        }

        public System.Drawing.Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.position; }
        }

        #endregion
    }

    public class OperationOutputLineDirection : IMotionOperationOutputSequence {
        #region IMotionOperationOutputSequence メンバ

        public IList<SequenceData> OutputSequence(IList<MotionObjectInfo> selected, IList<ProcParam<MotionProcEnv>> args, IEnumerable<ReadOnlyMotionFrame> frames, ProgressInformation progressInfo) {
            List<SequenceData> ret = new List<SequenceData>();
            foreach(MotionObjectInfo info in selected) {
                TimeSeriesValues values = new TimeSeriesValues("x", "y", "z");
                foreach(var frame in frames) {
                    LineObject line = frame[info] as LineObject;
                    if(line != null) {
                        try {
                            values[frame.Time] = new decimal?[] { (decimal)line.Edge.X, (decimal)line.Edge.Y, (decimal)line.Edge.Z };
                        } catch(ArithmeticException) {
                            values[frame.Time] = null;
                        }
                    } else {
                        values[frame.Time] = null;
                    }
                }
                SequenceData data = new SequenceData(values, null, PathEx.GiveName("Dir", info.Name));
                ret.Add(data);
            }
            return ret;
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
            return null;
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            return true;
        }

        public string GetCommandName() {
            return "LineDirection";
        }

        public string GetTitle() {
            return "線分のベクトルの向き / Line Direction";
        }

        public string GetDescription() {
            return "線分オブジェクトの視点から終点への向きを時系列データとして出力します";
        }

        public System.Drawing.Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.directionLine; }
        }

        #endregion
    }

    public class OperationOutputCylinderRadius : IMotionOperationOutputSequence {
        #region IMotionOperationOutputSequence メンバ

        public IList<SequenceData> OutputSequence(IList<MotionObjectInfo> selected, IList<ProcParam<MotionProcEnv>> args, IEnumerable<ReadOnlyMotionFrame> frames, ProgressInformation progressInfo) {
            List<SequenceData> ret = new List<SequenceData>();
            foreach(MotionObjectInfo info in selected) {
                TimeSeriesValues values = new TimeSeriesValues("radius");
                foreach(var frame in frames) {
                    CylinderObject cylinder = frame[info] as CylinderObject;
                    if(cylinder != null) {
                        try {
                            values[frame.Time] = new decimal?[] { (decimal)cylinder.Radius };
                        } catch(ArithmeticException) {
                            values[frame.Time] = null;
                        }
                    } else {
                        values[frame.Time] = null;
                    }
                }
                SequenceData data = new SequenceData(values, null, PathEx.GiveName("Radius", info.Name));
                ret.Add(data);
            }
            return ret;
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf(typeof(CylinderObject));
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
            return "CylinderRadius";
        }

        public string GetTitle() {
            return "円筒の半径 / Cylinder Radius";
        }

        public string GetDescription() {
            return "円筒オブジェクトの半径を時系列データとして出力します";
        }

        public System.Drawing.Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.radiusCylinder; }
        }

        #endregion
    }
    public class OperationOutputSphereRadius : IMotionOperationOutputSequence {
        #region IMotionOperationOutputSequence メンバ

        public IList<SequenceData> OutputSequence(IList<MotionObjectInfo> selected, IList<ProcParam<MotionProcEnv>> args, IEnumerable<ReadOnlyMotionFrame> frames, ProgressInformation progressInfo) {
            List<SequenceData> ret = new List<SequenceData>();
            foreach(MotionObjectInfo info in selected) {
                TimeSeriesValues values = new TimeSeriesValues("radius");
                foreach(var frame in frames) {
                    SphereObject sphere = frame[info] as SphereObject;
                    if(sphere != null) {
                        try {
                            values[frame.Time] = new decimal?[] { (decimal)sphere.Radius };
                        } catch(ArithmeticException) {
                            values[frame.Time] = null;
                        }
                    } else {
                        values[frame.Time] = null;
                    }
                }
                SequenceData data = new SequenceData(values, null, PathEx.GiveName("Radius", info.Name));
                ret.Add(data);
            }
            return ret;
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf(typeof(SphereObject));
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
            return "SphereRadius";
        }

        public string GetTitle() {
            return "球の半径 / Sphere Radius";
        }

        public string GetDescription() {
            return "球オブジェクトの半径を時系列データとして出力します";
        }

        public System.Drawing.Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.radiusSphere; }
        }

        #endregion
    }
    public class OperationOutputPlaneArea : IMotionOperationOutputSequence {
        #region IMotionOperationOutputSequence メンバ

        public IList<SequenceData> OutputSequence(IList<MotionObjectInfo> selected, IList<ProcParam<MotionProcEnv>> args, IEnumerable<ReadOnlyMotionFrame> frames, ProgressInformation progressInfo) {
            List<SequenceData> ret = new List<SequenceData>();
            foreach(MotionObjectInfo info in selected) {
                TimeSeriesValues values = new TimeSeriesValues("area");
                foreach(var frame in frames) {
                    PlaneObject sphere = frame[info] as PlaneObject;
                    if(sphere != null) {
                        try {
                            values[frame.Time] = new decimal?[] { sphere.GetDimensions() };
                        } catch(ArithmeticException) {
                            values[frame.Time] = null;
                        }
                    } else {
                        values[frame.Time] = null;
                    }
                }
                SequenceData data = new SequenceData(values, null, PathEx.GiveName("Area", info.Name));
                ret.Add(data);
            }
            return ret;
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf(typeof(PlaneObject));
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count > 0)
                return true;
            errorMessage = "平面オブジェクトを選択してください";
            return false;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            return null;
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            return true;
        }

        public string GetCommandName() {
            return "PlaneArea";
        }

        public string GetTitle() {
            return "平面の面積 / Plane Area";
        }

        public string GetDescription() {
            return "平面オブジェクトの面積を時系列データとして出力します";
        }

        public System.Drawing.Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.areaPlanes; }
        }

        #endregion
    }

    public class OperationOutputRelativeCoordinate : IMotionOperationOutputSequence {
        #region IMotionOperationOutputSequence メンバ

        public IList<SequenceData> OutputSequence(IList<MotionObjectInfo> selected, IList<ProcParam<MotionProcEnv>> args, IEnumerable<ReadOnlyMotionFrame> frames, ProgressInformation progressInfo) {
            MotionObjectSingleSelectParameter main = args[0] as MotionObjectSingleSelectParameter;
            SingleSelectParameter unit = args[1] as SingleSelectParameter;

            bool useRatio = unit.Value == 0;
            MotionObjectInfo firstAxis = main.Value;
            MotionObjectInfo secondAxis = selected.Where(info => info.IsTypeOf(typeof(LineObject)) && info != firstAxis).First();
            IList<MotionObjectInfo> pointInfoList = selected.Where(info => info.IsTypeOf(typeof(PointObject))).ToList();
            List<SequenceData> ret = new List<SequenceData>();
            foreach(MotionObjectInfo pointInfo in pointInfoList) {
                TimeSeriesValues values = new TimeSeriesValues("axis1", "axis2", "axis3");
                foreach(var frame in frames) {
                    PointObject point = frame[pointInfo] as PointObject;
                    LineObject line1 = frame[firstAxis] as LineObject;
                    LineObject line2 = frame[secondAxis] as LineObject;
                    decimal?[] relValues = null;
                    if(point != null && line1 != null && line2 != null) {
                        Vector3 anchor = line1.Position;
                        Vector3 pointRelPos = point.Position - anchor;
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
                        float[,] mat = new float[,] { { axis1.X, axis2.X, axis3.X }, { axis1.Y, axis2.Y, axis3.Y }, { axis1.Z, axis2.Z, axis3.Z } };
                        float[] vec = new float[] { pointRelPos.X, pointRelPos.Y, pointRelPos.Z };
                        SimultaneousEquations solve = SimultaneousEquations.Solve(mat, vec);
                        if(solve.Answers.Length == 3) {
                            relValues = new decimal?[3];
                            try {
                                relValues[0] = (decimal)solve.Answers[0];
                                relValues[1] = (decimal)solve.Answers[1];
                                relValues[2] = (decimal)solve.Answers[2];
                            } catch(OverflowException) { relValues = null; }
                        }
                    }
                    values.SetValue(frame.Time, relValues);
                }
                SequenceData data = new SequenceData(values, null, PathEx.GiveName("RelPos", pointInfo.Name));
                ret.Add(data);
            }
            return ret;
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf(typeof(PointObject)) || info.IsTypeOf(typeof(LineObject));
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Where(info => info.IsTypeOf(typeof(PointObject))).Count() == 0 || infoList.Where(info => info.IsTypeOf(typeof(LineObject))).Count() != 2) {
                errorMessage = "一つ以上の点オブジェクトと，線分オブジェクトを二つ選択してください";
                return false;
            }
            return true;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            MotionObjectSingleSelectParameter main = new MotionObjectSingleSelectParameter("First Axis", new Predicate<MotionObjectInfo>(info => info.IsTypeOf(typeof(LineObject))), true);
            SingleSelectParameter unit = new SingleSelectParameter("Unit", new string[] { "線分の長さに対する比", "単位長さ" });
            return new ProcParam<MotionProcEnv>[] { main, unit };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            MotionObjectSingleSelectParameter main = args[0] as MotionObjectSingleSelectParameter;
            SingleSelectParameter unit = args[1] as SingleSelectParameter;
            return true;
        }

        public string GetCommandName() {
            return "RelativeCoordinateByTwoLines";
        }

        public string GetTitle() {
            return "二線分からなる空間における点の座標 / Point Relative Coordinate By Two Lines";
        }

        public string GetDescription() {
            return "二つの線分とそれらに垂直な線分からなる座標系を作り，その座標系における指定された点の座標を時系列データとして出力します．\r\n(二線分の始点から終点へのベクトルx, yと，xとyの外積からなるベクトルz (|z| = √|x×y|，右手系)，一方の線分の始点s，及び点の座標pから，p-s = ax + by + czとなるa, b, cを求めます)";
        }

        public System.Drawing.Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.positionRelative; }
        }

        #endregion
    }

    public class OperationOutputAngleBetweenLines : IMotionOperationOutputSequence {
        #region IMotionOperationOutputSequence メンバ

        public IList<SequenceData> OutputSequence(IList<MotionObjectInfo> selected, IList<ProcParam<MotionProcEnv>> args, IEnumerable<ReadOnlyMotionFrame> frames, ProgressInformation progressInfo) {
            SingleSelectParameter unit = args[0] as SingleSelectParameter;
            bool degree = unit.Value == 0;

            MotionObjectInfo info1 = selected[0];
            MotionObjectInfo info2 = selected[1];
            TimeSeriesValues values = new TimeSeriesValues(degree ? "degree" : "radian");
            foreach(ReadOnlyMotionFrame frame in frames) {
                LineObject line1 = frame[info1] as LineObject;
                LineObject line2 = frame[info2] as LineObject;
                if(line1 != null && line2 != null) {
                    float cos = Vector3.Dot(line1.Direction(), line2.Direction());
                    double radian = 0;
                    if(cos <= -1) {
                        radian = Math.PI;
                    } else if(cos < 1) {
                        radian = Math.Acos(cos);
                    }
                    if(degree) {
                        values.SetValue(frame.Time, (decimal)(radian * 180 / Math.PI));
                    } else {
                        values.SetValue(frame.Time, (decimal)radian);
                    }
                } else {
                    values.SetValue(frame.Time, null);
                }
            }
            SequenceData data = new SequenceData(values, null, PathEx.GiveName("Angle", info1.Name, info2.Name));
            return new SequenceData[] { data };
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return info.IsTypeOf(typeof(LineObject));
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            if(infoList.Count != 2) {
                errorMessage = "二つの線分オブジェクトを選択してください";
                return false;
            }
            return true;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            SingleSelectParameter unit = new SingleSelectParameter("Unit", new string[] { "Degree", "Radian" });
            return new ProcParam<MotionProcEnv>[] { unit };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            SingleSelectParameter unit = args[0] as SingleSelectParameter;
            return true;
        }

        public string GetCommandName() {
            return "AngleBetweenLines";
        }

        public string GetTitle() {
            return "二線分の角度 / Angle Between two Lines";
        }

        public string GetDescription() {
            return "二つの線分オブジェクトの向きのなす角度を時系列データとして出力します";
        }

        public System.Drawing.Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.angleLines; }
        }

        #endregion
    }

    public class OperationOutputTotalLengthOfLines : IMotionOperationOutputSequence {
        #region IMotionOperationOutputSequence メンバ

        public IList<SequenceData> OutputSequence(IList<MotionObjectInfo> selected, IList<ProcParam<MotionProcEnv>> args, IEnumerable<ReadOnlyMotionFrame> frames, ProgressInformation progressInfo) {
            TimeSeriesValues values = new TimeSeriesValues("length");
            foreach(ReadOnlyMotionFrame frame in frames) {
                IList<LineObject> lines = selected.Select(info => frame[info] as LineObject).ToList();
                if(lines.All(line => line != null) && lines.Count > 0) {
                    try {
                        values.SetValue(frame.Time, (decimal)lines.Sum(line => line.Length()));
                    } catch(ArithmeticException) {
                        values.SetValue(frame.Time, null);
                    }
                } else {
                    values.SetValue(frame.Time, null);
                }
            }
            string name = selected[0].Name;
            if(selected.Count > 1)
                name += "-etc";
            SequenceData data = new SequenceData(values, null, PathEx.GiveName("Length", name));
            return new SequenceData[] { data };
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
            return "TotalLengthOfLines";
        }

        public string GetTitle() {
            return "線分の長さの合計 / Total Length of Lines";
        }

        public string GetDescription() {
            return "線分オブジェクトの各長さの合計を時系列データとして出力します";
        }

        public System.Drawing.Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.lengthLines; }
        }

        #endregion
    }

    public class OperationOutputLineCollisionTarget : IMotionOperationOutputSequence {
        #region IMotionOperationOutputSequence メンバ

        public IList<SequenceData> OutputSequence(IList<MotionObjectInfo> selected, IList<ProcParam<MotionProcEnv>> args, IEnumerable<ReadOnlyMotionFrame> frames, ProgressInformation progressInfo) {
            MotionObjectSingleSelectParameter main = args[0] as MotionObjectSingleSelectParameter;
            NumberParameter enlarge = args[1] as NumberParameter;
            float enlargement = 0;
            try { enlargement = (float)enlarge.Value; } catch(ArithmeticException) { }
            IList<MotionObjectInfo> targetInfoList = selected.Where(info => info != main.Value).ToList();


            ICSLabelSequence labelSequence = new ICSLabelSequence(TimeController.Singleton.EndTime);
            string prevLabelText = null;
            decimal prevTime = decimal.MinValue;

            decimal wholeEndTime = decimal.MinValue;
            foreach(var frame in frames) {
                wholeEndTime = frame.Time;

                LineObject line = frame[main.Value] as LineObject;
                string labelText = "";
                if(line == null || targetInfoList.Any(info => frame[info] == null)) {
                    labelText = "no data";
                } else {
                    float minDistance = float.MaxValue;
                    MotionObjectInfo nearest = null;
                    foreach(var targetInfo in targetInfoList) {
                        float distance;
                        if(frame[targetInfo].DetectLineCollision(line.Position, line.AnotherEnd, enlargement, out distance)) {
                            if(distance < minDistance) {
                                nearest = targetInfo;
                                minDistance = distance;
                            }
                        }
                    }
                    if(nearest != null) {
                        labelText = nearest.Name;
                    } else {
                        labelText = "else";
                    }
                }
                if(prevLabelText != labelText) {
                    decimal endTime = frame.Time;
                    if(prevTime < endTime && prevLabelText != null) {
                        labelSequence.SetLabel(prevTime, endTime, prevLabelText);
                    }
                    prevTime = endTime;
                    prevLabelText = labelText;
                }
            }
            if(prevTime < wholeEndTime && prevLabelText != null) {
                labelSequence.SetLabel(prevTime, wholeEndTime, prevLabelText);
            }
            SequenceData data = SequenceData.FromLabelSequence(labelSequence, PathEx.GiveName("Collision", main.Value.Name));
            return new SequenceData[] { data };
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
            return "LineCollisionTarget";
        }

        public string GetTitle() {
            return "線分の衝突対象検出 / Detect Collision of Line and other Objects";
        }

        public string GetDescription() {
            return "オブジェクトの中で指定された線分オブジェクトとの交点を持つものをラベルとして出力します．複数のオブジェクトが交点を持つ場合には，交点の位置が線分オブジェクトの始点に一倍近いものが出力されます．";
        }

        public System.Drawing.Bitmap IconBitmap {
            get { return global::MotionDataHandler.Properties.Resources.line_collision; }
        }

        #endregion
    }
}
