using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionDataHandler.Motion.DefaultOperations {
    // IMotionOperationGeneralを実装するファイル

    using Operation;
    using Script;
    using Sequence;
    using Misc;

    public class OperationRemoveObjects : IMotionOperationGeneral {
        #region IMotionOperationGeneral メンバ

        public void Operate(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, MotionDataSet dataSet, ProgressInformation progressInfo) {
            progressInfo.Initialize(dataSet.FrameLength, "Remove Object from Frames");
            foreach(MotionFrame frame in dataSet.EnumerateFrame()) {
                progressInfo.CurrentValue++;
                foreach(MotionObjectInfo info in selectedInfoList) {
                    frame.RemoveObject(info);
                }
            }
            foreach(MotionObjectInfo info in selectedInfoList) {
                dataSet.RemoveObject(info);
            }
            dataSet.DoObjectInfoSetChanged();
        }

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
            return new ProcParam<MotionProcEnv>[0];
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            return true;
        }

        public string GetCommandName() {
            return "RemoveObject";
        }

        public string GetTitle() {
            return "オブジェクトを削除 / Remove Object";
        }

        public string GetDescription() {
            return "すべてのフレームから指定されたオブジェクトを取り除きます";
        }

        public System.Drawing.Bitmap IconBitmap {
            get { return null; }
        }

        #endregion
    }

    public class OperationRename : IMotionOperationGeneral {
        #region IMotionOperationGeneral メンバ

        public void Operate(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, MotionDataSet dataSet, ProgressInformation progressInfo) {
            StringParameter name = args[0] as StringParameter;
            foreach(MotionObjectInfo info in selectedInfoList) {
                info.Name = name.Value ?? "unnamed";
            }
            dataSet.DoObjectInfoSetChanged();
        }

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
            StringParameter name = new StringParameter("New Name");
            return new ProcParam<MotionProcEnv>[] { name };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            StringParameter name = args[0] as StringParameter;
            if(name.Value == null) {
                errorMessage = global::MotionDataHandler.Properties.Settings.Default.Msg_CannotSpecifyNull;
                return false;
            }
            return true;
        }

        public string GetCommandName() {
            return "Rename";
        }

        public string GetTitle() {
            return "名前の変更 / Rename";
        }

        public string GetDescription() {
            return "選択されたオブジェクトの名前を変更します．複数選択の場合は末尾に番号がつきます";
        }

        public System.Drawing.Bitmap IconBitmap {
            get { return null; }
        }

        #endregion
    }

    public class OperationInterpolateLinear : IMotionOperationGeneral {
        #region IMotionOperationGeneral メンバ

        public void Operate(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, MotionDataSet dataSet, ProgressInformation progressInfo) {
            SingleSelectParameter mode = args[0] as SingleSelectParameter;
            NumberParameter limit2 = args[1] as NumberParameter;
            bool addMode = mode.Value == 1;
            int limit = (int)limit2.Value;

            progressInfo.Initialize(selectedInfoList.Count, "Interpolate");
            foreach(var info in selectedInfoList) {
                // 欠落範囲の集合
                RangeSet<int> missings = new RangeSet<int>();

                bool exist = true;
                int begin = 0;
                // 最初のフレームからかけている部分，最後のかけている部分は無視
                for(int i = 0; i < dataSet.FrameLength; i++) {
                    if(dataSet.GetFrameByIndex(i)[info] == null) {
                        if(exist) {
                            begin = i;
                            exist = false;
                        }
                    } else {
                        if(!exist) {
                            if(begin != 0) {
                                missings.Add(new RangeSet<int>.Range(begin, i));
                                exist = true;
                            }
                        }
                    }
                }
                // 別オブジェクトにするとき用に入力オブジェクトと出力オブジェクトを分ける
                MotionObjectInfo addInfo = info;
                if(addMode) {
                    // 別オブジェクトにするオプション
                    addInfo = new MotionObjectInfo(info.ObjectType, info);
                    addInfo.Name = PathEx.GiveName("interpolate", info.Name);
                    dataSet.AddObject(addInfo);
                }
                // 線形補間
                foreach(var range in missings) {
                    if(limit == 0 || range.End - range.Start <= limit) {
                        int pre = range.Start - 1;
                        int post = range.End;
                        MotionFrame preFrame = dataSet.GetFrameByIndex(pre);
                        MotionFrame postFrame = dataSet.GetFrameByIndex(post);
                        MotionObject preObject = preFrame[info];
                        if(preObject != null) {
                            for(int index = range.Start; index < range.End; index++) {
                                float interpolater = (float)(index - pre) / (post - pre);
                                MotionFrame frame = dataSet.GetFrameByIndex(index);
                                frame[addInfo] = preObject.InterpolateLinear(postFrame[info], interpolater);
                            }
                        }
                    }
                }
                progressInfo.CurrentValue++;
            }

            dataSet.DoFrameListChanged();
            if(addMode) {
                dataSet.DoObjectInfoSetChanged();
            }
        }

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
            SingleSelectParameter mode = new SingleSelectParameter("Mode", new string[] { "上書き", "追加" });
            NumberParameter limit = new NumberParameter("連続する欠落を補完するのフレーム数の上限", 0, 1000000, 0);
            limit.Value = 60;
            return new ProcParam<MotionProcEnv>[] { mode, limit };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            SingleSelectParameter mode = args[0] as SingleSelectParameter;
            NumberParameter limit = args[1] as NumberParameter;
            return true;
        }

        public string GetCommandName() {
            return "InterpolateFramesByLinear";
        }

        public string GetTitle() {
            return "欠落の線形補間 / Interpolate Frames by Linear";
        }

        public string GetDescription() {
            return "指定されたフレーム数以下のオブジェクトの欠落を線形補間します";
        }

        public System.Drawing.Bitmap IconBitmap {
            get { return null; }
        }

        #endregion
    }
    public class OperationClipFrame : IMotionOperationGeneral {
        #region IMotionOperationGeneral メンバ

        public void Operate(IList<MotionObjectInfo> selectedInfoList, IList<ProcParam<MotionProcEnv>> args, MotionDataSet dataSet, ProgressInformation progressInfo) {
            NumberParameter begin = args[0] as NumberParameter;
            NumberParameter end = args[1] as NumberParameter;
            dataSet.RemoveFrameAll((f, i) => f.Time < begin.Value || end.Value < f.Time);
        }

        #endregion

        #region IMotionOperationBase メンバ

        public bool FilterSelection(MotionObjectInfo info) {
            return true;
        }

        public bool ValidateSelection(IList<MotionObjectInfo> infoList, ref string errorMessage) {
            return true;
        }

        public IList<ProcParam<MotionProcEnv>> GetParameters() {
            NumberParameter begin = new NumberParameter("Begin Time", 0, 100000, 0);
            NumberParameter end = new NumberParameter("End Time", 0, 100000, 0);
            return new ProcParam<MotionProcEnv>[] { begin, end };
        }

        public bool ValidateArguments(IList<ProcParam<MotionProcEnv>> args, ref string errorMessage) {
            NumberParameter begin = args[0] as NumberParameter;
            NumberParameter end = args[1] as NumberParameter;
            return true;
        }

        public string GetCommandName() {
            return "ClipFrame";
        }

        public string GetTitle() {
            return "フレームを切り抜く / Clip Frame";
        }

        public string GetDescription() {
            return "指定された時間範囲の外にあるフレームを取り除きます";
        }

        public System.Drawing.Bitmap IconBitmap {
            get { return null; }
        }

        #endregion
    }
}
