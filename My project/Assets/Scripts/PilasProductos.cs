using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;
using System.Text;

public class PilasProductos : MonoBehaviour
{
    
    private Stack<Product> pila = new Stack<Product>();
    private List<Product> productosDisponibles = new List<Product>();

   
    private int totalGenerados = 0;
    private int totalDespachados = 0;
    private float tiempoDespachoTotal = 0f;
    private float tiempoInicio = 0f, tiempoFin = 0f;
    private Dictionary<string, int> despachadosPorTipo = new Dictionary<string, int>();

    
    [Header("Textos (TMP)")]
    public TMP_Text txtTope;          
    public TMP_Text txtCount;         
    public TMP_Text txtGenerados;     
    public TMP_Text txtDespachados;   
    public TMP_Text txtResultados;    

    [Header("Controles")]
    public GameObject panelResultados;
    public GameObject btnStart;
    public GameObject btnStop;

   
    [Header("Visual Stack (prefab UI)")]
    public RectTransform stackContainer;   
    public GameObject productUIPrefab;      
    public float itemHeight = 48f;          
    public float spacing = 6f;              
    public float pushAnimTime = 0.22f;
    public float popAnimTime = 0.22f;

   
    private List<GameObject> visualStack = new List<GameObject>();

    private bool running = false;
    private System.Random rng = new System.Random(1234);

    void Start()
    {
        
        despachadosPorTipo["Basico"] = 0;
        despachadosPorTipo["Fragil"] = 0;
        despachadosPorTipo["Pesado"] = 0;

        productosDisponibles = ProductParser.LoadFromTxt();

        
        txtTope.text = "Tope: ---";
        txtCount.text = "Pila.Count: 0";
        txtGenerados.text = "Generados: 0";
        txtDespachados.text = "Despachados: 0";
        panelResultados.SetActive(false);

        if (productUIPrefab == null) Debug.LogWarning("productUIPrefab no asignado (Inspector).");
        if (stackContainer == null) Debug.LogWarning("stackContainer no asignado (Inspector).");
    }

    void Update()
    {
      
        txtCount.text = $"Pila.Count: {pila.Count}";
    }


    public void StartSimulation()
    {
        if (running) return;
        running = true;
        tiempoInicio = Time.time;
        StartCoroutine(GenerationLoop());
        StartCoroutine(DispatcherLoop());
    }

    public void StopSimulation()
    {
        if (!running) return;
        running = false;
        tiempoFin = Time.time;
        ExportarResultados();
    }

    
    public void ManualPushOne()
    {
        if (productosDisponibles.Count == 0) return;
        Product p = productosDisponibles[totalGenerados % productosDisponibles.Count];
        DoPush(p);
    }

    public void ManualPopOne()
    {
        if (pila.Count == 0) return;
        DoPop();
    }

   
    IEnumerator GenerationLoop()
    {
        while (running)
        {
            int cant = rng.Next(1, 4); 
            for (int i = 0; i < cant; i++)
            {
                if (productosDisponibles.Count == 0) break;
                Product p = productosDisponibles[totalGenerados % productosDisponibles.Count];
                DoPush(p);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator DispatcherLoop()
    {
        while (running)
        {
            if (pila.Count > 0)
            {
                Product top = pila.Peek();
                yield return new WaitForSeconds(Mathf.Max(0.01f, top.tiempoDespacho));
                DoPop();
            }
            else
            {
                yield return new WaitForSeconds(0.25f);
            }
        }
    }

   
    private void DoPush(Product p)
    {
       
        pila.Push(p);
        totalGenerados++;

      
        txtGenerados.text = $"Generados: {totalGenerados}";

    
        if (productUIPrefab != null && stackContainer != null)
        {
            GameObject go = Instantiate(productUIPrefab, stackContainer);
            go.transform.SetAsFirstSibling(); 
            var pv = go.GetComponent<ProductVisual>();
            if (pv != null) pv.SetData(p);

            
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.pivot = new Vector2(0.5f, 1f);

           
            rt.anchoredPosition = new Vector2(0f, 20f);
            var cg = pv.canvasGroup;
            if (cg != null) cg.alpha = 0f;

           
            visualStack.Insert(0, go);

           
            float targetY = -0 * (itemHeight + spacing);
            StartCoroutine(AnimateMoveAndFade(rt, cg, targetY, pushAnimTime));

         
            RepositionVisualsSmooth();
        }
        else
        {
            
            txtTope.text = $"Tope: {p.id} - {p.nombre}";
        }

       
        txtTope.text = $"Tope: {p.id} - {p.nombre}";
    }

    private void DoPop()
    {
        if (pila.Count == 0) return;

     
        Product desp = pila.Pop();
        totalDespachados++;
        tiempoDespachoTotal += desp.tiempoDespacho;
        if (despachadosPorTipo.ContainsKey(desp.tipo)) despachadosPorTipo[desp.tipo]++;
        else despachadosPorTipo[desp.tipo] = 1;

       
        txtDespachados.text = $"Despachados: {totalDespachados}";

     
        if (visualStack.Count > 0)
        {
            GameObject go = visualStack[0];
            visualStack.RemoveAt(0);
            var pv = go.GetComponent<ProductVisual>();
            var rt = go.GetComponent<RectTransform>();
            var cg = go.GetComponent<CanvasGroup>();
          
            StartCoroutine(AnimatePopAndDestroy(rt, cg, popAnimTime));
        
            RepositionVisualsSmooth();
        }

       
        if (pila.Count > 0)
        {
            var top = pila.Peek();
            txtTope.text = $"Tope: {top.id} - {top.nombre}";
        }
        else
        {
            txtTope.text = "Tope: ---";
        }
    }

  
    IEnumerator AnimateMoveAndFade(RectTransform rt, CanvasGroup cg, float targetY, float duration)
    {
        if (rt == null) yield break;
        Vector2 start = rt.anchoredPosition;
        Vector2 end = new Vector2(0f, targetY);
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            rt.anchoredPosition = Vector2.Lerp(start, end, Mathf.SmoothStep(0f, 1f, k));
            if (cg != null) cg.alpha = Mathf.Lerp(cg.alpha, 1f, k);
            yield return null;
        }
        rt.anchoredPosition = end;
        if (cg != null) cg.alpha = 1f;
    }

    IEnumerator AnimatePopAndDestroy(RectTransform rt, CanvasGroup cg, float duration)
    {
        if (rt == null) { yield break; }
        Vector2 start = rt.anchoredPosition;
        Vector2 end = new Vector2(0f, 30f); 
        float t = 0f;
        float startAlpha = cg != null ? cg.alpha : 1f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            rt.anchoredPosition = Vector2.Lerp(start, end, Mathf.SmoothStep(0f, 1f, k));
            if (cg != null) cg.alpha = Mathf.Lerp(startAlpha, 0f, k);
            yield return null;
        }
       
        Destroy(rt.gameObject);
    }

    
    private void RepositionVisualsSmooth()
    {
        for (int i = 0; i < visualStack.Count; i++)
        {
            GameObject go = visualStack[i];
            RectTransform rt = go.GetComponent<RectTransform>();
            float targetY = -i * (itemHeight + spacing);
       
            StartCoroutine(MoveToY(rt, targetY, 0.18f));
        }
    }

    IEnumerator MoveToY(RectTransform rt, float newY, float duration)
    {
        if (rt == null) yield break;
        Vector2 start = rt.anchoredPosition;
        Vector2 end = new Vector2(0f, newY);
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            rt.anchoredPosition = Vector2.Lerp(start, end, Mathf.SmoothStep(0f, 1f, k));
            yield return null;
        }
        rt.anchoredPosition = end;
    }

 
    private void ExportarResultados()
    {
     
        var resultados = new Resultados
        {
            total_generados = totalGenerados,
            total_despachados = totalDespachados,
            total_en_pila = pila.Count,
            total_tiempo_despacho = tiempoDespachoTotal,
            tiempo_promedio_despacho = totalDespachados > 0 ? tiempoDespachoTotal / totalDespachados : 0f,
            despachados_por_tipo = string.Join(",", despachadosPorTipo.Select(kv => kv.Key + ":" + kv.Value)),
            tipo_mas_despachado = despachadosPorTipo.Count > 0 ? despachadosPorTipo.OrderByDescending(kv => kv.Value).First().Key : "-",
            tiempo_total_generacion = tiempoFin - tiempoInicio
        };

       
        string json = JsonUtility.ToJson(resultados, true);
        try
        {
            string filename = "resultados_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json";
            string path = Path.Combine(Application.streamingAssetsPath, filename);
            File.WriteAllText(path, json);
            Debug.Log("Resultados guardados en: " + path);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("No se pudo escribir JSON en StreamingAssets: " + ex.Message);
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("==== RESULTADOS ====");
        sb.AppendLine($"Total generados: {resultados.total_generados}");
        sb.AppendLine($"Total despachados: {resultados.total_despachados}");
        sb.AppendLine($"Total en pila: {resultados.total_en_pila}");
        sb.AppendLine($"Tiempo promedio despacho: {resultados.tiempo_promedio_despacho:F2} s");
        sb.AppendLine($"Tiempo total de despacho: {resultados.total_tiempo_despacho:F2} s");
        sb.AppendLine($"Tiempo total generación (desde Start hasta Stop): {resultados.tiempo_total_generacion:F2} s");
        sb.AppendLine("Despachados por tipo:");
        foreach (var kv in despachadosPorTipo)
        {
            sb.AppendLine($"  - {kv.Key}: {kv.Value}");
        }
        sb.AppendLine($"Tipo más despachado: {resultados.tipo_mas_despachado}");
        sb.AppendLine("====================");

       
        panelResultados.SetActive(true);
        txtResultados.text = sb.ToString();
    }

    [Serializable]
    public class Resultados
    {
        public int total_generados;
        public int total_despachados;
        public int total_en_pila;
        public float tiempo_promedio_despacho;
        public float total_tiempo_despacho;
        public string despachados_por_tipo;
        public string tipo_mas_despachado;
        public float tiempo_total_generacion;
    }
}
