using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;
    public float damage = 10f;
    public bool hasPiercing = false;

    private Vector2 direction;
    private float scaleMultiplier = 0.05f;
    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>(); // Track enemies hit to avoid re-hitting them

    void Start()
    {
        Destroy(gameObject, lifetime); // Destroy bullet after its lifetime expires
    }

    public void Initialize(Vector2 fireDirection, float bulletDamage, float bulletSpeed, bool piercing)
    {
        direction = fireDirection.normalized;
        damage = bulletDamage;
        speed = bulletSpeed;
        hasPiercing = piercing;

        // Calculate the bullet size based on damage
        float size = damage * scaleMultiplier;
        transform.localScale = new Vector3(size, size, size);

        // Ensure bullet uses continuous collision detection for accuracy at high speed
        if (GetComponent<Rigidbody2D>() != null)
        {
            GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    void Update()
    {
        // Move bullet in the specified direction
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !hitEnemies.Contains(other))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                hitEnemies.Add(other); // Track this enemy as hit
            }

            // Only destroy the bullet if it does not have piercing
            if (!hasPiercing)
            {
                Destroy(gameObject);
            }
        }
        else if (!other.CompareTag("Player")) // Destroy on hitting non-enemy or non-player object
        {
            Destroy(gameObject);
        }
    }
}
