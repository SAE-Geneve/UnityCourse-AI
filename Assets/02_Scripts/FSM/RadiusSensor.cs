using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Serialization;

public class RadiusSensor : MonoBehaviour
{

    [SerializeField] private string tagField;

    private HashSet<Transform> _sendored = new HashSet<Transform>();
    
   private void OnDrawGizmos()
    {
        if (_sendored.Count > 0)
        {
            Gizmos.color = new Color(1.0f, 0.5f, 0.0f); // Unity uses float-based Color
            Gizmos.DrawCube(transform.position + new Vector3(0, 7.5f, 0), new Vector3(1,1,1));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagField))
        {
            _sendored.Add(other.transform);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagField))
        {
            _sendored.Remove(other.transform);
        }
    }
}
