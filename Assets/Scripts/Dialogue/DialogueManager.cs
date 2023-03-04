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
    [SerializeField] private ConversationNode shoppingNode;
    [SerializeField] private ConversationNode purchaseNode;
    [SerializeField] private ConversationNode notEnoughCoinsNode;
	[SerializeField] private float timeBetweenLetterTyping;

	// TRACKERS
	private Conversation currentConversation;
	public Dialogue currentDialogue;
    private Dialogue defaultBrowsingDialogue;
    
    private InventoryObject selectedInventoryObject;
	public string currentSentence;
	public bool inConversation = false;
	public bool inShoppingMode = false;
    public bool hoveringOverItem = false;
    public bool purchasing = false; 
	private bool currentTextTyping = false;
    private ShopItem spinningShopItem;
	private Coroutine typingCoroutine;
	private Queue<ConversationNode> conversationNodes;
	private Queue<string> sentences;
    private int itemLayerMask;


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
        itemLayerMask = 1 << 6;
    }

	void Start()
	{
		InputReader.Instance.InteractEvent += OnInteract;
		InputReader.Instance.LeaveEvent += OnLeave;
	}

    private void Update() 
    {
        if (inShoppingMode)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit, 100f, itemLayerMask))
            {
                if (hit.collider.gameObject.TryGetComponent<ShopItem>(out ShopItem item))
                {
                    StopCoroutine(typingCoroutine);
                    item.TryStartItemDialogue();
                    spinningShopItem = item;
                }
            }
            else
            {
                SetItemText();
                if (spinningShopItem)
                {
                    spinningShopItem.StopSpinning();
                    spinningShopItem = null;
                }
                hoveringOverItem = false;
            }
        }

        if (inShoppingMode && !hoveringOverItem && (currentDialogue != defaultBrowsingDialogue))
        {
            StartDialogue(defaultBrowsingDialogue);
        }
    }
 
	private void OnInteract()
	{
        if (purchasing)
        {
            if (Inventory.Instance.GetPlayerCurrency() < selectedInventoryObject.objectPrice)
            {
                StartDialogue(notEnoughCoinsNode);
            }
            else
            {
                Inventory.Instance.SpendPlayerCurrency(selectedInventoryObject.objectPrice);
                Inventory.Instance.AddToInventory(selectedInventoryObject);
                StartDialogue(purchaseNode);
            }
            purchasing = false;
            conversationNodes.Enqueue(shoppingNode);
        }
        else if (hoveringOverItem)
        {
            TryPurchaseItem(selectedInventoryObject);
        }
        else if (inConversation && currentDialogue != null)
		{
			if (!currentTextTyping && !inShoppingMode)
			{
				DisplayNextSentence();
			}
			else
			{
				FinishTypingSentence();
			}
		}
	}

    private void TryPurchaseItem(InventoryObject selectedInventoryObject)
    {
        inShoppingMode = false;
        hoveringOverItem = false;
        purchasing = true;
        currentSentence = "Would you like to purchase the " + selectedInventoryObject.objectName + "? \n YES - [Spacebar]   NO - [Tab]";
		typingCoroutine = StartCoroutine(TypeSentence(currentSentence));
    }

    private void OnLeave()
    {
        if (purchasing)
        {
            purchasing = false;
            //conversationNodes.Enqu
            StartDialogue(shoppingNode);
        }
        else
        {
            ExitConversation();
        }
    }

    public void SetItemText()
	{
		itemText.text = "";
	}

	public void SetItemText(string itemName)
	{
		itemText.text = itemName;	
	}

    public void SetSelectedItem(InventoryObject inventoryObject)
    {
        selectedInventoryObject = inventoryObject;
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
        inShoppingMode = false;
		dialogueAnimator.SetTrigger("startConversation");
		currentConversation = conversation;
        exitConversationButton.gameObject.SetActive(true);
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
            case "DialogueEvent":
                DialogueEvent dialogueEvent = (DialogueEvent)conversationNode;
                if (dialogueEvent.enableShoppingMode)
                {
                    EnableShoppingMode(dialogueEvent.shoppingModeDialogue);
                }
                else
                {
                    DisableShoppingMode();
                }
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

    private void EnableShoppingMode(Dialogue browseDialogue)
    {
        defaultBrowsingDialogue = browseDialogue;
        StartDialogue(defaultBrowsingDialogue);
        inShoppingMode = true;

        //Clears dialogue
        //goes into a state where mousing over an item makes it rotate + brings up it's name and description
    }

    // private void TypeBrowsingDialogue()
    // {
    //     currentSentence = defaultBrowsingDialogue.sentences[0];
    //     typingCoroutine = StartCoroutine(TypeSentence(currentSentence));
    // }

    private void DisableShoppingMode()
    {
        inShoppingMode = false;
        defaultBrowsingDialogue = null;
    }

    //something like Present Item Options?

    public void SetSentence(string sentence)
    {
        dialogueText.text = sentence;
    }

	public void DisplayNextSentence()
	{
		//SetItemText(currentDialogue);
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

	public void ExitConversation()
	{   
        DisableShoppingMode();
		EndConversation();
	}

	public void EndDialogue()
	{		
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
        exitConversationButton.gameObject.SetActive(false);
		dialogueAnimator.SetTrigger("endConversation");
			StartCoroutine(DialogueCooldown());
	}

	private IEnumerator DialogueCooldown()
	{
		yield return new WaitForEndOfFrame();
		inConversation = false;
	}

}
