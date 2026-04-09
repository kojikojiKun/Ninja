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

public class EnemyController : MonoBehaviour,IAssassinateable,IDamageable
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
    private List<LightZone> m_lightZones = new List<LightZone>();
    private Timer m_timer;

    private AlertState m_alertState;
    private PlayerController m_player;
    private float m_brightness;
    private const float MAX_VIEW_SCORE = 40f;
    private const float CHECK_VIEW_INTERVAL = 0.2f;
    private float m_totalScore;

    public Transform OriginTransform { get; private set; }
    public event Action<EnemyController> OnScoreChanged;


    private void Awake()
    {
        OriginTransform = this.transform;

        m_agent = GetComponent<NavMeshAgent>();

        m_status = new EnemyStatus(m_data);
        m_core = new EnemyCore(m_status);
        m_motor = new EnemyMotor(m_status, m_agent, m_pointData);
        m_viewTarget = new ViewTarget(m_viewTargetProfile);
        m_lightEvaluator = new LightVisibilityEvaluator();
        m_timer = new Timer();

        m_viewTarget.OverrideValues(m_status.ViewAngle, m_status.ViewDistance);
        //パーセントを少数になおす。
        if (m_percentageOfChangeCautionState > 0)
            m_percentageOfChangeCautionState /= 100;

        m_timer.Reset();
    }

    public void Initialize(List<LightToCheck> checks, PlayerController player)
    {
        m_player = player;
        m_lightEvaluator.GetLights(checks, m_lightPow, m_distancePow);
        m_viewTarget.SetTarget(m_player.transform);
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
        if (m_player == null)
            return;


        if (!m_timer.IsOutOfDuration(CHECK_VIEW_INTERVAL))
            return;

        bool isSee = m_viewTarget.IsSeeTarget();

        float viewScore = m_lightEvaluator.EaseOfViewingScore(
                this.transform.position,
                m_player.gameObject.transform.position,
                m_brightness,
                m_status.ViewDistance
                );

        float delta = CalcScore(
            isSee,
            viewScore,
            Time.deltaTime
            );
        Debug.Log($"{isSee}..{viewScore}..{delta}");
        m_totalScore += delta;
        m_totalScore = Mathf.Clamp(m_totalScore, 0f, MAX_VIEW_SCORE);

        OnScoreChanged?.Invoke(this);
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
        float scorePercentage = (m_totalScore / MAX_VIEW_SCORE);

        //プレイヤーを捜索中でないときかつ警戒状態に移行する割合以下.
        if (m_motor.GetMoveState() != EnemyMoveState.Search && scorePercentage < m_percentageOfChangeCautionState)
        {
            m_alertState = AlertState.Normal;
        }
        //警戒状態に移行する割合以上100％以下.
        else if (scorePercentage >= m_percentageOfChangeCautionState && scorePercentage < 1f)
        {
            m_motor.GetTarget(m_player.gameObject.transform.position);
            m_alertState = AlertState.Caution;
        }
        //100%.
        else
        {
            m_alertState = AlertState.Discover;
        }
    }

    public float GetTotalScore()
    {
        return m_totalScore;
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

    public void BeAssasinate()
    {

    }

    public void TakeDamage(int value)
    {
        m_core.TakeDamage(value);
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
