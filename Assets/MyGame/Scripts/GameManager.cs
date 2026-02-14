using UnityEngine;
using System;
public class GameManager : MonoBehaviour
{
    public static GameManager s_Instance;
    public event Action<PlayerContoller> OnPlayerSpawned;

    [SerializeField] GameObject m_playerPrefab;
    [SerializeField] Transform m_startPos;
    public PlayerContoller PlayerCtrl { get; private set; }

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
        // テスト用.
        GameStart();
    }

    //ゲーム開始時に実行.
    public void GameStart()
    {
        GameObject player = Instantiate(m_playerPrefab, m_startPos.position, Quaternion.identity);
        PlayerCtrl = player.GetComponent<PlayerContoller>();

        //プレイヤーのスポーンを通知.
        OnPlayerSpawned?.Invoke(PlayerCtrl);
    }
}
