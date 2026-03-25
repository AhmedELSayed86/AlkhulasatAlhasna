namespace Alkhulasat.App.Views;

public partial class SplashPage : ContentPage
{
    public SplashPage()
    {
        InitializeComponent();
        // Do not navigate from the constructor; wait for the page to appear
    }

    //private async void NavigateToMain()
    //{
    //    // انتظر ثانيتين ليتمكن المستخدم من رؤية الشعار والنص
    //    await Task.Delay(2500);

    //    var app = Application.Current;
    //    if(app == null)
    //        return;

    //    // For single-window apps, update the root page of the first window
    //    if(app.Windows?.Count > 0)
    //    {
    //        app.Windows[0].Page = new AppShell();
    //        return;
    //    }

    //    // Fallback: open a new window with the shell (avoids deprecated MainPage setter)
    //    app.OpenWindow(new Window(new AppShell()));
    //}
    //private void NavigateToMain()
    //{
    //    MainThread.BeginInvokeOnMainThread(() =>
    //    {
    //        // هذه الطريقة هي الأكثر أماناً في أندرويد لتغيير الصفحة الرئيسية
    //        Application.Current?.MainPage = new AppShell();
    //    });
    //}
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // تأخير بسيط لضمان استقرار الشاشة
        await Task.Delay(50);

        // تشغيل الحركة (تأكد من مطابقة الأسماء الجديدة)
        await Task.WhenAll(
            AppLogoContainer.TranslateToAsync(0, -150, 400, Easing.CubicInOut),
            AppTitleLabel.FadeToAsync(0, 400) // يختفي تدريجياً
        );

        // 2. انتظر ثانية واحدة إضافية ليقرأ المستخدم "الخلاصة الحسناء"
        await Task.Delay(200);
        // الانتقال للـ Shell
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if(Application.Current != null)
            {
                // استبدال صفحة السلاش بالـ Shell الخاص بك
                //Application.Current.MainPage = new Alkhulasat.App.AppShell();
                Application.Current.Windows[0].Page = new AppShell();
            }
        });
    }

}