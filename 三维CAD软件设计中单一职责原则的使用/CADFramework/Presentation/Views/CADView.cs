// Presentation/Views/CADView.cs
using System;
using System.Collections.Generic;
using CADFramework.Bridge;
using CADFramework.GeometryKernel.Primitives;
using CADFramework.Presentation.Renderers;

namespace CADFramework.Presentation.Views
{
    /// <summary>
    /// CAD视图窗口
    /// 负责显示几何对象，但不包含几何计算逻辑
    /// </summary>
    public class CADView
    {
        private IRenderer _renderer;
        private GeometryObserver _geometryObserver;
        private SelectionManager _selectionManager;
        private CommandProcessor _commandProcessor;
        
        private List<IShape> _displayShapes;
        private List<int> _selectedIds;
        
        public CADView(IRenderer renderer, GeometryObserver observer, 
                       SelectionManager selectionManager, CommandProcessor commandProcessor)
        {
            _renderer = renderer;
            _geometryObserver = observer;
            _selectionManager = selectionManager;
            _commandProcessor = commandProcessor;
            
            _displayShapes = new List<IShape>();
            _selectedIds = new List<int>();
            
            // 订阅事件
            _selectionManager.SelectionChanged += OnSelectionChanged;
            _commandProcessor.CommandExecuted += OnCommandExecuted;
        }
        
        public void Initialize(int width, int height)
        {
            _renderer.Initialize(width, height);
            Refresh();
        }
        
        /// <summary>
        /// 刷新视图
        /// </summary>
        public void Refresh()
        {
            // 从观察者获取最新数据
            _displayShapes = _geometryObserver.GetAllShapes();
            
            // 更新选中状态
            _selectedIds.Clear();
            foreach (var shape in _selectionManager.SelectedShapes)
            {
                _selectedIds.Add(shape.Id);
            }
            
            // 渲染
            _renderer.RenderAllShapes(_displayShapes, _selectedIds);
        }
        
        /// <summary>
        /// 处理鼠标点击（UI事件）
        /// </summary>
        public void OnMouseClick(Point3D worldPosition)
        {
            Console.WriteLine($"[View] 鼠标点击位置: {worldPosition}");
            
            // 拾取形状
            var pickedShape = _selectionManager.PickShapeAtPoint(worldPosition, _displayShapes);
            
            if (pickedShape != null)
            {
                _selectionManager.SelectShape(pickedShape);
                Console.WriteLine($"[View] 选中形状: {pickedShape.Name}");
            }
            else
            {
                _selectionManager.ClearSelection();
                Console.WriteLine("[View] 清除选择");
            }
            
            Refresh();
        }
        
        /// <summary>
        /// 处理拖拽移动
        /// </summary>
        public void OnDragMove(Point3D startWorld, Point3D endWorld)
        {
            if (_selectionManager.SelectedShapes.Count > 0)
            {
                var delta = new Point3D(
                    endWorld.X - startWorld.X,
                    endWorld.Y - startWorld.Y,
                    endWorld.Z - startWorld.Z
                );
                
                var command = new MoveShapeCommand(_selectionManager.SelectedShapes[0], delta);
                _commandProcessor.ExecuteCommand(command);
                Refresh();
            }
        }
        
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine($"[View] 选择改变: 选中了 {e.Shapes.Count} 个形状");
            Refresh();
        }
        
        private void OnCommandExecuted(object sender, CommandEventArgs e)
        {
            Console.WriteLine($"[View] 命令执行: {e.Command.Description} ({(e.IsExecute ? "执行" : "撤销")})");
            Refresh();
        }
    }
}