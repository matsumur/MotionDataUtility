using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MotionDataHandler.Misc {
    public static class StreamEx {
        /// <summary>
        /// ストリームへの書き込みを一時ファイルに行った後，指定されたファイルを上書きします．
        /// </summary>
        /// <param name="fileName">保存先のファイル名</param>
        /// <param name="saveAction">ストリームへの書き込み処理</param>
        public static void SaferSave(string fileName, Action<Stream> saveAction) {
            string tmpPath = fileName + "~";
            string replacePath = fileName + "~~";
            using(Stream stream = new FileStream(tmpPath, FileMode.Create)) {
                saveAction(stream);
            }
            if(File.Exists(fileName)) {
                File.Replace(tmpPath, fileName, replacePath);
                File.Delete(replacePath);
            } else {
                File.Move(tmpPath, fileName);
            }
        }
    }
}
