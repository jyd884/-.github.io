// GeometryKernel/Topology/Edge.cs
using System;
using System.Collections.Generic;
using CADFramework.GeometryKernel.Primitives;

namespace CADFramework.GeometryKernel.Topology
{
    /// <summary>
    /// 拓扑边 - 连接两个顶点的几何元素
    /// </summary>
    public class Edge
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        // 拓扑关系：起点和终点
        public Vertex StartVertex { get; set; }
        public Vertex EndVertex { get; set; }
        
        // 拓扑关系：包含该边的面
        public List<Face> AdjacentFaces { get; private set; }
        
        // 几何曲线（可以是直线、圆弧、样条曲线等）
        public IShape Curve { get; set; }
        
        public Edge(int id, string name, Vertex start, Vertex end, IShape curve = null)
        {
            Id = id;
            Name = name;
            StartVertex = start;
            EndVertex = end;
            Curve = curve ?? new Line(id, name, start.Position, end.Position);
            AdjacentFaces = new List<Face>();
            
            // 建立双向关联
            start.AddAdjacentEdge(this);
            end.AddAdjacentEdge(this);
        }
        
        /// <summary>
        /// 获取边的长度
        /// </summary>
        public double GetLength()
        {
            if (Curve is Line line)
                return line.StartPoint.DistanceTo(line.EndPoint);
            else if (Curve is Circle circle)
                return circle.GetCircumference();
            else
                return StartVertex.Position.DistanceTo(EndVertex.Position);
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
        /// 判断是否为边界边（只属于一个面）
        /// </summary>
        public bool IsBoundaryEdge() => AdjacentFaces.Count == 1;
        
        /// <summary>
        /// 获取边的中点
        /// </summary>
        public Point3D GetMidPoint()
        {
            return new Point3D(
                (StartVertex.Position.X + EndVertex.Position.X) / 2,
                (StartVertex.Position.Y + EndVertex.Position.Y) / 2,
                (StartVertex.Position.Z + EndVertex.Position.Z) / 2
            );
        }
        
        /// <summary>
        /// 获取边的方向向量
        /// </summary>
        public Point3D GetDirection()
        {
            return new Point3D(
                EndVertex.Position.X - StartVertex.Position.X,
                EndVertex.Position.Y - StartVertex.Position.Y,
                EndVertex.Position.Z - StartVertex.Position.Z
            );
        }
        
        public override string ToString()
        {
            return $"Edge {Name} (ID:{Id}): {StartVertex.Name} -> {EndVertex.Name}, Length={GetLength():F2}";
        }
    }
}