using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MotionDataHandler.Misc {
    /// <summary>
    /// データグリッドを持つダイアログ
    /// </summary>
    public partial class DialogOKDataGrid : DialogOKCancel {
        public DialogOKDataGrid() {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
        }

        private void DataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e) {

        }
    }
}
