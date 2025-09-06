using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Globalization;

public static class ProductParser
{
   
    public static List<Product> LoadFromTxt(string filename = "productos.txt")
    {
        var list = new List<Product>();
        string path = Path.Combine(Application.streamingAssetsPath, filename);

        if (!File.Exists(path))
        {
            Debug.LogError($"No se encontró {filename} en StreamingAssets: {path}");
            return list;
        }

        string[] lines = File.ReadAllLines(path);
        int lineNum = 0;
        foreach (var raw in lines)
        {
            lineNum++;
            string line = raw.Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;
            var parts = line.Split('|');
            if (parts.Length < 6)
            {
                Debug.LogWarning($"Linea {lineNum} inválida (espera 6 columnas): {line}");
                continue;
            }
            try
            {
                string id = parts[0].Trim();
                string nombre = parts[1].Trim();
                string tipo = parts[2].Trim();
                float peso = float.Parse(parts[3].Trim(), CultureInfo.InvariantCulture);
                float precio = float.Parse(parts[4].Trim(), CultureInfo.InvariantCulture);
                int tiempo = int.Parse(parts[5].Trim(), CultureInfo.InvariantCulture);
                list.Add(new Product(id, nombre, tipo, peso, precio, tiempo));
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Error parseando linea {lineNum}: {ex.Message} -> '{line}'");
            }
        }

        if (list.Count == 0) Debug.LogWarning("No se cargaron productos (archivo vacío o formato incorrecto).");
        return list;
    }
}
