using Alkhulasat.Domain.Interfaces;
using System.Diagnostics;

namespace Alkhulasat.App.Services
{
    public class HapticService : IHapticService
    {
        public void PerformClick()
        {
            try
            {
                if(HapticFeedback.Default.IsSupported)
                    HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
            }
            catch
            {
                // تجاهل إذا كان الجهاز لا يدعم الاهتزاز
            }
        }
    }
}
