using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "InventoryObject", fileName = "NewInventoryObject")]
public class InventoryObject : ScriptableObject
{
   public string objectName;

   [TextArea(3, 10)]
   public string objectDescription;
   public Sprite objectImage;
}
