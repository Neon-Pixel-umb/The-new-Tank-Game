using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleEnemy : Enemy
{
    public float stunDuration = 1.5f; // Duration of the stun effect

    protected override void Start()
    {
        speed = 500f;               // Twice the speed of a standard enemy
        coinValue = 1;              // Default coin value for this enemy type
        base.Start();
    }

    protected override void ApplyCollisionEffect(TankController playerController)
    {
        playerController.ApplyStun(stunDuration); // Apply stun effect on collision
        Destroy(gameObject);
    }
}

