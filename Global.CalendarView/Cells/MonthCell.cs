using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Global.CalendarView.Models;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Global.CalendarView.Controls
{
    public class MonthCell : ContentView, INotifyPropertyChanged
    {
        public int VisibleIndex;
        public Month MonthControl;
        private CalendarList _calendarList;

        public MonthCell()
        {
            
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            if (Parent != null && _calendarList == null)
            {
                _calendarList = GetCalendarList(this);
                if (_calendarList != null)
                {
                    Content = CreateMonthTemplatedView();

                    MonthControl = GetChildOfTypeMonth(Content);
                    MonthControl.ClickedDay += _calendarList.PropagateClickedDate;

                    BindToCalendarList(_calendarList);
                }
            }
        }

        public View CreateMonthTemplatedView()
        {
            if (_calendarList.MonthTemplate != null)
            {
                if (_calendarList.MonthTemplate.CreateContent() is View monthView)
                {
                    return monthView;
                }
                else
                {
                    throw new InvalidOperationException("MonthTemplate must be either a MonthCell");
                }
            }
            return null;
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
            base.OnBindingContextChanged();

            if (MonthControl != null && BindingContext is KeyValuePair<int, DateTime> kvp)
            {
                VisibleIndex = kvp.Key;
                MonthControl.CurrentDate = kvp.Value;
            }
        }

        private void BindToCalendarList(CalendarList parent)
        {
            if (MonthControl != null && BindingContext is KeyValuePair<int, DateTime> kvp)
            {
                VisibleIndex = kvp.Key;
                MonthControl.CurrentDate = kvp.Value;
            }

            MonthControl.SkeletonDisplayMode = SkeletonDisplayMode.WhenDaysLoad;
            MonthControl.SetBinding(Month.MinDateProperty,
                new Binding(nameof(CalendarList.MinDate)) { Source = parent, Mode = BindingMode.OneWay });
            MonthControl.SetBinding(Month.MaxDateProperty,
                new Binding(nameof(CalendarList.MaxDate)) { Source = parent, Mode = BindingMode.OneWay });
            MonthControl.SetBinding(Month.MarkedDatesProperty,
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
