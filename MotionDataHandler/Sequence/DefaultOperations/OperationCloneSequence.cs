using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionDataHandler.Sequence.DefaultOperations {
    using Operation;
    using Misc;
    public class OperationCloneSequence : ISequenceOperation {
        #region ISequenceOperation メンバ

        public SequenceData Operate(IList<ProcParam<SequenceProcEnv>> args, SequenceProcEnv env) {
            return new SequenceData(env.SelectedSequence);
        }

        public IList<ProcParam<SequenceProcEnv>> GetParameters() {
            return new ProcParam<SequenceProcEnv>[0];
        }

        public string GetTitle() {
            return "複製 / Clone Sequence";
        }

        public string GetDescription() {
            return "シーケンスを複製します。";
        }

        public SequenceType OperationTargetType {
            get { return SequenceType.NumericLabel; }
        }

        public bool ReplacesInternalData {
            get { return false; }
        }
        public string GetCommandName() {
            return "Clone";
        }

        #endregion
    }
}
