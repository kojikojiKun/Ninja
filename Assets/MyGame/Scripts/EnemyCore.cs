using UnityEngine;
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

public class EnemyCore : IDamageable,IAssassinateable
{
    private EnemyStatus m_status;
    public Transform Transform { get; }

    public EnemyCore(EnemyStatus status,Transform transform)
    {
        m_status = status;
        Transform = transform;
    }

    public void TakeDamage(int value)
    {

    }

    public bool IsDead() { return m_status.Hp < 0; }
}
