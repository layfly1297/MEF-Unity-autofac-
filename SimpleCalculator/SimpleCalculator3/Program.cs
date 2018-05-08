using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace SimpleCalculator3
{

    public interface ICalculator
    {
        String Calculate(String input);
    }

    public interface IOperation
    {
        int Operate(int left, int right);
    }

    public interface IOperationData
    {
        Char Symbol { get; }
    }

    [Export(typeof(IOperation))]
    [ExportMetadata("Symbol", '+')]
    class Add : IOperation
    {
        public int Operate(int left, int right)
        {
            return left + right;
        }
    }

    [Export(typeof(IOperation))]
    [ExportMetadata("Symbol", '-')]
    class Subtract : IOperation
    {

        public int Operate(int left, int right)
        {
            return left - right;
        }

    }



    [Export(typeof(ICalculator))]
    class MySimpleCalculator : ICalculator
    {
        [ImportMany]
        IEnumerable<Lazy<IOperation, IOperationData>> operations;

        public String Calculate(String input)
        {
            int left;
            int right;
            Char operation;
            int fn = FindFirstNonDigit(input); //finds the operator 查找运算符
            if (fn < 0) return "Could not parse command.";

            try
            {
                //separate out the operands 分出运算符之外
                left = int.Parse(input.Substring(0, fn));
                right = int.Parse(input.Substring(fn + 1));
            }
            catch
            {
                return "Could not parse command.";
            }

            operation = input[fn];

            //经典计算
            foreach (Lazy<IOperation, IOperationData> i in operations)
            {
                if (i.Metadata.Symbol.Equals(operation)) return i.Value.Operate(left, right).ToString();
            }
            return "Operation Not Found!";
        }

        /// <summary>
        /// 返回非数字符号的索引
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private int FindFirstNonDigit(String s)
        {

            for (int i = 0; i < s.Length; i++)
            {
                if (!(Char.IsDigit(s[i]))) return i;
            }
            return -1;
        }


    }


    class Program
    {
        private CompositionContainer _container;

        [Import(typeof(ICalculator))]
        public ICalculator calculator;

 

        private Program()
        {
            //An aggregate catalog that combines multiple catalogs 一个组合多个目录的聚合目录
            var catalog = new AggregateCatalog();
            //Adds all the parts found in the same assembly as the Program class
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(Program).Assembly));
            //catalog.Catalogs.Add(new DirectoryCatalog("C:\\Users\\v-rizhou\\Downloads\\SimpleCalculator\\SimpleCalculator\\CS\\SimpleCalculator3\\Extensions"));
            catalog.Catalogs.Add(new DirectoryCatalog("./Plug"));


            //Create the CompositionContainer with the parts in the catalog
            _container = new CompositionContainer(catalog);

            //Fill the imports of this object
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
            Program p = new Program(); //Composition is performed in the constructor
            String s;
            Console.WriteLine("Enter Command:");
             

            while (true)
            {
                s = Console.ReadLine();
                Console.WriteLine(p.calculator.Calculate(s));

            }


        }
    }
}
