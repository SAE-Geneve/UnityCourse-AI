using UnityEngine;

public class PatrolFollower : MonoBehaviour
{

    [SerializeField] private Patrol patrol;
    [SerializeField] private float patrolSpeed = 10;
    [SerializeField] private float pointDistance = 1;

    private Rigidbody _rb;

    private PatrolPoint _currentPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 distance = Vector3.zero;
        
        if (patrol.IsValid)
        {
            if (_currentPoint != null)
            {
                distance = _currentPoint.transform.position - transform.position;
            }
            
            if (distance.magnitude < pointDistance)
            {
                _currentPoint = patrol.GetNextPoint();
            }

            // -- Now move --
            _rb.rotation = Quaternion.LookRotation(distance);
            _rb.linearVelocity = distance.normalized * patrolSpeed;
            
        }

    }
}
