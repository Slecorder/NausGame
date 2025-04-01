using UnityEngine;

public class MovimentEnemic : MonoBehaviour
{
    [SerializeField] [Range(5f, 30f)] private float velocitat = 10f;
    [SerializeField] private bool destruirAlSalir = true;
    
    private Vector3 direccio;
    private float radioCamp;
    private bool jocIniciat = false;
    
    // Método para inicializar la dirección de movimiento
    public void IniciarMoviment(Vector3 direccioInicial, float radio)
    {
        direccio = direccioInicial.normalized;
        radioCamp = radio;
        
        // Rotar la nave para que mire en la dirección del movimiento
        if (direccio != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direccio);
        }
    }
    
    void Start()
    {
        // Buscar el botón de inicio para saber cuándo comienza el juego
        GameObject botoInici = GameObject.FindGameObjectWithTag("StartButton");
        if (botoInici != null)
        {
            // Si existe el botón, esperar a que se desactive
            StartCoroutine(ComprovarBotoInici(botoInici));
        }
        else
        {
            // Si no hay botón, activar el movimiento inmediatamente
            jocIniciat = true;
        }
    }
    
    private System.Collections.IEnumerator ComprovarBotoInici(GameObject boto)
    {
        while (boto != null && boto.activeSelf)
        {
            yield return new WaitForSeconds(0.1f);
        }
        jocIniciat = true;
    }

    void Update()
    {
        // No moverse si el juego no ha comenzado
        if (!jocIniciat)
            return;
            
        // Mover la nave en la dirección establecida
        transform.position += direccio * velocitat * Time.deltaTime;
        
        // Comprobar si ha salido del campo
        if (destruirAlSalir && Vector3.Distance(Vector3.zero, transform.position) > radioCamp * 1.2f)
        {
            Debug.Log("Nau enemiga ha sortit del camp, destruint...");
            Destroy(gameObject);
        }
    }
    
    // Método para establecer la velocidad (útil para ajustar la dificultad)
    public void SetVelocitat(float novaVelocitat)
    {
        velocitat = novaVelocitat;
    }
}
