using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing;
using Microsoft.DirectX.Direct3D;

namespace MotionDataHandler.Motion {
    /// <summary>
    /// DirectX用のカメラ情報を保持するクラス
    /// </summary>
    public class DxCamera {
        readonly object _lockSetCameraPosition = new object();
        readonly object _lockSetView = new object();
        /// <summary>
        /// カメラの視野角．縦と横の内大きいほう．
        /// </summary>
        float _fieldOfView;
        /// <summary>
        /// カメラ視野のアスペクト比
        /// </summary>
        float _aspectRatio;
        /// <summary>
        /// カメラの縦の視野角
        /// </summary>
        float _sightAngle;
        /// <summary>
        /// スクリーンのサイズ．縦と横の内大きいほう
        /// </summary>
        float _viewSize;

        private Viewport _viewport;
        /// <summary>
        /// DirectXに渡すViewportを取得します
        /// </summary>
        public Viewport Viewport { get { return _viewport; } }
        /// <summary>
        /// 最も近いZ平面の値を取得または設定します．
        /// </summary>
        public float ZnearPlane;
        /// <summary>
        /// 最も遠いZ平面の値を取得または設定します．
        /// </summary>
        public float ZfarPlane;
        /// <summary>
        /// カメラの視野角を取得または設定します．
        /// </summary>
        public float FieldOfView {
            get { return _fieldOfView; }
            set {
                lock(_lockSetView) {
                    _fieldOfView = value;
                    if(this.ViewRectangle.Width == 0 || this.ViewRectangle.Height == 0) {
                        return;
                    }
                    if(this.ViewRectangle.Height > this.ViewRectangle.Width) {
                        _sightAngle = _fieldOfView;
                        _viewSize = this.ViewRectangle.Height;
                    } else {
                        _sightAngle = _fieldOfView * this.ViewRectangle.Height / this.ViewRectangle.Width;
                        _viewSize = this.ViewRectangle.Height;
                    }
                    _aspectRatio = (float)this.ViewRectangle.Width / this.ViewRectangle.Height;
                }
            }
        }
        [XmlIgnore]
        public Rectangle ViewRectangle {
            get { return new Rectangle(_viewport.X, _viewport.Y, _viewport.Width, _viewport.Height); }
            set {
                if(value.Width < 0) {
                    int width = -value.Width;
                    value.X = value.Right;
                    value.Width = width;
                }
                if(value.Height < 0) {
                    int height = -value.Height;
                    value.Y = value.Bottom;
                    value.Height = height;
                }
                _viewport.X = value.X;
                _viewport.Y = value.Y;
                _viewport.Width = value.Width;
                _viewport.Height = value.Height;
            }
        }
        /// <summary>
        /// スクリーンのサイズを設定します．
        /// </summary>
        /// <param name="width">幅．ピクセル単位</param>
        /// <param name="height">高さ．ピクセル単位</param>
        public void SetViewSize(int width, int height) {
            lock(_lockSetView) {
                _viewport.Width = width;
                _viewport.Height = height;
                FieldOfView = FieldOfView;
            }
        }
        /// <summary>
        /// スクリーンのサイズを設定します
        /// </summary>
        /// <param name="size">サイズ</param>
        public void SetViewSize(Size size) {
            SetViewSize(size.Width, size.Height);
        }
        /// <summary>
        /// スクリーンのサイズのうち大きいほうを返します．
        /// </summary>
        public float ViewLongEdge {
            get { return _viewSize; }
        }


        /// <summary>
        /// カメラ座標からスクリーン座標へ変換する行列を求めます．
        /// </summary>
        /// <param name="leftHanded">左手系ならばtrue</param>
        /// <returns></returns>
        public Matrix PerspectiveFov(bool leftHanded) {
            lock(_lockSetCameraPosition) {
                if(leftHanded) {
                    return Matrix.PerspectiveFovLH(_sightAngle, _aspectRatio, ZnearPlane, ZfarPlane);
                } else {
                    return Matrix.PerspectiveFovRH(_sightAngle, _aspectRatio, ZnearPlane, ZfarPlane);
                }
            }
        }
        /// <summary>
        /// カメラの空間座標
        /// </summary>
        Vector3 _cameraPosition;
        /// <summary>
        /// 注視点の空間座標
        /// </summary>
        Vector3 _cameraTarget;
        /// <summary>
        /// カメラの上方ベクトル
        /// </summary>
        Vector3 _cameraUpVector;

        /// <summary>
        /// カメラの視点の位置を取得または設定します。
        /// </summary>
        public Vector3 Position {
            get { return _cameraPosition; }
            set { _cameraPosition = value; }
        }
        /// <summary>
        /// カメラの注視点の位置を取得または設定します。
        /// </summary>
        public Vector3 TargetPosition {
            get { return _cameraTarget; }
            set { _cameraTarget = value; }
        }
        /// <summary>
        /// 視点から注視点までの相対ベクトルを取得します。設定するにはSetSightLineを呼び出します。
        /// </summary>
        public Vector3 SightLine {
            get { lock(_lockSetCameraPosition) { return _cameraTarget - _cameraPosition; } }
        }
        /// <summary>
        /// カメラの向きの単位ベクトルを返します。
        /// </summary>
        /// <returns>単位ベクトル</returns>
        public Vector3 SightDirection() {
            lock(_lockSetCameraPosition) {
                return Vector3.Normalize(_cameraTarget - _cameraPosition);
            }
        }
        /// <summary>
        /// カメラのSightDirectionとUpVectorの外積の単位ベクトルを返します。
        /// </summary>
        /// <returns>単位ベクトル</returns>
        public Vector3 RightDirection() {
            lock(_lockSetCameraPosition) {
                return Vector3.Normalize(Vector3.Cross(this.SightLine, UpVector));
            }
        }

        /// <summary>
        /// カメラのSightDirectionとRightDirectionの外積の単位ベクトルを返します。
        /// </summary>
        /// <returns>単位ベクトル</returns>
        public Vector3 UpDirection() {
            lock(_lockSetCameraPosition) {
                return Vector3.Normalize(Vector3.Cross(this.RightDirection(), this.SightLine));
            }
        }

        /// <summary>
        /// カメラの上方向のベクトルを取得または設定します。
        /// </summary>
        public Vector3 UpVector {
            get { return _cameraUpVector; }
            set { _cameraUpVector = value; normalizeUpVector(); }
        }
        /// <summary>
        /// ワールド座標からカメラ座標へ変換する行列を求めます．
        /// </summary>
        /// <param name="leftHanded">左手系ならばtrue</param>
        /// <returns></returns>
        public Matrix LookAt(bool leftHanded) {
            if(leftHanded) {
                return Matrix.LookAtLH(_cameraPosition, _cameraTarget, _cameraUpVector);
            } else {
                return Matrix.LookAtRH(_cameraPosition, _cameraTarget, _cameraUpVector);
            }
        }

        private DxCamera() { }
        /// <summary>
        /// 既定のコンストラクタ
        /// </summary>
        /// <param name="width">スクリーンの幅</param>
        /// <param name="height">スクリーンの高さ</param>
        public DxCamera(int width, int height) {
            _viewport.Width = width;
            _viewport.Height = height;
            _viewport.MinZ = 0f;
            _viewport.MaxZ = 1f;
            _viewport.X = _viewport.Y = 0;
            this.FieldOfView = (float)Math.PI / 3;

            _cameraTarget = Vector3.Empty;
            _cameraPosition = new Vector3(100, 0, 0);
            _cameraUpVector = new Vector3(0, 1, 0);
        }


        /// <summary>
        /// カメラの注視点の位置を設定します。
        /// </summary>
        /// <param title="target">カメラの注視点の位置</param>
        /// <param title="keepPosition">Positionの値を保持します。さもなければ、Targetの値を保持します。</param>
        public void SetTarget(Vector3 target, bool keepPosition) {
            lock(_lockSetCameraPosition) {
                if(keepPosition) {
                    _cameraTarget = target;
                } else {
                    Vector3 direction = _cameraTarget - _cameraPosition;
                    _cameraTarget = target;
                    _cameraPosition = target - direction;
                }
            }
        }
        /// <summary>
        /// カメラの向きを設定します。
        /// </summary>
        /// <param title="sightLine">カメラの向き</param>
        /// <param title="keepTarget">Targetの値を保持します。さもなければ、Positionの値を保持します。</param>
        public void SetSightLine(Vector3 sightLine, bool keepTarget) {
            lock(_lockSetCameraPosition) {
                if(keepTarget) {
                    _cameraPosition = _cameraTarget - sightLine;
                } else {
                    _cameraTarget = _cameraPosition + sightLine;
                }
            }
        }
        /// <summary>
        /// カメラの視点の位置を設定します。
        /// </summary>
        /// <param title="position">カメラの視点の位置</param>
        /// <param title="keepDirection">Directionの値を保持します。さもなければ、Targetの値を保持します。</param>
        public void SetPosition(Vector3 position, bool keepDirection) {
            lock(_lockSetCameraPosition) {
                if(keepDirection) {
                    Vector3 direction = _cameraTarget - _cameraPosition;
                    _cameraPosition = position;
                    _cameraTarget = position + direction;
                } else {
                    _cameraPosition = position;
                }
            }
        }

        /// <summary>
        /// 上方向ベクトルをカメラ方向と直行させて単位ベクトルにします
        /// </summary>
        private void normalizeUpVector() {
            lock(_lockSetCameraPosition) {
                Vector3 dir = Vector3.Normalize(_cameraTarget - _cameraPosition);
                Vector3 up = _cameraUpVector - Vector3.Dot(_cameraUpVector, dir) * dir;
                if(up == Vector3.Empty) {
                    Vector3 ex1 = new Vector3(1, 0, 0) - Vector3.Dot(new Vector3(1, 0, 0), dir) * dir;
                    Vector3 ex2 = new Vector3(0, 1, 0) - Vector3.Dot(new Vector3(0, 1, 0), dir) * dir;
                    if(ex1.LengthSq() > ex2.LengthSq()) {
                        up = ex1;
                    } else {
                        up = ex2;
                    }
                }
                _cameraUpVector = Vector3.Normalize(up);
            }
        }

        /// <summary>
        /// カメラを平行移動します。
        /// </summary>
        /// <param title="offset">移動量</param>
        public void Offset(Vector3 offset) {
            lock(_lockSetCameraPosition) {
                _cameraPosition += offset;
                _cameraTarget += offset;
            }
        }

        /// <summary>
        /// カメラに行列をかけて位置を変化させます
        /// </summary>
        /// <param name="sourceMatrix"></param>
        public void Transform(Matrix sourceMatrix) {
            lock(_lockSetCameraPosition) {
                _cameraPosition.TransformCoordinate(sourceMatrix);
                _cameraTarget.TransformCoordinate(sourceMatrix);
                _cameraUpVector.TransformNormal(sourceMatrix);
            }
        }

        /// <summary>
        /// 視線ベクトルに行列をかけて向きを変化させます．
        /// </summary>
        /// <param name="sourceMatrix"></param>
        /// <param name="keepTarget">trueの場合，視点を変更します．falseの場合は，注視点を変更します</param>
        public void TransformSightLine(Matrix sourceMatrix, bool keepTarget) {
            lock(_lockSetCameraPosition) {
                Vector3 line = this.SightLine;
                line.TransformNormal(sourceMatrix);
                this.SetSightLine(line, keepTarget);
                _cameraUpVector.TransformNormal(sourceMatrix);
                _cameraUpVector.Normalize();
            }
        }

        /// <summary>
        /// 視線ベクトルをスケールします
        /// </summary>
        /// <param name="ratio"></param>
        /// <param name="keepTarget">trueの場合，視点を変更します．falseの場合は，注視点を変更します</param>
        public void ScaleSightLine(float ratio, bool keepTarget) {
            lock(_lockSetCameraPosition) {
                SetSightLine(SightLine * ratio, keepTarget);
            }
        }

        /// <summary>
        /// カメラ情報を保存します
        /// </summary>
        /// <param name="stream"></param>
        public void Serialize(Stream stream) {
            lock(_lockSetCameraPosition) {
                XmlSerializer sel = new XmlSerializer(typeof(DxCamera));
                sel.Serialize(stream, this);
            }
        }

        /// <summary>
        /// カメラ情報を復元します
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static DxCamera Deserialize(Stream stream) {
            XmlSerializer sel = new XmlSerializer(typeof(DxCamera));
            return (DxCamera)sel.Deserialize(stream);
        }

        /// <summary>
        /// オブジェクト座標の点をスクリーン座標に投影します．
        /// </summary>
        /// <param name="point"></param>
        /// <param name="world"></param>
        /// <param name="leftHanded"></param>
        /// <returns></returns>
        public Vector3 Project(Vector3 point, Matrix world, bool leftHanded) {
            return Vector3.Project(point, _viewport, this.PerspectiveFov(leftHanded), this.LookAt(leftHanded), world);
        }

        /// <summary>
        /// ワールド座標の点をスクリーン座標に投影します．
        /// </summary>
        /// <param name="point"></param>
        /// <param name="world"></param>
        /// <param name="leftHanded"></param>
        /// <returns></returns>
        public Vector3 Project(Vector3 point, bool leftHanded) {
            return this.Project(point, Matrix.Identity, leftHanded);
        }

        /// <summary>
        /// スクリーン座標の点をオブジェクト座標に投影します．
        /// </summary>
        /// <param name="point"></param>
        /// <param name="world"></param>
        /// <param name="leftHanded"></param>
        /// <returns></returns>
        public Vector3 Unproject(Vector3 point, Matrix world, bool leftHanded) {
            return Vector3.Unproject(point, _viewport, this.PerspectiveFov(leftHanded), this.LookAt(leftHanded), world);
        }

        /// <summary>
        /// スクリーン座標の点をワールド座標に投影します．
        /// </summary>
        /// <param name="point"></param>
        /// <param name="leftHanded"></param>
        /// <returns></returns>
        public Vector3 Unproject(Vector3 point, bool leftHanded) {
            return this.Unproject(point, Matrix.Identity, leftHanded);
        }

        /// <summary>
        /// スクリーン座標の点からワールド座標内のベクトルに変換します
        /// </summary>
        /// <param name="point">スクリーン座標の点</param>
        /// <param name="leftHanded"></param>
        /// <returns></returns>
        public Vector3 GetClickedDirection(Vector2 point, bool leftHanded) {
            Vector3 tmp = new Vector3(point.X, point.Y, (this.Viewport.MinZ + _viewport.MaxZ) / 2);
            Vector3 tmp2 = Unproject(tmp, leftHanded);
            return Vector3.Normalize(tmp2 - this.Position);
        }

        /// <summary>
        /// スクリーン座標の点からワールド空間内の線分に変換します．端点はそれぞれカメラ座標の手前の面及び奥の面上にあります．
        /// </summary>
        /// <param name="point">スクリーン座標の点</param>
        /// <param name="leftHanded"></param>
        /// <returns></returns>
        public Vector3 GetClickedPositionAndDirection(Vector2 point, bool leftHanded, out Vector3 sightLine) {
            Vector3 dir = GetClickedDirection(point, leftHanded);
            sightLine = dir * (ZfarPlane - ZnearPlane);
            return this.Position + dir * ZnearPlane;
        }
    }
}