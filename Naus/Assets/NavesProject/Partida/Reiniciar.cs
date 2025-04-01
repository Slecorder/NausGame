using UnityEngine;
using UnityEngine.SceneManagement;
public class Reiniciar : MonoBehaviour
{
    public void ReiniciarJoc()
    {
        Scene escenaActual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(escenaActual.buildIndex);
    }   
}
