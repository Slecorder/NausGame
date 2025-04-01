using UnityEngine;

public class ArmaEnemiga : MonoBehaviour
{
    [Header("Configuración General")]
    [SerializeField] private GameObject proyectilPrefab;
    [SerializeField] private Transform proyectilSpawnPoint;
    [SerializeField] [Range(0.1f, 5f)] private float coolDown = 1.5f;
    [SerializeField] private bool disparoActivo = true;
    [SerializeField] [Range(0f, 10f)] private float retrasoInicialDisparo = 0f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip soDisparo;
    [SerializeField] private float volumSo = 0.6f;
    
    private float lastFireTime = 0f;
    private float tempsTranscorregut = 0f;
    private Renderer rendererNau; 

    void Start()
    {
        // Inicializar el tiempo de disparo con el cooldown para que no dispare inmediatamente
        lastFireTime = coolDown;
        
        // Obtener el renderer principal de la nave enemiga
        rendererNau = GetComponent<Renderer>();
    }


    bool CanFire
    {
        get
        {
            lastFireTime -= Time.deltaTime;
            return lastFireTime <= 0f;
        }
    }

    void Update()
    {
        // Controlar el retraso inicial
        if (retrasoInicialDisparo > 0)
        {
            tempsTranscorregut += Time.deltaTime;
            if (tempsTranscorregut < retrasoInicialDisparo)
                return;
        }
        
        // Disparar automáticamente si está activo, puede disparar y es visible
        if (disparoActivo && CanFire && (rendererNau == null || rendererNau.isVisible))
        {
            DispararProjectil();
        }
    }

    private void DispararProjectil()
    {
        lastFireTime = coolDown;
        Instantiate(proyectilPrefab, proyectilSpawnPoint.position, proyectilSpawnPoint.rotation);
        
        // Reproducir sonido de disparo
        ReproduirSo();
    }
    
    private void ReproduirSo()
    {
        if (soDisparo != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.ReproducirSonido(soDisparo, volumSo, false, transform.position);
        }
    }
    
    // Método para activar o desactivar el disparo
    public void SetDisparoActivo(bool activo)
    {
        disparoActivo = activo;
    }
    
    // Método para modificar el cooldown (para posibles power-ups o dificultad)
    public void ModificarCoolDown(float nuevoValor)
    {
        coolDown = nuevoValor;
    }
}
