using UnityEngine;

public class SubStateMachineBehaviour : StateMachineBehaviour
{
    [SerializeField] string m_triggerName;
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(m_triggerName);
    }
}
