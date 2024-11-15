using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hammer", menuName = "Items/Hammer")]
public class Hammer : Item
{
    public int healthIncrease = 1;

    private void OnEnable()
    {
        category = ItemCategory.Health;
        name = "Hammer";  // Sets the ScriptableObject name when it is loaded
        description = "Heals one heart";
        rarity = ItemRarity.Common;
        ID = 1;
    }

    public override void ApplyEffect(TankController player) // Changed to 'public' to match the base class
    {
        player.health += healthIncrease;
    }

}
