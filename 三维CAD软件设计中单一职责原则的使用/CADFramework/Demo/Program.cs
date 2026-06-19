// Demo/Program.cs
using System;
using System.Collections.Generic;
using CADFramework.Bridge;
using CADFramework.GeometryKernel.Events;
using CADFramework.GeometryKernel.Operations;
using CADFramework.GeometryKernel.Primitives;
using CADFramework.GeometryKernel.Topology;
using CADFramework.Presentation.Controllers;
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

            InitializeComponents();
            DemoCreateShapes();
            DemoSelection();
            DemoMoveOperations();
            DemoExtrudeOperation();
            DemoTopology();
            DemoBooleanOperations();
            DemoTransformations();
            DemoControllers();
            ShowArchitectureExplanation();

            Console.WriteLine("\n=== 演示完成 ===");
        }

        static void InitializeComponents()
        {
            IRenderer renderer = new OpenGLRenderer();

            _observer = new GeometryObserver();
            _selectionManager = new SelectionManager();
            _commandProcessor = new CommandProcessor();

            _view = new CADView(renderer, _observer, _selectionManager, _commandProcessor);
            _view.Initialize(800, 600);

            _propertyPanel = new PropertyPanel(_selectionManager);
        }

        static void DemoCreateShapes()
        {
            Console.WriteLine("\n【演示1：创建几何对象】");

            var line = new Line(1, "BaseLine",
                new Point3D(0, 0, 0),
                new Point3D(100, 0, 0));

            var rect = new Rectangle(2, "BaseRect",
                new Point3D(50, 50, 0),
                width: 100, height: 80);

            var createLineCmd = new CreateShapeCommand(_globalShapeList, line);
            var createRectCmd = new CreateShapeCommand(_globalShapeList, rect);

            _commandProcessor.ExecuteCommand(createLineCmd);
            _commandProcessor.ExecuteCommand(createRectCmd);

            GeometryEventPublisher.Instance.Publish(
                new GeometryEventArgs(GeometryEventType.ShapeAdded, line.Id, line.Name, line));
            GeometryEventPublisher.Instance.Publish(
                new GeometryEventArgs(GeometryEventType.ShapeAdded, rect.Id, rect.Name, rect));

            _view.Refresh();
        }

        static void DemoSelection()
        {
            Console.WriteLine("\n【演示2：选择对象】");

            if (_globalShapeList.Find(s => s.Name == "BaseRect") != null)
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
                var startPoint = new Point3D(50, 50, 0);
                var endPoint = new Point3D(80, 80, 0);
                _view.OnDragMove(startPoint, endPoint);

                Console.WriteLine("\n执行撤销...");
                _commandProcessor.Undo();
                _view.Refresh();

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

                var clickPoint = new Point3D(50, 50, 25);
                _view.OnMouseClick(clickPoint);
            }
        }

        static void DemoTopology()
        {
            Console.WriteLine("\n【演示5：拓扑结构】");

            var body = CreateBox(10, "Cube", new Point3D(0, 0, 0), 100, 100, 100);

            Console.WriteLine($"创建拓扑体: {body}");
            Console.WriteLine($"  面数: {body.Faces.Count}");
            Console.WriteLine($"  顶点数: {body.Vertices.Count}");
            Console.WriteLine($"  封闭性: {body.IsClosed()}");
            Console.WriteLine($"  体积: {body.ComputeVolume():F2}");
        }

        static void DemoBooleanOperations()
        {
            Console.WriteLine("\n【演示6：布尔运算】");

            var body1 = CreateBox(20, "Box1", new Point3D(0, 0, 0), 100, 100, 100);
            var body2 = CreateBox(21, "Box2", new Point3D(50, 50, 50), 100, 100, 100);

            var union = BooleanOp.Execute(body1, body2, BooleanOperationType.Union);
            Console.WriteLine($"并集结果: 成功={union.Success}, 体积={union.ResultBody?.Volume:F2}");

            var intersect = BooleanOp.Execute(body1, body2, BooleanOperationType.Intersect);
            Console.WriteLine($"交集结果: 成功={intersect.Success}, 体积={intersect.ResultBody?.Volume:F2}");

            var subtract = BooleanOp.Execute(body1, body2, BooleanOperationType.Subtract);
            Console.WriteLine($"差集结果: 成功={subtract.Success}, 体积={subtract.ResultBody?.Volume:F2}");
        }

        static Body CreateBox(int id, string name, Point3D origin, double width, double height, double depth)
        {
            var body = new Body(id, name);

            var vertices = new[]
            {
                new Vertex(id * 100 + 1, $"{name}_V1", origin),
                new Vertex(id * 100 + 2, $"{name}_V2", new Point3D(origin.X + width, origin.Y, origin.Z)),
                new Vertex(id * 100 + 3, $"{name}_V3", new Point3D(origin.X + width, origin.Y + height, origin.Z)),
                new Vertex(id * 100 + 4, $"{name}_V4", new Point3D(origin.X, origin.Y + height, origin.Z)),
                new Vertex(id * 100 + 5, $"{name}_V5", new Point3D(origin.X, origin.Y, origin.Z + depth)),
                new Vertex(id * 100 + 6, $"{name}_V6", new Point3D(origin.X + width, origin.Y, origin.Z + depth)),
                new Vertex(id * 100 + 7, $"{name}_V7", new Point3D(origin.X + width, origin.Y + height, origin.Z + depth)),
                new Vertex(id * 100 + 8, $"{name}_V8", new Point3D(origin.X, origin.Y + height, origin.Z + depth))
            };

            var edges = new[]
            {
                new Edge(id * 100 + 11, $"{name}_E1", vertices[0], vertices[1]),
                new Edge(id * 100 + 12, $"{name}_E2", vertices[1], vertices[2]),
                new Edge(id * 100 + 13, $"{name}_E3", vertices[2], vertices[3]),
                new Edge(id * 100 + 14, $"{name}_E4", vertices[3], vertices[0]),
                new Edge(id * 100 + 15, $"{name}_E5", vertices[4], vertices[5]),
                new Edge(id * 100 + 16, $"{name}_E6", vertices[5], vertices[6]),
                new Edge(id * 100 + 17, $"{name}_E7", vertices[6], vertices[7]),
                new Edge(id * 100 + 18, $"{name}_E8", vertices[7], vertices[4]),
                new Edge(id * 100 + 19, $"{name}_E9", vertices[0], vertices[4]),
                new Edge(id * 100 + 20, $"{name}_E10", vertices[1], vertices[5]),
                new Edge(id * 100 + 21, $"{name}_E11", vertices[2], vertices[6]),
                new Edge(id * 100 + 22, $"{name}_E12", vertices[3], vertices[7])
            };

            var faces = new[]
            {
                new Face(id * 100 + 31, $"{name}_Bottom", new List<Edge> { edges[0], edges[1], edges[2], edges[3] }),
                new Face(id * 100 + 32, $"{name}_Top", new List<Edge> { edges[4], edges[5], edges[6], edges[7] }),
                new Face(id * 100 + 33, $"{name}_Front", new List<Edge> { edges[0], edges[9], edges[4], edges[8] }),
                new Face(id * 100 + 34, $"{name}_Right", new List<Edge> { edges[1], edges[10], edges[5], edges[9] }),
                new Face(id * 100 + 35, $"{name}_Back", new List<Edge> { edges[2], edges[11], edges[6], edges[10] }),
                new Face(id * 100 + 36, $"{name}_Left", new List<Edge> { edges[3], edges[8], edges[7], edges[11] })
            };

            foreach (var face in faces)
            {
                body.AddFace(face);
            }

            body.Volume = width * height * depth;
            body.Centroid = body.ComputeCentroid();
            return body;
        }

        static void DemoTransformations()
        {
            Console.WriteLine("\n【演示7：变换操作】");

            var rect = new Rectangle(30, "TestRect", new Point3D(0, 0, 0), 100, 80);
            Console.WriteLine($"原始矩形: {rect}");

            TransformOp.Translate(rect, 50, 50, 0);
            Console.WriteLine($"平移后中心: {rect.Center}");

            TransformOp.RotateZ(rect, 45);
            Console.WriteLine($"旋转后中心: {rect.Center}");

            TransformOp.ScaleUniform(rect, 1.5);
            Console.WriteLine($"缩放后中心: {rect.Center}");

            TransformOp.Mirror(rect, MirrorPlane.XY);
            Console.WriteLine($"镜像后中心: {rect.Center}");
        }

        static void DemoControllers()
        {
            Console.WriteLine("\n【演示8：控制器系统】");

            var mouseController = new MouseController(_selectionManager, _commandProcessor);
            var keyboardController = new KeyboardController(_commandProcessor);
            var drawController = new DrawController(_globalShapeList, _commandProcessor);

            mouseController.MouseClicked += (_, position) => _view.OnMouseClick(position);
            mouseController.MouseDragged += (_, args) => _view.OnDragMove(args.StartPosition, args.EndPosition);

            mouseController.SetMode(ControllerType.Selection);
            mouseController.HandleMouseClick(new Point3D(80, 80, 0), MouseButton.Left);

            keyboardController.HandleKeyPress('z');
            keyboardController.HandleKeyPress('y');
            keyboardController.HandleKeyPress('h');

            drawController.SetDrawType(ControllerType.DrawRect);
            drawController.HandleMouseClick(new Point3D(200, 200, 0), MouseButton.Left);
            drawController.HandleMouseClick(new Point3D(300, 300, 0), MouseButton.Left);

            _view.Refresh();
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
            Console.WriteLine("   - Controllers: 负责鼠标/键盘/绘图输入");
            Console.WriteLine();
            Console.WriteLine("4. 单一职责原则体现:");
            Console.WriteLine("   ✓ 几何内核不关心如何显示");
            Console.WriteLine("   ✓ UI不包含几何计算逻辑");
            Console.WriteLine("   ✓ 桥接层处理两者通信");
            Console.WriteLine("   ✓ 命令系统独立处理撤销/重做");
            Console.WriteLine("   ✓ 可以轻松替换渲染器（OpenGL→Wireframe）");
            Console.WriteLine("=".PadRight(60, '='));
        }
    }
}