using UnityEngine;
using System.Collections;

namespace BSPTree
{

	public class Digger : MonoBehaviour
	{

		private Vector3 targetPos;
		private int wallSize;

		public void Begin(Vector3 _targetPos, int wallSize)
		{
			targetPos = _targetPos;
			this.wallSize = wallSize;
			BSPTree.objectsToSanitize.Add(this.gameObject);

			Dig();
		}

		private void UpdateTile()
		{
			BSPTree.SetTile((int)transform.position.x, (int)transform.position.y, 1);
			BSPTree.SetTile((int)transform.position.x + 1, (int)transform.position.y, 1);
			BSPTree.SetTile((int)transform.position.x - 1, (int)transform.position.y, 1);
			BSPTree.SetTile((int)transform.position.x, (int)transform.position.y + 1, 1);
			BSPTree.SetTile((int)transform.position.x, (int)transform.position.y - 1, 1);

			SurroundTilesWithWall((int)transform.position.x + 1, (int)transform.position.y);
			SurroundTilesWithWall((int)transform.position.x - 1, (int)transform.position.y);
			SurroundTilesWithWall((int)transform.position.x, (int)transform.position.y + 1);
			SurroundTilesWithWall((int)transform.position.x, (int)transform.position.y - 1);

		}

		public void Dig()
		{

			while (transform.position.x != targetPos.x)
			{

				if (transform.position.x < targetPos.x)
				{
					transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
				}
				else
				{
					transform.position = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
				}

				UpdateTile();
			}

			while (transform.position.y != targetPos.y)
			{
				if (transform.position.y < targetPos.y)
				{
					transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
				}
				else
				{
					transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
				}

				UpdateTile();
			}

			DestroyImmediate(this);
		}

		public void SurroundTilesWithWall(int x, int y)
		{
			//Dir dir = new Dir(1, 0);
			for (int i = -wallSize; i <= wallSize; i++)
			{
				for (int j = -wallSize; j <= wallSize; j++)
				{
					if (i != 0 && j != 0)
					{
						SetTileDir(x, y, new Dir(i, j));
					}
				}
			}
		}

		private static void SetTileDir(int x, int y, Dir dir)
		{
			if (BSPTree.GetGrid().GetTile(x + dir.x, y + dir.y) == 0)
			{
				BSPTree.SetTile(x + dir.x, y + dir.y, 2);
			}
		}

		public struct Dir
		{
			public int x;
			public int y;

			public Dir(int x, int y)
			{
				this.x = x;
				this.y = y;
			}
		}
	}
}