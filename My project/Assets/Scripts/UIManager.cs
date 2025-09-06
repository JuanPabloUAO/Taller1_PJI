using UnityEngine;
using TMPro;   

public class UIManager : MonoBehaviour
{
    [Header("Indicadores en pantalla")]
    public TMP_Text txtPileSize;
    public TMP_Text txtTop;
    public TMP_Text txtGenerated;
    public TMP_Text txtDispatched;

    [Header("Panel de resultados")]
    public GameObject resultsPanel;
    public TMP_Text txtResults;

    [Header("Referencia al simulador")]
    public StackSimulator simulator;

    void Start()
    {
        resultsPanel.SetActive(false);
        UpdateIndicators(0, null, 0, 0);
    }

   
    public void OnStartButton()
    {
        simulator.StartSimulation();
    }

    
    public void OnStopButton()
    {
        simulator.StopSimulation();
    }

    
    public void UpdateIndicators(int pileSize, Product topProduct, int generated, int dispatched)
    {
        txtPileSize.text = "Tamaño pila: " + pileSize;
        txtTop.text = topProduct != null ? "Tope: " + topProduct.id + " - " + topProduct.nombre : "Tope: ---";
        txtGenerated.text = "Generados: " + generated;
        txtDispatched.text = "Despachados: " + dispatched;
    }

    
    public void ShowResults(StackSimulator.Results r)
    {
        resultsPanel.SetActive(true);
        txtResults.text =
            $"Generados: {r.total_generados}\n" +
            $"Despachados: {r.total_despachados}\n" +
            $"En pila: {r.total_en_pila}\n" +
            $"Tiempo promedio despacho: {r.tiempo_promedio_despacho:F2}s\n" +
            $"Total tiempo despacho: {r.total_tiempo_despacho}s\n" +
            $"Por tipo: {r.despachados_por_tipo}\n" +
            $"Tipo mayor: {r.tipo_mas_despachado}";
    }
}