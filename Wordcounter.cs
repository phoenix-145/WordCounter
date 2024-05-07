using System.Text.RegularExpressions;

namespace WordCounter
{
    internal class Wordcounter
    {
        public static Dictionary<string, int> WordsWithCount = new();
        public static void GetFileLocation()
        {
            string input;
            
            do
            {
                Console.Write("Enter the file path (The file must be a text file.) or enter 0 to exit : ");
                input = Console.ReadLine()!;
                if (input == null || input == "")
                {
                    Console.WriteLine("\nInvalid input.");
                }
                else if(input == "0")
                {
                    return;
                }
                else if(!File.Exists(input))
                {
                    Console.WriteLine("\nFile not found.");
                }

            }while(input == null || input == "" || !File.Exists(input));

            ReadFromFile(input);
            WriteToFile(); 
        }
        private static void ReadFromFile(string path)
        {
            using(StreamReader reader = new StreamReader(path))
            {
                string rawtext = reader.ReadToEnd();
                string filtertext = Regex.Replace(rawtext, @"[^a-zA-Z0-9\s']", "");
                string[] split_text = filtertext.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                foreach(string word in split_text)
                {
                    if(WordsWithCount.ContainsKey(word))
                    {
                        WordsWithCount[word] += 1;
                    }
                    else
                    {
                        WordsWithCount.Add(word, 1);
                    }
                }
            }
        }
        private static void WriteToFile()
        {
            string FileToWriteTo;

            Console.Write("Enter your output file path(default : OutPut.txt) : ");
            FileToWriteTo = Console.ReadLine()!;

            if(FileToWriteTo == "")
            {
                FileToWriteTo = @"TextFiles\Output.txt";
            }
            if(!File.Exists(FileToWriteTo))
            {
                string prompt = @"The stated file does not exist. Would you like to enter the path again or use default path (TextFiles\Output.txt) ? ";
                string[] options = {"Enter Again", "Use default path"};
                Menu menu = new Menu(prompt, options);
                int input = menu.Run();

                switch(input)
                {
                    case 0:
                        WriteToFile();
                        break;
                    case 1:
                        FileToWriteTo = @"TextFiles\Output.txt";
                        break;
                }
            }

            using(StreamWriter writer = new StreamWriter(FileToWriteTo))
            {
                var OrderedWordsWithCount = WordsWithCount.OrderByDescending(x => x.Value); 
                foreach(var word in OrderedWordsWithCount)
                {
                    writer.WriteLine($"{word.Key} : {word.Value}");
                } 
            } 
        }
    }
    
}