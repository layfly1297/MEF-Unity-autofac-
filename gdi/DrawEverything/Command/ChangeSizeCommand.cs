using System.Collections;
using System.Drawing;

namespace DrawEverything.Command
{
    public class ChangeSizeCommand : ICommand
    {
        private readonly DrawBase _itemResized;
        private readonly PointF _oldPoint;
        private readonly PointF _newPoint;
        private readonly int _handle;

        //Disable default constructor
        private ChangeSizeCommand()
        {
        }

        public ChangeSizeCommand(DrawBase itemResized, PointF old, PointF newP, int handle)
        {
            _itemResized = itemResized;
            _oldPoint = new PointF(old.X, old.Y);
            _newPoint = new PointF(newP.X, newP.Y);
            _handle = handle;
        }

        #region ICommand Members

        public void Execute()
        {
            _itemResized.MoveHandleTo(_newPoint, _handle);
        }

        public void UnExecute()
        {
            _itemResized.MoveHandleTo(_oldPoint, _handle);
        }

        #endregion

    }
}