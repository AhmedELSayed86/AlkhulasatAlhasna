using Alkhulasat.BusinessLogic.Services;
using Alkhulasat.BusinessLogic.ViewModels;

namespace Alkhulasat.App.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly AzkarUpdateService _updateService;
        public MainPage(AzkarUpdateService updateService)
        {
            InitializeComponent();
            _updateService = updateService;
            lblVersion.Text = $"إصدار التطبيق: {AppInfo.Current.VersionString}";
        }

        private async void OnMorningAzkarClicked(object? sender, EventArgs e)
        {
            // التنقل المباشر هو الأصح مع المسارات المسجلة برمجياً
            await Shell.Current.GoToAsync($"{nameof(AzkarPage)}?category=Morning&title={Uri.EscapeDataString("أذكار الصباح")}");
        }

        private async void OnEveningAzkarClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(AzkarPage)}?category=Evening&title={Uri.EscapeDataString("أذكار المساء")}");
        }

        private async void OnPrayerAzkarClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(AzkarPage)}?category=Prayer&title={Uri.EscapeDataString("أذكار الصلاة")}");
        }

        private async void OnChildrenProtectionClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(AzkarPage)}?category=Children&title={Uri.EscapeDataString("ما يُعوذ به الأبناء")}");
        }

        private async void OnTasbihClicked(object? sender, EventArgs e)
        {
            // صفحة التسبيح (العداد الحر)
            await Navigation.PushAsync(new TasbihPage());
        }

        private async void OnAboutClicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new AboutPage());
        }

        private async void OnSettingsClicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }

        private async void OnDarkModeInfoClicked(object? sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new DarkModeInfoPage());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // تشغيل المزامنة بأمان دون الحاجة للبحث عن الخدمات يدوياً
            Task.Run(async () =>
            {
                try
                {
                    await _updateService.SyncAzkarAsync();
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Sync error: {ex.Message}");
                }
            });
        }
    }
}
