using System.Collections;
using System.Drawing;

namespace DrawEverything.Command
{
    /// <summary>
    /// 添加
    /// </summary>
    public class AddCommand : ICommand
    {
        #region Fields

        private readonly ArrayList _graphicListMoved;
        private DrawBase item;

        #endregion Fields

        #region Constructors

        public AddCommand(ArrayList itemsMoved, DrawBase itemP)
        {
            _graphicListMoved = itemsMoved;
            item = itemP;
        }

        //Disable default constructor
        private AddCommand()
        {

        }

        #endregion Constructors

        #region Methods

        public void Execute()
        {
            _graphicListMoved.Insert(0, item);
        }

        public void UnExecute()
        {
            _graphicListMoved.Remove(item);
        }

        #endregion Methods

    }
}