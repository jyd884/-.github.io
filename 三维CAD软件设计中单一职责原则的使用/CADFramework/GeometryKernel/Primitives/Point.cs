// GeometryKernel/Primitives/Point.cs
using System;

namespace CADFramework.GeometryKernel.Primitives
{
    /// <summary>
    /// 点几何定义
    /// </summary>
    public class Point : IShape
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Point3D Location { get; set; }
        
        // 点的显示大小（用于渲染）
        public double DisplaySize { get; set; } = 5.0;
        
        public Point(int id, string name, Point3D location)
        {
            Id = id;
            Name = name;
            Location = location;
        }
        
        public BoundingBox GetBoundingBox()
        {
            // 点的边界框很小（用于拾取）
            double epsilon = 0.1;
            return new BoundingBox(
                Location.X - epsilon, Location.Y - epsilon, Location.Z - epsilon,
                Location.X + epsilon, Location.Y + epsilon, Location.Z + epsilon
            );
        }
        
        public IShape Clone()
        {
            return new Point(Id, Name, Location);
        }
        
        public void Transform(TransformMatrix matrix)
        {
            Location = matrix.Apply(Location);
        }
        
        /// <summary>
        /// 计算点到点的距离
        /// </summary>
        public double DistanceTo(Point other)
        {
            return Location.DistanceTo(other.Location);
        }
        
        /// <summary>
        /// 判断两点是否重合
        /// </summary>
        public bool CoincidentWith(Point other, double tolerance = 1e-6)
        {
            return DistanceTo(other) < tolerance;
        }
        
        public override string ToString()
        {
            return $"Point {Name}: {Location}";
        }
    }
}