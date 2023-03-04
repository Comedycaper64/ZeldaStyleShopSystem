using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
	[SerializeField] private Conversation conversation;
	private bool inTalkingRange = false;

	private void Start() 
	{
		InputReader.Instance.InteractEvent += TriggerDialogue;
	}

    private void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space) && inTalkingRange)
		{
			TriggerDialogue();
		}
    }

    private void OnTriggerEnter(Collider other)
    {
		if (other.CompareTag("Player"))
		{
			inTalkingRange = true;
			DialogueManager.Instance.ShowInteractText();
		}
    }

    private void OnTriggerExit(Collider other)
    {
		if (other.CompareTag("Player"))
		{
			inTalkingRange = false;
			DialogueManager.Instance.CloseInteractText();
		}
	}

    public void TriggerDialogue()
	{
		if (!DialogueManager.Instance.inConversation && inTalkingRange)
		{
			DialogueManager.Instance.StartConversation(conversation);
			DialogueManager.Instance.CloseInteractText();
		}
	}
}
