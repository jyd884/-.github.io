// GeometryKernel/Primitives/Circle.cs
using System;

namespace CADFramework.GeometryKernel.Primitives
{
    /// <summary>
    /// 圆几何定义
    /// </summary>
    public class Circle : IShape
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Point3D Center { get; set; }
        public double Radius { get; set; }
        public Point3D Normal { get; set; }  // 圆的法向量（用于3D圆）
        
        public Circle(int id, string name, Point3D center, double radius, Point3D? normal = null)
        {
            Id = id;
            Name = name;
            Center = center;
            Radius = radius;
            Normal = normal ?? new Point3D(0, 0, 1); // 默认XY平面
        }
        
        public BoundingBox GetBoundingBox()
        {
            return new BoundingBox(
                Center.X - Radius, Center.Y - Radius, Center.Z - Radius,
                Center.X + Radius, Center.Y + Radius, Center.Z + Radius
            );
        }
        
        public IShape Clone()
        {
            return new Circle(Id, Name, Center, Radius, Normal);
        }
        
        public void Transform(TransformMatrix matrix)
        {
            Center = matrix.Apply(Center);
            // 法向量也需要变换（仅旋转部分，不平移）
            // 简化处理：只变换中心点
        }
        
        /// <summary>
        /// 计算点到圆的距离
        /// </summary>
        public double DistanceToPoint(Point3D point)
        {
            // 投影点到圆平面
            Point3D projected = ProjectToPlane(point);
            double distanceToCenter = projected.DistanceTo(Center);
            return Math.Abs(distanceToCenter - Radius);
        }
        
        /// <summary>
        /// 将点投影到圆所在的平面
        /// </summary>
        private Point3D ProjectToPlane(Point3D point)
        {
            // 简化：假设圆在XY平面
            return new Point3D(point.X, point.Y, Center.Z);
        }
        
        /// <summary>
        /// 计算圆的周长
        /// </summary>
        public double GetCircumference()
        {
            return 2 * Math.PI * Radius;
        }
        
        /// <summary>
        /// 计算圆的面积
        /// </summary>
        public double GetArea()
        {
            return Math.PI * Radius * Radius;
        }
        
        /// <summary>
        /// 判断点是否在圆内
        /// </summary>
        public bool ContainsPoint(Point3D point)
        {
            Point3D projected = ProjectToPlane(point);
            return projected.DistanceTo(Center) <= Radius;
        }
        
        public override string ToString()
        {
            return $"Circle {Name}: Center={Center}, Radius={Radius:F2}";
        }
    }
}