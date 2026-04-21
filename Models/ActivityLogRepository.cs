using System.Data;
using MySqlConnector;

namespace API_HUNT.Models
{
    public class ActivityLogRepository : IActivityLogRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ActivityLogRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void LogActivity(string empCode, string formName, string moduleName, int totalCount, string activity, string activityDetails)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var cmd = new MySqlCommand(@"
                INSERT INTO tbl_API_HUNT_Activity_Log_Tracker
                    (Emp_Code, Form_Name, Module_Name, Total_Count, Activity, Activity_Details, Activity_Date)
                VALUES
                    (@p_Emp_Code, @p_Form_Name, @p_Module_Name, @p_Total_Count, @p_Activity, @p_Activity_Details, NOW())", connection);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 30;
            cmd.Parameters.Add("@p_Emp_Code", MySqlDbType.Text).Value = empCode;
            cmd.Parameters.Add("@p_Form_Name", MySqlDbType.Text).Value = formName;
            cmd.Parameters.Add("@p_Module_Name", MySqlDbType.Text).Value = moduleName;
            cmd.Parameters.Add("@p_Total_Count", MySqlDbType.Int32).Value = totalCount;
            cmd.Parameters.Add("@p_Activity", MySqlDbType.Text).Value = activity;
            cmd.Parameters.Add("@p_Activity_Details", MySqlDbType.Text).Value = activityDetails;
            connection.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
