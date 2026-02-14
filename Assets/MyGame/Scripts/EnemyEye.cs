using UnityEngine;
using System.Collections.Generic;

public class EnemyEye : MonoBehaviour
{
    [SerializeField] private float m_checkInterval;
    private float m_timer;
    private Transform m_player;
    private float m_viewDistance;
    private float m_viewAngle;
    private bool m_hasSetStatus = false;

    public void ReceiveViewStatus(Transform player,float distance,float angle)
    {
        m_player = player;
        m_viewDistance = distance;
        m_viewAngle = angle;

        m_hasSetStatus = true;
    }

    void CheckSight()
    {
        Debug.Log("check");
        Vector3 dir = m_player.position - transform.position;

        if (dir.magnitude > m_viewDistance)
            return;

        //Ž‹–ì‚ÌL‚³‚ðŒvŽZ.
        float angle = Vector3.Angle(this.gameObject.transform.forward, dir);
        if (angle > m_viewAngle / 2)
            return;

        if(Physics.Raycast(
            this.gameObject.transform.position,
            dir.normalized,
            out RaycastHit hit,
            m_viewDistance
            ))
        {
            if (hit.transform == m_player)
            {
                //ƒvƒŒƒCƒ„[‚ð”­Œ©.
            }
        }
    }

    private void Update()
    {
        if (m_hasSetStatus == false)
            return;

        m_timer += Time.deltaTime;
        if (m_timer >= m_checkInterval)
        {
            CheckSight();
            m_timer = 0;
        }
    }
}
