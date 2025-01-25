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

    #region Timer
    private float m_ReviveCountdownTimer;
    private bool m_IsOnDirty = false;
    private float m_CurrDirtyDotTimer;
    #endregion

    #region Visuals
    private SpriteRenderer m_SR;
    [SerializeField] private Color m_DamagedFlashColor = Color.red;
    [SerializeField] private float m_DamagedFlashInterval = 0.5f;
    [SerializeField] private float m_DeadAlphaValue = 0.5f;

    private Coroutine m_CurrentlyPlayingFlashCoroutine;
    #endregion

    private void Start()
    {
        m_SR = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void HandleAwake()
    {
        base.HandleAwake();

        m_MaxHealth = GlobalEvents.Player.PlayerMaxHealth;
        m_CurrentHealth = GlobalEvents.Player.PlayerMaxHealth;

        IsDead = false;

        GlobalEvents.Player.OnPlayerMoveOntoTile += ToggleStandingOnDirty;
    }

    protected override void HandleDestroy()
    {
        base.HandleDestroy();

        GlobalEvents.Player.OnPlayerMoveOntoTile -= ToggleStandingOnDirty;
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

        if (m_CurrentlyPlayingFlashCoroutine != null)
        {
            StopCoroutine(m_CurrentlyPlayingFlashCoroutine);    
        }
        m_CurrentlyPlayingFlashCoroutine = StartCoroutine(FlashRedOnDamage());

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
        if (m_CurrentlyPlayingFlashCoroutine != null)
        {
            StopCoroutine(m_CurrentlyPlayingFlashCoroutine);
            m_CurrentlyPlayingFlashCoroutine = null;
        }

        Color deadColor = Color.white;
        deadColor.a = m_DeadAlphaValue;
        m_SR.color = deadColor;

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
                m_SR.color = Color.white;
                InternalIncHp(m_MaxHealth);
            }       
        }
        else if (m_IsOnDirty)
        {
            m_CurrDirtyDotTimer += Time.deltaTime;
            if (m_CurrDirtyDotTimer >= GlobalEvents.Player.DirtyDotInterval)
            {
                InternalDecHp(GlobalEvents.Player.DirtyDotDamage);
                m_CurrDirtyDotTimer = m_CurrDirtyDotTimer % GlobalEvents.Player.DirtyDotInterval;
            }
        }
    }

    private void ToggleStandingOnDirty(bool isOnDirty)
    {
        if (!isOnDirty)
        {
            m_CurrDirtyDotTimer = 0f;
        }
        m_IsOnDirty = isOnDirty;
    }

    private IEnumerator FlashRedOnDamage()
    {
        m_SR.color = m_DamagedFlashColor;

        float t = 0f;

        while (t < m_DamagedFlashInterval)
        {
            t += Time.deltaTime;
            yield return null;
        }

        m_SR.color = Color.white;
    }
}
