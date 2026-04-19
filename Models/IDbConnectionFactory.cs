namespace API_HUNT.Models
{
    public interface IDbConnectionFactory
    {
        SqlConnection CreateConnection();
    }
}
