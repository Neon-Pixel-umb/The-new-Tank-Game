using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BoxOfAmmo", menuName = "Items/BoxOfAmmo")]
public class BoxOfAmmo : Item
{
    public override void ApplyEffect(TankController player)
    {
        player.AdjustFireRate(1.5f);
    }
}
