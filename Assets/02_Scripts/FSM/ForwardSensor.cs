using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Serialization;

public class ForwardSensor : MonoBehaviour
{

    [SerializeField] private float radius;
    [SerializeField] private float distance;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private string tagField;
    [SerializeField] private double targetMaxTime;

    [HideInInspector] public Transform target;

    private double _targetDuration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.SphereCast(transform.position, radius, transform.forward, out RaycastHit hit, distance, layerMask))
        {
            if (hit.collider.CompareTag(tagField))
            {
                target = hit.transform;
                _targetDuration = 0;
            }
        }

        if (_targetDuration > targetMaxTime)
            target = null;
        _targetDuration += Time.deltaTime;
        
    }

    private void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(target.position, radius);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
