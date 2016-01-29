using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using _Dungeon = MyDungeon.Dungeon;

public class MyDungeonGenerator : MonoBehaviour
{

    public int sizeX = 20;
    public int sizeY = 20;
    public Vector2 roomSizeLimits = new Vector2(3, 9);
    public Vector2 roomRectangularityLimits = new Vector2(0, 4);
    void Start()
    {
        System.Random random = new System.Random("0".GetHashCode());
        _Dungeon dungeon = new _Dungeon(sizeX, sizeY, roomSizeLimits, roomRectangularityLimits, random, PrintOutput);
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
        Vector2 roomSizeLimits;
        Vector2 roomRectangularityLimits;

        //lists
        List<Room> rooms;
        List<Corridor> corridors;
        List<Item> items;
        //random generator
        System.Random random;

        //2d tile map array
        Tile[,] tileMap;
        Action<Tile[,]> callback;

        public Dungeon(int sizeX, int sizeY, Vector2 roomSize, Vector2 rectangularity, System.Random randomGenerator, Action<Tile[,]> callback)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.roomSizeLimits = roomSize;
            this.roomRectangularityLimits = rectangularity;
            random = randomGenerator;
            this.callback = callback;

            GenerateDungeon();
        }

        void GenerateDungeon()
        {
            tileMap = new Tile[sizeX, sizeY];
            Initialize();
            //Coord startRoomCenter = new Coord(sizeX / 2, sizeY / 2); //starting room in the middle fo the map
            ////Debug.Log("center of the map = " + sizeX / 2 + "," + sizeY / 2);
            //int startRoomSizeX = 8;
            //int startRoomSizeY = 6;
            //MakeRoom(startRoomCenter, startRoomSizeX, startRoomSizeY);
            //MakeRoom(new Coord(2, 2), 4, 4);

            //criar algoritmo que decide onde enfiar as salas
            PlaceRooms();
            callback(tileMap);
        }

        void PlaceRooms()
        {
            for (int i = 0; i < 100; i++)
            {
                int roomSize = random.Next((int)roomSizeLimits.x, (int)roomSizeLimits.y);
                int rectangularity = random.Next((int)roomRectangularityLimits.x, (int)roomRectangularityLimits.y); ;
                int width = roomSize;
                int height = roomSize;
                if (random.NextDouble() > 0.5f)
                {
                    width += rectangularity;
                    Debug.Log("random.NextDouble() > 0.5f, width = " + width);
                }
                else
                {
                    height += rectangularity;
                    Debug.Log("random.NextDouble() <= 0.5f, height = " + height);
                }

                int x = random.Next((sizeX - width) / 2) * 2 + 1;
                int y = random.Next((sizeY - height) / 2) * 2 + 1;
                if (OutsideLimits(x, y, width, height))
                {
                    continue;
                }

                Room room = new Room(new Rect(x, y, width, height));

                bool overlaps = false;
                foreach (Room other in rooms)
                {
                    if (other.rect.Overlaps(room.rect))
                    {
                        overlaps = true;
                        break;
                    }
                }

                if (overlaps)
                {
                    continue;
                }

                room.CreateRoom(ref tileMap);
                rooms.Add(room);
            }
        }

        bool OutsideLimits(int x, int y, int width, int height)
        {
            return x - width / 2 < 0 || y - height / 2 < 0 || x + width / 2 > sizeX || y + height / 2 > sizeY;
        }

        //void MakeRoom(Coord center, int sizeX, int sizeY)
        //{
        //    if (sizeX < 3 || sizeY < 3)
        //    {
        //        Debug.Log("Room too small: " + sizeX + "," + sizeY);
        //        return;
        //    }
        //    Room room = new Room(center, sizeX, sizeY);
        //    room.CreateRoom(ref tileMap);
        //    rooms.Add(room);
        //}

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
            public Vector2 pos
            {
                get
                {
                    return new Vector2(x, y);
                }
                set
                {
                    x = (int)value.x;
                    y = (int)value.y;
                }
            }
            public Coord(int x = -1, int y = -1)
            {
                this.x = x;
                this.y = y;
            }
            public Coord(Vector2 pos)
            {
                this.x = (int)pos.x;
                this.y = (int)pos.y;
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
            public Rect rect;

            public Tile[,] roomTiles;

            public List<Room> connectedRooms;

            public Room(Rect rect)
            {
                this.rect = rect;
            }

            public void CreateRoom(ref Tile[,] tileMap)
            {
                int width = (int)rect.width;
                int height = (int)rect.height;
                int centerX = (int)rect.center.x;
                int centerY = (int)rect.center.y;
                roomTiles = new Tile[width, height];
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        TileType type;
                        if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                        {
                            type = TileType.wall;
                        }
                        else
                        {
                            type = TileType.floor;
                        }
                        int xWorldPos = centerX + x - width / 2;
                        int yWorldPos = centerY + y - width / 2;

                        //Debug.Log("pos = " + xWorldPos + "," + yWorldPos + " - " + type.ToString());
                        Coord coord = new Coord(xWorldPos, yWorldPos);
                        Tile tile = new Tile(coord, type);

                        //Debug.Log("worldPos = " + xWorldPos + "," + yWorldPos);
                        roomTiles[x, y] = tileMap[xWorldPos, yWorldPos] = tile;
                    }
                }
            }

            public float DistanceToOther(Room other)
            {
                Vector2 distanceVector = Vector2.zero;
                if (other.rect.center.x > rect.center.x)//right
                {
                    distanceVector.x = (other.rect.center.x - other.rect.width / 2) - (rect.center.x + rect.width / 2);
                }
                else if (other.rect.center.x < rect.center.x)//left
                {
                    distanceVector.x = (rect.center.x - rect.width / 2) - (other.rect.center.x + other.rect.width / 2);
                }
                else//same x, maybe above or bellow
                {
                    distanceVector.x = 0;
                }
                if (other.rect.center.y > rect.center.y) //above
                {
                    distanceVector.y = (other.rect.center.y - other.rect.height / 2) - (rect.center.y + rect.height / 2);
                }
                else if (other.rect.center.y < rect.center.y) // bellow
                {
                    distanceVector.y = (rect.center.y - rect.height / 2) - (other.rect.center.y + other.rect.height / 2);
                }
                else//same y, maybe left or right
                {
                    distanceVector.y = 0;
                }

                float distance = Mathf.Sqrt(Mathf.Pow(distanceVector.x, 2) + Mathf.Pow(distanceVector.y, 2));
                //Debug.Log("Distance between rooms: " + distance);
                return distance;
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