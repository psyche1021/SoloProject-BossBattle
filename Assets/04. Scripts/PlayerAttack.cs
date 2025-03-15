using UnityEngine;

public class PlayerAttack : BaseAttack
{
    Animator animator;
    int hashAttackCount = Animator.StringToHash("AttackCount");

    public bool IsAttacking { get; set; }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public int AttackCount
    {
        get => animator.GetInteger(hashAttackCount);
        set => animator.SetInteger(hashAttackCount, value);
    }

    public override void Attack()
    {
        IsAttacking = true;
        animator.SetTrigger("Attack");
        AttackCount = 0;
    }

    public override void Skill()
    {
        IsAttacking = true;
        animator.SetTrigger("Attack");
        AttackCount = 1;
    }

    public void OnAttackAnimationEnd()
    {
        IsAttacking = false;
    }
}
