using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movimiento y Sensores")]
    public Transform ray;       // Arrastrá acá el objeto "GroundCheck" (Pies)
    public Transform wallCheck; // Arrastrá acá el objeto "WallCheck" (Ojos)
    public float groundDistance = 0.5f; // Largo del rayo de pies
    public float wallDistance = 0.5f;   // Largo del rayo de pared
    public LayerMask ground;    // Seleccioná la capa "Ground"
    public float speed = 2f;

    [Header("Ajustes de IA")]
    public float flipCooldown = 0.2f; // TIEMPO DE ESPERA para no vibrar (0.2s es ideal)
    private float lastFlipTime;       // Guarda cuándo fue el último giro
    private int direction = 1;

    [Header("Vida y Combate")]
    public float maxHealth = 50f;
    public float deathAnimationDuration = 1f;
    public int pointsOnDeath = 10;
    private float currentHealth;
    private bool isDead = false;

    // --- Componentes Cacheados ---
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        lastFlipTime = Time.time; // Inicializamos el timer
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        // Movemos al enemigo usando físicas
        rb.velocity = new Vector2(speed * direction, rb.velocity.y);
    }

    private void Update()
    {
        if (isDead) return;

        // Animación de caminar
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

        // --- LÓGICA ANTI-VIBRACIÓN ---
        // Solo chequeamos si pasó el tiempo de espera (CoolDown)
        if (Time.time > lastFlipTime + flipCooldown)
        {
            // Chequeamos los sensores
            bool pisoFaltante = !IsGrounded();
            bool hayPared = IsTouchingWall();

            // Si falta piso O hay pared -> Giramos
            if (pisoFaltante || hayPared)
            {
                Flip();
            }
        }
    }

    private void Flip()
    {
        // Guardamos el momento actual para reiniciar el cooldown
        lastFlipTime = Time.time;

        // Invertimos dirección matemática
        direction *= -1;

        // Invertimos visualmente el sprite
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private bool IsGrounded()
    {
        // Rayo hacia abajo desde los pies
        return Physics2D.Raycast(ray.position, Vector2.down, groundDistance, ground);
    }

    private bool IsTouchingWall()
    {
        // 1. Determinamos la dirección REAL basada en el dibujo (Scale)
        // Si la escala X es positiva, mira a la derecha. Si es negativa, izquierda.
        float sentidoVisual = transform.localScale.x > 0 ? 1f : -1f;
        Vector2 direccionReal = Vector2.right * sentidoVisual;

        // 2. Tiramos el rayo
        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, direccionReal, wallDistance, ground);

        // --- DEBUG VISUAL (Solo para que vos veas qué pasa) ---
        // Si toca algo, dibuja línea VERDE. Si no, dibuja línea ROJA.
        Color colorRayo = hit.collider != null ? Color.green : Color.red;
        Debug.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(direccionReal * wallDistance), colorRayo);

        // Si tocó algo, le decimos a la consola QUÉ tocó (para descubrir si se toca a sí mismo)
        if (hit.collider != null)
        {
            // Si ves en la consola que dice "Enemy" o el nombre de tu bicho, 
            // es que se está detectando a sí mismo.
            // Debug.Log("El rayo chocó con: " + hit.collider.name); 
        }
        // -----------------------------------------------------

        return hit.collider != null;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        col.enabled = false;

        // Si tenés un ScoreManager, descomentá esto:
        // FindObjectOfType<ScoreManager>().AddPoints(pointsOnDeath);

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        Destroy(gameObject, deathAnimationDuration);
    }

    private IEnumerator FlashRed()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = originalColor;
    }

    // --- GIZMOS PARA VER LOS RAYOS EN EL EDITOR ---
    private void OnDrawGizmos()
    {
        if (ray != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(ray.position, ray.position + Vector3.down * groundDistance);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            // Calculamos la dirección visual basada en la escala actual para el dibujo
            float lado = transform.localScale.x > 0 ? 1 : -1;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * lado * wallDistance);
        }
    }
}