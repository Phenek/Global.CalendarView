using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Global.CalendarView.Controls
{
    public class Calendar : StackLayout
    {
        /// <summary>
        ///     The current date property.
        /// </summary>
        public static readonly BindableProperty CurrentDateProperty =
            BindableProperty.Create(nameof(CurrentDate), typeof(DateTime), typeof(Calendar), DateTime.Today,
                propertyChanged: CurrentDateChanged);

        /// <summary>
        ///     The min date property.
        /// </summary>
        public static readonly BindableProperty MinDateProperty =
            BindableProperty.Create(nameof(MinDate), typeof(DateTime), typeof(Calendar), default(DateTime),
                propertyChanged: MinDateChanged);

        /// <summary>
        ///     The max date property.
        /// </summary>
        public static readonly BindableProperty MaxDateProperty =
            BindableProperty.Create(nameof(MaxDate), typeof(DateTime), typeof(Calendar), default(DateTime),
                propertyChanged: MaxDateChanged);

        /// <summary>
        ///     The first day of week property.
        /// </summary>
        public static readonly BindableProperty FirstDayProperty =
            BindableProperty.Create(nameof(FirstDay), typeof(DayOfWeek), typeof(Calendar), DayOfWeek.Sunday,
                propertyChanged: FirstDayChanged);

        /// <summary>
        ///     The marked dates property.
        /// </summary>
        public static readonly BindableProperty MarkedDatesProperty =
            BindableProperty.Create(nameof(MarkedDates), typeof(CalendarDictionary<DateTime, object>), typeof(Calendar),
                new CalendarDictionary<DateTime, object>(), propertyChanged: MarkedDatesChanged);

        /// <summary>
        ///     The day template property.
        /// </summary>
        public static readonly BindableProperty DayTemplateProperty =
            BindableProperty.Create(nameof(DayTemplate), typeof(ControlTemplate), typeof(Calendar),
                default(ControlTemplate), propertyChanged: DayTemplateChanged); //, validateValue: ValidateDayTemplate);

        /// <summary>
        ///     The week label day template property.
        /// </summary>
        public static readonly BindableProperty WeekDayTemplateProperty =
            BindableProperty.Create(nameof(WeekDayTemplate), typeof(ControlTemplate), typeof(Calendar),
                null, propertyChanged: WeekDayTemplateChanged); //, validateValue: ValidateDayTemplate);

        private readonly Month _month;

        private readonly Grid _panel;
        private readonly WeekDays _weekDays;
        private bool _isBusy;

        private Label _left;
        private Label _monthLabel;
        private Label _right;

        public Calendar()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand;
            VerticalOptions = LayoutOptions.Fill;
            Spacing = 0;

            _panel = PanelBuilder();
            _weekDays = new WeekDays(FirstDay);
            _month = new Month(FirstDay, CurrentDate, MinDate, MaxDate, MarkedDates);

            Children.Add(_panel);
            Children.Add(_weekDays);
            Children.Add(_month);
        }

        private List<DateTime> DaysSourceProperty { get; set; }

        public List<DateTime> DaysSource
        {
            get => DaysSourceProperty;
            set => DaysSourceProperty = value;
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
        ///     Gets or sets the day template.
        /// </summary>
        /// <value>The day template attributes.</value>
        public ControlTemplate WeekDayTemplate
        {
            get => (ControlTemplate) GetValue(WeekDayTemplateProperty);
            set => SetValue(WeekDayTemplateProperty, value);
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

        private Grid PanelBuilder()
        {
            var panel = new Grid();
            panel.RowDefinitions.Add(new RowDefinition {Height = 80});
            panel.ColumnDefinitions.Add(new ColumnDefinition {Width = GridLength.Star});
            panel.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(2, GridUnitType.Star)});
            panel.ColumnDefinitions.Add(new ColumnDefinition {Width = GridLength.Star});

            _left = new Label
            {
                Text = "‹",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                BackgroundColor = Color.LightBlue,
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            var leftClicked = new TapGestureRecognizer();
            leftClicked.Tapped += (s, e) => ChangeMonth(-1);
            _left.GestureRecognizers.Add(leftClicked);

            _monthLabel = new Label
            {
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromHex("#ca005d"),
                Text = CurrentDate.ToString("MMMM\nyyyy")
            };

            _right = new Label
            {
                Text = "›",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                BackgroundColor = Color.LightBlue,
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            var rightClicked = new TapGestureRecognizer();
            rightClicked.Tapped += (s, e) => ChangeMonth(1);
            _right.GestureRecognizers.Add(rightClicked);

            Grid.SetColumn(_left, 0);
            Grid.SetColumn(_monthLabel, 1);
            Grid.SetColumn(_right, 2);
            panel.Children.Add(_left);
            panel.Children.Add(_monthLabel);
            panel.Children.Add(_right);

            return panel;
        }

        public async void ChangeMonth(int nb)
        {
            if (_isBusy)
                return;
            _isBusy = true;
            CurrentDate = CurrentDate.AddMonths(nb);
            ChangePanelState();
            await Task.Delay(50);
            _isBusy = false;
        }

        public void ChangePanelState()
        {
            if (CurrentDate.AddMonths(-1).GetLastDayOfMonth() < MinDate)
            {
                _left.IsEnabled = false;
                _left.Text = "-";
                _left.BackgroundColor = Color.LightGray;
            }
            else
            {
                _left.Text = "‹";
                _left.IsEnabled = true;
                _left.BackgroundColor = Color.LightBlue;
            }

            if (CurrentDate.AddMonths(1).GetFirstDayOfMonth() > MaxDate)
            {
                _right.IsEnabled = false;
                _right.Text = "-";
                _right.BackgroundColor = Color.LightGray;
            }
            else
            {
                _right.IsEnabled = true;
                _right.Text = "›";
                _right.BackgroundColor = Color.LightBlue;
            }
        }

        /// <summary>
        ///     The the current date property changed.
        /// </summary>
        /// <param name="bindable">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void CurrentDateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Calendar calendar && newValue is DateTime date)
            {
                calendar._month.CurrentMonth = date;
                calendar._monthLabel.Text = date.ToString("MMMM\nyyyy");
            }
        }

        /// <summary>
        ///     The the minimun date property changed.
        /// </summary>
        /// <param name="bindable">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void MinDateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Calendar calendar)
                Console.WriteLine("MinDateChanged");
        }

        /// <summary>
        ///     The the maximum date property changed.
        /// </summary>
        /// <param name="bindable">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void MaxDateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Calendar calendar)
                Console.WriteLine("MaxDateChanged");
        }

        /// <summary>
        ///     The First Day of week property changed.
        /// </summary>
        /// <param name="bindable">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void FirstDayChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Calendar calendar)
            {
                calendar._weekDays.FirstDay = (DayOfWeek) newValue;
                calendar._month.FirstDay = (DayOfWeek) newValue;
            }
        }

        /// <summary>
        ///     The the current date property changed.
        /// </summary>
        /// <param name="bindable">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void DayTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Calendar calendar && newValue is ControlTemplate template)
                calendar._month.DayTemplate = template;
        }

        private static void WeekDayTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Calendar calendar && newValue is ControlTemplate template)
                calendar._weekDays.WeekDayTemplate = template;
        }

        /// <summary>
        ///     The marked dates property changed.
        /// </summary>
        /// <param name="bindable">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void MarkedDatesChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Calendar calendar)
                calendar._month.MarkedDates = (CalendarDictionary<DateTime, object>) newValue;
        }
    }

    public static class DateTimeDayOfMonthExtensions
    {
        public static int DaysInMonth(this DateTime value)
        {
            return DateTime.DaysInMonth(value.Year, value.Month);
        }

        public static DateTime GetFirstDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        public static DateTime GetLastDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.DaysInMonth());
        }

        public static DateTime GetFirstDayOfWeek(this DateTime value, DayOfWeek firstDay)
        {
            var firstDayInWeek = value.Date;

            while (firstDayInWeek.DayOfWeek != firstDay) firstDayInWeek = firstDayInWeek.AddDays(-1);

            return firstDayInWeek.Date;
        }

        public static DateTime GetLastDayOfWeek(this DateTime value, DayOfWeek firstDay)
        {
            var firstDayInWeek = GetFirstDayOfWeek(value, firstDay);
            return firstDayInWeek.AddDays(7).Date;
        }
    }
}