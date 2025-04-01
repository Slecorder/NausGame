using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class Destruccio : MonoBehaviour
{
    [Header("Configuració General")]
    [SerializeField] private bool esNauJugador = true; // True = nau del jugador, False = nau enemiga
    
    [Header("Efectes Visuals")]
    [SerializeField] private GameObject efecteExplosio;
    [SerializeField] private float duracioExplosio = 2f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip soExplosio;
    [SerializeField] private float volumSo = 1f;
    
    private AudioSource fontAudio;
    private bool jocIniciat = false;
    private GameObject objecteNau;
    private List<Collider> collidersActius = new List<Collider>();
    private GameObject textGameOver;
    private GameObject botoReiniciar;
    private bool partidaFinalitzada = false;
    
    // Tags per a la nau del jugador
    private static readonly string tagNauJugador = "Nau";
    private static readonly string tagEscutJugador = "Escut";
    private static readonly string tagProjectilEnemic = "ProjectilEnemic";
    
    // Tags per a la nau enemiga
    private static readonly string tagNauEnemiga = "NauEnemiga";
    private static readonly string tagEscutEnemic = "EscutEnemic";
    private static readonly string tagProjectilJugador = "Projectil";
    
    // Tag comú
    private static readonly string tagAsteroide = "Asteroide";
    
    private void Awake()
    {
        fontAudio = GetComponent<AudioSource>();
        if (fontAudio == null)
        {
            fontAudio = gameObject.AddComponent<AudioSource>();
        }
        fontAudio.spatialBlend = 0f; 
        fontAudio.volume = volumSo;
        
        // Obtenir l'objecte de la nau segons el tipus
        if (esNauJugador)
        {
            objecteNau = GameObject.FindGameObjectWithTag(tagNauJugador);
        }
        else
        {
            objecteNau = GameObject.FindGameObjectWithTag(tagNauEnemiga);
        }
        
        // Cercar el text de GameOver i ocultar-lo al començament (només per a la nau del jugador)
        if (esNauJugador)
        {
            textGameOver = GameObject.FindGameObjectWithTag("GameOver");
            if (textGameOver != null)
            {
                textGameOver.SetActive(false);
            }
            
            // Cercar el botó de reinici i ocultar-lo al començament
            botoReiniciar = GameObject.FindGameObjectWithTag("Reiniciar");
            if (botoReiniciar != null)
            {
                botoReiniciar.SetActive(false);
            }
        }
    }
    
    private void Start()
    {
        StartCoroutine(ComprovarIniciJoc());
    }
    
    private IEnumerator ComprovarIniciJoc()
    {
        GameObject botoInici = GameObject.FindGameObjectWithTag("StartButton");
        if (botoInici != null)
        {
            yield return new WaitUntil(() => !botoInici.activeSelf);
        }

        jocIniciat = true;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!jocIniciat)
        {
            return;
        }
        
        // Comprovar si existeix un escut actiu
        string tagEscut = esNauJugador ? tagEscutJugador : tagEscutEnemic;
        GameObject escut = GameObject.FindGameObjectWithTag(tagEscut);
        if (escut != null && escut.activeSelf)
        {
            // Si hi ha escut actiu, no processar la col·lisió
            return;
        }
        
        bool colisioValida = false;
        
        // Lògica diferent segons el tipus de nau
        if (esNauJugador)
        {
            // Nau del jugador: pot ser destruïda per asteroides i projectils enemics
            colisioValida = collision.gameObject.CompareTag(tagAsteroide) || 
                           collision.gameObject.CompareTag(tagProjectilEnemic);
            
            // Si es un projectil enemic, destruir-lo
            if (collision.gameObject.CompareTag(tagProjectilEnemic))
            {
                Destroy(collision.gameObject);
            }
        }
        else
        {
            // Nau enemiga: només pot ser destruïda per projectils del jugador
            colisioValida = collision.gameObject.CompareTag(tagProjectilJugador);
            
            // Si es un projectil del jugador, destruir-lo
            if (collision.gameObject.CompareTag(tagProjectilJugador))
            {
                Destroy(collision.gameObject);
            }
        }
        
        // Si la col·lisió és vàlida, destruir la nau
        if (colisioValida)
        {
            DestruirNau();
        }
    }
    
    // Mètode per destruir la nau
    private void DestruirNau()
    {
        // Crear l'efecte d'explosió
        if (efecteExplosio != null)
        {
            GameObject explosio = Instantiate(efecteExplosio, objecteNau.transform.position, Quaternion.identity);
            Destroy(explosio, duracioExplosio);
        }
        
        // Reproduir so d'explosió de manera que sempre s'escolti
        if (soExplosio != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.ReproducirSonido(soExplosio, volumSo, false, objecteNau.transform.position);
        }
        
        // Només mostrar GameOver i botó de reinici si és la nau del jugador
        if (esNauJugador)
        {
            textGameOver.SetActive(true);
            botoReiniciar.SetActive(true);
            
            // Guardar la puntuació final amb el nom del jugador
            if (ScoreManager.Instance != null && !partidaFinalitzada)
            {
                partidaFinalitzada = true;
                ScoreManager.Instance.FinalitzarPartida();
            }
        }
        else
        {
            // Si és la nau enemiga, registrar la destrucció i sumar 100 punts
            Debug.Log("Nau enemiga destruïda!");
            ScoreManager.Instance.SumarPunts(100);
        }

        // Desactivar els colliders de la nau per evitar més col·lisions
        Collider[] colliders = objecteNau.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }
        Destroy(objecteNau);
    }
}
