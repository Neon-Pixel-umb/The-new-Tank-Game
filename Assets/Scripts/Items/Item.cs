using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemCategory
{
    Health,
    Attack,
    Movement,
    Shop,
    Utility
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Legendary
}


public abstract class Item : ScriptableObject
{
    public string itemName;
    public Sprite sprite; // Item's visual representation
    public ItemCategory category;
    public ItemRarity rarity;
    public bool isActiveItem;
    public float rechargeRequirement;
    protected float rechargeProgress;
    protected int ID;
    public string description; 
    protected bool canRecharge; // Prevents recharge immediately after activation

    private float _rechargeProgress; // Private field for recharge progress

    // Method for activating the item, if it's an active item
    public virtual void Activate(TankController player)
    {
        if (isActiveItem && rechargeProgress >= rechargeRequirement)
        {
            ApplyEffect(player);
            rechargeProgress = rechargeRequirement; // Reset recharge after use
            canRecharge = false;
            player.StartCoroutine(RechargeDelay());
        }
    }

    public float RechargeProgress // Public property for accessing rechargeProgress
    {
        get { return _rechargeProgress; }
        set { _rechargeProgress = Mathf.Clamp(value, 0, rechargeRequirement); }
    }

    // Method to increase the recharge based on player actions
    public void IncreaseRecharge(float amount)
    {
        // Decrease rechargeProgress toward zero
        RechargeProgress -= amount;
    }

    private IEnumerator RechargeDelay()
    {
        yield return new WaitForSeconds(0.5f); // Delay before recharge starts
        canRecharge = true;
    }

    // Abstract method for applying the item's effect, to be defined in derived classes
    public abstract void ApplyEffect(TankController player);
}