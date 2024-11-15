using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Radar", menuName = "Items/Radar")]
public class Radar : Item
{
    private void OnEnable()
    {
    }

    public override void ApplyEffect(TankController player) // Changed to 'public' to match the base class
    {
        player.radarEnabled = true;
        player.UpdateMiniMap();
        player.AdjustDamage(0.5f);
    }
}
