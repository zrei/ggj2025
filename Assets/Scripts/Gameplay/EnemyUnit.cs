using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Damage hit feedback

public class EnemyUnit : MonoBehaviour, IDamagable
{
    public Transform UnitTransform => transform;
    public bool IsDead {  get; private set; }

    [field: SerializeField]
    public EnemyProjectile ProjectilePrefab { get; set; }

    private Collider2D m_Collider;

    private int m_MaxHealth;
    private int m_CurrentHealth;
    private int m_Attack;
    private float m_AttackSpeed;
    private float m_MovementSpeed;
    private float m_ProjectileSpeed;

    private void Awake()
    {
        m_Collider = GetComponentInChildren<Collider2D>();
    }

    public void Setup(int id, float attackMult, float hpMult, float movementSpeedMult, float projectileSpeedMult)
    {
        var dEnemy = DEnemy.GetDataById(id).Value;
        var health = Mathf.CeilToInt(dEnemy.Hp * hpMult);

        m_Attack = Mathf.CeilToInt(dEnemy.Attack * attackMult);
        m_MaxHealth = health;
        m_CurrentHealth = health;
        m_MovementSpeed = dEnemy.MovementSpeed * movementSpeedMult;
        m_ProjectileSpeed = dEnemy.ProjectileSpeed * projectileSpeedMult;
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
        m_Collider.enabled = false;

        // TODO: Finish playing death animation then destroy
        Destroy(gameObject);
    }

    public void Kill()
    {
        InternalDecHp(m_CurrentHealth);
    }
}
