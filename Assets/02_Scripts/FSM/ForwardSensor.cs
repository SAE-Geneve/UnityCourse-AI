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

    [HideInInspector] public Transform Target;

    private double _targetDuration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Transform runtimeTarget = null;
        if (Physics.SphereCast(transform.position, radius, transform.forward, out RaycastHit hit, distance, layerMask))
        {
            if (hit.collider.CompareTag(tagField))
            {
                runtimeTarget = hit.transform;
            }
        }

        if (runtimeTarget)
        {
            Target = runtimeTarget;
            _targetDuration = 0;
        }
        else if(Target)
        {
            _targetDuration += Time.deltaTime;
            if (_targetDuration > targetMaxTime)
                Target = null;
        }

    }

    private void OnDrawGizmos()
    {
        if (Target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Target.position, radius);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
