using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance {get; private set;}
    private List<InventoryObject> playerInventory;
    private List<int> inventoryItemNumber;
    private int playerCurrency;
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private Transform inventoryUI;
    [SerializeField] private GameObject inventoryItemUI;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one Inventory! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        playerCurrency = 100;
        playerInventory = new List<InventoryObject>();
        inventoryItemNumber = new List<int>();
        foreach(Transform child in inventoryUI)
        {
            Destroy(child.gameObject);
        }
    }

    private void UpdateCurrencyUI()
    {
        currencyText.text = "Coins: " + playerCurrency;
    }

    public int GetPlayerCurrency()
    {
        return playerCurrency;
    }

    public void SpendPlayerCurrency(int currency)
    {
        playerCurrency -= currency;
        UpdateCurrencyUI();
    }

    public void AddToInventory(InventoryObject inventoryObject)
    {
        if (playerInventory.Contains(inventoryObject))
        {
            inventoryItemNumber[playerInventory.IndexOf(inventoryObject)]++;
        }
        else
        {
            playerInventory.Add(inventoryObject);
            inventoryItemNumber.Add(1);
        }
        UpdateInventoryUI();
    }

    private void UpdateInventoryUI()
    {
        foreach(Transform child in inventoryUI)
        {
            Destroy(child.gameObject);
        }

        foreach(InventoryObject inventoryObject in playerInventory)
        {
            InventoryItemUI itemUI = Instantiate(inventoryItemUI, inventoryUI).GetComponent<InventoryItemUI>();
            itemUI.SetNumberText(inventoryItemNumber[playerInventory.IndexOf(inventoryObject)]);
            itemUI.SetSprite(inventoryObject.objectImage);
        }        
    }

    public List<InventoryObject> GetInventoryObjects()
    {
        return playerInventory;
    }

}
