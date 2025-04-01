using UnityEngine;

public class PuntuacionsCameraAnimation : MonoBehaviour
{
    [SerializeField] private float velocitatRotacioY = 5f;
    
    void Update()
    {
        // Rotar la càmera sobre l'eix Y (vertical)
        transform.Rotate(0, velocitatRotacioY * Time.deltaTime, 0);
    }
}
