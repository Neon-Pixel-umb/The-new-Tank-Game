using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Pause", menuName = "Items/Active/Pause")]
public class Pause : Item
{
    public float freezeDuration = 3f; // Duration for which enemies are frozen

    public override void ApplyEffect(TankController player)
    {
        SoundManager.Instance.PlayGood();
        player.StartCoroutine(player.FreezeEnemiesAndSpawning(freezeDuration));
    }
}
