using Alkhulasat.Domain.Interfaces;

namespace Alkhulasat.App.Services
{
    public class SettingsService : ISettingsService
    {
        // مفاتيح الحفظ
        private const string FontSizeKey = "UserFontSize";
        private const string HapticKey = "haptic_enabled";
        private const string ScreenOnKey = "screenon_enabled";
        private const string FemaleKey = "is_female";
        private const string AzkarVersionAppKey = "AzkarVersionApp";
        private const string AzkarVersionDbKey = "AzkarVersionDb";
        private const string SkipUpdateCheckKey = "SkipUpdateCheck";

        public double FontSize
        {
            get => Preferences.Default.Get(FontSizeKey, 22.0);
            set
            {
                Preferences.Default.Set(FontSizeKey, value);
                if(Application.Current != null)
                    Application.Current.Resources[FontSizeKey] = value;
            }
        }

        public bool IsHapticEnabled
        {
            get => Preferences.Default.Get(HapticKey, true);
            set => Preferences.Default.Set(HapticKey, value);
        }

        public bool IsKeepScreenOn
        {
            get => Preferences.Default.Get(ScreenOnKey, true);
            set
            {
                // 1. الحفظ في الذاكرة (للمرات القادمة)
                Preferences.Default.Set(ScreenOnKey, value);

                // 2. التطبيق الفوري على النظام (للحظة الحالية)
                MainThread.BeginInvokeOnMainThread(() => {
                    Microsoft.Maui.Devices.DeviceDisplay.Current.KeepScreenOn = value;
                });
            }
        }

        public bool IsFemaleVersion
        {
            get => Preferences.Default.Get(FemaleKey, false);
            set => Preferences.Default.Set(FemaleKey, value);
        }

        public void SetLastOpenDate(string category, string date) =>
            Preferences.Default.Set($"LastOpen_{category}", date);

        public string GetLastOpenDate(string category) =>
            Preferences.Default.Get($"LastOpen_{category}", string.Empty);

        // تنفيذ دالة جلب الإصدار
        public string GetAzkarVersionApp()
        {
            // القيمة الافتراضية "0.0" تعني أن التطبيق يفتح لأول مرة
            return Preferences.Default.Get(AzkarVersionAppKey, "0.0");
        }

        // تنفيذ دالة حفظ الإصدار
        public void SetAzkarVersionApp(string version)
        {
            Preferences.Default.Set(AzkarVersionAppKey, version);
        } 
        public string GetAzkarVersionDb()
        {
            // القيمة الافتراضية "0.0" تعني أن التطبيق يفتح لأول مرة
            return Preferences.Default.Get(AzkarVersionDbKey, "0.0");
        }

        public void SetAzkarVersionDb(string version)
        {
            Preferences.Default.Set(AzkarVersionDbKey, version);
        } 
        
        public string GetSkipUpdateCheck()
        { 
            return Preferences.Default.Get(SkipUpdateCheckKey, "false");
        }
        
        public void SetSkipUpdateCheck(string Check)
        {
            Preferences.Default.Set(SkipUpdateCheckKey, Check);
        } 
    }
}
