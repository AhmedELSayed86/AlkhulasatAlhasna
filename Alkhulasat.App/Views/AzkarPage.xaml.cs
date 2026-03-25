using Alkhulasat.BusinessLogic.Messages;
using Alkhulasat.BusinessLogic.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace Alkhulasat.App.Views;

[QueryProperty(nameof(Category), "category")]
[QueryProperty(nameof(Title), "title")]
public partial class AzkarPage : ContentPage
{
    private readonly AzkarViewModel _viewModel;

    // عندما تصل القيمة من الرابط، نمررها فوراً للـ ViewModel
    public string Category
    {
        set => _viewModel.CurrentCategory = value;
    }

    public string Title
    {
        set => _viewModel.PageTitle = value;
    }

    // .NET 10 سيقوم بحقن الـ ViewModel تلقائياً هنا
    public AzkarPage(AzkarViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    //public AzkarPage(string category, string title)
    //{
    //    InitializeComponent();

    //    // الحصول على مزود الخدمات من التطبيق الحالي
    //    Application? current = Application.Current;
    //    var services = current?.Handler.MauiContext?.Services;

    //    // جلب الخدمات المسجلة
    //    var repository = services?.GetService<IZekrRepository>();
    //    var settings = services?.GetService<ISettingsService>();
    //    var haptic = services?.GetService<IHapticService>(); // نحصل على الخدمة

    //    // التأكد من وجود الخدمات (اختياري)
    //    if(repository == null)
    //        throw new InvalidOperationException("ZekrRepository is لم يسجل in DI container.");
    //    if(settings == null)
    //        throw new InvalidOperationException("ISettingsService is لم يسجل in DI container.");

    //    // تمرير التبعيات كاملة
    //    var vm = new AzkarViewModel(repository, category, title, settings, haptic);
    //    BindingContext = vm;
    //    Category = category;
    //}

    private async void OnChildrenProtectionClicked(object? sender, TappedEventArgs e)
    {
        // استخدام ../ يغلق صفحة الأذكار الحالية ويفتح الجديدة بدلاً منها، مما يحافظ على نظافة مكدس التنقل
        await Shell.Current.GoToAsync($"/{nameof(AzkarPage)}?category=Children&title={Uri.EscapeDataString("ما يُعوذ به الأبناء")}");
        //await Shell.Current.GoToAsync($"{nameof(AzkarPage)}?category=Children&title={Uri.EscapeDataString("ما يُعوذ به الأبناء")}");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // الآن البيانات جاهزة داخل الـ VM، نبدأ التحميل
        if(!string.IsNullOrEmpty(_viewModel.CurrentCategory))
        {
            if(_viewModel.AzkarList == null || _viewModel.AzkarList.Count == 0)
            {
                await _viewModel.LoadAzkarCommand.ExecuteAsync(_viewModel.CurrentCategory);
            }
        }

        // تسجيل رسالة التمرير (Scroll)
        WeakReferenceMessenger.Default.Register<ScrollToZekrMessage>(this, (r, m) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if(_viewModel.AzkarList!.Contains(m.Value))
                {
                    AzkarCollection.ScrollTo(m.Value, position: ScrollToPosition.Start);
                }
            });
        });
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        WeakReferenceMessenger.Default.Unregister<ScrollToZekrMessage>(this);
    }
}
