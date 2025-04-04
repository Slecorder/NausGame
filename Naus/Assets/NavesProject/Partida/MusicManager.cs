using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Configuració de Música")]
    [SerializeField] private AudioClip musicaFons;
    [SerializeField] [Range(0f, 1f)] private float volumMusica = 0.5f;
    [SerializeField] private bool reproduirAlIniciar = true;
    [SerializeField] private bool repetirMusica = true;
    
    // Getters i setters per al volum de la música
    public float GetVolumMusica()
    {
        return volumMusica;
    }
    
    public void SetVolumMusica(float nouVolum)
    {
        volumMusica = Mathf.Clamp01(nouVolum);
        if (audioSource != null)
        {
            audioSource.volume = volumMusica;
        }
    }
    
    private AudioSource audioSource;
    
    private void Awake()
    {
        ConfigurarAudioSource();
    }
    
    private void ConfigurarAudioSource()
    {
        // Afegir un AudioSource si no existeix
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Configurar el AudioSource
        audioSource.clip = musicaFons;
        audioSource.volume = volumMusica;
        audioSource.loop = repetirMusica;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // Música 2D
        
        // Iniciar la reproducció si està configurat
        if (reproduirAlIniciar && musicaFons != null)
        {
            audioSource.Play();
        }
    }
}
