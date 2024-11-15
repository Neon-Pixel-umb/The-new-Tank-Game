using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class TankController : MonoBehaviour
{
    public TMP_Text coinText;
    public int coins = 0;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 3f;
    public float bulletSpeed = 10f;
    public float damage = 10f;
    public WaveManager waveManager;
    public GameObject startPanel; // Reference to Start/Game Over panel
    public TMP_Text gameOverText; // Reference to Game Over Text

    public bool hasPiercingBullets = false; // Start with piercing disabled
    public bool LeftOvers = false;

    private float nextFireTime = 0f;
    public float maxSpeed = 5f;
    public float acceleration = 5f;
    public float deceleration = 5f;
    public int health;
    [SerializeField] private int startingHealth = 4;

    private Vector2 currentVelocity = Vector2.zero;
    private Rigidbody2D rb;
    private Vector2 smoothVelocity = Vector2.zero;

    public Sprite normalSprite;
    public Sprite stunnedSprite;
    public Sprite invincibleSprite; // Special sprite for invincibility

    private SpriteRenderer spriteRenderer;

    public List<Image> heartIcons;

    private bool isStunned = false;
    private bool isInvincible = false;
    private bool isFullyInvincible = false; // New boolean for full invincibility with Star

    private bool isPaused = false;
    private float stunEndTime;
    private float invincibilityEndTime;
    private const float baseIFrameDuration = 1.5f;
    private Coroutine invincibilityCoroutine;

    private List<Item> items = new List<Item>();
    public Item activeItem;
    public Image activeItemImage;
    public Image rechargeOverlay;
    public bool radarEnabled = false; // Start radar as disabled
    public bool Stress = false;
    public bool Exhaust = false;
    
    public float damageMultiplier = 1f;
    public float circleStunDurationMultiplier = 1f;
    public Camera miniMapCamera; // Reference to MiniMap Camera

    void Start()
    {
        health = startingHealth;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.drag = 0;

        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateHealthDisplay();
        UpdateCoinDisplay();
        UpdateActiveItemUI();
        UpdateMiniMap(); // Initialize MiniMap settings based on radarEnabled
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // waveManager?.ShowItemSelection(true);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActivateActiveItem();
        }
    }

    public void SetPaused(bool paused)
    {
        isPaused = paused;
    }

    private void FixedUpdate()
    {
        if (isPaused) return;

        if (isStunned)
        {
            rb.velocity = Vector2.zero;
            if (Time.time >= stunEndTime) RemoveStun();
            else return;
        }

        if (isInvincible && Time.time >= invincibilityEndTime)
        {
            isInvincible = false;
            StopFlashing();
        }

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime && !isStunned)
        {
            FireBullet();
            nextFireTime = Time.time + (1f / fireRate);
        }

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector2 inputDirection = new Vector2(horizontalInput, verticalInput).normalized;

        Vector2 targetVelocity = inputDirection * (isStunned ? maxSpeed * 0.2f : maxSpeed);
        currentVelocity = Vector2.SmoothDamp(currentVelocity, targetVelocity, ref smoothVelocity, 
            (inputDirection.magnitude > 0) ? 1f / acceleration : 1f / deceleration);
        Vector2 newPosition = rb.position + currentVelocity * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);

        UpdateHealthDisplay();
        UpdateCoinDisplay();
        UpdateRechargeOverlay();
    }

    public void AddItem(Item newItem)
    {
        if (newItem.isActiveItem)
        {
            RemoveActiveItemEffects();
            activeItem = newItem;
            activeItem.RechargeProgress = -1;
            UpdateActiveItemUI();
        }
        else
        {
            items.Add(newItem);
            newItem.ApplyEffect(this);
        }

        
    }

    private void RemoveActiveItemEffects()
    {
        activeItem = null;
    }

    public void ActivateActiveItem()
{
    if (activeItem != null && activeItem.RechargeProgress == 0 && !Exhaust)
    {
        activeItem.ApplyEffect(this);
        activeItem.RechargeProgress = activeItem.rechargeRequirement; // Reset recharge after use

        UpdateRechargeOverlay(); // Immediately update overlay after activation
    }
}

    private void UpdateActiveItemUI()
    {
        if (activeItem != null)
        {
            activeItemImage.sprite = activeItem.sprite;
            rechargeOverlay.fillAmount = 1f; // Fully charged initially
            rechargeOverlay.gameObject.SetActive(true);
            activeItemImage.gameObject.SetActive(true); // Ensure image visibility
        }
        else
        {
            activeItemImage.sprite = null;
            rechargeOverlay.gameObject.SetActive(false);
            activeItemImage.gameObject.SetActive(false); // Hide image when no active item
        }
    }

    private void FireBullet()
    {
        Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 fireDirection = (targetPosition - (Vector2)firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.Initialize(fireDirection, damage, bulletSpeed, hasPiercingBullets);
        // Play shooting sound
        SoundManager.Instance.PlayShootSound();
    }

    public void TakeDamage(int damageAmount, float iframeDuration)
{
    // Skip taking damage if fully invincible or invincible (iFrames)
    if (isFullyInvincible || isInvincible) return;

    health -= damageAmount;
    SoundManager.Instance.PlayTakeDamageSound();
    health = Mathf.Clamp(health, 0, 12);
    UpdateHealthDisplay();

    if (health <= 0)
    {
        GameOver();
        return;
    }
    else
    {
        // Apply iFrames after taking damage
        invincibilityEndTime = Time.time + iframeDuration;
        isInvincible = true;
        invincibilityCoroutine = StartCoroutine(FlashSprite());
    }

    if(Stress){
        StartCoroutine(StressChallengeEffect());
    }
}

private IEnumerator StressChallengeEffect()
{
    int originalHealth = health;
    health = 1;
    UpdateHealthDisplay();
    yield return new WaitForSeconds(5f);
    if(health > 0){
        health = originalHealth;
        UpdateHealthDisplay();
    }
    
}


    public void UpdateHealthDisplay()
    {
        health = Mathf.Clamp(health, 0, 12);
        for (int i = 0; i < heartIcons.Count; i++)
        {
            heartIcons[i].enabled = i < health;
        }
    }

    public void AddCoins(int amount)
    {
        coins = Mathf.Clamp(coins + amount, 0, 999);
        UpdateCoinDisplay();
    }

    public void UpdateCoinDisplay()
    {
        coinText.text = "X: " + coins;
    }

    private void GameOver()
    {
        isPaused = true;
        gameOverText.gameObject.SetActive(true); // Show "Game Over" text
        startPanel.SetActive(true); // Show Start/Game Over panel
        waveManager.DisableGameActions(); // Prevent further gameplay actions
    }

    public void StartNewGame()
    {
        ResetPlayerState();
        startPanel.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        waveManager.ResetSpawners(); // Reactivate spawners for a new game
    }

    private void ResetPlayerState()
    {
        health = startingHealth;
        coins = 0;
        items.Clear();
        activeItem = null;
        isPaused = false;
        hasPiercingBullets = false;
        isInvincible = false;
        isFullyInvincible = false;
        radarEnabled = false;
        LeftOvers = false;
        maxSpeed = 5f;
        fireRate = 3f;
        bulletSpeed = 10f;
        damage = 10f;
        UpdateHealthDisplay();
        UpdateCoinDisplay();
        UpdateActiveItemUI();
        UpdateMiniMap();
    }

    public void ApplyStun(float duration)
{
    // Only apply stun if not fully invincible (Star active)
    if (isFullyInvincible || isStunned) return;

    isStunned = true;
    stunEndTime = Time.time + duration*circleStunDurationMultiplier;
    spriteRenderer.sprite = stunnedSprite;
    rb.velocity = Vector2.zero;
    currentVelocity = Vector2.zero;
}


    private void RemoveStun()
    {
        isStunned = false;
        spriteRenderer.sprite = normalSprite;
    }

   private IEnumerator FlashSprite()
    {
        while (isInvincible)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
        }
        spriteRenderer.enabled = true;
    }


    private void StopFlashing()
    {
        if (invincibilityCoroutine != null)
        {
            StopCoroutine(invincibilityCoroutine);
            invincibilityCoroutine = null;
        }
        spriteRenderer.enabled = true;
    }

    public void DealDamage(float damageAmount)
{
    if (activeItem != null && activeItem.RechargeProgress > 0)
    {
        activeItem.IncreaseRecharge(damageAmount * damageMultiplier);
        UpdateRechargeOverlay();
    }
}

private void UpdateRechargeOverlay()
{
    if (activeItem != null)
    {
        // Calculate recharge progress from max to zero
        float rechargeProgress = Mathf.Clamp01(activeItem.RechargeProgress / activeItem.rechargeRequirement);

        // Adjust the Y scale of the overlay based on recharge progress
        rechargeOverlay.rectTransform.localScale = new Vector3(1f, rechargeProgress, 1f);

        // Show overlay while recharging, hide when fully recharged
        rechargeOverlay.gameObject.SetActive(rechargeProgress > 0);
    }
    else
    {
        rechargeOverlay.gameObject.SetActive(false);
    }
}

public void UpdateMiniMap()
    {
        if (miniMapCamera != null)
        {
            if (radarEnabled)
            {
                miniMapCamera.cullingMask |= 1 << LayerMask.NameToLayer("MiniMapIconsHidden");
            }
            else
            {
                miniMapCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("MiniMapIconsHidden"));
            }
        }
    }

public void StartEnemyStun(Enemy enemy, float duration)
{
    StartCoroutine(StunEnemyCoroutine(enemy, duration));
}

private IEnumerator StunEnemyCoroutine(Enemy enemy, float duration)
{
    enemy.isStunned = true;
    yield return new WaitForSeconds(duration);
    enemy.isStunned = false;
}

    public void AdjustDamage(float baseChange)
{
    // Apply nonlinear adjustment based on whether we're increasing or decreasing damage
    if (baseChange > 0)
    {
        // Nonlinear increase
        damage += baseChange * Mathf.Pow((10f / (damage + 10f)), 0.5f);
    }
    else
    {
        // Nonlinear decrease
        damage += baseChange * Mathf.Pow((damage + 10f) / 10f, 0.5f);
    }

    // Clamp damage to a minimum value to avoid negative or zero damage
    damage = Mathf.Max(damage, 1f);
}

public void AdjustFireRate(float baseChange)
{
    if (baseChange > 0)
    {
        // Nonlinear increase in fire rate (i.e., faster shooting)
        fireRate += baseChange * Mathf.Pow((5f / (fireRate + 5f)), 0.5f);
    }
    else
    {
        // Nonlinear decrease in fire rate (i.e., slower shooting)
        fireRate += baseChange * Mathf.Pow((fireRate + 5f) / 5f, 0.5f);
    }

    // Clamp fire rate to a maximum or minimum value as needed
    fireRate = Mathf.Clamp(fireRate, 0.5f, 10f); // Adjust 0.5f and 10f based on game needs
}

public void AdjustSpeed(float speedChange)
{
    // Linear increase with a hard cap and a minimum value
    maxSpeed = Mathf.Clamp(maxSpeed + speedChange, 1f, 15f); // Minimum of 1, maximum of 15
}

public void AdjustBulletSpeed(float speedChange)
{
    // Linear increase with a hard cap and a minimum value
    bulletSpeed = Mathf.Clamp(bulletSpeed + speedChange, 1f, 20f); // Minimum of 1, maximum of 20
}

public IEnumerator FreezeEnemiesAndSpawning(float duration)
{
    // Find all active enemies in the scene
    Enemy[] enemies = FindObjectsOfType<Enemy>();

    // Stun each enemy
    foreach (var enemy in enemies)
    {
        enemy.isStunned = true;
    }

    // Stop enemy spawning
    EnemySpawner[] spawners = FindObjectsOfType<EnemySpawner>();
    foreach (var spawner in spawners)
    {
        spawner.enabled = false;
    }

    // Wait for the freeze duration
    yield return new WaitForSeconds(duration);

    // Unstun each enemy
    foreach (var enemy in enemies)
    {
        enemy.isStunned = false;
    }

    // Resume enemy spawning
    foreach (var spawner in spawners)
    {
        spawner.enabled = true;
    }
}

public IEnumerator ActivateFullInvincibility(float duration)
{
    isFullyInvincible = true;
    spriteRenderer.sprite = invincibleSprite; // Change to invincible sprite

    // Wait for the duration of full invincibility
    yield return new WaitForSeconds(duration);

    // Revert to normal state
    isFullyInvincible = false;
    spriteRenderer.sprite = normalSprite; // Change back to normal sprite
}





}
