using System;
using System.Collections.Generic;
using System.Text;

namespace MotionDataHandler.DataIO {
    /// <summary>
    /// モーションデータの相互変換のためのクラス
    /// </summary>
    public static class FrameConverter {
        public static PhaseSpaceFrame GetMotionFrame(TrcFrame frame) {
            PhaseSpaceFrame ret = new PhaseSpaceFrame();
            ret.Markers = new PhaseSpaceMarker[frame.Markers.Length];
            for (int i = 0; i < ret.Markers.Length; i++) {
                if (frame.Markers[i].HasValue) {
                    float x = 0f, y = 0f, z = 0f;
                    if (float.TryParse(frame.Markers[i].Value.X, out x)
                        && float.TryParse(frame.Markers[i].Value.Y, out y)
                        && float.TryParse(frame.Markers[i].Value.Z, out z)) {
                        ret.Markers[i] = new PhaseSpaceMarker(5, x, y, z);
                    } else {
                        ret.Markers[i] = new PhaseSpaceMarker(-1, 0, 0, 0);
                    }

                } else {
                    ret.Markers[i] = new PhaseSpaceMarker(-1, 0, 0, 0);
                }
            }
            ret.Time = frame.Time;
            return ret;
        }
        public static TrcFrame GetTrcFrame(PhaseSpaceFrame frame, int number) {
            TrcFrame ret = new TrcFrame();
            ret.Markers = new TrcMarker?[frame.Markers.Length];
            for (int i = 0; i < ret.Markers.Length; i++) {
                if (frame.Markers[i].Condition > 0) {
                    ret.Markers[i] = new TrcMarker(frame.Markers[i].X.ToString("R"),
                        frame.Markers[i].Y.ToString("R"),
                        frame.Markers[i].Z.ToString("R"));
                }
            }
            ret.Number = number;
            ret.Time = frame.Time;
            return ret;
        }
    }
}
