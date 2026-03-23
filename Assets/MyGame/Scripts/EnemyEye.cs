using UnityEngine;

public class EnemyEye
{
    private EnemyStatus m_status;
    private Transform m_player;
    private Transform m_eye;
    private LayerMask m_sightMask;

    public EnemyEye(EnemyStatus status, Transform enemiesEye, LayerMask mask)
    {
        m_status = status;
        m_eye = enemiesEye;
        m_sightMask = mask;
    }

    public void GetPlayer(Transform player)
    {
        m_player = player;
    }

    bool IsWithinViewAngle(Vector3 dir)
    {
        Vector3 toTarget = dir.normalized;

        //ターゲットが視野内ならtrue
        float dot = Vector3.Dot(m_eye.forward, toTarget);
        float threshold = Mathf.Cos(m_status.ViewAngle * 0.5f * Mathf.Deg2Rad);
        return dot >= threshold;
    }

    bool IsWithinDistance(Vector3 dir)
    {
        //sqrt回避のためm_status.ViewDistance^2で比較.
        return dir.sqrMagnitude <= m_status.ViewDistance * m_status.ViewDistance;
    }

    //一定間隔で視野内にプレイヤーがいるかどうかを判定.
    public bool IsSeePlayer()
    {
        if (m_player == null || m_eye == null || m_status == null)
            return false;

        Vector3 dir = m_player.position - m_eye.position;

        if (!IsWithinViewAngle(dir) || !IsWithinDistance(dir))
            return false;

        //Rayを飛ばして(sightMask)のオブジェクトのみを判定.
        if (Physics.Raycast(
            m_eye.position,
            dir.normalized,
            out RaycastHit hit,
            m_status.ViewDistance,
            m_sightMask
            ))
        {
            if (hit.collider.CompareTag("Player"))
                return true;
        }

        return false;
    }
}