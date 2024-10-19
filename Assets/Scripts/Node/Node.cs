using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Node :IHeapItem<Node>
{
   public bool Walkable;
   public Vector3 WorldPosition;

    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;

    public Node Parent;
    int heapIndex;


    public int MovementPenalty;

    public Node(bool _walkable, Vector3 _worldPos,int _gridx,int _gridy,int _penalty) 
    {
        Walkable = _walkable;
        WorldPosition = _worldPos;
        gridX = _gridx;
        gridY = _gridy;
        MovementPenalty = _penalty;
    }
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

    public int CompareTo(Node other)
    {
        int compare =fCost.CompareTo(other.fCost);
        if (compare == 0) 
        {
            compare=hCost.CompareTo(other.hCost);
        }
        return -compare;
    }
}
