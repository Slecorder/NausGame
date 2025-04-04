using UnityEngine;
using UnityEngine.UI;

public class VolumController : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider sliderVolumEfectes;
    [SerializeField] private Slider sliderVolumMusica;
    
    [Header("Referències")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private MusicManager musicManager;
    
    // Variables per guardar els valors inicials
    private float volumEfectesInicial;
    private float volumMusicaInicial;
    
    private void Start()
    {
        // Inicialitzar referències si no estan assignades
        if (audioManager == null)
            audioManager = FindObjectOfType<AudioManager>();
            
        if (musicManager == null)
            musicManager = FindObjectOfType<MusicManager>();
        
        // Configurar els sliders
        ConfigurarSliders();
        
        // Afegir listeners als sliders
        if (sliderVolumEfectes != null)
            sliderVolumEfectes.onValueChanged.AddListener(CanviarVolumEfectes);
            
        if (sliderVolumMusica != null)
            sliderVolumMusica.onValueChanged.AddListener(CanviarVolumMusica);
    }
    
    private void ConfigurarSliders()
    {
        // Configurar slider d'efectes
        if (sliderVolumEfectes != null && audioManager != null)
        {
            volumEfectesInicial = audioManager.GetVolumeGeneral();
            sliderVolumEfectes.value = volumEfectesInicial;
        }
        
        // Configurar slider de música
        if (sliderVolumMusica != null && musicManager != null)
        {
            volumMusicaInicial = musicManager.GetVolumMusica();
            sliderVolumMusica.value = volumMusicaInicial;
        }
    }
    
    public void CanviarVolumEfectes(float nouVolum)
    {
        if (audioManager != null)
        {
            audioManager.SetVolumeGeneral(nouVolum);
        }
    }
    
    public void CanviarVolumMusica(float nouVolum)
    {
        if (musicManager != null)
        {
            musicManager.SetVolumMusica(nouVolum);
        }
    }
    
    // Mètode per restaurar els valors inicials (opcional)
    public void RestaurarValorsInicials()
    {
        if (sliderVolumEfectes != null && audioManager != null)
        {
            sliderVolumEfectes.value = volumEfectesInicial;
            audioManager.SetVolumeGeneral(volumEfectesInicial);
        }
        
        if (sliderVolumMusica != null && musicManager != null)
        {
            sliderVolumMusica.value = volumMusicaInicial;
            musicManager.SetVolumMusica(volumMusicaInicial);
        }
    }
}
