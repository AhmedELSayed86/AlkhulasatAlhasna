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
                    // إضافة خطك العربي هنا (اسم الملف، والاسم المستعار الذي ستستخدمه)
                    // خط للعناوين والواجهة (خط ثقيل وجذاب)
                    fonts.AddFont("Cairo-Bold.ttf", "TitelFont");
                    // خط الازرار والقوائم (خط متوسط الوزن وسهل القراءة)
                    fonts.AddFont("Cairo-SemiBold.ttf", "UIFont");
                    // خط للآيات القرآنية (عثمان طه)
                    fonts.AddFont("UthmanTN-Regular.ttf", "QuranFont");
                    // خط للأذكار والأدعية (أميري)
                    fonts.AddFont("Amiri-Regular.ttf", "AzkarFont");
                    // خط التفسير والشرح (شهرزاد الجديدة)
                    fonts.AddFont("ScheherazadeNew-Medium.ttf", "HeaderFont");
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
