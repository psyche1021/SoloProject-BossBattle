using UnityEngine;

public abstract class BaseAttack : MonoBehaviour
{
    public abstract void Attack();
    public virtual void Skill()
    {
        return;
    }
}
