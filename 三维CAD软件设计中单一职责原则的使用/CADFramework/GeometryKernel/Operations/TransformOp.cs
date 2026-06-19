// GeometryKernel/Operations/TransformOp.cs
using System;
using CADFramework.GeometryKernel.Primitives;
using CADFramework.GeometryKernel.Topology;

namespace CADFramework.GeometryKernel.Operations
{
    /// <summary>
    /// 变换类型
    /// </summary>
    public enum TransformType
    {
        Translate,   // 平移
        Rotate,      // 旋转
        Scale,       // 缩放
        Mirror,      // 镜像
        Shear        // 剪切
    }
    
    /// <summary>
    /// 变换操作类
    /// </summary>
    public class TransformOp
    {
        /// <summary>
        /// 平移变换
        /// </summary>
        public static void Translate(IShape shape, double dx, double dy, double dz)
        {
            var matrix = TransformMatrix.Translation(dx, dy, dz);
            shape.Transform(matrix);
            
            Console.WriteLine($"[Transform] 平移 {shape.Name}: ({dx},{dy},{dz})");
        }
        
        /// <summary>
        /// 平移多个形状
        /// </summary>
        public static void TranslateMany(IShape[] shapes, double dx, double dy, double dz)
        {
            var matrix = TransformMatrix.Translation(dx, dy, dz);
            foreach (var shape in shapes)
            {
                shape.Transform(matrix);
            }
            Console.WriteLine($"[Transform] 批量平移 {shapes.Length} 个对象");
        }
        
        /// <summary>
        /// 旋转变换（绕Z轴）
        /// </summary>
        public static void RotateZ(IShape shape, double angleDegrees)
        {
            double angleRad = angleDegrees * Math.PI / 180.0;
            double cos = Math.Cos(angleRad);
            double sin = Math.Sin(angleRad);
            
            var matrix = new TransformMatrix();
            matrix.Matrix[0, 0] = cos;
            matrix.Matrix[0, 1] = -sin;
            matrix.Matrix[1, 0] = sin;
            matrix.Matrix[1, 1] = cos;
            
            shape.Transform(matrix);
            Console.WriteLine($"[Transform] 旋转 {shape.Name}: {angleDegrees}° 绕Z轴");
        }
        
        /// <summary>
        /// 旋转变换（绕任意轴）
        /// </summary>
        public static void RotateAroundAxis(IShape shape, Point3D axisPoint, Point3D axisDir, double angleDegrees)
        {
            // 实现绕任意轴的旋转（使用罗德里格斯旋转公式）
            double angleRad = angleDegrees * Math.PI / 180.0;
            double cos = Math.Cos(angleRad);
            double sin = Math.Sin(angleRad);
            double oneMinusCos = 1 - cos;
            
            // 归一化轴方向
            double len = Math.Sqrt(axisDir.X * axisDir.X + axisDir.Y * axisDir.Y + axisDir.Z * axisDir.Z);
            if (len < 1e-6) return;
            
            double ux = axisDir.X / len;
            double uy = axisDir.Y / len;
            double uz = axisDir.Z / len;
            
            // 构建旋转矩阵
            var matrix = new TransformMatrix();
            matrix.Matrix[0, 0] = cos + ux * ux * oneMinusCos;
            matrix.Matrix[0, 1] = ux * uy * oneMinusCos - uz * sin;
            matrix.Matrix[0, 2] = ux * uz * oneMinusCos + uy * sin;
            matrix.Matrix[1, 0] = uy * ux * oneMinusCos + uz * sin;
            matrix.Matrix[1, 1] = cos + uy * uy * oneMinusCos;
            matrix.Matrix[1, 2] = uy * uz * oneMinusCos - ux * sin;
            matrix.Matrix[2, 0] = uz * ux * oneMinusCos - uy * sin;
            matrix.Matrix[2, 1] = uz * uy * oneMinusCos + ux * sin;
            matrix.Matrix[2, 2] = cos + uz * uz * oneMinusCos;
            
            // 先平移到原点，旋转，再平移回来
            var toOrigin = TransformMatrix.Translation(-axisPoint.X, -axisPoint.Y, -axisPoint.Z);
            var fromOrigin = TransformMatrix.Translation(axisPoint.X, axisPoint.Y, axisPoint.Z);
            
            shape.Transform(toOrigin);
            shape.Transform(matrix);
            shape.Transform(fromOrigin);
            
            Console.WriteLine($"[Transform] 旋转 {shape.Name}: {angleDegrees}° 绕轴 ({ux:F2},{uy:F2},{uz:F2})");
        }
        
        /// <summary>
        /// 缩放变换
        /// </summary>
        public static void Scale(IShape shape, double sx, double sy, double sz, Point3D? center = null)
        {
            Point3D scaleCenter = center ?? new Point3D(0, 0, 0);
            
            // 平移到原点
            var toOrigin = TransformMatrix.Translation(-scaleCenter.X, -scaleCenter.Y, -scaleCenter.Z);
            // 缩放
            var scaleMatrix = new TransformMatrix();
            scaleMatrix.Matrix[0, 0] = sx;
            scaleMatrix.Matrix[1, 1] = sy;
            scaleMatrix.Matrix[2, 2] = sz;
            // 平移回去
            var fromOrigin = TransformMatrix.Translation(scaleCenter.X, scaleCenter.Y, scaleCenter.Z);
            
            shape.Transform(toOrigin);
            shape.Transform(scaleMatrix);
            shape.Transform(fromOrigin);
            
            Console.WriteLine($"[Transform] 缩放 {shape.Name}: ({sx:F2},{sy:F2},{sz:F2})");
        }
        
        /// <summary>
        /// 均匀缩放
        /// </summary>
        public static void ScaleUniform(IShape shape, double scale, Point3D? center = null)
        {
            Scale(shape, scale, scale, scale, center);
        }
        
        /// <summary>
        /// 镜像变换
        /// </summary>
        public static void Mirror(IShape shape, MirrorPlane plane)
        {
            var matrix = new TransformMatrix();
            
            switch (plane)
            {
                case MirrorPlane.XY:
                    matrix.Matrix[2, 2] = -1;
                    break;
                case MirrorPlane.XZ:
                    matrix.Matrix[1, 1] = -1;
                    break;
                case MirrorPlane.YZ:
                    matrix.Matrix[0, 0] = -1;
                    break;
            }
            
            shape.Transform(matrix);
            Console.WriteLine($"[Transform] 镜像 {shape.Name}: {plane}平面");
        }
        
        /// <summary>
        /// 剪切变换
        /// </summary>
        public static void Shear(IShape shape, double shXY, double shXZ, double shYX, double shYZ, double shZX, double shZY)
        {
            var matrix = new TransformMatrix();
            matrix.Matrix[0, 1] = shXY;
            matrix.Matrix[0, 2] = shXZ;
            matrix.Matrix[1, 0] = shYX;
            matrix.Matrix[1, 2] = shYZ;
            matrix.Matrix[2, 0] = shZX;
            matrix.Matrix[2, 1] = shZY;
            
            shape.Transform(matrix);
            Console.WriteLine($"[Transform] 剪切 {shape.Name}");
        }
        
        /// <summary>
        /// 获取变换矩阵的逆矩阵
        /// </summary>
        public static TransformMatrix Invert(TransformMatrix matrix)
        {
            // 简化：只处理平移和缩放组合
            var inverse = new TransformMatrix();
            
            // 逆平移
            inverse.Matrix[0, 3] = -matrix.Matrix[0, 3];
            inverse.Matrix[1, 3] = -matrix.Matrix[1, 3];
            inverse.Matrix[2, 3] = -matrix.Matrix[2, 3];
            
            // 逆缩放
            if (Math.Abs(matrix.Matrix[0, 0]) > 1e-6)
                inverse.Matrix[0, 0] = 1.0 / matrix.Matrix[0, 0];
            if (Math.Abs(matrix.Matrix[1, 1]) > 1e-6)
                inverse.Matrix[1, 1] = 1.0 / matrix.Matrix[1, 1];
            if (Math.Abs(matrix.Matrix[2, 2]) > 1e-6)
                inverse.Matrix[2, 2] = 1.0 / matrix.Matrix[2, 2];
            
            return inverse;
        }
    }
    
    /// <summary>
    /// 镜像平面枚举
    /// </summary>
    public enum MirrorPlane
    {
        XY,     // 关于XY平面对称
        XZ,     // 关于XZ平面对称
        YZ      // 关于YZ平面对称
    }
}