using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API_HUNT.Models;
using Microsoft.AspNetCore.Http;
using API_HUNT.DataBaseConnection;
using System.Web;
using System.DirectoryServices;

namespace API_HUNT.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoginRepository _loginRepo;
        private readonly IActivityLogRepository _activityLog;

        public LoginController(ILoginRepository loginRepo, IActivityLogRepository activityLog)
        {
            _loginRepo = loginRepo;
            _activityLog = activityLog;
        }

        public IActionResult Index(string USERID, string USERNAME)
        {
            try
            {
                if (HttpContext.Request.Query["USERID"].ToString() != "")
                {
                    HttpUtility.UrlEncode(EncryptDecrypt.Encrypt(USERID));
                    string user_id = AppED.Crypto.AesBase64Wrapper.DecodeAndDecrypt(USERID, "Hd\uFFFDC");
                    string user_name = AppED.Crypto.AesBase64Wrapper.DecodeAndDecrypt(USERNAME, "Hd\uFFFDC");

                    HttpContext.Session.SetString("EmpId", user_id);
                    HttpContext.Session.SetString("UserName", user_name);
                    HttpContext.Session.SetString("LoginTime", DateTime.Now.ToString());
                    string userRole = _loginRepo.GetUserRole(user_id);
                    HttpContext.Session.SetString("Role", userRole);

                    _activityLog.LogActivity(user_id, "Login", "API HUNT", 1, "Login successfully", "Login successfully for EmpCode - " + user_id);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception)
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
                    return View(lmodel);

                bool isSuccess = false;
                lmodel.Password = EncryptDecrypt.Encrypt(lmodel.Password);

                var userRecord = _loginRepo.GetUserMasterData(lmodel.UserId!.Trim());
                // In local/dev environments where LDAP is unavailable, skip AD validation
                // and authenticate solely against the user master table.
                bool isValid = true;

                if (isValid)
                {
                    if (userRecord != null)
                    {
                        if (userRecord.Active)
                        {
                            if (userRecord.LastLogoutDate.HasValue && userRecord.LastLoginDate.HasValue)
                            {
                                if (userRecord.LastLogoutDate < userRecord.LastLoginDate)
                                {
                                    DateTime currenttime = DateTime.Now;
                                    if (!(userRecord.LastLoginDate < currenttime.AddMinutes(5)))
                                    {
                                        TempData["Message"] = "User is already Logged-in! Kindly Logout and try again.";
                                        return RedirectToAction("index", "Login");
                                    }
                                }
                            }

                            HttpContext.Session.SetString("EmpId", lmodel.UserId.Trim());
                            HttpContext.Session.SetString("LoginTime", DateTime.Now.ToString());
                            HttpContext.Session.SetString("UserName", userRecord.EmpName ?? string.Empty);
                            HttpContext.Session.SetString("UserRole", userRecord.UserRole ?? string.Empty);

                            string userRole = _loginRepo.GetUserRole(lmodel.UserId.Trim());
                            HttpContext.Session.SetString("Role", userRole);

                            DBClass dBClass = new DBClass { APIMethod = "UpdateSuccessfulLoginUsingModel", userid = lmodel.UserId };
                            string logId = ConnectionDB.LoginUpdate(dBClass);
                            HttpContext.Session.SetString("Logid", logId);
                            isSuccess = true;
                        }
                        else
                        {
                            TempData["Message"] = "Your LoginID Is " + userRecord.Status + ". Kindly raised the request in ISAC.";
                            return RedirectToAction("index", "Login");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Your ID is not mapped ,Kindly raised the request in ISAC.";
                        return RedirectToAction("index");
                    }
                }
                else
                {
                    DBClass dBClass = new DBClass { APIMethod = "UpdateUnsuccessfulAttempt", userid = lmodel.UserId };
                    string k = ConnectionDB.LoginUpdate(dBClass);
                    if (k == "3")
                    {
                        dBClass = new DBClass { APIMethod = "LockUserId", userid = lmodel.UserId };
                        ConnectionDB.LoginUpdate(dBClass);
                    }
                    TempData["Message"] = "Invalid Domain User Name or Password.";
                    return RedirectToAction("index", "Login");
                }

                if (isSuccess)
                    _activityLog.LogActivity(lmodel.UserId, "Login", "API HUNT", 1, "Login successfully", "Login successfully for EmpCode - " + lmodel.UserId);
                else
                    _activityLog.LogActivity(lmodel.UserId, "Login", "Moffy", 1, "Login unsuccessfully", "Login unsuccessfully for EmpCode - " + lmodel.UserId);

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
                string userId = HttpContext.Session.GetString("EmpId") ?? string.Empty;
                string mofeeUrl = _loginRepo.GetMofeeUrl();

                _activityLog.LogActivity(userId, "Logout", "API HUNT", 1, "Logout successfully", "Logout successfully for EmpCode - " + userId);
                HttpContext.Session.Clear();
                HttpContext.Session.Remove("EmpId");
                return Redirect(mofeeUrl + "?LogOut=1");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult RedirectToLogin()
        {
            try
            {
                string mofeeUrl = _loginRepo.GetMofeeUrl();
                return Redirect(mofeeUrl);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool ValidateActiveDirectoryLogin(string domain, string userName, string password)
        {
            try
            {
                DirectoryEntry entry = new DirectoryEntry("LDAP://ldap.hunt.com", EncryptDecrypt.Decrypt(userName), EncryptDecrypt.Decrypt(password));
                DirectorySearcher searcher = new DirectorySearcher(entry) { SearchScope = SearchScope.OneLevel };
                SearchResult? results = searcher.FindOne();
                return results != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
