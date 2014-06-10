using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
namespace MotionDataHandler.Misc {
    /// <summary>
    /// デバッグ用エラー記録およびメッセージ表示クラス
    /// </summary>
    public class ErrorLogger {
        readonly static object _lock = new object();
        readonly static DateTime _at = DateTime.Now;
        public static string LogDirectory { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MotionDataHandler"); } }
        /// <summary>
        /// 例外情報をファイルに記録します
        /// </summary>
        /// <param name="ex">例外オブジェクト</param>
        /// <param name="message">追加されるメッセージ</param>
        public static void Log(Exception ex, string message) {
            try {
                lock(_lock) {
                    string dir = LogDirectory;
                    string root = Path.GetDirectoryName(dir);
                    if(!Directory.Exists(root)) {
                        Directory.CreateDirectory(root);
                    }
                    if(!Directory.Exists(dir)) {
                        Directory.CreateDirectory(dir);
                    }
                    string path = Path.Combine(dir, string.Format(".log-{0}{1}{2}-{3}{4}{5}.txt", _at.Year.ToString("D4"), _at.Month.ToString("D2"), _at.Day.ToString("D2"), _at.Hour.ToString("D2"), _at.Minute.ToString("D2"), _at.Second.ToString("D2")));
                    using(FileStream stream = new FileStream(path, FileMode.Append)) {
                        XmlWriterSettings setting = new XmlWriterSettings();
                        setting.Indent = true;
                        using(XmlWriter writer = XmlWriter.Create(stream, setting)) {
                            writer.WriteStartElement("Log");
                            writer.WriteElementString("Date", DateTime.Now.ToLongDateString());
                            writer.WriteElementString("Time", DateTime.Now.ToLongTimeString());
                            writer.WriteElementString("AppMessage", message);
                            writer.WriteStartElement("Exception");
                            writer.WriteCData(ex.ToString());
                            writer.WriteEndElement();
                            writer.WriteElementString("ExceptionType", ex.GetType().ToString());
                            writer.WriteElementString("Message", ex.Message);
                            writer.WriteElementString("Source", ex.Source);
                            writer.WriteStartElement("StackTrace");
                            writer.WriteCData(ex.StackTrace);
                            writer.WriteEndElement();
                            writer.WriteElementString("MethodModule", ex.TargetSite.Module.FullyQualifiedName);

                            writer.WriteElementString("MethodName", ex.TargetSite.Name);
                            writer.WriteStartElement("Data");
                            foreach(var k in ex.Data.Keys) {
                                writer.WriteElementString("Key", k.ToString());
                                writer.WriteElementString("Value", ex.Data[k].ToString());
                            }
                            writer.WriteEndElement();
#if DEBUG
                        writer.WriteStartElement("StackTrace");
                        try {
                            StackTrace trace = new StackTrace(true);
                            foreach (var frame in trace.GetFrames()) {
                                writer.WriteStartElement("StackFrame");
                                try {
                                    writer.WriteElementString("MethodName", frame.GetMethod().Name);
                                    writer.WriteElementString("MethodModule", frame.GetMethod().Module.FullyQualifiedName);
                                    writer.WriteElementString("FilePath", frame.GetFileName());
                                    writer.WriteElementString("FileLine", frame.GetFileLineNumber().ToString());
                                } catch (Exception) { }
                                writer.WriteEndElement();
                            }
                        } catch { }
                        writer.WriteEndElement();
#endif
                            writer.WriteEndElement();
                        }
                    }
                }
            } catch { }
        }
        /// <summary>
        /// 例外情報をダイアログに表示，ファイルに記録します
        /// </summary>
        /// <param name="ex">例外オブジェクト</param>
        /// <param name="message">追加されるメッセージ</param>
        public static void Tell(Exception ex, string message) {
            MessageBox.Show(string.Format("{0}{1}{2}", message, Environment.NewLine, ex.Message), ex.Source);
            Log(ex, message);
        }
        /// <summary>
        /// 例外情報をツールチップに表示，ファイルに記録します
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="ex">例外オブジェクト</param>
        /// <param name="message">追加されるメッセージ</param>
        public static void TellToolTip(Exception ex, string message, IWin32Window control) {
            ToolTip tip = new ToolTip();
            tip.IsBalloon = true;
            tip.Show(string.Format("{0}{1}{2}", message, Environment.NewLine, ex.Message), control, 3000);
            Log(ex, message);
        }
    }
}
