using MySqlConnector;

namespace API_HUNT.Models
{
    public interface IDbConnectionFactory
    {
        MySqlConnection CreateConnection();
    }
}
