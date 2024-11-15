using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ImprovedAmmo", menuName = "Items/ImprovedAmmo")]
public class ImprovedAmmo : Item
{
    public override void ApplyEffect(TankController player)
    {
        player.AdjustDamage(1.5f);
    }
}