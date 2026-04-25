using Alkhulasat.BusinessLogic.Messages;
using Alkhulasat.Domain.Interfaces;
using Alkhulasat.Domain.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;

namespace Alkhulasat.BusinessLogic.ViewModels
{
    // أضف partial ضروري جداً لتوليد الكود تلقائياً
    public partial class AzkarViewModel : ObservableObject
    {
        private readonly IZekrRepository _repository;
        private readonly ISettingsService _settings;
        private readonly IHapticService? _hapticService;

        [ObservableProperty]
        private ObservableCollection<ZekrModel> azkarList = [];

        [ObservableProperty]
        private string? pageTitle;

        [ObservableProperty]
        private string? currentCategory; // لتخزين الفئة الحالية (Morning, Evening, etc.)

        [ObservableProperty]
        bool isBusy; // تولد تلقائياً IsBusy

        //public AzkarViewModel(IZekrRepository repository, string category, string title, ISettingsService settings, IHapticService? hapticService = null)
        //{
        //    _repository = repository;
        //    _settings = settings;
        //    _hapticService = hapticService;
        //    PageTitle = title;
        //    CurrentCategory = category;

        //    // الاستماع لتغيير الجنس وإعادة تحميل البيانات من قاعدة البيانات
        //    WeakReferenceMessenger.Default.Register<SettingsChangedMessage>(this, async (r, m) =>
        //    {
        //        if(m.Value == "Gender")
        //        {
        //            await LoadAzkarAsync(CurrentCategory); // إعادة التحميل
        //        }
        //    });
        //}

        // المنشئ الآن نظيف تماماً ويقبل الخدمات فقط
        public AzkarViewModel(IZekrRepository repository, ISettingsService settings, IHapticService? hapticService = null)
        {
            _repository = repository;
            _settings = settings;
            _hapticService = hapticService;

            // الاشتراك في الرسائل كما هو
            WeakReferenceMessenger.Default.Register<SettingsChangedMessage>(this, async (r, m) =>
            {
                if(m.Value == "Gender" && !string.IsNullOrEmpty(CurrentCategory))
                {
                    await LoadAzkarAsync(CurrentCategory);
                }
            });
        }

        [RelayCommand]
        private async Task LoadAzkarAsync(string category)
        {
            if(IsBusy) return;
            IsBusy = true;

            try
            {
                bool isFemale = _settings.IsFemaleVersion;
                string today = DateTime.Now.ToString("yyyy-MM-dd");
                string lastOpen = _settings.GetLastOpenDate(category);

                if(today != lastOpen)
                {
                    await _repository.ResetDailyCountersAsync();
                    _settings.SetLastOpenDate(category, today);
                }

                var data = await _repository.GetAzkarByCategory(category, isFemale);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    AzkarList.Clear();
                    if(data != null && data.Count > 0)
                    {
                        foreach(var item in data)
                        {
                            AzkarList.Add(item);
                        }
                    }
                });
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadAzkarAsyncError: {ex.ToString()}");

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    // [Clean Code / .NET 10]: استخدام معمارية النوافذ المتعددة بدلاً من MainPage
                    if(Application.Current?.Windows.Count > 0)
                    {
                        var currentWindow = Application.Current.Windows[0];
                        if(currentWindow.Page != null)
                        {
                            // استخدام الميثود الجديدة DisplayAlertAsync المتوافقة مع .NET 10
                            await currentWindow.Page.DisplayAlertAsync(
                                "خطأ برمجي",
                                $"لم نتمكن من جلب الأذكار. التفاصيل: {ex.Message}",
                                "حسناً");
                        }
                    }
                });
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task IncrementCount(ZekrModel zekr)
        {
            if(zekr != null && zekr.ZekrCurrentCount < zekr.ZekrTargetCount)
            {
                zekr.ZekrCurrentCount++;
                await _repository.UpdateZekr(zekr);

                if(zekr.ZekrCurrentCount >= zekr.ZekrTargetCount)
                {
                    // 1. تفعيل الحالة في الموديل (ليعمل الـ DataTrigger الخاص بك فوراً)
                    //zekr.IsCompleted = true;

                    var index = AzkarList.IndexOf(zekr);
                    if(index < AzkarList.Count - 1)
                    {
                        // 2. انتظر قليلاً ليعالج المستخدم بصرياً أن علامة "✓" ظهرت 
                        // وأن الشفافية تغيرت (بفضل كود الـ XAML الخاص بك)
                        await Task.Delay(300);

                        var nextZekr = AzkarList[index + 1];

                        // 3. إرسال رسالة التمرير
                        WeakReferenceMessenger.Default.Send(new ScrollToZekrMessage(nextZekr));
                    }
                }

                // تشغيل الاهتزاز (تأكد من استخدام Click)
                if(_settings.IsHapticEnabled && _hapticService != null)
                {
                    _hapticService.PerformClick();
                }
            }
        }

        // أمر تصفير عداد الفئة الحالية
        [RelayCommand]
        public async Task ResetCurrentCategoryAsync()
        {
            // CurrentCategory هو المتغير الذي يحمل (Morning, Evening, etc.)
            if(CurrentCategory != null)
            {
                await _repository.ResetAzkarCountByCategoryAsync(CurrentCategory);

                // إعادة تحميل القائمة لتحديث الشاشة فوراً
                await LoadAzkarAsync(CurrentCategory);
            }
        }
    }
}
