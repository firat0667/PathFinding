using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class PathRequestManager : MonoBehaviour
{
    public static PathRequestManager Instance;
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();

    PathRequest currentPathRequest;

    DijkstraPathfinding pathfinding;

    bool isProcessingPath;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        pathfinding=GetComponent<DijkstraPathfinding>();
    }

    public static void RequestPath(Vector3 pathStart,Vector3 pathEnd,Action<Vector3[],bool> callback)
    {
        PathRequest pathRequest= new PathRequest(pathStart,pathEnd,callback);
        Instance.pathRequestQueue.Enqueue(pathRequest);
        Instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    public void FinishedProcessingPath(Vector3[]path,bool success)
    {
        currentPathRequest.callback(path,success);
        isProcessingPath = false;
        TryProcessNext();
    }


    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[],bool> callback;

        public PathRequest(Vector3 _start,Vector3 _end,Action<Vector3[],bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }
}
