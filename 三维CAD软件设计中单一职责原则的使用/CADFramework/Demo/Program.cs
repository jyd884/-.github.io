// Demo/Program.cs (更新部分)
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
        // ... 前面的代码保持不变 ...
        
        static void DemoTopology()
        {
            Console.WriteLine("\n【演示5：拓扑结构】");
            
            // 创建一个简单的立方体拓扑
            var v1 = new Vertex(1, "V1", new Point3D(0, 0, 0));
            var v2 = new Vertex(2, "V2", new Point3D(100, 0, 0));
            var v3 = new Vertex(3, "V3", new Point3D(100, 100, 0));
            var v4 = new Vertex(4, "V4", new Point3D(0, 100, 0));
            
            var e1 = new Edge(1, "E1", v1, v2);
            var e2 = new Edge(2, "E2", v2, v3);
            var e3 = new Edge(3, "E3", v3, v4);
            var e4 = new Edge(4, "E4", v4, v1);
            
            var face = new Face(1, "BottomFace", new List<Edge> { e1, e2, e3, e4 });
            var body = new Body(1, "Cube");
            body.AddFace(face);
            
            Console.WriteLine($"创建拓扑体: {body}");
            Console.WriteLine($"  封闭性: {body.IsClosed()}");
            Console.WriteLine($"  体积: {body.ComputeVolume():F2}");
        }
        
        static void DemoBooleanOperations()
        {
            Console.WriteLine("\n【演示6：布尔运算】");
            
            // 创建两个立方体
            var body1 = CreateBox(1, "Box1", new Point3D(0, 0, 0), 100, 100, 100);
            var body2 = CreateBox(2, "Box2", new Point3D(50, 50, 50), 100, 100, 100);
            
            // 并集
            var union = BooleanOp.Execute(body1, body2, BooleanOperationType.Union);
            Console.WriteLine($"并集结果: 成功={union.Success}, 体积={union.ResultBody?.Volume:F2}");
            
            // 交集
            var intersect = BooleanOp.Execute(body1, body2, BooleanOperationType.Intersect);
            Console.WriteLine($"交集结果: 成功={intersect.Success}, 体积={intersect.ResultBody?.Volume:F2}");
            
            // 差集
            var subtract = BooleanOp.Execute(body1, body2, BooleanOperationType.Subtract);
            Console.WriteLine($"差集结果: 成功={subtract.Success}, 体积={subtract.ResultBody?.Volume:F2}");
        }
        
        static Body CreateBox(int id, string name, Point3D origin, double width, double height, double depth)
        {
            var body = new Body(id, name);
            // 简化：创建6个面的立方体（省略详细实现）
            body.Volume = width * height * depth;
            return body;
        }
        
        static void DemoTransformations()
        {
            Console.WriteLine("\n【演示7：变换操作】");
            
            var rect = new Rectangle(10, "TestRect", new Point3D(0, 0, 0), 100, 80);
            Console.WriteLine($"原始矩形: {rect}");
            
            // 平移
            TransformOp.Translate(rect, 50, 50, 0);
            Console.WriteLine($"平移后: {rect.Center}");
            
            // 旋转
            TransformOp.RotateZ(rect, 45);
            
            // 缩放
            TransformOp.ScaleUniform(rect, 1.5);
            
            // 镜像
            TransformOp.Mirror(rect, MirrorPlane.XY);
        }
        
        static void DemoControllers()
        {
            Console.WriteLine("\n【演示8：控制器系统】");
            
            var mouseController = new MouseController(_selectionManager, _commandProcessor);
            var keyboardController = new KeyboardController(_commandProcessor);
            var drawController = new DrawController(_globalShapeList, _commandProcessor);
            
            // 设置模式
            mouseController.SetMode(ControllerType.Selection);
            drawController.SetDrawType(ControllerType.DrawRect);
            
            // 模拟用户操作
            mouseController.HandleMouseClick(new Point3D(100, 100, 0), MouseButton.Left);
            keyboardController.HandleKeyPress('z');  // 撤销
            keyboardController.HandleKeyPress('h');  // 帮助
            drawController.HandleMouseClick(new Point3D(200, 200, 0), MouseButton.Left);
            drawController.HandleMouseClick(new Point3D(300, 300, 0), MouseButton.Left);
        }
        
        // 更新Main方法
        static void Main(string[] args)
        {
            Console.WriteLine("=== CAD框架演示 - 完整版 ===\n");
            
            InitializeComponents();
            DemoCreateShapes();
            DemoSelection();
            DemoMoveOperations();
            DemoExtrudeOperation();
            DemoTopology();           // 新增
            DemoBooleanOperations();   // 新增
            DemoTransformations();     // 新增
            DemoControllers();         // 新增
            ShowArchitectureExplanation();
            
            Console.WriteLine("\n=== 演示完成 ===");
        }
    }
}