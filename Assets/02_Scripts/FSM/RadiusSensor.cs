using System;
using System.Linq;
using UnityEngine;

public class RadiusSensor : MonoBehaviour
{

    [SerializeField] private float radius;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private string tagField;

    private Vector3 _center;
    
    public bool HasDetected { get; set; }
    public Vector3 Center => _center;

    private Transform[] _sendoredItems = Array.Empty<Transform>();

    private void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask);

        if (colliders.Length > 0)
        {
            _sendoredItems = colliders.Where(c => c.CompareTag(tagField)).Select(c => c.transform).ToArray();
            _center = RecalculateCenter();
        }
        else
        {
            _sendoredItems = Array.Empty<Transform>();
        }
        
        HasDetected = _sendoredItems.Length > 0;
        
    }
    private void OnDrawGizmos()
    {
        if (_sendoredItems.Length > 0)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.position + new Vector3(0, 7.5f, 0), new Vector3(1, 1, 1));
        }
        else
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private Vector3 RecalculateCenter()
    {
        Vector3 center = Vector3.zero;

        if (_sendoredItems.Length > 0)
        {
            foreach (Transform sendoredItem in _sendoredItems)
            {
                if (sendoredItem) center += sendoredItem.position;
            }

            center /= _sendoredItems.Length;
        }

        return center;

    }
}
