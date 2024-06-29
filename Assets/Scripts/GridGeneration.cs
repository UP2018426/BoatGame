using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGeneration : MonoBehaviour
{
    public GameObject meshPrefab;
    public int gridSize = 5;
    public float meshSpacing = 1.0f;

    [SerializeField] private List<GameObject> meshList;

    public bool refreshMesh = false;

    void Start()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        GenerateMeshArray();
#endif
    }

    private void Update()
    {
        if (refreshMesh)
        {
            refreshMesh = false;

            /*foreach (var mesh in meshList)
            {
                //mesh.GetComponent<MeshGenerator>().parentTransform = transform.parent;
                //mesh.GetComponent<MeshGenerator>().UpdateMesh();
            }*/
        }
    }

    void GenerateMeshArray()
    {
        Vector3 startPosition = transform.position;

        for (int x = -gridSize / 2; x <= gridSize / 2; x++)
        {
            for (int y = -gridSize / 2; y <= gridSize / 2; y++)
            {
                // Skip the center point if you don't want a mesh at the starting position
                //if (x == 0 && y == 0) continue; 

                Vector3 position = startPosition + new Vector3(x * meshSpacing, 0, y * meshSpacing);
                GameObject wave = Instantiate(meshPrefab, Vector3.zero, Quaternion.identity);

                MeshGenerator waveMeshGenerator = wave.GetComponent<MeshGenerator>();

                waveMeshGenerator.parentPosition = position;
                
                waveMeshGenerator.SetMeshPosition();
                
                //waveMeshGenerator.UpdateMesh();
                
                //wave.transform.parent = this.transform;
                
                meshList.Add(wave);
            }
        }
    }
}
