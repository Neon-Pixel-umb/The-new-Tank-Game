using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleEnemy : Enemy
{
    protected override void Start()
    {
        speed = 300; // Higher Speed
        base.Start();
    }

    protected override void ApplyCollisionEffect(TankController playerController)
    {
        playerController.TakeDamage(1, 0.3f); // Deals more damage to the player
        Destroy(gameObject);
    }
}
