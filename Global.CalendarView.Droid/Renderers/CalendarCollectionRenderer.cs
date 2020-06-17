#if __ANDROID_29__
using AndroidX.RecyclerView.Widget;
#else
#endif
using Android.Content;
using Global.CalendarView.Controls;
using Global.CalendarView.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using FormsCarouselView = Xamarin.Forms.CarouselView;

[assembly: ExportRenderer(typeof(CalendarCollection), typeof(CalendarCollectionRenderer))]

namespace Global.CalendarView.Droid.Renderers
{
    public class CalendarCollectionRenderer : CollectionViewRenderer
    {
        private CalendarCollection CalendarCollection;

        public CalendarCollectionRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ItemsView> elementChangedEvent)
        {
            base.OnElementChanged(elementChangedEvent);

            if (Element is CalendarCollection calendarCollection)
                CalendarCollection = calendarCollection;
        }

        //      void UpdateVisualStates()
        //{
        //	if (!(GetLayoutManager() is LinearLayoutManager layoutManager))
        //		return;

        //	var first = layoutManager.FindFirstVisibleItemPosition();
        //	var last = layoutManager.FindLastVisibleItemPosition();

        //	if (first == -1)
        //		return;

        //	for (int i = first; i <= last; i++)
        //	{
        //		var cell = layoutManager.FindViewByPosition(i);
        //		if (cell is ItemContentView icv && icv.VisualElementRenderer)
        //		if (!((cell as ItemContentView)?.VisualElementRenderer?.Element is View itemView))
        //			return;

        //		if (!CalendarCollection.VisibleViews.Contains(itemView))
        //		{
        //			CalendarCollection.VisibleViews.Add(itemView);
        //		}
        //	}
        //}
    }
}