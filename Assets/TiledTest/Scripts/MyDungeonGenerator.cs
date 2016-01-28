using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using _Dungeon = MyDungeon.Dungeon;

public class MyDungeonGenerator : MonoBehaviour
{
    void Start()
    {
        System.Random random = new System.Random("0".GetHashCode());
        _Dungeon dungeon = new _Dungeon(20, 20, random, PrintOutput);
    }

    public void PrintOutput(_Dungeon.Tile[,] tileMap)
    {
        _Dungeon.Tile[,] myTileMap = tileMap;
        string output = "";
        for (int y = 0; y < tileMap.GetLength(1); y++)
        {
            for (int x = 0; x < tileMap.GetLength(0); x++)
            {
                output += ((int)(myTileMap[x, y].type)).ToString();
            }
            output += Environment.NewLine;
        }

        Debug.Log(output);
    }
}

namespace MyDungeon
{
    public class Dungeon
    {
        //dungeon map size
        int sizeX, sizeY;
        //lists
        List<Room> rooms;
        List<Corridor> corridors;
        List<Item> items;
        //random generator
        System.Random random;

        //2d tile map array
        Tile[,] tileMap;
        Action<Tile[,]> callback;

        public Dungeon(int sizeX, int sizeY, System.Random randomGenerator, Action<Tile[,]> callback)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            random = randomGenerator;
            this.callback = callback;

            GenerateDungeon();
        }

        void GenerateDungeon()
        {
            tileMap = new Tile[sizeX, sizeY];
            Initialize();
            Coord startRoomCenter = new Coord(sizeX / 2, sizeY / 2); //starting room in the middle fo the map
            //Debug.Log("center of the map = " + sizeX / 2 + "," + sizeY / 2);
            int startRoomSizeX = 8;
            int startRoomSizeY = 6;
            MakeRoom(startRoomCenter, startRoomSizeX, startRoomSizeY);
            MakeRoom(new Coord(2, 2), 4, 4);
            //criar algoritmo que decide onde enfiar as salas
            callback(tileMap);
        }

        void MakeRoom(Coord center, int sizeX, int sizeY)
        {
            if (sizeX < 3 || sizeY < 3)
            {
                Debug.Log("Room too small: " + sizeX + "," + sizeY);
                return;
            }
            Room room = new Room(center, sizeX, sizeY);
            room.CreateRoom(ref tileMap);
            rooms.Add(room);
        }

        public bool IsInsideMap(int x, int y)
        {
            bool inside = x >= 0 && x < sizeX && y >= 0 && y < sizeY;
            return inside;
        }

        public bool IsInsideMap(Coord coord)
        {
            return IsInsideMap(coord.x, coord.y);
        }

        private void Initialize()
        {
            rooms = new List<Room>();

            for (int x = 0; x < tileMap.GetLength(0); x++)
            {
                for (int y = 0; y < tileMap.GetLength(1); y++)
                {
                    tileMap[x, y] = new Tile(new Coord(x, y), TileType.unused);
                }
            }
        }

        public enum Direction
        {
            North,
            East,
            South,
            West
        }

        public class Coord
        {
            public int x;
            public int y;

            public Coord(int x = -1, int y = -1)
            {
                this.x = x;
                this.y = y;
            }
        }

        public class Tile
        {
            public Coord coord;

            public TileType type;

            public Tile(TileType type = TileType.unused)
            {
                this.coord = new Coord();
                this.type = type;
            }

            public Tile(Coord coord, TileType type = TileType.unused) : this(type)
            {
                this.coord = coord;
            }
        }

        public class Room
        {
            public Coord center;
            public int sizeX, sizeY;

            public Tile[,] roomTiles;

            public List<Room> connectedRooms;

            public Room(Coord center, int sizeX, int sizeY)
            {
                this.center = center;
                this.sizeX = sizeX;
                this.sizeY = sizeY;
            }

            public void CreateRoom(ref Tile[,] tileMap)
            {
                roomTiles = new Tile[sizeX, sizeY];

                for (int x = 0; x < sizeX; x++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        TileType type;
                        if (x == 0 || x == sizeX - 1 || y == 0 || y == sizeY - 1)
                        {
                            type = TileType.wall;
                        }
                        else
                        {
                            type = TileType.floor;
                        }
                        int xWorldPos = center.x - sizeX / 2 + x;
                        int yWorldPos = center.y - sizeY / 2 + y;
                        
                        //Debug.Log("pos = " + xWorldPos + "," + yWorldPos + " - " + type.ToString());
                        Coord coord = new Coord(xWorldPos, yWorldPos);
                        Tile tile = new Tile(coord, type);

                        roomTiles[x, y] = tileMap[xWorldPos, yWorldPos] = tile;
                    }
                }
            }
        }

        public class Corridor
        {
            public List<Room> connectingRooms;
        }

        public class Item
        {
            Coord coord;

            public ItemType type;


        }
    }
}
public enum TileType
{
    unused,
    wall,
    floor,
}
public enum ItemType
{
    type1,
    type2
}