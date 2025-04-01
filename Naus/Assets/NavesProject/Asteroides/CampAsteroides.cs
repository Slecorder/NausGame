using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CampAsteroides : MonoBehaviour
{
    [Header("Configuració Inicial")]
    [SerializeField] private List<GameObject> asteroidPrefab;
    [SerializeField] [Range(50f, 500f)] private float radio = 100f;
    [SerializeField] [Range(5, 1500)] private int cantitatAsteroides = 100;
    [SerializeField] [Range(0.2f, 10f)] private float tamanyMax = 5f;
    [SerializeField] [Range(0.2f, 10f)] private float tamanyMin = 1f;
    [SerializeField] [Range(10f, 50f)] private float zonaSeguretatNau = 20f;
    [SerializeField] [Range(100, 2000)] private int maximAsteroidesActius = 200;

    [Header("Configuració Spawn Continu")]
    [SerializeField] [Range(5f, 60f)] private float tempsInicialSpawn = 15f;
    [SerializeField] [Range(1f, 30f)] private float intervalSpawn = 3f;
    [SerializeField] [Range(1, 10)] private int asteroidsPerSpawn = 2;
    [SerializeField] [Range(0.5f, 10f)] private float velocitatAsteroides = 2f;
    
    [Header("Configuració Dificultat Progressiva")]
    [SerializeField] [Range(0f, 1f)] private float factorReduccioInterval = 0.05f;
    [SerializeField] [Range(0f, 1f)] private float factorAumentVelocitat = 0.1f;
    [SerializeField] [Range(30f, 300f)] private float intervalAumentDificultat = 60f;
    [SerializeField] [Range(0.5f, 10f)] private float intervalSpawnMinim = 1f;
    [SerializeField] [Range(0.5f, 50f)] private float velocitatMaxima = 5f;
    
    [Header("Configuració Power-Ups")]
    [SerializeField] private List<GameObject> powerUpPrefabs;
    [SerializeField] [Range(10f, 120f)] private float intervalPowerUp = 30f;
    [SerializeField] [Range(5f, 60f)] private float tempsInicialPowerUp = 20f;

    [Header("Configuració Naus Enemigues")]
    [SerializeField] private GameObject nauEnemigaPrefab;
    [SerializeField] [Range(10f, 120f)] private float intervalNauEnemiga = 25f;
    [SerializeField] [Range(5f, 60f)] private float tempsInicialNauEnemiga = 30f;

    private Transform trans;
    private Transform nau;
    private float tempsTranscorregut = 0f;
    private float tempsUltimSpawn = 0f;
    private float tempsUltimAumentDificultat = 0f;
    private bool spawnContinuActiu = false;
    private bool jocIniciat = false;
    private List<GameObject> asteroidsActius = new List<GameObject>();
    private float intervalSpawnActual;
    private float velocitatAsteroidsActual;
    private float tempsUltimPowerUp = 0f;
    private bool powerUpsActius = false;
    private float tempsUltimaNauEnemiga = 0f;
    private bool nausEnemiguesActives = false;

    private void Awake()
    {
        trans = transform;
        GameObject nauObj = GameObject.FindGameObjectWithTag("Nau");
        nau = nauObj.transform;
        
        InicialitzarValors();
    }
    
    private void Start()
    {
        StartCoroutine(ComprovarIniciJoc());
    }
    
    private IEnumerator ComprovarIniciJoc()
    {
        GameObject botoInici = GameObject.FindGameObjectWithTag("StartButton");
        
        // Si el botó existeix, esperar a que sigui deshabilitat
        if (botoInici != null)
        {
            yield return new WaitUntil(() => !botoInici.activeSelf);
        }
        
        // El joc ha començat
        jocIniciat = true;
        
        // Iniciar el comptador de temps per a l'aparició contínua
        tempsTranscorregut = 0f;
        tempsUltimSpawn = 0f;
        tempsUltimAumentDificultat = 0f;
    }
    
    private void OnEnable()
    {
        InicialitzarValors();
        Spawn();
    }
    
    private void InicialitzarValors()
    {
        tempsTranscorregut = 0f;
        tempsUltimSpawn = 0f;
        tempsUltimAumentDificultat = 0f;
        spawnContinuActiu = false;
        jocIniciat = false;
        intervalSpawnActual = intervalSpawn;
        velocitatAsteroidsActual = velocitatAsteroides;
        tempsUltimPowerUp = 0f;
        powerUpsActius = false;
        tempsUltimaNauEnemiga = 0f;
        nausEnemiguesActives = false;
    }

    private void Update()
    {
        // No fer res si el joc no ha començat
        if (!jocIniciat)
        {
            return;
        }
        
        // Actualitzar el temps transcorregut
        tempsTranscorregut += Time.deltaTime;
        
        // Comprovar si ja ha passat el temps inicial per a l'aparició contínua
        if (!spawnContinuActiu && tempsTranscorregut >= tempsInicialSpawn)
        {
            spawnContinuActiu = true;
            tempsUltimSpawn = tempsTranscorregut;
            tempsUltimAumentDificultat = tempsTranscorregut;
        }
        
        // Comprovar si ja ha passat el temps inicial per als power-ups
        if (!powerUpsActius && tempsTranscorregut >= tempsInicialPowerUp)
        {
            powerUpsActius = true;
            tempsUltimPowerUp = tempsTranscorregut;
        }
        
        // Comprovar si ja ha passat el temps inicial per a les naus enemigues
        if (!nausEnemiguesActives && tempsTranscorregut >= tempsInicialNauEnemiga)
        {
            nausEnemiguesActives = true;
            tempsUltimaNauEnemiga = tempsTranscorregut;
        }
        
        // Si l'aparició contínua està activa
        if (spawnContinuActiu)
        {
            // Augmentar la dificultat amb el temps
            if (tempsTranscorregut - tempsUltimAumentDificultat >= intervalAumentDificultat)
            {
                AumentarDificultat();
                tempsUltimAumentDificultat = tempsTranscorregut;
            }
            
            // Comprovar si és moment d'aparèixer nous asteroides
            if (tempsTranscorregut - tempsUltimSpawn >= intervalSpawnActual)
            {
                // Netejar la llista d'asteroides actius (eliminar referències nul·les)
                NetejarLlistaAsteroides();
                
                // Només fem aparèixer nous asteroides si no hem arribat al límit
                if (asteroidsActius.Count < maximAsteroidesActius)
                {
                    SpawnContinuAsteroides();
                    tempsUltimSpawn = tempsTranscorregut;
                }
            }
        }
        
        // Comprovar si és moment de generar un power-up d'escut
        if (powerUpsActius && tempsTranscorregut - tempsUltimPowerUp >= intervalPowerUp)
        {
            SpawnPowerUp();
            tempsUltimPowerUp = tempsTranscorregut;
        }
        
        // Comprovar si és moment de generar una nau enemiga
        if (nausEnemiguesActives && tempsTranscorregut - tempsUltimaNauEnemiga >= intervalNauEnemiga)
        {
            SpawnNauEnemiga();
            tempsUltimaNauEnemiga = tempsTranscorregut;
        }
    }
    
    // Mètode per augmentar la dificultat progressivament
    private void AumentarDificultat()
    {
        // Reduir l'interval d'aparició (més ràpid)
        intervalSpawnActual = Mathf.Max(intervalSpawnMinim, intervalSpawnActual * (1f - factorReduccioInterval));
        
        // Augmentar la velocitat dels asteroides (només per a nous asteroides)
        velocitatAsteroidsActual = Mathf.Min(velocitatMaxima, velocitatAsteroidsActual * (1f + factorAumentVelocitat));
        
        Debug.Log($"CampAsteroides: Dificultat augmentada - Interval: {intervalSpawnActual:F2}s, Velocitat (nous asteroides): {velocitatAsteroidsActual:F2}");
    }
    
    // Mètode per netejar la llista d'asteroides actius (eliminar referències nul·les)
    private void NetejarLlistaAsteroides()
    {
        for (int i = asteroidsActius.Count - 1; i >= 0; i--)
        {
            if (asteroidsActius[i] == null)
            {
                asteroidsActius.RemoveAt(i);
            }
        }
    }

    // Mètode per el spawn inicial de asteroides
    private void Spawn()
    {
        // Netejar la llista d'asteroides actius
        asteroidsActius.Clear();
        
        // Limitar la quantitat inicial d'asteroides al màxim permès
        int cantidadInicial = Mathf.Min(cantitatAsteroides, maximAsteroidesActius);
        
        for (int i = 0; i < cantidadInicial; i++)
        {
            // Generar posició aleatòria dins del radi
            Vector3 posicioAsteroide = GenerarPosicioAleatoria(false);
            
            // Comprovar si està massa a prop de la nau
            if (nau != null)
            {
                float distanciaANau = Vector3.Distance(new Vector3(nau.position.x, 0, nau.position.z), posicioAsteroide);
                
                // Si està massa a prop, generar una altra posició
                if (distanciaANau < zonaSeguretatNau)
                {
                    i--; // Decrementar i per tornar a intentar amb aquest asteroide
                    continue; // Saltar a la següent iteració
                }
            }
            
            // Crear l'asteroide
            CrearAsteroide(posicioAsteroide, false);
        }
    }
    
    // Mètode per el spawn continu de asteroides
    private void SpawnContinuAsteroides()
    {
        // Calcular quants asteroides podem fer aparèixer sense superar el límit
        int cantidadASpawnear = Mathf.Min(asteroidsPerSpawn, maximAsteroidesActius - asteroidsActius.Count);
        
        for (int i = 0; i < cantidadASpawnear; i++)
        {
            // Generar posició a la vora del camp
            Vector3 posicioAsteroide = GenerarPosicioAleatoria(true);
            
            // Crear l'asteroide amb moviment cap a la nau
            CrearAsteroide(posicioAsteroide, true);
        }
    }
    
    // Mètode per generar una posició aleatòria
    private Vector3 GenerarPosicioAleatoria(bool alBorde)
    {
        float angleRad = Random.Range(0, Mathf.PI * 2);
        float distance;
        
        if (alBorde)
        {
            distance = radio;
        }
        else
        {
            distance = Random.Range(0, radio * 0.7f);
        }
        
        float posX = Mathf.Cos(angleRad) * distance;
        float posZ = Mathf.Sin(angleRad) * distance;
        
        return new Vector3(
            trans.position.x + posX,
            0,
            trans.position.z + posZ
        );
    }
    
    // Mètode per crear un asteroide
    private GameObject CrearAsteroide(Vector3 posicio, bool ambMoviment)
    {
        // Instanciar l'asteroide
        GameObject asteroide = Instantiate(asteroidPrefab[Random.Range(0, asteroidPrefab.Count)], posicio, Quaternion.identity);
        
        // Escalar l'asteroide aleatòriament
        float escala = Random.Range(tamanyMin, tamanyMax);
        asteroide.transform.localScale = new Vector3(escala, escala, escala);
        
        // Afegir a la llista d'asteroides actius
        asteroidsActius.Add(asteroide);
        
        // Obtenir el component Asteroide
        Asteroide scriptAsteroide = asteroide.GetComponent<Asteroide>();
        
        // Si té el component, configurar-lo
        if (scriptAsteroide != null)
        {
            // Si s'ha de moure, configurar el moviment
            if (ambMoviment && nau != null)
            {
                // Utilitzar la posició actual de la nau com a objectiu
                Vector3 posicioNau = nau.position;
                scriptAsteroide.ConfigurarMoviment(posicioNau, velocitatAsteroidsActual);
            }
        }
        
        return asteroide;
    }

    private void SpawnPowerUp()
    {
        // Comprovar si hi ha prefabs assignats
        if (powerUpPrefabs == null || powerUpPrefabs.Count == 0)
        {
            Debug.LogWarning("CampAsteroides: No s'han assignat prefabs de power-ups");
            return;
        }
        
        // Generar una posició aleatòria dins del camp (no al borde)
        Vector3 posicioPowerUp = GenerarPosicioAleatoria(false);

        // Comprovar si està massa a prop de la nau
        if (nau != null)
        {
            float distanciaANau = Vector3.Distance(new Vector3(nau.position.x, 0, nau.position.z), posicioPowerUp);
            
            // Si està massa a prop, generar una altra posició
            if (distanciaANau < zonaSeguretatNau)
            {
                // Intentar una altra posició més lluny de la nau
                for (int i = 0; i < 5; i++)  // Intentar 5 vegades
                {
                    posicioPowerUp = GenerarPosicioAleatoria(false);
                    distanciaANau = Vector3.Distance(new Vector3(nau.position.x, 0, nau.position.z), posicioPowerUp);
                    
                    if (distanciaANau >= zonaSeguretatNau)
                    {
                        break;  // Posició vàlida trobada
                    }
                }
            }
        }
        
        // Seleccionar un power-up aleatorio de la lista
        int indexPowerUp = Random.Range(0, powerUpPrefabs.Count);
        GameObject powerUpPrefab = powerUpPrefabs[indexPowerUp];
        
        // Instanciar el power-up
        GameObject powerUp = Instantiate(powerUpPrefab, posicioPowerUp, Quaternion.identity);
     
        // Obtener el nombre del tipo de power-up para el mensaje de log
        string tipusPowerUp = powerUpPrefab.name;
        Debug.Log("CampAsteroides: Power-up " + tipusPowerUp + " generat a la posició " + posicioPowerUp);
    }

    // Mètode per generar una nau enemiga
    private void SpawnNauEnemiga()
    {
        // Si no hi ha prefab de nau enemiga, sortir
        if (nauEnemigaPrefab == null)
        {
            Debug.LogWarning("CampAsteroides: No s'ha assignat el prefab de nau enemiga");
            return;
        }
        
        // Determinar un punt aleatori al marge del camp
        Vector3 posicioSpawn = GenerarPosicioAleatoria(true); // true = al borde del campo
        
        // Calcular la direcció cap al punt oposat del camp
        Vector3 direccioAlCentre = new Vector3(trans.position.x, 0, trans.position.z) - posicioSpawn;
        Vector3 direccio = direccioAlCentre.normalized;
        
        // Instanciar la nau enemiga
        GameObject nauEnemiga = Instantiate(nauEnemigaPrefab, posicioSpawn, Quaternion.identity);
        
        // Configurar el moviment de la nau enemiga
        MovimentEnemic moviment = nauEnemiga.GetComponent<MovimentEnemic>();
        if (moviment != null)
        {
            moviment.IniciarMoviment(direccio, radio);
        }
        
        Debug.Log("CampAsteroides: Nau enemiga generada a la posició " + posicioSpawn);
    }
}
