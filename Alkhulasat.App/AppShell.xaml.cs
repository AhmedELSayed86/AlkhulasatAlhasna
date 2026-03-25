using Alkhulasat.App.Views;

namespace Alkhulasat.App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // تسجيل المسارات الفرعية
            // nameof يضمن أنك لو غيرت اسم الكلاس مستقبلاً، الكود لن ينكسر
            Routing.RegisterRoute(nameof(AzkarPage), typeof(AzkarPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        }
    }
}
