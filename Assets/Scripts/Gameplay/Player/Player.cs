using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player>, IDamagable
{
    public Transform UnitTransform => transform;
    public bool IsDead { get; private set; }

    private int m_MaxHealth;
    private int m_CurrentHealth;

    public static IntEvent OnPlayerHealthValueChanged;

    private float m_ReviveCountdownTimer;

    protected override void HandleAwake()
    {
        m_MaxHealth = GlobalEvents.Player.PlayerMaxHealth;
        m_CurrentHealth = GlobalEvents.Player.PlayerMaxHealth;

        IsDead = false;
    }

    public void ResetPlayer()
    {
        m_MaxHealth = GlobalEvents.Player.PlayerMaxHealth;
        m_CurrentHealth = GlobalEvents.Player.PlayerMaxHealth;

        IsDead = false;

        OnPlayerHealthValueChanged?.Invoke(m_CurrentHealth);
    }

    public void InternalIncHp(int value)
    {
        // TODO: Spawn Damage Text (?)
        m_CurrentHealth = Mathf.Min(m_CurrentHealth + value, m_MaxHealth);
        OnPlayerHealthValueChanged?.Invoke(m_CurrentHealth);
    }

    public void InternalDecHp(int value)
    {
        if (IsDead)
        {
            return;
        }

        m_CurrentHealth = Mathf.Max(m_CurrentHealth - value, 0);
        if (m_CurrentHealth <= 0)
        {
            OnPlayerHealthValueChanged?.Invoke(0);
            OnDeath();
        }
        else
        {
            OnPlayerHealthValueChanged?.Invoke(m_CurrentHealth);
        }
    }

    private void OnDeath()
    {
        // TODO
        IsDead = true;
        m_ReviveCountdownTimer = GlobalEvents.Player.ReviveTime;
        GlobalEvents.Player.OnPlayerDeath?.Invoke();
    }

    private void Update()
    {
        if (IsDead)
        {
            m_ReviveCountdownTimer -= Time.deltaTime;
            if (m_ReviveCountdownTimer <= 0)
            {
                IsDead = false;
                InternalIncHp(m_MaxHealth);
            }
                
        }
    }
}
