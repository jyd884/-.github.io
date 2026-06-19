// GeometryKernel/Topology/Face.cs
using System;
using System.Collections.Generic;
using CADFramework.GeometryKernel.Primitives;

namespace CADFramework.GeometryKernel.Topology
{
    /// <summary>
    /// 拓扑面 - 由一组边围成的区域
    /// </summary>
    public class Face
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        // 拓扑关系：组成面的边（循环）
        public List<Edge> Edges { get; private set; }
        
        // 拓扑关系：相邻的面（通过共享边）
        public List<Face> AdjacentFaces { get; private set; }
        
        // 几何曲面（平面、圆柱面、球面等）
        public IShape Surface { get; set; }
        
        // 面的法向量
        public Point3D Normal { get; set; }
        
        public Face(int id, string name, List<Edge> edges, IShape surface = null)
        {
            Id = id;
            Name = name;
            Edges = edges ?? new List<Edge>();
            Surface = surface;
            AdjacentFaces = new List<Face>();
            
            // 计算法向量（对于平面）
            ComputeNormal();
            
            // 建立边到面的关联
            foreach (var edge in edges)
            {
                edge.AddAdjacentFace(this);
            }
        }
        
        /// <summary>
        /// 计算面的法向量
        /// </summary>
        private void ComputeNormal()
        {
            if (Edges.Count >= 3)
            {
                // 使用前三个顶点计算法向量
                var v1 = Edges[0].StartVertex.Position;
                var v2 = Edges[0].EndVertex.Position;
                var v3 = Edges[1].EndVertex.Position;
                
                var u = new Point3D(v2.X - v1.X, v2.Y - v1.Y, v2.Z - v1.Z);
                var v = new Point3D(v3.X - v1.X, v3.Y - v1.Y, v3.Z - v1.Z);
                
                // 叉积
                Normal = new Point3D(
                    u.Y * v.Z - u.Z * v.Y,
                    u.Z * v.X - u.X * v.Z,
                    u.X * v.Y - u.Y * v.X
                );
                
                // 归一化
                double len = Math.Sqrt(Normal.X * Normal.X + Normal.Y * Normal.Y + Normal.Z * Normal.Z);
                if (len > 1e-6)
                {
                    Normal = new Point3D(Normal.X / len, Normal.Y / len, Normal.Z / len);
                }
            }
            else
            {
                Normal = new Point3D(0, 0, 1);
            }
        }
        
        /// <summary>
        /// 计算面的面积
        /// </summary>
        public double GetArea()
        {
            if (Surface is Rectangle rect)
                return rect.Width * rect.Height;
            else if (Surface is Circle circle)
                return circle.GetArea();
            else
                return 0; // 复杂面需要三角剖分计算面积
        }
        
        /// <summary>
        /// 添加相邻面
        /// </summary>
        public void AddAdjacentFace(Face face)
        {
            if (!AdjacentFaces.Contains(face))
                AdjacentFaces.Add(face);
        }
        
        /// <summary>
        /// 获取面的边界框
        /// </summary>
        public BoundingBox GetBoundingBox()
        {
            if (Surface != null)
                return Surface.GetBoundingBox();
            
            // 根据顶点计算边界框
            double minX = double.MaxValue, minY = double.MaxValue, minZ = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue, maxZ = double.MinValue;
            
            foreach (var edge in Edges)
            {
                var bbox = edge.Curve.GetBoundingBox();
                minX = Math.Min(minX, bbox.MinX);
                minY = Math.Min(minY, bbox.MinY);
                minZ = Math.Min(minZ, bbox.MinZ);
                maxX = Math.Max(maxX, bbox.MaxX);
                maxY = Math.Max(maxY, bbox.MaxY);
                maxZ = Math.Max(maxZ, bbox.MaxZ);
            }
            
            return new BoundingBox(minX, minY, minZ, maxX, maxY, maxZ);
        }
        
        public override string ToString()
        {
            return $"Face {Name} (ID:{Id}): Edges={Edges.Count}, Area={GetArea():F2}";
        }
    }
}