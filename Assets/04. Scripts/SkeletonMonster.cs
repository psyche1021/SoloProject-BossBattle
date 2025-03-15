using System.Collections;
using UnityEngine;

public class SkeletonMonster : BaseMonster
{
    enum State { Spawn, Idle, Walking, Attack }
    State currentState = State.Spawn;

    float detectionRange = 10f; 
    float attackRange = 5f;
    float distanceToPlayer;

    void Start()
    {
        SetHp(5);
        StartCoroutine(StateMachine());
    }

    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, target.position);
    }

    IEnumerator StateMachine()
    {
        while (!isDead)
        {
            switch (currentState)
            {
                case State.Spawn:
                    yield return StartCoroutine(DoSpawn());
                    break;

                case State.Idle:
                    yield return StartCoroutine(DoIdle());
                    break;

                case State.Walking:
                    yield return StartCoroutine(DoWalk());
                    break;

                case State.Attack:
                    yield return StartCoroutine(DoAttack());
                    break;
            }
        }
    }

    IEnumerator DoSpawn()
    {
        yield return new WaitForSeconds(1.3f);
        currentState = State.Idle;
    }

    IEnumerator DoIdle()
    {
        animator.SetTrigger("doIdle");
        while (currentState == State.Idle)
        {
            if (distanceToPlayer <= attackRange) // 플레이어가 공격 범위 내에 있으면
            {
                currentState = State.Attack; 
            }
            else
            {
                currentState = State.Walking; 
            }
            yield return null;
        }
    }

    protected override IEnumerator DoWalk()
    {
        animator.SetBool("doWalk", true);
        navMeshAgent.isStopped = false;

        while (currentState == State.Walking)
        {
            navMeshAgent.SetDestination(target.position);

            if (distanceToPlayer <= attackRange) 
            {
                animator.SetBool("doWalk", false);
                navMeshAgent.isStopped = true;
                currentState = State.Attack;
                yield break;
            }
            yield return null;
        }
    }

    protected override IEnumerator DoAttack()
    {
        while (currentState == State.Attack)
        {
            yield return StartCoroutine(LookAtPlayer());
            animator.SetBool("doAttack", true);
            yield return new WaitForSeconds(1.6f);
            animator.SetBool("doAttack", false);
            if (distanceToPlayer > attackRange)
            {
                currentState = State.Idle;
                yield break;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}