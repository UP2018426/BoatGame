using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPos;

    public float gCost;
    public float hCost;

    public int gridX;
    public int gridY;

    public Node parent;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPos = _worldPos;

        gridX = _gridX;
        gridY = _gridY; 
    }

    public float fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}

