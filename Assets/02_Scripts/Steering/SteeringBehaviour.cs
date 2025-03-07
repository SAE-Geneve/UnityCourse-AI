using UnityEngine;
using UnityEngine.Serialization;

public class SteeringBehaviour : MonoBehaviour
{
    
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float steeringDamp = 0.5f;
    
    [Header("Seek")]
    [SerializeField] private float seekFactor = 1f;
    
    [Header("Flee")]
    [SerializeField] private float fleeFactor = 1f;
    
    [Header("Arrival")]
    [SerializeField] private float arrivalRadius = 10f;
    
    [Header("Avoidance")]
    [SerializeField] [Range(0, 1)] private float avoidanceFactor = 1f;
    [SerializeField] private LayerMask avoidanceMask;
    [SerializeField] private float avoidanceDistance = 10;
    [SerializeField] private float avoidanceForce;
    [SerializeField] private float avoidanceRadius;
    
    private Vector3 _avoidPoint;
    private Vector3 _avoidHitPoint;
    
    private Rigidbody _rb;
    private GameObject _player;
    private GameObject _preedator;

    public bool playerHasGun = false;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _preedator = GameObject.FindGameObjectWithTag("Preedator");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 steeringResult = Vector3.zero;

        if (playerHasGun)
        {
            steeringResult += 2f * Flee(_player.transform.position);
        }
        else
        {
            steeringResult += seekFactor * Seek(_player.transform.position);
        }
        
        if(_preedator != null)
            steeringResult += fleeFactor * Flee(_preedator.transform.position);
        
        steeringResult += avoidanceFactor * ObstacleAvoidance();
        
        _rb.linearVelocity = steeringResult;
        
        // Max speed
        if(_rb.linearVelocity.magnitude > maxSpeed)
            _rb.linearVelocity = _rb.linearVelocity.normalized * maxSpeed;
        
    }

    private void OnDrawGizmos()
    {
        if (_rb != null && avoidanceFactor > 0)
        {
            //Gizmos.DrawWireSphere(transform.position, _avoidanceRadius);
            if (Vector3.Distance(_avoidPoint, transform.position) > Mathf.Epsilon && Vector3.Distance(_avoidHitPoint, transform.position) > Mathf.Epsilon)
            {
                Gizmos.DrawWireSphere(_avoidPoint, avoidanceRadius);
                Gizmos.DrawWireSphere(_avoidHitPoint, avoidanceRadius);
            }
        }


    }
    
    Vector3 Seek(Vector3 targetPosition)
    {
        
        Vector3 steeringForce = Vector3.zero;
        
        Vector3 actualVelocity = _rb.linearVelocity;
        Vector3 desiredVelocity = (targetPosition - transform.position).normalized * maxSpeed;
        
        steeringForce = (desiredVelocity - actualVelocity) * steeringDamp;
        
        // Debug draws
        Debug.DrawRay(transform.position, desiredVelocity, Color.magenta);
        Debug.DrawRay(transform.position, actualVelocity, Color.red);
        Debug.DrawRay(transform.position + actualVelocity, steeringForce, Color.green);
        
        return steeringForce;

    }
    
    Vector3 Flee(Vector3 targetPosition)
    {
        
        Vector3 steeringForce = Vector3.zero;
        
        Vector3 actualVelocity = _rb.linearVelocity;
        Vector3 desiredVelocity = (transform.position - targetPosition).normalized * maxSpeed;
        
        steeringForce = (desiredVelocity - actualVelocity) * steeringDamp;
        
        // Debug draws
        Debug.DrawRay(transform.position, desiredVelocity, Color.magenta);
        Debug.DrawRay(transform.position, actualVelocity, Color.red);
        Debug.DrawRay(transform.position + actualVelocity, steeringForce, Color.green);
        
        return steeringForce;

    }
    
    Vector3 SeekAndArrival(Vector3 targetPosition)
    {
        
        Vector3 steeringForce = Vector3.zero;
        
        Vector3 actualVelocity = _rb.linearVelocity;
        
        float actualSpeed = Mathf.Lerp(0, maxSpeed, Vector3.Distance(targetPosition, transform.position) / arrivalRadius);
        Vector3 desiredVelocity = (targetPosition - transform.position).normalized * actualSpeed;
        
        steeringForce = (desiredVelocity - actualVelocity) * steeringDamp;
        
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
            Vector3 steeringForce = desiredVelocity - actualVelocity;


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
}
