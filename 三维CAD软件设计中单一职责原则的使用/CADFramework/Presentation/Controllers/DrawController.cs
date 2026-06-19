// Presentation/Controllers/DrawController.cs
using System;
using System.Collections.Generic;
using CADFramework.Bridge;
using CADFramework.GeometryKernel.Events;
using CADFramework.GeometryKernel.Primitives;

namespace CADFramework.Presentation.Controllers
{
    /// <summary>
    /// 绘图控制器 - 处理交互式绘图
    /// </summary>
    public class DrawController : IController
    {
        private List<IShape> _shapeList;
        private CommandProcessor _commandProcessor;
        private ControllerType _drawType;
        private List<Point3D> _tempPoints;
        private IShape _previewShape;
        
        public DrawController(List<IShape> shapeList, CommandProcessor commandProcessor)
        {
            _shapeList = shapeList;
            _commandProcessor = commandProcessor;
            _tempPoints = new List<Point3D>();
        }
        
        public void SetDrawType(ControllerType type)
        {
            if (IsDrawType(type))
            {
                _drawType = type;
                ClearTempData();
                Console.WriteLine($"[DrawController] 设置绘图类型: {type}");
            }
        }
        
        public void HandleMouseClick(Point3D position, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;
            
            _tempPoints.Add(position);
            
            switch (_drawType)
            {
                case ControllerType.DrawLine:
                    if (_tempPoints.Count == 2)
                    {
                        CreateLine();
                        ClearTempData();
                    }
                    break;
                    
                case ControllerType.DrawRect:
                    if (_tempPoints.Count == 2)
                    {
                        CreateRectangle();
                        ClearTempData();
                    }
                    break;
                    
                case ControllerType.DrawCircle:
                    if (_tempPoints.Count == 1)
                    {
                        // 等待第二个点确定半径
                    }
                    else if (_tempPoints.Count == 2)
                    {
                        CreateCircle();
                        ClearTempData();
                    }
                    break;
            }
        }
        
        public void HandleMouseMove(Point3D position)
        {
            if (_tempPoints.Count > 0 && _tempPoints.Count < GetRequiredPoints())
            {
                UpdatePreview(position);
            }
        }
        
        public void HandleMouseDrag(Point3D start, Point3D end, MouseButton button)
        {
            // 拖拽绘图
            if (button == MouseButton.Left && _tempPoints.Count == 0)
            {
                _tempPoints.Add(start);
                _tempPoints.Add(end);
                
                switch (_drawType)
                {
                    case ControllerType.DrawRect:
                        CreateRectangle();
                        break;
                    case ControllerType.DrawCircle:
                        CreateCircle();
                        break;
                }
                
                ClearTempData();
            }
        }
        
        public void HandleKeyPress(char key)
        {
            if (key == 27) // ESC
            {
                ClearTempData();
                Console.WriteLine("[DrawController] 取消绘图");
            }
        }
        
        public string GetControllerName() => "DrawController";
        
        private bool IsDrawType(ControllerType type)
        {
            return type == ControllerType.DrawLine ||
                   type == ControllerType.DrawRect ||
                   type == ControllerType.DrawCircle;
        }
        
        private int GetRequiredPoints()
        {
            switch (_drawType)
            {
                case ControllerType.DrawLine: return 2;
                case ControllerType.DrawRect: return 2;
                case ControllerType.DrawCircle: return 2;
                default: return 0;
            }
        }
        
        private void CreateLine()
        {
            if (_tempPoints.Count < 2) return;
            
            int id = _shapeList.Count + 1;
            var line = new Line(id, $"Line_{id}", _tempPoints[0], _tempPoints[1]);
            
            var command = new CreateShapeCommand(_shapeList, line);
            _commandProcessor.ExecuteCommand(command);
            
            Console.WriteLine($"[DrawController] 创建线段: {line}");
        }
        
        private void CreateRectangle()
        {
            if (_tempPoints.Count < 2) return;
            
            // 计算矩形中心和尺寸
            double centerX = (_tempPoints[0].X + _tempPoints[1].X) / 2;
            double centerY = (_tempPoints[0].Y + _tempPoints[1].Y) / 2;
            double width = Math.Abs(_tempPoints[1].X - _tempPoints[0].X);
            double height = Math.Abs(_tempPoints[1].Y - _tempPoints[0].Y);
            
            int id = _shapeList.Count + 1;
            var rect = new Rectangle(id, $"Rect_{id}", 
                new Point3D(centerX, centerY, 0), width, height);
            
            var command = new CreateShapeCommand(_shapeList, rect);
            _commandProcessor.ExecuteCommand(command);
            
            Console.WriteLine($"[DrawController] 创建矩形: {rect}");
        }
        
        private void CreateCircle()
        {
            if (_tempPoints.Count < 2) return;
            
            double radius = _tempPoints[0].DistanceTo(_tempPoints[1]);
            int id = _shapeList.Count + 1;
            var circle = new Circle(id, $"Circle_{id}", _tempPoints[0], radius);
            
            var command = new CreateShapeCommand(_shapeList, circle);
            _commandProcessor.ExecuteCommand(command);
            
            Console.WriteLine($"[DrawController] 创建圆: {circle}");
        }
        
        private void UpdatePreview(Point3D currentPoint)
        {
            // 更新预览形状（虚线显示）
            if (_tempPoints.Count == 1)
            {
                Console.WriteLine($"[DrawController] 预览: {_tempPoints[0]} -> {currentPoint}");
            }
        }
        
        private void ClearTempData()
        {
            _tempPoints.Clear();
            _previewShape = null;
        }
    }
}