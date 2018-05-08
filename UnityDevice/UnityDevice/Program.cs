using DeviceInterface;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;


namespace UnityDevice
{
    class Program
    {
        static void Main(string[] args)
        {
            //创建容器对象
            IUnityContainer mycontainer = new UnityContainer();

            try
            {
                //配置文件注册
                UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
                section.Configure(mycontainer); //默认调用第一个容器 

                //section.Containers["test"].Configure(mycontainer);//通过名称调用
                //调用依赖
                ISqlHelper mysql = mycontainer.Resolve<ISqlHelper>();
                Console.WriteLine(mysql.SqlConnection());

                IOtherHelper Imysql = mycontainer.Resolve<IOtherHelper>();
                Console.WriteLine(Imysql.GetSqlConnection());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadKey();

        }
    }
}
