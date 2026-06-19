// GeometryKernel/Primitives/Rectangle.cs
using System;
using System.Collections.Generic;

namespace CADFramework.GeometryKernel.Primitives
{
    /// <summary>
    /// 矩形几何定义
    /// </summary>
    public class Rectangle : IShape
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Point3D Center { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        
        // 矩形的四个顶点（用于拾取检测）
        public List<Point3D> Vertices { get; private set; }
        
        public Rectangle(int id, string name, Point3D center, double width, double height)
        {
            Id = id;
            Name = name;
            Center = center;
            Width = width;
            Height = height;
            
            UpdateVertices();
        }
        
        private void UpdateVertices()
        {
            double halfW = Width / 2;
            double halfH = Height / 2;
            Vertices = new List<Point3D>
            {
                new Point3D(Center.X - halfW, Center.Y - halfH, Center.Z),
                new Point3D(Center.X + halfW, Center.Y - halfH, Center.Z),
                new Point3D(Center.X + halfW, Center.Y + halfH, Center.Z),
                new Point3D(Center.X - halfW, Center.Y + halfH, Center.Z)
            };
        }
        
        public BoundingBox GetBoundingBox()
        {
            double halfW = Width / 2;
            double halfH = Height / 2;
            return new BoundingBox(
                Center.X - halfW, Center.Y - halfH, Center.Z,
                Center.X + halfW, Center.Y + halfH, Center.Z
            );
        }
        
        public IShape Clone()
        {
            return new Rectangle(Id, Name, Center, Width, Height);
        }
        
        public void Transform(TransformMatrix matrix)
        {
            Center = matrix.Apply(Center);
            UpdateVertices();
        }
        
        /// <summary>
        /// 判断点是否在矩形内部
        /// </summary>
        public bool ContainsPoint(Point3D point)
        {
            double halfW = Width / 2;
            double halfH = Height / 2;
            return Math.Abs(point.X - Center.X) <= halfW &&
                   Math.Abs(point.Y - Center.Y) <= halfH;
        }
        
        public override string ToString()
        {
            return $"Rectangle {Name}: Center={Center}, W={Width}, H={Height}";
        }
    }
}