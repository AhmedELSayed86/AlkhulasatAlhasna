using Alkhulasat.Domain.Models;
using Microsoft.Data.Sqlite;

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

        private SqliteConnection? _database;
        public SqliteConnection? Database => _database;

        private readonly SemaphoreSlim _initSemaphore = new SemaphoreSlim(1, 1);

        public AppDbContext() { }

        public async Task InitializeAsync()
        {
            await _initSemaphore.WaitAsync();
            try
            {
                if(_database != null) return;

                // [إصلاح حرج جداً - Critical Fix]: 
                // هذا السطر إجباري في MAUI لتهيئة مكتبة SQLite المكتوبة بـ C/C++
                SQLitePCL.Batteries_V2.Init();
            
                // التأكد من نسخ قاعدة البيانات الحقيقية قبل فتح أي اتصال
                await DbConstants.CopyDatabaseIfNeeded();

                _database = new SqliteConnection($"Data Source={DbConstants.DatabasePath}");
                await _database.OpenAsync();
                // التعديل: استدعاء بناء الجداول الإلزامي
                await SetupDatabaseStructureAsync();
            }
            catch(Exception ex)
            {
                // رمي الخطأ للأعلى بدلاً من ابتلاعه لكي نراه بوضوح في الـ ViewModel
                throw new InvalidOperationException($"خطأ في تهيئة قاعدة البيانات: {ex.Message}", ex);
            }
            finally
            {
                _initSemaphore.Release();
            }
        }

        private async Task SetupDatabaseStructureAsync()
        {
            string createTableSql = @"
                CREATE TABLE IF NOT EXISTS ZekrModel (
                    ZekrID INTEGER PRIMARY KEY AUTOINCREMENT,
                    ZekrMale TEXT,
                    ZekrFemale TEXT,
                    ZekrCategory TEXT,
                    OrderIndex INTEGER,
                    ZekrTargetCount INTEGER,
                    ZekrCurrentCount INTEGER,
                    ZekrDescription TEXT,
                    ZekrTargetText TEXT
                );
                CREATE INDEX IF NOT EXISTS idx_zekr_category_order ON ZekrModel(ZekrCategory, OrderIndex);";

            using var command = _database!.CreateCommand();
            command.CommandText = createTableSql;
            await command.ExecuteNonQueryAsync();
        }

        private async Task EnsureInitialized()
        {
            if(_database == null)
                await InitializeAsync();
        }

        // --- عملية الـ Mapping الكاملة ---
        public async Task<List<ZekrModel>> GetAzkarByCategoryAsync(string category, bool isFemale)
        {
            var azkarList = new List<ZekrModel>();

            try
            {
                await EnsureInitialized();

                // استخدام علامه (!) يؤكد للكومبايلر أن _database مستحيل أن يكون null هنا
                // مما يحل تحذير "Dereference of a possibly null reference"
                using var command = _database!.CreateCommand();

                command.CommandText = @"
        SELECT *, 
        CASE 
            WHEN @isFemale = 1 AND (ZekrFemale IS NOT NULL AND ZekrFemale <> '') 
            THEN ZekrFemale 
            ELSE ZekrMale 
        END AS DisplayContent
        FROM ZekrModel 
        WHERE (ZekrCategory = @category OR ZekrCategory = 'Both') 
        ORDER BY OrderIndex";

                command.Parameters.AddWithValue("@isFemale", isFemale ? 1 : 0);
                command.Parameters.AddWithValue("@category", category);

                using var reader = await command.ExecuteReaderAsync();

                while(await reader.ReadAsync())
                {
                    var zekr = new ZekrModel
                    {
                        ZekrID = reader["ZekrID"] != DBNull.Value ? Convert.ToInt32(reader["ZekrID"]) : 0,
                        ZekrMale = reader["ZekrMale"]?.ToString() ?? string.Empty,
                        ZekrFemale = reader["ZekrFemale"]?.ToString() ?? string.Empty,
                        ZekrDescription = reader["ZekrDescription"]?.ToString() ?? string.Empty,
                        ZekrCategory = reader["ZekrCategory"]?.ToString() ?? string.Empty,
                        ZekrTargetCount = reader["ZekrTargetCount"] != DBNull.Value ? Convert.ToInt32(reader["ZekrTargetCount"]) : 1,
                        OrderIndex = reader["OrderIndex"] != DBNull.Value ? Convert.ToInt32(reader["OrderIndex"]) : 0,

                        // [إصلاح حرج]: إضافة החقول التي كانت مفقودة وتسبب اختفاء النص
                        ZekrCurrentCount = reader["ZekrCurrentCount"] != DBNull.Value ? Convert.ToInt32(reader["ZekrCurrentCount"]) : 0,
                        DisplayContent = reader["DisplayContent"]?.ToString() ?? string.Empty,
                  ZekrTargetText = reader["ZekrTargetText"]?.ToString() ?? string.Empty
                    };

                    azkarList.Add(zekr);
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CRITICAL DB ERROR]: {ex.Message}");
            }

            return azkarList;
        }

        // --- إصلاح SaveZekrAsync (بديل Insert و Update) ---
        public async Task<int> SaveZekrAsync(ZekrModel zekr)
        {
            await EnsureInitialized();
            using var command = _database!.CreateCommand();

            if(zekr.ZekrID != 0)
            {
                command.CommandText = @"
                    UPDATE ZekrModel SET 
                    ZekrMale = @male, ZekrFemale = @female, ZekrCategory = @cat, 
                    OrderIndex = @idx, ZekrTargetCount = @tCount, ZekrCurrentCount = @cCount, 
                    ZekrDescription = @desc, ZekrTargetText = @tText
                    WHERE ZekrID = @id";
                command.Parameters.AddWithValue("@id", zekr.ZekrID);
            }
            else
            {
                command.CommandText = @"
                    INSERT INTO ZekrModel (ZekrMale, ZekrFemale, ZekrCategory, OrderIndex, ZekrTargetCount, ZekrCurrentCount, ZekrDescription, ZekrTargetText)
                    VALUES (@male, @female, @cat, @idx, @tCount, @cCount, @desc, @tText)";
            }

            command.Parameters.AddWithValue("@male", (object?)zekr.ZekrMale ?? DBNull.Value);
            command.Parameters.AddWithValue("@female", (object?)zekr.ZekrFemale ?? DBNull.Value);
            command.Parameters.AddWithValue("@cat", (object?)zekr.ZekrCategory ?? DBNull.Value);
            command.Parameters.AddWithValue("@idx", zekr.OrderIndex);
            command.Parameters.AddWithValue("@tCount", zekr.ZekrTargetCount);
            command.Parameters.AddWithValue("@cCount", zekr.ZekrCurrentCount);
            command.Parameters.AddWithValue("@desc", (object?)zekr.ZekrDescription ?? DBNull.Value);
            command.Parameters.AddWithValue("@tText", (object?)zekr.ZekrTargetText ?? DBNull.Value);

            return await command.ExecuteNonQueryAsync();
        }

        // --- إصلاح ExecuteAsync ---
        public async Task<int> ExecuteAsync(string sql, params object[] args)
        {
            await EnsureInitialized();
            using var command = _database!.CreateCommand();
            command.CommandText = sql;

            // تحويل البارامترات من ? إلى @p0, @p1 في حال كنت تستخدم النمط القديم
            for(int i = 0; i < args.Length; i++)
            {
                var paramName = $"@p{i}";
                // إذا كان الـ SQL يحتوي على ? نقوم باستبداله تدريجياً
                if(command.CommandText.Contains("?"))
                {
                    int index = command.CommandText.IndexOf("?");
                    command.CommandText = command.CommandText.Remove(index, 1).Insert(index, paramName);
                }
                command.Parameters.AddWithValue(paramName, args[i] ?? DBNull.Value);
            }

            return await command.ExecuteNonQueryAsync();
        }

        public async Task<int> ResetAzkarCountByCategoryAsync(string category)
        {
            string sql = "UPDATE ZekrModel SET ZekrCurrentCount = 0 WHERE ZekrCategory = @p0";
            return await ExecuteAsync(sql, category);
        }

        public async Task<int> ResetDailyCountersAsync()
        {
            string sql = "UPDATE ZekrModel SET ZekrCurrentCount = 0";
            return await ExecuteAsync(sql);
        }
    }
}
