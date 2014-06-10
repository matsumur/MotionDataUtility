using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionDataHandler.Misc {
    /// <summary>
    /// 複数DLLにまたがるApplicationSettingsを一度に保存する
    /// </summary>
    public abstract class ChainedSettings {
        /// <summary>
        /// 保存・読み込み対象のApplicationSettingsを返します
        /// </summary>
        protected abstract global::System.Configuration.ApplicationSettingsBase Settings { get; }
        /// <summary>
        /// このオブジェクトが管理するApplicationSettingsの保存・読み込み時に連鎖的に保存・読み込みを行うChainedSettingsのリストを返します
        /// </summary>
        protected abstract IList<ChainedSettings> SubordinateSettings { get; }
        /// <summary>
        /// ストレージから設定を読み直します
        /// </summary>
        public virtual void Reload() {
            foreach(ChainedSettings settings in this.SubordinateSettings ?? new ChainedSettings[0]) {
                settings.Reload();
            }
            try {
                this.Settings.Reload();
            } catch(System.Configuration.ConfigurationException) { }
        }
        /// <summary>
        /// 設定をデフォルト値に戻します
        /// </summary>
        public virtual void Reset() {
            foreach(ChainedSettings settings in this.SubordinateSettings ?? new ChainedSettings[0]) {
                settings.Reset();
            }
            try {
                this.Settings.Reset();
            } catch(System.Configuration.ConfigurationException) { }
        }
        /// <summary>
        /// 設定をストレージに保存します
        /// </summary>
        public virtual void Save() {
            foreach(ChainedSettings settings in this.SubordinateSettings ?? new ChainedSettings[0]) {
                settings.Save();
            }
            try {
                this.Settings.Save();
            } catch(System.Configuration.ConfigurationException) { }
        }
        /// <summary>
        /// 設定を以前のバージョンから最新のバージョンに更新します
        /// </summary>
        public virtual void Upgrade() {
            foreach(ChainedSettings settings in this.SubordinateSettings ?? new ChainedSettings[0]) {
                settings.Upgrade();
            }
            try {
                this.Settings.Upgrade();
            } catch(System.Configuration.ConfigurationException) { }
        }
        /// <summary>
        /// オブジェクト作成時に同時にUpgradeを呼びます
        /// </summary>
        protected ChainedSettings() {
            this.Upgrade();
        }
        public virtual void Initialize() {
            // コンストラクタがあるからここでは何もしない
        }
    }
    public class MotionDataHandlerSettings : ChainedSettings {
        protected override System.Configuration.ApplicationSettingsBase Settings {
            get { return MotionDataHandler.Properties.Settings.Default; }
        }
        private MotionDataHandlerSettings() : base() { }
        private static MotionDataHandlerSettings _singleton = new MotionDataHandlerSettings();
        public static MotionDataHandlerSettings Singleton { get { return _singleton; } }

        protected override IList<ChainedSettings> SubordinateSettings {
            get { return null; }
        }
    }
}
