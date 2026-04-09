public enum AlertState
{
    Normal,
    Caution,
    Discover
}

public enum EnemyMoveState
{
    Patrol,
    Search,
    Chase
}

public class EnemyCore
{
    private EnemyStatus m_status;

    public EnemyCore(EnemyStatus status)
    {
        m_status = status;
    }

    public void TakeDamage(int value)
    {
        if (IsDead())
            return;

        m_status.Hp -= value;

        if (m_status.Hp < 0)
            m_status.Hp = 0;
    }

    public bool IsDead() { return m_status.Hp < 0; }
}
