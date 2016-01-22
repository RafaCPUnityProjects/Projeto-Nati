/* 
    Node class used in the algorithm
*/

using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX; //x pos in the grid
    public int gridY; //y pos in the grid
    public int movementPenalty;

    public int gCost;
    public int hCost;

    public Node parent;

    int heapIndex;

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }

        set
        {
            heapIndex = value;
        }
    }

    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY, int movementPenalty = 0)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
        this.movementPenalty = movementPenalty;
    }

    public int CompareTo(Node other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }
        return -compare;
    }
}
