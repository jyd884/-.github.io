// Demo/Program.cs
using System;
using System.Collections.Generic;
using CADFramework.Bridge;
using CADFramework.GeometryKernel.Events;
using CADFramework.GeometryKernel.Operations;
using CADFramework.GeometryKernel.Primitives;
using CADFramework.Presentation.Renderers;
using CADFramework.Presentation.Views;

namespace CADFramework.Demo
{
    class Program
    {
        private static List<IShape> _globalShapeList = new List<IShape>();
        private static GeometryObserver _observer;
        private static SelectionManager _selectionManager;
        private static CommandProcessor _commandProcessor;
        private static CADView _view;
        private static PropertyPanel _propertyPanel;
        
        static void Main(string[] args)
        {
            Console.WriteLine("=== CAD框架演示 - 单一职责原则解耦几何内核与UI ===\n");
            
            // 1. 初始化各个组件（依赖注入）
            InitializeComponents();
            
            // 2. 演示创建几何对象
            DemoCreateShapes();
            
            // 3. 演示选择功能
            DemoSelection();
            
            // 4. 演示移动操作（支持撤销/重做）
            DemoMoveOperations();
            
            // 5. 演示拉伸操作
            DemoExtrudeOperation();
            
            // 6. 显示架构说明
            ShowArchitectureExplanation();
            
            Console.WriteLine("\n=== 演示完成 ===");
        }
        
        static void InitializeComponents()
        {
            // 创建渲染器（可以轻松替换为DirectX、GDI+等）
            IRenderer renderer = new OpenGLRenderer();
            
            // 创建观察者（桥接层）
            _observer = new GeometryObserver();
            
            // 创建选择管理器
            _selectionManager = new SelectionManager();
            
            // 创建命令处理器
            _commandProcessor = new CommandProcessor();
            
            // 创建视图（UI层）
            _view = new CADView(renderer, _observer, _selectionManager, _commandProcessor);
            _view.Initialize(800, 600);
            
            // 创建属性面板
            _propertyPanel = new PropertyPanel(_selectionManager);
        }
        
        static void DemoCreateShapes()
        {
            Console.WriteLine("\n【演示1：创建几何对象】");
            
            // 创建线段
            var line = new Line(1, "BaseLine", 
                new Point3D(0, 0, 0), 
                new Point3D(100, 0, 0));
            
            // 创建矩形
            var rect = new Rectangle(2, "BaseRect", 
                new Point3D(50, 50, 0), 
                width: 100, height: 80);
            
            // 使用命令创建对象（支持撤销）
            var createLineCmd = new CreateShapeCommand(_globalShapeList, line);
            var createRectCmd = new CreateShapeCommand(_globalShapeList, rect);
            
            _commandProcessor.ExecuteCommand(createLineCmd);
            _commandProcessor.ExecuteCommand(createRectCmd);
            
            // 发布事件
            GeometryEventPublisher.Instance.Publish(
                new GeometryEventArgs(GeometryEventType.ShapeAdded, line.Id, line.Name, line));
            GeometryEventPublisher.Instance.Publish(
                new GeometryEventArgs(GeometryEventType.ShapeAdded, rect.Id, rect.Name, rect));
            
            // 刷新视图
            _view.Refresh();
        }
        
        static void DemoSelection()
        {
            Console.WriteLine("\n【演示2：选择对象】");
            
            // 模拟鼠标点击选择
            var rect = _globalShapeList.Find(s => s.Name == "BaseRect");
            if (rect != null)
            {
                var clickPoint = new Point3D(50, 50, 0);
                _view.OnMouseClick(clickPoint);
            }
        }
        
        static void DemoMoveOperations()
        {
            Console.WriteLine("\n【演示3：移动操作和撤销/重做】");
            
            var rect = _globalShapeList.Find(s => s.Name == "BaseRect");
            if (rect != null)
            {
                // 移动矩形
                var startPoint = new Point3D(50, 50, 0);
                var endPoint = new Point3D(80, 80, 0);
                _view.OnDragMove(startPoint, endPoint);
                
                // 演示撤销
                Console.WriteLine("\n执行撤销...");
                _commandProcessor.Undo();
                _view.Refresh();
                
                // 演示重做
                Console.WriteLine("\n执行重做...");
                _commandProcessor.Redo();
                _view.Refresh();
            }
        }
        
        static void DemoExtrudeOperation()
        {
            Console.WriteLine("\n【演示4：拉伸操作 - 将2D形状转换为3D实体】");
            
            var rect = _globalShapeList.Find(s => s.Name == "BaseRect") as Rectangle;
            if (rect != null)
            {
                // 纯几何操作，不涉及UI
                var extrudedBody = ExtrudeOp.ExtrudeRectangle(rect, height: 50);
                extrudedBody.Name = "ExtrudedBody";
                extrudedBody.Id = 3;
                
                var createBodyCmd = new CreateShapeCommand(_globalShapeList, extrudedBody);
                _commandProcessor.ExecuteCommand(createBodyCmd);
                
                GeometryEventPublisher.Instance.Publish(
                    new GeometryEventArgs(GeometryEventType.ShapeAdded, 
                                         extrudedBody.Id, 
                                         extrudedBody.Name, 
                                         extrudedBody));
                
                _view.Refresh();
                
                // 选择拉伸体查看属性
                var clickPoint = new Point3D(50, 50, 25);
                _view.OnMouseClick(clickPoint);
            }
        }
        
        static void ShowArchitectureExplanation()
        {
            Console.WriteLine("\n【架构说明】");
            Console.WriteLine("=".PadRight(60, '='));
            Console.WriteLine("1. 几何内核（GeometryKernel）:");
            Console.WriteLine("   - 完全不依赖UI");
            Console.WriteLine("   - 只负责几何定义和计算");
            Console.WriteLine("   - 通过事件发布器通知变化");
            Console.WriteLine();
            Console.WriteLine("2. 桥接层（Bridge）:");
            Console.WriteLine("   - GeometryObserver: 监听几何事件");
            Console.WriteLine("   - SelectionManager: 管理选择状态");
            Console.WriteLine("   - CommandProcessor: 处理命令（支持撤销/重做）");
            Console.WriteLine();
            Console.WriteLine("3. 表现层（Presentation）:");
            Console.WriteLine("   - IRenderer: 渲染器接口，可替换实现");
            Console.WriteLine("   - CADView: 视图层，只负责显示和交互");
            Console.WriteLine("   - PropertyPanel: 属性面板");
            Console.WriteLine();
            Console.WriteLine("4. 单一职责原则体现:");
            Console.WriteLine("   ✓ 几何内核不关心如何显示");
            Console.WriteLine("   ✓ UI不包含几何计算逻辑");
            Console.WriteLine("   ✓ 桥接层处理两者通信");
            Console.WriteLine("   ✓ 可以轻松替换渲染器（OpenGL→DirectX）");
            Console.WriteLine("   ✓ 可以轻松添加新视图（属性面板、树形视图等）");
            Console.WriteLine("=".PadRight(60, '='));
        }
    }
}