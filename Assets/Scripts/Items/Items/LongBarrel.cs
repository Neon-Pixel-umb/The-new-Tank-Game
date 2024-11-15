using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "LongBarrel", menuName = "Items/LongBarrel")]
public class LongBarrel : Item
{
    public override void ApplyEffect(TankController player)
    {
        player.AdjustFireRate(3f);
        player.bulletSpeed += 1f; // Ensure shot speed has an upper limit in TankController
    }
}
