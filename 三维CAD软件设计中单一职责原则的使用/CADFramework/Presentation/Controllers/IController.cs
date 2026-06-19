// Presentation/Controllers/IController.cs
using System;
using CADFramework.GeometryKernel.Primitives;

namespace CADFramework.Presentation.Controllers
{
    /// <summary>
    /// 控制器接口 - 处理用户输入
    /// </summary>
    public interface IController
    {
        void HandleMouseClick(Point3D position, MouseButton button);
        void HandleMouseMove(Point3D position);
        void HandleMouseDrag(Point3D start, Point3D end, MouseButton button);
        void HandleKeyPress(char key);
        string GetControllerName();
    }
    
    /// <summary>
    /// 鼠标按钮枚举
    /// </summary>
    public enum MouseButton
    {
        Left,
        Right,
        Middle
    }
    
    /// <summary>
    /// 控制器类型枚举
    /// </summary>
    public enum ControllerType
    {
        Selection,   // 选择控制器
        DrawLine,    // 绘制线段
        DrawRect,    // 绘制矩形
        DrawCircle,  // 绘制圆
        Pan,         // 平移视图
        Zoom,        // 缩放视图
        RotateView   // 旋转视图
    }
}