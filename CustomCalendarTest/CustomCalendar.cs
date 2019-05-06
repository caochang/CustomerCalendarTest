using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

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
        public static string MouthGridName { get { return cMouthGridName; } }
        /// <summary>
        /// 模板控件名称 年份Grid面板
        /// </summary>
        public static string YearGridName { get { return cYearGridName; } }

        #endregion

        #region const

        public const DayOfWeek cFirstDayOfWeek = DayOfWeek.Monday;
        public static readonly string[] cWeekDayNames = new string[] { "一", "二", "三", "四", "五", "六", "日" };

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
                for (int col = 0; col < 7; col++) {
                    FrameworkElement fe = null;
                    if (WeekDayTemplate != null) {
                        fe = WeekDayTemplate.LoadContent() as FrameworkElement;
                        fe.DataContext = cWeekDayNames[col];
                    }
                    else {
                        var ctrl = new ContentControl();
                        ctrl.Content = cWeekDayNames[col];
                        ctrl.HorizontalAlignment = HorizontalAlignment.Center;
                        ctrl.VerticalAlignment = VerticalAlignment.Center;
                        ctrl.Padding = new Thickness(10);
                        fe = ctrl;
                    }
                    Grid.SetRow(fe, 0);
                    Grid.SetColumn(fe, col);
                    mGridDay.Children.Add(fe);
                }
                for (int row = 1; row < 7; row++) {
                    for (int col = 0; col < 7; col++) {
                        var btn = new CustomCalendarButton(this);
                        btn.DisplayMode = CalendarButtonDisplayMode.Date;
                        btn.Style = CalendarButtonStyle;
                        Grid.SetRow(btn, row);
                        Grid.SetColumn(btn, col);
                        mGridDay.Children.Add(btn);
                        mDayBtnList.Add(btn);
                        btn.Click += DateBtn_Click;
                    }
                }
            }
            if (mGridMouth != null) {
                var mouth = 11;
                var isInactive = true;
                for (int row = 0; row < 4; row++) {
                    for (int col = 0; col < 4; col++) {
                        var btn = new CustomCalendarButton(this);
                        btn.DisplayMode = CalendarButtonDisplayMode.Mouth;
                        btn.Style = CalendarButtonStyle;
                        Grid.SetRow(btn, row);
                        Grid.SetColumn(btn, col);
                        mGridMouth.Children.Add(btn);
                        mMouthBtnList.Add(btn);
                        btn.Click += MouthBtn_Click;

                        btn.Content = mouth;
                        if (mouth == 1)
                            isInactive = !isInactive;
                        btn.IsInactive = isInactive;
                        mouth++;
                        if (mouth > 12)
                            mouth -= 12;
                    }
                }
            }
            if (mGridYear != null) {
                var year = 7;
                var isInactive = true;
                for (int row = 0; row < 4; row++) {
                    for (int col = 0; col < 4; col++) {
                        var btn = new CustomCalendarButton(this);
                        btn.DisplayMode = CalendarButtonDisplayMode.Year;
                        btn.Style = CalendarButtonStyle;
                        Grid.SetRow(btn, row);
                        Grid.SetColumn(btn, col);
                        mGridYear.Children.Add(btn);
                        mYearBtnList.Add(btn);
                        btn.Click += YearBtn_Click;

                        if (year % 10 == 0)
                            isInactive = !isInactive;
                        btn.IsInactive = isInactive;
                        year++;
                    }
                }
            }

            UpdateData();
            UpdateGrids();
            UpdateTitle();
        }

        private void DateBtn_Click(object sender, RoutedEventArgs e) {
            if (!(sender is CustomCalendarButton))
                return;

            var btn = sender as CustomCalendarButton;
            var date = btn.Date;
            DisplayDate = date;
            SelectedDate = date;
        }

        private void MouthBtn_Click(object sender, RoutedEventArgs e) {
            if (!(sender is CustomCalendarButton))
                return;

            var btn = sender as CustomCalendarButton;
            var newDT = new DateTime(btn.Date.Year, btn.Date.Month, 1);
            DisplayDate = newDT;
            if (!btn.IsInactive)
                DisplayMode = CalendarMode.Month;
        }

        private void YearBtn_Click(object sender, RoutedEventArgs e) {
            if (!(sender is CustomCalendarButton))
                return;

            var btn = sender as CustomCalendarButton;
            var newDT = new DateTime(btn.Date.Year, DisplayDate.Month, 1);
            DisplayDate = newDT;
            if (!btn.IsInactive)
                DisplayMode = CalendarMode.Year;
        }

        private void Previous_Click(object sender, RoutedEventArgs e) {
            switch (DisplayMode) {
                case CalendarMode.Month:
                    var year = DisplayDate.Year;
                    var mouth = DisplayDate.Month;
                    mouth--;
                    if (mouth < 1) {
                        mouth = 12;
                        year--;
                    }
                    DisplayDate = new DateTime(year, mouth, 1);
                    break;
                case CalendarMode.Year:
                    DisplayDate = new DateTime(DisplayDate.Year - 1, DisplayDate.Month, 1);
                    break;
                case CalendarMode.Decade:
                    DisplayDate = new DateTime(DisplayDate.Year - 10, DisplayDate.Month, 1);
                    break;
                default:
                    break;
            }
        }

        private void Title_Click(object sender, RoutedEventArgs e) {
            switch (DisplayMode) {
                case CalendarMode.Month:
                    DisplayMode = CalendarMode.Year;
                    break;
                case CalendarMode.Year:
                    DisplayMode = CalendarMode.Decade;
                    break;
                default:
                    break;
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e) {
            switch (DisplayMode) {
                case CalendarMode.Month:
                    var year = DisplayDate.Year;
                    var mouth = DisplayDate.Month;
                    mouth++;
                    if (mouth > 12) {
                        mouth = 1;
                        year++;
                    }
                    DisplayDate = new DateTime(year, mouth, 1);
                    break;
                case CalendarMode.Year:
                    DisplayDate = new DateTime(DisplayDate.Year + 1, DisplayDate.Month, 1);
                    break;
                case CalendarMode.Decade:
                    DisplayDate = new DateTime(DisplayDate.Year + 10, DisplayDate.Month, 1);
                    break;
                default:
                    break;
            }
        }

        private void UpdateData() {
            var year = DisplayDate.Year;
            var mouth = DisplayDate.Month;

            if(mGridYear != null) {
                var firstYear = year - (year % 10) - 3;
                for (int i = 0; i < mYearBtnList.Count; i++) {
                    var btn = mYearBtnList[i];
                    btn.Date = new DateTime(firstYear + i, mouth, 1);
                }
            }

            if(mGridMouth != null) {
                var firstMouth = 11;
                for (int i = 0; i < mMouthBtnList.Count; i++) {
                    var curMouth = firstMouth + i;
                    var curYear = DisplayDate.Year - 1;
                    while (curMouth > 12) {
                        curMouth -= 12;
                        curYear += 1;
                    }
                    var btn = mMouthBtnList[i];
                    btn.Date = new DateTime(curYear, curMouth, 1);
                }
            }

            if (mGridDay != null) {
                var firstDate = new DateTime(DisplayDate.Year, DisplayDate.Month, 1);
                var preDayCount = (int)firstDate.DayOfWeek - (int)cFirstDayOfWeek;
                if (preDayCount < 0)
                    preDayCount += 7;
                firstDate -= new TimeSpan(preDayCount, 0, 0, 0);
                for (int i = 0; i < mDayBtnList.Count; i++) {
                    var btn = mDayBtnList[i];
                    btn.Date = firstDate + new TimeSpan(i, 0, 0, 0);

                    btn.IsToday = false;
                    if (btn.Date.Date == DateTime.Now.Date)
                        btn.IsToday = true;
                    btn.IsInactive = false;
                    if (btn.Date.Month != DisplayDate.Month)
                        btn.IsInactive = true;
                }
            }
        }

        private void UpdateGrids() {
            switch (DisplayMode) {
                case CalendarMode.Month:
                    if (mGridDay != null)
                        mGridDay.Visibility = Visibility.Visible;
                    if (mGridMouth != null)
                        mGridMouth.Visibility = Visibility.Hidden;
                    if (mGridYear != null)
                        mGridYear.Visibility = Visibility.Hidden;
                    break;
                case CalendarMode.Year:
                    if (mGridDay != null)
                        mGridDay.Visibility = Visibility.Hidden;
                    if (mGridMouth != null)
                        mGridMouth.Visibility = Visibility.Visible;
                    if (mGridYear != null)
                        mGridYear.Visibility = Visibility.Hidden;
                    break;
                case CalendarMode.Decade:
                    if (mGridDay != null)
                        mGridDay.Visibility = Visibility.Hidden;
                    if (mGridMouth != null)
                        mGridMouth.Visibility = Visibility.Hidden;
                    if (mGridYear != null)
                        mGridYear.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private void UpdateTitle() {
            if (mBtnTitle == null)
                return;
            switch (DisplayMode) {
                case CalendarMode.Month:
                    mBtnTitle.Content = DisplayDate.ToString("yyyy-MM");
                    break;
                case CalendarMode.Year:
                    mBtnTitle.Content = DisplayDate.ToString("yyyy");
                    break;
                case CalendarMode.Decade:
                    var beginY = DisplayDate.Year - (DisplayDate.Year % 10);
                    var endY = beginY + 9;
                    mBtnTitle.Content = beginY + "-" + endY;
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// 当 选中日期SelectedDate 发生改变时触发
        /// </summary>
        public event EventHandler SelectedDateChanged;
        /// <summary>
        /// 当 显示日期DisplayDate 发生改变时触发
        /// </summary>
        public event EventHandler DisplayDateChanged;
        /// <summary>
        /// 当 显示模式DisplayMode 发生改变时触发
        /// </summary>
        public event EventHandler DisplayModeChanged;

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
            if (!(d is CustomCalendar))
                return;

            var cc = d as CustomCalendar;
            cc.UpdateData();
            cc.UpdateTitle();
            if (cc.DisplayDateChanged != null)
                cc.DisplayDateChanged(cc, EventArgs.Empty);
        }

        #endregion

        #region SelectedDate

        /// <summary>
        /// 当前选中的日期
        /// </summary>
        public DateTime? SelectedDate {
            get { return (DateTime?)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register("SelectedDate", typeof(DateTime?), typeof(CustomCalendar), new PropertyMetadata(null, SelectedDatePropertyChanged));

        private static void SelectedDatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (!(d is CustomCalendar))
                return;

            var cc = d as CustomCalendar;
            var seletedDateProcessed = false;
            foreach (var item in cc.mDayBtnList) {
                item.IsSelected = false;
                if (!seletedDateProcessed  && item.Date == cc.SelectedDate) {
                    item.IsSelected = true;
                    if (cc.SelectedDateChanged != null)
                        cc.SelectedDateChanged(cc, EventArgs.Empty);
                    seletedDateProcessed = true;
                }
            }
        }

        #endregion

        #region DisplayMode

        /// <summary>
        /// 显示模式
        /// </summary>
        public CalendarMode DisplayMode {
            get { return (CalendarMode)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode", typeof(CalendarMode), typeof(CustomCalendar), new PropertyMetadata(CalendarMode.Month, DisplayModePropertyChanged));

        private static void DisplayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (!(d is CustomCalendar))
                return;

            var cc = d as CustomCalendar;
            cc.UpdateGrids();
            cc.UpdateTitle();
            if (cc.DisplayModeChanged != null)
                cc.DisplayModeChanged(cc, EventArgs.Empty);
        }

        #endregion

        #region StyleAndTemplate

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
