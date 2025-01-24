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
    }

    public void InternalDecHp(int value)
    {
        if (IsDead)
        {
            return;
        }

        // TODO: Spawn Damage Text
        m_CurrentHealth -= value;
        if (m_CurrentHealth < 0)
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
    }
}
