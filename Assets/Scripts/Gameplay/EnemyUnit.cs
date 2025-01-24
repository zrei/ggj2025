using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Damage hit feedback

public class EnemyUnit : MonoBehaviour, IDamagable
{
    public Transform UnitTransform => transform;
    public bool IsDead {  get; private set; }

    private Collider2D m_Collider;

    private int m_MaxHealth;
    private int m_CurrentHealth;

    private void Awake()
    {
        m_Collider = GetComponentInChildren<Collider2D>();
    }

    public void Setup(int id)
    {
        var dEnemy = DEnemy.GetDataById(id).Value;
    }

    public void InternalIncHp(int value)
    {
        m_CurrentHealth = Mathf.Min(m_CurrentHealth + value, m_MaxHealth);
    }

    public void InternalDecHp(int value)
    {
        if (IsDead)
        {
            return;
        }

        m_CurrentHealth -= value;
        if (m_CurrentHealth < 0)
        {
            IsDead = true;
            OnDeath().Forget();
        }
    }

    private async UniTask OnDeath()
    {
        // TODO: Finish playing death animation then destroy
        Destroy(gameObject);
    }

    public void Kill()
    {
        InternalDecHp(m_CurrentHealth);
    }
}
