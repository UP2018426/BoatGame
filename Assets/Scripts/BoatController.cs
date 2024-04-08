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
    public float maxSteerAngle;


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
        rb.AddForceAtPosition(engineTransform.forward * (power * throttle), engineTransform.position, ForceMode.Force);
        // Steering Force
        rb.AddForceAtPosition(engineTransform.right * (steering * -maxSteerAngle), engineTransform.position, ForceMode.Force);
    }


}
