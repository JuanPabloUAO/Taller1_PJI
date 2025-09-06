using UnityEngine;

using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ProductParser
{
    public static List<Product> LoadFromTxt(string filename = "productos.txt")
    {
        string path = Path.Combine(Application.streamingAssetsPath, filename);
        var list = new List<Product>();
        if (!File.Exists(path))
        {
            Debug.LogError("No se encontró productos.txt en StreamingAssets: " + path);
            return list;
        }
        var lines = File.ReadAllLines(path);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var parts = line.Split('|');
            if (parts.Length < 6) continue;
            string id = parts[0];
            string nombre = parts[1];
            string tipo = parts[2];
            float peso = float.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture);
            float precio = float.Parse(parts[4], System.Globalization.CultureInfo.InvariantCulture);
            int tiempo = int.Parse(parts[5]);
            list.Add(new Product(id, nombre, tipo, peso, precio, tiempo));
        }
        return list;
    }
}

