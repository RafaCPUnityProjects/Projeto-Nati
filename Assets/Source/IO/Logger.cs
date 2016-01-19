using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Logger
{
    private static List<string> _Log = new List<string>();

    public static void Log(string _ToLog)
    {
        _Log.Add(_ToLog);
    }
    public static void WriteLog()
    {
        if(_Log.Count > 0)
        {
            foreach(string s in new List<string>(_Log))
            {
                AppendLog(s);
                _Log.Remove(s);
            }
        }
    }

    private static void AppendLog(string s)
    {
        System.IO.File.AppendAllText(FileManager.GetLogDir(), s + System.Environment.NewLine);
    }
}
