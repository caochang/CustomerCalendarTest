using System;
using System.Windows;
using System.Windows.Controls;

namespace CustomCalendarTest
{
    /// <summary>
    /// 用于日历中 日、月、年 呈现的按钮
    /// </summary>
    public class CustomCalendarButton : Button
    {
        static CustomCalendarButton() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomCalendarButton), new FrameworkPropertyMetadata(typeof(CustomCalendarButton)));
        }

        public CustomCalendarButton() { }

        public CustomCalendarButton(CustomCalendar ownerCalendar) : this() {
            Owner = ownerCalendar;
        }

        /// <summary>
        /// 此控件的 父日历控件
        /// </summary>
        public CustomCalendar Owner { get; private set; }

        protected override void OnClick() {
            base.OnClick();

            if (Owner == null)
                return;
        }

        #region 作为标志的 依赖属性

        /// <summary>
        /// 是否是 今天的日期
        /// </summary>
        public bool IsToday {
            get { return (bool)GetValue(IsTodayProperty); }
            internal set { SetValue(IsTodayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsToday.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTodayProperty =
            DependencyProperty.Register("IsToday", typeof(bool), typeof(CustomCalendarButton));

        /// <summary>
        /// 是否 被选中
        /// </summary>
        public bool IsSelected {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(CustomCalendarButton));

        /// <summary>
        /// 是否 超出当前显示范围
        /// </summary>
        public bool IsInactive {
            get { return (bool)GetValue(IsInactiveProperty); }
            set { SetValue(IsInactiveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsInactive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsInactiveProperty =
            DependencyProperty.Register("IsInactive", typeof(bool), typeof(CustomCalendarButton));

        /// <summary>
        /// 是否 无效
        /// </summary>
        public bool IsBlackedOut {
            get { return (bool)GetValue(IsBlackedOutProperty); }
            set { SetValue(IsBlackedOutProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsBlackedOut.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBlackedOutProperty =
            DependencyProperty.Register("IsBlackedOut", typeof(bool), typeof(CustomCalendarButton));

        #endregion

        #region Date 相关

        /// <summary>
        /// 当前按钮对应的日期
        /// </summary>
        public DateTime Date {
            get { return (DateTime)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Date.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(DateTime), typeof(CustomCalendarButton), new PropertyMetadata(DateTime.Now, DatePropertyChanged));

        private static void DatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (!(d is CustomCalendarButton))
                return;

            (d as CustomCalendarButton).UpdateContent();
        }

        /// <summary>
        /// 按钮显示模式
        /// </summary>
        public CalendarButtonDisplayMode DisplayMode {
            get { return (CalendarButtonDisplayMode)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode", typeof(CalendarButtonDisplayMode), typeof(CustomCalendarButton), new PropertyMetadata(CalendarButtonDisplayMode.Date, DisplayModePropertyChanged));

        private static void DisplayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (!(d is CustomCalendarButton))
                return;

            (d as CustomCalendarButton).UpdateContent();
        }

        private void UpdateContent() {
            switch (DisplayMode) {
                case CalendarButtonDisplayMode.Date:
                    Content = Date.Day;
                    break;
                case CalendarButtonDisplayMode.Mouth:
                    Content = Date.Month;
                    break;
                case CalendarButtonDisplayMode.Year:
                    Content = Date.Year;
                    break;
                default:
                    break;
            }
        }

        #endregion

    }

    /// <summary>
    /// 日期控件按钮 的日期模式
    /// </summary>
    public enum CalendarButtonDisplayMode
    {
        /// <summary>
        /// 日期
        /// </summary>
        Date = 0,
        /// <summary>
        /// 月份
        /// </summary>
        Mouth = 1,
        /// <summary>
        /// 年份
        /// </summary>
        Year = 2
    }
}
