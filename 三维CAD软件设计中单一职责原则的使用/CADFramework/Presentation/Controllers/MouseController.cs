// Presentation/Controllers/MouseController.cs
using System;
using CADFramework.Bridge;
using CADFramework.GeometryKernel.Primitives;

namespace CADFramework.Presentation.Controllers
{
    /// <summary>
    /// 鼠标控制器 - 处理鼠标交互
    /// </summary>
    public class MouseController : IController
    {
        private SelectionManager _selectionManager;
        private CommandProcessor _commandProcessor;
        private Point3D _lastMousePosition;
        private bool _isDragging = false;
        private ControllerType _currentMode = ControllerType.Selection;
        
        public event EventHandler<Point3D> MouseClicked;
        public event EventHandler<DragEventArgs> MouseDragged;
        
        public MouseController(SelectionManager selectionManager, CommandProcessor commandProcessor)
        {
            _selectionManager = selectionManager;
            _commandProcessor = commandProcessor;
        }
        
        public void SetMode(ControllerType mode)
        {
            _currentMode = mode;
            Console.WriteLine($"[MouseController] 切换到模式: {mode}");
        }
        
        public void HandleMouseClick(Point3D position, MouseButton button)
        {
            Console.WriteLine($"[MouseController] 鼠标点击: {button} at {position}");
            
            if (button == MouseButton.Left)
            {
                switch (_currentMode)
                {
                    case ControllerType.Selection:
                        // 选择逻辑由外部处理
                        MouseClicked?.Invoke(this, position);
                        break;
                        
                    case ControllerType.DrawLine:
                    case ControllerType.DrawRect:
                    case ControllerType.DrawCircle:
                        // 绘图模式下的点击处理
                        HandleDrawClick(position);
                        break;
                }
            }
            else if (button == MouseButton.Right)
            {
                // 右键菜单或取消操作
                CancelCurrentOperation();
            }
        }
        
        public void HandleMouseMove(Point3D position)
        {
            if (_isDragging)
            {
                HandleMouseDrag(_lastMousePosition, position, MouseButton.Left);
            }
            
            _lastMousePosition = position;
        }
        
        public void HandleMouseDrag(Point3D start, Point3D end, MouseButton button)
        {
            _isDragging = true;
            Console.WriteLine($"[MouseController] 拖拽: {start} -> {end}");
            
            var args = new DragEventArgs
            {
                StartPosition = start,
                EndPosition = end,
                Delta = new Point3D(end.X - start.X, end.Y - start.Y, end.Z - start.Z),
                Button = button
            };
            
            MouseDragged?.Invoke(this, args);
        }
        
        public void HandleKeyPress(char key)
        {
            Console.WriteLine($"[MouseController] 按键: {key}");
            
            switch (key)
            {
                case 's':
                case 'S':
                    SetMode(ControllerType.Selection);
                    break;
                case 'l':
                case 'L':
                    SetMode(ControllerType.DrawLine);
                    break;
                case 'r':
                case 'R':
                    SetMode(ControllerType.DrawRect);
                    break;
                case 'c':
                case 'C':
                    SetMode(ControllerType.DrawCircle);
                    break;
                case (char)27: // ESC键
                    CancelCurrentOperation();
                    break;
            }
        }
        
        public string GetControllerName() => "MouseController";
        
        private void HandleDrawClick(Point3D position)
        {
            // 简化的绘图逻辑
            Console.WriteLine($"[MouseController] 绘图模式点击: {_currentMode}");
            // 实际实现中会累积点并创建形状
        }
        
        private void CancelCurrentOperation()
        {
            _isDragging = false;
            Console.WriteLine("[MouseController] 取消当前操作");
        }
    }
    
    /// <summary>
    /// 拖拽事件参数
    /// </summary>
    public class DragEventArgs : EventArgs
    {
        public Point3D StartPosition { get; set; }
        public Point3D EndPosition { get; set; }
        public Point3D Delta { get; set; }
        public MouseButton Button { get; set; }
    }
}