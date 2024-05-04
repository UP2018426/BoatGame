using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private bool increaseSpeed = false;
    [SerializeField, Range(1.0f, 16.0f)] private float timeMultiplier = 1.0f;
    private float timeMultiplierLaterFrame = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if (timeMultiplier != timeMultiplierLaterFrame && increaseSpeed)
        {
            timeMultiplierLaterFrame = timeMultiplier;

            Time.timeScale = timeMultiplier;
            Time.fixedDeltaTime = 0.02f * timeMultiplierLaterFrame;
        }
    }
}
