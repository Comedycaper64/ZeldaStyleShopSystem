using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(menuName = "Dialogue", fileName = "NewDialogue")]
public class Dialogue : ConversationNode
{
	[TextArea(3, 10)]
	public string[] sentences;
}
