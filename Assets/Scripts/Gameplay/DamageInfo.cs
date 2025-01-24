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

    public DamageInfo(int damage, DamageType damageType, IDamagable target)
    {
        m_Damage = damage;
        m_DamageType = damageType;
        m_Target = target;
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
            var damageText = SpawnManager.Instance.SpawnDamageText(m_Target.UnitTransform.position);
            damageText.Setup(m_Damage);
            m_Target.InternalDecHp(m_Damage);
        }
    }
}
