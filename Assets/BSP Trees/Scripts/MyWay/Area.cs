using System;
using System.Collections.Generic;
using UnityEngine;

public class Area
{
    public int startX;
    public int startY;

    public int sizeX;
    public int sizeY;

    public Coord center;
    public Coord size;

    public Area()
    {

    }

    public Area(int startX, int startY, int sizeX, int sizeY)
    {
        this.startX = startX;
        this.startY = startY;

        this.sizeX = sizeX;
        this.sizeY = sizeY;

        center = new Coord(startX + sizeX / 2, startY + sizeY / 2);
        size = new Coord(sizeX, sizeY);
    }

    public List<Area> Split(int minXSize, int minYSize)
    {
        List<Area> newAreas = new List<Area>();

        bool splitH = BSPManager.random.NextDouble() > 0.5f;
        if (sizeX / sizeY > 1.25f)
        {
            splitH = true;
        }
        else if (sizeY / sizeX > 1.25f)
        {
            splitH = false;
        }
        int max;
        int splitPos;
        if (splitH)
        {
            max = sizeY - minYSize;
            if (max <= minYSize)
            {
                return newAreas;
            }
            else
            {
                splitPos = BSPManager.random.Next(minYSize, max);
            }
        }
        else
        {
            max = sizeX - minXSize;
            if (max <= minXSize)
            {
                return newAreas;
            }
            else
            {
                splitPos = BSPManager.random.Next(minXSize, max);
            }
        }

        if (splitH)
        {
            newAreas.Add(new Area(startX, startY, sizeX, splitPos));
            newAreas.Add(new Area(startX, startY + splitPos, sizeX, sizeY - splitPos));
        }
        else
        {
            newAreas.Add(new Area(startX, startY, splitPos, sizeY));
            newAreas.Add(new Area(startX + splitPos, startY, sizeX - splitPos, sizeY));
        }
        return newAreas;
    }

    public class Coord
    {
        private int x;
        private int y;

        public Coord()
        {

        }

        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector3 Coord2Vector3()
        {
            return new Vector3(x, 1, y);
        }
    }

}


