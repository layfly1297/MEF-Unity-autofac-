using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autofac;
using ClassLibrary1;
using ClassLibrary2PRT;
using DeviceLibrary;
using System.Reflection;

namespace AutoFacDevice
{
    class Program
    {
        static void Main(string[] args)
        {
            //创建容器
            var builder = new ContainerBuilder();
            builder.RegisterType<RecpPrinter_XBY>().As<PRTInterface>();//注册类型
            //builder.RegisterType<RecpPrinter_XBY>().AsImplementedInterfaces();
            //builder.RegisterType<Class1>().AsImplementedInterfaces();
            builder.RegisterType<PRTInterfaceLister>();//注册MPGMovieLister类型

            ////字符串为类型完全名称
            //builder.RegisterType(Type.GetType("ClassLibrary2PRT.Class1")).AsImplementedInterfaces();

            using (var container = builder.Build())
            {//创建容器
                var lister = container.Resolve<PRTInterfaceLister>();//使用
                lister.GetPRTInterface().ClosePort();
            }

            Console.ReadKey();
        }

    }


    public class PRTInterfaceLister
    {
        private readonly PRTInterface _PRTInterface;
        //增加了构造函数，参数是IMovieFinder对象
        public PRTInterfaceLister(PRTInterface prtinterface)
        {
            _PRTInterface = prtinterface;
        }

        public PRTInterface GetPRTInterface()
        {
            return _PRTInterface;
        }
    }

    class Program2
    {
        static void Mai2n(string[] args)
        {
            ContainerBuilder builder = new ContainerBuilder();//容器构造器  组件中的类型通过此对象注册到容器中
            builder.RegisterType<AutoFacManager>();//注册类型
            builder.RegisterType<Student>().As<IPerson>();//注册类型且用as方法指定此类型是IPerson接口

            using (IContainer container = builder.Build())//build方法创建容器
            {
                AutoFacManager manager = container.Resolve<AutoFacManager>();//通过resolve方法取得对象
                manager.Say();
            }
            Console.ReadKey();
        }
    }

    public interface IPerson
    {
        void Say();
    }

    public class Worker : IPerson
    {
        public void Say()
        {
            Console.WriteLine("我是一个工人！");
        }
    }

    public class Student : IPerson
    {
        public void Say()
        {
            Console.WriteLine("我是一个学生！");
        }
    }

    public class AutoFacManager
    {
        IPerson person;

        public AutoFacManager(IPerson MyPerson)
        {
            person = MyPerson;
        }

        public void Say()
        {
            person.Say();
        }
    }




}
