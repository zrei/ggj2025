using UnityEngine;

[System.Serializable]
public struct EnemyControllerState
{
    public EnemyBehaviour m_EnemyBehaviour;
    public float m_TimeToLast;
}

public class EnemyController : MonoBehaviour
{
    [Header("Projectiles")]
    [SerializeField] private EnemyThrowBehaviour.AdditionalEnemyThrowData m_ProjectileData;

    private EnemyControllerState m_NormalState;
    private EnemyControllerState m_HyperState;

    private float m_CurrCooldown = 0;
    private bool m_IsHyper = false;

    private EnemyUnit m_EnemyUnit;

    public void Setup(EnemyData enemyData, EnemyUnit enemyUnit)
    {
        m_EnemyUnit = enemyUnit;
        SetupState(ref m_NormalState, enemyData, enemyUnit, false);
        SetupState(ref m_HyperState, enemyData, enemyUnit, true);
        m_NormalState.m_EnemyBehaviour.Enter();
    }

    private void SetupState(ref EnemyControllerState enemyControllerState, EnemyData enemyData, EnemyUnit enemyUnit, bool isHyper)
    {
        if (!isHyper)
        {
            EnemyMoveBehaviour enemyMoveBehaviour = new EnemyMoveBehaviour();
            enemyMoveBehaviour.Setup(enemyUnit, GetComponent<Rigidbody2D>(), enemyData.MovementSpeed, enemyData.Param1, new EnemyMoveBehaviour.AdditionalMoveBehaviourData() { m_TileTargetType = enemyData.Target });
            enemyControllerState = new EnemyControllerState() { m_EnemyBehaviour = enemyMoveBehaviour, m_TimeToLast = 1 / enemyData.AttackSpeed };
        }
        else
        {
            if (enemyData.AtkType == AttackType.Projectile)
            {
                EnemyThrowBehaviour enemyThrowBehaviour = new EnemyThrowBehaviour();
                enemyThrowBehaviour.Setup(enemyUnit, GetComponent<Rigidbody2D>(), enemyData.Speed, enemyData.Param1, m_ProjectileData);
                enemyControllerState = new EnemyControllerState() { m_EnemyBehaviour = enemyThrowBehaviour, m_TimeToLast = enemyData.Duration };
            }
            else
            {
                EnemyMoveBehaviour enemyMoveBehaviour = new EnemyMoveBehaviour();
                enemyMoveBehaviour.Setup(enemyUnit, GetComponent<Rigidbody2D>(), enemyData.Speed, enemyData.Param1, new EnemyMoveBehaviour.AdditionalMoveBehaviourData() { m_TileTargetType = enemyData.Target });
                enemyControllerState = new EnemyControllerState() { m_EnemyBehaviour = enemyMoveBehaviour, m_TimeToLast = enemyData.Duration };
            }
        }
    }

    private void Update()
    {
        if (m_EnemyUnit.IsDead)
            return;

        m_CurrCooldown += Time.deltaTime;
        if (m_CurrCooldown > GetCurrDuration)
        {
            m_CurrCooldown = 0;
            m_IsHyper = !m_IsHyper;
            if (m_IsHyper)
            {
                m_NormalState.m_EnemyBehaviour.Exit();
                m_HyperState.m_EnemyBehaviour.Enter();
            }
            else
            {
                m_HyperState.m_EnemyBehaviour.Exit();
                m_NormalState.m_EnemyBehaviour.Enter();
            }
        }

        if (m_IsHyper)
        {
            m_HyperState.m_EnemyBehaviour.Run(Time.deltaTime);
        }
        else
        {
            m_NormalState.m_EnemyBehaviour.Run(Time.deltaTime);
        }
    }

    private float GetCurrDuration => m_IsHyper ? m_HyperState.m_TimeToLast : m_NormalState.m_TimeToLast;
}
