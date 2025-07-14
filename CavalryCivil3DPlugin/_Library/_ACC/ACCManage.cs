using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using CavalryCivil3DPlugin.Consoles;
using Autodesk.AutoCAD.Internal.Reactors;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Security.Policy;

namespace CavalryCivil3DPlugin._Library._ACC
{
    public class ACCManage
    {

        private string _ClientId;
        public string ClientId => _ClientId;

        private string _ClientSecret;
        public string ClientSecret => _ClientSecret;

        private string _AccountId;
        public string AccountId => _AccountId;

        private string _AccessToken;
        public string AccessToken => _AccessToken;

        private List<string> _Projects;
        public List<string> Projects => _Projects;

        private List<string> _Forms;
        public List<string> Forms => _Forms;

        private HttpClient _HttpClient;
        public HttpClient HttpCLient_ => _HttpClient;

        private string _ProjectName;
        public string ProjectName => _ProjectName;

        private string _ProjectId;
        public string ProjectId => _ProjectId;

        public StaticConsole Console { get; set; }



        public ACCManage(string _clientID, string _clientSecret, string _accounttId, string _projectName=null)
        {
            _ClientId = _clientID;
            _ClientSecret = _clientSecret;
            _AccountId = _accounttId;
            Console = new StaticConsole();
            _HttpClient = new HttpClient();
            _ProjectName = _projectName;
        }


        public async Task Initialize()
        {
            //_Console.ShowConsole("Retrieving Token...");
            Console.Print("Retrieving Token...");
            var accessToken = await GetAccessToken();
            Console.Print("Token Successfully Retrieved");
        }


        private async Task<string> GetAccessToken()
        {
            //{"scope", "data:read data:write account:read"}
            var formParams = new Dictionary<string, string>
            {
                {"client_id", _ClientId},
                {"client_secret", _ClientSecret},
                {"grant_type", "authorization_code"},
                {"scope", "data:read data:write account:read"}
            };
            //hj

            var formContent = new FormUrlEncodedContent(formParams);
            var response = await _HttpClient.PostAsync("https://developer.api.autodesk.com/authentication/v2/token", formContent);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var doc = Newtonsoft.Json.Linq.JObject.Parse(responseContent);
                string accessToken = (string)doc["access_token"];
                _AccessToken = accessToken;
                return accessToken;
            }

            return null; 
        }






        public async Task GetProjects()
        {
            Console.Print("Getting Projects...");
            int offset = 0;
            const int pageSize = 10;
            _Projects = new List<string>();


            
            while (true)
            {
                string url = $"https://developer.api.autodesk.com/hq/v1/accounts/{_AccountId}/projects?offset={offset}";
                _HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _AccessToken);
                HttpResponseMessage response = await _HttpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    JArray projects = JArray.Parse(content);

                    if (projects.Count == 0) break;
      
                    foreach (var project in projects)
                    {
                        string status = (string)project["status"];
                        if (status != "active")
                            continue;

                        string projectId = (string)project["id"];
                        string projectName = (string)project["name"];
                        _Projects.Add($"Project: {projectName}");

                        if (_ProjectName != null && ProjectName == projectName)
                        {
                            _ProjectId = projectId;
                        }
                    }
                }

                else
                {
                    _Console.ShowConsole($"Failed to get projects: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    break;
                }

                offset += pageSize;
            } 
        }


        public async Task GetForms()
        {
            if (_ProjectId != null)
            {
                Console.Print("Getting Forms...");
                Console.Print(_ProjectId);
                _Forms = new List<string>();

                string formsEndPoint = $"https://developer.api.autodesk.com/construction/forms/v1/projects/{_ProjectId}/form-templates";
                _HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _AccessToken);
                HttpResponseMessage response = await _HttpClient.GetAsync(formsEndPoint);

                Console.Print($"Access Form Templates: {response.StatusCode}");
            }

            else
            {
                Console.Print("Project not found.");
            }
            
        }

    }
}
