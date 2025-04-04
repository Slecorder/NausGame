using UnityEngine;
using UnityEngine.UI;

public class CameraInicial : MonoBehaviour
{
    [Header("Configuració de Rotació")]
    private readonly Vector3 puntCentral = Vector3.zero;
    [SerializeField] private float velocitatRotacio = 10f;
    [SerializeField] private float distancia = 15f;
    [SerializeField] private float alturaY = 5f;
    
    [Header("Configuració de Transició")]
    [SerializeField] private Camera cameraPrincipal;
    [SerializeField] private float duracioTransicio = 1.5f;
    private readonly AnimationCurve corbaTransicio = new AnimationCurve(
        new Keyframe(0, 0, 0, 0),
        new Keyframe(1, 1, 2, 0)
    );
    
    private float angleActual = 0f;
    private bool transicioIniciada = false;
    private float tempsTransicio = 0f;
    private GameObject botoInici;
    private GameObject botoSortir;
    
    // Variables per a la interpolació de càmeres
    private Vector3 posicioInicial;
    private Quaternion rotacioInicial;
    private Vector3 posicioFinal;
    private Quaternion rotacioFinal;
    
    private Camera cameraInicial;
    
    void Awake()
    {
        cameraInicial = GetComponent<Camera>();
        cameraPrincipal.gameObject.SetActive(false);
        cameraInicial.gameObject.SetActive(true);
    }
    
    void Start()
    {
        // Configurar botó d'inici
        botoInici = GameObject.FindGameObjectWithTag("StartButton");
        if (botoInici != null)
        {
            Button boto = botoInici.GetComponent<Button>();
            boto.onClick.AddListener(IniciarTransicio);
        }
        
        // Configurar botó de sortida
        botoSortir = GameObject.FindGameObjectWithTag("ExitButton");
        if (botoSortir != null)
        {
            Button botoExit = botoSortir.GetComponent<Button>();
            botoExit.onClick.AddListener(SortirDelJoc);
        }
    }

    void Update()
    {
        if (!transicioIniciada)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                IniciarTransicio();
            }
            RotarCamera();
        }
        else
        {
            RealitzarTransicio();
        }
    }
    
    private void RotarCamera()
    {
        // Incrementar l'angle
        angleActual += velocitatRotacio * Time.deltaTime;
        
        // Calcular la nova posició
        float x = puntCentral.x + distancia * Mathf.Cos(angleActual * Mathf.Deg2Rad);
        float z = puntCentral.z + distancia * Mathf.Sin(angleActual * Mathf.Deg2Rad);
        float y = puntCentral.y + alturaY;
        
        // Actualitzar la posició de la càmera
        transform.position = new Vector3(x, y, z);
        
        // Fer que la càmera miri cap al punt central
        transform.LookAt(puntCentral);
    }
    
    private void IniciarTransicio()
    {
        // Guardar la posició i rotació inicial
        posicioInicial = transform.position;
        rotacioInicial = transform.rotation;
        
        // Guardar la posició i rotació final (de la càmera principal)
        posicioFinal = cameraPrincipal.transform.position;
        rotacioFinal = cameraPrincipal.transform.rotation;
        
        transicioIniciada = true;
        tempsTransicio = 0f;
        
        // Deshabilitar el botó d'inici
        if (botoInici != null)
        {
            botoInici.SetActive(false);
        }
        
        // Deshabilitar el botó de sortida
        if (botoSortir != null)
        {
            botoSortir.SetActive(false);
        }
        
        // Deshabilitar el camp d'entrada del nom
        GameObject inputName = GameObject.FindGameObjectWithTag("InputName");
        if (inputName != null)
        {
            inputName.SetActive(false);
        }
        
        // Deshabilitar el botó de puntuacions
        GameObject puntuacionsButton = GameObject.FindGameObjectWithTag("PuntuacionsButton");
        if (puntuacionsButton != null)
        {
            puntuacionsButton.SetActive(false);
        }
        
        // Deshabilitar el panel de volum
        GameObject panelVolum = GameObject.Find("Volum");
        if (panelVolum != null)
        {
            panelVolum.SetActive(false);
        }
    }
    
    // Mètode per sortir del joc
    private void SortirDelJoc()
    {
        Debug.Log("Sortint del joc...");
        
        #if UNITY_EDITOR
            // Si s'està executant a l'editor, aturar el mode Play
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Si s'està executant com a aplicació compilada, tancar l'aplicació
            Application.Quit();
        #endif
    }
    
    private void RealitzarTransicio()
    {
        // Incrementar el temps de transició
        tempsTransicio += Time.deltaTime;
        
        // Calcular el progrés de la transició (0 a 1) amb la corba d'animació
        float t = Mathf.Clamp01(tempsTransicio / duracioTransicio);
        float progres = corbaTransicio.Evaluate(t);
        
        // Interpolar la posició i rotació de la càmera
        transform.position = Vector3.Lerp(posicioInicial, posicioFinal, progres);
        transform.rotation = Quaternion.Slerp(rotacioInicial, rotacioFinal, progres);
        
        if (t >= 1f)
        {
            cameraPrincipal.gameObject.SetActive(true);
            cameraInicial.gameObject.SetActive(false);
            enabled = false;
        }
    }
}
