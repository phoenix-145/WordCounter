using System.Net.Http.Headers;
using System.Configuration;
using System.Text.Json;
using static WordCounter.Program;

namespace WordCounter
{
    public class OpensubtitlesAPI
    {
        internal static readonly HttpClient httpClient= HttpClientFactory.httpClient;
        internal static readonly string OpensubtitlesAPI_Key = ConfigurationManager.AppSettings.Get("OpensubtitlesAPI_Key")!;
        internal static readonly string OpensubtitlesAPI_Username = ConfigurationManager.AppSettings.Get("OpensubtitlesAPI_Username")!;
        internal static readonly string OpensubtitlesAPI_Password = ConfigurationManager.AppSettings.Get("OpensubtitlesAPI_Password")!;
        internal static readonly string OpensubtitlesLoginURL = "https://api.opensubtitles.com/api/v1/login";
        internal static readonly string OpensubtitlesDownloadURL = "https://api.opensubtitles.com/api/v1/download";
        public static async Task MakeConnection()
        {
            string User_token = await Login();

            if(string.IsNullOrEmpty(User_token))
            {
                SlowPrintingText.SlowPrintText("Error. something went wrong during login. Program exiting.");
                return;
            }

            var client = httpClient;
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(OpensubtitlesDownloadURL),
                Headers = 
                {
                    { "User-Agent", "WordCounter/1.0" },
                    { "Accept", "application/json" },
                    { "Authorization", $"Bearer {User_token}" },
                    { "Api-Key", $"{OpensubtitlesAPI_Key}" }
                },
                Content = new StringContent("{\n\"file_id\": 123\n}")
                {
                    Headers = 
                    {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                }
            };

            using var response = await client.SendAsync(request);
            if(response.IsSuccessStatusCode)
            {
                var response_body = await response.Content.ReadAsStringAsync();
                if(!string.IsNullOrEmpty(response_body))
                {
                    Download download_response = JsonSerializer.Deserialize<Download>(response_body)!;

                    SlowPrintingText.SlowPrintText("\nSuccessfully retrieved download information.\n");
                    SlowPrintingText.SlowPrintText($"You have made {download_response.requests} requests. You have {download_response.remaining} downloads remaining in this account.");
                    SlowPrintingText.SlowPrintText(download_response.message!);

                    await DownloadSubtitles(download_response.link!, download_response.file_name!);
                }
                else 
                {
                    SlowPrintingText.SlowPrintText("Error. the server returned a bad response when requesting for download.");
                }
            }
            else
            {
                SlowPrintingText.SlowPrintText("Error. something went wrong when retrieving download information. Error code : " + response.StatusCode);
            }

        }
        private static async Task<string> Login()
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

            using var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var response_body = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(response_body))
                {
                    Login login_response = JsonSerializer.Deserialize<Login>(response_body)!;
                    SlowPrintingText.SlowPrintText($"\nSuccessfully logged into {OpensubtitlesAPI_Username}. \nYou have {login_response.user!.allowed_downloads} downloads left on this account.");
                    return login_response.token!;
                }
                else
                {
                    SlowPrintingText.SlowPrintText("Error. Something went wrong when. The server returned a bad response.");
                    return null!;
                }
            }
            else
            {
                SlowPrintingText.SlowPrintText("Error. Something went wrong when logging in. Error code : " + response.StatusCode);
                return null!;
            }
        }
        private static async Task DownloadSubtitles(string DownloadURL, string filename)
        {
            string filepathtodownload = $@"TextFiles\{filename}";

            try
            {
                SlowPrintingText.SlowPrintText($"Downloading subtitles to filepath : {filepathtodownload}.");

                using (var fileStream = new FileStream(filepathtodownload, FileMode.Create))
                {
                    HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(DownloadURL);
                    using Stream contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                    await contentStream.CopyToAsync(fileStream);
                }

                SlowPrintingText.SlowPrintText("Download was successful.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Download file error: {ex}");
            }
        }
    }
}