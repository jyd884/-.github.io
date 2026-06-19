// GeometryKernel/Operations/ExtrudeOp.cs
using System;
using CADFramework.GeometryKernel.Primitives;

namespace CADFramework.GeometryKernel.Operations
{
    /// <summary>
    /// 拉伸操作 - 将2D形状拉伸成3D实体
    /// 这是纯几何操作，不涉及UI
    /// </summary>
    public class ExtrudeOp
    {
        /// <summary>
        /// 拉伸矩形成为长方体
        /// </summary>
        public static ExtrudedBody ExtrudeRectangle(Rectangle rect, double height)
        {
            var body = new ExtrudedBody(rect.Id * 100, $"Extruded_{rect.Name}");
            body.BaseShape = rect.Clone();
            body.Height = height;
            body.ExtrudeDirection = new Point3D(0, 0, height);
            
            // 计算新的边界框
            var bbox = rect.GetBoundingBox();
            body.BoundingBox = new BoundingBox(
                bbox.MinX, bbox.MinY, bbox.MinZ,
                bbox.MaxX, bbox.MaxY, bbox.MaxZ + height
            );
            
            return body;
        }
    }
    
    /// <summary>
    /// 拉伸体（3D实体）
    /// </summary>
    public class ExtrudedBody : IShape
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IShape BaseShape { get; set; }
        public double Height { get; set; }
        public Point3D ExtrudeDirection { get; set; }
        public BoundingBox BoundingBox { get; set; }
        
        public ExtrudedBody(int id, string name)
        {
            Id = id;
            Name = name;
        }
        
        public BoundingBox GetBoundingBox() => BoundingBox;
        
        public IShape Clone()
        {
            var clone = new ExtrudedBody(Id, Name);
            clone.BaseShape = BaseShape.Clone();
            clone.Height = Height;
            clone.ExtrudeDirection = ExtrudeDirection;
            clone.BoundingBox = BoundingBox;
            return clone;
        }
        
        public void Transform(TransformMatrix matrix)
        {
            BaseShape.Transform(matrix);
            ExtrudeDirection = matrix.Apply(ExtrudeDirection);
            // 重新计算边界框
            var bbox = BaseShape.GetBoundingBox();
            BoundingBox = new BoundingBox(
                bbox.MinX, bbox.MinY, bbox.MinZ,
                bbox.MaxX + ExtrudeDirection.X,
                bbox.MaxY + ExtrudeDirection.Y,
                bbox.MaxZ + ExtrudeDirection.Z
            );
        }
        
        public override string ToString()
        {
            return $"ExtrudedBody {Name}: Height={Height}";
        }
    }
}