// Presentation/Views/PropertyPanel.cs
using System;
using System.Text;
using CADFramework.Bridge;
using CADFramework.GeometryKernel.Primitives;

namespace CADFramework.Presentation.Views
{
    /// <summary>
    /// 属性面板 - 显示选中对象的属性
    /// 完全独立于几何内核
    /// </summary>
    public class PropertyPanel
    {
        private SelectionManager _selectionManager;
        
        public PropertyPanel(SelectionManager selectionManager)
        {
            _selectionManager = selectionManager;
            _selectionManager.SelectionChanged += OnSelectionChanged;
        }
        
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayProperties();
        }
        
        public void DisplayProperties()
        {
            Console.WriteLine("\n=== 属性面板 ===");
            
            if (_selectionManager.SelectedShapes.Count == 0)
            {
                Console.WriteLine("未选中任何对象");
                return;
            }
            
            foreach (var shape in _selectionManager.SelectedShapes)
            {
                Console.WriteLine($"对象: {shape.Name} (ID: {shape.Id})");
                Console.WriteLine($"类型: {shape.GetType().Name}");
                
                if (shape is Line line)
                {
                    Console.WriteLine($"起点: {line.StartPoint}");
                    Console.WriteLine($"终点: {line.EndPoint}");
                    Console.WriteLine($"长度: {line.StartPoint.DistanceTo(line.EndPoint):F2}");
                }
                else if (shape is Rectangle rect)
                {
                    Console.WriteLine($"中心: {rect.Center}");
                    Console.WriteLine($"宽度: {rect.Width}");
                    Console.WriteLine($"高度: {rect.Height}");
                    Console.WriteLine($"面积: {rect.Width * rect.Height:F2}");
                }
                else if (shape is ExtrudedBody body)
                {
                    Console.WriteLine($"高度: {body.Height}");
                    Console.WriteLine($"体积: {GetVolume(body):F2}");
                }
                
                var bbox = shape.GetBoundingBox();
                Console.WriteLine($"边界框: [{bbox.MinX:F2}, {bbox.MinY:F2}, {bbox.MinZ:F2}] -> [{bbox.MaxX:F2}, {bbox.MaxY:F2}, {bbox.MaxZ:F2}]");
                Console.WriteLine("---");
            }
            
            Console.WriteLine("================\n");
        }
        
        private double GetVolume(ExtrudedBody body)
        {
            if (body.BaseShape is Rectangle rect)
            {
                return rect.Width * rect.Height * body.Height;
            }
            return 0;
        }
    }
}