using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Damage hit feedback

public class EnemyUnit : MonoBehaviour, IDamagable
{
    [SerializeField] private float m_DamageCooldown = 1f;
    public Transform UnitTransform => transform;
    public bool IsDead {  get; private set; }

    private Collider2D m_Collider;

    private int m_MaxHealth;
    private int m_CurrentHealth;
    public int Attack { get; private set; }
    private float m_AttackSpeed;
    private float m_MovementSpeed;
    private float m_HyperSpeed;

    #region Damage
    private SpriteRenderer m_SR;
    private float m_CurrDamageCooldown = 0f;
    #endregion

    [SerializeField] private EnemyController m_EnemyController;

    [SerializeField] private Color m_DamagedFlashColor = Color.red;
    [SerializeField] private float m_DamagedFlashInterval = 0.2f;

    private Coroutine m_CurrentlyPlayingFlashCoroutine;

    private void Awake()
    {
        m_Collider = GetComponentInChildren<Collider2D>();
        m_SR = GetComponentInChildren<SpriteRenderer>();
    }

    public void Setup(int id, float attackMult, float hpMult, float movementSpeedMult, float projectileSpeedMult)
    {
        var dEnemy = DEnemy.GetDataById(id).Value;
        var health = Mathf.CeilToInt(dEnemy.Hp * hpMult);

        Attack = Mathf.CeilToInt(dEnemy.Attack * attackMult);
        m_MaxHealth = health;
        m_CurrentHealth = health;
        m_MovementSpeed = dEnemy.MovementSpeed * movementSpeedMult;
        m_HyperSpeed = dEnemy.Speed * projectileSpeedMult;

        m_EnemyController.Setup(dEnemy, this);
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

        if (m_CurrentlyPlayingFlashCoroutine != null)
        {
            StopCoroutine(m_CurrentlyPlayingFlashCoroutine);
        }
        m_CurrentlyPlayingFlashCoroutine = StartCoroutine(FlashRedOnDamage());

        m_CurrentHealth -= value;
        if (m_CurrentHealth <= 0)
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsDead)
        {
            return;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            
            if (m_CurrDamageCooldown > 0)
                return;

            float playerSpeed = PlayerBeam.Instance.SpeedMagnitude;
            if (playerSpeed > GlobalEvents.Player.MinimumSpeedForDamage)
            {
                float damage = (playerSpeed - GlobalEvents.Player.MinimumSpeedForDamage) * GlobalEvents.Player.DamageScale;
                m_CurrDamageCooldown = m_DamageCooldown;
                new DamageInfo(Mathf.CeilToInt(damage), DamageType.HealthDec, this).ProcessDamage();
            }
        }
    }

    private void Update()
    {
        if (m_CurrDamageCooldown > 0)
            m_CurrDamageCooldown -= Time.deltaTime;
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
