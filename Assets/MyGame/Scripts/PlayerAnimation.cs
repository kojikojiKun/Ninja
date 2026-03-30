using UnityEngine;

public class PlayerAnimation
{
    private Animator m_animator;
    private bool m_isTurning;

    public PlayerAnimation(Animator animator)
    {
        m_animator = animator;
    }

    public void SetMultiplier(float moveSpeedRatio)
    {
        m_animator.SetFloat("SpeedMultiplier", moveSpeedRatio);
    }

    public void SetMoveParameters(Vector2 moveInput,PlayerMoveState state,float currentSpeed)
    {
        m_animator.SetBool("IsTurning", m_isTurning);

        m_animator.SetBool("IsMoving", currentSpeed > 0.01f);

        //Stop[0],Walk[1],Run[2],Crouch[3]
        m_animator.SetInteger("MoveState", (int)state);

        m_animator.SetFloat("Input_X", moveInput.x);
        m_animator.SetFloat("Input_Y", moveInput.y);
    }
}
