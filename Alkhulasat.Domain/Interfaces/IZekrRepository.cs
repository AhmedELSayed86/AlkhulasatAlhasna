using Alkhulasat.Domain.Models;

namespace Alkhulasat.Domain.Interfaces
{
    public interface IZekrRepository
    {
        Task InitializeRepositoryAsync();  
        Task SyncAzkarSmartlyAsync(List<ZekrModel> azkar);
        Task<List<ZekrModel>> GetAzkarByCategory(string category, bool isFemale);
        Task UpdateZekr(ZekrModel zekr);
        Task ResetDailyCountersAsync();
        Task ResetAzkarCountByCategoryAsync(string category);
    }

}
