using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionDataUtil {
    public class MotionDataUtilPlugin : Plugin.IPlugin {
        Plugin.IPluginHost _host;
        MotionDataUtilityForm form;
        public void Run() {
            form = MotionDataUtilityForm.Singleton;
            form.AttachIPlguinHost(Owner);
            form.Show();
        }


        public void ExecuteTemplate(Plugin.History history) {
        }

        /// <summary>
        /// プラグインの名前
        /// </summary>
        public string Name { get { return "MotionDataUtil"; } }

        /// <summary>
        /// プラグインのバージョン
        /// </summary>
        public string Version {
            get {
                System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
                System.Version ver = asm.GetName().Version;
                return ver.ToString();
            }
        }

        /// <summary>
        /// プラグインの説明
        /// </summary>
        public string Description { get { return "モーションデータユーティリティ"; } }

        /// <summary>
        /// プラグインのホストとなるAnnotationFieldのインタフェース
        /// </summary>
        public Plugin.IPluginHost Owner { get { return _host; } set { _host = value; } }
    }
}
