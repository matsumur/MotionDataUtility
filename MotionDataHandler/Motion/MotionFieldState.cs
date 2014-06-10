using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using System.Diagnostics;

namespace MotionDataHandler.Motion {
    using Misc;
    /// <summary>
    /// モーションデータの空間的な配置情報について保持するクラス
    /// </summary>
    public struct MotionFieldState {
        /// <summary>
        /// 空間が左手系であるかを取得または設定します。
        /// </summary>
        public bool LeftHanded;
        /// <summary>
        /// 床面の中心位置を取得または設定します。
        /// </summary>
        public Vector3 FloorCenter;
        /// <summary>
        /// 床面の上面方向を取得または設定します。
        /// </summary>
        public Vector3 FloorUpper {
            get {
                if (_floorUpper == Vector3.Empty || _floorParallel == Vector3.Empty) {
                    validateFloor();
                }
                return _floorUpper;
            }
            set {
                _floorUpper = value;
                validateFloor();
            }
        }
        /// <summary>
        /// 床面に平行な一つのベクトルを取得または設定します。
        /// </summary>
        public Vector3 FloorParallel {
            get {
                if (_floorParallel == Vector3.Empty || _floorParallel == Vector3.Empty) {
                    validateFloor();
                }
                return _floorParallel;
            }
            set {
                _floorParallel = value;
                validateFloor();
            }
        }
        
        /// <summary>
        /// FloorUpperとFloorParallelに垂直なベクトルを返します．
        /// </summary>
        /// <returns></returns>
        public Vector3 GetFloorSecondaryParallel() {
            return Vector3.Cross(FloorUpper, FloorParallel);
        }

        private Vector3 _floorUpper;
        private Vector3 _floorParallel;

        private void validateFloor() {
            if (_floorUpper == Vector3.Empty) {
                _floorUpper = new Vector3(0, 1, 0);
            }
            if (_floorParallel == Vector3.Empty) {
                _floorParallel = new Vector3(1, 0, 0);
            }
            _floorUpper.Normalize();
            _floorParallel.Normalize();
            float diff = Vector3.Dot(_floorUpper, _floorParallel);
            if (diff == 0) {
                _floorParallel = VectorEx.GetOneOfNormals(_floorUpper);
            } else {
                _floorParallel -= FloorUpper * diff;
                _floorParallel.Normalize();
            }

        }



    }
}
