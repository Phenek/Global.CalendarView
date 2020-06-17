using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Global.CalendarView.Controls
{
    public class WeekDays : Grid
    {
        /// <summary>
        ///     The first day of week property.
        /// </summary>
        public static readonly BindableProperty FirstDayProperty =
            BindableProperty.Create(nameof(FirstDay), typeof(DayOfWeek), typeof(Calendar), DayOfWeek.Sunday,
                propertyChanged: FirstDayChanged);

        /// <summary>
        ///     The week label day template property.
        /// </summary>
        public static readonly BindableProperty WeekDayTemplateProperty =
            BindableProperty.Create(nameof(WeekDayTemplate), typeof(ControlTemplate), typeof(Calendar),
                null, propertyChanged: WeekDayTemplateChanged); //, validateValue: ValidateDayTemplate);

        private List<View> _daysOfWeek;

        public WeekDays(DayOfWeek firstDay)
        {
            ColumnSpacing = 0;
            RowSpacing = 0;

            FirstDay = firstDay;
            GenerateWeekView();
        }

        public WeekDays()
        {
            ColumnSpacing = 0;
            RowSpacing = 0;

            GenerateWeekView();
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
        public ControlTemplate WeekDayTemplate
        {
            get => (ControlTemplate) GetValue(WeekDayTemplateProperty);
            set => SetValue(WeekDayTemplateProperty, value);
        }

        public void GenerateWeekView()
        {
            Children.Clear();
            ColumnDefinitions.Clear();

            if (WeekDayTemplate == null)
            {
                _daysOfWeek = GetDaysOfWeek()
                    .Select(dayDate => new Label
                    {
                        BindingContext = dayDate,
                        TextColor = Color.Gray,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 16
                    }).ToList<View>();
                foreach (var d in _daysOfWeek)
                    d.SetBinding(Label.TextProperty,
                        new Binding(nameof(BindingContext))
                            {Source = d, Mode = BindingMode.OneWay, StringFormat = "ddd"});
            }
            else
            {
                _daysOfWeek = new List<View>();
                foreach (var d in GetDaysOfWeek())
                {
                    if (!(WeekDayTemplate.CreateContent() is View day)) return;

                    day.BindingContext = d;
                    _daysOfWeek.Add(day);
                }
            }

            for (var i = 0; i < _daysOfWeek.Count(); ++i)
            {
                ColumnDefinitions.Add(new ColumnDefinition {Width = GridLength.Star});
                SetColumn(_daysOfWeek[i], i);
                Children.Add(_daysOfWeek[i]);
            }
        }

        /// <summary>
        ///     The First Day of week property changed.
        /// </summary>
        /// <param name="bindable">The object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void FirstDayChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is WeekDays weekDays)
            {
                var WeekDates = weekDays.GetDaysOfWeek();

                for (var i = 0; i < weekDays._daysOfWeek.Count(); ++i)
                    weekDays._daysOfWeek[i].BindingContext = WeekDates[i];
            }
        }

        private static void WeekDayTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is WeekDays weekDays && newValue is ControlTemplate template) weekDays.GenerateWeekView();
        }

        private List<DateTime> GetDaysOfWeek()
        {
            var begin = DateTime.Today.GetFirstDayOfWeek(FirstDay);

            return Enumerable.Range(0, 7)
                .Select(day => begin.AddDays(day))
                .ToList();
        }
    }
}