using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    protected Rigidbody2D m_Rigidbody;
    private Vector2 m_InitialPosition;
    private Vector2 m_Direction;
    private float m_DistanceFromOriginalPositionSquared;

    protected int m_Attack;
    private float m_Speed;

    private bool m_IsUsed;

    private Vector2 m_PreviousPosition;
    private bool m_HasPreviousPosition;

    private TargetType m_TargetType;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    /*
     * Called by enemy behaviour
     */
    public virtual void Setup(Vector2 target, Vector2 direction, int attack, float speed, TargetType targetType, params object[] additionalArguments)
    {
        m_InitialPosition = m_Rigidbody.position;
        m_TargetType = targetType;
        m_DistanceFromOriginalPositionSquared = (target - m_InitialPosition).sqrMagnitude;
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

            if (m_TargetType != TargetType.Player && (m_Rigidbody.position - m_InitialPosition).sqrMagnitude >= m_DistanceFromOriginalPositionSquared)
            {
                Activate(null);
            }
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
            Activate(null);
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            Activate(Player.Instance);
        }
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }

    protected virtual void Activate(IDamagable target = null) 
    {
        m_IsUsed = true;
        Despawn();
    }
}
