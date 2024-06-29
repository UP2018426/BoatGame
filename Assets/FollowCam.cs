using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    //[SerializeField] internal List<GameObject> cars;

    public BoatAi boatAi;
    [SerializeField] private Vector2 mouseSensitivity;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private float radius;
    //private int currentCarIndex = 0;
    private Vector3 currentRotation;

    [SerializeField] private float minVerticalAngle = -10.0f;
    [SerializeField] private float maxVerticalAngle = 75.0f;
    
    [SerializeField] private float minZoom = 10.0f;
    [SerializeField] private float maxZoom = 25.0f;

    void Start()
    {
        currentRotation = transform.eulerAngles;
    }

    private void Update()
    {
        if (boatAi/* && Check if focus is on small cam*/)
        {
            radius -= Input.mouseScrollDelta.y;
            radius = Mathf.Clamp(radius, minZoom, maxZoom);

            if (Input.GetMouseButtonDown(1))
            {
                LockCursor();
            }

            if (Input.GetMouseButtonUp(1))
            {
                UnlockCursor();
            }

            if (Input.GetMouseButton(1))
            {
                currentRotation.y += Input.GetAxis("Mouse X") * mouseSensitivity.x;
                currentRotation.x -= Input.GetAxis("Mouse Y") * mouseSensitivity.y;
            }
        
            // Apply rotations
        
            currentRotation.x = Mathf.Clamp(currentRotation.x, minVerticalAngle, maxVerticalAngle);
            Quaternion rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0);
            Vector3 direction = rotation * -Vector3.forward;
            Vector3 desiredPosition = boatAi.transform.position + direction * radius;
        
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.LookAt(boatAi.cameraLookPosition + boatAi.transform.position);
        }
    }

    void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    /*public void NextCar()
    {
        currentCarIndex++;
        if (currentCarIndex > cars.Count - 1)
            currentCarIndex = 0;
    }

    public void PreviousCar()
    {
        currentCarIndex--;
        if (currentCarIndex < 0)
            currentCarIndex = cars.Count - 1;
    }*/
}
