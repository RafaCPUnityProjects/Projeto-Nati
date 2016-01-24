using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class BSPManager : MonoBehaviour
{
    public static string seed;
    public static System.Random random = new System.Random(seed.GetHashCode());
    public int width = 80;
    public int height = 100;
    public int maxXSize = 20;
    public int maxYSize = 25;
    public int minXSize = 8;
    public int minYSize = 10;

    List<Area> areas = new List<Area>();
    void Start()
    {
        GenerateMap();
    }

    private void GenerateMap()
    {
        Area startingArea = new Area(width / 2, height / 2, width, height);
        areas.Clear();
        areas.Add(startingArea);
        while (true)
        {
            for (int i = 0; i < areas.Count; i++)
            {
                List<Area> newAreas = areas[i].Split(minXSize, minYSize);
                if (newAreas == null || newAreas.Count < 1)
                {
                    break;
                }
                else
                {
                    areas.AddRange(areas[i].Split(minXSize, minYSize));
                    areas.Remove(areas[i]);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if(areas != null && areas.Count > 1)
        {
            for (int i = 0; i < areas.Count; i++)
            {
                print("DrawGizmos: " + i);
                Gizmos.color = Color.white;
                Gizmos.DrawCube(areas[i].center.Coord2Vector3(),areas[i].size.Coord2Vector3());
            }
        }
    }
}
