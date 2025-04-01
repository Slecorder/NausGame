using UnityEngine;

public class Projectil : MonoBehaviour
{
    [SerializeField] [Range(5000f, 25000f)] float force = 10000f;
    [SerializeField] [Range(1f, 10f)] float tiempoDeVida = 3f;
    
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void OnEnable()
    {
        rb.AddForce(transform.forward * force);
        Destroy(gameObject, tiempoDeVida);
    }
}
