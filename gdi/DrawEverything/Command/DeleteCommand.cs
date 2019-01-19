/*************************************************
 * 描述：删除
 * 
 * Author：lican@mozihealthcare.cn
 * Date：2019/1/16 17:08:30
 * Update：
 * ************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawEverything.Command
{
    internal struct State
    {
        #region Fields

        public DrawBase Obj;
        public int Zorder;

        #endregion Fields
    }

    public class DeleteCommand : ICommand
    {
        #region Private Fields  

      
        private readonly ArrayList _graphicsList;
        private readonly ArrayList graphicsListDeleted;

        #endregion

        #region Properties  


        #endregion

        #region Events

        #endregion

        #region Constructors
        public DeleteCommand(ArrayList graphicsList)
        {
            _graphicsList = graphicsList;
            graphicsListDeleted = new ArrayList();
        }

        //Disable default constructor
        private DeleteCommand()
        {
        }

        #endregion

        #region Methods


        public void Execute()
        {
            int n = _graphicsList.Count;

            for (int i = n - 1; i >= 0; i--)
            {
                if (!((DrawBase) _graphicsList[i]).Selected) continue;
                State obj;
                obj.Obj = (DrawBase)_graphicsList[i];
                obj.Zorder = i;
                graphicsListDeleted.Add(obj);
                _graphicsList.RemoveAt(i);
            }
        }

        public void UnExecute()
        {
            for (int i = 0; i < graphicsListDeleted.Count; i++)
            {
                //put back whatever is deleted
                var obj = (State)graphicsListDeleted[i];
                _graphicsList.Insert(obj.Zorder, obj.Obj);
            }
        }


        #endregion

        #region Control Events

        #endregion

    }
}
