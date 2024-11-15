using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SharpPoint", menuName = "Items/SharpPoint")]
public class SharpPoint : Item
{
    public override void ApplyEffect(TankController player)
    {
        // Increase damage by 3
        player.AdjustDamage(3f);
        
        // Enable piercing for bullets
        player.hasPiercingBullets = true;
    }
}
