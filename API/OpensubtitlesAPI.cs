using System.Net.Http.Headers;
using System.Configuration;
using System.Text.Json;
using static WordCounter.Program;

namespace WordCounter
{
    public class OpensubtitlesAPI
    {
        private static readonly HttpClient httpClient= HttpClientFactory.httpClient;
        private static readonly string OpensubtitlesAPI_Key = ConfigurationManager.AppSettings.Get("OpensubtitlesAPI_Key")!;
        private static readonly string OpensubtitlesAPI_Username = ConfigurationManager.AppSettings.Get("OpensubtitlesAPI_Username")!;
        private static readonly string OpensubtitlesAPI_Password = ConfigurationManager.AppSettings.Get("OpensubtitlesAPI_Password")!;
        private static readonly string OpensubtitlesLoginURL = "https://api.opensubtitles.com/api/v1/login";
        private static readonly string OpensubtitlesDownloadURL = "https://api.opensubtitles.com/api/v1/download";
        private static readonly string Opensubtitles_MovieIdURL = "https://api.opensubtitles.com/api/v1/subtitles?tmdb_id=";
        internal static async Task<Subtitles> GetSubtitlesId(int TMDB_MovieId, string TMDB_MovieName)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(Opensubtitles_MovieIdURL + TMDB_MovieId),
                Headers = 
                {
                    { "User-Agent", "WordCounter/1.0" },
                    { "Api-Key", $"{OpensubtitlesAPI_Key}" },
                }
            };
            try
            {
                using var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var response_body = await response.Content.ReadAsStringAsync();
                    if(!string.IsNullOrEmpty(response_body))
                    {
                        Subtitles subtitles = JsonSerializer.Deserialize<Subtitles>(response_body)!;
                        if(subtitles.data!.Length > 0)
                        {
                            return subtitles;
                        }
                        else
                        {
                            SlowPrintingText.SlowPrintText($"No subtitles for {TMDB_MovieName} found. Press Enter to continue.");
                            Console.ReadLine();
                            return null!;
                        }
                    }
                    else
                    {
                        Console.WriteLine("The server returned a bad response.");
                        return null!;
                    }
                }
                else
                {
                    Console.WriteLine("Error. Something went wrong when retrieving the subtitles Id.");
                    return null!;
                }
            }
            catch (TaskCanceledException)
            {SlowPrintingText.SlowPrintText("API call timed out. API service may be down."); return null!; }
            catch (HttpRequestException)
            {SlowPrintingText.SlowPrintText("Error : no internet connection."); return null!; }
        }
        internal static async Task LoginAndDownloadSubtitles(int SubtitlesId) 
        {
            string User_token = await Login();

            if(string.IsNullOrEmpty(User_token))
            {
                SlowPrintingText.SlowPrintText("Error. something went wrong during login. Program exiting.");
                return;
            }

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
                Content = new StringContent($"{{\n\"file_id\": {SubtitlesId}\n}}")
                {
                    Headers = 
                    {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                }
            };
            try
            {
                using var response = await httpClient.SendAsync(request);
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
            catch (TaskCanceledException)
            {SlowPrintingText.SlowPrintText("API call timed out. API service may be down.");}
                        catch (HttpRequestException)
            {SlowPrintingText.SlowPrintText("Error : no internet connection."); }

        }
        private static async Task<string> Login()
        {
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
            try
            {
                using var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var response_body = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(response_body))
                    {
                        Login login_response = JsonSerializer.Deserialize<Login>(response_body)!;
                        SlowPrintingText.SlowPrintText($"\nSuccessfully logged into Opensubtitles as {OpensubtitlesAPI_Username}. \nYou have {login_response.user!.allowed_downloads} downloads left on this account.");
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
            catch (TaskCanceledException)
            {SlowPrintingText.SlowPrintText("API call timed out. API service may be down."); return null!; }
            catch (HttpRequestException)
            {SlowPrintingText.SlowPrintText("Error : no internet connection."); return null!; }
        }
        private static async Task DownloadSubtitles(string DownloadURL, string filename)
        {
            string filepathtodownload = $@"TextFiles\{filename}";

            try
            {
                SlowPrintingText.SlowPrintText($"Downloading subtitles to filepath : {filepathtodownload}.");

                using (var fileStream = new FileStream(filepathtodownload, FileMode.Create))
                {
                    try
                    {
                        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(DownloadURL);
                        using Stream contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                        await contentStream.CopyToAsync(fileStream);
                    }
                    catch(TaskCanceledException)
                    {SlowPrintingText.SlowPrintText("API call timed out. API service may be down.");}
                    catch (HttpRequestException)
                    {SlowPrintingText.SlowPrintText("Error : no internet connection."); }
                }
                SlowPrintingText.SlowPrintText("Download was successful.");
                filepathofdownloadedsubtitle = filepathtodownload;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Download file error: {ex}");
            }
        }
    }
}