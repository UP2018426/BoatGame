using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    public Rigidbody rb;
    public float depthBeforeSubmerged = 1f;
    public float displacementAmount= 3f;
    public int buoyancyPoints;
    public float waterDrag = 0.99f;
    public float waterAngularDrag = 0.5f;

    private void Start()
    {
        if (rb == null)
        {
            if (TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
            {
                rb = rigidbody;
            }
            else if(this.gameObject.transform.parent != null)
            {
                if(this.gameObject.transform.parent.TryGetComponent<Rigidbody>(out Rigidbody parentRigidbody))
                {
                    rb = parentRigidbody;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        //Debug.Log(buoyancyPoints);
        rb.AddForceAtPosition(Physics.gravity / buoyancyPoints, transform.position, ForceMode.Acceleration);

        float waveHeight = WaveManager.instance.CalculateWaveHeight(transform.position);
        
        if (transform.position.y < waveHeight)
        {
            float displacementMultiplier = Mathf.Clamp01((waveHeight -transform.position.y) / depthBeforeSubmerged) * displacementAmount;

            rb.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0f), transform.position, ForceMode.Acceleration);
            rb.AddForce(displacementMultiplier * -rb.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            rb.AddTorque(displacementMultiplier * -rb.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }
}
