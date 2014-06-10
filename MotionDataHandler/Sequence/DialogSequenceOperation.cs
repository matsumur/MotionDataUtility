using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MotionDataHandler.Sequence {
    using Misc;
    using Operation;
    public partial class DialogSequenceOperation : DialogOKCancel {
        Operation.OperationExecution _operator;
        Panel panelSub = new Panel();
        public IList<ProcParam<SequenceProcEnv>> Args {
            get { return _operator.Parameters; }
        }
        public DialogSequenceOperation(ISequenceOperation ope, SequenceViewerController controller, SequenceData current)
            : this(ope, controller, current, null) {
        }
        public DialogSequenceOperation(ISequenceOperation ope, SequenceViewerController controller, SequenceData current, IList<ProcParam<SequenceProcEnv>> paramList) {
            InitializeComponent();
            _operator = new Operation.OperationExecution(ope, controller, current);
            _operator.Parameters = paramList;
            panelSub = _operator.GetPanel();
            panelSub.Dock = DockStyle.Fill;
            panelMain.Controls.Add(panelSub);
            this.Text = ope.GetTitle();
            this.textSelected.Text = current.Title;
            textExplain.Text = ope.GetDescription();
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            _operator.Run(this);
            DialogResult = DialogResult.OK;
        }

        private void DialogSequenceOperation_Load(object sender, EventArgs e) {
            if(panelSub != null && panelSub.Controls.Count == 0) {
                this.Invoke(new EventHandler(buttonOK_Click), sender, e);
            }
        }
    }
}
