using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerController : MonoBehaviour
{
    // --- Variables de Movimiento ---
    private int movimientoHorizontal = 0;
    private bool isGround = false;
    private Vector2 mov = Vector2.zero;
    [SerializeField] float speed = 10f;
    [SerializeField] float jump = 15f;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // --- Variables de Vida y Daño ---
    // (Borramos barraDeVida porque ahora lo maneja el Singleton)
    [SerializeField] private float vidaMaxima = 100f;
    private float vidaActual;
    private bool isDead = false;

    // --- Variables de Retroceso ---
    [Header("Retroceso")]
    [SerializeField] private float knockbackForce = 15f;
    [SerializeField] private float knockbackDuration = 0.3f;
    private bool isKnockedBack = false;

    // --- Variables de Ataque ---
    [Header("Ataque")]
    public GameObject swordHitbox;
    public float attackDuration = 0.15f;
    public float attackDamage = 10f;
    private bool isAttacking = false;

    // --- Referencias a otros Scripts ---
    [Header("Referencias")]
    public followCamera cameraScript;
    public gameManager gameManager; // REFERENCIA NUEVA PARA EL CARTEL

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        vidaActual = vidaMaxima;

        if (swordHitbox != null)
        {
            swordHitbox.SetActive(false);
            var hb = swordHitbox.GetComponent<PlayerHitbox>();
            if (hb != null) hb.damage = attackDamage;
        }
    }

    void Update()
    {
        // Si el jugador está siendo empujado o está muerto, no procesa ninguna entrada.
        if (isKnockedBack || isDead) return;

        // Lógica de Ataque
        if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.X)) && !isAttacking)
        {
            animator.SetTrigger("Attack");
            StartCoroutine(EnableSword());
        }

        // Lógica de Animación
        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));

        // Lógica de Movimiento Horizontal
        if (Input.GetKey(KeyCode.D))
        {
            movimientoHorizontal = 1;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            movimientoHorizontal = -1;
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            movimientoHorizontal = 0;
        }
        mov = new Vector2(movimientoHorizontal, 0).normalized;

        // Lógica de Salto
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Saltar();
            isGround = false;
        }
    }

    private void FixedUpdate()
    {
        // Si el jugador está muerto, no se le aplica ninguna fuerza de movimiento.
        if (isKnockedBack || isDead) return;
        
        rb.velocity = new Vector2(mov.x * speed, rb.velocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Si el jugador está muerto, no interactúa con ninguna colisión.
        if (isDead) return;

        if (collision.gameObject.CompareTag("Floor"))
        {
            isGround = true;
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            RecibirDanio(10);
            Debug.Log("¡Colisionaste con un enemigo!");
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            rb.velocity = Vector2.zero;
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            StartCoroutine(StopKnockback());
        }
    }

    private void Saltar()
    {
        rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
    }

    // --- CORUTINAS ---
    private IEnumerator EnableSword()
    {
        isAttacking = true;
        if (swordHitbox != null) swordHitbox.SetActive(true);
        yield return new WaitForSeconds(attackDuration);
        if (swordHitbox != null) swordHitbox.SetActive(false);
        isAttacking = false;
    }

    private IEnumerator StopKnockback()
    {
        isKnockedBack = true;
        yield return new WaitForSeconds(knockbackDuration);
        isKnockedBack = false;
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }

    // --- FUNCIÓN DE DAÑO Y MUERTE ---
    public void RecibirDanio(float danio)
    {
        if (isDead) return;

        vidaActual = Mathf.Max(0, vidaActual - danio);

        // CONEXIÓN CON LA UI DE VIDA (La barra fantasma y el temblor)
        if (HealthBarUI.instance != null)
        {
            HealthBarUI.instance.ActualizarVida(vidaActual, vidaMaxima);
        }

        StartCoroutine(FlashRed());

        // Lógica de Muerte
        if (vidaActual <= 0)
        {
            isDead = true;
            animator.SetBool("isDead", true);

            // Le dice a la cámara que deje de seguirlo.
            if (cameraScript != null)
            {
                cameraScript.StopFollowing();
            }

            // Convierte su collider en un trigger para que atraviese el suelo.
            GetComponent<Collider2D>().isTrigger = true;

            // Anula cualquier movimiento horizontal para una caída vertical limpia.
            rb.velocity = new Vector2(0, rb.velocity.y);

            // Mantiene la rotación fija para que no vuelque al caer.
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            // --- LLAMADO AL GAME OVER CON DEMORA ---
            // Esperamos 1.5 segundos para que se vea la animación de muerte y caída
            if (gameManager != null)
            {
                Invoke("ActivarCartel", 0.8f);
            }
        }
    }

    // Función auxiliar para activar el cartel después del Invoke
void ActivarCartel()
    {
        // OPCIÓN A (Usando la variable arrastrada):
        if(gameManager != null) 
        {
             gameManager.GameOver(); // <--- Cambié 'MostrarGameOver' por 'GameOver'
        }
        
        // OPCIÓN B (Usando Singleton, más seguro y sin arrastrar nada):
        // gameManager.instance?.GameOver();
    }
}