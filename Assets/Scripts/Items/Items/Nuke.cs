using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuke", menuName = "Items/Active/Nuke")]
public class Nuke : Item
{
    public Nuke()
    {
        itemName = "Nuke";
        category = ItemCategory.Attack;
        rarity = ItemRarity.Rare;
        isActiveItem = true;
        rechargeRequirement = 500;
    }

    public override void ApplyEffect(TankController player)
    {
        float nukeDamage = player.damage * 5; // Damage is 5 times the player’s current damage
        SoundManager.Instance.PlayNuke();
        // Damage all enemies in the scene
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.TakeDamage(nukeDamage);
        }
    }
}
