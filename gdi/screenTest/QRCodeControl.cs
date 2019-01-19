using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoziEditorControl.Infrastructure.QRCoder;
using screenTest.PopupControl;

namespace screenTest
{
    public partial class QRCodeControl : UserControl
    {

        public event HandledEventHandler QRCodeImageChange;

        public Image QrCodeImage =>
                pictureBoxQrPreview.BackgroundImage;
        public string ContextQRCode { get; set; }

        public int QrSize
        {
            get { return qRCodeInvoke.Size; }
            set
            {
                qRCodeInvoke.Size = value == 0 ? 20 : value / 29;
                textBoxQRcodeContext_TextChanged(null, null);
            }
        }


        QrCoderInvoke qRCodeInvoke;
        public QRCodeControl()
        {
            InitializeComponent();
            qRCodeInvoke = new QrCoderInvoke();
            comboBoxErrorCorrectionLevel.SelectedIndex = 3;
            textBoxQRcodeContext_TextChanged(null, null);
        }

        private void textBoxQRcodeContext_TextChanged(object sender, EventArgs e)
        {
            if (textBoxQRcodeContext.Text != null)
            {
                pictureBoxQrPreview.BackgroundImage = qRCodeInvoke.CreateImgFile(textBoxQRcodeContext.Text, comboBoxErrorCorrectionLevel.SelectedIndex);
                this.pictureBoxQrPreview.Size = new System.Drawing.Size(pictureBoxQrPreview.Width, pictureBoxQrPreview.Height);
                this.pictureBoxQrPreview.SizeMode = PictureBoxSizeMode.CenterImage;
                pictureBoxQrPreview.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void buttonQRCodeSave_Click(object sender, EventArgs e)
        {
            ContextQRCode = textBoxQRcodeContext.Text;
            var popup = this.Parent as Popup;
            popup?.Close();
            QRCodeImageChange?.Invoke(this, null);
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            ContextQRCode = null;
            var popup = this.Parent as Popup;
            popup?.Close();
            QRCodeImageChange?.Invoke(this, null);
        }
    }

    internal class QrCoderInvoke
    {

        public QrCoderInvoke()
        {
            Size = 20;
        }
        public int Size { get; set; }

        /// <summary>
        /// 创建二维码
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="errorCorrectionLevel">容错级别 0，1 ，2，3 L:7%的字码容错率 、M:15% 、Q：25% 、H:30% 默认位最高级</param>
        /// <returns></returns>
        public Image CreateImgFile(string content, int errorCorrectionLevel = 3)
        {
            Image reslut = null;
            QrCodeGenerator.ECCLevel eccLevel = (QrCodeGenerator.ECCLevel)(errorCorrectionLevel);
            using (QrCodeGenerator qrGenerator = new QrCodeGenerator())
            {
                using (QrCodeData qrCodeData = qrGenerator.CreateQrCode(content, eccLevel))
                {
                    using (QrCode qrCode = new QrCode(qrCodeData))
                    {

                        reslut = qrCode.GetGraphic(Size, Color.Black, Color.White, null, 15, 6, true);
                    }
                }
            }
            return reslut;
        }


        /// <summary>
        /// 创建带有图像的二维码
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="logo">logo图像</param>
        /// <param name="errorCorrectionLevel">容错级别 0，1 ，2，3 L:7%的字码容错率 、M:15% 、Q：25% 、H:30% 默认位最高级</param>
        /// <returns></returns>
        public Image CreateQrCodeWithLogo(string content, Image logo, int errorCorrectionLevel = 3)
        {
            Image reslut = null;
            QrCodeGenerator.ECCLevel eccLevel = (QrCodeGenerator.ECCLevel)(errorCorrectionLevel);
            using (QrCodeGenerator qrGenerator = new QrCodeGenerator())
            {
                using (QrCodeData qrCodeData = qrGenerator.CreateQrCode(content, eccLevel))
                {
                    using (QrCode qrCode = new QrCode(qrCodeData))
                    {
                        //如果需要留白把false 修改位true
                        reslut = qrCode.GetGraphic(Size, Color.Black, Color.White, (Bitmap)logo, 15, 6, false);
                    }
                }
            }
            return reslut;
        }

    }
}
