using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DialogueAddItem", fileName = "NewDialogueAddItem")]
public class DialogueAddItem : ConversationNode
{
    public InventoryObject item;
    public string itemName;
}
