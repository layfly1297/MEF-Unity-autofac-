/*************************************************
 * 描述：
 * 
 * Author：lican@mozihealthcare.cn
 * Date：2019/1/16 17:27:23
 * Update：
 * ************************************************/

using System.Collections;

namespace DrawEverything.Command
{
    /// <summary>
    /// 粘贴
    /// </summary>
    public class PasteCommand : ICommand
    {
        #region Private Fields  
        private readonly ArrayList _graphicsList;
        private readonly ArrayList _toBePasted;
        #endregion

        #region Properties  


        #endregion

        #region Events

        #endregion

        #region Constructors
        //Disable default constructor

        public PasteCommand(ArrayList graphicsList, ArrayList toBePasted)
        {
            _graphicsList = graphicsList;
            //_toBePasted = new ArrayList();
            //_toBePasted.AddRange(toBePasted);
            _toBePasted = toBePasted;
        }
        #endregion

        #region Methods

        public void Execute()
        {
            int n = _toBePasted.Count;

            for (int i = n - 1; i >= 0; i--)
            {
                var obj = (DrawBase)_toBePasted[i];
                obj.Move(10, 10);
                obj.Selected = true;
                _graphicsList.Insert(0, obj);
            }
        }

        public void UnExecute()
        {
            int n = _toBePasted.Count;

            for (int i = n - 1; i >= 0; i--)
            {
                _graphicsList.Remove(_toBePasted[i]);
            }
        }


        #endregion

        #region Control Events

        #endregion


    }
}
