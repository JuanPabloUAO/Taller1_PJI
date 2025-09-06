using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StackSimulator : MonoBehaviour
{
    public UIManager uiManager;

   
    private Stack<Product> pila = new Stack<Product>();
    private List<Product> productPool;
    private bool running = false;

    
    private float tiempoInicio = 0f;
    private float tiempoFin = 0f;
    private int totalGenerados = 0;
    private int totalDespachados = 0;
    private float tiempoDespachoTotal = 0f;
    private Dictionary<string, int> despachadosPorTipo = new Dictionary<string, int>();

    void Start()
    {
        productPool = ProductParser.LoadFromTxt();
        despachadosPorTipo["Basico"] = 0;
        despachadosPorTipo["Fragil"] = 0;
        despachadosPorTipo["Pesado"] = 0;
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

        var results = BuildResults();
        string json = JsonUtility.ToJson(results, true);
        string filename = "results_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json";
        System.IO.File.WriteAllText(System.IO.Path.Combine(Application.streamingAssetsPath, filename), json);
        Debug.Log("Resultados guardados en: " + filename);
        uiManager.ShowResults(results);
    }

    IEnumerator GenerationLoop()
    {
        var rand = new System.Random(12345); 
        while (running)
        {
            int count = rand.Next(1, 4); 
            for (int i = 0; i < count; i++)
            {
                if (productPool.Count == 0) continue;

               
                Product p = productPool[totalGenerados % productPool.Count];

              
                pila.Push(p);
                Debug.Log("PUSH → Producto " + p.id + " agregado a la pila");

                totalGenerados++;
                uiManager.UpdateIndicators(pila.Count, pila.Peek(), totalGenerados, totalDespachados);
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
                
                Product current = pila.Peek();

                
                yield return new WaitForSeconds(current.tiempoDespacho);

                
                Product desp = pila.Pop();
                Debug.Log("POP → Producto " + desp.id + " retirado de la pila");

                totalDespachados++;
                tiempoDespachoTotal += desp.tiempoDespacho;
                despachadosPorTipo[desp.tipo]++;

                uiManager.UpdateIndicators(pila.Count, (pila.Count > 0 ? pila.Peek() : null), totalGenerados, totalDespachados);
            }
            else
            {
                yield return new WaitForSeconds(0.2f);
            }
            yield return null;
        }
    }

    
    [System.Serializable]
    public class Results
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

    private Results BuildResults()
    {
        Results r = new Results();
        r.total_generados = totalGenerados;
        r.total_despachados = totalDespachados;
        r.total_en_pila = pila.Count;
        r.total_tiempo_despacho = tiempoDespachoTotal;
        r.tiempo_promedio_despacho = totalDespachados > 0 ? (tiempoDespachoTotal / totalDespachados) : 0f;
        r.despachados_por_tipo = string.Join(",", despachadosPorTipo.Select(kv => kv.Key + ":" + kv.Value));
        r.tipo_mas_despachado = despachadosPorTipo.OrderByDescending(kv => kv.Value).First().Key;
        r.tiempo_total_generacion = tiempoFin - tiempoInicio;
        return r;
    }
}
