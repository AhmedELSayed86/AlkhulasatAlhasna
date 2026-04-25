using Alkhulasat.BusinessLogic.ViewModels;
using Alkhulasat.Domain.Interfaces;

namespace Alkhulasat.App.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _viewModel;
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
        _viewModel = new SettingsViewModel(settingsService);
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadSettings();
    }
}
