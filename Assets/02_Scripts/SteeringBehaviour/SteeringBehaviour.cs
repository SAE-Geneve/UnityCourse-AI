using System;
using System.Linq;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SteeringBehaviour : MonoBehaviour
{

    [SerializeField] public float _maxSpeed = 10f;
    [SerializeField][Range(0,1)] public float _turnFactor = 0.5f;

    [Header("Seek")]
    [SerializeField] [Range(0, 1)] private float _seekFactor = 1f;

    [Header("Wander")]
    [SerializeField] [Range(0, 1)] private float _wanderFactor = 1f;
    [SerializeField] private float _wanderDistance;
    [SerializeField] private float _wanderRadius;
    [SerializeField] private float _wanderRange = 180;

    [Header("Avoidance")]
    [SerializeField] [Range(0, 1)] private float _avoidanceFactor = 1f;
    [SerializeField] private LayerMask _avoidanceMask;
    [SerializeField] private float _avoidanceDistance = 10;
    [SerializeField] private float _avoidanceForce;
    [SerializeField] private float _avoidanceRadius;

    private Rigidbody rb;
    private Transform target;
    private float wanderAngle;
    private Vector3 wanderCenter;
    private Vector3 avoidPoint;
    private Vector3 avoidHitPoint;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        target = GameObject.FindGameObjectWithTag("Player").transform;
        wanderAngle = Random.Range(-180, 180);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Vector3 totalForce = Vector3.zero;
        totalForce += _seekFactor * Seek();
        Debug.DrawRay(transform.position, totalForce, new Color(.25f, .25f, .25f));
        totalForce += _wanderFactor * Wander();
        Debug.DrawRay(transform.position, totalForce, new Color(.5f, .5f, .5f));
        totalForce += _avoidanceFactor * ObstacleAvoidance();
        Debug.DrawRay(transform.position, totalForce, new Color(.75f, .75f, .75f));

        rb.linearVelocity += totalForce;

        // So, what's the magnitude ?
        float magnitude = rb.linearVelocity.magnitude;
        // Debug.Log("Velocity magnitude = " + magnitude);
        // Dont go too speed
        if (magnitude > _maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * _maxSpeed;
        }
        // Look forward
        if (magnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z));
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if(_wanderFactor > 0)
        {
            Gizmos.DrawWireSphere(wanderCenter, _wanderRadius);
        }

        if (rb != null && _avoidanceFactor > 0)
        {
           //Gizmos.DrawWireSphere(transform.position, _avoidanceRadius);
           if (Vector3.Distance(avoidPoint, transform.position) > Mathf.Epsilon && Vector3.Distance(avoidHitPoint, transform.position) > Mathf.Epsilon)
           {
               Gizmos.DrawWireSphere(avoidPoint, _avoidanceRadius);
               Gizmos.DrawWireSphere(avoidHitPoint, _avoidanceRadius);
           }
        }


    }

    Vector3 Seek()
    {
        if (_seekFactor == 0)
            return Vector3.zero;

        Vector3 desiredVelocity = target.position - transform.position;
        Vector3 currentVelocity = rb.linearVelocity;

        Vector3 seekVelocity = desiredVelocity - currentVelocity;

        // Debug 
        // Debug.DrawRay(transform.position, currentVelocity, Color.red * new Color(1,1,1,0.75f));
        // Debug.DrawRay(transform.position, desiredVelocity, Color.blue * new Color(1,1,1,0.75f));
        // Debug.DrawRay(transform.position + currentVelocity, seekVelocity, Color.yellow * new Color(1,1,1,0.75f));

        return seekVelocity;

    }

    Vector3 Wander()
    {

        if (_wanderFactor == 0)
            return Vector3.zero;

        Vector3 velocity = rb.linearVelocity.normalized;
        wanderCenter = transform.position + velocity * _wanderDistance;

        wanderAngle += Random.Range(-_wanderRange, _wanderRange);
        Vector3 wanderPoint = wanderCenter + Quaternion.Euler(0, wanderAngle, 0) * Vector3.forward * _wanderRadius;
 
        Vector3 desiredVelocity = wanderPoint - transform.position;
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 seekVelocity = desiredVelocity - currentVelocity;

        // Debug 1/2
        Debug.DrawRay(transform.position, wanderCenter - transform.position, Color.green);
        Debug.DrawRay(wanderCenter, wanderPoint - wanderCenter, Color.green);
        // Debug 2/2
        Debug.DrawRay(transform.position, currentVelocity, Color.red * new Color(1, 1, 1, 0.75f));
        Debug.DrawRay(transform.position, desiredVelocity, Color.blue * new Color(1, 1, 1, 0.75f));
        Debug.DrawRay(transform.position + currentVelocity, seekVelocity, Color.yellow * new Color(1, 1, 1, 0.75f));

        return seekVelocity;

    }

    Vector3 ObstacleAvoidance()
    {
        if (_avoidanceFactor == 0)
            return Vector3.zero;

        Vector3 currentVelocity = rb.linearVelocity;
        if (currentVelocity.magnitude < 0.001f)
            return Vector3.zero;

        if (Physics.SphereCast(transform.position,
            _avoidanceRadius,
            currentVelocity.normalized,
            out RaycastHit hit,
            _avoidanceDistance,
            _avoidanceMask))
        {

            Debug.Log("Obstacle Avoided : " + hit.collider.gameObject.name);

            Vector3 hitVector = hit.point - transform.position;
            avoidPoint = hit.point + Vector3.Reflect(hitVector, hit.normal).normalized * _avoidanceForce;
            avoidHitPoint = hit.point;
            
            Vector3 desiredVelocity = avoidPoint - transform.position;
            Vector3 seekVelocity = desiredVelocity - currentVelocity;


            // Debug 1/2
            Debug.DrawRay(transform.position, hitVector, Color.green);
            Debug.DrawRay(hit.point, hit.normal * 5f, Color.cyan);
            Debug.DrawRay(hit.point, avoidPoint - hit.point, Color.magenta);
            
            // Debug
            Debug.DrawRay(transform.position, currentVelocity, Color.red * new Color(1, 1, 1, 0.75f));
            Debug.DrawRay(transform.position, desiredVelocity, Color.blue * new Color(1, 1, 1, 0.75f));
            Debug.DrawRay(transform.position + currentVelocity, seekVelocity, Color.yellow * new Color(1, 1, 1, 0.75f));

            return seekVelocity;


        }
        else
        {
            avoidPoint = transform.position;
            avoidHitPoint = transform.position;
        }

        return Vector3.zero;

    }

    private void OnTriggerEnter(Collider other)
    {
        Transform obstacle;
    }
}
