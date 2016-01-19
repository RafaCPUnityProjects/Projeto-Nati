using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class FileManager
{
    public static readonly string SaveDir = "Saves/";
    private static string loggerPath;

    public static void SetupFileManager(string worldName)
    {
        CheckCreateDirectory(SaveDir + worldName + "/Log/");
        loggerPath = SaveDir + worldName + "/Log/Log.txt";
        File.WriteAllText(loggerPath, "");

    }

    public static void CheckCreateDirectory(string dir)
    {
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    public static string GetLogDir()
    {
        return loggerPath;
    }
}
