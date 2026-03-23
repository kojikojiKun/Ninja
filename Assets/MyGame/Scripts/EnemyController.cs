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
    [SerializeField] private float m_maxDisScore;
    private LightVisibilityEvaluator m_lightEvaluator;
    private HashSet<LightZone> m_lightZones = new HashSet<LightZone>();
    private float m_brightness;
    private float m_checkInterval;
    private float m_timer;
    public float TotalScore { get;private set; }


    private void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_status = new EnemyStatus(m_data);
        m_core = new EnemyCore(m_status);
        m_motor = new EnemyMotor(m_status, m_agent, m_pointData);
        m_enemyEye = new EnemyEye(m_status, m_eyeObj.transform, m_sightMask);
        m_disScore = new DiscoveryScore();
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

    void OnPlayerSpawned(Transform player)
    {
        m_enemyEye?.GetPlayer(player);
        m_player = player;
    }

    void OnCachedLights(LightVisibilityEvaluator lightEvaluator)
    {
        m_lightEvaluator = lightEvaluator;
    }

    private void OnTriggerEnter(Collider other)
    {
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

    //現在地の明るさを取得.
    void GetCurrentBrightness(float brightness)
    {
        m_brightness = brightness;
    }

    //複数のLightZone(Collider)に接触した場合最もBrightnessの値が大きいLightZoneのBrightnessを参照する.
    public void CalkBrightness()
    {
        if (m_lightZones.Count == 0)
        {
            GetCurrentBrightness(m_defDarkValue);
            return;
        }
        float max = 0f;

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
            m_player = GameManager.s_Instance.Player.transform;
    }

    private void Update()
    {
        NullCompletion();

        if (m_core.IsDead() == true)
            return;

        m_timer += Time.deltaTime;
        if (m_timer >= m_checkInterval)
        {
            if (m_enemyEye.IsSeePlayer())
            {
                TotalScore = m_disScore.IncreaceScore(TotalScore, m_lightEvaluator.EaseOfViewingScore(
                    this.transform.position,
                    m_player.position,
                    m_brightness
                    ));
            }
            else
            {
                TotalScore = m_disScore.DecreaceScore(TotalScore);
            }
            GameManager.s_Instance.NotifyEnemyScoreChanged(this);
            m_timer = 0;
        }

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
