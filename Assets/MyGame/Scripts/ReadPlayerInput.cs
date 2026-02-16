using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class ReadPlayerInput : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public bool IsRunPressed { get; private set; }

    public event Action OnCrouchPressed;
    public event Action OnJumpPressed;

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsRunPressed = true;
        }
        else if (context.canceled)
        {
            IsRunPressed = false;
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started)
            OnCrouchPressed?.Invoke();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
            OnJumpPressed?.Invoke();
    }
}
