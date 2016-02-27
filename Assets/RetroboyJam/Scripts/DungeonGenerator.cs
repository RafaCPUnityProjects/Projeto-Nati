using UnityEngine;
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
		public bool sanitize = true;
		//room stuff
		public int roomTries = 100;
		public VecInt roomXVariance = new VecInt(5, 7);
		public VecInt roomYVariance = new VecInt(5, 7);
		public int wallDepth = 2;
		public int connectionsPerRoom = 3;
		public float roomConnectionTreshold = 7f;
		//corridor stuff
		public int corridorSize = 2;
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

			//sanitize map
			if (sanitize)
				Sanitize();
			print("Map sanitized: " + sw.Elapsed.ToString());

			//print result
			PrintMap();
			print("Map Printed: " + sw.Elapsed.ToString());

			//stop stopwatch
			sw.Stop();
		}

		private void Sanitize()
		{
			List<VecInt> orthoDirections = new List<VecInt>();
			orthoDirections.Add(Direction.N);
			orthoDirections.Add(Direction.S);
			orthoDirections.Add(Direction.E);
			orthoDirections.Add(Direction.W);

			List<VecInt> diagonalDirections = new List<VecInt>();
			diagonalDirections.Add(Direction.NE);
			diagonalDirections.Add(Direction.NW);
			diagonalDirections.Add(Direction.SE);
			diagonalDirections.Add(Direction.SW);

			for (int y = 1; y < mapHeight - 1; y++)
			{
				for (int x = 1; x < mapWidth - 1; x++)
				{
					if (GetTile(x, y) == 0) //empty tile
					{
						int surroundingFloors = 0;
						foreach (var ortho in orthoDirections)
						{
							if (GetTile(x + ortho.x, y + ortho.y) == 2)
							{
								surroundingFloors++;
							}
						}
						foreach (var diagonal in diagonalDirections)
						{
							if (GetTile(x + diagonal.x, y + diagonal.y) == 2)
							{
								surroundingFloors++;
							}
						}
						if (surroundingFloors >= 6) //floor this tile
						{
							SetTile(x, y, 2);
						}
						else if (surroundingFloors == 5) //concave
						{
							SetTile(x, y, 1);
						}
						else if (surroundingFloors == 4)
						{
							SetTile(x, y, 1);
						}
						else if (surroundingFloors == 3 || surroundingFloors == 2) //normal wall
						{
							SetTile(x, y, 1);
						}
						else if (surroundingFloors == 1) //convex
						{
							SetTile(x, y, 1);
						}
					}
				}
			}
			//for (int y = 1; y < mapHeight - 1; y++)
			//{
			//	for (int x = 1; x < mapWidth - 1; x++)
			//	{
			//		if (GetTile(x, y) == 0) //empty tile
			//		{
			//			int surroundingWalls = 0;
			//			foreach (var ortho in orthoDirections)
			//			{
			//				if (GetTile(x + ortho.x, y + ortho.y) == 1)
			//				{
			//					surroundingWalls++;
			//				}
			//			}
			//			foreach (var diagonal in diagonalDirections)
			//			{
			//				if (GetTile(x + diagonal.x, y + diagonal.y) == 1)
			//				{
			//					surroundingWalls++;
			//				}
			//			}
			//			if (surroundingWalls == 5) //concave
			//			{
			//				SetTile(x, y, 1);
			//			}
			//			else if (surroundingWalls == 4)
			//			{
			//				SetTile(x, y, 1);
			//			}
			//			else if (surroundingWalls == 3 || surroundingWalls == 2) //normal wall
			//			{
			//				SetTile(x, y, 1);
			//			}
			//			else if (surroundingWalls == 1) //convex
			//			{
			//				SetTile(x, y, 1);
			//			}
			//		}
			//	}
			//}
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
						if (!allRooms[i].connectedRooms.Contains(allRooms[i].roomDistances[j].room))
						{
							ConnectRoomsWithCorridors(allRooms[i], allRooms[i].roomDistances[j].room);
						}
					}
				}
			}
		}

		void ConnectRoomsWithCorridors(Room roomA, Room roomB)
		{
			roomA.connectedRooms.Add(roomB);
			roomB.connectedRooms.Add(roomA);

			VecInt centerA = new VecInt(roomA.roomSpace.center);
			VecInt centerB = new VecInt(roomB.roomSpace.center);
			VecInt maxA = new VecInt(roomA.roomSpace.max);
			VecInt maxB = new VecInt(roomB.roomSpace.max);
			VecInt minA = new VecInt(roomA.roomSpace.min);
			VecInt minB = new VecInt(roomB.roomSpace.min);

			float deltaX = roomB.roomSpace.center.x - roomA.roomSpace.center.x;
			float deltaY = roomB.roomSpace.center.y - roomA.roomSpace.center.y;

			int xStart, yStart, xEnd, yEnd;
			bool xFirst = Mathf.Abs(deltaX) <= Mathf.Abs(deltaY);
			xStart = centerA.x;
			yStart = centerA.y;
			xEnd = centerB.x;
			yEnd = centerB.y;

			//if (xFirst)
			//{
			//	xStart = deltaX >= 0 ? maxA.x : minA.x;
			//	yStart = centerA.y;
			//	xEnd = centerB.x;
			//	yEnd = deltaY >= 0 ? minB.y : maxB.y;
			//}
			//else
			//{
			//	xStart = centerA.x;
			//	yStart = deltaY >= 0 ? maxA.y : minA.y;
			//	xEnd = deltaX >= 0 ? minB.x : maxB.x;
			//	yEnd = centerB.y;
			//}

			BuildCorridor(xStart, yStart, xEnd, yEnd, xFirst);
		}

		void BuildCorridor(int xStart, int yStart, int xEnd, int yEnd, bool xFirst)
		{
			if (xFirst)
			{
				if (xStart <= xEnd)
				{
					for (int x = xStart; x <= xEnd; x++)
					{
						PaintCorridor(x, yStart, false);
					}
				}
				else
				{
					for (int x = xStart; x >= xEnd; x--)
					{
						PaintCorridor(x, yStart, false);
					}
				}

				if (yStart <= yEnd)
				{
					for (int y = yStart; y <= yEnd; y++)
					{
						PaintCorridor(xEnd, y, true);
					}
				}
				else
				{
					for (int y = yStart; y >= yEnd; y--)
					{
						PaintCorridor(xEnd, y, true);
					}
				}
			}
			else
			{
				if (yStart <= yEnd)
				{
					for (int y = yStart; y <= yEnd; y++)
					{
						PaintCorridor(xStart, y, true);
					}
				}
				else
				{
					for (int y = yStart; y >= yEnd; y--)
					{
						PaintCorridor(xStart, y, true);
					}
				}
				if (xStart <= xEnd)
				{
					for (int x = xStart; x <= xEnd; x++)
					{
						PaintCorridor(x, yEnd, false);
					}
				}
				else
				{
					for (int x = xStart; x >= xEnd; x--)
					{
						PaintCorridor(x, yEnd, false);
					}
				}
			}
		}

		void PaintCorridor(int x, int y, bool addHorizontal)
		{
			SetTile(x, y, 2);

			for (int i = -corridorSize / 2; i < corridorSize / 2; i++)
			{
				if (addHorizontal)
				{
					SetTile(x + i, y, 2);

				}
				else
				{
					SetTile(x, y + i, 2);
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
		public static VecInt NE = new VecInt(1, 1);
		public static VecInt SW = new VecInt(-1, -1);
		public static VecInt SE = new VecInt(1, -1);
		public static VecInt NW = new VecInt(-1, 1);
	}
}
