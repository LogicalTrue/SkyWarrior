using UnityEngine;

public class BolaDeFuego : MonoBehaviour
{
    public float velocidad = 15f; // Más rápido
    public float danio = 20f;
    public float tiempoDeVida = 3f; // Tiempo de seguridad por si no choca nada

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // ESTA ES LA CLAVE: Usar transform.right
        // Si la bola nace rotada, transform.right apuntará a la izquierda.
        rb.velocity = transform.right * velocidad;

        Destroy(gameObject, tiempoDeVida);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si toca al Jugador
        if (collision.CompareTag("Player"))
        {
            playerController player = collision.GetComponent<playerController>();
            if (player != null)
            {
                player.RecibirDanio(danio);
            }
            Destroy(gameObject); // Desaparece al pegar
        }
        // Si toca el Piso o Paredes (Asegurate que tus pisos tengan el Tag "Floor" o "Ground")
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Default") || collision.CompareTag("Floor"))
        {
            Destroy(gameObject); // Desaparece al chocar pared
        }
    }
}