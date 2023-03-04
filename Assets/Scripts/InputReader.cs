using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, Controls.IPlayerActions
{
    public static InputReader Instance {get; private set;}
    public Vector2 MovementValue {get; private set;}
    public event Action InteractEvent;
    public event Action LeaveEvent;

    private Controls controls;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one InputReader! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() 
    {
        controls = new Controls();
        controls.Player.SetCallbacks(this);

        controls.Player.Enable();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) {return;}
        
        InteractEvent?.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MovementValue = context.ReadValue<Vector2>();
    }

    public void OnLeave(InputAction.CallbackContext context)
    {
        if (!context.performed) {return;}
        
        LeaveEvent?.Invoke();
    }
}
