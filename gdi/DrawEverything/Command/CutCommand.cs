/*************************************************
 * 描述：
 * 
 * Author：lican@mozihealthcare.cn
 * Date：2019/1/16 17:26:58
 * Update：
 * ************************************************/

using System.Collections;

namespace DrawEverything.Command
{
    /// <summary>
    /// 剪切
    /// </summary>
    public class CutCommand : ICommand
    {
        #region Private Fields  
        private readonly ArrayList _graphicsList;
        private readonly ArrayList _itemsToBeCut;

        #endregion

        #region Properties  


        #endregion

        #region Events

        #endregion

        #region Constructors

        public CutCommand(ArrayList graphicsList, ArrayList inMemory)
        {
            _graphicsList = graphicsList;
            _itemsToBeCut = new ArrayList();
            _itemsToBeCut.AddRange(inMemory);
        }

        //Disable default constructor

        #endregion

        #region Methods

        public void Execute()
        {
            for (int i = _graphicsList.Count - 1; i >= 0; i--)
            {
                if (_itemsToBeCut.Contains(_graphicsList[i]))
                {
                    _graphicsList.RemoveAt(i);
                }
            }
        }

        public void UnExecute()
        {
            foreach (var t in _itemsToBeCut) 
            {
                _graphicsList.Add(t);
            }

            _itemsToBeCut.Clear();
        }
        #endregion

        #region Control Events

        #endregion


    }
}
