using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ShareLib17
{
    public class HttpWebPost : IWebPost
    {
        private readonly CookieContainer container;
        private string errorMessage;
        private string postResponse;

        public HttpWebPost(CookieContainer Container)
        {
            container = Container;
        }

        private HttpClientHandler createClientHandler(string ProxyUrl, int Port)
        {
            HttpClientHandler result = new HttpClientHandler { CookieContainer = container };
            if (!string.IsNullOrWhiteSpace(ProxyUrl) && Port > 0)
            {
                IWebProxy proxy = new WebProxy(ProxyUrl, Port);
                proxy.Credentials = CredentialCache.DefaultCredentials;
                result = new HttpClientHandler
                {
                    CookieContainer = container,
                    Proxy = proxy,
                    UseProxy = true
                };
            }
            return result;
        }

        private async Task doPost(string RootUrl, string PostUrl, string Referer, string ContentType,
            Dictionary<string, string> Header, Dictionary<string, string> Query, string ProxyUrl, int Port)
        {
            try
            {
                var baseAddress = new Uri(RootUrl);
                using (var handler = createClientHandler(ProxyUrl, Port))
                {
                    using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                    {
                        client.DefaultRequestHeaders.Referrer = new Uri(Referer);
                        client.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
                        client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                        client.DefaultRequestHeaders.Add("Accept-Language", "en-CA,en;q=0.9,zh-CN;q=0.8,zh;q=0.7,en-GB;q=0.6,en-US;q=0.5");
                        client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.120 Safari/537.36");
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", ContentType);
                        foreach (string key in Header.Keys)
                        {
                            client.DefaultRequestHeaders.Add(key, Header[key]);
                        }
                        var content = new FormUrlEncodedContent(Query);
                        var response = await client.PostAsync(PostUrl, content);
                        response.EnsureSuccessStatusCode();
                        postResponse = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                errorMessage = "timeout";
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public string Post(string RootUrl, string PostUrl, string Referer, string ContentType, Dictionary<string, string> Header, Dictionary<string, string> Query)
        {
            return Post(RootUrl, PostUrl, Referer, ContentType, Header, Query, null, 0, 10000);
        }

        public string Post(string RootUrl, string PostUrl, string Referer, string ContentType, Dictionary<string, string> Header,
                                            Dictionary<string, string> Query, string ProxyUrl, int Port)
        {
            return Post(RootUrl, PostUrl, Referer, ContentType, Header, Query, ProxyUrl, Port, 30000);
        }

        public string Post(string RootUrl, string PostUrl, string Referer, string ContentType, Dictionary<string, string> Header,
                                            Dictionary<string, string> Query, string ProxyUrl, int Port, int Timeout)
        {
            postResponse = null;
            errorMessage = null;
            var cancelTask = new CancellationTokenSource();
            Task task = Task.Run(async () => await doPost(RootUrl, PostUrl, Referer, ContentType, Header, Query, ProxyUrl, Port));
            if (!task.Wait(Timeout)) throw new WebExceptionTimeout();
            if (errorMessage != null)
            {
                if (errorMessage == "timeout")
                    throw new WebExceptionTimeout();
                if (errorMessage.Contains("403") && errorMessage.ToLower().Contains("forbidden"))
                    throw new WebExceptionForbidden(errorMessage);
                else
                    throw new WebExceptionContent(errorMessage);
            }
            return postResponse;
        }

    }
}