using UnityEngine;
using System.Collections;

namespace BSPTree
{
	public class BSPTree : MonoBehaviour
	{
		public int dungeonWidth = 150;
		public int dungeonHeight = 150;
		public string seed = "0";
		public BSPNode parentNode;

		public static Grid levelGrid;

		private int roomID = 0;
		private System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
		public static System.Random prng;

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
			startCube.transform.localScale = new Vector3(dungeonWidth, 1, dungeonHeight);
			startCube.tag = "GenSection";
			startCube.transform.position = new Vector3(transform.position.x + startCube.transform.localScale.x / 2,
				transform.position.y,
				transform.position.z + startCube.transform.localScale.z / 2);

			levelGrid = new Grid((int)startCube.transform.localScale.x, (int)startCube.transform.localScale.z);
			/*//unnecessary
			for (int i = 0; i < levelGrid.Width(); i++){
				for (int j = 0; j < levelGrid.Height(); j++){
					levelGrid.setTile(i,j,0);		
				}
			}*/

			parentNode = new BSPNode();
			parentNode.setCube(startCube);


			for (int i = 0; i < 7; i++)
			{
				split(parentNode);
			}
			Debug.Log("BSP finished: " + sw.Elapsed.ToString());

			//create the rooms
			createRooms(parentNode);
			Debug.Log("Rooms created: " + sw.Elapsed.ToString());

			//connect the rooms
			connectRooms(parentNode);
			Debug.Log("Rooms connected: " + sw.Elapsed.ToString());

			//tidy up dungeon
			for (int k = 0; k < 5; k++)
			{

				for (int i = 0; i < levelGrid.Width(); i++)
				{
					for (int j = 0; j < levelGrid.Height(); j++)
					{
						removeSingles(i, j);
					}
				}
			}
			Debug.Log("Dungeon cleaned: " + sw.Elapsed.ToString());

			createLevel();
			Debug.Log("Level created: " + sw.Elapsed.ToString());
		}

		//split the tree
		public void split(BSPNode _aNode)
		{
			if (_aNode.getLeftNode() != null)
			{
				split(_aNode.getLeftNode());
			}
			else {
				_aNode.cut();
				return;
			}

			if (_aNode.getLeftNode() != null)
			{
				split(_aNode.getRightNode());
			}

		}

		public static Grid getGrid()
		{
			return levelGrid;
		}

		public static void setTile(int _x, int _y, int _value)
		{
			levelGrid.setTile(_x, _y, _value);
		}

		private void addRoom(BSPNode _aNode)
		{

			GameObject aObj = _aNode.getCube();

			GameObject aRoom = (GameObject)Instantiate(Resources.Load("BaseRoom"), aObj.transform.position, Quaternion.identity);
			aRoom.transform.localScale = new Vector3(
				(int)(MyRandomRange(10, aObj.transform.localScale.x - 5)),
				aRoom.transform.localScale.y,
				(int)(MyRandomRange(10, aObj.transform.localScale.z - 5)));
			aRoom.GetComponent<RoomCreator>().setup();
			aRoom.GetComponent<RoomCreator>().setID(roomID);
			aRoom.GetComponent<RoomCreator>().setParentNode(_aNode);
			_aNode.setRoom(aRoom);
			roomID++;
		}

		private void createRooms(BSPNode _aNode)
		{
			if (_aNode.getLeftNode() != null)
			{
				createRooms(_aNode.getLeftNode());
			}
			else {
				addRoom(_aNode);
				return;
			}

			if (_aNode.getRightNode() != null)
			{
				createRooms(_aNode.getRightNode());
			}
		}

		private void connectRooms(BSPNode _aNode)
		{
			if (_aNode.getLeftNode() != null)
			{
				connectRooms(_aNode.getLeftNode());

				if (_aNode.getRoom() != null)
				{
					_aNode.getRoom().GetComponent<RoomCreator>().connect();

					return;
				}

			}
			else {
				if (_aNode.getRoom() != null)
				{
					_aNode.getRoom().GetComponent<RoomCreator>().connect();

					return;
				}
			}

			if (_aNode.getRightNode() != null)
			{
				connectRooms(_aNode.getRightNode());

				if (_aNode.getRoom() != null)
				{
					_aNode.getRoom().GetComponent<RoomCreator>().connect();

					return;
				}
			}
			else {
				if (_aNode.getRoom() != null)
				{
					_aNode.getRoom().GetComponent<RoomCreator>().connect();

					return;
				}
			}

		}

		private void createLevel()
		{
			for (int i = 0; i < levelGrid.Width(); i++)
			{
				for (int j = 0; j < levelGrid.Height(); j++)
				{

					switch (levelGrid.getTile(i, j))
					{
						case 1:
							Instantiate(Resources.Load("FloorTile"), new Vector3(transform.position.x - (transform.localScale.x / 2) + i, transform.position.y + transform.localScale.y / 2, transform.position.z - (transform.localScale.z / 2) + j), Quaternion.identity);
							break;
						case 2:
							Instantiate(Resources.Load("WallTile"), new Vector3(transform.position.x - (transform.localScale.x / 2) + i, transform.position.y + transform.localScale.y / 2, transform.position.z - (transform.localScale.z / 2) + j), Quaternion.identity);
							break;
					}

				}
			}
		}

		//cellular automota rules for cleanup stage
		private void removeSingles(int _x, int _y)
		{
			int count = 0;

			if (_x < levelGrid.Width() - 1 && _x > 1 && _y > 1 && _y < levelGrid.Height() - 1)
			{
				if (levelGrid.getTile(_x + 1, _y) == 1)
				{
					count++;
				}

				if (levelGrid.getTile(_x - 1, _y) == 0)
				{
					return;
				}

				if (levelGrid.getTile(_x + 1, _y) == 0)
				{
					return;
				}

				if (levelGrid.getTile(_x, _y + 1) == 0)
				{
					return;
				}

				if (levelGrid.getTile(_x, _y - 1) == 0)
				{
					return;
				}


				//

				if (levelGrid.getTile(_x - 1, _y) == 1)
				{
					count++;
				}

				if (levelGrid.getTile(_x, _y + 1) == 1)
				{
					count++;
				}

				if (levelGrid.getTile(_x, _y - 1) == 1)
				{
					count++;
				}

				if (levelGrid.getTile(_x - 1, _y) == 1)
				{
					count++;
				}

				if (levelGrid.getTile(_x - 1, _y - 1) == 1)
				{
					count++;
				}

				if (levelGrid.getTile(_x + 1, _y - 1) == 1)
				{
					count++;
				}

				if (levelGrid.getTile(_x - 1, _y + 1) == 1)
				{
					count++;
				}

				if (levelGrid.getTile(_x + 1, _y + 1) == 1)
				{
					count++;
				}

				if (count >= 5)
				{
					levelGrid.setTile(_x, _y, 1);
				}
			}

		}

	}
}