namespace API_HUNT.DataBaseConnection
{
    public class ConnectionDB
    {
        // MySQL local connection string. Need to be updated with actual credentials for local testing.
        // Update Server, Database, Uid, and Pwd to match your local MySQL setup.
        private const string MySqlConnectionString = "";
        public string getConString(string appId, string extra, string encryptedPassword, bool isProd)
        {
            return MySqlConnectionString;
        }

        public static string LoginUpdate(API_HUNT.Models.DBClass dBClass) => string.Empty;
    }
}
