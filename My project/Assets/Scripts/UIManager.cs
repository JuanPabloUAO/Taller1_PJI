using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    public Text txtPileSize;
    public Text txtTop;
    public Text txtGenerated;
    public Text txtDispatched;
    public GameObject resultsPanel;
    public Text txtResults;

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
        txtTop.text = topProduct != null ? "Tope: " + topProduct.id + " - " + topProduct.nombre : "Tope: -";
        txtGenerated.text = "Generados: " + generated;
        txtDispatched.text = "Despachados: " + dispatched;
    }

    public void ShowResults(StackSimulator.Results r)
    {
        resultsPanel.SetActive(true);
        txtResults.text = $"Generados: {r.total_generados}\nDespachados: {r.total_despachados}\nEn pila: {r.total_en_pila}\nTiempo promedio despacho: {r.tiempo_promedio_despacho:F2}s\nTotal tiempo despacho: {r.total_tiempo_despacho}s\nPor tipo: {r.despachados_por_tipo}\nTipo mayor: {r.tipo_mas_despachado}";
    }
}
