namespace DrawEverything.Command
{
    /// <summary>
    /// 命令接口
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// 执行
        /// </summary>
        void Execute();

        /// <summary>
        /// 撤回执行
        /// </summary>
        void UnExecute();
    }
}