using System;
using System.Collections.Generic;
using System.ComponentModel;
using Global.CalendarView.Controls;
using Xamarin.Forms;

namespace SampleApp.Views
{
    public partial class DayCell : Global.CalendarView.Controls.DayCell
    {
        public DayCell()
        {
            InitializeComponent();

            DateChanged += DatePropertyChanged;
        }

        private void DatePropertyChanged(object sender, EventArgs e)
        {
            _label.TextColor = Date < DateTime.Today ? Color.LightGray : Date == DateTime.Today
                ? Color.White
                : Color.Black;
        }
    }
}
