using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Toolbox", menuName = "Items/Toolbox")]
public class Toolbox : Item
{
    // Start is called before the first frame update
    public int healthIncrease = 2;


    public override void ApplyEffect(TankController player) // Changed to 'public' to match the base class
    {
        player.health += healthIncrease;
    }
}
