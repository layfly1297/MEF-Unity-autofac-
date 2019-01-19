/// ***********************************************************************
///
/// =================================
/// CLR版本    ：4.0.30319.42000
/// 命名空间    ：screenTest
/// 文件名称    ：GDI_XORAND.cs
/// =================================
/// 创 建 者    ：lican
/// 创建日期    ：2018/11/26 17:17:29 
/// 邮箱        ：nihaolican@qq.com
/// 功能描述    ：GDI+贴图
/// 使用说明    ：
/// =================================
/// 修改者    ：
/// 修改日期    ：
/// 修改内容    ：
/// =================================
///
/// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Reflection;
using System.Drawing;

namespace screenTest
{
    class GDI_XORAND
    {


        #region Private Fields
        #endregion

        #region Properties


        #endregion

        #region Events

        #endregion

        #region Constructors




        #endregion

        #region Control Events

        #endregion

        #region Methods
        #region gdi APi



        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]

        public static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, int lpInitData);

        //释放用过的设备句柄

        [DllImport("user32.dll")]

        public static extern bool ReleaseDC(

         IntPtr hwnd, IntPtr hdc

         );


        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);


        [DllImport("gdi32.dll")]
        static public extern IntPtr CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
        public static extern bool BitBlt(IntPtr hdcDest, int nXDest,
        int nYDest, int nWidth, int nHeight, IntPtr hdcSrc,
        int nXSrc, int nYSrc, int dwRop);
        [DllImport("user32.dll", EntryPoint = "GetDC")]
        public static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("gdi32.dll")]
        public static extern int SetROP2(int h, int op);
        [DllImport("gdi32.dll")]
        static public extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll")]
        static public extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        [DllImport("gdi32.dll")]
        static public extern IntPtr DeleteDC(IntPtr hDC);

        [DllImport("gdi32.dll")]

        public static extern bool FillRgn(IntPtr hdc, IntPtr hrgn, IntPtr hbr);

        [DllImport("gdi32.dll")]

        public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect,

        int nBottomRect);

        [DllImport("gdi32.dll")]

        public static extern IntPtr CreateSolidBrush(Int32 crColor);

        [DllImport("gdi32.dll")]

        //设置透明的 
        public static extern int SetBkMode(IntPtr hdc, int iBkMode);

        //透明
        public const int TRANSPARENT = 1;

        //不透明
        public const int OPAQUE = 2;


        [DllImport("gdi32.dll")]

        static extern uint SetBkColor(IntPtr hdc, int crColor);

        [DllImport("gdi32.dll")]

        static extern uint SetTextColor(IntPtr hdc, int crColor);

        [DllImport("gdi32", EntryPoint = "CreateFontW", CharSet = CharSet.Auto)]

        static extern IntPtr CreateFontW(

        [In] Int32 nHeight,

        [In] Int32 nWidth,

        [In] Int32 nEscapement,

        [In] Int32 nOrientation,

        [In] FontWeight fnWeight,

        [In] Boolean fdwItalic,

        [In] Boolean fdwUnderline,

        [In] Boolean fdwStrikeOut,

        [In] FontCharSet fdwCharSet,

        [In] FontPrecision fdwOutputPrecision,

        [In] FontClipPrecision fdwClipPrecision,

        [In] FontQuality fdwQuality,

        [In] FontPitchAndFamily fdwPitchAndFamily,

        [In] String lpszFace);

        [DllImport("gdi32.dll")]

        public static extern int GetTextFace(IntPtr hdc, int nCount,

        [Out] StringBuilder lpFaceName);

        public const Int32 LF_FACESIZE = 32;

        [DllImport("gdi32.dll", ExactSpelling = true)]

        public static extern bool BitBlt(

        IntPtr hdcDest, // 目标设备的句柄

        int nXDest, // 目标对象的左上角的X坐标

        int nYDest, // 目标对象的左上角的Y坐标

        int nWidth, // 目标对象的矩形的宽度

        int nHeight, // 目标对象的矩形的长度

        IntPtr hdcSrc, // 源设备的句柄

        int nXSrc, // 源对象的左上角的X坐标

        int nYSrc, // 源对象的左上角的X坐标

        TernaryRasterOperations dwRop // 光栅的操作值

        );

        [DllImport("gdi32.dll")]

        public static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest,

        int nWidthDest, int nHeightDest,

        IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc,

        TernaryRasterOperations dwRop);

        /*
         * GetTextExtentPoint可以方便的获取一个字符串,或者字符串的部分的长度,因为可以通过cbString这个长度来控制获取的范围.

          GetTextMetrics则是获取一个字体的各种高度信息,包括height,ascent,descent,还包括字体能够表现的字符范围等等信息.

          GetCharABCWidthsFloatW则是获取某段连续字符串的abcwidth信息,abcwidth信息在某些情况下,需要特别注意,否则斜体会排版得很难看.

          GetCharWidth32获取一个连续的字符段的宽度信息, 但是根据实践,居然和GetTextExtentPoint获取的信息不大一致,暂时是少于实际占用的空间.使到计算出来的占用宽度实际上不足以容纳字符串的排版.

         * */
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]

        public static extern bool GetTextExtentPoint(IntPtr hdc, string lpString,

        int cbString, ref Size lpSize);

        [DllImport("Gdi32.dll", CharSet = CharSet.Auto)]

        public static extern bool GetTextMetrics(IntPtr hdc, out TEXTMETRIC lptm);

        [DllImport("gdi32.dll")]

        public static extern bool GetCharABCWidthsFloatW(IntPtr hdc, uint iFirstChar, uint iLastChar, [Out] ABCFloat[] lpABCF);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]

        //TextOutW是一个比较简单的函数,适合一般的场合,只需要设置X和Y的坐标即可

        //DrawText,则会控制输出的空间大小,排版规则.比较适合需要精确控制的场所,又或者比如说输出阿拉伯文字的时候,要设置为右对齐
        public static extern bool TextOutW(IntPtr hdc, int nXStart, int nYStart,

        string lpString, int cbString);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]

        public static extern bool GetCharWidth32(IntPtr hdc, uint iFirstChar, uint iLastChar,

        [Out] int[] lpBuffer);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]

        public static extern int DrawText(IntPtr hdc, string lpStr, int nCount, ref Rect lpRect, dwDTFormat wFormat);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern bool StretchBlt(
   int x,  //指定目的矩形区域左上角的X坐标
   int y,  //指定目的矩形区域左上角的Y坐标
   int nWidth,  //指定目的矩形区域的宽度
   int nHeight,  //指定目的矩形区域的高度
   IntPtr pSrcDC,
   int xSrc,  //指定源矩形区域左上角的X坐标
   int ySrc,  //指定源矩形区域左上角的Y坐标
   int nSrcWidth, //指定源矩形区域的宽度
   int nSrcHeight, //指定源矩形区域的高度
   IntPtr dwRop   //此参数参考MSDN，SRCCOPY类型为直接拷贝
);  //此函数将一个位图资源从一个矩形区域拷贝到另一个矩形区域，即缩放位图 


        //其中EnumFontCallBack为回调函数,通过这个回调函数,可以获取到系统所有的字体,包括点阵的字体.
        public class EnumFontFamilies
        {

            public const int LF_FACESIZE = 32;

            public delegate int EnumFontExDelegate(ref ENUMLOGFONTEX lpelfe, IntPtr lpntme, int FontType, int lParam);

            [DllImport("gdi32.dll", EntryPoint = "EnumFontFamiliesEx", CharSet = CharSet.Unicode)]

            public static extern int EnumFontFamiliesEx(IntPtr hDC, [In] LOGFONT logFont, EnumFontExDelegate enumFontExCallback,

            IntPtr lParam, uint dwFlags);

        }

        private Int32 EnumFontCallBack(ref ENUMLOGFONTEX lpelfe, IntPtr lpntme, int FontType, int lParam)
        {

            //Debug.WriteLine(lpelfe.elfFullName); 

            //if (lpelfe.elfFullName.Substring(0, 1) != "@")

            //{

            //    if (!sysFontList.Contains(lpelfe.elfFullName))

            //    {

            //        sysFontList.Add(lpelfe.elfFullName);

            //    }

            //}

            return 1;

        }
        #region enum

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct ABCFloat
        {

            public LOGFONT abcfA;

            public LOGFONT abcfB;

            public LOGFONT abcfC;
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]

        public struct ENUMLOGFONTEX
        {

            public LOGFONT elfLogFont;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]

            public string elfFullName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]

            public string elfStyle;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]

            public string elfScript;

        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]

        public class LOGFONT
        {

            public int lfHeight;

            public int lfWidth;

            public int lfEscapement;

            public int lfOrientation;

            public FontWeight lfWeight;

            [MarshalAs(UnmanagedType.U1)]

            public bool lfItalic;

            [MarshalAs(UnmanagedType.U1)]

            public bool lfUnderline;

            [MarshalAs(UnmanagedType.U1)]

            public bool lfStrikeOut;

            public FontCharSet lfCharSet;

            public FontPrecision lfOutPrecision;

            public FontClipPrecision lfClipPrecision;

            public FontQuality lfQuality;

            public FontPitchAndFamily lfPitchAndFamily;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]

            public string lfFaceName;

            public override string ToString()
            {

                StringBuilder sb = new StringBuilder();

                sb.Append("LOGFONT\n");

                sb.AppendFormat(" lfHeight: {0}\n", lfHeight);

                sb.AppendFormat(" lfWidth: {0}\n", lfWidth);

                sb.AppendFormat(" lfEscapement: {0}\n", lfEscapement);

                sb.AppendFormat(" lfOrientation: {0}\n", lfOrientation);

                sb.AppendFormat(" lfWeight: {0}\n", lfWeight);

                sb.AppendFormat(" lfItalic: {0}\n", lfItalic);

                sb.AppendFormat(" lfUnderline: {0}\n", lfUnderline);

                sb.AppendFormat(" lfStrikeOut: {0}\n", lfStrikeOut);

                sb.AppendFormat(" lfCharSet: {0}\n", lfCharSet);

                sb.AppendFormat(" lfOutPrecision: {0}\n", lfOutPrecision);

                sb.AppendFormat(" lfClipPrecision: {0}\n", lfClipPrecision);

                sb.AppendFormat(" lfQuality: {0}\n", lfQuality);

                sb.AppendFormat(" lfPitchAndFamily: {0}\n", lfPitchAndFamily);

                sb.AppendFormat(" lfFaceName: {0}\n", lfFaceName);

                return sb.ToString();

            }

        }

        public enum FontWeight : int
        {

            FW_DONTCARE = 0,

            FW_THIN = 100,

            FW_EXTRALIGHT = 200,

            FW_LIGHT = 300,

            FW_NORMAL = 400,

            FW_MEDIUM = 500,

            FW_SEMIBOLD = 600,

            FW_BOLD = 700,

            FW_EXTRABOLD = 800,

            FW_HEAVY = 900,

        }

        public enum FontCharSet : byte
        {

            ANSI_CHARSET = 0,

            DEFAULT_CHARSET = 1,

            SYMBOL_CHARSET = 2,

            SHIFTJIS_CHARSET = 128,

            HANGEUL_CHARSET = 129,

            HANGUL_CHARSET = 129,

            GB2312_CHARSET = 134,

            CHINESEBIG5_CHARSET = 136,

            OEM_CHARSET = 255,

            JOHAB_CHARSET = 130,

            HEBREW_CHARSET = 177,

            ARABIC_CHARSET = 178,

            GREEK_CHARSET = 161,

            TURKISH_CHARSET = 162,

            VIETNAMESE_CHARSET = 163,

            THAI_CHARSET = 222,

            EASTEUROPE_CHARSET = 238,

            RUSSIAN_CHARSET = 204,

            MAC_CHARSET = 77,

            BALTIC_CHARSET = 186,

        }

        public enum FontPrecision : byte
        {

            OUT_DEFAULT_PRECIS = 0,

            OUT_STRING_PRECIS = 1,

            OUT_CHARACTER_PRECIS = 2,

            OUT_STROKE_PRECIS = 3,

            OUT_TT_PRECIS = 4,

            OUT_DEVICE_PRECIS = 5,

            OUT_RASTER_PRECIS = 6,

            OUT_TT_ONLY_PRECIS = 7,

            OUT_OUTLINE_PRECIS = 8,

            OUT_SCREEN_OUTLINE_PRECIS = 9,

            OUT_PS_ONLY_PRECIS = 10,

        }

        public enum FontClipPrecision : byte
        {

            CLIP_DEFAULT_PRECIS = 0,

            CLIP_CHARACTER_PRECIS = 1,

            CLIP_STROKE_PRECIS = 2,

            CLIP_MASK = 0xf,

            CLIP_LH_ANGLES = (1 << 4),

            CLIP_TT_ALWAYS = (2 << 4),

            CLIP_DFA_DISABLE = (4 << 4),

            CLIP_EMBEDDED = (8 << 4),

        }

        public enum FontQuality : byte
        {

            DEFAULT_QUALITY = 0,

            DRAFT_QUALITY = 1,

            PROOF_QUALITY = 2,

            NONANTIALIASED_QUALITY = 3,

            ANTIALIASED_QUALITY = 4,

            CLEARTYPE_QUALITY = 5,

            CLEARTYPE_NATURAL_QUALITY = 6,

        }

        [Flags]

        public enum FontPitchAndFamily : byte
        {

            DEFAULT_PITCH = 0,

            FIXED_PITCH = 1,

            VARIABLE_PITCH = 2,

            FF_DONTCARE = (0 << 4),

            FF_ROMAN = (1 << 4),

            FF_SWISS = (2 << 4),

            FF_MODERN = (3 << 4),

            FF_SCRIPT = (4 << 4),

            FF_DECORATIVE = (5 << 4),

        }

        /// <summary> 

        /// Enumeration for the raster operations used in BitBlt. 

        /// In C++ these are actually #define. But to use these 

        /// constants with C#, a new enumeration type is defined. 

        /// </summary> 

        public enum TernaryRasterOperations
        {

            SRCCOPY = 0x00CC0020, /* dest = source */

            SRCPAINT = 0x00EE0086, /* dest = source OR dest */

            SRCAND = 0x008800C6, /* dest = source AND dest */

            SRCINVERT = 0x00660046, /* dest = source XOR dest */

            SRCERASE = 0x00440328, /* dest = source AND (NOT dest ) */

            NOTSRCCOPY = 0x00330008, /* dest = (NOT source) */

            NOTSRCERASE = 0x001100A6, /* dest = (NOT src) AND (NOT dest) */

            MERGECOPY = 0x00C000CA, /* dest = (source AND pattern) */

            MERGEPAINT = 0x00BB0226, /* dest = (NOT source) OR dest */

            PATCOPY = 0x00F00021, /* dest = pattern */

            PATPAINT = 0x00FB0A09, /* dest = DPSnoo */

            PATINVERT = 0x005A0049, /* dest = pattern XOR dest */

            DSTINVERT = 0x00550009, /* dest = (NOT dest) */

            BLACKNESS = 0x00000042, /* dest = BLACK */

            WHITENESS = 0x00FF0062, /* dest = WHITE */

        };

        [Flags]

        public enum dwDTFormat : int
        {

            DT_TOP = 0, DT_LEFT = 0x00000000, DT_CENTER = 0x00000001, DT_RIGHT = 0x00000002,

            DT_VCENTER = 0x00000004, DT_BOTTOM = 0x00000008, DT_WORDBREAK = 0x00000010, DT_SINGLELINE = 0x00000020,

            DT_EXPANDTABS = 0x00000040, DT_TABSTOP = 0x00000080, DT_NOCLIP = 0x00000100, DT_EXTERNALLEADING = 0x00000200,

            DT_CALCRECT = 0x00000400, DT_NOPREFIX = 0x00000800, DT_INTERNAL = 0x00001000

        };

        public struct Rect
        {

            public int Left, Top, Right, Bottom;

            public Rect(Rectangle r)
            {

                this.Left = r.Left;

                this.Top = r.Top;

                this.Bottom = r.Bottom;

                this.Right = r.Right;

            }

        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]

        public struct TEXTMETRIC
        {

            public Int32 tmHeight;

            public Int32 tmAscent;

            public Int32 tmDescent;

            public Int32 tmInternalLeading;

            public Int32 tmExternalLeading;

            public Int32 tmAveCharWidth;

            public Int32 tmMaxCharWidth;

            public Int32 tmWeight;

            public Int32 tmOverhang;

            public Int32 tmDigitizedAspectX;

            public Int32 tmDigitizedAspectY;

            public char tmFirstChar;

            public char tmLastChar;

            public char tmDefaultChar;

            public char tmBreakChar;

            public byte tmItalic;

            public byte tmUnderlined;

            public byte tmStruckOut;

            public byte tmPitchAndFamily;

            public byte tmCharSet;

        }
        #endregion

        #endregion

        //这里的一个AND和一个OR操作需要用BitBlt函数完成。我们先声明表示操作的一些常量：
        public const int SRCAND = 0x8800C6; // (DWORD) dest = source AND dest
        public const int SRCCOPY = 0xCC0020; // (DWORD) dest = source
        public const int SRCERASE = 0x440328; // (DWORD) dest = source AND (NOT dest )
        public const int SRCINVERT = 0x660046; // (DWORD) dest = source XOR dest
        public const int SRCPAINT = 0xEE0086; // (DWORD) dest = source OR dest
                                              //        （其实只要SRCAND和SRCPAINT就可以了，因为本文只讲到了AND和OR模式的贴图。其他模式的贴图可以类似操作。）
                                              //然后，我们在内存中建立一个缓冲区保存操作中生成的图片的中间结果：

        void test1()
        {
            //一般的使用画图的步骤
            //IntPtr pTarget = e.Graphics.GetHdc();

            //IntPtr pSource = CreateCompatibleDC(pTarget);

            //IntPtr pOrig = SelectObject(pSource, drawBmp.GetHbitmap());

            //StretchBlt(pTarget, 0, 0, this.Width, this.Height, pSource, 0, 0, drawWidth, drawHeight, TernaryRasterOperations.SRCCOPY);

            //IntPtr pNew = SelectObject(pSource, pOrig);

            //DeleteObject(pNew);

            //DeleteDC(pSource);

            //e.Graphics.ReleaseHdc(pTarget);
        }
        #endregion
    }
}
