using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;

namespace MotionDataHandler.Motion {
    /// <summary>
    /// Microsoft.DirectX.Direct3D.PrimitiveTypeと同等
    /// </summary>
    public enum PolygonType {
        PointList = 1,
        LineList = 2,
        LineStrip = 3,
        TriangleList = 4,
        TriangleStrip = 5,
        TriangleFan = 6,
    }
    /// <summary>
    /// Microsoft.DirectX.Direct3D.CustomVertex.PositionNormalColoredと同等
    /// </summary>
    public struct PolygonVertex {
        public int Color;
        public float Nx;
        public float Ny;
        public float Nz;
        public float X;
        public float Y;
        public float Z;
        public PolygonVertex(Vector3 pos, Vector3 nor, int c)
            : this(pos.X, pos.Y, pos.Z, nor.X, nor.Y, nor.Z, c) {
        }
        public PolygonVertex(float xvalue, float yvalue, float zvalue, float nxvalue, float nyvalue, float nzvalue, int c) {
            this.X = xvalue;
            this.Y = yvalue;
            this.Z = zvalue;
            this.Nx = nxvalue;
            this.Ny = nyvalue;
            this.Nz = nzvalue;
            this.Color = c;
        }
        public Vector3 Normal { get { return new Vector3(Nx, Ny, Nz); } set { Nx = value.X; Ny = value.Y; Nz = value.Z; } }
        public Vector3 Position { get { return new Vector3(X, Y, Z); } set { X = value.X; Y = value.Y; Z = value.Z; } }
    }

    /// <summary>
    /// 頂点のリストとプリミティブタイプの情報
    /// </summary>
    public class PolygonDatum {
        public PolygonType Type;
        public PolygonVertex[] Vertices;
        public PolygonDatum(PolygonType type, params PolygonVertex[] vertices) {
            this.Type = type;
            this.Vertices = vertices;
        }
        public PolygonDatum(PolygonType type, IList<PolygonVertex> vertices)
            : this(type, vertices.ToArray()) {
        }
        public int GetPrimitiveCount() {
            switch(this.Type) {
            case PolygonType.LineList:
                return this.Vertices.Length / 2;
            case PolygonType.LineStrip:
                return this.Vertices.Length - 1;
            case PolygonType.PointList:
                return this.Vertices.Length;
            case PolygonType.TriangleFan:
                return this.Vertices.Length - 2;
            case PolygonType.TriangleList:
                return Vertices.Length / 3;
            case PolygonType.TriangleStrip:
                return Vertices.Length - 2;
            }
            return 0;
        }
    }

    public class PolygonRenderHint {
        /// <summary>
        /// 点オブジェクトの描画時のサイズや，選択時の枠のサイズを表す値と取得または設定します
        /// </summary>
        public float MarginSize { get; set; }
        public PolygonRenderHint() {
            this.MarginSize = 32;
        }
    }
}
