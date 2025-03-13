using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class ForwardSensor : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private float angleDetection = 45;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private string tag;

    [SerializeField] private Color gizmoColor;

    public bool HasDetected;
    public Vector3 TargetPos = Vector3.zero;
    
    private Vector3 hitPosition;

    public void OnDrawGizmos()
    {
        Gizmos.color = HasDetected ? Color.green : gizmoColor;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, angleDetection, 0) * transform.forward * radius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -angleDetection, 0) * transform.forward * radius);
        
        Gizmos.DrawLine(transform.position, hitPosition);
        
    }


    // Update is called once per frame
    void Update()
    {
        HasDetected = false;
        hitPosition = transform.position;
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask);
        
        Collider goodObject = colliders.FirstOrDefault(c => c.CompareTag(tag));
        if (goodObject != null)
        {
            Vector3 goodObjectDistance = goodObject.bounds.center - transform.position;
            float angle = Vector3.Angle(transform.forward, goodObjectDistance);
            
            if(angle < angleDetection)
            {
                if(Physics.Raycast(transform.position, goodObjectDistance, out RaycastHit hit, radius, layerMask))
                {
                    hitPosition = hit.point;
                    if (hit.collider == goodObject)
                    {
                        HasDetected = true;
                        TargetPos = goodObject.transform.position;
                    }
                }
            }
        }

    }
}
