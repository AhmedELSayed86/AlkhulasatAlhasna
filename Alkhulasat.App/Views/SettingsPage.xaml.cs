using Alkhulasat.App.Services;
using Alkhulasat.BusinessLogic.Messages;
using Alkhulasat.BusinessLogic.ViewModels;
using Alkhulasat.DataAccess.Repositories;
using Alkhulasat.Domain.Interfaces;
using CommunityToolkit.Mvvm.Messaging;

namespace Alkhulasat.App.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();

        // الحصول على مزود الخدمات من التطبيق الحالي
        var services = Application.Current?.Handler?.MauiContext?.Services;
        if(services == null)
            throw new InvalidOperationException("MauiContext not available");

        // جلب ISettingsService
        var settingsService = services.GetService<ISettingsService>();
        if(settingsService == null)
            throw new InvalidOperationException("ISettingsService is not registered in DI container.");

        // إنشاء ViewModel مع حقن الخدمة
        BindingContext = new SettingsViewModel(settingsService);

        // عرض إصدار التطبيق
        lblVersion.Text = $"إصدار التطبيق: {AppInfo.Current.VersionString}";
    } 
}
