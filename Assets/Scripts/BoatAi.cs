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

    [SerializeField]
    float arriveSlowingDistance;

    [SerializeField]
    float speedMultiplier;

    Rigidbody rb;

    public bool enableDebug = false;

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
        UpdateSteeringValue(FindCutoffPoint(target));

        thisBoat.throttle = CalculateArriveVelocity(target);
        //Debug.Log(target.transform.position);
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

    Vector3 FindCutoffPoint(Transform targetTransform)
    {
        Vector3 targetVector = CleanVector(targetTransform.position);
        Vector3 thisVector = CleanVector(transform.position);

        Rigidbody targetRb = null;
        if (targetTransform.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
        {
            targetRb = rigidbody;
        }

        if (targetRb != null)
        {
            float lookAheadTime = (targetVector - thisVector).magnitude / (rb.velocity.magnitude + targetRb.velocity.magnitude);

            Vector3 cutoffPoint = targetVector + targetRb.velocity * lookAheadTime;

            return CleanVector(cutoffPoint);
        }

        return target.position;
    }

    void UpdateSteeringValue(Vector3 targetPos)
    {
        thisBoat.steering = (CalculateDirectionToTarget(targetPos) / 180) * steerStrength;

        Mathf.Clamp(thisBoat.steering, -1, 1);
    }

    float CalculateArriveVelocity(Transform target)
    {
        float distanceToTarget = (target.transform.position - this.transform.position).magnitude;

        if (distanceToTarget < arriveSlowingDistance) 
        {
            float speed = distanceToTarget / arriveSlowingDistance;

            speed = Mathf.Clamp01(speed);

            return speed;
        }

        return 1;
    }

    Vector3 CleanVector(Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }

    private void OnDrawGizmos()
    {
        if (!rb) {  return; }
        Gizmos.color = Color.red;
        /*Gizmos.DrawSphere(CalculateSeekDirection(target.transform.position) + transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, CalculateSeekDirection(target.transform.position) + transform.position);*/
        //Gizmos.DrawSphere(FindCutoffPoint(target), 0.5f);
    }
}
