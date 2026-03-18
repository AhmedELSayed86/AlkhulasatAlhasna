using Alkhulasat.Domain.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Alkhulasat.BusinessLogic.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace Alkhulasat.BusinessLogic.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly ISettingsService _settingsService;

        [ObservableProperty]
        private bool isHapticEnabled;

        [ObservableProperty]
        private bool isKeepScreenOn;

        [ObservableProperty]
        private bool isFemaleVersion;

        [ObservableProperty]
        private double fontSize;

        public SettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;

            // تحميل القيم المحفوظة عند فتح الصفحة
            IsHapticEnabled = _settingsService.IsHapticEnabled;
            IsKeepScreenOn = _settingsService.IsKeepScreenOn;
            IsFemaleVersion = _settingsService.IsFemaleVersion;
            FontSize = _settingsService.FontSize;
        }

        // هذه الدوال تُستدعى تلقائياً عند تغيير أي قيمة في الواجهة
        partial void OnIsHapticEnabledChanged(bool value) => _settingsService.IsHapticEnabled = value;
        //partial void OnIsFemaleVersionChanged(bool value) => _settingsService.IsFemaleVersion = value;
        //partial void OnFontSizeChanged(double value) => _settingsService.FontSize = value;

        partial void OnIsFemaleVersionChanged(bool value)
        {
            _settingsService.IsFemaleVersion = value;
            // نرسل نصاً لتمييز نوع التغيير
            WeakReferenceMessenger.Default.Send(new SettingsChangedMessage("Gender"));
            System.Diagnostics.Debug.WriteLine($"OnIsFemaleVersionChanged set to: {_settingsService.IsFemaleVersion}");
        }

        partial void OnFontSizeChanged(double value)
        {
            _settingsService.FontSize = value;
            WeakReferenceMessenger.Default.Send(new SettingsChangedMessage("FontSize"));
        }

        partial void OnIsKeepScreenOnChanged(bool value)
        {
            _settingsService.IsKeepScreenOn = value;
            WeakReferenceMessenger.Default.Send(new SettingsChangedMessage("IsKeepScreenOn"));
        }

    }
}
