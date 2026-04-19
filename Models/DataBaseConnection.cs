namespace API_HUNT.DataBaseConnection
{
    public class ConnectionDB
    {
        // MySQL local connection string.
        // Update Server, Database, Uid, and Pwd to match your local MySQL setup.
        private const string MySqlConnectionString =
            "Server=localhost;Port=3306;Database=hunt;Uid=root;Pwd=AVNS_GTO7rX-qUiOeLAXxmLz;Charset=utf8mb4;";

        public string getConString(string appId, string extra, string encryptedPassword, bool isProd)
        {
            return MySqlConnectionString;
        }

        public static string LoginUpdate(API_HUNT.Models.DBClass dBClass) => string.Empty;
    }
}
