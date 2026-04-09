public enum PlayerMoveState
{
    Idle,
    Walk,
    Run,
    CrouchWalk,
    CrouchIdle
}

public class PlayerCore
{
    private PlayableEntityStatus m_status;

    public PlayerCore(PlayableEntityStatus status)
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

    public bool IsDead() { return m_status.Hp <= 0; }
}
