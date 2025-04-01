using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PuntuacionsButton : MonoBehaviour
{
    [SerializeField] private string nomEscenaPuntuacions = "PuntuacioScene";
    
    private Button boto;
    
    void Start()
    {
        // Obtenir el component Button
        boto = GetComponent<Button>();
        
        // Afegir el listener per quan es faci clic
        if (boto != null)
        {
            boto.onClick.AddListener(CarregarEscenaPuntuacions);
        }
        else
        {
            Debug.LogError("PuntuacionsButton: No s'ha trobat el component Button");
        }
    }
    
    // Mètode per carregar l'escena de puntuacions
    public void CarregarEscenaPuntuacions()
    {
        Debug.Log("PuntuacionsButton: Carregant escena de puntuacions: " + nomEscenaPuntuacions);
        SceneManager.LoadScene(nomEscenaPuntuacions);
    }
}
