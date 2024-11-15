using System.Collections;
using UnityEngine;
using Pathfinding;

public abstract class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public int coinValue = 1; // Default coin reward for defeating enemy
    public float speed = 200f;
    public float nextWaypointDistance = 1f;
    public bool isStunned = false;

    protected Path path;
    protected int currentWaypoint = 0;
    protected Seeker seeker;
    protected Rigidbody2D rb;
    protected float healthScale; // Unused for now
    public Transform target;
    public delegate void EnemyDefeatedHandler();
    public event EnemyDefeatedHandler OnDefeated;

    protected virtual void Start()
{
    currentHealth = maxHealth;
    seeker = GetComponent<Seeker>();
    rb = GetComponent<Rigidbody2D>();

    // Automatically set the target to the player
    target = GameObject.FindWithTag("Player")?.transform;

    // Start pathfinding
    InvokeRepeating("UpdatePath", 0f, 0.5f);
}


    protected void UpdatePath()
    {
        if (seeker.IsDone() && target != null)
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    protected void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (isStunned)
    {
        rb.velocity = Vector2.zero; // Prevent movement when stunned
        return;
    }

    if (path == null) return;
    if (currentWaypoint >= path.vectorPath.Count) return;

    Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
    Vector2 force = direction * speed * Time.deltaTime;
    rb.velocity = force;

    float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
    if (distance < nextWaypointDistance)
    {
        currentWaypoint++;
    }
    }

    public virtual void TakeDamage(float damage)
    {
        TankController playerController = FindObjectOfType<TankController>();
        playerController.DealDamage(damage);
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
{
    TankController playerController = FindObjectOfType<TankController>();
    if (playerController != null)
    {
        
        playerController.AddCoins(coinValue);

        // 5% chance to add 5 extra coins
        if (Random.value < 0.05f)
        {
            SoundManager.Instance.PlayCoin();
            playerController.AddCoins(5);
            Debug.Log("Bonus 5 coins awarded!");
        }
    }

    OnDefeated?.Invoke();  // Notify WaveManager on death
    Destroy(gameObject);
}


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TankController playerController = collision.gameObject.GetComponent<TankController>();
            if (playerController != null)
            {
                ApplyCollisionEffect(playerController); // Apply specific collision effect
                OnDefeated?.Invoke();  // Notify WaveManager on death
                Destroy(gameObject);
            }
        }
    }

    protected abstract void ApplyCollisionEffect(TankController playerController);
}
