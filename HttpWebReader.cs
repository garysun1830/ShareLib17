using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ShareLib17
{
    public class HttpWebReader : IWebReader
    {
        //private const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:59.0) Gecko/20100101 Firefox/59.0";
        private const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.142 Safari/537.36";
        //private const string CONTENT_TYPE = "application/x-www-form-urlencoded";
        private const string CONTENT_TYPE = "text/html; charset=utf-8";
        //private const string ACCEPT = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        private const string ACCEPT = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
        //private const string ACCEPT_LANGUAGE_VALUE = "zh-CN,en-US;q=0.7,en;q=0.3";
        private const string ACCEPT_LANGUAGE_VALUE = "en-CA,en;q=0.9,zh-CN;q=0.8,zh;q=0.7,en-GB;q=0.6,en-US;q=0.5";
        //private const string ACCEPT_ENCODING_VALUE = "gzip, deflate";
        private const string ACCEPT_ENCODING_VALUE = "gzip, deflate, br";
        private const string NO_CACHE = "no-cache";
        private readonly IUserInteract userInteract;
        private readonly CookieContainer container;
        private string downMessageValue;
        private long receivedValue;
        private int downPercentValue;
        private Object thisLock;
        private bool cancelledValue;
        private HttpClient client;

        public CookieContainer Cookies { get { return container; } }

        public HttpWebReader()
        {
            thisLock = new Object();
            container = new CookieContainer();
        }

        public HttpWebReader(IUserInteract Userinteract) : this()
        {
            userInteract = Userinteract;
        }

        private string downMessage
        {
            set
            {
                lock (thisLock)
                {
                    downMessageValue = value;
                }
            }
            get
            {
                lock (thisLock)
                {
                    return downMessageValue;
                }

            }
        }

        private long received
        {
            set
            {
                lock (thisLock)
                {
                    receivedValue = value;
                }
            }
            get
            {
                lock (thisLock)
                {
                    return receivedValue;
                }
            }
        }

        private int downPercent
        {
            set
            {
                lock (thisLock)
                {
                    downPercentValue = value;
                }
            }
            get
            {
                lock (thisLock)
                {
                    return downPercentValue;
                }
            }
        }

        private bool cancelled
        {
            set
            {
                lock (thisLock)
                {
                    cancelledValue = value;
                }
            }
            get
            {
                lock (thisLock)
                {
                    return cancelledValue;
                }
            }
        }

        private static void setCookies(CookieContainer container, HttpWebResponse resp)
        {
            foreach (Cookie ck in resp.Cookies)
            {
                Cookie newck = new Cookie();
                newck.Domain = ck.Domain;
                //very important! this cookie may expires with a old expiry date 
                newck.Expires = DateTime.Now.AddYears(10);
                newck.Name = ck.Name;
                newck.Path = ck.Path;
                newck.Secure = ck.Secure;
                newck.Value = ck.Value;
                container.Add(newck);
            }
        }

        private static HttpWebRequest createRequest(string Url, CookieContainer container, string Referer)
        {
            return createRequest(Url, container, Referer, null, 0);
        }

        private static HttpWebRequest createRequest(string Url, CookieContainer container, string Referer, string ProxyUrl, int Port)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                   | SecurityProtocolType.Tls11
                   | SecurityProtocolType.Tls12
                   | SecurityProtocolType.Ssl3;
            //System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            HttpWebRequest rqst = (HttpWebRequest)WebRequest.Create(new Uri(Url));
            if (!string.IsNullOrEmpty(ProxyUrl))
            {
                IWebProxy proxy = Port == 0 ? new WebProxy(ProxyUrl) : new WebProxy(ProxyUrl, Port);
                proxy.Credentials = CredentialCache.DefaultCredentials;
                rqst.Proxy = proxy;
            }
            rqst.CookieContainer = container;
            rqst.UserAgent = USER_AGENT;
            rqst.ContentType = CONTENT_TYPE;
            rqst.Accept = ACCEPT;
            rqst.Headers.Add(HttpRequestHeader.AcceptLanguage, ACCEPT_LANGUAGE_VALUE);
            rqst.Headers.Add(HttpRequestHeader.CacheControl, "max-age=0");
            rqst.Headers.Add("Pragma", NO_CACHE);
            rqst.Headers.Add("Upgrade-Insecure-Request", "1");
            rqst.ProtocolVersion = HttpVersion.Version11;
            rqst.Host = MyFunc.ExtractHost(Url);
            rqst.AllowAutoRedirect = false;
            rqst.KeepAlive = true;
            rqst.Referer = Referer;
            return rqst;
        }
        public static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public string DownloadString(string Url)
        {
            Exception ex;
            return DownloadString(Url, null, null, null, 0, 0, out ex);
        }

        public string DownloadString(string Url, string Ref)
        {
            Exception ex;
            return DownloadString(Url, null, Ref, null, 0, 0, out ex);
        }

        public string DownloadString(string Url, Encoding Code, string ProxyUrl, int Port)
        {
            Exception error;
            return DownloadString(Url, Code, null, ProxyUrl, Port, 0, out error);
        }

        public string DownloadString(string Url, Encoding Code, string ProxyUrl, int Port, out Exception error)
        {
            return DownloadString(Url, Code, null, ProxyUrl, Port, 30000, out error);
        }

        public string DownloadString(string Url, Encoding Code, string ProxyUrl, int Port, int Timeout)
        {
            Exception ex;
            return DownloadString(Url, Code, Url, ProxyUrl, Port, Timeout, out ex);
        }

        public string DownloadString(string Url, Encoding Code, string Referer, string ProxyUrl, int Port, int Timeout)
        {
            Exception ex;
            return DownloadString(Url, Code, Referer, ProxyUrl, Port, Timeout, out ex);
        }

        public string DownloadString(string Url, Encoding Code, string Referer, string ProxyUrl, int Port, int Timeout, out Exception error)
        {
            error = null;
            if (Code == null)
                Code = Encoding.UTF8;
            if (Timeout == 0) Timeout = 10000;
            string result = null;
            HttpWebRequest rqst = createRequest(Url, container, Referer, ProxyUrl, Port);
            rqst.Timeout = Timeout;
            try
            {
                using (HttpWebResponse resp = (HttpWebResponse)rqst.GetResponse())
                {
                    setCookies(container, resp);
                    using (StreamReader rd = new StreamReader(resp.GetResponseStream(), Code))
                    {
                        result = rd.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                error = ex;
                string exMessage = ex.Message;
                if (ex.Response != null)
                {
                    using (StreamReader responseReader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        result = responseReader.ReadToEnd();
                    }
                }
            }
            return result;
        }

        public string TestDownloadString(string Url, Encoding Code, string Referer, string ProxyUrl, int Port)
        {
            if (Code == null)
                Code = Encoding.UTF8;
            string result = null;
            HttpWebRequest rqst = createRequest(Url, container, Referer, ProxyUrl, Port);
            rqst.Timeout = 10000;
            try
            {
                using (HttpWebResponse resp = (HttpWebResponse)rqst.GetResponse())
                {
                    setCookies(container, resp);
                    using (StreamReader rd = new StreamReader(resp.GetResponseStream(), Code))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            result = rd.ReadToEnd();
                            if (string.IsNullOrWhiteSpace(result)) return result;
                            Thread.Sleep(500);
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                string exMessage = ex.Message;
                if (ex.Response != null)
                {
                    using (StreamReader responseReader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        result = responseReader.ReadToEnd();
                    }
                }
            }
            return result;
        }

        private HttpClient createWebClient(string ProxyUrl, int Port)
        {
            var httpClientHandler = new HttpClientHandler();
            if (!string.IsNullOrWhiteSpace(ProxyUrl) && Port > 0)
            {

                IWebProxy proxy = new WebProxy(ProxyUrl, Port);
                proxy.Credentials = CredentialCache.DefaultCredentials;
                httpClientHandler = new HttpClientHandler
                {
                    Proxy = proxy,
                    UseProxy = true
                };
            }
            return new HttpClient(httpClientHandler);
        }

        private async Task ReadWebToFileAsync(string Url, string FileName, string Referer, string ProxyUrl, int Port, CancellationToken token)
        {
            var buffer = new byte[8192];
            int readCount = 0;
            downPercent = 0;
            downMessage = null;
            received = 0L;
            cancelled = false;
            try
            {
                client = createWebClient(ProxyUrl, Port);
                client.DefaultRequestHeaders.Referrer = new Uri(Referer);
                using (HttpResponseMessage response = client.GetAsync(Url, HttpCompletionOption.ResponseHeadersRead).Result)
                {
                    response.EnsureSuccessStatusCode();
                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                        fileStream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        while (true)
                        {
                            token.ThrowIfCancellationRequested();
                            if (userInteract.UserCancelTask() && received == 0)
                            {
                                cancelled = true;
                                break;
                            }
                            var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                            if (read == 0)
                                break;
                            await fileStream.WriteAsync(buffer, 0, read);
                            received += read;
                            readCount++;
                            if (readCount % 20 == 0)
                            {
                                downPercent = (int)(received * 100 / response.Content.Headers.ContentLength);
                                if (downPercent >= 98) downPercent = 100;
                                if (userInteract != null)
                                {
                                    userInteract.ProgressChanged(received, downPercent);
                                }
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                downMessage = "timeout";
            }
            catch (Exception ex)
            {
                downMessage = ex.Message;
            }
        }

        public void ReadWebToFileAsync(string Url, string FileName)
        {
            ReadWebToFileAsync(Url, FileName, null, null, 0, 100000);
        }

        public void ReadWebToFileAsync(string Url, string FileName, string Referer, string ProxyUrl, int Port, int Timeout)
        {
            client = null;
            var cancelTask = new CancellationTokenSource();
            Task task = Task.Run(async () => await ReadWebToFileAsync(Url, FileName, Referer, ProxyUrl, Port, cancelTask.Token));
            if (!task.Wait(Timeout))
            {
                cancelTask.Cancel();
                throw new WebExceptionTimeout();
            }
            if (downMessage != null)
            {
                if (downMessage == "timeout")
                    throw new WebExceptionTimeout();
                if (downMessage.Contains("403") && downMessage.ToLower().Contains("forbidden"))
                    throw new WebExceptionForbidden(downMessage);
                else
                    throw new WebExceptionContent(downMessage);
            }
            if (cancelled)
                throw new WebExceptionCancel();
        }

        public void DownloadToFile(string Url, string FileName)
        {
            using (WebClientEx web = new WebClientEx())
            {
                web.Credentials = CredentialCache.DefaultCredentials;
                web.DownloadFile(Url, FileName);
            }
        }

        public void Reset()
        {
            client = null;
        }

    }
}