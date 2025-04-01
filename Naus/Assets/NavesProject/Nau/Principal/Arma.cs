using UnityEngine;

public class Arma : MonoBehaviour
{
    [Header("Configuración General")]
    [SerializeField] private GameObject proyectilPrefab;
    [SerializeField] private Transform proyectilSpawnPoint;
    [SerializeField] [Range(0f, 5f)]private float coolDown = 0.50f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip soDisparo;
    [SerializeField] private float volumSo = 1f;
    
    private float lastFireTime = 0f;
    private float coolDownOriginal;
    private float coolDownReducido;
    private bool coolDownReducidoActivo = false;
    private float tiempoCoolDownReducido = 0f;
    private float duracionCoolDownReducido = 0f;

    private void Awake()
    {
        // Inicializar valores
        coolDownOriginal = coolDown;
    }

    bool CanFire{
        get{
            lastFireTime -= Time.deltaTime;
            return lastFireTime <= 0f;
        }
    }


    void Update()
    {
        // Controlar la duración del cooldown reducido
        if (coolDownReducidoActivo)
        {
            tiempoCoolDownReducido += Time.deltaTime;
            if (tiempoCoolDownReducido >= duracionCoolDownReducido)
            {
                RestaurarCoolDown();
            }
        }
        
        if (Input.GetKey(KeyCode.Space) && CanFire)
        {
            DispararProjectil();
        }
    }

    private void DispararProjectil()
    {
        lastFireTime = coolDown;
        Instantiate(proyectilPrefab, proyectilSpawnPoint.position, proyectilSpawnPoint.rotation);
        ReproduirSo();
    }
    
    // Método para reproducir el sonido de disparo
    private void ReproduirSo()
    {
        if (soDisparo != null && AudioManager.Instance != null)
        {
            // Usar el AudioManager para reproducir el sonido
            AudioManager.Instance.ReproducirSonido(soDisparo, volumSo, false, transform.position);
        }
    }
    
    // Método para reducir el cooldown temporalmente
    public void ReducirCoolDown(float multiplicador, float duracion)
    {
        // Guardar el cooldown original si no está ya reducido
        if (!coolDownReducidoActivo)
        {
            coolDownOriginal = coolDown;
        }
        
        // Calcular el nuevo cooldown reducido (menor valor = disparo más rápido)
        coolDownReducido = coolDownOriginal * multiplicador;
        
        // Aplicar el nuevo cooldown
        coolDown = coolDownReducido;
        
        // Configurar la duración
        duracionCoolDownReducido = duracion;
        tiempoCoolDownReducido = 0f;
        coolDownReducidoActivo = true;
        
        Debug.Log("Arma: Cooldown reducido a " + coolDown + " durant " + duracion + " segons");
    }

    public void RestaurarCoolDown()
    {
        coolDown = coolDownOriginal;
        coolDownReducidoActivo = false;
        
        Debug.Log("Arma: Cooldown restaurado a " + coolDown);
    }
}
