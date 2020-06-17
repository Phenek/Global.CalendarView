using Global.CalendarView.Controls;
using Global.CalendarView.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CalendarCollection), typeof(CalendarCollectionRenderer))]

namespace Global.CalendarView.iOS.Renderers
{
    public class CalendarCollectionRenderer : CollectionViewRenderer
    {
    }
}