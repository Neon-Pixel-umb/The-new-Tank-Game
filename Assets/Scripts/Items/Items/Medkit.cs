using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Medkit", menuName = "Items/Active/Medkit")]
public class Medkit : Item
{

    public int healthIncrease = 1;
    public override void ApplyEffect(TankController player)
    {
        SoundManager.Instance.PlayGood();
        player.health += healthIncrease;
    }

    
}
