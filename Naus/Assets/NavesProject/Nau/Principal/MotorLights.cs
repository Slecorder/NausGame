using UnityEngine;

public class MotorLights : MonoBehaviour
{
    [Header("Configuración de Luz")]
    [SerializeField] private Light motorLight;
    [SerializeField] private float intensidadMinima = 0.5f;
    [SerializeField] private float intensidadMaxima = 2.5f;
    [SerializeField] private float velocidadCambio = 2.0f;
    
    private Moviment controlNave;
    private float intensidadActual;

    void Start()
    {
        // Buscar el script de moviment
        controlNave = GetComponentInParent<Moviment>();
        
        intensidadActual = intensidadMinima;
        if (motorLight != null)
        {
            motorLight.intensity = intensidadActual;
        }
    }

    void Update()
    {
        if (motorLight == null || controlNave == null) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        // CAlcular magnitud del moviment
        float movimientoMagnitud = Mathf.Clamp01(new Vector2(horizontalInput, verticalInput).magnitude);
        
        // Calcular la intensitat basada en el moviment
        float intensidadObjetivo = Mathf.Lerp(intensidadMinima, intensidadMaxima, movimientoMagnitud);
        
        // Suavitzar el canvi de intensitat
        intensidadActual = Mathf.Lerp(intensidadActual, intensidadObjetivo, velocidadCambio * Time.deltaTime);
        
        // Aplicar la intensitat a la llum
        motorLight.intensity = intensidadActual;
    }
}
