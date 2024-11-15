using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TinyBullets", menuName = "Items/TinyBullets")]
public class TinyBullets : Item
{
    public override void ApplyEffect(TankController player)
    {
        player.AdjustFireRate(15f);
        player.AdjustDamage(-2f); // Decrease damage
    }
}