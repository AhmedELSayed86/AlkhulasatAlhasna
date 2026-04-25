namespace Alkhulasat.DataAccess
{
    public static class DbConstants
    {
        public const string DatabaseFilename = "AlkhulasatData.db3";
        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

        public static async Task CopyDatabaseIfNeeded()
        {
            var targetPath = DatabasePath;
           
            if(File.Exists(targetPath))
                return;
            try
            {
                // استخراج قاعدة البيانات الحقيقية من موارد التطبيق (Assets)
                using var stream = await FileSystem.OpenAppPackageFileAsync(DatabaseFilename);
                using var outStream = File.Create(targetPath);
                await stream.CopyToAsync(outStream);
            }
            catch(Exception)
            {
                // تجاهل الخطأ هنا ليقوم التطبيق بإنشاء قاعدة بيانات جديدة فارغة إذا لم يجد الملف المرفق
                System.Diagnostics.Debug.WriteLine("ملف قاعدة البيانات الأولي غير موجود، سيتم إنشاء ملف جديد.");
            }
        }
    }
}
