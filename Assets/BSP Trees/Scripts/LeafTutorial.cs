using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Room = Leaf.Room;

public class LeafTutorial : MonoBehaviour
{
    public int width;
    public int height;

    //public string seed;
    public bool useRandomSeed;

    //[Range(0, 10)]
    //public int borderSize = 5;

    int[,] map;

    public static string seed = "0";
    public static System.Random random = new System.Random(seed.GetHashCode());
    List<Room> debug_rooms;

    public static int MIN_LEAF_SIZE = 10;
    public static int MAX_LEAF_SIZE = 20;

    //List<Leaf> _leafs = new List<Leaf>();

    private int[,] _mapData;        // our map Data - we draw our map here to be turned into a tilemap later
    private List<Room> _rooms;  // a Vector that holds all our rooms
    private List<Room> _halls;  // a Vector that holds all our halls
    private List<Leaf> _leafs;      // a Vector that holds all our leafs


    void Start()
    {
        GenerateMap();
    }

    //private void GenerateMap()
    //{
    //    map = new int[width, height];
    //    debug_leafs = new List<Leaf>();

    //    SubdvideLeafs();

    //    //ProcessMap();
    //    //AddBorderToMap();
    //    //DrawMap();
    //}

    private void SubdvideLeafs()
    {
        // first, create a Leaf to be the 'root' of all Leafs.
        Leaf root = new Leaf(0, 0, width, height);
        _leafs.Add(root);

        bool did_split = true;
        // we loop through every Leaf in our Vector over and over again, until no more Leafs can be split.
        while (did_split)
        {
            did_split = false;
            for (int i = 0; i < _leafs.Count; i++)
            {
                Leaf leaf = _leafs[i];

                if (leaf.leftChild == null && leaf.rightChild == null) // if this Leaf is not already split...
                {
                    // if this Leaf is too big, or 75% chance...
                    if (leaf.width > MAX_LEAF_SIZE || leaf.height > MAX_LEAF_SIZE || random.NextDouble() > 0.25)
                    {
                        if (leaf.Split()) // split the Leaf!
                        {
                            // if we did split, push the child leafs to the Vector so we can loop into them next
                            _leafs.Add(leaf.leftChild);
                            _leafs.Add(leaf.rightChild);
                            //_leafs.Remove(leaf);
                            did_split = true;
                        }
                    }
                }
            }
        }
        //debug_rooms = _leafs;
        //print(debug_rooms.Count);
    }

    private void GenerateMap()
    {
        // reset our mapData
        _mapData = new int[width, height];

        // reset our Vectors 
        _rooms = new List<Room>();
        _halls = new List<Room>();
        _leafs = new List<Leaf>();

        //leaf l; // helper leaf

        // first, create a leaf to be the 'root' of all leaves.
        Leaf root = new Leaf(0, 0, width, height);
        _leafs.Add(root);

        bool did_split = true;
        // we loop through every leaf in our Vector over and over again, until no more leafs can be split.
        while (did_split)
        {
            did_split = false;
            for (int i = 0; i < _leafs.Count; i++)
            {
                var l = _leafs[i];

                if (l.leftChild == null && l.rightChild == null) // if this leaf is not already split...
                {
                    // if this leaf is too big, or 75% chance...
                    if (l.width > MAX_LEAF_SIZE || l.height > MAX_LEAF_SIZE || random.NextDouble() > 0.25)
                    {
                        if (l.Split()) // split the leaf!
                        {
                            // if we did split, push the child leafs to the Vector so we can loop into them next
                            _leafs.Add(l.leftChild);
                            _leafs.Add(l.rightChild);
                            did_split = true;
                        }
                    }
                }
            }
        }

        // next, iterate through each leaf and create a room in each one.
        root.CreateRooms();

        debug_rooms = new List<Room>();

        foreach (var l in _leafs)
        {
            // then we draw the room and hallway if it exists
            if (l.room != null)
            {
                DrawRoom(l.room);
            }

            if (l.halls != null && l.halls.Count > 0)
            {
                DrawHalls(l.halls);
            }
        }

        // randomly pick one of the rooms for the player to start in...
        //var startRoom:Rectangle = _rooms[Registry.randomNumber(0, _rooms.length - 1)];
        // and pick a random tile in that room for them to start on.
        //var _playerStart:Point = new Point(Registry.randomNumber(startRoom.x, startRoom.x + startRoom.width - 1), Registry.randomNumber(startRoom.y, startRoom.y + startRoom.height - 1));

        // move the player sprite to the starting location (to get ready for the user to hit 'play')
        //_player.x = _playerStart.x * 16 + 1;
        //_player.y = _playerStart.y * 16 + 1;

        // make our map Sprite's pixels a copy of our map Data BitmapData. Tell flixel the sprite is 'dirty' (so it flushes the cache for that sprite)
        //_sprMap.pixels = _mapData.clone();
        //_sprMap.dirty = true;

        //_buttonPlaymap.visible = true;

    }

    private void DrawHalls(List<Room> h)
    {
        // add each hall to the hall vector, and draw the hall onto our mapData
        foreach (var r in h)
        {
            _halls.Add(r);
            //draw
            debug_rooms.Add(r);
        }
    }

    private void DrawRoom(Room r)
    {
        // add this room to the room vector, and draw the room onto our mapData
        _rooms.Add(r);
        //draw
        debug_rooms.Add(r);

    }

    private void ProcessMap()
    {
        throw new NotImplementedException();
    }

    private void AddBorderToMap()
    {
        throw new NotImplementedException();
    }

    private void DrawMap()
    {
        throw new NotImplementedException();
    }

    struct Coord
    {
        public int tileX;
        public int tileY;

        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
    }

    //class Room : IComparable<Room>
    //{
    //    public List<Coord> tiles;
    //    public List<Coord> edgeTiles;
    //    public List<Room> connectedRooms;
    //    public int roomSize;
    //    public bool isMainRoom;
    //    public bool isAccessibleFromMainRoom;

    //    public Room() { }

    //    public Room(List<Coord> roomTiles, int[,] map)
    //    {
    //        tiles = roomTiles;
    //        roomSize = tiles.Count;

    //        connectedRooms = new List<Room>();
    //        edgeTiles = new List<Coord>();
    //        foreach (Coord tile in tiles)
    //        {
    //            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
    //            {
    //                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
    //                {
    //                    if (x == tile.tileX || y == tile.tileY)
    //                    {
    //                        if (map[x, y] == 1)
    //                        {
    //                            edgeTiles.Add(tile);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    public void SetAccessibleFromMainRoom()
    //    {
    //        if (!isAccessibleFromMainRoom)
    //        {
    //            isAccessibleFromMainRoom = true;
    //            foreach (Room connectedRoom in connectedRooms)
    //            {
    //                connectedRoom.isAccessibleFromMainRoom = true;
    //            }
    //        }
    //    }

    //    public static void ConnectRooms(Room roomA, Room roomB)
    //    {
    //        if (roomA.isAccessibleFromMainRoom)
    //        {
    //            roomB.SetAccessibleFromMainRoom();
    //        }
    //        else if (roomB.isAccessibleFromMainRoom)
    //        {
    //            roomA.SetAccessibleFromMainRoom();
    //        }
    //        roomA.connectedRooms.Add(roomB);
    //        roomB.connectedRooms.Add(roomA);
    //    }

    //    public bool IsConnected(Room otherRoom)
    //    {
    //        return connectedRooms.Contains(otherRoom);
    //    }

    //    public int CompareTo(Room otherRoom)
    //    {
    //        return otherRoom.roomSize.CompareTo(roomSize);
    //    }
    //}

    void OnDrawGizmos()
    {
        if (debug_rooms != null)
        {
            for (int i = 0; i < debug_rooms.Count; i++)
            {
                Gizmos.color = debug_rooms[i].isARoom ? new Color(1,1,1,0.25f) : Color.black;
                Vector3 center = new Vector3(debug_rooms[i].Center.x, i, debug_rooms[i].Center.z);
                Gizmos.DrawCube(center, debug_rooms[i].Size * .99f);
            }
        }
    }
}
