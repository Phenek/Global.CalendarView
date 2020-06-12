using System;
using Global.CalendarView.Controls;
using Global.CalendarView.iOS.Renderers;
using Xamarin.Forms.Platform.iOS;

[assembly: Xamarin.Forms.ExportRenderer(typeof(CalendarCollection), typeof(CalendarCollectionRenderer))]
namespace Global.CalendarView.iOS.Renderers
{
    public class CalendarCollectionRenderer : CollectionViewRenderer
    {
        public CalendarCollectionRenderer()
        {
        }
    }
}