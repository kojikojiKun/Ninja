using UnityEngine;
using System;
public class GameManager : MonoBehaviour
{
    public static GameManager s_Instance;
    public event Action<Transform> OnPlayerSpawned;
    public event Action<LightVisibilityEvaluator> OnCachedLights;
    private LightVisibilityEvaluator m_lightVisibilityEvaluator;

    [SerializeField] private GameObject m_playerPrefab;
    [SerializeField] private Transform m_startPos;

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
        GameStart();
    }

    public void CacheLights()
    {
        LightToCheck[] lightToChecks = FindObjectsByType<LightToCheck>(FindObjectsSortMode.None);
        m_lightVisibilityEvaluator = new LightVisibilityEvaluator(lightToChecks);
        OnCachedLights?.Invoke(m_lightVisibilityEvaluator);
    }

    //ゲーム開始時に実行.
    public void GameStart()
    {
        //プレイヤーのスポーンを通知.
        OnPlayerSpawned?.Invoke(m_playerPrefab.transform);
        CacheLights();
    }
}
