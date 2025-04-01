using UnityEngine;

public class Asteroide : MonoBehaviour
{
    [Header("Configuració General")]
    [SerializeField] private int puntsDeVida = 1;
    [SerializeField] private int puntsPorDestruccio = 10;

    [Header("Efectes Visuals")]
    [SerializeField] private GameObject efecteExplosio;
    [SerializeField] private float duracioExplosio = 1f;

    [Header("Audio")]
    [SerializeField] private AudioClip soExplosio;
    [SerializeField] private float volumSo = 1f;

    private Vector3 direccioMoviment;
    private float velocitatBase;
    private bool moventCapAlCentre = false;

    private Vector3 direccioRotacio;
    private float velocitatRotacio;

    private Rigidbody rb;
    private GameObject botoInici;

    private void Awake()
    {
        botoInici = GameObject.FindGameObjectWithTag("StartButton");
        rb = GetComponent<Rigidbody>();
        ConfigurarRotacioAleatoria();
    }
    private void Update()
    {
        transform.Rotate(direccioRotacio * velocitatRotacio * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (moventCapAlCentre)
        {
            ActualitzarMoviment();
        }
    }

    public void ConfigurarMoviment(Vector3 puntObjectiu, float velocitat)
    {
        direccioMoviment = (puntObjectiu - transform.position).normalized;
        velocitatBase = velocitat;
        moventCapAlCentre = true;
        ActualitzarMoviment();
    }

    public void ActualitzarVelocitat(float novaVelocitat)
    {
        if (moventCapAlCentre)
        {
            velocitatBase = novaVelocitat;
            ActualitzarMoviment();
        }
    }

    private void ActualitzarMoviment()
    {
        if (rb == null || rb.isKinematic)
        {
            transform.position += direccioMoviment * velocitatBase * Time.fixedDeltaTime;
        }
        else
        {
            rb.linearVelocity = direccioMoviment * velocitatBase;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (botoInici != null && botoInici.activeSelf)
            return;

        if (collision.gameObject.CompareTag("Projectil"))
        {
            puntsDeVida--;
            Destroy(collision.gameObject);

            if (puntsDeVida <= 0)
            {
                DestruirAsteroide();
            }
        }
    }

    private void DestruirAsteroide()
    {
        if (efecteExplosio != null)
        {
            GameObject explosio = Instantiate(efecteExplosio, transform.position, Quaternion.identity);
            explosio.transform.localScale = transform.localScale * 0.5f;
            Destroy(explosio, duracioExplosio);
        }

        if (soExplosio != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.ReproducirSonido(soExplosio, volumSo, false, transform.position);
        }

        ScoreManager.Instance.SumarPunts(puntsPorDestruccio);
        Destroy(gameObject);
    }

    private void ConfigurarRotacioAleatoria()
    {
        direccioRotacio = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;

        velocitatRotacio = Random.Range(2f, 20f);
    }
}
