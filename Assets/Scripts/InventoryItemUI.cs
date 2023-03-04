using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] private Image itemSprite;
    [SerializeField] private TextMeshProUGUI numberText;

    public void SetNumberText(int number)
    {
        numberText.text = number.ToString();
    }

    public void SetSprite(Sprite sprite)
    {
        itemSprite.sprite = sprite;
    }
    
}
