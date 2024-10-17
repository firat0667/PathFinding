using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
   public bool Walkable;
   public Vector3 WorldPosition;

    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;

    public Node Parent;

    public Node(bool _walkable, Vector3 _worldPos,int _gridx,int _gridy) 
    {
        Walkable = _walkable;
        WorldPosition = _worldPos;
        gridX = _gridx;
        gridY = _gridy;
    }
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
