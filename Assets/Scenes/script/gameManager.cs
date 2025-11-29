using UnityEngine;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    // --- SINGLETON ---
    // Esto permite que cualquiera llame a GameManager.instance.LoQueSea()
    public static gameManager instance;

    private void Awake()
    {
        // Lógica básica de Singleton: Si no hay uno, soy yo.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Si ya había otro, me destruyo para no duplicar
        }
    }

    [Header("Referencias UI")]
    public GameObject gameOverPanel; // El panel negro de muerte
    public GameObject winPanel;      // El panel de victoria (NUEVO)

    // --- COSAS DE INICIO ---
    void Start()
    {
        // Nos aseguramos que los carteles arranquen apagados
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        
        Time.timeScale = 1f; // Nos aseguramos que el tiempo corra
    }

    // --- LÓGICA DE PERDER ---
    public void GameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f; // Pausa el juego
        }
    }

    // --- LÓGICA DE GANAR (NUEVO) ---
    public void Victory()
    {
        Debug.Log("¡GANASTE!");
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            Time.timeScale = 0f; // Pausa el juego
        }
    }

    // --- BOTONES ---
    public void ReintentarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu"); // Asegurate que la escena se llame "Menu"
    }
    
    public void SiguienteNivel()
    {
        Time.timeScale = 1f;
        // Carga la siguiente escena en la lista de Build Settings
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}