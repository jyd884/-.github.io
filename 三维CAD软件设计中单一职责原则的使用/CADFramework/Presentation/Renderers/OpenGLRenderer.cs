// Presentation/Renderers/OpenGLRenderer.cs
using System;
using System.Collections.Generic;
using CADFramework.GeometryKernel.Primitives;

namespace CADFramework.Presentation.Renderers
{
    /// <summary>
    /// OpenGL渲染器实现（模拟）
    /// </summary>
    public class OpenGLRenderer : IRenderer
    {
        private int _width, _height;
        private double[] _currentColor = new double[] { 1, 1, 1 };
        
        public void Initialize(int width, int height)
        {
            _width = width;
            _height = height;
            Console.WriteLine($"[OpenGL] 初始化渲染器: {width}x{height}");
        }
        
        public void BeginRender()
        {
            Console.WriteLine("[OpenGL] 开始渲染");
            // 实际OpenGL代码: glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
        }
        
        public void EndRender()
        {
            Console.WriteLine("[OpenGL] 结束渲染");
            // 实际OpenGL代码: glFlush(); SwapBuffers();
        }
        
        public void RenderShape(IShape shape, bool isSelected = false)
        {
            SetColor(isSelected ? 1.0 : 0.0, isSelected ? 0.5 : 1.0, isSelected ? 0.0 : 0.0);
            
            if (shape is Line line)
            {
                Console.WriteLine($"[OpenGL] 渲染线段: {line.Name} [{line.StartPoint} -> {line.EndPoint}]");
                // 实际OpenGL代码: glBegin(GL_LINES); glVertex3d(...); glEnd();
            }
            else if (shape is Rectangle rect)
            {
                Console.WriteLine($"[OpenGL] 渲染矩形: {rect.Name} at {rect.Center}");
                // 实际OpenGL代码: 渲染四边形
            }
            else if (shape is ExtrudedBody body)
            {
                Console.WriteLine($"[OpenGL] 渲染拉伸体: {body.Name} (高度: {body.Height})");
            }
        }
        
        public void RenderAllShapes(List<IShape> shapes, List<int> selectedIds)
        {
            BeginRender();
            foreach (var shape in shapes)
            {
                bool isSelected = selectedIds.Contains(shape.Id);
                RenderShape(shape, isSelected);
            }
            EndRender();
        }
        
        public void Clear()
        {
            Console.WriteLine("[OpenGL] 清空场景");
        }
        
        public void SetColor(double r, double g, double b)
        {
            _currentColor = new double[] { r, g, b };
            Console.WriteLine($"[OpenGL] 设置颜色: RGB({r:F2},{g:F2},{b:F2})");
            // 实际OpenGL代码: glColor3d(r, g, b);
        }
    }
}