using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MotionDataHandler.Misc;
namespace MotionDataUtil {
    public class MotionDataUtilSettings : ChainedSettings {
        protected override System.Configuration.ApplicationSettingsBase Settings {
            get { return MotionDataUtil.Properties.Settings.Default; }
        }
        private MotionDataUtilSettings() : base() { }
        private static MotionDataUtilSettings _singleton = new MotionDataUtilSettings();
        public static MotionDataUtilSettings Singleton { get { return _singleton; } }


        protected override IList<ChainedSettings> SubordinateSettings {
            get { return new ChainedSettings[] { MotionDataHandlerSettings.Singleton }; }
        }
    }
}
