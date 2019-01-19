#define GDIPlus
#define DEBUG

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace screenTest
{

    // C#的宏定义必须出现在所有代码之前。当前我们只让DEBUG宏有效。 
    //#if DEBUG


    //#if DEBUG

    //#else
    //#endif

/*
 原理步骤： 
1、通过窗体 handle 创建设备上下文环境 
2、根据上下文环境句柄建立对应的与设备兼容的内存设备上下文环境（理解成一块画板，内存上的） 
3、通过窗体句柄得到长宽，通过CreateCompatibleBitmap得到窗体对应的设备环境句柄对应的位图区域（理解成自然界的一个美丽的景色） 
4、通过 SelectObject 指向性，把 3 中的位图区域句柄对应到 2 中的内存设备上下文中（理解成，我需要画这大自然的美丽景色，我需要有相应的画布，这里就是在画板上固定画布） 
5、绘图，包括各种 API 绘图或获取图形 
6、此处很重要，因为我们不是要过去框架的整体图形，我们要的是局部的，于是，我们可以通过 1 、2 、3 、4 的步骤，再建设一块画布（第三步有一点区别，就是长宽） 
7、通过 BitBlt 方法把之前画好的画的一个区域复制到我们新建的画板上， BitBlt 的功能是“对指定的源设备环境区域中的像素进行位块（bit_block）转换，以传送到目标设备环境” 
8、然后就简单了，Bitmap bmp = Bitmap.FromHbitmap(myBitmap); 把我们复制出来的第二块画板上的画生成位图图像 
9、再进行什么操作就随你了 
10、最后，最后，最最重要的是……不要忘了使用 DeleteDC 函数清除第 1 、 2 中建的上下文环境，因为这是 API，没有Java的自动清理机制
*/

    public partial class Form贴图GDI : Form
    {
        Graphics g;
        //m_filter是一个黑白图片，我们需要在m_filter为白的部分保持m_background的图像，而在m_filter为黑的部分贴上m_foreground的图像
        Bitmap m_background;//背景色
        Bitmap m_filter; //滤波器
        Bitmap m_foreground;//前景色
        public Form贴图GDI()
        {

            InitializeComponent();

            m_background = new Bitmap(@"m_background.png");
            m_filter = new Bitmap(@"m_filter.jpg");
            m_foreground = new Bitmap(@"m_foregrond.jpg");


#if GDIPlus
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true); //默认启动双缓冲
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
#endif

        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

#if GDIPlus
            //if (frame != null)
            //{
            //    e.Graphics.DrawImage(frame, ClientRectangle);
            //}
#else

#endif
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.IntPtr hdc = g.GetHdc();
            //背景
            System.IntPtr hMemBackground = GDI_XORAND.CreateCompatibleDC(hdc);


            //过滤器
            System.IntPtr hMemTemp = GDI_XORAND.CreateCompatibleDC(hdc);


            System.IntPtr hBmpBackground = GDI_XORAND.SelectObject(hMemBackground, m_background.GetHbitmap());
            var im = Image.FromHbitmap(hBmpBackground);
            pictureBox2.Image = im;

            System.IntPtr hBmpTemp = GDI_XORAND.SelectObject(hMemTemp, m_filter.GetHbitmap());
            im = Image.FromHbitmap(hBmpTemp);
            pictureBox3.Image = im;
            //随后是AND模式的贴图：

            GDI_XORAND.BitBlt(hMemBackground, 0, 0, 300, 300, hMemTemp, 0, 0, GDI_XORAND.SRCPAINT);

            ////照猫画虎，完成第二步OR模式的贴图：

            //GDI_XORAND.SelectObject(hMemTemp, m_foreground.GetHbitmap());
            //var i = 0;
            //GDI_XORAND.BitBlt(hMemBackground, i * 23 + 1, i * 23 + 1, m_foreground.Width, m_foreground.Height, hMemTemp, 0, 0, GDI_XORAND.SRCPAINT);

        
            GDI_XORAND.SelectObject(hMemBackground, hBmpBackground);
            im = Image.FromHbitmap(hBmpBackground);
            pictureBox4.Image = im;
            GDI_XORAND.SelectObject(hMemTemp, hBmpTemp);
            //释放
            g.ReleaseHdc();
            GDI_XORAND.DeleteDC(hMemTemp);
            GDI_XORAND.DeleteDC(hMemBackground);
            //this.Refresh();
            ////SnapShot(this);
            ///
            pictureBox1.Image = fullphoto(200, 400, 0, 0);

            //pictureBox2.Image = SnapShot(pictureBox1);

        }

        /// <summary>
        /// 粘贴图
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private Bitmap SnapShot(Control c)
        {
            Bitmap bmp;
            IntPtr dc1;
            IntPtr dc2;
            Graphics g;

            bmp = new Bitmap(c.Width, c.Height);
            g = System.Drawing.Graphics.FromImage(bmp);

            dc1 = g.GetHdc();

            dc2 = GDI_XORAND.GetDC(c.Handle);

            GDI_XORAND.BitBlt(dc1, 0, 0, c.Width, c.Height, dc2, 0, 0, 13369376);

            g.ReleaseHdc(dc1);
            g.Dispose();
            GDI_XORAND.DeleteDC(dc2);
            GDI_XORAND.DeleteDC(dc1);
            bmp.Save(@"snapshot.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            return bmp;

        }

        /// <summary> 
        /// 截取屏幕图像 
        /// </summary> 
        /// <param name=”Width”>宽</param> 
        /// <param name=”Height”>高</param> 
        /// <param name=”x”>x坐标(全屏时候为0)</param> 
        /// <param name=”y”>y坐标(全屏时候为0)</param> 
        /// <returns></returns>
        public Bitmap fullphoto(int Width, int Height, int x, int y)
        {

            Bitmap bitmap;

            //try

            //{

            IntPtr hScreenDc = GDI_XORAND.CreateDC("DISPLAY", null, null, 0); // 创建桌面句柄

            IntPtr hMemDc = GDI_XORAND.CreateCompatibleDC(hScreenDc); // 创建与桌面句柄相关连的内存DC

            //IntPtr hBitmap = GDI_XORAND.CreateCompatibleBitmap(hScreenDc, Width, Height);//创建图像
            //bitmap = Bitmap.FromHbitmap(hBitmap);
            //bitmap.Save(@"hBitmap.bmp");
            var hBitmap = m_background.GetHbitmap();
            //把位图选进内存DC 

            IntPtr hOldBitmap = GDI_XORAND.SelectObject(hMemDc, hBitmap); //第一次选择贴图对象0，0 图像在目标图的对象
            bitmap = Bitmap.FromHbitmap(hOldBitmap);
            bitmap.Save(@"HoldSelect.bmp");
            //0xcc0020
            //GDI_XORAND.BitBlt(hMemDc, x, y, Width, Height, hScreenDc, x, y, GDI_XORAND.SRCCOPY); //指定为贴图 为Hmemdc赋值
            GDI_XORAND.BitBlt(hScreenDc, x, y, Width, Height, hMemDc, x, y, GDI_XORAND.SRCCOPY); //为指定目标进行贴图

            IntPtr map = GDI_XORAND.SelectObject(hMemDc, hOldBitmap); //映射出贴图

            bitmap = Bitmap.FromHbitmap(map);
            bitmap.Save(@"map.bmp");


            GDI_XORAND.ReleaseDC(hBitmap, hScreenDc);

            GDI_XORAND.DeleteDC(hScreenDc);//删除用过的对象

            GDI_XORAND.DeleteDC(hMemDc);//删除用过的对象

            GDI_XORAND.DeleteDC(hOldBitmap);

            GDI_XORAND.DeleteObject(hBitmap);

            return bitmap;

        }

        /// <summary>
        /// 指定窗口区域截图
        /// </summary>
        /// <param name="handle">窗口句柄. (在windows应用程序中, 从Handle属性获得)</param>
        /// <param name="rect">窗口中的一个区域</param>
        /// <returns></returns>
        //public Bitmap CaptureWindow(IntPtr hWnd, RECT rect)
        //{
        //    // 获取设备上下文环境句柄
        //    IntPtr hscrdc = GDI_XORAND.GetDC(hWnd);

        //    // 创建一个与指定设备兼容的内存设备上下文环境（DC）
        //    IntPtr hmemdc = GDI_XORAND.CreateCompatibleDC(hscrdc);
        //    IntPtr myMemdc = GDI_XORAND.CreateCompatibleDC(hscrdc);

        //    // 返回指定窗体的矩形尺寸
        //    RECT rect1;
        //    GDI_XORAND.GetWindowRect(hWnd, out rect1);

        //    // 返回指定设备环境句柄对应的位图区域句柄
        //    IntPtr hbitmap = GDI_XORAND.CreateCompatibleBitmap(hscrdc, rect1.Right - rect1.Left, rect1.Bottom - rect1.Top);
        //    IntPtr myBitmap = GDI_XORAND.CreateCompatibleBitmap(hscrdc, rect.Right - rect.Left, rect.Bottom - rect.Top);

        //    //把位图选进内存DC 
        //    // IntPtr OldBitmap = (IntPtr)SelectObject(hmemdc, hbitmap);
        //    GDI_XORAND.SelectObject(hmemdc, hbitmap);
        //    GDI_XORAND.SelectObject(myMemdc, myBitmap);

        //    /////////////////////////////////////////////////////////////////////////////
        //    //
        //    // 下面开始所谓的作画过程，此过程可以用的方法很多，看你怎么调用 API 了
        //    //
        //    /////////////////////////////////////////////////////////////////////////////

        //    // 直接打印窗体到画布
        //    PrintWindow(hWnd, hmemdc, 0);

        //    // IntPtr hw = GetDesktopWindow();
        //    // IntPtr hmemdcClone = GetWindowDC(myBitmap);

        //    GDI_XORAND.BitBlt(myMemdc, 0, 0, rect.Right - rect.Left, rect.Bottom - rect.Top, hmemdc, rect.Left, rect.Top, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);
        //    //SelectObject(myMemdc, myBitmap);

        //    Bitmap bmp = Bitmap.FromHbitmap(myBitmap);
        //    GDI_XORAND.DeleteDC(hscrdc);
        //    GDI_XORAND.DeleteDC(hmemdc);
        //    GDI_XORAND.DeleteDC(myMemdc);
        //    return bmp;
        //}

        /// <summary>
        /// 半透明
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="src"></param>
        /// <param name="opcity"></param>
        private void HalfOpcityImage(Bitmap dest, Bitmap src, double opcity)
        {
            Rectangle rect = new Rectangle(0, 0, dest.Width, dest.Height);
            BitmapData destData = dest.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData srcData = src.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);

            IntPtr pD = destData.Scan0;
            IntPtr pS = srcData.Scan0;

            byte[] bufferD = new byte[dest.Width * dest.Height * 4];
            byte[] bufferS = new byte[src.Width * src.Height * 4];

            Marshal.Copy(pD, bufferD, 0, bufferD.Length);
            Marshal.Copy(pS, bufferS, 0, bufferS.Length);

            for (int i = 0; i < bufferD.Length; i += 4)
            {
                bufferD[i] = (byte)(bufferD[i] * opcity + bufferS[i] * (1 - opcity));
                bufferD[i + 1] = (byte)(bufferD[i + 1] * opcity + bufferS[i + 1] * (1 - opcity));
                bufferD[i + 2] = (byte)(bufferD[i + 2] * opcity + bufferS[i + 2] * (1 - opcity));
            }

            Marshal.Copy(bufferD, 0, pD, bufferD.Length);

            dest.UnlockBits(destData);
            src.UnlockBits(srcData);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            HalfOpcityImage(m_background, m_foreground, 1);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form贴图GDI_Load(object sender, EventArgs e)
        {
            g = pictureBox1.CreateGraphics();
            Form1_Load(sender, e);
        }


        private void Form1_Load(object sender,
            System.EventArgs e)
        {
            Graphics g1 = this.CreateGraphics();
            Graphics g2 = null;
            try
            {
                g1.SmoothingMode =
                    SmoothingMode.AntiAlias;
                g1.FillRectangle(SystemBrushes.Window,
                    0, 0, 300, 300);
                g1.DrawLine(new Pen(Color.Yellow, 2),
                    10, 10, 150, 10);
                g1.DrawLine(new Pen(Color.Teal, 2),
                    10, 10, 10, 150);
                g1.FillRectangle(Brushes.Blue,
                    30, 30, 70, 70);
                g1.FillEllipse(new HatchBrush
                    (HatchStyle.DashedDownwardDiagonal,
                        Color.Red, Color.Green),
                    110, 110, 100, 100);
                
                Bitmap curBitmap = new Bitmap(
                    this.ClientRectangle.Width,
                    this.ClientRectangle.Height, g1);
                curBitmap.Save("dyc.bmp");
                g2 = Graphics.FromImage(curBitmap);
                IntPtr hdc1 = g1.GetHdc();
                IntPtr hdc2 = g2.GetHdc();
                GDI_XORAND.BitBlt(hdc2, 0, 0,
                    this.ClientRectangle.Width,
                    this.ClientRectangle.Height,
                    hdc1, 0, 0, 13369376);
                g1.ReleaseHdc(hdc1);
                g2.ReleaseHdc(hdc2);
                g2.Dispose();
                g1.Dispose();
                curBitmap.Save("BitBltImg.jpg",
                    ImageFormat.Jpeg);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message.ToString());
            }
            finally
            {
                g2.Dispose();
                g1.Dispose();
            }
        }


    }


}