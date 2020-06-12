using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Global.CalendarView.Controls
{
    public class CalendarList : ContentView
    {
        /// <summary>
        ///     The Month template property.
        /// </summary>
        public static readonly BindableProperty MonthTemplateProperty =
            BindableProperty.Create(nameof(MonthTemplate), typeof(ControlTemplate), typeof(CalendarList),
                null); //, validateValue: ValidateDayTemplate);

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
            BindableProperty.Create(nameof(MaxDate), typeof(DateTime), typeof(CalendarList), DateTime.Today.AddYears(+2));

        /// <summary>
        ///     The marked dates property.
        /// </summary>
        public static readonly BindableProperty MarkedDatesProperty =
            BindableProperty.Create(nameof(MarkedDates), typeof(CalendarDictionary<DateTime, object>), typeof(CalendarList),
                new CalendarDictionary<DateTime, object>());//, propertyChanged: MarkedDatesChanged);


        public EventHandler<SelectedItemChangedEventArgs> ClickedDay;
        private Dictionary<int, DateTime> _monthList;
        private List<MonthCell> _monthCellList = new List<MonthCell>();
        private CollectionView _collectionView;

        public CalendarList()
        {
            _collectionView = new CollectionView
            {
                SelectionMode = SelectionMode.None,
                ItemsSource = _monthList = GetRangeMonths(),
            };
            _collectionView.ItemTemplate = new DataTemplate(() =>
            {
                var monthCell = new MonthCell();
                _monthCellList.Add(monthCell);
                return monthCell;
            });

            _collectionView.Scrolled += _collectionViewScrolledLoadDays;

            Content = _collectionView;
        }

        private void _collectionViewScrolledLoadDays(object sender, ItemsViewScrolledEventArgs e)
        {
            if (Math.Abs(e.VerticalDelta) < 10)
            {
                _monthCellList.ForEach((cell) =>
                {
                    if (cell.VisibleIndex >= e.FirstVisibleItemIndex && cell.VisibleIndex <= e.LastVisibleItemIndex)
                    {
                        var begin = cell.MonthControl.CurrentDate.GetFirstDayOfMonth().GetFirstDayOfWeek(cell.MonthControl.FirstDay);
                        if (cell.MonthControl.DaysViews.Any() && begin != cell.MonthControl.DaysViews[0].Date)
                            cell.MonthControl.LoadDays();
                    }
                    cell.MonthControl.IsScrolling = false;
                });
            }
            else
            {
                _monthCellList.ForEach((cell) =>
                {
                    cell.MonthControl.IsScrolling = true;
                });
            }
            Debug.WriteLine("HorizontalDelta: " + e.HorizontalDelta);
            Debug.WriteLine("VerticalDelta: " + e.VerticalDelta);
            Debug.WriteLine("HorizontalOffset: " + e.HorizontalOffset);
            Debug.WriteLine("VerticalOffset: " + e.VerticalOffset);
            Debug.WriteLine("FirstVisibleItemIndex: " + e.FirstVisibleItemIndex);
            Debug.WriteLine("CenterItemIndex: " + e.CenterItemIndex);
            Debug.WriteLine("LastVisibleItemIndex: " + e.LastVisibleItemIndex);
        }

        /// <summary>
        ///     Gets or sets the day template.
        /// </summary>
        /// <value>The day template attributes.</value>
        public ControlTemplate MonthTemplate
        {
            get => (ControlTemplate)GetValue(MonthTemplateProperty);
            set => SetValue(MonthTemplateProperty, value);
        }

        /// <summary>
        ///     Gets or sets the current date.
        /// </summary>
        /// <value>The current date attributes.</value>
        public DateTime CurrentDate
        {
            get => (DateTime)GetValue(CurrentDateProperty);
            set => SetValue(CurrentDateProperty, value);
        }

        /// <summary>
        ///     Gets or sets the minimum date.
        /// </summary>
        /// <value>The minimun date attributes.</value>
        public DateTime MinDate
        {
            get => (DateTime)GetValue(MinDateProperty);
            set => SetValue(MinDateProperty, value);
        }

        /// <summary>
        ///     Gets or sets the maximum date.
        /// </summary>
        /// <value>The maximum date attributes.</value>
        public DateTime MaxDate
        {
            get => (DateTime)GetValue(MaxDateProperty);
            set => SetValue(MaxDateProperty, value);
        }

        /// <summary>
        ///     Gets or sets marked dates.
        /// </summary>
        /// <value>The marked dates attributes.</value>
        public CalendarDictionary<DateTime, object> MarkedDates
        {
            get => (CalendarDictionary<DateTime, object>)GetValue(MarkedDatesProperty);
            set => SetValue(MarkedDatesProperty, value);
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

        private Dictionary<int, DateTime> GetRangeMonths()
        {
            if (MaxDate != default && MinDate != default)
            {
                var numberOfMonths = ((MaxDate.Year - MinDate.Year) * 12) + MaxDate.Month - MinDate.Month + 1;

                var dic = new Dictionary<int, DateTime>();
                Enumerable.Range(0, numberOfMonths).ForEach(d =>
                {
                    dic.Add(d, MinDate.GetFirstDayOfMonth().AddMonths(d));
                });
                return dic;
            }

            return new Dictionary<int, DateTime>();
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
