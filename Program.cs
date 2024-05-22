namespace WordCounter
{
    class Program
    {
        public static async Task Main()
        {
            await Menu();
        }
        private static async Task Menu()
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
                    await FetchMovieScript.GetMovieName();
                    break;
            }
        }
        public static class HttpClientFactory
        {
            public static readonly HttpClient httpClient = new();
        }
    }

}