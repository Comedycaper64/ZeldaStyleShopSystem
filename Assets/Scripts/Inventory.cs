using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance {get; private set;}
    private List<InventoryObject> playerInventory;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one Inventory! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        playerInventory = new List<InventoryObject>();
    }

    public void AddToInventory(InventoryObject inventoryObject)
    {
        playerInventory.Add(inventoryObject);
    }

    public List<InventoryObject> GetInventoryObjects()
    {
        return playerInventory;
    }

}
