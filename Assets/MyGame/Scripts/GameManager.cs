using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] Initializer m_initializer;
    [SerializeField] Registries m_registries;
    [SerializeField] Spawner m_spawner;
    [SerializeField] DiscoveryScoreManager m_discoveryScoreManager;
    [SerializeField] UIController m_uiController;

    public static GameManager s_Instance;

    private void Awake()
    {
        if (s_Instance != null && s_Instance != this)
        {
            Destroy(gameObject);
        }

        s_Instance = this;

        //ÉVÅ[ÉìÇÇ‹ÇΩÇ¢Ç≈Ç‡îjâÛÇµÇ»Ç¢.
        DontDestroyOnLoad(gameObject);

        m_discoveryScoreManager.OnUpdateHighScoreEnemy += m_uiController.OnUpdateHighScoreEnemy;
        m_spawner.OnSpawnedLight += m_registries.LightRegister;
        m_spawner.OnSpawnedEnemy += m_registries.EnemyRegister;
    }


    private void Start()
    {
        m_initializer.Initialize(m_uiController,m_registries.Enemies, m_registries.Lights);

        foreach(var enemy in m_registries.Enemies)
        {
            enemy.OnScoreChanged += m_discoveryScoreManager.NotifyEnemyScoreChanged;
        }
    }
}
