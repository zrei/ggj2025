using UnityEngine;

public class EnemyAOEProjectile : EnemyProjectile
{
    [System.Serializable]
    public struct AdditionalAOEInfo
    {
        [Tooltip("Radius of tiles to dirty")]
        public int m_DirtyRadius;
        [Tooltip("World radius for circle cast")]
        public float m_DamageRadius;
    }

    [SerializeField] private LayerMask m_PlayerLayerMask;
    private AdditionalAOEInfo m_AdditionalInfo;

    public override void Setup(Vector2 target, Vector2 direction, int attack, float speed, TargetType targetType, params object[] additionalArguments)
    {
        base.Setup(target, direction, attack, speed, targetType, additionalArguments);
        m_AdditionalInfo = (AdditionalAOEInfo)additionalArguments[0];
    }

    protected override void Activate(IDamagable target = null)
    {
        GridManager.Instance.ExplodeAOEBullet(m_Rigidbody.position, m_AdditionalInfo.m_DirtyRadius);

        if (target != null)
        {
            new DamageInfo(m_Attack, DamageType.HealthDec, target).ProcessDamage();
        }
        else
        {
            RaycastHit2D hit = Physics2D.CircleCast(m_Rigidbody.position, m_AdditionalInfo.m_DamageRadius, Vector2.zero, 0, m_PlayerLayerMask);
            if (hit.collider != null)
            {
                new DamageInfo(m_Attack, DamageType.HealthDec, Player.Instance).ProcessDamage();
            }
        }
        
        base.Activate(target);
    }
}
