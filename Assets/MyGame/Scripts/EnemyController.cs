using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class PatrolPointData
{
    public Transform Point;
    public float WaitTime;
}

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyData m_data;
    [SerializeField] private PatrolPointData[] m_pointData;
    [SerializeField] private GameObject m_eyeObj;
    private EnemyStatus m_status;
    private EnemyCore m_core;
    private NavMeshAgent m_agent;
    private EnemyMotor m_motor;
    private EnemyEye m_enemyEye;
    private AlertState m_alertState;

    private void OnEnable()
    {
        GameManager.s_Instance.OnPlayerSpawned += OnPlayerSpawned;
    }

    private void OnDisable()
    {
        GameManager.s_Instance.OnPlayerSpawned-=OnPlayerSpawned;
    }

    void OnPlayerSpawned(Transform player)
    {
        m_enemyEye.GetPlayer(player);
    }

    private void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_status = new EnemyStatus(m_data);
        m_core = new EnemyCore(m_status);
        m_motor = new EnemyMotor(m_status, m_agent, m_pointData);
        m_enemyEye = new EnemyEye(m_status, m_eyeObj.transform);
    }

    private void Update()
    {
        if(m_core.IsDead()==true)
            return;

        m_enemyEye.CheckSight();
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
