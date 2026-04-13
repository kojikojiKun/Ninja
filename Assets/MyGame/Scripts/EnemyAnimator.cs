using UnityEngine;

public class EnemyAnimator
{
    private Animator m_animator;

    public EnemyAnimator(Animator animator)
    {
        m_animator = animator;
    }

   public void Assassinate(AssassinateDirection dir)
    {
        m_animator.SetInteger("AssassinateDirection", (int)dir);
        m_animator.SetTrigger("Killed");
    }
}
