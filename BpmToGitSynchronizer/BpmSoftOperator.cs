using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BpmToGitSynchronizer
{
    class BpmSoftOperator
    {
        string url;
        string userName;
        string password;
        bool isPolling = false;
        bool isNetCore;
        CookieContainer cookies = default;

        /// <summary>
        /// Bpmsoft url-address
        /// </summary>
        public string Url { get => url; set => url = value; }
        /// <summary>
        /// Bpmsoft username of user which have access to configuration 
        /// </summary>
        public string UserName { get => userName; set => userName = value; }
        /// <summary>
        /// Bpmsoft user password
        /// </summary>
        public string Password { get => password; set => password = value; }
        /// <summary>
        /// Attribute which indicates the system platform (NetCore or NetFramework) 
        /// </summary>
        public bool IsNetCore { get => isNetCore; set => isNetCore = value; }
        public bool IsPolling { get => isPolling; set => isPolling = value; }

        /// <summary>
        /// Autentification service url-address
        /// </summary>
        private string AuthUrl { get
            {
                return Url + "/ServiceModel/AuthService.svc/Login";
            }
        }

        /// <summary>
        /// Common constructor
        /// </summary>
        /// <param name="url">Bpmsoft url-address</param>
        /// <param name="userName">Bpmsoft username of user which have access to configuration</param>
        /// <param name="password">Bpmsoft user password</param>
        /// <param name="isNetCore">Attribute which indicates the system platform (NetCore or NetFramework)</param>
        public BpmSoftOperator(string url, string userName, string password, bool isNetCore = false)
        {
            Url = url;
            UserName = userName;
            Password = password;
            IsNetCore = isNetCore;
        }

        /// <summary>
        /// Call bpmsoft authentification service
        /// </summary>
        /// <returns>Response's content</returns>
        public string Authenticate()
        {
            var body = @"{""UserName"":""" + UserName + @""", ""UserPassword"":""" + Password + @""" }";
            return SendPostRequest(AuthUrl, body, true);
        }

        /// <summary>
        /// Perform pulling changes from bpmsoft to file system 
        /// to Pkg directory
        /// </summary>
        /// <returns></returns>
        public string PullChangesToFileSystem()
        {
            Authenticate();
            string pathToService = IsNetCore ? $"{Url}/ServiceModel/AppInstallerService.svc/LoadPackagesToFileSystem" : $"{Url}/0/ServiceModel/AppInstallerService.svc/LoadPackagesToFileSystem";
            return SendPostRequest(pathToService, string.Empty);
        }

        /// <summary>
        /// Sends a request to update the date of the last synchronization session with the git repository
        /// </summary>
        /// <returns>Response's content</returns>
        public string UpdateLastGitSyncDate()
        {
            Authenticate();
            var serviceUri = "rest/BpmToGitSynchronizerIndicatorService/UpdateLastGitSyncDate";
            string pathToService = IsNetCore ? $"{Url}/{serviceUri}" : $"{Url}/0/{serviceUri}";

            var dateTimeUnspec = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
            var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(dateTimeUnspec, TimeZoneInfo.Local);

            var body = @"{""dateTime"": """ + utcDateTime.ToString("yyyy-MM-dd HH:mm:ss,fff") + @"""}";
            var response = SendPostRequest(pathToService, body);
            Console.WriteLine($"Result of updating last git sync date: {response}");
            return response;
        }

        /// <summary>
        /// Sends git operation status to CRM system
        /// </summary>
        /// <param name="status">Sync statuc code</param>
        /// <param name="message">Sync status message</param>
        /// <returns>Response's content</returns>
        internal string UpdateSyncStatus(SyncStatus status, string message = default)
        {
            Authenticate();
            var serviceUri = "rest/BpmToGitSynchronizerIndicatorService/UpdateGitSyncStatus";
            string pathToService = IsNetCore ? $"{Url}/{serviceUri}" : $"{Url}/0/{serviceUri}";

            var body = @"{""status"":""" + status + @""", ""message"":""" + message + @""" }";
            var response = SendPostRequest(pathToService, body);
            Console.WriteLine($"Result of updating sync status: {response}");
            return response;
        }

        /// <summary>
        /// Perform a POST request to the BpmSoft/Cretio system
        /// </summary>
        /// <param name="url">Url address of BpmSoft/Creatio</param>
        /// <param name="body">Request body (JSON)</param>
        /// <param name="isAuthRequest">Flag shows is authentification request</param>
        /// <returns>Request response</returns>
        private string SendPostRequest(string url, string body, bool isAuthRequest = false)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                byte[] content = Encoding.UTF8.GetBytes(body);
                request.ContentType = "application/json";
                request.ContentLength = content.Length;

                if (isAuthRequest)
                {
                    this.cookies = new CookieContainer();
                    request.CookieContainer = this.cookies;
                }
                else
                {
                    request.Timeout = 10 * 60 * 10000;
                    request.CookieContainer = new CookieContainer();
                    IEnumerable<Cookie> cookies = this.cookies.GetCookies(new Uri(Url)).Cast<Cookie>();
                    foreach (Cookie cookie in cookies)
                    {
                        request.CookieContainer.Add(cookie);
                        if (cookie.Name == "BPMCSRF") request.Headers.Set("BPMCSRF", cookie.Value);
                    }
                }

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(content, 0, content.Length);
                dataStream.Close();

                using var response = (HttpWebResponse)request.GetResponse();
                using var reader = new StreamReader(response.GetResponseStream());
                return reader.ReadToEnd();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        internal void WaitManualCommit(GitOperator gitOperator, System.Threading.CancellationToken token)
        {
            while(!token.IsCancellationRequested) {
                Console.WriteLine($"Send long polling request");
                Authenticate();
                var serviceUri = "rest/BpmToGitSynchronizerIndicatorService/CommitMessagePolling";
                string pathToService = IsNetCore ? $"{Url}/{serviceUri}" : $"{Url}/0/{serviceUri}";

                var body = "{}";
                CancellationTokenSource cancelTokenSource = new CancellationTokenSource(); 
                CancellationToken request_token = cancelTokenSource.Token;
                Task task = new Task(() => { 
                    var response = SendPostRequest(pathToService, body);
                    Console.WriteLine($"Result of updating sync status: {response}");

                    PullChangesToFileSystem();
                    gitOperator.StageChanges();
                    gitOperator.CommitChanges();
                    gitOperator.PushChanges();
                }, token);
                task.Start();
                task.Wait(token);
            }
        }
    }
}
