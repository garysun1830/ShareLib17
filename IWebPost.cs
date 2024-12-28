using System.Collections.Generic;

namespace ShareLib17
{

    public interface IWebPost
    {

        string Post(string RootUrl, string PostUrl, string Referer, string ContentType,
            Dictionary<string, string> Header, Dictionary<string, string> Query);
        string Post(string RootUrl, string PostUrl, string Referer, string ContentType,
            Dictionary<string, string> Header, Dictionary<string, string> Query, string ProxyUrl, int Port, int Timeout);
        string Post(string RootUrl, string PostUrl, string Referer, string ContentType,
            Dictionary<string, string> Header, Dictionary<string, string> Query, string ProxyUrl, int Port);
    }

}