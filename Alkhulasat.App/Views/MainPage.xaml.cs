using Alkhulasat.BusinessLogic.Services;
using Alkhulasat.BusinessLogic.ViewModels;

namespace Alkhulasat.App.Views
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();

            lblVersion.Text = $"إصدار التطبيق: {AppInfo.Current.VersionString}";
        }

        private async void OnMorningAzkarClicked(object sender, EventArgs e)
        {
            // نفتح صفحة الأذكار ونمرر لها النوع "Morning"
            await Navigation.PushAsync(new AzkarPage("Morning", "أذكار الصباح"));
        }

        private async void OnEveningAzkarClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AzkarPage("Evening", "أذكار المساء"));
        }

        private async void OnPrayerAzkarClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AzkarPage("Prayer", "أذكار الصلاة"));
        }

        private async void OnTasbihClicked(object sender, EventArgs e)
        {
            // صفحة التسبيح (العداد الحر)
            await Navigation.PushAsync(new TasbihPage());
        }

        private async void OnAboutClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AboutPage());
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }

        private async void OnDarkModeInfoClicked(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new DarkModeInfoPage());
        }

        // الدالة الجديدة لفتح صفحة تعويذ الأبناء
        private async void OnChildrenProtectionClicked(object sender, EventArgs e)
        {
            // نمرر النوع "Children" أو الاسم الذي تفضله
            await Navigation.PushAsync(new AzkarPage("Children", "ما يُعوذ به الأبناء"));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // تأخير بسيط لضمان ظهور الصفحة أولاً
            await Task.Delay(500);

            // بدء المزامنة في الخلفية (لا ننتظر)
            _ = Task.Run(async () =>
            {
                try
                {
                    var updateService = Handler.MauiContext.Services.GetService<AzkarUpdateService>();
                    if(updateService != null)
                        await updateService.SyncAzkarAsync();
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Sync error: {ex.Message}");
                }
            });
        }
    }
}
