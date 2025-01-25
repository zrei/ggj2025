public enum DamageType
{
    HealthInc,
    HealthDec,
}

public class DamageInfo
{
    private readonly int m_Damage;
    private readonly DamageType m_DamageType;
    private readonly IDamagable m_Target;
    private readonly bool m_ShowDamageText;

    public DamageInfo(int damage, DamageType damageType, IDamagable target, bool showDamageText = true)
    {
        m_Damage = damage;
        m_DamageType = damageType;
        m_Target = target;
        m_ShowDamageText = showDamageText;
    }

    public void ProcessDamage()
    {
        if (m_Target != null)
        {
            return;
        }

        if (m_DamageType == DamageType.HealthInc)
        {
            m_Target.InternalIncHp(m_Damage);
        }
        else if (m_DamageType == DamageType.HealthDec)
        {
            if (m_ShowDamageText)
            {
                var damageText = SpawnManager.Instance.SpawnDamageText(m_Target.UnitTransform.position);
                damageText.Setup(m_Damage);
            }

            m_Target.InternalDecHp(m_Damage);
        }
    }
}
