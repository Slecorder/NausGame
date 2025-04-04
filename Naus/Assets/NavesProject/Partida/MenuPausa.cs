using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [SerializeField] private GameObject panelPausa;
    [SerializeField] private KeyCode teclaPausa = KeyCode.Escape;
    
    private bool jocPausat = false;
    
    
    void Update()
    {
        if (Input.GetKeyDown(teclaPausa))
        {
            TogglePausa();
        }
    }
    
    public void TogglePausa()
    {
        jocPausat = !jocPausat;
        
        if (jocPausat)
        {
            PausarJoc();
        }
        else
        {
            ReprendreJoc();
        }
    }
    
    void PausarJoc()
    {
        panelPausa.SetActive(true);
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }
    
    public void ReprendreJoc()
    {
        panelPausa.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;
        jocPausat = false;
    }
    
    public void ReiniciarJoc()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Scene escenaActual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(escenaActual.name);
    }
    
    public void SortirJoc()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
