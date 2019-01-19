namespace screenTest
{
    partial class QRCodeControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnCancle = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxQRPreview = new System.Windows.Forms.GroupBox();
            this.pictureBoxQrPreview = new System.Windows.Forms.PictureBox();
            this.buttonQRCodeSave = new System.Windows.Forms.Button();
            this.textBoxQRcodeContext = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxErrorCorrectionLevel = new System.Windows.Forms.ComboBox();
            this.panel2.SuspendLayout();
            this.groupBoxQRPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxQrPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.btnCancle);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.groupBoxQRPreview);
            this.panel2.Controls.Add(this.buttonQRCodeSave);
            this.panel2.Controls.Add(this.textBoxQRcodeContext);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.comboBoxErrorCorrectionLevel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(405, 436);
            this.panel2.TabIndex = 3;
            // 
            // btnCancle
            // 
            this.btnCancle.Location = new System.Drawing.Point(316, 192);
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.Size = new System.Drawing.Size(75, 23);
            this.btnCancle.TabIndex = 11;
            this.btnCancle.Text = "取消";
            this.btnCancle.UseVisualStyleBackColor = true;
            this.btnCancle.Click += new System.EventHandler(this.btnCancle_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(344, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 16);
            this.label1.TabIndex = 10;
            this.label1.Text = "二维码";
            // 
            // groupBoxQRPreview
            // 
            this.groupBoxQRPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxQRPreview.Controls.Add(this.pictureBoxQrPreview);
            this.groupBoxQRPreview.Location = new System.Drawing.Point(10, 221);
            this.groupBoxQRPreview.Name = "groupBoxQRPreview";
            this.groupBoxQRPreview.Size = new System.Drawing.Size(384, 201);
            this.groupBoxQRPreview.TabIndex = 9;
            this.groupBoxQRPreview.TabStop = false;
            this.groupBoxQRPreview.Text = "预览";
            // 
            // pictureBoxQrPreview
            // 
            this.pictureBoxQrPreview.BackColor = System.Drawing.Color.White;
            this.pictureBoxQrPreview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBoxQrPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxQrPreview.Location = new System.Drawing.Point(3, 17);
            this.pictureBoxQrPreview.Name = "pictureBoxQrPreview";
            this.pictureBoxQrPreview.Size = new System.Drawing.Size(378, 181);
            this.pictureBoxQrPreview.TabIndex = 0;
            this.pictureBoxQrPreview.TabStop = false;
            // 
            // buttonQRCodeSave
            // 
            this.buttonQRCodeSave.Location = new System.Drawing.Point(196, 192);
            this.buttonQRCodeSave.Name = "buttonQRCodeSave";
            this.buttonQRCodeSave.Size = new System.Drawing.Size(75, 23);
            this.buttonQRCodeSave.TabIndex = 8;
            this.buttonQRCodeSave.Text = "确定";
            this.buttonQRCodeSave.UseVisualStyleBackColor = true;
            this.buttonQRCodeSave.Click += new System.EventHandler(this.buttonQRCodeSave_Click);
            // 
            // textBoxQRcodeContext
            // 
            this.textBoxQRcodeContext.AllowDrop = true;
            this.textBoxQRcodeContext.Location = new System.Drawing.Point(105, 73);
            this.textBoxQRcodeContext.Multiline = true;
            this.textBoxQRcodeContext.Name = "textBoxQRcodeContext";
            this.textBoxQRcodeContext.Size = new System.Drawing.Size(224, 98);
            this.textBoxQRcodeContext.TabIndex = 7;
            this.textBoxQRcodeContext.Text = "美智医疗";
            this.textBoxQRcodeContext.TextChanged += new System.EventHandler(this.textBoxQRcodeContext_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(34, 87);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 6;
            this.label5.Text = "文本内容：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(34, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "容错级：";
            // 
            // comboBoxErrorCorrectionLevel
            // 
            this.comboBoxErrorCorrectionLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxErrorCorrectionLevel.FormattingEnabled = true;
            this.comboBoxErrorCorrectionLevel.Items.AddRange(new object[] {
            "L",
            "M",
            "Q",
            "H"});
            this.comboBoxErrorCorrectionLevel.Location = new System.Drawing.Point(105, 23);
            this.comboBoxErrorCorrectionLevel.Name = "comboBoxErrorCorrectionLevel";
            this.comboBoxErrorCorrectionLevel.Size = new System.Drawing.Size(224, 20);
            this.comboBoxErrorCorrectionLevel.TabIndex = 4;
            this.comboBoxErrorCorrectionLevel.SelectedIndexChanged += new System.EventHandler(this.textBoxQRcodeContext_TextChanged);
            // 
            // QRCodeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Name = "QRCodeControl";
            this.Size = new System.Drawing.Size(405, 436);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBoxQRPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxQrPreview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxQRPreview;
        private System.Windows.Forms.PictureBox pictureBoxQrPreview;
        private System.Windows.Forms.Button buttonQRCodeSave;
        private System.Windows.Forms.TextBox textBoxQRcodeContext;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxErrorCorrectionLevel;
        private System.Windows.Forms.Button btnCancle;
    }
}
