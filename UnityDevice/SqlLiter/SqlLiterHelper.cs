using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DeviceInterface;
namespace SqlLiterService 
{
    //构造函数注入
    public class SqlLiterHelper : IOtherHelper
    {
       
        public string GetSqlConnection()
        {
            return "sadasd";
        }

    }
}
