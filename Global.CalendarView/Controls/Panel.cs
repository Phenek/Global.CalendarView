using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Global.CalendarView.Controls
{
    public class Panel : Grid
    {
        /// <summary>
        ///     The current date property.
        /// </summary>
        public static readonly BindableProperty CurrentDateProperty =
            BindableProperty.Create(nameof(CurrentDate), typeof(DateTime), typeof(Panel), DateTime.Today,
                propertyChanged: DatesChanged);

        /// <summary>
        ///     The min date property.
        /// </summary>
        public static readonly BindableProperty MinDateProperty =
            BindableProperty.Create(nameof(MinDate), typeof(DateTime), typeof(Panel), DateTime.Today.AddYears(-2),
                propertyChanged: DatesChanged);

        /// <summary>
        ///     The max date property.
        /// </summary>
        public static readonly BindableProperty MaxDateProperty =
            BindableProperty.Create(nameof(MaxDate), typeof(DateTime), typeof(Panel), DateTime.Today.AddYears(+2),
                propertyChanged: DatesChanged);

        /// <summary>
        ///     The background content property.
        /// </summary>
        public static readonly BindableProperty LeftViewProperty = BindableProperty.Create(nameof(LeftView),
            typeof(View), typeof(Panel), null, propertyChanged: LeftViewChanged);

        /// <summary>
        ///     The background content property.
        /// </summary>
        public static readonly BindableProperty MiddleViewProperty = BindableProperty.Create(nameof(MiddleView),
            typeof(View), typeof(Panel), null, propertyChanged: MiddleViewChanged);

        /// <summary>
        ///     The background content property.
        /// </summary>
        public static readonly BindableProperty RightViewProperty = BindableProperty.Create(nameof(RightView),
            typeof(View), typeof(Panel), null, propertyChanged: RightViewChanged);

        private bool _isBusy;
        private readonly TapGestureRecognizer _leftClicked = new TapGestureRecognizer();
        private readonly TapGestureRecognizer _rightClicked = new TapGestureRecognizer();

        public Panel()
        {
            RowDefinitions.Add(new RowDefinition());
            ColumnDefinitions.Add(new ColumnDefinition {Width = GridLength.Auto});
            ColumnDefinitions.Add(new ColumnDefinition {Width = GridLength.Star});
            ColumnDefinitions.Add(new ColumnDefinition {Width = GridLength.Auto});

            _leftClicked.Tapped += LeftClicked;
            _rightClicked.Tapped += RightClicked;
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
        ///     Gets or sets the Background Content.
        /// </summary>
        /// <value>The Background Content.</value>
        public View LeftView
        {
            get => (View) GetValue(LeftViewProperty);
            set => SetValue(LeftViewProperty, value);
        }

        /// <summary>
        ///     Gets or sets the Background Content.
        /// </summary>
        /// <value>The Background Content.</value>
        public View MiddleView
        {
            get => (View) GetValue(MiddleViewProperty);
            set => SetValue(MiddleViewProperty, value);
        }

        /// <summary>
        ///     Gets or sets the Background Content.
        /// </summary>
        /// <value>The Background Content.</value>
        public View RightView
        {
            get => (View) GetValue(RightViewProperty);
            set => SetValue(RightViewProperty, value);
        }

        public event EventHandler ClickedLeft;
        public event EventHandler ClickedRight;

        private void RightClicked(object sender, EventArgs e)
        {
            ChangeMonth(+1);
            ClickedRight?.Invoke(this, null);
        }

        private void LeftClicked(object sender, EventArgs e)
        {
            ChangeMonth(-1);
            ClickedLeft?.Invoke(this, null);
        }

        private static void DatesChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Panel panel) panel.ChangePanelState();
        }

        private static void LeftViewChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Panel panel)
            {
                if (oldValue is View oldView)
                {
                    panel.Children.Remove(oldView);
                    oldView.GestureRecognizers.Add(panel._leftClicked);
                }

                if (newValue is View newView)
                {
                    SetColumn(newView, 0);
                    panel.Children.Add(newView);
                    newView.GestureRecognizers.Add(panel._leftClicked);
                    panel.ChangePanelState();
                }
            }
        }

        private static void RightViewChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Panel panel)
            {
                if (oldValue is View oldView)
                {
                    panel.Children.Remove(oldView);
                    oldView.GestureRecognizers.Add(panel._rightClicked);
                }

                if (newValue is View newView)
                {
                    SetColumn(newView, 2);
                    panel.Children.Add(newView);
                    newView.GestureRecognizers.Add(panel._rightClicked);
                    panel.ChangePanelState();
                }
            }
        }

        private static void MiddleViewChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Panel panel)
            {
                if (oldValue is View oldView)
                {
                    panel.Children.Remove(oldView);
                    oldView.RemoveBinding(BindingContextProperty);
                }

                if (newValue is View newView)
                {
                    SetColumn(newView, 1);
                    newView.SetBinding(BindingContextProperty,
                        new Binding(nameof(CurrentDate)) {Source = panel, Mode = BindingMode.OneWay});
                    panel.Children.Add(newView);
                }
            }
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
            if (LeftView != null)
                LeftView.BindingContext = LeftView.IsEnabled = CurrentDate.AddMonths(-1).GetLastDayOfMonth() >= MinDate;
            if (RightView != null)
                RightView.BindingContext = RightView.IsEnabled = CurrentDate.AddMonths(1).GetFirstDayOfMonth() <= MaxDate;
        }
    }
}