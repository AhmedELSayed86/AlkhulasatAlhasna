using Alkhulasat.App.Services;
using Alkhulasat.Domain.Interfaces;

namespace Alkhulasat.App.Controls;

public partial class ZekrView : ContentView
{
    public static readonly BindableProperty ZekrClickCommandProperty =
        BindableProperty.Create(nameof(ZekrClickCommand), typeof(System.Windows.Input.ICommand), typeof(ZekrView));

    public System.Windows.Input.ICommand ZekrClickCommand
    {
        get => (System.Windows.Input.ICommand)GetValue(ZekrClickCommandProperty);
        set => SetValue(ZekrClickCommandProperty, value);
    }

    public static readonly BindableProperty ZekrDescriptionProperty =
        BindableProperty.Create(nameof(ZekrDescription), typeof(string), typeof(ZekrView), string.Empty);

    public static readonly BindableProperty DisplayContentProperty =
        BindableProperty.Create(nameof(DisplayContent), typeof(string), typeof(ZekrView), string.Empty);

    public static readonly BindableProperty ZekrTargetTextProperty =
        BindableProperty.Create(nameof(ZekrTargetText), typeof(string), typeof(ZekrView), string.Empty);

    public static readonly BindableProperty ZekrTargetCountProperty =
        BindableProperty.Create(nameof(ZekrTargetCount), typeof(string), typeof(ZekrView), string.Empty);

    public static readonly BindableProperty ZekrCategoryProperty =
        BindableProperty.Create(nameof(ZekrCategory), typeof(string), typeof(ZekrView), string.Empty);

    public static readonly BindableProperty ZekrCurrentTextProperty =
    BindableProperty.Create(nameof(ZekrCurrentText), typeof(string), typeof(ZekrView), string.Empty,
        defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty FullTargetTextProperty =
    BindableProperty.Create(nameof(FullTargetText), typeof(string), typeof(ZekrView), string.Empty,
        defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty ZekrCurrentCountProperty =
    BindableProperty.Create(nameof(ZekrCurrentCount), typeof(int), typeof(ZekrView), 0,
        defaultBindingMode: BindingMode.TwoWay); // تأكد من إضافة TwoWay

    public static readonly BindableProperty ZekrIDProperty =
        BindableProperty.Create(nameof(ZekrID), typeof(int), typeof(ZekrView), 0); // تم تغيير string لـ int

    public string ZekrDescription
    {
        get => (string)GetValue(ZekrDescriptionProperty);
        set => SetValue(ZekrDescriptionProperty, value);
    }

    public string DisplayContent
    {
        get => (string)GetValue(DisplayContentProperty);
        set => SetValue(DisplayContentProperty, value);
    }

    public string ZekrTargetText
    {
        get => (string)GetValue(ZekrTargetTextProperty);
        set => SetValue(ZekrTargetTextProperty, value);
    }

    public string ZekrTargetCount
    {
        get => (string)GetValue(ZekrTargetCountProperty);
        set => SetValue(ZekrTargetCountProperty, value);
    }

    public string ZekrCategory
    {
        get => (string)GetValue(ZekrCategoryProperty);
        set => SetValue(ZekrCategoryProperty, value);
    }

    public int ZekrCurrentCount
    {
        get => (int)GetValue(ZekrCurrentCountProperty);
        set => SetValue(ZekrCurrentCountProperty, value);
    }

    public string ZekrCurrentText
    {
        get => (string)GetValue(ZekrCurrentTextProperty);
        set => SetValue(ZekrCurrentTextProperty, value);
    }

    public string FullTargetText
    {
        get => (string)GetValue(FullTargetTextProperty);
        set => SetValue(FullTargetTextProperty, value);
    }

    public int ZekrID
    {
        get => (int)GetValue(ZekrIDProperty);
        set => SetValue(ZekrIDProperty, value);
    }

    public ZekrView()
    {
        InitializeComponent();
        // تأكد أن هذا السطر موجود في الـ XAML أو هنا برمجياً
        ControlTemplate = (ControlTemplate)Resources["ZekrViewControlTemplate"];
    }

    // أضف هذا الجزء الهام جداً لإجبار البيانات على الظهور
    //protected override void OnBindingContextChanged()
    //{
    //    base.OnBindingContextChanged();

    //    if(BindingContext is Alkhulasat.Domain.Models.ZekrModel model)
    //    {
    //        ZekrID = model.ZekrID;
    //        DisplayContent = model.DisplayContent;
    //        ZekrDescription = model.ZekrDescription;
    //        ZekrCategory = model.ZekrCategory;
    //        ZekrCurrentCount = model.ZekrCurrentCount;
    //        ZekrTargetCount = $"({model.ZekrTargetCount})";
    //        if(string.IsNullOrWhiteSpace(model.ZekrTargetText))
    //        {
    //            ZekrTargetText = $"({model.ZekrTargetCount})";
    //        }
    //        else
    //        {
    //            ZekrTargetText = $"{model.ZekrTargetText} ({model.ZekrTargetCount})";
    //        }
    //    }
    //}
}