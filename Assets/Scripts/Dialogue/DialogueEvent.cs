using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DialogueEvent", fileName = "NewDialogueEvent")]
public class DialogueEvent : ConversationNode
{
    public bool enableShoppingMode;
    public Dialogue shoppingModeDialogue;
}
