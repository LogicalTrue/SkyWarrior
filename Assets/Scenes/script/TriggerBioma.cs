using UnityEngine;

public class TriggerBioma : MonoBehaviour
{
    // Acá vas a arrastrar tu objeto "Parallax" (el que tiene el script grande)
    public parallaxMovement sistemaParallax;

private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Aviso que ALGO tocó la caja
        Debug.Log("LOG: Algo tocó el sensor: " + collision.gameObject.name);

        if (collision.CompareTag("Player"))
        {
            // 2. Aviso que fue el PLAYER y mando la orden
            Debug.Log("LOG: ¡Es el Player! Enviando orden SwitchToDesert...");
            sistemaParallax.SwitchToDesert();
        }
        else
        {
            // 3. Aviso si falló el Tag
            Debug.LogWarning("LOG: Falló el Tag. El objeto tiene el tag: " + collision.tag);
        }
    }
}