using UnityEngine;
using UnityEngine.Serialization;

public class SteeringBehaviour : MonoBehaviour
{
    
    private Rigidbody _rb;
    private GameObject _player;
    private GameObject _preedator;

    public bool playerHasGun = false;
    
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float steeringDamp = 0.5f;
    
    [Header("Seek")]
    [SerializeField] private float seekFactor = 1f;
    
    [Header("Flee")]
    [SerializeField] private float fleeFactor = 1f;
    
    [Header("Arrival")]
    [SerializeField] private float arrivalRadius = 10f;
    
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
        
        steeringResult += fleeFactor * Flee(_preedator.transform.position);
        
        //steeringResult += SeekAndArrival();
        
        _rb.linearVelocity = steeringResult;
        
        // Max speed
        if(_rb.linearVelocity.magnitude > maxSpeed)
            _rb.linearVelocity = _rb.linearVelocity.normalized * maxSpeed;
        
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
}
