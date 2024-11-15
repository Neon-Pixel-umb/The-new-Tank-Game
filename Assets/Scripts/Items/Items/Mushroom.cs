using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Mushroom", menuName = "Items/Mushroom")]
public class Mushroom : Item
{
    public override void ApplyEffect(TankController player)
    {
        player.health += 1; // Assuming max health is handled elsewhere
        player.AdjustDamage(5f);
    }
}
