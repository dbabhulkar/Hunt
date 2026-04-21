using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using API_HUNT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API_HUNT.Controllers
{
    public class JIRACreatorController : Controller
    {
        private readonly ISubmitRepository submitRepository;
        private readonly IJiraRepository jIRARepository;
        private readonly IActivityLogRepository _activityLog;
        private readonly HomeController _homeController;
        private readonly HttpClient _httpClient;
        public JIRACreatorController(HttpClient httpClient, ISubmitRepository submitRepo, IJiraRepository jiraRepo, IActivityLogRepository activityLog, HomeController homeControllerDep, IConfiguration configuration)
        {
            _httpClient = httpClient;
            submitRepository = submitRepo;
            jIRARepository = jiraRepo;
            _activityLog = activityLog;
            _homeController = homeControllerDep;

            // Set up Basic Authentication from configuration
            var username = configuration["JiraSettings:Username"] ?? string.Empty;
            var password = configuration["JiraSettings:Password"] ?? string.Empty;
            var basicAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", basicAuth);
        }

        public async Task<NewIntegration> CreateJIRA(NewIntegration newIntegration)
        {
            // API endpoint URL
            var apiUrl = "https://jira.hunt.com/rest/api/2/issue";

            //newIntegration = submitRepository.GetNewIntegrationById(newIntegration.IntegrationId);

            if (newIntegration.IN_Platform == "External")
            {
                if (newIntegration.serviceDetails != null)
                {
                    bool gwExistingIdProcessed = false;
                    bool gwNewIdProcessed = false;
                    foreach (var item in newIntegration.serviceDetails)
                    {
                        if (item.Existing_New_Id == 1 && item.Platform != "Internal" && !gwExistingIdProcessed) //1 Existing_New_Id is for Existing
                        {
                            gwExistingIdProcessed = true;
                            string ProdApp1 = string.Join(Environment.NewLine, newIntegration.serviceDetails.Where(x => x.Existing_New_Id == 1 && x.Platform != "Internal").Select((x, index) => $"{index + 1}. {x.ExternalServiceName}"));
                            int ExistingCount = newIntegration.serviceDetails.Count(s => s.Existing_New_Id == 1);
                            var PayloadForAPIINT = jIRARepository.PayloadForAPIINT(newIntegration, item.ProducerApplication, ExistingCount, ProdApp1);

                            var jsonPayload = JsonConvert.SerializeObject(PayloadForAPIINT);

                            // Create a StringContent with JSON payload
                            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var response = await _httpClient.PostAsync(apiUrl, content);

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Read the response content (usually contains the API response)
                                var responseContent = await response.Content.ReadAsStringAsync();
                                // Parse the JSON response using Newtonsoft.Json
                                var jsonResponse = JsonConvert.DeserializeObject<JIRAResponseModel>(responseContent);
                                // Extract the value of the "Key" field
                                var key = jsonResponse.Key.ToString();
                                jIRARepository.InsertJIRAId(item.ServiceID, key);
                                await JIRADocUpload(newIntegration, key);
                            }
                            else
                            {
                                // Handle the error, for example, log or return an error view
                                var errorMessage = $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                            }
                        }
                        if (item.Existing_New_Id == 2 && item.Platform != "Internal" && !gwNewIdProcessed) //2 Existing_New_Id is for New
                        {
                            gwNewIdProcessed = true;
                            string ProdApp2 = string.Join(Environment.NewLine, newIntegration.serviceDetails.Where(x => x.Existing_New_Id == 2 && x.Platform != "Internal").Select((x, index) => $"{index + 1}. {x.ExternalServiceName}"));
                            int NewCount = newIntegration.serviceDetails.Count(s => s.Existing_New_Id == 2);
                            var PayloadForAPIGATE = jIRARepository.PayloadForAPIGATE(newIntegration, item.ProducerApplication, NewCount, ProdApp2);

                            var jsonPayload = JsonConvert.SerializeObject(PayloadForAPIGATE);

                            // Create a StringContent with JSON payload
                            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var response = await _httpClient.PostAsync(apiUrl, content);

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Read the response content (usually contains the API response)
                                var responseContent = await response.Content.ReadAsStringAsync();
                                // Parse the JSON response using Newtonsoft.Json
                                var jsonResponse = JsonConvert.DeserializeObject<JIRAResponseModel>(responseContent);

                                // Extract the value of the "Key" field
                                var key = jsonResponse.Key.ToString();
                                jIRARepository.InsertJIRAId(item.ServiceID, key);
                                await JIRADocUpload(newIntegration, key);
                            }
                            else
                            {
                                // Handle the error, for example, log or return an error view
                                var errorMessage = $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                            }
                        }
                    }
                }
            }
            else if (newIntegration.IN_Platform == "Internal")
            {
                if (newIntegration.serviceDetails != null)
                {
                    bool obpExistingIdProcessed = false;
                    bool obpNewIdProcessed = false;
                    bool soaProcessed = false;
                    foreach (var item in newIntegration.serviceDetails)
                    {
                        if (item.MiddlewareNameId == 82 && !soaProcessed) //82 MiddlewareNameId is for SOA
                        {
                            soaProcessed = true;
                            string ProdApp3 = string.Join(", ", newIntegration.serviceDetails.Where(x => x.MiddlewareNameId == 82).Select((x, index) => $"{index + 1}. {x.InternalServiceName}"));

                            int SOACount = newIntegration.serviceDetails.Count(s => s.MiddlewareNameId == 82);
                            var PayloadForAPIGATE = jIRARepository.PayloadForSOAENHN(newIntegration, item.ProducerApplication, item.ServiceID, SOACount, ProdApp3);

                            var jsonPayload = JsonConvert.SerializeObject(PayloadForAPIGATE);

                            // Create a StringContent with JSON payload
                            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var response = await _httpClient.PostAsync(apiUrl, content);

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Read the response content (usually contains the API response)
                                var responseContent = await response.Content.ReadAsStringAsync();
                                // Parse the JSON response using Newtonsoft.Json
                                var jsonResponse = JsonConvert.DeserializeObject<JIRAResponseModel>(responseContent);

                                // Extract the value of the "Key" field
                                var key = jsonResponse.Key.ToString();
                                jIRARepository.InsertJIRAId(item.ServiceID, key);
                                await JIRADocUpload(newIntegration, key);
                            }
                            else
                            {
                                // Handle the error, for example, log or return an error view
                                var errorMessage = $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                            }
                        }

                        if (item.MiddlewareNameId == 83 && item.Existing_New_Id == 1 && !obpExistingIdProcessed) //83 MiddlewareNameId is for OBP
                        {
                            obpExistingIdProcessed = true;
                            string ProdApp4 = string.Join(Environment.NewLine, newIntegration.serviceDetails.Where(x => x.MiddlewareNameId == 83 && x.Existing_New_Id == 1).Select((x, index) => $"{index + 1}. {x.InternalServiceName}"));

                            int ExistingCount = newIntegration.serviceDetails.Count(s => s.MiddlewareNameId == 83 && s.Existing_New_Id == 1);
                            var PayloadForAPIGATE = jIRARepository.PayloadForOBPINT(newIntegration, item.ProducerApplication, item.ServiceID, ExistingCount, ProdApp4);

                            var jsonPayload = JsonConvert.SerializeObject(PayloadForAPIGATE);

                            // Create a StringContent with JSON payload
                            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var response = await _httpClient.PostAsync(apiUrl, content);

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Read the response content (usually contains the API response)
                                var responseContent = await response.Content.ReadAsStringAsync();
                                // Parse the JSON response using Newtonsoft.Json
                                var jsonResponse = JsonConvert.DeserializeObject<JIRAResponseModel>(responseContent);

                                // Extract the value of the "Key" field
                                var key = jsonResponse.Key.ToString();
                                jIRARepository.InsertJIRAId(item.ServiceID, key);
                                await JIRADocUpload(newIntegration, key);
                            }
                            else
                            {
                                // Handle the error, for example, log or return an error view
                                var errorMessage = $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                            }
                        }
                        if (item.MiddlewareNameId == 83 && item.Existing_New_Id == 2 && !obpNewIdProcessed) //83 MiddlewareNameId is for OBP
                        {
                            obpNewIdProcessed = true;
                            string ProdApp5 = string.Join(Environment.NewLine, newIntegration.serviceDetails.Where(x => x.MiddlewareNameId == 83 && x.Existing_New_Id == 2).Select((x, index) => $"{index + 1}. {x.InternalServiceName}"));

                            int NewCount = newIntegration.serviceDetails.Count(s => s.MiddlewareNameId == 83 && s.Existing_New_Id == 2);
                            var PayloadForAPIGATE = jIRARepository.PayloadForOBPENH(newIntegration, item.ProducerApplication, item.ServiceID, NewCount, ProdApp5);

                            var jsonPayload = JsonConvert.SerializeObject(PayloadForAPIGATE);

                            // Create a StringContent with JSON payload
                            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var response = await _httpClient.PostAsync(apiUrl, content);

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Read the response content (usually contains the API response)
                                var responseContent = await response.Content.ReadAsStringAsync();
                                // Parse the JSON response using Newtonsoft.Json
                                var jsonResponse = JsonConvert.DeserializeObject<JIRAResponseModel>(responseContent);

                                // Extract the value of the "Key" field
                                var key = jsonResponse.Key.ToString();
                                jIRARepository.InsertJIRAId(item.ServiceID, key);
                                await JIRADocUpload(newIntegration, key);
                            }
                            else
                            {
                                // Handle the error, for example, log or return an error view
                                var errorMessage = $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                            }
                        }
                    }
                }
            }
            return newIntegration;
        }
        //public async Task UploadZip(string jiraId, string folderName)
        //{
        //    var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIHuntDoc\\NewIntegrations\\" + folderName + "\\");
        //    string apiUrl = "https://jira.hunt.com/rest/api/2/issue/" + jiraId + "/attachments";

        //    // Create a memory stream to hold the zip file
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        // Create a new zip file in the memory stream
        //        using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        //        {
        //            // Get all files in the folder
        //            foreach (string filePath in Directory.GetFiles(basePath))
        //            {
        //                // Add each file to the zip file
        //                archive.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
        //            }
        //        }

        //        // Convert the memory stream to a byte array
        //        byte[] zipBytes = memoryStream.ToArray();

        //        // Convert the byte array to a base64-encoded string
        //        string base64Zip = Convert.ToBase64String(zipBytes);

        //        // Make a POST request to the API endpoint
        //        using (HttpClient client = new HttpClient())
        //        {
        //            // Create a JSON payload with the base64-encoded zip data
        //            string jsonPayload = $"{{\"zippedData\": \"{base64Zip}\"}}";
        //            StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        //            // Make the POST request
        //            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

        //            // Check the response status
        //            if (response.IsSuccessStatusCode)
        //            {
        //                Console.WriteLine("Zipped folder sent successfully!");
        //            }
        //            else
        //            {
        //                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
        //            }
        //        }
        //    }
        //}

        public async Task<string> JIRADocUpload(NewIntegration newIntegration, string jiraId)
        {
            string result = string.Empty;

            // API endpoint URL
            string apiUrl = "https://jira.hunt.com/rest/api/2/issue/" + jiraId + "/attachments";

            // folder name
            string folderName = "API" + newIntegration.CreatedAt.ToString("ddMMMyyyy") + newIntegration.IntegrationId;
            // folder path
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\APIHuntDoc\NewIntegrations\" + folderName);

            // Check if the folder exists
            if (!Directory.Exists(folderPath))
            {
                return "Folder not found";
            }
            // Get OBP document
            DataSet ds = submitRepository.DonwloadIntegrationDoc(newIntegration.IntegrationId);
            string DocName = "OBP_Document_" + newIntegration.IntegrationId + ".xlsx";
            _homeController.Table_Export_IntegratedXL(ds, newIntegration.IntegrationId, DocName);

            // Get a list of files in the folder
            var files = Directory.GetFiles(folderPath);

            // Convert the file paths to IFormFile
            var formFiles = files.Select(filePath =>
            {
                var fileName = Path.GetFileName(filePath);
                var stream = new FileStream(filePath, FileMode.Open);
                return new FormFile(stream, 0, stream.Length, null, fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "application/octet-stream"
                };
            }).ToList();


            // Create multipart/form-data content for file upload
            var content = new MultipartFormDataContent();
            // Add each file to the content
            foreach (var file in formFiles)
            {
                content.Add(new StreamContent(file.OpenReadStream()), "file", file.FileName);
            }

            // Add additional headers for the second API
            _httpClient.DefaultRequestHeaders.Add("X-Atlassian-Token", "no-check");

            // Send POST request
            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                // Read the response content (usually contains the API response)
                result = await response.Content.ReadAsStringAsync();
            }
            else
            {
                // Handle the error, for example, log or return an error view
                var errorMessage = $"Error: {response.StatusCode} - {response.ReasonPhrase}";
            }
            return result;
        }
    }
}
