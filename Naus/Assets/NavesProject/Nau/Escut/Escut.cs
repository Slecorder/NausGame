using UnityEngine;
using System.Collections;

public class Escut : MonoBehaviour
{
    [Header("Configuració de l'Escut")]
    [SerializeField] private float intensitatMaxima = 5f;
    [SerializeField] private float duracioParpelleig = 0.2f;
    [SerializeField] private int numParpelleigs = 3;
    [SerializeField] private Color colorBase = Color.cyan;
    [SerializeField] private int vidaMaxima = 3;
    [SerializeField] private float velocitatParpelleigFinal = 0.05f;
    [SerializeField] private int numParpelleigsFinal = 8;
    [SerializeField] private bool esEscutNauJugador = true; // True = nau del jugador, False = nau enemiga

    [Header("Audio")]
    [SerializeField] private AudioClip soImpacte;
    [SerializeField] private AudioClip soDestruccio;
    [SerializeField] private float volumSo = 1f;

    [Header("Rotació")]
    [SerializeField] private float velocitatRotacio = 30f;
    [SerializeField] private Vector3 direccioRotacio = new Vector3(0, 1, 0);

    [Header("Animació Textura")]
    [SerializeField] private float velocitatEscalatTextura = 0.5f;
    [SerializeField] private float tilingMinim = 1f;
    [SerializeField] private float tilingMaxim = 3f;
    private float tempsAnimacio = 0f;
    private bool estaParpellejant = false;
    private Color colorOriginal;
    private Renderer escutRenderer;
    private static readonly string tagAsteroide = "Asteroide";
    private static readonly string tagProjectilEnemic = "ProjectilEnemic";
    private static readonly string tagProjectilJugador = "Projectil";
    private int vidaActual;
    private bool partidaIniciada = false;

    private void Awake()
    {
        // Obtenir el renderer directament d'aquest objecte
        escutRenderer = GetComponent<Renderer>();

        // Guardar el color original d'emissió
        if (escutRenderer != null && escutRenderer.material.HasProperty("_EmissionColor"))
        {
            colorOriginal = escutRenderer.material.GetColor("_EmissionColor");
        }
        
        // Inicialitzar la vida de l'escut
        vidaActual = vidaMaxima;
        
        // Inicializar el tiling
        if (escutRenderer != null)
        {
            escutRenderer.material.mainTextureScale = new Vector2(tilingMinim, tilingMinim);
        }
    }
    
    private void Start()
    {
        GameObject botoInici = GameObject.FindGameObjectWithTag("StartButton");
        StartCoroutine(ComprovarBotoInici(botoInici));
    }
    
    private void Update()
    {
        // Rotar el escut constantment
        transform.Rotate(direccioRotacio * velocitatRotacio * Time.deltaTime);
        
        // Animacio tiling textura escut
        if (escutRenderer != null)
        {
            tempsAnimacio += Time.deltaTime;
            float factor = (Mathf.Sin(tempsAnimacio * velocitatEscalatTextura) + 1f) * 0.5f;
            float tilingActual = Mathf.Lerp(tilingMinim, tilingMaxim, factor);
            escutRenderer.material.mainTextureScale = new Vector2(tilingActual, tilingActual);
        }
    }
    
    private IEnumerator ComprovarBotoInici(GameObject boto)
    {
        while (boto != null && boto.activeSelf)
        {
            yield return new WaitForSeconds(0.1f);
        }
        partidaIniciada = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Només processar col·lisions si la partida ha començat
        if (!partidaIniciada)
        {
            return;
        }
        
        bool colisioValida = false;
        
        // Lògica diferent segons el tipus d'escut
        if (esEscutNauJugador)
        {
            // Escut de la nau del jugador: afectat per asteroides i projectils enemics
            colisioValida = (collision.gameObject.CompareTag(tagAsteroide) || 
                            collision.gameObject.CompareTag(tagProjectilEnemic)) && 
                            !estaParpellejant;
        }
        else
        {
            // Escut de la nau enemiga: només afectat per projectils del jugador
            colisioValida = collision.gameObject.CompareTag(tagProjectilJugador) && 
                            !estaParpellejant;
        }
        
        if (colisioValida)
        {
            vidaActual--;
            
            // Si s'ha quedat sense vida, fer parpelleig final ràpid
            if (vidaActual <= 0)
            {
                StartCoroutine(EfecteParpelleigFinal());
                ReproduirSo(soDestruccio);
            }
            else
            {
                // Si encara té vida, iniciar l'efecte de parpelleig normal
                StartCoroutine(EfecteParpelleig());
                ReproduirSo(soImpacte);
            }
            
            // Destruir el projectil al impactar (tant si és del jugador com enemic)
            if (collision.gameObject.CompareTag(tagProjectilEnemic) || 
                collision.gameObject.CompareTag(tagProjectilJugador))
            {
                Destroy(collision.gameObject);
            }
        }
    }

    private void ReproduirSo(AudioClip clip)
    {
        if (clip != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.ReproducirSonido(clip, volumSo, false, transform.position);
        }
    }

    private IEnumerator EfecteParpelleig()
    {
        // Evitar que s'iniciïn múltiples parpelleigs alhora
        estaParpellejant = true;

        // Verificar que el renderer i el material són vàlids
        if (escutRenderer != null && escutRenderer.material.HasProperty("_EmissionColor"))
        {
            // Calcular la intensitat basada en la vida restant
            float intensidadActual = intensitatMaxima * (1 + (vidaMaxima - vidaActual));
            
            // Realitzar diversos parpelleigs
            for (int i = 0; i < numParpelleigs; i++)
            {
                // Augmentar la intensitat (efecte HDR)
                Color colorIntens = colorBase * intensidadActual;
                escutRenderer.material.SetColor("_EmissionColor", colorIntens);
                escutRenderer.material.EnableKeyword("_EMISSION");

                // Esperar
                yield return new WaitForSeconds(duracioParpelleig / 2);

                // Tornar al color original
                escutRenderer.material.SetColor("_EmissionColor", colorOriginal);

                // Esperar abans del següent parpelleig
                yield return new WaitForSeconds(duracioParpelleig / 2);
            }
        }
        estaParpellejant = false;
    }
    
    private IEnumerator EfecteParpelleigFinal()
    {
        // Evitar que s'iniciïn múltiples parpelleigs alhora
        estaParpellejant = true;

        // Verificar que el renderer i el material són vàlids
        if (escutRenderer != null && escutRenderer.material.HasProperty("_EmissionColor"))
        {
            // Intensitat màxima per al parpelleig final
            float intensidadFinal = intensitatMaxima * 2;
            
            // Realitzar parpelleigs ràpids
            for (int i = 0; i < numParpelleigsFinal; i++)
            {
                // Augmentar la intensitat (efecte HDR)
                Color colorIntens = colorBase * intensidadFinal;
                escutRenderer.material.SetColor("_EmissionColor", colorIntens);
                escutRenderer.material.EnableKeyword("_EMISSION");

                // Esperar (més ràpid que el parpelleig normal)
                yield return new WaitForSeconds(velocitatParpelleigFinal / 2);

                // Tornar al color original
                escutRenderer.material.SetColor("_EmissionColor", colorOriginal);

                // Esperar abans del següent parpelleig (més ràpid)
                yield return new WaitForSeconds(velocitatParpelleigFinal / 2);
            }
        }

        // Desactivar l'escut després del parpelleig final
        gameObject.SetActive(false);
    }
    
    // Mètode públic per reactivar l'escut
    public void ReactivarEscut()
    {
        vidaActual = vidaMaxima;
        gameObject.SetActive(true);
        estaParpellejant = false;
    }
    
    // Mètode públic per obtenir la vida actual de l'escut
    public int GetVidaActual()
    {
        return vidaActual;
    }
}
