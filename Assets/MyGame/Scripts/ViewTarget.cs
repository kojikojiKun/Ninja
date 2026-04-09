using UnityEngine;

[System.Serializable]
public class ViewTargetProfile
{
    public Transform RayOrigin;
    public LayerMask Mask;
    public string TargetTagName;
    [Header("Enemyは0にする")]
    public float Angle;
    public float Distanece;
}

public class ViewTarget
{
    private Transform m_target;
    private Transform m_origin;
    private float m_angle;
    private float m_distance;
    private LayerMask m_sightMask;
    private string m_targetTagName;

    public ViewTarget(ViewTargetProfile profile)
    {
        m_origin = profile.RayOrigin;
        m_sightMask = profile.Mask;
        m_targetTagName = profile.TargetTagName;

        if (m_distance == 0 || m_angle == 0)
            return;

        m_angle = profile.Angle;
        m_distance = profile.Distanece;
    }

    public void SetTarget(Transform target)
    {
        m_target = target;
    }

    public void OverrideValues(float angle, float distance)
    {
        m_angle = angle;
        m_distance = distance;
    }

    bool IsWithinViewAngle(Vector3 dir)
    {
        Vector3 toTarget = dir.normalized;

        //ターゲットが視野内ならtrue
        float dot = Vector3.Dot(m_origin.forward, toTarget);
        float threshold = Mathf.Cos(m_angle * 0.5f * Mathf.Deg2Rad);
        return dot >= threshold;
    }

    bool IsWithinDistance(Vector3 dir)
    {
        //sqrt回避のためm_status.ViewDistance^2で比較.
        return dir.sqrMagnitude <= m_distance * m_distance;
    }

    //一定間隔で視野内にターゲットがいるかどうかを判定.
    public bool IsSeeTarget()
    {
        if (m_target == null || m_origin == null || m_distance == 0 || m_angle == 0)
            return false;

        Vector3 dir = (m_target.position + Vector3.up * 1.5f) - m_origin.position;

        if (!IsWithinViewAngle(dir) || !IsWithinDistance(dir))
            return false;

        //Rayを飛ばして(sightMask)のオブジェクトのみを判定.
        if (Physics.Raycast(
            m_origin.position,
            dir.normalized,
            out RaycastHit hit,
            m_distance,
            m_sightMask
            ))
        {
            if (hit.collider.CompareTag(m_targetTagName))
            {
                return true;
            }
        }

        return false;
    }
}
