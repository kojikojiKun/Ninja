using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    [SerializeField] PlayerController m_player;
    [SerializeField] Camera m_camera;

    public void Initialize(UIController uI,List<EnemyController> enemies, List<LightToCheck> checks)
    {
        uI.Initialize(m_camera);
        m_player.Initialize(m_camera);
        foreach (var enemy in enemies)
        {
            enemy.Initialize(checks, m_player);
        }
    }
}
