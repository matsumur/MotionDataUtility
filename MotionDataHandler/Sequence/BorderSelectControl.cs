using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MotionDataHandler.Sequence {
    public partial class BorderSelectionControl : ListBox {
        public BorderSelectionControl() {
            InitializeComponent();
        }
        LabelingBorders _border;
        public void AttachBorder(LabelingBorders border) {
            if(border == null)
                throw new ArgumentNullException("border", "'border' cannot be null");

            List<int> prevIndices = new List<int>();
            foreach(int prevIndex in this.SelectedIndices) {
                prevIndices.Add(prevIndex);
            }
            _border = border;
            Items.Clear();
            int index = 0;
            foreach (var text in _border.GetLabelNames(true)) {
                Items.Add(text);
                if(prevIndices.Contains(index)) {
                    SelectedIndices.Add(index);
                }
                index++;
            }
        }

        public void SelectRange(IList<string> borders) {
            SelectedItems.Clear();
            List<string> items = new List<string>();
            foreach (var item in Items) {
                items.Add((string)item);
            }
            foreach (var item in items) {
                if (borders.Contains(item)) {
                    SelectedItems.Add(item);
                }
            }
        }
    }
}
