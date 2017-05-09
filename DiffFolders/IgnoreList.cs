using System;
using System.IO;
using System.Xml;
using System.Configuration;
using System.Collections.Generic;

namespace DiffFolders
{
    internal class IgnoreList : Singleton<IgnoreList>
    {
        public Dictionary<string, string[]> ignoreList = new Dictionary<string, string[]>();
     
        public void AddIgnoreList(string path, string[] md5s)
        {
            string[] origMd5s;
            if (ignoreList.TryGetValue(path, out origMd5s))
            {
                ignoreList[path] = md5s;
            }
            else
            {
                ignoreList.Add(path, md5s);
            }
            SaveData();
        }

        public bool RemoveListExists(string path)
        {
            string[] origMd5s;
            if (ignoreList.TryGetValue(path, out origMd5s))
            {
                ignoreList.Remove(path);
                SaveData();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IgnoreListExists(string path, string[] md5s)
        {
            string[] origMd5s;
            if (ignoreList.TryGetValue(path, out origMd5s))
            {
                if (md5s[0].Equals(origMd5s[0]))
                {
                    if (md5s[1].Equals(origMd5s[1]))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (md5s[0].Equals(origMd5s[1]))
                {
                    if (md5s[1].Equals(origMd5s[0]))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void SaveData()
        {
            using (Stream stream = File.Open("data.bin", FileMode.Create))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bformatter.Serialize(stream, ignoreList);
            }
        }

        public void LoadData()
        {
            try
            {
                using (Stream stream = File.Open("data.bin", FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    ignoreList = (Dictionary<string, string[]>)bformatter.Deserialize(stream);
                }
            }
            catch
            {
                Console.WriteLine("Data file not found");
            }
        }
    }
}