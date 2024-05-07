namespace WordCounter
{
    class Program
    {
        public static void Main()
        {
            string prompt= "What would you like to do ?";
            string[] options = {"Count the number of words in a text file", "Function 2", "Function 3"};
            Menu menu= new Menu(prompt, options);
            menu.Run();
        }
    }

}