using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Port : MonoBehaviour
{
    public float interactionRange = 10f;
    private SphereCollider sphereCollider;

    [SerializeField]
    List<BoatAi> boatAiList;

    void Start()
    {
        sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.radius = interactionRange;
        sphereCollider.isTrigger = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, interactionRange);
    }
}
