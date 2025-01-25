using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Rigidbody2D m_Rigidbody;
    private Vector2 m_Direction;
    private int m_Attack;
    private float m_Speed;

    private bool m_IsUsed;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }
    /*
     * Called by enemy behaviour
     */
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
            m_Rigidbody.velocity = m_Direction * m_Speed;
        }
        // TODO: Dirty all that it touches
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (m_IsUsed)
        {
            return;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            new DamageInfo(m_Attack, DamageType.HealthDec, Player.Instance).ProcessDamage();
            m_IsUsed = true;
        }
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }
}
