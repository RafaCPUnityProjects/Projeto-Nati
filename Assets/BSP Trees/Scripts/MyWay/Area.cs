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
    }

    public List<Area> Split()
    {
        List<Area> newAreas = new List<Area>();

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

        public Vector3 Coord2Vector3(Coord coord)
        {
            return new Vector3(coord.x, 0, coord.y);
        }
    }

}


