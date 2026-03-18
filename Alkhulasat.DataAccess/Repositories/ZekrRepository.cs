using Alkhulasat.DataAccess.Context;
using Alkhulasat.Domain.Interfaces;
using Alkhulasat.Domain.Models;

namespace Alkhulasat.DataAccess.Repositories
{
    public class ZekrRepository: IZekrRepository
    {
        private readonly AppDbContext _context;
        public ZekrRepository(AppDbContext context)
        {
            _context = context;
        }
         
        public async Task<List<ZekrModel>> GetAzkarByCategory(string category, bool isFemale)
        { 
            return await _context.GetAzkarByCategoryAsync(category,   isFemale);
        }

        public async Task UpdateZekr(ZekrModel zekr)
        { 
            await _context.SaveZekrAsync(zekr);
        }

        public async Task ResetAzkarCountByCategoryAsync(string category)
        { 
            await _context.ResetAzkarCountByCategoryAsync(category);
        }

        public async Task ResetDailyCountersAsync()
        { 
            await _context.ResetDailyCountersAsync();
        }

        public async Task InitializeRepositoryAsync()
        {
            // استدعاء دالة التهيئة الموجودة في الـ Context الخاص بك
            // تأكد أن AppDbContext لديه دالة تهيئة عامة (Public)
            await _context.InitializeAsync();
        }

        public async Task SyncAzkarSmartlyAsync(List<ZekrModel> newAzkar)
        {
            var db = _context.Database;

            // جلب كل الأذكار المخزنة حالياً في الجهاز
            var existingAzkar = await db.Table<ZekrModel>().ToListAsync();

            await db.RunInTransactionAsync(conn =>
            {
                foreach(var cloudZekr in newAzkar)
                {
                    // البحث عن الذكر في الجهاز بناءً على رقم الترتيب الثابت (OrderIndex)
                    var localZekr = existingAzkar.FirstOrDefault(x => x.OrderIndex == cloudZekr.OrderIndex);

                    if(localZekr != null)
                    {
                        // الذكر موجود؛ نحدّث النصوص (حتى لو صححت خطأ لغوي سيتم تحديثه)
                        localZekr.ZekrMale = cloudZekr.ZekrMale;
                        localZekr.ZekrFemale = cloudZekr.ZekrFemale;
                        localZekr.ZekrCategory = cloudZekr.ZekrCategory;
                        localZekr.ZekrTargetCount = cloudZekr.ZekrTargetCount;
                        localZekr.ZekrDescription = cloudZekr.ZekrDescription;
                        localZekr.ZekrTargetText = cloudZekr.ZekrTargetText;

                        // لاحظ: لم نلمس localZekr.ZekrCurrentCount ليبقى عداد المستخدم كما هو
                        conn.Update(localZekr);
                    }
                    else
                    {
                        // ذكر جديد بـ OrderIndex جديد؛ نقوم بإضافته
                        conn.Insert(cloudZekr);
                    }
                }
            });
        }
    }
}
