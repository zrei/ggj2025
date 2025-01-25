using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBehaviour
{
    protected Rigidbody2D m_Rb;
    protected EnemyUnit m_EnemyUnit;

    // i am in hell
    public virtual void Setup(EnemyUnit enemyUnit, Rigidbody2D rigidbody, float speed, int param1, params object[] additionalArguments)
    {
        m_EnemyUnit = enemyUnit;
        m_Rb = rigidbody;
    }

    public abstract void Enter();
    public abstract void Run(float deltaTime);
    public abstract void Exit();
}

// just wander for now
public class EnemyMoveBehaviour : EnemyBehaviour
{
    public struct AdditionalMoveBehaviourData
    {
        public TargetType m_TileTargetType;
    }

    private AdditionalMoveBehaviourData m_AdditionalData;
    private float m_MovementSpeed;

    private List<Vector2> m_MoveLocations;
    private int m_MoveLocationIndex;

    public override void Setup(EnemyUnit enemyUnit, Rigidbody2D rigidbody, float speed, int param1, params object[] additionalArguments)
    {
        base.Setup(enemyUnit, rigidbody, speed, param1, additionalArguments);
        m_MovementSpeed = speed;
        m_AdditionalData = (AdditionalMoveBehaviourData)additionalArguments[0];
    }

    public override void Enter()
    {
        Debug.Log("Enter move state with speed " + m_MovementSpeed);
        //CalculateNewLocation(m_Rb.position);
    }

    public override void Run(float deltaTime)
    {
        /*
        m_Rb.position = Vector2.MoveTowards(m_Rb.position, m_MoveLocations[m_MoveLocationIndex], m_MovementSpeed * deltaTime);
        if (m_Rb.position == m_MoveLocations[m_MoveLocationIndex])
        {
            ++m_MoveLocationIndex;
            if (m_MoveLocationIndex >= m_MoveLocations.Count)
                CalculateNewLocation(m_Rb.position);
        }
        */
    }

    private void CalculateNewLocation(Vector2 currPosition)
    {
        m_MoveLocations = GridManager.Instance.GetRandomTileLocations(currPosition);
        m_MoveLocationIndex = 0;
    }

    public override void Exit()
    {
        Debug.Log("Exit move state with speed " + m_MovementSpeed);
        // pass
    }
}

public class EnemyThrowBehaviour : EnemyBehaviour
{
    public struct AdditionalEnemyThrowData
    {
        public EnemyProjectile m_EnemyProjectileToFire;
        public float m_RadiusFromCenterToSpawn;
    }

    private AdditionalEnemyThrowData m_AdditionalData;
    private float m_ProjectileSpeed;
    private int m_NumBullets;
    private bool m_BulletsHasBeenFired = false;

    public override void Enter()
    {
        Debug.Log("Enter fire state with speed " + m_ProjectileSpeed + " and " + m_NumBullets + " bullets");
        m_BulletsHasBeenFired = false;
    }

    public override void Setup(EnemyUnit enemyUnit, Rigidbody2D rigidbody, float speed, int param1, params object[] additionalArguments)
    {
        base.Setup(enemyUnit, rigidbody, speed, param1, additionalArguments);
        m_ProjectileSpeed = speed;
        m_NumBullets = param1;
        m_AdditionalData = (AdditionalEnemyThrowData) additionalArguments[0];
    }

    public override void Run(float deltaTime)
    {
        /*
        if (m_BulletsHasBeenFired)
            return; 
        
        for (int i = 0; i < m_NumBullets; ++i)
        {
            Vector3 direction = (Player.Instance.UnitTransform.position - m_EnemyUnit.UnitTransform.position).normalized;
            Vector3 spawnPosition = m_EnemyUnit.UnitTransform.position + direction * m_AdditionalData.m_RadiusFromCenterToSpawn;
            EnemyProjectile enemyProjectile = GameObject.Instantiate(m_AdditionalData.m_EnemyProjectileToFire, spawnPosition, Quaternion.identity);
            enemyProjectile.Setup(direction);
        }
        */
    }

    public override void Exit()
    {
        Debug.Log("Exit fire state with speed " + m_ProjectileSpeed + " and " + m_NumBullets + " bullets");
    }
}

/*
 * 
 *ENEMY
2 types of enemy: 

Easy: 
1) State: Normal: just that little guy dirtying the floor as it moves
2) State Hyper: Runs at a faster speed and dirtys the floor
Medium:
1) State Normal: also dirty the floor as it moves  (same as the easy one)
2) State Hyper: throws a projectile towards the player every few seconds , these projectile slides across the tiles , dirtying the tiles


The enemies will not chase the player  its job is just to dirty the place, and also because when the player collides with it, the enemy is the one taking the damage
*/