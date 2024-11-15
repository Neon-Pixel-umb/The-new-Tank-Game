using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MegaBullet", menuName = "Items/MegaBullet")]
public class MegaBullet : Item
{
    public override void ApplyEffect(TankController player)
    {
        float damageIncrease = Mathf.Max(player.damage * 0.5f, 30f);
        player.AdjustDamage(damageIncrease);
        player.AdjustFireRate(-2f);
    }
}
