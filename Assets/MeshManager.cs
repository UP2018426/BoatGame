using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshManager : MonoBehaviour
{
    public Transform targetShipTransform;
    
    [SerializeField] private int gridSize, halfGridSize;
    private float startPositionX, startPositionZ;

    public int meshArraySize;
    
    void Start()
    {
        startPositionX = targetShipTransform.transform.position.x;
        startPositionZ = targetShipTransform.transform.position.z;
    }

    void FixedUpdate()
    {
        float halfGrid = gridSize / 2;
        float positionX = targetShipTransform.transform.position.x;
        float positionZ = targetShipTransform.transform.position.z;
        
        transform.position = new Vector3(startPositionX, transform.position.y, transform.position.z);
        if (positionX > startPositionX + halfGrid) startPositionX += gridSize;
        else if (positionX < startPositionX - halfGrid) startPositionX -= gridSize;
        
        transform.position = new Vector3(transform.position.x, transform.position.y, startPositionZ);
        if (positionZ > startPositionZ + halfGrid) startPositionZ += gridSize;
        else if (positionZ < startPositionZ - halfGrid) startPositionZ -= gridSize;
        
        transform.position = new Vector3(startPositionX, transform.position.y, startPositionZ);
    }
}
