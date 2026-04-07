using UnityEngine;
using Unity.Cinemachine;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] Initializer m_initializer;
    [SerializeField] Registries m_registries;
    [SerializeField] DiscoveryScoreManager m_discoveryScoreManager;
    [SerializeField] UIController m_uiController;
    
    public static GameManager s_Instance;

    private void Awake()
    {if (s_Instance != null && s_Instance != this)
        {
            Destroy(gameObject);
        }

        s_Instance = this;

        //ÉVÅ[ÉìÇÇ‹ÇΩÇ¢Ç≈Ç‡îjâÛÇµÇ»Ç¢.
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        m_discoveryScoreManager.OnUpdateHighScoreEnemy += m_uiController.OnUpdateHighScoreEnemy;
        m_initializer.Initialize(m_registries.Enemies,m_registries.Lights);
    }
}
