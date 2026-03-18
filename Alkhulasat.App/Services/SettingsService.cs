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
        private const string AzkarVersionKey = "AzkarVersion";

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
        public string GetAzkarVersion()
        {
            // القيمة الافتراضية "0.0" تعني أن التطبيق يفتح لأول مرة
            return Preferences.Default.Get(AzkarVersionKey, "0.0");
        }

        // تنفيذ دالة حفظ الإصدار
        public void SetAzkarVersion(string version)
        {
            Preferences.Default.Set(AzkarVersionKey, version);
        } 
    }
}
