using UnityEngine;
using Unity.Cinemachine;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform m_startPos;
    [SerializeField] private GameObject m_player;
    [SerializeField] private Camera m_mainCam;
    [SerializeField] private float m_lightScorePow;
    [SerializeField] private float m_distanceScorePow;

    public Camera MainCamera => m_mainCam;
    public EnemyController HighestScoreEnemy { get; private set; }
    public GameObject Player => m_player;
    public LightVisibilityEvaluator LightVisibilityEvaluator { get; private set; }
    public static GameManager s_Instance;
    public event Action<Transform> OnPlayerSpawned;
    public event Action<LightVisibilityEvaluator> OnCachedLights;

    private void Awake()
    {
        if (s_Instance != null && s_Instance != this)
        {
            Destroy(gameObject);
        }

        s_Instance = this;

        //シーンをまたいでも破壊しない.
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        NotifyPlayerSpawn();
        NotifyCacheLights();
    }

    //シーン上のLightToCheckオブジェクトをキャッシュ.
    public void NotifyCacheLights()
    {
        LightToCheck[] lightToChecks = FindObjectsByType<LightToCheck>(FindObjectsSortMode.None);
        LightVisibilityEvaluator = new LightVisibilityEvaluator(lightToChecks, m_lightScorePow, m_distanceScorePow);
        OnCachedLights?.Invoke(LightVisibilityEvaluator);
    }

    public void NotifyPlayerSpawn()
    {
        //プレイヤーのスポーンを通知.
        OnPlayerSpawned?.Invoke(Player.transform);
    }

    public void NotifyEnemyScoreChanged(EnemyController enemy)
    {
        //プレイヤー発見スコアが最も高い敵が更新されたときのみ実行.
        if (HighestScoreEnemy == null || enemy.TotalScore > HighestScoreEnemy.TotalScore)
        {
            HighestScoreEnemy = enemy;
        }
    }
}
