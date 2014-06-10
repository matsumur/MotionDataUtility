using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MotionDataHandler.Sequence {
    public partial class SequenceIndexSelectControl : ListBox {
        public SequenceIndexSelectControl() {
            InitializeComponent();
        }

        public void SetItemsFromSequence(SequenceData sequence) {
            this.SetItemsFromSequenceValues(sequence.Values);
        }

        public void SetItemsFromSequenceValues(TimeSeriesValues values) {
            this.SetItems(values.ColumnNames);
        }

        public void SetItems(IEnumerable<string> names) {
            this.Items.Clear();
            foreach(string name in names) {
                this.Items.Add(name);
            }
        }

        public SequenceIndexSelectControl(IContainer container) {
            container.Add(this);

            InitializeComponent();
        }

    }
}
