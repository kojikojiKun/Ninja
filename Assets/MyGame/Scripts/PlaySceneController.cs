using UnityEngine;

public class PlaySceneController : MonoBehaviour
{
    [SerializeField] Initializer m_initializer;
    [SerializeField] Registries m_registries;
    [SerializeField] Spawner m_spawner;
    [SerializeField] DiscoveryScoreManager m_discoveryScoreManager;
    [SerializeField] UIController m_uiController;

    public static PlaySceneController Instance;

    private void Awake()
    {
        Instance = this;

        m_discoveryScoreManager.OnUpdateHighScoreEnemy += m_uiController.OnUpdateHighScoreEnemy;
    }


    private void Start()
    {
        foreach (var enemy in m_registries.GetEnemies())
        {
            enemy.OnScoreChanged += m_discoveryScoreManager.NotifyEnemyScoreChanged;
        }
        m_initializer.Initialize(m_uiController, m_registries.GetEnemies(), m_registries.GetLights());
    }
}
