using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MotionDataHandler.Misc;

namespace MotionDataUtil {
    public partial class TimeControllerPlayerForm : Form {
        public TimeControllerPlayerForm() {
            InitializeComponent();
            timePlayer1.AttachTimeController(TimeController.Singleton);
        }
    }
}
