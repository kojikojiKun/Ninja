using UnityEngine;

public class EnemyEye
{
    private EnemyStatus m_status;
    private Transform m_player;
    private Transform m_eye;
    private float m_checkIntercal;
    private float m_timer;

    public EnemyEye(EnemyStatus status,Transform enemiesEye)
    {
        m_status = status;
        m_eye = enemiesEye;
    }

    public void GetPlayer(Transform player)
    {
        m_player = player;
    }

    bool IsWithinViewAngle()
    {
        Vector3 toTarget= (m_player.position-m_eye.position).normalized;
        float dot = Vector3.Dot(m_eye.forward, toTarget);
        float threshold = Mathf.Cos(m_status.ViewAngle * 0.5f * Mathf.Deg2Rad);
        return dot >= threshold;
    }

    bool IsWithinDistance(Vector3 dir)
    {
        //sqrt回避のためm_status.ViewDistance^2で比較.
        return dir.sqrMagnitude <= m_status.ViewDistance*m_status.ViewDistance;
    }

    //一定間隔で視野内のオブジェクトがプレイヤーかどうかを判定.
    public void CheckSight()
    {
        m_timer += Time.deltaTime;
        if (m_timer < m_checkIntercal)
            return;

        Vector3 dir = m_player.position- m_eye.position;

        if(!IsWithinViewAngle() || !IsWithinDistance(dir))
            return;

        if(Physics.Raycast(
            m_eye.position,
            dir.normalized,
            out RaycastHit hit,
            m_status.ViewDistance
            ))
        {
            if (hit.collider.CompareTag("Player"))
                Debug.Log("player found");
        }

        m_timer = 0f;
    }
}