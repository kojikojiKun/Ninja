public enum AlertState
{
    Normal,
    Caution,
    Discover
}

public class EnemyCore : IDamageable
{
    private EnemyStatus m_status;

    public EnemyCore(EnemyStatus status)
    {
        m_status = status;
    }

    public void TakeDamage(int value)
    {

    }

    public bool IsDead() { return m_status.Hp < 0; }
}
