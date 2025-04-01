using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class PuntuacionsDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text textPuntuacions;
    [SerializeField] private string nomEscenaJoc = "SampleScene"; // Nom de l'escena principal del joc
    [SerializeField] private Button botoTornar;
    
    private string rutaArxiuXML;
    
    void Start()
    {
        // Definir la ruta de l'arxiu XML
#if UNITY_EDITOR
        // En modo editor, guardar en la carpeta del proyecto
        string projectPath = Application.dataPath;
        rutaArxiuXML = Path.Combine(projectPath, "NavesProject/Partida/puntuacions.xml");
#else
        // En build final, guardar en persistentDataPath
        rutaArxiuXML = Path.Combine(Application.persistentDataPath, "puntuacions.xml");
#endif
        
        // Configurar el botó de tornar
        if (botoTornar != null)
        {
            botoTornar.onClick.AddListener(TornarAlJoc);
        }
        
        // Carregar i mostrar les puntuacions
        MostrarPuntuacions();
    }
    
    // Mètode per carregar i mostrar les puntuacions
    private void MostrarPuntuacions()
    {
        if (textPuntuacions == null)
        {
            Debug.LogError("PuntuacionsDisplay: No s'ha assignat el component Text per mostrar les puntuacions");
            return;
        }
        
        List<PuntuacioJugador> puntuacions = CarregarPuntuacionsXML();
        
        if (puntuacions.Count == 0)
        {
            textPuntuacions.text = "No hi ha puntuacions guardades.";
            return;
        }
        
        // Crear el text amb les puntuacions
        string textPuntuacio = "MILLORS PUNTUACIONS\n\n";
        
        for (int i = 0; i < puntuacions.Count; i++)
        {
            PuntuacioJugador p = puntuacions[i];
            
            // Formatear la posición con padding para alinear
            string posicio = (i + 1).ToString().PadRight(3);
            
            // Formatear la puntuación con padding para alinear
            string punts = p.Puntuacio.ToString().PadLeft(6);
            
            // Añadir la fecha si está disponible
            string dataText = p.Data != DateTime.MinValue ? " - " + p.Data.ToString("dd/MM/yyyy HH:mm") : "";
            
            textPuntuacio += posicio + ". " + p.Nom + ": " + punts + " punts" + dataText + "\n";
        }
        
        textPuntuacions.text = textPuntuacio;
    }
    
    // Mètode per carregar les puntuacions des de l'arxiu XML
    private List<PuntuacioJugador> CarregarPuntuacionsXML()
    {
        List<PuntuacioJugador> puntuacions = new List<PuntuacioJugador>();
        
        // Comprovar si l'arxiu existeix
        if (File.Exists(rutaArxiuXML))
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(rutaArxiuXML);
                
                XmlNodeList nodesPuntuacio = doc.SelectNodes("//Puntuacio");
                
                foreach (XmlNode node in nodesPuntuacio)
                {
                    string nom = node.SelectSingleNode("Nom").InnerText;
                    int punts = int.Parse(node.SelectSingleNode("Punts").InnerText);
                    
                    // Intentar obtenir la data, si existeix
                    DateTime data = DateTime.MinValue;
                    XmlNode nodeData = node.SelectSingleNode("Data");
                    if (nodeData != null)
                    {
                        DateTime.TryParse(nodeData.InnerText, out data);
                    }
                    
                    puntuacions.Add(new PuntuacioJugador(nom, punts, data));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("PuntuacionsDisplay: Error al carregar les puntuacions: " + e.Message);
            }
        }
        
        return puntuacions;
    }
    
    // Mètode per tornar a l'escena principal del joc
    private void TornarAlJoc()
    {
        Debug.Log("PuntuacionsDisplay: Carregant escena de joc: " + nomEscenaJoc);
        SceneManager.LoadScene(nomEscenaJoc);
    }
}
