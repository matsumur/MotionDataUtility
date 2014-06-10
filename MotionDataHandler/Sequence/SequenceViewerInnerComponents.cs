using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MotionDataHandler.Sequence {
/// <summary>
/// SequenceViewerで用いるUI
/// </summary>
    public partial class SequenceViewerInnerComponents : Component {
        public SequenceViewerInnerComponents() {
            InitializeComponent();
        }

        public SequenceViewerInnerComponents(IContainer container) {
            container.Add(this);

            InitializeComponent();
        }
    }
}
