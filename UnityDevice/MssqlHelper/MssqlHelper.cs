using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DeviceInterface;
namespace MssqlHelperService
{
    public class MssqlHelper : ISqlHelper
    {
        public string SqlConnection()
        {
            return "this mysql."; 
        }
    }
}
