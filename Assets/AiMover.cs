using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiMover : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 1f;
    [SerializeField]
    float rangeToTurn = 10f;

    bool bMovingAway = true;

    [SerializeField]
    BoatAi boatAi;

    [SerializeField]
    bool followAI;

    public Rigidbody rb;

    public Vector3 rbVelocity;
    public Vector3 myVelocity;

    public Vector3 force;

    private void Start()
    {
        boatAi = GetComponent<BoatAi>();
    }

    void Update()
    {
        /*if (!followAI)
        {
            if(transform.position.x > rangeToTurn)
            {
                bMovingAway = false;
            }
            else if(transform.position.x < -rangeToTurn)
            {
                bMovingAway = true;
            }

            if (bMovingAway)
            {
                transform.position = new Vector3(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x - moveSpeed * Time.deltaTime, transform.position.y, transform.position.z);
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, boatAi.seekDirection, boatAi.maxSpeed * Time.deltaTime);
        }*/
    }
}
