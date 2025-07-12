using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TikFinity.Models
{
    public static class Logger
    {
        public static void LogMessage(string message)
        {
            string directoryPath = "./scripts"; // Define the folder path (relative or absolute)
            string filePath = Path.Combine(directoryPath, "debug_log.txt");
            string filePath3 = Path.Combine(directoryPath, "array.txt");
            string filePath2 = Path.Combine(directoryPath, "count.txt");
            // Ensure the directory exists
            Directory.CreateDirectory(directoryPath);

            // Append the message to the file (or create the file if it doesn't exist)
            File.AppendAllText(filePath, $"{DateTime.Now}: {message}\n");

        }

        public static Gifts GetGifts()
        {
            string directoryPath = "./scripts"; // Define the folder path (relative or absolute)
            string filePath = Path.Combine(directoryPath, "gifts.json");
            string jsonContent = File.ReadAllText(filePath);
            var giftdetails = JsonConvert.DeserializeObject<Gifts>(jsonContent);

            return giftdetails;
        }
    }
}
