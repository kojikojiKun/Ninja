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
        float dot = Vector3.Dot(m_eye.forward, toTarget);
        float threshold = Mathf.Cos(m_status.ViewAngle * 0.5f * Mathf.Deg2Rad);
        return dot >= threshold;
    }

    bool IsWithinDistance(Vector3 dir)
    {
        //sqrt‰ñ”ð‚Ì‚½‚ßm_status.ViewDistance^2‚Å”äŠr.
        return dir.sqrMagnitude <= m_status.ViewDistance * m_status.ViewDistance;
    }

    //ˆê’èŠÔŠu‚ÅŽ‹–ì“à‚ÉƒvƒŒƒCƒ„[‚ª‚¢‚é‚©‚Ç‚¤‚©‚ð”»’è.
    public bool IsSeePlayer()
    {
        if (m_player == null || m_eye == null || m_status == null)
            return false;

        Vector3 dir = m_player.position - m_eye.position;

        if (!IsWithinViewAngle(dir) || !IsWithinDistance(dir))
            return false;

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