using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.DirectX;
using System.Drawing;
using System.Threading;
using System.Collections.ObjectModel;
using Microsoft.DirectX.Direct3D;

namespace MotionDataHandler.Motion {
    using Misc;

    public abstract class MotionObject : ICloneable {
        /// <summary>
        /// カメラ空間でのz距離を求めます．
        /// </summary>
        /// <param name="position">カメラの位置</param>
        /// <param name="direction">カメラの向き．正規化したものを渡す</param>
        /// <param name="minDistance">カメラとオブジェクトの，カメラの向きに関する距離の最小値を返す</param>
        /// <param name="maxDistance">カメラとオブジェクトの，カメラの向きに関する距離の最大値を返す</param>
        /// <returns></returns>
        public abstract bool GetDistanceDirectional(Vector3 position, Vector3 direction, out float minDistance, out float maxDistance);
        /// <summary>
        /// このオブジェクト上で指定された点に最も近い点の座標を返します
        /// </summary>
        /// <param name="position">点の座標</param>
        /// <returns></returns>
        public abstract Vector3 GetNearestFrom(Vector3 position);
        /// <summary>
        /// オブジェクトの重心位置を取得します．
        /// </summary>
        public abstract Vector3 GravityPoint { get; }
        /// <summary>
        /// オブジェクトのデータをバイナリ形式でシリアライズします
        /// </summary>
        /// <param name="writer"></param>
        public abstract void WriteBinary(BinaryWriter writer);
        /// <summary>
        /// バイナリ形式でシリアライズされたデータを読み込みます．
        /// </summary>
        /// <param name="reader"></param>
        public abstract void ReadBinary(BinaryReader reader);
        /// <summary>
        /// オブジェクトを複製します．
        /// </summary>
        /// <returns></returns>
        public abstract Object Clone();
        /// <summary>
        /// オブジェクトと線分の衝突検出をします．
        /// </summary>
        /// <param name="lineSource">線分の開始位置</param>
        /// <param name="lineDestination">線分の終了位置</param>
        /// <param name="enlargement">オブジェクトを拡大する量</param>
        /// <param name="distanceFromSource">線分の開始位置から衝突点までの距離</param>
        /// <returns></returns>
        public abstract bool DetectLineCollision(Vector3 lineSource, Vector3 lineDestination, float enlargement, out float distanceFromSource);
        /// <summary>
        /// オブジェクトを描画するためのポリゴンデータを作成して返します
        /// </summary>
        /// <param name="color"></param>
        /// <param name="camera"></param>
        /// <param name="hint"></param>
        /// <returns></returns>
        public virtual IList<PolygonDatum> Render(int color, DxCamera camera, PolygonRenderHint hint) {
            // とりあえずはてなを描画
            IList<PolygonDatum> ret = new List<PolygonDatum>();
            Vector2[] hatena1 = new Vector2[]{
              new Vector2(104,84),
              new Vector2(66,82),
              new Vector2(108,68),
              new Vector2(84,38),
              new Vector2(120,58),
              new Vector2(118,22),
              new Vector2(146,60),
              new Vector2(158,26),
              new Vector2(154,64),
              new Vector2(188,46),
              new Vector2(160,80),
              new Vector2(198,80),
              new Vector2(152,94),
              new Vector2(186,114),
              new Vector2(128,118),
              new Vector2(156,144),
              new Vector2(110,158),
              new Vector2(150,160),
              new Vector2(110,176),
              new Vector2(146,176),
            };

            Vector2[] hatena2 = new Vector2[]{
              new Vector2(128,214),
              new Vector2(128,190),
              new Vector2(146,202),
              new Vector2(154,220),
              new Vector2(144,234),
              new Vector2(128,240),
              new Vector2(112,234),
              new Vector2(106,216),
              new Vector2(112,200),
              new Vector2(128,190),
            };

            Vector3 right = camera.RightDirection() * hint.MarginSize * 2;
            Vector3 down = -camera.UpDirection() * hint.MarginSize * 2;
            List<PolygonVertex> vertices = new List<PolygonVertex>();
            for(int i = 0; i < hatena1.Length; i++) {
                float x = (hatena1[i].X - 128f) / 128f;
                float y = (hatena1[i].Y - 128f) / 128f;
                vertices.Add(new PolygonVertex(this.GravityPoint + right * x + down * y, -camera.SightDirection(), color));
            }
            ret.Add(new PolygonDatum(PolygonType.TriangleStrip, vertices));

            vertices = new List<PolygonVertex>();
            for(int i = 0; i < hatena2.Length; i++) {
                float x = (hatena2[i].X - 128f) / 128f;
                float y = (hatena2[i].Y - 128f) / 128f;
                vertices.Add(new PolygonVertex(this.GravityPoint + right * x + down * y, -camera.SightDirection(), color));
            }
            ret.Add(new PolygonDatum(PolygonType.TriangleFan, vertices));

            return ret;
        }

        public virtual IList<PolygonDatum> RenderSelectionMark(int color, DxCamera camera, PolygonRenderHint hint) {
            IList<PolygonDatum> ret = new List<PolygonDatum>();
            Vector3 right = camera.RightDirection() * hint.MarginSize * 2;
            Vector3 down = -camera.UpDirection() * hint.MarginSize * 2;
            ret.Add(new PolygonDatum(PolygonType.LineList,
                        new PolygonVertex(this.GravityPoint + right - down + camera.SightDirection(), -camera.SightDirection(), color),
                        new PolygonVertex(this.GravityPoint + right + down + camera.SightDirection(), -camera.SightDirection(), color),
                        new PolygonVertex(this.GravityPoint - right + down + camera.SightDirection(), -camera.SightDirection(), color),
                        new PolygonVertex(this.GravityPoint - right - down + camera.SightDirection(), -camera.SightDirection(), color),
                        new PolygonVertex(this.GravityPoint + right - down + camera.SightDirection(), -camera.SightDirection(), color)));
            return ret;
        }

        /// <summary>
        /// オブジェクトが指定された型であるか，そのサブクラスである場合にtrueを返します．
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsTypeOf(Type type) {
            Type thisType = this.GetType();
            return thisType == type || thisType.IsSubclassOf(type);
        }

        public abstract MotionObject InterpolateLinear(MotionObject at1, float interpolater);
        public abstract bool HasArea();
        public abstract MotionObject CloneOffsetObject(Vector3 offset);
        public virtual Bitmap GetIcon() {
            return global::MotionDataHandler.Properties.Resources.question;
        }
    }

    public class PointObject : MotionObject {
        /// <summary>
        /// 点オブジェクトの座標を取得または設定します
        /// </summary>
        public Vector3 Position;
        public PointObject() { }
        public PointObject(Vector3 position) {
            this.Position = position;
        }
        public override bool GetDistanceDirectional(Vector3 position, Vector3 direction, out float minDistance, out float maxDistance) {
            if(direction == Vector3.Empty) {
                minDistance = maxDistance = 0;
                return false;
            }
            Vector3 diff = this.Position - position;
            minDistance = maxDistance = Vector3.Dot(direction, diff);
            return true;
        }
        public override Vector3 GravityPoint {
            get { return this.Position; }
        }
        public override void WriteBinary(BinaryWriter writer) {
            VectorEx.WriteVector3(writer, this.Position);
        }
        public override void ReadBinary(BinaryReader reader) {
            this.Position = VectorEx.ReadVector3(reader);
        }
        public override object Clone() {
            return new PointObject(this.Position);
        }

        /// <summary>
        /// 描画用の頂点座標
        /// </summary>
        static Vector3[] vertexDirections;
        /// <summary>
        /// 描画用の頂点インデックス
        /// </summary>
        static int[][] primitiveIndices;
        /// <summary>
        /// 描画用のプリミティブタイプ
        /// </summary>
        static PolygonType[] primitiveTypes;
        /// <summary>
        /// 描画用データ初期化用のロックオブジェクト
        /// </summary>
        static object _lockDrawPrimitive = new object();

        public override IList<PolygonDatum> Render(int color, DxCamera camera, PolygonRenderHint hint) {
            IList<PolygonDatum> ret = new List<PolygonDatum>();
            lock(_lockDrawPrimitive) {
                if(vertexDirections == null) {
                    vertexDirections = new Vector3[12];
                    vertexDirections[0] = new Vector3(0, -1, 0);
                    vertexDirections[11] = new Vector3(0, 1, 0);
                    float z = (float)Math.Sin(Math.PI / 6);
                    float s = (float)Math.Cos(Math.PI / 6);
                    for(int i = 0; i < 5; i++) {
                        float x = (float)Math.Cos(Math.PI * 2 * i / 5);
                        float y = (float)Math.Sin(Math.PI * 2 * i / 5);
                        vertexDirections[i + 1] = new Vector3(s * x, -z, s * y);
                        vertexDirections[i + 6] = new Vector3(-s * x, z, -s * y);
                    }
                    primitiveIndices = new int[][] {
                        new int[] { 0, 1, 5, 4, 3, 2, 1 },
                        new int[] { 1, 8, 5, 7, 4, 6, 3, 10, 2, 9, 1, 8 },
                        new int[] { 11, 10, 6, 7, 8, 9, 10 }
                    };
                    primitiveTypes = new PolygonType[] { PolygonType.TriangleFan, PolygonType.TriangleStrip, PolygonType.TriangleFan };
                }
            }
            for(int i = 0; i < primitiveIndices.Length; i++) {
                List<PolygonVertex> vertices = new List<PolygonVertex>();
                for(int j = 0; j < primitiveIndices[i].Length; j++) {
                    vertices.Add(new PolygonVertex(this.Position + vertexDirections[primitiveIndices[i][j]] * hint.MarginSize, vertexDirections[primitiveIndices[i][j]], color));
                }
                ret.Add(new PolygonDatum(primitiveTypes[i], vertices));
            }
            return ret;

        }

        public override IList<PolygonDatum> RenderSelectionMark(int color, DxCamera camera, PolygonRenderHint hint) {
            IList<PolygonDatum> ret = new List<PolygonDatum>();
            Vector3 front = camera.SightDirection();
            Vector3 vert1 = camera.UpDirection();
            Vector3 vert2 = camera.RightDirection();
            const int div = 32;
            List<PolygonVertex> vertices = new List<PolygonVertex>();
            for(int i = 0; i < div + 2; i++) {
                float sin = (float)Math.Sin(Math.PI * 2 * i / div);
                float cos = (float)Math.Cos(Math.PI * 2 * i / div);
                Vector3 dir = vert2 * cos + vert1 * sin;
                vertices.Add(new PolygonVertex(this.Position + dir * hint.MarginSize * (i % 2 == 0 ? 1.5f : 1f), -front, color));
            }
            ret.Add(new PolygonDatum(PolygonType.TriangleStrip, vertices));
            return ret;
        }
        public override bool DetectLineCollision(Vector3 lineSource, Vector3 lineDestination, float enlargement, out float distanceFromSource) {
            // |E + aD - C| = R
            // D^2 PermutationEnumeratorAux^2 + 2D(E-C)PermutationEnumeratorAux + (E-C)^2 - R^2 = 0
            distanceFromSource = 0;

            Vector3 diff = lineSource - this.Position;
            Vector3 lineEdge = lineDestination - lineSource;
            float radius = enlargement;
            float coefQuadric = lineEdge.LengthSq();
            float coefLinear = 2f * Vector3.Dot(lineEdge, diff);
            float coefConst = diff.LengthSq() - radius * radius;
            QuadraticEquation qe = QuadraticEquation.Solve(coefQuadric, coefLinear, coefConst);
            if(qe.Answers.Length > 0) {
                foreach(var ans in qe.Answers) {
                    if(ans >= 0 && ans <= 1) {
                        distanceFromSource = ans * lineEdge.Length();
                        return true;
                    }
                }
            }
            return false;
        }
        public override MotionObject InterpolateLinear(MotionObject at1, float interpolater) {
            PointObject o0 = this;
            PointObject o1 = at1 as PointObject;
            if(o0 == null || o1 == null)
                return null;
            return new PointObject(Vector3.Lerp(o0.Position, o1.Position, interpolater));
        }

        public override bool HasArea() {
            return false;
        }

        public override Vector3 GetNearestFrom(Vector3 position) {
            return this.Position;
        }

        public override MotionObject CloneOffsetObject(Vector3 offset) {
            return new PointObject(this.Position + offset);
        }
        public override Bitmap GetIcon() {
            return global::MotionDataHandler.Properties.Resources.obj_point;
        }
    }

    public class LineObject : MotionObject {
        /// <summary>
        /// 線分の始点座標を取得または設定します
        /// </summary>
        public Vector3 Position;
        /// <summary>
        /// 線分の視点から終点の相対座標を取得または設定します
        /// </summary>
        public Vector3 Edge;
        /// <summary>
        /// 線分の向きを表す単位ベクトルを返します
        /// </summary>
        /// <returns></returns>
        public Vector3 Direction() { return Vector3.Normalize(this.Edge); }
        /// <summary>
        /// 線分の長さを返します
        /// </summary>
        /// <returns></returns>
        public float Length() { return this.Edge.Length(); }
        /// <summary>
        /// 線分の終端を返します
        /// </summary>
        public Vector3 AnotherEnd { get { return this.Position + this.Edge; } }
        public LineObject() { }
        public LineObject(Vector3 position, Vector3 edge) {
            this.Position = position;
            this.Edge = edge;
        }
        public override object Clone() {
            return new LineObject(this.Position, this.Edge);
        }
        public override bool GetDistanceDirectional(Vector3 position, Vector3 direction, out float minDistance, out float maxDistance) {
            if(direction == Vector3.Empty) {
                minDistance = maxDistance = 0;
                return false;
            }
            float distance1 = Vector3.Dot(this.Position - position, direction);
            float distance2 = Vector3.Dot(this.AnotherEnd - position, direction);
            minDistance = Math.Min(distance1, distance2);
            maxDistance = Math.Max(distance1, distance2);
            return true;
        }
        public override Vector3 GravityPoint {
            get { return this.Position + this.Edge * 0.5f; }
        }
        public override void WriteBinary(BinaryWriter writer) {
            VectorEx.WriteVector3(writer, this.Position);
            VectorEx.WriteVector3(writer, this.Edge);
        }
        public override void ReadBinary(BinaryReader reader) {
            this.Position = VectorEx.ReadVector3(reader);
            this.Edge = VectorEx.ReadVector3(reader);
        }
        public override IList<PolygonDatum> Render(int color, DxCamera camera, PolygonRenderHint hint) {
            IList<PolygonDatum> ret = new List<PolygonDatum>();
            Vector3 front = camera.SightDirection();
            Vector3 direction = this.Direction();
            Vector3 offset = this.GravityPoint - camera.Position;
            Vector3 widen = Vector3.Normalize(Vector3.Cross(offset, direction));

            // 線分
            ret.Add(new PolygonDatum(PolygonType.LineList,
                        new PolygonVertex(this.Position, -front, color),
                        new PolygonVertex(this.AnotherEnd, -front, color)));
            // 矢尻の表と裏
            ret.Add(new PolygonDatum(PolygonType.TriangleList,
                        new PolygonVertex(this.AnotherEnd, -front, color),
                        new PolygonVertex(this.AnotherEnd + (widen - direction) * hint.MarginSize, -front, color),
                        new PolygonVertex(this.AnotherEnd + (-widen - direction) * hint.MarginSize, -front, color),
                        new PolygonVertex(this.AnotherEnd, -front, color),
                        new PolygonVertex(this.AnotherEnd + (-widen - direction) * hint.MarginSize, -front, color),
                        new PolygonVertex(this.AnotherEnd + (widen - direction) * hint.MarginSize, -front, color)));
            return ret;
        }

        public override IList<PolygonDatum> RenderSelectionMark(int color, DxCamera camera, PolygonRenderHint hint) {
            IList<PolygonDatum> ret = new List<PolygonDatum>();

            Vector3 front = camera.SightDirection();
            Vector3 direction = this.Direction();
            Vector3 offset = this.GravityPoint - camera.Position;
            Vector3 widen = Vector3.Normalize(Vector3.Cross(offset, direction));

            // 帯の表
            ret.Add(new PolygonDatum(PolygonType.TriangleStrip,
                        new PolygonVertex(this.Position + widen * 0.5f * hint.MarginSize, -front, color),
                        new PolygonVertex(this.Position - widen * 0.5f * hint.MarginSize, -front, color),
                        new PolygonVertex(this.AnotherEnd + (-direction * 2f + widen * 0.5f) * hint.MarginSize, -front, color),
                        new PolygonVertex(this.AnotherEnd + (-direction * 2f - widen * 0.5f) * hint.MarginSize, -front, color)));
            // 帯の裏
            ret.Add(new PolygonDatum(PolygonType.TriangleStrip,
                        new PolygonVertex(this.Position + widen * 0.5f * hint.MarginSize, -front, color),
                        new PolygonVertex(this.AnotherEnd + (-direction * 2f + widen * 0.5f) * hint.MarginSize, -front, color),
                        new PolygonVertex(this.Position - widen * 0.5f * hint.MarginSize, -front, color),
                        new PolygonVertex(this.AnotherEnd + (-direction * 2f - widen * 0.5f) * hint.MarginSize, -front, color)));
            // 大きな矢尻の表裏
            ret.Add(new PolygonDatum(PolygonType.TriangleList,
                        new PolygonVertex(this.AnotherEnd, -front, color),
                        new PolygonVertex(this.AnotherEnd + (widen - direction) * hint.MarginSize * 2f, -front, color),
                        new PolygonVertex(this.AnotherEnd + (-widen - direction) * hint.MarginSize * 2f, -front, color),
                        new PolygonVertex(this.AnotherEnd, -front, color),
                        new PolygonVertex(this.AnotherEnd + (-widen - direction) * hint.MarginSize * 2f, -front, color),
                        new PolygonVertex(this.AnotherEnd + (widen - direction) * hint.MarginSize * 2f, -front, color)));
            return ret;
        }
        public override bool DetectLineCollision(Vector3 lineSource, Vector3 lineDestination, float enlargement, out float distanceFromSource) {
            Vector3 edgeDir = Vector3.Normalize(this.Edge);
            return GeometryCalc.GetCollisionCylinder(lineSource, lineDestination, this.Position - edgeDir * enlargement, this.AnotherEnd + edgeDir * enlargement, enlargement, out distanceFromSource);
        }
        public override MotionObject InterpolateLinear(MotionObject at1, float interpolater) {
            LineObject o0 = this;
            LineObject o1 = at1 as LineObject;
            if(o0 == null || o1 == null)
                return null;
            return new LineObject(Vector3.Lerp(o0.Position, o1.Position, interpolater), Vector3.Lerp(o0.Edge, o1.Edge, interpolater));
        }
        public override bool HasArea() {
            return false;
        }
        public override Vector3 GetNearestFrom(Vector3 position) {
            Vector3 toAnchor = position - this.Position;
            Vector3 lineDir = this.Edge;
            Vector3 normalLineDir = Vector3.Normalize(lineDir);
            float length = lineDir.Length();
            float toAnchorVertLen = Vector3.Dot(toAnchor, normalLineDir);
            if(toAnchorVertLen >= 0 && toAnchorVertLen < length) {
                Vector3 toAnchorHoriz = toAnchor - normalLineDir * toAnchorVertLen;
                return position - toAnchorHoriz;
            }
            if((this.Position - position).LengthSq() < (this.AnotherEnd - position).LengthSq()) {
                return this.Position;
            } else {
                return this.AnotherEnd;
            }
        }
        public override MotionObject CloneOffsetObject(Vector3 offset) {
            return new LineObject(this.Position + offset, this.Edge);
        }
        public override Bitmap GetIcon() {
            return global::MotionDataHandler.Properties.Resources.obj_line;
        }
    }

    public class CylinderObject : MotionObject {
        /// <summary>
        /// 円筒の底面の中心座標を取得または設定します
        /// </summary>
        public Vector3 Position;
        /// <summary>
        /// 円筒の中心軸のベクトルを取得または設定します
        /// </summary>
        public Vector3 Axis;
        private float _radius;
        /// <summary>
        /// 円筒の半径を取得または設定します
        /// </summary>
        public float Radius {
            get { return _radius; }
            set {
                if(value < 0)
                    value = -value;
                _radius = value;
            }
        }
        /// <summary>
        /// 中心軸の向きを表すベクトルを返します
        /// </summary>
        /// <returns></returns>
        public Vector3 AxisDirection() { return Vector3.Normalize(this.Axis); }
        /// <summary>
        /// 中心軸の長さを返します
        /// </summary>
        /// <returns></returns>
        public float AxisLength() { return this.Axis.Length(); }
        /// <summary>
        /// 円筒の他の底面の中心座標を取得または設定します
        /// </summary>
        public Vector3 AnotherEnd { get { return this.Position + this.Axis; } }
        public CylinderObject() { }
        public CylinderObject(Vector3 position, Vector3 axis, float radius) {
            this.Position = position;
            this.Axis = axis;
            this.Radius = radius;
        }
        public override object Clone() {
            return new CylinderObject(this.Position, this.Axis, this.Radius);
        }
        public override bool GetDistanceDirectional(Vector3 position, Vector3 direction, out float minDistance, out float maxDistance) {
            if(direction == Vector3.Empty) {
                minDistance = maxDistance = 0;
                return false;
            }
            Vector3 axisDirection = this.AxisDirection();
            Vector3 forward = (direction - Vector3.Dot(axisDirection, direction) * axisDirection) * this.Radius;
            List<float> candidate = new List<float>();

            candidate.Add(Vector3.Dot(direction, this.Position - forward - position));
            candidate.Add(Vector3.Dot(direction, this.Position + forward - position));
            candidate.Add(Vector3.Dot(direction, this.AnotherEnd - forward - position));
            candidate.Add(Vector3.Dot(direction, this.AnotherEnd + forward - position));
            minDistance = candidate.Min();
            maxDistance = candidate.Max();
            return true;
        }
        public override Vector3 GravityPoint {
            get { return this.Position + this.Axis * 0.5f; }
        }
        public override void WriteBinary(BinaryWriter writer) {
            VectorEx.WriteVector3(writer, this.Position);
            VectorEx.WriteVector3(writer, this.Axis);
            writer.Write(this.Radius);
        }
        public override void ReadBinary(BinaryReader reader) {
            this.Position = VectorEx.ReadVector3(reader);
            this.Axis = VectorEx.ReadVector3(reader);
            this.Radius = reader.ReadSingle();
        }

        public override IList<PolygonDatum> Render(int color, DxCamera camera, PolygonRenderHint hint) {
            IList<PolygonDatum> ret = new List<PolygonDatum>();

            Vector3 normalDir = camera.SightDirection();
            Vector3 axisDir = this.AxisDirection();
            Vector3 front = camera.SightDirection();
            const int div = 32;
            Dictionary<int, Vector2> circleEdge = new Dictionary<int, Vector2>();
            for(int i = 0; i <= div; i++) {
                circleEdge[i] = new Vector2((float)Math.Cos(Math.PI * 2 * i / div), (float)Math.Sin(Math.PI * 2 * i / div));
            }
            Vector3 vert = Vector3.Cross(axisDir, front);
            vert = Vector3.Normalize(vert - Vector3.Dot(vert, axisDir) * axisDir);
            Vector3 vert2 = Vector3.Cross(vert, axisDir);
            var dirSet = new[]{
                new { B = Vector3.Empty, A = this.Axis, Dir = -1 },
                new { B = this.Axis, A = Vector3.Empty, Dir = 1 },
            };
            foreach(var param in dirSet) {
                List<PolygonVertex> vertices = new List<PolygonVertex>();
                for(int i = 0; i <= div; i++) {
                    Vector3 outer = vert2 * circleEdge[i].Y + vert * circleEdge[i].X;
                    vertices.Add(new PolygonVertex(this.Position + param.B + outer * this.Radius, Vector3.Normalize(param.Dir * outer), color));
                    vertices.Add(new PolygonVertex(this.Position + param.A + outer * this.Radius, Vector3.Normalize(param.Dir * outer), color));
                }
                ret.Add(new PolygonDatum(PolygonType.TriangleStrip, vertices));
            }
            return ret;

        }

        public override IList<PolygonDatum> RenderSelectionMark(int color, DxCamera camera, PolygonRenderHint hint) {
            IList<PolygonDatum> ret = new List<PolygonDatum>();

            Vector3 normalDir = camera.SightDirection();
            Vector3 axisDir = this.AxisDirection();
            Vector3 front = camera.SightDirection();

            Vector3 halfAxis = this.Axis * 0.5f;
            Vector3 center = this.Position + halfAxis;
            Vector3 axisVert = Vector3.Normalize(Vector3.Cross(front, this.Axis)) * this.Radius;
            Vector3 axisDirNormal = Vector3.Normalize(axisDir) * hint.MarginSize;
            Vector3 axisVertNormal = Vector3.Normalize(axisVert) * hint.MarginSize;
            Vector3[] dirSet2 = new Vector3[]{
                 halfAxis + axisVert,  halfAxis + axisVert + axisDirNormal + axisVertNormal,
                -halfAxis + axisVert, -halfAxis + axisVert - axisDirNormal + axisVertNormal,
                -halfAxis - axisVert, -halfAxis - axisVert - axisDirNormal - axisVertNormal,
                 halfAxis - axisVert,  halfAxis - axisVert + axisDirNormal - axisVertNormal,
                 halfAxis + axisVert,  halfAxis + axisVert + axisDirNormal + axisVertNormal
            };
            List<PolygonVertex> vertices = new List<PolygonVertex>();
            foreach(var dir in dirSet2) {
                vertices.Add(new PolygonVertex(center + dir, -front, color));
            }
            ret.Add(new PolygonDatum(PolygonType.TriangleStrip, vertices));
            return ret;

        }
        public override bool DetectLineCollision(Vector3 lineSource, Vector3 lineDestination, float enlargement, out float distanceFromSource) {
            Vector3 axisEnlargement = Vector3.Normalize(this.Axis) * enlargement;
            return GeometryCalc.GetCollisionCylinder(lineSource, lineDestination, this.Position - axisEnlargement, this.AnotherEnd + axisEnlargement, this.Radius + enlargement, out distanceFromSource);
        }
        public override MotionObject InterpolateLinear(MotionObject at1, float interpolater) {
            CylinderObject o0 = this;
            CylinderObject o1 = at1 as CylinderObject;
            if(o0 == null || o1 == null)
                return null;
            return new CylinderObject(Vector3.Lerp(o0.Position, o1.Position, interpolater), Vector3.Lerp(o0.Axis, o1.Axis, interpolater), o1.Radius * interpolater + o0.Radius * (1f - interpolater));
        }
        public override bool HasArea() {
            return true;
        }
        public override Vector3 GetNearestFrom(Vector3 position) {
            Vector3 toAnchor = position - this.Position;
            Vector3 normalAxis = this.AxisDirection();
            float axisLength = this.AxisLength();
            float toAnchorVertLen = Vector3.Dot(toAnchor, normalAxis);
            if(toAnchorVertLen >= 0 && toAnchorVertLen < axisLength) {
                Vector3 toAnchorHoriz = toAnchor - normalAxis * toAnchorVertLen;
                return position - toAnchorHoriz + Vector3.Normalize(toAnchorHoriz) * this.Radius;
            }
            Vector3 location1, location2;
            float distance1 = GeometryCalc.GetDistanceWithCircle(position, this.Position, normalAxis * this.Radius, out location1);
            float distance2 = GeometryCalc.GetDistanceWithCircle(position, this.AnotherEnd, normalAxis * this.Radius, out location2);
            if(distance1 < distance2) {
                return location1;
            } else {
                return location2;
            }
        }
        public override MotionObject CloneOffsetObject(Vector3 offset) {
            return new CylinderObject(this.Position + offset, this.Axis, this.Radius);
        }
        public override Bitmap GetIcon() {
            return global::MotionDataHandler.Properties.Resources.obj_cylinder;
        }
    }

    public class SphereObject : MotionObject {
        /// <summary>
        /// 球の中心座標を取得または設定します
        /// </summary>
        public Vector3 Position;
        private float _radius;
        /// <summary>
        /// 球の半径を取得または設定します
        /// </summary>
        public float Radius {
            get { return _radius; }
            set {
                if(value < 0)
                    value = -value;
                _radius = value;
            }
        }
        public SphereObject() { }
        public SphereObject(Vector3 position, float radius) {
            this.Position = position;
            this.Radius = radius;
        }
        public override object Clone() {
            return new SphereObject(this.Position, this.Radius);
        }
        public override bool GetDistanceDirectional(Vector3 position, Vector3 direction, out float minDistance, out float maxDistance) {
            if(direction == Vector3.Empty) {
                minDistance = maxDistance = 0;
                return false;
            }
            float d = Vector3.Dot(direction, this.Position - position);
            minDistance = d - this.Radius;
            maxDistance = d + this.Radius;
            return true;
        }
        public override void WriteBinary(BinaryWriter writer) {
            VectorEx.WriteVector3(writer, this.Position);
            writer.Write(this.Radius);
        }
        public override void ReadBinary(BinaryReader reader) {
            this.Position = VectorEx.ReadVector3(reader);
            this.Radius = reader.ReadSingle();
        }
        public override Vector3 GravityPoint {
            get { return this.Position; }
        }
        public override IList<PolygonDatum> Render(int color, DxCamera camera, PolygonRenderHint hint) {
            IList<PolygonDatum> ret = new List<PolygonDatum>();

            const int slice = 16;
            const int stack = 16;
            Dictionary<int, Vector2> circleEdge = new Dictionary<int, Vector2>();
            for(int i = 0; i <= slice; i++) {
                circleEdge[i] = new Vector2((float)Math.Cos(Math.PI * 2 * i / slice), (float)Math.Sin(Math.PI * 2 * i / slice));
            }
            float yDelta = Radius * 2 / stack;
            for(int i = 0; i < stack; i++) {
                Vector3 vOffset1 = new Vector3(0, Radius - yDelta * i, 0);
                Vector3 vOffset2 = new Vector3(0, Radius - yDelta * (i + 1), 0);
                float hRadius1 = Radius * (float)Math.Sqrt(1f - Math.Pow((2f * i - stack) / stack, 2));
                float hRadius2 = Radius * (float)Math.Sqrt(1f - Math.Pow((2f * (i + 1) - stack) / stack, 2));
                List<PolygonVertex> vertices = new List<PolygonVertex>();
                for(int j = 0; j <= slice; j++) {
                    float sin = circleEdge[j].Y;
                    float cos = circleEdge[j].X;
                    Vector3 posOffset1 = vOffset1 + new Vector3(cos * hRadius1, 0, sin * hRadius1);
                    Vector3 posOffset2 = vOffset2 + new Vector3(cos * hRadius2, 0, sin * hRadius2);
                    vertices.Add(new PolygonVertex(this.Position + posOffset1, Vector3.Normalize(posOffset1), color));
                    vertices.Add(new PolygonVertex(this.Position + posOffset2, Vector3.Normalize(posOffset2), color));
                }
                ret.Add(new PolygonDatum(PolygonType.TriangleStrip, vertices));
            }
            return ret;
        }
        public override IList<PolygonDatum> RenderSelectionMark(int color, DxCamera camera, PolygonRenderHint hint) {
            IList<PolygonDatum> ret = new List<PolygonDatum>();

            Vector3 front = camera.SightDirection();
            Vector3 vert1 = camera.UpDirection();
            Vector3 vert2 = camera.RightDirection();
            const int div = 32;
            List<PolygonVertex> vertices = new List<PolygonVertex>();
            for(int i = 0; i < div + 2; i++) {
                float sin = (float)Math.Sin(Math.PI * 2 * i / div);
                float cos = (float)Math.Cos(Math.PI * 2 * i / div);
                Vector3 dir = vert2 * cos + vert1 * sin;
                vertices.Add(new PolygonVertex(this.Position + dir * (this.Radius + (i % 2 == 0 ? hint.MarginSize : 0)), -front, color));
            }
            ret.Add(new PolygonDatum(PolygonType.TriangleStrip, vertices));
            return ret;
        }
        public override bool DetectLineCollision(Vector3 lineSource, Vector3 lineDestination, float enlargement, out float distanceFromSource) {
            return GeometryCalc.GetCollisionSphere(lineSource, lineDestination, this.Position, this.Radius + enlargement, out distanceFromSource);
        }
        public override MotionObject InterpolateLinear(MotionObject at1, float interpolater) {
            SphereObject o0 = this;
            SphereObject o1 = at1 as SphereObject;
            if(o0 == null || o1 == null)
                return null;
            return new SphereObject(Vector3.Lerp(o0.Position, o1.Position, interpolater), o1.Radius * interpolater + o0.Radius * (1f - interpolater));
        }
        public override bool HasArea() {
            return true;
        }
        public override Vector3 GetNearestFrom(Vector3 position) {
            Vector3 toAnchor = position - this.Position;
            if(toAnchor.Length() < this.Radius) {
                return position;
            } else {
                return this.Position + Vector3.Normalize(toAnchor) * this.Radius;
            }
        }
        public override MotionObject CloneOffsetObject(Vector3 offset) {
            return new SphereObject(this.Position + offset, this.Radius);
        }
        public override Bitmap GetIcon() {
            return global::MotionDataHandler.Properties.Resources.obj_sphere;
        }
    }
    public class PlaneObject : MotionObject, IXmlSerializable {
        private Vector3[] _points = new Vector3[0];
        /// <summary>
        /// 平面の外周の点の座標を取得または設定します
        /// </summary>
        public Vector3[] Points {
            get { return _points; }
            set {
                if(value == null)
                    value = new Vector3[0];
                _points = value;
            }
        }
        public PlaneObject() { }
        public PlaneObject(IEnumerable<Vector3> points) {
            if(points == null)
                throw new ArgumentNullException("points", "'points' cannot be null");
            this.Points = points.ToArray();
        }

        public override object Clone() {
            return new PlaneObject(this.Points);
        }

        public override bool GetDistanceDirectional(Vector3 position, Vector3 direction, out float minDistance, out float maxDistance) {
            minDistance = maxDistance = 0;
            if(direction == Vector3.Empty) {
                return false;
            }
            List<float> candidate = (from point in this.Points select Vector3.Dot(direction, point - position)).ToList();
            if(candidate.Count == 0) {
                return false;
            }
            minDistance = candidate.Min();
            maxDistance = candidate.Max();
            if(minDistance < 0 && maxDistance > 0) {
                minDistance = 0;
            }
            return true;
        }

        public decimal GetDimensions() {
            decimal ret = 0;
            for(int i = 0; i < this.Points.Length - 2; i++) {
                float a = (this.Points[i + 1] - this.Points[i]).Length();
                float b = (this.Points[i + 2] - this.Points[i]).Length();
                float c = (this.Points[i + 2] - this.Points[i + 1]).Length();
                double s = (double)(a + b + c) / 2;
                double dim = Math.Sqrt(s * (s - a) * (s - b) * (s - c));
                ret += (decimal)dim;
            }
            return ret;
        }

        #region IXmlSerializable

        public System.Xml.Schema.XmlSchema GetSchema() {
            throw new System.NotImplementedException();
        }

        public void ReadXml(XmlReader reader) {
            if(reader.IsEmptyElement) {
                reader.Skip();
                return;
            }
            reader.ReadStartElement(this.GetType().Name);
            while(reader.NodeType != XmlNodeType.None) {
                if(reader.NodeType == XmlNodeType.EndElement)
                    break;
                switch(reader.Name) {
                case "Points":
                    string lengthStr;
                    if(reader.IsEmptyElement || (lengthStr = reader.GetAttribute("Length")) == null) {
                        reader.Skip();
                    } else {
                        reader.ReadStartElement("Points");
                        int length;
                        if(int.TryParse(lengthStr, out length) && length >= 3) {
                            this.Points = new Vector3[length];
                            string[] values = reader.ReadString().Split('\t');
                            for(int i = 0; i < length; i++) {
                                Vector3 tmp;
                                if(VectorEx.TryParse(values, i * 3, out tmp)) {
                                    this.Points[i] = tmp;
                                } else {
                                    break;
                                }
                            }
                        }
                        reader.ReadEndElement();
                    }
                    break;
                default:
                    reader.Skip();
                    break;
                }
            }
            reader.ReadEndElement();
        }
        public void WriteXml(XmlWriter writer) {
            if(this.Points == null)
                return;
            writer.WriteStartElement("Points");
            writer.WriteStartAttribute("Length");
            writer.WriteValue(Points.Length);
            writer.WriteEndAttribute();
            foreach(var point in this.Points) {
                writer.WriteString(string.Format("{0}\t{1}\t{2}\t", point.X, point.Y, point.Z));
            }
            writer.WriteEndElement();
        }


        #endregion

        /// <summary>
        /// バイナリデータからオブジェクトを読み込みます。
        /// </summary>
        /// <param title="reader">読み込み元</param>
        public override void ReadBinary(BinaryReader reader) {
            this.Points = new Vector3[reader.ReadInt32()];
            for(int i = 0; i < this.Points.Length; i++) {
                this.Points[i] = VectorEx.ReadVector3(reader);
            }
        }

        /// <summary>
        /// オブジェクトをバイナリデータとして書き込みます。
        /// </summary>
        /// <param title="writer">書き込み先</param>
        public override void WriteBinary(BinaryWriter writer) {
            writer.Write(this.Points.Length);
            for(int i = 0; i < Points.Length; i++) {
                VectorEx.WriteVector3(writer, this.Points[i]);
            }
        }

        public override Vector3 GravityPoint {
            get {
                Vector3 ret = Vector3.Empty;
                if(this.Points != null && this.Points.Length > 0)
                    ret = this.Points.Aggregate((x, y) => x + y) * (1f / this.Points.Length);
                return ret;
            }
        }
        public override IList<PolygonDatum> Render(int color, DxCamera camera, PolygonRenderHint hint) {
            IList<PolygonDatum> ret = new List<PolygonDatum>();
            if(this.Points == null) {
                return ret;
            }

            Vector3 front = camera.SightDirection();
            List<PolygonVertex> vertices = new List<PolygonVertex>();
            // 最後の点で折り返す。法線の向きを正しくするために最後の点は3回参照される。
            for(int i = 0; i < this.Points.Length * 2 + 1; i++) {
                int index = i;
                if(index >= this.Points.Length) {
                    if(index >= this.Points.Length + 2) {
                        // 折り返し
                        index = this.Points.Length * 2 - i;
                    } else {
                        // 折り返し地点
                        index = this.Points.Length - 1;
                    }
                }
                // 表の向きを求める
                Vector3 normal = Vector3.Empty;
                for(int k = 0; k < 3; k++) {
                    if(index + k - 2 < 0)
                        continue;
                    if(index + k >= this.Points.Length)
                        continue;
                    var dir = Vector3.Normalize(Vector3.Cross(this.Points[index + k - 2] - this.Points[index + k - 1], this.Points[index + k] - this.Points[index + k - 1]));
                    if(i >= Points.Length)
                        dir = -dir;
                    if((i + k) % 2 == 0) {
                        normal += dir;
                    } else {
                        normal -= dir;
                    }
                }
                vertices.Add(new PolygonVertex(this.Points[index], Vector3.Normalize(normal), color));
            }
            ret.Add(new PolygonDatum(PolygonType.TriangleStrip, vertices));
            return ret;
        }
        public override IList<PolygonDatum> RenderSelectionMark(int color, DxCamera camera, PolygonRenderHint hint) {
            IList<PolygonDatum> ret = new List<PolygonDatum>();
            if(this.Points == null) {
                return ret;
            }

            Vector3 front = camera.SightDirection();
            for(int i = 0; i < this.Points.Length - 1; i++) {
                Vector3 diff = this.Points[i + 1] - this.Points[i];
                Vector3 normal = Vector3.Normalize(Vector3.Cross(front, diff)) * hint.MarginSize;
                ret.Add(new PolygonDatum(PolygonType.TriangleStrip,
                                new PolygonVertex(this.Points[i] - normal, -front, color),
                                new PolygonVertex(this.Points[i + 1] - normal, -front, color),
                                new PolygonVertex(this.Points[i] + normal, -front, color),
                                new PolygonVertex(this.Points[i + 1] + normal, -front, color)));
            }
            return ret;
        }
        public override bool DetectLineCollision(Vector3 lineSource, Vector3 lineDestination, float enlargement, out float distanceFromSource) {
            bool ret = false;
            distanceFromSource = 0;
            for(int i = 0; i < this.Points.Length - 2; i++) {
                float tmpDistance;
                Vector3 point1 = this.Points[i];
                Vector3 point2 = this.Points[i + 1];
                Vector3 point3 = this.Points[i + 2];
                if(enlargement != 0) {
                    // 三角形の各辺をenlargementだけ外側に大きくする計算

                    Vector3 diff12 = point2 - point1;
                    Vector3 diff23 = point3 - point2;
                    Vector3 diff31 = point1 - point3;
                    // 三角形のなす平面に垂直なベクトル
                    Vector3 vertical = Vector3.Normalize(Vector3.Cross(diff12, diff31));
                    // 三角形の外向きのベクトル: 三角形のなす平面上で，各辺と垂直なベクトル
                    Vector3 out12 = Vector3.Normalize(Vector3.Cross(diff12, vertical));
                    Vector3 out23 = Vector3.Normalize(Vector3.Cross(diff23, vertical));
                    Vector3 out31 = Vector3.Normalize(Vector3.Cross(diff31, vertical));
                    // 内向きだったら外向きに直す
                    if(Vector3.Dot(out12, diff23) > 1)
                        out12 = -out12;
                    if(Vector3.Dot(out23, diff31) > 1)
                        out23 = -out23;
                    if(Vector3.Dot(out31, diff12) > 1)
                        out31 = -out31;
                    // 拡張した角を求める 
                    Vector3 out1 = out12 + out31;
                    Vector3 out2 = out23 + out12;
                    Vector3 out3 = out31 + out23;
                    try {
                        out1 = Vector3.Normalize(out1) * (2f / out1.Length());
                        out2 = Vector3.Normalize(out2) * (2f / out2.Length());
                        out3 = Vector3.Normalize(out3) * (2f / out3.Length());
                    } catch(ArithmeticException) {
                    }
                    point1 += out1 * enlargement;
                    point2 += out2 * enlargement;
                    point3 += out3 * enlargement;
                }
                if(GeometryCalc.GetCollisionTriangle(lineSource, lineDestination, point1, point2, point3, out tmpDistance)) {
                    if(ret) {
                        if(distanceFromSource > tmpDistance)
                            distanceFromSource = tmpDistance;
                    } else {
                        ret = true;
                        distanceFromSource = tmpDistance;
                    }
                }
            }
            return ret;
        }
        public override MotionObject InterpolateLinear(MotionObject at1, float interpolater) {
            PlaneObject o0 = this;
            PlaneObject o1 = at1 as PlaneObject;
            if(o0 == null || o1 == null || o0.Points.Length != o1.Points.Length)
                return null;
            return new PlaneObject(o0.Points.Select((p0, i) => Vector3.Lerp(p0, o1.Points[i], interpolater)));
        }
        public override bool HasArea() {
            return true;
        }
        public override Vector3 GetNearestFrom(Vector3 position) {
            float distance = float.MaxValue;
            Vector3 nearestPosition = Vector3.Empty;
            for(int i = 0; i < this.Points.Length - 2; i++) {
                Vector3 testLocation;
                float testDistance = GeometryCalc.GetDistanceWithTriangle(position, this.Points[i], this.Points[i + 1], this.Points[i + 2], out testLocation);
                if(testDistance < distance) {
                    distance = testDistance;
                    nearestPosition = testLocation;
                }
            }
            return nearestPosition;
        }
        public override MotionObject CloneOffsetObject(Vector3 offset) {
            return new PlaneObject(this.Points.Select(p => p + offset));
        }
        public override Bitmap GetIcon() {
            return global::MotionDataHandler.Properties.Resources.obj_plane;
        }
    }

    public class BoxObject : MotionObject {
        public Vector3 BaseVertex;
        public Vector3 Edge1, Edge2, Edge3;
        public BoxObject() {
        }
        public BoxObject(Vector3 baseVertex, Vector3 edge1, Vector3 edge2, Vector3 edge3) {
            this.BaseVertex = baseVertex;
            this.Edge1 = edge1;
            this.Edge2 = edge2;
            this.Edge3 = edge3;
        }

        public override bool GetDistanceDirectional(Vector3 position, Vector3 direction, out float minDistance, out float maxDistance) {
            minDistance = float.MaxValue;
            maxDistance = float.MinValue;
            bool found = false;
            for(int i = 0; i < 2; i++) {
                for(int j = 0; j < 2; j++) {
                    for(int k = 0; k < 2; k++) {
                        Vector3 vertex = this.BaseVertex + this.Edge1 * i + this.Edge2 * j + this.Edge3 * k;
                        Vector3 toVertex = vertex - position;
                        float distance = Vector3.Dot(direction, toVertex);
                        if(distance > 0) {
                            found = true;
                            if(maxDistance < distance)
                                maxDistance = distance;
                            if(minDistance > distance)
                                minDistance = distance;
                        }
                    }
                }
            }
            return found;
        }

        public override Vector3 GetNearestFrom(Vector3 position) {
            float minDistance = float.MaxValue;
            Vector3 ret = position;
            for(int i = 0; i < 2; i++) {
                Vector3 location;
                float distance;
                distance = GeometryCalc.GetDistanceWithParallelogram(position, this.BaseVertex + this.Edge1 * i, this.Edge2, this.Edge3, out location);
                if(distance < minDistance) {
                    minDistance = distance;
                    ret = location;
                }
                distance = GeometryCalc.GetDistanceWithParallelogram(position, this.BaseVertex + this.Edge2 * i, this.Edge3, this.Edge1, out location);
                if(distance < minDistance) {
                    minDistance = distance;
                    ret = location;
                }
                distance = GeometryCalc.GetDistanceWithParallelogram(position, this.BaseVertex + this.Edge3 * i, this.Edge1, this.Edge2, out location);
                if(distance < minDistance) {
                    minDistance = distance;
                    ret = location;
                }
            }
            return ret;
        }

        public override Vector3 GravityPoint {
            get { return this.BaseVertex + 0.5f * (this.Edge1 + this.Edge2 + this.Edge3); }
        }

        public override void WriteBinary(BinaryWriter writer) {
            VectorEx.WriteVector3(writer, this.BaseVertex);
            VectorEx.WriteVector3(writer, this.Edge1);
            VectorEx.WriteVector3(writer, this.Edge2);
            VectorEx.WriteVector3(writer, this.Edge3);
        }

        public override void ReadBinary(BinaryReader reader) {
            this.BaseVertex = VectorEx.ReadVector3(reader);
            this.Edge1 = VectorEx.ReadVector3(reader);
            this.Edge2 = VectorEx.ReadVector3(reader);
            this.Edge3 = VectorEx.ReadVector3(reader);
        }

        public override object Clone() {
            return new BoxObject(this.BaseVertex, this.Edge1, this.Edge2, this.Edge3);
        }

        public override MotionObject InterpolateLinear(MotionObject at1, float interpolater) {
            BoxObject o0 = this;
            BoxObject o1 = at1 as BoxObject;
            if(o0 == null || o1 == null)
                return null;
            return new BoxObject(Vector3.Lerp(o0.BaseVertex, o1.BaseVertex, interpolater), Vector3.Lerp(o0.Edge1, o1.Edge1, interpolater), Vector3.Lerp(o0.Edge2, o1.Edge2, interpolater), Vector3.Lerp(o0.Edge3, o1.Edge3, interpolater));
        }

        public override bool HasArea() {
            return true;
        }
        public override bool DetectLineCollision(Vector3 lineSource, Vector3 lineDestination, float enlargement, out float distanceFromSource) {
            distanceFromSource = float.MaxValue;
            bool found = false;
            for(int i = 0; i < 2; i++) {
                float distance;
                if(GeometryCalc.GetCollisionParallelogram(lineSource, lineDestination, this.BaseVertex + this.Edge1 * i, this.Edge2, this.Edge3, out distance)) {
                    found = true;
                    if(distance < distanceFromSource)
                        distanceFromSource = distance;
                }
                if(GeometryCalc.GetCollisionParallelogram(lineSource, lineDestination, this.BaseVertex + this.Edge2 * i, this.Edge3, this.Edge1, out distance)) {
                    found = true;
                    if(distance < distanceFromSource)
                        distanceFromSource = distance;
                }
                if(GeometryCalc.GetCollisionParallelogram(lineSource, lineDestination, this.BaseVertex + this.Edge3 * i, this.Edge1, this.Edge2, out distance)) {
                    found = true;
                    if(distance < distanceFromSource)
                        distanceFromSource = distance;
                }
            }
            return found;
        }
        public override IList<PolygonDatum> Render(int color, DxCamera camera, PolygonRenderHint hint) {
            IList<PolygonDatum> ret = new List<PolygonDatum>();
            Vector3 baseVertex = this.BaseVertex;
            Vector3 edge1 = this.Edge1;
            Vector3 edge2 = this.Edge2;
            Vector3 edge3 = this.Edge3;
            foreach(var edges in new[] { new Vector3[] { edge1, edge2, edge3 }, new Vector3[] { edge2, edge3, edge1 }, new Vector3[] { edge3, edge1, edge2 } }) {
                for(int i = 0; i < 2; i++) {
                    Vector3 norm = -camera.SightDirection();
                    Vector3 planeNorm = -Vector3.Normalize(Vector3.Cross(edges[1], edges[2]));
                    // 縁の線
                    ret.Add(new PolygonDatum(PolygonType.LineStrip,
                                        new PolygonVertex(baseVertex + edges[0] * i, norm, color),
                                        new PolygonVertex(baseVertex + edges[0] * i + edges[1], norm, color),
                                        new PolygonVertex(baseVertex + edges[0] * i + edges[1] + edges[2], norm, color),
                                        new PolygonVertex(baseVertex + edges[0] * i + edges[2], norm, color),
                                        new PolygonVertex(baseVertex + edges[0] * i, norm, color)));

                    // どっちが表かわからないので両方描画する
                    ret.Add(new PolygonDatum(PolygonType.TriangleStrip,
                                        new PolygonVertex(baseVertex + edges[0] * i, planeNorm, color),
                                        new PolygonVertex(baseVertex + edges[0] * i + edges[1], planeNorm, color),
                                        new PolygonVertex(baseVertex + edges[0] * i + edges[2], planeNorm, color),
                                        new PolygonVertex(baseVertex + edges[0] * i + edges[1] + edges[2], planeNorm, color)));

                    ret.Add(new PolygonDatum(PolygonType.TriangleStrip,
                                        new PolygonVertex(baseVertex + edges[0] * i, -planeNorm, color),
                                        new PolygonVertex(baseVertex + edges[0] * i + edges[2], -planeNorm, color),
                                        new PolygonVertex(baseVertex + edges[0] * i + edges[1], -planeNorm, color),
                                        new PolygonVertex(baseVertex + edges[0] * i + edges[1] + edges[2], -planeNorm, color)));
                }
            }
            return ret;
        }
        public override IList<PolygonDatum> RenderSelectionMark(int color, DxCamera camera, PolygonRenderHint hint) {
            IList<PolygonDatum> ret = new List<PolygonDatum>();
            Vector3 norm = -camera.SightDirection();
            Vector3 edge1Dir = Vector3.Normalize(this.Edge1);
            Vector3 edge2Dir = Vector3.Normalize(this.Edge2);
            Vector3 edge3Dir = Vector3.Normalize(this.Edge3);

            Vector3 baseVertex = this.BaseVertex - (edge1Dir + edge2Dir + edge3Dir) * hint.MarginSize;
            Vector3 edge1 = this.Edge1 + edge1Dir * hint.MarginSize * 2;
            Vector3 edge2 = this.Edge2 + edge2Dir * hint.MarginSize * 2;
            Vector3 edge3 = this.Edge3 + edge3Dir * hint.MarginSize * 2;
            foreach(var edges in new[] { new Vector3[] { edge1, edge2, edge3 }, new Vector3[] { edge2, edge3, edge1 }, new Vector3[] { edge3, edge1, edge2 } }) {
                for(int i = 0; i < 2; i++) {
                    ret.Add(new PolygonDatum(PolygonType.LineStrip,
                                        new PolygonVertex(this.BaseVertex + edges[0] * i, norm, color),
                                        new PolygonVertex(this.BaseVertex + edges[0] * i + edges[1], norm, color),
                                        new PolygonVertex(this.BaseVertex + edges[0] * i + edges[1] + edges[2], norm, color),
                                        new PolygonVertex(this.BaseVertex + edges[0] * i + edges[2], norm, color),
                                        new PolygonVertex(this.BaseVertex + edges[0] * i, norm, color)));
                }
            }
            return ret;
        }
        public override MotionObject CloneOffsetObject(Vector3 offset) {
            return new BoxObject(this.BaseVertex, this.Edge1, this.Edge2, this.Edge3);
        }
    }
    public class BoxFrameObject : MotionObject {
        public Vector3 BaseVertex;
        public Vector3 Edge1, Edge2, Edge3;
        public BoxFrameObject() {
        }
        public BoxFrameObject(Vector3 baseVertex, Vector3 edge1, Vector3 edge2, Vector3 edge3) {
            this.BaseVertex = baseVertex;
            this.Edge1 = edge1;
            this.Edge2 = edge2;
            this.Edge3 = edge3;
        }

        public override bool GetDistanceDirectional(Vector3 position, Vector3 direction, out float minDistance, out float maxDistance) {
            minDistance = float.MaxValue;
            maxDistance = float.MinValue;
            bool found = false;
            for(int i = 0; i < 2; i++) {
                for(int j = 0; j < 2; j++) {
                    for(int k = 0; k < 2; k++) {
                        Vector3 vertex = this.BaseVertex + this.Edge1 * i + this.Edge2 * j + this.Edge3 * k;
                        Vector3 toVertex = vertex - position;
                        float distance = Vector3.Dot(direction, toVertex);
                        if(distance > 0) {
                            found = true;
                            if(maxDistance < distance)
                                maxDistance = distance;
                            if(minDistance > distance)
                                minDistance = distance;
                        }
                    }
                }
            }
            return found;
        }

        public override Vector3 GetNearestFrom(Vector3 position) {
            float minDistance = float.MaxValue;
            Vector3 ret = position;
            for(int i = 0; i < 2; i++) {
                for(int j = 0; j < 2; j++) {
                    Vector3 pre1 = this.BaseVertex + this.Edge2 * i + this.Edge3 * j;
                    Vector3 pre2 = this.BaseVertex + this.Edge1 * i + this.Edge3 * j;
                    Vector3 pre3 = this.BaseVertex + this.Edge1 * i + this.Edge2 * j;
                    float distance;
                    Vector3 location;
                    distance = GeometryCalc.GetDistancePointAndLine(position, pre1, pre1 + this.Edge1, out location);
                    if(distance < minDistance) {
                        minDistance = distance;
                        ret = location;
                    }
                    distance = GeometryCalc.GetDistancePointAndLine(position, pre2, pre2 + this.Edge2, out location);
                    if(distance < minDistance) {
                        minDistance = distance;
                        ret = location;
                    }
                    distance = GeometryCalc.GetDistancePointAndLine(position, pre3, pre3 + this.Edge3, out location);
                    if(distance < minDistance) {
                        minDistance = distance;
                        ret = location;
                    }
                }
            }
            return ret;
        }

        public override Vector3 GravityPoint {
            get { return this.BaseVertex + 0.5f * (this.Edge1 + this.Edge2 + this.Edge3); }
        }

        public override void WriteBinary(BinaryWriter writer) {
            VectorEx.WriteVector3(writer, this.BaseVertex);
            VectorEx.WriteVector3(writer, this.Edge1);
            VectorEx.WriteVector3(writer, this.Edge2);
            VectorEx.WriteVector3(writer, this.Edge3);
        }

        public override void ReadBinary(BinaryReader reader) {
            this.BaseVertex = VectorEx.ReadVector3(reader);
            this.Edge1 = VectorEx.ReadVector3(reader);
            this.Edge2 = VectorEx.ReadVector3(reader);
            this.Edge3 = VectorEx.ReadVector3(reader);
        }

        public override object Clone() {
            return new BoxFrameObject(this.BaseVertex, this.Edge1, this.Edge2, this.Edge3);
        }

        public override MotionObject InterpolateLinear(MotionObject at1, float interpolater) {
            BoxFrameObject o0 = this;
            BoxFrameObject o1 = at1 as BoxFrameObject;
            if(o0 == null || o1 == null)
                return null;
            return new BoxFrameObject(Vector3.Lerp(o0.BaseVertex, o1.BaseVertex, interpolater), Vector3.Lerp(o0.Edge1, o1.Edge1, interpolater), Vector3.Lerp(o0.Edge2, o1.Edge2, interpolater), Vector3.Lerp(o0.Edge3, o1.Edge3, interpolater));
        }

        public override bool HasArea() {
            return false;
        }
        public override bool DetectLineCollision(Vector3 lineSource, Vector3 lineDestination, float enlargement, out float distanceFromSource) {
            distanceFromSource = float.MaxValue;
            bool found = false;
            Vector3 edge1Dir = Vector3.Normalize(this.Edge1);
            Vector3 edge2Dir = Vector3.Normalize(this.Edge2);
            Vector3 edge3Dir = Vector3.Normalize(this.Edge3);
            Vector3 edge1 = this.Edge1 + edge1Dir * enlargement * 2;
            Vector3 edge2 = this.Edge2 + edge2Dir * enlargement * 2;
            Vector3 edge3 = this.Edge3 + edge3Dir * enlargement * 2;
            for(int i = 0; i < 2; i++) {
                for(int j = 0; j < 2; j++) {
                    Vector3 pre1 = this.BaseVertex + this.Edge2 * i + this.Edge3 * j - edge1Dir * enlargement;
                    Vector3 pre2 = this.BaseVertex + this.Edge1 * i + this.Edge3 * j - edge2Dir * enlargement;
                    Vector3 pre3 = this.BaseVertex + this.Edge1 * i + this.Edge2 * j - edge3Dir * enlargement;
                    float distance;
                    if(GeometryCalc.GetCollisionCylinder(lineSource, lineDestination, pre1, pre1 + edge1, enlargement, out distance)) {
                        found = true;
                        if(distanceFromSource < distance)
                            distanceFromSource = distance;
                    }
                    if(GeometryCalc.GetCollisionCylinder(lineSource, lineDestination, pre2, pre2 + edge2, enlargement, out distance)) {
                        found = true;
                        if(distanceFromSource < distance)
                            distanceFromSource = distance;
                    }
                    if(GeometryCalc.GetCollisionCylinder(lineSource, lineDestination, pre3, pre3 + edge3, enlargement, out distance)) {
                        found = true;
                        if(distanceFromSource < distance)
                            distanceFromSource = distance;
                    }
                }
            }
            return found;
        }
        public override IList<PolygonDatum> Render(int color, DxCamera camera, PolygonRenderHint hint) {
            IList<PolygonDatum> ret = new List<PolygonDatum>();
            Vector3 baseVertex = this.BaseVertex;
            Vector3 edge1 = this.Edge1;
            Vector3 edge2 = this.Edge2;
            Vector3 edge3 = this.Edge3;
            foreach(var edges in new[] { new Vector3[] { edge1, edge2, edge3 }, new Vector3[] { edge2, edge3, edge1 }, new Vector3[] { edge3, edge1, edge2 } }) {
                for(int i = 0; i < 2; i++) {
                    Vector3 norm = -camera.SightDirection();
                    Vector3 planeNorm = -Vector3.Normalize(Vector3.Cross(edges[1], edges[2]));
                    // 縁の線
                    ret.Add(new PolygonDatum(PolygonType.LineStrip,
                    new PolygonVertex(baseVertex + edges[0] * i, norm, color),
                    new PolygonVertex(baseVertex + edges[0] * i + edges[1], norm, color),
                    new PolygonVertex(baseVertex + edges[0] * i + edges[1] + edges[2], norm, color),
                    new PolygonVertex(baseVertex + edges[0] * i + edges[2], norm, color),
                    new PolygonVertex(baseVertex + edges[0] * i, norm, color)));
                }
            }
            return ret;
        }
        public override IList<PolygonDatum> RenderSelectionMark(int color, DxCamera camera, PolygonRenderHint hint) {
            IList<PolygonDatum> ret = new List<PolygonDatum>();
            Vector3 norm = -camera.SightDirection();
            Vector3 edge1Dir = Vector3.Normalize(this.Edge1);
            Vector3 edge2Dir = Vector3.Normalize(this.Edge2);
            Vector3 edge3Dir = Vector3.Normalize(this.Edge3);

            Vector3 edge1 = this.Edge1;
            Vector3 edge2 = this.Edge2;
            Vector3 edge3 = this.Edge3;
            foreach(var edges in new[] { new Vector3[] { edge1, edge2, edge3 }, new Vector3[] { edge2, edge3, edge1 }, new Vector3[] { edge3, edge1, edge2 } }) {
                Vector3[] enlarge = new Vector3[3];
                for(int i = 0; i < 3; i++) {
                    enlarge[i] = Vector3.Normalize(edges[i]) * hint.MarginSize;
                }
                for(int i = 0; i < 2; i++) {
                    Vector3 baseVertex = this.BaseVertex;
                    if(i == 0) {
                        baseVertex -= enlarge[0];
                    } else {
                        baseVertex += edges[0] + enlarge[0];
                    }
                    List<Vector3> frameEdges = new List<Vector3>();
                    frameEdges.Add(baseVertex);
                    frameEdges.Add(baseVertex - enlarge[1] - enlarge[2]);
                    frameEdges.Add(baseVertex + edges[1]);
                    frameEdges.Add(baseVertex + edges[1] + enlarge[1] - enlarge[2]);
                    frameEdges.Add(baseVertex + edges[1] + edges[2]);
                    frameEdges.Add(baseVertex + edges[1] + enlarge[1] + edges[2] + enlarge[2]);
                    frameEdges.Add(baseVertex + edges[2]);
                    frameEdges.Add(baseVertex - enlarge[1] + edges[2] + enlarge[2]);
                    frameEdges.Add(baseVertex);
                    frameEdges.Add(baseVertex - enlarge[1] - enlarge[2]);

                    Vector3[] tmp = frameEdges.ToArray();
                    frameEdges.AddRange(tmp.Reverse().Skip(1));
                    ret.Add(new PolygonDatum(PolygonType.TriangleStrip, (frameEdges.Select(v => new PolygonVertex(v, norm, color)).ToArray())));
                }
            }
            return ret;
        }
        public override MotionObject CloneOffsetObject(Vector3 offset) {
            return new BoxFrameObject(this.BaseVertex + offset, this.Edge1, this.Edge2, this.Edge3);
        }
    }
}
