using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image itemIcon;
    public TMP_Text itemNameText;
    public TMP_Text descriptionText;
    private Item item;
    private WaveManager waveManager;
    private bool isPicked = false;

    public void SetItem(Item newItem, WaveManager manager)
    {
        item = newItem;
        waveManager = manager;
        isPicked = false;

        // Reset icon and text visibility when setting a new item
        itemIcon.sprite = item.sprite;
        itemIcon.enabled = true;  // Ensure icon is visible
        itemNameText.text = item.itemName;
        itemNameText.enabled = true;  // Ensure name is visible
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (descriptionText != null && item != null && !isPicked)
        {
            descriptionText.text = item.description;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (descriptionText != null)
        {
            descriptionText.text = "";
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isPicked || item == null) return;  // Do nothing if already picked or no item assigned

        FindObjectOfType<TankController>().AddItem(item);
        isPicked = true;

        // Hide icon and text after item is picked
        itemIcon.enabled = false;
        itemNameText.enabled = false;
        waveManager.ItemPicked();  // Notify WaveManager of the selection
    }
}
