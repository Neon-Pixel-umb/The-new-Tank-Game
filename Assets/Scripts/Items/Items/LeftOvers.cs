using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LeftOvers", menuName = "Items/LeftOvers")]
public class LeftOvers : Item
{
    public int healthIncrease = 1;
    public override void ApplyEffect(TankController player)
    {
        SoundManager.Instance.PlayGood();
        player.health += healthIncrease;
        player.LeftOvers = true;
    }
}
