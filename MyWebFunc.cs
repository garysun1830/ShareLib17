using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Reflection;
using System.Net.Mail;
using System.Runtime.Caching;
using System.ComponentModel;

namespace ShareLib17
{

    public class WebExceptionTimeout : Exception
    {
        public WebExceptionTimeout() : base("") { }
    }

    public class WebExceptionCancel : Exception
    {
        public WebExceptionCancel() : base("") { }
    }

    public class WebExceptionForbidden : Exception
    {
        public WebExceptionForbidden() : base("") { }
        public WebExceptionForbidden(string message) : base(message) { }
    }

    public class WebExceptionContent : Exception
    {
        public WebExceptionContent() : base("") { }
        public WebExceptionContent(string message) : base(message) { }
    }

    public interface IUserInteract
    {
        bool UserCancelTask();
        void ProgressChanged(long received, int percent);
    }

    public static partial class MyFunc
    {

        public static string ReadWeb(string Url, Encoding Code, out string Error)
        {
            string tmp = null;
            return ReadWeb(Url, Code, null, ref tmp, out Error);
        }

        public static string ReadWeb(string Url, Encoding Code, CookieContainer container, ref string LastPage, out string Error)
        {
            return ReadWeb(Url, Code, container, null, 0, ref LastPage, 0, out Error);
        }

        public static string ReadWeb(string Url, Encoding Code, string ProxyUrl, int port, out string Error)
        {
            string tmp = null;
            return ReadWeb(Url, Code, null, ProxyUrl, port, ref tmp, 0, out Error);
        }

        public static string ReadWeb(string Url, Encoding Code, string ProxyUrl, int port)
        {
            string tmp = null;
            string error;
            return ReadWeb(Url, Code, null, ProxyUrl, port, ref tmp, 0, out error);
        }

        public static string ReadWeb(string Url, Encoding Code, string ProxyUrl, int port, int Timeout, out string Error)
        {
            string tmp = null;
            return ReadWeb(Url, Code, null, ProxyUrl, port, ref tmp, Timeout, out Error);
        }

        public static string ReadWeb(string Url, Encoding Code, CookieContainer container, string ProxyUrl, int port, ref string LastPage, int Timeout, out string Error)
        {
            Error = null;
            try
            {
                if (!Regex.IsMatch(Url, "^https?://", RegexOptions.IgnoreCase))
                    Url = "http://" + Url;
                using (WebClientEx web = new WebClientEx(container, LastPage, Code, ProxyUrl, port))
                {
                    web.Credentials = CredentialCache.DefaultCredentials;
                    string result = web.DownloadString(Url);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return "";
            }
        }

        public static string ReadWeb(string Url, Encoding Code)
        {
            string error;
            return ReadWeb(Url, Code, out error);
        }

        public static string ReadWeb(string Url)
        {
            return ReadWeb(Url, null);
        }

        public static void ReadWebToFileAsync(string Url, string FileName, string ProxyUrl, int port, IUserInteract userInteract,
                int Timeout, out Exception Error)
        {
            Error = null;
            try
            {
                if (!Regex.IsMatch(Url, "^https?://", RegexOptions.IgnoreCase))
                    Url = "http://" + Url;
                using (WebClientEx web = new WebClientEx(userInteract, ProxyUrl, port))
                {
                    web.ReadWebToFileAsync(Url, FileName, Timeout, out Error);
                }
            }
            catch (Exception ex)
            {
                Error = ex;
            }
        }

        public static string ReadWebToStringAsync(string Url, Encoding Code, string ProxyUrl, int port, int Timeout, out Exception Error)
        {
            Error = null;
            try
            {
                if (!Regex.IsMatch(Url, "^https?://", RegexOptions.IgnoreCase))
                    Url = "http://" + Url;
                using (WebClientEx web = new WebClientEx(Code, ProxyUrl, port))
                {
                    web.Credentials = CredentialCache.DefaultCredentials;
                    string result = web.ReadWebToStringAsync(Url, Timeout, out Error);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Error = ex;
                return "";
            }
        }

    }

}
