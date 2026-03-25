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
        private string pageTitle;

        [ObservableProperty]
        string currentCategory; // لتخزين الفئة الحالية (Morning, Evening, etc.)

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
                // جلب الإعدادات مرة واحدة خارج الحلقة التكرارية للسرعة
                bool isFemale = _settings.IsFemaleVersion;
                string today = DateTime.Now.ToString("yyyy-MM-dd");
                string lastOpen = _settings.GetLastOpenDate(category);

                // تصفير العدادات إذا تغير اليوم
                if(today != lastOpen)
                {
                    await _repository.ResetDailyCountersAsync();
                    _settings.SetLastOpenDate(category, today);
                }

                var data = await _repository.GetAzkarByCategory(category, isFemale);

                if(data != null && data.Any())
                {
                    AzkarList = new ObservableCollection<ZekrModel>(data);
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
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
                // 1. زيادة العداد
                zekr.ZekrCurrentCount++;

                // 3. حفظ التغيير في قاعدة البيانات SQLite
                await _repository.UpdateZekr(zekr);

                // --- الجزء المضاف للتحريك ---
                if(zekr.ZekrCurrentCount >= zekr.ZekrTargetCount)
                {
                    var index = AzkarList.IndexOf(zekr);
                    if(index < AzkarList.Count - 1)
                    {
                        var nextZekr = AzkarList[index + 1];
                        // إرسال رسالة بالذكر التالي ليتم التمرير إليه
                        WeakReferenceMessenger.Default.Send(new ScrollToZekrMessage(nextZekr));
                    }
                }
                // -------------------------

                // فقط إذا كان الاهتزاز مفعلاً والخدمة موجودة
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
            await _repository.ResetAzkarCountByCategoryAsync(CurrentCategory);

            // إعادة تحميل القائمة لتحديث الشاشة فوراً
            await LoadAzkarAsync(CurrentCategory);
        }
    }
}
