using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MotionDataHandler;
using MotionDataHandler.Motion;
using MotionDataHandler.Misc;

namespace MotionDataUtil {
    public partial class MotionDataViewerForm : Form {
        MotionDataSet _dataSet;
        public MotionDataViewerForm(MotionDataSet dataSet) {
            InitializeComponent();
            if(dataSet == null)
                throw new ArgumentNullException("dataSet", "'dataSet' cannot be null");
            _dataSet = dataSet;
            motionDataViewer1.AttachDataSet(dataSet);
            motionDataViewer1.AttachTimeController(TimeController.Singleton);
        }

        private void MotionDataViewerForm_KeyDown(object sender, KeyEventArgs e) {

        }

        private void MotionDataViewerForm_FormClosing(object sender, FormClosingEventArgs e) {
            this.motionDataViewer1.DetachDataSet();
        }

        private void motionDataViewer1_Load(object sender, EventArgs e) {
            this.motionDataViewer1.RequestRender();
        }

        private void MotionDataViewerForm_KeyPress(object sender, KeyPressEventArgs e) {

        }

        private void motionDataViewer1_KeyDown(object sender, KeyEventArgs e) {
            MessageBox.Show(e.KeyCode.ToString());
        }
    }
}
