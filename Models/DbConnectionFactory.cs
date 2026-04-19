namespace API_HUNT.Models
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlConnection CreateConnection()
        {
            var conn = new SqlConnection(_connectionString);
            conn.StateChange += (sender, e) =>
            {
                if (e.CurrentState == System.Data.ConnectionState.Open)
                {
                    using var cmd = ((SqlConnection)sender!).CreateCommand();
                    cmd.CommandText = "SET NAMES utf8mb4 COLLATE utf8mb4_0900_ai_ci;";
                    cmd.ExecuteNonQuery();
                }
            };
            return conn;
        }
    }
}
