using UnityEngine;
using UnityEngine.Serialization;

public class SteeringBehaviour : MonoBehaviour
{

    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float steeringDamp = 0.5f;

    [Header("Seek")]
    [SerializeField] [Range(0, 1)] private float seekFactor = 1f;

    [Header("Flee")]
    [SerializeField] [Range(0, 1)] private float fleeFactor = 1f;

    [Header("Arrival")]
    [SerializeField] private float arrivalRadius = 10f;

    [FormerlySerializedAs("_wanderFactor")]
    [Header("Wander")]
    [SerializeField] [Range(0, 1)] private float wanderFactor = 1f;
    [SerializeField] private float wanderDistance;
    [SerializeField] private float wanderRadius;
    [SerializeField] private float wanderRange = 180;

    [Header("Avoidance")]
    [SerializeField] [Range(0, 1)] private float avoidanceFactor = 1f;
    [SerializeField] private LayerMask avoidanceMask;
    [SerializeField] private float avoidanceDistance = 10;
    [SerializeField] private float avoidanceForce;
    [SerializeField] private float avoidanceRadius;

    // Avoid fields
    private Vector3 _avoidPoint;
    private Vector3 _avoidHitPoint;

    // Wander fields
    private float _wanderAngle;
    private Vector3 _wanderCenter;

    private Rigidbody _rb;
    private GameObject _steeringTarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _steeringTarget = GameObject.FindGameObjectWithTag("Player");

        _wanderAngle = Random.Range(-180, 180);

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 steeringResult = Vector3.zero;

        if (seekFactor > 0) 
            steeringResult += seekFactor * Seek(_steeringTarget.transform.position);
        if (fleeFactor > 0) 
            steeringResult += fleeFactor * Flee(_steeringTarget.transform.position);
        if (wanderFactor > 0) 
            steeringResult += wanderFactor * Wander();
        if (avoidanceFactor > 0) 
            steeringResult += avoidanceFactor * ObstacleAvoidance();

        _rb.linearVelocity = steeringResult;

        // Max speed
        if (_rb.linearVelocity.magnitude > maxSpeed)
            _rb.linearVelocity = _rb.linearVelocity.normalized * maxSpeed;

    }

    private void OnDrawGizmos()
    {
        if (avoidanceFactor > 0)
        {
            //Gizmos.DrawWireSphere(transform.position, _avoidanceRadius);
            if (Vector3.Distance(_avoidPoint, transform.position) > Mathf.Epsilon &&
                Vector3.Distance(_avoidHitPoint, transform.position) > Mathf.Epsilon)
            {
                Gizmos.DrawWireSphere(_avoidPoint, avoidanceRadius);
                Gizmos.DrawWireSphere(_avoidHitPoint, avoidanceRadius);
            }
        }

        if (wanderFactor > 0)
        {
            Gizmos.DrawWireSphere(_wanderCenter, wanderRadius);
        }

    }

    Vector3 Seek(Vector3 targetPosition)
    {

        Vector3 actualVelocity = _rb.linearVelocity;
        Vector3 desiredVelocity = (targetPosition - transform.position).normalized * maxSpeed;

        Vector3 steeringForce = (desiredVelocity - actualVelocity) * steeringDamp;

        // Debug draws
        Debug.DrawRay(transform.position, desiredVelocity, Color.magenta);
        Debug.DrawRay(transform.position, actualVelocity, Color.red);
        Debug.DrawRay(transform.position + actualVelocity, steeringForce, Color.green);

        return steeringForce;

    }

    Vector3 Flee(Vector3 targetPosition)
    {

        Vector3 actualVelocity = _rb.linearVelocity;
        Vector3 desiredVelocity = (transform.position - targetPosition).normalized * maxSpeed;

        Vector3 steeringForce = (desiredVelocity - actualVelocity) * steeringDamp;

        // Debug draws
        Debug.DrawRay(transform.position, desiredVelocity, Color.magenta);
        Debug.DrawRay(transform.position, actualVelocity, Color.red);
        Debug.DrawRay(transform.position + actualVelocity, steeringForce, Color.green);

        return steeringForce;

    }

    Vector3 SeekAndArrival(Vector3 targetPosition)
    {

        Vector3 actualVelocity = _rb.linearVelocity;

        float actualSpeed = Mathf.Lerp(0, maxSpeed, Vector3.Distance(targetPosition, transform.position) / arrivalRadius);
        Vector3 desiredVelocity = (targetPosition - transform.position).normalized * actualSpeed;

        Vector3 steeringForce = (desiredVelocity - actualVelocity) * steeringDamp;

        // Debug draws
        Debug.DrawRay(transform.position, desiredVelocity, Color.magenta);
        Debug.DrawRay(transform.position, actualVelocity, Color.red);
        Debug.DrawRay(transform.position + actualVelocity, steeringForce, Color.green);

        return steeringForce;

    }


    Vector3 ObstacleAvoidance()
    {
        if (avoidanceFactor == 0)
            return Vector3.zero;

        Vector3 actualVelocity = _rb.linearVelocity;
        if (actualVelocity.magnitude < 0.001f)
            return Vector3.zero;

        if (Physics.SphereCast(transform.position,
            avoidanceRadius,
            actualVelocity.normalized,
            out RaycastHit hit,
            avoidanceDistance,
            avoidanceMask))
        {

            Debug.Log("Obstacle Avoided : " + hit.collider.gameObject.name);

            Vector3 hitVector = hit.point - transform.position;
            _avoidPoint = hit.point + Vector3.Reflect(hitVector, hit.normal).normalized * avoidanceForce;
            _avoidHitPoint = hit.point;

            Vector3 desiredVelocity = _avoidPoint - transform.position;
            Vector3 steeringForce = (desiredVelocity - actualVelocity) * steeringDamp;


            // Debug 1/2
            Debug.DrawRay(transform.position, hitVector, Color.green);
            Debug.DrawRay(hit.point, hit.normal * 5f, Color.cyan);
            Debug.DrawRay(hit.point, _avoidPoint - hit.point, Color.yellow);

            // Debug
            // Debug draws
            Debug.DrawRay(transform.position, desiredVelocity, Color.magenta);
            Debug.DrawRay(transform.position, actualVelocity, Color.red);
            Debug.DrawRay(transform.position + actualVelocity, steeringForce, Color.green);

            return steeringForce;


        }
        else
        {
            _avoidPoint = transform.position;
            _avoidHitPoint = transform.position;
        }

        return Vector3.zero;

    }

    Vector3 Wander()
    {

        Vector3 velocity = _rb.linearVelocity.normalized;
        _wanderCenter = transform.position + velocity * wanderDistance;

        _wanderAngle += Random.Range(-wanderRange, wanderRange);
        Vector3 wanderPoint = _wanderCenter + Quaternion.Euler(0, _wanderAngle, 0) * Vector3.forward * wanderRadius;

        Vector3 desiredVelocity = wanderPoint - transform.position;
        Vector3 currentVelocity = _rb.linearVelocity;
        Vector3 seekVelocity = (desiredVelocity - currentVelocity) * steeringDamp;

        // Debug 1/2
        Debug.DrawRay(transform.position, _wanderCenter - transform.position, Color.green);
        Debug.DrawRay(_wanderCenter, wanderPoint - _wanderCenter, Color.green);
        // Debug 2/2
        Debug.DrawRay(transform.position, currentVelocity, Color.red * new Color(1, 1, 1, 0.75f));
        Debug.DrawRay(transform.position, desiredVelocity, Color.blue * new Color(1, 1, 1, 0.75f));
        Debug.DrawRay(transform.position + currentVelocity, seekVelocity, Color.yellow * new Color(1, 1, 1, 0.75f));

        return seekVelocity;

    }
}
