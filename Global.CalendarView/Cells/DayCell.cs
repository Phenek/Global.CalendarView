using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Global.CalendarView.Controls
{
    public abstract class DayCell : ContentView, INotifyPropertyChanged
    {
        private DateTime _date;

        private int _index;

        private bool _outOfMonth;

        public DateTime Date
        {
            get => _date;
            set
            {
                if (_date != value)
                {
                    SetProperty(ref _date, value);
                    DateChanged.Invoke(this, new EventArgs());
                }
            }
        }

        public bool OutOfMonth
        {
            get => _outOfMonth;
            set => SetProperty(ref _outOfMonth, value);
        }

        public int Index
        {
            get => _index;
            set => SetProperty(ref _index, value);
        }

        public event EventHandler DateChanged;

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

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext is DateTime date)
                Date = date;
        }
    }
}