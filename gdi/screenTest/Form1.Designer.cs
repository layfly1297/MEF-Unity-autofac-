namespace screenTest
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.customControl2OneNodeEditBoxcs1 = new screenTest.CustomControl2OneNodeEditBoxcs();
            this.SuspendLayout();
            // 
            // customControl2OneNodeEditBoxcs1
            // 
            this.customControl2OneNodeEditBoxcs1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customControl2OneNodeEditBoxcs1.Location = new System.Drawing.Point(0, 0);
            this.customControl2OneNodeEditBoxcs1.MinimumSize = new System.Drawing.Size(125, 125);
            this.customControl2OneNodeEditBoxcs1.Name = "customControl2OneNodeEditBoxcs1";
            this.customControl2OneNodeEditBoxcs1.Size = new System.Drawing.Size(1145, 732);
            this.customControl2OneNodeEditBoxcs1.TabIndex = 0;
            this.customControl2OneNodeEditBoxcs1.Text = "customControl2OneNodeEditBoxcs1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1145, 732);
            this.Controls.Add(this.customControl2OneNodeEditBoxcs1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private CustomControl2OneNodeEditBoxcs customControl2OneNodeEditBoxcs1;
    }
}