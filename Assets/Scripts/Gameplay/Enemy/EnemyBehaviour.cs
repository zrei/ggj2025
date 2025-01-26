using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBehaviour
{
    protected float m_Speed;
    protected Rigidbody2D m_Rb;
    protected EnemyUnit m_EnemyUnit;
    protected TargetType m_TargetType;

    // i am in hell
    public virtual void Setup(EnemyUnit enemyUnit, Rigidbody2D rigidbody, float speed, TargetType targetType, int param1, DamageTypeDealt param2, params object[] additionalArguments)
    {
        m_EnemyUnit = enemyUnit;
        m_Rb = rigidbody;
        m_Speed = speed;
        m_TargetType = targetType;
    }

    public abstract void Enter();
    public abstract void Run(float deltaTime);
    public abstract void Exit();
}

// just wander for now
public class EnemyMoveBehaviour : EnemyBehaviour
{
    private List<Vector2> m_MoveLocations;
    private int m_MoveLocationIndex;
    private Vector2 m_PreviousLocation;
    private bool m_HasPreviousLocation = false;

    public override void Setup(EnemyUnit enemyUnit, Rigidbody2D rigidbody, float speed, TargetType targetType, int param1, DamageTypeDealt param2,params object[] additionalArguments)
    {
        base.Setup(enemyUnit, rigidbody, speed, targetType, param1, param2, additionalArguments);
    }

    public override void Enter()
    {
        m_HasPreviousLocation = false;
        CalculateNewLocation(m_Rb.position);
    }

    public override void Run(float deltaTime)
    {
        if (m_HasPreviousLocation)
        {
            GridManager.Instance.SetTileStatus(m_PreviousLocation, TileType.DIRTY);
        }

        if (m_MoveLocations.Count == 0)
        {
            CalculateNewLocation(m_Rb.position);
        }

        if (m_MoveLocations.Count == 0)
            return;

        m_Rb.position = Vector2.MoveTowards(m_Rb.position, m_MoveLocations[m_MoveLocationIndex], m_Speed * deltaTime);
        if (m_Rb.position == m_MoveLocations[m_MoveLocationIndex])
        {
            ++m_MoveLocationIndex;
            if (m_MoveLocationIndex >= m_MoveLocations.Count)
                CalculateNewLocation(m_Rb.position);
        }

        m_PreviousLocation = m_Rb.position;
        m_HasPreviousLocation = true;
    }

    private void CalculateNewLocation(Vector2 currPosition)
    {
        if (m_TargetType != TargetType.Player)
        {
            m_MoveLocations = GridManager.Instance.GetRandomTileLocations(currPosition, m_TargetType, Player.Instance.UnitTransform.position);
        }
        else
        {
            m_MoveLocations = GridManager.Instance.GetRandomTileLocations(currPosition, m_TargetType);
        }
        
        m_MoveLocationIndex = 0;
    }

    public override void Exit()
    {
        // pass
    }
}

public class EnemyThrowBehaviour : EnemyBehaviour
{
    [System.Serializable]
    public struct AdditionalEnemyThrowData
    {
        public EnemyProjectile m_EnemyProjectileToFire;
        public float m_RadiusFromCenterToSpawn;
        public EnemyAOEProjectile.AdditionalAOEInfo m_AdditionalAOEInfo;
    }

    private AdditionalEnemyThrowData m_AdditionalData;
    private int m_NumBullets;
    private bool m_BulletsHasBeenFired = false;

    public override void Enter()
    {
        m_BulletsHasBeenFired = false;
    }

    public override void Setup(EnemyUnit enemyUnit, Rigidbody2D rigidbody, float speed, TargetType targetType, int param1, DamageTypeDealt param2,params object[] additionalArguments)
    {
        base.Setup(enemyUnit, rigidbody, speed, targetType, param1, param2, additionalArguments);
        m_NumBullets = param1;
        m_AdditionalData = (AdditionalEnemyThrowData) additionalArguments[0];
    }

    public override void Run(float deltaTime)
    {
        if (m_BulletsHasBeenFired)
            return; 
        
        for (int i = 0; i < m_NumBullets; ++i)
        {
            Vector3 targetLocation = GridManager.Instance.GetRandomTargetLocation(m_Rb.position, m_TargetType);
            Vector3 direction = (targetLocation - m_EnemyUnit.UnitTransform.position).normalized;
            Vector3 spawnPosition = m_EnemyUnit.UnitTransform.position + direction * m_AdditionalData.m_RadiusFromCenterToSpawn;
            EnemyProjectile enemyProjectile = GameObject.Instantiate(m_AdditionalData.m_EnemyProjectileToFire, spawnPosition, Quaternion.identity);
            enemyProjectile.Setup(targetLocation, direction, m_EnemyUnit.Attack, m_Speed, m_TargetType, m_AdditionalData.m_AdditionalAOEInfo);
        }

        m_BulletsHasBeenFired = true;
    }

    public override void Exit()
    {
        // pass
    }
}
