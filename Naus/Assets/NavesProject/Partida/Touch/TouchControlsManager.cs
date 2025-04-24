using UnityEngine;
using UnityEngine.UI;

public class TouchControlsManager : MonoBehaviour
{
    public GameObject[] touchButtons; // Asigna aquí los 4 botones desde el inspector
    private GameObject botoInici;

    void Start()
    {
        botoInici = GameObject.FindGameObjectWithTag("StartButton");
        SetButtonsActive(false);
    }

    void Update()
    {
        #if UNITY_ANDROID
        bool partidaIniciada = (botoInici == null || !botoInici.activeSelf);
        SetButtonsActive(partidaIniciada);
        #else
        SetButtonsActive(false);
        #endif
    }

    void SetButtonsActive(bool active)
    {
        foreach (var btn in touchButtons)
        {
            if (btn != null) btn.SetActive(active);
        }
    }
}
