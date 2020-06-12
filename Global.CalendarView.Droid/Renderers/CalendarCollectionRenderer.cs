using System;
using System.Collections.Generic;
using Android.Content;
using Global.CalendarView.Controls;
using Global.CalendarView.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using System.Collections;
using System.ComponentModel;
#if __ANDROID_29__
using AndroidX.RecyclerView.Widget;
#else
using Android.Support.V7.Widget;
#endif
using Android.Views;
using Java.Interop;
using FormsCarouselView = Xamarin.Forms.CarouselView;
using Xamarin.Forms.Platform.Android.CollectionView;

[assembly: Xamarin.Forms.ExportRenderer(typeof(CalendarCollection), typeof(CalendarCollectionRenderer))]
namespace Global.CalendarView.Droid.Renderers
{
    public class CalendarCollectionRenderer : CollectionViewRenderer
    {
		CalendarCollection CalendarCollection;

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
