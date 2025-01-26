using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAOEProjectile : EnemyProjectile
{
    // or
    private Vector2 m_TargetLocation;
    private TargetType m_TargetType;
    private int m_Radius;
    private LayerMask m_PlayerLayerMask;

    protected override void Activate(IDamagable target = null)
    {
        GridManager.Instance.ExplodeAOEBullet(transform.position, m_Radius);
        Physics2D.CircleCast(transform.position, m_Radius, Vector2.zero, 0, m_PlayerLayerMask);
    }
}
