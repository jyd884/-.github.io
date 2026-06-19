// Presentation/Controllers/KeyboardController.cs
using System;
using System.Collections.Generic;

namespace CADFramework.Presentation.Controllers
{
    /// <summary>
    /// 键盘控制器 - 处理键盘快捷键
    /// </summary>
    public class KeyboardController : IController
    {
        private CommandProcessor _commandProcessor;
        private Dictionary<char, Action> _shortcuts;
        
        public event EventHandler<char> KeyPressed;
        
        public KeyboardController(CommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
            InitializeShortcuts();
        }
        
        private void InitializeShortcuts()
        {
            _shortcuts = new Dictionary<char, Action>
            {
                { 'z', () => _commandProcessor.Undo() },
                { 'Z', () => _commandProcessor.Undo() },
                { 'y', () => _commandProcessor.Redo() },
                { 'Y', () => _commandProcessor.Redo() },
                { 'd', () => Console.WriteLine("[Keyboard] 删除选中对象") },
                { 'D', () => Console.WriteLine("[Keyboard] 删除选中对象") },
                { 'g', () => Console.WriteLine("[Keyboard] 分组选中对象") },
                { 'h', () => ShowHelp() }
            };
        }
        
        public void HandleMouseClick(Point3D position, MouseButton button)
        {
            // 键盘控制器不处理鼠标点击
        }
        
        public void HandleMouseMove(Point3D position)
        {
            // 键盘控制器不处理鼠标移动
        }
        
        public void HandleMouseDrag(Point3D start, Point3D end, MouseButton button)
        {
            // 键盘控制器不处理鼠标拖拽
        }
        
        public void HandleKeyPress(char key)
        {
            Console.WriteLine($"[KeyboardController] 按键: {key}");
            
            if (_shortcuts.ContainsKey(key))
            {
                _shortcuts[key]();
            }
            
            KeyPressed?.Invoke(this, key);
        }
        
        public string GetControllerName() => "KeyboardController";
        
        private void ShowHelp()
        {
            Console.WriteLine("\n=== 快捷键帮助 ===");
            Console.WriteLine("Z: 撤销");
            Console.WriteLine("Y: 重做");
            Console.WriteLine("D: 删除");
            Console.WriteLine("G: 分组");
            Console.WriteLine("S: 选择模式");
            Console.WriteLine("L: 画线模式");
            Console.WriteLine("R: 画矩形模式");
            Console.WriteLine("C: 画圆模式");
            Console.WriteLine("ESC: 取消操作");
            Console.WriteLine("H: 显示帮助");
            Console.WriteLine("================\n");
        }
    }
}