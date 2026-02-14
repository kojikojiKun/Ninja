using UnityEngine;
using UnityEngine.InputSystem;

public class ReadPlayerInput: MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public bool IsRunPressed { get; private set; }
    public bool IsCrouchPressed { get; private set; }
    public bool IsJumpPressed { get; private set; }

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
        {
            IsCrouchPressed = true;
        }
        else if (context.canceled)
        {
            IsCrouchPressed = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsJumpPressed = true;
        }
        else if (context.canceled)
        {
            IsJumpPressed = false;
        }
    }
}
