using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Logger
{
    private static List<string> _log = new List<string>();

    public static void Log(string _ToLog)
    {
        _log.Add(_ToLog);
    }
    public static void WriteLog()
    {
        if(_log.Count > 0)
        {
            foreach(string s in new List<string>(_log))
            {
                AppendLog(s);
                _log.Remove(s);
            }
        }
    }

    private static void AppendLog(string s)
    {
        System.IO.File.AppendAllText(FileManager.GetLogDir(), s + System.Environment.NewLine);
    }
}
