using UnityEngine;

public class deathZoneController : MonoBehaviour
{
    // Esta función se ejecuta cuando otro collider entra en el trigger.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificamos si lo que entró tiene el tag "Player".
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("El jugador ha caído en la zona de muerte.");

            // Buscamos el script del jugador y llamamos a su función de recibir daño.
            playerController player = collision.GetComponent<playerController>();
            if (player != null)
            {
                // Le aplicamos daño suficiente para matarlo instantáneamente.
                player.RecibirDanio(999f); 
            }
        }
    }
}