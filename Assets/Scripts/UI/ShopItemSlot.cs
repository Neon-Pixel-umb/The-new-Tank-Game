using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ShopItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image itemIcon;
    public TMP_Text priceText; // Text to display item price
    public TMP_Text descriptionText;
    private Item item;
    private int price;
    private bool isOnSale = false;
    private bool isPurchased = false;
    private WaveManager waveManager;

    // Setup item in the slot with WaveManager reference
    public void SetItem(Item newItem, int itemPrice, bool sale, WaveManager manager)
    {
        item = newItem;
        price = itemPrice;
        isOnSale = sale;
        waveManager = manager;
        isPurchased = false;

        // Display icon and price
        itemIcon.sprite = item.sprite;
        itemIcon.enabled = true;
        priceText.text = isOnSale ? $"${price}" : $"${price}"; // Show sale price if applicable
        priceText.enabled = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Display item description on hover if not purchased
        if (descriptionText != null && item != null && !isPurchased)
        {
            descriptionText.text = item.description;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Clear description text when hover ends
        if (descriptionText != null)
        {
            descriptionText.text = "";
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isPurchased || item == null) return;  // Do nothing if already purchased or no item assigned

        // Check if the player has enough coins
        TankController playerController = FindObjectOfType<TankController>();
        if (playerController.coins >= price)
        {
            playerController.coins -= price; // Deduct price from player's coins
            playerController.AddItem(item);  // Add the item to player's inventory
            SoundManager.Instance.PlayRegister();
            playerController.UpdateCoinDisplay();
            playerController.UpdateHealthDisplay();
            isPurchased = true;

            // Update UI to reflect purchase
            itemIcon.enabled = false;
            priceText.enabled = false;

            Debug.Log($"{item.itemName} purchased for ${price}");
        }
        else
        {
            Debug.Log("Not enough coins to purchase this item");
        }
    }
}
