using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using screenTest.PopupControl;
using DrawEverything;
using DrawEverything.Command;

namespace screenTest
{
    /// <summary>
    /// 容器元素
    /// </summary>
    public partial class CustomControlContainer : Control
    {
        /// <summary>
        /// 如果为false，则尚未完成该工具
        /// </summary>
        public Boolean IsComplete;

        private BufferedGraphics _bufferedGraphics;
        private BufferedGraphicsContext _bufferedGraphicsContext;
        private DrawBase currentbase;
        private readonly UndoRedo _undoRedo;
        private readonly Win32Caret caret;
        private int MoveHandNum = -1;
        private bool IsChangeSize; 
        private readonly ArrayList _inMemoryList;

        private bool _isCut;

        //组集合
        private readonly List<KeyValuePair<string, DrawRadioBox>> _listRadioGroup = new List<KeyValuePair<string, DrawRadioBox>>();

        public CustomControlContainer()
        {
            InitializeComponent();

            caret = new Win32Caret(this);

            this.AllowDrop = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            //this.DoubleBuffered = true;
            //SetStyle(ControlStyles.ResizeRedraw, true);
            //SetStyle(ControlStyles.Selectable, true);
            SetBufferedGraphics();
            //elementList = new List<object>();
            elementList = new ArrayList();
            _undoRedo = new UndoRedo();
            _inMemoryList = new ArrayList();
            //添加1
            DrawRectangleBase recbase = DrawRectangleBase.Create(new Rectangle(0, 0, 100, 100), "ele1");
            recbase.BackGroundDescriptText = "条码";
            recbase.Stroke = Color.White;
            recbase.Thick = 1;
            recbase.Fill = Color.Red;
            elementList.Add(recbase);


            //2
            //DrawImage imag = DrawImage.Create(new DataStructrueImage()
            //{
            //    Href = @"C:\Users\lican\Desktop\screenTest\bin\Debug\ioc.png",
            //    X = 50,
            //    Y = 50,
            //    Width = 100,
            //    Height = 100,
            //});
            //elementList.Add(imag);

            //3

            DrawTextBlock textblock = DrawTextBlock.Create(new DataStructureTextBlock
            {
                Alignment = StringAlignment.Near,
                BackColor = Color.White,
                Height = 30,
                Id = "ad",
                Text = "特效文字",
                TextBlockFont = new Font("宋体", 14),
                Width = 100,
                X = 20,
                Y = 300
            });
            elementList.Add(textblock);

            //4

            //DrawRadioBox radioBox = DrawRadioBox.Create(120, 150, 100, new Font("宋体", 14).GetHeight(), new Font("宋体", 14), "车市1", Properties.Resources.Checkbox,
            //    Properties.Resources.unchebox);
            //radioBox.Stroke = Color.White;
            //radioBox.GroupName = groupName;
            //elementList.Add(radioBox);

            //添加5
            DrawRectangleBase qrDraw = DrawRectangleBase.Create(new Rectangle(200, 0, 100, 100), "qrcode");
            qrDraw.BackGroundDescriptText = "二维码";
            qrDraw.Stroke = Color.Transparent;
            qrDraw.Thick = 1;
            qrDraw.Fill = Color.LightGray;
            elementList.Add(qrDraw);


            //添加多项单选框 根据逻辑实现
            var groupName = "RadioGroup1";
            DrawRadioBox radioBox1 = DrawRadioBox.Create(30, 150, 40, new Font("宋体", 14).GetHeight(), new Font("宋体", 14), "男", Properties.Resources.Checkbox,
                Properties.Resources.unchebox);
            radioBox1.Stroke = Color.White;
            radioBox1.GroupName = groupName;
            radioBox1.Id = 0;
            elementList.Add(radioBox1);

            DrawRadioBox radioBox2 = DrawRadioBox.Create(80, 150, 40, new Font("宋体", 14).GetHeight(), new Font("宋体", 14), "女", Properties.Resources.Checkbox,
                Properties.Resources.unchebox);
            radioBox2.Stroke = Color.White;
            radioBox2.GroupName = groupName;
            radioBox2.Id = 1;
            elementList.Add(radioBox2);

            DrawRadioBox radioBox = DrawRadioBox.Create(120, 150, 100, new Font("宋体", 14).GetHeight(), new Font("宋体", 14), "特殊", Properties.Resources.Checkbox,
                Properties.Resources.unchebox);
            radioBox.Stroke = Color.White;
            radioBox.GroupName = groupName;
            elementList.Add(radioBox);
            _listRadioGroup.Add(new KeyValuePair<string, DrawRadioBox>(groupName, radioBox));
            _listRadioGroup.Add(new KeyValuePair<string, DrawRadioBox>(groupName, radioBox1));
            _listRadioGroup.Add(new KeyValuePair<string, DrawRadioBox>(groupName, radioBox2));
        }

        public void AddElement(DrawBase item)
        {
            elementList?.Add(item);
            AddCommand cmd = new AddCommand(elementList, item);
            _undoRedo.AddCommand(cmd);
            this.Invalidate();
        }

        public void AddElement(List<DrawBase> items)
        {
            elementList?.Add(items);
            this.Invalidate();
        }

        public Point CurrentFocus => lastPoint;
        private void SetBufferedGraphics()
        {
            //Screen.GetWorkingArea
            int w = this.Width, h = this.Height;
            if (w == 0)
            {
                w = Screen.GetWorkingArea(Control.MousePosition).Width;
            }
            if (h == 0)
            {
                h = Screen.GetWorkingArea(Control.MousePosition).Height;
            }
            _bufferedGraphicsContext = BufferedGraphicsManager.Current;
            _bufferedGraphicsContext.MaximumBuffer = new Size(h, h);
            _bufferedGraphics = _bufferedGraphicsContext.Allocate(this.CreateGraphics(),
                new Rectangle(0, 0, w, h));
        }

        /// <summary>
        /// 当前点的第一个对象
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool PointOnElement(Point p)
        {
            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //sw.Start(); 
            foreach (var t in elementList)
            {
                var obj = (DrawBase)t;
                var result = obj.HitTest(p);
                if (result == -1) { continue; }
                currentbase = obj;
                currentbase.Selected = true;
                return true;
            }

            //sw.Stop();
            //TimeSpan ts2 = sw.Elapsed;
            //Console.WriteLine("Stopwatch For总共花费{0}ms.", ts2.TotalMilliseconds);
            //查找所有 lambda 效率没有循环块
            //var hitItems = elementList.Where(predicate: t => ((DrawBase)t).HitTest(p) > -1).ToArray();
            //查找一个
            //var hitItems = elementList.First(predicate: t => ((DrawBase)t).HitTest(p) > -1); 
            return false;
        }
        public DrawBase GetFirstSelected()
        {
            foreach (DrawBase o in elementList)
            {
                if (o.Selected)
                    return o;
            }
            return null;
        }

        #region methon 赋值 粘贴 剪切
        /// <summary>
        /// Clear all objects in the list
        /// </summary>
        /// <returns>
        /// true if at least one object is deleted
        /// </returns>
        public bool Clear()
        {
            bool result = (elementList.Count > 0);
            elementList.Clear();
            return result;
        }

        /// <summary>
        /// Cut selected items
        /// </summary>
        /// <returns>
        /// true if at least one object is cut
        /// </returns>
        public void CutSelection()
        {
            int i;
            int n = elementList.Count;
            _inMemoryList.Clear();
            for (i = n - 1; i >= 0; i--)
            {
                if (((DrawBase)elementList[i]).Selected)
                {
                    _inMemoryList.Add(elementList[i]);
                }
            }
            _isCut = true;

            var cmd = new CutCommand(elementList, _inMemoryList);
            cmd.Execute();
            _undoRedo.AddCommand(cmd);
        }

        /// <summary>
        /// Copies selected items
        /// </summary>
        /// <returns>
        /// true if at least one object is copied
        /// </returns>
        public bool CopySelection()
        {
            bool result = false;
            int n = elementList.Count;

            for (int i = n - 1; i >= 0; i--)
            {
                if (((DrawBase)elementList[i]).Selected)
                {
                    _inMemoryList.Clear();
                    _inMemoryList.Add(elementList[i]);
                    result = true;
                    _isCut = false;
                }
            }

            return result;
        }


        /// <summary>
        /// Paste selected items
        /// </summary>
        /// <returns>
        /// true if at least one object is pasted
        /// </returns>
        public void PasteSelection()
        {
            int n = _inMemoryList.Count;

            UnselectAll();

            if (n > 0)
            {
                var tempList = new ArrayList();

                int i;
                for (i = n - 1; i >= 0; i--)
                {
                    tempList.Add(((DrawBase)_inMemoryList[i]).Clone());
                }

                if (_inMemoryList.Count > 0)
                {
                    var cmd = new PasteCommand(elementList, tempList);
                    cmd.Execute();
                    _undoRedo.AddCommand(cmd);

                    //If the items are cut, we will not delete it
                    if (_isCut)
                        _inMemoryList.Clear();
                }
            }
        }
        #endregion
        #region  override

        //绘制
        protected override void OnPaint(PaintEventArgs pe)
        {
            IsComplete = false;
            base.OnPaint(pe);
            _bufferedGraphics.Render(pe.Graphics);
            IsComplete = true;
        }

        public void UnselectAll()
        {
            foreach (DrawBase o in elementList)
            {
                o.Selected = false;
            }
        }

        //鼠标按下
        protected override void OnMouseDown(MouseEventArgs e)
        {

            lastPoint = e.Location;
            UnselectAll();
            if (!PointOnElement(lastPoint))
            {
                currentbase = null;
                #region 光标
                caret.Destroy();
                caret.Create(2, this.Font.Height);
                caret.SetPosition(e.X, e.Y);
                caret.Show();
                #endregion
            }
            else
            {
                caret.Hide();
                this.Invalidate();
            }
            this.Invalidate();
            this.Focus();
            base.OnMouseDown(e);
        }
        //鼠标移动
        protected override void OnMouseMove(MouseEventArgs e)
        {

            if (MouseButtons == MouseButtons.Left)
            {
                //foreach (var t in elementList)
                //{
                //DrawBase obj = (DrawBase)t;
                //if (!obj.Selected) continue;
                if (currentbase != null)
                {
                    DrawBase obj = currentbase;
                    #region ChangeSize 
                    if (IsChangeSize)
                    {
                        #region//添加undoredo 
                        var cmd = new ChangeSizeCommand(obj, lastPoint, e.Location, MoveHandNum);
                        _undoRedo.AddCommand(cmd);
                        #endregion
                        obj.MoveHandleTo(e.Location, MoveHandNum);
                    }

                    #endregion
                    #region Move
                    else
                    {
                        float dx = e.X - lastPoint.X;
                        float dy = e.Y - lastPoint.Y;
                        #region//添加undoredo
                        var movedItemsList = new ArrayList() { obj };
                        var cmd = new MoveCommand(movedItemsList, new PointF(dx, dy));
                        _undoRedo.AddCommand(cmd);
                        #endregion
                        obj.Move(dx, dy);
                    }
                    #endregion

                    lastPoint = e.Location;
                    this.Invalidate();
                    return;
                }
                //}
            }
            else
            {
                //未按鼠标按钮时设置光标
                if (e.Button == MouseButtons.None)
                {
                    IsChangeSize = false;
                    foreach (var t in elementList)
                    {
                        DrawBase obj = (DrawBase)t;
                        var handleTemp = obj.HitTest(e.Location);
                        switch (handleTemp)
                        {
                            case -1:
                                this.Cursor = Cursors.Default;
                                break;
                            case 0:
                                this.Cursor = Cursors.SizeAll; //四方向
                                return;
                            default:
                                this.Cursor = obj.GetHandleCursor(handleTemp);
                                MoveHandNum = handleTemp;
                                IsChangeSize = true;
                                return;
                        }
                    }
                }
            }
            base.OnMouseMove(e);

        }
        //鼠标抬起
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

        }

        protected override void OnClick(EventArgs e)
        {
            DrawRadioBox temp = currentbase as DrawRadioBox;
            if (temp != null)
            {
                if (!string.IsNullOrEmpty(temp.GroupName))
                {
                    if (_listRadioGroup != null && _listRadioGroup.Count > 0)
                    {
                        var radioGrp = _listRadioGroup.FindAll(p => p.Key == temp.GroupName);
                        if (temp.Checked)
                        {
                            return;
                        }
                        temp.Checked = true;
                        foreach (var item in radioGrp)
                        {
                            if (item.Value != temp)
                            {
                                item.Value.Checked = false;
                            }
                        }
                    }
                }
                else
                {
                    temp.Checked = !temp.Checked;
                }
                this.Invalidate();
            }

            base.OnClick(e);

        }

        private void Dsd_BarCodeImageChange(object sender, HandledEventArgs e)
        {
            var temp = ((BarCodeControl)sender);
            if (currentbase is DrawRectangleBase)
            {
                var elebarcode = ((DrawRectangleBase)currentbase);
                elebarcode.FillBackGroundImage = temp.BarCodeImage;
                this.Invalidate();
                temp.Dispose();
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// 获取绘制的方框。
        /// 我们选择时，可能是由左到右，由上到下，也有可能由下到上，由右到左。
        /// 为些程序需要判断一下。
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public Rectangle GetNormalizedRectangle(int x1, int y1, int x2, int y2)
        {
            if (x2 < x1)
            {
                int tmp = x2;
                x2 = x1;
                x1 = tmp;
            }
            if (y2 < y1)
            {
                int tmp = y2;
                y2 = y1;
                y1 = tmp;
            }
            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }


        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            //判断当前是否有元素
            var iszhong = false;

            if (elementList.Count > 0)
            {
                DrawBase obj = null;
                foreach (var t in elementList)
                {
                    obj = (DrawBase)t;
                    var result = obj.HitTest(e.Location);
                    if (result != -1)
                    {
                        iszhong = true;
                        break;
                    }
                }
                if (iszhong)
                {
                    if (obj is DrawTextBlock)
                    {
                        currentbase = (DrawTextBlock)obj;

                        var singleSelectModel = new SingleSelectModel
                        {
                            Height = 300,
                            Items = new List<SingelSelectDataModleItems>
                            {
                                new SingelSelectDataModleItems
                                {
                                    DisableText = "舒张早期奔马律",
                                    Value = "有舒张早期奔马律,无舒张晚期奔马律、重叠型奔马律、开瓣音、关瓣音、喀喇音、心包叩击音、肿瘤扑落音、收缩早期喷射音、收缩中晚期喀喇音",
                                    Tag = "SZZQBML"
                                },
                                new SingelSelectDataModleItems
                                {
                                    DisableText = "开瓣音",
                                    Value = "有开瓣音,无舒张早期奔马律、舒张晚期奔马律、重叠型奔马律、关瓣音、喀喇音、心包叩击音、肿瘤扑落音、收缩早期喷射音、收缩中晚期喀喇音",
                                    Tag = "KBY"
                                },
                                new SingelSelectDataModleItems
                                {
                                    DisableText = "关瓣音",
                                    Value = "有关瓣音,无舒张早期奔马律、舒张晚期奔马律、重叠型奔马律、开瓣音、喀喇音、心包叩击音、肿瘤扑落音、收缩早期喷射音、收缩中晚期喀喇音",
                                    Tag = "GBY"
                                },
                                new SingelSelectDataModleItems
                                {
                                    DisableText = "舒张晚期奔马律",
                                    Value = "有舒张晚期奔马律,无舒张早期奔马律、重叠型奔马律、开瓣音、关瓣音、喀喇音、心包叩击音、肿瘤扑落音、收缩早期喷射音、收缩中晚期喀喇音",
                                    Tag = "SZWQBML"
                                }
                            },
                            OneSelect = false
                        };

                        #region 一下为必须设置项 

                        //singleSelectModel.TextBoxDispalySelectContext = false;
                        singleSelectModel.SelectComplete += SingleSelectModel_SelectComplete;
                        singleSelectModel.SetDataSource(ListSourceType.Items);
                        #endregion

                        PopupControl.Popup pa = new PopupControl.Popup(singleSelectModel)
                        {
                            Resizable = true,
                            RenderMode = ToolStripRenderMode.Professional,
                            MinimumSize = new Size(20, 20),
                            MaximumSize = new Size(1000, 1000),
                            Height = 300
                        };
                        pa.Show(PointToScreen(e.Location));
                    }

                    if (currentbase is DrawRectangleBase)
                    {
                        var temp = (DrawRectangleBase)currentbase;
                        if (temp.Name == "ele1")
                        {
                            BarCodeControl dsd = new BarCodeControl();
                            dsd.BarHeight = temp.Rect.Height;
                            dsd.BarWidth = temp.Rect.Width;
                            dsd.BarCodeImageChange += Dsd_BarCodeImageChange; ;
                            PopupControl.Popup pa = new PopupControl.Popup(dsd)
                            {
                                Resizable = true,
                                RenderMode = ToolStripRenderMode.Professional,
                                MinimumSize = new Size(20, 20),
                                MaximumSize = new Size(1000, 1000),
                            }; pa.Show(MousePosition);
                        }

                        else if (temp.Name == "qrcode")
                        {
                            QRCodeControl dsd = new QRCodeControl();
                            dsd.QrSize = Math.Min(temp.Rect.Width, temp.Rect.Width);
                            dsd.QRCodeImageChange += Dsd_QRCodeImageChange;
                            PopupControl.Popup pa = new PopupControl.Popup(dsd)
                            {
                                Resizable = true,
                                RenderMode = ToolStripRenderMode.Professional,
                                MinimumSize = new Size(20, 20),
                                MaximumSize = new Size(1000, 1000),
                            }; pa.Show(MousePosition);
                        }

                        this.Invalidate();
                    }
                }

            }

            base.OnMouseDoubleClick(e);

        }

        private void Dsd_QRCodeImageChange(object sender, HandledEventArgs e)
        {
            var temp = ((QRCodeControl)sender);
            if (!(currentbase is DrawRectangleBase)) return;
            var elebarcode = ((DrawRectangleBase)currentbase);
            elebarcode.FillBackGroundImage = temp.QrCodeImage;
            this.Invalidate();
            temp.Dispose();
        }

        private void SingleSelectModel_SelectComplete(object sender, EventArgs e)
        {
            //在 使用控件时就已知数据对象类型 
            if (sender == null)
            {
                //取消值为空
                return;
            }
            var eExtrend = (EventArgsListSourceType)e;
            switch (eExtrend.CurrentType)
            {
                case ListSourceType.SimpleItem:
                    MessageBox.Show(sender.ToString());
                    break;
                case ListSourceType.Items:
                    if (eExtrend.OneSelect)
                    {
                        //在 使用控件时就已知数据对象类型 
                        var simpleItem = sender as SingelSelectDataModleItems;
                        MessageBox.Show(simpleItem != null
                            ? simpleItem.Value
                            : sender.ToString());
                    }
                    else
                    {
                        var simpleItem = sender as List<SingelSelectDataModleItems>;
                        if (simpleItem != null) ((DrawTextBlock)currentbase).Text = simpleItem[0].Value;
                        this.Invalidate();
                    }
                    break;
                case ListSourceType.BindSouceName:
                    if (eExtrend.OneSelect)
                    {
                        var simpleItem = sender as SingelSelectDataModleItems;
                        MessageBox.Show(simpleItem != null
                            ? simpleItem.Value
                            : sender.ToString());
                    }
                    else
                    {
                        var simpleItem = sender as List<object>;
                        MessageBox.Show(simpleItem != null
                            ? simpleItem.Count().ToString()
                            : sender.ToString());
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            OnDraw();
            base.OnInvalidated(e);
            ClearMemory();
        }
        //键盘按下
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if ((e.KeyCode == Keys.Z) && e.Control)
            {
                _undoRedo.Undo();
            }
            if ((e.KeyCode == Keys.Y) && e.Control)
            {
                _undoRedo.Redo();
            }

            if (e.KeyCode == Keys.Delete)
            {

                var cmd = new DeleteCommand(elementList);
                cmd.Execute();
                _undoRedo.AddCommand(cmd);

            }

            if ((e.KeyCode == Keys.C) && e.Control)
            {
                CopySelection();
            }

            if ((e.KeyCode == Keys.V) && e.Control)
            {
                PasteSelection();
            }

            if ((e.KeyCode == Keys.X) && e.Control)
            {
                CutSelection();
            }

            this.Invalidate();

        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            base.OnPreviewKeyDown(e);
        }
        //键盘抬起
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }

        //大小变化
        protected override void OnResize(EventArgs e)
        {
            SetBufferedGraphics();
            base.OnResize(e);
        }

        //获取焦点
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
        }

        //失去焦点
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
        }

        //位置变化
        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
        }

        #endregion

        #region Field


        private Point lastPoint;
        //大小及位置
        private Rectangle recctangle;

        //绘制是否完成
        private bool drawSuccess;

        //总元素数
        private int elementCount;

        //行开始点
        private Point startPoint;

        //行结束点
        private Point endPoint;

        //左边元素
        private object leftElement;

        //右边元素
        private object rightElement;

        //总行数
        private int rowCount;

        //第一行
        private int firstRow;

        //结束行
        private int endRow;

        //当前活动元素
        private object activateElement;

        //元素集合
        //private List<object> elementList;
        private readonly ArrayList elementList;

        //段落开头
        private Point paragraphHome;
        //段落结尾
        private Point paragraphEnd;

        #endregion

        #region Attribute 

        #endregion

        #region Draw

        //绘制
        private void OnDraw()
        {
            Graphics g = _bufferedGraphics.Graphics;
            {
                //73 90 128  背景清理
                g.Clear(Color.FromArgb(73, 90, 128));
                if (elementList.Count <= 0) return;
                elementCount = elementList.Count;
                ////按反顺序 /枚举列表，以获取顶部的第一个对象
                for (int i = elementCount - 1; i >= 0; i--)
                {
                    DrawBase obj = (DrawBase)elementList[i];
                    if (obj.Selected)
                    {
                        obj.DrawTracker(g);
                    }
                    obj.Draw(g);
                }
            }
        }

        #endregion


        #region element Operate

        //添加元素
        public void AddElement(object element)
        {

        }


        public void RemoveElement(object element)
        {

        }


        #endregion


        #region 图像缩放
        private double LogicalToDeviceUnitsScalingFactor => this.CreateGraphics().DpiX / 96.0;

        private InterpolationMode interpolationMode = InterpolationMode.Invalid;
        private InterpolationMode InterpolationMode
        {
            get
            {
                if (interpolationMode == InterpolationMode.Invalid)
                {
                    int num = (int)Math.Round(LogicalToDeviceUnitsScalingFactor * 100.0);
                    interpolationMode = num % 100 != 0 ? (num >= 100 ? InterpolationMode.HighQualityBicubic : InterpolationMode.HighQualityBilinear) : InterpolationMode.NearestNeighbor;
                }
                return interpolationMode;
            }
        }

        private Bitmap ScaleBitmapToSize(Bitmap logicalImage, Size deviceImageSize)
        {
            Bitmap bitmap = new Bitmap(deviceImageSize.Width, deviceImageSize.Height, logicalImage.PixelFormat);
            using (Graphics graphics = Graphics.FromImage((Image)bitmap))
            {
                graphics.InterpolationMode = InterpolationMode;
                RectangleF srcRect = new RectangleF(0.0f, 0.0f, (float)logicalImage.Size.Width, (float)logicalImage.Size.Height);
                RectangleF destRect = new RectangleF(0.0f, 0.0f, (float)deviceImageSize.Width, (float)deviceImageSize.Height);
                srcRect.Offset(-0.5f, -0.5f);
                graphics.DrawImage((Image)logicalImage, destRect, srcRect, GraphicsUnit.Pixel);
            }
            return bitmap;
        }

        #endregion


        #region
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        /// <summary>      
        /// 释放内存      
        /// </summary>      
        public static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }

        #endregion


        class ScaleHelper
        {

            public void Scale(float ratio)
            {
                this.ScaleCore(ratio, ratio);
            }

            /// <summary>缩放整个控件和任何子控件。</summary>
            /// <param name="dx">水平比例因子。</param>
            /// <param name="dy">垂直比例因子。</param> 
            public void Scale(float dx, float dy)
            {
                //临时挂起布局
                try
                {
                    this.ScaleCore(dx, dy);
                }
                finally
                {
                    //恢复布局
                }
            }

            /// <summary>按指定的比例因子缩放控件和所有子控件。</summary>
            /// <param name="factor">
            ///   一个包含水平和垂直比例因子的 <see cref="T:System.Drawing.SizeF" />。
            /// </param> 
            public void Scale(SizeF factor)
            {
                //using (new LayoutTransaction(this, (IArrangedElement)this, PropertyNames.Bounds, false))
                //{
                //    this.ScaleControl(factor, factor, this);
                //    if (this.ScaleChildren)
                //    {
                //        Control.ControlCollection controlCollection = (Control.ControlCollection)this.Properties.GetObject(Control.PropControlsCollection);
                //        if (controlCollection != null)
                //        {
                //            for (int index = 0; index < controlCollection.Count; ++index)
                //                controlCollection[index].Scale(factor);
                //        }
                //    }
                //}
                //LayoutTransaction.DoLayout((IArrangedElement)this, (IArrangedElement)this, PropertyNames.Bounds);
            }

            internal virtual void Scale(SizeF includedFactor, SizeF excludedFactor, Control requestingControl)
            {
                //using (new LayoutTransaction(this, (IArrangedElement)this, PropertyNames.Bounds, false))
                //{
                //    this.ScaleControl(includedFactor, excludedFactor, requestingControl);
                //    this.ScaleChildControls(includedFactor, excludedFactor, requestingControl);
                //}
                //LayoutTransaction.DoLayout((IArrangedElement)this, (IArrangedElement)this, PropertyNames.Bounds);
            }

            internal void ScaleChildControls(SizeF includedFactor, SizeF excludedFactor, Control requestingControl)
            {
                //if (!this.ScaleChildren)
                //    return;
                //Control.ControlCollection controlCollection = (Control.ControlCollection)this.Properties.GetObject(Control.PropControlsCollection);
                //if (controlCollection == null)
                //    return;
                //for (int index = 0; index < controlCollection.Count; ++index)
                //    controlCollection[index].Scale(includedFactor, excludedFactor, requestingControl);
            }

            internal void ScaleControl(SizeF includedFactor, SizeF excludedFactor, Control requestingControl)
            {
                try
                {
                    //this.IsCurrentlyBeingScaled = true;
                    BoundsSpecified specified1 = BoundsSpecified.None;
                    BoundsSpecified specified2 = BoundsSpecified.None;
                    if (!includedFactor.IsEmpty)
                        //specified1 = this.RequiredScaling;
                        if (!excludedFactor.IsEmpty)
                            //specified2 |= ~this.RequiredScaling & BoundsSpecified.All;
                            if (specified1 != BoundsSpecified.None)
                                this.ScaleControl(includedFactor, specified1);
                    if (specified2 != BoundsSpecified.None)
                        this.ScaleControl(excludedFactor, specified2);
                    if (includedFactor.IsEmpty)
                        return;
                    //this.RequiredScaling = BoundsSpecified.None;
                }
                finally
                {
                    //this.IsCurrentlyBeingScaled = false;
                }
            }

            /// <summary>缩放控件的位置、大小、空白和边距。</summary>
            /// <param name="factor">控件高度和宽度的缩放因子。</param>
            /// <param name="specified">
            ///   一个 <see cref="T:System.Windows.Forms.BoundsSpecified" /> 值，指定在定义控件的大小和位置时要使用的控件边界。
            /// </param> 
            protected virtual void ScaleControl(SizeF factor, BoundsSpecified specified)
            {
                //CreateParams createParams = this.CreateParams;
                //NativeMethods.RECT rect = new NativeMethods.RECT(0, 0, 0, 0);
                //this.AdjustWindowRectEx(ref rect, createParams.Style, this.HasMenu, createParams.ExStyle);
                //Size b1 = this.MinimumSize;
                //Size size1 = this.MaximumSize;
                //this.MinimumSize = Size.Empty;
                //this.MaximumSize = Size.Empty;
                //Rectangle scaledBounds = this.GetScaledBounds(this.Bounds, factor, specified);
                //float width = factor.Width;
                //float height = factor.Height;
                //Padding padding = this.Padding;
                //Padding margin = this.Margin;
                //if ((double)width == 1.0)
                //    specified &= ~(BoundsSpecified.X | BoundsSpecified.Width);
                //if ((double)height == 1.0)
                //    specified &= ~(BoundsSpecified.Y | BoundsSpecified.Height);
                //if ((double)width != 1.0)
                //{
                //    padding.Left = (int)Math.Round((double)padding.Left * (double)width);
                //    padding.Right = (int)Math.Round((double)padding.Right * (double)width);
                //    margin.Left = (int)Math.Round((double)margin.Left * (double)width);
                //    margin.Right = (int)Math.Round((double)margin.Right * (double)width);
                //}
                //if ((double)height != 1.0)
                //{
                //    padding.Top = (int)Math.Round((double)padding.Top * (double)height);
                //    padding.Bottom = (int)Math.Round((double)padding.Bottom * (double)height);
                //    margin.Top = (int)Math.Round((double)margin.Top * (double)height);
                //    margin.Bottom = (int)Math.Round((double)margin.Bottom * (double)height);
                //}
                //this.Padding = padding;
                //this.Margin = margin;
                //Size size2 = rect.Size;
                //if (!b1.IsEmpty)
                //{
                //    b1 -= size2;
                //    b1 = this.ScaleSize(LayoutUtils.UnionSizes(Size.Empty, b1), factor.Width, factor.Height) + size2;
                //}
                //if (!size1.IsEmpty)
                //{
                //    Size b2 = size1 - size2;
                //    size1 = this.ScaleSize(LayoutUtils.UnionSizes(Size.Empty, b2), factor.Width, factor.Height) + size2;
                //}
                //Size unbounded = LayoutUtils.ConvertZeroToUnbounded(size1);
                //Size size3 = LayoutUtils.UnionSizes(LayoutUtils.IntersectSizes(scaledBounds.Size, unbounded), b1);
                //if (DpiHelper.EnableAnchorLayoutHighDpiImprovements && this.ParentInternal != null && this.ParentInternal.LayoutEngine == DefaultLayout.Instance)
                //    DefaultLayout.ScaleAnchorInfo((IArrangedElement)this, factor);
                //this.SetBoundsCore(scaledBounds.X, scaledBounds.Y, size3.Width, size3.Height, BoundsSpecified.All);
                //this.MaximumSize = size1;
                //this.MinimumSize = b1;
            }

            /// <summary>此方法与此类无关。</summary>
            /// <param name="dx">水平比例因子。</param>
            /// <param name="dy">垂直比例因子。</param>
            [EditorBrowsable(EditorBrowsableState.Never)]
            protected virtual void ScaleCore(float dx, float dy)
            {
                //this.SuspendLayout();
                try
                {
                    //int x = (int)Math.Round((double)this.x * (double)dx);
                    //int y = (int)Math.Round((double)this.y * (double)dy);
                    //int width = this.width;
                    //if ((this.controlStyle & ControlStyles.FixedWidth) != ControlStyles.FixedWidth)
                    //    width = (int)Math.Round((double)(this.x + this.width) * (double)dx) - x;
                    //int height = this.height;
                    //if ((this.controlStyle & ControlStyles.FixedHeight) != ControlStyles.FixedHeight)
                    //    height = (int)Math.Round((double)(this.y + this.height) * (double)dy) - y;
                    //this.SetBounds(x, y, width, height, BoundsSpecified.All);
                    //Control.ControlCollection controlCollection = (Control.ControlCollection)this.Properties.GetObject(Control.PropControlsCollection);
                    //if (controlCollection == null)
                    //    return;
                    //for (int index = 0; index < controlCollection.Count; ++index)
                    //    controlCollection[index].Scale(dx, dy);
                }
                finally
                {
                    //this.ResumeLayout();
                }
            }

            /// <summary>
            /// 缩放大小
            /// </summary>
            /// <param name="startSize">起始大小</param>
            /// <param name="x">x缩放因子</param>
            /// <param name="y">y缩放因子</param>
            /// <returns></returns>
            internal Size ScaleSize(Size startSize, float x, float y)
            {
                Size size = startSize;
                ////if (!this.GetStyle(ControlStyles.FixedWidth))
                size.Width = (int)Math.Round((double)size.Width * (double)x);
                //if (!this.GetStyle(ControlStyles.FixedHeight))
                size.Height = (int)Math.Round((double)size.Height * (double)y);
                return size;
            }

            //缩放比例 = old高/old宽=new高/new 宽
        }
    }

}
