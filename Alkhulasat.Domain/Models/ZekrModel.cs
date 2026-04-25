using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;

namespace Alkhulasat.Domain.Models
{ 
    public partial class ZekrModel : ObservableObject
    { 
        // --- 1. خصائص البيانات الثابتة (Data Properties) ---
        // هذه الخصائص لا تتغير أثناء بقاء المستخدم في نفس الشاشة، لذا لا تحتاج [ObservableProperty]

        [JsonIgnore] // أمان: منع تصدير الـ ID إذا تم تحويل الكائن لـ JSON
        public int ZekrID { get; set; }

        public string ZekrMale { get; set; } = string.Empty;
        public string ZekrFemale { get; set; } = string.Empty;
        public string ZekrDescription { get; set; } = string.Empty;
        public string ZekrCategory { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
         
        // الخصائص التي لا ترتبط بقاعدة البيانات (محسوبة أو للعرض فقط)
        public string DisplayContent { get; set; } = string.Empty;
        public string ZekrTargetText { get; set; } = string.Empty;


        // --- 2. الخصائص التفاعلية (Reactive Properties) ---
        // هنا نترك الكومبايلر يقوم بعمله السحري. بمجرد تغيير _zekrCurrentCount،
        // سيقوم بتحديث نفسه، ثم يرسل إشعاراً للشاشة لتحديث الـ 3 خصائص المكتوبة أسفله.

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ZekrCurrentText))]
        [NotifyPropertyChangedFor(nameof(IsCompleted))]
        [NotifyPropertyChangedFor(nameof(Progress))]
        private int _zekrCurrentCount;

        // [تحسين]: جعلت الهدف التفاعلي أيضاً، لأنه لو تغير الهدف لأي سبب، يجب أن تتحدث النسب!
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ZekrCurrentText))]
        [NotifyPropertyChangedFor(nameof(IsCompleted))]
        [NotifyPropertyChangedFor(nameof(Progress))]
        private int _zekrTargetCount;

        public string FullTargetText => string.IsNullOrWhiteSpace(ZekrTargetText)
    ? $"({ZekrTargetCount})"
    : $"{ZekrTargetText} ({ZekrTargetCount})";

        // --- 3. الخصائص المحسوبة (Computed Properties) ---
        // هذه الخصائص لا تحتاج إلى (set) لأنها تُحسب تلقائياً بناءً على الخصائص التفاعلية.

        public string ZekrCurrentText => $"{ZekrCurrentCount} / {ZekrTargetCount}";

        public bool IsCompleted => ZekrCurrentCount >= ZekrTargetCount;

        // حماية من القسمة على صفر (DivideByZeroException)
        public double Progress => ZekrTargetCount > 0 ? (double)ZekrCurrentCount / ZekrTargetCount : 0;
    }
}