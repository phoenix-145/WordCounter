using System.Net.Http.Headers;
using System.Configuration;

namespace WordCounter
{
    public class OpensubtitlesAPI
    {
        private static readonly HttpClient httpClient= new HttpClient();
        private static readonly string OpensubtitlesAPI_Key = ConfigurationManager.AppSettings.Get("OpensubtitlesAPI_Key")!;
        private static readonly string OpensubtitlesAPI_Username = ConfigurationManager.AppSettings.Get("OpensubtitlesAPI_Username")!;
        private static readonly string OpensubtitlesAPI_Password = ConfigurationManager.AppSettings.Get("OpensubtitlesAPI_Password")!;
        private static readonly string OpensubtitlesLoginURL = "https://api.opensubtitles.com/api/v1/login";
        public static async Task MakeConnection()
        {
            await Login();
            Console.ReadLine();

            var client = httpClient;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://api.opensubtitles.com/api/v1/download"),
                Headers = 
                {
                    { "User-Agent", "test, v1" },
                    { "Accept", "application/json" },
                    { "Authorization", "Bearer H891Ie2XKDNfjraQ8XewgPjt19qw1FSB" },
                    { "Api-Key", "H891Ie2XKDNfjraQ8XewgPjt19qw1FSB" }
                },
                Content = new StringContent("{\n\"file_id\": 123\n}")
                {
                    Headers = 
                    {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                }
            };
            
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                if(body == "" || body == null || body == "200")
                {
                    Console.WriteLine("Denied");
                    Console.ReadLine();
                }
                Console.WriteLine(body);
            }
        }
        private static async Task Login()
        {
            var client = httpClient;
            var request = new HttpRequestMessage
            {
                Method= HttpMethod.Post,
                RequestUri = new Uri(OpensubtitlesLoginURL),
                Headers = 
                {
                    { "User-Agent", "WordCounter/1.0" },
                    { "Accept", "application/json" },
                    { "Api-Key", OpensubtitlesAPI_Key }
                },
                Content = new StringContent($"{{\n  \"username\": \"{OpensubtitlesAPI_Username}\",\n  \"password\": \"{OpensubtitlesAPI_Password}\"\n}}")
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                }
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine(body);
                Console.Read();
            }
        }
    }
}