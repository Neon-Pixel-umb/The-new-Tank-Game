using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gear", menuName = "Items/Gear")]
public class Gear : Item
{
     public int healthIncrease = 1;

    private void OnEnable()
    {
        category = ItemCategory.Health;
        name = "Gear";  // Sets the ScriptableObject name when it is loaded
        description = "Heals one heart";
        rarity = ItemRarity.Common;
    }

    public override void ApplyEffect(TankController player) // Changed to 'public' to match the base class
    {
        player.health += healthIncrease;
    }
}
