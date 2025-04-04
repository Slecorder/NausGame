using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [Header("Configuración General")]
    [SerializeField] [Range(1, 20)] private int maxAudioSources = 10;
    [SerializeField] [Range(0f, 1f)] private float volumeGeneral = 1f;

    public static AudioManager Instance { get; private set; }
    
    // Getters i setters per al volum general
    public float GetVolumeGeneral()
    {
        return volumeGeneral;
    }
    
    public void SetVolumeGeneral(float nouVolum)
    {
        volumeGeneral = Mathf.Clamp01(nouVolum);
        ActualitzarVolumTotsSons();
    }
    
    private List<AudioSource> audioSourcePool = new List<AudioSource>();
    private int currentAudioSourceIndex = 0;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            for (int i = 0; i < maxAudioSources; i++)
            {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 0f;
                audioSourcePool.Add(audioSource);
            }
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    
    // Actualitza el volum de tots els AudioSources actius
    private void ActualitzarVolumTotsSons()
    {
        foreach (AudioSource source in audioSourcePool)
        {
            if (source.isPlaying)
            {
                // Mantenir el volum relatiu però actualitzar amb el nou volum general
                float volumRelatiu = source.volume / volumeGeneral;
                source.volume = volumRelatiu * volumeGeneral;
            }
        }
    }
    
    public void ReproducirSonido(AudioClip clip, float volume = 1f, bool espacial = false, Vector3 posicion = default)
    {
        if (clip == null || audioSourcePool.Count == 0)
            return;
            
        AudioSource audioSource = audioSourcePool[currentAudioSourceIndex];
        
        audioSource.clip = clip;
        audioSource.volume = volume * volumeGeneral;
        
        if (espacial)
        {
            audioSource.spatialBlend = 1f;
            audioSource.transform.position = posicion;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.minDistance = 5f;
            audioSource.maxDistance = 30f;
        }
        else
        {
            audioSource.spatialBlend = 0f;
        }
        
        audioSource.Play();
        
        currentAudioSourceIndex = (currentAudioSourceIndex + 1) % audioSourcePool.Count;
    }
    
    public void AjustarVolumenGeneral(float nuevoVolumen)
    {
        volumeGeneral = Mathf.Clamp01(nuevoVolumen);
        
        foreach (AudioSource audioSource in audioSourcePool)
        {
            if (audioSource.isPlaying)
            {
                audioSource.volume = audioSource.volume / volumeGeneral * volumeGeneral;
            }
        }
    }
    
    public void DetenerTodosSonidos()
    {
        foreach (AudioSource audioSource in audioSourcePool)
        {
            audioSource.Stop();
        }
    }
}
