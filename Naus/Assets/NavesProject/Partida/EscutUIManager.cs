using UnityEngine;
using System.Collections.Generic;

public class EscutUIManager : MonoBehaviour
{
    private List<GameObject> imatgesEscutUI = new List<GameObject>();
    private Escut escutScript;
    private int vidaActualEscut = 3;
    private int vidaAnteriorEscut = 3;
    
    void Start()
    {
        GameObject[] objectesEscutUI = GameObject.FindGameObjectsWithTag("EscutUI");
        
        // Ordenar les imatges per nom (assumint que tenen noms com EscutVida1, EscutVida2, EscutVida3)
        System.Array.Sort(objectesEscutUI, (a, b) => a.name.CompareTo(b.name));
        imatgesEscutUI.AddRange(objectesEscutUI);
        
        // Buscar l'escut del jugador
        GameObject escut = GameObject.FindGameObjectWithTag("Escut");
        if (escut != null)
        {
            escutScript = escut.GetComponent<Escut>();
        }
    }
    
    void Update()
    {
        vidaActualEscut = escutScript.GetVidaActual();

        if (vidaActualEscut != vidaAnteriorEscut)
        {
            ActualitzarImatgesEscut();
            vidaAnteriorEscut = vidaActualEscut;
        }
    }
    
    private void ActualitzarImatgesEscut()
    {
        for (int i = 0; i < imatgesEscutUI.Count; i++)
        {
            if (i < vidaActualEscut)
            {
                imatgesEscutUI[i].SetActive(true);
            }
            else
            {
                imatgesEscutUI[i].SetActive(false);
            }
        }
    }
}
