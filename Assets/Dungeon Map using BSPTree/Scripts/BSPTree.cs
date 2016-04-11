using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BSPTree
{
	public class BSPTree : MonoBehaviour
	{
		public int dungeonWidth = 150;
		public int dungeonHeight = 150;
		public string seed = "0";
		public int wallSize = 1;
		public BSPNode parentNode;
		//public int roomSize = 10;
		public bool fillWalls = true;
		public bool sanitizeLevel = true;
		public int minRoomSize = 10;
		public static Grid levelGrid;

		private int roomID = 0;
		private System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
		public static System.Random prng;

		public static List<GameObject> objectsToSanitize = new List<GameObject>();

		public static float MyRandomRange(float min, float max)
		{
			return Mathf.Lerp(min, max, (float)prng.NextDouble());
		}

		public static int MyRandomRange(int min, int max)
		{
			return prng.Next(min, max);
		}

		void Start()
		{
			sw.Start();
			Debug.Log("Stopwatch started");
			prng = new System.Random(seed.GetHashCode());
			GameObject startCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			objectsToSanitize.Add(startCube);
			startCube.transform.localScale = new Vector3(dungeonWidth, dungeonHeight, 1);
			startCube.tag = "GenSection";
			startCube.transform.position = new Vector3(
				transform.position.x + startCube.transform.localScale.x / 2,
				transform.position.y + startCube.transform.localScale.y / 2,
				transform.position.z);

			levelGrid = new Grid(dungeonWidth, dungeonHeight);
			
			parentNode = new BSPNode(minRoomSize);
			parentNode.SetCube(startCube);


			for (int i = 0; i < 7; i++)
			{
				Split(parentNode);
			}
			Debug.Log("BSP finished: " + sw.Elapsed.ToString());

			//create the rooms
			CreateRooms(parentNode);
			Debug.Log("Rooms created: " + sw.Elapsed.ToString());

			//connect the rooms
			ConnectRooms(parentNode);
			Debug.Log("Rooms connected: " + sw.Elapsed.ToString());

			//tidy up dungeon
			for (int k = 0; k < 5; k++)
			{

				for (int i = 0; i < levelGrid.Width(); i++)
				{
					for (int j = 0; j < levelGrid.Height(); j++)
					{
						RemoveSingles(i, j);
					}
				}
			}
			Debug.Log("Dungeon cleaned: " + sw.Elapsed.ToString());

			CreateLevel();
			Debug.Log("Level created: " + sw.Elapsed.ToString());
		}

		//split the tree
		public void Split(BSPNode aNode)
		{
			if (aNode.GetLeftNode() != null)
			{
				Split(aNode.GetLeftNode());
			}
			else {
				aNode.Cut();
				return;
			}

			if (aNode.GetLeftNode() != null)
			{
				Split(aNode.GetRightNode());
			}

		}

		public static Grid GetGrid()
		{
			return levelGrid;
		}

		public static void SetTile(int _x, int _y, int _value)
		{
			levelGrid.SetTile(_x, _y, _value);
		}

		private void AddRoom(BSPNode aNode)
		{

			GameObject aObj = aNode.GetCube();

			GameObject aRoom = (GameObject)Instantiate(Resources.Load("BaseRoom"), aObj.transform.position, Quaternion.identity);
			objectsToSanitize.Add(aRoom);
			aRoom.transform.localScale = new Vector3(
				(int)(MyRandomRange(minRoomSize, aObj.transform.localScale.x - minRoomSize/2)),
				(int)(MyRandomRange(minRoomSize, aObj.transform.localScale.y - minRoomSize/2)),
				aRoom.transform.localScale.z);
			aRoom.GetComponent<RoomCreator>().Setup(wallSize);
			aRoom.GetComponent<RoomCreator>().SetID(roomID);
			aRoom.GetComponent<RoomCreator>().SetParentNode(aNode);
			aNode.SetRoom(aRoom);
			roomID++;
		}

		private void CreateRooms(BSPNode aNode)
		{
			if (aNode.GetLeftNode() != null)
			{
				CreateRooms(aNode.GetLeftNode());
			}
			else {
				AddRoom(aNode);
				return;
			}

			if (aNode.GetRightNode() != null)
			{
				CreateRooms(aNode.GetRightNode());
			}
		}

		private void ConnectRooms(BSPNode aNode)
		{
			if (aNode.GetLeftNode() != null)
			{
				ConnectRooms(aNode.GetLeftNode());

				if (aNode.GetRoom() != null)
				{
					aNode.GetRoom().GetComponent<RoomCreator>().Connect();

					return;
				}

			}
			else {
				if (aNode.GetRoom() != null)
				{
					aNode.GetRoom().GetComponent<RoomCreator>().Connect();

					return;
				}
			}

			if (aNode.GetRightNode() != null)
			{
				ConnectRooms(aNode.GetRightNode());

				if (aNode.GetRoom() != null)
				{
					aNode.GetRoom().GetComponent<RoomCreator>().Connect();

					return;
				}
			}
			else {
				if (aNode.GetRoom() != null)
				{
					aNode.GetRoom().GetComponent<RoomCreator>().Connect();

					return;
				}
			}
		}

		private void CreateLevel()
		{
			if (sanitizeLevel)
			{
				foreach (var go in objectsToSanitize)
				{
					Destroy(go);
				}
			}
			objectsToSanitize.Clear();

			GameObject levelRoot = new GameObject();
			levelRoot.name = "LevelRoot";
			levelRoot.transform.position = Vector3.zero;

			for (int i = 0; i < levelGrid.Width(); i++)
			{
				for (int j = 0; j < levelGrid.Height(); j++)
				{
					string tileToLoad = "";
					switch (levelGrid.GetTile(i, j))
					{
						case 0:
							if (fillWalls)
							{
								tileToLoad = "WallTile";
							}
							break;
						case 1:
							tileToLoad = "FloorTile";
							break;
						case 2:
							tileToLoad = "WallTile";
							break;
					}
					if (tileToLoad != "")
					{
						GameObject go = (GameObject)Instantiate(
									Resources.Load(tileToLoad),
									new Vector3(
										transform.position.x - (transform.localScale.x / 2) + i,
										transform.position.y - (transform.localScale.y / 2) + j,
										transform.position.z + transform.localScale.z / 2),
									Quaternion.identity);
						go.transform.parent = levelRoot.transform;
					}
				}
			}
			if (!sanitizeLevel)
			{
				levelRoot.SetActive(false);
			}
		}

		//cellular automota rules for cleanup stage
		private void RemoveSingles(int x, int y)
		{
			int count = 0;

			if (x < levelGrid.Width() - 1 && x > 1 && y > 1 && y < levelGrid.Height() - 1)
			{
				if (levelGrid.GetTile(x + 1, y) == 1)
				{
					count++;
				}

				if (levelGrid.GetTile(x - 1, y) == 0)
				{
					return;
				}

				if (levelGrid.GetTile(x + 1, y) == 0)
				{
					return;
				}

				if (levelGrid.GetTile(x, y + 1) == 0)
				{
					return;
				}

				if (levelGrid.GetTile(x, y - 1) == 0)
				{
					return;
				}


				//

				if (levelGrid.GetTile(x - 1, y) == 1)
				{
					count++;
				}

				if (levelGrid.GetTile(x, y + 1) == 1)
				{
					count++;
				}

				if (levelGrid.GetTile(x, y - 1) == 1)
				{
					count++;
				}

				if (levelGrid.GetTile(x - 1, y) == 1)
				{
					count++;
				}

				if (levelGrid.GetTile(x - 1, y - 1) == 1)
				{
					count++;
				}

				if (levelGrid.GetTile(x + 1, y - 1) == 1)
				{
					count++;
				}

				if (levelGrid.GetTile(x - 1, y + 1) == 1)
				{
					count++;
				}

				if (levelGrid.GetTile(x + 1, y + 1) == 1)
				{
					count++;
				}

				if (count >= 5)
				{
					levelGrid.SetTile(x, y, 1);
				}
			}
		}
	}
}