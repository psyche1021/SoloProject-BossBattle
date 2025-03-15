using UnityEngine;

public class AttackReset : StateMachineBehaviour
{ 
    [SerializeField] string triggerName;

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        animator.ResetTrigger(triggerName);
    }
}
