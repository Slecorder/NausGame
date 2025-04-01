using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text textPuntuacio;
    public TMP_Text textRecord;

    private int puntuacio = 0;
    private int record = 0;
    private string nomJugador = "Jugador";
    private string rutaArxiuXML;
    
    // Singleton per accedir des d'altres scripts
    public static ScoreManager Instance { get; private set; }
    
    private void Awake()
    {
        // Configurar singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        // Definir la ruta de l'arxiu XML
#if UNITY_EDITOR
        // En modo editor, guardar en la carpeta del proyecto
        string projectPath = Application.dataPath;
        rutaArxiuXML = Path.Combine(projectPath, "NavesProject/Partida/puntuacions.xml");
#else
        // En build final, guardar en persistentDataPath
        rutaArxiuXML = Path.Combine(Application.persistentDataPath, "puntuacions.xml");
#endif
        
        // Carregar el rècord des de l'arxiu XML
        record = ObtindreMillorPuntuacio();
    }

    void Start()
    {
        // Buscar el camp d'entrada del nom
        GameObject inputNameObj = GameObject.FindGameObjectWithTag("InputName");
        if (inputNameObj != null)
        {
            TMP_InputField inputField = inputNameObj.GetComponent<TMP_InputField>();
            if (inputField != null)
            {
                // Afegir listener per quan canviï el text
                inputField.onValueChanged.AddListener(ActualitzarNomJugador);
            }
        }
        
        ActualitzarUI();
    }
    
    // Mètode per actualitzar el nom del jugador
    private void ActualitzarNomJugador(string nouNom)
    {
        if (!string.IsNullOrEmpty(nouNom))
        {
            nomJugador = nouNom;
        }
    }
    
    // Mètode públic per sumar punts quan es destrueix un asteroide
    public void SumarPunts(int punts)
    {
        puntuacio += punts;
        if (puntuacio > record)
        {
            record = puntuacio;
        }
        ActualitzarUI();
    }
    
    // Mètode per actualitzar la interfície d'usuari
    private void ActualitzarUI()
    {
        if (textPuntuacio != null)
        {
            textPuntuacio.text = "Puntuació: " + puntuacio;
        }
        
        if (textRecord != null)
        {
            textRecord.text = "Rècord: " + record;
        }
    }
    
    // Mètode per reiniciar la puntuació
    public void ReiniciarPuntuacio()
    {
        puntuacio = 0;
        ActualitzarUI();
    }
    
    // Mètode per guardar la puntuació en XML
    public void GuardarPuntuacioXML()
    {
        // Crear una llista de puntuacions
        List<PuntuacioJugador> puntuacions = CarregarPuntuacionsXML();
        
        // Afegir la puntuació actual amb la data actual
        puntuacions.Add(new PuntuacioJugador(nomJugador, puntuacio, DateTime.Now));
        
        // Ordenar les puntuacions de major a menor
        puntuacions.Sort((a, b) => b.Puntuacio.CompareTo(a.Puntuacio));
        
        // Limitar la llista a les 10 millors puntuacions
        if (puntuacions.Count > 10)
        {
            puntuacions = puntuacions.GetRange(0, 10);
        }
        
        // Guardar les puntuacions en XML
        GuardarPuntuacionsXML(puntuacions);
        
        // Actualitzar el rècord si és necessari
        if (puntuacions.Count > 0 && puntuacions[0].Puntuacio > record)
        {
            record = puntuacions[0].Puntuacio;
            ActualitzarUI();
        }
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
                Debug.LogError("Error al carregar les puntuacions: " + e.Message);
            }
        }
        
        return puntuacions;
    }
    
    // Mètode per guardar les puntuacions en l'arxiu XML
    private void GuardarPuntuacionsXML(List<PuntuacioJugador> puntuacions)
    {
        try
        {
            // Assegurar que el directori existeix
            string directori = Path.GetDirectoryName(rutaArxiuXML);
            if (!Directory.Exists(directori))
            {
                Directory.CreateDirectory(directori);
            }
            
            XmlDocument doc = new XmlDocument();
            
            // Crear l'element arrel
            XmlElement root = doc.CreateElement("Puntuacions");
            doc.AppendChild(root);
            
            // Afegir cada puntuació
            foreach (PuntuacioJugador p in puntuacions)
            {
                XmlElement nodePuntuacio = doc.CreateElement("Puntuacio");
                
                XmlElement nodeNom = doc.CreateElement("Nom");
                nodeNom.InnerText = p.Nom;
                nodePuntuacio.AppendChild(nodeNom);
                
                XmlElement nodePunts = doc.CreateElement("Punts");
                nodePunts.InnerText = p.Puntuacio.ToString();
                nodePuntuacio.AppendChild(nodePunts);
                
                // Afegir la data
                if (p.Data != DateTime.MinValue)
                {
                    XmlElement nodeData = doc.CreateElement("Data");
                    nodeData.InnerText = p.Data.ToString("yyyy-MM-dd HH:mm:ss");
                    nodePuntuacio.AppendChild(nodeData);
                }
                
                root.AppendChild(nodePuntuacio);
            }
            
            // Guardar l'arxiu
            doc.Save(rutaArxiuXML);
            Debug.Log("Puntuacions guardades a: " + rutaArxiuXML);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al guardar les puntuacions: " + e.Message);
        }
    }
    
    // Mètode que es crida quan el jugador mor
    public void FinalitzarPartida()
    {
        Debug.Log("Partida finalitzada. Puntuació: " + puntuacio + " Nom: " + nomJugador);
        GuardarPuntuacioXML();
    }
    
    // Mètode per obtenir la millor puntuació de l'arxiu XML
    private int ObtindreMillorPuntuacio()
    {
        List<PuntuacioJugador> puntuacions = CarregarPuntuacionsXML();
        
        if (puntuacions.Count > 0)
        {
            // Ordenar les puntuacions de major a menor
            puntuacions.Sort((a, b) => b.Puntuacio.CompareTo(a.Puntuacio));
            return puntuacions[0].Puntuacio;
        }
        
        return 0; // Si no hi ha puntuacions, retornar 0
    }
}
