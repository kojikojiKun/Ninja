using UnityEngine;

public enum NoiseState
{
    None,
    Small,
    Middle,
    Big
}

public class SneakSystem : MonoBehaviour
{
    private PlayerContoller m_playerCtrl;

    [Tooltip("コライダーの大きさが小さい順に並べる"),SerializeField] GameObject[] m_noiseRange = new GameObject[3];
    private NoiseState m_currentNoiseState;

    private void Awake()
    {
        m_playerCtrl = GetComponent<PlayerContoller>();
        GenerateNoise(PlayerMoveState.Stop);
    }

    //プレイヤーの移動状況を受け取り、状況に応じた騒音の範囲を設定する.
    public void GenerateNoise(PlayerMoveState playerState)
    {
        for (int i = 0; i < m_noiseRange.Length; i++)
        {
            m_noiseRange[i].SetActive(false);
        }

        //騒音の発生範囲を変更する.
        switch (playerState)
        {
            //止まっているとき.
            case PlayerMoveState.Stop:
                m_currentNoiseState = NoiseState.None;
                break;

            //しゃがんで歩いているとき.
            case PlayerMoveState.CrouchWalk:
                m_currentNoiseState = NoiseState.Small;
                m_noiseRange[0].SetActive(true);
                break;

            //歩いているとき.
            case PlayerMoveState.Walk:
                m_currentNoiseState = NoiseState.Middle;
                m_noiseRange[1].SetActive(true);
                break;

            //走っているとき.
            case PlayerMoveState.Run:
                m_currentNoiseState = NoiseState.Big;
                m_noiseRange[2].SetActive(true);
                break;
        }
    }
}
