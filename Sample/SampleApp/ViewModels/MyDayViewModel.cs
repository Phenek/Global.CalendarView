using System;
using Xamarin.Forms;

namespace SampleApp.ViewModels
{
    public class MyDayViewModel : BaseViewModel
    {
        public DateTime _date;
        public DateTime Begin;
        public DateTime End;

        public Color FrameColor => Date.Date == Begin.Date || Date.Date == End.Date
            ? (Color) Application.Current.Resources["PrimaryColor"]
            : Date.Date > Begin.Date && Date.Date < End.Date
                ? Color.Salmon
                : Color.Transparent;

        public Color DayColor => Date == DateTime.Today ? Color.Black : Color.Transparent;

        public Thickness FrameMargin
        {
            get
            {
                var begin = Date.Date == Begin.Date;
                var end = Date.Date == End.Date;

                var left = -5.0;
                var right = -5.0;
                if (begin) left = 1;
                if (end) right = 1;

                return new Thickness(left, 1, right, 1);
            }
        }

        public DateTime Date
        {
            get => _date;
            set
            {
                SetProperty(ref _date, value);
                OnPropertyChanged(nameof(FrameMargin));
                OnPropertyChanged(nameof(FrameColor));
            }
        }
    }
}