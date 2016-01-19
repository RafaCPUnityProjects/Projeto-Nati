using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

public class WorldManager : MonoBehaviour
{

    private static WorldManager _Instance;
    private World _world;
    private List<Delegate> _Rundels = new List<Delegate>();

    void Start()
    {
        string worldName = "Dev";
        FileManager.SetupFileManager(worldName);
        Logger.Log("WorldManager Initialized");

        _Instance = this;
        _world = new World(worldName, worldName.GetHashCode());
        Logger.Log("World Initialized");
    }

    public void RegisterDelegate(Delegate d)
    {
        _Rundels.Add(d);
    }

    public World GetWorld()
    {
        return _world;
    }

    void Update()
    {
        RunAllDelegates();
    }

    void RunAllDelegates()
    {
        List<Delegate> rundelCopy = null;
        lock (_Rundels)
        {
            rundelCopy = new List<Delegate>(_Rundels);
        }
        foreach (Delegate d in rundelCopy)
        {
            if (d != null)
            {
                try
                {
                    d.DynamicInvoke();

                }
                catch (Exception e)
                {
                    Logger.Log(e.StackTrace);
                    Debug.Log(e.StackTrace);
                }

                _Rundels.Remove(d);
            }
        }
    }

    void OnApplicationQuit()
    {
        _world.RequestWorldThreadStop();
        while (_world.GetWorldThread().IsAlive) ;
        Logger.Log("Worldthread succesfully closed");

        Logger.WriteLog();
    }

    public static WorldManager GetInstance()
    {
        return _Instance;
    }
}
