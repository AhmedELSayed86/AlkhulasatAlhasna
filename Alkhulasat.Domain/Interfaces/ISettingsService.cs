namespace Alkhulasat.Domain.Interfaces
{
    public interface ISettingsService
    {
        bool IsHapticEnabled { get; set; }

        bool IsKeepScreenOn { get; set; }
        bool IsFemaleVersion { get; set; }

        double FontSize { get; set; }

        // إضافة دوال التاريخ هنا
        string GetLastOpenDate(string category);
        void SetLastOpenDate(string category, string date);
        string GetAzkarVersion();
        void SetAzkarVersion(string version);
    }
}
