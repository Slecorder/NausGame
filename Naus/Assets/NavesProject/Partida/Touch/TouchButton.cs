using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum ButtonType { Accelerar, Disparar, Dreta, Esquerra }
    public ButtonType buttonType;
    public Moviment moviment;
    public List<Arma> armes;

    public void OnPointerDown(PointerEventData eventData)
    {
        switch (buttonType)
        {
            case ButtonType.Accelerar:
                if (moviment != null) moviment.touchAccelerar = true;
                break;
            case ButtonType.Dreta:
                if (moviment != null) moviment.touchDreta = true;
                break;
            case ButtonType.Esquerra:
                if (moviment != null) moviment.touchEsquerra = true;
                break;
            case ButtonType.Disparar:
                if (armes != null)
                {
                    foreach (var arma in armes)
                    {
                        if (arma != null) arma.touchDisparar = true;
                    }
                }
                break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        switch (buttonType)
        {
            case ButtonType.Accelerar:
                if (moviment != null) moviment.touchAccelerar = false;
                break;
            case ButtonType.Dreta:
                if (moviment != null) moviment.touchDreta = false;
                break;
            case ButtonType.Esquerra:
                if (moviment != null) moviment.touchEsquerra = false;
                break;
            case ButtonType.Disparar:
                if (armes != null)
                {
                    foreach (var arma in armes)
                    {
                        if (arma != null) arma.touchDisparar = false;
                    }
                }
                break;
        }
    }
}
