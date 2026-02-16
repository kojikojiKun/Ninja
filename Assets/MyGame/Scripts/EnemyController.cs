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
    private Transform m_player;
    private EnemyStatus m_status;
    private EnemyCore m_core;
    private NavMeshAgent m_agent;
    private EnemyMotor m_motor;
    private EnemyEye m_enemyEye;
    private AlertState m_alertState;

    [SerializeField] private float m_defDarkValue;
    private LightVisibilityEvaluator m_lightEvaluator;
    private float m_brightness;
    private float m_finalBrightness;
    private float m_checkBrightnessInterval;
    private float m_timer;

    private void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_status = new EnemyStatus(m_data);
        m_core = new EnemyCore(m_status);
        m_motor = new EnemyMotor(m_status, m_agent, m_pointData);
        m_enemyEye = new EnemyEye(m_status, m_eyeObj.transform);
    }

    private void OnEnable()
    {
        GameManager.s_Instance.OnPlayerSpawned += OnPlayerSpawned;
        GameManager.s_Instance.OnCachedLights += OnCachedLights;
    }

    private void OnDisable()
    {
        GameManager.s_Instance.OnPlayerSpawned -= OnPlayerSpawned;
        GameManager.s_Instance.OnCachedLights -= OnCachedLights;
    }

    void OnPlayerSpawned(Transform player)
    {
        m_enemyEye.GetPlayer(player);
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
            GetCurrentBrightness(zone.Brightness);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out LightZone zone))
        {
            GetCurrentBrightness(m_defDarkValue);
        }
    }

    public void GetCurrentBrightness(float brightness)
    {
        m_brightness = brightness;
    }

    void CheckFinalBrightness()
    {
        m_timer += Time.deltaTime;
        if (m_timer <= m_checkBrightnessInterval)
            return;

        float score = m_lightEvaluator.GetLightScore(m_player.position);
        m_finalBrightness = m_brightness * score;
        Debug.Log(m_finalBrightness);
        m_timer = 0;
    }

    private void Update()
    {
        if (m_core.IsDead() == true)
            return;

        CheckFinalBrightness();
        m_enemyEye.CheckSight();

        if (m_enemyEye.IsSeePlayer == true)
            Debug.Log("see player");

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
