using System;
using System.Net;
using System.Text;

namespace ShareLib17
{
    public class SysWebReader : IWebReader
    {
        private readonly IUserInteract userInteract;
        private readonly CookieContainer container;
        public CookieContainer Cookies { get { return container; } }

        public SysWebReader()
        {
        }

        public SysWebReader(IUserInteract Userinteract, CookieContainer Container)
        {
            userInteract = Userinteract;
            container = Container;
        }

        public string TestDownloadString(string Url, Encoding Code, string Referer, string ProxyUrl, int Port)
        {
            return null;
        }

        public string DownloadString(string Url)
        {
            return DownloadString(Url, null, null, null, 0, 0);
        }

        public string DownloadString(string Url, Encoding Code, string ProxyUrl, int Port)
        {
            return DownloadString(Url, Code, null, ProxyUrl, Port, 0);
        }

        public string DownloadString(string Url, Encoding Code, string ProxyUrl, int Port, out Exception error)
        {
            return DownloadString(Url, Code, null, ProxyUrl, Port, 0, out error);
        }

        public string DownloadString(string Url, Encoding Code, string ProxyUrl, int Port, int Timeout)
        {
            return DownloadString(Url, Code, Url, ProxyUrl, Port, Timeout);

        }

        public string DownloadString(string Url, Encoding Code, string LastPage, string ProxyUrl, int Port, int Timeout)
        {
            Exception error;
            return DownloadString(Url, Code, LastPage, ProxyUrl, Port, Timeout, out error);
        }

        public string DownloadString(string Url, Encoding Code, string LastPage, string ProxyUrl, int Port, int Timeout, out Exception error)
        {
            error = null;
            if (Code == null)
                Code = Encoding.UTF8;
            using (WebClientEx web = WebClientEx.create(container, LastPage, Code, ProxyUrl, Port, Url))
            {
                web.Credentials = CredentialCache.DefaultCredentials;
                return web.DownloadString(Url);
            }
        }

        public void ReadWebToFileAsync(string Url, string FileName)
        {
            ReadWebToFileAsync(Url, FileName, null, null, 0, 100000);
        }

        public void ReadWebToFileAsync(string Url, string FileName, string LastPage, string ProxyUrl, int Port, int Timeout)
        {
            using (WebClientEx web = WebClientEx.create(container, LastPage, ProxyUrl, Port, userInteract))
            {
                web.Credentials = CredentialCache.DefaultCredentials;
                web.ReadWebToFileAsync(Url, FileName, Timeout);
            }
        }

        public void DownloadToFile(string Url, string FileName)
        {
            using (WebClientEx web = new WebClientEx())
            {
                web.Credentials = CredentialCache.DefaultCredentials;
                try
                {
                    web.DownloadFile(Url, FileName);
                }
                catch { }
            }
        }

        public void Reset()
        {

        }


    }

}
