using Alkhulasat.App.Services;

namespace Alkhulasat.App.Views;

public partial class AboutPage : ContentPage
{
    public AboutPage()
    {
        InitializeComponent();
        // جلب رقم الإصدار تلقائياً كما فعلنا في WPF سابقاً
        lblVersionApp.Text = $"إصدار التطبيق: {AppInfo.Current.VersionString}";
        lblVersionDb.Text = $"إصدار قاعدة البيانات: {Preferences.Default.Get("AzkarVersionDb", "0.0")}";
    }

    private async void OnOpenPdfClicked(object? sender, EventArgs e)
    {
        var button = sender as Button;
        var url = button?.CommandParameter as string;

        if(!string.IsNullOrEmpty(url))
        {
            await Launcher.Default.OpenAsync(new Uri(url));
        }
    }
}
