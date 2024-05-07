namespace WordCounter
{
    class Program
    {
        public static void Main()
        {
            Menu();
        }
        private static void Menu()
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
            }
        }
    }

}