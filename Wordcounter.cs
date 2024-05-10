using System.Text.RegularExpressions;
using System.IO;
using System.Reflection.Metadata;

namespace WordCounter
{
    internal class Wordcounter
    {
        public static Dictionary<string, int> WordsWithCount = new();
        public static void GetFileLocation()
        {
            string input;
            bool isfilevalid;
            
            do
            {
                isfilevalid = true;

                Console.Write("\nEnter the file path (The file must be a text file.) or enter 0 to exit : ");
                input = Console.ReadLine()!;

                if (input == null || input == "")
                {
                    Console.WriteLine($"\nInvalid input.\n");
                    isfilevalid = false;
                }
                else if(input == "0")
                {
                    return;
                }
                else if(!File.Exists(input))
                {
                    Console.WriteLine("\nFile not found.\n");
                    isfilevalid = false;
                }
                else if(GetFileType(input) == false.ToString() || GetFileType(input) != ".txt")
                {
                    Console.WriteLine("\nFile is not a text file.");
                    isfilevalid = false;
                }

            }while(isfilevalid == false);

            ReadFromFile(input!);
            WriteToFile(); 
        }
        private static void ReadFromFile(string path)
        {
            using(StreamReader reader = new StreamReader(path))
            {
                string rawtext = reader.ReadToEnd().ToLower();
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

            Console.Write("Enter your output file path(default : Output.txt) : ");
            FileToWriteTo = Console.ReadLine()!;

            if(FileToWriteTo == "")
            {
                FileToWriteTo = @"TextFiles\Output.txt";
            }
            if(!File.Exists(FileToWriteTo))
            {
                string prompt = @"The stated file does not exist. Would you like to enter the path again or use default path (TextFiles\Output.txt) ? ";
                string[] options = {"Enter Again", "Use default path", "Create a new text file at the specified path"};
                Menu menu = new Menu(prompt, options);
                int input = menu.Run();

                switch(input)
                {
                    case 0:
                        WriteToFile();
                        return;
                    case 1:
                        FileToWriteTo = @"TextFiles\Output.txt";
                        break;
                    case 2:
                        FileToWriteTo += ".txt";
                        break;
                }
            }

            try
            {
                using(StreamWriter writer = new StreamWriter(FileToWriteTo))
                {
                    var OrderedWordsWithCount = WordsWithCount.OrderByDescending(x => x.Value); 
                    foreach(var word in OrderedWordsWithCount)
                    {
                        writer.WriteLine($"{word.Key} : {word.Value}");
                    } 
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("\nSomething wrong happened : " + ex.Message);
                WriteToFile();
                return;
            }
                
            SlowPrintingText.SlowPrintText($"The output has been successfully written in {FileToWriteTo}."); 
        }
        private static string GetFileType(string input)
        {
            string filetype;
            try
            {
                FileInfo fileInfo= new FileInfo(input);
                filetype = fileInfo.Extension;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false.ToString();
            }
            return filetype;
        }
    }
    
}