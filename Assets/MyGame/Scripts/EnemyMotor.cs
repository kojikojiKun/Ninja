using UnityEngine;
using UnityEngine.AI;

public class EnemyMotor
{
    private EnemyStatus m_status;
    private NavMeshAgent m_agent;
    private Transform[] m_wayPoints;
    private Vector3 m_lastTargetPos;
    private float[] m_waitTimes;
    private int m_currentPointIndex = 0;
    private int m_direction;
    private float m_waitTime;
    private Timer m_patrolTimer;
    private Timer m_searchTimer;
    private EnemyMoveState m_movingState;

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
        m_patrolTimer = new Timer();
        m_searchTimer = new Timer();

        m_patrolTimer.Reset();
        m_searchTimer.Reset();
    }

    public void GetTarget(Vector3 targetPos)
    {
        if (m_lastTargetPos == Vector3.zero)
        {
            m_lastTargetPos = targetPos;
            GoToDestination(m_lastTargetPos);
        }
    }

    public void GoToDestination(Vector3 target)
    {
        m_agent.SetDestination(target);
    }

    bool HasArrival()
    {
        return m_agent.pathPending == false &&
            m_agent.remainingDistance <= m_agent.stoppingDistance;
    }

    /* 最後のポイントまで移動したら最初のポイントまで順番に移動する.
     * 最初のポイントまで移動が終わったら最後まで移動を繰り返す.
    */
    void SetNextPoint()
    {
        if (m_wayPoints.Length == 0)
            return;

        m_patrolTimer.Reset();

        int index = m_currentPointIndex;
        if (index == m_wayPoints.Length - 1)
        {
            m_direction = -1;
        }
        else if (index == 0)
        {
            m_direction = 1;
        }
        GoToDestination(m_wayPoints[m_currentPointIndex].position);
        m_currentPointIndex += m_direction;


        m_waitTime = m_waitTimes[m_currentPointIndex];
    }

    public EnemyMoveState GetMoveState()
    {
        return m_movingState;
    }

    void ChangeAlertStateByScore(float scorePercentage)
    {
       /* float limit = m_status.CautionScorePercentage;
        //プレイヤーを捜索中でないときかつ警戒状態に移行する割合以下.
        if (m_movingState != EnemyMoveState.Search && scorePercentage < limit)
        {
            m_alertState = AlertState.Normal;
        }
        //警戒状態に移行する割合以上100％以下.
        else if (scorePercentage >= limit && scorePercentage < 1f)
        {
            m_motor.GetTarget(m_player.gameObject.transform.position);
            m_alertState = AlertState.Caution;
        }
        //100%.
        else
        {
            m_alertState = AlertState.Discover;
        }
       */
    }

    public void Patrol()
    {
        if (!HasArrival())
            return;

        m_movingState = EnemyMoveState.Patrol;

        if (m_patrolTimer.IsOutOfDuration(m_waitTime))
            SetNextPoint();
    }

    public void Search()
    {
        if (!HasArrival())
            return;

        if (m_searchTimer.IsOutOfDuration(m_status.SearchTime))
        {
            m_lastTargetPos = Vector3.zero;
            m_movingState = EnemyMoveState.Patrol;
            m_searchTimer.Reset();
        }
    }

    public void Chase()
    {
        m_movingState = EnemyMoveState.Chase;
    }
}
