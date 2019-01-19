#pragma warning disable 1587
/// ***********************************************************************
///
/// =================================
/// CLR版本    ：4.0.30319.42000
/// 命名空间    ：DrawEverything
/// 文件名称    ：DrawRadioBoxGroup.cs
/// =================================
/// 创 建 者    ：lican
/// 创建日期    ：2019/1/10 14:19:55 
/// 邮箱        ：nihaolican@qq.com
/// 功能描述    ：单选框组
/// 使用说明    ：
/// =================================
/// 修改者    ：
/// 修改日期    ：
/// 修改内容    ：
/// =================================
///
/// ***********************************************************************
#pragma warning restore 1587

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawEverything
{
    public class DrawRadioBoxGroup : DrawBase
    {

        #region Private Fields

        private List<DrawRadioBox> Childs;

        #endregion

        #region Properties

        //true 是单选组 false 为多选择组
        public bool IsRadioGroup { get; set; }

        #endregion

        #region Events

        #endregion

        #region Constructors

        public DrawRadioBoxGroup()
        {
            Childs = new List<DrawRadioBox>();
        }

        #endregion

        #region Control Events

        #endregion

        #region Methods 


        public void Add(DrawRadioBox item)
        {
            Childs.Add(item);
        }


        public void Remove(DrawRadioBox item)
        {
            Childs.Remove(item);
        }

        #endregion
    }
}