using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MotionDataUtil {
    static class Program {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main() {
            string[] cmds = System.Environment.GetCommandLineArgs();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(MotionDataUtilityForm.Singleton);
        }
    }

}
