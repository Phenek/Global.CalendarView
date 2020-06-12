using System;
using Xamarin.Forms;

namespace SampleApp.Controls
{
    public class SkeletonView : BoxView
    {
        public SkeletonView()
        {
            CornerRadius = 6;
            BackgroundColor = Color.FromHex("#cccccc");
            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.Fill;

            var smoothAnimation = new Animation
            {
                {0, 0.5, new Animation(f => Opacity = f, 1, 0.2, Easing.Linear)},
                {0.5, 1, new Animation(f => Opacity = f, 0.2, 1, Easing.Linear)}
            };

            Device.BeginInvokeOnMainThread(() => smoothAnimation.Commit(this, "SmoothAnimation", 16, 2000, Easing.Linear, null, () => true));

        }
    }
}
