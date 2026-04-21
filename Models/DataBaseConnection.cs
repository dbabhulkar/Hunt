namespace API_HUNT.DataBaseConnection
{
    public class ConnectionDB
    {
        private static string _connectionString = string.Empty;

        public static void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string getConString(string appId, string extra, string encryptedPassword, bool isProd)
        {
            return _connectionString;
        }

        public static string LoginUpdate(API_HUNT.Models.DBClass dBClass) => string.Empty;
    }
}
