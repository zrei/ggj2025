using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Rigidbody2D m_Rigidbody;
    private Vector2 m_Direction;
    private int m_Attack;
    private float m_Speed;

    private bool m_IsUsed;

    private Vector2 m_PreviousPosition;
    private bool m_HasPreviousPosition;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    /*
     * Called by enemy behaviour
     */

    // need to pass enemy location and... range 
    public void Setup(Vector2 direction, int attack, float speed)
    {
        m_Direction = direction;
        m_Attack = attack;
        m_Speed = speed;
        m_IsUsed = false;
    }

    private void Update()
    {
        if (!m_IsUsed)
        {
            if (m_HasPreviousPosition && GridManager.Instance.IsPositionValid(m_Rigidbody.position))
                GridManager.Instance.SetTileStatus(m_Rigidbody.position, TileType.DIRTY);
            m_Rigidbody.velocity = m_Direction * m_Speed;

            m_PreviousPosition = m_Rigidbody.position;
            m_HasPreviousPosition = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (m_IsUsed)
        {
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            m_IsUsed = true;
            Despawn();
        }
        if (other.gameObject.CompareTag("Player"))
        {
            new DamageInfo(m_Attack, DamageType.HealthDec, Player.Instance).ProcessDamage();
            m_IsUsed = true;
            Despawn();
        }
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }

    protected virtual void Activate() { }
}
