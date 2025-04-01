using UnityEngine;
using System.Collections;

public class PowerUpEscut : MonoBehaviour
{
    private Vector3 posicioInicial;
    private Vector3 escalaInicial;
    
    [Header("Configuració Rotació")]
    [SerializeField] [Range(5f, 100f)] private float velocitatRotacio = 30f;
    [SerializeField] private Vector3 direccioRotacio = new Vector3(1, 0, 0);
    [SerializeField] [Range(5f, 60f)] private float tempsDeVida = 15f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip soRecoleccio;
    [SerializeField] private float volumSo = 0.7f;
    
    private float tempsTranscorregut = 0f;
    
    
    void Update()
    {
        // Rotar el power-up sobre sí mismo
        transform.Rotate(direccioRotacio * velocitatRotacio * Time.deltaTime);
        tempsTranscorregut += Time.deltaTime;
        if (tempsTranscorregut >= tempsDeVida)
        {
            // Destruir el power-up cuando se agote su tiempo de vida
            Debug.Log("PowerUpEscut: Temps de vida esgotat, destruint power-up");
            Destroy(gameObject);
        }
    }

    // Método para reproducir el sonido de recolección
    private void ReproduirSo()
    {
        if (soRecoleccio != null)
        {
            // Crear un GameObject temporal para reproducir el sonido
            GameObject tempAudio = new GameObject("TempAudioPowerUpEscut");
            
            // Añadir un AudioSource
            AudioSource audioTemp = tempAudio.AddComponent<AudioSource>();
            audioTemp.clip = soRecoleccio;
            audioTemp.volume = volumSo;
            audioTemp.spatialBlend = 0f; // Sonido 2D (sin atenuación espacial)
            audioTemp.Play();
            
            // Destruir el objeto temporal después de reproducir el sonido
            Destroy(tempAudio, soRecoleccio.length + 0.1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ColliderNau"))
        {
            Escut escut = other.transform.parent.GetComponentInChildren<Escut>(true);
            
            if (escut != null)
            {
                // Reproducir sonido de recolección
                ReproduirSo();
                
                escut.ReactivarEscut();
                Destroy(gameObject);
                Debug.Log("PowerUpEscut: Escut reactivat correctament");
            }
            else
            {
                Debug.LogWarning("PowerUpEscut: No s'ha trobat el component Escut en la nau");
            }
        }
    }
}
