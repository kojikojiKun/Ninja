using UnityEngine;

public class PlayerAnimator
{
    private Animator m_animator;

    public PlayerAnimator(Animator animator)
    {
        m_animator = animator;
    }

    public void Assassinate(AssassinateDirection direction, bool isSuccess)
    {
        if (!isSuccess)
            return;

        m_animator.SetInteger("AssassinateDirection", (int)direction);
        m_animator.SetTrigger("Assassinate");
    }
}
