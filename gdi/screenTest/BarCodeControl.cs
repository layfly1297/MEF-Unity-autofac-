using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoziEditorControl.Infrastructure.BarCodeLib;
using System.Drawing.Imaging;
using screenTest.PopupControl;

namespace screenTest
{
    public partial class BarCodeControl : UserControl
    {
        public event HandledEventHandler BarCodeImageChange;
        Image _barCodeImage;
        BarCoderInvoke barCodeInvoke;
        public string ContextBarCode { get; set; }

        public int BarHeight
        {
            set
            {
                if (barCodeInvoke != null)
                { barCodeInvoke.Height = value; }
            }
        }

        public int BarWidth
        {
            set
            {
                if (barCodeInvoke != null)
                { barCodeInvoke.Width = value; }
            }
        }

        public BarCodeControl()
        {
            InitializeComponent();
            barCodeInvoke = new BarCoderInvoke();
            comboBoxAlignment.DataSource = System.Enum.GetNames(typeof(LabelPositions));
            comboBoxBarCodeStyle.DataSource = System.Enum.GetNames(typeof(TYPE));
            barCodeInvoke.SetDefaultProperty();
            barCodeInvoke.Width = picBoxBarCode.Width;
            barCodeInvoke.Height = picBoxBarCode.Height;

            #region 值去除留白  去除留白无法设置大小 
            barCodeInvoke.BarWidth = 1;
            //barCodeInvoke.AspectRatio = 1;
            #endregion

            comboBoxBarCodeStyle.SelectedIndex = (int)TYPE.CODE128C;

            comboBoxAlignment.SelectedIndex = (int)LabelPositions.BOTTOMCENTER;
            _barCodeImage = barCodeInvoke.Encode((TYPE)comboBoxBarCodeStyle.SelectedIndex, textBoxContext.Text);

            //picBoxBarCode.Refresh();

        }

        private void comboBoxBarCodeStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxBarCodeStyle.SelectedItem != null && comboBoxAlignment.SelectedItem != null && textBoxContext.Text != null)
            {
                barCodeInvoke.LabelPosition = (LabelPositions)comboBoxAlignment.SelectedIndex;
                barCodeInvoke.IncludeLabel = checkBoxContextVisble.Checked;
                _barCodeImage = barCodeInvoke.Encode((TYPE)comboBoxBarCodeStyle.SelectedIndex, textBoxContext.Text);
                picBoxBarCode.Refresh();
            }
        }

        private void picBoxBarCode_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.White, e.ClipRectangle);
            if (_barCodeImage == null)
            {
                if (barCodeInvoke.Errors.Count > 0)
                {
                    var wz = e.Graphics.MeasureString(barCodeInvoke.Errors[0], SystemFonts.DefaultFont).Width;
                    var line = Math.Ceiling(wz / picBoxBarCode.Width);
                    for (int i = 0; i < line; i++)
                    {
                        int count = (int)(barCodeInvoke.Errors[0].Length / line);
                        if (i == 0)
                        {
                            e.Graphics.DrawString(barCodeInvoke.Errors[0].Substring(0, count), SystemFonts.DefaultFont, SystemBrushes.Desktop, 0, 0);
                        }
                        else
                        {
                            e.Graphics.DrawString(barCodeInvoke.Errors[0].Substring(i * count, count), SystemFonts.DefaultFont, SystemBrushes.Desktop, 0, i * SystemFonts.DefaultFont.GetHeight());
                        }
                    }
                }
                return;
            }
            var x = picBoxBarCode.Width == _barCodeImage.Width ? 0 :
                picBoxBarCode.Width - _barCodeImage.Width > 0 ? (picBoxBarCode.Width - _barCodeImage.Width) / 2 : 0;
            var y = (picBoxBarCode.Height - _barCodeImage.Height) / 2;
            if (_barCodeImage.Width > picBoxBarCode.Width)
            {
                float xs = (float)picBoxBarCode.Width / _barCodeImage.Width;
                float ys = (float)picBoxBarCode.Height / _barCodeImage.Height;
                e.Graphics.ScaleTransform(xs, ys);
            }
            e.Graphics.DrawImage(_barCodeImage, x, y);
        }

        private void buttonSureBarcode_Click(object sender, EventArgs e)
        {

            ContextBarCode = textBoxContext.Text;
            var popup = this.Parent as Popup;
            popup?.Close();
            BarCodeImageChange?.Invoke(this, null);
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            ContextBarCode = null;
            var popup = this.Parent as Popup;
            popup?.Close();
            _barCodeImage = null;
            BarCodeImageChange?.Invoke(this, null);
        }

        public Image BarCodeImage => _barCodeImage;
    }


    internal class BarCoderInvoke : IDisposable
    {
        #region Variables
        private Barcode ibarcode = new Barcode();

        #endregion

        #region Properties

        public string RawData
        {
            get { return ibarcode.RawData; }
            set { ibarcode.RawData = value; }
        }

        public string EncodedValue
        {
            get { return ibarcode.EncodedValue; }
        }

        public TYPE EncodedType
        {
            get { return ibarcode.EncodedType; }
            set { ibarcode.EncodedType = value; }
        }

        public Image EncodedImage
        {
            get { return ibarcode.EncodedImage; }
        }

        public Color ForeColor
        {
            get { return ibarcode.ForeColor; }
            set { ibarcode.ForeColor = value; }
        }

        public Color BackColor
        {
            get { return ibarcode.BackColor; }
            set { ibarcode.BackColor = value; }
        }

        public Font LabelFont
        {
            get { return ibarcode.LabelFont; }
            set { ibarcode.LabelFont = value; }
        }

        public LabelPositions LabelPosition
        {
            get { return ibarcode.LabelPosition; }
            set { ibarcode.LabelPosition = value; }
        }
        public RotateFlipType RotateFlipType
        {
            get { return ibarcode.RotateFlipType; }
            set { ibarcode.RotateFlipType = value; }
        }
        public int Width
        {
            get { return ibarcode.Width; }
            set { ibarcode.Width = value; }
        }

        public int Height
        {
            get { return ibarcode.Height; }
            set { ibarcode.Height = value; }
        }

        public int? BarWidth
        {
            get { return ibarcode.BarWidth; }
            set { ibarcode.BarWidth = value; }
        }

        public double? AspectRatio
        {
            get { return ibarcode.AspectRatio; }
            set { ibarcode.AspectRatio = value; }
        } //推荐值2

        public bool IncludeLabel
        {
            get { return ibarcode.IncludeLabel; }
            set { ibarcode.IncludeLabel = value; }
        }


        public String AlternateLabel
        {
            get { return ibarcode.AlternateLabel; }
            set { ibarcode.AlternateLabel = value; }
        }

        public double EncodingTime
        {
            get { return ibarcode.EncodingTime; }
            set { ibarcode.EncodingTime = value; }
        }

        public string XML
        {
            get { return ibarcode.XML; }
        }

        public ImageFormat ImageFormat
        {
            get { return ibarcode.ImageFormat; }
            set { ibarcode.ImageFormat = value; }
        }

        public List<string> Errors
        {
            get { return ibarcode.Errors; }
        }

        public AlignmentPositions Alignment
        {
            get { return ibarcode.Alignment; }
            set { ibarcode.Alignment = value; }
        }

        public byte[] EncodedImageBytes
        {
            get
            {
                return ibarcode.Encoded_Image_Bytes;
            }
        }

        #endregion

        public void SaveImage(string fileName, ImageFormat imageFormat)
        {
            ibarcode.SaveImage(fileName, imageFormat);
        }
        public Image Encode(TYPE iType, string stringToEncode)
        {
            try
            {
                return ibarcode.Encode(iType, stringToEncode, Width, Height);
            }
            catch (Exception e)
            {
                Errors.Clear();
                Errors.Add(e.Message);
                return null;
            }
        }

        public void SetDefaultProperty()
        {
            EncodedType = TYPE.CODE128;
            ForeColor = Color.Black;
            BackColor = Color.White;
            Width = 300;
            Height = 150;
            //BarWidth = 2;
            //AspectRatio = 2;
            ImageFormat = ImageFormat.Jpeg;
            LabelFont = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
            Alignment = AlignmentPositions.CENTER;
            LabelPosition = LabelPositions.BOTTOMCENTER;
            RotateFlipType = RotateFlipType.RotateNoneFlipNone;
        }

        public void Dispose()
        {
            ibarcode.Dispose();
            ibarcode = null;
        }
    }
}
