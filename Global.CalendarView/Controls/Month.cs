using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Global.CalendarView.Models;
using Xamarin.Forms;

namespace Global.CalendarView.Controls
{
    public class Month : ContentView
    {
        /// <summary>
        ///     The current date property.
        /// </summary>
        public static readonly BindableProperty CurrentMonthProperty =
            BindableProperty.Create(nameof(CurrentMonth), typeof(DateTime), typeof(Month), DateTime.Today,
                propertyChanged: CurrentMonthChanged);

        /// <summary>
        ///     The min date property.
        /// </summary>
        public static readonly BindableProperty MinDateProperty =
            BindableProperty.Create(nameof(MinDate), typeof(DateTime), typeof(Month), DateTime.Today.AddYears(-2));

        /// <summary>
        ///     The max date property.
        /// </summary>
        public static readonly BindableProperty MaxDateProperty =
            BindableProperty.Create(nameof(MaxDate), typeof(DateTime), typeof(Month), DateTime.Today.AddYears(+2));

        /// <summary>
        ///     The first day of week property.
        /// </summary>
        public static readonly BindableProperty FirstDayProperty =
            BindableProperty.Create(nameof(FirstDay), typeof(DayOfWeek), typeof(Month), DayOfWeek.Sunday,
                propertyChanged: FirstDayChanged);

        /// <summary>
        ///     The marked dates property.
        /// </summary>
        public static readonly BindableProperty MarkedDatesProperty =
            BindableProperty.Create(nameof(MarkedDates), typeof(CalendarDictionary<DateTime, object>), typeof(Month),
                new CalendarDictionary<DateTime, object>(), propertyChanged: MarkedDatesChanged);

        /// <summary>
        ///     The day template property.
        /// </summary>
        public static readonly BindableProperty DayTemplateProperty =
            BindableProperty.Create(nameof(DayTemplate), typeof(ControlTemplate), typeof(Month),
                null, propertyChanged: DayTemplateChanged); //, validateValue: ValidateDayTemplate);

        /// <summary>
        ///     The Skeleton content property.
        /// </summary>
        public static readonly BindableProperty SkeletonViewProperty = BindableProperty.Create(nameof(SkeletonView),
            typeof(View), typeof(Month), null, propertyChanged: SkeletonViewChanged);

        /// <summary>
        ///     The first day of week property.
        /// </summary>
        public static readonly BindableProperty CalendarModeProperty =
            BindableProperty.Create(nameof(CalendarMode), typeof(CalendarMode), typeof(Month));

        private Grid _grid;

        public EventHandler<SelectedItemChangedEventArgs> ClickedDay;

        public Month()
        {
            DaysViews = new List<DayCell>();
            MarkedDates.CollectionChanged += (sender, e) => MarkedDatesCollectionChanged(e);
        }

        public Month(DayOfWeek firstDay, DateTime currentDate, DateTime minDate, DateTime maxDate,
            CalendarDictionary<DateTime, object> markedDates)
        {
            FirstDay = firstDay;
            CurrentMonth = currentDate;
            MinDate = minDate;
            MaxDate = maxDate;
            MarkedDates = markedDates;

            DaysViews = new List<DayCell>();
            MarkedDates.CollectionChanged += (sender, e) => MarkedDatesCollectionChanged(e);
        }

        private List<DayCell> DaysViewsProperty { get; set; }
        public bool IsGenerating { get; set; }
        public bool IsLoading { get; set; }

        public List<DayCell> DaysViews
        {
            get => DaysViewsProperty;
            set => DaysViewsProperty = value;
        }

        /// <summary>
        ///     Gets or sets the current date.
        /// </summary>
        /// <value>The current date attributes.</value>
        public DateTime CurrentMonth
        {
            get => (DateTime) GetValue(CurrentMonthProperty);
            set => SetValue(CurrentMonthProperty, value);
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
        ///     Gets or sets the current date.
        /// </summary>
        /// <value>The current date attributes.</value>
        public DayOfWeek FirstDay
        {
            get => (DayOfWeek) GetValue(FirstDayProperty);
            set => SetValue(FirstDayProperty, value);
        }

        /// <summary>
        ///     Gets or sets the day template.
        /// </summary>
        /// <value>The day template attributes.</value>
        public ControlTemplate DayTemplate
        {
            get => (ControlTemplate) GetValue(DayTemplateProperty);
            set => SetValue(DayTemplateProperty, value);
        }

        /// <summary>
        ///     Gets or sets the Skeleton Content.
        /// </summary>
        /// <value>The Background Content.</value>
        public View SkeletonView
        {
            get => (View) GetValue(SkeletonViewProperty);
            set => SetValue(SkeletonViewProperty, value);
        }

        /// <summary>
        ///     Gets or sets the current date.
        /// </summary>
        /// <value>The current date attributes.</value>
        public CalendarMode CalendarMode
        {
            get => (CalendarMode) GetValue(CalendarModeProperty);
            set => SetValue(CalendarModeProperty, value);
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

        private void Clicked_Tapped(object sender, EventArgs e)
        {
            if (ClickedDay != null && sender is DayCell cell)
            {
                var eventArg = new SelectedItemChangedEventArgs(cell.BindingContext, cell.Index);
                ClickedDay?.Invoke(cell, eventArg);
            }
        }

        public async Task GenerateMonthView()
        {
            if (IsGenerating)
                return;
            IsGenerating = true;

            await Task.Run(() =>
            {
                try
                {
                    _grid = new Grid
                    {
                        ColumnSpacing = 0,
                        RowSpacing = 0
                    };
                    _grid.SetBinding(BackgroundColorProperty,
                        new Binding(nameof(BackgroundColor)) {Source = this, Mode = BindingMode.OneWay});

                    var dates = Enumerable.Range(0, 6 * 7)
                        .Select(d => new DateTime())
                        .ToList();
                    var itemTemplate = new DataTemplate(() =>
                    {
                        if (DayTemplate.CreateContent() is DayCell day)
                        {
                            day.Index = DaysViews.Count;
                            Grid.SetColumn(day, day.Index % 7);
                            Grid.SetRow(day, day.Index / 7 % 6);

                            DaysViews.Add(day);
                            var clicked = new TapGestureRecognizer();
                            clicked.Tapped += Clicked_Tapped;
                            day.GestureRecognizers.Add(clicked);

                            return day;
                        }
                        else
                        {
                            throw new InvalidOperationException("DayTemplate must be either a DayCell");
                        }
                    });

                    BindableLayout.SetItemTemplate(_grid, itemTemplate);
                    BindableLayout.SetItemsSource(_grid, dates);


                    //_grid.ColumnDefinitions =
                    //    new ColumnDefinitionCollection
                    //    {
                    //        new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)},
                    //        new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)},
                    //        new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)},
                    //        new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)},
                    //        new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)},
                    //        new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)},
                    //        new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)}
                    //    };

                    //// 6 weeks max in a month
                    //for (var i = 0; i < 6; ++i)
                    //{
                    //    _grid.RowDefinitions.Add(new RowDefinition {Height = 50});
                    //    // 7 days in a weeks
                    //    for (var j = 0; j < 7; ++j)
                    //        if (DayTemplate != null)
                    //        {
                    //            if (DayTemplate.CreateContent() is DayCell day)
                    //            {
                    //                day.Index = i * j;
                    //                Grid.SetColumn(day, j);
                    //                Grid.SetRow(day, i);
                    //                DaysViews.Add(day);
                    //                _grid.Children.Add(day);

                    //                var clicked = new TapGestureRecognizer();
                    //                clicked.Tapped += Clicked_Tapped;
                    //                day.GestureRecognizers.Add(clicked);
                    //            }
                    //            else
                    //            {
                    //                throw new InvalidOperationException("DayTemplate must be either a DayCell");
                    //            }
                    //        }
                    //}

                    Device.InvokeOnMainThreadAsync(() =>
                    {
                        Content = _grid;
                        IsGenerating = false;
                        LoadDays();

                        ////SkeletonView.IsVisible = false;
                        //SkeletonView.Opacity = 0.0;
                        //Grid.SetColumn(SkeletonView, 0);
                        //Grid.SetRow(SkeletonView, 0);
                        //Grid.SetColumnSpan(SkeletonView, 7);
                        //Grid.SetRowSpan(SkeletonView, 8);
                        //_grid.Children.Add(SkeletonView);
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });
        }

        public bool LoadDays()
        {
            if (IsGenerating || IsLoading || _grid == null) return false;

            IsLoading = true;

            if (CalendarMode == CalendarMode.Tab)
            {
                _grid.Opacity = 0.0;
                var a = new Animation
                {
                    {0.2, 1, new Animation(f => _grid.Opacity = f, 0.0, 1.0, Easing.Linear)}
                };
                a.Commit(this, "MonthAnimation", 16, 300);
            }

            var dates = GetDates();
            for (var i = 0; i < DaysViews.Count(); ++i)
            {
                var dayView = DaysViews[i];
                if (i < dates.Count())
                {
                    var date = dates[i];
                    var outOfMonth = date.Month != CurrentMonth.Month;

                    if (MarkedDates != null && MarkedDates.ContainsKey(date))
                        dayView.BindingContext = MarkedDates[date];
                    else
                        dayView.BindingContext = null;

                    dayView.OutOfMonth = outOfMonth;
                    dayView.Date = date;
                }
                else
                {
                    dayView.OutOfMonth = true;
                    dayView.BindingContext = null;
                    dayView.Date = new DateTime();
                }
            }

            IsLoading = false;
            return true;
        }

        private List<DateTime> GetDates()
        {
            var begin = CurrentMonth.GetFirstDayOfMonth().GetFirstDayOfWeek(FirstDay);
            var end = CurrentMonth.GetLastDayOfMonth().GetLastDayOfWeek(FirstDay);
            var numberOfDays = Convert.ToInt32((end - begin).TotalDays);

            return Enumerable.Range(0, numberOfDays)
                .Select(day => begin.AddDays(day))
                .ToList();
        }

        /// <summary>
        ///     The the current date property changed.
        /// </summary>
        /// <param name="bindable">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void CurrentMonthChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Month month)
                if (month.CalendarMode != CalendarMode.List)
                    month.LoadDays();
        }

        /// <summary>
        ///     The First Day of week property changed.
        /// </summary>
        /// <param name="bindable">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void FirstDayChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Month month) month.LoadDays();
        }

        /// <summary>
        ///     The the current date property changed.
        /// </summary>
        /// <param name="bindable">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void DayTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Month month && newValue is ControlTemplate template)
                month.GenerateMonthView();
        }

        private static void SkeletonViewChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Month month && month.IsGenerating && newValue is View skeleton)
            {
                skeleton.InputTransparent = true;
                month.Content = skeleton;
                month.Content.FadeTo(1.0, 50);
            }
        }

        /// <summary>
        ///     The marked dates property changed.
        /// </summary>
        /// <param name="bindable">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void MarkedDatesChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is Month month)) return;

            if (oldValue is CalendarDictionary<DateTime, object> oldDic)
                oldDic.ClearCollectionChangedHandler();

            if (newValue is CalendarDictionary<DateTime, object> newDic)
            {
                newDic.CollectionChanged += (sender, e) => month.MarkedDatesCollectionChanged(e);
                month.UpdateAllMarkers();
            }
        }

        private void MarkedDatesCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (KeyValuePair<DateTime, object> kvp in e.OldItems)
                    if (DaysViews.Find(v => v.Date == kvp.Key) is DayCell d)
                        d.BindingContext = null;

            if (e.NewItems != null)
                foreach (KeyValuePair<DateTime, object> kvp in e.NewItems)
                    if (DaysViews.Find(v => v.Date == kvp.Key) is DayCell d)
                        d.BindingContext = kvp.Value;
        }

        public void UpdateAllMarkers()
        {
            try
            {
                if (DaysViews != null && MarkedDates != null)
                    foreach (var view in DaysViews)
                        if (MarkedDates.ContainsKey(view.Date))
                            view.BindingContext = MarkedDates[view.Date];
                        else
                            view.BindingContext = null;
            }
            catch
            {
                UpdateAllMarkers();
            }
        }
    }
}