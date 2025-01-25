using System.Collections.Generic;
using UnityEngine;

// just wander for now
public class EnemyMoveBehaviour
{
    private List<Vector2> m_MoveLocations;
    private int m_MoveLocationIndex;

    public void Run(Rigidbody2D rb, float moveSpeed, float deltaTime)
    {
        if (m_MoveLocations.Count == 0 || m_MoveLocations == null)
            CalculateNewLocation(rb.position);

        rb.position = Vector2.MoveTowards(rb.position, m_MoveLocations[m_MoveLocationIndex], moveSpeed * deltaTime);
        if (rb.position == m_MoveLocations[m_MoveLocationIndex])
        {
            ++m_MoveLocationIndex;
            if (m_MoveLocationIndex >= m_MoveLocations.Count)
                CalculateNewLocation(rb.position);
        }
    }

    private void CalculateNewLocation(Vector2 currPosition)
    {
        m_MoveLocations = GridManager.Instance.GetRandomTileLocations(currPosition);
        m_MoveLocationIndex = 0;
    }
    /*
    [SerializeField] private float m_MoveSpeed = 10;

    private 
    private List<Vector2> m_MoveLocations;
    private Rigidbody2D m_Rigidbody;
    private int m_MoveLocationIndex;
    
    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        CalculateNewLocation();
    }

    private void Update()
    {
        if (m_MoveLocations.Count == 0)
            CalculateNewLocation();

        m_Rigidbody.position = Vector2.MoveTowards(transform.position, m_MoveLocations[m_MoveLocationIndex], m_MoveSpeed * Time.deltaTime);
        if (m_Rigidbody.position == m_MoveLocations[m_MoveLocationIndex])
        {
            ++m_MoveLocationIndex;
            if (m_MoveLocationIndex >= m_MoveLocations.Count)
                CalculateNewLocation();
        }
    }

    private void CalculateNewLocation()
    {
        m_MoveLocations = GridManager.Instance.GetRandomTileLocations(transform.position);
        m_MoveLocationIndex = 0;
    }
    */
}

public class EnemyThrowBehaviour
{
    /*
    public void Run(Rigidbody2D rb, float radiusFromCenterToSpawn, float deltaTime)
    {
        Player.UnitTransform.position - rb.position;

        rb.position = Vector2.MoveTowards(rb.position, m_MoveLocations[m_MoveLocationIndex], moveSpeed * deltaTime);
        if (rb.position == m_MoveLocations[m_MoveLocationIndex])
        {
            ++m_MoveLocationIndex;
            if (m_MoveLocationIndex >= m_MoveLocations.Count)
                CalculateNewLocation(rb.position);
        }
    }

    private void CalculateNewLocation(Vector2 currPosition)
    {
        m_MoveLocations = GridManager.Instance.GetRandomTileLocations(currPosition);
        m_MoveLocationIndex = 0;
    }
    */
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