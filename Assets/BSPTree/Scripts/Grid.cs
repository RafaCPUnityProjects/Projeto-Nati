using UnityEngine;
using System.Collections;

namespace BSPTree
{
	public class Grid
	{

		private int gridWidth;
		private int gridHeight;

		private int[,] grid;

		public Grid(int _width, int _height)
		{
			gridWidth = _width;
			gridHeight = _height;

			grid = new int[gridWidth, gridHeight];
		}

		public void SetTile(int x, int y, int value)
		{
			if(x <= 0 || x >= gridWidth-1 || y <= 0 || y >= gridHeight-1)
			{
				return;
			}

			try
			{
				grid[x, y] = value;

			}
			catch (System.Exception ex)
			{
				Debug.Log("Exception: " + ex.Message);
				Debug.Log("StackTrace: " + ex.StackTrace);
				Debug.Log("x = " + x + "|y = " + y);
			}
		}

		public int GetTile(int x, int y)
		{
			int returnValue = 0;
			if (x <= 0 || x >= gridWidth - 1 || y <= 0 || y >= gridHeight - 1)
			{
				return 0;
			}
			try
			{
				returnValue = grid[x, y];

			}
			catch (System.Exception ex)
			{
				Debug.Log("Exception: " + ex.Message);
				Debug.Log("StackTrace: " + ex.StackTrace);
				Debug.Log("x = " + x + "|y = " + y);
			}

			return returnValue;
		}

		public int Width()
		{
			return gridWidth;
		}

		public int Height()
		{
			return gridHeight;
		}
	}
}