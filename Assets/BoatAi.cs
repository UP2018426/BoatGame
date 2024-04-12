using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BoatAi : MonoBehaviour
{
    //public Vector3 targetLocation;
    public Transform targetLocation;

    [SerializeField]
    float directionToTarget = 0; // The direction towards the target from -1 to 1
    float angleToTarget = 0; // The direction towards the target from -360 to 360

    [SerializeField]
    float distanceToTarget;

    [SerializeField]
    float speedLerp, turnLerp;

    [SerializeField]
    public float maxSpeed;

    public Vector3 seekDirection;

    Rigidbody rb;

    // This is used to calculate the current velocity
    Vector3 lastFramePosition;

    enum BoatAiStates
    {
        Idle,
        Wandering,
        Patrolling,
        Following
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (targetLocation != null)
        {
            directionToTarget = CalculateDirectionToTarget();
            distanceToTarget = (targetLocation.position - transform.position).magnitude;
        }

        Debug.Log("Custom: " + CalculateCurrentVelocity());

        //seekDirection = CalculateSeekDirection();

        // This needs to be last!
        lastFramePosition = transform.position;
    }

    float CalculateDirectionToTarget()
    {
        //Vector3 temp = (targetLocation.position - transform.position).normalized;

        Vector3 directionToTarget = targetLocation.position - transform.position;

        // Calculate rotation to face the target
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

        // Convert quaternion to euler angles and extract the yaw angle
        float rv = targetRotation.eulerAngles.y;

        // Find opposite side on a compass
        if (rv >= 180)
        {
            rv -= 180;
        }
        else
        {
            rv += 180;
        }

        rv = (rv / 180) - 1;

        return rv;
        //return (Mathf.Atan(temp.x / temp.z) * 2) / Mathf.PI;
    }

    Vector3 CalculateCurrentVelocity()
    {
        //return (transform.position - lastFramePosition) / Time.deltaTime;
        return rb.velocity;
    }

    Vector3 CalculateSeekDirection()
    {
        Vector3 DesiredVelocity = (targetLocation.position - transform.position).normalized * maxSpeed;

        return DesiredVelocity - CalculateCurrentVelocity();
    }

    private void OnDrawGizmos()
    {
        if (!rb) {  return; }
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(CalculateSeekDirection(), 0.5f);
        Gizmos.DrawLine(transform.position, CalculateSeekDirection());
    }
}
