using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform m_startPos;
    [SerializeField] private GameObject m_player;

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

        //ÉVÅ[ÉìÇÇ‹ÇΩÇ¢Ç≈Ç‡îjâÛÇµÇ»Ç¢.
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        NotifyPlayerSpawn();
        NotifyCacheLights();
    }

    public void NotifyCacheLights()
    {
        LightToCheck[] lightToChecks = FindObjectsByType<LightToCheck>(FindObjectsSortMode.None);
        LightVisibilityEvaluator = new LightVisibilityEvaluator(lightToChecks);
        OnCachedLights?.Invoke(LightVisibilityEvaluator);
    }

    public void NotifyPlayerSpawn()
    {
        OnPlayerSpawned?.Invoke(Player.transform);
    }

    public void NotifyEnemyScoreChanged(EnemyController enemy)
    {
        if (HighestScoreEnemy == null || enemy.TotalScore > HighestScoreEnemy.TotalScore)
        {
            HighestScoreEnemy = enemy;
        }
    }
}
