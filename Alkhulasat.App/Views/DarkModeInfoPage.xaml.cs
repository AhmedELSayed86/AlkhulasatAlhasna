namespace Alkhulasat.App.Views;

public partial class DarkModeInfoPage : ContentPage
{
	public DarkModeInfoPage()
	{
		InitializeComponent();
	}

    private async void OpenHarvardLink(object sender, EventArgs e)
    {
        await Launcher.Default.OpenAsync("https://www.health.harvard.edu/staying-healthy/blue-light-has-a-dark-side");
    }

    private async void OpenEyeAcademyLink(object sender, EventArgs e)
    {
        await Launcher.Default.OpenAsync("https://www.aao.org");
    }

    private async void OpenGoogleLink(object sender, EventArgs e)
    {
        // الرابط الرسمي لأبحاث جوجل حول التصميم الداكن
        await Launcher.Default.OpenAsync("https://developer.android.com");
    }

    private async void OpenMicrosoftLink(object sender, EventArgs e)
    {
        // رابط بحث مايكروسوفت حول فوائد الوضع الداكن والتركيز
        await Launcher.Default.OpenAsync("https://news.microsoft.com");
    }

    private async void OpenWebECALink(object sender, EventArgs e)
    {
        // الرابط المباشر لمقال فوائد الوضع الداكن من WebECA
        await Launcher.Default.OpenAsync("https://www.webeca.com/eye-care-resources/is-dark-mode-better-for-your-eyes");
    }
}