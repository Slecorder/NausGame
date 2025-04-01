using UnityEngine;
using System.Collections;

public class PowerUpDisparo : MonoBehaviour
{    
    [Header("Configuració Rotació")]
    [SerializeField] [Range(5f, 100f)] private float velocitatRotacio = 30f;
    [SerializeField] private Vector3 direccioRotacio = new Vector3(1, 0, 0);
    [SerializeField] [Range(5f, 60f)] private float tempsDeVida = 15f;    
    
    [Header("Configuració Disparo")]
    [SerializeField] [Range(0.1f, 0.9f)] private float multiplicadorCoolDown = 0.5f;
    [SerializeField] [Range(3f, 20f)] private float duracioBoost = 8f;

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
            Debug.Log("PowerUpDisparo: Temps de vida esgotat, destruint power-up");
            Destroy(gameObject);
        }
    }

    // Método para reproducir el sonido de recolección
    private void ReproduirSo()
    {
        if (soRecoleccio != null)
        {
            // Crear un GameObject temporal para reproducir el sonido
            GameObject tempAudio = new GameObject("TempAudioPowerUpDisparo");
            
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
            // Buscar TODAS las componentes Arma en la nave y sus hijos
            Arma[] armas = other.transform.parent.GetComponentsInChildren<Arma>(true);
            
            if (armas != null && armas.Length > 0)
            {
                // Reproducir sonido de recolección
                ReproduirSo();
                
                // Aplicar el power-up a todas las armas encontradas
                foreach (Arma arma in armas)
                {
                    arma.ReducirCoolDown(multiplicadorCoolDown, duracioBoost);
                }
                
                // Destruir el power-up
                Destroy(gameObject);
                
                Debug.Log("PowerUpDisparo: Velocitat de disparo augmentada en " + armas.Length + " armas (cooldown x" + multiplicadorCoolDown + ") durant " + duracioBoost + " segons");
            }
            else
            {
                Debug.LogWarning("PowerUpDisparo: No s'ha trobat cap component Arma en la nau");
            }
        }
    }
}
