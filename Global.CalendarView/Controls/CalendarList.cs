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
        ///     The Month template property.
        /// </summary>
        public static readonly BindableProperty MonthTemplateProperty =
            BindableProperty.Create(nameof(MonthTemplate), typeof(ControlTemplate),
                typeof(CalendarList)); //, validateValue: ValidateDayTemplate);

        /// <summary>
        ///     The current date property.
        /// </summary>
        public static readonly BindableProperty CurrentDateProperty =
            BindableProperty.Create(nameof(CurrentDate), typeof(DateTime), typeof(CalendarList), default,
                propertyChanged: CurrentDateChanged);

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

        /// <summary>
        ///     The Visible views property.
        /// </summary>
        public static readonly BindableProperty VisibleViewsProperty =
            BindableProperty.Create(nameof(VisibleViews), typeof(ObservableCollection<MonthCell>), typeof(Calendar),
                new ObservableCollection<MonthCell>());

        /// <summary>
        ///     The Month template Height property.
        /// </summary>
        public static readonly BindableProperty TemplateViewHeightProperty =
            BindableProperty.Create(nameof(TemplateViewHeight), typeof(double), typeof(Calendar),
                -1d);

        private readonly CollectionView _collectionView;
        private bool _isScrolling;
        private readonly List<MonthCell> _monthCellList = new List<MonthCell>();
        private readonly List<DateTime> _monthList;
        private double _verticalDelta;


        public EventHandler<SelectedItemChangedEventArgs> ClickedDay;

        public CalendarList()
        {
            _collectionView = new CollectionView
            {
                SelectionMode = SelectionMode.None,
                ItemsSource = _monthList = GetRangeMonths()
            };
            _collectionView.ItemTemplate = new DataTemplate(() =>
            {
                var monthCell = new MonthCell(this);
                _monthCellList.Add(monthCell);
                return monthCell;
            });

            _collectionView.Scrolled += _collectionViewScrolledLoadDays;
            //VisibleViews.CollectionChanged += VisibleViews_CollectionChanged;

            Content = _collectionView;
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
        ///     Gets or sets the month template.
        /// </summary>
        /// <value>The day template attributes.</value>
        public ControlTemplate MonthTemplate
        {
            get => (ControlTemplate) GetValue(MonthTemplateProperty);
            set => SetValue(MonthTemplateProperty, value);
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

        public ObservableCollection<MonthCell> VisibleViews
        {
            get => (ObservableCollection<MonthCell>) GetValue(VisibleViewsProperty);
            private set => SetValue(MarkedDatesProperty, value);
        }

        public double TemplateViewHeight
        {
            get => (double) GetValue(TemplateViewHeightProperty);
            set => SetValue(TemplateViewHeightProperty, value);
        }

        //private void VisibleViews_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    VisibleViews.OrderBy(o => (_verticalDelta > 0) ? o.VisibleIndex : -o.VisibleIndex)
        //            .ForEach((cell) =>
        //            {
        //                if (!_isScrolling && !cell.IsLoaded)
        //                    cell.LoadCell();
        //            });
        //}

        private void _collectionViewScrolledLoadDays(object sender, ItemsViewScrolledEventArgs e)
        {
            _verticalDelta = e.VerticalDelta;

            if (Math.Abs(e.VerticalDelta) < 10
                || e.LastVisibleItemIndex == _monthList.Count - 1
                || e.FirstVisibleItemIndex == 0)
            {
                if (e.LastVisibleItemIndex == _monthList.Count - 1)
                    Console.WriteLine("EndOfTheList");
                else if (e.FirstVisibleItemIndex == 0)
                    Console.WriteLine("StartOfTheList");
                else
                    Console.WriteLine("Middle");

                _monthCellList //.OrderBy(o => (e.VerticalDelta > 0) ? o.VisibleIndex :- o.VisibleIndex)
                    .ForEach(cell =>
                    {
                        var index = _monthList.FindIndex(a =>
                        {
                            if (!(cell.BindingContext is DateTime cellDate)) return false;
                            return a == cellDate;
                        });
                        if (index >= e.FirstVisibleItemIndex && index <= e.LastVisibleItemIndex)
                            if (!cell.IsLoaded)
                                cell.LoadCell();
                    });
            }

            if (Math.Abs(e.VerticalDelta) < 10)
                _isScrolling = false;
            else
                _isScrolling = true;
            Debug.WriteLine("HorizontalDelta: " + e.HorizontalDelta);
            Debug.WriteLine("VerticalDelta: " + e.VerticalDelta);
            Debug.WriteLine("HorizontalOffset: " + e.HorizontalOffset);
            Debug.WriteLine("VerticalOffset: " + e.VerticalOffset);
            Debug.WriteLine("FirstVisibleItemIndex: " + e.FirstVisibleItemIndex);
            Debug.WriteLine("CenterItemIndex: " + e.CenterItemIndex);
            Debug.WriteLine("LastVisibleItemIndex: " + e.LastVisibleItemIndex);
        }

        private static void CurrentDateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CalendarList calendarList)
            {
                //var month = calendarList._monthList.Find(d => d.Value.Year == calendarList.CurrentDate.Year && d.Value.Month == calendarList.CurrentDate.Month);
                //if (month != default)

                //    calendarList._collectionView.ScrollTo(month);
            }
        }

        private List<DateTime> GetRangeMonths()
        {
            if (MaxDate != default && MinDate != default)
            {
                var numberOfMonths = (MaxDate.Year - MinDate.Year) * 12 + MaxDate.Month - MinDate.Month + 1;

                var list = new List<DateTime>();
                Enumerable.Range(0, numberOfMonths).ForEach(d =>
                {
                    list.Add(MinDate.GetFirstDayOfMonth().AddMonths(d));
                });
                return list;
            }

            return new List<DateTime>();
        }

        public void PropagateClickedDate(object sender, SelectedItemChangedEventArgs e)
        {
            ClickedDay?.Invoke(sender, e);
        }

        ///// <summary>
        /////     The marked dates property changed.
        ///// </summary>
        ///// <param name="bindable">The object.</param>
        ///// <param name="oldValue">The old value.</param>
        ///// <param name="newValue">The new value.</param>
        //private static void MarkedDatesChanged(BindableObject bindable, object oldValue, object newValue)
        //{
        //    if (!(bindable is Month month)) return;

        //    if (oldValue is CalendarDictionary<DateTime, object> oldDic)
        //        oldDic.ClearCollectionChangedHandler();

        //    if (newValue is CalendarDictionary<DateTime, object> newDic)
        //    {
        //        newDic.CollectionChanged += (sender, e) => month.MarkedDatesCollectionChanged(e);
        //        month.UpdateAllMarkers();
        //    }
        //}
    }
}