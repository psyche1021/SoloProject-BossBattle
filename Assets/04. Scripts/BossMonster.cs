using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossMonster : BaseMonster
{
    enum State { Rest, Think, Walk, Spell }
    State currentState = State.Rest;
    public GameObject missilePrefab;
    public Transform missilePortA;
    public Transform missilePortB;
    public GameObject skeletonPrefab;
    public Transform skeletonPortA;
    public Transform skeletonPortB;
    public GameObject effectPrefab;
    float hpPercent;
    public Slider slider;
    float distanceToPlayer;

    void Start()
    {
        SetHp(30);
        StartCoroutine(StateMachine()); 
    }

    void Update()
    {
        hpPercent = CurrentHp / MaxHp;
        slider.value = Mathf.Lerp(slider.value, hpPercent, Time.deltaTime * 10);
        distanceToPlayer = Vector3.Distance(transform.position, target.position);
    }

    IEnumerator StateMachine()
    {
        while (!isDead)
        {
            yield return StartCoroutine(LookAtPlayer());

            switch (currentState)
            {
                case State.Rest:
                    yield return StartCoroutine(DoRest());
                    break;
                case State.Think:
                    yield return StartCoroutine(DoThink());
                    break;
                case State.Walk:
                    yield return StartCoroutine(DoWalk());
                    break;
                case State.Spell:
                    yield return StartCoroutine(DoSpell());
                    break;
            }
        }
    }

    protected override IEnumerator DoRest()
    {
        Debug.Log("휴식 중...");
        yield return StartCoroutine(LookAtPlayer());
        animator.SetTrigger("doRest");
        yield return new WaitForSeconds(2f);
        currentState = State.Think;
    }

    protected override IEnumerator DoThink()
    {
        Debug.Log("생각 중...");
        yield return StartCoroutine(LookAtPlayer());
        animator.SetTrigger("doThink");
        yield return new WaitForSeconds(2f);

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        currentState = distanceToPlayer > 25f ? State.Walk : State.Spell;
    }

    protected override IEnumerator DoWalk()
    {
        Debug.Log("추적 중...");
        yield return StartCoroutine(LookAtPlayer());
        animator.SetBool("doWalk", true);
        navMeshAgent.isStopped = false;

        while (true)
        {
            navMeshAgent.SetDestination(target.position);

            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance || distanceToPlayer <= 20f)
            {
                Debug.Log("추적 종료");
                animator.SetBool("doWalk", false);
                navMeshAgent.isStopped = true;
                currentState = State.Think;
                yield break;
            }

            yield return null;
        }
    }

    protected override IEnumerator DoSpell()
    {
        Debug.Log("스펠 시전 중...");
        yield return StartCoroutine(LookAtPlayer());
        effectPrefab.SetActive(true);
        bool useSpell = Random.Range(0, 2) == 0;
        animator.SetInteger("doSpell", useSpell ? 1 : 2);
        yield return new WaitForSeconds(1.0f);

        switch (useSpell)
        {
            case true:
                yield return StartCoroutine(SpellFirst());
                break;

            case false:
                yield return StartCoroutine(SpellSecond());
                break;
        }

        yield return new WaitForSeconds(1.0f);
        effectPrefab.SetActive(false);
        animator.SetInteger("doSpell", 0);
        currentState = State.Rest;
    }

    IEnumerator SpellFirst()
    {
        GameObject instantMissileA = Instantiate(missilePrefab, missilePortA.position, missilePortA.rotation);
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target;

        yield return new WaitForSeconds(2.0f);

        GameObject instantMissileB = Instantiate(missilePrefab, missilePortB.position, missilePortB.rotation);
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;
    }

    IEnumerator SpellSecond()
    {
        GameObject instantSkeletonA = Instantiate(skeletonPrefab, skeletonPortA.position, skeletonPortA.rotation);
        SkeletonMonster skmA = instantSkeletonA.GetComponent<SkeletonMonster>();
        skmA.target = target;

        GameObject instantSkeletonB = Instantiate(skeletonPrefab, skeletonPortB.position, skeletonPortB.rotation);
        SkeletonMonster skmB = instantSkeletonB.GetComponent<SkeletonMonster>();
        skmB.target = target;

        yield return new WaitForSeconds(8.0f);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 25f);
    }
}
