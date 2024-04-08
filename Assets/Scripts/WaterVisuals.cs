using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
// This will add the appropriate components on the object if they don't already exist

public class WaterVisuals : MonoBehaviour
{
    // This is in charge of updating the position of the vertices on the Vertex Wave
    // THE WaterManager IS JUST FOR VISUALISATION PURPOSES

    private MeshFilter meshFilter;

#if UNITY_EDITOR
    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }
#endif

    private void Update()
    {
        Vector3[] verts = meshFilter.mesh.vertices;
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i].y = WaveManager.instance.CalculateWaveHeight(transform.position + verts[i]);
        }

        // Need to make sure we recalculate the mesh otherwise it will look flat
        meshFilter.mesh.vertices = verts;
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.RecalculateTangents();
    }
}
