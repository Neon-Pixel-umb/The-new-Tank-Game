using UnityEngine;

public class DiamondEnemy : Enemy
{
    protected override void Start()
    {
        coinValue = 2;    // Rewards more coins on death
        base.Start();
    }

    protected override void ApplyCollisionEffect(TankController playerController)
    {
        playerController.TakeDamage(2, 1.5f); // Deals more damage to the player
        Destroy(gameObject);
    }
}
