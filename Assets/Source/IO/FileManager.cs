using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class FileManager
{
    public static readonly string saveDir = "Saves/";
    public static readonly string blockTexturesDir = "Resources/Textures/Block/";
    private static string loggerPath;

    public static void SetupFileManager(string worldName)
    {
        CheckCreateDirectory(saveDir + worldName + "/Log/");
        loggerPath = saveDir + worldName + "/Log/Log.txt";
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
