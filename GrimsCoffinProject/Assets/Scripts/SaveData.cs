using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
// Code sourced from https://www.youtube.com/watch?v=KZft1p8t2lQ
public static class SaveData
{
    public static void SaveToJSON<T>(List<T> data, string fileName)
    {
        string content = JsonHelper.ToJson<T>(data.ToArray(), true);
        WriteFile(GetPath("RescueData.json"), content);
    }

    public static List<T> ReadFromJSON<T>(string fileName)
    {
        string content = ReadFile(GetPath(fileName));
        if (string.IsNullOrEmpty(content) || content == "{}")
        {
            return new List<T>();
        }

        if (content.TrimStart().StartsWith("["))
        {
            content = "{ \"Items\": " + content + " }";
        }

        List<T> res = JsonHelper.FromJson<T>(content).ToList();
        return res;
    }
    private static string GetPath(string fileName)
    {
        return Application.dataPath + "/" + fileName;
    }
    private static void WriteFile(string path, string content)
    {
        FileStream fileStream = new FileStream(path, FileMode.Create);
        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(content);
        }
    }
    private static string ReadFile(string path)
    {
        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string content = reader.ReadToEnd();
                return content;
            }
        }
        return "";
    }
}