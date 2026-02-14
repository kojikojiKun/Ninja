using UnityEngine;

public class PlayerCore
{
    private PlayableEntityStatus m_status;
    public PlayerCore(PlayableEntityStatus status)
    {
        m_status = status;
    }

    public bool IsDead => m_status.Hp <= 0;
}
