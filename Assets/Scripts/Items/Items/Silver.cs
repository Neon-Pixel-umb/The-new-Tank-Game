using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Silver", menuName = "Items/Silver")]
public class Silver : Item
{
    public override void ApplyEffect(TankController player)
    {
        player.AddCoins(250);
    }
}
