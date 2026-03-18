using Alkhulasat.BusinessLogic.Messages;
using Alkhulasat.BusinessLogic.Services;
using Alkhulasat.Domain.Interfaces;
using CommunityToolkit.Mvvm.Messaging;

namespace Alkhulasat.App
{
    public partial class App : Application
    {
        private readonly ISettingsService _settingsService;
        private readonly AzkarUpdateService _updateService; // 1. أضف هذا الحقل

        public App(ISettingsService settingsService, AzkarUpdateService updateService)
        {
            InitializeComponent();

            _settingsService = settingsService;
            _updateService = updateService; // 3. تعيين الخدمة
                              
            // 1. تطبيق الأحجام المحفوظة عند التشغيل الأول
            UpdateAllFontSizes(_settingsService.FontSize);

            //// استدعاء تحديث الأذكار (غير متزامن، لا ننتظر)
            //_ = Task.Run(async () => await _updateService.SyncAzkarAsync()).ContinueWith(t =>
            //{
            //    if(t.IsFaulted)
            //        Debug.WriteLine($"Sync failed: {t.Exception?.Message}");
            //});

            // 2. الاشتراك في الرسائل لتحديث الأحجام والتأنيث فوراً
            WeakReferenceMessenger.Default.Register<SettingsChangedMessage>(this, (r, m) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if(m.Value == "FontSize")
                    {
                        UpdateAllFontSizes(_settingsService.FontSize);
                    }
                    else if(m.Value == "Gender")
                    {
                        // يمكن هنا إرسال رسالة أخرى للـ ViewModels لإعادة تحميل النصوص
                        // الـ ViewModels المشتركة ستستجيب تلقائياً
                    }
                });
            }); 
        }

        public void UpdateAllFontSizes(double baseSize)
        {
            // الوصول لموارد التطبيق وتحديث المفاتيح المختلفة
            Resources["UserFontSize"] = baseSize;         // الخط الأساسي
            Resources["QuranFontSize"] = baseSize + 6;    // خط القرآن
            Resources["NoteFontSize"] = baseSize - 4;     // خط الحواشي/التفسير
        }
         
        protected override Window CreateWindow(IActivationState? activationState)
        {
            // تنفيذ الإعدادات هنا يضمن أن الـ Window متاح ولا يحدث خطأ JavaProxy
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Microsoft.Maui.Devices.DeviceDisplay.Current.KeepScreenOn = _settingsService.IsKeepScreenOn;
                UpdateAllFontSizes(_settingsService.FontSize);
            });

            return new Window(new Alkhulasat.App.Views.SplashPage());
        }
    }
}