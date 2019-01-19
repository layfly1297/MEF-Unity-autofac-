using System;
using System.Windows.Forms;

namespace screenTest
{
    public class WheelExtend
    {
        /// <summary>
        /// Accumulates mouse wheel deltas and reports the actual number of lines to scroll.
        /// </summary>
        public sealed class MouseWheelHandler
        { 

            const int WHEEL_DELTA = 120;

            int mouseWheelDelta;

            public int GetScrollAmount(MouseEventArgs e)
            {
                // accumulate the delta to support high-resolution mice 支持高分辨率
                mouseWheelDelta += e.Delta;
                Console.WriteLine("Mouse rotation: new delta=" + e.Delta + ", total delta=" + mouseWheelDelta);
                //算出滚动的行数
                int linesPerClick = Math.Max(SystemInformation.MouseWheelScrollLines, 1);
                //算出滚动距离
                int scrollDistance = mouseWheelDelta * linesPerClick / WHEEL_DELTA;
                //为什么取模值
                mouseWheelDelta %= Math.Max(1, WHEEL_DELTA / linesPerClick);
                return scrollDistance;
            }

            public void Scroll(ScrollBar scrollBar, MouseEventArgs e)
            {
                int newvalue = scrollBar.Value - GetScrollAmount(e) * scrollBar.SmallChange;
                scrollBar.Value = Math.Max(scrollBar.Minimum, Math.Min(scrollBar.Maximum - scrollBar.LargeChange + 1, newvalue));
            }
        }
    }
}