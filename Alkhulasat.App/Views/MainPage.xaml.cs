using Alkhulasat.App.Services;
using Alkhulasat.BusinessLogic.Services;

namespace Alkhulasat.App.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly AzkarUpdateService _updateService;
        private string _azkarVersionApp = "0.0";

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

        private async void OnPropheticIstikharaClicked(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(AzkarPage)}?category=Istikhara&title={Uri.EscapeDataString("دعاء الاستخارة")}");
        }

        private async void OnTravelClicked(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(AzkarPage)}?category=Travel&title={Uri.EscapeDataString("دعاء السفر")}");
        }

        private async void OnWakingClicked(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(AzkarPage)}?category=Waking&title={Uri.EscapeDataString("دعاء الاستيقاظ")}");
        }

        private async void OnSleepClicked(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(AzkarPage)}?category=Sleep&title={Uri.EscapeDataString("دعاء النوم")}");
        }

        private async void OnSufficiencyPart1Clicked(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(AzkarPage)}?category=SufficiencyPart1&title={Uri.EscapeDataString("أذكار الكفاية 1 الأذكار المطلقَة")}");
        }

        private async void OnSufficiencyPart2Clicked(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(AzkarPage)}?category=SufficiencyPart2&title={Uri.EscapeDataString("أذكار الكفاية 2 الأذكار المقيَّدة")}");
        }

        private async void OnLeavingHouseClicked(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(AzkarPage)}?category=LeavingHouse&title={Uri.EscapeDataString("أذكار الخروج من المنزل")}");
        }

        private async void OnEnteringHouseClicked(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(AzkarPage)}?category=EnteringHouse&title={Uri.EscapeDataString("أذكار دخول المنزل")}");
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
                SettingsService settingsService = new SettingsService();
                try
                {
                    await _updateService.SyncAzkarAsync();

                    // 1. التحقق أولاً: هل اختار المستخدم "ليس الآن" سابقاً لهذا الإصدار؟
                    // سنستخدم Preferences لتخزين قرار المستخدم
                    bool skipUpdate = Preferences.Default.Get("SkipUpdateCheck", false);
                    if(skipUpdate) return;


                    string version = settingsService.GetAzkarVersionApp();
                    if(version != null)
                    {
                        await _updateService.SyncVersionAppAsync();

                        if(version != settingsService.GetAzkarVersionApp())
                        {
                            await MainThread.InvokeOnMainThreadAsync(async () =>
                            {
                                bool update = await DisplayAlertAsync("تحديث جديد",
                                   "يوجد إصدار جديد للأذكار، هل تريد الانتقال للمتجر للتحديث الآن؟",
                                   "تحديث الآن",
                                   "ليس الآن");

                                if(update)
                                {
                                    // استبدل YOUR_PACKAGE_NAME بمعرف تطبيقك الفعلي
                                    //string url = $"https://play.google.com/store/apps/details?id=com.Code_Development.mauiazkar&hl=ar{AppInfo.Current.PackageName}";
                                    string url = "https://play.google.com/store/apps/details?id=com.Code_Development.mauiazkar&hl=ar";
                                    await Launcher.Default.OpenAsync(url);
                                }
                                else
                                {
                                    // 2. إذا ضغط "ليس الآن"، نحفظ قراره لكي لا تظهر الرسالة مرة أخرى
                                    Preferences.Default.Set("SkipUpdateCheck", true);
                                    settingsService.SetAzkarVersionApp(version);
                                }
                            });
                        }
                    }
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Sync error: {ex.Message}");
                }
                finally
                {
                    // تحديث النسخة المعروضة في صفحة "حول"
                    _azkarVersionApp = settingsService.GetAzkarVersionApp();
                }
            });
        }
    }
}
