// GeometryKernel/Operations/BooleanOp.cs
using System;
using System.Collections.Generic;
using CADFramework.GeometryKernel.Primitives;
using CADFramework.GeometryKernel.Topology;

namespace CADFramework.GeometryKernel.Operations
{
    /// <summary>
    /// 布尔运算类型
    /// </summary>
    public enum BooleanOperationType
    {
        Union,      // 并集
        Intersect,  // 交集
        Subtract    // 差集 (A - B)
    }
    
    /// <summary>
    /// 布尔运算结果
    /// </summary>
    public class BooleanResult
    {
        public Body ResultBody { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public double OperationTime { get; set; }
    }
    
    /// <summary>
    /// 布尔运算类 - 实现实体间的并、交、差运算
    /// </summary>
    public class BooleanOp
    {
        /// <summary>
        /// 执行布尔运算
        /// </summary>
        public static BooleanResult Execute(Body bodyA, Body bodyB, BooleanOperationType operation)
        {
            var result = new BooleanResult();
            var startTime = DateTime.Now;
            
            try
            {
                // 检查输入有效性
                if (!ValidateInput(bodyA, bodyB, out string error))
                {
                    result.Success = false;
                    result.ErrorMessage = error;
                    return result;
                }
                
                // 计算包围盒相交性
                var bboxA = bodyA.GetBoundingBox();
                var bboxB = bodyB.GetBoundingBox();
                
                if (!BoundingBoxesIntersect(bboxA, bboxB))
                {
                    // 不相交时的处理
                    result.ResultBody = HandleDisjointBodies(bodyA, bodyB, operation);
                }
                else
                {
                    // 相交时的处理
                    result.ResultBody = PerformBooleanOperation(bodyA, bodyB, operation);
                }
                
                result.Success = result.ResultBody != null;
                result.OperationTime = (DateTime.Now - startTime).TotalMilliseconds;
                
                if (!result.Success)
                    result.ErrorMessage = "布尔运算失败";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"异常: {ex.Message}";
            }
            
            return result;
        }
        
        /// <summary>
        /// 验证输入
        /// </summary>
        private static bool ValidateInput(Body bodyA, Body bodyB, out string error)
        {
            error = null;
            
            if (bodyA == null || bodyB == null)
            {
                error = "输入实体不能为空";
                return false;
            }
            
            if (!bodyA.IsClosed())
            {
                error = "实体A不是封闭的";
                return false;
            }
            
            if (!bodyB.IsClosed())
            {
                error = "实体B不是封闭的";
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// 检查包围盒是否相交
        /// </summary>
        private static bool BoundingBoxesIntersect(BoundingBox bboxA, BoundingBox bboxB)
        {
            return !(bboxA.MaxX < bboxB.MinX || bboxA.MinX > bboxB.MaxX ||
                     bboxA.MaxY < bboxB.MinY || bboxA.MinY > bboxB.MaxY ||
                     bboxA.MaxZ < bboxB.MinZ || bboxA.MinZ > bboxB.MaxZ);
        }
        
        /// <summary>
        /// 处理不相交的实体
        /// </summary>
        private static Body HandleDisjointBodies(Body bodyA, Body bodyB, BooleanOperationType operation)
        {
            switch (operation)
            {
                case BooleanOperationType.Union:
                    // 并集：返回两个实体的组合
                    return CombineDisjointBodies(bodyA, bodyB);
                    
                case BooleanOperationType.Intersect:
                    // 交集：返回空
                    return null;
                    
                case BooleanOperationType.Subtract:
                    // 差集：返回原实体A
                    return (Body)bodyA.Clone();
                    
                default:
                    return null;
            }
        }
        
        /// <summary>
        /// 组合不相交的实体
        /// </summary>
        private static Body CombineDisjointBodies(Body bodyA, Body bodyB)
        {
            var combined = new Body(bodyA.Id, $"{bodyA.Name}_Union_{bodyB.Name}");
            
            // 添加所有面
            foreach (var face in bodyA.Faces)
                combined.AddFace(face);
            foreach (var face in bodyB.Faces)
                combined.AddFace(face);
            
            // 重新计算体积和质心
            combined.Volume = bodyA.ComputeVolume() + bodyB.ComputeVolume();
            combined.Centroid = ComputeCombinedCentroid(bodyA, bodyB);
            
            return combined;
        }
        
        /// <summary>
        /// 执行实际的布尔运算（简化实现）
        /// </summary>
        private static Body PerformBooleanOperation(Body bodyA, Body bodyB, BooleanOperationType operation)
        {
            Console.WriteLine($"[BooleanOp] 执行 {operation} 运算");
            Console.WriteLine($"  实体A: {bodyA.Name} (体积: {bodyA.ComputeVolume():F2})");
            Console.WriteLine($"  实体B: {bodyB.Name} (体积: {bodyB.ComputeVolume():F2})");

            var result = (Body)bodyA.Clone();
            double volumeA = bodyA.ComputeVolume();
            double volumeB = bodyB.ComputeVolume();
            double overlapVolume = ComputeOverlapVolume(bodyA.GetBoundingBox(), bodyB.GetBoundingBox());

            switch (operation)
            {
                case BooleanOperationType.Union:
                    foreach (var face in bodyB.Faces)
                        result.AddFace(face);
                    result.Volume = volumeA + volumeB - overlapVolume;
                    break;
                    
                case BooleanOperationType.Intersect:
                    result = (Body)(volumeA < volumeB ? bodyA.Clone() : bodyB.Clone());
                    result.Volume = overlapVolume;
                    break;
                    
                case BooleanOperationType.Subtract:
                    result.Volume = Math.Max(0, volumeA - overlapVolume);
                    break;
            }
            
            result.Name = $"Result_{operation}";
            result.Centroid = ComputeCombinedCentroid(bodyA, bodyB);
            
            Console.WriteLine($"  结果体积: {result.Volume:F2}");
            
            return result;
        }
        
        /// <summary>
        /// 计算组合体的质心
        /// </summary>
        private static Point3D ComputeCombinedCentroid(Body bodyA, Body bodyB)
        {
            double volA = bodyA.ComputeVolume();
            double volB = bodyB.ComputeVolume();
            double totalVol = volA + volB;
            
            if (totalVol == 0)
                return new Point3D(0, 0, 0);
            
            var cA = bodyA.ComputeCentroid();
            var cB = bodyB.ComputeCentroid();
            
            return new Point3D(
                (cA.X * volA + cB.X * volB) / totalVol,
                (cA.Y * volA + cB.Y * volB) / totalVol,
                (cA.Z * volA + cB.Z * volB) / totalVol
            );
        }
        
        /// <summary>
        /// 快速检测两个实体是否相交
        /// </summary>
        public static bool Intersects(Body bodyA, Body bodyB)
        {
            var bboxA = bodyA.GetBoundingBox();
            var bboxB = bodyB.GetBoundingBox();
            return BoundingBoxesIntersect(bboxA, bboxB);
        }

        private static double ComputeOverlapVolume(BoundingBox bboxA, BoundingBox bboxB)
        {
            double overlapX = Math.Max(0, Math.Min(bboxA.MaxX, bboxB.MaxX) - Math.Max(bboxA.MinX, bboxB.MinX));
            double overlapY = Math.Max(0, Math.Min(bboxA.MaxY, bboxB.MaxY) - Math.Max(bboxA.MinY, bboxB.MinY));
            double overlapZ = Math.Max(0, Math.Min(bboxA.MaxZ, bboxB.MaxZ) - Math.Max(bboxA.MinZ, bboxB.MinZ));
            return overlapX * overlapY * overlapZ;
        }
    }
}