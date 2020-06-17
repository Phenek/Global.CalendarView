using SampleApp.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SampleApp
{
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();

            var info = DeviceDisplay.MainDisplayInfo;

            var top = info.Height / info.Density / 3;
            _stackLayout.Margin = new Thickness(0, top, 0, 0);

            _logo.TranslationY = -info.Height / info.Density / 3;

            toCalendarTabBtn.Clicked += (sender, e) => { Navigation.PushAsync(new CalendarTabPage()); };
            toCalendarListBtn.Clicked += (sender, e) => { Navigation.PushAsync(new CalendarListPage()); };
            toCalendarList2Btn.Clicked += (sender, e) => { Navigation.PushAsync(new CalendarList2Page()); };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();


            var smoothAnimation = new Animation();

            //foreach (var)
            //{
            //    {0, 1, new Animation(f => _label.TranslationY = f, _label.TranslationY, translateY, Easing.Linear)},
            //    {0, 1, new Animation(f => _label.TranslationX = f, _label.TranslationX, translateX, Easing.Linear)},
            //    {0, 1, new Animation(f => _label.FontSize = f, _label.FontSize, EntryFontSize, Easing.Linear)}
            //};

            //if (EntryLayoutType == EntryLayoutType.Besieged)
            //    smoothAnimation.Add(0, 1,
            //        new Animation(f => Input.TranslationY = f, Input.TranslationY, translateY, Easing.Linear));

            //Device.BeginInvokeOnMainThread(() =>
            //    smoothAnimation.Commit(this, "EntryAnimation", 16, 200, Easing.Linear));
        }
    }
}