using UnityEngine;

[System.Serializable]
public class NoiseRangeProfile
{
    public Collider NoiseCollider;
    public PlayerMoveState OcuurCondirion ;
}
public class SoundRangeController : MonoBehaviour
{
    [SerializeField] NoiseRangeProfile[] m_profiles= new NoiseRangeProfile[4];

    private void Awake()
    {
        for (int i = 0; i < m_profiles.Length; i++)
            m_profiles[i].NoiseCollider.enabled = false;
    }

    public void ApplyNoiseRange(PlayerMoveState currentMoveState)
    {
        foreach(var n in m_profiles)
        {
            bool active = n.OcuurCondirion == currentMoveState;
            n.NoiseCollider.enabled = active;
        }
    }
}
