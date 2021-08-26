using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TaskList
{
    public class WebRequestHandler
    {
        private HttpClient Client { get; }
        public WebRequestHandler()
        {
            Client = new HttpClient();
        }
        public async Task<string> Get(string url)
        {
            using (var client = new HttpClient())
            {
                using (var response = await client
                    .GetAsync(url, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false))
                {
                    return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : "ERROR";
                }
            }
        }

        public async Task<string> Post(string url, object obj)
        {
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    var json = JsonConvert.SerializeObject(obj);
                    using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                    {
                        request.Content = stringContent;

                        using (var response = await client
                            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                            .ConfigureAwait(false))
                        {
                            return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : "ERROR";
                        }
                    }
                }
            }
        }
    }
}
