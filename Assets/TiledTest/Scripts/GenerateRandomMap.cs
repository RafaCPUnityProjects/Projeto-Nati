using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GenerateRandomMap : MonoBehaviour
{
    public string seed;
    public bool useRandomSeed = false;

    public GameObject[] rooms3x3;
    public GameObject[] rooms5x5;
    public GameObject[] rooms7x7;
    public GameObject[] rooms10x10;


    public Vector2[] posOf3x3Rooms;
    public Vector2[] posOf5x5Rooms;
    public Vector2[] posOf7x7Rooms;
    public Vector2[] posOf10x10Rooms;

    public int numberOf3x3Rooms;
    public int numberOf5x5Rooms;
    public int numberOf7x7Rooms;
    public int numberOf10x10Rooms;

    List<GameObject[]> allRooms;
    int totalNumberOfRooms;

    public float tileSize;

    System.Random random;

    void Start()
    {
        InitializeSeed();
        initializeRoomList();
        GenerateMap();
    }

    private void initializeRoomList()
    {
        totalNumberOfRooms = numberOf3x3Rooms + numberOf5x5Rooms + numberOf7x7Rooms + numberOf10x10Rooms;
        allRooms = new List<GameObject[]>();
        allRooms.Add(rooms3x3);
        allRooms.Add(rooms5x5);
        allRooms.Add(rooms7x7);
        allRooms.Add(rooms10x10);
    }

    void InitializeSeed()
    {
        if(useRandomSeed)
        {
            seed += System.DateTime.Now.ToString();
            print("Random seed = " + seed);
        }
        random = new System.Random(seed.GetHashCode());
    }

    Bounds GetBounds(GameObject room)
    {
        Bounds roomBounds = room.GetComponentInChildren<MeshFilter>().mesh.bounds;

        return roomBounds;
    }

    int RandomRoomSize()
    {
        return random.Next(allRooms.Count);
    }

    int RandomRoomNumber(int roomsPosInAllRooms)
    {
        return random.Next(allRooms[roomsPosInAllRooms].Length);
    }

    Vector3 RandomDirection(Bounds firstRoom, Bounds secondRoom)
    {
        int randomDirection = random.Next(4);
        Vector3 worldPos = firstRoom.center;
        print(worldPos);
        switch (randomDirection)
        {
            case 0: //right
                print("right");
                worldPos.x += (firstRoom.extents.x + secondRoom.extents.x);
                break;
            case 1: //down
                print("down");
                worldPos.y -= (firstRoom.extents.y + secondRoom.extents.y);
                break;
            case 2: //left
                print("left");
                worldPos.x -= (firstRoom.extents.x + secondRoom.extents.x);
                break;
            case 3://up
                print("up");
                worldPos.y += (firstRoom.extents.y + secondRoom.extents.y);
                break;
            case 4:
                print("SWITCH OUT OF BOUNDS");
                break;
        }
        //print("world pos = " + worldPos);
        worldPos -= secondRoom.center;
        return worldPos;
    }

    private void GenerateMap()
    {
        List<GameObject> rooms = new List<GameObject>();
        List<Bounds> bounds = new List<Bounds>();
        //start room is a big room
        rooms.Add(Instantiate(rooms10x10[RandomRoomNumber(3)], Vector3.zero, Quaternion.identity) as GameObject);
        Bounds bound0 = GetBounds(rooms[0]);
        for (int i = 1; i < totalNumberOfRooms; i++)
        {
            int rndSize = RandomRoomSize();
            GameObject newRoom = Instantiate(allRooms[rndSize][RandomRoomNumber(rndSize)]) as GameObject;
            rooms.Add(newRoom);
            Bounds newRoomBounds = GetBounds(newRoom);
            bounds.Add(newRoomBounds);
            newRoom.transform.position = RandomDirection(GetBounds(rooms[i - 1]), newRoomBounds);
            print("parent = " + rooms[i - 1].name + "-" + rooms[i - 1].transform.position);
            print("child = " + newRoom.name + " - " + newRoom.transform.position);
        }

        //// start room
        ////10x10
        //for (int i = 0; i < posOf10x10Rooms.Length; i++)
        //{
        //    GameObject room = Instantiate(rooms10x10[random.Next(rooms10x10.Length)], posOf10x10Rooms[i] * tileSize, Quaternion.identity) as GameObject;
        //    rooms.Add(room);
        //}
        ////7x7
        //for (int i = 0; i < posOf7x7Rooms.Length; i++)
        //{
        //    GameObject room = Instantiate(rooms7x7[random.Next(rooms7x7.Length)], posOf7x7Rooms[i] * tileSize, Quaternion.identity) as GameObject;
        //    rooms.Add(room);
        //}
        ////5x5
        //for (int i = 0; i < posOf5x5Rooms.Length; i++)
        //{
        //    GameObject room = Instantiate(rooms5x5[random.Next(rooms5x5.Length)], posOf5x5Rooms[i] * tileSize, Quaternion.identity) as GameObject;
        //    rooms.Add(room);
        //}
        ////3x3
        //for (int i = 0; i < posOf3x3Rooms.Length; i++)
        //{
        //    GameObject room = Instantiate(rooms3x3[random.Next(rooms3x3.Length)], posOf3x3Rooms[i] * tileSize, Quaternion.identity) as GameObject;
        //    rooms.Add(room);
        //}
    }

}
