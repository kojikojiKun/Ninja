public enum PlayerMoveState
{
    Idle,
    Walk,
    Run,
    Crouch,
    CrouchIdle
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
