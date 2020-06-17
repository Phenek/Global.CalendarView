using Xamarin.Forms;

namespace SampleApp.Controls
{
    public class SkeletonView : BoxView
    {
        /// <summary>
        ///     The Entry Title property.
        /// </summary>
        public static readonly BindableProperty AnimatedProperty =
            BindableProperty.Create(nameof(Animated), typeof(bool), typeof(SkeletonView), true,
                propertyChanged: AnimatedChanged);

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

            if (Animated)
                Device.BeginInvokeOnMainThread(() =>
                    smoothAnimation.Commit(this, "SmoothAnimation", 16, 2000, Easing.Linear, null, () => true));
        }

        /// <summary>
        ///     Gets or sets the Animated Property.
        /// </summary>
        /// <value>The entry text.</value>
        public bool Animated
        {
            get => (bool) GetValue(AnimatedProperty);
            set => SetValue(AnimatedProperty, value);
        }

        private static void AnimatedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SkeletonView v)
                if (v.Animated)
                {
                    var smoothAnimation = new Animation
                    {
                        {0, 0.5, new Animation(f => v.Opacity = f, 1, 0.2, Easing.Linear)},
                        {0.5, 1, new Animation(f => v.Opacity = f, 0.2, 1, Easing.Linear)}
                    };
                    Device.BeginInvokeOnMainThread(() =>
                        smoothAnimation.Commit(v, "SmoothAnimation", 16, 2000, Easing.Linear, null, () => true));
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() => v.AbortAnimation("SmoothAnimation"));
                }
        }
    }
}