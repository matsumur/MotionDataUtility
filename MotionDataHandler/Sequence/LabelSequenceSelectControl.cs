using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MotionDataHandler.Sequence {
    public partial class SequenceSelectionControl : UserControl {
        public SequenceSelectionControl() {
            InitializeComponent();
        }
        SequenceViewerController _controller = null;

        public void AttachController(SequenceViewerController controller) {
            AttachController(controller, x => true);
        }

        /// <summary>
        /// ビューア群を所有するコントローラを割り当てます。
        /// </summary>
        /// <param title="controller">ビューア群を所有するコントローラ</param>
        /// <param title="conditionToShowOnList">表示するビューアをリストに表示する条件</param>
        public void AttachController(SequenceViewerController controller, Func<SequenceView, bool> conditionToShowOnList) {
            var prevSequences = this.SelectedItems;

            _controller = controller;
            _conditionToShow = conditionToShowOnList;
            if(_controller != null) {
                listLabelSequence.SuspendLayout();
                listLabelSequence.Items.Clear();
                listLabelSequence.SelectedIndices.Clear();
                var viewers = _controller.GetViewList();
                int count = 0;
                int index = 0;
                foreach(var viewer in viewers) {
                    if(_conditionToShow(viewer)) {
                        listLabelSequence.Items.Add(viewer.Sequence.Title);
                        if(prevSequences.Select(p => p.Title).Contains(viewer.Sequence.Title)) {
                            listLabelSequence.SelectedIndices.Add(index);
                        }
                        index++;
                    }
                    count++;
                }
                listLabelSequence.ResumeLayout();
            }
        }

        public void DetachController() {
            _controller = null;
        }

        private Func<SequenceView, bool> _conditionToShow = x => true;

        private void LabelSequenceSelectControl_Load(object sender, EventArgs e) {

        }

        public SelectionMode SelectionMode {
            get { return listLabelSequence.SelectionMode; }
            set { listLabelSequence.SelectionMode = value; }
        }

        public IList<SequenceData> SelectedItems {
            get {
                if(_controller == null)
                    return new List<SequenceData>();
                return (from string title in listLabelSequence.SelectedItems
                        let sequence = _controller.GetSequenceByTitle(title)
                        where sequence != null
                        select sequence).ToList();
            }
        }
        public ListBox.SelectedIndexCollection SelectedIndices {
            get { return listLabelSequence.SelectedIndices; }
        }

        [Category("動作"), Description("項目の選択が変更された時に呼び出されるイベント")]
        public event EventHandler SelectedIndexChanged;

        private void listLabelSequence_SelectedIndexChanged(object sender, EventArgs e) {
            if(SelectedIndexChanged != null) {
                SelectedIndexChanged.Invoke(this, e);
            }
        }
    }
}
