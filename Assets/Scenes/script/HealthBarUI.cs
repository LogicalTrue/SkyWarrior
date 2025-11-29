using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    // --- SINGLETON (Para que el Player lo encuentre fácil) ---
    public static HealthBarUI instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    [Header("Referencias")]
    public Image barraVerde;    // La barra de vida actual (health)
    public Image barraFantasma; // La barra blanca (ghost)
    
    [Header("Configuración")]
    public float velocidadFantasma = 0.5f;
    public float intensidadTemblor = 5f;
    public float duracionTemblor = 0.2f;

    private RectTransform rectTransform;
    private Vector3 posicionOriginal;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if(rectTransform != null) posicionOriginal = rectTransform.anchoredPosition;
    }

    public void ActualizarVida(float vidaActual, float vidaMaxima)
    {
        float porcentaje = vidaActual / vidaMaxima;

        // 1. Bajamos la barra verde de golpe
        if(barraVerde != null) barraVerde.fillAmount = porcentaje;

        // 2. Iniciamos la animación del fantasma y el temblor
        StopAllCoroutines();
        StartCoroutine(AnimacionFantasma(porcentaje));
        StartCoroutine(Sacudon());
    }

    private IEnumerator AnimacionFantasma(float targetFill)
    {
        yield return new WaitForSeconds(0.1f); // Pequeña pausa
        while (barraFantasma != null && barraFantasma.fillAmount > targetFill)
        {
            barraFantasma.fillAmount -= velocidadFantasma * Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator Sacudon()
    {
        float tiempo = 0;
        while (tiempo < duracionTemblor && rectTransform != null)
        {
            float x = Random.Range(-1f, 1f) * intensidadTemblor;
            float y = Random.Range(-1f, 1f) * intensidadTemblor;
            rectTransform.anchoredPosition = posicionOriginal + new Vector3(x, y, 0);
            tiempo += Time.deltaTime;
            yield return null;
        }
        if(rectTransform != null) rectTransform.anchoredPosition = posicionOriginal;
    }
}