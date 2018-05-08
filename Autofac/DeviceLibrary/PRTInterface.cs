using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeviceLibrary
{
    /// <summary>
    /// 凭条打印机接口
    /// </summary>
    public interface PRTInterface
    {
        int OpenPort(string pPortName, int pComBaudrate);

        int ClosePort();

        string PrintText(string pText, int pLeftMargin, int pRowWidth, int pRowHeight, int fontSize);

        string PrintPicture(string pPicPath, int pLeftMargin, int pDistance);

        string PrintFeedLine(int pRows);

        string GetPrinterStatus();
    }
}
