using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DeviceInterface;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace ReceiptPrinter_XBY
{
    //特定的导出

    [Export(typeof(IReceiptPrinter))]
    //[ExportMetadata("FactoryType", "XBY")]
    public class ReceiptPrinter : IReceiptPrinter
    {
        public int Open()
        {
            Console.WriteLine("XinBeiYang ReceiptPrinter");
            return 0;
        }
    }
}
