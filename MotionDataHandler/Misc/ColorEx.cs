using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MotionDataHandler.Misc {
    public static class ColorEx {
        /// <summary>
        /// HSV色空間からColorオブジェクトを作成します。
        /// </summary>
        /// <param title="h">色相(radian)</param>
        /// <param title="s">彩度(0 to 1)</param>
        /// <param title="value">明度(0 to 1)</param>
        /// <returns></returns>
        public static Color ColorFromHSV(float h, float s, float v) {
            if(s < 0)
                s = 0;
            if(s > 1)
                s = 1;
            if(v < 0)
                v = 0;
            if(v > 1)
                v = 1;
            h %= (float)(Math.PI * 2);
            if(h < 0)
                h += (float)(Math.PI * 2);
            float hTmp = h / (float)(Math.PI / 3);
            int type = (int)(hTmp);
            if(type > 5) {
                type = 5;
                hTmp = 5;
            }
            float f = hTmp - (float)type;
            float p = v * (1f - s);
            float q = v * (1f - f * s);
            float t = v * (1f - (1 - f) * s);
            int V = (int)(255 * v);
            int P = (int)(255 * p);
            int Q = (int)(255 * q);
            int T = (int)(255 * t);
            Color ret = Color.Empty;
            switch(type) {
            case 0:
                ret = Color.FromArgb(V, T, P);
                break;
            case 1:
                ret = Color.FromArgb(Q, V, P);
                break;
            case 2:
                ret = Color.FromArgb(P, V, T);
                break;
            case 3:
                ret = Color.FromArgb(P, Q, V);
                break;
            case 4:
                ret = Color.FromArgb(T, P, V);
                break;
            case 5:
                ret = Color.FromArgb(V, P, Q);
                break;
            }
            return ret;
        }

        /// <summary>
        /// ColorオブジェクトをHSV色空間に変換します。
        /// </summary>
        /// <param title="color">Colorオブジェクト</param>
        /// <param title="h">色相(radian)</param>
        /// <param title="s">彩度(0 to 1)</param>
        /// <param title="value">明度(0 to 1)</param>
        public static void ColorToHSV(Color color, out float h, out float s, out float v) {
            int max, min, delta;
            int offset = 0;
            if(color.R > color.G && color.R > color.B) {
                max = color.R;
                min = color.G > color.B ? color.B : color.G;
                delta = color.G - color.B;
                offset = 0;
            } else if(color.G > color.R && color.G > color.B) {
                max = color.G;
                min = color.R > color.B ? color.B : color.R;
                delta = color.B - color.R;
                offset = 2;
            } else {
                max = color.B;
                min = color.G > color.R ? color.R : color.G;
                delta = color.R - color.G;
                offset = 4;
            }
            if(max - min == 0) {
                h = 0;
            } else {
                h = (float)(Math.PI / 3 * ((double)delta / (max - min) + offset));
                if(h < 0)
                    h += (float)(Math.PI * 2);
            }
            if(max == 0) {
                s = 1;
            } else {
                s = (float)(max - min) / max;
            }
            v = (float)max / 255;
        }
        /// <summary>
        /// 二つの色を加算します
        /// </summary>
        /// <param name="color1"></param>
        /// <param name="color2"></param>
        /// <returns></returns>
        public static Color AddColor(Color color1, Color color2) {
            return Color.FromArgb(Math.Min(color1.R + color2.R, 255), Math.Min(color1.G + color2.G, 255), Math.Min(color1.B + color2.B, 255));
        }
        /// <summary>
        /// 二つの色を減算します
        /// </summary>
        /// <param name="color1"></param>
        /// <param name="color2"></param>
        /// <returns></returns>
        public static Color SubtractColor(Color color1, Color color2) {
            return Color.FromArgb(Math.Max(color1.R - color2.R, 0), Math.Max(color1.G - color2.G, 0), Math.Max(color1.B - color2.B, 0));
        }

        static int getMonochrome(Color color) {
            return (color.R * 30 + color.G * 59 + color.B * 11) / 100;
        }
        /// <summary>
        /// 見やすい背景色を返します
        /// </summary>
        /// <param title="color">描画色</param>
        /// <returns>背景色</returns>
        public static Color GetComplementaryColor(Color color) {
            int mono = getMonochrome(color);
            return mono > 128 ? Color.Black : Color.White;
            float bright = color.GetBrightness();
            if(bright >= 0.3f && bright < 0.7f) {
                if(color.GetSaturation() > 0.75f) {
                    return Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
                }
                return bright > 0.5f ? Color.Black : Color.White;
            }
            return bright > 0.5f ? Color.Black : Color.White;
        }

    }
}
