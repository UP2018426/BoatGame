using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private bool increaseSpeed = false;
    [SerializeField, Range(1.0f, 15.0f)] private float timeMultiplier = 1.0f;
    private float timeMultiplierLaterFrame = 0.0f;

    [SerializeField] private TextMeshProUGUI currentSpeedText;

    [SerializeField] private string speedText;

    [SerializeField] private Slider slider;

    // Update is called once per frame
    void Update()
    {
        timeMultiplier = slider.value;

        currentSpeedText.text = timeMultiplier.ToString("F2") + speedText;
        
        if (timeMultiplier != timeMultiplierLaterFrame && increaseSpeed)
        {
            timeMultiplierLaterFrame = timeMultiplier;

            Time.timeScale = timeMultiplier;
            Time.fixedDeltaTime = 0.02f * timeMultiplierLaterFrame;
        }
    }
}
