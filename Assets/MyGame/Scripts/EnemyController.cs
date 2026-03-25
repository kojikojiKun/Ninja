using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class PatrolPointData
{
    public Transform Point;
    public float WaitTime;
}

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour, IAmbientLightReader
{
    [SerializeField] private EnemyData m_data;
    [SerializeField] private PatrolPointData[] m_pointData;
    [SerializeField] private GameObject m_eyeObj;
    [SerializeField] private LayerMask m_sightMask;
    private Transform m_player;
    private EnemyStatus m_status;
    private EnemyCore m_core;
    private NavMeshAgent m_agent;
    private EnemyMotor m_motor;
    private EnemyEye m_enemyEye;
    private DiscoveryScore m_disScore;
    private AlertState m_alertState;

    [SerializeField] private float m_defDarkValue;
    private LightVisibilityEvaluator m_lightEvaluator;
    private HashSet<LightZone> m_lightZones = new HashSet<LightZone>();
    private float m_brightness;
    private float m_checkInterval;
    private float m_timer;
    private const float MAX_VIEW_SCORE = 40f;
    public float TotalScore { get;private set; }


    private void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_status = new EnemyStatus(m_data);
        m_core = new EnemyCore(m_status);
        m_motor = new EnemyMotor(m_status, m_agent, m_pointData);
        m_enemyEye = new EnemyEye(m_status, m_eyeObj.transform, m_sightMask);
        m_disScore = new DiscoveryScore(MAX_VIEW_SCORE);
    }

    private void OnEnable()
    {
        if (GameManager.s_Instance == null)
            return;

        GameManager.s_Instance.OnPlayerSpawned += OnPlayerSpawned;
        GameManager.s_Instance.OnCachedLights += OnCachedLights;
    }

    private void OnDisable()
    {
        if (GameManager.s_Instance == null)
            return;

        GameManager.s_Instance.OnPlayerSpawned -= OnPlayerSpawned;
        GameManager.s_Instance.OnCachedLights -= OnCachedLights;
    }

    private void Start()
    {
        if (GameManager.s_Instance?.Player != null && m_player == null)
            OnPlayerSpawned(GameManager.s_Instance.Player.transform);

        if (GameManager.s_Instance?.LightVisibilityEvaluator != null && m_lightEvaluator == null)
            OnCachedLights(GameManager.s_Instance.LightVisibilityEvaluator);
    }

    /// =======================================================================
    /// Read Event
    /// =======================================================================
    void OnPlayerSpawned(Transform player)
    {
        m_enemyEye?.GetPlayer(player);
        m_player = player;
    }

    void OnCachedLights(LightVisibilityEvaluator lightEvaluator)
    {
        m_lightEvaluator = lightEvaluator;
    }
    /// =======================================================================
    /// finish


    /// =======================================================================
    /// ColliderTriggerEnter
    /// =======================================================================
    private void OnTriggerEnter(Collider other)
    {
        //プレイヤーの騒音オブジェクトに接触.
        if (m_alertState != AlertState.Caution && other.CompareTag("Noise"))
        {
            //警戒状態に移行.
            m_alertState = AlertState.Caution;
            
            //音の発生源を目的地に設定する.
            m_motor.GoToDestination(other.gameObject.transform.position);
        }
       
        if (other.CompareTag("Player"))
        {
            //発見状態に移行.
            m_alertState = AlertState.Discover;
        }

        if (other.TryGetComponent(out LightZone zone))
        {
            m_lightZones.Add(zone);
            CalkBrightness();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out LightZone zone))
        {
            m_lightZones.Remove(zone);
            CalkBrightness();
        }
    }
    /// =======================================================================
    /// finish

    //現在地の明るさを取得.
    void GetCurrentBrightness(float brightness)
    {
        m_brightness = brightness;
    }


    public void CalkBrightness()
    {
        //一つも接触していないとき.
        if (m_lightZones.Count == 0)
        {
            //デフォルトの値を渡す.
            GetCurrentBrightness(m_defDarkValue);
            return;
        }
        float max = 0f;

        //複数のLightZone(Collider)に接触した場合最もBrightnessの値が大きいLightZoneのBrightnessを参照する.
        foreach (var zone in m_lightZones)
        {
            max = Mathf.Max(max, zone.Brightness);
        }

        GetCurrentBrightness(max);
    }

    //必要なコンポーネントがnullならセット.
    void NullCompletion()
    {
        if (m_lightEvaluator == null)
            m_lightEvaluator = GameManager.s_Instance.LightVisibilityEvaluator;
        if (m_player == null)
        {
            m_player = GameManager.s_Instance.Player.transform;
            m_enemyEye?.GetPlayer(m_player);
        }
    }

    void CheckViewingScore()
    {
        if (m_enemyEye.IsSeePlayer())
        {
            //プレイヤーが見えていたら発見スコア上昇.
            TotalScore = m_disScore.IncreaceScore(TotalScore, m_lightEvaluator.EaseOfViewingScore(
                this.transform.position,
                m_player.position,
                m_brightness,
                m_status.ViewDistance
                ));
        }
        else
        {
            //プレイヤー未発見なら発見スコア減少.
            TotalScore = m_disScore.DecreaceScore(TotalScore);
        }
        GameManager.s_Instance.NotifyEnemyScoreChanged(this);
    }

    void ChangeBehaviour()
    {
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

    private void Update()
    {
        NullCompletion();

        if (m_core.IsDead() == true)
            return;

        m_timer += Time.deltaTime;
        if (m_timer >= m_checkInterval)
        {
            CheckViewingScore();
            m_timer = 0;
        }

        //発見スコアの最大値に対する割合で警戒状態を変更.
        float scorePercentage = (TotalScore / MAX_VIEW_SCORE);
        //50％以下かつプレイヤーを捜索中でないとき.
        if (scorePercentage < 0.5f && !m_motor.IsSearching)
        {
            m_alertState = AlertState.Normal;
        }
        //50％以上100％以下.
        else if (scorePercentage >= 0.5f && scorePercentage < 1f)
        {
            m_motor.GetTarget(m_player.position);
            m_alertState = AlertState.Caution;
        }
        //100%.
        else
        {
            m_alertState = AlertState.Discover;
        }
        Debug.Log($"totalScore={TotalScore},{scorePercentage * 100}%");
        ChangeBehaviour();
    }

    void OnDrawGizmos()
    {
        if (m_eyeObj == null || m_status == null)
            return;

        Vector3 origin = m_eyeObj.transform.position;
        float dist = m_status.ViewDistance;
        float angle = m_status.ViewAngle;

        int segments = 20;

        Gizmos.color = Color.yellow;

        Vector3 prevPoint = origin;

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float currentAngle = -angle * 0.5f + angle * t;

            Vector3 dir = Quaternion.Euler(0, currentAngle, 0) * m_eyeObj.transform.forward;
            Vector3 point = origin + dir * dist;

            Gizmos.DrawLine(origin, point);

            if (i > 0)
                Gizmos.DrawLine(prevPoint, point);

            prevPoint = point;
        }
    }
}
