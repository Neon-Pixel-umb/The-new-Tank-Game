using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Oil", menuName = "Items/Oil")]
public class Oil : Item
{
    public override void ApplyEffect(TankController player)
    {
        player.AdjustFireRate(3f);
        player.maxSpeed += 1f; // Ensure you have logic to cap speed in TankController
    }
}
