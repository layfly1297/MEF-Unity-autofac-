using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;

namespace DrawEverything
{
    /// <inheritdoc />
    /// <summary>
    /// 抽象基类 
    /// </summary>
    public abstract class DrawBase : ICloneable, IDisposable
    {
        #region Staric

        /// <summary>
        /// 像素点
        /// </summary>
        public static PointF Dpi;
        //对象ID 
        public static int ObjectId;

        #endregion

        #region Properties

        /// <summary>
        /// 是否击中周围
        /// </summary>
        [Browsable(false)]
        public bool HitOnCircumferance { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        [Browsable(false)]
        public int Id { get; set; }

        /// <summary>
        /// 是否选择
        /// </summary>
        [Browsable(false)]
        public bool Selected { get; set; }

        /// <summary>
        ///背景颜色
        /// </summary>
        public Color Fill { get; set; }

        /// <summary>
        /// 画笔颜色
        /// </summary>
        public Color Stroke { get; set; }

        /// <summary>
        /// 画笔宽度
        /// </summary>
        [Browsable(false)]
        protected float StrokeWidth { get; set; }

        /// <summary>
        /// 刻度值
        /// </summary>
        public int Thick
        {
            get
            {
                return (int)(StrokeWidth / Zoom);
            }
            set
            {
                StrokeWidth = (int)(value * Zoom);
            }
        }

        public static float Zoom = 1;

        /// <summary>
        ///手柄数 /瞄点
        /// </summary>
        [Browsable(false)]
        public virtual int HandleCount => 0;

        /// <summary>
        /// 最近使用的颜色
        /// </summary>
        public static Color LastUsedColor { get; set; }

        /// <summary>
        /// 最近使用的画笔宽度
        /// </summary>
        public static float LastUsedPenWidth { get; set; }

        //名称
        public string Name { get; set; }

        #endregion

        #region Virtual Functions

        /// <summary>
        /// Draw object
        /// </summary>
        /// <param name="g"></param>
        public virtual void Draw(Graphics g)
        {
        }

        protected DrawBase()
        {
            Name = "";
            Fill = Color.Empty;
            Id = 0;
            SetId();
        }

        static DrawBase()
        {
            LastUsedPenWidth = 1;
            LastUsedColor = Color.Black;
        }

        private void SetId()
        {
            Id = ObjectId++;
        }

        /// <summary>
        ///根据手柄索引获取位置点
        /// </summary>
        /// <param name="handleNumber"></param>
        /// <returns></returns>
        public virtual PointF GetHandle(int handleNumber)
        {
            return new PointF(0, 0);
        }

        /// <summary>
        /// 手柄点的矩形大小
        /// </summary>
        /// <param name="handleNumber"></param>
        /// <returns></returns>
        public virtual RectangleF GetHandleRectangle(int handleNumber)
        {
            var point = GetHandle(handleNumber);

            //return new RectangleF(point.X - 1f*2.45f, point.Y - 1f * 2.45f, 2f * 2.45f, 2f * 2.45f); //设置跟踪点矩形大小
            //return new RectangleF(point.X - 3, point.Y - 3, 6, 6);
            return new RectangleF(point.X - 3, point.Y - 3, 7, 7);
        }

        /// <summary>
        /// 选择对象绘画跟踪点
        /// </summary>
        /// <param name="g"></param>
        public virtual void DrawTracker(Graphics g)
        {
            if (!Selected)
                return;

            var brush = new SolidBrush(Color.LightSeaGreen);
            for (int i = 1; i <= HandleCount; i++)
            {
                try
                {
                    g.FillEllipse(brush, GetHandleRectangle(i));
                }
                catch
                {
                    // ignored
                }
            }
            brush.Dispose();
        }

        /// <summary>
        /// 是否击中测试
        /// 返回值: -1   没点击
        ///                0 - 击中内部
        ///                > 1 - 击中手柄点
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public virtual int HitTest(PointF point)
        {
            return -1;
        }


        /// <summary>
        /// 点是否在对象内
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected virtual bool PointInObject(PointF point)
        {
            return false;
        }

        /// <summary>
        /// 获取手柄点的光标
        /// </summary>
        /// <param name="handleNumber"></param>
        /// <returns></returns>
        public virtual Cursor GetHandleCursor(int handleNumber)
        {
            return Cursors.Default;
        }

        /// <summary>
		/// 获取边界手柄的裁剪器 
		/// </summary>
		/// <param name="handleNumber"></param>
		/// <returns></returns>
        public virtual Cursor GetOutlineCursor(int handleNumber)
        {
            return Cursors.Cross;
        }

        /// <summary>
        /// 对象是否与矩形相交
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public virtual bool IntersectsWith(RectangleF rectangle)
        {
            return false;
        }

        /// <summary>
        /// 移动对象
        /// </summary>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        public virtual void Move(float deltaX, float deltaY)
        {
        }

        /// <summary>
        /// 移动跟踪点坐标 改变大小
        /// </summary>
        /// <param name="point"></param>
        /// <param name="handleNumber"></param>
        public virtual void MoveHandleTo(PointF point, int handleNumber)
        {

        }

        /// <summary>
        /// 点击一个跟踪点
        /// </summary>
        /// <param name="handle"></param>
        public virtual void MouseClickOnHandle(int handle)
        {
        }

        /// <summary>
        /// 垃圾场(用于调试) 
        /// </summary>
        public virtual void Dump()
        {
            Trace.WriteLine("");
            Trace.WriteLine(GetType().Name);
            Trace.WriteLine("Selected = " + Selected.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary> 
        /// 格式化对象。在对象调整大小结束时调用此函数
        /// </summary>
        public virtual void Normalize()
        {
        }

        /// <summary>
        /// 鼠标点击边框
        /// </summary>
        public virtual void MouseClickOnBorder()
        {

        }

        /// <summary>
        /// 将对象保存到序列化流 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="scale"></param>
        public virtual void SaveToXml(XmlTextWriter writer, double scale)
        {
        }

        /// <summary>
        /// 加载序列化对象
        /// </summary>
        /// <param name="reader"></param>
        public virtual void LoadFromXml(XmlTextReader reader)
        {
        }

        #endregion

        #region Other functions

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Initialize()
        {
            Stroke = LastUsedColor;
            StrokeWidth = LastUsedPenWidth * Zoom;
        }

        public static string Color2String(Color c)
        {
            if (c.IsNamedColor)
            {
                return c.Name;
            }

            byte[] bytes = BitConverter.GetBytes(c.ToArgb());

            string sColor = "#";
            sColor += BitConverter.ToString(bytes, 2, 1);
            sColor += BitConverter.ToString(bytes, 1, 1);
            sColor += BitConverter.ToString(bytes, 0, 1);

            return sColor;
        }


        /// <summary>
        /// 大小变化
        /// </summary>
        /// <param name="newscale"></param>
        /// <param name="oldscale"></param>
        public virtual void Resize(SizeF newscale, SizeF oldscale)
        {
        }

        /// <summary>
        /// 计算原有位置点与旧矩形对应的比例,求出当前大小矩形点的位置
        /// </summary>
        /// <param name="pp"></param>
        /// <param name="newscale"></param>
        /// <param name="oldscale"></param>
        /// <returns></returns>
        public static PointF RecalcPoint(PointF pp, SizeF newscale, SizeF oldscale)
        {
            PointF p = pp;
            p.X = p.X / oldscale.Width;  //计算比例x
            p.Y = p.Y / oldscale.Height;//计算比例y
            p.X = p.X * newscale.Width; //计算出当前的x点
            p.Y = p.Y * newscale.Height; //计算出当前的y点
            return p;
        }

        /// <summary>
        /// 重合板浮点
        /// </summary>
        /// <param name="val"></param>
        /// <param name="newscale1"></param>
        /// <param name="oldscale1"></param>
        /// <returns></returns>
        public static float RecalcFloat(float val, float newscale1, float oldscale1)
        {
            val = val / oldscale1; //计算原宽度占用比例 
            val = val * newscale1;//新的乘以比例得出宽度
            return val;
        }

        /// <summary>
        /// 计算描边（线）宽度
        /// </summary>
        /// <param name="newscale"></param>
        /// <param name="oldscale"></param>
        public void RecalcStrokeWidth(SizeF newscale, SizeF oldscale)
        {
            StrokeWidth = RecalcFloat(StrokeWidth, newscale.Width, oldscale.Width);
        }



        /// <summary>
        /// 解析大小
        /// </summary>
        /// <param name="str"></param>
        /// <param name="dpi"></param>
        /// <returns></returns>
        public static float ParseSize(string str, float dpi)
        {
            float koef = 1;
            int ind = str.IndexOf("pt");
            if (ind == -1)
                ind = str.IndexOf("px");
            if (ind == -1)
                ind = str.IndexOf("pc");
            if (ind == -1)
            {
                ind = str.IndexOf("cm");
                if (ind > 0)
                {
                    koef = dpi / 2.54f;
                }
            }
            if (ind == -1)
            {
                ind = str.IndexOf("mm");
                if (ind > 0)
                {
                    koef = dpi / 25.4f;
                }
            }
            if (ind == -1)
            {
                ind = str.IndexOf("in");
                if (ind > 0)
                {
                    koef = dpi;
                }
            }
            if (ind > 0)
                str = str.Substring(0, ind);
            str = RemoveAlphas(str);
            try
            {
                float res = float.Parse(str, CultureInfo.InvariantCulture);
                if (koef != 1.1)
                    res *= koef;
                return res;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        static string RemoveAlphas(string str)
        {
            string s = str.Trim();
            string res = "";
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] < '0' || s[i] > '9')
                    if (s[i] != '.')
                        continue;
                res += s[i];
            }
            return res;
        }

        #endregion


        #region ICloneable Members

        public virtual object Clone()
        {
            return MemberwiseClone();
        }


        #endregion

        #region IDisposable Support

        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    Errors.Clear();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~DrawBase() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion

        #region Error

        protected List<string> _Errors = new List<string>();

        public List<string> Errors
        {
            get { return this._Errors; }
        }

        public void Error(string ErrorMessage)
        {
            this._Errors.Add(ErrorMessage);
            throw new Exception(ErrorMessage);
        }

        #endregion
    }
}

