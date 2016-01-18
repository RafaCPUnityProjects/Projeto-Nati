using UnityEngine;
using System.Collections;

public class WorldManager : MonoBehaviour
{

    private static WorldManager _Instance;
    private World _world;

    void Start()
    {
        string worldName = "Dev";
        FileManager.SetupFileManager(this);
        _Instance = this;
        _world = new World(worldName, worldName.GetHashCode());
    }

    public World GetWorld()
    {
        return _world;
    }

    void Update()
    {

    }

    void OnApplicationQuit()
    {

    }
}
