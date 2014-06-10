using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using System.IO;
using System.Text;
using System.Linq;

namespace MotionDataHandler.Motion {
    using Misc;

    /// <summary>
    /// モーションデータの計算用のクラス
    /// </summary>
    public static class GeometryCalc {

        /// <summary>
        /// 点と三角形ポリゴンとの最短距離を返します
        /// </summary>
        /// <param name="anchorPoint">基準点</param>
        /// <param name="vertex1">三角形の頂点</param>
        /// <param name="vertex2">三角形の頂点</param>
        /// <param name="vertex3">三角形の頂点</param>
        /// <param name="location">基準点に対して最も近い三角形上の点</param>
        /// <returns></returns>
        public static float GetDistanceWithTriangle(Vector3 anchorPoint, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, out Vector3 location) {
            Vector3 edge2 = vertex2 - vertex1;
            Vector3 edge3 = vertex3 - vertex1;
            Vector3 triangleVert = Vector3.Normalize(Vector3.Cross(edge2, edge3));
            Vector3 toAnchor = anchorPoint - vertex1;
            float vertDistance = Vector3.Dot(triangleVert, toAnchor);

            Vector3 testDir = -triangleVert * vertDistance * 2;
            Motion.Old.MotionDataLine vertLine = new Motion.Old.MotionDataLine(anchorPoint, testDir);
            float distance;
            if(GeometryCalc.GetCollisionTriangle(vertLine.End, vertLine.AnotherEnd, vertex1, vertex2, vertex3, out distance)) {
                location = anchorPoint + Vector3.Normalize(testDir) * distance;
                return distance;
            }
            Vector3 location1, location2, location3;
            float distance1 = GeometryCalc.GetDistanceWithLine(anchorPoint, vertex1, vertex2, out location1);
            float distance2 = GeometryCalc.GetDistanceWithLine(anchorPoint, vertex2, vertex3, out location2);
            float distance3 = GeometryCalc.GetDistanceWithLine(anchorPoint, vertex3, vertex1, out location3);
            if(distance1 < distance2 && distance1 < distance3) {
                location = location1;
                return distance1;
            } else if(distance2 < distance1 && distance2 < distance3) {
                location = location2;
                return distance2;
            } else {
                location = location3;
                return distance3;
            }
        }
        /// <summary>
        /// 線分と円筒が交点を持つかを返します．
        /// </summary>
        /// <param name="lineSource">線分の始点</param>
        /// <param name="lineDestination">線分の終点</param>
        /// <param name="end1">円筒の一方の側面の中心</param>
        /// <param name="end2">円筒の他方の側面の中心</param>
        /// <param name="radius">円筒の半径</param>
        /// <param name="distanceFromSource">線分の始点から円筒までの最短距離</param>
        /// <returns></returns>
        public static bool GetCollisionCylinder(Vector3 lineSource, Vector3 lineDestination, Vector3 end1, Vector3 end2, float radius, out float distanceFromSource) {
            distanceFromSource = 0;
            // B:line.end T:line.lineDir E:cylinder.End N:cylinder.lineDir R:cylinder.radius
            // |(B+aT)-E - ((B+aT)-E) . N * N| = R
            // [(T-T.N N)a + (B-E-B.N N+E.N N)]^2 = R
            Vector3 lineEdge = lineDestination - lineSource;
            Vector3 axis = end2 - end1;
            Vector3 axisDir = Vector3.Normalize(axis);
            float axisLength = axis.Length();
            Vector3 position = end1 - axisDir * radius;

            Vector3 coefQuadricVec = lineEdge - Vector3.Dot(lineEdge, axisDir) * axisDir;
            Vector3 endDiff = lineSource - end1;
            Vector3 coefConstVec = endDiff - Vector3.Dot(endDiff, axisDir) * axisDir;
            float A = coefQuadricVec.LengthSq();
            float B = 2f * Vector3.Dot(coefQuadricVec, coefConstVec);
            float C = coefConstVec.LengthSq() - radius * radius;
            QuadraticEquation eq = QuadraticEquation.Solve(A, B, C);
            if(eq.Indefinite) {
                distanceFromSource = 0;
                return true;
            }
            foreach(var ans in eq.Answers) {
                if(ans < 0)
                    continue;
                Vector3 point = lineSource + lineEdge * ans;
                Vector3 toPoint = point - end1;
                float horiz = Vector3.Dot(toPoint, axisDir);
                if(0 <= horiz && horiz <= axisLength) {
                    distanceFromSource = ans * lineEdge.LengthSq();
                    return true;
                }
            }
            return false;
        }

        public static bool GetCollisionSphere(Vector3 lineSource, Vector3 lineDestination, Vector3 position, float radius, out float distanceFromSource) {
            // Abs(ベクトルの始点 + a線分ベクトル - 球の中心) = 球の半径
            // |E + aD - C| = R
            // D^2 x^2 + 2D(E-C)x + (E-C)^2 - R^2 = 0
            distanceFromSource = 0;

            Vector3 diff = lineSource - position;
            Vector3 lineEdge = lineDestination - lineSource;
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


        public static float GetDistanceWithParallelogram(Vector3 anchorPoint, Vector3 baseVertex, Vector3 edge1, Vector3 edge2, out Vector3 location) {
            // 平行四辺形のある平面の法線
            Vector3 normOnParallelogram = Vector3.Normalize(Vector3.Cross(edge1, edge2));
            // baseVertexへ
            Vector3 toBaseVertex = baseVertex - anchorPoint;
            // anchorPointから平行四辺形のある平面への最短ベクトル(垂線)
            Vector3 perpendicular = normOnParallelogram * Vector3.Dot(toBaseVertex, normOnParallelogram);
            // 垂線の足
            Vector3 foot = anchorPoint + perpendicular;
            // 垂線の足が平行四辺形の中かどうか
            if(GeometryCalc.PointOverParallelogram(foot, baseVertex, edge1, edge2)) {
                location = foot;
                return perpendicular.Length();
            }

            Vector3 oppositeVertex = baseVertex + edge1 + edge2;
            Vector3 location1, location2, location3, location4;
            float distance1 = GeometryCalc.GetDistanceWithLine(anchorPoint, baseVertex, baseVertex + edge1, out location1);
            float distance2 = GeometryCalc.GetDistanceWithLine(anchorPoint, baseVertex, baseVertex + edge2, out location2);
            float distance3 = GeometryCalc.GetDistanceWithLine(anchorPoint, oppositeVertex, oppositeVertex - edge1, out location3);
            float distance4 = GeometryCalc.GetDistanceWithLine(anchorPoint, oppositeVertex, oppositeVertex - edge2, out location4);
            float minDistance = new float[] { distance1, distance2, distance3, distance4 }.Min();
            if(minDistance == distance1) {
                location = location1;
            } else if(minDistance == distance2) {
                location = location2;
            } else if(minDistance == distance3) {
                location = location3;
            } else {
                location = location4;
            }
            return minDistance;
        }

        public static bool PointOverTriangle(Vector3 point, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3) {
            Vector3 edge1to2 = vertex2 - vertex1;
            Vector3 edge2to3 = vertex3 - vertex2;
            Vector3 edge3to1 = vertex1 - vertex3;
            Vector3 edge1to2Dir = Vector3.Normalize(edge1to2);
            Vector3 edge2to3Dir = Vector3.Normalize(edge2to3);
            Vector3 edge3to1Dir = Vector3.Normalize(edge3to1);
            // 平面上で辺に垂直で内側向きのベクトル
            Vector3 edge1to2Norm = edge2to3 - edge1to2Dir * Vector3.Dot(edge2to3, edge1to2Dir);
            Vector3 edge2to3Norm = edge3to1 - edge2to3Dir * Vector3.Dot(edge3to1, edge2to3Dir);
            Vector3 edge3to1Norm = edge1to2 - edge3to1Dir * Vector3.Dot(edge1to2, edge3to1Dir);
            // pointから頂点へのベクトル
            Vector3 toVertex1 = vertex1 - point;
            Vector3 toVertex2 = vertex2 - point;
            // 内向きのベクトルとの内積
            float toEdge1to2 = Vector3.Dot(toVertex1, edge1to2Norm);
            float toEdge2to3 = Vector3.Dot(toVertex2, edge2to3Norm);
            float toEdge3to1 = Vector3.Dot(toVertex1, edge3to1Norm);
            // 内部にあるなら内積は負
            return toEdge1to2 <= 0 && toEdge2to3 <= 0 && toEdge3to1 <= 0;
        }

        /// <summary>
        /// 点が平行四辺形上にあるかを返します．点が平面上にない場合には垂線の足を使います．
        /// </summary>
        /// <param name="point">点</param>
        /// <param name="baseVertex">平行四辺形の一つの頂点</param>
        /// <param name="edge1">平行四辺形の一つの辺のベクトル</param>
        /// <param name="edge2">平行四辺形の一つの辺のベクトル</param>
        /// <returns></returns>
        public static bool PointOverParallelogram(Vector3 point, Vector3 baseVertex, Vector3 edge1, Vector3 edge2) {
            Vector3 edge1Dir = Vector3.Normalize(edge1);
            Vector3 edge2Dir = Vector3.Normalize(edge2);
            // 平面上で辺に垂直で内側向きのベクトル
            Vector3 edge1Norm = edge2 - edge1Dir * Vector3.Dot(edge2, edge1Dir);
            Vector3 edge2Norm = edge1 - edge2Dir * Vector3.Dot(edge1, edge2Dir);
            // baseVertexから反対の頂点
            Vector3 oppositeVertex = baseVertex + edge1 + edge2;
            // pointから頂点へのベクトル
            Vector3 toBaseVertex = baseVertex - point;
            Vector3 toOppositeVertex = oppositeVertex - point;
            // 内向きのベクトルとの内積
            float toEdge1 = Vector3.Dot(toBaseVertex, edge1Norm);
            float toEdge2 = Vector3.Dot(toBaseVertex, edge2Norm);
            float toOppositeEdge1 = Vector3.Dot(toOppositeVertex, -edge1Norm);
            float toOppositeEdge2 = Vector3.Dot(toOppositeVertex, -edge2Norm);
            // 内部にあるなら内積は負
            return toEdge1 <= 0 && toEdge2 <= 0 && toOppositeEdge1 <= 0 && toOppositeEdge2 <= 0;
        }

        /// <summary>
        /// 線分と三角形が交点を持つかを求めます
        /// </summary>
        /// <param name="lineSource"></param>
        /// <param name="lineDestination"></param>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="vertex3"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static bool GetCollisionTriangle(Vector3 lineSource, Vector3 lineDestination, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, out float distance) {
            // E + aD = (V2-V1)b + (V3-V1)c + V1
            // aD + b(V1-V2) + c(V1-V3) = V1 - E
            distance = 0;
            Vector3 diff2 = vertex2 - vertex1;
            Vector3 diff3 = vertex3 - vertex1;
            Vector3 lineEdge = lineDestination - lineSource;
            Vector3 rightConst = vertex1 - lineSource;
            float[,] mat = new float[3, 3];
            float[] vec = new float[3];

            mat[0, 0] = lineEdge.X;
            mat[1, 0] = lineEdge.Y;
            mat[2, 0] = lineEdge.Z;
            mat[0, 1] = -diff2.X;
            mat[1, 1] = -diff2.Y;
            mat[2, 1] = -diff2.Z;
            mat[0, 2] = -diff3.X;
            mat[1, 2] = -diff3.Y;
            mat[2, 2] = -diff3.Z;
            vec[0] = rightConst.X;
            vec[1] = rightConst.Y;
            vec[2] = rightConst.Z;

            SimultaneousEquations se = SimultaneousEquations.Solve(mat, vec);
            if(se.Answers.Length == 0) {
                distance = 0;
                return false;
            }
            for(int i = 0; i < 3; i++) {
                if(se.Answers[i] < 0 || se.Answers[i] >= 1) {
                    distance = 0;
                    return false;
                }
            }
            // 三角形なので和が1以下
            if(se.Answers[1] + se.Answers[2] >= 1) {
                distance = 0;
                return false;
            }

            distance = lineEdge.Length() * se.Answers[0];
            return true;
        }

        public static bool GetCollisionParallelogram(Vector3 lineSource, Vector3 lineDestination, Vector3 baseVertex, Vector3 edge1, Vector3 edge2, out  float distance) {
            distance = float.MaxValue;
            Vector3 lineEdge = lineDestination - lineSource;
            // 平行四辺形のある平面の法線
            Vector3 normOnParallelogram = Vector3.Normalize(Vector3.Cross(edge1, edge2));
            // baseVertexへ
            Vector3 toBaseVertex = baseVertex - lineSource;
            // 平面の法線成分
            float toBaseVertexDistance = Vector3.Dot(normOnParallelogram, toBaseVertex);
            float lineEdgeDistance = Vector3.Dot(normOnParallelogram, lineEdge);
            if(lineEdgeDistance == 0) {
                // 引数の線分と平行四辺形が平行な時
                return false;
            }
            float ratio = toBaseVertexDistance / lineEdgeDistance;
            if(ratio < 0 && 1 < ratio) {
                // 向きが逆であるか，長さが足りない
                return false;
            }
            // 引数の線分と平行四辺形のある平面との交点へのベクトル
            Vector3 lineEdgeToParallelogram = lineEdge * ratio;
            // 引数の線分と平行四辺形のある平面との交点
            Vector3 onPlane = lineSource + lineEdgeToParallelogram;
            if(GeometryCalc.PointOverParallelogram(onPlane, baseVertex, edge1, edge2)) {
                distance = lineEdgeToParallelogram.Length();
                return true;
            }
            return false;
        }

        #region GetDistanceWith

        /// <summary>
        /// 基準点と他の点との最短位置と距離を返します。
        /// </summary>
        /// <param title="anchorPoint">基準点</param>
        /// <param title="point">対象となる点</param>
        /// <param title="location">出力される最短位置</param>
        /// <returns>距離</returns>
        public static float GetDistanceWithPoint(Vector3 anchorPoint, Vector3 point, out Vector3 location) {
            //
            location = point;
            return (anchorPoint - point).Length();
        }

        /// <summary>
        /// 基準点と円との最短位置と距離を返します。
        /// </summary>
        /// <param title="anchorPoint">基準点</param>
        /// <param title="center">対象となる円の中心</param>
        /// <param title="verticalDirectionAndRadius">対象となる円の半径の長さを持つ垂直ベクトル</param>
        /// <param title="location">出力される最短位置</param>
        /// <returns>距離</returns>
        public static float GetDistanceWithCircle(Vector3 anchorPoint, Vector3 center, Vector3 verticalDirectionAndRadius, out Vector3 location) {
            float radius = verticalDirectionAndRadius.Length();
            Vector3 normalVert = Vector3.Normalize(verticalDirectionAndRadius);
            Vector3 toAnchor = anchorPoint - center;
            Vector3 toAnchorVert = Vector3.Dot(toAnchor, normalVert) * normalVert;
            Vector3 toAnchorHoriz = toAnchor - toAnchorVert;
            if(toAnchorHoriz.Length() > radius) {
                location = center + Vector3.Normalize(toAnchorHoriz) * radius;
            } else {
                if(Vector3.Dot(toAnchorVert, normalVert) < 0)
                    toAnchorVert = -toAnchorVert;
                location = anchorPoint - toAnchorVert;
            }
            return (anchorPoint - location).Length();
        }
        /// <summary>
        /// 基準点と円筒との最短位置と距離を返します。
        /// </summary>
        /// <param title="anchorPoint">基準点</param>
        /// <param title="end1">対象となる円筒の底面の中心</param>
        /// <param title="end2">対象となる円筒の上面の中心</param>
        /// <param title="radius">対象となる円筒の半径</param>
        /// <param title="location">出力される最短位置</param>
        /// <returns>距離</returns>
        public static float GetDistanceWithCylinder(Vector3 anchorPoint, Vector3 end1, Vector3 end2, float radius, out Vector3 location) {
            Vector3 toAnchor = anchorPoint - end1;
            Vector3 axis = end2 - end1;
            Vector3 normalAxis = Vector3.Normalize(axis);
            float axisLength = axis.Length();
            float toAnchorVertLen = Vector3.Dot(toAnchor, normalAxis);
            if(toAnchorVertLen >= 0 && toAnchorVertLen < axisLength) {
                Vector3 toAnchorHoriz = toAnchor - normalAxis * toAnchorVertLen;
                location = anchorPoint - toAnchorHoriz + Vector3.Normalize(toAnchorHoriz) * radius;
                return (anchorPoint - location).Length();
            }
            Vector3 location1, location2;
            float distance1 = GetDistanceWithCircle(anchorPoint, end1, normalAxis * radius, out location1);
            float distance2 = GetDistanceWithCircle(anchorPoint, end2, normalAxis * radius, out location2);
            if(distance1 < distance2) {
                location = location1;
                return distance1;
            } else {
                location = location2;
                return distance2;
            }
        }
        /// <summary>
        /// 基準点と線分との最短位置と距離を出力します。
        /// </summary>
        /// <param title="anchorPoint">基準点</param>
        /// <param title="end1">対象となる線分の一つの端点</param>
        /// <param title="end2">対象となる線分の他の端点</param>
        /// <param title="location">出力される最短位置</param>
        /// <returns>距離</returns>
        public static float GetDistanceWithLine(Vector3 anchorPoint, Vector3 end1, Vector3 end2, out Vector3 location) {
            Vector3 toAnchor = anchorPoint - end1;
            Vector3 lineDir = end2 - end1;
            Vector3 normalLineDir = Vector3.Normalize(lineDir);
            float length = lineDir.Length();
            float toAnchorVertLen = Vector3.Dot(toAnchor, normalLineDir);
            if(toAnchorVertLen >= 0 && toAnchorVertLen < length) {
                Vector3 toAnchorHoriz = toAnchor - normalLineDir * toAnchorVertLen;
                location = anchorPoint - toAnchorHoriz;
                return (anchorPoint - location).Length();
            }
            Vector3 location1, location2;
            float distance1 = GetDistanceWithPoint(anchorPoint, end1, out location1);
            float distance2 = GetDistanceWithPoint(anchorPoint, end2, out location2);
            if(distance1 < distance2) {
                location = location1;
                return distance1;
            } else {
                location = location2;
                return distance2;
            }
        }
        #endregion

        /// <summary>
        /// 二つの線分を基にした正規直交基底を表す3つのVector3を返します。
        /// </summary>
        /// <param title="line1End1">一方の線分の一方の端点</param>
        /// <param title="line1End2">一方の線分の他方の端点</param>
        /// <param title="line2End1">他方の線分の一方の端点</param>
        /// <param title="line2End2">他方の線分の他方の端点</param>
        /// <returns>3つのVector3</returns>
        public static Vector3[] GetCoordinateFromTwoLines(Vector3 line1End1, Vector3 line1End2, Vector3 line2End1, Vector3 line2End2) {
            Vector3 dir1 = line1End2 - line1End1;
            Vector3 dir1Normal = Vector3.Normalize(dir1);
            Vector3 dir2 = line2End2 - line2End1;
            Vector3 dir2Independent = dir2 - dir1Normal * Vector3.Dot(dir1Normal, dir2);
            Vector3 dir3 = Vector3.Cross(dir1, dir2);
            Vector3 dir3Normal = Vector3.Normalize(dir3);
            float dir3Length = dir3.Length();
            float dir3StdLength = (float)Math.Sqrt(dir3Length);
            Vector3 dir3Std = dir3Normal * dir3StdLength;
            return new[] { dir1, dir2Independent, dir3Std };
        }

        /// <summary>
        /// 二つの線分を結ぶ垂直な線分が引ける場合にその線分の端点を返します。
        /// </summary>
        /// <param title="end1">一方の線分の端点</param>
        /// <param title="dir1">一方の線分の他の端点の相対座標</param>
        /// <param title="end2">他方の線分の端点</param>
        /// <param title="dir2">他方の線分の他の端点の相対座標</param>
        /// <param title="pointOnLine1">二線分にとって垂直な線分の一方の端点</param>
        /// <param title="pointOnLine2">二線分にとって垂直な線分の他方の端点</param>
        /// <returns>引けた場合はtrue</returns>
        public static bool GetNearestPointsInLines(Vector3 end1, Vector3 dir1, Vector3 end2, Vector3 dir2, out Vector3 pointOnLine1, out Vector3 pointOnLine2) {
            // min: |(e1+Ad1)-(e2+Bd2)|
            // 両直線にとって垂直な線分で端点が各直線に含まれる
            // (d1, (e1+Ad1)-(e2+Bd2)) = (d2, (e1+Ad1)-(e2+Bd2)) = 0
            // A|d1|^2   - B(d1,d2) = (d1, e2-e1)
            // A(d1, d2) - B|d2|^2  = (d2, e2-e1)
            float[,] mat = new float[2, 2];
            float[] vec = new float[2];
            Vector3 endDiff = end2 - end1;
            float dot = Vector3.Dot(dir1, dir2);
            mat[0, 0] = dir1.LengthSq();
            mat[0, 1] = -dot;
            mat[1, 0] = dot;
            mat[1, 1] = -dir2.LengthSq();
            vec[0] = Vector3.Dot(dir1, endDiff);
            vec[1] = Vector3.Dot(dir2, endDiff);
            SimultaneousEquations se = SimultaneousEquations.Solve(mat, vec);
            if(se.Answers.Length != 2) {
                pointOnLine1 = pointOnLine2 = Vector3.Empty;
                return false;
            }
            float dirRatio1 = se.Answers[0];
            float dirRatio2 = se.Answers[1];
            if(dirRatio1 < 0 || dirRatio2 < 0 || dirRatio1 >= 1 || dirRatio2 >= 1) {
                pointOnLine1 = pointOnLine2 = Vector3.Empty;
                return false;
            }
            pointOnLine1 = end1 + dir1 * dirRatio1;
            pointOnLine2 = end2 + dir2 * dirRatio2;
            return true;
        }

        public static bool GetNearestPointsInLinesWithEdges(Vector3 end1, Vector3 dir1, Vector3 end2, Vector3 dir2, out Vector3 pointOnLine1, out Vector3 pointOnLine2) {
            if(GetNearestPointsInLines(end1, dir1, end2, dir2, out pointOnLine1, out pointOnLine2)) {
                return true;
            }
            float minDistance = float.MaxValue;
            float distance;
            distance = GetDistanceWithLine(end1, end2, end2 + dir2, out pointOnLine2);
            if(distance < minDistance) {
                minDistance = distance;
                pointOnLine1 = end1;
            }
            distance = GetDistanceWithLine(end1 + dir1, end2, end2 + dir2, out pointOnLine2);
            if(distance < minDistance) {
                minDistance = distance;
                pointOnLine1 = end1 + dir1;
            }
            distance = GetDistanceWithLine(end2, end1, end1 + dir1, out pointOnLine1);
            if(distance < minDistance) {
                minDistance = distance;
                pointOnLine2 = end2;
            }
            distance = GetDistanceWithLine(end2 + dir2, end1, end1 + dir1, out pointOnLine1);
            if(distance < minDistance) {
                minDistance = distance;
                pointOnLine2 = end2 + dir2;
            }
            return true;
        }
        static bool getCrossPoint(Vector3 end1, Vector3 dir1, Vector3 end2, Vector3 dir2, out Vector3 point) {
            point = Vector3.Empty;
            // 二線が交差していないと変な結果を出す
            // むしろ二線の距離が最小となる位置を出すべきなのだろうか
            float[,] mat = new float[2, 2];
            float[] vec = new float[2];
            float useX = Math.Abs(dir1.X) + Math.Abs(dir2.X);
            float useY = Math.Abs(dir1.Y) + Math.Abs(dir2.Y);
            float useZ = Math.Abs(dir1.Z) + Math.Abs(dir2.Z);
            // C1+aV1=C2+bV2
            // aV1-bV2=C2-C1
            Vector3 endDif = end2 - end1;
            if(useX < useY && useX < useZ) {
                mat[0, 0] = dir1.Z;
                mat[0, 1] = -dir2.Z;
                mat[1, 0] = dir1.Y;
                mat[1, 1] = -dir2.Y;
                vec[0] = endDif.Z;
                vec[1] = endDif.Y;
            } else if(useY < useX && useY < useZ) {
                mat[0, 0] = dir1.X;
                mat[0, 1] = -dir2.X;
                mat[1, 0] = dir1.Z;
                mat[1, 1] = -dir2.Z;
                vec[0] = endDif.X;
                vec[1] = endDif.Z;
            } else {
                mat[0, 0] = dir1.X;
                mat[0, 1] = -dir2.X;
                mat[1, 0] = dir1.Y;
                mat[1, 1] = -dir2.Y;
                vec[0] = endDif.X;
                vec[1] = endDif.Y;
            }
            SimultaneousEquations se = SimultaneousEquations.Solve(mat, vec);
            if(se.Answers.Length == 0)
                return false;
            point = end1 + dir1 * se.Answers[0];
            return true;
        }

        public static bool GetFarthestPoints(out int index1, out int index2, params Vector3[] points) {
            if(points.Length < 2) {
                index1 = index2 = 0;
                return false;
            }
            index1 = -1;
            index2 = 0;
            while(true) {
                int farIndex = -1;
                float farLength = -1;
                for(int i = 0; i < points.Length; i++) {
                    if(i == index2)
                        continue;
                    float length = (points[index2] - points[i]).Length();
                    if(length > farLength) {
                        farLength = length;
                        farIndex = i;
                    }
                }
                if(farIndex == index1)
                    break;
                index1 = index2;
                index2 = farIndex;
            }
            return true;
        }

        public static bool GetCircumscribedCircle(Vector3 point1, Vector3 point2, Vector3 point3, out Vector3 center) {
            center = Vector3.Empty;
            // 二辺を取得
            Vector3 diffA = point2 - point1;
            Vector3 diffB = point3 - point1;
            // 三角形のなす面と垂直のベクトル
            Vector3 backward = Vector3.Cross(diffA, diffB);
            if(backward == Vector3.Empty) // 三点は直線上
                return false;
            // 二辺の三角形上の垂線
            Vector3 vertA = Vector3.Cross(backward, diffA);
            Vector3 vertB = Vector3.Cross(backward, diffB);
            // 二辺の中点
            Vector3 centerA = point1 + diffA * 0.5f;
            Vector3 centerB = point1 + diffB * 0.5f;
            // 二ベクトルの交点が外心
            return getCrossPoint(centerA, vertA, centerB, vertB, out center);
        }

        public static bool GetCircumscribedSphere(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4, out Vector3 center) {
            center = Vector3.Empty;
            Vector3 centerA, centerB;
            // 二面の外心
            if(!GetCircumscribedCircle(point1, point2, point3, out centerA))
                return false;
            if(!GetCircumscribedCircle(point1, point2, point4, out centerB))
                return false;
            // 辺を取得
            Vector3 diffA = point2 - point1;
            Vector3 diffB = point3 - point1;
            Vector3 diffC = point4 - point1;
            // 二面の垂線
            Vector3 vertA = Vector3.Cross(diffA, diffB);
            Vector3 vertB = Vector3.Cross(diffA, diffC);
            // 二ベクトルの交点が外心
            return getCrossPoint(centerA, vertA, centerB, vertB, out center);
        }

        public static bool GetContainingSphere(out Vector3 center, out float radius, params Vector3[] points) {
            center = Vector3.Empty;
            radius = float.MaxValue;
            bool found = false;
            foreach(var indices in Combination.EnumerateCombinations(points.Length, 4)) {
                Vector3 tmpCenter;
                float tmpRadius = 0;
                if(GetCircumscribedSphere(points[indices[0]], points[indices[1]], points[indices[2]], points[indices[3]], out tmpCenter)) {
                    for(int i = 0; i < points.Length; i++) {
                        float tmpTmpRadius = (tmpCenter - points[i]).Length();
                        if(tmpTmpRadius > tmpRadius)
                            tmpRadius = tmpTmpRadius;
                    }
                }
                if(tmpRadius < radius) {
                    radius = tmpRadius;
                    center = tmpCenter;
                    found = true;
                }
            }
            foreach(var indices in Combination.EnumerateCombinations(points.Length, 3)) {
                Vector3 tmpCenter;
                float tmpRadius = 0;
                if(GetCircumscribedCircle(points[indices[0]], points[indices[1]], points[indices[2]], out tmpCenter)) {
                    for(int i = 0; i < points.Length; i++) {
                        float tmpTmpRadius = (tmpCenter - points[i]).Length();
                        if(tmpTmpRadius > tmpRadius)
                            tmpRadius = tmpTmpRadius;
                    }
                }
                if(tmpRadius < radius) {
                    radius = tmpRadius;
                    center = tmpCenter;
                    found = true;
                }
            }
            foreach(var indices in Combination.EnumerateCombinations(points.Length, 2)) {
                Vector3 tmpCenter = Vector3.Lerp(points[indices[0]], points[indices[1]], 0.5f);
                float tmpRadius = 0;
                for(int i = 0; i < points.Length; i++) {
                    float tmpTmpRadius = (tmpCenter - points[i]).Length();
                    if(tmpTmpRadius > tmpRadius)
                        tmpRadius = tmpTmpRadius;
                }
                if(tmpRadius < radius) {
                    radius = tmpRadius;
                    center = tmpCenter;
                    found = true;
                }
            }

            return found;
        }





        /// <summary>
        /// 点と線分の最短距離を返します。
        /// </summary>
        /// <param title="point">点</param>
        /// <param title="end1">線分の一方の端点</param>
        /// <param title="end2">線分の他方の端点</param>
        /// <returns>空間距離</returns>
        public static float GetDistancePointAndLine(Vector3 point, Vector3 end1, Vector3 end2, out Vector3 nearestPoint) {
            Vector3 edge = end2 - end1;
            float lineLength = edge.Length();
            Vector3 edgeDir = Vector3.Normalize(edge);

            Vector3 toEnd1 = end1 - point;
            Vector3 toEnd2 = end2 - point;
            float horiz = Vector3.Dot(toEnd2, edgeDir);
            if(lineLength > 0 && horiz >= 0 && horiz <= lineLength) {
                // 線との距離
                Vector3 lineNormal = toEnd2 - horiz * edgeDir;
                nearestPoint = point + lineNormal;
                return lineNormal.Length();
            }
            // 端点との距離
            float distance1 = toEnd1.Length();
            float distance2 = toEnd2.Length();
            if(distance1 < distance2) {
                nearestPoint = end1;
                return distance1;
            } else {
                nearestPoint = end2;
                return distance2;
            }
        }
        /// <summary>
        /// 点と線分の最短距離を返します。
        /// </summary>
        /// <param title="point">点</param>
        /// <param title="end1">線分の一方の端点</param>
        /// <param title="end2">線分の他方の端点</param>
        /// <returns>平面距離</returns>
        public static float GetDistancePointAndLine(Vector2 point, Vector2 end1, Vector2 end2, out Vector2 nearestPoint) {
            Vector2 edge = end2 - end1;
            float lineLength = edge.Length();
            Vector2 edgeDir = Vector2.Normalize(edge);

            Vector2 toEnd1 = end1 - point;
            Vector2 toEnd2 = end2 - point;
            float horiz = Vector2.Dot(toEnd2, edgeDir);
            if(lineLength > 0 && horiz >= 0 && horiz <= lineLength) {
                // 線との距離
                Vector2 lineNormal = toEnd2 - horiz * edgeDir;
                nearestPoint = point + lineNormal;
                return lineNormal.Length();
            }
            // 端点との距離
            float distance1 = toEnd1.Length();
            float distance2 = toEnd2.Length();
            if(distance1 < distance2) {
                nearestPoint = end1;
                return distance1;
            } else {
                nearestPoint = end2;
                return distance2;
            }
        }

        public static IList<uint> OptimizePlaneOrder(IList<MotionObjectInfo> infoList, Motion.ReadOnlyMotionFrame frame) {
            var infoList2 = (from info in infoList where info.ObjectType == typeof(PointObject) select info).ToList();
            if(infoList2.Count < 3)
                throw new ArgumentException("3 or more points needed");
            if(!infoList2.All(info => frame[info] != null))
                throw new ArgumentException("some points missing in the frame");

            List<uint> idSequence;
            if(infoList2.Count <= 10) {
                Dictionary<int, MotionObjectInfo> map = new Dictionary<int, MotionObjectInfo>();
                for(int i = 0; i < infoList2.Count; i++) {
                    map[i] = infoList2[i];
                }
                TravelingSalesmanProblem<int> tsp = TravelingSalesmanProblem<int>.Solve(map.Keys.ToList(), (x, y) => {
                    return (frame[map[x]].GravityPoint - frame[map[y]].GravityPoint).Length();
                });
                idSequence = tsp.Answer.Select(i => map[i].Id).ToList();
            } else {
                idSequence = new List<uint>();
                List<uint> ids = new List<uint>(infoList2.Select(info => info.Id).ToList());
                uint lastId = ids[0];
                idSequence.Add(lastId);
                ids.RemoveAt(0);
                while(ids.Count > 0) {
                    Dictionary<uint, float> distances = new Dictionary<uint, float>();
                    Vector3 currentPos = frame[lastId].GravityPoint;
                    foreach(var id in ids) {
                        distances[id] = (frame[id].GravityPoint - currentPos).LengthSq();
                    }
                    float min = distances.Min(d => d.Value);
                    var selects = (from d in distances where d.Value == min select d.Key).ToList();
                    if(selects.Count == 0) {
                        throw new InvalidOperationException("computation failed");
                    }
                    lastId = selects.First();
                    idSequence.Add(lastId);
                    ids.Remove(lastId);
                }
            }
            // 交互にする
            List<uint> ret = new List<uint>();
            uint next;
            while(true) {
                if(idSequence.Count == 0)
                    break;
                next = idSequence[0];
                ret.Add(next);
                idSequence.Remove(next);
                if(idSequence.Count == 0)
                    break;
                next = idSequence[idSequence.Count - 1];
                ret.Add(next);
                idSequence.Remove(next);
            }

            return ret;
        }

        public static IList<uint> OptimizePlaneOrder(IList<MotionObjectInfo> infoList, Motion.MotionFrame frame) {
            return OptimizePlaneOrder(infoList, new ReadOnlyMotionFrame(frame));
        }
    }


}

