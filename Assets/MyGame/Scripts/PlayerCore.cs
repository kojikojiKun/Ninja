public enum PlayerMoveState
{
    Stop,
    Walk,
    Run,
    Crouch
}

public class PlayerCore :IDamageable
{
    private PlayableEntityStatus m_status;

    public PlayerCore(PlayableEntityStatus status)
    {
        m_status = status;
    }

    public void TakeDamage(int value)
    {

    }

    public bool IsDead() { return m_status.Hp < 0; }
}
