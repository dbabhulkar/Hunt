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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API_Adda.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Security;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml.Style;
using System.Drawing;
using OfficeOpenXml;
using System.IO.Compression;
using System.Net.Http;

//using MvcContrib.Filters;


namespace API_Adda.Controllers
{
    [CustomFilter]

    // [PassParametersDuringRedirect]
    public class HomeController : Controller
    {
        SqlConnection sqlCon = new SqlConnection(Startup.connectionstring);

        SubmitRepository submitRepository = new SubmitRepository();
        int workflowstatus = 0;
        SqlCommand cmd = null;
        SqlDataAdapter sda = null;

        private readonly HttpClient _httpClient;

        public IActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {

                throw;
            }

        }
        [HttpGet]
        public IActionResult DiagnoseIssue()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {

                throw;
            }

        }
        [HttpPost]
        public IActionResult DiagnoseIssue(IFormCollection fc)
        {
            string URLTab = "";
            try
            {
                string RadioValue = fc["diagnose"];
                if (RadioValue == "OBP")
                {
                    RadioValue = "OBP UAT";
                }
                else if (RadioValue == "SOA")
                {
                    RadioValue = "SOA UAT";
                }
                else if (RadioValue == "APIGW")
                {
                    RadioValue = "APIGW UAT";
                }
                DataTable dt = submitRepository.DiagnoseUrl(RadioValue);
                if (dt.Rows.Count > 0)
                {
                    URLTab = dt.Rows[0]["URL"].ToString();
                }
                if (URLTab == "")
                {
                    return Content("No URL!!!");
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return Redirect(URLTab);

        }
        [HttpGet]
        public IActionResult DiagnoseIssueLive()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {

                throw;
            }

        }
        [HttpPost]
        public IActionResult DiagnoseIssueLive(IFormCollection fc)
        {
            string URLTab = "";
            try
            {

                string RadioValue = fc["diagnose"];
                if (RadioValue == "OBP")
                {
                    RadioValue = "OBP Live";
                }
                else if (RadioValue == "SOA")
                {
                    RadioValue = "SOA Live";
                }
                else if (RadioValue == "APIGW")
                {
                    RadioValue = "APIGW Live";
                }
                DataTable dt = submitRepository.DiagnoseUrl(RadioValue);
                if (dt.Rows.Count > 0)
                {
                    URLTab = dt.Rows[0]["URL"].ToString();
                }
                if (URLTab == "")
                {
                    return Content("No URL!!!");
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            return Redirect(URLTab);
        }

        [HttpGet]

        public IActionResult search(string button = null)
        {
            try
            {
                SearchAPI searchModel = new SearchAPI();
                searchModel = submitRepository.searchAPIList(searchModel);
                string UserId = HttpContext.Session.GetString("EmpId");
                CaptureProductivityDetails(sqlCon, UserId, "Search", "API ADDA", 1, "Search ", " Test API In for EmpCode - " + UserId.Trim());
                ModelState.Clear();
                return View(searchModel);
                //var ActionName = TempData["SearchAction"];
                //if (ActionName == null || "SearchView" == TempData["SearchAction"].ToString())
                //{
                //    SearchAPI searchModel = new SearchAPI();
                //    searchModel = submitRepository.searchAPIList(searchModel);
                //    return View(searchModel);
                //}
                //else
                //{
                //    var searchModel = TempData["SearchData"];
                //    var result = JsonConvert.DeserializeObject<SearchAPI>(searchModel.ToString());
                //    TempData["SearchAction"] = "SearchView";
                //    return View(result);
                //}
            }
            catch (Exception)
            {

                throw;
            }


        }

        public IActionResult NewIntegrationList(NewIntegration newIntegration = null, int integrationId = 0, string Status = "")
        {
            try
            {
                // Logger.log("frmload");
                string statusName = "";
                if (Status == "1")
                {
                    statusName = "Review To BTGUser";
                }
                else if (Status == "3")
                {
                    statusName = "Review To ITUSER";
                }
                else if (Status == "4")
                {
                    statusName = "Review To ITARCHITECH";
                }
                else if (Status == "2")
                {
                    statusName = "Rejected";
                }
                else if (Status == "5")
                {
                    statusName = "Closed";
                }
                else if (Status == "6")
                {
                    statusName = "Feedbacktouser";
                    newIntegration.AssignFrom = HttpContext.Session.GetString("Role");

                    newIntegration = submitRepository.GetNewIntegrationDetails(newIntegration);
                    ViewBag.BTGUSER = newIntegration.integrationLists.Where(x => x.AssignTo == "BTGUSER").Count();
                    ViewBag.ITUSER = newIntegration.integrationLists.Where(x => x.AssignTo == "ITUSER").Count();
                    ViewBag.ITARCHITECH = newIntegration.integrationLists.Where(x => x.AssignTo == "ITARCHITECH").Count();
                    ViewBag.Rejected = newIntegration.integrationLists.Where(x => x.Status == "Rejected").Count();
                    ViewBag.Closed = newIntegration.integrationLists.Where(x => x.Status == "Closed").Count();
                    if (HttpContext.Session.GetString("Role") == "BTGUSER")
                    {
                        ViewBag.Feedbacktouser = newIntegration.integrationLists.Where(x => x.Status == "Feedback by BTG User").Count();
                    }
                    else if (HttpContext.Session.GetString("Role") == "ITUSER")
                    {
                        ViewBag.Feedbacktouser = newIntegration.integrationLists.Where(x => x.Status == "Feedback by ITUser").Count();
                    }
                    else if (HttpContext.Session.GetString("Role") == "ITARCHITECH")
                    {
                        ViewBag.Feedbacktouser = newIntegration.integrationLists.Where(x => x.Status == "Feedback by ITARCHITECH").Count();
                    }


                    newIntegration = submitRepository.GetFeedbackDetails(newIntegration, newIntegration.AssignFrom);
                }
                if (newIntegration.IntegrationId > 0)
                {
                    newIntegration = submitRepository.GetNewIntegrationById(newIntegration.IntegrationId);
                    ViewBag.SubmitValue = "Update";
                    ViewBag.Number = "1";
                    ViewBag.Role = HttpContext.Session.GetString("Role");
                }

                else
                {
                    newIntegration.AssignFrom = HttpContext.Session.GetString("Role");
                    newIntegration.UserId = HttpContext.Session.GetString("EmpId");
                    newIntegration = submitRepository.GetNewIntegrationDetails(newIntegration);
                    ViewBag.SubmitValue = "Save";
                    ViewBag.Number = "0";
                    ViewBag.Role = HttpContext.Session.GetString("Role");
                }
                if (HttpContext.Session.GetString("Role") == "USER")
                {
                    ViewBag.BTGUSER = newIntegration.integrationLists.Where(x => x.AssignTo == "BTGUSER").Count();
                    ViewBag.ITUSER = newIntegration.integrationLists.Where(x => x.AssignTo == "ITUSER").Count();
                    ViewBag.ITARCHITECH = newIntegration.integrationLists.Where(x => x.AssignTo == "ITARCHITECH").Count();
                    ViewBag.Rejected = newIntegration.integrationLists.Where(x => x.Status == "Rejected").Count();
                    ViewBag.Closed = newIntegration.integrationLists.Where(x => x.Status == "Closed").Count();

                    ViewBag.statusName = "False";
                    if (statusName != "")
                    {
                        if (statusName == "Review To ITUSER")
                        {
                            newIntegration.integrationLists = newIntegration.integrationLists.Where(x => x.AssignTo == "ITUSER").ToList();
                        }
                        else if (statusName == "Review To ITARCHITECH")
                        {
                            newIntegration.integrationLists = newIntegration.integrationLists.Where(x => x.AssignTo == "ITARCHITECH").ToList();
                        }

                        else if (statusName == "Review To BTGUser")
                        {
                            newIntegration.integrationLists = newIntegration.integrationLists.Where(x => x.AssignTo == "BTGUSER").ToList();
                        }
                        else
                        {
                            newIntegration.integrationLists = newIntegration.integrationLists.Where(x => x.Status == statusName).ToList();
                        }
                        ViewBag.statusName = "True";

                    }
                }
                else if (HttpContext.Session.GetString("Role") == "BTGUSER")
                {
                    ViewBag.BTGUSER = newIntegration.integrationLists.Where(x => x.AssignTo == "BTGUSER").Count();
                    ViewBag.ITUSER = newIntegration.integrationLists.Where(x => x.AssignTo == "ITUSER").Count();
                    ViewBag.ITARCHITECH = newIntegration.integrationLists.Where(x => x.AssignTo == "ITARCHITECH").Count();
                    ViewBag.Rejected = newIntegration.integrationLists.Where(x => x.Status == "Rejected").Count();
                    ViewBag.Closed = newIntegration.integrationLists.Where(x => x.Status == "Closed").Count();
                    if (HttpContext.Session.GetString("Role") == "BTGUSER")
                    {
                        ViewBag.Feedbacktouser = newIntegration.integrationLists.Where(x => x.Status == "Feedback by BTG User").Count();
                    }
                    ViewBag.statusName = "False";
                    if (statusName != "")
                    {

                        if (statusName == "Review To ITUSER")
                        {
                            newIntegration.integrationLists = newIntegration.integrationLists.Where(x => x.AssignTo == "ITUSER").ToList();
                            HttpContext.Session.SetString("AccessRight", "No AccessRight");

                        }
                        else if (statusName == "Review To ITARCHITECH")
                        {
                            newIntegration.integrationLists = newIntegration.integrationLists.Where(x => x.AssignTo == "ITARCHITECH").ToList();
                            // ViewBag.AccessRight = "No AccessRight";
                            HttpContext.Session.SetString("AccessRight", "No AccessRight");
                        }
                        else if (statusName == "Review To BTGUser")
                        {
                            newIntegration.integrationLists = newIntegration.integrationLists.Where(x => x.AssignTo == "BTGUSER").ToList();
                            ViewBag.AccessRight = "AccessRight";
                            HttpContext.Session.SetString("AccessRight", "AccessRight");
                        }
                        else if (statusName == "Feedbacktouser")
                        {
                            newIntegration.integrationLists = newIntegration.integrationLists.Where(x => x.Status == "Feedback by BTG User").ToList();
                            ViewBag.AccessRight = "AccessRight";
                            HttpContext.Session.SetString("AccessRight", "No AccessRight");
                        }
                        else
                        {
                            newIntegration.integrationLists = newIntegration.integrationLists.Where(x => x.Status == statusName).ToList();
                            //ViewBag.AccessRight = "No AccessRight";
                            HttpContext.Session.SetString("AccessRight", "No AccessRight");
                        }
                        ViewBag.statusName = "True";

                    }
                    else
                    {
                        newIntegration.integrationLists = newIntegration.integrationLists.Where(x => x.AssignTo == "BTGUSER").ToList();
                        HttpContext.Session.SetString("AccessRight", "AccessRight");
                    }

                }
                else if (HttpContext.Session.GetString("Role") == "ITUSER")
                {
                    ViewBag.BTGUSER = newIntegration.integrationLists.Where(x => x.AssignTo == "BTGUSER").Count();
                    ViewBag.ITUSER = newIntegration.integrationLists.Where(x => x.AssignTo == "ITUSER").Count();
                    ViewBag.ITARCHITECH = newIntegration.integrationLists.Where(x => x.AssignTo == "ITARCHITECH").Count();
                    ViewBag.Rejected = newIntegration.integrationLists.Where(x => x.Status == "Rejected").Count();
                    ViewBag.Closed = newIntegration.integrationLists.Where(x => x.Status == "Closed").Count();
                    if (HttpContext.Session.GetString("Role") == "ITUSER")
                    {
                        ViewBag.Feedbacktouser = newIntegration.integrationLists.Where(x => x.Status == "Feedback by IT User").Count();
                    }

                    ViewBag.statusName = "False";
                    if (statusName != "")
                    {

                        if (statusName == "Review To ITUSER")
                        {
                            newIntegration.integrationLists = newIntegration.integrationLists.Where(x => x.AssignTo == "ITUSER").ToList();
                            HttpContext.Session.SetString("AccessRight", "AccessRight");
                        }
                        else if (statusName == "Review To ITARCHITECH")
                        {
                            newIntegration.integrationLists = newIntegration.integrationLists.Where(x => x.AssignTo == "ITARCHITECH").ToList();
                            HttpContext.Session.SetString("AccessRight", "No AccessRight");
                        }
                        else if (statusName == "Review To BTGUser")
                        {
                            newIntegration.integrationLists = newIntegration.integrationLists.Where(x => x.AssignTo == "BTGUSER").ToList();
                            ViewBag.AccessRight = "AccessRight";
                            HttpContext.Session.SetString("AccessRight", "No AccessRight");
                        }
                        else if (statusName == "Feedbacktouser")
                        {
                            newIntegration.integrationLists = newIntegration.integrationLists.Where(x => x.Status == "Feedback by IT User").ToList();
                            ViewBag.AccessRight = "AccessRight";
                            HttpContext.Session.SetString("AccessRight", "AccessRight");
                        }
                        else
                        {
                            newIntegration.integrationLists = newIntegration.integrationLists.Where(x => x.Status == statusName).ToList();
                            HttpContext.Session.SetString("AccessRight", "No AccessRight");
                        }
                        ViewBag.statusName = "True";

                    }
                    else
                    {
                        newIntegration.integrationLists = newIntegration.integrationLists.Where(x => x.AssignTo == "ITUSER").ToList();
                        HttpContext.Session.SetString("AccessRight", "AccessRight");
                    }
                }
                else if (HttpContext.Session.GetString("Role") == "ITARCHITECH")
                {
                    ViewBag.BTGUSER = newIntegration.integrationLists.Where(x => x.AssignTo == "BTGUSER").Count();
                    ViewBag.ITUSER = newIntegration.integrationLists.Where(x => x.AssignTo == "ITUSER").Count();
                    ViewBag.ITARCHITECH = newIntegration.integrationLists.Where(x => x.AssignTo == "ITARCHITECH").Count();
                    ViewBag.Rejected = newIntegration.integrationLists.Where(x => x.Status == "Rejected").Count();
                    ViewBag.Closed = newIntegration.integrationLists.Where(x => x.Status == "Closed").Count();
                    if (HttpContext.Session.GetString("Role") == "ITARCHITECH")
                    {
                        ViewBag.Feedbacktouser = newIntegration.integrationLists.Where(x => x.Status == "Feedback by IT ARCHITECH").Count();
                    }
                    ViewBag.statusName = "False";
                    if (statusName != "")
                    {

                        if (statusName == "Review To ITUSER")
                        {
                            newIntegration.integrationLists = newIntegration.integrationLists.Where(x => x.AssignTo == "ITUSER").ToList();
                            HttpContext.Session.SetString("AccessRight", "No AccessRight");
                        }
                        else if (statusName == "Review To ITARCHITECH")
                        {
                            newIntegration.integrationLists = newIntegration.integrationLists.Where(x => x.AssignTo == "ITARCHITECH").ToList();
                            HttpContext.Session.SetString("AccessRight", "AccessRight");
                        }
                        else if (statusName == "Review To BTGUser")
                        {
                            newIntegration.integrationLists = newIntegration.integrationLists.Where(x => x.AssignTo == "BTGUSER").ToList();
                            ViewBag.AccessRight = "AccessRight";
                            HttpContext.Session.SetString("AccessRight", "No AccessRight");
                        }
                        else if (statusName == "Feedbacktouser")
                        {
                            newIntegration.integrationLists = newIntegration.integrationLists.Where(x => x.Status == "Feedback by IT ARCHITECH").ToList();
                            ViewBag.AccessRight = "AccessRight";
                            HttpContext.Session.SetString("AccessRight", "AccessRight");
                        }
                        else
                        {
                            newIntegration.integrationLists = newIntegration.integrationLists.Where(x => x.Status == statusName).ToList();
                            HttpContext.Session.SetString("AccessRight", "No AccessRight");
                        }
                        ViewBag.statusName = "True";

                    }
                    else
                    {
                        newIntegration.integrationLists = newIntegration.integrationLists.Where(x => x.AssignTo == "ITARCHITECH").ToList();
                        HttpContext.Session.SetString("AccessRight", "AccessRight");
                    }
                }

                return View(newIntegration);
            }
            catch (Exception)
            {

                throw;
            }

        }
        public IActionResult newintegration(NewIntegration newIntegration = null, int integrationId = 0)
        {
            try
            {
                string createdDate = null;
                if (newIntegration.IntegrationId > 0)
                {
                    ModelState.Clear();
                    newIntegration = submitRepository.GetNewIntegrationById(newIntegration.IntegrationId);
                    createdDate = newIntegration.CreatedAt.ToString("ddMMMyyyy");

                    //TempData["APIAddaId"] = "API" + createdDate + newIntegration.ParentIntegrationId;
                    var FolderName = "API" + createdDate + (newIntegration.ParentIntegrationId == null || newIntegration.ParentIntegrationId == 0 ? newIntegration.IntegrationId : newIntegration.ParentIntegrationId) + @"\";
                    TempData["APIAddaId"] = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\APIAddaDoc\NewIntegrations\") + FolderName;
                    //TempData["APIAddaId"] = "API" + createdDate + (newIntegration.ParentIntegrationId == null || newIntegration.ParentIntegrationId == 0 ? newIntegration.IntegrationId : newIntegration.ParentIntegrationId);
                    if (newIntegration.ParentIntegrationId == 0)
                    {
                        HttpContext.Session.SetString("folderName", "API" + createdDate + newIntegration.IntegrationId);
                    }
                    else
                    {
                        HttpContext.Session.SetString("folderName", "API" + createdDate + newIntegration.ParentIntegrationId);
                    }

                    if (newIntegration.workflowstatus == 12)
                    {
                        ViewBag.SubmitValue = "Draft";
                    }
                    else
                    {
                        ViewBag.SubmitValue = "Update";
                    }
                    ViewBag.Number = "1";
                    TempData["Editbydb"] = "NewIntegration?integrationId=" + integrationId;

                    //if (newIntegration.UserJourneyFiles != "" && newIntegration.UserJourneyFiles != null)
                    //{
                    //    String[] strlist = newIntegration.UserJourneyFiles.Split("_");
                    //    string setFileNameM = strlist[1].ToString();
                    //    newIntegration.fileName = setFileNameM;
                    //    newIntegration.modifyFileName = newIntegration.UserJourneyFiles;
                    //}

                }
                else
                {
                    if (TempData["SaveFlag"] == null)
                    {
                        ModelState.Clear();
                    }
                    TempData["Editbydb"] = "NewIntegration";
                    newIntegration.AssignFrom = HttpContext.Session.GetString("Role");
                    newIntegration.UserId = HttpContext.Session.GetString("EmpId");
                    newIntegration = submitRepository.GetNewIntegrationDetails(newIntegration);
                    var dateTime = DateTime.Now;
                    newIntegration.GoLiveDate = DateTime.Now.Date;
                    newIntegration.IntegrationId = 0;


                    if (newIntegration.integrationLists == null || newIntegration.integrationLists.Count == 0)
                    {
                        newIntegration.integrationLists = new List<IntegrationList>();
                        newIntegration.integrationLists.Add(new IntegrationList()
                        {
                            workflowstatus = 0

                        });


                    }

                    ViewBag.SubmitValue = "Save";
                    TempData["PopupEdit"] = "AddMode";
                    HttpContext.Session.SetString("PopupEdit", "AddMode");
                    ViewBag.Number = "0";
                }



                return View(newIntegration);
            }
            catch (Exception)
            {

                throw;
            }

        }


        public IActionResult newintegrationDetails(int integrationId)
        {
            try
            {
                NewIntegration newIntegration = new NewIntegration();
                newIntegration.IntegrationId = integrationId;
                return RedirectToAction("newintegration", newIntegration);
            }
            catch (Exception)
            {

                throw;
            }

        }

        [HttpPost]
        public IActionResult newintegration(NewIntegration umodel, string button = null)//,IFormFile ExpAPISPCFDocumentFile = null
        {
            try
            {
                var IntegrationId = umodel.IntegrationId;
                NewIntegration umodel1 = submitRepository.GetNewIntegrationById(umodel.IntegrationId);
                var serviceId = ""; var nonZeroserviceIds = "";
                if (umodel.serviceDetails != null)
                {
                    if (umodel.serviceDetails.Count > 0)
                    {
                        foreach (var item in umodel.serviceDetails)
                        {
                            serviceId += item.ServiceID + ",";
                            string[] serviceIds = serviceId.Split(',');
                            var nonZeroserviceId = serviceIds.Where(value => !value.Equals("0")).ToList();
                            nonZeroserviceIds = string.Join(",", nonZeroserviceId);
                        }
                    }
                }
                string date;
                DataTable dataSet = new DataTable();
                cmd = new SqlCommand("sp_APIA_NewAPIIntegration", sqlCon);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdentFlag", "GetDate");
                cmd.Parameters.AddWithValue("@IntegrationId", umodel.IntegrationId);
                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }
                sda = new SqlDataAdapter(cmd);
                sda.Fill(dataSet);
                sqlCon.Close();
                if (dataSet.Rows.Count > 0)
                {
                    foreach (DataRow row in dataSet.Rows)
                    {
                        umodel.CreatedAt = (DateTime)row["CreatedAt"];
                    }
                    date = umodel.CreatedAt.ToString("ddMMMyyyy");
                }
                else
                {
                    date = DateTime.Now.ToString("ddMMMyyyy");
                }
                NewIntegration newIntegration = new NewIntegration();


                cmd = new SqlCommand("sp_APIA_NewAPIIntegration", sqlCon);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdentFlag", "GetId");
                if (sqlCon.State == ConnectionState.Closed)
                {
                    sqlCon.Open();
                }
                sda = new SqlDataAdapter(cmd);
                sda.Fill(dataSet);
                sqlCon.Close();

                for (int i = 0; i < dataSet.Rows.Count; i++)
                {
                    if (dataSet.Rows[i]["IntegrationId"].ToString() != null && dataSet.Rows[i]["IntegrationId"].ToString() != "")
                    {
                        newIntegration.IntegrationId = Convert.ToInt32(dataSet.Rows[i]["IntegrationId"]) + 1;
                    }
                }
                //if (button == "ServiceDocument")
                //{
                //    if (umodel.serviceDetails1.ExpAPISPCFDocument != null)
                //    {
                //        var basePathdoc = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + date + "" + umodel.IntegrationId + "\\");
                //        basePathdoc = basePathdoc + "\\" + umodel.serviceDetails1.modifyExpectedAPISpecificationFileName;
                //        FileInfo file = new FileInfo(basePathdoc);
                //        if (file.Exists)//check file exsit or not  
                //        {
                //            file.Delete();
                //            umodel.serviceDetails1.SfileName = string.Empty;
                //            umodel.serviceDetails1.modifyExpectedAPISpecificationFileName = string.Empty;
                //            umodel.serviceDetails1.ExpectedAPISpecificationFiles = null;
                //        }
                //    }
                //}
                if (button == "JDocument")
                {
                    if (umodel.UserJourneyDocument != null || umodel.modifyFileName != null)
                    {

                        var basePathdoc = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + DateTime.Now.ToString("ddMMMyyyy") + "" + newIntegration.IntegrationId + "\\");
                        basePathdoc = basePathdoc + umodel.modifyFileName;
                        FileInfo file = new FileInfo(basePathdoc);
                        try
                        {
                            if (file.Exists)//check file exsit or not  
                            {
                                file.Delete();
                                umodel.fileName = string.Empty;
                                umodel.modifyFileName = string.Empty;
                                umodel.UserJourneyFiles = null;
                                umodel.UserJourneyFiles = string.Empty;
                            }
                        }
                        catch (Exception ex) { }

                    }

                    TempData["Editbydb"] = "NewIntegration";
                    umodel.IntegrationId = 0;
                    ViewBag.SubmitValue = "Save";
                    TempData["PopupEdit"] = "AddMode";
                    HttpContext.Session.SetString("PopupEdit", "AddMode");
                    ViewBag.Number = "0";
                    umodel = submitRepository.GetAllDropDownListData(umodel);
                    return View(umodel);
                }
                if (button == "RDocument")
                {

                    if (umodel.RDConceptNoteDocument != null || umodel.modifyRDConceptNoteFileName != null)
                    {
                        var basePathdoc = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + DateTime.Now.ToString("ddMMMyyyy") + "" + newIntegration.IntegrationId + "\\");
                        basePathdoc = basePathdoc + umodel.modifyRDConceptNoteFileName;
                        FileInfo file = new FileInfo(basePathdoc);
                        if (file.Exists) //check file exsit or not  
                        {
                            file.Delete();
                            umodel.RfileName = string.Empty;
                            umodel.modifyRDConceptNoteFileName = string.Empty;
                            umodel.RDConceptNoteFiles = null;
                            umodel.RDConceptNoteDoc = string.Empty;
                        }
                    }
                    TempData["Editbydb"] = "NewIntegration";
                    umodel.IntegrationId = 0;
                    ViewBag.SubmitValue = "Save";
                    TempData["PopupEdit"] = "AddMode";
                    HttpContext.Session.SetString("PopupEdit", "AddMode");
                    ViewBag.Number = "0";
                    umodel = submitRepository.GetAllDropDownListData(umodel);
                    return View(umodel);
                }
                int ServiceID = 0;

                if (button.Contains("Edit"))
                {
                    String[] strlist = button.Split("+");
                    ServiceID = Convert.ToInt32(strlist[1].ToString());
                    button = strlist[0].ToString();
                    umodel = submitRepository.GetAllDropDownListData(umodel);
                    RemoveListData(umodel);


                }


                if (button == "Add")
                {
                    umodel = submitRepository.GetAllDropDownListData(umodel);
                    //if (umodel.IntegrationId == 0 &&  umodel.serviceDetails1.ServiceID == 1)
                    //{
                    //    TempData["PopupEdit"] = "AddMode";
                    //    HttpContext.Session.SetString("PopupEdit", "AddMode");
                    //}
                    //-------------------------------------------UpdateList-- UpdateRow --------------------------
                    if (HttpContext.Session.GetString("PopupEdit") != null && HttpContext.Session.GetString("PopupEdit") == "EditMode")
                    {
                        int TemserviceId = Convert.ToInt32(HttpContext.Session.GetString("ServiceID"));


                        foreach (var item in umodel.serviceDetails)
                        {
                            if (TemserviceId == item.ServiceID)
                            {
                                item.InternalServiceName = umodel.serviceDetails1.InternalServiceName;
                                item.ServiceID = umodel.serviceDetails1.ServiceID;
                                item.Purpose = umodel.serviceDetails1.Purpose;
                                item.ExternalServiceName = umodel.serviceDetails1.ExternalServiceName;
                                item.ProducerApplication = umodel.serviceDetails1.ProducerApplication;
                                //item.IsAPIGW = umodel.serviceDetails1.IsAPIGW;
                                item.Transformation = umodel.serviceDetails1.Transformation;
                                item.Volume = umodel.serviceDetails1.Volume;
                                item.Existing_New_Id = umodel.serviceDetails1.Existing_New_Id;
                                item.Rest_SOAP_Id = umodel.serviceDetails1.Rest_SOAP_Id;
                                item.ServiceType_Id = umodel.serviceDetails1.ServiceType_Id;
                                item.APIType_Id = umodel.serviceDetails1.APIType_Id;
                                item.APICategory_Id = umodel.serviceDetails1.APICategory_Id;
                                item.RistClassify = umodel.serviceDetails1.RistClassify;
                                item.TotalScore = umodel.serviceDetails1.TotalScore;
                                item.DomainName_Id = umodel.serviceDetails1.DomainName_Id;
                                item.ConsumerDC = umodel.serviceDetails1.ConsumerDC;
                                item.ProducerDC = umodel.serviceDetails1.ProducerDC;
                                item.Platform = umodel.serviceDetails1.Platform;
                                //item.SfileName = umodel.serviceDetails1.SfileName;
                                //item.ExpAPISPCFDocument= umodel.serviceDetails1.ExpAPISPCFDocument;
                                //if (umodel.serviceDetails1.ExpAPISPCFDocument != null)
                                //{
                                //    umodel.serviceDetails1.SfileName = umodel.serviceDetails1.ExpAPISPCFDocument.FileName;

                                //    string extension = Path.GetExtension(umodel.serviceDetails1.SfileName);
                                //   // foreach (var file in umodel.serviceDetails1.ExpAPISPCFDocument)
                                //    //{
                                //        var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + DateTime.Now.ToString("ddMMMyyyy") + "" + newIntegration.IntegrationId + "\\");
                                //        bool basePathExists = System.IO.Directory.Exists(basePath);
                                //        if (!basePathExists) Directory.CreateDirectory(basePath);
                                //        umodel.serviceDetails1.modifyExpectedAPISpecificationFileName = "ExpAPISPCFDoc" + "_API_" + DateTime.Now.ToString("ddMMMyyyy") + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + "_" + newIntegration.IntegrationId + extension;


                                //        var filePath = Path.Combine(basePath, umodel.serviceDetails1.modifyExpectedAPISpecificationFileName);
                                //        using (var stream = new FileStream(filePath, FileMode.Create))
                                //        {
                                //        umodel.serviceDetails1.ExpAPISPCFDocument.CopyTo(stream);
                                //        }
                                //    //}
                                //}
                                SetFilePath(umodel);
                                ////item.SfileName = umodel.serviceDetails1.SfileName;
                                //item.modifyExpectedAPISpecificationFileName = umodel.serviceDetails1.modifyExpectedAPISpecificationFileName;

                                //item.modifyExpectedAPISpecificationFileName = umodel.serviceDetails1.modifyExpectedAPISpecificationFileName;
                                //item.AddServiceStatus = umodel.serviceDetails1.AddServiceStatus;
                            }
                        }
                        if (ViewBag.SubmitValue == "Save" || umodel.IntegrationId == 0)
                        {


                            ViewBag.SubmitValue = "Save";
                            TempData["Editbydb"] = "NewIntegration";
                            umodel = submitRepository.GetAllDropDownListData(umodel);
                            //TempData["PopupEdit"] = "AddMode";
                            HttpContext.Session.SetString("PopupEdit", "AddMode");
                            //umodel.serviceDetails1.ExpAPISPCFDocument = null;
                            // umodel.serviceDetails1.SfileName = null;
                            //umodel.serviceDetails1.modifyExpectedAPISpecificationFileName = null;
                            //if(umodel.modifyFileName!=null)
                            //   umodel.fileName = umodel.modifyFileName;
                            //if(umodel.modifyRDConceptNoteFileName!=null)
                            //    umodel.RfileName = umodel.modifyRDConceptNoteFileName;
                            ModelState.Clear();
                            if (umodel.modifyFileName != "" && umodel.modifyFileName != null)
                            {
                                umodel.fileName = umodel.modifyFileName;
                            }
                            if (umodel.modifyRDConceptNoteFileName != "" && umodel.modifyRDConceptNoteFileName != null)
                            {
                                umodel.RfileName = umodel.modifyRDConceptNoteFileName;
                            }
                            umodel.RDConceptNoteFiles = umodel.RDConceptNoteDoc;
                            return View(umodel);
                        }
                        else
                        {
                            if (umodel.workflowstatus == 12)
                            {
                                ViewBag.SubmitValue = "Draft";

                            }
                            else
                            {
                                ViewBag.SubmitValue = "Update";
                            }

                            umodel = submitRepository.GetNewIntegrationByIdEditPopwhendbcreate(umodel.IntegrationId, umodel);
                            umodel.integrationLists = new List<IntegrationList>();
                            umodel.integrationLists.Add(new IntegrationList()
                            {
                                integrationId = umodel.IntegrationId,
                                workflowstatus = umodel.workflowstatus,
                                AssignTo = umodel.AssignTo,
                                AssignFrom = umodel.AssignFrom,

                            });
                            TempData["Editbydb"] = "NewIntegration?integrationId=" + umodel.IntegrationId;
                            //var a=umodel.serviceDetails.Where()

                            TempData["PopupEdit"] = "UpdateMode";
                            HttpContext.Session.SetString("PopupEdit", "UpdateMode");
                            ViewBag.Number = "1";
                            ModelState.Clear();
                            if (umodel.modifyFileName != "" && umodel.modifyFileName != null)
                            {
                                //String[] strlist = umodel.modifyFileName.Split("_");
                                //string FileNameM = strlist[1].ToString();
                                //umodel.fileName = FileNameM;
                                umodel.fileName = umodel.modifyFileName;
                            }
                            if (umodel.modifyRDConceptNoteFileName != "" && umodel.modifyRDConceptNoteFileName != null)
                            {
                                //String[] strlist = umodel.modifyFileName.Split("_");
                                //string FileNameM = strlist[1].ToString();
                                //umodel.fileName = FileNameM;
                                umodel.RfileName = umodel.modifyRDConceptNoteFileName;
                            }
                            umodel.RDConceptNoteFiles = umodel.RDConceptNoteDoc;
                            return View(umodel);


                        }

                    }
                    else
                    {
                        //-------------------------------------------AddList-New Row---------------------------
                        ViewBag.SubmitValue = "Save";
                        //umodel = submitRepository.GetAllDropDownListData(umodel);
                        if (umodel.serviceDetails == null)
                        {
                            //if (umodel.serviceDetails1.ExpAPISPCFDocument != null)
                            //{
                            //    umodel.serviceDetails1.SfileName = ExpAPISPCFDocumentFile.FileName;
                            //    string extension = Path.GetExtension(umodel.serviceDetails1.SfileName);
                            //   // foreach (var file in umodel.serviceDetails1.ExpAPISPCFDocument)
                            //    //{
                            //        var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + DateTime.Now.ToString("ddMMMyyyy") + "" + newIntegration.IntegrationId + "\\");
                            //        bool basePathExists = System.IO.Directory.Exists(basePath);
                            //        if (!basePathExists) Directory.CreateDirectory(basePath);
                            //        umodel.serviceDetails1.modifyExpectedAPISpecificationFileName = "ExpAPISPCFDoc" + "_API_" + DateTime.Now.ToString("ddMMMyyyy") + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + "_" + newIntegration.IntegrationId + extension;
                            //        var filePath = Path.Combine(basePath, umodel.serviceDetails1.modifyExpectedAPISpecificationFileName);
                            //        using (var stream = new FileStream(filePath, FileMode.Create))
                            //        {
                            //             ExpAPISPCFDocumentFile.CopyTo(stream);
                            //        }
                            //    //}
                            //}
                            //else if (umodel.serviceDetails1.modifyExpectedAPISpecificationFileName != "" && umodel.serviceDetails1.modifyExpectedAPISpecificationFileName != null)
                            //{
                            //    //String[] strlist = umodel.serviceDetails1.modifyExpectedAPISpecificationFileName.Split("_");
                            //    //string FileNameM = strlist[1].ToString();
                            //    //umodel.serviceDetails1.SfileName = FileNameM;
                            //    //umodel.serviceDetails1.SfileName = umodel.serviceDetails1.modifyExpectedAPISpecificationFileName;
                            //}
                            umodel.serviceDetails = new List<ServiceDetails>();
                            umodel.serviceDetails.Add(new ServiceDetails
                            {
                                //ServiceID = 1,
                                InternalServiceName = umodel.serviceDetails1.InternalServiceName,// string.Empty,
                                Purpose = umodel.serviceDetails1.Purpose,  //string.Empty,

                                // Existing_New = umodel.serviceDetails1.Existing_New,
                                ExternalServiceName = umodel.serviceDetails1.ExternalServiceName,// string.Empty,
                                ProducerApplication = umodel.serviceDetails1.ProducerApplication,// string.Empty,
                                                                                                 // IsAPIGW = umodel.serviceDetails1.IsAPIGW,
                                                                                                 // Rest_SOAP = string.Empty,
                                Transformation = umodel.serviceDetails1.Transformation,
                                Volume = umodel.serviceDetails1.Volume,
                                Existing_New_Id = umodel.serviceDetails1.Existing_New_Id,
                                Rest_SOAP_Id = umodel.serviceDetails1.Rest_SOAP_Id,
                                ServiceType_Id = umodel.serviceDetails1.ServiceType_Id,
                                APIType_Id = umodel.serviceDetails1.APIType_Id,
                                APICategory_Id = umodel.serviceDetails1.APICategory_Id,
                                //APIRiskScore_Id = umodel.serviceDetails1.APIRiskScore_Id,
                                //PartnerRiskScore_Id = umodel.serviceDetails1.PartnerRiskScore_Id,
                                RistClassify = umodel.serviceDetails1.RistClassify,
                                TotalScore = umodel.serviceDetails1.TotalScore,
                                DomainName_Id = umodel.serviceDetails1.DomainName_Id,
                                ConsumerDC = umodel.serviceDetails1.ConsumerDC,
                                ProducerDC = umodel.serviceDetails1.ProducerDC,
                                Platform = umodel.serviceDetails1.Platform,
                                SfileName = umodel.serviceDetails1.SfileName,
                                modifyExpectedAPISpecificationFileName = umodel.serviceDetails1.modifyExpectedAPISpecificationFileName,
                                AddServiceStatus = umodel.serviceDetails1.AddServiceStatus,

                            });
                        }
                        else
                        {
                            RemoveListData(umodel);
                            //if (umodel.serviceDetails1.ExpAPISPCFDocument != null)
                            //{
                            //    umodel.serviceDetails1.SfileName = ExpAPISPCFDocumentFile.FileName;
                            //    string extension = Path.GetExtension(umodel.serviceDetails1.SfileName);
                            //   // foreach (var file in umodel.serviceDetails1.ExpAPISPCFDocument)
                            //   // {
                            //        var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + DateTime.Now.ToString("ddMMMyyyy") + "" + newIntegration.IntegrationId + "\\");
                            //        bool basePathExists = System.IO.Directory.Exists(basePath);
                            //        if (!basePathExists) Directory.CreateDirectory(basePath);
                            //        umodel.serviceDetails1.modifyExpectedAPISpecificationFileName = "ExpAPISPCFDoc" + "_API_" + DateTime.Now.ToString("ddMMMyyyy") + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + "_" + newIntegration.IntegrationId + extension;
                            //        var filePath = Path.Combine(basePath, umodel.serviceDetails1.modifyExpectedAPISpecificationFileName);
                            //        using (var stream = new FileStream(filePath, FileMode.Create))
                            //        {
                            //        ExpAPISPCFDocumentFile.CopyTo(stream);
                            //    }
                            //    //}
                            //}
                            //else if (umodel.serviceDetails1.modifyExpectedAPISpecificationFileName != "" && umodel.serviceDetails1.modifyExpectedAPISpecificationFileName != null)
                            //{
                            //    //String[] strlist = umodel.serviceDetails1.modifyExpectedAPISpecificationFileName.Split("_");
                            //    //string FileNameM = strlist[1].ToString();
                            //    //umodel.serviceDetails1.SfileName = FileNameM;
                            //    umodel.serviceDetails1.SfileName = umodel.serviceDetails1.modifyExpectedAPISpecificationFileName;
                            //}
                            umodel.serviceDetails.Add(new ServiceDetails
                            {

                                //ServiceID = ListCount+1,
                                InternalServiceName = umodel.serviceDetails1.InternalServiceName,// string.Empty,
                                Purpose = umodel.serviceDetails1.Purpose,  //string.Empty,
                                                                           //Existing_New = umodel.serviceDetails1.Existing_New,

                                ExternalServiceName = umodel.serviceDetails1.ExternalServiceName,// string.Empty,
                                ProducerApplication = umodel.serviceDetails1.ProducerApplication,// string.Empty,
                                //IsAPIGW = umodel.serviceDetails1.IsAPIGW,
                                //Rest_SOAP = string.Empty,
                                Transformation = umodel.serviceDetails1.Transformation,
                                Volume = umodel.serviceDetails1.Volume,
                                Existing_New_Id = umodel.serviceDetails1.Existing_New_Id,
                                Rest_SOAP_Id = umodel.serviceDetails1.Rest_SOAP_Id,
                                ServiceType_Id = umodel.serviceDetails1.ServiceType_Id,
                                APIType_Id = umodel.serviceDetails1.APIType_Id,
                                APICategory_Id = umodel.serviceDetails1.APICategory_Id,
                                // APIRiskScore_Id = umodel.serviceDetails1.APIRiskScore_Id,
                                // PartnerRiskScore_Id = umodel.serviceDetails1.PartnerRiskScore_Id,
                                RistClassify = umodel.serviceDetails1.RistClassify,
                                TotalScore = umodel.serviceDetails1.TotalScore,
                                DomainName_Id = umodel.serviceDetails1.DomainName_Id,
                                ConsumerDC = umodel.serviceDetails1.ConsumerDC,
                                ProducerDC = umodel.serviceDetails1.ProducerDC,
                                Platform = umodel.serviceDetails1.Platform,
                                SfileName = umodel.serviceDetails1.SfileName,
                                modifyExpectedAPISpecificationFileName = umodel.serviceDetails1.modifyExpectedAPISpecificationFileName,
                                AddServiceStatus = "NewServiceAdd",
                                //AddServiceStatus=umodel.serviceDetails1.AddServiceStatus,
                            });

                        }

                        if (umodel.UserJourneyDocument != null)
                        {
                            umodel.fileName = umodel.UserJourneyDocument.FileName;
                            HttpContext.Session.SetString("FileName", umodel.fileName);
                            string extension = Path.GetExtension(umodel.fileName);
                            // foreach (var file in umodel.UserJourneyDocument)
                            // {
                            var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + DateTime.Now.ToString("ddMMMyyyy") + "" + newIntegration.IntegrationId + "\\");
                            bool basePathExists = System.IO.Directory.Exists(basePath);
                            if (!basePathExists) Directory.CreateDirectory(basePath);
                            umodel.modifyFileName = "JourneyDoc" + "_API_" + DateTime.Now.ToString("ddMMMyyyy") + "_" + newIntegration.IntegrationId + extension;

                            var filePath = Path.Combine(basePath, umodel.modifyFileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                umodel.UserJourneyDocument.CopyTo(stream);
                            }
                            UploadFile(umodel.modifyFileName, umodel.UserJourneyDocument); // Uncomment on prod
                            // }
                        }
                        else if (umodel.modifyFileName != "" && umodel.modifyFileName != null)
                        {
                            //String[] strlist = umodel.modifyFileName.Split("_");
                            //string FileNameM = strlist[1].ToString();
                            //umodel.fileName = FileNameM;
                            umodel.fileName = umodel.modifyFileName;
                        }
                        if (umodel.RDConceptNoteDocument != null)
                        {
                            umodel.RfileName = umodel.RDConceptNoteDocument.FileName;
                            HttpContext.Session.SetString("RFileName", umodel.RfileName);
                            string extension = Path.GetExtension(umodel.RfileName);
                            // foreach (var file in umodel.RDConceptNoteDocument)
                            // {
                            var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + DateTime.Now.ToString("ddMMMyyyy") + "" + newIntegration.IntegrationId + "\\");
                            bool basePathExists = System.IO.Directory.Exists(basePath);
                            if (!basePathExists) Directory.CreateDirectory(basePath);
                            umodel.modifyRDConceptNoteFileName = "RDDoc" + "_API_" + DateTime.Now.ToString("ddMMMyyyy") + "_" + newIntegration.IntegrationId + extension;

                            var filePath = Path.Combine(basePath, umodel.modifyRDConceptNoteFileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                umodel.RDConceptNoteDocument.CopyTo(stream);
                            }
                            UploadFile(umodel.modifyRDConceptNoteFileName, umodel.RDConceptNoteDocument); // Uncomment on prod
                            // }
                        }
                        else if (umodel.modifyRDConceptNoteFileName != "" && umodel.modifyRDConceptNoteFileName != null)
                        {
                            //String[] strlist = umodel.modifyRDConceptNoteFileName.Split("_");
                            //string FileNameM = strlist[1].ToString();
                            //umodel.RfileName = FileNameM;
                            umodel.RfileName = umodel.modifyRDConceptNoteFileName;
                        }

                        //umodel.serviceDetails1.ExpAPISPCFDocument = null;
                        umodel.serviceDetails1.SfileName = null;
                        //umodel.serviceDetails1.modifyExpectedAPISpecificationFileName = null;
                        if (ViewBag.SubmitValue == "Save" || umodel.IntegrationId == 0)
                        {
                            ViewBag.SubmitValue = "Save";
                            TempData["Editbydb"] = "NewIntegration";

                            if (umodel.workflowstatus == 12)
                            {
                                ViewBag.SubmitValue = "Draft";
                            }
                        }
                        else
                        {
                            ViewBag.SubmitValue = "Update";
                            if (umodel.workflowstatus == 12)
                            {
                                ViewBag.SubmitValue = "Draft";
                            }
                            TempData["Editbydb"] = "NewIntegration?integrationId=" + umodel.IntegrationId;

                            //int UIntegrationId = Convert.ToInt32(HttpContext.Session.GetString("UIntegrationId"));
                            // umodel = submitRepository.GetNewIntegrationById(UIntegrationId);
                        }
                        ViewBag.Number = "1";
                        ModelState.Clear();
                        TempData["PopupEdit"] = "AddMode";
                        HttpContext.Session.SetString("PopupEdit", "AddMode");



                        return View(umodel);

                    }

                }
                else if (button == "Edit")
                {
                    var SelectedServiceDetails = umodel.serviceDetails.Where(x => x.ServiceID == ServiceID).ToList();

                    umodel.serviceDetails1 = new ServiceDetails();
                    foreach (var item in SelectedServiceDetails)
                    {
                        umodel.serviceDetails1.InternalServiceName = item.InternalServiceName;
                        umodel.serviceDetails1.ServiceID = item.ServiceID;
                        umodel.serviceDetails1.Purpose = item.Purpose;
                        umodel.serviceDetails1.ExternalServiceName = item.ExternalServiceName;
                        umodel.serviceDetails1.ProducerApplication = item.ProducerApplication;
                        //umodel.serviceDetails1.IsAPIGW = item.IsAPIGW;
                        umodel.serviceDetails1.Transformation = item.Transformation;
                        umodel.serviceDetails1.Volume = item.Volume;
                        umodel.serviceDetails1.Existing_New_Id = item.Existing_New_Id;
                        umodel.serviceDetails1.Rest_SOAP_Id = item.Rest_SOAP_Id;
                        umodel.serviceDetails1.ServiceType_Id = item.ServiceType_Id;
                        umodel.serviceDetails1.APIType_Id = item.APIType_Id;
                        umodel.serviceDetails1.APICategory_Id = item.APICategory_Id;
                        //umodel.serviceDetails1.APIRiskScore_Id = item.APIRiskScore_Id;
                        //umodel.serviceDetails1.PartnerRiskScore_Id = item.PartnerRiskScore_Id;
                        umodel.serviceDetails1.RistClassify = item.RistClassify;
                        umodel.serviceDetails1.TotalScore = item.TotalScore;
                        umodel.serviceDetails1.DomainName_Id = item.DomainName_Id;
                        umodel.serviceDetails1.ConsumerDC = item.ConsumerDC;
                        umodel.serviceDetails1.ProducerDC = item.ProducerDC;
                        umodel.serviceDetails1.Platform = item.Platform;
                        umodel.serviceDetails1.SfileName = item.SfileName;
                        umodel.serviceDetails1.modifyExpectedAPISpecificationFileName = item.modifyExpectedAPISpecificationFileName;
                        umodel.serviceDetails1.ExpectedAPISpecificationFiles = item.ExpectedAPISpecificationFiles;
                        //umodel.serviceDetails1.AddServiceStatus= item.AddServiceStatus;
                    }
                    //SetDropdowndata

                    //var a=umodel.serviceDetails.Where()

                    if (umodel.UserJourneyDocument != null)
                    {
                        umodel.fileName = umodel.UserJourneyDocument.FileName;
                        string extension = Path.GetExtension(umodel.fileName);
                        // foreach (var file in umodel.UserJourneyDocument)
                        // {
                        var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + date + "" + (umodel.ParentIntegrationId == null || umodel.ParentIntegrationId == 0 ? umodel.IntegrationId : umodel.ParentIntegrationId) + "\\");
                        bool basePathExists = System.IO.Directory.Exists(basePath);
                        if (!basePathExists) Directory.CreateDirectory(basePath);
                        umodel.modifyFileName = "JourneyDoc" + "_API_" + date + "_" + umodel.IntegrationId + extension;

                        var filePath = Path.Combine(basePath, umodel.modifyFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            umodel.UserJourneyDocument.CopyTo(stream);
                        }
                        // }
                    }
                    else if (umodel.modifyFileName != "" && umodel.modifyFileName != null)
                    {
                        //String[] strlist = umodel.modifyFileName.Split("_");
                        //string FileNameM = strlist[1].ToString();
                        //umodel.fileName = FileNameM;
                        umodel.fileName = HttpContext.Session.GetString("FileName");
                    }

                    if (umodel.RDConceptNoteDocument != null)
                    {
                        umodel.RfileName = umodel.RDConceptNoteDocument.FileName;
                        string extension = Path.GetExtension(umodel.RfileName);
                        //foreach (var file in umodel.RDConceptNoteDocument)
                        // {
                        var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + date + "" + (umodel.ParentIntegrationId == null || umodel.ParentIntegrationId == 0 ? umodel.IntegrationId : umodel.ParentIntegrationId) + "\\");
                        bool basePathExists = System.IO.Directory.Exists(basePath);
                        if (!basePathExists) Directory.CreateDirectory(basePath);
                        umodel.modifyRDConceptNoteFileName = "RDDoc" + "_API_" + date + "_" + umodel.IntegrationId + extension;
                        umodel.RfileName = umodel.modifyRDConceptNoteFileName;
                        var filePath = Path.Combine(basePath, umodel.modifyRDConceptNoteFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            umodel.RDConceptNoteDocument.CopyTo(stream);
                        }
                        //}
                    }
                    else if (umodel.modifyRDConceptNoteFileName != "" && umodel.modifyRDConceptNoteFileName != null)
                    {
                        //String[] strlist = umodel.modifyRDConceptNoteFileName.Split("_");
                        //string FileNameM = strlist[1].ToString();
                        //umodel.RfileName = FileNameM;
                        umodel.RfileName = HttpContext.Session.GetString("RFileName");
                    }
                    //if (umodel.serviceDetails1.ExpAPISPCFDocument != null)
                    //{
                    //    umodel.serviceDetails1.SfileName = umodel.serviceDetails1.ExpAPISPCFDocument.FileName;
                    //    string extension = Path.GetExtension(umodel.serviceDetails1.SfileName);
                    //    //foreach (var file in umodel.serviceDetails1.ExpAPISPCFDocument)
                    //   // {
                    //        var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + date + "" + (umodel.ParentIntegrationId == null || umodel.ParentIntegrationId == 0 ? umodel.IntegrationId : umodel.ParentIntegrationId) + "\\");
                    //        bool basePathExists = System.IO.Directory.Exists(basePath);
                    //        if (!basePathExists) Directory.CreateDirectory(basePath);
                    //        umodel.serviceDetails1.modifyExpectedAPISpecificationFileName = "ExpAPISPCFDoc" + "_API_" + date + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + "_" + umodel.IntegrationId + extension;
                    //        var filePath = Path.Combine(basePath, umodel.serviceDetails1.modifyExpectedAPISpecificationFileName);
                    //        using (var stream = new FileStream(filePath, FileMode.Create))
                    //        {
                    //        umodel.serviceDetails1.ExpAPISPCFDocument.CopyTo(stream);
                    //        }
                    //   // }
                    //}
                    //else if (umodel.serviceDetails1.modifyExpectedAPISpecificationFileName != "" && umodel.serviceDetails1.modifyExpectedAPISpecificationFileName != null)
                    //{
                    //    //String[] strlist = umodel.serviceDetails1.modifyExpectedAPISpecificationFileName.Split("_");
                    //    //string FileNameM = strlist[1].ToString();
                    //    //umodel.serviceDetails1.SfileName = FileNameM
                    //    //umodel.serviceDetails1.SfileName = umodel.serviceDetails1.SfileName;
                    //}

                    if (ViewBag.SubmitValue == "Save" || umodel.IntegrationId == 0)
                    {
                        ViewBag.SubmitValue = "Save";
                        TempData["Editbydb"] = "NewIntegration";
                    }
                    else
                    {
                        TempData["Editbydb"] = "NewIntegration?integrationId=" + umodel.IntegrationId;
                        if (umodel.workflowstatus == 12)
                        {
                            ViewBag.SubmitValue = "Draft";
                        }
                        else
                        {
                            ViewBag.SubmitValue = "Update";
                        }
                        //umodel.workflowstatus = umodel.workflowstatus;
                        //umodel.IntegrationId = umodel.IntegrationId;
                        //umodel.AssignTo = umodel.AssignTo;
                        //umodel.AssignFrom = umodel.AssignFrom;

                        umodel.integrationLists = new List<IntegrationList>();
                        umodel.integrationLists.Add(new IntegrationList()
                        {
                            integrationId = umodel.IntegrationId,
                            workflowstatus = umodel.workflowstatus,
                            AssignTo = umodel.AssignTo,
                            AssignFrom = umodel.AssignFrom,

                        });
                        SetFilePath(umodel);

                        // int UIntegrationId = Convert.ToInt32(HttpContext.Session.GetString("UIntegrationId"));
                        //umodel = submitRepository.GetNewIntegrationById(umodel.IntegrationId);
                    }
                    ModelState.Clear();
                    // ViewBag.SubmitValue = "Save";
                    TempData["PopupEdit"] = "EditMode";
                    ViewBag.Number = "1";
                    HttpContext.Session.SetString("PopupEdit", "EditMode");
                    HttpContext.Session.SetString("ServiceID", Convert.ToString(ServiceID));
                    return View(umodel);
                }
                ///---------------------------------------------------Save Data In data base ----------------------------------
                else if (button == "SUBMIT")
                {

                    if (umodel.workflowstatus != 12)
                    {
                        TempData["Editbydb"] = "NewIntegration";
                        // if (!ModelState.IsValid || umodel.serviceDetails == null)


                        // bool IsExist = false;//submitRepository.IsConsumerAppValidation(umodel.ProjectId);
                        // if (IsExist != true)

                        bool IsvalidConsumerApp = submitRepository.IsConsumerAppValidation(umodel.ConsumerApplication);
                        if (IsvalidConsumerApp != true)
                        {

                            if (umodel.UserJourneyDocument != null)
                            {
                                umodel.fileName = umodel.UserJourneyDocument.FileName;
                                string extension = Path.GetExtension(umodel.fileName);
                                //foreach (var file in umodel.UserJourneyDocument)
                                // {
                                var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + DateTime.Now.ToString("ddMMMyyyy") + "" + newIntegration.IntegrationId + "\\");
                                bool basePathExists = System.IO.Directory.Exists(basePath);
                                if (!basePathExists) Directory.CreateDirectory(basePath);
                                umodel.modifyFileName = "JourneyDoc" + "_API_" + DateTime.Now.ToString("ddMMMyyyy") + "_" + newIntegration.IntegrationId + extension;

                                var filePath = Path.Combine(basePath, umodel.modifyFileName);
                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    umodel.UserJourneyDocument.CopyTo(stream);
                                }
                                // }
                            }
                            else if (umodel.modifyFileName != "" && umodel.modifyFileName != null)
                            {
                                umodel.fileName = umodel.modifyFileName;
                            }
                            if (umodel.RDConceptNoteDocument != null)
                            {
                                umodel.RfileName = umodel.RDConceptNoteDocument.FileName;
                                string extension = Path.GetExtension(umodel.RfileName);
                                //  foreach (var file in umodel.RDConceptNoteDocument)
                                //{
                                var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + DateTime.Now.ToString("ddMMMyyyy") + "" + newIntegration.IntegrationId + "\\");
                                bool basePathExists = System.IO.Directory.Exists(basePath);
                                if (!basePathExists) Directory.CreateDirectory(basePath);
                                umodel.modifyRDConceptNoteFileName = "RDDoc" + "_API_" + DateTime.Now.ToString("ddMMMyyyy") + "_" + newIntegration.IntegrationId + extension;

                                var filePath = Path.Combine(basePath, umodel.modifyRDConceptNoteFileName);
                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    umodel.RDConceptNoteDocument.CopyTo(stream);
                                }
                                //}
                            }
                            else if (umodel.modifyRDConceptNoteFileName != "" && umodel.modifyRDConceptNoteFileName != null)
                            {
                                umodel.RfileName = umodel.modifyRDConceptNoteFileName;
                            }
                            if (umodel.serviceDetails == null || nonZeroserviceIds == "")
                            {
                                ViewBag.SubmitValue = "Save";
                                ViewBag.Number = "1";
                                TempData["SaveFlag"] = true;
                                if (umodel.lstServiceDetails == null)
                                {
                                    TempData["ServicedtMsg"] = "Please add atleast one service detail";
                                }
                                return RedirectToAction("newintegration", umodel);
                            }

                            umodel.SpName = "sp_APIA_NewAPIIntegration";
                            umodel.MethodFlag = "AddNewIntegration";
                            umodel.UserJourneyFiles = umodel.modifyFileName;
                            umodel.UserJourneyFiles = umodel.UserJourneyFiles;
                            umodel.RDConceptNoteFiles = umodel.modifyRDConceptNoteFileName;
                            umodel.UserJourneyFiles = umodel.UserJourneyFiles;
                            umodel.RDConceptNoteFiles = umodel.RDConceptNoteFiles;
                            umodel.UserId = HttpContext.Session.GetString("EmpId");
                            umodel.AssignTo = "BTGUSER";
                            umodel.AssignFrom = HttpContext.Session.GetString("Role");
                            umodel.workflowstatus = 1;
                            umodel = submitRepository.SaveDetails(umodel);
                            umodel.MethodFlag = "AddServiceDetails";
                            //umodel.serviceDetails.Where(x => x.Platform == "internal" || x.Platform == "external" || ).Count() == 1 ? "All" : "single";
                            umodel = submitRepository.SaveServiceDetails(umodel);
                            SendEmail obj = new SendEmail();
                            string LogUserid = HttpContext.Session.GetString("EmpId").ToString();
                            obj.SendMailAlert(umodel, LogUserid, "Created", newIntegration.IntegrationId); // Uncomment on prod

                            if (umodel.IntegrationId > 0)
                            {
                                TempData["Result"] = "Data Submit Successfully";
                                TempData["IsSuccess"] = "True";
                                ModelState.Clear();
                                umodel = new NewIntegration();
                                ViewBag.Number = "0";
                                string UserId = HttpContext.Session.GetString("EmpId");
                                CaptureProductivityDetails(sqlCon, UserId, "New Intergration", "API ADDA", 1, "NewIntegration save ", " Save NewIntegration for EmpCode - " + UserId.ToString().Trim());
                            }
                            else
                            {
                                TempData["Result"] = "Something Went Wrong..Please try again later";
                                TempData["IsSuccess"] = "False";
                            }
                        }
                        else
                        {
                            TempData["Result"] = "Consumer Application is in System add spoc.";
                            TempData["IsSuccess"] = "False";
                            ViewBag.SubmitValue = "Save";
                            ViewBag.Number = "1";
                            return RedirectToAction("newintegration");
                        }

                    }
                    else if (umodel.workflowstatus == 12)
                    {

                        TempData["Editbydb"] = "NewIntegration?integrationId=" + umodel.IntegrationId;

                        if (umodel.UserJourneyDocument != null)
                        {
                            umodel.fileName = umodel.UserJourneyDocument.FileName;
                            HttpContext.Session.SetString("FileName", umodel.fileName);
                            string extension = Path.GetExtension(umodel.fileName);
                            //foreach (var file in umodel.UserJourneyDocument)
                            // {
                            var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + DateTime.Now.ToString("ddMMMyyyy") + "" + newIntegration.IntegrationId + "\\");
                            bool basePathExists = System.IO.Directory.Exists(basePath);
                            if (!basePathExists) Directory.CreateDirectory(basePath);
                            umodel.UserJourneyFiles = "JourneyDoc" + "_API_" + DateTime.Now.ToString("ddMMMyyyy") + "_" + newIntegration.IntegrationId + extension;

                            var filePath = Path.Combine(basePath, umodel.UserJourneyFiles);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                umodel.UserJourneyDocument.CopyTo(stream);
                            }
                            // }
                        }
                        else if (umodel.modifyFileName != "" && umodel.modifyFileName != null)
                        {
                            umodel.UserJourneyFiles = umodel.modifyFileName;
                        }
                        if (umodel.RDConceptNoteDocument != null)
                        {
                            // foreach (var file in umodel.RDConceptNoteDocument)
                            // {
                            umodel.RfileName = umodel.RDConceptNoteDocument.FileName;
                            HttpContext.Session.SetString("RFileName", umodel.RfileName);
                            string extension = Path.GetExtension(umodel.RfileName);
                            var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + DateTime.Now.ToString("ddMMMyyyy") + "" + newIntegration.IntegrationId + "\\");
                            bool basePathExists = System.IO.Directory.Exists(basePath);
                            if (!basePathExists) Directory.CreateDirectory(basePath);
                            umodel.RDConceptNoteFiles = "RDDoc" + "_API_" + DateTime.Now.ToString("ddMMMyyyy") + "_" + newIntegration.IntegrationId + extension;

                            var filePath = Path.Combine(basePath, umodel.RDConceptNoteFiles);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                umodel.RDConceptNoteDocument.CopyTo(stream);
                            }
                            //}
                        }
                        else if (umodel.modifyRDConceptNoteFileName != "" && umodel.modifyRDConceptNoteFileName != null)
                        {
                            umodel.RDConceptNoteFiles = umodel.modifyRDConceptNoteFileName;
                        }
                        if (umodel.serviceDetails == null || nonZeroserviceIds == "")
                        {
                            ViewBag.SubmitValue = "Save";
                            ViewBag.Number = "1";
                            TempData["SaveFlag"] = true;
                            if (umodel.lstServiceDetails == null)
                            {
                                TempData["ServicedtMsg"] = "Please add atleast one service detail";
                            }
                            return RedirectToAction("newintegration", umodel);
                        }
                        umodel.SpName = "sp_APIA_NewAPIIntegration";
                        umodel.MethodFlag = "UpdateIntegrationDetails";
                        umodel.UserId = HttpContext.Session.GetString("EmpId");

                        if (umodel.UserJourneyFiles != null)
                            umodel.UserJourneyFiles = umodel.UserJourneyFiles.TrimEnd(',');


                        if (HttpContext.Session.GetString("Role") == "USER")
                        {
                            umodel.workflowstatus = 1;
                            umodel.AssignTo = "BTGUSER";
                            umodel.AssignFrom = "USER";

                        }

                        umodel = submitRepository.UpdateNewIntegration(umodel);
                        umodel.MethodFlag = "DarftUpdateServiceDetails";
                        umodel = submitRepository.DarftUpdateServiceDetails(umodel);

                        if (umodel.IntegrationId > 0)
                        {

                            ModelState.Clear();
                            TempData["Result"] = "Data Submit Successfully";
                            TempData["IsSuccess"] = "True";
                            CaptureProductivityDetails(sqlCon, umodel.UserId, "New Intergration", "API ADDA", 1, "NewIntegration  update ", " Update NewIntegration for EmpCode - " + umodel.UserId.ToString().Trim());
                            umodel = new NewIntegration();
                            ViewBag.Number = "0";
                        }
                        else
                        {
                            TempData["Result"] = "Something Went Wrong..Please try again later";
                            TempData["IsSuccess"] = "False";
                        }

                    }

                }

                ///---------------------------------------------------Update Data In data base ----------------------------------
                else if (button == "Update")
                {

                    TempData["Editbydb"] = "NewIntegration?integrationId=" + umodel.IntegrationId;

                    if (umodel.UserJourneyDocument != null)
                    {
                        //-----------------------when new file upioad ,first old file detete to folder---------------start--------------
                        DeletePriviousdocinupdate(umodel);
                    }
                    //var basePathdoc = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\Files");
                    //basePathdoc = basePathdoc + "\\" + umodel.UserJourneyFiles;
                    //FileInfo file = new FileInfo(basePathdoc);
                    //if (file.Exists)//check file exsit or not  
                    //{
                    //    file.Delete();
                    //    umodel.fileName = string.Empty;
                    //    umodel.modifyFileName = string.Empty;
                    //    umodel.UserJourneyFiles = null;

                    //}
                    //-----------------------when new file Add--------------


                    if (umodel.UserJourneyDocument != null)
                    {
                        //  foreach (var fileJD in umodel.UserJourneyDocument)
                        // {
                        umodel.fileName = umodel.UserJourneyDocument.FileName;
                        string extension = Path.GetExtension(umodel.fileName);
                        var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + date + "" + (umodel.ParentIntegrationId == null || umodel.ParentIntegrationId == 0 ? umodel.IntegrationId : umodel.ParentIntegrationId) + "\\");
                        bool basePathExists = System.IO.Directory.Exists(basePath);
                        if (!basePathExists) Directory.CreateDirectory(basePath);
                        umodel.UserJourneyFiles = "JourneyDoc" + "_API_" + date + "_" + umodel.IntegrationId + extension;
                        var filePath = Path.Combine(basePath, umodel.UserJourneyFiles);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            umodel.UserJourneyDocument.CopyTo(stream);
                        }
                        //}
                    }
                    else if (umodel.modifyFileName != "" && umodel.modifyFileName != null)
                    {
                        umodel.UserJourneyFiles = umodel.modifyFileName;
                    }
                    else if (umodel.SequenceDiagFiles != "" && umodel.SequenceDiagFiles != null)
                    {
                        umodel.modifySequenceDiagFileName = umodel.SequenceDiagFiles;
                    }

                    if (umodel.RDConceptNoteDocument != null)
                    {
                        if (umodel.RDConceptNoteDocument != null)
                        {

                            // foreach (var file in umodel.RDConceptNoteDocument)
                            //{
                            umodel.RfileName = umodel.RDConceptNoteDocument.FileName;
                            string extension = Path.GetExtension(umodel.RfileName);

                            var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + date + "" + (umodel.ParentIntegrationId == null || umodel.ParentIntegrationId == 0 ? umodel.IntegrationId : umodel.ParentIntegrationId) + "\\");
                            bool basePathExists = System.IO.Directory.Exists(basePath);
                            if (!basePathExists) Directory.CreateDirectory(basePath);
                            umodel.RDConceptNoteFiles = "RDDoc" + "_API_" + date + "_" + umodel.IntegrationId + extension;

                            var filePath = Path.Combine(basePath, umodel.RDConceptNoteFiles);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                umodel.RDConceptNoteDocument.CopyTo(stream);
                            }
                            //}
                        }
                        else if (umodel.modifyRDConceptNoteFileName != "" && umodel.modifyRDConceptNoteFileName != null)
                        {
                            umodel.RDConceptNoteFiles = umodel.modifyRDConceptNoteFileName;
                        }
                    }
                    #region if You want add multifile name
                    //    foreach (var fileJD in umodel.UserJourneyDocument)
                    //    {
                    //        // var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\Files\\");
                    //        var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\Files");
                    //        bool basePathExists = System.IO.Directory.Exists(basePath);
                    //        if (!basePathExists) Directory.CreateDirectory(basePath);
                    //        umodel.UserJourneyFiles += Guid.NewGuid().ToString().Substring(0, 6) + "_" + fileJD.FileName + ",";
                    //        var filePath = Path.Combine(basePath, umodel.UserJourneyFiles);
                    //        using (var stream = new FileStream(filePath, FileMode.Create))
                    //        {
                    //            await fileJD.CopyToAsync(stream);
                    //        }
                    //    }
                    //}
                    //else if (umodel.modifyFileName != "" && umodel.modifyFileName != null)
                    //{
                    //    umodel.UserJourneyFiles = umodel.modifyFileName;
                    //}

                    //    if (umodel.modifyFileName != "" && umodel.modifyFileName != null)
                    //    {
                    //        if (HttpContext.Session.GetString("Role") == "USER")
                    //        {
                    //            var basePathdoc = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\Files");
                    //            basePathdoc = basePathdoc + "\\" + umodel.UserJourneyFiles;
                    //            FileInfo file = new FileInfo(basePathdoc);
                    //            if (file.Exists)//check file exsit or not  
                    //            {
                    //                file.Delete();

                    //            }

                    //        }
                    //    }
                    //}
                    //else if (umodel.modifyFileName != "" && umodel.modifyFileName != null)
                    //{
                    //    if (HttpContext.Session.GetString("Role") == "USER")
                    //    {
                    //        var basePathdoc = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\Files");
                    //        basePathdoc = basePathdoc + "\\" + umodel.UserJourneyFiles;
                    //        FileInfo file = new FileInfo(basePathdoc);
                    //        if (file.Exists)//check file exsit or not  
                    //        {
                    //            file.Delete();

                    //        }

                    //    }

                    //    umodel.UserJourneyFiles = umodel.modifyFileName;
                    //}
                    #endregion

                    umodel.SpName = "sp_APIA_NewAPIIntegration";
                    umodel.MethodFlag = "UpdateIntegrationDetails";
                    umodel.UserId = HttpContext.Session.GetString("EmpId");

                    if (umodel.UserJourneyFiles != null)
                        umodel.UserJourneyFiles = umodel.UserJourneyFiles.TrimEnd(',');
                    if (umodel.RDConceptNoteFiles != null)
                        umodel.RDConceptNoteFiles = umodel.RDConceptNoteFiles.TrimEnd(',');


                    if (HttpContext.Session.GetString("Role") == "USER")
                    {
                        var AssignToU = umodel.AssignFrom;
                        var AssignToF = umodel.AssignTo;
                        if (umodel.AssignFrom == "BTGUSER")
                        {
                            //----User Revert to BTG User status 9

                            umodel.workflowstatus = 9;
                        }
                        else if (umodel.AssignFrom == "ITUSER")
                        {
                            //----User Revert to IT USER status 10

                            umodel.workflowstatus = 10;
                        }
                        else if (umodel.AssignFrom == "ITARCHITECH")
                        {
                            //----User Revert to IT Architect 11

                            umodel.workflowstatus = 11;
                        }
                        umodel.AssignTo = AssignToU;
                        umodel.AssignFrom = AssignToF;

                    }
                    umodel = submitRepository.UpdateNewIntegration(umodel);
                    umodel.MethodFlag = "UpdateServiceDetails";
                    umodel = submitRepository.UpdateServiceDetails(umodel);

                    if (umodel.IntegrationId > 0)
                    {
                        ModelState.Clear();
                        TempData["Result"] = "Data Updated Successfully";
                        TempData["IsSuccess"] = "True";
                        CaptureProductivityDetails(sqlCon, umodel.UserId, "New Intergration", "API ADDA", 1, "NewIntegration update ", " Update NewIntegration for EmpCode - " + umodel.UserId.ToString().Trim());
                        umodel = new NewIntegration();
                        ViewBag.Number = "0";
                    }
                    else
                    {
                        TempData["Result"] = "Something Went Wrong..Please try again later";
                        TempData["IsSuccess"] = "False";
                    }


                    //}

                }
                else if (button == "DRAFT")
                {
                    if (umodel.IntegrationId == 0)
                    {
                        if (umodel.serviceDetails != null && umodel.serviceDetails.Count > 0)
                        {
                            umodel.serviceDetails = umodel.serviceDetails.Where(x => x.Purpose != null).ToList();
                        }
                        TempData["Editbydb"] = "NewIntegration";
                        // if (!ModelState.IsValid || umodel.serviceDetails == null)

                        if (umodel.UserJourneyDocument != null)
                        {
                            umodel.fileName = umodel.UserJourneyDocument.FileName;
                            string extension = Path.GetExtension(umodel.fileName);
                            //foreach (var file in umodel.UserJourneyDocument)
                            //{
                            var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + DateTime.Now.ToString("ddMMMyyyy") + "" + newIntegration.IntegrationId + "\\");
                            bool basePathExists = System.IO.Directory.Exists(basePath);
                            if (!basePathExists) Directory.CreateDirectory(basePath);
                            umodel.modifyFileName = "JourneyDoc" + "_API_" + DateTime.Now.ToString("ddMMMyyyy") + "_" + umodel.ProjectId + extension;
                            var filePath = Path.Combine(basePath, umodel.modifyFileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                umodel.UserJourneyDocument.CopyTo(stream);
                            }
                            UploadFile(umodel.modifyFileName, umodel.UserJourneyDocument);
                            //}
                        }
                        else if (umodel.modifyFileName != "" && umodel.modifyFileName != null)
                        {
                            umodel.fileName = umodel.modifyFileName;
                        }
                        if (umodel.RDConceptNoteDocument != null)
                        {
                            umodel.RfileName = umodel.RDConceptNoteDocument.FileName;
                            string extension = Path.GetExtension(umodel.RfileName);
                            //foreach (var file in umodel.RDConceptNoteDocument)
                            // {
                            var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + DateTime.Now.ToString("ddMMMyyyy") + "" + newIntegration.IntegrationId + "\\");
                            bool basePathExists = System.IO.Directory.Exists(basePath);
                            if (!basePathExists) Directory.CreateDirectory(basePath);
                            umodel.modifyRDConceptNoteFileName = "RDDoc" + "_API_" + DateTime.Now.ToString("ddMMMyyyy") + "_" + umodel.ProjectId + extension;

                            var filePath = Path.Combine(basePath, umodel.modifyRDConceptNoteFileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                umodel.RDConceptNoteDocument.CopyTo(stream);
                            }
                            UploadFile(umodel.modifyRDConceptNoteFileName, umodel.RDConceptNoteDocument);
                            // }
                        }
                        else if (umodel.modifyRDConceptNoteFileName != "" && umodel.modifyRDConceptNoteFileName != null)
                        {
                            umodel.RfileName = umodel.modifyRDConceptNoteFileName;
                        }
                        if (umodel.serviceDetails == null || serviceId == "0," || serviceId == "")
                        {
                            ViewBag.SubmitValue = "Save";
                            ViewBag.Number = "1";
                            TempData["SaveFlag"] = true;
                            if (umodel.lstServiceDetails == null)
                            {
                                TempData["ServicedtMsg"] = "Please add atleast one service detail";
                            }
                            return RedirectToAction("newintegration", umodel);
                        }
                        umodel.SpName = "sp_APIA_NewAPIIntegration";
                        umodel.MethodFlag = "DraftAddNewIntegration";
                        umodel.UserJourneyFiles = umodel.modifyFileName;
                        umodel.RDConceptNoteFiles = umodel.modifyRDConceptNoteFileName;
                        umodel.UserId = HttpContext.Session.GetString("EmpId");
                        //umodel.AssignTo = "BTGUSER";
                        umodel.AssignFrom = HttpContext.Session.GetString("Role");
                        umodel.workflowstatus = 12;
                        umodel = submitRepository.DraftSaveDetails(umodel);
                        umodel.MethodFlag = "DraftAddServiceDetails";
                        umodel = submitRepository.DraftSaveServiceDetails(umodel);

                        if (umodel.IntegrationId > 0)
                        {
                            TempData["Result"] = "Data Draft Successfully";
                            TempData["IsSuccess"] = "True";
                            ModelState.Clear();
                            umodel = new NewIntegration();
                            ViewBag.Number = "0";
                            string UserId = HttpContext.Session.GetString("EmpId");
                            CaptureProductivityDetails(sqlCon, UserId, "New Intergration", "API ADDA", 1, "NewIntegration save ", " Daft NewIntegration for EmpCode - " + UserId.ToString().Trim());

                        }
                        else
                        {
                            TempData["Result"] = "Something Went Wrong..Please try again later";
                            TempData["IsSuccess"] = "False";
                        }
                    }
                    else
                    {
                        if (umodel.serviceDetails == null || serviceId == "0," || serviceId == "")
                        {
                            ViewBag.SubmitValue = "Save";
                            ViewBag.Number = "1";
                            TempData["SaveFlag"] = true;
                            if (umodel.lstServiceDetails == null)
                            {
                                TempData["ServicedtMsg"] = "Please add atleast one service detail";
                            }
                            return RedirectToAction("newintegration", umodel);
                        }

                        TempData["Editbydb"] = "NewIntegration?integrationId=" + umodel.IntegrationId;

                        if (umodel.UserJourneyDocument != null)
                        {
                            DeletePriviousdocinupdate(umodel);
                            // foreach (var fileJD in umodel.UserJourneyDocument)
                            //{
                            umodel.fileName = umodel.UserJourneyDocument.FileName;
                            string extension = Path.GetExtension(umodel.fileName);
                            var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + DateTime.Now.ToString("ddMMMyyyy") + "" + umodel.IntegrationId + "\\");
                            bool basePathExists = System.IO.Directory.Exists(basePath);
                            if (!basePathExists) Directory.CreateDirectory(basePath);
                            umodel.UserJourneyFiles = "JourneyDoc" + "_API_" + DateTime.Now.ToString("ddMMMyyyy") + "_" + umodel.ProjectId + extension;

                            var filePath = Path.Combine(basePath, umodel.UserJourneyFiles);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                umodel.UserJourneyDocument.CopyTo(stream);
                            }
                            //}
                        }
                        else if (umodel.modifyFileName != "" && umodel.modifyFileName != null)
                        {
                            if (HttpContext.Session.GetString("Role") == "USER")
                            {
                                umodel.RfileName = umodel.RDConceptNoteDocument.FileName;
                                string extension = Path.GetExtension(umodel.RfileName);
                                var basePathdoc = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + DateTime.Now.ToString("ddMMMyyyy") + "" + umodel.IntegrationId + "\\");
                                basePathdoc = basePathdoc + "\\" + umodel.UserJourneyFiles;
                                FileInfo file = new FileInfo(basePathdoc);
                                if (file.Exists)//check file exsit or not  
                                {
                                    file.Delete();
                                }
                            }
                            umodel.UserJourneyFiles = umodel.modifyFileName;
                        }

                        if (umodel.RDConceptNoteDocument != null)
                        {
                            DeletePriviousdocinupdate(umodel);

                            // foreach (var fileJD in umodel.RDConceptNoteDocument)
                            // {
                            umodel.RfileName = umodel.RDConceptNoteDocument.FileName;
                            string extension = Path.GetExtension(umodel.RfileName);
                            var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + DateTime.Now.ToString("ddMMMyyyy") + "" + umodel.IntegrationId + "\\");
                            bool basePathExists = System.IO.Directory.Exists(basePath);
                            if (!basePathExists) Directory.CreateDirectory(basePath);
                            umodel.RDConceptNoteFiles = "RDDoc" + "_API_" + DateTime.Now.ToString("ddMMMyyyy") + "_" + umodel.ProjectId + extension;
                            var filePath = Path.Combine(basePath, umodel.RDConceptNoteFiles);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                umodel.RDConceptNoteDocument.CopyTo(stream);
                            }
                            //}
                        }
                        else if (umodel.modifyRDConceptNoteFileName != "" && umodel.modifyRDConceptNoteFileName != null)
                        {
                            if (HttpContext.Session.GetString("Role") == "USER")
                            {
                                var basePathdoc = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + DateTime.Now.ToString("ddMMMyyyy") + "" + umodel.IntegrationId + "\\");
                                basePathdoc = basePathdoc + "\\" + umodel.RDConceptNoteFiles;
                                FileInfo file = new FileInfo(basePathdoc);
                                if (file.Exists)//check file exsit or not  
                                {
                                    file.Delete();
                                }
                            }
                            umodel.RDConceptNoteFiles = umodel.modifyRDConceptNoteFileName;
                        }

                        umodel.SpName = "sp_APIA_NewAPIIntegration";
                        umodel.MethodFlag = "UpdateIntegrationDetails";
                        umodel.UserId = HttpContext.Session.GetString("EmpId");

                        //if (umodel.UserJourneyFiles != null)
                        //    umodel.UserJourneyFiles = umodel.UserJourneyFiles.TrimEnd(',');


                        if (HttpContext.Session.GetString("Role") == "USER")
                        {

                            var deletecount = submitRepository.DeleteDraft(IntegrationId.ToString(), serviceId);
                            umodel = submitRepository.UpdateNewIntegration(umodel);
                            umodel.MethodFlag = "UpdateServiceDetails";
                            umodel = submitRepository.UpdateServiceDetails(umodel);

                            if (umodel.IntegrationId > 0)
                            {
                                ModelState.Clear();
                                TempData["Result"] = "Data Draft Successfully";
                                TempData["IsSuccess"] = "True";
                                CaptureProductivityDetails(sqlCon, umodel.UserId, "New Intergration", "API ADDA", 1, "NewIntegration update darft", " Update NewIntegration for EmpCode - " + umodel.UserId.ToString().Trim());
                                umodel = new NewIntegration();
                                ViewBag.Number = "0";
                            }
                            else
                            {
                                TempData["Result"] = "Something Went Wrong..Please try again later";
                                TempData["IsSuccess"] = "False";
                            }

                        }


                        ViewBag.EditPopup = "EditPopup";

                        umodel = submitRepository.GetNewIntegrationDetails(umodel);

                    }
                }

            }
            catch (Exception Ex)
            {

                throw;
            }


            //return View(umodel);
            return RedirectToAction("NewIntegrationList", "Home");
            //return RedirectToAction("newintegration",);
        }

        private void CaptureProductivityDetails(object sqlCon, object userId, string v1, string v2, int v3, string v4, string v5)
        {
            throw new NotImplementedException();
        }

        [HttpPost]

        public IActionResult search(SearchAPI searchModel)
        {
            try
            {
                if (searchModel.lstProducer == null)
                {
                    searchModel.lstProducer = new List<Producer>();
                    searchModel.lstProducer.Add(new Producer() { ProducerId = "FLEXCUDE", ProducerName = "FLEXCUDE", IsCheck = false });
                    searchModel.lstProducer.Add(new Producer() { ProducerId = "DCMS", ProducerName = "DCMS", IsCheck = false });
                    searchModel.lstProducer.Add(new Producer() { ProducerId = "APS", ProducerName = "APS", IsCheck = false });
                    searchModel.lstProducer.Add(new Producer() { ProducerId = "VISION_PLUS", ProducerName = "VISION PLUS", IsCheck = false });
                }
                if (searchModel.lstConsumer == null)
                {
                    searchModel.lstConsumer = new List<Consumer>();
                    searchModel.lstConsumer.Add(new Consumer() { ConsumerId = "NETBANKING", ConsumerName = "NETBANKING", IsCheck = false });
                    searchModel.lstConsumer.Add(new Consumer() { ConsumerId = "BACKBASE", ConsumerName = "BACKBASE", IsCheck = false });
                    searchModel.lstConsumer.Add(new Consumer() { ConsumerId = "APIGW", ConsumerName = "APIGW", IsCheck = false });
                    searchModel.lstConsumer.Add(new Consumer() { ConsumerId = "DAP", ConsumerName = "DAP", IsCheck = false });
                }
                int MiddlewarecheckCount = 0;
                int ServiceprovidercheckCount = 0;
                int ProducercheckCount = 0;
                int ConsumercheckCount = 0;
                //MiddlewarecheckCount = searchModel.lstMiddlewares.Where(x => x.IsCheck == true).Count();
                //ServiceprovidercheckCount = searchModel.lstServiceProviders.Where(x => x.IsCheck == true).Count();
                //ProducercheckCount = searchModel.lstProducer.Where(x => x.IsCheck == true).Count();
                //ConsumercheckCount = searchModel.lstConsumer.Where(x => x.IsCheck == true).Count();
                if (searchModel.lstMiddlewares != null)
                {
                    MiddlewarecheckCount = searchModel.lstMiddlewares.Where(x => x.IsCheck == true).Count();
                }
                if (searchModel.lstServiceProviders != null)
                {
                    ServiceprovidercheckCount = searchModel.lstServiceProviders.Where(x => x.IsCheck == true).Count();
                }
                if (searchModel.lstProducer != null)
                {
                    ProducercheckCount = searchModel.lstProducer.Where(x => x.IsCheck == true).Count();
                }
                if (searchModel.lstConsumer != null)
                {
                    ConsumercheckCount = searchModel.lstConsumer.Where(x => x.IsCheck == true).Count();
                }

                if (MiddlewarecheckCount > 0 || ServiceprovidercheckCount > 0 || ProducercheckCount > 0 || ConsumercheckCount > 0)
                {

                    if (MiddlewarecheckCount > 0)
                    {
                        searchModel.strMiddleware = searchModel.lstMiddlewares.Where(x => x.IsCheck == true)
                            .Select(x => x.middlewareId).Aggregate((a, x) => a + "," + x);
                    }
                    if (ServiceprovidercheckCount > 0)
                    {


                        searchModel.strProvider = searchModel.lstServiceProviders.Where(x => x.IsCheck == true)
                            .Select(x => x.ServiceProviderId).Aggregate((a, x) => a + "," + x);
                    }
                    searchModel.SpName = "sp_APIA_Search_API";
                    searchModel.IdentFlag = "GetFilterData";
                    searchModel.MethodFlag = "GetFilterData";
                }
                else
                {
                    if (searchModel.SearchAPIURL == null)
                    {
                        searchModel = new SearchAPI();
                        searchModel = submitRepository.searchAPIList(searchModel);
                        // TempData["SearchAction"] = "SearchView";
                        return View(searchModel);
                    }

                    searchModel.SpName = "sp_APIA_Search_API";
                    searchModel.IdentFlag = "getAll";
                    searchModel.MethodFlag = "GetAllAPIs";
                }
            }
            catch (Exception ex)
            {

                //throw ex;
                return RedirectToAction("search");
            }
            ModelState.Clear();
            searchModel = submitRepository.searchAPIList(searchModel);
            string UserId = HttpContext.Session.GetString("EmpId");
            CaptureProductivityDetails(sqlCon, UserId, "Search", "API ADDA", 1, "Search ", " Test API In for EmpCode - " + UserId.Trim());
            return View(searchModel);

            // TempData["SearchAction"] = "submitForm";
            // TempData["SearchData"] = JsonConvert.SerializeObject(searchModel);
            // //return View(searchModel);
            //return RedirectToAction("search");

            //return RedirectToAction(nameof(search));
        }


        [HttpPost]
        public IActionResult TestAPI(SearchAPI searchAPI, string button = null)
        {
            ModelState.Clear();
            try
            {
                if (button != null)
                {
                    // searchAPI.ServiceUrl = "https://hbentbpuatap.hdfcbankuat.com:9550" + searchAPI.ServiceUrl;
                    searchAPI.ServiceUrl = searchAPI.ServiceUrl;
                    if (button.ToLower() == "test")
                    {
                        string UserId = HttpContext.Session.GetString("EmpId");
                        CaptureProductivityDetails(sqlCon, UserId, "Test API", "API ADDA", 1, "Test API ", " Test API In for EmpCode - " + UserId.Trim());
                        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
                        if (System.Net.ServicePointManager.SecurityProtocol == (SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls))
                            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        //--------------------------------------old--------------------------------------------
                        //TempData["ActionName"] = "submitForm";
                        //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(searchAPI.ServiceUrl);
                        //request.Method = "POST";
                        //request.ContentType = "application/Json";
                        //ASCIIEncoding encoding = new ASCIIEncoding();
                        //byte[] byte1 = encoding.GetBytes(searchAPI.Raw); //output
                        //request.ContentLength = byte1.Length;
                        //Stream requestStream = request.GetRequestStream();
                        //requestStream.Write(byte1, 0, byte1.Length);
                        //requestStream.Close();
                        //HttpWebResponse webresponse = (HttpWebResponse)request.GetResponse();
                        //Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                        //StreamReader responseStream = new StreamReader(webresponse.GetResponseStream(), enc);
                        //searchAPI.response = responseStream.ReadToEnd();
                        //webresponse.Close();

                        /// -------------------------------------New ----------------------------------------------
                        /// 
                        TempData["ActionName"] = "submitForm";
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(searchAPI.ServiceUrl);
                        request.Method = "POST";
                        request.ContentType = "application/Json";
                        ASCIIEncoding encoding = new ASCIIEncoding();
                        byte[] byte1 = encoding.GetBytes(searchAPI.Raw); //output
                        request.ContentLength = byte1.Length;
                        string[] SoapList = Convert.ToString(searchAPI.Raw).Split('<');
                        string[] SoapList1 = SoapList[1].Split(':');
                        if (SoapList1[0].ToString() == "soapenv")
                        {
                            request.ContentType = "text/xml";
                        }
                        Stream requestStream = request.GetRequestStream();
                        requestStream.Write(byte1, 0, byte1.Length);
                        requestStream.Close();
                        HttpWebResponse webresponse = (HttpWebResponse)request.GetResponse();
                        Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                        StreamReader responseStream = new StreamReader(webresponse.GetResponseStream(), enc);
                        searchAPI.response = responseStream.ReadToEnd();
                        webresponse.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                searchAPI.response = ex.Message;
            }
            //TempData["searchAPITemp"] = searchAPI;
            TempData["TestApidata"] = JsonConvert.SerializeObject(searchAPI);
            //TempData.Keep();
            return View(searchAPI);
            //return RedirectToAction("TestAPI");
        }

        public IActionResult APIDetail(SearchAPI searchAPI, string button = null)
        {
            ModelState.Clear();
            try
            {
                if (button != null)
                {
                    // searchAPI.ServiceUrl = "https://hbentbpuatap.hdfcbankuat.com:9550" + searchAPI.ServiceUrl;
                    searchAPI.ServiceUrl = searchAPI.ServiceUrl;
                    if (button.ToLower() == "Test API")
                    {
                        string UserId = HttpContext.Session.GetString("EmpId");
                        CaptureProductivityDetails(sqlCon, UserId, "Test API", "API ADDA", 1, "Test API ", "Test API In for EmpCode - " + UserId.Trim());

                        TempData["ActionName"] = "submitForm";
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(searchAPI.ServiceUrl);
                        request.Method = "POST";
                        request.ContentType = "application/Json";
                        ASCIIEncoding encoding = new ASCIIEncoding();
                        byte[] byte1 = encoding.GetBytes(searchAPI.Raw); //output
                        request.ContentLength = byte1.Length;
                        Stream requestStream = request.GetRequestStream();
                        requestStream.Write(byte1, 0, byte1.Length);
                        requestStream.Close();
                        HttpWebResponse webresponse = (HttpWebResponse)request.GetResponse();
                        Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                        StreamReader responseStream = new StreamReader(webresponse.GetResponseStream(), enc);
                        searchAPI.response = responseStream.ReadToEnd();
                        webresponse.Close();



                    }
                }
            }
            catch (Exception ex)
            {
                searchAPI.response = ex.Message;
            }
            //TempData["searchAPITemp"] = searchAPI;
            // TempData["TestApidata"] = JsonConvert.SerializeObject(searchAPI);
            //TempData.Keep();
            // return View(searchAPI);
            return View("apiDetails");
        }

        public IActionResult TestAPI()
        {
            //var ActionName = TempData["ActionName"];
            //if (ActionName == null || "TestApIView" == TempData["ActionName"].ToString())
            //{
            //    return View();
            //}
            //else
            //{
            //    var searchAPI = TempData["TestApidata"];
            //    var result = JsonConvert.DeserializeObject<SearchAPI>(searchAPI.ToString());
            //    TempData["ActionName"] = "TestApIView";
            //    return View(result);
            //}

            return View();
        }


        public IActionResult apiDetails(int id, string ser, SearchAPI search_apiold, string button = null)
        {
            DataTable dt = new DataTable();
            SearchAPI search_api = new SearchAPI();
            try
            {


                if (button != null)
                {

                    ModelState.Clear();
                    search_api.ServiceUrl = search_apiold.ServiceUrl;
                    search_api.ServiceDesc = search_apiold.ServiceDesc;
                    search_api.ServiceType = search_apiold.ServiceType;
                    search_api.Raw = search_apiold.Raw;
                    search_api.lstConsumer = search_apiold.lstConsumer;
                    search_api.lstMiddlewares = search_apiold.lstMiddlewares;
                    search_api.lstProducer = search_apiold.lstProducer;
                    search_api.lstServiceProviders = search_apiold.lstServiceProviders;
                    if (button == "Test API")
                    {
                        string UserId = HttpContext.Session.GetString("EmpId");
                        CaptureProductivityDetails(sqlCon, UserId, "Test API", "API ADDA", 1, "Test API ", " Test API In for EmpCode - " + UserId.Trim());
                        //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
                        //if (System.Net.ServicePointManager.SecurityProtocol == (SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls))
                        //    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                        // -----------------------------------------------------old-----------------------------
                        //TempData["ActionName"] = "submitForm";
                        //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(search_api.ServiceUrl);
                        //request.Method = "POST";
                        //request.ContentType = "application/Json";
                        //ASCIIEncoding encoding = new ASCIIEncoding();
                        //byte[] byte1 = encoding.GetBytes(search_api.Raw); //output
                        //request.ContentLength = byte1.Length;
                        //Stream requestStream = request.GetRequestStream();
                        //requestStream.Write(byte1, 0, byte1.Length);
                        //requestStream.Close();
                        //HttpWebResponse webresponse = (HttpWebResponse)request.GetResponse();
                        //Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                        //StreamReader responseStream = new StreamReader(webresponse.GetResponseStream(), enc);
                        //search_api.response = responseStream.ReadToEnd();
                        //webresponse.Close();

                        //----------------------------------------New------with soap handale--------------------
                        TempData["ActionName"] = "submitForm";
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(search_api.ServiceUrl);
                        request.Method = "POST";
                        request.ContentType = "application/Json";
                        ASCIIEncoding encoding = new ASCIIEncoding();
                        byte[] byte1 = encoding.GetBytes(search_api.Raw); //output
                        request.ContentLength = byte1.Length;
                        string[] SoapList = Convert.ToString(search_api.Raw).Split('<');
                        string[] SoapList1 = SoapList[1].Split(':');
                        if (SoapList1[0].ToString() == "soapenv")
                            // request.ContentType = "application/soap";
                            request.ContentType = "text/xml";
                        //request.ContentType = "application/soap+xml";
                        Stream requestStream = request.GetRequestStream();
                        requestStream.Write(byte1, 0, byte1.Length);
                        requestStream.Close();
                        HttpWebResponse webresponse = (HttpWebResponse)request.GetResponse();
                        Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                        StreamReader responseStream = new StreamReader(webresponse.GetResponseStream(), enc);
                        search_api.response = responseStream.ReadToEnd();
                        webresponse.Close();

                    }

                    return View(search_api);
                }
                else
                {
                    string UserId = HttpContext.Session.GetString("EmpId");
                    CaptureProductivityDetails(sqlCon, UserId, "API Details", "API ADDA", 1, "API Details ", " API Details In for EmpCode - " + UserId.Trim());


                    search_api.APIMainId = id;
                    search_api.SpName = "sp_APIA_Search_API";
                    search_api.IdentFlag = "GetDetailsById";
                    search_api.MethodFlag = "GetDetailsOfAPIById";
                    dt = submitRepository.GetAPIDetails(search_api);
                    search_api.ServiceUrl = dt.Rows[0]["ServiceURL"].ToString();
                    search_api.ServiceDesc = dt.Rows[0]["SERVICE_DESC"].ToString();
                    search_api.ServiceType = dt.Rows[0]["SERVICE_TYPE"].ToString();
                    search_api.RequestSample = dt.Rows[0]["REQUEST_SAMPLE"].ToString();
                    search_api.ResponseSample = dt.Rows[0]["RESPONSE_SAMPLE"].ToString();
                    search_api.apiCategory = dt.Rows[0]["apiCategory"].ToString();
                    search_api.UserJourneyFiles = dt.Rows[0]["APIAddFiles"].ToString();
                    // search_api.SearchAPIURL = ser;
                    search_api.lstConsumer = search_apiold.lstConsumer;
                    search_api.lstMiddlewares = search_apiold.lstMiddlewares;
                    search_api.lstProducer = search_apiold.lstProducer;
                    search_api.lstServiceProviders = search_apiold.lstServiceProviders;
                    search_api.ServiceDocumentFile = dt.Rows[0]["ServiceDocumrntfile"].ToString();
                    HttpContext.Session.SetString("ServiceDocumentFilePath", search_api.ServiceDocumentFile);

                    search_api.Raw = "No Data Available";

                    string path = dt.Rows[0]["FileName"].ToString();
                    String[] serviceUrlList;


                    ///  ---------------------------------------------Json File Read---------------------------------------------------
                    if (search_api.apiCategory == "SOAP" || search_api.apiCategory == "REST" || search_api.apiCategory == null || search_api.apiCategory == "")
                    {
                        serviceUrlList = ser.Split("/");
                        int countLastServiceUrlValue = serviceUrlList.Count();
                        search_api.ServiceUrl = serviceUrlList[countLastServiceUrlValue - 1].Split('?')[0];
                        search_api.Raw = "No Data Available";
                        if (path != null && path != "")
                        {

                            var basePathdoc = Path.Combine(Directory.GetCurrentDirectory() + "\\Collections");
                            basePathdoc = basePathdoc + "\\" + path + ".json";
                            FileInfo file = new FileInfo(basePathdoc);
                            if (file.Exists)
                            {
                                var myJsonString = System.IO.File.ReadAllText("./Collections/" + path + ".json");
                                var jObject = JObject.Parse(myJsonString);
                                //---------------------------------------------------- Json File read-----------------------------------------------------///

                                foreach (var item in jObject["item"])
                                {
                                    if (item["request"] != null)
                                    {
                                        var request = item["request"];

                                        if (request != null)
                                        {
                                            var url = request["url"];
                                            if (url != null)
                                            {
                                                var Urlraw = url["raw"].ToString();
                                                String[] UrlList = Urlraw.Split("/");
                                                int countLastUrlValue = UrlList.Count();
                                                Urlraw = UrlList[countLastUrlValue - 1].Split('?', '\n')[0];
                                                if (Urlraw == search_api.ServiceUrl)
                                                {
                                                    var body = request["body"];
                                                    if (body != null)
                                                    {
                                                        string raw = body["raw"].ToString();
                                                        search_api.Raw = raw;
                                                        search_api.ServiceUrl = url["raw"].ToString();

                                                    }
                                                    break;
                                                }
                                            }
                                        }

                                    }
                                    else if (item["item"] != null)///-------------------case1-----------when dobule Item
                                    {
                                        var items = item["item"];

                                        for (int i = 0; i < items.Count(); i++)
                                        {
                                            if (items[i]["item"] != null)
                                            {
                                                items = items[i]["item"];
                                                i = 0;
                                            }
                                            else
                                            {
                                                var request = items[i]["request"];
                                                if (request != null)
                                                {
                                                    var url = request["url"];
                                                    if (url != null)
                                                    {
                                                        var Urlraw = url["raw"].ToString();
                                                        String[] UrlList = Urlraw.Split("/");
                                                        int countLastUrlValue = UrlList.Count();
                                                        Urlraw = UrlList[countLastUrlValue - 1].Split('?', '\n')[0];
                                                        if (Urlraw == search_api.ServiceUrl)
                                                        {
                                                            var body = request["body"];
                                                            if (body != null)
                                                            {
                                                                string raw = body["raw"].ToString();
                                                                search_api.Raw = raw;
                                                                search_api.ServiceUrl = url["raw"].ToString();

                                                            }
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }


                                    }
                                    if (search_api.Raw != "No Data Available")
                                    {
                                        break;
                                    }
                                }

                            }


                        }

                    }
                    #region //---------------------file json by class object-------------------------
                    //if (search_api.apiCategory != "SOAP")
                    //{

                    //    search_api.Raw = "No Data Available";
                    //    if (path != null && path != "")
                    //    {
                    //        var basePathdoc = Path.Combine(Directory.GetCurrentDirectory() + "\\Collections");
                    //        basePathdoc = basePathdoc + "\\" + path + ".json";
                    //        FileInfo file = new FileInfo(basePathdoc);
                    //        if (file.Exists)
                    //        {
                    //            var myJsonString = System.IO.File.ReadAllText("./Collections/" + path + ".json");

                    //            serviceUrlList = ser.Split("/");
                    //            int countLastServiceUrlValue = serviceUrlList.Count();
                    //            search_api.ServiceUrl = serviceUrlList[countLastServiceUrlValue - 1].Split('?')[0]; 
                    //            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Application>(myJsonString);

                    //            for (int i = 0; i < data.item.Count; i++)
                    //            {
                    //                if (data.item[i].request != null)
                    //                {
                    //                    if (data.item[i].request.url != null)
                    //                    {
                    //                        string url = data.item[i].request.url.raw;

                    //                        String[] UrlList = url.Split("/");
                    //                        int countLastUrlValue = UrlList.Count();
                    //                        url = UrlList[countLastUrlValue - 1];
                    //                        if (url == search_api.ServiceUrl)
                    //                        {
                    //                            string Name = data.item[i].name;
                    //                            string raw = data.item[i].request.body.raw;
                    //                            if (data.item[i].response.Count > 0)
                    //                            {
                    //                                string response = data.item[i].response.ToString();
                    //                                search_api.response = response;
                    //                            }
                    //                            else
                    //                            {
                    //                                search_api.response = "No Data Available";
                    //                            }
                    //                            search_api.Name = Name;
                    //                            search_api.Raw = raw;
                    //                            break;
                    //                        }
                    //                    }
                    //                }
                    //            }
                    //        }
                    //        else
                    //        {
                    //            search_api.response = "No Data Available";
                    //        }

                    //    }
                    //    else
                    //    {
                    //        search_api.response = "No Data Available";
                    //    }

                    //}
                    //else
                    //{

                    //    search_api.Raw = "No Data Available";
                    //    if (path != null && path != "")
                    //    {
                    //        var basePathdoc = Path.Combine(Directory.GetCurrentDirectory() + "\\Collections");
                    //        basePathdoc = basePathdoc + "\\" + path + ".json";
                    //        FileInfo file = new FileInfo(basePathdoc);
                    //        if (file.Exists)
                    //        {
                    //            var myJsonString = System.IO.File.ReadAllText("./Collections/" + path + ".json");

                    //            serviceUrlList = ser.Split("/");
                    //            int countLastServiceUrlValue = serviceUrlList.Count();
                    //            search_api.ServiceUrl = serviceUrlList[countLastServiceUrlValue - 1].Split('?')[0]; 
                    //            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<SoapApplication>(myJsonString);

                    //            for (int i = 0; i < data.item.Count; i++)
                    //            {

                    //                if (data.item[i] != null)
                    //                {
                    //                    for (int j = 0; j < data.item[i].Item.Count; j++)
                    //                    {
                    //                        if (data.item[i].Item[j].request != null)
                    //                        {
                    //                            string url = data.item[i].Item[j].request.url.raw;
                    //                            String[] UrlList = url.Split("/");
                    //                            int countLastUrlValue = UrlList.Count();
                    //                            url = UrlList[countLastUrlValue - 1];
                    //                            if (url == search_api.ServiceUrl)
                    //                            {
                    //                                string Name = data.item[i].name;
                    //                                string raw = data.item[i].Item[j].request.body.raw;
                    //                                //if (data.item[i].Item[j].response.Count > 0)
                    //                                //{


                    //                                //    //search_api.responseSoas.Add(new ResponseSoa()
                    //                                //    //{
                    //                                //    //    name = data.item[i].Item[j].response[j].name.ToString(),
                    //                                //    //    code = Convert.ToInt32(data.item[i].Item[j].response[j].code.ToString()),
                    //                                //    //    status = data.item[i].Item[j].response[j].status.ToString(),
                    //                                //    //    _postman_previewlanguage = data.item[i].Item[j].response[j]._postman_previewlanguage.ToString(),

                    //                                //    //});

                    //                                //    ResponseSoa responseSoa = new ResponseSoa();
                    //                                //    responseSoa.name = data.item[i].Item[j].response[j].name.ToString();
                    //                                //    responseSoa.status = data.item[i].Item[j].response[j].status.ToString();
                    //                                //    responseSoa.code = Convert.ToInt32(data.item[i].Item[j].response[j].code.ToString());
                    //                                //    responseSoa._postman_previewlanguage = data.item[i].Item[j].response[j]._postman_previewlanguage.ToString();
                    //                                //    search_api.response = "{Name:" + responseSoa.name + "Code:" + responseSoa.code + "status: " + responseSoa.status + "postman_previewlanguage " + responseSoa._postman_previewlanguage + "}";
                    //                                //}
                    //                                //else
                    //                                //{
                    //                                //    search_api.response = "No Data Available";
                    //                                //}
                    //                                search_api.Name = Name;
                    //                                search_api.Raw = raw;
                    //                                break;
                    //                            }
                    //                        }


                    //                    }

                    //                    if (search_api.Name != null)
                    //                    {
                    //                        break;
                    //                    }
                    //                }
                    //            }
                    //       }
                    //        //else
                    //        //    {
                    //        //        search_api.response = "No Data Available";
                    //        //    }
                    //        }
                    //        //else
                    //        //{
                    //        //    search_api.response = " No Data Available";
                    //        //}


                    //    }

                    // search_api.ServiceUrl = "https://hbentbpuatap.hdfcbankuat.com:9550"+dt.Rows[0]["ServiceURL"].ToString();
                    #endregion

                    // 
                    search_api.ServiceUrl = search_api.ServiceUrl.Replace("{{obp_url}}", "http://10.226.163.7:8002");

                    return View(search_api);
                }
            }
            catch (Exception ex)
            {
                // search_api.Raw = ex.Message;

                search_api.ResponseSample = ex.Message;
                search_api.response = ex.Message;
                return View(search_api);
            }



        }

        public ActionResult SearchFilter(SearchAPI searchModel)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/search.cshtml");
            }
            StringBuilder sb = new StringBuilder();
            foreach (var items in searchModel.lstMiddlewares)
            {
                sb.Append(items.MiddlewareName + ",");
            }
            ViewBag.middlewareNames = sb.ToString();
            return View("search");
        }


        //public IActionResult Appprovalintegration()
        //{
        //    return View();
        //}
        public IActionResult Appprovalintegration(NewIntegration newIntegration = null, int integrationId = 0)
        {

            try
            {

                @ViewBag.Role = HttpContext.Session.GetString("Role");
                if (newIntegration.IntegrationId > 0)
                {
                    ModelState.Clear();

                    newIntegration = submitRepository.GetNewIntegrationById(newIntegration.IntegrationId);
                    newIntegration.Role = HttpContext.Session.GetString("Role");
                    newIntegration = submitRepository.GetBTGUserDropDownListData(newIntegration);
                    newIntegration = submitRepository.GetITUserDropDownListData(newIntegration);
                    if (newIntegration.ParentIntegrationId == 0)
                    {
                        HttpContext.Session.SetString("folderName", "API" + newIntegration.CreatedAt.ToString("ddMMMyyyy") + newIntegration.IntegrationId);
                    }
                    else
                    {
                        HttpContext.Session.SetString("folderName", "API" + newIntegration.CreatedAt.ToString("ddMMMyyyy") + newIntegration.ParentIntegrationId);
                    }

                    if (HttpContext.Session.GetString("AccessRight") == "AccessRight")
                    {
                        @ViewBag.UserType = HttpContext.Session.GetString("Role");
                    }
                    else
                    {
                        @ViewBag.UserType = "AllActionButtonHide";
                    }
                    var createdDate = newIntegration.CreatedAt.ToString("ddMMMyyyy");
                    var FolderName = "API" + createdDate + (newIntegration.ParentIntegrationId == null || newIntegration.ParentIntegrationId == 0 ? newIntegration.IntegrationId : newIntegration.ParentIntegrationId) + @"\";
                    TempData["APIAddaId"] = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\APIAddaDoc\NewIntegrations\") + FolderName;
                    TempData["Editbydb"] = "Appprovalintegration?integrationId=" + integrationId;


                    //ViewBag.ProjectMgrBTG = newIntegration.ProjectMgrBTG;
                    //ViewBag.ProjectMgrIT = newIntegration.ProjectMgrIT;
                    //ViewBag.ProjectName = newIntegration.ProjectName;
                    //ViewBag.ProjectId = newIntegration.ProjectId;
                    //ViewBag.GoLiveDate = Convert.ToString(newIntegration.GoLiveDate);
                    //ViewBag.BusinesssJustification = newIntegration.BusinesssJustification;
                    //ViewBag.BusinessSponsor = newIntegration.BusinessSponsor;
                    //ViewBag.ExecutiveSponsor = newIntegration.ExecutiveSponsor;
                    //ViewBag.CostCenterCode = newIntegration.CostCenterCode;
                    //ViewBag.UserJourneyDoc = newIntegration.UserJourneyFiles;


                    // @ViewBag.UserType = "ITUSER";
                    // @ViewBag.UserType = "ITARCHITECH";

                    // ViewBag.
                    //ViewBag.SubmitValue = "Update";
                    //ViewBag.Number = "1";
                }

                //  newIntegration.Role = HttpContext.Session.GetString("Role");
                return View(newIntegration);
            }
            catch (Exception)
            {

                throw;
            }

        }

        //
        /// Approval work Flow User Wise.
        ////
        ///

        [HttpPost]
        //

        //public async Task<IActionResult> Appprovalintegration(NewIntegration umodel, string button = null,string approvaldata=null)
        public IActionResult Appprovalintegration(NewIntegration umodel, string button = null)
        {
            var httpClient = new HttpClient();
            var jiraApiService = new JIRACreatorController(httpClient);
            try
            {
                // var serviceDetails1 = umodel.serviceDetails1;

                var umodel1 = submitRepository.GetNewIntegrationById(umodel.IntegrationId);
                string ConsumerApplicationId = umodel1.integrationLists[0].ConsumerApplicationId;
                string LogUserid = umodel1.integrationLists[0].CreatedBy;
                if (umodel.ParentIntegrationId == 0)
                {
                    HttpContext.Session.SetString("folderName", "API" + umodel1.CreatedAt.ToString("ddMMMyyyy") + umodel.IntegrationId);
                }
                else
                {
                    HttpContext.Session.SetString("folderName", "API" + umodel1.CreatedAt.ToString("ddMMMyyyy") + umodel.ParentIntegrationId);
                }
                //umodel = submitRepository.GetNewIntegrationById(umodel.IntegrationId);
                //  umodel.Role = HttpContext.Session.GetString("Role");
                int ServiceID = 0;
                @ViewBag.Role = HttpContext.Session.GetString("Role");
                // umodel = submitRepository.GetBTGUserDropDownListData(umodel);
                // umodel = submitRepository.GetITUserDropDownListData(umodel);
                //if (umodel.SequenceDiagDocument != null)
                //{
                //    umodel.SqfileName = umodel.SequenceDiagDocument[0].FileName;
                //    HttpContext.Session.SetString("SqfileName", umodel.SqfileName);
                //    string extension = Path.GetExtension(umodel.SqfileName);
                //    foreach (var file in umodel.SequenceDiagDocument)
                //    {
                //        //var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + DateTime.Now.ToString("ddMMMyyyy") + "" + umodel.IntegrationId + "\\");
                //        var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + "API" + HttpContext.Session.GetString("folderName") + "" + umodel.IntegrationId + "\\");
                //        bool basePathExists = System.IO.Directory.Exists(basePath);
                //        if (!basePathExists) Directory.CreateDirectory(basePath);
                //        umodel.modifySequenceDiagFileName = "SQDiagram" + "_API_" + DateTime.Now.ToString("ddMMMyyyy") + "_" + umodel.IntegrationId + extension;

                //        var filePath = Path.Combine(basePath, umodel.modifySequenceDiagFileName);
                //        using (var stream = new FileStream(filePath, FileMode.Create))
                //        {
                //            file.CopyToAsync(stream);
                //        }
                //    }
                //}
                //else if (umodel.modifySequenceDiagFileName != "" && umodel.modifySequenceDiagFileName != null)
                //{
                //    //String[] strlist = umodel.modifySequenceDiagFileName.Split("_");
                //    //string FileNameM = strlist[1].ToString();
                //    //umodel.SqfileName = FileNameM;
                //    umodel.SqfileName = HttpContext.Session.GetString("SqfileName");
                //}
                if (button.Contains("Edit"))
                {
                    var ProjectManagerBTG = umodel.ProjectMgrBTG;
                    var ProjectManagerIT = umodel.ProjectMgrIT;
                    var ProjectName = umodel.ProjectName;
                    var ProjectID = umodel.ProjectId;
                    var PlannedGoLiveDate = umodel.GoLiveDate;
                    var BusinessJustification = umodel.BusinesssJustification;
                    var BusinessSponsor = umodel.BusinessSponsor;
                    var ExecutiveSponsor = umodel.ExecutiveSponsor;
                    var ConsumerApplication = umodel.ConsumerApplication;
                    var CostCenterCode = umodel.CostCenterCode;
                    var ChannelId = umodel.ChannelId;
                    var ContainerName = umodel.ContainerName;
                    var BTGUSER = umodel.BTGUSER;
                    var ITUSER = umodel.ITUSER;
                    var ParentIntegrationI = umodel.ParentIntegrationId;
                    // serviceDetails1 = umodel.serviceDetails1;


                    String[] strlist = button.Split("+");
                    ServiceID = Convert.ToInt32(strlist[1].ToString());
                    button = strlist[0].ToString();
                    //umodel = submitRepository.GetNewIntegrationById(umodel.IntegrationId);

                    umodel.ProjectMgrBTG = ProjectManagerBTG;
                    umodel.ProjectMgrIT = ProjectManagerIT;
                    umodel.ProjectName = ProjectName;
                    umodel.ProjectId = ProjectID;
                    umodel.GoLiveDate = PlannedGoLiveDate;
                    umodel.BusinesssJustification = BusinessJustification;
                    umodel.BusinessSponsor = BusinessSponsor;
                    umodel.ExecutiveSponsor = ExecutiveSponsor;
                    umodel.ConsumerApplication = ConsumerApplication;
                    umodel.CostCenterCode = CostCenterCode;
                    umodel.ChannelId = ChannelId;
                    umodel.ContainerName = ContainerName;
                    umodel.BTGUSER = BTGUSER;
                    umodel.ITUSER = ITUSER;

                    umodel = submitRepository.GetAllDropDownListData(umodel);
                    umodel = submitRepository.GetBTGUserDropDownListData(umodel);
                    umodel = submitRepository.GetITUserDropDownListData(umodel);
                    umodel = submitRepository.GetQuestionList(umodel, ServiceID);

                    //RemoveListData(umodel);


                }
                if (button == "Update")
                {
                    umodel = submitRepository.GetAllDropDownListData(umodel);
                    umodel = submitRepository.GetBTGUserDropDownListData(umodel);
                    umodel = submitRepository.GetITUserDropDownListData(umodel);

                    if (umodel.SequenceDiagDocument != null)
                    {
                        umodel.SqfileName = umodel.SequenceDiagDocument.FileName;
                        HttpContext.Session.SetString("SqfileName", umodel.SqfileName);
                        string extension = Path.GetExtension(umodel.SqfileName);
                        //foreach (var file in umodel.SequenceDiagDocument)
                        //{
                        var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + HttpContext.Session.GetString("folderName") + "\\");
                        bool basePathExists = System.IO.Directory.Exists(basePath);
                        if (!basePathExists) Directory.CreateDirectory(basePath);
                        umodel.modifySequenceDiagFileName = "SequenceDiagDoc" + "_API_" + DateTime.Now.ToString("ddMMMyyyy") + "_" + umodel.IntegrationId + extension;

                        var filePath = Path.Combine(basePath, umodel.modifySequenceDiagFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            umodel.SequenceDiagDocument.CopyTo(stream);
                        }
                        //}
                    }
                    else if (umodel.modifySequenceDiagFileName != "" && umodel.modifySequenceDiagFileName != null)
                    {
                        //String[] strlist = umodel.modifySequenceDiagFileName.Split("_");
                        //string FileNameM = strlist[1].ToString();
                        //umodel.SqfileName = FileNameM;
                        umodel.SqfileName = umodel.modifySequenceDiagFileName;
                    }

                    // umodel.serviceDetails1 = serviceDetails1;
                    //-------------------------------------------UpdateList-- UpdateRow --------------------------
                    if (HttpContext.Session.GetString("PopupEdit") != null && HttpContext.Session.GetString("PopupEdit") == "EditMode")
                    {
                        int TemserviceId = Convert.ToInt32(HttpContext.Session.GetString("ServiceID"));

                        foreach (var item in umodel.serviceDetails)
                        {
                            if (TemserviceId == item.ServiceID)
                            {

                                item.InternalServiceName = umodel.serviceDetails1.InternalServiceName;
                                item.ServiceID = umodel.serviceDetails1.ServiceID;
                                item.Purpose = umodel.serviceDetails1.Purpose;
                                item.ExternalServiceName = umodel.serviceDetails1.ExternalServiceName;
                                item.ProducerApplication = umodel.serviceDetails1.ProducerApplication;
                                item.IsAPIGW = umodel.serviceDetails1.IsAPIGW;
                                item.Transformation = umodel.serviceDetails1.Transformation;
                                item.Volume = umodel.serviceDetails1.Volume;
                                item.Existing_New_Id = umodel.serviceDetails1.Existing_New_Id;
                                item.Rest_SOAP_Id = umodel.serviceDetails1.Rest_SOAP_Id;
                                item.ServiceType_Id = umodel.serviceDetails1.ServiceType_Id;
                                item.APIType_Id = umodel.serviceDetails1.APIType_Id;
                                item.APICategory_Id = umodel.serviceDetails1.APICategory_Id;
                                item.DomainName_Id = umodel.serviceDetails1.DomainName_Id;
                                item.ConsumerDC = umodel.serviceDetails1.ConsumerDC;
                                item.ProducerDC = umodel.serviceDetails1.ProducerDC;
                                item.Platform = umodel.serviceDetails1.Platform;
                                ViewBag.APICategory = item.APICategory_Id;
                                ViewBag.ExistingNewId = item.Existing_New_Id;
                                ViewBag.currentserviceid = item.ServiceID;
                                item.TotalScore = umodel.serviceDetails1.TotalScore;
                                item.RistClassify = umodel.serviceDetails1.RistClassify;
                                item.MiddlewareNameId = umodel.serviceDetails1.MiddlewareNameId;

                                if (umodel.QuestinonselectedList != null)
                                {
                                    if (umodel.QuestinonselectedList.Count > 0)
                                    {
                                        List<QuestionServiceDetails> listQuestionServiceDetails = new List<QuestionServiceDetails>();

                                        var QuestionLists = umodel.QuestionList.Where(x => x.QType == umodel.IN_Platform.Trim()).ToList();
                                        if (umodel.IN_Platform.Trim() != "Internal")
                                        {
                                            QuestionLists = umodel.QuestionList.Where(x => x.QType == umodel.IN_Platform.Trim() && x.APICategoryId == umodel.serviceDetails1.APICategory_Id).ToList();
                                        }


                                        for (int i = 0; i < umodel.QuestinonselectedList.Count; i++)
                                        {
                                            var selecedList = umodel.QDropValuesList.Where(x => x.Weightage == umodel.QuestinonselectedList[i].Weightage && x.QID == QuestionLists[i].ID).ToList();
                                            umodel.QuestinonselectedList[i].QID = selecedList[0].QID;
                                            umodel.QuestinonselectedList[i].QuesOptionsID = selecedList[0].ID;
                                            umodel.QuestinonselectedList[i].Weightage = selecedList[0].Weightage;
                                            umodel.QuestinonselectedList[i].Val = selecedList[0].Val;
                                            umodel.QuestinonselectedList[i].QusServiceID = umodel.serviceDetails1.ServiceID;

                                            QuestionServiceDetails questionService = new QuestionServiceDetails();
                                            questionService.QID = umodel.QuestinonselectedList[i].QID;
                                            questionService.QuesOptionsID = umodel.QuestinonselectedList[i].QuesOptionsID;
                                            questionService.QusServiceID = umodel.serviceDetails1.ServiceID;
                                            questionService.Weightage = umodel.QuestinonselectedList[i].Weightage;
                                            questionService.Val = umodel.QuestinonselectedList[i].Val;

                                            listQuestionServiceDetails.Add(questionService);

                                            //umodel.QuestionServiceDetailsList.Add(new QuestionServiceDetails()
                                            //{
                                            //    QID = umodel.QuestinonselectedList[i].QusId,
                                            //    QuesOptionsID = umodel.QuestinonselectedList[i].QusChidId,
                                            //    QusServiceID = umodel.serviceDetails1.ServiceID,

                                            //});
                                        }
                                        if (umodel.QuestionServiceDetailsList != null)
                                        {
                                            var svcId = listQuestionServiceDetails.Select(x => x.QusServiceID).FirstOrDefault();

                                            umodel.QuestionServiceDetailsList.RemoveAll(x => x.QusServiceID == svcId);
                                            if (umodel.QuestionServiceDetailsList.Count > 0)
                                            {
                                                umodel.QuestionServiceDetailsList.AddRange(listQuestionServiceDetails);
                                            }
                                            else
                                            {
                                                umodel.QuestionServiceDetailsList = listQuestionServiceDetails;
                                            }
                                        }
                                        else
                                        {
                                            umodel.QuestionServiceDetailsList = listQuestionServiceDetails;
                                        }

                                    }
                                }
                                else
                                {
                                    if (umodel.QuestionServiceDetailsList != null)
                                    {


                                        for (int i = 0; i < umodel.QuestionServiceDetailsList.Count; i++)
                                        {

                                            if (TemserviceId == umodel.QuestionServiceDetailsList[i].QusServiceID)
                                            {

                                                umodel.QuestionServiceDetailsList[i].QusServiceID = umodel.QuestionServiceDetailsList[i].QusServiceID;
                                                umodel.QuestionServiceDetailsList[i].QID = umodel.QuestionServiceDetailsList[i].QID;
                                                umodel.QuestionServiceDetailsList[i].QuesOptionsID = umodel.QuestionServiceDetailsList[i].QuesOptionsID;
                                                umodel.QuestionServiceDetailsList[i].Weightage = umodel.QuestionServiceDetailsList[i].Weightage;
                                                umodel.QuestionServiceDetailsList[i].Val = umodel.QuestionServiceDetailsList[i].Val;

                                            }
                                        }

                                    }

                                }
                            }
                        }
                        //----
                        TempData["Editbydb"] = "Appprovalintegration?integrationId=" + umodel.IntegrationId;
                        TempData["PopupEdit"] = "UpdateMode";
                        HttpContext.Session.SetString("PopupEdit", "UpdateMode");
                        ViewBag.Number = "1";
                        ModelState.Clear();
                        SetFilePath(umodel);
                        @ViewBag.UserType = HttpContext.Session.GetString("Role");

                        return View(umodel);
                    }
                }
                else if (button == "Edit")
                {
                    if (umodel.SequenceDiagDocument != null)
                    {
                        umodel.SqfileName = umodel.SequenceDiagDocument.FileName;
                        HttpContext.Session.SetString("SqfileName", umodel.SqfileName);
                        string extension = Path.GetExtension(umodel.SqfileName);
                        //foreach (var file in umodel.SequenceDiagDocument)
                        //{
                        var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + HttpContext.Session.GetString("folderName") + "\\");
                        bool basePathExists = System.IO.Directory.Exists(basePath);
                        if (!basePathExists) Directory.CreateDirectory(basePath);
                        umodel.modifySequenceDiagFileName = "SequenceDiagDoc" + "_API_" + DateTime.Now.ToString("ddMMMyyyy") + "_" + umodel.IntegrationId + extension;

                        var filePath = Path.Combine(basePath, umodel.modifySequenceDiagFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            umodel.SequenceDiagDocument.CopyTo(stream);
                        }
                        //}
                    }
                    else if (umodel.modifySequenceDiagFileName != "" && umodel.modifySequenceDiagFileName != null)
                    {
                        //String[] strlist = umodel.modifySequenceDiagFileName.Split("_");
                        //string FileNameM = strlist[1].ToString();
                        //umodel.SqfileName = FileNameM;
                        umodel.SqfileName = umodel.modifySequenceDiagFileName;
                    }

                    //umodel.serviceDetails1= serviceDetails1;
                    var SelectedServiceDetails = umodel.serviceDetails.Where(x => x.ServiceID == ServiceID).ToList();
                    umodel.serviceDetails1 = new ServiceDetails();
                    foreach (var item in SelectedServiceDetails)
                    {
                        umodel.serviceDetails1.InternalServiceName = item.InternalServiceName;
                        umodel.serviceDetails1.ServiceID = item.ServiceID;
                        umodel.serviceDetails1.Purpose = item.Purpose;
                        umodel.serviceDetails1.ExternalServiceName = item.ExternalServiceName;
                        umodel.serviceDetails1.ProducerApplication = item.ProducerApplication;
                        umodel.serviceDetails1.IsAPIGW = item.IsAPIGW;
                        umodel.serviceDetails1.Transformation = item.Transformation;
                        umodel.serviceDetails1.Volume = item.Volume;
                        umodel.serviceDetails1.Existing_New_Id = item.Existing_New_Id;
                        umodel.serviceDetails1.Rest_SOAP_Id = item.Rest_SOAP_Id;
                        umodel.serviceDetails1.ServiceType_Id = item.ServiceType_Id;
                        umodel.serviceDetails1.APIType_Id = item.APIType_Id;
                        umodel.serviceDetails1.APICategory_Id = item.APICategory_Id;
                        //umodel.serviceDetails1.APIRiskScore_Id = item.APIRiskScore_Id;
                        //umodel.serviceDetails1.PartnerRiskScore_Id = item.PartnerRiskScore_Id;
                        // umodel.serviceDetails1.RistClassify = item.RistClassify;
                        //  umodel.serviceDetails1.TotalScore = item.TotalScore;
                        umodel.serviceDetails1.DomainName_Id = item.DomainName_Id;
                        umodel.serviceDetails1.ConsumerDC = item.ConsumerDC;
                        umodel.serviceDetails1.ProducerDC = item.ProducerDC;
                        umodel.serviceDetails1.Platform = item.Platform;
                        //umodel.serviceDetails1.OprationApi = item.OprationApi;
                        //umodel.serviceDetails1.ApiHandle = item.ApiHandle;
                        //umodel.serviceDetails1.FinancialTransaction = item.FinancialTransaction;
                        //umodel.serviceDetails1.EstimateValue = item.EstimateValue;
                        //umodel.serviceDetails1.Availability = item.Availability;
                        umodel.serviceDetails1.TotalScore = item.TotalScore;
                        umodel.serviceDetails1.MiddlewareNameId = item.MiddlewareNameId;
                        umodel.serviceDetails1.RistClassify = item.RistClassify;
                        umodel.serviceDetails1.ExpectedAPISpecificationFiles = item.ExpectedAPISpecificationFiles;
                        ViewBag.currentserviceid = umodel.serviceDetails1.ServiceID;

                        ViewBag.APICategory = umodel.serviceDetails1.APICategory_Id;

                        ViewBag.ExistingNewId = umodel.serviceDetails1.Existing_New_Id;


                        if (umodel.QuestionServiceDetailsList != null)
                        {
                            if (umodel.QuestionServiceDetailsList.Count > 0)
                            {
                                var checkDataQueServic = umodel.QuestionServiceDetailsList.Where(x => x.QusServiceID == ServiceID).ToList();

                                if (checkDataQueServic != null)
                                {
                                    if (checkDataQueServic.Count > 0)
                                    {
                                        List<Questinonselected> ListQuesselectedlistdb = new List<Questinonselected>();
                                        foreach (var QuesitemEdit in checkDataQueServic)
                                        {
                                            Questinonselected Questinonselected1 = new Questinonselected();
                                            Questinonselected1.QusServiceID = QuesitemEdit.QusServiceID;
                                            Questinonselected1.QID = QuesitemEdit.QID;
                                            Questinonselected1.QuesOptionsID = QuesitemEdit.QuesOptionsID;
                                            Questinonselected1.Weightage = QuesitemEdit.Weightage;
                                            Questinonselected1.Val = QuesitemEdit.Val;
                                            ListQuesselectedlistdb.Add(Questinonselected1);
                                        }
                                        umodel.QuestinonselectedList = ListQuesselectedlistdb;
                                    }
                                    //if (checkDataQueServic.Count > 0)
                                    //{
                                    //    var Qus = checkDataQueServic;//umodel.QuestionServiceDetailsList;

                                    //    List<Questinonselected> ListQuesselectedlistdb = new List<Questinonselected>();

                                    //    foreach (var Quesitem in Qus)
                                    //    {
                                    //        Questinonselected Questinonselected1 = new Questinonselected();
                                    //        Questinonselected1.QusServiceID = Quesitem.QusServiceID;
                                    //        Questinonselected1.QID = Quesitem.QID;
                                    //        Questinonselected1.QuesOptionsID = Quesitem.QuesOptionsID;
                                    //        Questinonselected1.Weightage = Quesitem.Weightage;
                                    //        Questinonselected1.Val = Quesitem.Val;
                                    //        ListQuesselectedlistdb.Add(Questinonselected1);
                                    //    }
                                    //    umodel.QuestinonselectedList = ListQuesselectedlistdb;

                                    //}
                                }
                            }
                        }
                    }

                    // ---------------------------Assign Question Value-------------------




                    ModelState.Clear();
                    var createdDate = umodel.CreatedAt.ToString("ddMMMyyyy");
                    var FolderName = "API" + createdDate + (umodel.ParentIntegrationId == null || umodel.ParentIntegrationId == 0 ? umodel.IntegrationId : umodel.ParentIntegrationId) + @"\";
                    TempData["APIAddaId"] = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\APIAddaDoc\NewIntegrations\") + HttpContext.Session.GetString("folderName") + @"\";
                    // ViewBag.SubmitValue = "Save";
                    TempData["Editbydb"] = "Appprovalintegration?integrationId=" + umodel.IntegrationId;
                    TempData["PopupEdit"] = "EditMode";
                    ViewBag.Number = "1";
                    HttpContext.Session.SetString("PopupEdit", "EditMode");
                    HttpContext.Session.SetString("ServiceID", Convert.ToString(ServiceID));
                    @ViewBag.UserType = HttpContext.Session.GetString("Role");
                    return View(umodel);
                }
                //// -------------------------------------------Update Session Update -----------------------------------------------------------------------------
                else if (button == "Feedback to User")
                {

                    if (umodel.SequenceDiagDocument != null)
                    {
                        umodel.SqfileName = umodel.SequenceDiagDocument.FileName;
                        HttpContext.Session.SetString("SqfileName", umodel.SqfileName);
                        string extension = Path.GetExtension(umodel.SqfileName);
                        // foreach (var file in umodel.SequenceDiagDocument)
                        // {
                        var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + HttpContext.Session.GetString("folderName") + "\\");
                        bool basePathExists = System.IO.Directory.Exists(basePath);
                        if (!basePathExists) Directory.CreateDirectory(basePath);
                        umodel.modifySequenceDiagFileName = "SequenceDiagDoc" + "_API_" + DateTime.Now.ToString("ddMMMyyyy") + "_" + umodel.IntegrationId + extension;

                        var filePath = Path.Combine(basePath, umodel.modifySequenceDiagFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            umodel.SequenceDiagDocument.CopyTo(stream);
                        }
                        //}
                    }
                    else if (umodel.modifySequenceDiagFileName != "" && umodel.modifySequenceDiagFileName != null)
                    {
                        //String[] strlist = umodel.modifySequenceDiagFileName.Split("_");
                        //string FileNameM = strlist[1].ToString();
                        //umodel.SqfileName = FileNameM;
                        umodel.SqfileName = umodel.modifySequenceDiagFileName;
                    }
                    else if (umodel.SequenceDiagFiles != "" && umodel.SequenceDiagFiles != null)
                    {
                        umodel.modifySequenceDiagFileName = umodel.SequenceDiagFiles;
                    }
                    if (HttpContext.Session.GetString("Role") == "BTGUSER")
                    {
                        //Feedback by BTG User workflowstatus = 4;

                        workflowstatus = 4;


                    }
                    else if (HttpContext.Session.GetString("Role") == "ITUSER")
                    {
                        //Feedback by IT User  workflowstatus = 5;
                        workflowstatus = 5;

                    }
                    else if (HttpContext.Session.GetString("Role") == "ITARCHITECH")
                    {
                        //Feedback by IT Architect  workflowstatus = 6;
                        workflowstatus = 6;

                    }



                    if (umodel.FeedbackCount >= 3 && HttpContext.Session.GetString("Role") == "BTGUSER")
                    {

                        TempData["IsSuccess"] = "False";
                        TempData["Result"] = "Feedback to user allow only 3 times ";
                        return RedirectToAction("NewIntegrationList", "Home");
                    }
                    else if (umodel.FeedbackCount >= 1 && HttpContext.Session.GetString("Role") == "ITUSER")
                    {
                        TempData["IsSuccess"] = "False";
                        TempData["Result"] = "Feedback to user allow only 1 time ";

                        return RedirectToAction("NewIntegrationList", "Home");
                    }
                    else if (umodel.FeedbackCount >= 1 && HttpContext.Session.GetString("Role") == "ITARCHITECH")
                    {
                        TempData["IsSuccess"] = "False";
                        TempData["Result"] = "Feedback to user allow only 1 time ";
                        return RedirectToAction("NewIntegrationList", "Home");
                    }
                    else
                    {
                        umodel.AssignTo = "USER";
                        umodel.AssignFrom = HttpContext.Session.GetString("Role");
                        umodel.UserId = HttpContext.Session.GetString("EmpId");
                        umodel.workflowstatus = workflowstatus;

                        UpdateWorkflowIntegrationservice(umodel);

                        SendEmail obj = new SendEmail();
                        obj.SendMailAlert(umodel, LogUserid, "Feedback to User", 0, ConsumerApplicationId); // Uncomment on prod

                        umodel.SpName = "SP_APIA_WorkFlowApprovalProcess";
                        umodel.MethodFlag = "FeedbackCount";
                        umodel.UserId = HttpContext.Session.GetString("EmpId");
                        umodel.workflowstatus = workflowstatus;
                        umodel = submitRepository.AddFeedback(umodel);


                        //umodel.SpName = "SP_APIA_WorkFlowApprovalProcess";
                        //umodel.MethodFlag = "AddFeedback";
                        // umodel = submitRepository.AddFeedback(umodel);

                        if (umodel.FeedbackId > 0)
                        {

                            //ViewBag.Result = "Data Saved Successfully";
                            TempData["Result"] = "Feedback sent to user successfully";
                            TempData["IsSuccess"] = "True";
                            ModelState.Clear();
                            umodel = new NewIntegration();
                            ViewBag.Number = "0";
                            string UserId = HttpContext.Session.GetString("EmpId");
                            CaptureProductivityDetails(sqlCon, UserId, "Appprovalintegration", "API ADDA", 1, "Feedback sent to user successfully ", "Feedback for EmpCode - " + UserId.ToString().Trim());
                        }
                        else
                        {
                            TempData["IsSuccess"] = "False";
                            TempData["Result"] = "Something Went Wrong..Please try again later";
                        }


                    }
                }
                else if (button == "Reviewed")
                {
                    //------------ Reviewd by BTG User----- workflowstatus = 7


                    if (umodel.SequenceDiagDocument != null)
                    {
                        umodel.SqfileName = umodel.SequenceDiagDocument.FileName;
                        HttpContext.Session.SetString("SqfileName", umodel.SqfileName);
                        string extension = Path.GetExtension(umodel.SqfileName);
                        // foreach (var file in umodel.SequenceDiagDocument)
                        // {
                        var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + HttpContext.Session.GetString("folderName") + "\\");
                        bool basePathExists = System.IO.Directory.Exists(basePath);
                        if (!basePathExists) Directory.CreateDirectory(basePath);
                        umodel.modifySequenceDiagFileName = "SequenceDiagDoc" + "_API_" + DateTime.Now.ToString("ddMMMyyyy") + "_" + umodel.IntegrationId + extension;

                        var filePath = Path.Combine(basePath, umodel.modifySequenceDiagFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            umodel.SequenceDiagDocument.CopyTo(stream);
                        }
                        //}
                    }
                    else if (umodel.modifySequenceDiagFileName != "" && umodel.modifySequenceDiagFileName != null)
                    {
                        //String[] strlist = umodel.modifySequenceDiagFileName.Split("_");
                        //string FileNameM = strlist[1].ToString();
                        //umodel.SqfileName = FileNameM;
                        umodel.SqfileName = umodel.modifySequenceDiagFileName;
                    }
                    else if (umodel.SequenceDiagFiles != "" && umodel.SequenceDiagFiles != null)
                    {
                        umodel.modifySequenceDiagFileName = umodel.SequenceDiagFiles;
                    }

                    workflowstatus = 7;

                    umodel.AssignTo = "ITUSER";
                    umodel.AssignFrom = HttpContext.Session.GetString("Role");
                    umodel.UserId = HttpContext.Session.GetString("EmpId");
                    umodel.workflowstatus = workflowstatus;
                    UpdateWorkflowIntegrationservice(umodel);

                    SendEmail obj = new SendEmail();
                    obj.SendMailAlert(umodel, LogUserid, "Reviewed by BTG User", 0, ConsumerApplicationId); // Uncomment on prod

                    //umodel.SpName = "SP_APIA_WorkFlowApprovalProcess";
                    //umodel.MethodFlag = "AddFeedback";
                    //
                    //umodel = submitRepository.AddFeedback(umodel);
                    if (umodel.FeedbackId > 0)
                    {
                        TempData["Result"] = "Request sent to IT User for review successfully";
                        TempData["IsSuccess"] = "True";
                        ModelState.Clear();
                        umodel = new NewIntegration();
                        ViewBag.Number = "0";
                        string UserId = HttpContext.Session.GetString("EmpId");
                        CaptureProductivityDetails(sqlCon, UserId, "Appprovalintegration", "API ADDA", 1, "Request sent to IT User for review successfully", "Request sent for EmpCode - " + UserId.ToString().Trim());
                    }
                    else
                    {
                        TempData["IsSuccess"] = "False";
                        TempData["Result"] = "Something Went Wrong..Please try again later";
                    }


                }
                else if (button == "Architech Review")
                {

                    ///----Reviewed by IT User----  workflowstatus = 8
                    ///


                    if (umodel.SequenceDiagDocument != null)
                    {
                        umodel.SqfileName = umodel.SequenceDiagDocument.FileName;
                        HttpContext.Session.SetString("SqfileName", umodel.SqfileName);
                        string extension = Path.GetExtension(umodel.SqfileName);
                        // foreach (var file in umodel.SequenceDiagDocument)
                        // {
                        var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + HttpContext.Session.GetString("folderName") + "\\");
                        bool basePathExists = System.IO.Directory.Exists(basePath);
                        if (!basePathExists) Directory.CreateDirectory(basePath);
                        umodel.modifySequenceDiagFileName = "SequenceDiagDoc" + "_API_" + DateTime.Now.ToString("ddMMMyyyy") + "_" + umodel.IntegrationId + extension;

                        var filePath = Path.Combine(basePath, umodel.modifySequenceDiagFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            umodel.SequenceDiagDocument.CopyTo(stream);
                        }
                        //}
                    }
                    else if (umodel.modifySequenceDiagFileName != "" && umodel.modifySequenceDiagFileName != null)
                    {
                        //String[] strlist = umodel.modifySequenceDiagFileName.Split("_");
                        //string FileNameM = strlist[1].ToString();
                        //umodel.SqfileName = FileNameM;
                        umodel.SqfileName = umodel.modifySequenceDiagFileName;
                    }
                    else if (umodel.SequenceDiagFiles != "" && umodel.SequenceDiagFiles != null)
                    {
                        umodel.modifySequenceDiagFileName = umodel.SequenceDiagFiles;
                    }

                    workflowstatus = 8;
                    umodel.AssignTo = "ITARCHITECH";
                    umodel.AssignFrom = HttpContext.Session.GetString("Role");
                    umodel.UserId = HttpContext.Session.GetString("EmpId");
                    umodel.workflowstatus = workflowstatus;
                    UpdateWorkflowIntegrationservice(umodel);

                    SendEmail obj = new SendEmail();
                    obj.SendMailAlert(umodel, LogUserid, "Reviewed by IT User", 0, ConsumerApplicationId); // Uncomment on prod

                    if (umodel.FeedbackId > 0)
                    {
                        TempData["Result"] = "Request sent to IT Architech for review successfully";
                        TempData["IsSuccess"] = "True";
                        ModelState.Clear();
                        umodel = new NewIntegration();
                        ViewBag.Number = "0";
                        string UserId = HttpContext.Session.GetString("EmpId");
                        CaptureProductivityDetails(sqlCon, UserId, "Appprovalintegration", "API ADDA", 1, "Request sent to IT Architech for review successfully", "Request sent for EmpCode - " + UserId.ToString().Trim());
                    }
                    else
                    {
                        TempData["IsSuccess"] = "False";
                        TempData["Result"] = "Something Went Wrong..Please try again later";
                    }


                }
                else if (button == "Reject")
                {
                    ////----Rejected workflowstatus = 2;


                    if (umodel.SequenceDiagDocument != null)
                    {
                        umodel.SqfileName = umodel.SequenceDiagDocument.FileName;
                        HttpContext.Session.SetString("SqfileName", umodel.SqfileName);
                        string extension = Path.GetExtension(umodel.SqfileName);
                        //foreach (var file in umodel.SequenceDiagDocument)
                        // {
                        var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + HttpContext.Session.GetString("folderName") + "\\");
                        bool basePathExists = System.IO.Directory.Exists(basePath);
                        if (!basePathExists) Directory.CreateDirectory(basePath);
                        umodel.modifySequenceDiagFileName = "SequenceDiagDoc" + "_API_" + DateTime.Now.ToString("ddMMMyyyy") + "_" + umodel.IntegrationId + extension;

                        var filePath = Path.Combine(basePath, umodel.modifySequenceDiagFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            umodel.SequenceDiagDocument.CopyTo(stream);
                        }
                        // }
                    }
                    else if (umodel.modifySequenceDiagFileName != "" && umodel.modifySequenceDiagFileName != null)
                    {
                        //String[] strlist = umodel.modifySequenceDiagFileName.Split("_");
                        //string FileNameM = strlist[1].ToString();
                        //umodel.SqfileName = FileNameM;
                        umodel.SqfileName = umodel.modifySequenceDiagFileName;
                    }
                    else if (umodel.SequenceDiagFiles != "" && umodel.SequenceDiagFiles != null)
                    {
                        umodel.modifySequenceDiagFileName = umodel.SequenceDiagFiles;
                    }


                    workflowstatus = 2;

                    umodel.AssignTo = "USER";
                    umodel.AssignFrom = HttpContext.Session.GetString("Role");
                    umodel.UserId = HttpContext.Session.GetString("EmpId");
                    umodel.workflowstatus = workflowstatus;
                    UpdateWorkflowIntegrationservice(umodel);

                    SendEmail obj = new SendEmail();
                    obj.SendMailAlert(umodel, LogUserid, "Rejected", 0, ConsumerApplicationId); // Uncomment on prod

                    if (umodel.FeedbackId > 0)
                    {
                        TempData["Result"] = "Request rejected successfully";
                        TempData["IsSuccess"] = "True";
                        ModelState.Clear();
                        umodel = new NewIntegration();
                        ViewBag.Number = "0";
                        string UserId = HttpContext.Session.GetString("EmpId");
                        CaptureProductivityDetails(sqlCon, UserId, "Appprovalintegration", "API ADDA", 1, "Request rejected successfully", "Request sent for EmpCode - " + UserId.ToString().Trim());
                    }
                    else
                    {
                        TempData["IsSuccess"] = "False";
                        TempData["Result"] = "Something Went Wrong..Please try again later";
                    }


                }
                else if (button == "Approved")
                {

                    if (umodel.SequenceDiagDocument != null)
                    {
                        umodel.SqfileName = umodel.SequenceDiagDocument.FileName;
                        HttpContext.Session.SetString("SqfileName", umodel.SqfileName);
                        string extension = Path.GetExtension(umodel.SqfileName);
                        //foreach (var file in umodel.SequenceDiagDocument)
                        //{
                        var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIAddaDoc\\NewIntegrations\\" + HttpContext.Session.GetString("folderName") + "\\");
                        bool basePathExists = System.IO.Directory.Exists(basePath);
                        if (!basePathExists) Directory.CreateDirectory(basePath);
                        umodel.modifySequenceDiagFileName = "SequenceDiagDoc" + "_API_" + DateTime.Now.ToString("ddMMMyyyy") + "_" + umodel.IntegrationId + extension;

                        var filePath = Path.Combine(basePath, umodel.modifySequenceDiagFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            umodel.SequenceDiagDocument.CopyTo(stream);
                        }
                        //}
                    }
                    else if (umodel.modifySequenceDiagFileName != "" && umodel.modifySequenceDiagFileName != null)
                    {
                        //String[] strlist = umodel.modifySequenceDiagFileName.Split("_");
                        //string FileNameM = strlist[1].ToString();
                        //umodel.SqfileName = FileNameM;
                        umodel.SqfileName = umodel.modifySequenceDiagFileName;
                    }
                    else if (umodel.SequenceDiagFiles != "" && umodel.SequenceDiagFiles != null)
                    {
                        umodel.modifySequenceDiagFileName = umodel.SequenceDiagFiles;
                    }


                    // ----Closed = 3;
                    workflowstatus = 3;
                    umodel.AssignTo = "USER";
                    umodel.AssignFrom = HttpContext.Session.GetString("Role");
                    umodel.UserId = HttpContext.Session.GetString("EmpId");
                    umodel.workflowstatus = workflowstatus;
                    umodel.UserJourneyFiles = umodel.UserJourneyFiles;
                    UpdateWorkflowIntegrationservice(umodel);

                    //JIRA Creation Logic
                    jiraApiService.CreateJIRA(umodel);
                    //End

                    //Send Email logic start
                    SendEmail obj = new SendEmail();
                    obj.SendMailAlert(umodel, LogUserid, "Approved", 0, ConsumerApplicationId); // Uncomment on prod
                    // End

                    //umodel.SpName = "SP_APIA_WorkFlowApprovalProcess";
                    //umodel.MethodFlag = "AddFeedback";
                    //umodel = submitRepository.AddFeedback(umodel);
                    if (umodel.FeedbackId > 0)
                    {
                        TempData["Result"] = "Request approved successfully";
                        TempData["IsSuccess"] = "True";
                        ModelState.Clear();
                        umodel = new NewIntegration();
                        ViewBag.Number = "0";
                        string UserId = HttpContext.Session.GetString("EmpId");
                        CaptureProductivityDetails(sqlCon, UserId, "Appprovalintegration", "API ADDA", 1, "Request approved successfully", "Request sent for EmpCode - " + UserId.ToString().Trim());
                    }
                    else
                    {
                        TempData["IsSuccess"] = "False";
                        TempData["Result"] = "Something Went Wrong..Please try again later";
                    }
                }

                // umodel = submitRepository.GetNewIntegrationDetails(umodel);


            }

            catch (Exception Ex)
            {
                throw;
            }
            //return View(umodel);
            return RedirectToAction("NewIntegrationList", "Home");
        }

        [HttpGet]
        public IActionResult DownloadFile(string fileName)
        {
            try
            {
                int Id = Convert.ToInt32(TempData["integrationId"]);
                string UserId = HttpContext.Session.GetString("EmpId");
                SqlConnection sqlCon = new SqlConnection(Startup.connectionstring);
                string path = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\APIAddaDoc\NewIntegrations\") + HttpContext.Session.GetString("folderName") + "\\" + fileName;
                byte[] bytes = System.IO.File.ReadAllBytes(path);
                CaptureProductivityDetails(sqlCon, UserId, "DownloadFile", "API ADDA", 1, "Download File ", " DownloadFile In for EmpCode - " + UserId.Trim());
                return File(bytes, "application/octet-stream", fileName);
            }
            catch (Exception Ex)
            {
                return NotFound();
            }


        }
        [HttpGet]
        public FileResult DownloadFileOBP(string fileName)
        {
            string filePath = "";
            try
            {
                int Id = Convert.ToInt32(TempData["integrationId"]);
                string UserId = HttpContext.Session.GetString("EmpId");
                SqlConnection sqlCon = new SqlConnection(Startup.connectionstring);
                filePath = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot\APIAddaDoc\NewIntegrations\" + HttpContext.Session.GetString("folderName") + "\\");
                filePath = filePath + fileName;
                byte[] bytes = System.IO.File.ReadAllBytes(filePath);
                CaptureProductivityDetails(sqlCon, UserId, "DownloadFile", "API ADDA", 1, "Download File ", " DownloadFile In for EmpCode - " + UserId.Trim());
                return File(bytes, "application/octet-stream", fileName);
            }

            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost]
        public JsonResult DownloadIntegrationFiles(int IntegrationId)
        {
            string DocName = "";
            try
            {
                int Id = Convert.ToInt32(TempData["integrationId"]);
                DataSet ds = submitRepository.DonwloadIntegrationDoc(IntegrationId);
                DocName = "OBP_Document_" + IntegrationId + ".xlsx";
                Table_Export_IntegratedXL(ds, IntegrationId, DocName);
            }
            catch (Exception ex)
            {
                DocName = "";
                throw;
            }
            //return File(fileBytes, "application/force-download", fileName);
            return new JsonResult(DocName);
        }
        public void Table_Export_IntegratedXL(DataSet ds, int Id, string DocName)
        {
            string filePath = "";
            string FolderfilePath = "";
            try
            {
                string CreatedBy = ds.Tables[0].Rows[0]["CreatedBy"].ToString();
                string ProjectId = ds.Tables[0].Rows[0]["ProjectId"].ToString();
                string ProjectName = ds.Tables[0].Rows[0]["ProjectName"].ToString();
                string CreatedAt = ds.Tables[0].Rows[0]["CreatedAt"].ToString();
                string PlannedGoLiveDate = ds.Tables[0].Rows[0]["PlannedGoLiveDate"].ToString();

                string ProjectSPOC = ds.Tables[0].Rows[0]["ProjectSPOC"].ToString();

                string BusinessJustification = ds.Tables[0].Rows[0]["BusinessJustification"].ToString();

                string BusinessSponsor = ds.Tables[0].Rows[0]["BusinessSponsor"].ToString();

                string ExecutiveSponsor = ds.Tables[0].Rows[0]["ExecutiveSponsor"].ToString();

                string CostCenterCode = ds.Tables[0].Rows[0]["CostCenterCode"].ToString();

                string ObpJeeraID = ds.Tables[0].Rows[0]["OBP_JIRA_ID"].ToString();

                string ApiaddaProjectid = ds.Tables[0].Rows[0]["APIAddaProjectID"].ToString();

                DateTime CreatedDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["CreatedDate"]);

                var dtdata = new DataTable("tblData");

                dtdata.Columns.Add(new DataColumn("Service Details", typeof(string)));

                dtdata.Columns.Add(new DataColumn("Purpose of Service", typeof(string)));

                dtdata.Columns.Add(new DataColumn("Service Name ( If Available )", typeof(string)));

                dtdata.Columns.Add(new DataColumn("Existing/New", typeof(string)));

                dtdata.Columns.Add(new DataColumn("Consumer Application", typeof(string)));

                dtdata.Columns.Add(new DataColumn("Producer Application", typeof(string)));

                dtdata.Columns.Add(new DataColumn("Channel ID to map", typeof(string)));

                dtdata.Columns.Add(new DataColumn("Rest/SOAP", typeof(string)));

                dtdata.Columns.Add(new DataColumn("Transformation , IF any", typeof(string)));

                dtdata.Columns.Add(new DataColumn("Container Name", typeof(string)));

                dtdata.Columns.Add(new DataColumn("Function Domain Name", typeof(string)));

                dtdata.Columns.Add(new DataColumn("Tech Domain Name", typeof(string)));

                dtdata.Columns.Add(new DataColumn("API Category", typeof(string)));

                dtdata.Columns.Add(new DataColumn("API RISK Score", typeof(string)));

                dtdata.Columns.Add(new DataColumn("API RISK Clasification", typeof(string)));

                dtdata.Columns.Add(new DataColumn("Service Type", typeof(string)));

                dtdata.Columns.Add(new DataColumn("API Type", typeof(string)));

                dtdata.Columns.Add(new DataColumn("Volume TPS", typeof(string)));

                dtdata.Columns.Add(new DataColumn("Middleware Name", typeof(string)));

                for (var i = 0; i < ds.Tables[1].Rows.Count; i++)

                {

                    var row = dtdata.NewRow();

                    row[0] = ds.Tables[1].Rows[i]["ServiceDetails"].ToString();

                    row[1] = ds.Tables[1].Rows[i]["PurposeofService"].ToString();

                    row[2] = ds.Tables[1].Rows[i]["ServiceName"].ToString();

                    row[3] = ds.Tables[1].Rows[i]["Existing_New"].ToString();

                    row[4] = ds.Tables[1].Rows[i]["ConsumerApplication"].ToString();

                    row[5] = ds.Tables[1].Rows[i]["ProducerApplication"].ToString();

                    row[6] = ds.Tables[1].Rows[i]["ChannelID"].ToString();

                    row[7] = ds.Tables[1].Rows[i]["Rest_SOAP"].ToString();

                    row[8] = ds.Tables[1].Rows[i]["Transformation"].ToString();

                    row[9] = ds.Tables[1].Rows[i]["ContainerName"].ToString();

                    row[10] = ds.Tables[1].Rows[i]["FunctionDomainName"].ToString();

                    row[11] = ds.Tables[1].Rows[i]["TechDomainName"].ToString();

                    row[12] = ds.Tables[1].Rows[i]["APICategory"].ToString();

                    row[13] = ds.Tables[1].Rows[i]["APIRISKScore"].ToString();

                    row[14] = ds.Tables[1].Rows[i]["APIRISKClasification"].ToString();

                    row[15] = ds.Tables[1].Rows[i]["ServiceType"].ToString();

                    row[16] = ds.Tables[1].Rows[i]["APIType"].ToString();

                    row[17] = ds.Tables[1].Rows[i]["VolumeTPS"].ToString();

                    row[18] = ds.Tables[1].Rows[i]["MiddlewareName"].ToString();

                    dtdata.Rows.Add(row);

                }

                //TempData["FolderName"] = "API" + CreatedDate.ToString("ddMMMyyyy") + "" + Id + "";
                //HttpContext.Session.SetString("FolderName", "API" + CreatedDate.ToString("ddMMMyyyy") + "" + Id + "");

                FolderfilePath = Path.Combine(Directory.GetCurrentDirectory() + @"\wwwroot\APIAddaDoc\NewIntegrations\" + "API" + CreatedDate.ToString("ddMMMyyyy") + "" + Id + "" + "\\");

                filePath = FolderfilePath + DocName;

                var existingFile = new FileInfo(filePath);

                if (existingFile.Exists)

                {

                    existingFile.Delete();

                }

                if (!Directory.Exists(FolderfilePath))

                {

                    System.IO.Directory.CreateDirectory(FolderfilePath);

                }

                using (var package = new ExcelPackage(existingFile))

                {

                    var ws = package.Workbook.Worksheets.Add("Integration Details_" + Id + "");

                    //ws.Cells["A1"].Value = "Product Statistics";

                    //ws.Cells[1, 1, 1, 6].Merge = true;

                    ws.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    ws.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.MidnightBlue);

                    ws.Cells[1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    ws.Cells[1, 2].Style.Fill.BackgroundColor.SetColor(Color.MidnightBlue);

                    ws.Cells[1, 1].Value = "Purpose of this Document";

                    ws.Column(1).Width = 30;

                    ws.Cells[1, 1].Style.Font.Bold = true;

                    ws.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells[1, 2].Value = "This document is mandatory to log project in OBP OR SOA. It will be considered as RD for OBP/SOA and will be referred in FS creation.\n It is used for Architecture signoff for any new service Integration.\n Please Attach singed document in FPN to avoid any delay in FPN Approval";

                    ws.Cells[1, 2].Style.Font.Bold = true;

                    ws.Cells[1, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Row(1).Height = 62;

                    ws.Cells[1, 2, 1, 15].Merge = true;



                    ws.Cells[2, 1].Value = "Preparation by";

                    ws.Cells[2, 1].Style.Font.Bold = true;

                    ws.Cells[2, 2, 2, 15].Merge = true;

                    ws.Cells[2, 2].Value = CreatedBy;

                    ws.Cells[3, 1].Value = "Project ID";

                    ws.Cells[3, 1].Style.Font.Bold = true;

                    ws.Cells[3, 2].Value = ProjectId;

                    ws.Cells[3, 2, 3, 15].Merge = true;

                    ws.Cells[4, 1].Value = "Project Name";

                    ws.Cells[4, 1].Style.Font.Bold = true;

                    ws.Cells[4, 2].Value = ProjectName;

                    ws.Cells[4, 2, 4, 15].Merge = true;

                    ws.Cells[5, 1].Value = "Project Logged Date";

                    ws.Cells[5, 1].Style.Font.Bold = true;

                    ws.Cells[5, 2].Value = CreatedAt;

                    ws.Cells[5, 2, 5, 15].Merge = true;

                    ws.Cells[6, 1].Value = "Planned Go Live Date";

                    ws.Cells[6, 1].Style.Font.Bold = true;

                    ws.Cells[6, 2].Value = PlannedGoLiveDate;

                    ws.Cells[6, 2, 6, 15].Merge = true;

                    ws.Cells[7, 1].Value = "Project SPOC";

                    ws.Cells[7, 1].Style.Font.Bold = true;

                    ws.Cells[7, 2].Value = ProjectSPOC;

                    ws.Cells[7, 2, 7, 15].Merge = true;

                    ws.Cells[8, 1].Value = "Business Justification / Benefits";

                    ws.Cells[8, 1].Style.Font.Bold = true;

                    ws.Cells[8, 2].Value = BusinessJustification;

                    ws.Cells[8, 2, 8, 15].Merge = true;

                    ws.Cells[9, 1].Value = "Business Sponsor";

                    ws.Cells[9, 1].Style.Font.Bold = true;

                    ws.Cells[9, 2].Value = BusinessSponsor;

                    ws.Cells[9, 2, 9, 15].Merge = true;

                    ws.Cells[10, 1].Value = "Executive Sponsor";

                    ws.Cells[10, 1].Style.Font.Bold = true;

                    ws.Cells[10, 2].Value = ExecutiveSponsor;

                    ws.Cells[10, 2, 10, 15].Merge = true;

                    ws.Cells[11, 1].Value = "Cost Center Code";

                    ws.Cells[11, 1].Style.Font.Bold = true;

                    ws.Cells[11, 2].Value = CostCenterCode;

                    ws.Cells[11, 2, 11, 15].Merge = true;

                    ws.Cells[12, 1].Value = "OBP JIRA ID";

                    ws.Cells[12, 1].Style.Font.Bold = true;

                    ws.Cells[12, 2].Value = ObpJeeraID;

                    ws.Cells[12, 2, 12, 15].Merge = true;

                    ws.Cells[13, 1].Value = "API Adda Project ID";

                    ws.Cells[13, 1].Style.Font.Bold = true;

                    ws.Cells[13, 2].Value = ApiaddaProjectid;

                    ws.Cells[13, 2, 13, 15].Merge = true;

                    ws.Cells[14, 2, 14, 15].Merge = true;



                    //ws.Cells[1, 1, 1, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //ws.Cells[3, 3, 6, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    //ws.Cells[3, 4, 6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;



                    ws.Cells[15, 1].LoadFromDataTable(dtdata, true);

                    ws.Cells[15, 1].Style.Font.Bold = true;

                    ws.Cells[15, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    ws.Cells[15, 1].Style.Fill.BackgroundColor.SetColor(Color.MidnightBlue);

                    ws.Cells[15, 2].Style.Font.Bold = true;

                    ws.Cells[15, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    ws.Cells[15, 2].Style.Fill.BackgroundColor.SetColor(Color.MidnightBlue);

                    ws.Cells[15, 3].Style.Font.Bold = true;

                    ws.Cells[15, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    ws.Cells[15, 3].Style.Fill.BackgroundColor.SetColor(Color.MidnightBlue);

                    ws.Cells[15, 4].Style.Font.Bold = true;

                    ws.Cells[15, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    ws.Cells[15, 4].Style.Fill.BackgroundColor.SetColor(Color.MidnightBlue);

                    ws.Cells[15, 5].Style.Font.Bold = true;

                    ws.Cells[15, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    ws.Cells[15, 5].Style.Fill.BackgroundColor.SetColor(Color.MidnightBlue);

                    ws.Cells[15, 6].Style.Font.Bold = true;

                    ws.Cells[15, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    ws.Cells[15, 6].Style.Fill.BackgroundColor.SetColor(Color.MidnightBlue);

                    ws.Cells[15, 7].Style.Font.Bold = true;

                    ws.Cells[15, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    ws.Cells[15, 7].Style.Fill.BackgroundColor.SetColor(Color.MidnightBlue);

                    ws.Cells[15, 8].Style.Font.Bold = true;

                    ws.Cells[15, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    ws.Cells[15, 8].Style.Fill.BackgroundColor.SetColor(Color.MidnightBlue);

                    ws.Cells[15, 9].Style.Font.Bold = true;

                    ws.Cells[15, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    ws.Cells[15, 9].Style.Fill.BackgroundColor.SetColor(Color.MidnightBlue);

                    ws.Cells[15, 10].Style.Font.Bold = true;

                    ws.Cells[15, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    ws.Cells[15, 10].Style.Fill.BackgroundColor.SetColor(Color.MidnightBlue);

                    ws.Cells[15, 11].Style.Font.Bold = true;

                    ws.Cells[15, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    ws.Cells[15, 11].Style.Fill.BackgroundColor.SetColor(Color.MidnightBlue);

                    ws.Cells[15, 12].Style.Font.Bold = true;

                    ws.Cells[15, 12].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    ws.Cells[15, 12].Style.Fill.BackgroundColor.SetColor(Color.MidnightBlue);

                    ws.Cells[15, 13].Style.Font.Bold = true;

                    ws.Cells[15, 13].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    ws.Cells[15, 13].Style.Fill.BackgroundColor.SetColor(Color.MidnightBlue);

                    ws.Cells[15, 14].Style.Font.Bold = true;

                    ws.Cells[15, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    ws.Cells[15, 14].Style.Fill.BackgroundColor.SetColor(Color.MidnightBlue);

                    ws.Cells[15, 15].Style.Font.Bold = true;

                    ws.Cells[15, 15].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    ws.Cells[15, 15].Style.Fill.BackgroundColor.SetColor(Color.MidnightBlue);

                    ws.Cells[15, 16].Style.Font.Bold = true;

                    ws.Cells[15, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    ws.Cells[15, 16].Style.Fill.BackgroundColor.SetColor(Color.MidnightBlue);

                    ws.Cells[15, 17].Style.Font.Bold = true;

                    ws.Cells[15, 17].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    ws.Cells[15, 17].Style.Fill.BackgroundColor.SetColor(Color.MidnightBlue);

                    ws.Cells[15, 18].Style.Font.Bold = true;

                    ws.Cells[15, 18].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    ws.Cells[15, 18].Style.Fill.BackgroundColor.SetColor(Color.MidnightBlue);

                    ws.Cells[15, 19].Style.Font.Bold = true;

                    ws.Cells[15, 19].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    ws.Cells[15, 19].Style.Fill.BackgroundColor.SetColor(Color.MidnightBlue);

                    int dtCount = ds.Tables[1].Rows.Count + 1;



                    for (var i = 0; i < dtCount; i++)

                    {

                        int j = 15 + i;

                        ws.Cells[j, 2].AutoFitColumns();

                        ws.Cells[j, 3].AutoFitColumns();

                        ws.Cells[j, 4].AutoFitColumns();

                        ws.Cells[j, 5].AutoFitColumns();

                        ws.Cells[j, 6].AutoFitColumns();

                        ws.Cells[j, 7].AutoFitColumns();

                        ws.Cells[j, 8].AutoFitColumns();

                        ws.Cells[j, 9].AutoFitColumns();

                        ws.Cells[j, 10].AutoFitColumns();

                        ws.Cells[j, 11].AutoFitColumns();

                        ws.Cells[j, 12].AutoFitColumns();

                        ws.Cells[j, 13].AutoFitColumns();

                        ws.Cells[j, 14].AutoFitColumns();

                        ws.Cells[j, 15].AutoFitColumns();

                        ws.Cells[j, 16].AutoFitColumns();

                        ws.Cells[j, 17].AutoFitColumns();

                        ws.Cells[j, 18].AutoFitColumns();

                        ws.Cells[j, 19].AutoFitColumns();
                        //ws.Row(2).hei = 5;

                    }

                    package.Save();

                    //var filePatheNew = Path.Combine(FloderexistingFile, DocName);

                    //using (var stream = new FileStream(filePatheNew, FileMode.Create))

                    //{

                    //    await file.CopyToAsync(stream);

                    //}

                }

            }

            catch (Exception ex)

            {

                DocName = "";

            }

        }

        public IActionResult faq()
        {
            string UserId = HttpContext.Session.GetString("EmpId");
            CaptureProductivityDetails(sqlCon, UserId, "FAQ", "API ADDA", 1, "FAQ ", "Faq view  for EmpCode - " + UserId.ToString().Trim());
            return View();
        }
        //[HttpPost]
        //public JsonResult Index(string prefix)
        //{
        //    //Note : you can bind same list from database  
        //    List<TestAPI> ObjList = new List<TestAPI>()
        //    {

        //        new TestAPI {ID=1,ServiceURL="Latur" },
        //        new TestAPI {ID=2,ServiceURL="Mumbai" },
        //        new TestAPI {ID=3,ServiceURL="Pune" },
        //        new TestAPI {ID=4,ServiceURL="Delhi" },
        //        new TestAPI {ID=5,ServiceURL="Dehradun" },
        //        new TestAPI {ID=6,ServiceURL="Noida" },
        //        new TestAPI {ID=7,ServiceURL="New Delhi" }

        //};
        //    //Searching records from list using LINQ query  
        //    //var Name = (from N in ObjList
        //    //            where N.ServiceURL.StartsWith(prefix)
        //    //            select new { N.ServiceURL });
        //    var a = ObjList.Where(x => x.ServiceURL.ToLower().Contains(prefix.ToLower())).Select(x => x.ServiceURL).ToList();


        //    return Json(a);
        //}


        public void RemoveListData(NewIntegration umodel)
        {
            int ServiceID = 0;
            var SelectedServiceDetails = umodel.serviceDetails.Where(x => x.ServiceID == ServiceID).ToList();

            if (SelectedServiceDetails.Count > 0)
            {
                foreach (var item in SelectedServiceDetails)
                {
                    var itemToRemove = umodel.serviceDetails.Single(r => r.ServiceID == item.ServiceID);
                    umodel.serviceDetails.Remove(itemToRemove);
                }
            }
        }

        public void UpdateWorkflowIntegrationservice(NewIntegration umodel)
        {
            try
            {

                //umodel.workflowstatus = workflowstatus;
                umodel.SpName = "sp_APIA_NewAPIIntegration";
                umodel.MethodFlag = "UpdateIntegrationDetails";
                // umodel.UserId = HttpContext.Session.GetString("EmpId");

                umodel = submitRepository.UpdateNewIntegration(umodel);
                umodel.MethodFlag = "UpdateServiceDetails";
                umodel = submitRepository.UpdateServiceDetails(umodel);


                if (umodel.QuestionServiceDetailsList != null)
                {
                    if (umodel.QuestionServiceDetailsList.Count > 0)
                    {
                        umodel.MethodFlag = "QuestionInsert";
                        umodel = submitRepository.InsertQuestion(umodel);
                    }
                }


            }
            catch (Exception)
            {

                throw;
            }


        }

        public ActionResult SetSession()
        {
            HttpContext.Session.SetString("PopupEdit", "AddMode");
            return View("newIntegration");
        }


        [HttpPost]
        public JsonResult AutoFilterExistingServiceName(string prefix)
        {
            NewIntegration ObjNewInt = new NewIntegration();
            ObjNewInt = submitRepository.GetExectingServiceName();
            var a = ObjNewInt.ExistingServiceNameList.Where(x => x.ExServiceName.ToLower().Contains(prefix.ToLower()) || x.ExCodServiceId.ToLower().Contains(prefix.ToLower())).ToList();
            return Json(a);
        }

        public JsonResult GetExistingServiceRecordById(int id)
        {
            try
            {

                //umodel = submitRepository.GetAllDropDownListData(umodel);
                var record = submitRepository.ExistingServiceRecordRepository(id);
                var result = new
                {
                    ProducerApplication = record.serviceDetails[0].ProducerApplication,
                    APITypeId = record.serviceDetails[0].APIType_Id,
                    APICatId = record.serviceDetails[0].APICategory_Id,
                    DomainId = record.serviceDetails[0].DomainName_Id,
                    RestSoapId = record.serviceDetails[0].Rest_SOAP_Id,
                    //ProducerDCId = record.serviceDetails[0].ProducerDC,
                    ServiceTypeId = record.serviceDetails[0].ServiceType_Id,
                    apiRiskScoreId = record.serviceDetails[0].APIRiskScore_Id,
                    ExternalServiceName = record.serviceDetails[0].ExternalServiceName,
                    TotalScore = record.serviceDetails[0].TotalScore,
                    RistClassify = record.serviceDetails[0].RistClassify,



                };
                return Json(result);
            }
            catch (Exception)
            {

                throw;
            }



        }

        [HttpPost]
        public JsonResult GetAutoTestApi(string prefix)
        {
            var a = (dynamic)null;
            TestAPI ObjNewInt = new TestAPI();
            ObjNewInt = submitRepository.GetTestApiurlAuto();
            if (ObjNewInt.TestAPIAutoList != null)
            {
                a = ObjNewInt.TestAPIAutoList.Where(x => x.ServiceURL.ToLower().Contains(prefix.ToLower())).ToList();
            }
            return Json(a);
        }

        public JsonResult GetRequestRecord(string path, string ServiceUrlValue, string apiCategoryValue)
        {
            try
            {
                DataTable dt = new DataTable();
                SearchAPI search_api = new SearchAPI();

                String[] serviceUrlList;
                //search_api.ServiceUrl = ServiceUrlValue;
                //string serviceUrl = "";
                //----------------Final API Test code Start--file Read-------------------------------------------------------------
                if (apiCategoryValue == "SOAP" || apiCategoryValue == "REST" || apiCategoryValue == null || apiCategoryValue == "")
                {
                    serviceUrlList = ServiceUrlValue.Split("/");
                    int countLastServiceUrlValue = serviceUrlList.Count();
                    search_api.ServiceUrl = serviceUrlList[countLastServiceUrlValue - 1].Split('?')[0];
                    search_api.Raw = "No Data Available";
                    if (path != null && path != "")
                    {

                        var basePathdoc = Path.Combine(Directory.GetCurrentDirectory() + "\\Collections");
                        basePathdoc = basePathdoc + "\\" + path + ".json";
                        FileInfo file = new FileInfo(basePathdoc);
                        if (file.Exists)
                        {
                            var myJsonString = System.IO.File.ReadAllText("./Collections/" + path + ".json");
                            var jObject = JObject.Parse(myJsonString);
                            //---------------------------------------------------- Json File read-----------------------------------------------------///

                            foreach (var item in jObject["item"])
                            {
                                if (item["request"] != null)
                                {
                                    var request = item["request"];

                                    if (request != null)
                                    {
                                        var url = request["url"];
                                        if (url != null)
                                        {
                                            var Urlraw = url["raw"].ToString();
                                            String[] UrlList = Urlraw.Split("/");
                                            int countLastUrlValue = UrlList.Count();
                                            Urlraw = UrlList[countLastUrlValue - 1].Split('?', '\n')[0];
                                            if (Urlraw == search_api.ServiceUrl)
                                            {
                                                var body = request["body"];
                                                if (body != null)
                                                {
                                                    string raw = body["raw"].ToString();
                                                    search_api.Raw = raw;
                                                    search_api.ServiceUrl = url["raw"].ToString();

                                                }
                                                break;
                                            }
                                        }
                                    }

                                }
                                else if (item["item"] != null)///-------------------case1-----------when dobule Item
                                {
                                    var items = item["item"];

                                    for (int i = 0; i < items.Count(); i++)
                                    {
                                        if (items[i]["item"] != null)
                                        {
                                            items = items[i]["item"];
                                            i = 0;
                                        }
                                        else
                                        {
                                            var request = items[i]["request"];
                                            if (request != null)
                                            {
                                                var url = request["url"];
                                                if (url != null)
                                                {
                                                    var Urlraw = url["raw"].ToString();
                                                    String[] UrlList = Urlraw.Split("/");
                                                    int countLastUrlValue = UrlList.Count();
                                                    Urlraw = UrlList[countLastUrlValue - 1].Split('?', '\n')[0];
                                                    if (Urlraw == search_api.ServiceUrl)
                                                    {
                                                        var body = request["body"];
                                                        if (body != null)
                                                        {
                                                            string raw = body["raw"].ToString();
                                                            search_api.Raw = raw;
                                                            search_api.ServiceUrl = url["raw"].ToString();
                                                            search_api.ServiceUrl = search_api.ServiceUrl.Replace("{{obp_url}}/", "http://10.226.163.7:8002/");
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //foreach (var itemitem in item["item"])
                                    //{
                                    //    var request = itemitem["request"];
                                    //    if (request != null)
                                    //    {
                                    //        var url = request["url"];
                                    //        if (url != null)
                                    //        {
                                    //            var Urlraw = url["raw"].ToString();
                                    //            String[] UrlList = Urlraw.Split("/");
                                    //            int countLastUrlValue = UrlList.Count();
                                    //            Urlraw = UrlList[countLastUrlValue - 1].Split('?')[0];
                                    //            if (Urlraw == search_api.ServiceUrl)
                                    //            {
                                    //                var body = request["body"];
                                    //                if (body != null)
                                    //                {
                                    //                    string raw = body["raw"].ToString();
                                    //                    search_api.Raw = raw;

                                    //                }
                                    //                break;
                                    //            }
                                    //        }
                                    //    }

                                    //}

                                }

                                if (search_api.Raw != "No Data Available")
                                {
                                    break;
                                }

                            }

                        }


                    }
                    var result = new
                    {
                        Request = search_api.Raw,
                        Response = search_api.response,
                        Url = search_api.ServiceUrl,

                    };
                    return Json(result);
                }
                //----------------Final API Test code End--file Read---------------------------------------------------------------------------

                if (apiCategoryValue != "SOAP")
                {

                    serviceUrlList = ServiceUrlValue.Split("/");
                    int countLastServiceUrlValue = serviceUrlList.Count();
                    search_api.ServiceUrl = serviceUrlList[countLastServiceUrlValue - 1].Split('?')[0];
                    search_api.Raw = "No Data Available";
                    if (path != null && path != "")
                    {
                        // search_api.ServiceUrl = @"https://hbentbpuatap.hdfcbankuat.com:9550/OBPMultiFTWebServices/OnlineTransactionInquiryRestWrapper/doTransactionInquiry";
                        var basePathdoc = Path.Combine(Directory.GetCurrentDirectory() + "\\Collections");
                        basePathdoc = basePathdoc + "\\" + path + ".json";
                        FileInfo file = new FileInfo(basePathdoc);
                        if (file.Exists)
                        {
                            var myJsonString = System.IO.File.ReadAllText("./Collections/" + path + ".json");
                            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Application>(myJsonString);

                            for (int i = 0; i < data.item.Count; i++)
                            {
                                if (data.item[i].request != null)
                                {
                                    if (data.item[i].request.url != null)
                                    {
                                        string url = data.item[i].request.url.raw;


                                        String[] UrlList = url.Split("/");
                                        int countLastUrlValue = UrlList.Count();
                                        url = UrlList[countLastUrlValue - 1];
                                        //if (result == dt.Rows[0]["serviceURL"].ToString())
                                        if (url == search_api.ServiceUrl)
                                        {
                                            string UrlFinal = data.item[i].request.url.raw;
                                            string Name = data.item[i].name;
                                            string raw = data.item[i].request.body.raw;
                                            if (data.item[i].response.Count > 0)
                                            {
                                                string response = data.item[i].response.ToString();
                                                search_api.response = response;
                                            }
                                            else
                                            {
                                                search_api.response = "No Data Available";
                                            }
                                            // ModelState.Clear();
                                            search_api.Name = Name;
                                            search_api.Raw = raw;
                                            search_api.ServiceUrl = UrlFinal;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        //else
                        //{
                        //    search_api.response = " No Data Available";
                        //}

                    }
                    var result = new
                    {
                        Request = search_api.Raw,
                        Response = search_api.response,
                        Url = search_api.ServiceUrl,

                    };
                    return Json(result);
                }
                else
                {
                    serviceUrlList = ServiceUrlValue.Split("/");
                    int countLastServiceUrlValue = serviceUrlList.Count();
                    search_api.ServiceUrl = serviceUrlList[countLastServiceUrlValue - 1].Split('?')[0];


                    search_api.Raw = "No Data Available";
                    if (path != null && path != "")
                    {
                        // search_api.ServiceUrl = @"https://hbentbpuatap.hdfcbankuat.com:9550/OBPMultiFTWebServices/OnlineTransactionInquiryRestWrapper/doTransactionInquiry";
                        var basePathdoc = Path.Combine(Directory.GetCurrentDirectory() + "\\Collections");
                        basePathdoc = basePathdoc + "\\" + path + ".json";
                        FileInfo file = new FileInfo(basePathdoc);
                        if (file.Exists)
                        {
                            var myJsonString = System.IO.File.ReadAllText("./Collections/" + path + ".json");
                            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<SoapApplication>(myJsonString);

                            for (int i = 0; i < data.item.Count; i++)
                            {

                                if (data.item[i] != null)
                                {
                                    for (int j = 0; j < data.item[i].Item.Count; j++)
                                    {
                                        if (data.item[i].Item[j].request != null)
                                        {
                                            string url = data.item[i].Item[j].request.url.raw;
                                            String[] UrlList = url.Split("/");
                                            int countLastUrlValue = UrlList.Count();
                                            url = UrlList[countLastUrlValue - 1];
                                            //if (result == dt.Rows[0]["serviceURL"].ToString())
                                            if (url == search_api.ServiceUrl)
                                            {
                                                string Name = data.item[i].name;
                                                string raw = data.item[i].Item[j].request.body.raw;
                                                search_api.Name = Name;
                                                search_api.Raw = raw;
                                                break;
                                            }
                                        }
                                    }


                                }
                                if (search_api.Name != null)
                                {
                                    break;
                                }

                            }
                        }
                        //else
                        //{
                        //    search_api.response = " No Data Available";
                        //}

                    }
                    var result = new
                    {
                        Request = search_api.Raw,
                        Response = search_api.response,

                    };
                    return Json(result);
                }


                //var record = submitRepository.ExistingServiceRecordRepository(id);



            }
            catch (Exception ex)
            {

                throw;
            }



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

        public void DeletePriviousdocinupdate(NewIntegration umodel)
        {
            try
            {
                //-----------------------when new file upioad ,first old file detete to folder---------------start--------------
                var basePathdoc = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\Files");
                basePathdoc = basePathdoc + "\\" + umodel.UserJourneyFiles;
                FileInfo file = new FileInfo(basePathdoc);
                if (file.Exists)//check file exsit or not  
                {
                    file.Delete();
                    umodel.fileName = string.Empty;
                    umodel.modifyFileName = string.Empty;
                    umodel.UserJourneyFiles = null;

                }
            }
            catch (Exception)
            {

                throw;
            }
        }



        [HttpPost]
        public JsonResult AutoExternalServiceNameFilter(string prefix)
        {
            NewIntegration ObjNewInt = new NewIntegration();
            ObjNewInt = submitRepository.GetExtenalServiceName();
            var a = ObjNewInt.ExternalServiceNameList.Where(x => x.ExternalServicName.ToLower().Contains(prefix.ToLower()) || x.ExternalCodServiceId.ToLower().Contains(prefix.ToLower())).ToList();
            return Json(a);
        }

        public JsonResult GetExternalServiceNameRecordById(string ExternalServiceText)
        {

            try
            {

                //umodel = submitRepository.GetAllDropDownListData(umodel)
                string resultdefultval = null;

                var record = submitRepository.ExtenalServiceRecordRepository(ExternalServiceText);
                if (record != null)
                {
                    var result = new
                    {
                        InternalServiceName = record.serviceDetails[0].InternalServiceName,
                        ProducerApplication = record.serviceDetails[0].ProducerApplication,
                        APITypeId = record.serviceDetails[0].APIType_Id,
                        APICatId = record.serviceDetails[0].APICategory_Id,
                        DomainId = record.serviceDetails[0].DomainName_Id,
                        RestSoapId = record.serviceDetails[0].Rest_SOAP_Id,
                        ProducerDCId = record.serviceDetails[0].ProducerDC,
                        ServiceTypeId = record.serviceDetails[0].ServiceType_Id,
                        apiRiskScoreId = record.serviceDetails[0].APIRiskScore_Id,
                        TotalScore = record.serviceDetails[0].TotalScore,
                        RistClassify = record.serviceDetails[0].RistClassify,


                    };
                    return Json(result);
                }
                return Json(resultdefultval);
            }
            catch (Exception)
            {

                throw;
            }



        }


        //--------------------------------------------------------Producer and consumer app auto fill----------------------------------

        [HttpPost]
        public JsonResult AutoProducerAppFilter(string prefix)
        {
            NewIntegration ObjNewInt = new NewIntegration();
            ObjNewInt = submitRepository.GetProducerName();
            var a = ObjNewInt.ProducerAppList.Where(x => x.ProducerName.ToLower().Contains(prefix.ToLower()) || x.ITGRCCodeProducer.ToLower().Contains(prefix.ToLower())).ToList();
            return Json(a);
        }

        public JsonResult GetProducerAppId(int Id)
        {

            try
            {

                //umodel = submitRepository.GetAllDropDownListData(umodel)
                string resultdefultval = null;

                var record = submitRepository.ProducerRecordRepository(Id);
                if (record != null)
                {
                    var result = new
                    {
                        ProducerDC = record.serviceDetails[0].ProducerDC,
                    };
                    return Json(result);
                }
                return Json(resultdefultval);
            }
            catch (Exception)
            {

                throw;
            }



        }


        public JsonResult GetProducerAppfullName(string FullName)
        {

            try
            {

                //umodel = submitRepository.GetAllDropDownListData(umodel)
                string resultdefultval = null;

                var record = submitRepository.ProducerRecordTextRepository(FullName);
                if (record != null)
                {
                    var result = new
                    {
                        ProducerDC = record.serviceDetails[0].ProducerDC,
                    };
                    return Json(result);
                }
                return Json(resultdefultval);
            }
            catch (Exception)
            {

                throw;
            }



        }


        [HttpPost]
        public JsonResult AutoConsumerAppFilter(string prefix)
        {
            NewIntegration ObjNewInt = new NewIntegration();
            ObjNewInt = submitRepository.GetCosumerName();
            var a = ObjNewInt.ConsumerAppList.Where(x => x.ConsumerName.ToLower().Contains(prefix.ToLower()) || x.ITGRCCodeConsumer.ToLower().Contains(prefix.ToLower())).ToList();
            return Json(a);
        }

        public JsonResult GetConsumerAppId(int Id)
        {

            try
            {
                string resultdefultval = null;
                var record = submitRepository.CosumerRecordRepository(Id);
                if (record != null)
                {
                    var result = new
                    {
                        ConsumerDC = record.serviceDetails[0].ConsumerDC,

                    };
                    return Json(result);
                }
                return Json(resultdefultval);
            }
            catch (Exception)
            {

                throw;
            }



        }

        public JsonResult GetConsumerAppText(string ConsumerApptext)
        {

            try
            {
                string resultdefultval = null;
                var record = submitRepository.CosumerRecordTextRepositoryT(ConsumerApptext);
                if (record != null)
                {
                    var result = new
                    {
                        ConsumerDC = record.serviceDetails[0].ConsumerDC,

                    };
                    return Json(result);
                }
                return Json(resultdefultval);
            }
            catch (Exception)
            {

                throw;
            }



        }

        public IActionResult DisplayFile(string fileName)
        {

            string filePath = Path.Combine(fileName);
            int pos = fileName.LastIndexOf(".") + 1;
            int pos1 = fileName.LastIndexOf("\\") + 1;
            var extension = fileName.Substring(pos, fileName.Length - pos);
            var diplayFilename = fileName.Substring(pos1, fileName.Length - pos1);
            extension = "." + extension;

            if (System.IO.File.Exists(filePath))
            {
                //byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                //string contentType = GetContentType(extension);
                //return File(fileBytes, contentType, diplayFilename);
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                if (extension == ".docx" || extension == ".xlsx"/* || extension == ".txt" */|| extension == ".xls" || extension == ".zip" || extension == ".7z" || extension == ".doc")
                {
                    return File(fileBytes, "application/octet-stream", diplayFilename);
                }
                else
                {
                    string contentType = GetContentType(extension);
                    return File(fileBytes, contentType);
                }
            }
            else
            {
                return NotFound();
            }
        }
        private string GetContentType(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".png":
                    return "image/png";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".gif":
                    return "image/gif";
                case ".pdf":
                    return "application/pdf";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".txt":
                    return "text/plain";
                case ".zip":
                    return "application/x-zip-compressed";
                case ".7z":
                    return "application/x-7z-compressed";
                case ".doc":
                    return "application/msword";
                case ".xls":
                    return "application/vnd.ms-excel";
                default:
                    return "application/octet-stream"; // Default to binary data for other file types
            }
        }
        [HttpPost]
        public JsonResult GetQuestonDetail(NewIntegration umodel)
        {
            try
            {
                // newIntegration  umodel = submitRepository.GetQuestionList(umodel, ServiceID);
                umodel = submitRepository.GetQuestionListInNewchange(umodel);
                //var test = inplatform;
                List<Questinonselected> listQuestionServiceDetails = new List<Questinonselected>();

                var QuestionLists = umodel.QuestionList.Where(x => x.QType == umodel.IN_Platform).ToList();
                if (umodel.IN_Platform.Trim() != "Internal")
                {
                    QuestionLists = umodel.QuestionList.Where(x => x.QType == umodel.IN_Platform.Trim() && x.APICategoryId == umodel.serviceDetails1.APICategory_Id).ToList();
                }
                if (umodel.QuestinonselectedList.Count == 0)
                {
                    for (int i = 0; i < QuestionLists.Count; i++)
                    {
                        var selecedList = umodel.QDropValuesList.Where(x => x.QID == QuestionLists[i].ID).ToList();
                        for (int j = 0; j < selecedList.Count; j++)
                        {
                            //umodel.QuestinonselectedList[j].QID = selecedList[j].QID;
                            //umodel.QuestinonselectedList[j].QuesOptionsID = selecedList[j].ID;
                            //umodel.QuestinonselectedList[j].Weightage = selecedList[j].Weightage;
                            //umodel.QuestinonselectedList[j].Val = selecedList[j].Val;
                            //umodel.QuestinonselectedList[j].QusServiceID = umodel.serviceDetails1.ServiceID;

                            Questinonselected questionService = new Questinonselected();
                            questionService.QID = selecedList[j].QID;
                            questionService.QuesOptionsID = selecedList[j].ID;
                            questionService.QusServiceID = umodel.serviceDetails1.ServiceID;
                            questionService.Weightage = selecedList[j].Weightage;
                            questionService.Val = selecedList[j].Val;

                            listQuestionServiceDetails.Add(questionService);
                        }



                    }
                    umodel.QuestinonselectedList = listQuestionServiceDetails;
                }
                //else
                //{

                //    //for (int i = 0; i < umodel.QuestinonselectedList.Count; i++)
                //    //{
                //    //    var selecedList = umodel.QDropValuesList.Where(x => x.Weightage == umodel.QuestinonselectedList[i].Weightage && x.QID == QuestionLists[i].ID).ToList();
                //    //    umodel.QuestinonselectedList[i].QID = selecedList[0].QID;
                //    //    umodel.QuestinonselectedList[i].QuesOptionsID = selecedList[0].ID;
                //    //    umodel.QuestinonselectedList[i].Weightage = selecedList[0].Weightage;
                //    //    umodel.QuestinonselectedList[i].Val = selecedList[0].Val;
                //    //    umodel.QuestinonselectedList[i].QusServiceID = umodel.serviceDetails1.ServiceID;

                //    //    QuestionServiceDetails questionService = new QuestionServiceDetails();
                //    //    questionService.QID = umodel.QuestinonselectedList[i].QID;
                //    //    questionService.QuesOptionsID = umodel.QuestinonselectedList[i].QuesOptionsID;
                //    //    questionService.QusServiceID = umodel.serviceDetails1.ServiceID;
                //    //    questionService.Weightage = umodel.QuestinonselectedList[i].Weightage;
                //    //    questionService.Val = umodel.QuestinonselectedList[i].Val;

                //    //    listQuestionServiceDetails.Add(questionService);

                //    //    //umodel.QuestionServiceDetailsList.Add(new QuestionServiceDetails()
                //    //    //{
                //    //    //    QID = umodel.QuestinonselectedList[i].QusId,
                //    //    //    QuesOptionsID = umodel.QuestinonselectedList[i].QusChidId,
                //    //    //    QusServiceID = umodel.serviceDetails1.ServiceID,

                //    //    //});
                //    //}
                //}


                //newIntegration = Adminrepository.UserMasterEdit(data);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return new JsonResult(umodel);
        }

        public IActionResult DownloadZip(string zipFolderName)
        {
            try
            {
                string path = zipFolderName;
                int pos = path.LastIndexOf("/") + 1;
                zipFolderName = path.Substring(pos, path.Length - pos);

                //string filename = zipFolderName.Split('.').Last();
                string FolderPath = HttpContext.Session.GetString("ServiceDocumentFilePath");
                // string directoryPath = Path.GetDirectoryName(FolderPath.Replace("E:", "D:"));
                string directoryPath = Path.GetDirectoryName(FolderPath);
                if (Directory.Exists(directoryPath))
                {
                    string zipFileName = $"{zipFolderName}_{DateTime.Now:ddMMMyyyy}.zip";
                    //string zipFileName = $"{zipFolderName}_{DateTime.Now:ddMMMyyyyHHmmss}.zip";
                    string zipFilePath = Path.Combine(Path.GetTempPath(), zipFileName);
                    using (var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                    {
                        foreach (var filePath in Directory.GetFiles(directoryPath))
                        {
                            var entry = zipArchive.CreateEntry(Path.GetFileName(filePath));
                            using (var entryStream = entry.Open())
                            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                            {
                                fileStream.CopyTo(entryStream);
                            }
                        }
                    }
                    byte[] zipBytes = System.IO.File.ReadAllBytes(zipFilePath);
                    System.IO.File.Delete(zipFilePath);

                    return File(zipBytes, "application/zip", zipFileName);
                }
                else
                {
                    return NotFound("Folder Not Found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost]
        public JsonResult GetBTGPorjectMngr(string txt_BTGmngrname)
        {
            NewIntegration newIntegration = new NewIntegration();
            newIntegration = submitRepository.GetBTGProjectmgr();
            var btgNM = newIntegration.lstBTGProjectmngr.Where(x => x.ProjectMgrBTG.ToLower().Contains(txt_BTGmngrname.ToLower())).ToList();
            return Json(btgNM);
        }

        public void SetFilePath(NewIntegration newIntegration)
        {
            var FolderName = "API" + newIntegration.CreatedAt.ToString("ddMMMyyyy") + (newIntegration.ParentIntegrationId == null || newIntegration.ParentIntegrationId == 0 ? newIntegration.IntegrationId : newIntegration.ParentIntegrationId) + @"\";
            TempData["APIAddaId"] = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\APIAddaDoc\NewIntegrations\") + FolderName;
        }

        [HttpPost]
        public IActionResult UploadFile(string fileName, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Content("File not selected or empty");
            }
            // FTP server details
            var ftpCredential = submitRepository.GetFTPCredential();
            string ftpServer = ftpCredential.HostName;
            string ftpUsername = ftpCredential.UserName;
            string ftpPassword = ftpCredential.Password;

            // Full FTP path (including remote directory and filename)
            string ftpPath = $"{ftpServer}{fileName}";

            try
            {
                // Create FTP request
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(ftpPath);
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpRequest.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                ftpRequest.UseBinary = true;

                // Copy file contents to the FTP request stream
                using (Stream ftpStream = ftpRequest.GetRequestStream())
                {
                    file.CopyTo(ftpStream);
                }

                // Get FTP response
                FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                string uploadStatus = ftpResponse.StatusDescription;

                // Dispose of the FTP response
                ftpResponse.Close();

                return Content($"File uploaded successfully. Status: {uploadStatus}");
            }
            catch (Exception ex)
            {
                return Content($"Error uploading file: {ex.Message}");
            }
        }
    }
}

