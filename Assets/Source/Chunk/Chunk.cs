using UnityEngine;
using System.Collections;
using System.Threading;
using System;

public class Chunk
{
    private Int2nd location;
    private World world;
    private bool hasGenerated = false;
    private bool hasDrawn = false;

    public delegate void Rundel();

    public Chunk(Int2nd location, World world)
    {
        this.location = location;
        this.world = world;
        Thread t = new Thread(Generate);
        t.Priority = System.Threading.ThreadPriority.Highest;
        t.Start();
    }

    public void Generate()
    {
        hasGenerated = true;
        Draw();
    }

    public void Draw()
    {
        hasDrawn = true;
        WorldManager.GetInstance().RegisterDelegate(new Rundel(_Draw));
    }

    private void _Draw()
    {
        Logger.Log("Has used _Draw() function");
    }
}
