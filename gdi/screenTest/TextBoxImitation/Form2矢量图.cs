using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Drawing.Imaging;
// for Marshal.Copy
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace TextBoxImitation
{
    public partial class Form2矢量图 : Form
    {
        private Metafile metafile1;
        private Graphics.EnumerateMetafileProc metafileDelegate;
        private Point destPoint;
        /// <summary>
        /// 释放指针句柄
        /// </summary>
        /// <param name="hObject"></param>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        public Form2矢量图()
        {
            InitializeComponent();
            //避免锁定处理 
            metafile1 = new Metafile(@"newFile.wmf");
            metafileDelegate = new Graphics.EnumerateMetafileProc(MetafileCallback);
            destPoint = new Point(20, 10);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.EnumerateMetafile(metafile1, destPoint, metafileDelegate);
        }
        private bool MetafileCallback(
           EmfPlusRecordType recordType,
           int flags,
           int dataSize,
           IntPtr data,
           PlayRecordCallback callbackData)
        {
            byte[] dataArray = null;
            if (data != IntPtr.Zero)
            {
                // Copy the unmanaged record to a managed byte buffer 
                // that can be used by PlayRecord.
                dataArray = new byte[dataSize];
                Marshal.Copy(data, dataArray, 0, dataSize);
            }

            metafile1.PlayRecord(recordType, flags, dataSize, dataArray);

            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreateMetaFile_Click(sender, e);
            //ViewFile_Click（se
        }

        /// <summary>
        /// 查看矢量图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewFile_Click(object sender, System.EventArgs e)
        {
            // Create a Graphics object
            Graphics g = this.CreateGraphics();
            g.Clear(this.BackColor);

            // Create a Metafile 
            Metafile curMetafile = new Metafile("newFile.wmf");

            // Draw metafile using DrawImage
            g.DrawImage(curMetafile, 0, 0);

            // Dispose of object
            g.Dispose();
        }

        /// <summary>
        /// 创建矢量图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateMetaFile_Click(object sender, System.EventArgs e)
        {
            Metafile curMetafile = null;
            // Create a Graphics object
            Graphics g = this.CreateGraphics();

            // Get HDC
            IntPtr hdc = g.GetHdc();

            // Create a rectangle
            Rectangle rect = new Rectangle(0, 0, 200, 200);

            // Use HDC to create a metafile with a name

            try
            {
                curMetafile = new Metafile("再次创建.emf", hdc);
            }

            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                g.ReleaseHdc(hdc);
                g.Dispose();
                return;
            }

            // Create a Graphics object from the Metafile object
            Graphics g1 = Graphics.FromImage(curMetafile);

            // Set smooting mode
            g1.SmoothingMode = SmoothingMode.HighQuality;

            // Fill a rectangle on the Metafile object
            g1.FillRectangle(Brushes.Green, rect);
            rect.Y += 110;

            // Draw an ellipse on the Metafile object
            LinearGradientBrush lgBrush =
            new LinearGradientBrush(
            rect, Color.Red, Color.Blue, 45.0f);
            //g1.FillEllipse(lgBrush, rect);

            // Draw text on the Metafile object
            rect.Y += 110;
            //g1.DrawString("MetaFile Sample",
            //new Font("Verdana", 20),
            //lgBrush, 200, 200,
            //StringFormat.GenericTypographic);
            g1.DrawString("文字输入测试 Sample",
                new Font("宋体", 15),
                lgBrush, 200, 200,
                StringFormat.GenericTypographic);
            // Release objects
            g.ReleaseHdc(hdc);
            g1.Dispose();
            g.Dispose();
            curMetafile.Save("再次创建2.emf");
            //curMetafile.Save("metaFile.bmp", ImageFormat.Bmp);
        }


        /// <summary>
        /// 创建位图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateBitmapFile_Click(object sender, System.EventArgs e)
        {
            Bitmap curMetafile = new Bitmap(450, 400); 
             
            // Create a rectangle
            Rectangle rect = new Rectangle(0, 0, 200, 200);

            // Use HDC to create a metafile with a name


            // Create a Graphics object from the Metafile object
            //Graphics g1 = Graphics.FromImage(curMetafile);
            Graphics g1 = pictureBox1.CreateGraphics();

            // Set smooting mode
            g1.SmoothingMode = SmoothingMode.HighQuality;

            // Fill a rectangle on the Metafile object
            g1.FillRectangle(Brushes.Green, rect);
            rect.Y += 110;

            // Draw an ellipse on the Metafile object
            LinearGradientBrush lgBrush =
            new LinearGradientBrush(
            rect, Color.Red, Color.Blue, 45.0f);
            g1.FillEllipse(lgBrush, rect);

            // Draw text on the Metafile object
            rect.Y += 110;
            g1.DrawString("MetaFile Sample",
            new Font("Verdana", 20),
            lgBrush, 200, 200,
            StringFormat.GenericTypographic); 
            // Release objects 
            g1.Dispose();
            //curMetafile.Save("cl.bmp");
            //pictureBox1.Image = curMetafile; 
        }


        /// <summary>
        /// 读取元文件记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnumerateMEtaFile_Click(object sender, System.EventArgs e)
        {
            // Create a Graphics object
            Graphics g = this.panel1.CreateGraphics();
            g.Clear(this.BackColor);

            // Create a Metafile object from a file
            Metafile curMetafile = new Metafile("newFile.wmf");

            // Set EnumerateMetafileProc property
            Graphics.EnumerateMetafileProc enumMetaCB =
            new Graphics.EnumerateMetafileProc(EnumMetaCB);

            // Enumerate metafile
            g.EnumerateMetafile(curMetafile,
            new Point(0, 0), enumMetaCB);

            // Dispose of objects
            curMetafile.Dispose();
            g.Dispose();
        }

        private bool EnumMetaCB(EmfPlusRecordType recordType, int flags, int dataSize, IntPtr data, PlayRecordCallback callbackData)

        {
            string str = " ";

            // Play only EmfPlusRecordType.FillEllipse records

            if (recordType == EmfPlusRecordType.FillEllipse
            || recordType == EmfPlusRecordType.FillRects
            || recordType == EmfPlusRecordType.DrawEllipse
            || recordType == EmfPlusRecordType.DrawRects)
            {
                str = "Record type:" + recordType.ToString() +
                ", Flags:" + flags.ToString() +
                ", Data :" + data.ToString();
                MessageBox.Show(str);
            }

            return true;
        }
        /// <summary>
        /// 读取元件头信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetafileHeaderInfo_Clcik(object sender, System.EventArgs e)
        {
            // Create a Metafile object
            Metafile curMetafile = new Metafile("newFile.wmf");

            // Get metafile header
            MetafileHeader header = curMetafile.GetMetafileHeader();
            // Read metafile header attributes
            string mfAttributes = " ";
            mfAttributes += "Type :" + header.Type.ToString();
            mfAttributes += "Bounds :" + header.Bounds.ToString();
            mfAttributes += "Size :" + header.MetafileSize.ToString();
            mfAttributes += "Version :" + header.Version.ToString();

            // Display message box
            MessageBox.Show(mfAttributes);

            // dispose of object
            curMetafile.Dispose();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ViewFile_Click(sender, e);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            EnumerateMEtaFile_Click(sender, e);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MetafileHeaderInfo_Clcik(sender, e);
        }
        float wy = 0.5f;
        private void button5_Click(object sender, EventArgs e)
        {
            //创建矢量图元件
            Metafile metafile;
            using (Graphics offScreenGraphics = Graphics.FromHwndInternal(IntPtr.Zero))
            {
                IntPtr hDC = offScreenGraphics.GetHdc();
                metafile = new Metafile(hDC, EmfType.EmfPlusDual); 
                offScreenGraphics.ReleaseHdc();
            }

  
            using (Graphics graphics = Graphics.FromImage(metafile))
            {
                graphics.FillRectangle(Brushes.LightYellow, 0, 0, 300, 100);
                graphics.DrawLine(Pens.Red, 50, 50, 100, 100);
                Font f = new Font("宋体", 15);
                graphics.DrawString("sfsdsfdsfasasf一二三四123456", f, Brushes.Black, 0, 0);

            }

            //metafile.Save("test.wmf");
            //IntPtr hEMF, hEMF2;
            //hEMF = metafile.GetHenhmetafile(); // invalidates mf
            //再包装一次
            Metafile metafile2;
            using (Graphics offScreenGraphics = Graphics.FromHwndInternal(IntPtr.Zero))
            {
                IntPtr hDC = offScreenGraphics.GetHdc();
                metafile2 = new Metafile(hDC, EmfType.EmfPlusDual);
                offScreenGraphics.ReleaseHdc();
            }
            using (Graphics graphics = Graphics.FromImage(metafile2))
            {
                wy++;
                graphics.FillRectangle(Brushes.WhiteSmoke, 0, 0, 0, 0);
                graphics.DrawImage(metafile, wy, 0);
            }
            //Graphics graphics = Graphics.FromImage(metafile);
            //graphics.FillRectangle(Brushes.LightYellow, 0, 0, 100, 100);
            //graphics.DrawLine(Pens.Red, 50, 50, 100, 100);
            //graphics.Dispose();
            this.panel2.CreateGraphics().DrawImage(metafile2, 50, 50);
            metafile.Dispose();
            metafile2.Dispose();

        }

        private void button6_Click(object sender, EventArgs e)
        {
            #region 模糊 问题
            Bitmap btmp = new Bitmap(500, 400); 
            Graphics gbt = Graphics.FromImage(btmp);
            //gbt.PageUnit = GraphicsUnit.Millimeter;
            gbt.Clear(Color.White);
            Font f = new Font("宋体", 15);
            gbt.DrawString("sfsdsfdsfasasf一二三四123456", f, Brushes.Black, 0, 0);
            Bitmap cl = new Bitmap(btmp.Width + 300, btmp.Height + 300);
            //cl.SetResolution(300, 300);
            Graphics g = Graphics.FromImage(cl);

            g.PageUnit = GraphicsUnit.Pixel;
            //g.InterpolationMode = InterpolationMode.NearestNeighbor; //临近值解决一层模糊
            g.Clear(Color.Black); 
            g.DrawImage(btmp, 0.32188F, 0);
            gbt.Dispose();
            g.Dispose();
            //g.DrawImage(btmp, 0, 0);
            cl.Save(@"12F.bmp");  
            btmp.Dispose(); 
            this .pictureBox1.CreateGraphics().DrawImage(cl, 0, 0);
            #endregion  

        }
    }


}
