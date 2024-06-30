using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class BoatAi : MonoBehaviour
{
    public enum BoatAiStates
    {
        None = 0,
        Idle,
        Reversing,
        Wandering,
        Travelling,
        Following
    }

    enum BoatAiRole
    {
        None = 0,
        Merchant,
        Fishing,
        Tanker,
    }

    public Transform target;

    [SerializeField] private Vector3 navigationTarget;

    private BoatController thisBoatController;

    [SerializeField] private BoatAiRole role;

    [SerializeField] private BoatAiStates boatAiState;
    public BoatAiStates GetBoatAiState() { return boatAiState; }
    
    private BoatAiStates lastAiState;

    [SerializeField] public float idleTime;

    [SerializeField] private List<Transform> portTransforms;

    [SerializeField] private float steerStrength = 2;

    [SerializeField] private float arriveSlowingDistance;

    [SerializeField] private float stopDistance;

    [SerializeField] private float speedMultiplier;
    
    private Rigidbody rb;

    private Pathfinding pathfinding;

    [SerializeField] private LayerMask pathfindIgnore;

    [SerializeField] private List<BoatController> boatsInRange = new();

    [SerializeField] private float sightRange = 25.0f;

    [Header("Follow Camera")]
    public Vector3 cameraLookPosition;

    public float AngleWeightScalar, DistanceWeightScalar;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        thisBoatController = GetComponent<BoatController>();
        pathfinding = GetComponent<Pathfinding>();

        if (boatAiState == BoatAiStates.None)
        {
            if (role == BoatAiRole.Merchant)
            {
                boatAiState = BoatAiStates.Travelling;
            }
        }
        
        // Create a Sphere Collider trigger 
        SphereCollider colldier = gameObject.AddComponent<SphereCollider>();
        colldier.isTrigger = true;
        colldier.center = new Vector3(0, 0, 0);
        colldier.radius = sightRange;
    }

    void Update()
    {
        if (lastAiState != boatAiState)
        {
            lastAiState = boatAiState;
            AiStateChanged(boatAiState);
        }
        
        if (boatAiState == BoatAiStates.Idle)
        {
            idleTime -= Time.deltaTime;
            if (idleTime < 0.0f)
            {
                boatAiState = BoatAiStates.Travelling;
            }
        }
        
        if (boatAiState == BoatAiStates.Reversing)
        {
            idleTime -= Time.deltaTime;
            if (idleTime < 0.0f)
            {
                idleTime = 10f;
                boatAiState = BoatAiStates.Idle;
            }
            
            thisBoatController.throttle = -1f;
            thisBoatController.steering = 0f;
        }
        
        if (boatAiState == BoatAiStates.Travelling)
        {
            UpdateNavigationTarget(target);
            
            UpdateSteeringValue(navigationTarget);
            
            thisBoatController.throttle = CalculateArriveVelocity(target.position);
        }
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.TryGetComponent<Port>(out Port port))
        {
            if (target != null)
            {
                if (target.gameObject == port.gameObject)
                {
                    Debug.Log("Entered port");

                    boatAiState = BoatAiStates.Idle;
                }
            }
        }

        if (otherCollider.TryGetComponent<BoatController>(out BoatController otherBoat))
        {
            if (!boatsInRange.Contains(otherBoat))
            {
                boatsInRange.Add(otherBoat);
            }
        }
    }

    private void OnTriggerExit(Collider otherCollider)
    {
        if (otherCollider.TryGetComponent<BoatController>(out BoatController otherBoat))
        {
            if (boatsInRange.Contains(otherBoat))
            {
                boatsInRange.Remove(otherBoat);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Terrain"))
        {
            // Reverse for X seconds
            Debug.Log("Reversing");
            idleTime = 10f;
            boatAiState = BoatAiStates.Reversing;
        }
    }

    void AiStateChanged(BoatAiStates newState)
    {
        if (newState == BoatAiStates.Idle)
        {
            thisBoatController.steering = 0;
            thisBoatController.throttle = 0;

            idleTime = 5.0f;
        }

        if (boatAiState == BoatAiStates.Travelling)
        {
            FindTarget();
        }
    }

    List<Transform> GetAllPorts()
    {
        List<Transform> returnList = new();
        
        // TODO: Needs to be optimised. Maybe have a GameManager that stores all ports rather than recalculating
        GameObject[] ports = GameObject.FindGameObjectsWithTag("Port");

        foreach (GameObject port in ports)
        {
            returnList.Add(port.transform);
        }

        return returnList;
    }

    void FindTarget()
    {
        if (role == BoatAiRole.Merchant)
        {
            portTransforms.Clear();
            portTransforms = GetAllPorts();

            int closestIndex = 0;
            float closestDistance = float.MaxValue;

            // Find nearest port
            for (int i = 0; i < portTransforms.Count; i++)
            {
                float distFromPort = (portTransforms[i].position - transform.position).magnitude;
                if (distFromPort < closestDistance)
                {
                    closestDistance = (portTransforms[i].position - transform.position).magnitude;
                    closestIndex = i;
                }
            }
            
            portTransforms.RemoveAt(closestIndex);

            // Find a random port that is NOT the nearest one
            // Note: RangeMax is Exclusive not Inclusive!
            int randomIndex = Random.Range(0, portTransforms.Count);
            target = portTransforms[randomIndex];
        }
    }

    void UpdateNavigationTarget(Transform targetTransform)
    {
        Vector3 cutoffPoint = FindCutoffPoint(targetTransform);

        Vector3 rayDirection = cutoffPoint - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, rayDirection, out hit, rayDirection.magnitude, ~pathfindIgnore))
        {
            if (hit.collider.gameObject == target.gameObject)
            {
                navigationTarget = cutoffPoint;
                Debug.DrawLine(cutoffPoint, transform.position, Color.green);
            }
            
            // TODO: For now just grabbing the next node in the pathfinding solution. 
            // Instead, find the furthest visible pathfinding node and travel there 
            //navigationTarget = pathfinding.FindPathList(transform.position, targetTransform.transform.position)[0];
            else if (pathfinding.vector3Path.Count > 0)
            {
                navigationTarget = pathfinding.vector3Path[0];
                Debug.DrawLine(pathfinding.vector3Path[0], transform.position, Color.red);
            }
        }
        /*else
        {
            Debug.Log("Ray missed");
            
            navigationTarget = cutoffPoint;
            Debug.DrawLine(cutoffPoint, transform.position, Color.green);
        }*/
    }

    float CalculateDirectionToTarget(Vector3 targetVector3)
    {
        Vector3 targetDir = targetVector3 - transform.position;
        targetDir.y = 0f;

        float angle = Vector3.SignedAngle(transform.forward, targetDir, Vector3.up);
        return angle;
    }

    float CalculateAvoidanceForce()
    {
        if (boatsInRange.Count > 0)
        {
            List<Vector3> cutoffPoints = new();
            List<Vector3> cutoffPointsShortlist = new();
            // Have a list of all obstacles within distance D
            // For each object, determine that angle it is from the boat using the method in CalculateDirectionToTarget().
    
            for (int i = 0; i < boatsInRange.Count; i++)
            {
                cutoffPoints.Add(FindCutoffPoint(boatsInRange[i].gameObject.transform));
            }
            
            for (int i = 0; i < cutoffPoints.Count; i++)
            {
                float directionToTarget = CalculateDirectionToTarget(cutoffPoints[i]);
    
                if (directionToTarget is < 90 and > -90)
                {
                    cutoffPointsShortlist.Add(cutoffPoints[i]);
                    
                    /*float distance = (cutoffPoints[i] - transform.position).magnitude;
                    if (closestDistance > distance)
                    {
                        closestIndex = i;
                        closestDistance = distance;
                        indexDirectionToTarget = directionToTarget;
                    }*/
                }
            }
            
            int closestIndex = 0;
            float closestDistance = float.MaxValue;
            float indexDirectionToTarget = 0.0f;
    
            for (int i = 0; i < cutoffPointsShortlist.Count; i++)
            {
                float directionToTarget = CalculateDirectionToTarget(cutoffPointsShortlist[i]);
                //Debug.Log(directionToTarget);
                
                float distance = (cutoffPoints[i] - transform.position).magnitude;
                if (closestDistance > distance)
                {
                    closestIndex = i;
                    closestDistance = distance;
                    indexDirectionToTarget = directionToTarget;
                }
            }

            //float directionMultiplier = indexDirectionToTarget > 0 ? -1 : 1;

            // DirectionScale * Distancescale * direction
            if (cutoffPointsShortlist.Count > 0)
            {
                float angleWeight = CalculateDirectionToTarget(cutoffPointsShortlist[closestIndex]) / 90f;

                float distanceWeight = (((cutoffPointsShortlist[closestIndex] - transform.position).magnitude * -1) + sightRange) / sightRange;
                
                float rv = (-(angleWeight * AngleWeightScalar) + -(distanceWeight * DistanceWeightScalar));

                thisBoatController.throttle = distanceWeight;
                Debug.Log(distanceWeight);

                return rv;
            }
    
            //return /*float steerForce = */indexDirectionToTarget > 0 ? -1 : 1;
    
            // For each object within a FOV angle F, calculate its cutoff point using the existing function FindCutoffPoint(). maybe dont have FOV???
            // For each cutoff point that lands in a smaller dot product value in front of the ship, find the nearest one
            // Find if this point is left or right of us
            // Find the dot product to the point (PointAngle).
            // If the point is to the left of us do PointAngle * -1 (flip the angle)
            // Calculate some multipliers for steering
            // Our distance to the point
            // Our speed
            // ???
            // Return PointAngle * "some multipliers for steering" 
    
        } 
        
        return 0.0f;
    }

    Vector3 CalculateCurrentVelocity()
    {
        return rb.velocity;
    }

    Vector3 CalculateSeekDirection(Vector3 targetVector3)
    {
        targetVector3.y = 0f;
        Vector3 thisPosition = new Vector3(transform.position.x, 0 , transform.position.z);
        Vector3 DesiredVelocity = (targetVector3 - thisPosition).normalized * CalculateCurrentVelocity().magnitude;

        Vector3 velocity = CalculateCurrentVelocity();
        velocity.y = 0f;

        return DesiredVelocity - velocity;
    }

    Vector3 FindCutoffPoint(Transform targetTransform)
    {
        Vector3 targetVector = CleanVector(targetTransform.position);
        Vector3 thisVector = CleanVector(transform.position);

        if (targetTransform.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidbodyComponent))
        {
            if (rigidbodyComponent)
            {
                float lookAheadTime = (targetVector - thisVector).magnitude / (rb.velocity.magnitude + rigidbodyComponent.velocity.magnitude);

                Vector3 cutoffPoint = targetVector + rigidbodyComponent.velocity * lookAheadTime;

                return CleanVector(cutoffPoint);
            }
        }
        
        return targetVector;
    }

    void UpdateSteeringValue(Vector3 targetPos)
    {
        thisBoatController.steering = (CalculateDirectionToTarget(targetPos) / 180) * steerStrength;

        if (boatsInRange.Count > 0)
        {
            thisBoatController.steering += CalculateAvoidanceForce();
        }
        
        Mathf.Clamp(thisBoatController.steering, -1, 1);
    }

    float CalculateArriveVelocity(Vector3 target)
    {
        float distanceToTarget = (target - this.transform.position).magnitude;

        if (distanceToTarget < stopDistance)
        {
            return 0.0f;
        }
        if (distanceToTarget < arriveSlowingDistance) 
        {
            float speed = distanceToTarget / arriveSlowingDistance;

            speed = Mathf.Clamp01(speed);

            return speed;
        }

        return 1.0f;
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
