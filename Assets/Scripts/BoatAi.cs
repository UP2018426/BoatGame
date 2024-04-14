using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BoatAi : MonoBehaviour
{
    //public Vector3 targetLocation;
    public Transform target;
    public Vector3 navTarget;

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
        navTarget = CalculateSeekDirection(target.position);

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
        targetVector3.y = 0f;
        Vector3 thisPosition = new Vector3(transform.position.x, 0 , transform.position.z);
        Vector3 DesiredVelocity = (targetVector3 - thisPosition).normalized * maxSpeed;

        Vector3 velocity = CalculateCurrentVelocity();
        velocity.y = 0f;

        return DesiredVelocity - velocity;
    }

    void UpdateSteeringValue()
    {
        //thisBoat.steering = (CalculateDirectionToTarget(target.position) / 180) * steerStrength;
        thisBoat.steering = (CalculateDirectionToTarget(navTarget + transform.position) / 180) * steerStrength;

        Mathf.Clamp(thisBoat.steering, -1, 1);
    }

    private void OnDrawGizmos()
    {
        if (!rb) {  return; }
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(navTarget + transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, navTarget + transform.position);
    }
}
