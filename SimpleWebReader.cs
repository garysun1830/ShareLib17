using System;
using System.IO;
using System.Net;
using System.Text;

namespace ShareLib17
{
    public class SimpleWebReader : IWebReader
    {
        public CookieContainer Cookies { get { return null; } }

        public SimpleWebReader()
        {
        }

        public SimpleWebReader(IUserInteract Userinteract, CookieContainer Container)
        {
        }

        public string DownloadString(string Url)
        {
            return DownloadString(Url, null, null, null, 0, 0);
        }

        public string DownloadString(string Url, Encoding Code, string ProxyUrl, int Port)
        {
            return DownloadString(Url, Code, null, ProxyUrl, Port, 0);
        }

        public string TestDownloadString(string Url, Encoding Code, string Referer, string ProxyUrl, int Port)
        {
            return null;
        }

        public string DownloadString(string Url, Encoding Code, string ProxyUrl, int Port, out Exception error)
        {
            return DownloadString(Url, Code, null, ProxyUrl, Port, 0, out error);
        }

        public string DownloadString(string Url, Encoding Code, string ProxyUrl, int Port, int Timeout)
        {
            return DownloadString(Url, Code, null, ProxyUrl, Port, Timeout);

        }

        public string DownloadString(string Url, Encoding Code, string LastPage, string ProxyUrl, int Port, int Timeout)
        {
            Exception error;
            return DownloadString(Url, Code, LastPage, ProxyUrl, Port, Timeout, out error);
        }

        public string DownloadString(string Url, Encoding Code, string LastPage, string ProxyUrl, int Port, int Timeout, out Exception error)
        {
            error = null;
            Stream stream;
            StreamReader reader;
            string response = null;
            WebClient webClient = new WebClient();

            using (webClient)
            {
                try
                {
                    // open and read from the supplied URI
                    stream = webClient.OpenRead(Url);
                    reader = new StreamReader(stream);
                    response = reader.ReadToEnd();
                }
                catch (WebException ex)
                {
                    error = ex;
                    if (ex.Response is HttpWebResponse)
                    {
                        // Add you own error handling as required
                        switch (((HttpWebResponse)ex.Response).StatusCode)
                        {
                            case HttpStatusCode.NotFound:
                            case HttpStatusCode.Unauthorized:
                                response = null;
                                break;

                            default:
                                throw ex;
                        }
                    }
                }
                return response;
            }

        }

        public void ReadWebToFileAsync(string Url, string FileName)
        {
        }

        public void ReadWebToFileAsync(string Url, string FileName, string LastPage, string ProxyUrl, int Port, int Timeout)
        {
        }

        public void DownloadToFile(string Url, string FileName)
        {
        }

        public void Reset()
        {

        }


    }

}
