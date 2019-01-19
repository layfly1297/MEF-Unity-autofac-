/*************************************************
 * 描述：定义弹出窗口的动画类型。
 * 
 * Author：liquan@mozihealthcare.cn
 * Date：2018/6/26 10:37:44
 * Update：
 * ************************************************/

using System;

namespace screenTest.PopupControl
{
    /// <summary>
    /// 弹出窗口的动画类型
    /// </summary>
    [Flags]
    internal enum PopupAnimations
    {
        /// <summary>
        /// 不使用动画
        /// </summary>
        None = 0,
        /// <summary>
        /// 从左到右动画窗口。此标志可与滚动或幻灯片动画一起使用。
        /// </summary>
        LeftToRight = 0x00001,
        /// <summary>
        /// 从右到左的动画窗口。此标志可与滚动或幻灯片动画一起使用。
        /// </summary>
        RightToLeft = 0x00002,
        /// <summary>
        /// 从上到下动画窗口。此标志可与滚动或幻灯片动画一起使用。
        /// </summary>
        TopToBottom = 0x00004,
        /// <summary>
        /// 从底部到顶部动画窗口。此标志可与滚动或幻灯片动画一起使用。
        /// </summary>
        BottomToTop = 0x00008,
        /// <summary>
        /// 当窗口显示时，如果窗口隐藏或向外扩展，则窗口会出现向内折叠。
        /// </summary>
        Center = 0x00010,
        /// <summary>
        /// 使用幻灯片动画。
        /// </summary>
        Slide = 0x40000,
        /// <summary>
        /// 使用淡入淡出效果。
        /// </summary>
        Blend = 0x80000,
        /// <summary>
        /// 使用滚动动画。
        /// </summary>
        Roll = 0x100000,
        /// <summary>
        /// 使用系统默认动画。
        /// </summary>
        SystemDefault = 0x200000,
    }
}
