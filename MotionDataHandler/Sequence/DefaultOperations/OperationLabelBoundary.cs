using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionDataHandler.Sequence.DefaultOperations {
    using Operation;
    using Misc;
    public class OperationLabelBoundary : ISequenceOperation {

        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            var labelSequence = env.SelectedSequence.GetLabelSequence();
            var labels = (args[0] as LabelSelectParameter).Value;
            var beforeStart = (args[1] as NumberParameter).Value;
            var afterStart = (args[2] as NumberParameter).Value;
            var beforeEnd = (args[3] as NumberParameter).Value;
            var afterEnd = (args[4] as NumberParameter).Value;

            ICSLabelSequence tmp = new ICSLabelSequence();
            foreach(var label in labelSequence.EnumerateLabels()) {
                if(labels.Contains(label.LabelText)) {
                    decimal preStart = label.BeginTime - beforeStart;
                    decimal postStart = label.BeginTime + afterStart;
                    if(postStart > preStart) {
                        tmp.SetLabel(preStart, postStart, label.LabelText + "_begin");
                    }
                    decimal preEnd = label.EndTime - beforeEnd;
                    decimal postEnd = label.EndTime + afterEnd;
                    if(postEnd > preEnd) {
                        tmp.SetLabel(preEnd, postEnd, label.LabelText + "_end");
                    }
                }
            }
            return SequenceData.FromLabelSequence(tmp, PathEx.GiveName("Boundary", env.SelectedSequence.Title), null);
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            List<ProcParam<SequenceProcEnv>> ret = new List<ProcParam<SequenceProcEnv>>();
            ret.Add(new LabelSelectParameter("Target Label", true));
            ret.Add(new NumberParameter("Second before Boundary at Label Start", 0, 1000, 3));
            ret.Add(new NumberParameter("Second after Boundary at Label Start", 0, 1000, 3));
            ret.Add(new NumberParameter("Second before Boundary at Label End", 0, 1000, 3));
            ret.Add(new NumberParameter("Second after Boundary at Label End", 0, 1000, 3));
            return ret;
        }

        public string GetDescription() {
            return "選択されたラベルの前後部分を取り出した新しいラベル列を作成します。";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.Label; }
        }

        public string GetTitle() {
            return "ラベル境界抽出 / Label Boundary";
        }

        public bool ReplacesInternalData {
            get { return false; }
        }
        public string GetCommandName() {
            return "ExtractLabelBoundary";
        }
        #endregion
    }
}
