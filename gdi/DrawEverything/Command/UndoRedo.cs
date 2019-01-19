using System.Collections.Generic;

namespace DrawEverything.Command
{
    /// <summary>
    /// 撤销重做
    /// </summary>
    public class UndoRedo
    {
        /// <summary>
        /// 堆栈
        /// </summary>
        private readonly Stack<ICommand> _undoCommands = new Stack<ICommand>();
        private readonly Stack<ICommand> _redoCommands = new Stack<ICommand>();

        /// <summary>
        /// 撤销
        /// </summary>
        public void Undo()
        {
            if (_undoCommands.Count > 0)
            {
                //出栈
                ICommand command = _undoCommands.Pop();
                command.UnExecute();
                //进栈
                _redoCommands.Push(command);
            }
        }

        /// <summary>
        ///再做
        /// </summary>
        public void Redo()
        {
            if (_redoCommands.Count > 0)
            {
                ICommand command = _redoCommands.Pop();
                command.Execute();
                _undoCommands.Push(command);
            }
        }
        /// <summary>
        /// 命令添加栈
        /// </summary>
        /// <param name="cmd"></param>
        public void AddCommand(ICommand cmd)
        {
            _undoCommands.Push(cmd);
        }
    }
}