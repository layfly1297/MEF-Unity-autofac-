using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLibrary1
{
    using DeviceLibrary;
    using System.Runtime.InteropServices;

    public class RecpPrinter_XBY : PRTInterface
    {
        int PRTInterface.ClosePort()
        {
            // throw new NotImplementedException();
            Console.WriteLine("closePort");
            return 1;
        }

        string PRTInterface.GetPrinterStatus()
        {
            Console.WriteLine("GetPrinterStatus");
            return "";
        }

        int PRTInterface.OpenPort(string pPortName, int pComBaudrate)
        {
            Console.WriteLine("OpenPort");
            return 0;
        }

        string PRTInterface.PrintFeedLine(int pRows)
        {
            Console.WriteLine("PrintFeedLine");
            return "";
        }

        string PRTInterface.PrintPicture(string pPicPath, int pLeftMargin, int pDistance)
        {
            Console.WriteLine("PrintPicture");
            return "";
        }

        string PRTInterface.PrintText(string pText, int pLeftMargin, int pRowWidth, int pRowHeight, int fontSize)
        {
            Console.WriteLine("PrintText");
            return "";
        }




    }
}
