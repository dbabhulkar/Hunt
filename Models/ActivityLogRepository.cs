using System.Data;

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
            using var cmd = new SqlCommand(@"
                INSERT INTO tbl_API_HUNT_Activity_Log_Tracker
                    (Emp_Code, Form_Name, Module_Name, Total_Count, Activity, Activity_Details, Activity_Date)
                VALUES
                    (@p_Emp_Code, @p_Form_Name, @p_Module_Name, @p_Total_Count, @p_Activity, @p_Activity_Details, NOW())", connection);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.Parameters.Add("@p_Emp_Code", SqlDbType.Text).Value = empCode;
            cmd.Parameters.Add("@p_Form_Name", SqlDbType.Text).Value = formName;
            cmd.Parameters.Add("@p_Module_Name", SqlDbType.Text).Value = moduleName;
            cmd.Parameters.Add("@p_Total_Count", SqlDbType.Int).Value = totalCount;
            cmd.Parameters.Add("@p_Activity", SqlDbType.Text).Value = activity;
            cmd.Parameters.Add("@p_Activity_Details", SqlDbType.Text).Value = activityDetails;
            connection.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
