using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using screenTest.PopupControl;

namespace screenTest
{
    //下拉复选框
    public partial class MultiSelectModel : UserControl
    {
     
        #region Private Field
        //最近索引
        private int lastIndex = -1;

        //当前数据源类型
        private ListSourceType currentListSourceType;

        //单选设置
        private const int NumMax = 1;

        //上一个索引
        private int beforeindex = 0;

        #endregion

        #region Public Event
        [Description("选择事件完成")]
        public event EventHandler SelectComplete;
        #endregion

        #region Public Property 

        [Description("单选多选默认false 多选 ,true 多选")]
        public bool OneSelect { get; set; }

        [Description("文本框中是否显示选项内容，true 显示，反之 false, 默认不使用，场景不多")]
        public bool TextBoxDispalySelectContext { get; set; }

        [Description("输出内容")]
        public object SelectContext { get; set; }

        [Description("选项内容ListSource_Items ，使用时 SourceType=Items")]
        public List<SingelSelectDataModleItems> Items { get; set; }


        [Description("选项内容ListSource_DataSource ，使用时 SourceType=BindSouceName")]
        public SingelSelectDataModleDataSource DataSource { get; set; }


        [Description("选项内容ListSource_DataSource ，使用时 SourceType=SimpleItem ")]
        public object[] ObjSimpleItem { get; set; }


        [Description("选项内容为多选时，连接字符")]
        public char ListValueSeparatorChar { get; set; }

        #endregion

        #region Structure
        //#CCCCCC  #FFFFFF 666699
        #region 色彩配置


        void SetColor()
        {
            //1
            //this.textBox1.BackColor = ColorTranslator.FromHtml("#CCCCCC"); //Color.(#CCCCCC)
            //this.checkedListBox1.BackColor = ColorTranslator.FromHtml("#FFFFFF");
            //this.panel1.BackColor = ColorTranslator.FromHtml("#666699");
            //this.BackColor = ColorTranslator.FromHtml("#666699");
            //2 黑白
            this.textBox1.BackColor = Color.FromArgb(245, 246, 235); // 
            this.checkedListBox1.BackColor = Color.FromArgb(239, 240, 220); // 
            this.panel1.BackColor = Color.FromArgb(201, 202, 187);
            this.BackColor = Color.FromArgb(201, 202, 187);

            //3 绿色和蓝色等典型
            //this.textBox1.BackColor = Color.FromArgb(86, 163, 108);
            //this.checkedListBox1.BackColor = Color.FromArgb(94, 133, 121);
            //this.panel1.BackColor = Color.FromArgb(119, 195, 79);
            //this.BackColor = Color.FromArgb(119, 195, 79);
            //this.button1.BackColor = Color.FromArgb(126, 136, 79);
            //this.button2.BackColor = Color.FromArgb(126, 136, 79);
            //this.button1.FlatAppearance.BorderSize = 0; 
            //this.button1.FlatStyle = FlatStyle.Flat;
            //this.button2.FlatAppearance.BorderSize = 0;
            //this.button2.FlatStyle = FlatStyle.Flat;
            //this.button1.ForeColor = Color.White;
            //this.button2.ForeColor = Color.White;
            //Winform的话，设置FlatStyle为Flat，并且设置FlatAppearance下的BorderSize为0.
        }
        #endregion

        public MultiSelectModel()
        {
            ListValueSeparatorChar = '&';
            this.ResizeRedraw = true;
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲 
            InitializeComponent();
            SetColor();
            this.Padding = new Padding(0, 0, 1, 1);
            OneSelect = false;
            this.checkedListBox1.Width = this.textBox1.Width = this.Width;
            this.checkedListBox1.Margin = new Padding(0);
            this.textBox1.Margin = new Padding(0);
        }
        #endregion

        #region Public Method
        /// <summary>
        /// 数据源
        /// </summary>
        public void SetDataSource(ListSourceType type)
        {
            currentListSourceType = type;
            if (checkedListBox1 == null) return;
            switch (type)
            {
                case ListSourceType.Items:
                    if (Items == null || Items.Count <= 0) return;
                    checkedListBox1.BeginUpdate();
                    checkedListBox1.DataSource = Items;
                    checkedListBox1.ValueMember = "Value";
                    checkedListBox1.DisplayMember = "DisableText";
                    checkedListBox1.EndUpdate();
                    if (OneSelect)
                    {
                        SelectContext = new SingelSelectDataModleItems();
                    }
                    else
                    {
                        SelectContext = new List<SingelSelectDataModleItems>();
                    }
                    break;
                case ListSourceType.BindSouceName:
                    if (DataSource == null) return;
                    checkedListBox1.BeginUpdate();
                    checkedListBox1.DataSource = DataSource.DataSource;
                    checkedListBox1.ValueMember = DataSource.ValueMember;
                    checkedListBox1.DisplayMember = DataSource.DisplayMember;
                    checkedListBox1.EndUpdate();
                    SelectContext = OneSelect ? new object() : new List<object>();
                    break;
                case ListSourceType.SimpleItem:
                    if (ObjSimpleItem == null) return;
                    checkedListBox1.Items.Clear();
                    checkedListBox1.Items.AddRange(ObjSimpleItem);
                    SelectContext = "";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        #endregion

        #region Private Method
        //设置显示只针对简单项使用文本内容
        private void OneSelectSetTextBoxText()
        {
            if (checkedListBox1 == null) return;
            switch (currentListSourceType)
            {
                case ListSourceType.Items:
                    if (Items == null || Items.Count <= 0) return;
                    textBox1.Text = ((SingelSelectDataModleItems)SelectContext).DisableText;
                    break;
                case ListSourceType.BindSouceName:
                    if (DataSource == null) return;
                    //textBox1.Text = ((SingelSelectDataModleItems)SelectContext).DisableText;
                    break;
                case ListSourceType.SimpleItem:
                    if (ObjSimpleItem == null) return;
                    textBox1.Text = SelectContext.ToString();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currentListSourceType), currentListSourceType, null);
            }

        }

        //移动到项时 绘制边框样式
        private void checkedListBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender == null) return;
            var checkedlstbox = ((CheckedListBox)sender);
            //获取鼠标停留上的项索引
            var index = checkedlstbox.IndexFromPoint(e.Location);
            if (index == -1 || index == lastIndex) return;
            var rec = checkedlstbox.GetItemRectangle(index);
            lastIndex = index;
            checkedlstbox.Refresh();
            var graphics = checkedlstbox.CreateGraphics();
            ControlPaint.DrawBorder(graphics, rec, Color.LightBlue, ButtonBorderStyle.Solid);
            graphics.Dispose();

        }

        //选定位多选
        private void button1_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            if (btn.Text == @"取消")
            {
                SelectContext = null;
            }
            SelectComplete?.Invoke(SelectContext, new EventArgsListSourceType { CurrentType = currentListSourceType, OneSelect = this.OneSelect });
            var popup = this.Parent as Popup;
            popup?.Close();
        }

        //点击选项事件
        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (OneSelect)
            {
                OneItemCheck(sender, e);
            }
            else
            {
                MultipleItemCheck(sender, e);
            }
        }


        #region 拆分方法体checkedListBox1_ItemCheck

        /// <summary>
        /// 单选项操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OneItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.CurrentValue == CheckState.Checked)
            {
                if (TextBoxDispalySelectContext)
                {
                    textBox1.Text = SelectContext.ToString();
                }
                return;
            }
            int truecount = 0;
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemChecked(i))
                {
                    truecount++;
                }
            }
            if (truecount >= NumMax)//判断当前选项是否超出限制范围
            {
                ((CheckedListBox)sender).SetItemChecked(beforeindex, false);
            }
            beforeindex = e.Index;//记住前一次选择的索引值
            e.NewValue = CheckState.Checked;
            SelectContext = checkedListBox1.Items[e.Index];
            if (TextBoxDispalySelectContext)
            {
                OneSelectSetTextBoxText();
            }

        }

        //多选项 //只有简单项时才会有分割字符
        private void MultipleItemCheck(object sender, ItemCheckEventArgs e)
        {
            var item = checkedListBox1.Items[e.Index];
            switch (currentListSourceType)
            {

                case ListSourceType.SimpleItem:
                    if (!checkedListBox1.GetItemChecked(e.Index))
                    {
                        SelectContext += (string.IsNullOrEmpty(SelectContext.ToString())
                            ? $"{item}"
                            : $"{ListValueSeparatorChar}{item}");
                        if (TextBoxDispalySelectContext)
                        {
                            textBox1.Text = SelectContext.ToString();
                        }
                    }
                    else
                    {
                        if (SelectContext.ToString().Contains(item.ToString()))
                        {
                            SelectContext = SelectContext.ToString().Replace($"{ListValueSeparatorChar}{item}", "");
                            SelectContext = SelectContext.ToString().Replace($"{item}", "");
                            if ((!(string.IsNullOrEmpty(SelectContext.ToString())) && SelectContext.ToString()[0] == ListValueSeparatorChar))
                            {
                                SelectContext = SelectContext.ToString().Remove(0, 1);
                            }
                            if (TextBoxDispalySelectContext)
                            {
                                textBox1.Text = SelectContext.ToString();
                            }
                        }
                    }
                    break;

                case ListSourceType.Items:
                    var lstSingelItems = ((List<SingelSelectDataModleItems>)SelectContext);
                    if (!checkedListBox1.GetItemChecked(e.Index))
                    { //集合添加
                        lstSingelItems.Add((SingelSelectDataModleItems)item);
                    }
                    else
                    {
                        if (lstSingelItems.Contains(item))
                        {
                            //集合中移除
                            lstSingelItems.Remove((SingelSelectDataModleItems)item);
                        }
                    }
                    break;
                case ListSourceType.BindSouceName:
                    var lstBindSouceName = ((List<object>)SelectContext);
                    if (!checkedListBox1.GetItemChecked(e.Index))
                    { //集合添加 
                        lstBindSouceName.Add(item);
                    }
                    else
                    {
                        if (lstBindSouceName.Contains(item))
                        {
                            //集合中移除
                            lstBindSouceName.Remove(item);
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion

        /// <summary>
        /// 限制输入内容为大小写字母
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkedListBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            //{
            //    e.Handled = true;
            //} 
        }

        //查找索引定位
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (checkedListBox1.Items.Count <= 0)
            {
                return;
            }
            if (TextBoxDispalySelectContext) return;
            if (textBox1.Text.TrimEnd().Length <= 0) return;
            //查找内容 
            var inde = checkedListBox1.FindStringExact(textBox1.Text);
            if (inde != -1)
            {
                checkedListBox1.SetItemChecked(inde, true);
            }
            else
            {
                inde = QueryLinkIndex(textBox1.Text);
                checkedListBox1.SetItemChecked(inde, true);
            }
            this.checkedListBox1.SelectedIndex = inde;
        }

        //根据内容值查找 、为包含根据集合中的某个字段
        private int QueryLinkIndex(string parme)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.Items[i].ToString().Contains(parme))
                {
                    return i;
                }
            }
            return 0;
        }
        #endregion

        #region protected Override 

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

            ControlPaint.DrawSizeGrip(e.Graphics, Color.LightGray, new Rectangle(panel1.Width - 10, panel1.Height - 10, 10, 10));
        }
        protected override void WndProc(ref Message m)
        {
            var popup = Parent as Popup;
            if (popup != null && popup.ProcessResizing(ref m))
            {
                return;
            }
            //if (m.Msg == 0x0014) // 禁掉清除背景消息
            //    return;
            base.WndProc(ref m);

        }
        #endregion
    }
}
