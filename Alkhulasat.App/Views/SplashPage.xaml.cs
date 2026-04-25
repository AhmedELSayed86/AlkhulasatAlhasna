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
    //protected override async void OnAppearing()
    //{
    //    base.OnAppearing();

    //    // تأخير بسيط لضمان استقرار الشاشة
    //    await Task.Delay(50);

    //    // تشغيل الحركة (تأكد من مطابقة الأسماء الجديدة)
    //    await Task.WhenAll(
    //        AppLogoContainer.TranslateToAsync(0, -150, 400, Easing.CubicInOut),
    //        AppTitleLabel.FadeToAsync(0, 400) // يختفي تدريجياً
    //    );

    //    // 2. انتظر ثانية واحدة إضافية ليقرأ المستخدم "الخلاصة الحسناء"
    //    await Task.Delay(200);
    //    // الانتقال للـ Shell
    //    MainThread.BeginInvokeOnMainThread(() =>
    //    {
    //        if(Application.Current != null)
    //        {
    //            // استبدال صفحة السلاش بالـ Shell الخاص بك
    //            //Application.Current.MainPage = new Alkhulasat.App.AppShell();
    //            //Application.Current.Windows[0].Page = new AppShell();
    //        }
    //    });
    //}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            // 1. تنفيذ الأنميشن (تأكد من مطابقة هذه الأسماء في ملف XAML)
            await Task.WhenAll(
                AppLogoContainer.TranslateToAsync(0, -150, 600, Easing.CubicInOut),
                AppTitleLabel.FadeToAsync(1, 600)
            );

            // 2. وقت انتظار إضافي لراحة العين
            await Task.Delay(1500);

            // 3. الانتقال الآمن للشاشة الرئيسية
            MainThread.BeginInvokeOnMainThread(() =>
            {// الطريقة الحديثة والآمنة 100% في .NET MAUI
                var app = Application.Current;
                if(app?.Windows.Count > 0)
                {
                    // الطريقة الوحيدة المدعومة الآن لتحديث الواجهة في النوافذ
                    app.Windows[0].Page = new AppShell();
                }
                //else
                //{
                //    // Fallback: إذا لم نتمكن من الوصول إلى النافذة، افتح نافذة جديدة
                //    Application.Current?.OpenWindow(new Window(new AppShell()));
                //}
            });
        }
        catch(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Splash Error: {ex.Message}");
            // في حال فشل الأنميشن، انتقل فوراً حتى لا يعلق المستخدم
            //Application.Current.MainPage = new AppShell(); 
            Application.Current?.OpenWindow(new Window(new AppShell()));
        }
    }
}