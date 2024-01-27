using Hunt.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Hunt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API_Adda.Models;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using API_Adda.DataBaseConnection;
using System.Web;
using System.DirectoryServices;

namespace API_Adda.Controllers
{
    public class LoginController : Controller
    {
        SqlConnection sqlCon = new SqlConnection(Startup.connectionstring);
        DataSet chkstatus = new DataSet();
        ResponseContent message = new ResponseContent();
        public IActionResult Index(string USERID, string USERNAME)
        {
            try
            {
                string UserId = "";
                if (HttpContext.Request.Query["USERID"].ToString() != "")
                {
                    HttpUtility.UrlEncode(EncryptDecrypt.Encrypt(USERID));
                    string user_id = AppED.Crypto.AesBase64Wrapper.DecodeAndDecrypt(USERID, "HdƒC");
                    string user_name = AppED.Crypto.AesBase64Wrapper.DecodeAndDecrypt(USERNAME, "HdƒC");

                    HttpContext.Session.SetString("EmpId", user_id);
                    HttpContext.Session.SetString("UserName", user_name);
                    HttpContext.Session.SetString("LoginTime", DateTime.Now.ToString());
                    string UserRole = GetUserRole(user_id);
                    HttpContext.Session.SetString("Role", UserRole);
                    UserId = HttpContext.Session.GetString("EmpId");
                    CaptureProductivityDetails(sqlCon, UserId, "Login", "API ADDA", 1, "Login successfully", "Login successfully for EmpCode - " + UserId.ToString().Trim());

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    //CaptureProductivityDetails(sqlCon, UserId, "Login", "API ADDA", 1, "Login successfully", "Login successfully for EmpCode - " + UserId.ToString().Trim());
                    return View();
                }
            }
            catch (Exception ex)
            {

                throw;
            }




        }
        [HttpPost]
        public ActionResult Index(LoginModel lmodel)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(lmodel);
                }

                message.isSuccess = "true";
                bool Isvalid = false;
                //lmodel.Password = AESEncrytDecry.DecryptStringAES(lmodel.Password);
                lmodel.Password = EncryptDecrypt.Encrypt(lmodel.Password);
                chkstatus = checkUserMaster(lmodel.UserId.Trim());
                Isvalid = ValidateActiveDirectoryLogin("ldap.hbctxdom.com", EncryptDecrypt.Encrypt(lmodel.UserId), lmodel.Password);
                //Isvalid = true;  // comment on production
                if (Isvalid)
                {
                    if (chkstatus.Tables[0].Rows.Count > 0)
                    {
                        //if ("True" == "True") // comment on production
                        if (chkstatus.Tables[0].Rows[0]["Active"].ToString() == "True")
                        {
                            if ((chkstatus.Tables[0].Rows[0]["LastLogoutDate"].ToString()) != null && (chkstatus.Tables[0].Rows[0]["LastLogOutDate"].ToString()) != "")
                            {
                                DateTime? datetimeLastLogout = Convert.ToDateTime(chkstatus.Tables[0].Rows[0]["LastLogoutDate"].ToString());
                                DateTime? datetimeLogin = Convert.ToDateTime(chkstatus.Tables[0].Rows[0]["LastLoginDate"].ToString());
                                if (datetimeLastLogout < datetimeLogin)
                                {
                                    DateTime currenttime = System.DateTime.Now;
                                    if (!(datetimeLogin < currenttime.AddMinutes(5)))
                                    {
                                        TempData["Message"] = "User is already Logged-in! Kindly Logout and try again.";

                                        message.isSuccess = "false";
                                        //return new JsonResult(message);
                                        return RedirectToAction("index", "Login");
                                    }
                                }
                            }


                            HttpContext.Session.SetString("EmpId", lmodel.UserId.Trim());
                            HttpContext.Session.SetString("LoginTime", DateTime.Now.ToString());
                            string UserRole = GetUserRole(lmodel.UserId.Trim());
                            HttpContext.Session.SetString("Role", UserRole);

                            DBClass dBClass = new DBClass();
                            dBClass.APIMethod = "UpdateSuccessfulLoginUsingModel";
                            dBClass.userid = lmodel.UserId;
                            string logId = ConnectionDB.LoginUpdate(dBClass);
                            HttpContext.Session.SetString("Logid", logId);
                            //message.Url = "/Home/index";
                            //message.isSuccess = "true";

                        }
                        else
                        {
                            TempData["Message"] = "Your LoginID Is " + chkstatus.Tables[0].Rows[0]["Status"].ToString() + ". Kindly raised the request in ISAC.";
                            message.isSuccess = "false";
                            return RedirectToAction("index", "Login");

                        }
                    }
                    else
                    {
                        TempData["Message"] = "Your ID is not mapped ,Kindly raised the request in ISAC.";
                        message.isSuccess = "false";
                        //TempData.Keep("Message");
                        //TempData.Peek("Message");

                        return RedirectToAction("index");
                    }
                }
                else
                {
                    string k = "";
                    DBClass dBClass = new DBClass();
                    dBClass.APIMethod = "UpdateUnsuccessfulAttempt";
                    dBClass.userid = lmodel.UserId;
                    k = ConnectionDB.LoginUpdate(dBClass);
                    if ((k == "3"))
                    {
                        dBClass = new DBClass();
                        dBClass.APIMethod = "LockUserId";
                        dBClass.userid = lmodel.UserId;
                        ConnectionDB.LoginUpdate(dBClass);
                    }
                    TempData["Message"] = "Invalid Domain User Name or Password.";
                    message.isSuccess = "false";
                    return RedirectToAction("index", "Login");
                }
                if (message.isSuccess == "true")
                {
                    CaptureProductivityDetails(sqlCon, lmodel.UserId, "Login", "API ADDA", 1, "Login successfully", "Login successfully for EmpCode - " + lmodel.UserId.ToString().Trim());

                }
                else
                {
                    CaptureProductivityDetails(sqlCon, lmodel.UserId, "Login", "Moffy", 1, "Login unsuccessfully", "Login unsuccessfully for EmpCode - " + lmodel.UserId.ToString().Trim());

                }

                return RedirectToAction("index", "Home");
            }
            catch (Exception)
            {

                throw;
            }

        }

        [HttpGet]
        public ActionResult LogoutUser()
        {
            try
            {
                string Url = "";
                string MofeeUrllink = GetMOfeeUrl();
                string UserId = HttpContext.Session.GetString("EmpId");

                Url = MofeeUrllink;     //"http://localhost:1438/Login.aspx";

                CaptureProductivityDetails(sqlCon, UserId, "Logout", "API ADDA", 1, "Logout successfully", "Logout successfully for EmpCode - " + UserId.ToString().Trim());
                HttpContext.Session.Clear();
                HttpContext.Session.Remove("EmpId");
                return Redirect(Url + "?LogOut=1");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        [HttpPost]
        public ActionResult RedirectToLogin()
        {

            try
            {
                string MofeeUrllink = GetMOfeeUrl();
                //string sess = Global.UserID;
                //string link = "http://localhost:1438/Login.aspx";
                //string abc = HttpContext.Session.GetString("LoginType").ToString();
                //Global.LoginUrl = null;
                return Redirect(MofeeUrllink);

            }
            catch (Exception)
            {

                throw;
            }
        }

        private DataSet checkUserMaster(string userName)
        {
            SqlCommand sqlcmd = null;
            try
            {
                LoginModel log = new LoginModel();
                DataSet ds = new DataSet();
                sqlcmd = new SqlCommand("sp_Login_New", sqlCon);
                sqlcmd.CommandTimeout = 0;
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.Parameters.Add("@Type", SqlDbType.VarChar).Value = 5;
                sqlcmd.Parameters.Add("@Empcode", SqlDbType.VarChar).Value = userName;
                SqlDataAdapter da = new SqlDataAdapter(sqlcmd);
                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }
                da.Fill(ds);
                da.Dispose();
                sqlcmd.Dispose();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    log.BranchCode = ds.Tables[0].Rows[0]["BranchCode"].ToString();
                    log.GlobalOrgIdRole = ds.Tables[0].Rows[0]["ProfileDescription"].ToString();
                    log.RefID = Convert.ToInt32(ds.Tables[0].Rows[0]["ID"]);
                    log.UserName = ds.Tables[0].Rows[0]["EmpName"].ToString();
                    HttpContext.Session.SetString("UserName", ds.Tables[0].Rows[0]["EmpName"].ToString());
                    //Session["LUserName"] = ds.Tables[0].Rows[0]["EmpName"].ToString();
                    log.Role = ds.Tables[0].Rows[0]["User_Role"].ToString();
                    HttpContext.Session.SetString("UserRole", ds.Tables[0].Rows[0]["User_Role"].ToString());

                }
                sqlCon.Close();
                return ds;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open)
                {
                    sqlCon.Close();
                }
                if (sqlcmd != null)
                {
                    sqlcmd.Dispose();
                }
            }
        }
        private DataTable chkmappmaster(int intdataid, string Username)
        {
            SqlCommand sqlcmd = null;
            try
            {
                DataSet ds = new DataSet();
                sqlcmd = new SqlCommand("check_user", sqlCon);
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = Username;
                sqlcmd.Parameters.Add("@intdataid", SqlDbType.VarChar).Value = intdataid;
                SqlDataAdapter da = new SqlDataAdapter(sqlcmd);
                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }
                da.Fill(ds);
                da.Dispose();
                sqlcmd.Dispose();
                sqlCon.Close();
                return ds.Tables[0];
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open)
                {
                    sqlCon.Close();
                }
                if (sqlcmd != null)
                {
                    sqlcmd.Dispose();
                }
            }
        }

        private string GetMOfeeUrl()
        {
            SqlCommand sqlcmd = null;
            try
            {
                string URl = "";
                DataTable dt = new DataTable();
                sqlcmd = new SqlCommand("SP_Select_Mofee_Url", sqlCon);
                sqlcmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(sqlcmd);
                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    URl = dt.Rows[0]["Url"].ToString();
                }
                da.Dispose();
                sqlcmd.Dispose();
                sqlCon.Close();
                return URl;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open)
                {
                    sqlCon.Close();
                }
                if (sqlcmd != null)
                {
                    sqlcmd.Dispose();
                }
            }
        }

        public string GetUserRole(string UserId)
        {
            string userRole = "";
            SqlCommand sqlcmd = null;
            try
            {

                DataTable dt = new DataTable();
                sqlcmd = new SqlCommand("SP_APIA_GetUserRole", sqlCon);
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.Parameters.AddWithValue("@UserId", UserId);
                SqlDataAdapter da = new SqlDataAdapter(sqlcmd);
                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    userRole = dt.Rows[0]["Role"].ToString();
                }
                else
                {
                    userRole = "USER";
                }
                return userRole;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open)
                {
                    sqlCon.Close();
                }
                if (sqlcmd != null)
                {
                    sqlcmd.Dispose();
                }
            }

        }

        public bool ValidateActiveDirectoryLogin(string Domain, string UserName, string Password)
        {
            bool success = false;
            //return true;
            try
            {
                string DN;
                DN = "ldap.hbctxdom.com";
                DirectoryEntry Entry = new DirectoryEntry("LDAP://ldap.hbctxdom.com", EncryptDecrypt.Decrypt(UserName), EncryptDecrypt.Decrypt(Password));
                DirectorySearcher searcher = new DirectorySearcher(Entry);
                searcher.SearchScope = SearchScope.OneLevel;

                SearchResult results = searcher.FindOne();
                success = (results != null);
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }

        public void CaptureProductivityDetails(SqlConnection Con, string Empcode, string Form_Name, string Module_Name, int Total_Count, string Activity, string Activity_Details)
        {
            SqlCommand cmd = null;
            try
            {
                if (Con.State == ConnectionState.Closed) { Con.Open(); }

                cmd = new SqlCommand("USP_Insert_Data_In_Activity_Log_Tracker_API_Adda", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Emp_Code", SqlDbType.Text).Value = Empcode;
                cmd.Parameters.Add("@Form_Name", SqlDbType.Text).Value = Form_Name;
                cmd.Parameters.Add("@Module_Name", SqlDbType.Text).Value = Module_Name;
                cmd.Parameters.Add("@Total_Count", SqlDbType.Int).Value = Total_Count;
                cmd.Parameters.Add("@Activity", SqlDbType.Text).Value = Activity;
                cmd.Parameters.Add("@Activity_Details", SqlDbType.Text).Value = Activity_Details;


                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
                cmd.Dispose();



                if (Con.State == ConnectionState.Open) { Con.Close(); }
            }
            catch (Exception)
            { throw; }
            finally
            {
                if (Con.State == ConnectionState.Open) { Con.Close(); }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
        }
    }
}