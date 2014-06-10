using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.DirectX;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace MotionDataHandler.Misc {
    /// <summary>
    /// 三次元ベクトルの拡張クラス
    /// </summary>
    public static class VectorEx {
        public static bool IsFinite(Vector3 vector) {
            return !float.IsInfinity(vector.X) && !float.IsNaN(vector.X)
                && !float.IsInfinity(vector.Y) && !float.IsNaN(vector.Y)
                && !float.IsInfinity(vector.Z) && !float.IsNaN(vector.Z);
        }
        /// <summary>
        /// 三次元ベクトルの一つの法線ベクトルを返します。結果は正規化されます。
        /// </summary>
        /// <param title="lineDir">もととなるベクトル</param>
        /// <returns>法線ベクトル</returns>
        public static Vector3 GetOneOfNormals(Vector3 direction) {
            direction.Normalize();
            Vector3 tmp1 = new Vector3(0, 1, 0);
            Vector3 normal1 = tmp1 - Vector3.Dot(tmp1, direction) * direction;
            Vector3 tmp2 = new Vector3(0, 0, 1);
            Vector3 normal2 = tmp2 - Vector3.Dot(tmp2, direction) * direction;
            Vector3 normal = normal1.LengthSq() > normal2.LengthSq() ? normal1 : normal2;
            return Vector3.Normalize(normal);
        }

        /// <summary>
        /// stringの配列をVector3に変換します。
        /// </summary>
        /// <param title="values">もととなるstringの配列</param>
        /// <param title="offset">配列の変換開始位置</param>
        /// <param title="vec">出力されるVector3</param>
        /// <returns>変換に成功したらtrue</returns>
        public static bool TryParse(string[] values, int offset, out Vector3 vec) {
            if(values.Length < offset + 3) {
                vec = Vector3.Empty;
                return false;
            }
            float x, y, z;
            if(float.TryParse(values[offset + 0], out x)
                && float.TryParse(values[offset + 1], out y)
                && float.TryParse(values[offset + 2], out z)) {
                vec = new Vector3(x, y, z);
                return true;
            }
            vec = Vector3.Empty;
            return false;
        }

        public static Vector3 ReadVector3(System.IO.BinaryReader reader) {
            float x, y, z;
            x = reader.ReadSingle();
            y = reader.ReadSingle();
            z = reader.ReadSingle();
            return new Vector3(x, y, z);
        }

        public static void WriteVector3(System.IO.BinaryWriter writer, Vector3 vec) {
            writer.Write(vec.X);
            writer.Write(vec.Y);
            writer.Write(vec.Z);
        }
    }
}
