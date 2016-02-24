using UnityEngine;
using System.Collections;

namespace BSPTree
{
	public class RoomCreator : MonoBehaviour
	{
		private int roomID;

		private BSPNode parentNode;

		private GameObject sibiling;
		private int wallSize;

		public void Setup(int wallSize)
		{
			this.wallSize = wallSize;
			transform.position = new Vector3((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);

			transform.position = new Vector3(transform.position.x - (transform.localScale.x / 2), transform.position.y - (transform.localScale.y / 2), transform.position.z);

			for (int i = (int)transform.position.x; i < (int)transform.position.x + transform.localScale.x; i++)
			{
				for (int j = (int)transform.position.y; j < (int)transform.position.y + transform.localScale.y; j++)
				{
					BSPTree.SetTile(i, j, 1);
				}
			}

			for (int i = 0; i < transform.localScale.x + 1; i++)
			{
				BSPTree.SetTile((int)transform.position.x + i, (int)transform.position.y, 2);
				BSPTree.SetTile((int)transform.position.x + i, (int)(transform.position.y + transform.localScale.y), 2);
			}

			for (int i = 0; i < transform.localScale.y + 1; i++)
			{
				BSPTree.SetTile((int)transform.position.x, (int)transform.position.y + i, 2);
				BSPTree.SetTile((int)(transform.position.x + transform.localScale.x), (int)transform.position.y + i, 2);
			}

		}

		public void SetID(int _aID)
		{
			roomID = _aID;
		}

		public void SetParentNode(BSPNode _aNode)
		{
			parentNode = _aNode;
		}

		public void Connect()
		{
			GetSibiling();

			if (sibiling != null)
			{

				Vector3 startPos = new Vector3();
				Vector3 endPos = new Vector3();

				if (sibiling.transform.position.y + sibiling.transform.localScale.y < transform.position.y)
				{
					startPos = ChooseDoorPoint(0);
					endPos = sibiling.GetComponent<RoomCreator>().ChooseDoorPoint(2);
				}
				else if (sibiling.transform.position.y > transform.position.y + transform.localScale.y)
				{
					startPos = ChooseDoorPoint(2);
					endPos = sibiling.GetComponent<RoomCreator>().ChooseDoorPoint(1);
				}
				else if (sibiling.transform.position.x + sibiling.transform.localScale.x < transform.position.x)
				{
					startPos = ChooseDoorPoint(3);
					endPos = sibiling.GetComponent<RoomCreator>().ChooseDoorPoint(1);
				}
				else if (sibiling.transform.position.x > transform.position.x + transform.localScale.x)
				{
					startPos = ChooseDoorPoint(1);
					endPos = sibiling.GetComponent<RoomCreator>().ChooseDoorPoint(3);
				}


				GameObject aDigger = (GameObject)Instantiate(Resources.Load("Digger"), startPos, Quaternion.identity);
				aDigger.GetComponent<Digger>().Begin(endPos, wallSize);


				parentNode = FindRoomlessParent(parentNode);

				if (parentNode != null)
				{

					int aC = BSPTree.MyRandomRange(0, 2);

					if (aC == 0)
					{
						parentNode.SetRoom(this.gameObject);
					}
					else
					{
						parentNode.SetRoom(sibiling.gameObject);
					}

					sibiling.GetComponent<RoomCreator>().SetParentNode(parentNode);
				}

			}

		}

		private void GetSibiling()
		{
			if (parentNode.GetParentNode() != null)
			{
				if (parentNode.GetParentNode().GetLeftNode() != parentNode)
				{
					sibiling = parentNode.GetParentNode().GetLeftNode().GetRoom();
				}
				else {
					sibiling = parentNode.GetParentNode().GetRightNode().GetRoom();
				}
			}
		}

		public Vector3 ChooseDoorPoint(int index)
		{
			switch (index)
			{
				case 0:
					return new Vector3(
						(int)(transform.position.x + BSPTree.MyRandomRange(1, transform.localScale.x - 2)),
						(int)(transform.position.y),
						transform.position.z);
				case 1:
					return new Vector3(
						(int)(transform.position.x + transform.localScale.x),
						(int)(transform.position.y + BSPTree.MyRandomRange(1, transform.localScale.y - 2)),
						transform.position.z);
				case 2:
					return new Vector3(
						(int)(transform.position.x + BSPTree.MyRandomRange(1, transform.localScale.x - 2)),
						(int)(transform.position.y + transform.localScale.y),
						transform.position.z);
				case 3:
					return new Vector3(
						(int)(transform.position.x + 1),
						(int)(transform.position.y + BSPTree.MyRandomRange(1, transform.localScale.y - 2)),
						transform.position.z);
				default:
					return new Vector3(0, 0, 0);
			}
		}

		public BSPNode GetParent()
		{
			return parentNode;
		}

		public BSPNode FindRoomlessParent(BSPNode _aNode)
		{
			if (_aNode != null)
			{
				if (_aNode.GetRoom() == null)
				{
					return _aNode;
				}
				else {
					return FindRoomlessParent(_aNode.GetParentNode());
				}
			}

			return null;
		}
	}
}
