using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField]
    private Transform centerOfMass, engineTransform;

    // Controls
    [Range(0, 1)]
    public float throttle;
    public float power;

    [Range(-1, 1)]
    public float steering;
    public float steerForce;
    public float steerForceMax;
    public float steeringSpeedMultiplier;


    void Start()
    {
        Buoyancy[] buoys = transform.GetComponentsInChildren<Buoyancy>();

        rb = GetComponent<Rigidbody>();

        for (int i = 0; i < buoys.Length; i++)
        {
            buoys[i].buoyancyPoints = buoys.Length;
        }

        if (centerOfMass != null)
        {
            rb.automaticCenterOfMass = false;
        }
        else
        {
            rb.automaticCenterOfMass = true;
        }
    }

    void Update()
    {
        rb.centerOfMass = centerOfMass.localPosition;

        // Apply Power
        rb.AddForceAtPosition(engineTransform.forward * (power * throttle), engineTransform.position, ForceMode.Acceleration);

        // Steering Force

        float steeringMultiplier = rb.velocity.magnitude * steeringSpeedMultiplier;
        //Debug.Log(steeringMultiplier);
        steeringMultiplier = -steerForce * Mathf.Clamp(steeringMultiplier, 1, steerForceMax);


        rb.AddForceAtPosition(engineTransform.right * (steering * steeringMultiplier), engineTransform.position, ForceMode.Force);
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.LogWarning(gameObject.name + " has collided with " + other.gameObject.name);
    }
}
