using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MotionDataUtilExe {
    static class Program {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if(true) {
                Application.Run(MotionDataUtil.MotionDataUtilityForm.Singleton);
            } else {
                Application.Run(MotionDataUtil.SequenceViewerForm.Singleton);
            }
        }
    }
}
