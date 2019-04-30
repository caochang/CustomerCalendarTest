using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CustomCalendarTest
{
    /// <summary>
    /// 自定义 日历 控件
    /// </summary>
    [TemplatePart(Name = cPreButtonName, Type = typeof(Button))]
    [TemplatePart(Name = cTitleButtonName, Type = typeof(Button))]
    [TemplatePart(Name = cNextButtonName, Type = typeof(Button))]
    [TemplatePart(Name = cDayGridName, Type = typeof(Grid))]
    [TemplatePart(Name = cMouthGridName, Type = typeof(Grid))]
    [TemplatePart(Name = cYearGridName, Type = typeof(Grid))]
    [StyleTypedProperty(Property = cTitleBtnStyleName, StyleTargetType = typeof(Button))]
    [StyleTypedProperty(Property = cCalendarBtnStyleName, StyleTargetType = typeof(CustomCalendarButton))]
    public class CustomCalendar : Control
    {
        static CustomCalendar() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomCalendar), new FrameworkPropertyMetadata(typeof(CustomCalendar)));
        }

        #region TemplatePart Names

        private const string cPreButtonName = "Part_BtnPrevious";
        private const string cTitleButtonName = "Part_BtnTitle";
        private const string cNextButtonName = "Part_BtnNext";
        private const string cDayGridName = "Part_Days";
        private const string cMouthGridName = "Part_Mouths";
        private const string cYearGridName = "Part_Years";

        /// <summary>
        /// 模板控件名称 向前按钮
        /// </summary>
        public static string PreBtnName { get { return cPreButtonName; } }

        /// <summary>
        /// 模板控件名称 标题按钮
        /// </summary>
        public static string TitleBtnName { get { return cTitleButtonName; } }

        /// <summary>
        /// 模板控件名称 向后按钮
        /// </summary>
        public static string NextBtnName { get { return cNextButtonName; } }

        /// <summary>
        /// 模板控件名称 日期Grid面板
        /// </summary>
        public static string DayGridName { get { return cDayGridName; } }
        /// <summary>
        /// 模板控件名称 月份Grid面板
        /// </summary>
        public static string MouthGridName { get { return cDayGridName; } }
        /// <summary>
        /// 模板控件名称 年份Grid面板
        /// </summary>
        public static string YearGridName { get { return cDayGridName; } }


        #endregion

        #region Style and DataTemplate Names

        /// <summary>
        /// 标题按钮样式名称
        /// </summary>
        public const string cTitleBtnStyleName = "TitleButtonStyle";
        /// <summary>
        /// 日期按钮样式名称
        /// </summary>
        public const string cCalendarBtnStyleName = "CalendarButtonStyle";
        /// <summary>
        /// 星期标题模板名称
        /// </summary>
        public const string cWeekDayTemplateName = "WeekDayTemplate";

        #endregion

        #region Calendar Data

        private ChineseLunisolarCalendar mCalendarData = new ChineseLunisolarCalendar();

        #endregion

        #region ApplyTemplate

        private Button mBtnPrevious = null;
        private Button mBtnTitle = null;
        private Button mBtnNext = null;
        private Grid mGridDay = null;
        private Grid mGridMouth = null;
        private Grid mGridYear = null;

        private List<CustomCalendarButton> mDayBtnList = new List<CustomCalendarButton>();
        private List<CustomCalendarButton> mMouthBtnList = new List<CustomCalendarButton>();
        private List<CustomCalendarButton> mYearBtnList = new List<CustomCalendarButton>();

        /// <summary>
        /// 重写OnApplyTemplate
        /// 获取模板对象
        /// </summary>
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            mBtnPrevious = GetTemplateChild(PreBtnName) as Button;
            mBtnTitle = GetTemplateChild(TitleBtnName) as Button;
            mBtnNext = GetTemplateChild(NextBtnName) as Button;
            mGridDay = GetTemplateChild(DayGridName) as Grid;
            mGridMouth = GetTemplateChild(MouthGridName) as Grid;
            mGridYear = GetTemplateChild(YearGridName) as Grid;

            if (mBtnPrevious != null)
                mBtnPrevious.Click += Previous_Click;
            if (mBtnTitle != null)
                mBtnTitle.Click += Title_Click;
            if (mBtnNext != null)
                mBtnNext.Click += Next_Click;

            if (mGridDay != null) {
                for (int row = 0; row < 7; row++) {
                    for (int col = 0; col < 7; col++) {
                        var btn = new CustomCalendarButton();
                        btn.Style = CalendarButtonStyle;
                        Grid.SetRow(btn, row);
                        Grid.SetColumn(btn, col);
                        mGridDay.Children.Add(btn);
                        mDayBtnList.Add(btn);
                    }
                }
            }
            if (mGridMouth != null) {
                for (int row = 0; row < 4; row++) {
                    for (int col = 0; col < 4; col++) {
                        var btn = new CustomCalendarButton();
                        btn.Style = CalendarButtonStyle;
                        Grid.SetRow(btn, row);
                        Grid.SetColumn(btn, col);
                        mGridMouth.Children.Add(btn);
                        mMouthBtnList.Add(btn);
                    }
                }
            }
            if (mGridYear != null) {
                for (int row = 0; row < 4; row++) {
                    for (int col = 0; col < 4; col++) {
                        var btn = new CustomCalendarButton();
                        btn.Style = CalendarButtonStyle;
                        Grid.SetRow(btn, row);
                        Grid.SetColumn(btn, col);
                        mGridYear.Children.Add(btn);
                        mYearBtnList.Add(btn);
                    }
                }
            }
        }

        private void Previous_Click(object sender, RoutedEventArgs e) {

        }

        private void Title_Click(object sender, RoutedEventArgs e) {

        }

        private void Next_Click(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }

        #endregion

        #region DisplayDate

        /// <summary>
        /// 当前显示的日期
        /// 控制 日期
        /// </summary>
        public DateTime DisplayDate {
            get { return (DateTime)GetValue(DisplayDateProperty); }
            set { SetValue(DisplayDateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayDate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayDateProperty =
            DependencyProperty.Register("DisplayDate", typeof(DateTime), typeof(CustomCalendar), new PropertyMetadata(DateTime.Now, DisplayDatePropertyChanged));

        private static void DisplayDatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        }

        #endregion

        #region 其他依赖属性

        /// <summary>
        /// 日期按钮样式
        /// </summary>
        public Style CalendarButtonStyle {
            get { return (Style)GetValue(CalendarButtonStyleProperty); }
            set { SetValue(CalendarButtonStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CalendarButtonStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CalendarButtonStyleProperty =
            DependencyProperty.Register("CalendarButtonStyle", typeof(Style), typeof(CustomCalendar));

        /// <summary>
        /// 标题按钮样式
        /// </summary>
        public Style TitleButtonStyle {
            get { return (Style)GetValue(TitleButtonStyleProperty); }
            set { SetValue(TitleButtonStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleButtonStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleButtonStyleProperty =
            DependencyProperty.Register("TitleButtonStyle", typeof(Style), typeof(CustomCalendar));

        /// <summary>
        /// 星期标题模板
        /// </summary>
        public DataTemplate WeekDayTemplate {
            get { return (DataTemplate)GetValue(WeekDayTemplateProperty); }
            set { SetValue(WeekDayTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WeekDayTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WeekDayTemplateProperty =
            DependencyProperty.Register("WeekDayTemplate", typeof(DataTemplate), typeof(CustomCalendar));

        #endregion
    }
}
