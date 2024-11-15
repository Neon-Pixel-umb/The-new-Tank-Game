using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FlashBang", menuName = "Items/Active/FlashBang")]
public class FlashBang : Item
{
    public float pushbackForce = 500f; // Adjustable pushback force
    public float stunDuration = 2f;    // Duration of stun effect
    public float effectRadius = 5f;    // Radius within which enemies are affected

    public override void ApplyEffect(TankController player)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(player.transform.position, effectRadius, LayerMask.GetMask("Enemy"));
        SoundManager.Instance.PlayFlash();
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Apply damage
                enemy.TakeDamage(player.damage * 2);

                // Apply pushback force
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    Vector2 pushDirection = (enemyRb.position - (Vector2)player.transform.position).normalized;
                    enemyRb.AddForce(pushDirection * pushbackForce);
                }

                // Request TankController to start the stun coroutine
                player.StartEnemyStun(enemy, stunDuration);
            }
        }

        Debug.Log("FlashBang activated, nearby enemies damaged and stunned.");
    }
}
