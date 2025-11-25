using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    public float damage = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!gameObject.activeSelf) return; // si el hitbox está apagado, no pega

        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>(); // tu script de enemigo
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Golpeaste al enemigo!");
            }
        }
    }
}
