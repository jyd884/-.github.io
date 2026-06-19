// Presentation/Renderers/IRenderer.cs
using System.Collections.Generic;
using CADFramework.GeometryKernel.Primitives;

namespace CADFramework.Presentation.Renderers
{
    /// <summary>
    /// 渲染器接口
    /// 不同的渲染实现（OpenGL, DirectX, GDI+等）
    /// </summary>
    public interface IRenderer
    {
        void Initialize(int width, int height);
        void BeginRender();
        void EndRender();
        void RenderShape(IShape shape, bool isSelected = false);
        void RenderAllShapes(List<IShape> shapes, List<int> selectedIds);
        void Clear();
        void SetColor(double r, double g, double b);
    }
}