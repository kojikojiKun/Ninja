using UnityEngine;

public class PlayerAnimation
{
    private Animator m_animator;

    public PlayerAnimation(Animator animator)
    {
        m_animator = animator;
    }

    public void SetMultiplier(float moveSpeedRatio)
    {
        m_animator.SetFloat("SpeedMultiplier", moveSpeedRatio);
    }

    public void MoveAnimation(Vector2 moveInput,PlayerMoveState state,float currentSpeed)
    {
        //Idle[0],Walk[1],Run[2],Crouch[3],CrouchIdle[4]
        m_animator.SetInteger("MoveState", (int)state);

        m_animator.SetFloat("Input_X", moveInput.x);
        m_animator.SetFloat("Input_Y", moveInput.y);
    }

    public void StartTurn(bool isTurning)
    {
        m_animator.SetBool("IsTurning", isTurning);
    }
}
