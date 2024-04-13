using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BoatAi : MonoBehaviour
{
    //public Vector3 targetLocation;
    public Transform target;

    public BoatController thisBoat;

    [SerializeField]
    float directionToTarget = 0; // The direction towards the target from -1 to 1

    [SerializeField]
    float distanceToTarget;

    [SerializeField]
    float speedLerp, turnLerp;

    [SerializeField]
    float steerStrength;

    [SerializeField]
    public float maxSpeed;

    public Vector3 seekDirection;

    Rigidbody rb;

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
        UpdateSteeringValue();
    }

    float CalculateDirectionToTarget(Vector3 targetVector3)
    {
        Vector3 targetDir = targetVector3 - transform.position;
        targetDir.y = 0f;

        float angle = Vector3.SignedAngle(transform.forward, targetDir, Vector3.up);
        return angle;
    }

    Vector3 CalculateCurrentVelocity()
    {
        //return (transform.position - lastFramePosition) / Time.deltaTime;
        return rb.velocity;
    }

    Vector3 CalculateSeekDirection(Vector3 targetVector3)
    {
        Vector3 DesiredVelocity = (targetVector3 - transform.position).normalized * maxSpeed;

        return DesiredVelocity - CalculateCurrentVelocity();
    }

    void UpdateSteeringValue()
    {
        thisBoat.steering = CalculateDirectionToTarget(target.position) * steerStrength;

        Mathf.Clamp(thisBoat.steering, -1, 1);
    }

    private void OnDrawGizmos()
    {
        if (!rb) {  return; }
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(CalculateSeekDirection(target.position), 0.5f);
        Gizmos.DrawLine(transform.position, CalculateSeekDirection(target.position));
    }
}
