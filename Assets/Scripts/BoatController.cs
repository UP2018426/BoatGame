using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField]
    private Transform COM, engineTransform;

    // Controls
    [Range(0, 1)]
    public float throttle;
    public float power;

    [Range(-1, 1)]
    public float steering;
    public float steerForce;
    public float steerForceMax;
    public float steeringSpeedMultiplier;


    // Start is called before the first frame update
    void Start()
    {
        Buoyancy[] buoys = transform.GetComponentsInChildren<Buoyancy>();

        rb = GetComponent<Rigidbody>();

        for (int i = 0; i < buoys.Length; i++)
        {
            buoys[i].buoyancyPoints = buoys.Length;
        }

        if (COM != null)
        {
            rb.automaticCenterOfMass = false;
        }
        else
        {
            rb.automaticCenterOfMass = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        rb.centerOfMass = COM.localPosition;

        // Apply Power
        rb.AddForceAtPosition(engineTransform.forward * (power * throttle), engineTransform.position, ForceMode.Acceleration);

        // Steering Force

        float steeringMultiplier = rb.velocity.magnitude * steeringSpeedMultiplier;
        Debug.Log(steeringMultiplier);
        steeringMultiplier = -steerForce * Mathf.Clamp(steeringMultiplier, 1, steerForceMax);


        rb.AddForceAtPosition(engineTransform.right * (steering * steeringMultiplier), engineTransform.position, ForceMode.Force);

        //Debug.Log("Rigidbody: " + rb.velocity);
    }
}
