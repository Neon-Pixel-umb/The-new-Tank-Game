using UnityEngine;

public class SquareEnemy : Enemy
{
    protected override void ApplyCollisionEffect(TankController playerController)
    {
        playerController.TakeDamage(1, 0.7f); // Basic damage
        Destroy(gameObject);
    }
}
