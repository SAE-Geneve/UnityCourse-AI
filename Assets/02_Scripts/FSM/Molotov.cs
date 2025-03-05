using System;
using Unity.VisualScripting;
using UnityEngine;

public class Molotov : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private float lifeTime = 2f;
    
    private bool _isGrounded;
    private Rigidbody _rb;
    private float _timer = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isGrounded)
            _rb.angularVelocity = Vector3.forward * rotationSpeed;
        else
        {
            if (_rb.linearVelocity.magnitude < 0.75f)
            {
                Debug.Log("explode");
                impactEffect.SetActive(true);
            }
        }
        
        impactEffect.transform.rotation = Quaternion.identity;
        if (_timer != 0 && Time.time - _timer > lifeTime)
        {
            Destroy(gameObject);
        }
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Ground"))
        {
            _isGrounded = true;
            _timer = Time.time;
        }
    }
    private void OnCollisionExit(Collision other)
    {
        if(other.collider.CompareTag("Ground"))
        {
            _isGrounded = false;
            _timer = 0;
        }
    }
    
    
}
