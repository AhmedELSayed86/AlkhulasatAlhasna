namespace Alkhulasat.App.Views;

public partial class AboutPage : ContentPage
{
    public AboutPage()
    {
        InitializeComponent();
        // جلب رقم الإصدار تلقائياً كما فعلنا في WPF سابقاً
        lblVersion.Text = $"إصدار التطبيق: {AppInfo.Current.VersionString}";
    }

    private async void OnOpenPdfClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var url = button?.CommandParameter as string;

        if(!string.IsNullOrEmpty(url))
        {
            await Launcher.Default.OpenAsync(new Uri(url));
        }
    }
}
