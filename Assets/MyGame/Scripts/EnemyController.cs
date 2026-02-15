using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    private enum EnemyState
    {
        Normal, //通常状態.
        Caution, //警戒状態,
        Discover //発見状態.
    }
    [SerializeField] private EnemyData m_data;
    [SerializeField] private PatrolPointData[] m_pointData;
    private Transform[] m_patrolPoints;
    private float[] m_waitTimes;
    private float m_patrolSpeed;
    private float m_chaseSpeed;
    private float m_waitTime;
    private float m_waitTimer;
    private int m_currentPointIndex = 0;
    private int m_direction = 1;

    private float m_searchTime;

    private EnemyState m_state;
    private PlayerContoller m_playerCtrl;
    private NavMeshAgent m_agent;
    private EnemyEye m_eye;

    private void OnEnable()
    {
        //イベント購読.
        GameManager.s_Instance.OnPlayerSpawned += OnPlayerSpawned;
    }

    private void OnDisable()
    {
        //イベント解除.
        GameManager.s_Instance.OnPlayerSpawned -= OnPlayerSpawned;
    }

    private void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_eye = GetComponentInChildren<EnemyEye>();
    }

    private void Start()
    {
        m_patrolSpeed = m_data.PatrolSpeed;
        m_searchTime = m_data.SearchTime;
        m_chaseSpeed = m_data.ChaseSpeed;

        m_patrolPoints = new Transform[m_pointData.Length];
        m_waitTimes = new float[m_pointData.Length];
        for (int i = 0; i < m_pointData.Length; i++)
        {
            m_patrolPoints[i] = m_pointData[i].Point;
            m_waitTimes[i] = m_pointData[i].WaitTime;
        }

        m_waitTime = m_waitTimes[m_currentPointIndex];

        m_eye.ReceiveViewStatus(
            m_playerCtrl.gameObject.transform,
            m_data.ViewDistance,
            m_data.ViewAngle
            );
    }

    //プレイヤースポーン時にPlayerControllerを取得.
    private void OnPlayerSpawned(PlayerContoller player)
    {
        m_playerCtrl = player;
    }

    //渡された目的地まで移動.
    private void GoToDestination(Transform target)
    {
        m_agent.SetDestination(target.position);
    }

    //目的地に到着したかどうかを返す.
    private bool IsArrival()
    {
        return m_agent.pathPending == false && m_agent.remainingDistance <= m_agent.stoppingDistance;
    }

    //次の目的地まで移動.
    private void SetNextPoint()
    {
        if (m_patrolPoints.Length == 0)
            return;

        m_waitTimer = 0;

        //現在地が最後の巡回ポイントのとき.
        if (m_currentPointIndex == m_patrolPoints.Length - 1)
        {
            //巡回ポイントをもどる.
            m_direction = -1;
        }
        //現在地が最初の巡回ポイントのとき.
        else if (m_currentPointIndex == 0)
        {
            //巡回ポイントを進む.
            m_direction = 1;
        }

        //目的地を設定し、次の目的地まで移動.
        m_currentPointIndex += m_direction;
        GoToDestination(m_patrolPoints[m_currentPointIndex]);
        
        //巡回ポイントに応じた待機時間を代入.
        m_waitTime = m_waitTimes[m_currentPointIndex];
    }

    //経路を巡回.
    private void Patrol()
    {
        //目的地に到達したかチェック.
        if (IsArrival()==true)
        {
            m_waitTimer += Time.deltaTime;

            //一定時間経過で次の目的地まで移動.
            if (m_waitTimer >= m_waitTime)
            {
                SetNextPoint();
            }
        }
    }

    //プレイヤーまたは音の発生源を探す.
    private void Search()
    {
        //目的地に到達したかチェック.
        if (IsArrival() == true)
        {
            m_waitTimer += Time.deltaTime;

            //一定時間経過で警戒状態を解く.
            if (m_waitTimer >= m_waitTime)
            {
                m_state = EnemyState.Normal;
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Noise"))
        {
            m_state = EnemyState.Caution;
            Debug.Log("noise heared");
            //音の発生源を目的地にする.
            GoToDestination(other.transform);
        }
        else if (other.CompareTag("Player"))
        {
            m_state = EnemyState.Discover;
        }
    }

    private void Update()
    {
        switch (m_state)
        {
            case EnemyState.Normal:
                Patrol();
                break;
            case EnemyState.Caution:
                Search();
                break;
        }
    }
}
