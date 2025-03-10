using UnityEngine;
using UnityEngine.InputSystem;

public class MolotovThrower : MonoBehaviour
{
    
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float throwForce;
    [SerializeField] private Molotov cocktail;
    
    public void OnShoot(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            Molotov c = Instantiate(cocktail, throwPoint.position, throwPoint.rotation);
            if (c.TryGetComponent(out Rigidbody rb))
            {
                rb.AddForce(throwPoint.transform.forward * throwForce, ForceMode.Impulse);
            }
        }
        
    }
}
