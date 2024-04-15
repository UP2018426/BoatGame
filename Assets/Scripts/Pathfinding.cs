using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Transform seeker, target;

    public Vector3 nextNode;

    public PathGrid grid; // Fix this I.F.
    public PathGrid pg;

    void Start()
    {

        if(TryGetComponent<PathGrid>(out PathGrid pathGrid))
        {
            grid = GetComponent<PathGrid>();
            pg = GetComponent<PathGrid>();
        }
        //grid = TryGetComponent<PathGrid>();
    }

    void Update()
    {
        if (pg != null) // pg!= null is a recent addition
        {
            if (pg.path != null)
            {
                if (pg.path.Count > 0)
                {
                    if (target != null)
                    {
                        FindPath(seeker.transform.position, target.transform.position);
                    }

                    nextNode = pg.path[0].worldPos;
                }
            }
        }
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldpoint(startPos);
        Node targetNode = grid.NodeFromWorldpoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while(openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if(openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if(currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);

                return;
            }

            foreach (Node neighbours in grid.GetNeigbours(currentNode))
            {
                if(!neighbours.walkable || closedSet.Contains(neighbours))
                {
                    continue;
                }

                float newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbours);
                if(newMovementCostToNeighbour < currentNode.gCost || !openSet.Contains(neighbours)){
                    neighbours.gCost = newMovementCostToNeighbour;
                    neighbours.hCost = GetDistance(neighbours, targetNode);
                    neighbours.parent = currentNode;

                    if (!openSet.Contains(neighbours))
                    {
                        openSet.Add(neighbours);
                    }
                }
            }


        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        grid.path = path;
    }

    float GetDistance(Node nodeA, Node nodeB)
    {
        float distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        float distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if(distX > distY)
        {
            return 14.14213562373095f * distY + 10 * (distX - distY);
        }
        return 14.14213562373095f * distX + 10 * (distY - distX);
    }
}
