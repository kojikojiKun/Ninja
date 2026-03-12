using UnityEngine;
using UnityEngine.AI;

public class EnemyMotor: IEnemyMover
{
    private EnemyStatus m_status;
    private NavMeshAgent m_agent;
    private Transform[] m_wayPoints;
    private float[] m_waitTimes;
    private int m_currentPointIndex = 0;
    private int m_direction;
    private float m_waitTime;
    private float m_timer;

    public EnemyMotor(EnemyStatus status,NavMeshAgent agent,PatrolPointData[] pointData)
    {
        m_status = status;
        m_agent = agent;
        m_wayPoints=new Transform[pointData.Length];
        m_waitTimes=new float[pointData.Length];

        for (int i = 0; i < pointData.Length; i++)
        {
            m_wayPoints[i]=pointData[i].Point;
            m_waitTimes[i] = pointData[i].WaitTime;
        }
    }

    public void GoToDestination(Transform target)
    {
        m_agent.SetDestination(target.transform.position);
    }

    bool HasArrival() { 
        return m_agent.pathPending==false &&
            m_agent.remainingDistance<=m_agent.stoppingDistance;
    }

    /* 最後のポイントまで移動したら最初のポイントまで順番に移動する.
     * 最初のポイントまで移動が終わったら最後まで移動を繰り返す.
    */
    void SetNextPoint()
    {
        if (m_wayPoints.Length == 0)
            return;

        m_timer = 0;

        int index = m_currentPointIndex;
        if (index == m_wayPoints.Length - 1)
        {
            m_direction = -1;
        }
        else if (index == 0)
        {
            m_direction = 1;
        }
        GoToDestination(m_wayPoints[m_currentPointIndex]);
        m_currentPointIndex += m_direction;


        m_waitTime = m_waitTimes[m_currentPointIndex];
    }

    public void Patrol()
    {
        if (!HasArrival())
            return;

        m_timer += Time.deltaTime;

        if (m_timer >= m_waitTime)
        {
            SetNextPoint();
        }
    }

    public void Search() { }
    public void Chase() { }
}
