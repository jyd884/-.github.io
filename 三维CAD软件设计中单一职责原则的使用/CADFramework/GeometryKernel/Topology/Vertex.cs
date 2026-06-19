// GeometryKernel/Topology/Vertex.cs
using System;
using System.Collections.Generic;
using CADFramework.GeometryKernel.Primitives;

namespace CADFramework.GeometryKernel.Topology
{
    /// <summary>
    /// 拓扑顶点 - 表示几何中的点，包含拓扑关系
    /// </summary>
    public class Vertex
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Point3D Position { get; set; }
        
        // 拓扑关系：与该顶点相连的边
        public List<Edge> AdjacentEdges { get; private set; }
        
        // 拓扑关系：包含该顶点的面
        public List<Face> AdjacentFaces { get; private set; }
        
        public Vertex(int id, string name, Point3D position)
        {
            Id = id;
            Name = name;
            Position = position;
            AdjacentEdges = new List<Edge>();
            AdjacentFaces = new List<Face>();
        }
        
        /// <summary>
        /// 添加邻接边
        /// </summary>
        public void AddAdjacentEdge(Edge edge)
        {
            if (!AdjacentEdges.Contains(edge))
                AdjacentEdges.Add(edge);
        }
        
        /// <summary>
        /// 添加邻接面
        /// </summary>
        public void AddAdjacentFace(Face face)
        {
            if (!AdjacentFaces.Contains(face))
                AdjacentFaces.Add(face);
        }
        
        /// <summary>
        /// 获取顶点的度数（连接的边数）
        /// </summary>
        public int GetDegree() => AdjacentEdges.Count;
        
        /// <summary>
        /// 判断两个顶点是否相同
        /// </summary>
        public bool IsCoincident(Vertex other, double tolerance = 1e-6)
        {
            return Position.DistanceTo(other.Position) < tolerance;
        }
        
        public override string ToString()
        {
            return $"Vertex {Name} (ID:{Id}): {Position}, Degree={GetDegree()}";
        }
    }
}