using System;

[System.Serializable]
public class PuntuacioJugador
{
    public string Nom { get; set; }
    public int Puntuacio { get; set; }
    public DateTime Data { get; set; }

    public PuntuacioJugador(string nom, int puntuacio, DateTime data = default)
    {
        Nom = nom;
        Puntuacio = puntuacio;
        Data = data;
    }
}
