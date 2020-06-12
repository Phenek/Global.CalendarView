using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Global.CalendarView.Controls
{
    public class CalendarCollection : CollectionView
    {
        /// <summary>
        ///     The Visible views property.
        /// </summary>
        public static readonly BindableProperty VisibleViewsProperty =
            BindableProperty.Create(nameof(VisibleViews), typeof(ObservableCollection<View>), typeof(Calendar),
                new ObservableCollection<View>());

        public ObservableCollection<View> VisibleViews => (ObservableCollection<View>)GetValue(VisibleViewsProperty);

        public CalendarCollection()
        {
        }
    }
}
