using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class PatrolPointData
{
    public Transform Point;
    public float WaitTime;
}

[RequireComponent(typeof(NavMeshAgent))]

public class EnemyController : MonoBehaviour, IAssassinateable, IDamageable
{
    [SerializeField] private DiscoveryScoreCoefficients m_coefficients;
    [SerializeField] private PatrolPointData[] m_pointData;
    [SerializeField] private EnemyData m_data;
    [SerializeField] private ViewProfile m_viewProfile;
    [SerializeField] private Transform m_viewOrigin;
    [SerializeField, Range(0, 99.9999f)] private float m_percentageOfChangeCautionState;
    [SerializeField] private AssassinateData[] m_assassinateData;

    private EnemyStatus m_status;
    private EnemyCore m_core;
    private NavMeshAgent m_agent;
    private EnemyMotor m_motor;
    private EnemyDiscoveryScore m_enemyScore;
    private ViewTarget m_viewTarget;
    private LightZoneTrigger m_lightTrigger;
    private HearNoiseTrigger m_noiseTrigger;
    private Animator m_animator;
    private EnemyAnimator m_enemyAnimator;
    private Timer m_timer;

    private AlertState m_alertState;
    private PlayerController m_player;
    private const float CHECK_VIEW_INTERVAL = 0.2f;
    public Transform OriginTransform { get; private set; }

    public Dictionary<AssassinateDirection, AssassinateData>
        DataMap
    { get; private set; }
        = new Dictionary<AssassinateDirection, AssassinateData>();

    public event Action<EnemyController> OnScoreChanged;


    private void Awake()
    {
        OriginTransform = this.transform;
        foreach (var data in m_assassinateData)
        {
            DataMap.Add(data.Direction, data);
        }

        m_agent = GetComponent<NavMeshAgent>();
        m_lightTrigger = GetComponentInChildren<LightZoneTrigger>();
        m_noiseTrigger = GetComponentInChildren<HearNoiseTrigger>();
        m_animator = GetComponent<Animator>();

        m_status = new EnemyStatus(m_data);
        m_core = new EnemyCore(m_status);
        m_motor = new EnemyMotor(m_status, m_agent, m_pointData);
        m_enemyScore = new EnemyDiscoveryScore(m_viewProfile, m_coefficients, OriginTransform);
        m_viewTarget = new ViewTarget(m_viewProfile, m_viewOrigin);
        m_enemyAnimator = new EnemyAnimator(m_animator);
        m_timer = new Timer();

        //パーセントを少数になおす。
        if (m_percentageOfChangeCautionState > 0)
            m_percentageOfChangeCautionState /= 100;

        m_timer.Reset();
    }

    private void OnEnable()
    {
        m_lightTrigger.OnLightEnter += OnLightEnter;
        m_lightTrigger.OnLightExit += OnLigtExit;
        m_noiseTrigger.OnHearNoise += OnHearNoise;
    }

    private void OnDisable()
    {
        m_lightTrigger.OnLightEnter -= OnLightEnter;
        m_lightTrigger.OnLightExit -= OnLigtExit;
        m_noiseTrigger.OnHearNoise -= OnHearNoise;
    }

    public void Initialize(List<LightToCheck> checks, PlayerController player)
    {
        m_player = player;
        m_enemyScore.RegistLights(checks);
    }

    private void Update()
    {
        if (m_core.IsDead() == true)
        {
            Debug.Log("dead");
            return;
        }

        CheckViewScore();
        ChangeAlertStateByScore();
        ChangeBehaviour();
    }

    public void OnHearNoise()
    {
        m_alertState = AlertState.Caution;
    }

    public void OnLightEnter(LightZone zone)
    {
        m_enemyScore.RegistLightZone(zone);
    }

    public void OnLigtExit(LightZone zone)
    {
        m_enemyScore.UnRegistLightZone(zone);
    }

    void ChangeAlertStateByScore()
    {

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

    void CheckViewScore()
    {
        if (!m_timer.IsOutOfDuration(CHECK_VIEW_INTERVAL))
        {
            Transform target = m_player.gameObject.transform;
            bool isSeeTarget = m_viewTarget.IsSeeTarget(target);

            m_enemyScore.CheckScore(target, isSeeTarget);
        }

        OnScoreChanged?.Invoke(this);
    }

    public float GetTotalScore()
    {
        return m_enemyScore.GetTotalScore();
    }

    public void TakeDamage(int value)
    {
        // m_core.TakeDamage(value);
    }

    public void BeAssassinate(AssassinateDirection dir)
    {
        m_enemyAnimator.Assassinate(dir);
    }
}
