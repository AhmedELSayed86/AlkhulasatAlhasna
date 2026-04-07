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
                    HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Haptic error: {ex.Message}");
            }
        }
    }
}
