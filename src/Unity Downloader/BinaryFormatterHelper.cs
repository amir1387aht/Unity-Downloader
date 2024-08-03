using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Unity_Downloader
{
    internal class BinaryFormatterHelper
    {
        public static void SerializeToFile<T>(string filePath, T obj)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, obj);
            }
        }

        public static void SerializeToFile<T>(FileStream fileStream, T obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fileStream, obj);
        }

        public static T DeserializeFromFile<T>(string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(fileStream);
            }
        }

        public static T DeserializeFromFile<T>(StreamReader reader)
        {
            // Read the binary data as a string
            string base64String = reader.ReadToEnd();
            byte[] bytes = Convert.FromBase64String(base64String);

            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(memoryStream);
            }
        }
    }
}