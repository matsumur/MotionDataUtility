using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MotionDataHandler;
using MotionDataHandler.Misc;

namespace MotionDataUtil {
    public class TSeqViewerPlugin : Plugin.IPlugin {
        Plugin.IPluginHost _host;
        SequenceViewerForm form;
        public void Run() {
            form = SequenceViewerForm.Singleton;
            form.AttachIPluginHost(Owner);
            TimeController.Singleton.ExtendDuration(_host.Duration);
            TimeController.Singleton.AttachIPluginHost(Owner);
            form.Show();
        }


        public void ExecuteTemplate(Plugin.History history) {
        }

        /// <summary>
        /// プラグインの名前
        /// </summary>
        public string Name { get { return typeof(TSeqViewerPlugin).Name; } }

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
        public string Description { get { return "TSeqViewer"; } }

        /// <summary>
        /// プラグインのホストとなるAnnotationFieldのインタフェース
        /// </summary>
        public Plugin.IPluginHost Owner { get { return _host; } set { _host = value; } }
    }
}
