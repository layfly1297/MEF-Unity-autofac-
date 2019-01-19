using System.Collections;
using System.Drawing;

namespace DrawEverything.Command
{
    /// <summary>
    /// 移动命令
    /// </summary>
    public class MoveCommand : ICommand
    {
        #region Fields

        private readonly ArrayList _graphicListMoved;
        private PointF _deltaMoved;

        #endregion Fields

        #region Constructors

        public MoveCommand(ArrayList itemsMoved, PointF delta)
        {
            _graphicListMoved = new ArrayList();
            _deltaMoved = new PointF();

            _graphicListMoved.AddRange(itemsMoved);
            _deltaMoved = delta;
        }

        //Disable default constructor
        private MoveCommand()
        {

        }

        #endregion Constructors

        #region Methods

        public void Execute()
        {
            for (int i = 0; i < _graphicListMoved.Count; i++)
            {
                ((DrawBase)_graphicListMoved[i]).Move(_deltaMoved.X, _deltaMoved.Y);
            }
        }

        public void UnExecute()
        {
            for (int i = 0; i < _graphicListMoved.Count; i++)
            {
                ((DrawBase)_graphicListMoved[i]).Move(-1 * _deltaMoved.X, -1 * _deltaMoved.Y);
            }
        }

        #endregion Methods

    }
}