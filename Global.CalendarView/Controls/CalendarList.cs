using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Global.CalendarView.Controls
{
    public class CalendarList : ContentView
    {
        /// <summary>
        ///     The Skeleton content property.
        /// </summary>
        public static readonly BindableProperty SkeletonTemplateProperty = BindableProperty.Create(
            nameof(SkeletonTemplate),
            typeof(ControlTemplate), typeof(CalendarList));

        /// <summary>
        ///     The first day of week property.
        /// </summary>
        public static readonly BindableProperty FirstDayProperty =
            BindableProperty.Create(nameof(FirstDay), typeof(DayOfWeek), typeof(CalendarList), DayOfWeek.Sunday, propertyChanged: FirstDayChanged);

        /// <summary>
        ///     The day template property.
        /// </summary>
        public static readonly BindableProperty DayTemplateProperty =
            BindableProperty.Create(nameof(DayTemplate), typeof(DataTemplate), typeof(CalendarList),
                null);

        /// <summary>
        ///     The day template property.
        /// </summary>
        public static readonly BindableProperty MonthHeaderTemplateProperty =
            BindableProperty.Create(nameof(MonthHeaderTemplate), typeof(DataTemplate), typeof(CalendarList),
                null);

        /// <summary>
        ///     The current date property.
        /// </summary>
        public static readonly BindableProperty CurrentDateProperty =
            BindableProperty.Create(nameof(CurrentDate), typeof(DateTime), typeof(CalendarList), default);

        /// <summary>
        ///     The min date property.
        /// </summary>
        public static readonly BindableProperty MinDateProperty =
            BindableProperty.Create(nameof(MinDate), typeof(DateTime), typeof(CalendarList), DateTime.Today);

        /// <summary>
        ///     The max date property.
        /// </summary>
        public static readonly BindableProperty MaxDateProperty =
            BindableProperty.Create(nameof(MaxDate), typeof(DateTime), typeof(CalendarList),
                DateTime.Today.AddYears(+2));

        /// <summary>
        ///     The marked dates property.
        /// </summary>
        public static readonly BindableProperty MarkedDatesProperty =
            BindableProperty.Create(nameof(MarkedDates), typeof(CalendarDictionary<DateTime, object>),
                typeof(CalendarList),
                new CalendarDictionary<DateTime, object>()); //, propertyChanged: MarkedDatesChanged);

        private readonly CollectionView _collectionView;
        private bool _isScrolling;
        private List<ListedDaysPerMonth> _monthList;
        private double _verticalDelta;


        public EventHandler<SelectedItemChangedEventArgs> ClickedDay;

        public CalendarList()
        {
            _collectionView = new CollectionView
            {
                SelectionMode = SelectionMode.None,
                ItemsSource = _monthList = GetRangeMonths(),
                ItemsLayout = new GridItemsLayout(7, ItemsLayoutOrientation.Vertical),
                IsGrouped = true
            };

            _collectionView.SetBinding(CollectionView.GroupHeaderTemplateProperty, new Binding(nameof(MonthHeaderTemplate)) { Source = this, Mode = BindingMode.OneWay });
            _collectionView.SetBinding(CollectionView.ItemTemplateProperty, new Binding(nameof(DayTemplate)) { Source = this, Mode = BindingMode.OneWay });

            //_collectionView.Scrolled += _collectionViewScrolledLoadDays;
            //VisibleViews.CollectionChanged += VisibleViews_CollectionChanged;

            Content = _collectionView;
        }

        /// <summary>
        ///     Gets or sets the current date.
        /// </summary>
        /// <value>The current date attributes.</value>
        public DayOfWeek FirstDay
        {
            get => (DayOfWeek)GetValue(FirstDayProperty);
            set => SetValue(FirstDayProperty, value);
        }

        /// <summary>
        ///     Gets or sets the skeleton template.
        /// </summary>
        /// <value>The day template attributes.</value>
        public ControlTemplate SkeletonTemplate
        {
            get => (ControlTemplate) GetValue(SkeletonTemplateProperty);
            set => SetValue(SkeletonTemplateProperty, value);
        }

        /// <summary>
        ///     Gets or sets the day template.
        /// </summary>
        /// <value>The day template attributes.</value>
        public DataTemplate DayTemplate
        {
            get => (DataTemplate)GetValue(DayTemplateProperty);
            set => SetValue(DayTemplateProperty, value);
        }

        /// <summary>
        ///     Gets or sets the Month Header template.
        /// </summary>
        /// <value>The day template attributes.</value>
        public DataTemplate MonthHeaderTemplate
        {
            get => (DataTemplate)GetValue(MonthHeaderTemplateProperty);
            set => SetValue(MonthHeaderTemplateProperty, value);
        }

        /// <summary>
        ///     Gets or sets the current date.
        /// </summary>
        /// <value>The current date attributes.</value>
        public DateTime CurrentDate
        {
            get => (DateTime) GetValue(CurrentDateProperty);
            set => SetValue(CurrentDateProperty, value);
        }

        /// <summary>
        ///     Gets or sets the minimum date.
        /// </summary>
        /// <value>The minimun date attributes.</value>
        public DateTime MinDate
        {
            get => (DateTime) GetValue(MinDateProperty);
            set => SetValue(MinDateProperty, value);
        }

        /// <summary>
        ///     Gets or sets the maximum date.
        /// </summary>
        /// <value>The maximum date attributes.</value>
        public DateTime MaxDate
        {
            get => (DateTime) GetValue(MaxDateProperty);
            set => SetValue(MaxDateProperty, value);
        }

        /// <summary>
        ///     Gets or sets marked dates.
        /// </summary>
        /// <value>The marked dates attributes.</value>
        public CalendarDictionary<DateTime, object> MarkedDates
        {
            get => (CalendarDictionary<DateTime, object>) GetValue(MarkedDatesProperty);
            set => SetValue(MarkedDatesProperty, value);
        }

        private static void FirstDayChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CalendarList calendarList)
            {
                calendarList._collectionView.ItemsSource = calendarList._monthList = calendarList.GetRangeMonths();
            }
        }

        //private void _collectionViewScrolledLoadDays(object sender, ItemsViewScrolledEventArgs e)
        //{
        //    _verticalDelta = e.VerticalDelta;

        //    if (Math.Abs(e.VerticalDelta) < 10
        //        || e.LastVisibleItemIndex == _monthList.Count - 1
        //        || e.FirstVisibleItemIndex == 0)
        //    {
        //        if (e.LastVisibleItemIndex == _monthList.Count - 1)
        //            Console.WriteLine("EndOfTheList");
        //        else if (e.FirstVisibleItemIndex == 0)
        //            Console.WriteLine("StartOfTheList");
        //        else
        //            Console.WriteLine("Middle");

        //        _monthCellList //.OrderBy(o => (e.VerticalDelta > 0) ? o.VisibleIndex :- o.VisibleIndex)
        //            .ForEach(cell =>
        //            {
        //                var index = _monthList.FindIndex(a =>
        //                {
        //                    if (!(cell.BindingContext is DateTime cellDate)) return false;
        //                    return a == cellDate;
        //                });
        //                if (index >= e.FirstVisibleItemIndex && index <= e.LastVisibleItemIndex)
        //                    if (!cell.IsLoaded)
        //                        cell.LoadCell();
        //            });
        //    }

        //    if (Math.Abs(e.VerticalDelta) < 10)
        //        _isScrolling = false;
        //    else
        //        _isScrolling = true;
        //    Debug.WriteLine("HorizontalDelta: " + e.HorizontalDelta);
        //    Debug.WriteLine("VerticalDelta: " + e.VerticalDelta);
        //    Debug.WriteLine("HorizontalOffset: " + e.HorizontalOffset);
        //    Debug.WriteLine("VerticalOffset: " + e.VerticalOffset);
        //    Debug.WriteLine("FirstVisibleItemIndex: " + e.FirstVisibleItemIndex);
        //    Debug.WriteLine("CenterItemIndex: " + e.CenterItemIndex);
        //    Debug.WriteLine("LastVisibleItemIndex: " + e.LastVisibleItemIndex);
        //}

        private List<ListedDaysPerMonth> GetRangeMonths()
        {
            if (MaxDate != default && MinDate != default)
            {
                //Month
                var numberOfMonths = (MaxDate.Year - MinDate.Year) * 12 + MaxDate.Month - MinDate.Month + 1;
                var monthList = new List<ListedDaysPerMonth>();
                Enumerable.Range(0, numberOfMonths).ForEach(d =>
                {
                    monthList.Add(new ListedDaysPerMonth(MinDate.GetFirstDayOfMonth().AddMonths(d)));
                });

                //Populate days datetime in monthlist.
                monthList.ForEach((m) =>
                {
                    var begin = m.Month.GetFirstDayOfMonth().GetFirstDayOfWeek(FirstDay);
                    var end = m.Month.GetLastDayOfMonth().GetLastDayOfWeek(FirstDay);
                    var numberOfDays = Convert.ToInt32((end - begin).TotalDays);
                    
                    Enumerable.Range(0, numberOfDays).ForEach(day => m.Add(begin.AddDays(day)));
                });

                return monthList;
            }

            return new List<ListedDaysPerMonth>();
        }

        public void PropagateClickedDate(object sender, SelectedItemChangedEventArgs e)
        {
            ClickedDay?.Invoke(sender, e);
        }

    }
}