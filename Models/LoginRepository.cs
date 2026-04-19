using System;
using System.Data;

namespace API_HUNT.Models
{
    public class LoginRepository : ILoginRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public LoginRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public LoginUserRecord? GetUserMasterData(string userName)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var cmd = new SqlCommand(@"
                SELECT a.EmpName, a.EmpCode, a.ProfileDescription, a.ProfileId, a.BranchCode, a.Id AS ID, a.Active,
                  CASE
                    WHEN a.Locked = 1 THEN 'Locked'
                    WHEN a.Dormant = 1 THEN 'Dormant'
                    WHEN a.Enabled = 0 THEN 'Disabled'
                    WHEN a.Active = 1 THEN 'Active'
                  END AS Status,
                  a.LastLoginDate,
                  a.LastLogoutDate,
                  b.EmpRole AS User_Role
                FROM UserMaster a
                LEFT JOIN TBL_OVI_RM_Hierarchy_Mapping b ON a.EmpCode = b.EmpCode
                WHERE LTRIM(RTRIM(a.Empcode)) = @p_Empcode
                  AND IFNULL(a.Flag,'') <> 'Resigned'", connection);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Add("@p_Empcode", SqlDbType.VarChar).Value = userName;
            var da = new SqlDataAdapter(cmd);
            var ds = new DataSet();
            connection.Open();
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count == 0)
                return null;

            var row = ds.Tables[0].Rows[0];
            return new LoginUserRecord
            {
                EmpName = row["EmpName"].ToString(),
                UserRole = row["User_Role"].ToString(),
                BranchCode = row["BranchCode"].ToString(),
                ProfileDescription = row["ProfileDescription"].ToString(),
                ID = row["ID"] == DBNull.Value ? null : Convert.ToInt32(row["ID"]),
                Active = Convert.ToInt32(row["Active"]) == 1,
                LastLogoutDate = row["LastLogoutDate"] == DBNull.Value || string.IsNullOrEmpty(row["LastLogoutDate"].ToString())
                    ? null : Convert.ToDateTime(row["LastLogoutDate"].ToString()),
                LastLoginDate = row["LastLoginDate"] == DBNull.Value || string.IsNullOrEmpty(row["LastLoginDate"].ToString())
                    ? null : Convert.ToDateTime(row["LastLoginDate"].ToString()),
                Status = row["Status"].ToString(),
            };
        }

        public string GetUserRole(string userId)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var cmd = new SqlCommand("SELECT Role FROM tbl_API_HUNT_USER WHERE EmpCode = @p_UserId AND IsActive = 1", connection);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@p_UserId", userId);
            var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            connection.Open();
            da.Fill(dt);
            return dt.Rows.Count > 0 ? dt.Rows[0]["Role"].ToString()! : "USER";
        }

        public string GetMofeeUrl()
        {
            using var connection = _connectionFactory.CreateConnection();
            using var cmd = new SqlCommand("SELECT Url FROM tbl_Mofee_Url LIMIT 1", connection);
            var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            connection.Open();
            da.Fill(dt);
            return dt.Rows.Count > 0 ? dt.Rows[0]["Url"].ToString()! : "/Login/Index";
        }
    }
}
