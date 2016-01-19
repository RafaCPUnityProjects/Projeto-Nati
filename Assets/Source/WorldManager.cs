using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

public class WorldManager : MonoBehaviour
{

    private static WorldManager _instance;
    private World _world;
    private List<Delegate> _rundels = new List<Delegate>();

    void Start()
    {
        string worldName = "Dev";
        FileManager.SetupFileManager(worldName);
        Logger.Log("WorldManager Initialized");

        _instance = this;
        _world = new World(worldName, worldName.GetHashCode());
        Logger.Log("World Initialized");
        Block.stone.GetBlockID();
        BlockRegistry.PrintAllBlocks();
    }

    void Update()
    {
        RunAllDelegates();
        Logger.WriteLog();
    }

    public void RegisterDelegate(Delegate d)
    {
        _rundels.Add(d);
    }

    public World GetWorld()
    {
        return _world;
    }

    void RunAllDelegates()
    {
        List<Delegate> rundelCopy = null;
        lock (_rundels)
        {
            rundelCopy = new List<Delegate>(_rundels);
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

                _rundels.Remove(d);
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
        return _instance;
    }
}
