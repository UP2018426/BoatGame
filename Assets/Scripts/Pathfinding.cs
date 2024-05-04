using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Vector3 seeker, target;

    public Vector3 nextNode;

    public PathGrid grid;

    public List<Vector3> vector3Path = new();

    public BoatAi boatAi;

    void Start()
    {
        if(TryGetComponent<PathGrid>(out PathGrid pathGrid))
        {
            grid = pathGrid;
        }

        if (TryGetComponent<BoatAi>(out BoatAi ai))
        {
            boatAi = ai;
        }
    }

    void Update()
    {
        if (boatAi != null)
        {
            seeker = transform.position;

            if (boatAi.target != null)
            {
                target = boatAi.target.position;
                
                vector3Path.Clear();
                vector3Path = FindPathList(seeker, target);

                if (grid != null) // Do null check inside FindPath and remove this line
                {
                    FindPath(seeker, target);

                    if (grid.path != null)
                    {
                        if (grid.path.Count > 0)
                        {
                            nextNode = grid.path[0].worldPos; // Set up a delagate
                        }
                    }
                }
            }
        }
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldpoint(startPos);
        Node targetNode = grid.NodeFromWorldpoint(targetPos);
        
        // Check to see if either node is in an invalid location
        if (!startNode.walkable)
        {
            Debug.LogWarning("Start Node for pathfinding on " + this.gameObject.name + " is in an invalid location!");
        }
        if (!targetNode.walkable)
        {
            Debug.LogWarning("Target Node for pathfinding on " + this.gameObject.name + " is in an invalid location!");
        }

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

    public List<Vector3> FindPathList(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldpoint(startPos);
        Node targetNode = grid.NodeFromWorldpoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        List<Vector3> returnList = new();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePathAsVector3List(startNode, targetNode);
            }

            foreach (Node neighbours in grid.GetNeigbours(currentNode))
            {
                if (!neighbours.walkable || closedSet.Contains(neighbours))
                {
                    continue;
                }

                float newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbours);
                if (newMovementCostToNeighbour < currentNode.gCost || !openSet.Contains(neighbours))
                {
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

        return new List<Vector3>();
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

    List<Vector3> RetracePathAsVector3List(Node startNode, Node endNode)
    {
        List<Vector3> pathV3 = new List<Vector3>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            pathV3.Add(currentNode.worldPos);
            currentNode = currentNode.parent;
        }

        pathV3.Reverse();

        return pathV3;
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
