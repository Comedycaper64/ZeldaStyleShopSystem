using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DialogueManager : MonoBehaviour
{
	public static DialogueManager Instance {get; private set;}

	// SERIALIZABLES
	[SerializeField] private TextMeshProUGUI dialogueText;
	[SerializeField] private TextMeshProUGUI itemText;
	[SerializeField] private TextMeshProUGUI interactText;
	[SerializeField] private Animator dialogueAnimator;
	[SerializeField] private AudioSource dialogueAudioSource;
	[SerializeField] private Transform exitConversationButton;
    [SerializeField] private GameObject shopCamera;
	[SerializeField] private float timeBetweenLetterTyping;

	// TRACKERS
	private Conversation currentConversation;
	private Dialogue currentDialogue;
	private string currentSentence;
	public bool inConversation = false;
	private bool currentTextTyping = false;
	private Coroutine typingCoroutine;
	private Queue<ConversationNode> conversationNodes;
	private Queue<string> sentences;


	private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one DialogueManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

		sentences = new Queue<string>();
		conversationNodes = new Queue<ConversationNode>();
    }

	void Start()
	{
		InputReader.Instance.InteractEvent += OnInteract;
	}

	private void OnInteract()
	{
		if (inConversation && currentDialogue != null)
		{
			if (!currentTextTyping)
			{
				DisplayNextSentence();
			}
			else
			{
				FinishTypingSentence();
			}
		}
	}

    // public void SetItemText()
	// {
	// 	itemText.text = "";
	// }

	public void SetItemText(Dialogue dialogue)
	{
		//itemText.text = dialogue.characterName;		
	}

	public void ShowInteractText()
	{
		interactText.enabled = true;
	}

	public void CloseInteractText()
	{
		interactText.enabled = false;
	}


	public void StartConversation(Conversation conversation)
	{
        shopCamera.SetActive(true);
		inConversation = true;
		dialogueAnimator.SetTrigger("startConversation");
		currentConversation = conversation;
		conversationNodes.Clear();
		dialogueAudioSource.volume = SoundManager.Instance.GetSoundEffectVolume() / 2;
		foreach (ConversationNode conversationNode in currentConversation.conversationNodes)
		{
			conversationNodes.Enqueue(conversationNode);
		}
		StartDialogue(conversationNodes.Dequeue());
	}

	public void StartDialogue(ConversationNode conversationNode)
	{
		string nodeType = conversationNode.GetType().ToString();

		switch(nodeType)
		{
			case "DialogueAddItem":
				currentDialogue = ScriptableObject.CreateInstance<Dialogue>();
				AddItem((DialogueAddItem)conversationNode);
				break;

			default:
			case "Dialogue":
				currentDialogue = (Dialogue)conversationNode;
				sentences.Clear();
				foreach (string sentence in currentDialogue.sentences)
				{
					sentences.Enqueue(sentence);
				}

				DisplayNextSentence();
				break;
		}
	}

	private void AddItem(DialogueAddItem dialogueAddItem)
	{
		if (!Inventory.Instance.GetInventoryObjects().Contains(dialogueAddItem.item))
		{
			Inventory.Instance.AddToInventory(dialogueAddItem.item);
			currentSentence = "Item purchased: " + dialogueAddItem.itemName;
			typingCoroutine = StartCoroutine(TypeSentence(currentSentence));
		}
		else
		{
			currentSentence = dialogueAddItem.itemName + " already in inventory";
			typingCoroutine = StartCoroutine(TypeSentence(currentSentence));
		}
	}

    //something like Present Item Options?


	public void DisplayNextSentence()
	{
		SetItemText(currentDialogue);
		if (sentences.Count == 0)
		{
			EndDialogue();
			return;
		}

		string sentence = sentences.Dequeue();

		if (typingCoroutine != null)
			StopCoroutine(typingCoroutine);
		typingCoroutine = StartCoroutine(TypeSentence(sentence));
	}

	IEnumerator TypeSentence(string sentence)
	{
		currentSentence = sentence;
		currentTextTyping = true;
		dialogueText.text = "";
		foreach (char letter in sentence.ToCharArray())
		{
			dialogueText.text += letter;
			dialogueAudioSource.Play();
			yield return new WaitForSeconds(timeBetweenLetterTyping);
		}
		currentTextTyping = false;
	}

	private void FinishTypingSentence()
    {
        if (typingCoroutine != null)
			StopCoroutine(typingCoroutine);
		dialogueText.text = currentSentence;
		currentTextTyping = false;
    }

	public void ExitConversationButton()
	{
		exitConversationButton.gameObject.SetActive(false);
		EndDialogue();
	}

	public void EndDialogue()
	{		
		//SetItemText();
		StartCoroutine(TypeSentence(""));
		
		if (conversationNodes.TryDequeue(out ConversationNode nextNode))
		{
			StartDialogue(nextNode);
		}
		else
		{
			EndConversation();
		}
	}

	private void EndConversation()
	{
        shopCamera.SetActive(false);
		dialogueAnimator.SetTrigger("endConversation");
			StartCoroutine(DialogueCooldown());
	}

	private IEnumerator DialogueCooldown()
	{
		yield return new WaitForEndOfFrame();
		inConversation = false;
	}

}
