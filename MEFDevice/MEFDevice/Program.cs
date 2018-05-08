using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using DeviceInterface;
namespace MEFDevice
{

    public class Program
    {
        //定义容器

        public CompositionContainer _container;

        //导入凭条打印机
        [Import(typeof(IReceiptPrinter))]
        public IReceiptPrinter receiptPrinter;

        public Program()
        {
            //创建组合目录
            var catalog = new AggregateCatalog();
            //添加同一个程序集中的program部件类
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(Program).Assembly));
            //添加目录组件
            catalog.Catalogs.Add(new DirectoryCatalog("./Plug"));
            //在目录中创建合成容器
            _container = new CompositionContainer(catalog);

            //填入这个对象的导入
            try
            {
                this._container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }

        static void Main(string[] args)
        {
            //简单的MEF
            Program p = new Program();
            p.receiptPrinter.Open();
            Console.ReadLine();
        }
    }
}
