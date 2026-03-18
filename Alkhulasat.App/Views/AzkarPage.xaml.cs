using Alkhulasat.BusinessLogic.Messages;
using Alkhulasat.BusinessLogic.ViewModels;
using Alkhulasat.DataAccess.Repositories;
using Alkhulasat.Domain.Interfaces;
using CommunityToolkit.Mvvm.Messaging;

namespace Alkhulasat.App.Views;

public partial class AzkarPage : ContentPage
{
    string _Category;
    public AzkarPage(string category, string title)
    {
        InitializeComponent();

        // الحصول على مزود الخدمات من التطبيق الحالي
        var services = Application.Current.Handler.MauiContext.Services;

        // جلب الخدمات المسجلة
        var repository = services.GetService<IZekrRepository>();
        var settings = services.GetService<ISettingsService>();
        var haptic = services.GetService<IHapticService>(); // نحصل على الخدمة

        // التأكد من وجود الخدمات (اختياري)
        if(repository == null)
            throw new InvalidOperationException("ZekrRepository is لم يسجل in DI container.");
        if(settings == null)
            throw new InvalidOperationException("ISettingsService is لم يسجل in DI container.");

        // تمرير التبعيات كاملة
        BindingContext = new AzkarViewModel(repository, category, title, settings, haptic);
        _Category = category;
       
        var vm = new AzkarViewModel(repository, category, title, settings, haptic);
        BindingContext = vm;

        // استقبال رسالة التمرير
        WeakReferenceMessenger.Default.Register<ScrollToZekrMessage>(this, (r, m) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // التمرير للذكر التالي وجعله في أعلى الشاشة (Start)
                AzkarCollection.ScrollTo(m.Value, animate: true, position: ScrollToPosition.Start);
            });
        });
    }

    // الدالة الجديدة لفتح صفحة تعويذ الأبناء 
    private async void OnChildrenProtectionClicked(object sender, TappedEventArgs e)
    {
        if(_Category != "Children")
        {
            // نمرر النوع "Children" أو الاسم الذي تفضله
            await Navigation.PushAsync(new AzkarPage("Children", "ما يُعوذ به الأبناء"));
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if(BindingContext is AzkarViewModel vm && (vm.AzkarList == null || vm.AzkarList.Count == 0))
        {
            _ = Task.Run(async () => await vm.LoadAzkarCommand.ExecuteAsync(vm.CurrentCategory));
        }
    }
}
