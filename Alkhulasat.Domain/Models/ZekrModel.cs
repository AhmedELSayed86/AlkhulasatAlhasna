using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.Text.Json.Serialization;

namespace Alkhulasat.Domain.Models
{
    public partial class ZekrModel : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        [JsonIgnore] // أخبر الجيسون بتجاهل هذا الحقل عند القراءة
        public int ZekrID { get; set; }
        public string ZekrMale { get; set; } = string.Empty;
        public string ZekrFemale { get; set; } = string.Empty;
        public string ZekrDescription { get; set; } = string.Empty;
        public int ZekrTargetCount { get; set; }

        [Ignore]
        public string ZekrTargetText { get; set; } = string.Empty;

        // الخاصية التي سنعرضها في الـ XAML(تتغير بالتأنيث)
        //[property: Ignore]
        //[ObservableProperty]
        //private string displayContent = string.Empty;

        //[Ignore]
        public string DisplayContent { get; set; } = string.Empty;

        public string ZekrCategory { get; set; } = string.Empty;

        public int OrderIndex { get; set; } // رقم الترتيب (1, 2, 3...)


        // نجعل العداد "خاصية مراقبة"
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCompleted))]
        [NotifyPropertyChangedFor(nameof(Progress))]
        [NotifyPropertyChangedFor(nameof(ZekrCurrentText))] // <--- أضف هذا السطر المهم
        private int zekrCurrentCount;

        // قم بتغيير هذه الخاصية لتكون مقروءة فقط وتقوم بتوليد النص تلقائياً
        [Ignore]
        public string ZekrCurrentText => $"{ZekrCurrentCount} \\ {ZekrTargetCount}";

        [Ignore]
        public bool IsCompleted => ZekrCurrentCount >= ZekrTargetCount;

        [Ignore]
        public double Progress => ZekrTargetCount > 0 ? (double)ZekrCurrentCount / ZekrTargetCount : 0;
    }
}
