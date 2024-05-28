using System.Net.NetworkInformation;

namespace WordCounter
{
    class Program
    {
        public static string filepathofdownloadedsubtitle = "";
        public static async Task Main()
        {
            await Menu();
        }
        private static async Task Menu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(AsciiArt.MainMenuArt);

                string prompt= "What would you like to do ?";
                string[] options = {"Count the number of words in a local text file", "Search and download the movie script online", "Function 3"};
                Menu menu= new(prompt, options);
                int SelectedOption = menu.Run();

                switch (SelectedOption)
                {
                    case 0:
                        Wordcounter.GetFileLocation();
                        break;
                    case 1:
                        if(InternetConnectionChecker.IsInternetAvailable())
                        {
                            await FetchMovieScript.GetMovieName();
                            if(!string.IsNullOrEmpty(filepathofdownloadedsubtitle))
                            {
                                Wordcounter.ReadFromFile(filepathofdownloadedsubtitle);
                                Wordcounter.WriteToFile();

                            }
                        }
                        else
                        {
                             SlowPrintingText.SlowPrintText("\nThis feature is currently not available as there is no internet connection.");
                        }
                        break;

                }
            }
        }
        public static class HttpClientFactory
        {
            public static readonly HttpClient httpClient = new()
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
        }
        public static class InternetConnectionChecker
        {
            public static bool IsInternetAvailable()
            {
                try
                {
                    using (var ping = new Ping())
                    {
                        var reply = ping.Send("8.8.8.8", 3000);
                        return reply.Status == IPStatus.Success;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }

}