using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    [SerializeField] InventoryObject inventoryObject;
    [SerializeField] GameObject itemModel;
    private bool spin = false;

    private void Update() 
    {
        if (spin)
        {
            itemModel.transform.Rotate(new Vector3(0, 100, 0) * Time.deltaTime);    
        }
    }

    public void TryStartItemDialogue()
    {
        if (!DialogueManager.Instance.hoveringOverItem)
        {
            DialogueManager.Instance.hoveringOverItem = true;
            DialogueManager.Instance.SetItemText(inventoryObject.objectName);
            DialogueManager.Instance.SetSentence(inventoryObject.objectDescription);
            DialogueManager.Instance.SetSelectedItem(inventoryObject);
            DialogueManager.Instance.currentDialogue = null;
            spin = true;
        }
    }

    public void StopSpinning()
    {
        spin = false;
    }
}
