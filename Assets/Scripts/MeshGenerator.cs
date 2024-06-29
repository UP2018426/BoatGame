using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] verticies;
    int[] triangles;

    //public Transform parentTransform;
    public Vector3 parentPosition;

    [SerializeField, Range(0,256)]
    private int gridSize = 256;

    [SerializeField]
    bool updateMesh = false;

    private float startPositionX, startPositionZ;

    // Start is called before the first frame update
    void Start()
    {
        SetMeshPosition();

        /*mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;*/

        /*CreatePlaneMesh();
        UpdateMesh();*/

        //startPositionX = parentPosition.x;
        //startPositionZ = parentPosition.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (updateMesh)
        {
            CreatePlaneMesh();
            UpdateMesh();
        }
    }

    public void SetMeshPosition()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreatePlaneMesh();
        UpdateMesh();
    }

    void CreatePlaneMesh()
    {
        verticies = new Vector3[(gridSize + 1) * (gridSize + 1)];

        for (int i = 0, x = 0; x <= gridSize; x++)
        {
            for (int z = 0; z <= gridSize; z++)
            {
                verticies[i] = new Vector3(z + parentPosition.x - (gridSize / 2), 0, x + parentPosition.z - (gridSize / 2));
                i++;
            }
        }

        triangles = new int[gridSize * gridSize * 6];

        int vert = 0;
        int tri = 0;

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                triangles[tri + 0] = vert + 0;
                triangles[tri + 1] = vert + gridSize + 1;
                triangles[tri + 2] = vert + 1;
                triangles[tri + 3] = vert + 1;
                triangles[tri + 4] = vert + gridSize + 1;
                triangles[tri + 5] = vert + gridSize + 2;

                vert++;
                tri += 6;
            }

            vert++;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = verticies;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}
