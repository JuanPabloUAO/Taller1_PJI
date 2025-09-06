using UnityEngine;
using System;

[Serializable]
public class Product
{
    public string id;
    public string nombre;
    public string tipo; // Basico, Fragil, Pesado
    public float peso;
    public float precio;
    public int tiempoDespacho; // segundos

    public Product(string id, string nombre, string tipo, float peso, float precio, int tiempoDespacho)
    {
        this.id = id;
        this.nombre = nombre;
        this.tipo = tipo;
        this.peso = peso;
        this.precio = precio;
        this.tiempoDespacho = tiempoDespacho;
    }
}

