using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Global.CalendarView.Controls;
using Global.InputForms.Models;
using Naxam.I18n;
using Naxam.I18n.Forms;
using SampleApp.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SampleApp.ViewModels
{
    public class CalendarViewModel : BaseViewModel
    {
        public DateTime _beginRange;
        private DateTime _currentDate = DateTime.Today;
        public DateTime _endRange;
        public CalendarDictionary<DateTime, object> _markedDates;
        private DateTime _maxDate = DateTime.Today.AddYears(1);

        private DateTime _minDate = DateTime.Today;

        public CalendarViewModel()
        {
            MarkedDates = new CalendarDictionary<DateTime, object>();
            if (!MarkedDates.ContainsKey(DateTime.Today))
            {
                var obj = new MyDayViewModel
                {
                    Date = DateTime.Today
                };
                MarkedDates.Add(DateTime.Today, obj);
            }
        }

        public CalendarDictionary<DateTime, object> MarkedDates
        {
            get => _markedDates;
            set => SetProperty(ref _markedDates, value);
        }

        public DateTime MinDate
        {
            get => _minDate;
            set => SetProperty(ref _minDate, value);
        }

        public DateTime MaxDate
        {
            get => _maxDate;
            set => SetProperty(ref _maxDate, value);
        }

        public DateTime CurrentDate
        {
            get => _currentDate;
            set => SetProperty(ref _currentDate, value);
        }

        public DateTime BeginRange
        {
            get => _beginRange;
            set
            {
                SetProperty(ref _beginRange, value);
                ReloadRange();
            }
        }

        public DateTime EndRange
        {
            get => _endRange;
            set
            {
                SetProperty(ref _endRange, value);
                ReloadRange();
            }
        }

        private List<DateTime> GetRangeDates()
        {
            if (BeginRange != default && EndRange != default)
            {
                var numberOfDays = Convert.ToInt32((EndRange - BeginRange).TotalDays + 1);

                return Enumerable.Range(0, numberOfDays)
                    .Select(day => BeginRange.AddDays(day))
                    .ToList();
            }

            return new List<DateTime>();
        }

        public void ReloadRange()
        {
            Task.Run(() =>
            {
                try
                {
                    var markedDatesDic = new CalendarDictionary<DateTime, object>();

                    var allDates = GetRangeDates();
                    //Add MyMission to Markers
                    foreach (var date in allDates)
                    {
                        var obj = new MyDayViewModel
                        {
                            Date = date,
                            Begin = BeginRange,
                            End = EndRange
                        };
                        markedDatesDic.Add(date, obj);
                    }

                    //Add Today to markers
                    if (!markedDatesDic.ContainsKey(DateTime.Today))
                    {
                        var obj = new MyDayViewModel
                        {
                            Date = DateTime.Today
                        };
                        markedDatesDic.Add(DateTime.Today, obj);
                    }

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        MarkedDates = markedDatesDic;
                        IsBusy = false;
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    IsBusy = false;
                }
            });
        }
    }
}