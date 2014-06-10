using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MotionDataHandler.Sequence {
    using Misc;
    public partial class DialogCloseSequenceViewers : DialogOKCancel {
        readonly SequenceViewerController _controller;
        public DialogCloseSequenceViewers(SequenceViewerController controller) {
            if(controller == null)
                throw new ArgumentNullException("controller", "'controller' cannot be null");
            InitializeComponent();
            _controller = controller;
        }

        private void controlViewers_SelectedIndexChanged(object sender, EventArgs e) {
            buttonOK.Enabled = controlViewers.SelectedItems.Count > 0;
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            _controller.RemoveView(controlViewers.SelectedItems.Select(seq => seq.Title).ToList());
            _controller.DoAllocationChanged();
            DialogResult = DialogResult.OK;
        }

        private void DialogCloseSequenceViewers_FormClosed(object sender, FormClosedEventArgs e) {
            controlViewers.DetachController();
        }

        private void DialogCloseSequenceViewers_Load(object sender, EventArgs e) {
            controlViewers.AttachController(_controller);
        }
    }
}
