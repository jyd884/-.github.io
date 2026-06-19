// GeometryKernel/Primitives/Line.cs
using System;

namespace CADFramework.GeometryKernel.Primitives
{
    /// <summary>
    /// 线段几何定义
    /// </summary>
    public class Line : IShape
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Point3D StartPoint { get; set; }
        public Point3D EndPoint { get; set; }
        
        public Line(int id, string name, Point3D start, Point3D end)
        {
            Id = id;
            Name = name;
            StartPoint = start;
            EndPoint = end;
        }
        
        public BoundingBox GetBoundingBox()
        {
            return new BoundingBox(
                Math.Min(StartPoint.X, EndPoint.X),
                Math.Min(StartPoint.Y, EndPoint.Y),
                Math.Min(StartPoint.Z, EndPoint.Z),
                Math.Max(StartPoint.X, EndPoint.X),
                Math.Max(StartPoint.Y, EndPoint.Y),
                Math.Max(StartPoint.Z, EndPoint.Z)
            );
        }
        
        public IShape Clone()
        {
            return new Line(Id, Name, StartPoint, EndPoint);
        }
        
        public void Transform(TransformMatrix matrix)
        {
            StartPoint = matrix.Apply(StartPoint);
            EndPoint = matrix.Apply(EndPoint);
        }
        
        /// <summary>
        /// 计算点到线段的最短距离
        /// 用于拾取检测
        /// </summary>
        public double DistanceToPoint(Point3D point)
        {
            double dx = EndPoint.X - StartPoint.X;
            double dy = EndPoint.Y - StartPoint.Y;
            double dz = EndPoint.Z - StartPoint.Z;
            double len2 = dx * dx + dy * dy + dz * dz;
            
            if (len2 == 0) return StartPoint.DistanceTo(point);
            
            double t = ((point.X - StartPoint.X) * dx + 
                       (point.Y - StartPoint.Y) * dy + 
                       (point.Z - StartPoint.Z) * dz) / len2;
            
            if (t < 0) return StartPoint.DistanceTo(point);
            if (t > 1) return EndPoint.DistanceTo(point);
            
            Point3D projection = new Point3D(
                StartPoint.X + t * dx,
                StartPoint.Y + t * dy,
                StartPoint.Z + t * dz
            );
            
            return projection.DistanceTo(point);
        }
        
        public override string ToString()
        {
            return $"Line {Name}: {StartPoint} -> {EndPoint}";
        }
    }
}