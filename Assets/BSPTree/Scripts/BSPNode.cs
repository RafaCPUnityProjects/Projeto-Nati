using UnityEngine;
using System.Collections;

namespace BSPTree
{
	public class BSPNode
	{
		private int minRoomSize;
		GameObject cube;
		BSPNode parentNode;
		BSPNode leftNode;
		BSPNode rightNode;
		Color myColor;

		private bool isConnected = false;
		GameObject room;

		public BSPNode(int minRoomSize)
		{
			this.minRoomSize = minRoomSize;
			myColor = new Color(
				BSPTree.MyRandomRange(0.0f, 1.0f), 
				BSPTree.MyRandomRange(0.0f, 1.0f), 
				BSPTree.MyRandomRange(0.0f, 1.0f));
		}

		public void SetLeftNode(BSPNode _aNode)
		{
			leftNode = _aNode;
		}

		public void SetRightNode(BSPNode _aNode)
		{
			rightNode = _aNode;
		}

		public void SetParentNode(BSPNode _aNode)
		{
			parentNode = _aNode;
		}

		public BSPNode GetLeftNode()
		{
			return leftNode;
		}

		public BSPNode GetRightNode()
		{
			return rightNode;
		}

		public BSPNode GetParentNode()
		{
			return parentNode;
		}

		void SplitX(GameObject aSection)
		{
			float xSplit = BSPTree.MyRandomRange(minRoomSize, aSection.transform.localScale.x - minRoomSize);

			if (xSplit > minRoomSize)
			{
				GameObject cube0 = GameObject.CreatePrimitive(PrimitiveType.Cube);
				cube0.transform.localScale = new Vector3(xSplit, aSection.transform.localScale.y, aSection.transform.localScale.z);
				cube0.transform.position = new Vector3(
					aSection.transform.position.x - ((xSplit - aSection.transform.localScale.x) / 2),
					aSection.transform.position.y,
					aSection.transform.position.z);
				cube0.GetComponent<Renderer>().material.color = new Color(
					BSPTree.MyRandomRange(0.0f, 1.0f), 
					BSPTree.MyRandomRange(0.0f, 1.0f), 
					BSPTree.MyRandomRange(0.0f, 1.0f));
				cube0.tag = "GenSection";
				BSPTree.objectsToSanitize.Add(cube0);
				leftNode = new BSPNode(minRoomSize);
				leftNode.SetCube(cube0);
				leftNode.SetParentNode(this);

				GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
				float split1 = aSection.transform.localScale.x - xSplit;
				cube1.transform.localScale = new Vector3(split1, aSection.transform.localScale.y, aSection.transform.localScale.z);
				cube1.transform.position = new Vector3(
					aSection.transform.position.x + ((split1 - aSection.transform.localScale.x) / 2),
					aSection.transform.position.y,
					aSection.transform.position.z);
				cube1.GetComponent<Renderer>().material.color = new Color(
					BSPTree.MyRandomRange(0.0f, 1.0f), 
					BSPTree.MyRandomRange(0.0f, 1.0f), 
					BSPTree.MyRandomRange(0.0f, 1.0f));
				cube1.tag = "GenSection";
				BSPTree.objectsToSanitize.Add(cube1);
				rightNode = new BSPNode(minRoomSize);
				rightNode.SetCube(cube1);
				rightNode.SetParentNode(this);

				GameObject.DestroyImmediate(aSection);
			}
		}

		void SplitY(GameObject aSection)
		{
			float ySplit = BSPTree.MyRandomRange(minRoomSize, aSection.transform.localScale.y - minRoomSize);
			float ySplit1 = aSection.transform.localScale.y - ySplit;

			if (ySplit > minRoomSize)
			{
				GameObject cube0 = GameObject.CreatePrimitive(PrimitiveType.Cube);
				cube0.transform.localScale = new Vector3(aSection.transform.localScale.x, ySplit, aSection.transform.localScale.z);
				cube0.transform.position = new Vector3(
					aSection.transform.position.x,
					aSection.transform.position.y - ((ySplit - aSection.transform.localScale.y) / 2),
					aSection.transform.position.z);
				cube0.GetComponent<Renderer>().material.color = new Color(
					BSPTree.MyRandomRange(0.0f, 1.0f), 
					BSPTree.MyRandomRange(0.0f, 1.0f), 
					BSPTree.MyRandomRange(0.0f, 1.0f));
				cube0.tag = "GenSection";
				BSPTree.objectsToSanitize.Add(cube0);
				leftNode = new BSPNode(minRoomSize);
				leftNode.SetCube(cube0);
				leftNode.SetParentNode(this);

				GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
				cube1.transform.localScale = new Vector3(aSection.transform.localScale.x, ySplit1, aSection.transform.localScale.z);
				cube1.transform.position = new Vector3(
					aSection.transform.position.x,
					aSection.transform.position.y + ((ySplit1 - aSection.transform.localScale.y) / 2),
					aSection.transform.position.z);
				cube1.GetComponent<Renderer>().material.color = new Color(
					BSPTree.MyRandomRange(0.0f, 1.0f), 
					BSPTree.MyRandomRange(0.0f, 1.0f), 
					BSPTree.MyRandomRange(0.0f, 1.0f));
				cube1.tag = "GenSection";
				BSPTree.objectsToSanitize.Add(cube1);
				rightNode = new BSPNode(minRoomSize);
				rightNode.SetCube(cube1);
				rightNode.SetParentNode(this);

				GameObject.DestroyImmediate(aSection);
			}
		}

		public void SetCube(GameObject _aCube)
		{
			cube = _aCube;
		}

		public GameObject GetCube()
		{
			return cube;
		}

		public void Cut()
		{
			float choice = BSPTree.MyRandomRange(0f, 1f);
			if (choice <= 0.5)
			{
				SplitX(cube);
			}
			else {
				SplitY(cube);
			}
		}

		public Color GetColor()
		{
			return myColor;
		}

		public void SetRoom(GameObject _aRoom)
		{
			room = _aRoom;
		}

		public GameObject GetRoom()
		{
			return room;
		}

		public void SetConnected()
		{
			isConnected = true;
		}

		public bool GetIsConnected()
		{
			return isConnected;
		}
	}
}
