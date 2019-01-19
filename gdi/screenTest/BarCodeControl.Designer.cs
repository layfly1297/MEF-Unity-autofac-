namespace screenTest
{
    partial class BarCodeControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BarCodeControl));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancle = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.GroupBoxBarcode = new System.Windows.Forms.GroupBox();
            this.picBoxBarCode = new System.Windows.Forms.PictureBox();
            this.buttonSureBarcode = new System.Windows.Forms.Button();
            this.checkBoxContextVisble = new System.Windows.Forms.CheckBox();
            this.textBoxContext = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxAlignment = new System.Windows.Forms.ComboBox();
            this.comboBoxBarCodeStyle = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.GroupBoxBarcode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxBarCode)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnCancle);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.GroupBoxBarcode);
            this.panel1.Controls.Add(this.buttonSureBarcode);
            this.panel1.Controls.Add(this.checkBoxContextVisble);
            this.panel1.Controls.Add(this.textBoxContext);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.comboBoxAlignment);
            this.panel1.Controls.Add(this.comboBoxBarCodeStyle);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(356, 429);
            this.panel1.TabIndex = 2;
            // 
            // btnCancle
            // 
            this.btnCancle.Location = new System.Drawing.Point(251, 193);
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.Size = new System.Drawing.Size(75, 23);
            this.btnCancle.TabIndex = 10;
            this.btnCancle.Text = "取消";
            this.btnCancle.UseVisualStyleBackColor = true;
            this.btnCancle.Click += new System.EventHandler(this.btnCancle_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(2, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(324, 18);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // GroupBoxBarcode
            // 
            this.GroupBoxBarcode.Controls.Add(this.picBoxBarCode);
            this.GroupBoxBarcode.Location = new System.Drawing.Point(3, 264);
            this.GroupBoxBarcode.Name = "GroupBoxBarcode";
            this.GroupBoxBarcode.Size = new System.Drawing.Size(323, 146);
            this.GroupBoxBarcode.TabIndex = 8;
            this.GroupBoxBarcode.TabStop = false;
            this.GroupBoxBarcode.Text = "预览";
            // 
            // picBoxBarCode
            // 
            this.picBoxBarCode.BackColor = System.Drawing.Color.White;
            this.picBoxBarCode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picBoxBarCode.Location = new System.Drawing.Point(17, 20);
            this.picBoxBarCode.Name = "picBoxBarCode";
            this.picBoxBarCode.Size = new System.Drawing.Size(294, 114);
            this.picBoxBarCode.TabIndex = 0;
            this.picBoxBarCode.TabStop = false;
            this.picBoxBarCode.Paint += new System.Windows.Forms.PaintEventHandler(this.picBoxBarCode_Paint);
            // 
            // buttonSureBarcode
            // 
            this.buttonSureBarcode.Location = new System.Drawing.Point(149, 193);
            this.buttonSureBarcode.Name = "buttonSureBarcode";
            this.buttonSureBarcode.Size = new System.Drawing.Size(75, 23);
            this.buttonSureBarcode.TabIndex = 7;
            this.buttonSureBarcode.Text = "确定";
            this.buttonSureBarcode.UseVisualStyleBackColor = true;
            this.buttonSureBarcode.Click += new System.EventHandler(this.buttonSureBarcode_Click);
            // 
            // checkBoxContextVisble
            // 
            this.checkBoxContextVisble.AutoSize = true;
            this.checkBoxContextVisble.Location = new System.Drawing.Point(20, 200);
            this.checkBoxContextVisble.Name = "checkBoxContextVisble";
            this.checkBoxContextVisble.Size = new System.Drawing.Size(96, 16);
            this.checkBoxContextVisble.TabIndex = 6;
            this.checkBoxContextVisble.Text = "是否显示内容";
            this.checkBoxContextVisble.UseVisualStyleBackColor = true;
            this.checkBoxContextVisble.CheckedChanged += new System.EventHandler(this.comboBoxBarCodeStyle_SelectedIndexChanged);
            // 
            // textBoxContext
            // 
            this.textBoxContext.Location = new System.Drawing.Point(89, 135);
            this.textBoxContext.Name = "textBoxContext";
            this.textBoxContext.Size = new System.Drawing.Size(196, 21);
            this.textBoxContext.TabIndex = 5;
            this.textBoxContext.Text = "123456";
            this.textBoxContext.TextChanged += new System.EventHandler(this.comboBoxBarCodeStyle_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 140);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "文本内容：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "对齐方式：";
            // 
            // comboBoxAlignment
            // 
            this.comboBoxAlignment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAlignment.FormattingEnabled = true;
            this.comboBoxAlignment.Items.AddRange(new object[] {
            "左对齐",
            "右对齐",
            "居中"});
            this.comboBoxAlignment.Location = new System.Drawing.Point(89, 80);
            this.comboBoxAlignment.Name = "comboBoxAlignment";
            this.comboBoxAlignment.Size = new System.Drawing.Size(196, 20);
            this.comboBoxAlignment.TabIndex = 2;
            this.comboBoxAlignment.SelectedIndexChanged += new System.EventHandler(this.comboBoxBarCodeStyle_SelectedIndexChanged);
            // 
            // comboBoxBarCodeStyle
            // 
            this.comboBoxBarCodeStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBarCodeStyle.FormattingEnabled = true;
            this.comboBoxBarCodeStyle.Location = new System.Drawing.Point(89, 27);
            this.comboBoxBarCodeStyle.Name = "comboBoxBarCodeStyle";
            this.comboBoxBarCodeStyle.Size = new System.Drawing.Size(196, 20);
            this.comboBoxBarCodeStyle.TabIndex = 1;
            this.comboBoxBarCodeStyle.SelectedIndexChanged += new System.EventHandler(this.comboBoxBarCodeStyle_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "条码样式：";
            // 
            // BarCodeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "BarCodeControl";
            this.Size = new System.Drawing.Size(356, 429);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.GroupBoxBarcode.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picBoxBarCode)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox GroupBoxBarcode;
        private System.Windows.Forms.PictureBox picBoxBarCode;
        private System.Windows.Forms.Button buttonSureBarcode;
        private System.Windows.Forms.CheckBox checkBoxContextVisble;
        private System.Windows.Forms.TextBox textBoxContext;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxAlignment;
        private System.Windows.Forms.ComboBox comboBoxBarCodeStyle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancle;
    }
}
