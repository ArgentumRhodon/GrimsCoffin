using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class SaveDataWebGL
{
    public static void SaveToJSON<T>(List<T> data, string fileName)
    {
        string content = JsonHelper.ToJson<T>(data.ToArray(), true);
        WriteFile(GetPath("RescueData.json"), content);
    }

    // Existing method for desktop builds.
    public static List<T> ReadFromJSON<T>(string fileName)
    {
        string content = ReadFile(GetPath(fileName));
        return ParseJSON<T>(content);
    }

    // New method to parse JSON directly from a string (for WebGL).
    public static List<T> ReadFromJSONString<T>(string json)
    {
        return ParseJSON<T>(json);
    }

    // Shared JSON parsing logic.
    private static List<T> ParseJSON<T>(string json)
    {
        if (string.IsNullOrEmpty(json) || json == "{}")
        {
            return new List<T>();
        }

        // If the JSON starts with an array, wrap it so JsonHelper can parse it.
        if (json.TrimStart().StartsWith("["))
        {
            json = "{ \"Items\": " + json + " }";
        }

        List<T> res = JsonHelper.FromJson<T>(json).ToList();
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
