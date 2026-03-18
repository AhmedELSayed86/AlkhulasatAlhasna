using Alkhulasat.Domain.Models;
using SQLite;

namespace Alkhulasat.DataAccess.Context
{
    public class AppDbContext
    {
        private static readonly AsyncLazy<AppDbContext> _instance = new AsyncLazy<AppDbContext>(async () =>
        {
            var context = new AppDbContext();
            await context.InitializeAsync();
            return context;
        });

        public static Task<AppDbContext> Instance => _instance.Value;

        private SQLiteAsyncConnection? _database;
        // أضف هذه الخاصية لكي يراها الـ Repository
        public SQLiteAsyncConnection? Database => _database;

        //private List<ZekrModel> _cachedAzkar;
        private readonly SemaphoreSlim _initSemaphore = new SemaphoreSlim(1, 1);
        //private readonly SemaphoreSlim _cacheSemaphore = new SemaphoreSlim(1, 1);

        // منشئ عام (للحقن)
        public AppDbContext()
        {
            // لا نقوم بالتهيئة هنا، بل نتركها للمرة الأولى عند الاستخدام
        }

        public async Task InitializeAsync()
        {
            await _initSemaphore.WaitAsync();
            try
            {
                if(_database != null) return;

                // تهيئة متزامنة أولية (أسرع للإدراج)
                await Task.Run(() => SetupDatabaseStructure());

                _database = new SQLiteAsyncConnection(DbConstants.DatabasePath,
                    SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
            }
            finally
            {
                _initSemaphore.Release();
            }
        }

        private void SetupDatabaseStructure()
        {
            using(var db = new SQLiteConnection(DbConstants.DatabasePath))
            {
                db.CreateTable<ZekrModel>();
                // إنشاء الفهرس لسرعة البحث والترتيب
                db.Execute("CREATE INDEX IF NOT EXISTS idx_zekr_category_order ON ZekrModel(ZekrCategory, OrderIndex)");
            }
        }

        private async Task EnsureInitialized()
        {
            if(_database == null)
                await InitializeAsync();
        }

        // دالة جلب البيانات المحدثة
        public async Task<List<ZekrModel>> GetAzkarByCategoryAsync(string category, bool isFemale)
        {
            await EnsureInitialized();

            // نستخدم جملة SQL مع Alias واضح
            string sql = @"
        SELECT *, 
        CASE 
            WHEN ? = 1 AND (ZekrFemale IS NOT NULL AND ZekrFemale <> '') 
            THEN ZekrFemale 
            ELSE ZekrMale 
        END AS DisplayContent
        FROM ZekrModel 
        WHERE (ZekrCategory = ? OR ZekrCategory = 'Both') 
        ORDER BY OrderIndex";

            if(_database == null)
                throw new InvalidOperationException("Database not initialized");
            var azkar = await _database.QueryAsync<ZekrModel>(sql, isFemale ? 1 : 0, category);

            // صمام الأمان: إذا رجع الحقل فارغاً من SQL نتيجة تعارض في المكتبة
            // نقوم بملئه يدوياً من البيانات الموجودة فعلياً في الـ List
            //foreach(var item in azkar.Where(x => string.IsNullOrWhiteSpace(x.DisplayContent)))
            //{
            //    item.DisplayContent = isFemale && !string.IsNullOrWhiteSpace(item.ZekrFemale)
            //                          ? item.ZekrFemale
            //                          : item.ZekrMale;
            //}

            return azkar;
        }

        public async Task<int> SaveZekrAsync(ZekrModel zekr)
        {
            await EnsureInitialized();
            if(_database == null) throw new InvalidOperationException("Database not initialized");
            return zekr.ZekrID != 0 ? await _database.UpdateAsync(zekr) : await _database.InsertAsync(zekr);
        }

        public async Task<int> ExecuteAsync(string sql, params object[] args)
        {
            await EnsureInitialized(); // هذا السطر هو الحل لمنع الـ NullReferenceException
            if(_database == null) throw new InvalidOperationException("Database not initialized");
            return await _database.ExecuteAsync(sql, args);
        }

        public async Task<int> ResetAzkarCountByCategoryAsync(string category)
        {
            // التأكد من تهيئة قاعدة البيانات أولاً
            await EnsureInitialized();

            // نستخدم "ZekrCategory" و "ZekrCurrentCount" بناءً على تسميات كلاس ZekrModel
            string sql = "UPDATE ZekrModel SET ZekrCurrentCount = 0 WHERE ZekrCategory = ?";
            if(_database == null) throw new InvalidOperationException("Database not initialized");
            return await _database.ExecuteAsync(sql, category);
        }

        public async Task<int> ResetDailyCountersAsync()
        {
            // التأكد من تهيئة قاعدة البيانات أولاً
            await EnsureInitialized();

            string sql = "UPDATE ZekrModel SET ZekrCurrentCount = 0";
            if(_database == null) throw new InvalidOperationException("Database not initialized");
            return await _database.ExecuteAsync(sql);
        }
    }
}
