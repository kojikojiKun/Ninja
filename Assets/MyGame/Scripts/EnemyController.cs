using System.Collections.Generic;
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
    [SerializeField] private float m_lightPow;
    [SerializeField] private float m_distancePow;
    [SerializeField] private float m_decSpeed;
    [SerializeField] private PatrolPointData[] m_pointData;
    [SerializeField] private ViewTargetProfile m_viewTargetProfile;
    [SerializeField, Range(0, 99.9999f)] private float m_percentageOfChangeCautionState;

    private EnemyStatus m_status;
    private EnemyCore m_core;
    private NavMeshAgent m_agent;
    private EnemyMotor m_motor;
    private ViewTarget m_viewTarget;
    private LightVisibilityEvaluator m_lightEvaluator;
    private HashSet<LightZone> m_lightZones = new HashSet<LightZone>();
    private Timer m_timer;

    private AlertState m_alertState;
    private Transform m_player;
    private float m_brightness;
    private const float MAX_VIEW_SCORE = 40f;
    private const float CHECK_VIEW_INTERVAL = 0.2f;

    public float TotalScore { get; private set; }


    private void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();

        m_status = new EnemyStatus(m_data);
        m_core = new EnemyCore(m_status, this.transform);
        m_motor = new EnemyMotor(m_status, m_agent, m_pointData);
        m_viewTarget = new ViewTarget(m_viewTargetProfile);
        m_timer = new Timer();

        m_viewTarget.OverrideValues(m_status.ViewAngle, m_status.ViewDistance);
        //パーセントを少数になおす。
        if (m_percentageOfChangeCautionState > 0)
            m_percentageOfChangeCautionState /= 100;

        m_timer.Reset();
    }

    private void OnEnable()
    {
        Registries.Instance.EnemyRegister(this);
    }

    private void OnDisable()
    {
        Registries.Instance.EnemyUnRegister(this);
    }

    public void Initialize(HashSet<LightToCheck> checks, PlayerContoller player)
    {
        m_lightEvaluator = new LightVisibilityEvaluator(checks, m_lightPow, m_distancePow);
        m_player = player.transform;
    }

    /// =======================================================================
    /// ColliderTrigger
    /// =======================================================================
    private void OnTriggerEnter(Collider other)
    {
        //プレイヤーの騒音オブジェクトに接触.
        if (m_alertState != AlertState.Caution && other.CompareTag("Noise"))
            HandleNoise(other.gameObject.transform.position);

        if (other.CompareTag("Player"))
            HandlePlayer();

        if (other.TryGetComponent(out LightZone zone))
            HandleLightEnter(zone);
    }

    void HandleNoise(Vector3 position)
    {
        m_alertState = AlertState.Caution;
        m_motor.GetTarget(position);
    }

    void HandlePlayer()
    {
        m_alertState = AlertState.Discover;
    }

    void HandleLightEnter(LightZone zone)
    {
        m_lightZones.Add(zone);
        m_brightness = m_lightEvaluator.CalkBrightness(m_lightZones);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out LightZone zone))
        {
            m_lightZones.Remove(zone);
            m_brightness = m_lightEvaluator.CalkBrightness(m_lightZones);
        }
    }
    /// =======================================================================
    /// ColliderTrigger Finish <summary>
    /// =======================================================================

    void CheckViewingScore()
    {
        if (!m_timer.IsOutOfDuration(CHECK_VIEW_INTERVAL))
            return;

        bool isSee = m_viewTarget.IsSeeTarget();

        float viewScore = m_lightEvaluator.EaseOfViewingScore(
                this.transform.position,
                m_player.position,
                m_brightness,
                m_status.ViewDistance
                );

        float delta = CalcScore(
            isSee,
            viewScore,
            Time.deltaTime
            );

        TotalScore += delta;
        TotalScore = Mathf.Clamp(TotalScore, 0f, MAX_VIEW_SCORE);
    }

    public float CalcScore(bool isSee, float viewScore, float deltaTime)
    {
        if (isSee)
        {
            return viewScore * deltaTime;
        }
        else
        {
            return -m_decSpeed * deltaTime;
        }
    }

    void ChangeAlertStateByScore()
    {
        //発見スコアの最大値に対する割合で警戒状態を変更.
        float scorePercentage = (TotalScore / MAX_VIEW_SCORE);

        //プレイヤーを捜索中でないときかつ警戒状態に移行する割合以下.
        if (m_motor.MovingState != EnemyMoveState.Search && scorePercentage < m_percentageOfChangeCautionState)
        {
            m_alertState = AlertState.Normal;
        }
        //警戒状態に移行する割合以上100％以下.
        else if (scorePercentage >= m_percentageOfChangeCautionState && scorePercentage < 1f)
        {
            m_motor.GetTarget(m_player.position);
            m_alertState = AlertState.Caution;
        }
        //100%.
        else
        {
            m_alertState = AlertState.Discover;
        }
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
        if (m_core.IsDead() == true)
            return;
        CheckViewingScore();
        ChangeAlertStateByScore();
        ChangeBehaviour();
    }
}
