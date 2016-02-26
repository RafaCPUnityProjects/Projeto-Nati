﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Retroboy
{
	public class DungeonGenerator : MonoBehaviour
	{

		//random stuff
		public string seed = "0";
		public bool randomSeed = false;
		public System.Random prng;
		//map stuff
		public int mapWidth = 100;
		public int mapHeight = 100;
		//room stuff
		public int roomTries = 100;
		public VecInt roomXVariance = new VecInt(5, 7);
		public VecInt roomYVariance = new VecInt(5, 7);
		public int wallDepth = 2;
		public int connectionsPerRoom = 3;
		public float roomConnectionTreshold = 7f;
		//print stuff
		public GameObject[] tilePrefabs;
		public Transform mapRootGO;
		public bool printColors = true;

		//private stuff
		private int[,] map;
		private List<Room> allRooms = new List<Room>();
		private System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
		private int currentRegion = -1;


		void Start()
		{
			if (randomSeed)
			{
				seed += DateTime.Now.ToString();
			}
			prng = new System.Random(seed.GetHashCode());

			GenerateDungeon();
		}

		void GenerateDungeon()
		{
			//start stopwatch
			sw.Start();

			//initialize map[,]
			map = new int[mapWidth, mapHeight];
			print("Map Created: " + sw.Elapsed.ToString());

			//create rooms
			CreateRooms();
			print("Rooms Created: " + sw.Elapsed.ToString());

			//connect rooms
			ConnectRooms();
			print("Rooms connected: " + sw.Elapsed.ToString());

			//print result
			PrintMap();
			print("Map Printed: " + sw.Elapsed.ToString());

			//stop stopwatch
			sw.Stop();
		}

		void CreateRooms()
		{
			for (int i = 0; i < roomTries; i++)
			{
				Rect newRoom = new Rect(
					prng.Next(mapWidth), //x
					prng.Next(mapHeight), //y
					prng.Next(roomXVariance.x, roomXVariance.y + 1), //sizeX
					prng.Next(roomYVariance.x, roomYVariance.y + 1)); //sizeY

				if (newRoom.xMax >= mapWidth || newRoom.yMax >= mapHeight)
				{
					continue;
				}

				bool overlaps = false;
				foreach (var room in allRooms)
				{
					if (newRoom.Overlaps(room.roomSpace))
					{
						overlaps = true;
						continue;
					}
				}
				if (overlaps)
				{
					continue;
				}
				allRooms.Add(new Room(newRoom, AddRegion()));
				SetRoomInMap(newRoom);
			}
			print("Number of rooms: " + allRooms.Count);
		}

		int AddRegion()
		{
			currentRegion++;
			return currentRegion;
		}

		void SetRoomInMap(Rect newRoom)
		{
			for (int y = (int)newRoom.y; y < (int)newRoom.yMax; y++)
			{
				for (int x = (int)newRoom.x; x < (int)newRoom.xMax; x++)
				{
					int type = 2; //floor
					if (x < (int)newRoom.x + wallDepth ||
						x >= (int)newRoom.xMax - wallDepth ||
						y < (int)newRoom.y + wallDepth ||
						y >= (int)newRoom.yMax - wallDepth)
					{
						type = 0; //wall
					}
					SetTile(x, y, type);
				}
			}
		}

		void ConnectRooms()
		{
			allRooms.Sort();
			FindNeighborRooms();
			Connect();

		}

		void Connect()
		{
			for (int i = 0; i < allRooms.Count; i++)
			{
				allRooms[i].roomDistances.Sort();
				for (int j = 0; j < connectionsPerRoom; j++)
				{
					if (allRooms[i].roomDistances[j].distance < roomConnectionTreshold)
					{
						ConnectRoomsWithCorridors(allRooms[i], allRooms[i].roomDistances[j].room);
					}
				}
			}
		}

		void ConnectRoomsWithCorridors(Room roomA, Room roomB)
		{
			VecInt centerA = new VecInt(roomA.roomSpace.center);
			VecInt centerB = new VecInt(roomB.roomSpace.center);

			int xDist = Mathf.Abs(centerB.x - centerA.x);
			int yDist = Mathf.Abs(centerB.y - centerA.y);

			int xStart, xEnd, yStart, yEnd;

			if (centerA.x <= centerB.x)
			{
				xStart = centerA.x;
				xEnd = centerB.x;
			}
			else
			{
				xStart = centerB.x;
				xEnd = centerA.x;
			}

			if (centerA.y <= centerB.y)
			{
				yStart = centerA.y;
				yEnd = centerB.y;
			}
			else
			{
				yStart = centerB.y;
				yEnd = centerA.y;
			}

			if (xDist <= yDist)
			{
				for (int x = xStart; x <= xEnd; x++)
				{
					SetTile(x, yStart, 2);
					SetTile(x, yStart + 1, 2);
				}

				for (int y = yStart; y <= yEnd; y++)
				{
					SetTile(xEnd, y, 2);
					SetTile(xEnd + 1, y, 2);
				}
			}
			else
			{
				for (int y = yStart; y <= yEnd; y++)
				{
					SetTile(xStart, y, 2);
					SetTile(xStart + 1, y, 2);
				}

				for (int x = xStart; x <= xEnd; x++)
				{
					SetTile(x, yEnd, 2);
					SetTile(x, yEnd + 1, 2);
				}
			}
		}

		bool InBoundaries(VecInt pos)
		{
			return pos.x > 0 && pos.x < mapWidth - 1 && pos.y > 0 && pos.y < mapHeight - 1;
		}

		void FindNeighborRooms()
		{
			for (int i = 0; i < allRooms.Count; i++)
			{
				for (int j = 0; j < allRooms.Count; j++)
				{
					if (i != j) //dont look at the same room
					{
						float distance = Vector2.Distance(allRooms[i].roomSpace.center, allRooms[j].roomSpace.center);
						allRooms[i].roomDistances.Add(new RoomDistance(allRooms[j], distance));
					}
				}
			}
		}

		//private Room FindClosestRoom(int i, bool secondPass = false)
		//{
		//	float minDistance = float.MaxValue;
		//	int minIndex = -1;
		//	for (int j = 0; j < allRooms.Count; j++)
		//	{
		//		if (i == j)
		//		{
		//			continue;
		//		}
		//		if (!secondPass && allRooms[j].foundClosest)
		//		{
		//			continue;
		//		}
		//		if (allRooms[i].closestRoom == allRooms[j])
		//		{
		//			continue;
		//		}

		//		float distance = Vector2.Distance(allRooms[i].roomSpace.center, allRooms[j].roomSpace.center);
		//		if (distance < minDistance)
		//		{
		//			minDistance = distance;
		//			minIndex = j;
		//		}
		//	}
		//	if (minIndex != -1)
		//	{
		//		print("closest room to " + i + " is " + minIndex);
		//		return allRooms[minIndex];
		//	}
		//	else
		//	{
		//		print("couldnt find closest room");
		//		return null;
		//	}
		//}

		void PrintMap()
		{
			Vector2 tileSize = tilePrefabs[0].GetComponent<SpriteRenderer>().sprite.bounds.size;
			for (int y = 0; y < mapHeight; y++)
			{
				for (int x = 0; x < mapWidth; x++)
				{
					Vector3 pos = new Vector3(tileSize.x * x, tileSize.y * y);
					GameObject tile = Instantiate(tilePrefabs[GetTile(x, y)], pos, Quaternion.identity) as GameObject;
					if (printColors)
					{
						foreach (var room in allRooms)
						{
							if (room.Contains(x, y))
							{
								tile.GetComponent<SpriteRenderer>().color = room.color;
								continue;
							}
						}
					}
					tile.transform.parent = mapRootGO;
				}
			}
		}

		void SetTile(VecInt pos, int type)
		{
			SetTile(pos.x, pos.y, type);
		}

		void SetTile(int posX, int posY, int type)
		{
			map[posX, posY] = type;
		}

		int GetTile(VecInt pos)
		{
			return GetTile(pos.x, pos.y);
		}

		int GetTile(int posX, int posY)
		{
			return map[posX, posY];
		}
	}

	struct RoomDistance : IComparable<RoomDistance>
	{
		public Room room;
		public float distance;

		public RoomDistance(Room otherRoom, float distance) : this()
		{
			this.room = otherRoom;
			this.distance = distance;
		}

		public int CompareTo(RoomDistance other)
		{
			return distance.CompareTo(other.distance);
		}
	}

	class Room : IComparable<Room>
	{
		public Rect roomSpace;
		public List<Room> connectedRooms = new List<Room>();
		public List<RoomDistance> roomDistances = new List<RoomDistance>();
		public bool foundClosest = false;
		public int region;
		public bool hasBeenConnected = false;

		public Color color = new Color();

		public Room() { }

		public Room(Rect roomSpace, int region)
		{
			this.roomSpace = roomSpace;
			this.region = region;
			color = new Color(
					UnityEngine.Random.Range(0f, 1f),
					UnityEngine.Random.Range(0f, 1f),
					UnityEngine.Random.Range(0f, 1f));
		}

		public void Connect(Room other)
		{
			hasBeenConnected = true;
			other.hasBeenConnected = true;
			for (int i = 0; i < connectedRooms.Count; i++)
			{
				connectedRooms[i].region = other.region;
			}
			region = other.region;
			connectedRooms.Add(other);
		}

		public float Size()
		{
			return roomSpace.size.x * roomSpace.size.y;
		}

		public bool Contains(int x, int y)
		{
			return roomSpace.Contains(new Vector2(x, y));
		}

		public void AddClosest(Room otherRoom, float distance)
		{
			roomDistances.Add(new RoomDistance(otherRoom, distance));
			roomDistances.Sort();
			color = roomDistances[0].room.color;
		}

		public bool IsConnected(Room otherRoom)
		{
			return connectedRooms.Contains(otherRoom);
		}

		public int CompareTo(Room otherRoom)
		{
			return otherRoom.Size().CompareTo(Size());
		}
	}

	[System.Serializable]
	public struct VecInt
	{
		public int x;
		public int y;

		public VecInt(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public VecInt(Vector2 vector2)
		{
			x = Mathf.RoundToInt(vector2.x);
			y = Mathf.RoundToInt(vector2.y);
		}

		public VecInt Add(VecInt other)
		{
			return new VecInt(x + other.x, y + other.y);
		}

		public bool Equals(VecInt other)
		{
			return x == other.x && y == other.y;
		}

		public static VecInt Vector2ToVecInt(Vector2 vector2)
		{
			return new VecInt(Mathf.RoundToInt(vector2.x), Mathf.RoundToInt(vector2.y));
		}
	}

	public class Direction
	{
		public static VecInt N = new VecInt(0, 1);
		public static VecInt S = new VecInt(0, -1);
		public static VecInt E = new VecInt(1, 0);
		public static VecInt W = new VecInt(-1, 0);
	}
}