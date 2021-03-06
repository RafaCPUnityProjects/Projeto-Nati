﻿/*
    Responsible for processing the path requests
*/

using UnityEngine;
using System.Collections.Generic;
using System;

public class PathRequestManager : MonoBehaviour
{
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static PathRequestManager instance;
    Pathfinding pathFinding;

    bool isProcessingPath;

    void Awake()
    {
        instance = this;
        pathFinding = GetComponent<Pathfinding>();
    }

    //Static method called for a path between 2 world points
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callBack)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callBack);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    private void TryProcessNext()
    {
        if(!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathFinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    public void FinishProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.callBack(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callBack;

        public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callBack)
        {
            this.pathStart = pathStart;
            this.pathEnd = pathEnd;
            this.callBack = callBack;
        }
    }
}
