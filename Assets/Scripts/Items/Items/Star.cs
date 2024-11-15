using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Star", menuName = "Items/Active/Star")]
public class Star : Item
{
    public float invincibilityDuration = 5f; // Duration of invincibility

    public override void ApplyEffect(TankController player)
    {
        SoundManager.Instance.PlayGood();
        player.StartCoroutine(player.ActivateFullInvincibility(invincibilityDuration));
    }
}
