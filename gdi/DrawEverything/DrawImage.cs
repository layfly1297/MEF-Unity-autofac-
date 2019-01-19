using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace DrawEverything
{
    public class DrawImage : DrawRectangleBase
    {
        #region Fields

        public static string CurrentFileName = "";
        public static Image CurrentImage;

        private const string Tag = "image";

        private string _fileName = "";
        private string _id = "";
        private Image _image;
        private bool _reload;

        #endregion Fields

        #region Constructors

        public DrawImage()
        {
            SetRectangleF(0, 0, 1, 1);
            Initialize();
        }

        public DrawImage(float x, float y)
        {
            if (CurrentImage == null)
            {
                var bmp = new Bitmap(100, 100);
                CurrentImage = bmp;
            }
            InitBox();
            _image = CurrentImage;
            _fileName = CurrentFileName;
            RectangleFz = new RectangleF(x, y, _image.Width, _image.Height);
            Initialize();
        }

        public DrawImage(string fileName, float x, float y, float width, float height)
        {
            InitBox();
            _fileName = fileName;
            try
            {
                _image = Image.FromFile(fileName);
            }
            catch (Exception ex)
            {
                Error("DrawArea DrawImage" + ex.ToString());
            }
            RectangleF z = new RectangleF(x, y, width, height);
            Initialize();
        }

        #endregion Constructors

        #region Properties

        //[Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public String FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _reload = true;
                _fileName = value;
            }
        }

     

        #endregion Properties

        #region Methods

        public static DrawImage Create(DataStructrueImage svg)
        {
            try
            {
                DrawImage dobj =
                    new DrawImage(svg.Href, svg.X, svg.Y, svg.Width, svg.Height);

                var di = new DrawImage();
                if (!di.FillFromSvg(svg))
                    return null;
                dobj = di;
                return dobj;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get Image object from byte array
        /// </summary>
        /// <param name="arrb"></param>
        /// <returns></returns>
        public static Image ImageFromBytes(byte[] arrb)
        {
            if (arrb == null)
                return null;
            try
            {
                // Perform the conversion
                var ms = new MemoryStream();
                const int offset = 0;
                ms.Write(arrb, offset, arrb.Length - offset);
                Image im = new Bitmap(ms);
                ms.Close();
                return im;
            }
            catch (Exception)
            { 
                return null;
            }
        }

        /// <summary>
        /// Load image from file to byte array
        /// </summary>
        /// <param name="flnm">File name</param>
        /// <returns>byte array</returns>
        public static byte[] ReadPngMemImage(string flnm)
        {
            try
            {
                FileStream fs = new FileStream(flnm, FileMode.Open, FileAccess.Read);
                MemoryStream ms = new MemoryStream();
                Bitmap bm = new Bitmap(fs);
                bm.Save(ms, ImageFormat.Png);
                BinaryReader br = new BinaryReader(ms);
                ms.Position = 0;
                byte[] arrpic = br.ReadBytes((int)ms.Length);
                br.Close();
                fs.Close();
                ms.Close();
                return arrpic;
            }
            catch (Exception ex)
            { 
                return null;
            }
        }

        public override void Draw(Graphics g)
        {
            try
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;//使绘图质量最高，即消除锯齿
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                if (_reload)
                {
                    _image = ImageFromBytes(ReadPngMemImage(_fileName));
                    Width = _image.Width;
                    Height = _image.Height;
                    _reload = false;
                }
                if (_image != null)
                    g.DrawImage(_image, RectangleFz);
                else
                    base.Draw(g);
            }
            catch (Exception ex)
            { 
            }
        }

        [CLSCompliant(false)]
        public bool FillFromSvg(DataStructrueImage svg)
        {
            try
            {
                RectangleFz = new RectangleF(svg.X, svg.Y, svg.Width, svg.Height);
                _fileName = svg.Href;
                _id = svg.Id;
                try
                {
                    _image = Image.FromFile(_fileName);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            catch (Exception ex0)
            {

                return false;
            }
        }


        void InitBox()
        {
            Stroke = Color.Red;
            StrokeWidth = 0.5f; //1;
        }

        #endregion Methods
    }
}