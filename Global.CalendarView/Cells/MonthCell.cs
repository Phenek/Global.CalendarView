using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Global.CalendarView.Models;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Global.CalendarView.Controls
{
    public class MonthCell : ContentView, INotifyPropertyChanged
    {
        private Grid _grid;
        private View _skeleton;
        private View _monthView;
        private Month _monthControl;
        private CalendarList _calendarList;

        public MonthCell(CalendarList calendarList)
        {
            _calendarList = calendarList;

            Content = _grid = new Grid()
            {
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = calendarList.TemplateViewHeight,
                MinimumHeightRequest = calendarList.TemplateViewHeight,
            };

            _skeleton = CreateSkeletonTemplatedView();

            Grid.SetColumn(_skeleton, 0);
            Grid.SetRow(_skeleton, 0);
            Grid.SetColumnSpan(_skeleton, 1);
            Grid.SetRowSpan(_skeleton, 1);

            _grid.Children.Add(_skeleton);

            Task.Run(() =>
            {
                _monthView = CreateMonthTemplatedView();
                _monthControl = GetChildOfTypeMonth(_monthView);
                _monthControl.ClickedDay += _calendarList.PropagateClickedDate;

                Grid.SetColumn(_monthView, 0);
                Grid.SetRow(_monthView, 0);
                Grid.SetColumnSpan(_monthView, 1);
                Grid.SetRowSpan(_monthView, 1);
                Device.BeginInvokeOnMainThread(() =>
                {
                    BindMonthControlToCalendarList(_calendarList);
                    _grid.Children.Insert(0,_monthView);
                });
            });
        }

        public bool IsLoaded { get; set; }

        private static void IsScrollingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is Month month)) return;
            if ((bool)oldValue == true && (bool)newValue == false && !month.IsGenerating && !month.IsLoading)
            {
                //var begin = month.CurrentDate.GetFirstDayOfMonth().GetFirstDayOfWeek(month.FirstDay);
                //if (month.DaysViews.Any() && begin != month.DaysViews[0].Date)
                //    month.LoadDays();
            }
        }

        public View CreateMonthTemplatedView()
        {
            if (_calendarList.MonthTemplate != null)
            {
                if (_calendarList.MonthTemplate.CreateContent() is View monthView)
                {
                    monthView.HeightRequest = _calendarList.TemplateViewHeight;
                    return monthView;
                }
                else
                {
                    throw new InvalidOperationException("MonthTemplate must be either a MonthCell");
                }
            }
            return null;
        }

        public View CreateSkeletonTemplatedView()
        {
            if (_calendarList.SkeletonTemplate != null)
            {
                if (_calendarList.SkeletonTemplate.CreateContent() is View skeletonView)
                {
                    skeletonView.HeightRequest = _calendarList.TemplateViewHeight;
                    return skeletonView;
                }
                else
                {
                    throw new InvalidOperationException("MonthTemplate must be either a View");
                }
            }
            return null;
        }

        public void LoadCell()
        {
            if (_monthControl == null) return;

            var begin = _monthControl.CurrentMonth.GetFirstDayOfMonth().GetFirstDayOfWeek(_monthControl.FirstDay);
            if (_monthControl.DaysViews.Any() && begin != _monthControl.DaysViews[0].Date)
            {
                _monthControl.LoadDays();
            }

            if (_skeleton.Opacity == 1.0)
            {
                //_monthView.IsVisible = true;
                _skeleton.InputTransparent = true;
                var a = new Animation
                {
                    {0.2, 1, new Animation(f => _skeleton.Opacity = f, 1.0, 0.0, Easing.Linear)}
                };
                a.Commit(this, "MonthAnimation", 16, 100);//, finished: (d, b) => _skeleton.IsVisible = false);
            }

            IsLoaded = true;
        }

        public CalendarList GetCalendarList(VisualElement element)
        {
            if (element != null)
            {
                var parent = element.Parent;
                while (parent != null)
                {
                    if (parent is CalendarList calendarList)
                    {
                        return calendarList;
                    }
                    parent = parent.Parent;
                }
            }
            return null;
        }

        protected override void OnBindingContextChanged()
        {
            IsLoaded = false;

            base.OnBindingContextChanged();

            //if (BindingContext == null) return;

            if (_skeleton != null)
            {

                //_skeleton.IsVisible = true;
                _skeleton.InputTransparent = false;
                _skeleton.Opacity = 1.0;
            }
            if (_monthView != null)
            {
                //_monthView.IsVisible = false;
            }
        }

        private void BindMonthControlToCalendarList(CalendarList parent)
        {
            if (_monthControl == null) return;

            _monthControl.CalendarMode = CalendarMode.List;
            _monthControl.SetBinding(Month.MinDateProperty,
                new Binding(nameof(CalendarList.MinDate)) { Source = parent, Mode = BindingMode.OneWay });
            _monthControl.SetBinding(Month.MaxDateProperty,
                new Binding(nameof(CalendarList.MaxDate)) { Source = parent, Mode = BindingMode.OneWay });
            _monthControl.SetBinding(Month.MarkedDatesProperty,
                new Binding(nameof(CalendarList.MarkedDates)) { Source = parent, Mode = BindingMode.OneWay });
        }

        private Month GetChildOfTypeMonth(View mainView)
        {
            if (mainView is Month theOne)
                return theOne;
            else if (mainView?.GetType()?.GetProperty("Content")?.GetValue(mainView) is View content)
            {
                var res = GetChildOfTypeMonth(content);
                if (res != null) return res;
            }
            if (mainView is Layout<View> layout)
            {
                foreach (var v in layout.Children)
                {
                    var res = GetChildOfTypeMonth(v);
                    if (res != null) return res;
                };
            }
            return null;
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "",
        Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
