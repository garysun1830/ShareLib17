using System;
using System.Net;
using System.Text;

namespace ShareLib17
{

    public interface IWebReader
    {
        CookieContainer Cookies { get; }
        string DownloadString(string Url);
        string DownloadString(string Url, Encoding Code, string ProxyUrl, int Port);
        string DownloadString(string Url, Encoding Code, string ProxyUrl, int Port, out Exception error);
        string DownloadString(string Url, Encoding Code, string ProxyUrl, int Port, int Timeout);
        string DownloadString(string Url, Encoding Code, string Referer, string ProxyUrl, int Port, int Timeout);
        string DownloadString(string Url, Encoding Code, string Referer, string ProxyUrl, int Port, int Timeout, out Exception error);
        void ReadWebToFileAsync(string Url, string FileName);
        void ReadWebToFileAsync(string Url, string FileName, string Referer, string ProxyUrl, int Port, int Timeoute);
        void DownloadToFile(string Url, string FileName);
        void Reset();
        string TestDownloadString(string Url, Encoding Code, string Referer, string ProxyUrl, int Port);
    }

}