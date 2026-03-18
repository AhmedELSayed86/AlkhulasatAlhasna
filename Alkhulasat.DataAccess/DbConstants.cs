namespace Alkhulasat.DataAccess
{
    public static class DbConstants
    {
        public const string DatabaseFilename = "AlkhulasatData.db3";
       
        public static string DatabasePath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DatabaseFilename);
     }
}
