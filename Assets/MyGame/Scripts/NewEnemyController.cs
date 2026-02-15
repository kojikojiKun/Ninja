using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class PatrolPointData
{
    public Transform Point;
    public float WaitTime;
}

[RequireComponent(typeof(NavMeshAgent))]
public class NewEnemyController : MonoBehaviour
{
    [SerializeField] private EnemyData m_data;
    [SerializeField] private PatrolPointData[] m_pointData; 
    private EnemyStatus m_status;
    private EnemyCore m_core;
    private NavMeshAgent m_agent;
    private EnemyMotor m_motor;
    private AlertState m_alertState;

    private void Awake()
    {
        m_status = new EnemyStatus(m_data);
        m_core = new EnemyCore(m_status);
        m_motor = new EnemyMotor(m_status, m_agent.GetComponent<NavMeshAgent>(), m_pointData);
    }

    private void Update()
    {
        if(m_core.IsDead()==true)
            return;

        switch (m_alertState)
        {
            case AlertState.Normal:
                m_motor.Patrol();
                break;
            case AlertState.Caution:
                m_motor.Search();
                break;
            case AlertState.Discover:
                m_motor.Chase();
                break;
        }
    }
}
