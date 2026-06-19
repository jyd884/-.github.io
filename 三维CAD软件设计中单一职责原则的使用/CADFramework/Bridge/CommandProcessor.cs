// Bridge/CommandProcessor.cs
using System;
using System.Collections.Generic;
using CADFramework.GeometryKernel.Operations;
using CADFramework.GeometryKernel.Primitives;

namespace CADFramework.Bridge
{
    /// <summary>
    /// 命令处理器 - 处理用户操作并转换为几何操作
    /// 支持撤销/重做
    /// </summary>
    public class CommandProcessor
    {
        private Stack<ICommand> _undoStack = new Stack<ICommand>();
        private Stack<ICommand> _redoStack = new Stack<ICommand>();
        
        public event EventHandler<CommandEventArgs> CommandExecuted;
        
        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear();
            
            CommandExecuted?.Invoke(this, new CommandEventArgs(command, true));
        }
        
        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                var command = _undoStack.Pop();
                command.Undo();
                _redoStack.Push(command);
                
                CommandExecuted?.Invoke(this, new CommandEventArgs(command, false));
            }
        }
        
        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                var command = _redoStack.Pop();
                command.Execute();
                _undoStack.Push(command);
                
                CommandExecuted?.Invoke(this, new CommandEventArgs(command, true));
            }
        }
    }
    
    /// <summary>
    /// 命令接口
    /// </summary>
    public interface ICommand
    {
        void Execute();
        void Undo();
        string Description { get; }
    }
    
    /// <summary>
    /// 创建形状命令
    /// </summary>
    public class CreateShapeCommand : ICommand
    {
        private List<IShape> _shapeList;
        private IShape _shape;
        
        public string Description => $"Create {_shape.Name}";
        
        public CreateShapeCommand(List<IShape> shapeList, IShape shape)
        {
            _shapeList = shapeList;
            _shape = shape;
        }
        
        public void Execute()
        {
            _shapeList.Add(_shape);
        }
        
        public void Undo()
        {
            _shapeList.Remove(_shape);
        }
    }
    
    /// <summary>
    /// 移动形状命令
    /// </summary>
    public class MoveShapeCommand : ICommand
    {
        private IShape _shape;
        private TransformMatrix _oldMatrix;
        private TransformMatrix _newMatrix;
        private Point3D _oldPosition;
        private Point3D _newPosition;
        
        public string Description => $"Move {_shape.Name}";
        
        public MoveShapeCommand(IShape shape, Point3D delta)
        {
            _shape = shape;
            _newMatrix = TransformMatrix.Translation(delta.X, delta.Y, delta.Z);
            
            // 保存旧位置（简化处理）
            if (shape is Rectangle rect)
                _oldPosition = rect.Center;
        }
        
        public void Execute()
        {
            _shape.Transform(_newMatrix);
        }
        
        public void Undo()
        {
            var inverse = TransformMatrix.Translation(-_newMatrix.Matrix[0, 3], 
                                                       -_newMatrix.Matrix[1, 3], 
                                                       -_newMatrix.Matrix[2, 3]);
            _shape.Transform(inverse);
        }
    }
    
    public class CommandEventArgs : EventArgs
    {
        public ICommand Command { get; set; }
        public bool IsExecute { get; set; }
        
        public CommandEventArgs(ICommand command, bool isExecute)
        {
            Command = command;
            IsExecute = isExecute;
        }
    }
}