using UnityEngine;
using System.Collections;

public class World
{
    private string worldName;
    private int seed;

    public World(string worldName, int seed)
    {
        this.worldName = worldName;
        this.seed = seed;

        SetupWorld();
    }

    private void SetupWorld()
    {

    }
}
