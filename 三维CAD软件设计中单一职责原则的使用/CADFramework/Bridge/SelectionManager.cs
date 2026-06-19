// Bridge/SelectionManager.cs
using System;
using System.Collections.Generic;
using CADFramework.GeometryKernel.Events;
using CADFramework.GeometryKernel.Primitives;

namespace CADFramework.Bridge
{
    /// <summary>
    /// 选择管理器 - 管理当前选中的几何对象
    /// 完全独立于UI实现
    /// </summary>
    public class SelectionManager
    {
        private List<IShape> _selectedShapes = new List<IShape>();
        
        public IReadOnlyList<IShape> SelectedShapes => _selectedShapes;
        
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
        
        /// <summary>
        /// 选择单个形状
        /// </summary>
        public void SelectShape(IShape shape)
        {
            _selectedShapes.Clear();
            _selectedShapes.Add(shape);
            
            // 发布选择改变事件
            var args = new SelectionChangedEventArgs(shape, true);
            SelectionChanged?.Invoke(this, args);
            
            // 同时发布几何事件（通知其他监听者）
            GeometryEventPublisher.Instance.Publish(
                new GeometryEventArgs(GeometryEventType.SelectionChanged, shape.Id, shape.Name, shape)
            );
        }
        
        /// <summary>
        /// 多选
        /// </summary>
        public void SelectShapes(List<IShape> shapes)
        {
            _selectedShapes.Clear();
            _selectedShapes.AddRange(shapes);
            
            var args = new SelectionChangedEventArgs(shapes, true);
            SelectionChanged?.Invoke(this, args);
        }
        
        /// <summary>
        /// 清除选择
        /// </summary>
        public void ClearSelection()
        {
            var oldSelection = new List<IShape>(_selectedShapes);
            _selectedShapes.Clear();
            
            var args = new SelectionChangedEventArgs(oldSelection, false);
            SelectionChanged?.Invoke(this, args);
        }
        
        /// <summary>
        /// 根据鼠标位置拾取形状
        /// 这是一个业务逻辑，不涉及UI坐标转换
        /// </summary>
        public IShape PickShapeAtPoint(Point3D worldPoint, List<IShape> allShapes, double tolerance = 0.1)
        {
            IShape closest = null;
            double minDistance = tolerance;
            
            foreach (var shape in allShapes)
            {
                double distance = GetDistanceToShape(shape, worldPoint);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = shape;
                }
            }
            
            return closest;
        }
        
        private double GetDistanceToShape(IShape shape, Point3D point)
        {
            // 根据形状类型计算距离
            if (shape is Line line)
                return line.DistanceToPoint(point);
            else if (shape is Rectangle rect)
                return rect.ContainsPoint(point) ? 0 : double.MaxValue;
            else
                return double.MaxValue;
        }
    }
    
    /// <summary>
    /// 选择改变事件参数
    /// </summary>
    public class SelectionChangedEventArgs : EventArgs
    {
        public List<IShape> Shapes { get; set; }
        public bool IsSelected { get; set; }
        
        public SelectionChangedEventArgs(IShape shape, bool isSelected)
        {
            Shapes = new List<IShape> { shape };
            IsSelected = isSelected;
        }
        
        public SelectionChangedEventArgs(List<IShape> shapes, bool isSelected)
        {
            Shapes = shapes;
            IsSelected = isSelected;
        }
    }
}