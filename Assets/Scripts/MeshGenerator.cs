using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] verticies;
    int[] triangles;

    public Transform parentTransform;

    [SerializeField, Range(0,256)]
    private int xSize = 10;

    [SerializeField, Range(0,256)]
    private int zSize = 10;

    [SerializeField]
    bool updateMesh = false;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreatePlaneMesh();
        UpdateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        if (true)
        {
            CreatePlaneMesh();
            UpdateMesh();

            updateMesh = false;
        }
    }

    void CreatePlaneMesh()
    {
        verticies = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, x = 0; x <= xSize; x++)
        {
            for (int z = 0; z <= zSize; z++)
            {
                verticies[i] = new Vector3(z + parentTransform.position.x - (xSize / 2), 0, x + parentTransform.position.z - (zSize / 2));
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tri = 0;

        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                triangles[tri + 0] = vert + 0;
                triangles[tri + 1] = vert + zSize + 1;
                triangles[tri + 2] = vert + 1;
                triangles[tri + 3] = vert + 1;
                triangles[tri + 4] = vert + zSize + 1;
                triangles[tri + 5] = vert + zSize + 2;

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
