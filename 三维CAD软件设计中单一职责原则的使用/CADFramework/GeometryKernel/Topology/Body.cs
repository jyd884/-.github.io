// GeometryKernel/Topology/Body.cs
using System;
using System.Collections.Generic;
using CADFramework.GeometryKernel.Primitives;

namespace CADFramework.GeometryKernel.Topology
{
    /// <summary>
    /// 拓扑体 - 由一组面围成的封闭区域（3D实体）
    /// </summary>
    public class Body : IShape
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        // 拓扑关系：组成体的面
        public List<Face> Faces { get; private set; }
        
        // 拓扑关系：组成体的边（所有面的边的并集）
        public List<Edge> Edges { get; private set; }
        
        // 拓扑关系：组成体的顶点
        public List<Vertex> Vertices { get; private set; }
        
        // 体积
        public double Volume { get; set; }
        
        // 质心
        public Point3D Centroid { get; set; }
        
        public Body(int id, string name)
        {
            Id = id;
            Name = name;
            Faces = new List<Face>();
            Edges = new List<Edge>();
            Vertices = new List<Vertex>();
        }
        
        /// <summary>
        /// 添加面到体
        /// </summary>
        public void AddFace(Face face)
        {
            if (!Faces.Contains(face))
            {
                Faces.Add(face);
                
                // 收集所有边和顶点
                foreach (var edge in face.Edges)
                {
                    if (!Edges.Contains(edge))
                        Edges.Add(edge);
                    
                    if (!Vertices.Contains(edge.StartVertex))
                        Vertices.Add(edge.StartVertex);
                    if (!Vertices.Contains(edge.EndVertex))
                        Vertices.Add(edge.EndVertex);
                }
            }
        }
        
        /// <summary>
        /// 检查体是否封闭（每个边被两个面共享）
        /// </summary>
        public bool IsClosed()
        {
            foreach (var edge in Edges)
            {
                if (edge.AdjacentFaces.Count != 2)
                    return false;
            }
            return true;
        }
        
        /// <summary>
        /// 计算体积（使用散度定理简化）
        /// </summary>
        public double ComputeVolume()
        {
            if (!IsClosed())
                return 0;
            
            // 简化：对于长方体，体积 = 长*宽*高
            var bbox = GetBoundingBox();
            return (bbox.MaxX - bbox.MinX) * 
                   (bbox.MaxY - bbox.MinY) * 
                   (bbox.MaxZ - bbox.MinZ);
        }
        
        /// <summary>
        /// 计算质心
        /// </summary>
        public Point3D ComputeCentroid()
        {
            var bbox = GetBoundingBox();
            return new Point3D(
                (bbox.MinX + bbox.MaxX) / 2,
                (bbox.MinY + bbox.MaxY) / 2,
                (bbox.MinZ + bbox.MaxZ) / 2
            );
        }
        
        public BoundingBox GetBoundingBox()
        {
            double minX = double.MaxValue, minY = double.MaxValue, minZ = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue, maxZ = double.MinValue;
            
            foreach (var vertex in Vertices)
            {
                minX = Math.Min(minX, vertex.Position.X);
                minY = Math.Min(minY, vertex.Position.Y);
                minZ = Math.Min(minZ, vertex.Position.Z);
                maxX = Math.Max(maxX, vertex.Position.X);
                maxY = Math.Max(maxY, vertex.Position.Y);
                maxZ = Math.Max(maxZ, vertex.Position.Z);
            }
            
            return new BoundingBox(minX, minY, minZ, maxX, maxY, maxZ);
        }
        
        public IShape Clone()
        {
            var clone = new Body(Id, Name);
            // 深拷贝需要复制所有拓扑元素（简化处理）
            clone.Faces = new List<Face>(Faces);
            clone.Edges = new List<Edge>(Edges);
            clone.Vertices = new List<Vertex>(Vertices);
            clone.Volume = Volume;
            clone.Centroid = Centroid;
            return clone;
        }
        
        public void Transform(TransformMatrix matrix)
        {
            foreach (var vertex in Vertices)
            {
                vertex.Position = matrix.Apply(vertex.Position);
            }
            Centroid = matrix.Apply(Centroid);
        }
        
        public override string ToString()
        {
            return $"Body {Name} (ID:{Id}): Faces={Faces.Count}, Vertices={Vertices.Count}, Closed={IsClosed()}";
        }
    }
}