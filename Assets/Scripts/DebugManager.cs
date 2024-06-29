using System;
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

    [SerializeField] private Camera mapCamera, shipCamera;
    
    public bool inspectingShip = false;

    private Transform selectedShip;

    private BoatAi selectedShipBoatAi;

    [SerializeField] private TextMeshProUGUI statusText, pathfindingText, boatSpeedText;

    [SerializeField] private GameObject shipCameraOutput;

    private void Start()
    {
        inspectingShip = false;

        if (shipCamera.gameObject.GetComponent<FollowCam>().boatAi == null)
        {
            shipCameraOutput.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeMultiplier = slider.value;

        currentSpeedText.text = timeMultiplier.ToString("F2") + speedText;
        
        if (timeMultiplier != timeMultiplierLaterFrame && increaseSpeed)
        {
            timeMultiplierLaterFrame = timeMultiplier;

            Time.timeScale = timeMultiplier;
            Time.fixedDeltaTime = 0.02f * timeMultiplier;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!inspectingShip)
            {
                Vector3 mouseHitPosition = Input.mousePosition;

                Ray clickRay = mapCamera.ScreenPointToRay(mouseHitPosition);

                RaycastHit hit;

                if (Physics.Raycast(clickRay, out hit))
                {
                    if (hit.transform.CompareTag("Boat"))
                    {
                        selectedShip = hit.transform;
                        Debug.Log("Selected ship is: " + selectedShip.name);

                        selectedShipBoatAi = selectedShip.GetComponent<BoatAi>();
                        
                        UseShipCamera(selectedShipBoatAi);
                    }
                    else
                    {
                        shipCameraOutput.SetActive(false);
                    }
                }
                else
                {
                    shipCameraOutput.SetActive(false);
                }
            }
        }

        if (selectedShipBoatAi)
        {
            statusText.text = "Status: " + selectedShipBoatAi.GetBoatAiState().ToString();
            //pathfindingText.text = selectedShipBoatAi.
            boatSpeedText.text = "Speed: " + selectedShipBoatAi.transform.GetComponent<Rigidbody>().velocity.magnitude.ToString("F1");
        }
    }

    void UseMapCamera()
    {
        shipCamera.transform.gameObject.SetActive(false);
        
        mapCamera.transform.gameObject.SetActive(true);
    }

    void UseShipCamera(BoatAi boat)
    {
        //mapCamera.transform.gameObject.SetActive(false);
        
        //shipCamera.transform.gameObject.SetActive(true);

        FollowCam followCamera = shipCamera.GetComponent<FollowCam>();
        
        followCamera.boatAi = boat;
        
        shipCameraOutput.SetActive(true);

        //shipCamera.transform.parent = boat.transform;

        //shipCamera.transform.localPosition = boat.cameraOffset;

        //shipCamera.transform.LookAt(boat.transform.position + boat.cameraLookPosition);
    }
}
