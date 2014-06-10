using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionDataHandler.Misc {
    /// <summary>
    /// PluginHostが変更されたときのデータを提供します
    /// </summary>
    public class IPluginHostChangedEventArgs : EventArgs {
        public Plugin.IPluginHost PluginHost;
        public IPluginHostChangedEventArgs(Plugin.IPluginHost pluginHost) {
            PluginHost = pluginHost;
        }
    }
}
