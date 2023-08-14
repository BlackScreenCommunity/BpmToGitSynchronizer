using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace GitCaller
{
    class BpmSoftOperator
    {
        string url;
        string userName;
        string password;
        bool isNetCore;
        CookieContainer cookies = default;

        public string Url { get => url; set => url = value; }
        public string UserName { get => userName; set => userName = value; }
        public string Password { get => password; set => password = value; }
        public bool IsNetCore { get => isNetCore; set => isNetCore = value; }
        public string AuthUrl { get
            {
                return Url + "/ServiceModel/AuthService.svc/Login";
            }
        }

        public BpmSoftOperator(string url, string userName, string password, bool isNetCore = false)
        {
            Url = url;
            UserName = userName;
            Password = password;
            IsNetCore = isNetCore;
        }

        public string Auth()
        {
            var body = @"{""UserName"":""" + UserName + @""", ""UserPassword"":""" + Password + @""" }";
            return SendPostRequest(AuthUrl, body, true);
        }

        public string PullChangesToFileSystem()
        {
            Auth();
            string pathToService = IsNetCore ? $"{Url}/ServiceModel/AppInstallerService.svc/LoadPackagesToFileSystem" : $"{Url}/0/ServiceModel/AppInstallerService.svc/LoadPackagesToFileSystem";
            return SendPostRequest(pathToService, "");
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
    }
}
