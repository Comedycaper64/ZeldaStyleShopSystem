using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    [SerializeField] InventoryObject inventoryObject;

    public void TryStartItemDialogue()
    {
        if (!DialogueManager.Instance.hoveringOverItem)
        {
            DialogueManager.Instance.hoveringOverItem = true;
            DialogueManager.Instance.SetItemText(inventoryObject.objectName);
            DialogueManager.Instance.SetSentence(inventoryObject.objectDescription);
            DialogueManager.Instance.SetSelectedItem(inventoryObject);
            DialogueManager.Instance.currentDialogue = null;
        }
    }
}
