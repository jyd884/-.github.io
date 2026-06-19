// GeometryKernel/Primitives/IShape.cs
using System;

namespace CADFramework.GeometryKernel.Primitives
{
    /// <summary>
    /// 几何形状基类接口
    /// 几何内核只关注数学定义，不关心如何显示
    /// </summary>
    public interface IShape
    {
        int Id { get; set; }
        string Name { get; set; }
        BoundingBox GetBoundingBox();
        IShape Clone();
        void Transform(TransformMatrix matrix);
    }

    /// <summary>
    /// 边界框（用于碰撞检测和视图裁剪）
    /// </summary>
    public struct BoundingBox
    {
        public double MinX, MinY, MinZ;
        public double MaxX, MaxY, MaxZ;
        
        public BoundingBox(double minX, double minY, double minZ, 
                          double maxX, double maxY, double maxZ)
        {
            MinX = minX; MinY = minY; MinZ = minZ;
            MaxX = maxX; MaxY = maxY; MaxZ = maxZ;
        }
        
        public bool Contains(Point3D point)
        {
            return point.X >= MinX && point.X <= MaxX &&
                   point.Y >= MinY && point.Y <= MaxY &&
                   point.Z >= MinZ && point.Z <= MaxZ;
        }
    }

    /// <summary>
    /// 3D点结构
    /// </summary>
    public struct Point3D
    {
        public double X, Y, Z;
        
        public Point3D(double x, double y, double z)
        {
            X = x; Y = y; Z = z;
        }
        
        public static Point3D operator +(Point3D a, Point3D b)
        {
            return new Point3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        
        public static Point3D operator -(Point3D a, Point3D b)
        {
            return new Point3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
        
        public double DistanceTo(Point3D other)
        {
            double dx = X - other.X;
            double dy = Y - other.Y;
            double dz = Z - other.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
        
        public override string ToString()
        {
            return $"({X:F2}, {Y:F2}, {Z:F2})";
        }
    }

    /// <summary>
    /// 变换矩阵
    /// </summary>
    public class TransformMatrix
    {
        public double[,] Matrix { get; private set; }
        
        public TransformMatrix()
        {
            Matrix = new double[4, 4];
            // 初始化为单位矩阵
            for (int i = 0; i < 4; i++)
                Matrix[i, i] = 1;
        }
        
        public static TransformMatrix Translation(double dx, double dy, double dz)
        {
            var m = new TransformMatrix();
            m.Matrix[0, 3] = dx;
            m.Matrix[1, 3] = dy;
            m.Matrix[2, 3] = dz;
            return m;
        }
        
        public Point3D Apply(Point3D point)
        {
            double x = Matrix[0, 0] * point.X + Matrix[0, 1] * point.Y + 
                       Matrix[0, 2] * point.Z + Matrix[0, 3];
            double y = Matrix[1, 0] * point.X + Matrix[1, 1] * point.Y + 
                       Matrix[1, 2] * point.Z + Matrix[1, 3];
            double z = Matrix[2, 0] * point.X + Matrix[2, 1] * point.Y + 
                       Matrix[2, 2] * point.Z + Matrix[2, 3];
            return new Point3D(x, y, z);
        }
    }
}