using UnityEngine;
using System.Collections;

public class PowerUpVelocitat : MonoBehaviour
{    
    [Header("Configuració Rotació")]
    [SerializeField] [Range(5f, 100f)] private float velocitatRotacio = 30f;
    [SerializeField] private Vector3 direccioRotacio = new Vector3(1, 0, 0);
    [SerializeField] [Range(5f, 60f)] private float tempsDeVida = 15f;
    
    [Header("Configuració Boost")]
    [SerializeField] [Range(1.2f, 3f)] private float multiplicadorVelocitat = 1.5f;
    [SerializeField] [Range(3f, 20f)] private float duracioBoost = 5f;

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
            Debug.Log("PowerUpVelocitat: Temps de vida esgotat, destruint power-up");
            Destroy(gameObject);
        }
    }

    // Método para reproducir el sonido de recolección
    private void ReproduirSo()
    {
        if (soRecoleccio != null)
        {
            // Crear un GameObject temporal para reproducir el sonido
            GameObject tempAudio = new GameObject("TempAudioPowerUpVelocitat");
            
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
            // Buscar el componente Moviment en la nave o sus padres
            Moviment moviment = other.transform.parent.GetComponentInChildren<Moviment>(true);
            
            if (moviment != null)
            {
                // Reproducir sonido de recolección
                ReproduirSo();
                
                // Aumentar la velocidad de la nave
                moviment.AugmentarVelocitat(multiplicadorVelocitat, duracioBoost);
                
                // Destruir el power-up
                Destroy(gameObject);
                
                Debug.Log("PowerUpVelocitat: Velocitat augmentada x" + multiplicadorVelocitat + " durant " + duracioBoost + " segons");
            }
            else
            {
                Debug.LogWarning("PowerUpVelocitat: No s'ha trobat el component Moviment en la nau");
            }
        }
    }
}
