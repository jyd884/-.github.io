// Bridge/GeometryObserver.cs
using System;
using System.Collections.Generic;
using CADFramework.GeometryKernel.Events;
using CADFramework.GeometryKernel.Primitives;

namespace CADFramework.Bridge
{
    /// <summary>
    /// 几何观察者 - 监听几何变化并转发给UI
    /// 这是一个中间层，确保几何内核不知道UI的存在
    /// </summary>
    public class GeometryObserver
    {
        private List<IShape> _shapes;
        
        public GeometryObserver()
        {
            _shapes = new List<IShape>();
            
            // 订阅几何事件
            GeometryEventPublisher.Instance.GeometryChanged += OnGeometryChanged;
        }
        
        private void OnGeometryChanged(object sender, GeometryEventArgs e)
        {
            Console.WriteLine($"[Bridge] 接收到几何事件: {e}");
            
            // 根据事件类型更新内部数据
            switch (e.EventType)
            {
                case GeometryEventType.ShapeAdded:
                    if (e.ShapeData is IShape shape)
                        _shapes.Add(shape);
                    break;
                    
                case GeometryEventType.ShapeRemoved:
                    _shapes.RemoveAll(s => s.Id == e.ShapeId);
                    break;
                    
                case GeometryEventType.ShapeModified:
                    // 更新已存在的形状
                    var index = _shapes.FindIndex(s => s.Id == e.ShapeId);
                    if (index >= 0 && e.ShapeData is IShape newShape)
                        _shapes[index] = newShape;
                    break;
            }
        }
        
        public List<IShape> GetAllShapes()
        {
            return new List<IShape>(_shapes);
        }
        
        public IShape FindShapeById(int id)
        {
            return _shapes.Find(s => s.Id == id);
        }
    }
}