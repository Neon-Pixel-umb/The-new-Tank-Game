using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public GameObject tooltipPanel;
    public TMP_Text tooltipText;

    private void Start()
    {
        HideTooltip();
    }

    private void Update()
    {
        if (tooltipPanel.activeSelf)
        {
            tooltipPanel.transform.position = Input.mousePosition;
        }
    }

    public void ShowTooltip(string description)
    {
        tooltipText.text = description;
        tooltipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}
