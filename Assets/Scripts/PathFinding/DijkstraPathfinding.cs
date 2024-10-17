using System;
using System.Collections.Generic;
using UnityEngine;

public class DijkstraPathfinding : MonoBehaviour
{

    public Transform Seeker, Target;


    Grid grid;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }
    private void Update()
    {
        FindPath(Seeker.position, Target.position);
    }
    void FindPath(Vector3 startPos,Vector3 targetpos)
    {
        Node startNode=grid.NodeFromWorldPoint(startPos);
        Node targetNode=grid.NodeFromWorldPoint(targetpos);

        List<Node> openset = new List<Node>();
        HashSet<Node> closedset = new HashSet<Node>();

        openset.Add(startNode);

        while (openset.Count > 0)
        {
            Node currentNode = openset[0];
            for (int i = 1; i < openset.Count; i++)
            {
                if (openset[i].fCost <= currentNode.fCost  && openset[i].hCost<currentNode.hCost)
                {
                    currentNode = openset[i];
                }
            }
            openset.Remove(currentNode);
            closedset.Add(currentNode);
            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }
             

            foreach (Node neighboor in grid.Getneighbours(currentNode))
            {
                if(!neighboor.Walkable || closedset.Contains(neighboor))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighboor);

                if(newMovementCostToNeighbour < currentNode.gCost || !openset.Contains(neighboor))
                {
                    neighboor.gCost = newMovementCostToNeighbour;
                    neighboor.hCost = GetDistance(neighboor,targetNode);
                    neighboor.Parent = currentNode;

                    if(!openset.Contains(neighboor))
                        openset.Add(neighboor);
                }


            }
            
        }
    }
    void RetracePath(Node startNode,Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        grid.path = path;
    }

    int GetDistance(Node nodeA, Node nodeB) 
    { 
        int dstX = Mathf.Abs(nodeA.gridX-nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY-nodeB.gridY);

        if(dstX > dstY)
            return 14*dstY + 10 * (dstX - dstY);

        return 14*dstX + 10 * (dstY - dstX);
    }
}
