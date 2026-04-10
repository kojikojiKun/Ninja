using UnityEngine;

public class ViewTarget
{
    private Transform m_origin;
    private float m_angle;
    private float m_distance;
    private LayerMask m_sightMask;
    private string m_targetTagName;
    private Vector3 m_heightOffset;

    public ViewTarget(ViewProfile profile,Transform rayOrigin)
    {
        m_origin = rayOrigin;
        m_sightMask = profile.Mask;
        m_targetTagName = profile.TargetTagName;
        m_angle = profile.Angle;
        m_distance = profile.Distanece;
        m_heightOffset = Vector3.up * profile.HeightOffset;
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
    public bool IsSeeTarget(Transform target)
    {
        if (target == null || m_origin == null)
            return false;

        Vector3 dir = (target.position + m_heightOffset) - m_origin.position;

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
                return true;
        }
        return false;
    }
}
