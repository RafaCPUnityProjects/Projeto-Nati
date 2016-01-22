/* 
    This is the pathfinding  class that employs the A* algorithm
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Grid))]
[RequireComponent(typeof(PathRequestManager))]
public class Pathfinding : MonoBehaviour
{
    PathRequestManager requestManager;
    Grid grid;

    void Awake()
    {
        grid = GetComponent<Grid>();
        requestManager = GetComponent<PathRequestManager>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        //Stopwatch sw = new Stopwatch(); //stopwatch to keep track of the search times
        //sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        // A* algorithm
        Node startNode = grid.WorldPoint2Node(startPos);
        Node targetNode = grid.WorldPoint2Node(targetPos);

        if (startNode.walkable && targetNode.walkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxGridSize);
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    //sw.Stop();
                    //print("Path found in: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbourNode in grid.GetNeighbours(currentNode))
                {
                    if (!neighbourNode.walkable || closedSet.Contains(neighbourNode))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour =
                        currentNode.gCost +
                        GetDistance(currentNode, neighbourNode) +
                        neighbourNode.movementPenalty;
                    if (newMovementCostToNeighbour < neighbourNode.gCost || !openSet.Contains(neighbourNode))
                    {
                        neighbourNode.gCost = newMovementCostToNeighbour;
                        neighbourNode.hCost = GetDistance(neighbourNode, targetNode);

                        neighbourNode.parent = currentNode;
                        if (!openSet.Contains(neighbourNode))
                        {
                            openSet.Add(neighbourNode);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbourNode);
                        }
                    }
                }
            }
        }
        yield return null;
        if (pathSuccess)
        {
            waypoints = Nodes2WorldPoints(startNode, targetNode);
        }
        requestManager.FinishProcessingPath(waypoints, pathSuccess);
    }

    Vector3[] Nodes2WorldPoints(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);

        return waypoints;
    }

    //Returns a simpler path removing unnecessary points
    private Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                directionOld = directionNew;
                waypoints.Add(path[i].worldPosition);
            }
        }
        return waypoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        else
        {
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }
}
