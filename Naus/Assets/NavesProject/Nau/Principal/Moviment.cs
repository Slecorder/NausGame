using UnityEngine;
using UnityEngine.UI;

public class Moviment : MonoBehaviour
{
    [Header("Configuració Bàsica")]
    [SerializeField] [Range(0.1f, 200f)] private float velocitat = 40f;
    [SerializeField] [Range(0.1f, 300f)] private float velocitatRotacio = 150f;
    [SerializeField] [Range(0f, 50f)] private float inclinacioLateral = 15f; // Graus d'inclinació lateral
    
    [Header("Configuració Inèrcia")]
    [SerializeField] [Range(0.1f, 100f)] private float acceleracio = 50f;
    [SerializeField] [Range(0.1f, 100f)] private float desacceleracio = 50f;
    [SerializeField] [Range(0.01f, 1f)] private float velocitatMinima = 0.1f;
    
    [Header("Límits")]
    [SerializeField] private float velocitatMaxima = 40f;
    [SerializeField] private float limitX = 45f; // Límite en el eje X (ancho del área de juego)
    [SerializeField] private float limitZ = 45f; // Límite en el eje Z (altura del área de juego)
    
    // Variables per al PowerUp de velocitat
    private float velocitatOriginal;
    private float velocitatAugmentada;
    private bool velocitatAugmentadaActiva = false;
    private float tempsVelocitatAugmentada = 0f;
    private float duracioVelocitatAugmentada = 0f;
    
    // Variables de moviment
    private float velocitatActual = 0f;
    private Vector3 direccioMoviment = Vector3.forward;
    private Rigidbody rb;
    private GameObject botoInici;
    private GameObject efecteVelocitat;
    
    void Awake()
    {
        velocitatOriginal = velocitat;
        velocitatActual = 0f;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        botoInici = GameObject.FindGameObjectWithTag("StartButton");
        efecteVelocitat = transform.Find("VelocityEffect").gameObject;
    }
    
    void Update()
    {
        // No fer res si el joc no ha començat
        if (botoInici != null && botoInici.activeSelf)
        {
            return;
        }
        
        // Controlar la duració del PowerUp de velocitat
        if (velocitatAugmentadaActiva)
        {
            tempsVelocitatAugmentada += Time.deltaTime;
            if (tempsVelocitatAugmentada >= duracioVelocitatAugmentada)
            {
                RestaurarVelocitat();
            }
        }
        
        // Obtenir input del jugador
        float inputRotacio = Input.GetAxis("Horizontal");
        float inputAcceleracio = Input.GetAxis("Vertical");
        
        // Aplicar rotació en Y (gir horitzontal)
        if (inputRotacio != 0)
        {
            transform.Rotate(0, inputRotacio * velocitatRotacio * Time.deltaTime, 0);
            float inclinacio = -inputRotacio * inclinacioLateral;
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, inclinacio);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        }
        
        // Aplicar acceleració/desacceleració amb inèrcia
        if (inputAcceleracio > 0)
        {
            velocitatActual = Mathf.MoveTowards(velocitatActual, velocitat * inputAcceleracio, acceleracio * Time.deltaTime);
        }
        else
        {
            velocitatActual = Mathf.MoveTowards(velocitatActual, 0, desacceleracio * Time.deltaTime);
        }
        
        // Limitar la velocitat màxima
        velocitatActual = Mathf.Clamp(velocitatActual, 0, velocitatMaxima);
        
        // Actualitzar la direcció de moviment basada en la rotació de la nau
        direccioMoviment = transform.forward;
    }
    
    void FixedUpdate()
    {
        // Aplicar moviment físic només si la velocitat és significativa
        if (velocitatActual > velocitatMinima)
        {
            // Calcular la nova posició
            Vector3 newPosition = transform.position + direccioMoviment * velocitatActual * Time.fixedDeltaTime;
            
            // Limitar la nova posició
            newPosition.x = Mathf.Clamp(newPosition.x, -limitX, limitX);
            newPosition.z = Mathf.Clamp(newPosition.z, -limitZ, limitZ);
            
            // Aplicar la nova posició
            transform.position = newPosition;
        }
        else
        {
            velocitatActual = 0;
        }
    }
    
    public void AugmentarVelocitat(float multiplicador, float duracio)
    {
        // Guardar la velocitat original si no està ja augmentada
        if (!velocitatAugmentadaActiva)
        {
            velocitatOriginal = velocitat;
        }
        
        // Calcular i aplicar la nova velocitat
        velocitatAugmentada = velocitatOriginal * multiplicador;
        velocitat = velocitatAugmentada;
        
        // Configurar la duració
        duracioVelocitatAugmentada = duracio;
        tempsVelocitatAugmentada = 0f;
        velocitatAugmentadaActiva = true;

        efecteVelocitat.SetActive(true);
        
        Debug.Log("Moviment: Velocitat augmentada a " + velocitat + " durant " + duracio + " segons");
    }
    
    // Mètode per restaurar la velocitat original
    public void RestaurarVelocitat()
    {
        velocitat = velocitatOriginal;
        velocitatAugmentadaActiva = false;
        
        efecteVelocitat.SetActive(false);
        
        Debug.Log("Moviment: Velocitat restaurada a " + velocitat);
    }
}
