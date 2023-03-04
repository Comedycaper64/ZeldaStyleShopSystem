using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    [SerializeField] private float movementSpeed;

    private void Awake() 
    {
        controller = GetComponent<CharacterController>();    
    }

    private void Update() 
    {
        Vector2 movementValue = InputReader.Instance.MovementValue;
        if (DialogueManager.Instance.inConversation) 
        {
            movementValue = new Vector2(0,0);
        }        
        controller.Move(new Vector3(movementValue.x, 0, movementValue.y) * movementSpeed * Time.deltaTime);
    }
}
