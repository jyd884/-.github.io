// GeometryKernel/Events/GeometryEvent.cs
using System;

namespace CADFramework.GeometryKernel.Events
{
    /// <summary>
    /// 几何事件类型
    /// </summary>
    public enum GeometryEventType
    {
        ShapeAdded,      // 形状添加
        ShapeRemoved,    // 形状删除
        ShapeModified,   // 形状修改
        SelectionChanged // 选择改变
    }
    
    /// <summary>
    /// 几何事件参数
    /// 几何内核只产生事件，不关心谁监听
    /// </summary>
    public class GeometryEventArgs : EventArgs
    {
        public GeometryEventType EventType { get; set; }
        public int ShapeId { get; set; }
        public string ShapeName { get; set; }
        public object ShapeData { get; set; }
        public DateTime Timestamp { get; set; }
        
        public GeometryEventArgs(GeometryEventType type, int shapeId, string shapeName, object data = null)
        {
            EventType = type;
            ShapeId = shapeId;
            ShapeName = shapeName;
            ShapeData = data;
            Timestamp = DateTime.Now;
        }
        
        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss}] {EventType}: {ShapeName} (ID:{ShapeId})";
        }
    }
    
    /// <summary>
    /// 几何事件发布器
    /// 使用观察者模式，解耦几何内核和UI
    /// </summary>
    public class GeometryEventPublisher
    {
        private static GeometryEventPublisher _instance;
        public static GeometryEventPublisher Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GeometryEventPublisher();
                return _instance;
            }
        }
        
        // 事件定义 - UI层可以订阅
        public event EventHandler<GeometryEventArgs> GeometryChanged;
        
        public void Publish(GeometryEventArgs args)
        {
            GeometryChanged?.Invoke(this, args);
        }
    }
}