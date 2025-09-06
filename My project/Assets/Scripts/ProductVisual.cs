using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class ProductVisual : MonoBehaviour
{
    [Header("UI refs")]
    public TextMeshProUGUI txtTitle;     // 👈 cambio a TextMeshProUGUI
    public TextMeshProUGUI txtSubtitle;  // 👈 cambio a TextMeshProUGUI
    public Image background;             // Imagen de fondo
    public Image typeBadge;              // Marcador opcional
    public CanvasGroup canvasGroup;

    void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (background == null) background = GetComponent<Image>();
    }

    public void SetData(Product p)
    {
        if (p == null) return;

        if (txtTitle != null)
            txtTitle.text = $"{p.id} - {p.nombre}";

        if (txtSubtitle != null)
            txtSubtitle.text = $"{p.tipo} | {p.peso:0.0}kg | ${p.precio:0.00}";

        SetBackgroundByType(p.tipo);
    }

    public void SetBackgroundByType(string tipo)
    {
        if (background == null) return;

        string hex = "#FFFFFF";
        if (tipo.Equals("Basico", System.StringComparison.OrdinalIgnoreCase)) hex = "#E3FCEF";
        else if (tipo.Equals("Fragil", System.StringComparison.OrdinalIgnoreCase)) hex = "#FFF1F0";
        else if (tipo.Equals("Pesado", System.StringComparison.OrdinalIgnoreCase)) hex = "#EEF2FF";

        if (ColorUtility.TryParseHtmlString(hex, out Color col))
            background.color = col;

        if (typeBadge != null)
            typeBadge.color = Color.Lerp(background.color, Color.black, 0.25f);
    }

    public void SetAlpha(float a)
    {
        if (canvasGroup != null) canvasGroup.alpha = a;
    }
}
