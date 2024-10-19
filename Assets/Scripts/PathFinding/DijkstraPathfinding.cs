using System;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Collections;
public class DijkstraPathfinding : MonoBehaviour
{

   // public Transform Seeker, Target;

    PathRequestManager pathRequestManager;


    Grid grid;

    private void Awake()
    {
        pathRequestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<Grid>();
       
    }

    IEnumerator FindPath(Vector3 startPos,Vector3 targetpos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];

        bool pathSucces = false;

        Node startNode=grid.NodeFromWorldPoint(startPos);
        Node targetNode=grid.NodeFromWorldPoint(targetpos);

        if(startNode.Walkable && targetNode.Walkable)
        {
            Heap<Node> openset = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedset = new HashSet<Node>();

            openset.Add(startNode);

            while (openset.Count > 0)
            {
                Node currentNode = openset.RemoveFirst();
                closedset.Add(currentNode);
                if (currentNode == targetNode)
                {
                    sw.Stop();
                    print("Path found :" + sw.ElapsedMilliseconds + "ms");
                    pathSucces = true;
                    RetracePath(startNode, targetNode);
                    break;
                }


                foreach (Node neighboor in grid.Getneighbours(currentNode))
                {
                    if (!neighboor.Walkable || closedset.Contains(neighboor))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighboor)+neighboor.MovementPenalty;

                    if (newMovementCostToNeighbour < currentNode.gCost || !openset.Contains(neighboor))
                    {
                        neighboor.gCost = newMovementCostToNeighbour;
                        neighboor.hCost = GetDistance(neighboor, targetNode);
                        neighboor.Parent = currentNode;

                        if (!openset.Contains(neighboor))
                            openset.Add(neighboor);
                    }


                }

            }
            yield return null;
            if (pathSucces)
            {
                waypoints = RetracePath(startNode, targetNode);
            }
            pathRequestManager.FinishedProcessingPath(waypoints, pathSucces);
        }
     
       
    }
    Vector3[] RetracePath(Node startNode,Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        Vector3[] waypoints =SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;

    }

    Vector3[] SimplifyPath(List<Node> path) 
    { 
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++) 
        { 
            Vector2 directionNew=new Vector2(path[i - 1].gridX - path[i].gridX, 
                path[i-1].gridY-path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].WorldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB) 
    { 
        int dstX = Mathf.Abs(nodeA.gridX-nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY-nodeB.gridY);

        if(dstX > dstY)
            return 14*dstY + 10 * (dstX - dstY);

        return 14*dstX + 10 * (dstY - dstX);
    }

    public void StartFindPath(Vector3 pathStart, Vector3 pathEnd)
    {
        StartCoroutine(FindPath(pathStart,pathEnd));
    }
}
