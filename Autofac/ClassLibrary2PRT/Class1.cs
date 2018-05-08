using DeviceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLibrary2PRT
{
    public class Class1 : PRTInterface
    {
        public int ClosePort()
        {
            Console.WriteLine("Class1");
            return 0;
        }

        public string GetPrinterStatus()
        {
            throw new NotImplementedException();
        }

        public int OpenPort(string pPortName, int pComBaudrate)
        {
            throw new NotImplementedException();
        }

        public string PrintFeedLine(int pRows)
        {
            throw new NotImplementedException();
        }

        public string PrintPicture(string pPicPath, int pLeftMargin, int pDistance)
        {
            throw new NotImplementedException();
        }

        public string PrintText(string pText, int pLeftMargin, int pRowWidth, int pRowHeight, int fontSize)
        {
            throw new NotImplementedException();
        }
    }
}
