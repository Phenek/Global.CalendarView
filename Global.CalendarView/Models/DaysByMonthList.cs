using System;
using System.Collections.Generic;

namespace Global.CalendarView
{
    public class ListedDaysPerMonth : List<DateTime>
    {
        public DateTime Month;

        public ListedDaysPerMonth(DateTime month)
        {
            Month = month;
        }
    }
}
