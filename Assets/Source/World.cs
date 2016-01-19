using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class World
{
    private string worldName;
    private int seed;
    private Thread worldThread;
    private bool worldThreadRunning = false;
    private List<Chunk> _chunks = new List<Chunk>();

    public World(string worldName, int seed)
    {
        this.worldName = worldName;
        this.seed = seed;

        SetupWorld();
    }

    private void SetupWorld()
    {
        worldThreadRunning = true;
        worldThread = new Thread(Tick);
        worldThread.Start();
    }

    private void Tick()
    {
        while (worldThreadRunning)
        {
            try
            {
                if (_chunks.Count < 1)
                {
                    _chunks.Add(new Chunk(new Int2nd(0, 0), this));
                }
            }
            catch (System.Exception e)
            {
                Logger.Log(e.StackTrace);
            }
        }
    }

    public void RequestWorldThreadStop()
    {
        worldThreadRunning = false;
    }

    public string GetWorldName()
    {
        return worldName;
    }

    public Thread GetWorldThread()
    {
        return worldThread;
    }
}
