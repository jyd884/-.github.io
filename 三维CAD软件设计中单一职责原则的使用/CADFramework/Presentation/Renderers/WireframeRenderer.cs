// Presentation/Renderers/WireframeRenderer.cs
using System;
using System.Collections.Generic;
using CADFramework.GeometryKernel.Primitives;
using CADFramework.GeometryKernel.Topology;

namespace CADFramework.Presentation.Renderers
{
    /// <summary>
    /// 线框渲染器 - 只渲染轮廓线，不渲染面
    /// </summary>
    public class WireframeRenderer : IRenderer
    {
        private int _width, _height;
        private double[] _currentColor = new double[] { 0, 0, 0 };
        private bool _showHiddenLines = false;
        
        public WireframeRenderer(bool showHiddenLines = false)
        {
            _showHiddenLines = showHiddenLines;
        }
        
        public void Initialize(int width, int height)
        {
            _width = width;
            _height = height;
            Console.WriteLine($"[Wireframe] 初始化线框渲染器: {width}x{height}, 显示隐藏线={_showHiddenLines}");
        }
        
        public void BeginRender()
        {
            Console.WriteLine("[Wireframe] 开始线框渲染");
        }
        
        public void EndRender()
        {
            Console.WriteLine("[Wireframe] 结束线框渲染");
        }
        
        public void RenderShape(IShape shape, bool isSelected = false)
        {
            SetColor(isSelected ? 1.0 : 0.0, isSelected ? 0.0 : 0.0, isSelected ? 0.0 : 0.0);
            
            if (shape is Line line)
            {
                Console.WriteLine($"[Wireframe] 渲染线段: {line.Name} [{line.StartPoint} -> {line.EndPoint}]");
            }
            else if (shape is Rectangle rect)
            {
                Console.WriteLine($"[Wireframe] 渲染矩形边框: {rect.Name}");
                // 渲染矩形的四条边
                var vertices = rect.Vertices;
                for (int i = 0; i < vertices.Count; i++)
                {
                    var p1 = vertices[i];
                    var p2 = vertices[(i + 1) % vertices.Count];
                    Console.WriteLine($"  边 {i+1}: {p1} -> {p2}");
                }
            }
            else if (shape is Circle circle)
            {
                Console.WriteLine($"[Wireframe] 渲染圆: {circle.Name}, 中心={circle.Center}, 半径={circle.Radius}");
            }
            else if (shape is Body body)
            {
                Console.WriteLine($"[Wireframe] 渲染实体轮廓: {body.Name}");
                foreach (var edge in body.Edges)
                {
                    Console.WriteLine($"  边: {edge.Name}");
                }
            }
        }
        
        public void RenderAllShapes(List<IShape> shapes, List<int> selectedIds)
        {
            BeginRender();
            Console.WriteLine($"[Wireframe] 开始批量渲染 {shapes.Count} 个对象");
            
            foreach (var shape in shapes)
            {
                bool isSelected = selectedIds.Contains(shape.Id);
                RenderShape(shape, isSelected);
            }
            
            EndRender();
        }
        
        public void Clear()
        {
            Console.WriteLine("[Wireframe] 清空线框场景");
        }
        
        public void SetColor(double r, double g, double b)
        {
            _currentColor = new double[] { r, g, b };
            Console.WriteLine($"[Wireframe] 设置线框颜色: RGB({r:F2},{g:F2},{b:F2})");
        }
        
        /// <summary>
        /// 设置是否显示隐藏线
        /// </summary>
        public void SetShowHiddenLines(bool show)
        {
            _showHiddenLines = show;
            Console.WriteLine($"[Wireframe] 显示隐藏线: {show}");
        }
    }
}