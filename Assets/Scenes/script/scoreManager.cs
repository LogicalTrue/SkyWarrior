using System.Collections;
using UnityEngine;
using TMPro; // No te olvides de este para que funcione TextMeshPro

public class ScoreManager : MonoBehaviour
{
    [Header("Componentes")]
    public TextMeshProUGUI scoreText; // Arrastrá acá tu objeto de texto

    [Header("Efecto de Puntos")]
    public float punchScale = 1.5f;   // Cuánto se va a agrandar (ej: 1.5 es 150%)
    public float punchDuration = 0.1f; // Duración de la animación de agrandar/achicar

    private int currentScore = 0;
    private Vector3 originalScale;

    // Se ejecuta una sola vez al inicio
    void Start()
    {
        currentScore = 0;
        scoreText.text = currentScore.ToString();
        
        // Guardamos la escala original del texto para saber a qué tamaño volver
        originalScale = scoreText.transform.localScale;
    }

    // Función pública para que otros scripts (como el del enemigo) la puedan llamar
    public void AddPoints(int points)
    {
        currentScore += points;
        scoreText.text = currentScore.ToString();

        // Si ya hay una animación corriendo, la paramos para empezar la nueva
        StopAllCoroutines();
        StartCoroutine(PunchEffect());
    }

    // Corutina que crea la animación de escala suave
    private IEnumerator PunchEffect()
    {
        float elapsedTime = 0f;
        Vector3 targetScale = originalScale * punchScale;

        // --- Fase de Agrandar Suavemente ---
        while (elapsedTime < punchDuration)
        {
            // Interpola linealmente (Lerp) desde la escala original a la más grande
            scoreText.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / punchDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // Espera al siguiente frame antes de continuar el bucle
        }
        // Asegura que termine exactamente en el tamaño grande
        scoreText.transform.localScale = targetScale;

        // --- Fase de Achicar Suavemente (volver al original) ---
        elapsedTime = 0f; // Reseteamos el contador
        while (elapsedTime < punchDuration)
        {
            // Ahora interpola desde la escala grande de vuelta a la original
            scoreText.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / punchDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Asegura que termine exactamente en el tamaño original
        scoreText.transform.localScale = originalScale;
    }
}