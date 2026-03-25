using Alkhulasat.App.Services;
using Alkhulasat.App.Views;
using Alkhulasat.BusinessLogic.Services;
using Alkhulasat.BusinessLogic.ViewModels;
using Alkhulasat.DataAccess.Context;
using Alkhulasat.DataAccess.Repositories;
using Alkhulasat.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Alkhulasat.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

                    // تسجيل خطوطك المخصصة لتطابق الستايلات
                    fonts.AddFont("Your-Quran-Font.ttf", "QuranFont");
                    fonts.AddFont("Your-Azkar-Font.ttf", "AzkarFont");
                    fonts.AddFont("Your-Header-Font.ttf", "HeaderFont");
                    fonts.AddFont("Your-UI-Font.ttf", "UIFont");
                    fonts.AddFont("Your-Title-Font.ttf", "TitelFont");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            // أضف هذه الأسطر لتسجيل الخدمات 
            // --- تسجيل الخدمات (Dependency Injection) ---

            // 1. تسجيل الخدمات (Services)
            builder.Services.AddSingleton<IZekrRepository, ZekrRepository>();
            builder.Services.AddSingleton<ISettingsService, SettingsService>();
            builder.Services.AddSingleton<IHapticService, HapticService>();
            builder.Services.AddSingleton<AzkarUpdateService>();

            // 2. تسجيل الـ ViewModels
            builder.Services.AddTransient<AzkarViewModel>();

            // 3. تسجيل الصفحات (Pages) - ضروري جداً لـ .NET 10 DI
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<AzkarPage>();
            builder.Services.AddTransient<TasbihPage>(); // وأي صفحة أخرى تستخدم Injection
              
            // 1. تسجيل HttpClient كخدمة مفردة (Singleton)
            builder.Services.AddSingleton<HttpClient>();
             
            builder.Services.AddSingleton<IAssetService, MauiAssetService>();
             
            builder.Services.AddSingleton<AppDbContext>();
        
            builder.Services.AddTransient<SettingsPage>();
             
            SQLitePCL.Batteries_V2.Init();
            return builder.Build();
        }
    }
}
