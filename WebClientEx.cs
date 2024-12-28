using System;
using System.ComponentModel;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace ShareLib17
{

    public class WebClientEx : WebClient
    {

        private readonly IUserInteract userInteract;
        private DateTime lastSave;
        private string downMessage;
        private string downloadText;
        private long received;
        private int downPercent;
        private bool downloading;

        public static WebClientEx create(CookieContainer container, string Referer, Encoding Code, string ProxyUrl, int port, IUserInteract Userinteract, string Url)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            if (container == null)
                return new WebClientEx(Referer, Code, ProxyUrl, port, Userinteract, Url);
            else
                return new CookieWebClient(container, Referer, Code, ProxyUrl, port, Userinteract, Url);
        }

        public static WebClientEx create(string ProxyUrl, int port, IUserInteract Userinteract)
        {
            return create(null, null, null, ProxyUrl, port, Userinteract, null);
        }

        public static WebClientEx create(Encoding Code, string ProxyUrl, int port)
        {
            return create(null, null, Code, ProxyUrl, port, null, null);
        }

        public static WebClientEx create(CookieContainer container, string Referer, Encoding Code, string ProxyUrl, int port, string Url)
        {
            return create(container, Referer, Code, ProxyUrl, port, null, Url);
        }

        public static WebClientEx create(CookieContainer container, string Referer, string ProxyUrl, int port, IUserInteract Userinteract)
        {
            return create(container, Referer, null, ProxyUrl, port, Userinteract, null);
        }

        public WebClientEx()
        {
            Headers[HttpRequestHeader.Accept] = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0";
            Headers[HttpRequestHeader.CacheControl] = "max-age=0";
        }

        public WebClientEx(string Referer, Encoding Code, string Url) : this()
        {
            if (Code != null)
                Encoding = Code;
            Headers[HttpRequestHeader.Referer] = Referer;
            Headers[HttpRequestHeader.Host] = MyFunc.ExtractHost(Url);
        }

        public WebClientEx(string Referer, Encoding Code, string ProxyUrl, int port, IUserInteract Userinteract, string Url) : this(Referer, Code, Url)
        {
            userInteract = Userinteract;
            if (!string.IsNullOrEmpty(ProxyUrl))
            {
                IWebProxy proxy = port == 0 ? new WebProxy(ProxyUrl) : new WebProxy(ProxyUrl, port);
                proxy.Credentials = CredentialCache.DefaultCredentials;
                Proxy = proxy;
            }
        }

        public WebClientEx(string LastPage, Encoding Code, string ProxyUrl, int port)
            : this(LastPage, Code, ProxyUrl, port, null, null)
        {
        }

        public WebClientEx(IUserInteract Userinteract, string ProxyUrl, int port) : this(null, null, ProxyUrl, port, Userinteract, null)
        {
        }

        public WebClientEx(Encoding Code, string ProxyUrl, int port) : this(null, Code, ProxyUrl, port, null, null)
        {
        }
        public string ReadWebToStringAsync(string Url, int Timeout)
        {
            downMessage = null;
            downloadText = null;
            received = 0;
            DateTime lastSave = DateTime.Now;
            DownloadStringAsync(new Uri(Url));
            Thread.Sleep(Timeout);
            CancelAsync();
            if (received == 0)
                throw new WebExceptionTimeout();
            if (downMessage != null)
            {
                if (downMessage.Contains("403") && downMessage.ToLower().Contains("forbidden"))
                    throw new WebExceptionForbidden(downMessage);
                else
                  if (downMessage.Contains("503") && downMessage.ToLower().Contains("unavailable"))
                    throw new WebExceptionForbidden(downMessage);
                else
                  if (downMessage.ToLower().Contains("unable to connect"))
                    throw new WebExceptionForbidden(downMessage);
                else
                  if (downMessage.Contains("407") && downMessage.ToLower().Contains("authenticatio"))
                    throw new WebExceptionForbidden(downMessage);
                else
                    throw new WebExceptionContent(downMessage);
            }
            return downloadText;
        }

        protected override void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
        {
            base.OnDownloadProgressChanged(e);
            lastSave = DateTime.Now;
            received = e.BytesReceived;
            downPercent = e.ProgressPercentage;
            if (userInteract != null)
            {
                userInteract.ProgressChanged(received, downPercent);
                if (userInteract.UserCancelTask() && received == 0)
                    CancelAsync();
            }
            Thread.Sleep(200);
        }

        protected override void OnDownloadStringCompleted(DownloadStringCompletedEventArgs e)
        {
            base.OnDownloadStringCompleted(e);
            if (e.Error != null)
                downMessage = e.Error.Message;
            else
                downloadText = e.Result;
        }

        protected override void OnDownloadFileCompleted(AsyncCompletedEventArgs e)
        {
            base.OnDownloadFileCompleted(e);
            if (e.Error != null)
                downMessage = e.Error.Message;
            downloading = false;
        }

        private bool isBusy()
        {
            return IsBusy || downloading;
        }

        public void ReadWebToFileAsync(string Url, string FileName, int Timeout)
        {
            Exception error = null;
            downMessage = null;
            downloadText = null;
            received = 0;
            DateTime lastSave = DateTime.Now;
            downloading = true;
            DownloadFileAsync(new Uri(Url), FileName);
            while (isBusy())
            {
                if (downMessage != null)
                {
                    error = new Exception();
                    while (isBusy())
                    {
                        CancelAsync();
                        Thread.Sleep(100);
                    }
                    break;
                }
                if (userInteract != null && userInteract.UserCancelTask() && received == 0)
                {
                    error = new WebExceptionCancel();
                    while (isBusy())
                    {
                        CancelAsync();
                        Thread.Sleep(100);
                    }
                    break;
                }
                if (DateTime.Now > lastSave.AddMinutes(Timeout))
                {
                    error = new WebExceptionTimeout();
                    while (isBusy())
                    {
                        CancelAsync();
                        Thread.Sleep(100);
                    }
                    break;
                }
                Thread.Sleep(1000);
            }
            if (error != null)
                throw error;
            if (userInteract != null && userInteract.UserCancelTask())
                throw new WebExceptionCancel();
            if ((downMessage != null || downPercent < 100))
            {
                if (downMessage.Contains("403") && downMessage.ToLower().Contains("forbidden"))
                    throw new WebExceptionForbidden(downMessage);
                else
                    throw new WebExceptionContent(downMessage);
            }
        }
    }

    public class CookieWebClient : WebClientEx
    {

        private CookieContainer container;

        public CookieWebClient(CookieContainer Container, string LastPage, Encoding Code, string ProxyUrl, int port, IUserInteract Userinteract, string Url)
            : base(LastPage, Code, ProxyUrl, port, Userinteract, Url)
        {
            container = Container;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest r = base.GetWebRequest(address);
            if (container != null)
            {
                var request = r as HttpWebRequest;
                if (request != null)
                {
                    request.CookieContainer = container;
                }
            }
            return r;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            WebResponse response = base.GetWebResponse(request, result);
            if (container != null)
                ReadCookies(response);
            return response;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);
            if (container != null)
                ReadCookies(response);
            return response;
        }

        private void ReadCookies(WebResponse r)
        {
            var response = r as HttpWebResponse;
            if (response != null)
            {
                CookieCollection cookies = response.Cookies;
                container.Add(cookies);
            }
        }

    }

}
