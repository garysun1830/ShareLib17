using System;
using System.IO;
using System.Net;
using System.Text;

namespace ShareLib17
{
    public class FileWebReader : IWebReader
    {

        public CookieContainer Cookies { get { return null; } }

        public string DownloadString(string Url)
        {
            return File.ReadAllText(Url);
        }

        public string TestDownloadString(string Url, Encoding Code, string Referer, string ProxyUrl, int Port)
        {
            return null;
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
            return DownloadString(Url, Code, null, ProxyUrl, Port, Timeout);
        }

        public string DownloadString(string Url, Encoding Code, string LastPage, string ProxyUrl, int Port, int Timeout)
        {
            return DownloadString(Url);
        }

        public string DownloadString(string Url, Encoding Code, string LastPage, string ProxyUrl, int Port, int Timeout, out Exception error)
        {
            error = null;
            return DownloadString(Url);
        }

        public void ReadWebToFileAsync(string Url, string FileName)
        {
            DownloadToFile(Url, FileName);
        }

        public void ReadWebToFileAsync(string Url, string FileName, string LastPage, string ProxyUrl, int Port, int Timeout)
        {
            DownloadToFile(Url, FileName);
        }

        public void DownloadToFile(string Url, string FileName)
        {
            File.WriteAllText(FileName, DownloadString(Url));
        }

        public void Reset()
        {

        }


    }

}
