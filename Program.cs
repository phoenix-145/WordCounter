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
            string prompt= "What would you like to do ?";
            string[] options = {"Count the number of words in a text file", "Function 2", "Function 3"};
            Menu menu= new Menu(prompt, options);
            int SelectedOption = menu.Run();

            switch (SelectedOption)
            {
                case 0:
                    Wordcounter.GetFileLocation();
                    break;
                case 1:
                    await TmdbAPI.GetMovieName();
                    break;
            }
        }
        public static class HttpClientFactory
        {
            public static readonly HttpClient httpClient = new();
        }
    }

}