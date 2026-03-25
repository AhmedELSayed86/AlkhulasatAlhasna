namespace Alkhulasat.App.Views
{
    public partial class TasbihPage : ContentPage
    {
        int count = 0;
        int totalCount = 0;

        public TasbihPage()
        {
            InitializeComponent();

            // تحميل الإجمالي المحفوظ
            totalCount = Preferences.Default.Get("TotalTasbih", 0);
            lblTotal.Text = totalCount.ToString();
        }

        private void OnCounterTapped(object? sender, EventArgs e)
        {
            count++;
            totalCount++;
            lblCount.Text = count.ToString();
            lblTotal.Text = totalCount.ToString();

            // حفظ الإجمالي فوراً
            Preferences.Default.Set("TotalTasbih", totalCount);

            // إضافة الاهتزاز (Haptic Feedback)
            try
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
            }
            catch { /* الجهاز لا يدعم */ }
        }

        private void OnResetClicked(object? sender, TappedEventArgs e)
        {
            count = 0;
            lblCount.Text = "0";
            // الاهتزاز قد يسبب انهياراً في المحاكي إذا لم يتم التعامل معه بحذر
            try
            {
                if(HapticFeedback.Default.IsSupported)
                    HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
            }
            catch { /* تجاهل الخطأ في المحاكي */ }
        }
    }
}
