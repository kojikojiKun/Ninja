using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class ReadPlayerInput : MonoBehaviour
{
    public event Action OnCrouchPressed;
    public event Action OnJumpPressed;
    public event Action OnAttackPressed;
    public event Action OnAssassinated;
    public event Action<bool> OnRunPressing;

    private Vector2 m_moveInput;

    public void OnMove(InputAction.CallbackContext context)
    {
        m_moveInput = context.ReadValue<Vector2>();
    }

    public Vector2 GetMoveInput()
    {
        return m_moveInput;
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
            OnRunPressing?.Invoke(true);
        else if (context.canceled)
            OnRunPressing?.Invoke(false);
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

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
            OnAttackPressed?.Invoke();
    }

    public void OnAssassinate(InputAction.CallbackContext context)
    {
        if (context.started)
            OnAssassinated?.Invoke();

    }
}
