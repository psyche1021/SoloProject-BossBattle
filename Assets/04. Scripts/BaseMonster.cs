using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseMonster : MonoBehaviour
{
    public Transform target;

    protected bool isDead = false;

    public bool IsDead => isDead;

    protected NavMeshAgent navMeshAgent;
    protected new Rigidbody rigidbody;
    protected Animator animator;
    SkinnedMeshRenderer[] skinMeshes;
    MeshRenderer[] armorMeshes;

    public float CurrentHp { get; set; }

    public float MaxHp { get; set; }

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        skinMeshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        armorMeshes = GetComponentsInChildren<MeshRenderer>();

        if (MaxHp <= 0) SetHp(100); // 체력 미설정 시 임의 할당
    }

    void FixedUpdate()
    {
        FreezeRotation();
    }

    protected virtual IEnumerator DoRest() => null;
    protected virtual IEnumerator DoThink() => null;
    protected abstract IEnumerator DoWalk();
    protected virtual IEnumerator DoSpell() => null;
    protected virtual IEnumerator DoAttack() => null;

    void FreezeRotation()
    {
        rigidbody.linearVelocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }

    protected void SetHp(float maxHp)
    {
        MaxHp = maxHp;
        CurrentHp = MaxHp;
    }

    protected virtual IEnumerator Die()
    {
        isDead = true;

        rigidbody.isKinematic = true;

        Collider[] colliders = GetComponents<Collider>();
        foreach(Collider collider in colliders)
        {
            collider.enabled = false;
        }

        animator.SetTrigger("doDie");

        navMeshAgent.isStopped = true;
        navMeshAgent.enabled = false;

        Invoke("DestroyObject", 5f);

        yield return null;
    }

    protected virtual void Hit(int damage)
    {
        if (isDead) return;

        CurrentHp -= damage;
        StartCoroutine(HitEffect());
        if (CurrentHp <= 0) StartCoroutine(Die());
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.gameObject.CompareTag("Attack"))
        {
            Hit(1);
            other.gameObject.GetComponentInParent<PlayerStatus>().OnAttackHit();
        }
        else if (other.gameObject.CompareTag("Skill"))
        {
            Hit(5);
            other.gameObject.GetComponentInParent<PlayerStatus>().OnSkillHit();
        }
    }

    IEnumerator HitEffect() 
    {
        // 맞았으니 색상 변경
        foreach (SkinnedMeshRenderer mesh in skinMeshes) mesh.material.color = Color.yellow;
        foreach (MeshRenderer mesh in armorMeshes) mesh.material.color = Color.yellow;

        yield return new WaitForSeconds(0.5f);

        // 원상복구
        foreach (SkinnedMeshRenderer mesh in skinMeshes) mesh.material.color = Color.white;
        foreach (MeshRenderer mesh in armorMeshes) mesh.material.color = Color.white;

        yield return new WaitForSeconds(0.5f);
    }

    protected IEnumerator LookAtPlayer()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            float angle = Quaternion.Angle(transform.rotation, lookRotation);

            while (angle > 1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2f);
                angle = Quaternion.Angle(transform.rotation, lookRotation);
                yield return null;
            }
        }
    }
}
