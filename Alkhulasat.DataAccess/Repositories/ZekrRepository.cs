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
            var connection = _context.Database;
            if(connection == null) return;

            // 1. جلب البيانات الحالية للمقارنة
            var existingAzkar = new List<int>();
            using(var checkCmd = connection.CreateCommand())
            {
                checkCmd.CommandText = "SELECT OrderIndex FROM ZekrModel";
                using var reader = await checkCmd.ExecuteReaderAsync();
                while(reader.Read()) existingAzkar.Add(reader.GetInt32(0));
            }

            // 2. بدء عملية (Transaction) يدوية
            using var transaction = connection.BeginTransaction();
            try
            {
                foreach(var cloudZekr in newAzkar)
                {
                    using var command = connection.CreateCommand();
                    command.Transaction = transaction;

                    if(existingAzkar.Contains(cloudZekr.OrderIndex))
                    {
                        command.CommandText = @"UPDATE ZekrModel SET 
                                ZekrMale = @male, ZekrFemale = @female, ZekrTargetCount = @tCount,
                                ZekrCategory = @cat, ZekrDescription = @desc, ZekrTargetText = @tText
                                WHERE OrderIndex = @idx";
                    }
                    else
                    {
                        command.CommandText = @"INSERT INTO ZekrModel 
                                (ZekrMale, ZekrFemale, ZekrCategory, OrderIndex, ZekrTargetCount, ZekrCurrentCount, ZekrDescription, ZekrTargetText) 
                                VALUES (@male, @female, @cat, @idx, @tCount, 0, @desc, @tText)";
                    }

                    command.Parameters.AddWithValue("@male", (object?)cloudZekr.ZekrMale ?? DBNull.Value);
                    command.Parameters.AddWithValue("@female", (object?)cloudZekr.ZekrFemale ?? DBNull.Value);
                    command.Parameters.AddWithValue("@idx", cloudZekr.OrderIndex);
                    command.Parameters.AddWithValue("@cat", (object?)cloudZekr.ZekrCategory ?? DBNull.Value);
                    command.Parameters.AddWithValue("@tCount", cloudZekr.ZekrTargetCount);
                    command.Parameters.AddWithValue("@desc", (object?)cloudZekr.ZekrDescription ?? DBNull.Value);
                    command.Parameters.AddWithValue("@tText", (object?)cloudZekr.ZekrTargetText ?? DBNull.Value);
                    await command.ExecuteNonQueryAsync();
                }
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
