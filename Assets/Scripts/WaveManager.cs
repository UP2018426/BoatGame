using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    public float simulationSpeed    = 1.0f;
    public float timePlayed         = 0.0f;
    [Range(-2.0f, 2.0f)]
    public float offset             = 0.0f;

    // Wave properties. At the moment these need to be set up manually.  
    // TODO: Find a way to find this automatically from the shader
    public Vector4 waveA = new Vector4(1, 0, 0.5f, 10);
    public Vector4 waveB = new Vector4(0, 1, 0.25f, 20);
    public Vector4 waveC = new Vector4(1, 1, 0.15f, 10);

    public Renderer waveRenderer;
    public Shader waveShader;

    private void Awake()
    {
        timePlayed = 0;

        // Create a Singleton
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            // Destroy this if another WaveManager already exists
            Destroy(this);
        }
    }

    private void Update()
    {
        // Update the time since start
        timePlayed += Time.deltaTime * simulationSpeed;

        if (waveRenderer != null)
        {
            waveRenderer.material.SetVector("_WaveA", waveA);
            waveRenderer.material.SetVector("_WaveB", waveB);
            waveRenderer.material.SetVector("_WaveC", waveC);
        }
    }

    private float GerstnerWave(Vector4 wave, Vector3 point, float time)
    {
        // Matching Gerstner Wave calculation to the shader
        float steepness = wave.z;
        float wavelength = wave.w;
        float k = (2 * Mathf.PI) / wavelength;
        float c = Mathf.Sqrt(9.8f / k);
        Vector2 d = new Vector2(wave.x, wave.y).normalized;
        float f = k * (Vector2.Dot(d, new Vector2(point.x, point.z)) - c * time);
        float a = steepness / k;

        float waveHeight = d.x * (a * Mathf.Cos(f)) + a * Mathf.Sin(f) + d.y * (a * Mathf.Cos(f));

        return waveHeight;
    }

    // Call this method to get the height of the wave at a certain coordinate
    public float CalculateWaveHeight(Vector3 point)
    {
        float height = 0;

        height += GerstnerWave(waveA, point, timePlayed + offset);
        height += GerstnerWave(waveB, point, timePlayed + offset);
        height += GerstnerWave(waveC, point, timePlayed + offset);

        return height;
    }

}
