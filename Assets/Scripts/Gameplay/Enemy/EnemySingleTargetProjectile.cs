public class EnemySingleTargetProjectile : EnemyProjectile {
    protected override void Activate(IDamagable target = null)
    {
        if (target != null)
        {
            new DamageInfo(m_Attack, DamageType.HealthDec, target).ProcessDamage();
        }

        base.Activate(target);
    }
}
